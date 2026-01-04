using LogManager.Interfaces; // For FileMetadata

namespace LogManager.Interfaces;

public class FilterOptions
{
    public bool OnlyNewest { get; set; }
    public List<string> MacAddresses { get; set; } = new();
    public bool? PassStatus { get; set; } // null: all, true: pass, false: fail
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
}

public interface ILogFilterStrategy
{
    Task<IEnumerable<FileMetadata>> ApplyFilterAsync(IEnumerable<FileMetadata> files, FilterOptions options);
}
