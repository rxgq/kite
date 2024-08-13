namespace judas;

internal class Lexer(string source)
{
    public string Source { get; set; } = source;
    public int Current { get; set; } = 0;
    public List<Token> Tokens { get; set; } = [];

    public List<Token> Tokenize() {
        while (!IsEof()) {
            if (IsWhiteSpace()) continue;

            Tokens.Add(NextToken());
            Advance();
        }

        Tokens.Add(new(TokenType.Eof, ""));
        return Tokens;
    }

    private Token NextToken() {
        char c = Source[Current];
        
        return Source[Current] switch {
            '(' => new(TokenType.LeftParen, c),
            ')' => new(TokenType.RightParen, c),
            ',' => new(TokenType.Comma, c),
            ';' => new(TokenType.SemiColon, c),

            '+' or '-' or '/' or '%' => new(TokenType.BinaryOp, c),
            '*' => OnStar(),
            '=' => OnEquals(),

            _ when char.IsDigit(c) => OnNumber(),
            _ when char.IsLetter(c) => OnLetter(),
            
            ' ' => new(TokenType.Space, Source[Current]),
            _ => new(TokenType.Bad, c)
        };
    }

    private Token OnStar() {
        if (Peek() == '*') {
            Advance();
            return new(TokenType.BinaryOp, "**");
        }

        return new(TokenType.BinaryOp, '*');
    }

    private Token OnEquals() {
        if (Peek() == '=') {
            Advance();
            return new(TokenType.Equality, "==");
        }

        return new(TokenType.Assignment, '=');
    }

    private Token OnNumber() {
        int start = Current;
        while (!IsEof() && char.IsDigit(Source[Current])) {
            Advance();
        }

        Current--;
        return new(TokenType.Number, Source[start..(Current + 1)]);
    }

    private Token OnLetter() {
        int start = Current;
        while (!IsEof() && char.IsLetter(Source[Current])) {
            Advance();
        }

        Current--;
        var lexeme = Source[start..(Current + 1)];

        if (Token.Keywords.TryGetValue(lexeme, out TokenType type))
            return new(type, lexeme);

        return new(TokenType.Identifier, lexeme);
    }

    private bool IsWhiteSpace() {
        if (char.IsWhiteSpace(Source[Current])) {
            Advance();
            return true;
        }
        
        return false;
    }

    private void Advance() => Current++;

    private char Peek() 
        => Current + 1 < Source.Length ? Source[Current + 1] : '\0';

    private bool IsEof() => Current > Source.Length - 1;

    public void Print() {
        foreach (var token in Tokens) 
            Console.WriteLine(token.ToString());
    }
}