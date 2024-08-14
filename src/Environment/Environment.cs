namespace Runic;

public class Environment(Environment? parent = null)
{
    public Environment? Parent { get; set; } = parent;
    public Dictionary<string, (ValueType, bool)> Variables { get; set; } = [];

    public ValueType DeclareVariable(string variable, ValueType type, bool isMutable) {
        if (Variables.ContainsKey(variable))
            throw new Exception("Attempted to redeclare already declared variable");

        Variables[variable] = (type, isMutable);
        return type;
    }

    public ValueType AssignVariable(string variable, ValueType type) {
        var env = ResolveVariable(variable);
        env.Variables[variable] = (type, true);

        return type;
    }

    public Environment ResolveVariable(string variable) {
        if (Variables.ContainsKey(variable)) return this;

        if (Parent is null)
            throw new Exception($"Attempted to assign to non existent varibale {variable}");

        return Parent.ResolveVariable(variable);
    }

    public (ValueType, bool)? LookupVariable(string variable) {
        return Variables.TryGetValue(variable, out var val) ? val : null; 
    }
}