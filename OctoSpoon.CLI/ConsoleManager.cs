static class ConsoleManager
{
    public static string Request(string? printText = null)
    {
        if (printText != null)
            Console.WriteLine("\n" + printText);

        Console.Write("\r\n> ");

        string? text = null;

        do
        {
            text = Console.ReadLine();
        } while (String.IsNullOrWhiteSpace(text));

        return text;
    }

    public static int RequestNumber(int maxNum, string? printText = null)
    {
        if (printText != null)
            Console.WriteLine("\n" + printText);

        int selected;
        string text;
        do
        {
            text = Request();
        } while (!(int.TryParse(text, out selected) && selected > 0 && selected <= maxNum));

        return selected;
    }

    public static int SelectorRequest(string[] selector, string? printText = null)
    {
        var maxIndex = selector.Length > 5 ? 5 : selector.Length;

        if (printText != null)
            Console.WriteLine("\n" + printText);

        for (var i = 0; i < maxIndex; i++)
        {
            Console.WriteLine($"{i + 1} {selector[i]}");
        }

        int selected;
        string text;
        do
        {
            text = Request();
        } while (!(int.TryParse(text, out selected) && selected > 0 && selected <= maxIndex));

        return selected - 1;
    }
}