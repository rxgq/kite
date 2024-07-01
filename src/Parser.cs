using judas_script.src.Libraries;

namespace Judas;

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
            Expressions.Add(expression);
        }

        return Expressions;
    }

    private Expr ParseExpression()
    {
        Token currentToken = CurrentToken();

        return currentToken.Type switch
        {
            TokenType.PLUS or TokenType.MINUS or TokenType.STAR or TokenType.SLASH or TokenType.MOD or 
            TokenType.GREATER_THAN or TokenType.LESS_THAN or 
            TokenType.GREATER_THAN_EQUALS or TokenType.LESS_THAN_EQUALS or
            TokenType.NUMBER or TokenType.BOOLEAN or
            TokenType.EQUALS or TokenType.NOT_EQUALS => ParseBinary(),

            TokenType.INCREMENT or TokenType.DECREMENT or TokenType.NOT => ParseUnary(),

            TokenType.ASSIGNMENT or TokenType.IDENTIFIER => ParseAssignment(),
            TokenType.KEYWORD => ParseVariableDeclaration(),
            TokenType.METHOD => ParseMethodCall(),

            _ => throw new JudasException.BadException(),
        };
    }

    private Expr ParseMethodCall() 
    {
        Expr expr = ParsePrimary();

        if (expr is MethodCallExpr method)
        {
            // initial parameter
            var parameters = new List<Token> { Advance() };

            if (parameters[0].Type == TokenType.METHOD)
                return new MethodCallExpr(method.Identifier, new List<Token>());

            if (parameters[0].Type == TokenType.TERMINATOR)
                return new MethodCallExpr(method.Identifier, new List<Token>());

            while (Advance().Type == TokenType.SEPARATOR) 
            {
                var nextParameter = Advance();
                parameters.Add(nextParameter);

                if (nextParameter.Type == TokenType.TERMINATOR)
                    throw new JudasException.ExpectedSyntaxException("parameter after ','");

                continue;
            }

            // some random offset
            Current--;

            if (Advance().Type != TokenType.TERMINATOR)
                throw new JudasException.ExpectedSyntaxException(";");

            return new MethodCallExpr(method.Identifier, parameters);
        }


        return new UnknownExpr();
    }

    private Expr ParseAssignment() 
    {
        Expr assignee = ParsePrimary();

        var op = Advance();
        if (op.Type != TokenType.ASSIGNMENT && op.Type != TokenType.PLUS_EQUALS
            && op.Type != TokenType.MINUS_EQUALS && op.Type != TokenType.STAR_EQUALS
            && op.Type != TokenType.SLASH_EQUALS) 
        {
            throw new Exception($"Invalid op: {op.Type} for assignment");
        }
        
        Expr assigner = ParsePrimary();

        var terminator = Advance();
        if (terminator.Type != TokenType.TERMINATOR)
            throw new Exception("Expected ;");

        if (assignee is IdentifierExpr identAssignee) 
        {
            if (assigner is IdentifierExpr identAssigner)
                return new AssignmentExpr(op.Lexeme, identAssigner, identAssignee.Name);

            else if (assigner is NumericExpr numericAssigner)
                return new AssignmentExpr(op.Lexeme, numericAssigner, identAssignee.Name);
        }

        return new UnknownExpr();
    }

    private Expr ParseVariableDeclaration() 
    {
        Expr expr = ParsePrimary();

        if (expr is VariableDeclarationExpr keyword) 
        {
            var identifier = Advance();

            var equals = Advance();
            if (equals.Type != TokenType.ASSIGNMENT)
                throw new Exception("Expected =");

            var value = ParsePrimary();
            var variable = new VariableDeclarationExpr(keyword.Declaration, identifier.Lexeme, value);

            var terminator = Advance();
            if (terminator.Type != TokenType.TERMINATOR)
                throw new Exception("Expected ;");

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
            TokenType.STRING => new StringLiteralExpr(token.Value),

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