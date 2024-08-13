namespace judas;

public class Interpreter(Program program)
{
    private Program Program { get; set; } = program;

    public ValueType Interpret() {
        ValueType last = new UndefinedType();

        var env = new Environment();

        foreach (var expr in Program.Body) {
            last = InterpretExpression(expr, env);
        }

        return last;
    }

    private ValueType InterpretExpression(Expression expr, Environment env) {
        return expr.Kind switch {
            ExprType.NumericExpr => new NumericType(((NumericExpression)expr).Value),
            ExprType.BinaryExpr => InterpretBinaryExpr((BinaryExpression)expr, env),
            ExprType.IdentifierExpr => InterpretIdentifier((IdentifierExpression)expr, env),
            ExprType.VariableDeclaratorExpr => InterpretVariableDeclaration((VariableDeclarator)expr, env),
            ExprType.AssignmentExpr => InterpretAssignment((AssignmentExpression)expr, env),
            _ => new UndefinedType(),
        };
    }

    private ValueType InterpretVariableDeclaration(VariableDeclarator expr, Environment env) {
        var variable = env.LookupVariable(expr.Identifier);

        if (variable is not null)
            throw new Exception("Attempted to redeclare already existing variable");

        if (expr.Declarator == "let" && expr.Value is UndefinedExpression) {
            throw new Exception("Cannot assign undefined to variable with declarator 'let'");
        }

        ValueType value = expr.Value is not null 
            ? InterpretExpression(expr.Value, env) 
            : new UndefinedType();

        env.DeclareVariable(expr.Identifier, value, expr.IsMutable);
        return value;
    }

    private ValueType InterpretAssignment(AssignmentExpression expr, Environment env) {
        var variable = (IdentifierExpression)expr.Assignee;

        if (env.LookupVariable(variable.Symbol) is null)
            throw new Exception("Attempted to modify or update undeclared variable");

        if (!env.LookupVariable(variable.Symbol)!.Value.Item2)
            throw new Exception("Attempted to reassign constant variable");

        var value = expr.Value is not null 
            ? InterpretExpression(expr.Value, env) 
            : new UndefinedType();

        return env.AssignVariable(variable.Symbol, value);
    }

    private ValueType InterpretIdentifier(IdentifierExpression expr, Environment env) {
        return env.LookupVariable(expr.Symbol)!.Value.Item1 ?? 
            throw new Exception("Attempted to modify or reference undefined variable");
    }

    private NumericType InterpretBinaryExpr(BinaryExpression expr, Environment env) {
        NumericType leftValue = (NumericType)InterpretExpression(expr.Left, env);
        NumericType rightValue = (NumericType)InterpretExpression(expr.Right, env);

        double left = (double)leftValue.Value!;
        double right = (double)rightValue.Value!;

        return expr.Operator.Value switch {
            "+" => new NumericType(left + right),
            "-" => new NumericType(left - right),
            "*" => new NumericType(left * right),
            "/" => new NumericType(left / right),
            "%" => new NumericType(left % right),
            "**" => new NumericType(Math.Pow(left, right)),
            _ => throw new Exception("Unexpected token found while parsing binary expression"),
        };
    }
}