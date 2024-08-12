namespace judas;

internal class Parser(List<Token> tokens) {
    public List<Token> Tokens { get; set; } = tokens;
    public Program Program { get; set; } = new();
    public int Current { get; set; } = 0;

    public Program Parse() {
        while (!IsEof()) {
            Program.Body.Add(ParseExpression());
            Advance();
        }

        return Program;
    }

    private Expression ParseExpression() {
        return ParsePrimary();
    }

    private Expression ParsePrimary() {
        var token = Tokens[Current];

        switch (token.Type) {
            case TokenType.Identifier:
                return new IdentifierExpression(token.Value);
            
            case TokenType.Number:
                if (!float.TryParse(token.Value, out float val))
                    throw new Exception("Attempted to parse non-float token as float");

                return new NumericLiteral(val);

            default: throw new Exception("Unkown Token");
        }
    }

    private void Advance() => Current++; 

    private bool IsEof()
        => Tokens[Current].Type == TokenType.Eof;

    public void Print() {
        Console.WriteLine();
        foreach (var expr in Program.Body)
            Console.WriteLine(expr.ToString());
    }
}