# Build from Source Code
This comprehensive guide illustrates the procedures for retrieving source code from a GitHub repository, compiling the project source code into a library, and seamlessly integrating the compiled library into your project. Within this document, we will navigate through these steps, presenting them clearly and straightforwardly.

## Build the Library
To get started, clone the project from the following URL using your preferred Git client (command line or GUI). You can open a terminal and enter the following Git clone command as shown below:
```shell
git clone https://github.com/relogiclabs/JsonSchema-DotNet.git
```
Next, use .NET build command `dotnet build` to build the project and Retrieve the DLL file, `RelogicLabs.JsonSchema.dll` from the `bin` folder.

## Add the Library to Your Project
To integrate the library with your project, you can create a `libs` folder within your project directory and place the retrieved DLL file into the designated folder. Alternatively, if your IDE supports adding references, you can conveniently select the DLL from the `libs` folder. Alternatively, you can manually modify your project file `.csproj` using a text editor and include the following XML snippet:
```xml
<ItemGroup>
    <Reference Include="RelogicLabs.JsonSchema">
    <HintPath>libs\RelogicLabs.JsonSchema.dll</HintPath>
    </Reference>
</ItemGroup>
```
Additionally, this project has a dependency on ANTLR runtime, which you can integrate by executing the following command:
```shell
dotnet add package Antlr4.Runtime.Standard --version 4.13.1
```
## Write a Sample to Test
With all the necessary components in place, you are now ready to create a sample schema and validate a corresponding JSON against the schema. The subsequent example presents a C# class featuring a method designed for validating a sample JSON based on a provided schema. If you are working with C# 11 or above, you can enhance the code further by utilizing new C# language features like raw string literals, file scoped namespaces and others.
```c#
using RelogicLabs.JsonSchema;

namespace CSharpApplication
{
    public class SampleSchema
    {
        public bool CheckIsValid()
        {
            var schema =
                @"%title: ""User Profile Response""
                %version: 1.0.0
                %schema:
                {
                    ""user"": {
                        ""id"": @range(1, 10000) #integer,
                        /*username does not allow special characters*/
                        ""username"": @regex(""[a-z_]{3,30}"") #string,
                        /*currently only one role is allowed by system*/
                        ""role"": ""user"" #string,
                        ""isActive"": #boolean, //user account current status
                        ""registeredAt"": #time,
                        ""profile"": {
                            ""firstName"": @regex(""[A-Za-z ]{3,50}"") #string,
                            ""lastName"": @regex(""[A-Za-z ]{3,50}"") #string,
                            ""dateOfBirth"": #date,
                            ""age"": @range(18, 130) #integer,
                            ""email"": @email #string,
                            ""pictureURL"": @url #string,
                            ""address"": {
                                ""street"": @length(10, 200) #string,
                                ""city"": @length(3, 50) #string,
                                ""country"": @regex(""[A-Za-z ]{3,50}"") #string
                            } #object #null
                        }
                    }
                }";
            var json =
                @"{
                    ""user"": {
                        ""id"": 9999,
                        ""username"": ""johndoe"",
                        ""role"": ""user"",
                        ""isActive"": true,
                        ""registeredAt"": ""2023-09-06T15:10:30.639Z"",
                        ""profile"": {
                            ""firstName"": ""John"",
                            ""lastName"": ""Doe"",
                            ""dateOfBirth"": ""1993-06-17"",
                            ""age"": 30,
                            ""email"": ""john.doe@example.com"",
                            ""pictureURL"": ""https://example.com/picture.jpg"",
                            ""address"": {
                                ""street"": ""123 Some St"",
                                ""city"": ""Some town"",
                                ""country"": ""Some Country""
                            }
                        }
                    }
                }";
            JsonSchema jsonSchema = new(schema);
            return jsonSchema.IsValid(json);
        }
    }
}
```

## Create Validation Errors
Let's intentionally introduce a few errors by modifying the previous JSON document and then examine the validation results. To begin, we'll alter the `id` within the `user` object to a string type and observe the outcome. Additionally, we'll modify the `username` by inserting a space into its value, thus creating an invalid `username`. Below is the revised JSON representation, now containing these purposeful errors.
```json
{
    "user": {
        "id": "not number",
        "username": "john doe",
        "role": "user",
        "isActive": true,
        "profile": {
            "firstName": "John",
            "lastName": "Doe",
            "age": 30,
            "email": "john.doe@example.com",
            "pictureURL": "https://example.com/picture.jpg",
            "address": {
                "street": "123 Some St",
                "city": "Some town",
                "country": "Some Country"
            }
        }
    }
}
```
To achieve the desired outcome, please make the following changes to the preceding code. Specifically, ensure that any schema validation errors are displayed in the console. The modified code snippet that invokes the `WriteError` method to display the errors if validation fails is as follows:

```c#
...

JsonSchema jsonSchema = new(schema);
if(!jsonSchema.IsValid(json)) jsonSchema.WriteError();

...
```
Here is the error as displayed in the console. More specific errors will be listed first, followed by more general errors. Consequently, the specific errors will precisely pinpoint the issues within the JSON document, while the generic errors will provide contextual information about where the errors occurred.

```accesslog
Schema (Line: 6:47) Json (Line: 3:30) [DTYP04]: Data type mismatch. Data type #integer is expected but found #string inferred by "not number".
Schema (Line: 6:30) Json (Line: 3:30) [FUNC03]: Function @range(1, 10000) is incompatible with the target data type. Applying to a supported data type such as #number is expected but applied to an unsupported data type #string of "not number".
Schema (Line: 8:36) Json (Line: 4:36) [REGX01]: Regex pattern does not match. String of pattern "[a-z_]{3,30}" is expected but found "john doe" that mismatches with pattern.
Schema (Line: 5:28) Json (Line: 2:28) [VALD01]: Validation failed. Value {"id": @range(1, 10000) #integer, "username": @regex("[a-z_]{3,30}") #string, "role": "user" #string, "isActive": #boolean, "register...ing, "country": @regex("[A-Za-z ]{3,50}") #string} #object #null}} is expected but found {"id": "not number", "username": "john doe", "role": "user", "isActive": true, "registeredAt": "2023-09-06T15:10:30.639Z", "profile":...: "123 Some St", "city": "Some town", "country": "Some Country"}}}.
Schema (Line: 4:16) Json (Line: 1:0) [VALD01]: Validation failed. Value {"user": {"id": @range(1, 10000) #integer, "username": @regex("[a-z_]{3,30}") #string, "role": "user" #string, "isActive": #boolean, ...ng, "country": @regex("[A-Za-z ]{3,50}") #string} #object #null}}} is expected but found {"user": {"id": "not number", "username": "john doe", "role": "user", "isActive": true, "registeredAt": "2023-09-06T15:10:30.639Z", "... "123 Some St", "city": "Some town", "country": "Some Country"}}}}.
```

## Assertion for Validation
To utilize this library for test automation and API testing, you can use the following alternative code snippet to perform assertions on input JSON against a specified schema. For instance, let's examine how to assert the JSON, which has been intentionally altered to introduce some errors, against the aforementioned schema. The following demonstrates the adjusted code for asserting the JSON with errors:
```c#
...

try {
    JsonAssert.IsValid(schema, json);
} catch(Exception e) {
    Console.Error.WriteLine(e);
}

...
```
The following presents the printed stack trace for the preceding example. It's important to note that when using `JsonAssert`, it throws an exception upon encountering the first error, thus preventing the continuation of processing the rest of the schema:
```accesslog
RelogicLabs.JsonSchema.Exceptions.JsonSchemaException: DTYP04: Data type mismatch
Expected (Schema Line: 6:47): data type #integer
Actual (Json Line: 3:30): found #string inferred by "not number"

   at RelogicLabs.JsonSchema.Tree.RuntimeContext.FailWith(Exception exception)
   at RelogicLabs.JsonSchema.Types.JNode.FailWith(Exception exception)
   at RelogicLabs.JsonSchema.Types.JDataType.MatchForReport(JNode node, Boolean nested)
   at RelogicLabs.JsonSchema.Types.JValidator.<>c__DisplayClass15_0.<MatchDataType>b__2(JDataType d)
   at RelogicLabs.JsonSchema.Utilities.CollectionExtensions.ForEach[T](IEnumerable`1 enumeration, Action`1 action)
   at RelogicLabs.JsonSchema.Types.JValidator.MatchDataType(JNode node)
   at RelogicLabs.JsonSchema.Types.JValidator.Match(JNode node)
   at RelogicLabs.JsonSchema.Types.JObject.Match(JNode node)
   at RelogicLabs.JsonSchema.Types.JValidator.Match(JNode node)
   at RelogicLabs.JsonSchema.Types.JObject.Match(JNode node)
   at RelogicLabs.JsonSchema.Types.JValidator.Match(JNode node)
   at RelogicLabs.JsonSchema.Types.JRoot.Match(JNode node)
   at RelogicLabs.JsonSchema.JsonAssert.IsValid(String schemaExpected, String jsonActual)
   at CSharpApplication.SampleSchema.CheckIsValid() in /SampleSchema.cs:line 62
```
For more information about the schema syntax format and library functionalities, please refer to the reference documentation [here](/JsonSchema-DotNet/api/index.html).
