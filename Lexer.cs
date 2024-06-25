namespace judas_script;

public enum TokenType
{
    PLUS, MINUS, STAR, SLASH, MOD,
    PLUS_EQUALS, MINUS_EQUALS, STAR_EQUALS, SLASH_EQUALS, MOD_EQUALS,
    NOT_EQUALS, EQUALS, ASSIGNMENT, NOT,

    GREATER_THAN, LESS_THAN,
    GREATER_THAN_EQUALS, LESS_THAN_EQUALS,

    STRING, NUMBER,

    IDENTIFIER, KEYWORD,
    BOOLEAN, NULL,

    METHOD,

    WHITESPACE, NEWLINE,
    BAD, EOF,
}


public sealed class Token
{
    public TokenType Type { get; }
    public object? Value { get; }
    public string Lexeme { get; }

    public Token(TokenType type, object? value, string lexeme)
    {
        Type = type;
        Value = value;
        Lexeme = lexeme;
    }

    internal static readonly Dictionary<string, TokenType> Keywords = new()
    {
        { "and", TokenType.KEYWORD },
        { "or", TokenType.KEYWORD },
        { "not", TokenType.KEYWORD },
        { "true", TokenType.BOOLEAN },
        { "false", TokenType.BOOLEAN },
        { "is", TokenType.KEYWORD },
        { "if", TokenType.KEYWORD },
        { "else if", TokenType.KEYWORD },
        { "else", TokenType.KEYWORD },
        { "null", TokenType.NULL },
        { "let", TokenType.KEYWORD },
        { "echo", TokenType.METHOD },
    };

    public override string ToString()
        => $"Type: {Type,-22} || Value: {Value,-22} || Lexeme: {Lexeme,-22}";
}

internal sealed class Lexer
{
    public List<Token> Tokens { get; } = new();

    public readonly string Source;

    public int Start { get; private set; }
    public int Current { get; private set; } = -1;

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

        Tokens.Add(new Token(TokenType.EOF, null, ""));

        return Tokens;
    }

    private void NextToken()
    {
        switch (Source[Current])
        {
            case '+':
                OnOperator(IsDoubleOp() ? TokenType.PLUS_EQUALS : TokenType.PLUS);
                break;
            case '-':
                OnOperator(IsDoubleOp() ? TokenType.MINUS_EQUALS : TokenType.MINUS);
                break;
            case '*':
                OnOperator(IsDoubleOp() ? TokenType.STAR_EQUALS : TokenType.STAR);
                break;
            case '/':
                OnOperator(IsDoubleOp() ? TokenType.SLASH_EQUALS : TokenType.SLASH);
                break;
            case '%':
                OnOperator(IsDoubleOp() ? TokenType.MOD_EQUALS : TokenType.MOD);
                break;
            case '!':
                OnOperator(IsDoubleOp() ? TokenType.NOT_EQUALS : TokenType.NOT);
                break;
            case '=':
                OnOperator(IsDoubleOp() ? TokenType.EQUALS : TokenType.ASSIGNMENT);
                break;
            case '>':
                OnOperator(IsDoubleOp() ? TokenType.GREATER_THAN_EQUALS : TokenType.GREATER_THAN);
                break;
            case '<':
                OnOperator(IsDoubleOp() ? TokenType.LESS_THAN_EQUALS : TokenType.LESS_THAN);
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
                OnIdentifier();
                break;
        }
    }

    private bool IsEndOfFile()
        => Current >= Source.Length - 1;

    private void Advance() => Current++;

    private char Peek()
        => IsEndOfFile() ? '\0' : Source[Current + 1];

    private bool IsDoubleOp()
    {
        if (IsEndOfFile())
            return false;

        if (Source[Current + 1] != '=')
            return false;

        Advance();
        return true;
    }

    private string CurrentChars()
        => Source[Start..(Current + 1)];

    private void OnOperator(TokenType type)
        => Tokens.Add(new Token(type, null, CurrentChars()));

    private void OnBadToken()
        => Tokens.Add(new Token(TokenType.BAD, null, CurrentChars()));

    private void OnWhiteSpace()
        => Tokens.Add(new Token(TokenType.WHITESPACE, null, CurrentChars()));

    private void OnNewLine()
        => Tokens.Add(new Token(TokenType.NEWLINE, "\\n", "\\n"));

    private void OnStringLiteral()
    {
        while (Peek() != '\"' && !IsEndOfFile())
            Advance();

        Advance();

        string lexeme = CurrentChars();
        string value = lexeme[1..^1];

        Tokens.Add(new Token(TokenType.STRING, value, lexeme));
    }

    private void OnNumber()
    {
        while (char.IsDigit(Peek()) && !IsEndOfFile())
            Advance();

        if (Peek() == '.' && char.IsDigit(Source[Current + 2]))
            Advance();

        while (char.IsDigit(Peek()) && !IsEndOfFile())
            Advance();

        string lexeme = CurrentChars();
        double value = double.Parse(lexeme);
        Tokens.Add(new Token(TokenType.NUMBER, value, lexeme));
    }

    private void OnIdentifier()
    {
        while (char.IsLetterOrDigit(Peek()) && !IsEndOfFile())
            Advance();

        string lexeme = CurrentChars().Trim();

        if (Token.Keywords.TryGetValue(lexeme, out TokenType tokenType))
            Tokens.Add(new Token(tokenType, lexeme, lexeme));
        else
            Tokens.Add(new Token(TokenType.IDENTIFIER, lexeme, lexeme));
    }
}
