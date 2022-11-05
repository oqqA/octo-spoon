// var token = ConsoleManager.Request("Please input token");

// var root = Directory.GetCurrentDirectory();
// var dotenvPath = Path.Combine("./" + root, ".env");
// DotEnv.Config(new DotEnvOptions(env));
// DotEnv.Load();
// var envVars = DotEnv.Read();

// var token = ConsoleManager.Request("Please input github token (Settings -> Developer settings -> Personal access tokens):"); // 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
// todo: checkToken()
// var author = ConsoleManager.Request("Please input author:"); // cleannetcode 

// var nameRepo = ConsoleManager.Request("Please input name repository:"); // Index

namespace OctoSpoon.CLI
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            var token = "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";
            var author = "cleannetcode";
            var nameRepo = "Index";

            var githubRepository = new GithubRepository(token);
            var discussions = await githubRepository.GetDiscussions(author, nameRepo);

            var selectorDiscussions = discussions?.Select(p => p.title);

            // var selectedIndex = ConsoleManager.SelectorRequest(selectorDiscussions.ToArray(), "Please select discussion:");
            var selectedIndex = 0;
            var comments = await githubRepository.GetDiscussions(author, nameRepo, discussions[selectedIndex].number);
            var selectorComments = comments.Select(p => p.body);
            ConsoleManager.SelectorRequest(selectorComments.ToArray(), "Please select comments:");

            Console.WriteLine("Goodbye, World!");
        }
    }

}