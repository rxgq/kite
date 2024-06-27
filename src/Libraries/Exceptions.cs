namespace judas_script.src.Libraries;

internal abstract class JudasException : Exception
{
    public string Message { get; set; }

    internal class NoValidOverloadException : JudasException
    {
        public string Method { get; set; }
        public int ArgsCount { get; set; }

        public NoValidOverloadException(string method, int argsCount)
        {
            Method = method;
            ArgsCount = argsCount;
            Message = $"No valid overload for '{method}' that takes {argsCount} parameters";
        }
    }

    internal class ExpectedSyntaxException : JudasException
    {
        public string Syntax { get; set; }

        public ExpectedSyntaxException(string syntax)
        {
            Syntax = syntax;
            Message = $"Expected {syntax}";
        }
    }

    internal class BadException : JudasException
    {
        public string Message { get; set; }

        public BadException()
        {
            Message = $"There seems to be an error with the judas source code. Contact creator.";
        }
    }
}

