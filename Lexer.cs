namespace judas_script;

enum TokenType
{
    OPERATOR,
    STRING, NUMBER,

    WHITESPACE, NEWLINE,
    BAD, EOF,
}

internal class Token
{
    public TokenType Type;
    public object? Value;

    public Token(TokenType type, object? value)
    {
        Type = type;
        Value = value;
    }

    public override string ToString()
        => $"\nType: {Type,-16} || Value: {Value,-16}";
}

internal class Lexer
{
    public List<Token> Tokens = new();

    public readonly string Source;

    public int Start;
    public int Current = -1;

    public Lexer(string source)
    {
        Source = source;
    }

    public List<Token> Tokenize()
    {
        while (!IsEndOfFile())
        {
            Advance();
            Start = Current;

            NextToken();
        }

        Tokens.Add(new Token(TokenType.EOF, null));

        return Tokens;
    }

    private void NextToken()
    {
        switch (Source[Current])
        {
            case '+':
            case '-':
            case '*':
            case '/':
            case '%':
            case '!':
            case '=':
            case '>':
            case '<':
                OnOperator();
                break;

            case '\"':
                OnStringLiteral();
                break;

            case >= '0' and <= '9':
                OnNumber();
                break;

            case '\t':
            case '\r':
                Advance();
                break;

            case '\n':
                OnNewLine();
                break;

            case ' ':
                OnWhiteSpace();
                break;

            default:
                OnBadToken();
                break;
        }
    }

    private bool IsEndOfFile()
        => Current >= Source.Length - 1;

    private void Advance() => Current++;

    private char Peek()
        => IsEndOfFile() ? '\0' : Source[Current + 1];

    private string CurrentChars()
        => Source[Start..(Current + 1)];

    private void OnOperator()
    {
        if (Peek() == '=')
            Advance();

        Tokens.Add(new Token(TokenType.OPERATOR, CurrentChars()));
    }

    private void OnBadToken()
        => Tokens.Add(new Token(TokenType.BAD, CurrentChars()));

    private void OnWhiteSpace()
        => Tokens.Add(new Token(TokenType.WHITESPACE, CurrentChars()));

    private void OnNewLine()
        => Tokens.Add(new Token(TokenType.NEWLINE, "\\n"));

    private void OnStringLiteral() 
    {
        while (Peek() != '\"' && !IsEndOfFile()) 
            Advance();

        Advance();
        Tokens.Add(new Token(TokenType.STRING, CurrentChars()));
    }

    private void OnNumber() 
    {
        while (char.IsDigit(Peek()) && !IsEndOfFile())
            Advance();

        if (Peek() == '.' && char.IsDigit(Source[Current + 2]))
            Advance();

        while (char.IsDigit(Peek()) && !IsEndOfFile())
            Advance();

        Tokens.Add(new Token(TokenType.NUMBER, CurrentChars()));
    }
}
