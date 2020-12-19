using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess_Layer;
using ErrorLogger;
using Modelss.MModels;
using Service_Layer;

namespace DataManager
{
    public class DataManager
    {
        public string connectionString { get; set; }
        public string Pathtosave { get; set; }
        public string Filename { get; set; }
        public string Schemaname { get; set; }
        public string TargetFolderPath { get; set; }
        public void MakeTransaction(int num)
        {
            try
            {
                IFiller<PersonalInfo> filler = new Repository(connectionString, num);

                IxmlGeneratorService ser1 = new XmlPerson();
                
                ser1.GenerateXmlFile(filler.GetPersons(), Pathtosave, Filename);
                ser1.GenerateXsdSchema(Pathtosave, Filename, Schemaname);
            }
            catch (Exception ex)
            {
                IErLogger logger=new Logger();
                logger.WriteError(ex);
            }
          
        }
        public void SendInfo()
        {
            try
            {
                
                IFileTransferService ser1 = new Transactions();
                ser1.TransferFiles(Filename, Schemaname, Pathtosave, TargetFolderPath);
            }
            catch (Exception ex)
            {
                IErLogger logger = new Logger();
                logger.WriteError(ex);
            }
           
        }
    }
}
