using System.Text;

namespace Kite;

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

    private ValueType InterpretExpression(Expression expr, Environment env) => expr.Kind switch {
        ExprType.NumericExpr => new NumericType(((NumericExpression)expr).Value),
        ExprType.BoolExpr => new BoolType(((BooleanExpression)expr).Value ? "true" : "false"),
        ExprType.StringExpr => new StringType(((StringExpression)expr).Value),
        ExprType.HaltExpr => new SpecialType("halt"),
        ExprType.SkipExpr => new SpecialType("skip"),
        ExprType.BinaryExpr => InterpretBinaryExpr((BinaryExpression)expr, env),
        ExprType.UnaryExpr => InterpretUnaryExpression((UnaryExpression)expr, env),
        ExprType.RelationalExpr => InterpretRelationalExpr((RelationalExpression)expr, env),
        ExprType.IdentifierExpr => InterpretIdentifier((IdentifierExpression)expr, env),
        ExprType.VariableDeclaratorExpr => InterpretVariableDeclaration((VariableDeclarator)expr, env),
        ExprType.AssignmentExpr => InterpretAssignment((AssignmentExpression)expr, env),
        ExprType.LogicalExpr => InterpretLogicalExpr((LogicalExpression)expr, env),
        ExprType.IfStatementExpr => InterpretIfStatement((IfStatement)expr, env),
        ExprType.WhileStatementExpr => InterpretWhileStatement((WhileStatement)expr, env),
        ExprType.FunctionDeclarationExpr => InterpretFunctionDeclaration((FunctionDeclaration)expr, env),
        ExprType.FunctionCallExpr => InterpretFunctionCall((FunctionCall)expr, env),
        ExprType.EchoExpr => InterpretEcho((EchoStatement)expr, env),
        _ => new UndefinedType(),
    };

    private ValueType InterpretEcho(EchoStatement expr, Environment env) {
        var output = new StringBuilder();

        foreach (var valueExpr in expr.Values) {
            var value = InterpretExpression(valueExpr, env);

            var stringValue = value switch {
                NumericType numeric => numeric.Value.ToString(),
                BoolType boolean => boolean.Value.ToString(),
                StringType str => str.Value,
                UndefinedType _ => "undefined",
                _ => throw new Exception($"Unsupported type '{value.GetType()}' for echo statement")
            };

            output.Append(stringValue);
        }

        Console.WriteLine(output.ToString());
        return new StringType(output.ToString());
    }

    private ValueType InterpretFunctionDeclaration(FunctionDeclaration expr, Environment env) {
        var function = new FunctionType(expr.Identifier, expr.Args, expr.Body);
        env.DeclareVariable(expr.Identifier, function, isMutable: false);
        return function;
    }

    private ValueType InterpretFunctionCall(FunctionCall expr, Environment env) {
        if (env.LookupVariable(expr.Identifier)?.Item1 is not FunctionType function)
            throw new Exception($"Function '{expr.Identifier}' is not defined.");

        var functionEnv = new Environment(env);

        for (int i = 0; i < function.Args.Count; i++) {
            var argValue = InterpretExpression(expr.Args[i], env);
            functionEnv.DeclareVariable(function.Args[i], argValue, isMutable: false);
        }

        ValueType result = new UndefinedType();
        foreach (var statement in function.Body.Body) {
            result = InterpretExpression(statement, functionEnv);
        }

        return result;
    }

    private ValueType InterpretWhileStatement(WhileStatement whileStmt, Environment env) {
        var conditionValue = InterpretExpression(whileStmt.Condition, env);

        if (conditionValue is not BoolType boolCondition)
            throw new Exception("While statement condition must evaluate to a boolean");

        while ((bool)boolCondition!.Value!) {
            foreach (var statement in whileStmt.Consequent!.Body) {
                var expr = InterpretExpression(statement, env);
            
                if (expr is SpecialType special) {
                    if (special.Value == "halt") return expr;
                    if (special.Value == "skip") break;
                }
            }

            boolCondition = InterpretExpression(whileStmt.Condition, env) as BoolType;
        }

        return conditionValue;
    }

    private ValueType InterpretIfStatement(IfStatement ifStmt, Environment env) {
        var conditionValue = InterpretExpression(ifStmt.Condition, env);

        if (conditionValue is not BoolType boolCondition)
            throw new Exception("If statement condition must evaluate to a boolean");

        if ((bool)boolCondition.Value!) {
            foreach (var statement in ifStmt.Consequent!.Body) {
                InterpretExpression(statement, env);
            }
            return conditionValue;
        }

        if (ifStmt.Alternate != null) {
            return InterpretIfStatement(ifStmt.Alternate, env);
        }

        return conditionValue;
    }

    private ValueType InterpretVariableDeclaration(VariableDeclarator expr, Environment env) {
        var variable = env.LookupVariable(expr.Identifier);

        if (variable != null) {
            if (!variable.Value.Item2)
                throw new Exception($"Attempted to reassign constant variable '{expr.Identifier}'");

            ValueType value = expr.Value is not null
                ? InterpretExpression(expr.Value, env)
                : new UndefinedType();

            return env.AssignVariable(expr.Identifier, value);
        }

        if (expr.Declarator == "let" && expr.Value is UndefExpression) {
            throw new Exception("Cannot assign undefined to variable with declarator 'let'");
        }

        ValueType initialValue = expr.Value is not null 
            ? InterpretExpression(expr.Value, env) 
            : new UndefinedType();

        env.DeclareVariable(expr.Identifier, initialValue, expr.IsMutable);
        return initialValue;
    }


    private ValueType InterpretAssignment(AssignmentExpression expr, Environment env) {
        var variable = (IdentifierExpression)expr.Assignee;

        if (env.LookupVariable(variable.Symbol) is null)
            throw new Exception($"Attempted to modify or update undeclared variable {variable.Symbol}");

        var isMutable = env.LookupVariable(variable.Symbol)!.Value.Item2;
        if (!isMutable)
            throw new Exception("Attempted to reassign constant variable");

        var value = expr.Value is not null 
            ? InterpretExpression(expr.Value, env) 
            : new UndefinedType();

        return env.AssignVariable(variable.Symbol, value);
    }

    private ValueType InterpretIdentifier(IdentifierExpression expr, Environment env) {
        var variable = env.LookupVariable(expr.Symbol);
        
        if (variable is null) throw new Exception($"Attempted to modify or reference undefined variable {expr.Symbol}");
        return variable.Value.Item1;
    }

    private ValueType InterpretUnaryExpression(UnaryExpression expr, Environment env) {
        var rightValue = InterpretExpression(expr.Right, env);

        if (rightValue is NumericType numericValue) {
            double right = (double)numericValue.Value!;

            return expr.Operator.Value switch {
                "-" => new NumericType(-right),
                "~" => new NumericType(~(int)right),
                "++" => new NumericType(right + 1),
                "--" => new NumericType(right - 1),
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