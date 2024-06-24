using System.Xml;

namespace judas_script;

enum TokenType 
{ 
    OPERATOR,

    BAD,
    EOF,
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
        => $"\nType: {Type} || Value: {Value}";
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
        while (Current < Source.Length - 1) 
        {
            Current++;

            switch (Source[Current]) 
            {
                case '+':
                case '-':
                case '*':
                case '/':
                case '%':
                    OnOperator();
                    break;
            }
        }

        return Tokens;
    }

    public void OnOperator() 
    {
        Tokens.Add(new Token(TokenType.OPERATOR, Source[Current]));
    }
}
