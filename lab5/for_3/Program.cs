using System;
using System.ServiceProcess;
using System.IO;
using System.Threading.Tasks;

namespace Lab3
{
    static class Program
    {
        static async Task Main()
        {
            FileManager service = new FileManager();
            try
            {
                ServiceBase.Run(service);
            }
            catch (Exception ex)
            {
                using (StreamWriter sw = new StreamWriter(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Exceptions.txt"), true))
                {
                    await sw.WriteLineAsync($"{DateTime.Now:dd/MM/yyyy HH:mm:ss} Exception: {ex.Message}");
                }
            }
        }
    }
}
