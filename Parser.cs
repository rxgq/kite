namespace judas_script;

public sealed class Parser
{
    private readonly List<Token> Tokens;
    private int Current = 0;

    public Parser(List<Token> tokens)
    {
        Tokens = tokens;
    }

    public void Parse()
    {
        while (!IsEOFToken())
        {
            Expr expression = ParseExpression();
            Console.WriteLine(expression);
        }
    }

    private Expr ParseExpression()
    {
        return ParseBinary();
    }

    private Expr ParseBinary(int precedence = 0)
    {
        Expr left = ParsePrimary();

        while (!IsEOFToken() && Precedence(CurrentToken()) >= precedence)
        {
            Token operatorToken = Advance();

            Expr right = ParseBinary(Precedence(operatorToken) + 1);
            left = new BinaryExpr(left, right, operatorToken.Lexeme);
        }

        return left;
    }

    private Expr ParsePrimary()
    {
        Token token = Advance();

        return token.Type switch
        {
            TokenType.NUMBER => new NumericExpr(double.Parse(token.Lexeme)),
            TokenType.IDENTIFIER => new IdentifierExpr(token.Lexeme),
            _ => new UnknownExpr(),
        };
    }

    private Token Advance()
    {
        if (!IsEOFToken()) 
            Current++;

        return Previous();
    }

    private Token Previous()
        => Tokens[Current - 1];

    private Token CurrentToken()
        => Tokens[Current];

    private bool IsEOFToken()
        => Tokens[Current].Type == TokenType.EOF;

    private static int Precedence(Token token)
    {
        return token.Type switch
        {
            TokenType.PLUS or TokenType.MINUS => 1,

            TokenType.STAR or TokenType.SLASH or TokenType.MOD => 2,

            TokenType.GREATER_THAN or TokenType.LESS_THAN or 
            TokenType.GREATER_THAN_EQUALS or TokenType.LESS_THAN_EQUALS or 
            TokenType.EQUALS or TokenType.NOT_EQUALS => 3,

            _ => 0,
        };
    }
}