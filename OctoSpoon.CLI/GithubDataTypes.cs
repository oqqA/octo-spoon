public class Discussion
{
    public int number { get; set; }
    public string? title { get; set; }
}

public class CommentNode
{
    public Author author { get; set; }
    public string? body { get; set; }
}

public class Author
{
    public string? login { get; set; }
}

