using LogManager.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LogManager.Services;

public interface IConfigManager
{
    AppConfiguration LoadConfig();
    void SaveConfig(AppConfiguration config);
}

public class ConfigManager : IConfigManager
{
    private readonly string _configPath = "appsettings.json";

    public AppConfiguration LoadConfig()
    {
        // We load raw JSON to bind to our model, passing "AppConfiguration" section manually if needed
        // But simpler: just read file and deserialize section
        if (!File.Exists(_configPath)) return new AppConfiguration();

        var json = File.ReadAllText(_configPath);
        var jObject = JObject.Parse(json);
        var configSection = jObject["AppConfiguration"];
        return configSection?.ToObject<AppConfiguration>() ?? new AppConfiguration();
    }

    public void SaveConfig(AppConfiguration config)
    {
        // 1. Read existing to preserve other settings (like Serilog)
        var json = File.Exists(_configPath) ? File.ReadAllText(_configPath) : "{}";
        var jObject = JObject.Parse(json);

        // 2. Update section
        jObject["AppConfiguration"] = JToken.FromObject(config);

        // 3. Write back
        File.WriteAllText(_configPath, jObject.ToString(Formatting.Indented));

        // Note: For changes to take effect in IOptions<T>, we usually rely on reloadOnChange: true in Program.cs
        // However, IOptionsSnapshot is needed to see changes in scoped/transient services.
        // Singleton services (like MainForm if it were singleton) won't see it via IOptions.
        // Our MainForm calls serviceProvider to get SftpFileProvider (Transient), so it should get new options IF the configuration root reloads.
    }
}
