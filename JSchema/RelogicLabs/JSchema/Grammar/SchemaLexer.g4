lexer grammar SchemaLexer;

// Sections
S_TITLE : '%title';
S_VERSION : '%version';
S_IMPORT : '%import';
S_PRAGMA : '%pragma';
S_DEFINE : '%define';
S_SCHEMA : '%schema';
S_SCRIPT : '%script' -> pushMode(DIRECTIVE_SCRIPT1);

// Keywords
S_TRUE : 'true';
S_FALSE : 'false';
S_NULL : 'null';

// Symbols
S_COLON : ':';
S_COMMA : ',';
S_STAR : '*';
S_LBRACE : '{';
S_RBRACE : '}';
S_LBRACKET : '[';
S_RBRACKET : ']';
S_LPAREN : '(';
S_RPAREN : ')';
S_OPTIONAL : '?';
S_UNDEFINED : '!';

// Identifiers
S_GENERAL_ID : IDENTIFIER ( '.' IDENTIFIER )*;
S_ALIAS : '$' IDENTIFIER;
S_DATATYPE : '#' ALPHA+;
S_FUNCTION : '@' IDENTIFIER;
S_RECEIVER : '&' IDENTIFIER;

fragment IDENTIFIER : ALPHA ALPHANUMERIC*;
fragment ALPHA : [A-Za-z_];
fragment ALPHANUMERIC : [A-Za-z0-9_];

// String
S_STRING : '"' ( ESCAPE | SAFE_CODEPOINT )* '"';
fragment ESCAPE : '\\' ( ["\\/bfnrt] | UNICODE );
fragment UNICODE : 'u' HEXDIGIT HEXDIGIT HEXDIGIT HEXDIGIT;
fragment HEXDIGIT : [0-9a-fA-F];
fragment SAFE_CODEPOINT : ~["\\\u0000-\u001F];

// Numbers
S_INTEGER : '-'? INTDIGIT;
S_FLOAT : S_INTEGER FRACTION;
S_DOUBLE : S_INTEGER FRACTION? EXPONENT;

fragment FRACTION : '.' DIGIT+;
fragment INTDIGIT : '0' | [1-9] DIGIT*;
fragment EXPONENT : [eE] [+\-]? DIGIT+;
fragment DIGIT : [0-9];

// Hidden Tokens
S_WHITE_SPACE : [\n\r\t ]+ -> channel(HIDDEN);
S_BLOCK_COMMENT : '/*' .*? '*/' -> channel(HIDDEN);
S_LINE_COMMENT : '//' ~('\r' | '\n')* -> channel(HIDDEN);

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
G_NOT : 'not';

// Literals
G_INTEGER : S_INTEGER;
G_DOUBLE : S_INTEGER FRACTION? EXPONENT?;
G_STRING : S_STRING;
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
G_MOD : '%';
G_GT : '>';
G_LT : '<';
G_LE : '<=';
G_GE : '>=';
G_EQ : '==';
G_NE : '!=';
G_LNOT : '!';
G_LAND : '&&';
G_LOR : '||';

G_ADD_ASSIGN : '+=';
G_SUB_ASSIGN : '-=';
G_MUL_ASSIGN : '*=';
G_DIV_ASSIGN : '/=';
G_MOD_ASSIGN : '%=';

// Next Sections
G_DEFINE : '%define' -> type(S_DEFINE), popMode;
G_SCHEMA : '%schema' -> type(S_SCHEMA), popMode;
G_SCRIPT : '%script' -> type(S_SCRIPT);

// Hidden Tokens
G_WHITE_SPACE : S_WHITE_SPACE -> channel(HIDDEN);
G_BLOCK_COMMENT : S_BLOCK_COMMENT -> channel(HIDDEN);
G_LINE_COMMENT : S_LINE_COMMENT -> channel(HIDDEN);