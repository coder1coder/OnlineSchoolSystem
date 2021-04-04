using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;

namespace OnlineSchoolSystem.Client
{
    internal class JsonSettings: ISettings
    {
        private readonly string _filePath;
        private readonly JObject _settings;

        public JsonSettings(string filePath)
        {
            _filePath = filePath;

            //create file with default values
            if (!File.Exists(_filePath))
                File.WriteAllText(_filePath, JsonConvert.SerializeObject(new {
                    authorizationEndpoint = "https://www.googleapis.com/youtube/v3/"
                }));

            var json = File.ReadAllText(_filePath);

            _settings = JObject.Parse(json);
        }

        public string Get(string key) => _settings[key]?.ToString() ?? "";

        public void Set(string key, string value)
        {
            if (!string.IsNullOrWhiteSpace(key) && !string.IsNullOrWhiteSpace(value))
            {
                _settings[key] = value;
                File.WriteAllText(_filePath, _settings.ToString());
            }            
        }
    }
}