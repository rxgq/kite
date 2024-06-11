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
            case '+':
                if (Match('='))
                    AddToken(TokenType.PLUS_EQUALS);
                else
                    AddToken(TokenType.PLUS);
                break;

            case '-':
                if (Match('='))
                    AddToken(TokenType.MINUS_EQUALS);
                else
                    AddToken(TokenType.MINUS);
                break;
            case '*':
                if (Match('='))
                    AddToken(TokenType.STAR_EQUALS);
                else
                    AddToken(TokenType.STAR);
                break;
            case '/':
                if (Match('='))
                    AddToken(TokenType.SLASH_EQUALS);
                else
                    AddToken(TokenType.SLASH);
                break;

            case '<':
                if (Match('='))
                    AddToken(TokenType.LESS_EQUALS);
                else 
                    AddToken(TokenType.LESS_THAN);
                break;

            case '>':
                if (Match('='))
                    AddToken(TokenType.GREATER_EQUALS);
                else
                    AddToken(TokenType.GREATER_THAN);
                break;

            case '=':
                if (Match('='))
                    AddToken(TokenType.DOUBLE_EQUALS);
                else
                    AddToken(TokenType.EQUALS);
                break;

            case '\r':
                break;

            case '\n':
                Line++;
                break;

            case ' ':
                AddToken(TokenType.WHITESPACE);
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

            if (length != 0)
            {
                text = Source.Substring(Start, length + 1);
            }
            else 
            {
                text = Source[Current].ToString();
            }
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
