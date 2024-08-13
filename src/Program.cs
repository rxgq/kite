namespace judas;

internal abstract class Judas {
    static void Main() {
        var path = "example\\code.txt";
        var source = File.ReadAllText(path);

        var lexer = new Lexer(source);
        var tokens = lexer.Tokenize();
        lexer.Print();

        var parser = new Parser(tokens);
        var expressions = parser.Parse();
        parser.Print();

        var interpreter = new Interpreter(expressions);
        var result = interpreter.Interpret();

        Console.Write($"RESULT: {result.Value}");
    }
}