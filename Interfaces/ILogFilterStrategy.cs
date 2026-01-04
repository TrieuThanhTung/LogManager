using LogManager.Interfaces; // For FileMetadata

namespace LogManager.Interfaces;

public class FilterOptions
{
    public bool OnlyNewest { get; set; }
    public List<string> MacAddresses { get; set; } = new();
    public bool? PassStatus { get; set; } 
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
}

public interface ILogFilterStrategy
{
    Task<IEnumerable<FileMetadata>> ApplyFilterAsync(IEnumerable<FileMetadata> files, FilterOptions options);
}
