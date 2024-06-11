using System.Runtime.CompilerServices;

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
        int typeWidth = 10;
        int valueWidth = 10;
        int textWidth = 10;

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
                break;
            case '\n':
                Line++;
                break;

            default: AddToken(TokenType.BAD); 
                break;
        }

        Advance();
    }

    public void AddToken(TokenType type)
    {
        string text = (Current < Source.Length) ? Source[Current].ToString() : "";
        Tokens.Add(new Token(type, null, text, Line));
    }

    private void Advance() 
    {
        Current++;
    }

    private char Peek()
    {
        if (IsEndOfFile()) 
            return ' ';

        return Source[Current];
    }

    public bool IsEndOfFile() => Current >= Source.Length;
}
