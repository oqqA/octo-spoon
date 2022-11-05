using dotenv.net;

namespace OctoSpoon.CLI
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            DotEnv.Load();
            var envVars = DotEnv.Read();
            var token = envVars["TOKEN"];
            // var token = ConsoleManager.Request("Please input github token (Settings -> Developer settings -> Personal access tokens):"); // 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
            // todo: checkToken()

            var githubRepository = new GithubRepository(token);

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

            Console.WriteLine("Goodbye, World!");
        }
    }
}