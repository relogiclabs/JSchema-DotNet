parser grammar JsonParser;

options { tokenVocab = JsonLexer; }

json
    : valueNode EOF
    ;

valueNode
    : primitiveNode
    | objectNode
    | arrayNode
    ;

objectNode
    : LBRACE ( propertyNode ( COMMA propertyNode )* )? RBRACE
    ;

propertyNode
    : STRING COLON valueNode
    ;

arrayNode
    : LBRACKET ( valueNode ( COMMA valueNode )* )? RBRACKET
    ;

primitiveNode
    : TRUE          # PrimitiveTrue
    | FALSE         # PrimitiveFalse
    | STRING        # PrimitiveString
    | INTEGER       # PrimitiveInteger
    | FLOAT         # PrimitiveFloat
    | DOUBLE        # PrimitiveDouble
    | NULL          # PrimitiveNull
    ;