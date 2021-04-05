using Newtonsoft.Json;
using OnlineSchoolSystem.Models.Configurations;
using System.IO;

namespace OnlineSchoolSystem.Client
{
    internal class ConfigurationManager
    {
        private readonly string _filePath;

        public ApplicationConfig Application;

        public ConfigurationManager(string filePath)
        {
            _filePath = filePath;

            if (File.Exists(_filePath) == false)
                throw new FileNotFoundException($"Не удается найти файл настроек", _filePath);

            var settingsJson = File.ReadAllText(_filePath);
            Application = JsonConvert.DeserializeObject<ApplicationConfig>(settingsJson);
        }
    }
}