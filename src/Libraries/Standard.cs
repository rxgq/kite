using Judas;

namespace judas_script.src.Libraries;

internal class Standard
{
    public static void OnEcho(MethodCallExpr expr)
    {
        if (expr.Parameters[0].Type == TokenType.IDENTIFIER)
        {
            var text = Interpreter.Variables[expr.Parameters[0].Lexeme];
            Console.WriteLine(text);
            return;
        }

        if (expr.Parameters[0] is Token t)
        {
            var text = t.Value;
            Console.WriteLine(text);
            return;
        }

        throw new Exception("Invalid parameter for echo");
    }

    public static void OnEchoCount(MethodCallExpr expr)
    {
        var value = expr.Parameters[1].Value;

        if (value is double count)
            for (int i = 0; i < count; i++)
                OnEcho(expr);

        return;
    }

    public static void OnExit(MethodCallExpr expr)
    {
        if (expr.Identifier != "exit")
            throw new JudasException.BadException();

        Console.Write("Judas Interpreter Exited");
        Environment.Exit(0);
    }
}
