%{
// ��� ���������� ����������� � ����� GPPGParser, �������������� ����� ������, ������������ �������� gppg
    public BlockNode root; // �������� ���� ��������������� ������ 
    public Parser(AbstractScanner<ValueType, LexLocation> scanner) : base(scanner) { }
%}

%output = SimpleYacc.cs

%union { 
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

%token BEGIN END CYCLE ASSIGN SEMICOLON PLUS MINUS
%token <iVal> INUM 
%token <dVal> RNUM 
%token <sVal> ID

%type <eVal> expr ident term
%type <stVal> assign statement cycle 
%type <blVal> stlist block

%%

progr   : block { root = $1; }
		;

stlist	: statement 
			{ 
				$$ = new BlockNode($1); 
			}
		| stlist SEMICOLON statement 
			{ 
				$1.Add($3); 
				$$ = $1; 
			}
		;

statement: assign { $$ = $1; }
		| block   { $$ = $1; }
		| cycle   { $$ = $1; }
	;

ident 	: ID { $$ = new IdNode($1); }	
		;
	
assign 	: ident ASSIGN expr { $$ = new AssignNode($1 as IdNode, $3); ((IdNode)$1).Value = $3.Eval(); }
		;

term    : ident  { $$ = $1 as IdNode; }
		| INUM { $$ = new IntNumNode($1); }
		;

expr	: expr PLUS term { $$ = new BinExprNode($1, $3, OpType.Plus); }
		| expr MINUS term { $$ = new BinExprNode($1, $3, OpType.Minus); }
		| term { $$ = $1; }
		;

block	: BEGIN stlist END { $$ = $2; }
		;

cycle	: CYCLE expr statement { $$ = new CycleNode($2, $3); }
		;
	
%%

