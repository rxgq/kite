namespace judas;

public class Interpreter(Program program)
{
    public Program Program { get; set; } = program;

    public ValueType Interpret() {
        ValueType last = new UndefinedType();

        var env = new Environment();

        foreach (var expr in Program.Body) {
            last = InterpretExpression(expr, env);
        }

        return last;
    }

    public ValueType InterpretExpression(Expression expr, Environment env) {
        return expr.Kind switch {
            ExprType.NumericExpr => new NumericType(((NumericExpression)expr).Value),
            ExprType.BinaryExpr => InterpretBinaryExpr((BinaryExpression)expr, env),
            ExprType.IdentifierExpr => InterpretIdentifier((IdentifierExpression)expr, env),
            ExprType.VariableDeclaratorExpr => InterpretVariableDeclaration((VariableDeclarator)expr, env),
            _ => new UndefinedType(),
        };
    }

    public ValueType InterpretVariableDeclaration(VariableDeclarator expr, Environment env) {
        if (env.LookupVariable(expr.Identifier) is not null)
            throw new Exception("Attempted to redeclare already existing variable");

        ValueType value = expr.Value is not null 
            ? InterpretExpression(expr.Value, env) 
            : new UndefinedType();

        env.DeclareVariable(expr.Identifier, value, expr.IsMutable);
        return value;
    }

    public ValueType InterpretIdentifier(IdentifierExpression expr, Environment env) {
        return env.LookupVariable(expr.Symbol) ?? throw new Exception("Attempted to modify or reference undefined variable");
    }

    public NumericType InterpretBinaryExpr(BinaryExpression expr, Environment env) {
        NumericType leftValue = (NumericType)InterpretExpression(expr.Left, env);
        NumericType rightValue = (NumericType)InterpretExpression(expr.Right, env);

        float left = (float)leftValue.Value;
        float right = (float)rightValue.Value;

        return expr.Operator.Value switch {
            "+" => new NumericType(left + right),
            "-" => new NumericType(left - right),
            "*" => new NumericType(left * right),
            "/" => new NumericType(left / right),
            "%" => new NumericType(left % right),
            _ => throw new Exception("Unexpected token found while parsing binary expression"),
        };
    }
}