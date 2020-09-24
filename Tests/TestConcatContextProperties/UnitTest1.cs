using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Winterdom.BizTalk.PipelineTesting;
using BizTalkComponents.PipelineComponents.ConcatContextProperties;

namespace TestConcatContextProperties
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestConcatProperties()
        {
            var pipeline = PipelineFactory.CreateEmptySendPipeline();
            pipeline.AddComponent(new ConcatContextProperties
            {
                PropertyPath="https://schemas.company.com/propertyschema#concatenatedValue",
                PromoteProperty=true,
                Parameters= "\"Abc\",{https://schemas.company.com/propertyschema#property1},{https://schemas.company.com/propertyschema#property2},\"jkl\"",

            }, PipelineStage.PreAssemble);
            var msg = MessageHelper.CreateFromString("");
            msg.Context.Promote("property1", "https://schemas.company.com/propertyschema", "def");
            msg.Context.Promote("property2", "https://schemas.company.com/propertyschema", "ghi");
            var output = pipeline.Execute(msg);
            string concatenatedValue = (string)output.Context.Read("concatenatedValue", "https://schemas.company.com/propertyschema");
            Assert.AreEqual(concatenatedValue, "Abcdefghijkl");
        }

        [TestMethod]
        public void TestPropertyNotExisted()
        {
            var pipeline = PipelineFactory.CreateEmptySendPipeline();
            pipeline.AddComponent(new ConcatContextProperties
            {
                PropertyPath = "https://schemas.company.com/propertyschema#concatenatedValue",
                PromoteProperty = true,
                Parameters = "\"Abc\",{https://schemas.company.com/propertyschema#property1},{https://schemas.company.com/propertyschema#property2},\"123\"",
            }, PipelineStage.PreAssemble);
            var msg = MessageHelper.CreateFromString("");
            msg.Context.Promote("property1", "https://schemas.company.com/propertyschema", "def");
            var output = pipeline.Execute(msg);
            string concatenatedValue = (string)output.Context.Read("concatenatedValue", "https://schemas.company.com/propertyschema");
            Assert.AreEqual(concatenatedValue, "Abcdef123");
        }
        [TestMethod, ExpectedException(typeof(ArgumentException))]
        public void TestPropertyNotExistedThrowException()
        {
            var pipeline = PipelineFactory.CreateEmptySendPipeline();
            pipeline.AddComponent(new ConcatContextProperties
            {
                PropertyPath = "https://schemas.company.com/propertyschema#concatenatedValue",
                PromoteProperty = true,
                Parameters = "\"Abc\",{https://schemas.company.com/propertyschema#property1},{https://schemas.company.com/propertyschema#property2},\"123\"",
                ThrowException=true
            }, PipelineStage.PreAssemble);
            var msg = MessageHelper.CreateFromString("");
            msg.Context.Promote("property1", "https://schemas.company.com/propertyschema", "def");
            var output = pipeline.Execute(msg);
            string concatenatedValue = (string)output.Context.Read("concatenatedValue", "https://schemas.company.com/propertyschema");
            Assert.AreEqual(concatenatedValue, "Abcdef123");
        }

        [TestMethod, ExpectedException(typeof(ArgumentException))]
        public void TestConcatPropertiesWrongParameters()
        {
            var pipeline = PipelineFactory.CreateEmptySendPipeline();
            pipeline.AddComponent(new ConcatContextProperties
            {
                PropertyPath = "https://schemas.company.com/propertyschema#concatenatedValue",
                PromoteProperty = true,
                Parameters = "\"Abc,{https://schemas.company.com/propertyschema#property1},{https://schemas.company.com/propertyschema#property2},\"jkl\"",

            }, PipelineStage.PreAssemble);
            var msg = MessageHelper.CreateFromString("");
            msg.Context.Promote("property1", "https://schemas.company.com/propertyschema", "def");
            msg.Context.Promote("property2", "https://schemas.company.com/propertyschema", "ghi");
            var output = pipeline.Execute(msg);
            string concatenatedValue = (string)output.Context.Read("concatenatedValue", "https://schemas.company.com/propertyschema");
            Assert.AreEqual(concatenatedValue, "Abcdefghijkl");
        }

        [TestMethod]
        public void TestSpecialCharacters()
        {
            var pipeline = PipelineFactory.CreateEmptySendPipeline();
            pipeline.AddComponent(new ConcatContextProperties
            {
                PropertyPath = "https://schemas.company.com/propertyschema#concatenatedValue",
                PromoteProperty = true,
                Parameters = "{https://schemas.company.com/propertyschema#property1},\"he{LF}l{CR}lo{CRLF}Wor{TAB}ld\"",

            }, PipelineStage.PreAssemble);
            var msg = MessageHelper.CreateFromString("");
            msg.Context.Promote("property1", "https://schemas.company.com/propertyschema", "def");
            msg.Context.Promote("property2", "https://schemas.company.com/propertyschema", "ghi");
            var output = pipeline.Execute(msg);
            string concatenatedValue = (string)output.Context.Read("concatenatedValue", "https://schemas.company.com/propertyschema");
            Assert.AreEqual(concatenatedValue, "defhe\nl\rlo\r\nWor\tld");
        }
    }
}
