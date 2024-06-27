namespace Judas;

internal abstract class JudasException : Exception
{
    public string Message { get; set; }
}

internal class NoValidOverloadException : JudasException 
{
    public string Method { get; set; }
    public int ArgsCount { get; set; }

    public NoValidOverloadException(string method, int argsCount) 
    {
        Method = method;
        ArgsCount = argsCount;
        Message = $"No valid overload for '{method}' that takes {argsCount} parameters.";
    }
}