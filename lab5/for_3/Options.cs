using System.IO.Compression;

namespace Lab3
{
    public class Options
    {
        public string SourcePath { get; set; }
        public string TargetPath { get; set; }
        public string LogFilePath { get; set; }
        public bool NeedToEncrypt { get; set; }
        public ArchiveOptions ArchiveOptions { get; set; }
        public Options() { }
        public Options(string sourcePath, string targetPath, string logFilePath, ArchiveOptions archiveOptions, bool needToEncrypt)
        {
            SourcePath = sourcePath;
            TargetPath = targetPath;
            LogFilePath = logFilePath;
            ArchiveOptions = archiveOptions;
            NeedToEncrypt = needToEncrypt;
        }
    }
}
