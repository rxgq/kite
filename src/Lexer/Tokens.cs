namespace Runic;

public enum TokenType {
    Number, 
    Identifier, 
    String,

    True,
    False,
    Undefined,
    Default,

    Let,
    Mut,
    If,
    Elif,
    Else,
    For,
    While,
    Def,
    Return,
    Echo,
    Skip,
    Halt,

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
    CompoundBinOp,
    Inc,
    Dec,

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
        { "return", TokenType.Return },
        { "or", TokenType.Or },
        { "not", TokenType.Not },
        { "true", TokenType.True },
        { "false", TokenType.False },
        { "default", TokenType.Default },
        { "undefined", TokenType.Undefined },
        { "def", TokenType.Def },
        { "echo", TokenType.Echo },
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

