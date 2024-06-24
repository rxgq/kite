using System.Xml;

namespace judas_script;

enum TokenType 
{ 
    OPERATOR,

    ASSIGNMENT,
    EQUALITY,

    WHITESPACE,
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
                OnOperator();
                break;

            case '=':
                if (Peek() == '=')
                {
                    Advance();
                    OnEquality();
                }

                else
                    OnAssignment();

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

    private void Advance()
        => Current++;

    private char Peek() 
        => IsEndOfFile() ? '\0' : Source[Current + 1];

    private string CurrentChars() 
    {
        if (IsEndOfFile())
            return Source[Current].ToString();

        return Source[Start..(Current + 1)];
    }

    private void OnOperator() 
        => Tokens.Add(new Token(TokenType.OPERATOR, CurrentChars()));

    private void OnAssignment() 
        => Tokens.Add(new Token(TokenType.ASSIGNMENT, CurrentChars()));

    private void OnEquality() 
        => Tokens.Add(new Token(TokenType.EQUALITY, CurrentChars()));

    private void OnBadToken() 
        => Tokens.Add(new Token(TokenType.BAD, CurrentChars()));

    private void OnWhiteSpace() 
       =>  Tokens.Add(new Token(TokenType.WHITESPACE, CurrentChars()));
}
