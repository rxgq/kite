namespace judas_script.src;

public enum ExprType
{
    Binary, Unary, Identifier, Assignment,

    Numeric, StringLiteral, BooleanLiteral, 
    
    IfStatement, EndOfInput, Grouping, Unknown
}

public abstract class Expr
{
    public ExprType Type { get; set; }
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

    public override string ToString() => $"({Left} {Operator} {Right})\n";
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

public class UnknownExpr : Expr
{
    public UnknownExpr()
    {
        Type = ExprType.Unknown;
    }

    public override string ToString() => "Unknown Expression";
}
