using LogManager.Interfaces;

namespace LogManager.Services;

public interface IMacAddressProvider
{
    Task<List<string>> GetMacAddressesAsync(string filePath);
}

public class MacAddressProvider : IMacAddressProvider
{
    // User requirement: "lấy file chứa macs: từ local hoặc folder chứa log trên server sftp."
    // So we need to support reading the MAC file from either source.
    // For simplicity, we can just read the stream.

    private readonly LocalFileProvider _localProvider;
    private readonly SftpFileProvider _sftpProvider;

    public MacAddressProvider(LocalFileProvider localProvider, SftpFileProvider sftpProvider)
    {
        _localProvider = localProvider;
        _sftpProvider = sftpProvider;
    }

    public async Task<List<string>> GetMacAddressesAsync(string filePath)
    {
        // TODO: Implement logic to choose provider based on filePath or config
        if (filePath.StartsWith("sftp:"))
        {
             return await GetMacAddressesAsync(_sftpProvider, filePath.Replace("sftp:", ""));
        }
        return await GetMacAddressesAsync(_localProvider, filePath);
    }

    public async Task<List<string>> GetMacAddressesAsync(IFileSourceProvider provider, string filePath)
    {
        try
        {
            using var stream = await provider.GetFileStreamAsync(filePath);
            using var reader = new StreamReader(stream);
            var content = await reader.ReadToEndAsync();
            
            // Assume MACs are line separated or comma separated? 
            // "lấy file chứa macs" -> get file containing macs. 
            // Let's assume line by line for now.
            var lines = content.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            return lines.Select(l => l.Trim()).Where(l => !string.IsNullOrWhiteSpace(l)).ToList(); // Add regex validation if needed
        }
        catch (Exception)
        {
            // Log error
            return new List<string>();
        }
    }
}
