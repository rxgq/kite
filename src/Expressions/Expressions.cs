namespace judas;

public enum ExprType {
    IfStatementExpr,
    BlockStatementExpr,
    VariableDeclaratorExpr,
    AssignmentExpr,
    BinaryExpr,
    LogicalExpr,
    RelationalExpr,
    UnaryExpr,
    IdentifierExpr,
    UndefinedExpr,
    NumericExpr,
    StringExpr,
    BoolExpr,

    EchoExpr
}

public class Program {
    public List<Expression> Body { get; set; } = [];
}

public abstract class Expression(ExprType kind) {
    public ExprType Kind { get; set; } = kind;
}

public class EchoStatement(Expression expr) : Expression(ExprType.EchoExpr) {
    public Expression Value { get; set; } = expr;

    public override string ToString()
        => $"[echo {Value.ToString()}]";
}

public class IfStatement(Expression condition, BlockStatement? consequent, IfStatement? alternate = null) : Expression(ExprType.IfStatementExpr) {
    public Expression Condition { get; set; } = condition;
    public BlockStatement? Consequent { get; set; } = consequent;
    public IfStatement? Alternate { get; set; } = alternate;

    public override string ToString()
        => $"[if {Condition} {Consequent} | {Alternate}]";
}

public class BlockStatement(List<Expression> body) : Expression(ExprType.BlockStatementExpr) {
    public List<Expression> Body { get; set; } = body;

    public override string ToString()
        => $"{{\n{string.Join("\n", Body)}\n}}";
}

public class VariableDeclarator(string declarator, string identifier, Expression? value, bool isMutable = false) : Expression(ExprType.VariableDeclaratorExpr) {
    public string Declarator { get; set; } = declarator;
    public bool IsMutable { get; set; } = isMutable;
    public string Identifier { get; set; } = identifier;
    public Expression? Value { get; set; } = value;

    public override string ToString()
        => $"[{Declarator} {Identifier}: {Value}]";
}

public class AssignmentExpression(Expression assignee, Expression value) : Expression(ExprType.AssignmentExpr) {
    public Expression Assignee { get; set; } = assignee;
    public Expression Value { get; set; } = value;

    public override string ToString()
        => $"[{Assignee} = {Value}]";
}

public class BinaryExpression (Expression left, Expression right, Token op) : Expression(ExprType.BinaryExpr) {
    public Expression Left { get; set; } = left;
    public Expression Right { get; set; } = right;
    public Token Operator { get; set; } = op;

    public override string ToString()
        => $"BIN: ({Left} {Operator.Value} {Right})";
}

public class LogicalExpression (Expression left, Expression right, Token op) : Expression(ExprType.LogicalExpr) {
    public Expression Left { get; set; } = left;
    public Expression Right { get; set; } = right;
    public Token Operator { get; set; } = op;

    public override string ToString()
        => $"LOG: ({Left} {Operator} {Right})";
}

public class RelationalExpression (Expression left, Expression right, Token op) : Expression(ExprType.RelationalExpr) {
    public Expression Left { get; set; } = left;
    public Expression Right { get; set; } = right;
    public Token Operator { get; set; } = op;

    public override string ToString()
        => $"REL: ({Left} {Operator} {Right})";
}

public class UnaryExpression (Token op, Expression right) : Expression(ExprType.UnaryExpr) {
    public Token Operator { get; set; } = op;
    public Expression Right { get; set; } = right;

    public override string ToString()
        => $"({Operator}{Right})";
}

public class IdentifierExpression (string symbol) : Expression(ExprType.IdentifierExpr) {
    public string Symbol { get; set; } = symbol;

    public override string ToString()
        => $"[{Symbol}]";
}

public class UndefinedExpression(string symbol) : Expression(ExprType.UndefinedExpr) {
    public string Symbol { get; set; } = symbol;

    public override string ToString()
        => $"[Undefined]";
}

public class NumericExpression(float value) : Expression(ExprType.NumericExpr) {
    public float Value { get; set; } = value;

    public override string ToString()
        => Value.ToString();
}

public class StringExpression(string value) : Expression(ExprType.StringExpr) {
    public string Value { get; set; } = value;
    
    public override string ToString()
        => $"String: [{Value}]";
}

public class BooleanExpression(bool value) : Expression(ExprType.BoolExpr) {
    public bool Value { get; set; } = value;

    public override string ToString()
        => Value.ToString();
}