using LogManager.Forms;
using LogManager.Interfaces;
using LogManager.Models;
using LogManager.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace LogManager;

static class Program
{
    public static IServiceProvider ServiceProvider { get; private set; }
    public static IConfiguration Configuration { get; private set; }

    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();

        // Setup Configuration
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

        Configuration = builder.Build();

        // Setup Logging
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(Configuration)
            .CreateLogger();

        // Setup DI
        var services = new ServiceCollection();
        ConfigureServices(services);

        ServiceProvider = services.BuildServiceProvider();

        Application.Run(ServiceProvider.GetRequiredService<MainForm>());
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        // Configuration
        services.AddSingleton(Configuration);
        services.Configure<AppConfiguration>(Configuration.GetSection("AppConfiguration"));

        // Logging
        services.AddLogging(loggingBuilder => loggingBuilder.AddSerilog());

        // Forms
        services.AddTransient<MainForm>();

        // Services
        services.AddSingleton<LocalFileProvider>();
        services.AddTransient<SftpFileProvider>(); 
        services.AddTransient<MacAddressProvider>();

        services.AddTransient<IConfigManager, ConfigManager>();
        services.AddTransient<SettingsForm>();
        services.AddTransient<ILogFilterStrategy, LogFilterService>();
    }
}