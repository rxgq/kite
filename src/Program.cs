namespace judas;

internal abstract class Judas {
    static void Main() {
        var path = "example\\code.txt";
        var source = File.ReadAllText(path);

        var lexer = new Lexer(source);
        var tokens = lexer.Tokenize();

        lexer.Print();

        var parser = new Parser(tokens);
        parser.Parse();

    }
}