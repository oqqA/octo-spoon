namespace OctoSpoon.CLI;
public class Settings
{
    public string? Token { get; set; }
    public List<CachePathDiscussion>? CachePathsToDiscussions { get; set; }
}

public class CachePathDiscussion
{
    public string? Author { get; set; }
    public string? RepositoryName { get; set; }
    public int DiscussionNumber { get; set; }
}

