namespace judas_script.src;

internal class Interpreter
{
    List<Expr> Expressions { get; set; }

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

            _ => ExprType.Unknown
        };
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
