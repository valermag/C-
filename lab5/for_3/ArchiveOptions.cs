using System.IO.Compression;

namespace Lab3
{
    public class ArchiveOptions
    {
        public ArchiveOptions() { }
        public bool NeedToArchive { get; set; }
        public CompressionLevel Level { get; set; }
    }
}
