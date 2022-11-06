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

public class Repository
{
    public string? name { get; set; }
    public string? description { get; set; }
    public string? updatedAt { get; set; }
}
