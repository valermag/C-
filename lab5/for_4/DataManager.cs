using System.ServiceProcess;
using HelpLibrary;

namespace Lab4
{
    public partial class DataManager : ServiceBase
    {
        readonly DataBaseWorker appInsights;
        readonly DataOptions dataOptions;
        public DataManager(DataOptions dataOptions, DataBaseWorker appInsights)
        {
            InitializeComponent();
            this.dataOptions = dataOptions;
            this.appInsights = appInsights;
        }
        protected override void OnStart(string[] args)
        {
            DataBaseWorker reader = new DataBaseWorker(dataOptions.ConnectionString);
            FileTransfer fileTransfer = new FileTransfer(dataOptions.OutputFolder, dataOptions.SourcePath);
            string customersFileName = "customers";
            await reader.GetCustomersAsync(dataOptions.OutputFolder, appInsights, customersFileName);
            await fileTransfer.SendFileToFtpAsync($"{customersFileName}.xml");
            await fileTransfer.SendFileToFtpAsync($"{customersFileName}.xsd");
            await appInsights.InsertInsightAsync("Files uploaded");
        }
        protected override void OnStop()
        {
            await appInsights.InsertInsightAsync("Stop");
            await appInsights.WriteInsightsToXmlAsync(dataOptions.OutputFolder);
        }
    }
}
