namespace judas;

enum TokenType {
    Number, 
    Identifier, 

    Var,
    Const,

    If,
    Elif,
    Else,
    For,
    While,

    And,
    Or,
    Not,

    BinaryOp, 
    Equality, 
    NotEquality,
    Assignment, 

    LeftParen, 
    RightParen,
    Comma,
    SemiColon,

    Space,
    Eof,
    Bad
}

internal class Token {
    public TokenType Type { get; set; }
    public string Value { get; set; }

    public static readonly Dictionary<string, TokenType> Keywords = new() {
        { "var", TokenType.Var },
        { "if", TokenType.If },
        { "elif", TokenType.Elif },
        { "else", TokenType.Else },
        { "for", TokenType.For },
        { "while", TokenType.While },
        { "and", TokenType.And },
        { "or", TokenType.Or },
    };

    public Token(TokenType type, string value) {
        Type = type;
        Value = value;
    }

    public Token(TokenType type, char value) {
        Type = type;
        Value = value.ToString();
    }

    public override string ToString() => $"{Type, -12} {Value}";
}

