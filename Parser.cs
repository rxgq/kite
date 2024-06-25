using System.Globalization;

namespace judas_script;

public enum ExprType
{ 
    Binary,
    Numeric,
    Identifier,
    Program
}

internal abstract class Stmt 
{ 
    public ExprType Type;
}

internal class ProgramStmt : Stmt 
{
    public List<Stmt> Body = new();

    public ProgramStmt()
    {
        Type = ExprType.Program;
    }

    public override string ToString()
    {
        Console.Write($"\nType: {Type,-16}");
        foreach (var expr in Body)
        {
            Console.Write($"\nType: {expr.Type,-16}");
        }
        return "";
    }
}

internal abstract class Expr : Stmt {}

internal class Binary : Expr 
{
    public ExprType ExprType = ExprType.Binary;
    public Expr Left;
    public Expr Right;
    public string Operator;
}

internal class Identifer : Expr
{
    public ExprType ExprType = ExprType.Identifier;
    public string Symbol;

    public Identifer(string symbol) 
    {
        Symbol = symbol;
    }

    public override string ToString()
        => $"\nType: {ExprType, -16} || Symbol: {Symbol, -16}";
}

internal class Numeric : Expr
{
    public ExprType ExprType = ExprType.Numeric;
    public float Value;

    public Numeric(float value) 
    {
        Value = value;
    }

    public override string ToString()
        => $"\nType: {ExprType,-16} || Value: {Value,-16}";
}


internal class Parser
{
    public readonly List<Token> Tokens;
    public int Current = -1;

    public Parser(List<Token> tokens) 
    {
        Tokens = tokens;
    }

    public ProgramStmt Parse() 
    {
        var program = new ProgramStmt();

        while (!IsEOFToken())
        {
            Advance();

            if (Tokens[Current].Type == TokenType.EOF)
                continue;

            var stmt = ParseStmt();
            program.Body.Add(stmt);
            Console.Write(stmt.ToString());
        }

        return program;
    }

    private Expr ParseStmt() 
    {
        return ParseExpr();
    }

    private Expr ParseExpr() 
    {
        return Additive();
    }

    private Expr Additive() 
    {
        var left = Primary();

        while (Tokens[Current].Lexeme == "+" || Tokens[Current].Lexeme == "-")
        {
        
        }


        return new Binary();
    }

    private Expr Primary() 
    {
        var token = Tokens[Current];

        return token.Type switch
        {
            TokenType.IDENTIFIER => new Identifer(token.Value.ToString()),
            TokenType.NUMBER => new Numeric(float.Parse(token.Value.ToString(), CultureInfo.InvariantCulture)),
            
            _ => new Identifer(""),
        };
    }


    public void Advance() => Current++;

    public bool IsEOFToken()
        => Current != -1 && Tokens[Current].Type == TokenType.EOF; 
}
