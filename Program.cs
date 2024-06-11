namespace judas_script;

internal class Program
{
    static void Main(string[] args)
    {
        var path = "C:\\Users\\adunderdale\\test.txt";
        var lines = File.ReadAllText(path);

        Scanner scanner = new(source: lines);
        var tokens = scanner.Scan();

        foreach (var token in tokens) {
            Console.WriteLine(token.ToString());
        }
    }
}