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

        // 1. Pass/Fail Status Filter
        if (options.PassStatus.HasValue)
        {
            var pattern = options.PassStatus.Value ? _config.PassPattern : _config.FailPattern;
            var regex = new Regex(pattern, RegexOptions.IgnoreCase);
            result = result.Where(f => regex.IsMatch(f.Name));
        }

        // 2. MAC Address Filter
        if (options.MacAddresses != null && options.MacAddresses.Any())
        {
            // Requirement: "lấy theo macs hoặc lấy hết"
            // Filter files that contain any of the MAC addresses in their name OR content?
            // Usually "log file per mac" or "log file contains mac".
            // If the filename contains the MAC, passing it here is fast.
            // If checking content is needed, it's much slower and requires async stream reading.
            // User requirement: "options lọc các file logs: ... lấy theo macs hoặc lấy hết."
            // Assuming filename matching for now as it's standard for log management.
            // If content search is needed, we need to iterate and open each file.
            // Let's implement filename matching first.
            
            result = result.Where(f => options.MacAddresses.Any(mac => f.Name.Contains(mac, StringComparison.OrdinalIgnoreCase)));
        }

        // 3. Date Filter
        if (options.FromDate.HasValue)
        {
            result = result.Where(f => f.LastModified >= options.FromDate.Value);
        }
        if (options.ToDate.HasValue)
        {
            result = result.Where(f => f.LastModified <= options.ToDate.Value);
        }

        // 4. Only Newest
        // "chỉ lấy file mới nhất" -> Newest of ALL? Or newest per MAC?
        // Usually "Newest per Group" or just "Absolute Newest".
        // "lấy hết tất cả các file log theo yêu cầu, chỉ lấy file mới nhất." -> Get all requested logs, [OR] only get the newest file.
        // It sounds like a Toggle: "Get All" vs "Get Newest".
        // 4. Only Newest
        if (options.OnlyNewest)
        {
            if (options.MacAddresses != null && options.MacAddresses.Any())
            {
                // Newest Per MAC
                var newestFiles = new List<FileMetadata>();
                foreach (var mac in options.MacAddresses)
                {
                    // Find files for this MAC
                    var macFiles = result.Where(f => f.Name.Contains(mac, StringComparison.OrdinalIgnoreCase));
                    
                    // Take newest
                    var newest = macFiles.OrderByDescending(f => f.LastModified).FirstOrDefault();
                    if (newest != null)
                    {
                        newestFiles.Add(newest);
                    }
                }
                // Distinct to avoid duplicates if a file contains multiple MACs (edge case but possible)
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
