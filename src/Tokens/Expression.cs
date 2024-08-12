namespace judas;

internal abstract class Expression {}

internal class BinaryExpression (Expression left, Expression right, TokenType op) : Expression {
    public Expression Left { get; set; } = left;
    public Expression Right { get; set; } = right;
    public TokenType Operator { get; set; } = op;

    public override string ToString()
        => $"[{Left} {Operator} {Right}]";
}

internal class LogicalExpression (Expression left, Expression right, TokenType op) : Expression {
    public Expression Left { get; set; } = left;
    public Expression Right { get; set; } = right;
    public TokenType Operator { get; set; } = op;

    public override string ToString()
        => $"[{Left} {Operator} {Right}]";
}

internal class IdentifierExpression (string symbol) : Expression {
    public string Symbol { get; set; } = symbol;

    public override string ToString()
        => $"Identifier: [{Symbol}]";
}


internal class NumericLiteral(float value) : Expression {
    public float Value { get; set; } = value;

    public override string ToString()
        => $"Numeric: [{Value}]";
}

internal class StringLiteral(string value) : Expression {
    public string Value { get; set; } = value;
    
    public override string ToString()
        => $"String: [{Value}]";
}