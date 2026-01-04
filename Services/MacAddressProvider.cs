using LogManager.Interfaces;

namespace LogManager.Services;

public interface IMacAddressProvider
{
    Task<List<string>> GetMacAddressesAsync(string filePath);
}

public class MacAddressProvider : IMacAddressProvider
{
    private readonly LocalFileProvider _localProvider;
    private readonly SftpFileProvider _sftpProvider;

    public MacAddressProvider(LocalFileProvider localProvider, SftpFileProvider sftpProvider)
    {
        _localProvider = localProvider;
        _sftpProvider = sftpProvider;
    }

    public async Task<List<string>> GetMacAddressesAsync(string filePath)
    {
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
            
            var lines = content.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            return lines.Select(l => l.Trim()).Where(l => !string.IsNullOrWhiteSpace(l)).ToList(); 
        }
        catch (Exception)
        {
            return new List<string>();
        }
    }
}
