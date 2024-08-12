namespace judas;

internal class VariableDeclaration {
    List<VariableDeclarator> Declarations { get; set; }
 }

internal class VariableDeclarator {
    public string Identifier { get; set; }
    public string Literal { get; set; }
}