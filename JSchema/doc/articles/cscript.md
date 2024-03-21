# CScript Domain-Specific Language
CScript, a domain-specific scripting language, designed specifically for JSchema document. This lightweight, interpreted, and dynamically typed C-style language seamlessly integrates with the foundational principles of the existing JSchema architecture. With the use of dynamic typing and programming constructs similar to both JSON and JavaScript, streamlines the process of writing concise, user-friendly, and flexible logic for constraint or validation functions.

Although CScript is similar to JavaScript in many aspects, the CScript interpreter is designed to quickly halt (fail-fast) and report errors instead of trying to continue with potentially corrupted or unexpected data. This ensures a seamless alignment with the validation requirements of modern web services, significantly enhancing readability and productivity in both documentation and implementation, as well as reducing the potential for bugs.

## Keywords & Reserved Words
Here is a list of keywords (reserved words) in the CScript language. You cannot use any of the following words as identifiers in your scripts. Some keywords are reserved for future extensions and are not currently in use.

| Description               | Current Keyword (Reserved Word)                  |
|---------------------------|--------------------------------------------------|
| Variable Declaration      | `var`; (`const`)                                 |
| Conditional Flow Control  | `if`; `else`; (`switch`; `case`; `default`)      |
| Iterative Flow Control    | `for`; `foreach`; `while`; (`do`)                |
| Jump Control              | `break`; (`continue`)                            |
| Membership Management     | `in`                                             |
| Function Declaration      | `function`; `constraint`; `future`; `subroutine` |
| Function Data & Control   | `return`; `target`; `caller`                     |
| Literals Value            | `true`; `false`; `null`; `undefined`             |
| Exception Management      | `tryof`; `throw`                                 |
| Class & Object Management | (`class`; `new`; `super`; `this`)                |
| Script Modularization     | (`import`)                                       |

## Data Types
Data types play a pivotal role in specifying the fundamental structure of values that serve as the foundational components for data manipulation within CScript. In the dynamically typed CScript language, data types are classified into two main categories: primitive immutable value types and composite reference types.

Composite types include `#array` and `#object`, while the remaining types fall under the category of primitive types. Both primitive values and composite references of schema accessible from CScript are readonly and unmodifiable nodes. The following table lists CScript data types:

| SN | Type Name    | Example                                    |
|----|--------------|--------------------------------------------|
| 1  | `#array`     | `[1, 2, 3]`; `["item1", [2.5, 5.8]]`; `[]` |
| 2  | `#boolean`   | `true`; `false`                            |
| 3  | `#double`    | `10.5`; `20E-5`; `5E+5`                    |
| 4  | `#integer`   | `10`; `20`; `100`; `500`                   |
| 5  | `#null`      | `null`                                     |
| 6  | `#object`    | `{ k1: "text", k2: { "k11" : 10 } }`; `{}` |             
| 7  | `#range`     | `1..10`; `-10..-5`; `-10..`; `..100`       |
| 8  | `#string`    | `"any text"`; `""`                         |
| 9  | `#undefined` | `undefined`                                |
| 10 | `#void`      | Used only for internal purposes            |

The `#void` type is reserved for internal operations, including initializing unassigned l-values (variables, properties, and array elements) to the default state, among other cases. To explicitly represent the absence of a value in your script, use the `undefined` or `null` literal.

## Operators & Precedences
CScript operators are symbols that are used to perform operations on variables and values. The direct operation of any operator that requires a modifiable l-value including `++`, `--` or `=` will raise an exception for the readonly schema nodes. The following table lists the operators according to their precedences from the highest to the lowest:

| SN | Category                      | Operator             |
|----|-------------------------------|----------------------|
| 1  | Property Access & Parentheses | `.`; `[]`; `()`      |
| 2  | Unary Minus & Logical Not     | `-`; `!`             |
| 3  | Postfix Increment/Decrement   | `i++`; `i--`         |
| 4  | Prefix Increment/Decrement    | `++i`; `--i`         |
| 5  | Arithmetic Multiplicative     | `*`; `/`             |
| 6  | Arithmetic Additive           | `+`; `-`             |
| 7  | Sequence Range                | `..`                 |
| 8  | Relational Comparison         | `>`; `<`; `>=`; `<=` |
| 9  | Equality Comparison           | `==`; `!=`           |
| 10 | Logical And (Short-Circuit)   | `&&`                 |
| 11 | Logical Or (Short-Circuit)    | `||`                 |
| 12 | Assignment                    | `=`                  |

## Function Types
Function types are essential for specifying the executable units that serve as the building-blocks of validation process within CScript. All function types can also accept variable number of arguments, specified by an ellipsis `...` after the last parameter name which is then bound to an array containing the remaining arguments.

Additionally, all types of functions can be overloaded with varying numbers of fixed parameters, along with one that includes a variable argument parameter. A function name can be overloaded with only one variable argument definition, regardless of the number of required arguments preceding it. Fixed argument functions always take precedence or priority over variable argument functions when arguments match both definitions. Below are the various kinds of functions, each with distinct purposes, used in the CScript language:

### Constraint Function
The constraint function defines conditions for the target JSON value. They assess whether the target JSON value satisfies the conditions specified by the functions. The function should return true if the conditions are met; otherwise, it should return false. Even if the function exits without returning any value, it is assumed that all conditions are met since any early return from the function usually implies that the conditions are not satisfied.

Within the scope including nested scopes of a constraint function, the `target` keyword refers to the JSON value or node to which the constraint function is applied, and the `caller` keyword refers to the schema node that invokes this constraint function. The subsequent example illustrates various alternative forms of the definition of a constraint function named `example` with the main keyword `constraint`:
```js
constraint example(param1, param2, param3) {  }
constraint function example(param1, param2, params...) {  }
```

### Future Constraint Function
The future constraint function extends the utility of the constraint function by also considering receiver values. They ensure that the validations are deferred until the receivers of the schema have received their anticipated values, thus evaluating the specified conditions imposed by the functions at a delayed phase. The subsequent example illustrates various alternative forms of the definition of a future constraint function named `example` with the main keyword `future`:
```js
future example(param1, param2, param3) {  }
future constraint example(param1, param2, param3) {  }
future function example(param1, param2, param3) {  }
future constraint function example(param1, param2, params...) {  }
```

### Subroutine Function
The subroutine function supports the constraint function by promoting code reusability, readability, and modularization. These functions serve as auxiliary units, enhancing the organization and maintainability of validation in CScript, and are not available in the schema context for invocation. Conversely, the constraint function, as well as the future function, are special functions available in the schema context, and are not invocable from the script context without the `target` and `caller` information, thereby preventing any potential overloading conflicts between subroutine and constraint functions. The subsequent example illustrates various alternative forms of the definition of a subroutine function named `example` with the main keyword `subroutine`:
```js
subroutine example(param1, param2, param3) {  }
subroutine function example(param1, param2, params...) {  }
```

The CScript language provides a wide range of functions, data types, and programming constructs that can be used to implement diverse validation logic. This allows for the handling of complex validation requirements, both at the level of individual JSON values and groups of values received across different parts of the JSON document, ultimately ensuring the structural integrity of the entire JSON document.