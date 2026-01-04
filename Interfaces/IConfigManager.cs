using LogManager.Models;

namespace LogManager.Interfaces;

public interface IConfigManager
{
    AppConfiguration LoadConfig();
    void SaveConfig(AppConfiguration config);
}