namespace judas;

internal abstract class Judas {
    static void Main(string[] args) {
        if (args.Length == 0) {
            Console.WriteLine("Usage: dotnet run -- <path_to_code>");
            return;
        }

        var path = args[0];
        if (!File.Exists(path)) {
            Console.WriteLine($"File not found: {path}");
            return;
        }

        var source = File.ReadAllText(path);

        var lexer = new Lexer(source);
        var tokens = lexer.Tokenize();
        //lexer.Print();

        var parser = new Parser(tokens);
        var expressions = parser.Parse();
        //parser.Print();

        var interpreter = new Interpreter(expressions);
        var result = interpreter.Interpret();
    }
}