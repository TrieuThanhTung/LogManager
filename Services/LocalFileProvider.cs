using LogManager.Interfaces;

namespace LogManager.Services;

public class LocalFileProvider : IFileSourceProvider
{
    public Task<IEnumerable<FileMetadata>> GetFilesAsync(string path, bool recursive = false)
    {
        if (!Directory.Exists(path))
        {
            return Task.FromResult(Enumerable.Empty<FileMetadata>());
        }

        var directoryInfo = new DirectoryInfo(path);
        var searchOption = recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
        var files = directoryInfo.GetFiles("*.*", searchOption)
            .Select(f => new FileMetadata
            {
                Name = f.Name,
                FullPath = f.FullName,
                Size = f.Length,
                LastModified = f.LastWriteTime,
                IsDirectory = false
            });

        return Task.FromResult(files);
    }

    public Task<Stream> GetFileStreamAsync(string filePath)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException("File not found", filePath);
        }

        // Open with FileShare.ReadWrite to allow reading while being written to (common for logs)
        var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        return Task.FromResult<Stream>(stream);
    }

    public Task<bool> IsConnectedAsync()
    {
        return Task.FromResult(true);
    }
}
