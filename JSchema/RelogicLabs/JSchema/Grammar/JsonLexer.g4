lexer grammar JsonLexer;

TRUE : 'true';
FALSE : 'false';
NULL : 'null';
LBRACKET : '[';
RBRACKET : ']';
LBRACE : '{';
RBRACE : '}';
COMMA : ',';
COLON : ':';

// String
STRING : '"' ( ESCAPE | SAFE_CODEPOINT )* '"';
fragment ESCAPE : '\\' ( ["\\/bfnrt] | UNICODE );
fragment UNICODE : 'u' HEXDIGIT HEXDIGIT HEXDIGIT HEXDIGIT;
fragment HEXDIGIT : [0-9a-fA-F];
fragment SAFE_CODEPOINT : ~["\\\u0000-\u001F];

// Numbers
INTEGER : '-'? INTDIGIT;
FLOAT : INTEGER FRACTION;
DOUBLE : INTEGER FRACTION? EXPONENT;

fragment FRACTION : '.' DIGIT+;
fragment INTDIGIT : '0' | [1-9] DIGIT*;
fragment EXPONENT : [eE] [+\-]? DIGIT+;
fragment DIGIT : [0-9];

WHITE_SPACE : [\n\r\t ]+ -> skip;