lexer grammar SchemaLexer;

// Sections
TITLE : '%title';
VERSION : '%version';
IMPORT : '%import';
PRAGMA : '%pragma';
DEFINE : '%define';
SCHEMA : '%schema';
SCRIPT : '%script' -> pushMode(DIRECTIVE_SCRIPT1);

// Keywords
TRUE : 'true';
FALSE : 'false';
NULL : 'null';

// Symbols
COLON : ':';
COMMA : ',';
STAR : '*';
LBRACE : '{';
RBRACE : '}';
LBRACKET : '[';
RBRACKET : ']';
LPAREN : '(';
RPAREN : ')';
OPTIONAL : '?';
UNDEFINED : '!';

// Identifiers
FULL_IDENTIFIER : IDENTIFIER ( '.' IDENTIFIER )*;
ALIAS : '$' IDENTIFIER;
DATATYPE : '#' ALPHA+;
FUNCTION : '@' IDENTIFIER;
RECEIVER : '&' IDENTIFIER;

fragment IDENTIFIER : ALPHA ALPHANUMERIC*;
fragment ALPHA : [A-Za-z_];
fragment ALPHANUMERIC : [A-Za-z0-9_];

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

// Hidden Tokens
WHITE_SPACE : [\n\r\t ]+ -> channel(HIDDEN);
BLOCK_COMMENT : '/*' .*? '*/' -> channel(HIDDEN);
LINE_COMMENT : '//' ~('\r' | '\n')* -> channel(HIDDEN);

//---------------DIRECTIVE_SCRIPT1---------------
mode DIRECTIVE_SCRIPT1;

// Keywords
G_VAR : 'var';
G_IF : 'if';
G_ELSE : 'else';
G_WHILE : 'while';
G_FOR : 'for';
G_FOREACH : 'foreach';
G_IN : 'in';
G_BREAK : 'break';
G_CONSTRAINT : 'constraint';
G_TARGET : 'target';
G_CALLER : 'caller';
G_SUBROUTINE : 'subroutine';
G_TRYOF : 'tryof';
G_THROW : 'throw';
G_FUNCTION : 'function';
G_RETURN : 'return';
G_FUTURE : 'future';
G_TRUE : 'true';
G_FALSE : 'false';
G_NULL : 'null';
G_UNDEFINED : 'undefined';

// Reserved Keywords
G_THIS : 'this';
G_NEW : 'new';
G_CONTINUE : 'continue';
G_DO : 'do';
G_CONST : 'const';
G_SWITCH : 'switch';
G_CASE : 'case';
G_IMPORT : 'import';
G_CLASS : 'class';
G_SUPER : 'super';
G_DEFAULT : 'default';

// Literals
G_INTEGER : INTEGER;
G_DOUBLE : INTEGER FRACTION? EXPONENT?;
G_STRING : STRING;
G_IDENTIFIER : IDENTIFIER;

// Separator Symbols
G_LBRACE : '{';
G_RBRACE : '}';
G_LBRACKET : '[';
G_RBRACKET : ']';
G_LPAREN : '(';
G_RPAREN : ')';
G_SEMI : ';';
G_COMMA : ',';
G_DOT : '.';
G_COLON : ':';
G_RANGE : '..';
G_ELLIPSIS : '...';

// Operator Symbols
G_ASSIGN : '=';
G_INC : '++';
G_DEC : '--';
G_PLUS : '+';
G_MINUS : '-';
G_MUL : '*';
G_DIV : '/';
G_GT : '>';
G_LT : '<';
G_LE : '<=';
G_GE : '>=';
G_EQ : '==';
G_NE : '!=';
G_NOT : '!';
G_AND : '&&';
G_OR : '||';

// Next Sections
DEFINE1 : '%define' -> type(DEFINE), popMode;
SCHEMA1 : '%schema' -> type(SCHEMA), popMode;
SCRIPT1 : '%script' -> type(SCRIPT);

// Hidden Tokens
WHITE_SPACE1 : WHITE_SPACE -> channel(HIDDEN);
BLOCK_COMMENT1 : BLOCK_COMMENT -> channel(HIDDEN);
LINE_COMMENT1 : LINE_COMMENT -> channel(HIDDEN);