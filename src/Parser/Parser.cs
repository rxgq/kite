namespace judas;

internal class Parser(List<Token> tokens) {
    public List<Token> Tokens { get; set; } = tokens;
    public Program Program { get; set; } = new();
    public int Current { get; set; } = 0;

    public Program Parse() {
        while (!IsEof()) {
            Program.Body.Add(ParseStatement());
        }

        return Program;
    }

    private Expression ParseStatement() {
        return Tokens[Current].Type switch
        {
            TokenType.Let or TokenType.Mut => ParseVariableDeclaration(),
            _ => ParseExpression(),
        };
    }

    private Expression ParseVariableDeclaration() {
        var declarator = Consume();
        var identifier = ExpectToBe(TokenType.Identifier, "Expected identifier after declaration");
        var isMut = declarator.Type == TokenType.Mut;

        var next = Consume();
        if (next.Type == TokenType.Assignment) {
            var val = ParseExpression();
            ExpectToBe(TokenType.SemiColon, "Expected semi colon after variable declaration");

            return new VariableDeclarator(declarator.Value, identifier.Value, val, isMut);
        } 
        else if (next.Type == TokenType.SemiColon) {
            if (!isMut) throw new Exception("Cannot declare a constant variable as undefined");

            return new VariableDeclarator(declarator.Value, identifier.Value, null, isMut);
        }
        
        throw new Exception("Unexpected token found in variable declaration");
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
                Advance();
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

            default: throw new Exception("Unkown Token found while parsing primary expression");
        }
    }

    private Token ExpectToBe(TokenType type, string msg) {
        if (Tokens[Current].Type != type) throw new Exception(msg);
        return Consume();
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