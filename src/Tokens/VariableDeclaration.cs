namespace judas;

internal class VariableDeclaration {
    List<VariableDeclarator> Declarations { get; set; } = [];
 }

internal class VariableDeclarator(string identifier, string literal, Expression init) {
    public string Identifier { get; set; } = identifier;
    public string Literal { get; set; } = literal;
    public Expression Init { get; set; } = init;

    public override string ToString()
        => $"[{Identifier} {Literal}: {Init}]";
}