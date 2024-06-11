namespace judas_script;


public class Token
{
    public static Dictionary<string, TokenType> Keywords = new()
    {
        { "if", TokenType.IF },
        { "for", TokenType.FOR },
        { "true", TokenType.TRUE },
        { "false", TokenType.FALSE },
        { "null", TokenType.NULL },
        { "print", TokenType.PRINT }
    };

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
