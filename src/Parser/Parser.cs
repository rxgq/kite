using System.Linq.Expressions;

namespace judas;

internal class Parser(List<Token> tokens) {
    public List<Token> Tokens { get; set; } = tokens;
    public Program Program { get; set; } = new();
    public int Current { get; set; } = 0;

    public Program Parse() {
        while (!IsEof()) {
            Program.Body.Add(ParseStatement());
        }

        return Program;
    }

    private Expression ParseStatement() {
        return Tokens[Current].Type switch
        {
            TokenType.Let or TokenType.Mut => ParseVariableDeclaration(),
            TokenType.If or TokenType.Elif or TokenType.Else => ParseIfStatement(),
            _ => ParseExpression(),
        };
    }

    private Expression ParseIfStatement() {
        Advance();
        var condition = ParseExpression();

        ExpectToBe(TokenType.LeftBrace, "Expected '{' after condition");

        Expression? consequent = ParseBlockStatement();

        ExpectToBe(TokenType.RightBrace, "Expected '}' after condition");

        return new IfStatement(condition, consequent as BlockStatement);
    }

    private Expression ParseBlockStatement() {
        List<Expression> statements = [];

        while (!Match("}")) {
            var stmt = ParseStatement();
            statements.Add(stmt);
        }

        return new BlockStatement(statements);
    }

    private Expression ParseVariableDeclaration() {
        var declarator = Consume();
        var identifier = ExpectToBe(TokenType.Identifier, "Expected identifier after declaration");
        var isMut = declarator.Type == TokenType.Mut;

        var next = Consume();
        if (next.Type == TokenType.Assignment) {
            var val = ParseExpression();
            
            ExpectToBe(TokenType.SemiColon, "Expected semi colon after variable declaration");

            return new VariableDeclarator(declarator.Value, identifier.Value, val, isMut);
        } 
        else if (next.Type == TokenType.SemiColon) {
            if (!isMut) throw new Exception("Cannot declare a constant variable as undefined");

            return new VariableDeclarator(declarator.Value, identifier.Value, null, isMut);
        }
        
        throw new Exception("Unexpected token found in variable declaration");
    }

    private Expression ParseExpression() {
        return ParseAssignment();
    }

    // private Expression ParseLogicalOr() {
    //     var expr = ParseLogicalAnd();

    // }

    // private Expression ParseLogicalAnd() {
    //     var expr = ParseEquality();
        
    // }

    // private Expression ParseEquality() {
    //     var expr = ParseRelational();

    // }

    // private Expression ParseRelational() {
    //     var expr = ParseUnary();

    //     if (Match(">") || Match("<") || Match(">=") || Match("<=")) {

    //     }
    // }

    private Expression ParseAssignment() {
        var left = ParseAdditive();
        
        if (Tokens[Current].Type == TokenType.Assignment) {
            Advance();

            var val = ParseAssignment();
            ExpectToBe(TokenType.SemiColon, "Expected semi colon after assignment expression");
            
            return new AssignmentExpression(left, val);
        }

        return left;
    }

    private Expression ParseAdditive () {
        var left = ParseMultiplicative();

        while (Match("+") || Match("-")) {
            var op = Consume();
            var right = ParseMultiplicative();

            left = new BinaryExpression(left, right, op);
        }

        return left;
    }
    
    private Expression ParseUnary() {
        if (Match("-") || Match("!")) {
            var op = Consume();
            var right = ParseUnary();
            return new UnaryExpression(op, right);
        }

        return ParsePrimary(); 
    }

    private Expression ParseMultiplicative() {
        var left = ParseUnary();

        while (Match("*") || Match("/") || Match("%") || Match("**")) {
            var op = Consume();
            var right = ParseUnary();

            left = new BinaryExpression(left, right, op);
        }

        return left;
    }

    private Expression ParsePrimary() {
        var token = Tokens[Current];

        switch (token.Type) {
            case TokenType.Identifier:
                Advance();
                return new IdentifierExpression(token.Value);
            
            case TokenType.Number:
                if (!float.TryParse(token.Value, out float flt))
                    throw new Exception("Attempted to parse non-float token as float");

                Advance();
                return new NumericExpression(flt);

            case TokenType.Undefined:
                Advance();
                return new UndefinedExpression(Tokens[Current].Value);

            case TokenType.False or TokenType.True:
                Advance();
                return new BooleanExpression(token.Type == TokenType.True);

            case TokenType.LeftParen:
                Advance();
                var val = ParseExpression();
                Advance();
                return val;

            default: throw new Exception($"Unkown Token found while parsing primary expression '{Tokens[Current].Type}'");
        }
    }

    private Token ExpectToBe(TokenType type, string msg) {
        if (Tokens[Current].Type != type) throw new Exception(msg + $", found {Tokens[Current].Type}");
        return Consume();
    }

    private bool Match(string symbol)
        => !IsEof() && Tokens[Current].Value == symbol;

    private Token Consume() {
        return Tokens[Current++];
    }

    private void Advance() => Current++; 

    private bool IsEof()
        => Current >= Tokens.Count || Tokens[Current].Type == TokenType.Eof;
        
    public void Print() {
        Console.WriteLine();
        foreach (var expr in Program.Body)
            Console.WriteLine(expr.ToString());
    }
}