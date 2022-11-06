using System.Text.Json;

namespace OctoSpoon.CLI
{
    public class SettingsRepository
    {
        private const string FileName = "settings.json";

        public bool Save(Settings newSettings)
        {
            var json = JsonSerializer.Serialize(newSettings, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(FileName, json);

            return true;
        }

        public Settings Get()
        {
            var settings = new Settings();

            if (File.Exists(FileName))
            {
                var json = File.ReadAllText(FileName);
                settings = JsonSerializer.Deserialize<Settings>(json);

            }
            else
            {
                settings.CachePathsToDiscussions = new List<CachePathDuscussion>();
            }

            return settings;
        }
    }
}