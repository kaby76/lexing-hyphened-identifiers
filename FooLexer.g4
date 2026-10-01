lexer grammar FooLexer;
options { superClass=FooLexerBase; }
VAR_ : 'var';
SEMI : ';';
IDENTIFIER : [a-zA-Z] ([a-zA-Z0-9] | { this.IsHyphenedId() }? '-')* ;
MINUS : '-' ;
PLUS : '+';
NUMBER : [0-9]+ ;
WS : [ \t\r\n]+ -> skip ;
