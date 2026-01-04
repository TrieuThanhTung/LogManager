using LogManager.Interfaces;
using LogManager.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LogManager.Services;

public class ConfigManager : IConfigManager
{
    private readonly string _configPath = "appsettings.json";

    public AppConfiguration LoadConfig()
    {
        if (!File.Exists(_configPath)) return new AppConfiguration();

        var json = File.ReadAllText(_configPath);
        var jObject = JObject.Parse(json);
        var configSection = jObject["AppConfiguration"];
        return configSection?.ToObject<AppConfiguration>() ?? new AppConfiguration();
    }

    public void SaveConfig(AppConfiguration config)
    {
        var json = File.Exists(_configPath) ? File.ReadAllText(_configPath) : "{}";
        var jObject = JObject.Parse(json);
        
        jObject["AppConfiguration"] = JToken.FromObject(config);
        
        File.WriteAllText(_configPath, jObject.ToString(Formatting.Indented));
    }
}
