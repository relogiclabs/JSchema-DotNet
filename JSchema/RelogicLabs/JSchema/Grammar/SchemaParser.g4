parser grammar SchemaParser;

options { tokenVocab = SchemaLexer; }

//---------------Schema Rules---------------
schema
    : titleNode? versionNode? ( importNode | pragmaNode )*
            ( defineNode | scriptNode )*
            schemaCoreNode
            ( defineNode | scriptNode )* EOF                      # CompleteSchema
    | validatorNode EOF                                           # ShortSchema
    ;

schemaCoreNode
    : S_SCHEMA S_COLON validatorNode
    ;

titleNode
    : S_TITLE S_COLON S_STRING
    ;

versionNode
    : S_VERSION S_COLON S_STRING
    ;

importNode
    : S_IMPORT S_COLON S_GENERAL_ID ( S_COMMA S_GENERAL_ID )?
    ;

pragmaNode
    : S_PRAGMA S_GENERAL_ID S_COLON primitiveNode
    ;

defineNode
    : S_DEFINE aliasNode S_COLON validatorMainNode
    ;

validatorNode
    : validatorMainNode
    | aliasNode
    ;

validatorMainNode
    : valueNode functionNode* datatypeNode* receiverNode* S_OPTIONAL?
    | functionNode+ datatypeNode* receiverNode* S_OPTIONAL?
    | datatypeNode+ receiverNode* S_OPTIONAL?
    ;

aliasNode
    : S_ALIAS
    ;

valueNode
    : primitiveNode
    | objectNode
    | arrayNode
    ;

receiverNode
    : S_RECEIVER
    ;

objectNode
    : S_LBRACE ( propertyNode ( S_COMMA propertyNode )* )? S_RBRACE
    ;

propertyNode
    : S_STRING S_COLON validatorNode
    ;

arrayNode
    : S_LBRACKET ( validatorNode ( S_COMMA validatorNode )* )? S_RBRACKET
    ;

datatypeNode
    : S_DATATYPE S_STAR? ( S_LPAREN aliasNode S_RPAREN )?
    ;

functionNode
    : S_FUNCTION S_STAR? ( S_LPAREN ( argumentNode ( S_COMMA argumentNode )* )? S_RPAREN )?
    ;

argumentNode
    : valueNode
    | receiverNode
    ;

primitiveNode
    : S_TRUE                # TrueNode
    | S_FALSE               # FalseNode
    | S_STRING              # StringNode
    | S_INTEGER             # IntegerNode
    | S_FLOAT               # FloatNode
    | S_DOUBLE              # DoubleNode
    | S_NULL                # NullNode
    | S_UNDEFINED           # UndefinedNode
    ;


//---------------Script Rules---------------
scriptNode
    : S_SCRIPT G_COLON G_LBRACE globalStatement+ G_RBRACE
    ;

globalStatement
    : functionDeclaration
    | varStatement
    ;

statement
    : varStatement
    | expressionStatement
    | ifStatement
    | whileStatement
    | forStatement
    | foreachStatement
    | returnStatement
    | breakStatement
    | blockStatement
    ;

functionDeclaration
    : ( G_CONSTRAINT G_FUNCTION? | G_FUTURE G_CONSTRAINT? G_FUNCTION? | G_SUBROUTINE G_FUNCTION? )
            name=G_IDENTIFIER G_LPAREN
            ( G_IDENTIFIER ( G_COMMA G_IDENTIFIER )* G_ELLIPSIS? )?
            G_RPAREN blockStatement
    ;

varStatement
    : G_VAR varDeclaration ( G_COMMA varDeclaration )* G_SEMI
    ;

varDeclaration
    : G_IDENTIFIER ( G_ASSIGN expression )?
    ;

expressionStatement
    : expression G_SEMI
    ;

ifStatement
    : G_IF G_LPAREN expression G_RPAREN statement ( G_ELSE statement )?
    ;

whileStatement
    : G_WHILE G_LPAREN expression G_RPAREN statement
    ;

forStatement
    : G_FOR G_LPAREN ( varStatement | initialization=expressionList G_SEMI | G_SEMI )
            condition=expression? G_SEMI updation=expressionList?
            G_RPAREN statement
    ;

expressionList
    : expression ( G_COMMA expression )*
    ;

foreachStatement
    : G_FOREACH G_LPAREN G_VAR G_IDENTIFIER G_IN expression G_RPAREN statement
    ;

returnStatement
    : G_RETURN expression G_SEMI
    ;

breakStatement
    : G_BREAK G_SEMI
    ;

blockStatement
    : G_LBRACE statement* G_RBRACE
    ;

expression
    : expression G_LBRACKET expression G_RBRACKET                               # MemberBracketExpression
    | expression G_DOT G_IDENTIFIER                                             # MemberDotExpression
    | G_IDENTIFIER G_LPAREN ( expression ( G_COMMA expression )* )? G_RPAREN    # InvokeFunctionExpression
    | expression G_DOT G_IDENTIFIER
            G_LPAREN ( expression ( G_COMMA expression )* )? G_RPAREN           # InvokeMethodExpression
    | G_PLUS expression                                                         # UnaryPlusExpression
    | G_MINUS expression                                                        # UnaryMinusExpression
    | G_LNOT expression                                                         # LogicalNotExpression
    | expression G_LBRACKET expression G_RBRACKET ( G_INC | G_DEC )             # PostIncDecExpression
    | expression G_DOT G_IDENTIFIER ( G_INC | G_DEC )                           # PostIncDecExpression
    | G_IDENTIFIER ( G_INC | G_DEC )                                            # PostIncDecExpression
    | ( G_INC | G_DEC ) expression G_LBRACKET expression G_RBRACKET             # PreIncDecExpression
    | ( G_INC | G_DEC ) expression G_DOT G_IDENTIFIER                           # PreIncDecExpression
    | ( G_INC | G_DEC ) G_IDENTIFIER                                            # PreIncDecExpression
    | expression ( G_MUL | G_DIV | G_MOD ) expression                           # MultiplicativeExpression
    | expression ( G_PLUS | G_MINUS ) expression                                # AdditiveExpression
    | expression G_RANGE expression?                                            # RangeBothExpression
    | G_RANGE expression                                                        # RangeEndExpression
    | expression ( G_GE | G_LE | G_GT | G_LT ) expression                       # RelationalExpression
    | expression ( G_EQ | G_NE ) expression                                     # EqualityExpression
    | expression G_LAND expression                                              # LogicalAndExpression
    | expression G_LOR expression                                               # LogicalOrExpression
    | expression G_LBRACKET expression G_RBRACKET G_ASSIGN expression           # AssignmentBracketExpression
    | expression G_DOT G_IDENTIFIER G_ASSIGN expression                         # AssignmentDotExpression
    | G_IDENTIFIER G_ASSIGN expression                                          # AssignmentIdExpression
    | expression G_LBRACKET expression G_RBRACKET
            ( G_ADD_ASSIGN | G_SUB_ASSIGN | G_MUL_ASSIGN
            | G_DIV_ASSIGN | G_MOD_ASSIGN ) expression                          # AssignmentAugExpression
    | expression G_DOT G_IDENTIFIER
            ( G_ADD_ASSIGN | G_SUB_ASSIGN | G_MUL_ASSIGN
            | G_DIV_ASSIGN | G_MOD_ASSIGN ) expression                          # AssignmentAugExpression
    | G_IDENTIFIER ( G_ADD_ASSIGN | G_SUB_ASSIGN | G_MUL_ASSIGN
            | G_DIV_ASSIGN | G_MOD_ASSIGN ) expression                          # AssignmentAugExpression
    | G_TARGET                                                                  # TargetExpression
    | G_CALLER                                                                  # CallerExpression
    | G_IDENTIFIER                                                              # IdentifierExpression
    | literal                                                                   # LiteralExpression
    | G_LPAREN expression G_RPAREN                                              # ParenthesizedExpression
    | G_TRYOF G_LPAREN expression G_RPAREN                                      # TryofExpression
    | G_THROW G_LPAREN expression ( G_COMMA expression )? G_RPAREN              # ThrowExpression
    ;

literal
    : G_TRUE                                                                    # TrueLiteral
    | G_FALSE                                                                   # FalseLiteral
    | G_INTEGER                                                                 # IntegerLiteral
    | G_DOUBLE                                                                  # DoubleLiteral
    | G_STRING                                                                  # StringLiteral
    | G_LBRACKET ( expression ( G_COMMA expression )* )? G_RBRACKET             # ArrayLiteral
    | G_LBRACE
            ( keys+=( G_IDENTIFIER | G_STRING )
                    G_COLON values+=expression
            ( G_COMMA keys+=( G_IDENTIFIER | G_STRING )
                    G_COLON values+=expression )* )?
      G_RBRACE                                                                  # ObjectLiteral
    | G_NULL                                                                    # NullLiteral
    | G_UNDEFINED                                                               # UndefinedLiteral
    ;