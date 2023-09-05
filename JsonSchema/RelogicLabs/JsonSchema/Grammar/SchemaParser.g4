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
    : DEFINE aliasName COLON validatorMain
    ;

aliasName
    : ALIAS
    ;

validatorMain
    : value function* datatype* OPTIONAL?
    | function+ datatype* OPTIONAL?
    | datatype+ OPTIONAL?
    ;

validator
    : validatorMain
    | aliasName
    ;

value
    : primitive
    | object
    | array
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
    : DATATYPE STAR? (LPAREN aliasName RPAREN)?
    ;

function
    : FUNCTION STAR? (LPAREN (value (COMMA value)*)? RPAREN)?
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
