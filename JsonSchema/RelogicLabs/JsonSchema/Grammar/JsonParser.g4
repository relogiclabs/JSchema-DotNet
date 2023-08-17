parser grammar JsonParser;

options { tokenVocab = JsonLexer; }

json
    : value EOF
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
    : STRING COLON value
    ;

array
    : LBRACKET (value (COMMA value)*)? RBRACKET
    ;

primitive
    : TRUE          # PrimitiveTrue
    | FALSE         # PrimitiveFalse
    | STRING        # PrimitiveString
    | INTEGER       # PrimitiveInteger
    | FLOAT         # PrimitiveFloat
    | DOUBLE        # PrimitiveDouble
    | NULL          # PrimitiveNull
    ;