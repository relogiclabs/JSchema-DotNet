New JSON Schema is a practical tool for defining and validating the structure of JSON documents. It allows you to specify the expected format, data types, and constraints of JSON data. This guide will help you get started with the New JSON Schema.

# Getting Started
This guide will walk you through the essential steps to quickly get up and running with New JSON Schema library. It is also assume that you are familiar with .NET SDK and C# programming language. You can use a prebuilt library and skip the build library section.

## Build the Library
To get started, clone the project from the following URL using your preferred Git client (command line or GUI). You can open a terminal and enter the following Git clone command as shown below:
```shell
git clone https://github.com/relogiclabs/JsonSchema-DotNet.git
```
Next, use .NET build command `dotnet build` to build the project and Retrieve the DLL file, `RelogicLabs.JsonSchema.dll` from the `bin` folder.

## Add the Library to Your Project
To integrate the library with your project, you can create a `libs` folder within your project directory and place the retrieved DLL file (or any prebuilt DLL file) into the designated folder. Alternatively, if your IDE supports adding references, you can conveniently select the DLL from the `libs` folder. Alternatively, you can manually modify your project file `.csproj` using a text editor and include the following XML snippet:
```xml
<ItemGroup>
    <Reference Include="RelogicLabs.JsonSchema">
    <HintPath>libs\RelogicLabs.JsonSchema.dll</HintPath>
    </Reference>
</ItemGroup>
```
Additionally, this project has a dependency on ANTLR runtime, which you can integrate by executing the following command:
```shell
dotnet add package Antlr4.Runtime.Standard --version 4.13.0
```
## Write a Sample to Test
With the necessary components in place, you are now prepared to create a sample schema and validate a corresponding JSON against the schema. The subsequent example presents a C# class featuring a method designed for validating a sample JSON based on a provided schema.
```c#
using RelogicLabs.JsonSchema;

namespace CSharpApplication;

public class SampleSchema
{
    public bool CheckIsValid()
    {
        var schema =
            """
            %title: "User Profile Response"
            %version: 1.0.0
            %schema:
            {
                "user": {
                    "id": @range(1, 10000) #integer,
                    /*username does not allow special characters*/
                    "username": @regex("[a-z_]{3,30}") #string,
                    /*currently only one role is allowed by system*/
                    "role": "user" #string,
                    "isActive": #boolean, //user account current status
                    "profile": {
                        "firstName": @regex("[A-Za-z ]{3,50}") #string,
                        "lastName": @regex("[A-Za-z ]{3,50}") #string,
                        "age": @range(18, 130) #integer,
                        "email": @email #string,
                        "pictureURL": @url #string,
                        "address": {
                            "street": @length(10, 200) #string,
                            "city": @length(3, 50) #string,
                            "country": @regex("[A-Za-z ]{3,50}") #string
                        } #object #null
                    }
                }
            }
            """;
        var json =
            """
            {
                "user": {
                    "id": 1234,
                    "username": "johndoe",
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
            """;
        JsonSchema jsonSchema = new(schema);
        return jsonSchema.IsValid(json);
    }
}
```
For more information about the schema syntax format and library functionalities, please refer to the reference documentation [here](/JsonSchema-DotNet/api/index.html).

## Create Some Validation Errors
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
Schema (Line: 6:31) Json (Line: 3:14) [DTYP02]: Data type mismatch. Data type #integer is expected but found #string inferred by "not number".
Schema (Line: 6:14) [FUNC03]: Function @range(1, 10000) is applicable on JNumber but applied on JString of "not number"
Schema (Line: 8:20) Json (Line: 4:20) [REGX01]: Regex pattern does not match. String of pattern "[a-z_]{3,30}" is expected but found "john doe" that mismatches with pattern.
Schema (Line: 5:12) Json (Line: 2:12) [VALD01]: Validation Failed. Value {"id": @range(1, 10000) #integer, "username": @regex("[a-z_]{3,30}") #string, "role": "user" #string, "isActive": #boolean, "profile"...ing, "country": @regex("[A-Za-z ]{3,50}") #string} #object #null}} is expected but found {"id": "not number", "username": "john doe", "role": "user", "isActive": true, "profile": {"firstName": "John", "lastName": "Doe", "a...: "123 Some St", "city": "Some town", "country": "Some Country"}}}.
Schema (Line: 4:0) Json (Line: 1:0) [VALD01]: Validation Failed. Value {"user": {"id": @range(1, 10000) #integer, "username": @regex("[a-z_]{3,30}") #string, "role": "user" #string, "isActive": #boolean, ...ng, "country": @regex("[A-Za-z ]{3,50}") #string} #object #null}}} is expected but found {"user": {"id": "not number", "username": "john doe", "role": "user", "isActive": true, "profile": {"firstName": "John", "lastName": ... "123 Some St", "city": "Some town", "country": "Some Country"}}}}.
```
