
# Concat Context Properties
Allows to concatenate several context properties and text values and writes the result to a context property.

## Properties
|Property|Type|Description|
|--|--|--|
|Parameters |String (Required)| A parameters array separated by comma, parameters can be fixed text or context properties (see n |
|ThrowException| Boolean | If true, the component throws an exception if a parameter is invalid, the default is false.
|PropertyPath| String (Required)|The context property the concatenated value will be stored in.
|PromoteProperty| Boolean| if true, the property specified in the PropertyPath will be promoted, otherwise it will be written to the context without promotion.

### *Parameters*
The  parameters property is an array of parameters separated by comma, like concat function in different programing languages, parameters should have at least two parameters.

A parameter can be a fixed text or a context property, fixed texts must be quoted with double quotation "text value", while a context property must be enclosed with curly brackets {http://namespace#property}.

There are few special characters allowed in the text parameters, these characters are: {CR}, {LF}, {CRLF}, {TAB}
the following is an example of the parameters property value:
~~~ 
"Get{TAB}Properties{CRLF}",{https://namespace1#property1},"-",{https://namespace2#property2}
~~~
if a property does not exist in the message context, and ThrowException is set to True, InvalidArgumentException will be thrown, otherwise, the property will be ignored and it will be repalced with empty string.
