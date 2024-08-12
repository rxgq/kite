namespace judas;

internal abstract class Expression {}

internal class BinaryExpression(Expression left, Expression right, TokenType op) : Expression {
    public Expression Left { get; set; } = left;
    public Expression Right { get; set; } = right;
    public TokenType Operator { get; set; } = op;
}

internal class LogicalExpression (Expression left, Expression right, TokenType op) : Expression {
    public Expression Left { get; set; } = left;
    public Expression Right { get; set; } = right;
    public TokenType Operator { get; set; } = op;
}

internal class Identifier : Expression {
    public string Symbol { get; set; }
}

internal class NumericLiteral : Expression {
    public int Value { get; set; }
}

internal class StringLiteral : Expression {
    public int Value { get; set; }
}