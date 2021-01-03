using System.IO;

namespace HelpLibrary
{
    public class FileTransfer
    {
        readonly string ftpFolder;
        readonly string outputFolder;
        public FileTransfer(string outputFolder, string ftpFolder)
        {
            this.ftpFolder = ftpFolder;
            this.outputFolder = outputFolder;
        }
        public Task SendFileToFtpAsync(string fileName)
        {
            return Task.Run(() =>
            {
                if (File.Exists(Path.Combine(ftpFolder, fileName)))
                {
                    File.Delete(Path.Combine(ftpFolder, fileName));
                }
                File.Copy(Path.Combine(outputFolder, fileName), Path.Combine(ftpFolder, fileName));
            });
        }
    }
}
