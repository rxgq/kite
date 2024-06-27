namespace Judas;

public enum ExprType
{
    Binary, Unary, Identifier, VariableDeclaration,

    Numeric, StringLiteral, BooleanLiteral, WhiteSpace,
    
    IfStatement, EndOfInput, Grouping, Unknown, MethodCall,
}

public abstract class Expr
{
    public ExprType Type { get; set; }

    public void ToString(Expr expr)
    {
        if (expr is BinaryExpr bl)
            Console.Write(bl.ToString());

        else if (expr is UnaryExpr ub)
            Console.Write(ub.ToString());
    }
}

public class BinaryExpr : Expr
{
    public Expr Left { get; set; }
    public Expr Right { get; set; }
    public string Operator { get; set; }

    public BinaryExpr(Expr left, Expr right, string op)
    {
        Type = ExprType.Binary;
        Left = left;
        Right = right;
        Operator = op;
    }

    public override string ToString() => $"({Left} {Operator} {Right})";
}

public class UnaryExpr : Expr 
{
    public Expr Value { get; set; }

    public string Operator { get; set; }

    public UnaryExpr(Expr value, string op) 
    {
        Type = ExprType.Unary;
        Value = value;
        Operator = op;
    }
    public override string ToString() => $"({Value}{Operator})\n";
}


public class StringLiteralExpr : Expr
{
    public object Value { get; set; }

    public StringLiteralExpr(object value)
    {
        Type = ExprType.StringLiteral;
        Value = value;
    }

    public override string ToString() => Value.ToString();
}

public class WhiteSpaceExpr : Expr 
{
    public WhiteSpaceExpr() 
    {
        Type = ExprType.WhiteSpace;
    }
}

public class BooleanLiteralExpr : Expr
{
    public object Value { get; set; }

    public BooleanLiteralExpr(object value)
    {
        Type = ExprType.BooleanLiteral;
        Value = value;
    }

    public override string ToString() => Value.ToString();
}

public class NumericExpr : Expr
{
    public double Value { get; set; }

    public NumericExpr(double value)
    {
        Type = ExprType.Numeric;
        Value = value;
    }

    public override string ToString() => Value.ToString();
}

public class IdentifierExpr : Expr
{
    public string Name { get; set; }

    public IdentifierExpr(string name)
    {
        Type = ExprType.Identifier;
        Name = name;
    }

    public override string ToString() => Name;
}

public class VariableDeclarationExpr : Expr 
{
    public string Declaration { get; set; }

    public string Identifier { get; set; }

    public object Value { get; set; }

    public VariableDeclarationExpr(string declaration, string identifer, object value) 
    {
        Type = ExprType.VariableDeclaration;
        Declaration = declaration;
        Identifier = identifer;
        Value = value;
    }
}

public class MethodCallExpr : Expr 
{
    public string Identifier { get; set; }
    public List<Token> Parameters { get; set; }

    public MethodCallExpr(string identifer, List<Token> parameters)
    {
        Type = ExprType.MethodCall;
        Identifier = identifer;
        Parameters = parameters;
    }

    public override string ToString() => $"Method: {Identifier} || Params: {Parameters.Count}\n";
}


public class UnknownExpr : Expr
{
    public UnknownExpr()
    {
        Type = ExprType.Unknown;
    }

    public override string ToString() => "Unknown Expression";
}
