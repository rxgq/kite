namespace judas;

public class Environment(Environment parent = null)
{
    public Environment? Parent { get; set; } = parent;
    public Dictionary<string, (ValueType, bool)> Variables { get; set; } = [];

    public ValueType DeclareVariable(string variable, ValueType type, bool IsMutable) {
        if (Variables.ContainsKey(variable))
            throw new Exception("Attempted to redeclare already declared variable");

        Variables[variable] = (type, IsMutable);
        return type;
    }

    public ValueType AssignVariable(string variable, ValueType type) {
        var env = ResolveVariable(variable);
        env.Variables[variable] = (type, false);

        return type;
    }

    public Environment ResolveVariable(string variable) {
        if (Variables.ContainsKey(variable)) return this;

        if (Parent is null)
            throw new Exception($"Attempted to assign to non existent varibale {variable}");

        return Parent.ResolveVariable(variable);
    }

    public ValueType? LookupVariable(string variable) {
        return Variables.TryGetValue(variable, out var val) ? val.Item1 : null; 
    }
}