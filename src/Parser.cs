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
        return ParseAdditive();
    }

    private Expression ParseAdditive () {
        var left = ParseMultiplicative();

        while (Match("+") || Match("-")) {
            var op = Consume();
            var right = ParseMultiplicative();

            left = new BinaryExpression(left, right, op);
        }

        return left;
    }

    private Expression ParseMultiplicative() {
        var left = ParsePrimary();

        while (Match("*") || Match("/") || Match("%")) {
            var op = Consume();
            var right = ParsePrimary();

            left = new BinaryExpression(left, right, op);
        }

        return left;
    }

    private Expression ParsePrimary() {
        var token = Tokens[Current];

        switch (token.Type) {
            case TokenType.Identifier:
                return new IdentifierExpression(token.Value);
            
            case TokenType.Number:
                if (!float.TryParse(token.Value, out float flt))
                    throw new Exception("Attempted to parse non-float token as float");

                Advance();
                return new NumericExpression(flt);

            case TokenType.Undefined:
                return new UndefinedExpression(Tokens[Current].Value);

            case TokenType.LeftParen:
                Advance();
                var val = ParseExpression();
                Advance();
                return val;

            default: throw new Exception("Unkown Token");
        }
    }

    private bool Match(string symbol)
        => !IsEof() && Tokens[Current].Value == symbol;

    private Token Consume() {
        return Tokens[Current++];
    }

    private void Advance() => Current++; 

    private bool IsEof()
        => Current >= Tokens.Count || Tokens[Current].Type == TokenType.Eof;
        
    public void Print() {
        Console.WriteLine();
        foreach (var expr in Program.Body)
            Console.WriteLine(expr.ToString());
    }
}