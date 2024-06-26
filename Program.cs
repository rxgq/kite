using judas_script.src;

namespace judas_script;

internal class Program
{
    static void Main()
    {
        var code = File.ReadAllText("C:\\Users\\adunderdale\\test.txt");

        var lexer = new Lexer(code);
        var tokens = lexer.Tokenize();

        foreach (var token in tokens)
            Console.WriteLine(token.ToString());

        var parser = new Parser(tokens);
        List<Expr> expressions = parser.Parse();

        var interpreter = new Interpreter(expressions);
        var result = interpreter.Interpret();

        Console.Write(result);
    }
}
