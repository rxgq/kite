using judas_script.src;
using System.Linq.Expressions;

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

/*        foreach (var expression in expressions)
            Console.Write(expression.ToString());*/

        var interpreter = new Interpreter(expressions);
        var result = interpreter.Interpret();

        Console.Write($"\n{result}");
    }
}
