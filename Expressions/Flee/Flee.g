grammar Flee;

options
{
	language = CSharp3;
	TokenLabelType = CommonToken;
	output = AST;
	ASTLabelType = CommonTree;
}

@lexer::namespace { Expressions.Flee }
@parser::namespace { Expressions.Flee }
@modifier { internal }

prog
	: expression
	;

expression
	: logical_xor_expression ( OR logical_xor_expression )*
	;

logical_xor_expression
	: logical_and_expression ( XOR logical_and_expression )*
	;

logical_and_expression
	: equality_expression ( AND equality_expression )*
	;

equality_expression
	: in_expression ( ( '=' | '<>' ) in_expression )*
	;

in_expression
	: relational_expression ( IN ( IDENTIFIER | '(' argument_expression_list ')' ) )?
	;

relational_expression
	: shift_expression ( ( '<' | '>' | '<=' | '>=' ) shift_expression )*
	;

shift_expression
	: additive_expression ( ( '<<' | '>>' ) additive_expression )*
	;

additive_expression
	: multiplicative_expression ( '+' multiplicative_expression | '-' multiplicative_expression )*
	;

multiplicative_expression
	: cast_expression ( '^' cast_expression | '*' cast_expression | '/' cast_expression | '%' cast_expression )*
	;

cast_expression
	: CAST '(' expression ',' IDENTIFIER ( '[' ',' * ']' )? ')'
	| unary_expression
	;

unary_expression
	: postfix_expression
	| unary_operator cast_expression
	;

argument_expression_list
	: expression ( ',' expression )*
	;

postfix_expression
	: primary_expression
		( '[' argument_expression_list ']'
        | '(' ')'
        | '(' argument_expression_list ')'
        | '.' IDENTIFIER
        )*
	;

primary_expression
	: IDENTIFIER
	| constant
	| '(' expression ')'
	;

unary_operator
	: '+'
	| '-'
	| NOT
	;

constant
    : TRUE
	| FALSE
	| NULL
	| DATETIME_LITERAL
	| TIMESPAN_LITERAL
    | HEX_LITERAL
    | OCTAL_LITERAL
    | DECIMAL_LITERAL
    | CHARACTER_LITERAL
	| STRING_LITERAL
    | FLOATING_POINT_LITERAL
    ;

AND
	: ('A'|'a')('N'|'n')('D'|'d')
	;
	
IN
	: ('I'|'i')('N'|'n')
	;
	
OR
	: ('O'|'o')('R'|'r')
	;
	
XOR
	: ('X'|'x')('O'|'o')('R'|'r')
	;
	
CAST
	: ('C'|'c')('A'|'a')('S'|'s')('T'|'t')
	;
	
NOT
	: ('N'|'n')('O'|'o')('T'|'t')
	;

TRUE
	: ('T'|'t')('R'|'r')('U'|'u')('E'|'e')
	;

FALSE
	: ('F'|'f')('A'|'a')('L'|'l')('S'|'s')('E'|'e')
	;

NULL
	: ('N'|'n')('U'|'u')('L'|'l')('L'|'l')
	;

CHARACTER_LITERAL
    : '\'' ( ~('\\'|'\'') | EscapeSequence ) '\''
    ;

TIMESPAN_LITERAL
	: '#' '#' ( ~'#' )* '#'
	;

DATETIME_LITERAL
	: '#' ( ~'#' )* '#'
	;

STRING_LITERAL
	:  '"' ( ~('\\'|'"') | EscapeSequence )* '"'
    ;

HEX_LITERAL
	: '0' ('x'|'X') HexDigit+ IntegerTypeSuffix?
	;

DECIMAL_LITERAL
	: ('0' | '1'..'9' '0'..'9'*) IntegerTypeSuffix?
	;

OCTAL_LITERAL
	: '0' ('0'..'7')+ IntegerTypeSuffix?
	;

fragment
HexDigit : ('0'..'9'|'a'..'f'|'A'..'F') ;

fragment
IntegerTypeSuffix
	: ('u' | 'U') ('l' | 'L')?
	| ('l' | 'L')
	;

FLOATING_POINT_LITERAL
    : ('0'..'9')+ '.' ('0'..'9')* Exponent? FloatTypeSuffix?
    | '.' ('0'..'9')+ Exponent? FloatTypeSuffix?
    | ('0'..'9')+ Exponent FloatTypeSuffix?
    | ('0'..'9')+ FloatTypeSuffix
	;

fragment
Exponent
	: ('e'|'E') ('+'|'-')? ('0'..'9')+
	;

fragment
FloatTypeSuffix
	: ('f'|'F'|'d'|'D'|'m'|'M')
	;

fragment
EscapeSequence
    : '\\' ('b'|'t'|'n'|'f'|'r'|'\"'|'\''|'\\')
    | OctalEscape
    | UnicodeEscape
    ;

fragment
OctalEscape
    : '\\' ('0'..'3') ('0'..'7') ('0'..'7')
    | '\\' ('0'..'7') ('0'..'7')
    | '\\' ('0'..'7')
    ;

fragment
UnicodeEscape
	: '\\' ('u'|'U') HexDigit HexDigit HexDigit HexDigit
	;

IDENTIFIER
	: LETTER (LETTER|'0'..'9')*
	;
	
fragment
LETTER
	: 'A'..'Z'
	| 'a'..'z'
	| '_'
	;

WS
	: (' '|'\t'|'\r'|'\n')+
	{
// This construct is to make ANTLRWorks happy.
#if true
		Skip();
#else
		$channel = HIDDEN;
#endif
	}
	;