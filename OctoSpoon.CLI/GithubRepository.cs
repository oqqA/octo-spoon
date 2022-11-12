using System.Text.Json;
using System.Text.Json.Nodes;

namespace OctoSpoon.CLI
{
    public class GithubRepository : IDisposable
    {
        private readonly HttpClient httpClient;
        public GithubRepository(string token)
        {
            httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri("https://api.github.com/graphql");
            httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:106.0) Gecko/20100101 Firefox/106.0");
            httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
        }

        public static string? GetToken()
        {
            var settings = SettingsRepository.Get();
            return settings.Token;
        }

        public static async Task<bool> IsValidToken(string token)
        {
            using var tempGithubRepository = new GithubRepository(token);
            var tempRepositories = await tempGithubRepository.GetRepositories("github");

            var isValidToken = tempRepositories != null && tempRepositories.Length != 0;
            if (isValidToken)
            {
                var settings = SettingsRepository.Get();
                settings.Token = token;
                SettingsRepository.Save(settings);
            }

            return isValidToken;
        }

        public async Task<JsonObject?> GraphqlRequest(StringContent requireJson)
        {
            var response = await httpClient.PostAsync("", requireJson);
            var responseJson = await response.Content.ReadAsStreamAsync();
            return JsonSerializer.Deserialize<JsonObject>(responseJson);
        }

        public async Task<Discussion[]> GetDiscussions(string author, string nameRepository)
        {
            var queryBuilder = new QueryBuilder
            {
                Query = @"
                    query {
                        repository(owner: $owner, name: $name) {
                            discussions(first: 100) {
                                totalCount
                                nodes {
                                    number
                                    title
                                }
                            }
                        }
                    }
                ",
                Variables = new
                {
                    owner = author,
                    name = nameRepository
                }
            };

            var graphqlResponse = await GraphqlRequest(queryBuilder.GetJson());

            if (graphqlResponse?.Count == 2)
                return Array.Empty<Discussion>(); // string? error = graphqlResponse["message"].GetValue<string>();

            var node = graphqlResponse?["data"]?["repository"]?["discussions"]?["nodes"];

            if (node == null)
                return Array.Empty<Discussion>();

            return JsonSerializer.Deserialize<Discussion[]>(node.ToJsonString()) ?? Array.Empty<Discussion>();
        }

        public async Task<CommentNode[]> GetComments(string author, string nameRepository, int numberDiscussion)
        {
            var queryBuilder = new QueryBuilder
            {
                Query = @"
                    query {
                        repository(owner: $owner, name: $name) {
                            discussion(number: $number) {
                                comments(first: 100) {
                                    totalCount
                                    
                                    nodes {
                                        author {
                                            login
                                        }
                                        body
                                    }
                                }
                            }
                        }
                    }
                ",
                Variables = new
                {
                    owner = author,
                    name = nameRepository,
                    number = numberDiscussion
                }
            };

            var graphqlResponse = await GraphqlRequest(queryBuilder.GetJson());

            if (graphqlResponse?.Count == 2)
                return Array.Empty<CommentNode>(); // string? error = graphqlResponse["message"].GetValue<string>();

            var node = graphqlResponse?["data"]?["repository"]?["discussion"]?["comments"]?["nodes"];

            if (node == null)
                return Array.Empty<CommentNode>();

            return JsonSerializer.Deserialize<CommentNode[]>(node.ToJsonString()) ?? Array.Empty<CommentNode>();
        }

        public async Task<Repository[]> GetRepositories(string author)
        {
            var queryBuilder = new QueryBuilder
            {
                Query = @"
                    query {
                        repositoryOwner(login: $owner) {
                        
                            repositories(first: 100) {
                                totalCount
                                
                                nodes {
                                    name
                                    description
                                    updatedAt
                                }
                            }
                        }
                    }
                ",
                Variables = new
                {
                    owner = author,
                }
            };

            var graphqlResponse = await GraphqlRequest(queryBuilder.GetJson());

            if (graphqlResponse?.Count == 2)
                return Array.Empty<Repository>(); // string? error = graphqlResponse["message"].GetValue<string>();

            var node = graphqlResponse?["data"]?["repositoryOwner"]?["repositories"]?["nodes"];

            if (node == null)
                return Array.Empty<Repository>();

            return JsonSerializer.Deserialize<Repository[]>(node.ToJsonString()) ?? Array.Empty<Repository>();
        }

        public void Dispose()
        {
            httpClient.Dispose();
        }
    }

}
