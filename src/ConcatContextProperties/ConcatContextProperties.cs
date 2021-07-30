using System;
using BizTalkComponents.Utils;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Message.Interop;
using IComponent = Microsoft.BizTalk.Component.Interop.IComponent;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using System.Text;

namespace BizTalkComponents.PipelineComponents.ConcatContextProperties
{

    [ComponentCategory(CategoryTypes.CATID_PipelineComponent)]
    [ComponentCategory(CategoryTypes.CATID_Any)]
    [System.Runtime.InteropServices.Guid("C34F8675-DFCA-4B12-A9F3-727234FB2338")]

    public partial class ConcatContextProperties : IBaseComponent, IComponent, IComponentUI, IPersistPropertyBag
    {
        private const string ParamRegEx = @"(?<param>\"".*?\""|\{.+?#.+?\})(?:\s*,\s*(?<param>\"".*?\""|\{.+?#.+?\}))+$";
        [RequiredRuntime]
        [DisplayName("Parameters to concatenate ")]
        [Description("Specify the parameters to concatenate, e.g. text1,{http://namespace#property},text2")]
        [RegularExpression(@"^(\"".*?\""|\{.+?#.+?\})(\s*,\s*(\"".*?\""|\{.+?#.+?\}))+$")]
        public string Parameters { get; set; }

        [RequiredRuntime]
        [DisplayName("Throw Exception ")]
        [Description("Throw exceptions if the parameters cannot be evaluated.")]

        public bool ThrowException { get; set; }

        [DisplayName("Property Path")]
        [Description("The property path where the returned value will be promoted to, i.e. http://temupuri.org#MyProperty.")]
        [RegularExpression(@"^.+?#.*$",
         ErrorMessage = "A property path should be formatted as namespace#property.")]
        [RequiredRuntime]
        public string PropertyPath { get; set; }

        [DisplayName("Promote Property")]
        [Description("Specifies whether the property should be promoted or just written to the context.")]
        [RequiredRuntime]
        public bool PromoteProperty { get; set; }

        public IBaseMessage Execute(IPipelineContext pContext, IBaseMessage pInMsg)
        {
            if (Disabled)
            {
                return pInMsg;
            }

            string errorMessage;

            if (!Validate(out errorMessage))
            {
                throw new ArgumentException(errorMessage);
            }

            var ctx = pInMsg.Context;
            var m = Regex.Match(Parameters, ParamRegEx);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < m.Groups["param"].Captures.Count; i++)
            {
                string value = m.Groups["param"].Captures[i].Value;
                bool IsCtxProperty = value.StartsWith("{") & value.EndsWith("}");
                value = value.Substring(1, value.Length - 2);
                if (IsCtxProperty)
                {
                    object retval = null;
                    if (!ctx.TryRead(new ContextProperty(value), out retval) & ThrowException)
                    {
                        throw new ArgumentException("Cannot read property", m.Groups["param"].Captures[i].Value);
                    }
                    value = retval == null ? null : retval.ToString();
                }
                else
                {
                    value = value.Replace("{CR}", "\r").Replace("{LF}", "\n").Replace("{CRLF}", "\r\n").Replace("{TAB}", "\t");
                }
                sb.Append(value);
            }
            if (PromoteProperty)
                ctx.Promote(new ContextProperty(PropertyPath), sb.ToString());
            else
                ctx.Write(new ContextProperty(PropertyPath), sb.ToString());
            return pInMsg;
        }
    }
}
