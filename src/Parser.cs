namespace judas;

internal class Parser(List<Token> tokens) {
    public List<Token> Tokens { get; set; } = tokens;
    public Program Program { get; set; }
    public int Current { get; set; }

    public Program Parse() {
        while (!IsEof()) {
            Program.Body.Add(ParseExpression());
        }

        return Program;
    }

    private Expression ParseExpression() {

    }

    private bool IsEof()
        => Tokens[Current].Type == TokenType.Eof;
}