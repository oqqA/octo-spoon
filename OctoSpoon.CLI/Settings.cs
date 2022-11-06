public class Settings
{
    public string Token { get; set; }
    public List<CachePathDuscussion> CachePathsToDiscussions { get; set; }
}

public class CachePathDuscussion
{
    public string Author { get; set; }

    public string RepositoryName { get; set; }

    public int DiscussionNumber { get; set; }
}

