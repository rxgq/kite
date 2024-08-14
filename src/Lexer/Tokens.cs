namespace judas;

public enum TokenType {
    Number, 
    Identifier, 

    Let,
    Mut,

    True,
    False,
    Undefined,
    Default,

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
    NotEqual,
    Assignment,
    GreaterThan,
    LessThan,
    GreaterThanEq,
    LessThanEq,

    LeftParen, 
    RightParen,
    LeftBrace,
    RightBrace,
    Comma,
    SemiColon,

    Space,
    Eof,
    Bad
}

public class Token {
    public TokenType Type { get; set; }
    public string Value { get; set; }

    public static readonly Dictionary<string, TokenType> Keywords = new() {
        { "let", TokenType.Let },
        { "mut", TokenType.Mut },
        { "if", TokenType.If },
        { "elif", TokenType.Elif },
        { "else", TokenType.Else },
        { "for", TokenType.For },
        { "while", TokenType.While },
        { "and", TokenType.And },
        { "or", TokenType.Or },
        { "true", TokenType.True },
        { "false", TokenType.False },
        { "default", TokenType.Default },
        { "undefined", TokenType.Undefined },
    };

    public Token(TokenType type, string value) {
        Type = type;
        Value = value;
    }

    public Token(TokenType type, char value) {
        Type = type;
        Value = value.ToString();
    }

    public override string ToString() => $"{Value}";
}

