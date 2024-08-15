namespace Kite;

public abstract class ValueType {
    public string? Type { get; set; }
    public object? Value { get; set; }
}

public class FunctionType : ValueType {
    public string Name { get; }
    public List<string> Args { get; }
    public BlockStatement? Body { get; }

    public FunctionType(string name, List<string> args, BlockStatement? body = null) {
        Type = "function";
        Name = name;
        Args = args;
        Body = body;
    }
}

public class UndefinedType : ValueType {

    public UndefinedType() {
        Type = "undefined";
        Value = "undefined";
    }
}

public class NumericType : ValueType {

    public NumericType(double val) {
        Type = "number";
        Value = val;
    }
}

public class StringType : ValueType {

    public StringType(string val) {
        Type = "string";
        Value = val;
    }
}

public class BoolType : ValueType {

    public BoolType(string val) {
        Type = "boolean";
        Value = val == "true";
    }
}

// used for statements such as 'skip' and 'halt'
public class SpecialType : ValueType {

    public SpecialType(string val) {
        Type = "special";
        Value = val;
    }
}