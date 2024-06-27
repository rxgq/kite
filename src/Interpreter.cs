namespace judas_script.src;

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
        object lastResult = null;

        foreach (var expr in Expressions)
            lastResult = Evaluate(expr);

        return lastResult;
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

            _ => ExprType.Unknown
        };
    }

    public static object MethodExpr(MethodCallExpr expr) 
    {
        switch (expr.Identifier) 
        {
            case "echo": OnEcho(expr);
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


                throw new Exception("Invalid operands for and");

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

    public static void OnEcho(MethodCallExpr expr) 
    {
        if (expr.Parameters[0] is Token t) 
        {
            var text = t.Value;
            Console.WriteLine(text);
            return;
        }

        throw new Exception("Invalid parameter for echo");
    }
}
