using judas_script.src.Libraries;

namespace Judas;

internal class Interpreter
{
    List<Expr> Expressions { get; set; }
    public static Dictionary<string, object> Variables = new();

    public Interpreter(List<Expr> expressions)
    {
        Expressions = expressions;
    }

    public object Interpret()
    {
        object? result = null;

        foreach (var expr in Expressions)
            result = Evaluate(expr);

        return result;
    }

    public object Evaluate(Expr expr)
    {
        return expr.Type switch
        {
            ExprType.Binary => BinaryExpr((BinaryExpr)expr),
            ExprType.Unary => UnaryExpr((UnaryExpr)expr),
            ExprType.StringLiteral => ((StringLiteralExpr)expr).Value,
            ExprType.Numeric => ((NumericExpr)expr).Value,
            ExprType.BooleanLiteral => ((BooleanLiteralExpr)expr).Value,
            ExprType.MethodCall => MethodExpr((MethodCallExpr)expr),
            ExprType.VariableDeclaration => VariableDeclarationExpr((VariableDeclarationExpr)expr),
            ExprType.Assignment => AssignmentExpr((AssignmentExpr)expr),

            _ => ExprType.Unknown
        };
    }

    public static object AssignmentExpr(AssignmentExpr expr) 
    {
        if (expr.Assigner is IdentifierExpr identExpr) 
            Variables[expr.Assignee] = Variables[identExpr.Name];

        else if (expr.Assigner is NumericExpr numericExpr)
            Variables[expr.Assignee] = numericExpr.Value;

        return null;
    }



    public static object VariableDeclarationExpr(VariableDeclarationExpr expr)
    {
        var value = "";
        if (expr.Value is StringLiteralExpr stringExpr) 
            value = ProcessString(expr.Value.ToString());

        Variables.Add(expr.Identifier, value);
        return null;
    }

    private static string ProcessString(string value)
    {
        var parts = new List<string>();
        var literalParts = value.Split(new[] { '{', '}' }, StringSplitOptions.None);

        bool isExpression = false;
        foreach (var part in literalParts)
        {
            if (isExpression)
            {
                if (Variables.TryGetValue(part, out var variableValue))
                    parts.Add(variableValue.ToString());

                else
                    throw new Exception($"Variable {part} not found.");
            }
            else
                parts.Add(part);

            isExpression = !isExpression;
        }

        return string.Join(string.Empty, parts);
    }


    public static object MethodExpr(MethodCallExpr expr) 
    {
        switch (expr.Identifier) 
        {
            case "echo":
                if (expr.Parameters.Count == 1)
                    Standard.OnEcho(expr);
                else if (expr.Parameters.Count == 2)
                    Standard.OnEchoCount(expr);
                else 
                    throw new JudasException.NoValidOverloadException("echo", expr.Parameters.Count);

                return null;

            case "exit":
                if (expr.Parameters.Count != 0)
                    throw new JudasException.NoValidOverloadException("exit", expr.Parameters.Count);

                Standard.OnExit(expr);
                return null;
            
            default:
                throw new Exception("Invalid method");
        }
    }

    public object BinaryExpr(BinaryExpr expr)
    {
        var left = Evaluate(expr.Left);
        var right = Evaluate(expr.Right);

        switch (expr.Operator)
        {
            case "+":
                if (left is double pl && right is double pr)
                    return pl + pr;
                throw new Exception("Invalid operands for +");

            case "-":
                if (left is double sl && right is double sr)
                    return sl - sr;
                throw new Exception("Invalid operands for -");

            case "*":
                if (left is double tl && right is double tp)
                    return tl * tp;
                throw new Exception("Invalid operands for *");

            case "/":
                if (left is double dl && right is double dr)
                    return dl / dr;
                throw new Exception("Invalid operands for /");

            case "%":
                if (left is double ml && right is double mr)
                    return ml % mr;
                throw new Exception("Invalid operands for %");

            case "+=":
                if (left is double ql && right is double qr)
                    return ql += qr;
                throw new Exception("Invalid operands for %");

            case "==":
            case "!=":
                if (left.GetType() != right.GetType())
                    throw new Exception("Invalid operands for ==");

                if (left is bool bl && right is bool br)
                    return expr.Operator == "==" ? bl == br : bl != br;

                if (left.GetType() == typeof(string) || left.GetType().IsValueType)
                    return expr.Operator == "==" ? left.Equals(right) : !left.Equals(right);

                return left == right;

            case ">":
            case "<":
                if (left is double gl && right is double gr)
                    return expr.Operator == ">" ? gl > gr : gl < gr;
                throw new Exception("Invalid operands for >");

            case ">=":
            case "<=":
                if (left is double el && right is double er)
                    return expr.Operator == ">=" ? (el > er || el == er) : (el < er || el == er);
                throw new Exception("Invalid operands for >");

            case "and":
                if (left is bool al && right is bool ar)
                    return al && ar;
                throw new Exception("Invalid operands for and");

            case "or":
                if (left is bool ol && right is bool or)
                    return ol || or;
                throw new Exception("Invalid operands for or");

            case "xor":
                if (left is bool xorl && right is bool xorr)
                    return xorl ^ xorr;
                throw new Exception("Invalid operands for xor");

            case "nand":
                if (left is bool nal && right is bool nar)
                    return !(nal && nar);
                throw new Exception("Invalid operands for nand");

            case "nor":
                if (left is bool norl && right is bool norr)
                    return !(norl || norr);
                throw new Exception("Invalid operands for nor");

            case "xnor":
                if (left is bool xnorl && right is bool xnorr)
                    return !(xnorl ^ xnorr);
                throw new Exception("Invalid operands for xnor");

            default:
                throw new Exception("Binary Expression parsing exception");
        }
    }

    public object UnaryExpr(UnaryExpr expr)
    {
        var value = Evaluate(expr.Value);

        switch (expr.Operator)
        {
            case "++":
                if (value is double il)
                    return il + 1;

                throw new Exception("Invalid operand type for ++");
            case "--":
                if (value is double dl)
                    return dl - 1;
                throw new Exception("Invalid operand type for --");

            case "**":
                if (value is double el)
                    return Math.Pow(el, 2);
                throw new Exception("Invalid operand type for **");

            default:
                throw new Exception("Unsupported unary operator: " + expr.Operator);
        }
    }
}
