using System.Linq.Expressions;

namespace judas_script;

internal class Program
{
    static void Main(string[] args)
    {
        var lines = File.ReadAllText("C:\\Users\\adunderdale\\test.txt");

        var lexer = new Lexer(lines);
        var tokens = lexer.Tokenize();

        foreach (var token in tokens) 
            Console.Write(token.ToString());

        // parser
    }
}
