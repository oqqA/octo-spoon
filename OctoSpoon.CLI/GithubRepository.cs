using System.Diagnostics.Contracts;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace OctoSpoon.CLI
{
    public class GithubRepository
    {
        private HttpClient httpClient;
        public GithubRepository(string token)
        {
            httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri("https://api.github.com/graphql");
            httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:106.0) Gecko/20100101 Firefox/106.0");
            httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
        }

        public async Task<JsonObject?> GraphqlRequest(StringContent requereJson)
        {
            var response = await httpClient.PostAsync("", requereJson);
            var responseJson = await response.Content.ReadAsStreamAsync();
            return JsonSerializer.Deserialize<JsonObject>(responseJson);
        }

        public async Task<List<Discussion>?> GetDiscussions(string author, string nameRepo)
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
                    name = nameRepo
                }
            };

            var graphqlResponse = await GraphqlRequest(queryBuilder.GetJson());

            var node = graphqlResponse?["data"]?["repository"]?["discussions"]?["nodes"];
            var data = node?.ToJsonString();

            return JsonSerializer.Deserialize<List<Discussion>>(data);
        }

        public async Task<List<CommentNode>?> GetComments(string author, string nameRepo, int number)
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
                    name = nameRepo,
                    number = number
                }
            };

            var graphqlResponse = await GraphqlRequest(queryBuilder.GetJson());

            var node = graphqlResponse["data"]["repository"]["discussion"]["comments"]["nodes"];
            var data = node?.ToJsonString();

            return JsonSerializer.Deserialize<List<CommentNode>>(data);
        }

        public async Task<List<Repository>?> GetRepositories(string author)
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

            var node = graphqlResponse["data"]["repositoryOwner"]["repositories"]["nodes"];
            var data = node?.ToJsonString();

            return JsonSerializer.Deserialize<List<Repository>>(data);
        }

    }

}
