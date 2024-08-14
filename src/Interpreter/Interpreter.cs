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
            ExprType.BoolExpr => new BoolType(((BooleanExpression)expr).Value ? "true" : "false"),
            ExprType.BinaryExpr => InterpretBinaryExpr((BinaryExpression)expr, env),
            ExprType.UnaryExpr => InterpretUnaryExpression((UnaryExpression)expr, env),
            ExprType.RelationalExpr => InterpretRelationalExpr((RelationalExpression)expr, env),
            ExprType.IdentifierExpr => InterpretIdentifier((IdentifierExpression)expr, env),
            ExprType.VariableDeclaratorExpr => InterpretVariableDeclaration((VariableDeclarator)expr, env),
            ExprType.AssignmentExpr => InterpretAssignment((AssignmentExpression)expr, env),
            ExprType.LogicalExpr => InterpretLogicalExpr((LogicalExpression)expr, env),
            //ExprType.IfStatementExpr => InterpretIfStatement((IfStatement)expr, env),
            _ => new UndefinedType(),
        };
    }

    // private ValueType InterpretIfStatement(IfStatement ifStmt, Environment env) {

    // }

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

        var isMutable = env.LookupVariable(variable.Symbol)!.Value.Item2;
        if (!isMutable)
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

    private ValueType InterpretUnaryExpression(UnaryExpression expr, Environment env) {
        var rightValue = InterpretExpression(expr.Right, env);

        if (rightValue is NumericType numericValue) {
            double right = (double)numericValue.Value!;

            return expr.Operator.Value switch {
                "-" => new NumericType(-right),
                "~" => new NumericType(~(int)right),
                "++" => new NumericType(++right),
                "--" => new NumericType(--right),
                _ => throw new Exception($"Unexpected numeric unary operator '{expr.Operator.Value}'"),
            };
        }

        if (rightValue is BoolType boolValue) {
            bool right = (bool)boolValue.Value!;

            return expr.Operator.Value switch {
                "not" => new BoolType(!right ? "true" : "false"),
                _ => throw new Exception($"Unexpected boolean unary operator '{expr.Operator.Value}'"),
            };
        }

        throw new Exception($"Unsupported type '{rightValue.GetType()}' for unary operation");
    }


    private BoolType InterpretLogicalExpr(LogicalExpression expr, Environment env) {
        BoolType leftValue = (BoolType)InterpretExpression(expr.Left, env);
        BoolType rightValue = (BoolType)InterpretExpression(expr.Right, env);

        bool left = (bool)leftValue.Value!;
        bool right = (bool)rightValue.Value!;

        return expr.Operator.Value switch {
            "and" => new(left && right ? "true" : "false"),
            "or" => new(left || right ? "true" : "false"),
            _ => throw new Exception($"Unexpected token found while interpreting logical expression '{expr.Operator.Value}'"),
        };
    }

    private BoolType InterpretRelationalExpr(RelationalExpression expr, Environment env) {
        NumericType leftValue = (NumericType)InterpretExpression(expr.Left, env);
        NumericType rightValue = (NumericType)InterpretExpression(expr.Right, env);

        double left = (double)leftValue.Value!;
        double right = (double)rightValue.Value!;

        return expr.Operator.Value switch {
            ">" => new(left > right ? "true" : "false"),
            "<" => new(left < right ? "true" : "false"),
            "<=" => new(left <= right ? "true" : "false"),
            ">=" => new(left <= right ? "true" : "false"),
            "!=" => new(left != right ? "true" : "false"),
            "==" => new(left == right ? "true" : "false"),
            _ => throw new Exception($"Unexpected token found while interpreting relational expression '{expr.Operator.Value}'"),
        };
    }

    private NumericType InterpretBinaryExpr(BinaryExpression expr, Environment env) {
        NumericType leftValue = (NumericType)InterpretExpression(expr.Left, env);
        NumericType rightValue = (NumericType)InterpretExpression(expr.Right, env);

        double left = (double)leftValue.Value!;
        double right = (double)rightValue.Value!;

        return expr.Operator.Value switch {
            "+" => new(left + right),
            "-" => new(left - right),
            "*" => new(left * right),
            "/" => new(left / right),
            "%" => new(left % right),
            "**" => new(Math.Pow(left, right)),
            "&" => new((int)left & (int)right),
            "|" => new((int)left | (int)right),
            "^" => new((int)left ^ (int)right),
            _ => throw new Exception($"Unexpected token found while interpreting binary expression '{expr.Operator.Value}'"),
        };
    }
}