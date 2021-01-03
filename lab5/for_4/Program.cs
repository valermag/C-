using System;
using System.ServiceProcess;
using System.IO;
using HelpLibrary;
using System.Xml.Schema;
using System.Xml.Linq;

namespace Lab4
{
    static class Program
    {
        static readonly string xmlPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Parsers","configs", "config.xml");
        static readonly string xsdPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Parsers","configs", "config.xsd");
        static readonly string jsonPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Parsers","configs", "appsettings.json");
        static async Task Main()
        {
            ConfigManager configManager;
            DataOptions dataOptions;
            DataBaseWorker appInsights;
            try
            {
                if (File.Exists(xmlPath) && File.Exists(xsdPath))
                {
                    XmlSchemaSet schema = new XmlSchemaSet();
                    schema.Add(string.Empty, xsdPath);
                    XDocument xdoc = XDocument.Load(xmlPath);
                    xdoc.Validate(schema, ValidationEventHandler);
                    configManager = new ConfigManager(xmlPath, typeof(DataOptions));
                }
                else if (File.Exists(jsonPath))
                {
                    configManager = new ConfigManager(jsonPath, typeof(DataOptions));
                }
                else
                {
                    throw new Exception("No options");
                }
                dataOptions = configManager.GetOptions<DataOptions>();
                appInsights = new DataBaseWorker(dataOptions.LoggerConnectionString);
                await appInsights.ClearInsightsAsync();
                await appInsights.InsertInsightAsync("Connection established");
            }
            catch (Exception ex)
            {
                using (StreamWriter sw = new StreamWriter(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Exceptions.txt"), true))
                {
                    await sw.WriteLineAsync($"{DateTime.Now:dd/MM/yyyy HH:mm:ss} Exception: {ex.Message}");
                }
                return;
            }
            try
            {
                DataManager service = new DataManager(dataOptions, appInsights);
                ServiceBase.Run(service);
            }
            catch (Exception ex)
            {
                await appInsights.InsertInsightAsync("Exception: " + ex.Message);
                await appInsights.WriteInsightsToXmlAsync(dataOptions.OutputFolder);
            }
        }
        static void ValidationEventHandler(object sender, ValidationEventArgs e)
        {
            if (Enum.TryParse("Error", out XmlSeverityType type) && type == XmlSeverityType.Error)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
