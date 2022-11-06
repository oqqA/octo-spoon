using System.Text;

namespace OctoSpoon.CLI
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            Console.InputEncoding = Encoding.Unicode;
            Console.OutputEncoding = Encoding.Unicode;

            var settingsRepository = new SettingsRepository();
            var settings = settingsRepository.Get();

            if (settings.Token == null || !await isTokenValid(settings.Token))
            {
                do
                {
                    settings.Token = ConsoleManager.Request("Please create and input github token:\r\n---> https://github.com/settings/tokens \r\n(Select access -> read:discussion)");

                    if (await isTokenValid(settings.Token))
                        break;
                    else
                        Console.WriteLine("\nERROR: Bad token");

                } while (true);

                settingsRepository.Save(settings);
            }

            // todo: print last cache 

            var githubRepository = new GithubRepository(settings.Token);
            // <>
            var temprepositoriess = await githubRepository.GetRepositories("githubaefq23243tf324rgfd");
            // <>

            var author = ConsoleManager.Request("Please input author:");
            // todo: checkAuthor()

            var repositories = await githubRepository.GetRepositories(author);
            var selectorRepositories = repositories?.Select(p => p.name + ((p.description != null) ? " - " + p.description : ""));
            var indexRepository = ConsoleManager.SelectorRequest(selectorRepositories.ToArray(), "Please select repositories:");


            var discussions = await githubRepository.GetDiscussions(author, repositories[indexRepository].name);
            var selectorDiscussions = discussions?.Select(p => p.title);
            var selectedIndex = ConsoleManager.SelectorRequest(selectorDiscussions.ToArray(), "Please select discussion:");


            var comments = await githubRepository.GetComments(author, repositories[indexRepository].name, discussions[selectedIndex].number);
            var selectorComments = comments.Select(p => p.body);
            // ConsoleManager.SelectorRequest(selectorComments.ToArray(), "Please select comments:");

            var cache = new CachePathDuscussion()
            {
                Author = author,
                RepositoryName = repositories[indexRepository].name,
                DiscussionNumber = discussions[selectedIndex].number
            };
            settings.CachePathsToDiscussions.Add(cache);
            settingsRepository.Save(settings);

            var randomCount = ConsoleManager.RequestNumber(comments.Count, $"Found {comments.Count} comments, please input count random comments you need:");
            var takenComments = comments.OrderBy(x => new Random().Next()).Take(randomCount);

            foreach (var node in takenComments)
            {
                Console.WriteLine($"\n---> Author: {node.author.login} \r\n\n {node.body}");
            }

            Console.WriteLine("\nGoodbye, World!");
        }

        public static async Task<bool> isTokenValid(string token)
        {
            var tempGithubRepository = new GithubRepository(token);
            var tempRepositories = await tempGithubRepository.GetRepositories("github");
            return tempRepositories != null;
        }
    }
}
