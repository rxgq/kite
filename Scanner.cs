namespace judas_script;

public enum TokenType
{
    PLUS, PLUS_EQUALS,
    MINUS, MINUS_EQUALS,
    STAR, STAR_EQUALS,
    SLASH, SLASH_EQUALS,

    LESS_EQUALS, GREATER_EQUALS,
    LESS_THAN, GREATER_THAN,
    EQUALS, DOUBLE_EQUALS,
    NOT_EQUALS, IDENTIFIER,

    STRING_LITERAL, NUMBER,

    IF, FOR, TRUE, FALSE, NULL, PRINT,

    VAR,

    BAD,
    WHITESPACE,
    NEWLINE,
    EOF
}

internal class Scanner
{
    private readonly List<Token> Tokens = new();
    private readonly string Source;

    public int Start;
    public int End;

    public int Current;
    public int Line = 1;

    public Scanner(string source) 
    {
        Source = source;
    }

    public List<Token> Scan() 
    {
        while (!IsEndOfFile()) 
        {
            Start = Current;
            ScanToken();
        }

        AddToken(TokenType.EOF, "");

        return Tokens;
    }

    public void ScanToken()
    {
        if (IsEndOfFile())
            return;

        char c = Source[Current];

        switch (c)
        {
            case '+': AddToken(Match('=') ? TokenType.PLUS_EQUALS : TokenType.PLUS);
                break;

            case '-': AddToken(Match('=') ? TokenType.MINUS_EQUALS : TokenType.MINUS);
                break;

            case '*': AddToken(Match('=') ? TokenType.STAR_EQUALS : TokenType.PLUS);
                break;

            case '/':
                if (Match('='))
                    AddToken(TokenType.SLASH_EQUALS);

                else if (Match('/')) 
                    while (Peek() != '\n' && !IsEndOfFile()) 
                        Advance();
                else
                    AddToken(TokenType.SLASH);

                break;

            case '<': AddToken(Match('=') ? TokenType.LESS_EQUALS : TokenType.LESS_THAN);
                break;

            case '>': AddToken(Match('=') ? TokenType.GREATER_EQUALS : TokenType.GREATER_THAN);
                break;

            case '=': AddToken(Match('=') ? TokenType.DOUBLE_EQUALS : TokenType.EQUALS);
                break;

            case '!': AddToken(Match('=') ? TokenType.NOT_EQUALS : TokenType.BAD);
                break;


            case '"':
                while (!Match('"')) 
                    Advance();

                string value = Source.Substring(Start + 1, Current - 1);
                AddToken(TokenType.STRING_LITERAL, value);
                break;

            case 'v':
                if (Current + 3 < Source.Length && Source.Substring(Current, 4) == "var ")
                {
                    Advance(3);
                    AddToken(TokenType.VAR);
                }
                break;


            case '\r':
            case '\t':
                break;

            case '\n':
                Line++;
                break;

            case ' ': AddToken(TokenType.WHITESPACE);
                break;

            default:
                if (char.IsDigit(c))
                    TokenizeNumber();
                
                else if (char.IsLetter(c))
                    TokenizeLetter();

                else
                    AddToken(TokenType.BAD);

                break;

        }

        Advance();
    }

    private void TokenizeNumber() 
    {
        bool hasDecimal = false;

        while (char.IsDigit(Peek()) || (Peek() == '.' && !hasDecimal))
        {
            if (Peek() == '.')
            {
                if (hasDecimal)
                {
                    AddToken(TokenType.BAD);
                    return;
                }

                hasDecimal = true;
            }

            Advance();
        }

        AddToken(TokenType.NUMBER);
    }

    private void TokenizeLetter() 
    {
        while (char.IsLetter(Peek()))
            Advance();

        AddToken(TokenType.IDENTIFIER);
    }

    private void AddToken(TokenType type, string? text = null)
    {
        if (text == null)
        {
            int length = Math.Min(Current - Start, Source.Length - Start);
            text = length != 0 ? Source.Substring(Start, length) : Source[Current].ToString();
        }

        Tokens.Add(new Token(type, null, text, Line));
    }

    private void Advance() 
    {
        Current++;
    }

    private void Advance(int n) 
    {
        Current += n;
    }

    private bool Match(char expected)
    {
        if (IsEndOfFile() || Source[Current + 1] != expected)
            return false;

        Current++;
        return true;
    }

    private char Peek()
    {
        if (IsEndOfFile()) 
            return ' ';

        return Source[Current];
    }

    private bool IsEndOfFile() => Current >= Source.Length;
}
