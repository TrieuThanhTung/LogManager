using LogManager.Interfaces;
using LogManager.Models;
using Microsoft.Extensions.Options;
using System.Text.RegularExpressions;

namespace LogManager.Services;

public class LogFilterService : ILogFilterStrategy
{
    private readonly AppConfiguration _config;

    public LogFilterService(IOptions<AppConfiguration> config)
    {
        _config = config.Value;
    }

    public Task<IEnumerable<FileMetadata>> ApplyFilterAsync(IEnumerable<FileMetadata> files, FilterOptions options)
    {
        var result = files;

        if (options.PassStatus.HasValue)
        {
            var pattern = options.PassStatus.Value ? _config.PassPattern : _config.FailPattern;
            var regex = new Regex(pattern, RegexOptions.IgnoreCase);
            result = result.Where(f => regex.IsMatch(f.Name));
        }

        if (options.MacAddresses != null && options.MacAddresses.Any())
        {
            result = result.Where(f => options.MacAddresses.Any(mac => f.Name.Contains(mac, StringComparison.OrdinalIgnoreCase)));
        }

        if (options.FromDate.HasValue)
        {
            result = result.Where(f => f.LastModified >= options.FromDate.Value);
        }
        if (options.ToDate.HasValue)
        {
            result = result.Where(f => f.LastModified <= options.ToDate.Value);
        }
        
        if (options.OnlyNewest)
        {
            if (options.MacAddresses != null && options.MacAddresses.Any())
            {
                var newestFiles = new List<FileMetadata>();
                foreach (var mac in options.MacAddresses)
                {
                    var macFiles = result.Where(f => f.Name.Contains(mac, StringComparison.OrdinalIgnoreCase));
                    
                    var newest = macFiles.OrderByDescending(f => f.LastModified).FirstOrDefault();
                    if (newest != null)
                    {
                        newestFiles.Add(newest);
                    }
                }
                result = newestFiles.GroupBy(f => f.FullPath).Select(g => g.First()); 
            }
            else
            {
                // Global Newest (Standard)
                var newest = result.OrderByDescending(f => f.LastModified).FirstOrDefault();
                result = newest != null ? new[] { newest } : Enumerable.Empty<FileMetadata>();
            }
        }

        return Task.FromResult(result);
    }
}
