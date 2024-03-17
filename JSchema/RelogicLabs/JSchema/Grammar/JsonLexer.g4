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

STRING : '"' (ESCAPE | SAFECODEPOINT)* '"';
fragment ESCAPE : '\\' ( ["\\/bfnrt] | UNICODE);
fragment UNICODE : 'u' HEXDIGIT HEXDIGIT HEXDIGIT HEXDIGIT;
fragment HEXDIGIT : [0-9a-fA-F];
fragment SAFECODEPOINT : ~["\\\u0000-\u001F];

INTEGER : '-' ? INTDIGIT;
FLOAT : INTEGER ('.' DIGIT+);
DOUBLE : INTEGER ('.' DIGIT+)? EXPONENT;

fragment INTDIGIT : '0' | [1-9] DIGIT*;
fragment EXPONENT : [eE] [+\-]? DIGIT+;
fragment DIGIT : [0-9];

WHITE_SPACE : [\n\r\t ]+ -> skip;
