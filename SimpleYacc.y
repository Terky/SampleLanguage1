%{
// This defenition goes to class GPPGParser, which is a parser, that generated by gppg system
    public BlockNode root; // Syntax tree root 
    public Parser(AbstractScanner<ValueType, LexLocation> scanner) : base(scanner) { }
%}

%output = SimpleYacc.cs

%union { 
			public bool bVal;
			public double dVal; 
			public int iVal; 
			public string sVal; 
			public Node nVal;
			public ExprNode eVal;
			public StatementNode stVal;
			public BlockNode blVal;
       }

%using ProgramTree;

%namespace SimpleParser

%token BEGIN END CYCLE ASSIGN SEMICOLON PLUS MINUS LEFT_BRACKET RIGHT_BRACKET DIV MULT VAR COLON COMMA
%token <iVal> INUM 
%token <dVal> DNUM 
%token <bVal> BVAL
%token <sVal> ID

%type <eVal> expr ident term factor
%type <stVal> assign statement cycle decl
%type <blVal> stlist block

%%

progr    : fun_list function { root = $2; }
		 ;

fun_list : fun_list function { }
		 |
		 ;

function : header block {  }
		 ;

header   : ID ID LEFT_BRACKET arguments RIGHT_BRACKET {  }
		 ;

arguments: arguments COMMA ID ID
		 | ID ID
		 |
		 ;

stlist	 : statement 
			{ 
				$$ = new BlockNode($1); 
			}
		 | stlist SEMICOLON statement 
			{ 
				if ($3 != null)
				{
					$1.Add($3);
				}
				$$ = $1; 
			}
		 ;

statement: assign { $$ = $1; }
		| decl	  { $$ = $1; }
		| block   { $$ = $1; }
		| cycle   { $$ = $1; }
		| { $$ = null; }
	;

decl	: VAR ID COLON ID { $$ = new DeclNode($2, $4); }
		;

ident 	: ID { $$ = new IdNode($1); }	
		;
	
assign 	: ident ASSIGN expr { $$ = new AssignNode($1 as IdNode, $3); }
		;

expr	: expr PLUS term { $$ = new BinExprNode($1, $3, OpType.Plus); }
		| expr MINUS term { $$ = new BinExprNode($1, $3, OpType.Minus); }
		| term { $$ = $1; }
		;

term    : term MULT factor { $$ = new BinExprNode($1, $3, OpType.Mult); }
		| term DIV factor { $$ = new BinExprNode($1, $3, OpType.Div); }
		| factor { $$ = $1; }
		;

factor  : LEFT_BRACKET expr RIGHT_BRACKET { $$ = $2; }
		| ident  { $$ = $1 as IdNode; }
		| INUM { $$ = new IntNumNode($1); }
		| DNUM { $$ = new DoubleNumNode($1); }
		| BVAL { $$ = new BoolNode($1); }
		;

block	: BEGIN	stlist END { $$ = $2; }
		;

cycle	: CYCLE expr statement { $$ = new CycleNode($2, $3); }
		;

%%

