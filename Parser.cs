namespace judas_script;

internal abstract class ExpressionNode 
{
    TokenType Type;
}

internal sealed class ExprNode 
{ 

}

internal sealed class NumberNode 
{ 

}

internal sealed class BinaryNode 
{ 

}


internal sealed class Parser
{
    private List<Token> Tokens = new();
    private int Position;

    public Parser(List<Token> tokens) 
    { 
        Tokens = tokens;
    }

    public void Parse() 
    {
        foreach (var token in Tokens) 
        {
            if (token.Type == TokenType.EOF)
                return;


        }
    }

    private Token Current() => Tokens[Position];
}
