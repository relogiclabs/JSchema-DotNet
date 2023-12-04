parser grammar SchemaParser;

options { tokenVocab = SchemaLexer; }

schema
    : title? version? include* pragma*
            define* schemaBase define* EOF  # AggregateSchema
    | validator EOF                         # CoreSchema
    ;

schemaBase
    : SCHEMA COLON validator
    ;

title
    : TITLE COLON STRING
    ;

version
    : VERSION COLON1 VERSION_NUMBER1;

include
    : INCLUDE COLON IDENTIFIER (COMMA IDENTIFIER)?
    ;

pragma
    : PRAGMA IDENTIFIER COLON primitive
    ;

define
    : DEFINE alias COLON validatorMain
    ;

alias
    : ALIAS
    ;

validatorMain
    : value function* datatype* receiver* OPTIONAL?
    | function+ datatype* receiver* OPTIONAL?
    | datatype+ receiver* OPTIONAL?
    ;

validator
    : validatorMain
    | alias
    ;

value
    : primitive
    | object
    | array
    ;

receiver
    : RECEIVER
    ;

object
    : LBRACE (property (COMMA property)*)? RBRACE
    ;

property
    : STRING COLON validator
    ;

array
    : LBRACKET (validator (COMMA validator)*)? RBRACKET
    ;

datatype
    : DATATYPE STAR? (LPAREN alias RPAREN)?
    ;

function
    : FUNCTION STAR? (LPAREN (argument (COMMA argument)*)? RPAREN)?
    ;

argument
    : value
    | receiver
    ;

primitive
    : TRUE          # PrimitiveTrue
    | FALSE         # PrimitiveFalse
    | STRING        # PrimitiveString
    | INTEGER       # PrimitiveInteger
    | FLOAT         # PrimitiveFloat
    | DOUBLE        # PrimitiveDouble
    | NULL          # PrimitiveNull
    | UNDEFINED     # PrimitiveUndefined
    ;