namespace judas_script;

internal class Program
{
    static void Main(string[] args)
    {
        var code = File.ReadAllText("C:\\Users\\adunderdale\\test.txt");

        var lexer = new Lexer(code);
        var tokens = lexer.Tokenize();

/*        foreach (var token in tokens) 
            Console.WriteLine(token.ToString());*/

        var parser = new Parser(tokens);
        var stmt = parser.Parse();

        Console.Write(stmt.ToString());
    }
}
