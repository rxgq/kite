namespace Runic;

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
            TokenType.Echo => ParseEcho(),
            TokenType.Let or TokenType.Mut => ParseVariableDeclaration(),
            TokenType.If or TokenType.Elif or TokenType.Else => ParseIfStatement(),
            TokenType.While => ParseWhileStatement(),
            TokenType.Def => ParseFunctionDeclaration(),
            TokenType.Return => ParseReturnStatement(),
            _ => ParseExpression(),
        };
    }

    private Expression ParseReturnStatement() {
        var returnToken = Consume();

        if (Match(";")) {
            Consume();
            return new ReturnStatement(null);
        }

        var returnValue = ParseExpression();

        ExpectToBe(TokenType.SemiColon, "Expected ';' after return statement");
        return new ReturnStatement(returnValue);
    }


    private Expression ParseFunctionDeclaration() {
        Advance();

        var identifier = Consume();

        ExpectToBe(TokenType.LeftParen, "Expected '(' after function name");
        var args = new List<string>();

        if (!Match(")")) {
            do {
                var arg = Consume();
                args.Add(arg.Value);

                if (Match(",")) Advance();
                else if (Match(")")) break;
                else throw new Exception("Expected ',' or ')' after parameter");
            } while (true);
        }

        ExpectToBe(TokenType.RightParen, "Expected ')' after parameters");

        ExpectToBe(TokenType.LeftBrace, "Expected '{' after parameters");
        var body = (BlockStatement)ParseBlockStatement();
        ExpectToBe(TokenType.RightBrace, "Expected '}' after function body");

        return new FunctionDeclaration(identifier.Value, [.. args], body);
    }

    private Expression ParseFunctionCall(string functionName) {
        ExpectToBe(TokenType.LeftParen, "Expected '(' after function name");

        var args = new List<Expression>();

        if (!Match(")")) {
            do {
                var arg = ParseExpression();
                args.Add(arg);

                if (Match(",")) Advance();
                else if (Match(")")) break;
                else throw new Exception("Expected ',' or ')' after argument");
            } while (true);
        }

        ExpectToBe(TokenType.RightParen, "Expected ')' after function call");
        ExpectToBe(TokenType.SemiColon, "Expected ';' after function call");

        return new FunctionCall(functionName, args);
    }

    private Expression ParseWhileStatement() {
        Advance();
        var condition = ParseExpression();

        ExpectToBe(TokenType.LeftBrace, "Expected '{' after condition");
        var consequent = (BlockStatement)ParseBlockStatement();
        ExpectToBe(TokenType.RightBrace, "Expected '}' after condition");

        return new WhileStatement(condition, consequent);
    }

    private Expression ParseIfStatement() {
        Advance();
        var condition = ParseExpression();

        ExpectToBe(TokenType.LeftBrace, "Expected '{' after condition");
        var consequent = (BlockStatement)ParseBlockStatement();
        ExpectToBe(TokenType.RightBrace, "Expected '}' after condition");

        IfStatement? rootIf = new IfStatement(condition, consequent);
        IfStatement? currentIf = rootIf;

        while (Match("elif")) {
            Advance();
            var elifCondition = ParseExpression();

            ExpectToBe(TokenType.LeftBrace, "Expected '{' after elif condition");
            var elifConsequent = (BlockStatement)ParseBlockStatement();
            ExpectToBe(TokenType.RightBrace, "Expected '}' after elif condition");

            var elifStatement = new IfStatement(elifCondition, elifConsequent);

            currentIf.Alternate = elifStatement;
            currentIf = elifStatement;
        }

        if (Match("else")) {
            Advance();
            ExpectToBe(TokenType.LeftBrace, "Expected '{' after else");
            var elseBranch = (BlockStatement)ParseBlockStatement();
            ExpectToBe(TokenType.RightBrace, "Expected '}' after else");

            currentIf.Alternate = new IfStatement(new BooleanExpression(true), elseBranch);
        }

        return rootIf;
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

    private Expression ParseEcho() {
        Advance();

        var expressions = new List<Expression>();

        do {
            var primaryExpr = ParsePrimary();
            expressions.Add(primaryExpr);

            if (Tokens[Current].Type == TokenType.Comma) Advance();
            else break;
        } while (true);

        ExpectToBe(TokenType.SemiColon, "Expected semi colon after 'echo'");

        return new EchoStatement(expressions);
    }


    private Expression ParseExpression() {
        return ParseAssignment();
    }
    
    private Expression ParseAssignment() {
        var left = ParseLogicalOr();
        
        if (Tokens[Current].Type == TokenType.Assignment) {
            Advance();

            var val = ParseAssignment(); 
            ExpectToBe(TokenType.SemiColon, "Expected semi colon after assignment expression");
            
            return new AssignmentExpression(left, val);
        }

        return left;
    }

    private Expression ParseLogicalOr() {
        var expr = ParseLogicalAnd();

        while (Match("or")) {
            var op = Consume();
            var right = ParseLogicalAnd();
            expr = new LogicalExpression(expr, right, op);
        }

        return expr;
    }

    private Expression ParseLogicalAnd() {
        var expr = ParseBitwise();

        while (Match("and")) {
            var op = Consume();
            var right = ParseBitwise();
            expr = new LogicalExpression(expr, right, op);
        }

        return expr;
    }
    
    private Expression ParseBitwise() {
        var expr = ParseRelational();

        while (Match("&") || Match("|") || Match("^") || Match("<<") || Match(">>")) {
            var op = Consume();
            var right = ParseRelational();
            expr = new BinaryExpression(expr, right, op);
        }

        return expr;
    }

    private Expression ParseRelational() {
        var expression = ParseAdditive();

        while (Match(">") || Match("<") || Match(">=") || Match("<=") || Match("!=") || Match("==")) {
            var operatorToken = Consume();
            var right = ParseAdditive();
            expression = new RelationalExpression(expression, right, operatorToken);
        }

        return expression;
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

    private Expression ParseMultiplicative() {
        var left = ParseUnary();

        while (Match("*") || Match("/") || Match("%") || Match("**")) {
            var op = Consume();
            var right = ParseUnary();

            left = new BinaryExpression(left, right, op);
        }

        return left;
    }
    
    private Expression ParseUnary() {
        if (Match("-") || Match("not") || Match("~") || Match("++") || Match("--")) {
            var op = Consume();
            var right = ParseUnary();

            return new UnaryExpression(op, right);
        }

        return ParsePrimary(); 
    }

    private Expression ParsePrimary() {
        var token = Tokens[Current];

        switch (token.Type) {
            case TokenType.Identifier:
                Advance();

                if (Match("(")) return ParseFunctionCall(token.Value);

                return new IdentifierExpression(token.Value);
            
            case TokenType.Number:
                if (!float.TryParse(token.Value, out float flt))
                    throw new Exception("Attempted to parse non-float token as float");

                Advance();
                return new NumericExpression(flt);

            case TokenType.String:
                Advance();
                return new StringExpression(token.Value);

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