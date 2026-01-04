using LogManager.Interfaces;
using LogManager.Models;
using Microsoft.Extensions.Options;
using Renci.SshNet;

namespace LogManager.Services;

public class SftpFileProvider : IFileSourceProvider, IDisposable
{
    private readonly SftpConfiguration _config;
    private SftpClient? _client;

    public SftpFileProvider(IOptions<AppConfiguration> config)
    {
        _config = config.Value.Sftp;
    }

    // Helper constructor for manual config if needed (e.g. from UI settings)
    public SftpFileProvider(SftpConfiguration config)
    {
        _config = config;
    }

    public async Task ConnectAsync()
    {
        if (_client != null && _client.IsConnected) return;

        _client = new SftpClient(_config.Host, _config.Port, _config.Username, _config.Password);
        await Task.Run(() => _client.Connect());
    }

    public async Task<IEnumerable<FileMetadata>> GetFilesAsync(string path, bool recursive = false)
    {
        if (!await IsConnectedAsync())
        {
            await ConnectAsync();
        }

        // Use configured path if input is empty, or specific path if provided
        string searchPath = string.IsNullOrWhiteSpace(path) ? _config.RemoteLogPath : path;

        return await Task.Run(() =>
        {
            return GetFilesInternal(searchPath, recursive);
        });
    }

    private IEnumerable<FileMetadata> GetFilesInternal(string path, bool recursive)
    {
         if (!_client!.Exists(path)) return Enumerable.Empty<FileMetadata>();

         var items = _client.ListDirectory(path).Where(f => !f.Name.StartsWith(".")); // Exclude . and ..
         var results = new List<FileMetadata>();

         foreach (var item in items)
         {
             if (item.IsDirectory)
             {
                 if (recursive)
                 {
                     results.AddRange(GetFilesInternal(item.FullName, true));
                 }
             }
             else
             {
                 results.Add(new FileMetadata
                 {
                     Name = item.Name,
                     FullPath = item.FullName,
                     Size = item.Length,
                     LastModified = item.LastWriteTime,
                     IsDirectory = false
                 });
             }
         }
         return results;
    }

    public async Task<Stream> GetFileStreamAsync(string filePath)
    {
        if (!await IsConnectedAsync())
        {
            await ConnectAsync();
        }

        var memoryStream = new MemoryStream();
        await Task.Run(() => _client!.DownloadFile(filePath, memoryStream));
        memoryStream.Position = 0;
        return memoryStream;
    }

    public Task<bool> IsConnectedAsync()
    {
        return Task.FromResult(_client != null && _client.IsConnected);
    }

    public void Dispose()
    {
        _client?.Disconnect();
        _client?.Dispose();
    }
}
