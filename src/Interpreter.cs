namespace judas;

public class Interpreter(Program program)
{
    public Program Program { get; set; } = program;

    public ValueType Interpret() {
        ValueType last = new UndefinedType();

        foreach (var expr in Program.Body) {
            last = InterpretExpression(expr);
        }

        return last;
    }

    public ValueType InterpretExpression(Expression expr) {
        return expr.Kind switch {
            ExprType.NumericExpr => new NumericType(((NumericExpression)expr).Value),
            ExprType.BinaryExpr => InterpretBinaryExpr((BinaryExpression)expr),
            _ => new UndefinedType(),
        };
    }

    public NumericType InterpretBinaryExpr(BinaryExpression expr) {
        NumericType leftValue = (NumericType)InterpretExpression(expr.Left);
        NumericType rightValue = (NumericType)InterpretExpression(expr.Right);

        return expr.Operator.Value switch { // fix this
            "+" => new NumericType((float)leftValue.Value + (float)rightValue.Value),
            "-" => new NumericType((float)leftValue.Value - (float)rightValue.Value),
            "*" => new NumericType((float)leftValue.Value * (float)rightValue.Value),
            "/" => new NumericType((float)leftValue.Value / (float)rightValue.Value),
            "%" => new NumericType((float)leftValue.Value % (float)rightValue.Value),
            _ => throw new Exception("Unexpected token found while parsing binary expression"),
        };
    }
}