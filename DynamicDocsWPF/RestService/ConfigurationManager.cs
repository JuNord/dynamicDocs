using System.IO;
using Newtonsoft.Json;

namespace RestService
{
    public class ConfigurationManager
    {
        private static readonly ConfigurationManager ConfManager = new ConfigurationManager();

        private ConfigurationManager()
        {
        }

        public string Url
        {
            get => GetConfiguration().Url;
            set
            {
                var config = GetConfiguration();
                config.Url = value;
                Save(config);
            }
        }

        public static ConfigurationManager GetInstance()
        {
            return ConfManager;
        }

        private Configuration GetConfiguration()
        {
            if (File.Exists("config.conf"))
                try
                {
                    return JsonConvert.DeserializeObject<Configuration>(File.ReadAllText("config.conf"));
                }
                catch (JsonSerializationException)
                {
                }

            var standardConfig = new Configuration
            {
                Url = "http://localhost:8000/Service"
            };
            Save(standardConfig);
            return standardConfig;
        }

        private void Save(Configuration configuration)
        {
            File.WriteAllText("config.conf", JsonConvert.SerializeObject(configuration, Formatting.Indented));
        }
    }
}