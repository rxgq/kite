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

    BAD, 
    WHITESPACE,
    NEWLINE,
    EOF
}

public class Token 
{
    public TokenType Type;
    public object Value;
    public string Text;
    public int Line;

    public Token(TokenType type, object value, string text, int line)
    {
        Type = type;
        Value = value;
        Text = text;
        Line = line;
    }

    public override string ToString()
    {
        int typeWidth = 15;
        int valueWidth = 15;
        int textWidth = 15;

        string typeStr = Type.ToString().PadRight(typeWidth);
        string valueStr = (Value?.ToString() ?? "null").PadRight(valueWidth);
        string textStr = Text.PadRight(textWidth);

        return $"{typeStr} | value: {valueStr} | text: {textStr} | line: {Line}";
    }
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

            case '\r':
                break;

            case '\n':
                Line++;
                break;

            case ' ': AddToken(TokenType.WHITESPACE);
                break;

            default: AddToken(TokenType.BAD); 
                break;
        }

        Advance();
    }

    public void AddToken(TokenType type, string text = null)
    {
        if (text == null)
        {
            int length = Current - Start;

            text = length != 0 ? 
                Source.Substring(Start, length + 1) : 
                Source[Current].ToString();
        }

        Tokens.Add(new Token(type, null, text, Line));
    }

    private void Advance() 
    {
        Current++;
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

    public bool IsEndOfFile() => Current >= Source.Length;
}
