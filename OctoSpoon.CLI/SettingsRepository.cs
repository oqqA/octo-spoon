using System.Text.Json;

namespace OctoSpoon.CLI
{
    public class SettingsRepository
    {
        private const string FileName = "settings.json";

        public static bool Save(Settings newSettings)
        {

            var json = JsonSerializer.Serialize(newSettings, new JsonSerializerOptions { WriteIndented = true });

            var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var path = Path.Combine(baseDirectory, FileName);
            File.WriteAllText(path, json);

            return true;
        }

        public static Settings Get()
        {
            var settings = new Settings();

            var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var path = Path.Combine(baseDirectory, FileName);

            if (File.Exists(path))
            {
                var json = File.ReadAllText(path);
                settings = JsonSerializer.Deserialize<Settings>(json) ?? new Settings();
            }
            else
            {
                settings.CachePathsToDiscussions = new List<CachePathDiscussion>();
            }

            return settings;
        }

        public static void SaveCache(string author, string nameRepository, int numberDiscussion)
        {
            var cache = new CachePathDiscussion()
            {
                Author = author,
                RepositoryName = nameRepository,
                DiscussionNumber = numberDiscussion
            };

            var settings = Get() ?? new Settings();
            settings?.CachePathsToDiscussions?.Add(cache);
            Save(settings ?? new Settings());
        }
    }
}