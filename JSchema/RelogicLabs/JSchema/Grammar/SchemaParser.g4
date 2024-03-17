parser grammar SchemaParser;

options { tokenVocab = SchemaLexer; }

//---------------Schema Rules---------------
schema
    : titleNode? versionNode? ( importNode | pragmaNode )*
            ( defineNode | scriptNode )*
            schemaMain
            ( defineNode | scriptNode )* EOF                      # CompleteSchema
    | validatorNode EOF                                           # ShortSchema
    ;

schemaMain
    : SCHEMA COLON validatorNode
    ;

titleNode
    : TITLE COLON STRING
    ;

versionNode
    : VERSION COLON STRING
    ;

importNode
    : IMPORT COLON FULL_IDENTIFIER ( COMMA FULL_IDENTIFIER )?
    ;

pragmaNode
    : PRAGMA FULL_IDENTIFIER COLON primitiveNode
    ;

defineNode
    : DEFINE aliasNode COLON validatorMain
    ;

aliasNode
    : ALIAS
    ;

validatorMain
    : valueNode functionNode* datatypeNode* receiverNode* OPTIONAL?
    | functionNode+ datatypeNode* receiverNode* OPTIONAL?
    | datatypeNode+ receiverNode* OPTIONAL?
    ;

validatorNode
    : validatorMain
    | aliasNode
    ;

valueNode
    : primitiveNode
    | objectNode
    | arrayNode
    ;

receiverNode
    : RECEIVER
    ;

objectNode
    : LBRACE ( propertyNode ( COMMA propertyNode )* )? RBRACE
    ;

propertyNode
    : STRING COLON validatorNode
    ;

arrayNode
    : LBRACKET ( validatorNode ( COMMA validatorNode )* )? RBRACKET
    ;

datatypeNode
    : DATATYPE STAR? ( LPAREN aliasNode RPAREN )?
    ;

functionNode
    : FUNCTION STAR? ( LPAREN ( argumentNode ( COMMA argumentNode )* )? RPAREN )?
    ;

argumentNode
    : valueNode
    | receiverNode
    ;

primitiveNode
    : TRUE                # PrimitiveTrue
    | FALSE               # PrimitiveFalse
    | STRING              # PrimitiveString
    | INTEGER             # PrimitiveInteger
    | FLOAT               # PrimitiveFloat
    | DOUBLE              # PrimitiveDouble
    | NULL                # PrimitiveNull
    | UNDEFINED           # PrimitiveUndefined
    ;


//---------------Script Rules---------------
scriptNode
    : SCRIPT G_COLON G_LBRACE globalStatement+ G_RBRACE
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
    : G_VAR varInitialization ( G_COMMA varInitialization )* G_SEMI
    ;

varInitialization
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
    : refExpression                                                             # AllRefExpression
    | G_MINUS expression                                                        # UnaryMinusExpression
    | G_NOT expression                                                          # LogicalNotExpression
    | refExpression G_INC                                                       # PostIncrementExpression
    | refExpression G_DEC                                                       # PostDecrementExpression
    | G_INC refExpression                                                       # PreIncrementExpression
    | G_DEC refExpression                                                       # PreDecrementExpression
    | expression ( G_MUL | G_DIV ) expression                                   # MultiplicativeExpression
    | expression ( G_PLUS | G_MINUS ) expression                                # AdditiveExpression
    | expression G_RANGE expression?                                            # RangeBothExpression
    | G_RANGE expression                                                        # RangeEndExpression
    | expression ( G_GE | G_LE | G_GT | G_LT ) expression                       # RelationalExpression
    | expression ( G_EQ | G_NE ) expression                                     # EqualityExpression
    | expression G_AND expression                                               # LogicalAndExpression
    | expression G_OR expression                                                # LogicalOrExpression
    | refExpression G_ASSIGN expression                                         # AssignmentExpression
    | literal                                                                   # LiteralExpression
    | G_LPAREN expression G_RPAREN                                              # ParenthesizedExpression
    | G_TRYOF G_LPAREN expression G_RPAREN                                      # TryofExpression
    | G_THROW G_LPAREN expression ( G_COMMA expression )? G_RPAREN              # ThrowExpression
    ;

refExpression
    : refExpression G_DOT G_IDENTIFIER                                          # DotExpression
    | refExpression G_LBRACKET expression G_RBRACKET                            # IndexExpression
    | G_IDENTIFIER G_LPAREN ( expression ( G_COMMA expression )* )? G_RPAREN    # InvokeExpression
    | G_TARGET                                                                  # TargetExpression
    | G_CALLER                                                                  # CallerExpression
    | G_IDENTIFIER                                                              # IdentifierExpression
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