namespace judas_script;

public enum TokenType
{
    PLUS,
    MINUS,
    STAR,
    SLASH, 

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

    public Token(TokenType type, object value, string text)
    {
        Type = type;
        Value = value;
        Text = text;
    }

    public override string ToString()
    {
        int typeWidth = 10;
        int valueWidth = 10;
        int textWidth = 10;

        string typeStr = Type.ToString().PadRight(typeWidth);
        string valueStr = (Value?.ToString() ?? "null").PadRight(valueWidth);
        string textStr = Text.PadRight(textWidth);

        return $"{typeStr} | value: {valueStr} | text: {textStr}";
    }
}

internal class Scanner
{
    private readonly List<Token> Tokens = new();
    private readonly string Source;

    public int Start;
    public int End;

    public int Current;

    public Scanner(string source) 
    {
        Source = source;
    }

    public List<Token> Scan() 
    {
        while (!IsEndOfFile()) 
        {
            ScanToken();
        }

        AddToken(TokenType.EOF);

        return Tokens;
    }

    public void ScanToken()
    {
        if (IsEndOfFile())
            return;

        switch (Source[Current])
        {
            case '+': AddToken(TokenType.PLUS);
                break;

            case '-': AddToken(TokenType.MINUS);
                break;

            case '*': AddToken(TokenType.STAR);
                break;

            case '/': AddToken(TokenType.SLASH);
                break;

            case ' ': AddToken(TokenType.WHITESPACE);
                break;

            case '\r':
                if (Current + 1 < Source.Length && Source[Current + 1] == '\n')
                    Current++;

                AddToken(TokenType.NEWLINE);
                break;

            default: AddToken(TokenType.BAD); 
                break;
        }

        Current++;
    }

    public void AddToken(TokenType type)
    {
        string text = (Current < Source.Length) ? Source[Current].ToString() : "";
        Tokens.Add(new Token(type, null, text));
    }

    public bool IsEndOfFile() => Current >= Source.Length;
}
