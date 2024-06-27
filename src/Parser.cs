namespace judas_script.src;

public sealed class Parser
{
    private readonly List<Token> Tokens;
    private int Current = 0;

    List<Expr> Expressions { get; set; } = new();

    public Parser(List<Token> tokens)
    {
        Tokens = tokens;
    }

    public List<Expr> Parse()
    {
        while (!IsEOFToken())
        {
            Expr expression = ParseExpression();

            if (expression.Type == ExprType.WhiteSpace)
                continue;

            Expressions.Add(expression);
        }

        return Expressions;
    }

    private Expr ParseExpression()
    {
        return ParseMethodCall();
    }

    private Expr ParseMethodCall() 
    {
        Expr expr = ParsePrimary();

        if (expr is MethodCallExpr method) 
        {
            var param = Advance();
            return new MethodCallExpr(method.Identifier, new List<object> { param });
        }


        return new UnknownExpr();
    }

    private Expr ParseAssignment() 
    {
        Expr expr = ParsePrimary();

        if (expr is VariableDeclarationExpr keyword) 
        {
            var identifier = Advance();

            // skip equals symbol
            Advance();

            var value = Advance();

            var variable = new VariableDeclarationExpr(keyword.Declaration, identifier.Lexeme, value.Value);

            // consume terminator
            Advance();
            Interpreter.Variables.Add(variable.Identifier, variable.Value);

            return variable;
        }

        return new UnknownExpr();
    }

    private Expr ParseUnary() 
    {
        Expr left = ParsePrimary();

        while (!IsEOFToken())
        {
            Token operatorToken = Advance();
            left = new UnaryExpr(left, operatorToken.Lexeme);
        }

        return left;
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
            TokenType.BOOLEAN => new BooleanLiteralExpr(bool.Parse(token.Lexeme)),
            TokenType.IDENTIFIER => new IdentifierExpr(token.Lexeme),
            TokenType.WHITESPACE => new WhiteSpaceExpr(),

            TokenType.METHOD => new MethodCallExpr(token.Lexeme, null),
            TokenType.KEYWORD => new VariableDeclarationExpr(token.Lexeme, null, null),

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
            TokenType.INCREMENT or TokenType.DECREMENT or TokenType.NOT => 4,
            _ => 0,
        };
    }
}