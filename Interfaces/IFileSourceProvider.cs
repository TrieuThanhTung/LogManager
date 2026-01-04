namespace LogManager.Interfaces;

public class FileMetadata
{
    public string Name { get; set; } = string.Empty;
    public string FullPath { get; set; } = string.Empty;
    public long Size { get; set; }
    public DateTime LastModified { get; set; }
    public bool IsDirectory { get; set; }
}

public interface IFileSourceProvider
{
    Task<IEnumerable<FileMetadata>> GetFilesAsync(string path, bool recursive = false);
    Task<Stream> GetFileStreamAsync(string filePath);
    Task<bool> IsConnectedAsync();
}
