using System.Text;

namespace OctoSpoon.CLI
{
    internal class Program
    {
        private static async Task Main()
        {
            Console.InputEncoding = Encoding.Unicode;
            Console.OutputEncoding = Encoding.Unicode;

            var token = await GetToken();
            using var githubRepository = new GithubRepository(token);

            // todo: print last activity 

            var author = await GetAuthor(githubRepository);
            var nameRepository = await GetNameRepository(githubRepository, author);
            var numberDiscussion = await GetNumberDiscussion(githubRepository, author, nameRepository);

            // todo: save last activity 
            // SettingsRepository.SaveCache(author, nameRepository, numberDiscussion);

            var randomComments = await GetRandomComments(githubRepository, author, nameRepository, numberDiscussion);

            foreach (var comment in randomComments)
            {
                Console.WriteLine($"\n---> Author: {comment?.Author?.Login} \r\n\n {comment?.Body?.Trim()}");
            }

            Console.WriteLine("\nThanks, goodbye!");
        }

        private static async Task<string> GetToken()
        {
            var token = GithubRepository.GetToken();

            while (!await GithubRepository.IsValidToken(token ?? ""))
            {
                if (token != null)
                {
                    Console.WriteLine("\nERROR: Bad token");
                }

                token = ConsoleManager.Request("""
                Please create and input github token:
                ---> https://github.com/settings/tokens 
                (Select access -> read:discussion)
                """);
            }

            return token ?? "";
        }

        private static async Task<string> GetAuthor(GithubRepository githubRepository)
        {
            string author;
            Repository[] repositories;
            do
            {
                author = ConsoleManager.Request("Please input author:");
                repositories = await githubRepository.GetRepositories(author);
            } while (repositories.Length == 0);

            return author;
        }

        private static async Task<string> GetNameRepository(GithubRepository githubRepository, string author)
        {
            var repositories = (await githubRepository.GetRepositories(author))
                .OrderByDescending(x => DateTime.Parse(x.UpdatedAt ?? ""))
                .ToArray(); ;

            var selectorRepositories = repositories
                .Select(p => p.Name + ((p.Description != null) ? " - " + p.Description : ""))
                .ToArray();

            var indexRepository = ConsoleManager.SelectorRequest(
                "Please select repositories:",
                selectorRepositories
            );

            return repositories[indexRepository]?.Name ?? "";
        }

        private static async Task<int> GetNumberDiscussion(GithubRepository githubRepository, string author, string nameRepository)
        {
            var discussions = await githubRepository.GetDiscussions(author, nameRepository);

            var selectorDiscussions = discussions
                .Select(p => p.Title ?? "")
                .ToArray();

            var selectedIndex = ConsoleManager.SelectorRequest(
                "Please select discussion:",
                selectorDiscussions);

            return discussions[selectedIndex].Number;
        }

        private static async Task<CommentNode[]> GetRandomComments(GithubRepository githubRepository, string author, string nameRepository, int numberDiscussion)
        {
            var comments = await githubRepository.GetComments(author, nameRepository, numberDiscussion);

            var randomCount = ConsoleManager.RequestNumber(
                $"Found {comments.Length} comments, please input count random comments you need:",
                comments.Length
            );

            var randomComments = comments
                .OrderBy(x => new Random().Next())
                .Take(randomCount)
                .ToArray();

            return randomComments;
        }
    }
}
