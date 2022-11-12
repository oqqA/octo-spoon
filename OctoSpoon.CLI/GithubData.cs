using System.Text.Json.Serialization;

namespace OctoSpoon.CLI;
public class Discussion
{
    [JsonPropertyName("number")]
    public int Number { get; set; }
    [JsonPropertyName("title")]
    public string? Title { get; set; }
}

public class CommentNode
{
    [JsonPropertyName("author")]
    public Author? Author { get; set; }

    [JsonPropertyName("body")]
    public string? Body { get; set; }
}

public class Author
{
    [JsonPropertyName("login")]
    public string? Login { get; set; }
}

public class Repository
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("updatedAt")]
    public string? UpdatedAt { get; set; }
}
