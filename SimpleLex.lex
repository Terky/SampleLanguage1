%using SimpleParser;
%using QUT.Gppg;
%using System.Linq;

%namespace SimpleScanner

Alpha 	[a-zA-Z_]
Digit   [0-9] 
AlphaDigit {Alpha}|{Digit}
INTNUM  -?{Digit}+
REALNUM {INTNUM}\.{INTNUM}
BOOLVAL "true"|"false"
ID {Alpha}{AlphaDigit}*

%%

{INTNUM} { 
  yylval.iVal = int.Parse(yytext); 
  return (int)Tokens.INUM; 
}

{REALNUM} { 
  yylval.dVal = double.Parse(yytext); 
  return (int)Tokens.DNUM;
}

{BOOLVAL} {
	yylval.bVal = bool.Parse(yytext);
	return (int)Tokens.BVAL;
}

{ID}  { 
  int res = ScannerHelper.GetIDToken(yytext);
  if (res == (int)Tokens.ID)
	yylval.sVal = yytext;
  return res;
}

"+" { return (int)Tokens.PLUS; }
"-" { return (int)Tokens.MINUS; }
"=" { return (int)Tokens.ASSIGN; }
";" { return (int)Tokens.SEMICOLON; }
"(" { return (int)Tokens.LEFT_BRACKET; }
")" { return (int)Tokens.RIGHT_BRACKET; }
"*" { return (int)Tokens.MULT; }
"/" { return (int)Tokens.DIV; }
":" { return (int)Tokens.COLON; }
"," { return (int)Tokens.COMMA; }
"{" { return (int)Tokens.BEGIN; }
"}" { return (int)Tokens.END; }
">" { return (int)Tokens.GT; }
"<" { return (int)Tokens.LT; }
">=" { return (int)Tokens.GET; }
"<=" { return (int)Tokens.LET; }
"!=" { return (int)Tokens.NEQ; }
"==" { return (int)Tokens.EQ; }
"&&" { return (int)Tokens.AND; }
"||" { return (int)Tokens.OR; }
"!" { return (int)Tokens.NOT; }

[^ \r\n] {
	LexError();
}

%{
  yylloc = new LexLocation(tokLin, tokCol, tokELin, tokECol);
%}

%%

public override void yyerror(string format, params object[] args) // syntax exception handling
{
  var ww = args.Skip(1).Cast<string>().ToArray();
  string errorMsg = string.Format("({0},{1}): Found {2}, expect {3}", yyline, yycol, args[0], string.Join(" or ", ww));
  throw new SyntaxException(errorMsg);
}

public void LexError()
{
  string errorMsg = string.Format("({0},{1}): Unknown symbol {2}", yyline, yycol, yytext);
  throw new LexException(errorMsg);
}

class ScannerHelper 
{
  private static Dictionary<string,int> keywords;

  static ScannerHelper() 
  {
    keywords = new Dictionary<string,int>();
    keywords.Add("cycle",(int)Tokens.CYCLE);
	keywords.Add("var", (int)Tokens.VAR);
	keywords.Add("return", (int)Tokens.RETURN);
  }
  public static int GetIDToken(string s)
  {
	if (keywords.ContainsKey(s.ToLower()))
	  return keywords[s];
	else
      return (int)Tokens.ID;
  }
  
}
