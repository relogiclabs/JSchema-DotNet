lexer grammar SchemaLexer;

// Components
TITLE: '%title';
VERSION: '%version' -> pushMode(DIRECTIVE_VERSION1);
INCLUDE: '%include';
PRAGMA: '%pragma';
DEFINE: '%define';
SCHEMA: '%schema';

// Keywords
TRUE: 'true';
FALSE: 'false';
NULL: 'null';

// Symbols
COLON: ':';
COMMA: ',';
STAR: '*';
LBRACE: '{';
RBRACE: '}';
LBRACKET: '[';
RBRACKET: ']';
LPAREN: '(';
RPAREN: ')';
OPTIONAL: '?';
UNDEFINED: '!';

// Identifiers
IDENTIFIER: BASE_IDENTIFIER ('.' BASE_IDENTIFIER)*;
ALIAS: '$' BASE_IDENTIFIER;
DATATYPE: '#' ALPHA+;
FUNCTION: '@' BASE_IDENTIFIER;

fragment BASE_IDENTIFIER: ALPHA ALPHANUMERIC*;
fragment ALPHA: [A-Za-z_];
fragment ALPHANUMERIC: [A-Za-z0-9_];

// String
STRING: '"' (ESCAPE | SAFECODEPOINT)* '"';
fragment ESCAPE: '\\' (["\\/bfnrt] | UNICODE);
fragment UNICODE: 'u' HEXDIGIT HEXDIGIT HEXDIGIT HEXDIGIT;
fragment HEXDIGIT: [0-9a-fA-F];
fragment SAFECODEPOINT: ~["\\\u0000-\u001F];

// Numbers
INTEGER: '-'? INTDIGIT;
FLOAT: INTEGER ('.' DIGIT+);
DOUBLE: INTEGER ('.' DIGIT+)? EXPONENT;

fragment INTDIGIT: '0' | [1-9] DIGIT*;
fragment EXPONENT: [eE] [+\-]? DIGIT+;
fragment DIGIT: [0-9];

// Comments
MULTILINE_COMMENT: MULTILINE_CMT -> channel(HIDDEN);
LINE_COMMENT: LINE_CMT -> channel(HIDDEN);

fragment MULTILINE_CMT: '/*' .*? '*/';
fragment LINE_CMT: '//' ~('\r' | '\n')*;

// Whitespace
WHITE_SPACE: WHITE_SPC -> channel(HIDDEN);
fragment WHITE_SPC: [\n\r\t ]+;

//---------------DIRECTIVE_VERSION1---------------
mode DIRECTIVE_VERSION1;

COLON1: ':';
VERSION_NUMBER1: DIGIT ('.' DIGIT+)* -> popMode;
WHITE_SPACE1: WHITE_SPC -> channel(HIDDEN);
MULTILINE_COMMENT1: MULTILINE_CMT -> channel(HIDDEN);
LINE_COMMENT1: LINE_CMT -> channel(HIDDEN);