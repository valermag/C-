using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace ClassLibrary2
{
    //Parts of main model 
  public   class Pathes
    {

        public string Source1 { get; set; }
        public string Target1 { get; set; }

        public string SchemaName { get; set; }
        public string ArchFolder { get; set; }
    }

   public  class CryptOptions
    {
        public int key { get; set; }
        public string Extension { get; set; }
        public string Cryptext { get; set; }

        public string Decryptf(string path)
        {
            var text = File.ReadAllText(path);
            byte[] buf = Encoding.Unicode.GetBytes(text);
            for (var i = 0; i < buf.Length; i++)
            {
                buf[i] = (byte)(buf[i] ^ key);
            }

            text = Encoding.Unicode.GetString(buf);
            var tpath = path.Remove(path.Length - Extension.Length) + Cryptext;
            File.WriteAllText(tpath, text);
            return tpath;
        }

        public string Encryptf(string newtargetpath)
        {
            var text = File.ReadAllText(newtargetpath);

            byte[] buf = Encoding.Unicode.GetBytes(text);
            for (var i = 0; i < buf.Length; i++)
            {
                buf[i] = (byte)(buf[i] ^ key);
            }

            text = Encoding.Unicode.GetString(buf);
            var delfile = newtargetpath;
            newtargetpath = newtargetpath.Remove(newtargetpath.Length - Cryptext.Length) + Extension;
            File.WriteAllText(newtargetpath, text);
            File.Delete(delfile);

            return newtargetpath;
        }
    }

    public class CopyOptions
    {
        public char w { get; set; }

        public string Copystr(string stpath, string Targetpath)
        {

            var i = stpath.Length - 1;
            while (w != stpath[i]) i--;
            var name = stpath.Substring(i);
            var newpath = Path.Combine(Targetpath, name);
            FileInfo fileInf = new FileInfo(stpath);
            if (fileInf.Exists)
            {
                fileInf.CopyTo(newpath, true);
            }

            return newpath;
        }
    }

    public class ArchiveOptions
    {
        public string Cryptextension { get; set; }

        
        public string ArchivizeCrypt(string tpath)
        {
            var stpath = tpath.Remove(tpath.Length - Cryptextension.Length) + ".gz";
            using (FileStream sourceStream = new FileStream(tpath, FileMode.Open))
            {
                using (FileStream targetStream = File.Create(stpath))
                {
                    using (GZipStream compressionStream = new GZipStream(targetStream, CompressionMode.Compress))
                    {
                        sourceStream.CopyTo(compressionStream);
                    }
                }
            }

            return stpath;
        }

        public string DearchivizeCrypt(string newpath)
        {
            string newtargetpath = newpath.Remove(newpath.Length - 3) + Cryptextension;
            using (FileStream sourceStream = new FileStream(newpath, FileMode.OpenOrCreate))
            {
                using (FileStream targetStream = File.Create(newtargetpath))
                {
                    using (GZipStream decompressionStream = new GZipStream(sourceStream, CompressionMode.Decompress))
                    {
                        decompressionStream.CopyTo(targetStream);
                    }
                }

                File.Delete(newpath);
            }

            return newtargetpath;
        }

       
    }

    public class Validationclass
    {
        public readonly string SchemaPath;

        public static bool per;

        public Validationclass(string path)
        {
            SchemaPath = path;
            per = true;
        }
        public bool CheckFile( string filePath)
        {
           
            XmlSchemaSet sc = new XmlSchemaSet();
            sc.Add(null, SchemaPath);

            XmlReaderSettings settings = new XmlReaderSettings();
            settings.ValidationType = ValidationType.Schema;
            settings.Schemas = sc;
            settings.ValidationEventHandler += new ValidationEventHandler(ValidationCallBack);

            // Create the XmlReader object.
            XmlReader reader = XmlReader.Create(Path.Combine(filePath), settings);

            // Parse the file. 
            while (reader.Read()) ;
            reader.Close();
            return per;
        }
        private static void ValidationCallBack(object sender, ValidationEventArgs e)
        {
            Console.WriteLine("Validation Error: {0}", e.Message);
            per= false;
        }
    }
    
 //Main model
     public class EtlXmlJsonOption
    {

        public Pathes pathes { get; set; }
        public ArchiveOptions archivizeOptions { get; set; }
        public CryptOptions cryptOptions { get; set; }
        public CopyOptions copyOptions { get; set; }

        public void Do(string filePath)
        {
            
         
            Validationclass val=new Validationclass(Path.Combine(pathes.Source1,pathes.SchemaName));
           if (val.CheckFile(filePath))
           {
               if (File.Exists(Path.Combine(pathes.Source1, pathes.SchemaName)))
               {
                 // FileInfo info=new FileInfo();
                   File.Delete(Path.Combine(pathes.Source1, pathes.SchemaName));
                }
              
               var cryptstr = cryptOptions.Decryptf(filePath);
               var archstr = archivizeOptions.ArchivizeCrypt(cryptstr);
               var newstr = copyOptions.Copystr(archstr, pathes.Target1);
               var newcrypt = archivizeOptions.DearchivizeCrypt(newstr);
               var getstr = cryptOptions.Encryptf(newcrypt);

               string folder = Path.Combine(pathes.Target1, pathes.ArchFolder);
               var stpath = getstr.Remove(getstr.Length - cryptOptions.Extension.Length) + ".gz";
               using (FileStream sourceStream = new FileStream(getstr, FileMode.Open))
               {
                   using (FileStream targetStream = File.Create(stpath))
                   {
                       using (GZipStream compressionStream = new GZipStream(targetStream, CompressionMode.Compress))
                       {
                           sourceStream.CopyTo(compressionStream);
                       }
                   }
               }
               var i = stpath.Length - 1;
               while (copyOptions.w != stpath[i]) i--;
               var name = stpath.Substring(i);
               var newpath = Path.Combine(folder, name);
               File.Copy(stpath, newpath);
            }
        }

    }
    //Class with parsers
     public class ParsOptions
    {
        private string path;

        public ParsOptions(string name)
        {
            path = name;
        }

        public T GetModel<T>()
        {
            var i = path.Length - 1;
            while (path[i] != '.') --i;
            var ext = path.Substring(i);
            if (ext == ".json")
            {
                IParser parser = new jsonParser(path, typeof(T));
                return parser.GetOptions<T>();
            }
            else
            {
                IParser parser = new XmlParser(path, typeof(T));
                return parser.GetOptions<T>();
            }
        }
    }

    //for parsers
     interface IParser
    {
        public T GetOptions<T>();
    }
    //json parser
     class jsonParser : IParser
    {
        private Type classs;
        private string path;

        public jsonParser(string p, Type tipe)
        {
            path = p;
            classs = tipe;
        }

        public T GetOptions<T>()
        {
            object result = Activator.CreateInstance(typeof(T));

            try
            {
                PropertyInfo[] properties = typeof(T).GetProperties();


                foreach (PropertyInfo pi in properties)
                {

                    
                    DeserializeRecursive(pi, result, FindElement<T>());
                }
            }
            catch (Exception)
            {
                result = null;

            }



            return (T)result;
        }

        private JsonElement FindElement<T>()
        {
            string json;
            using (StreamReader file = File.OpenText(path))
            {
                json = file.ReadToEnd();
            }

            JsonDocument doc = JsonDocument.Parse(json);

            if (typeof(T) == classs)
            {
                return doc.RootElement;
            }

            PropertyInfo[] properties = classs.GetProperties();

            JsonElement result = default;

            foreach (PropertyInfo pi in properties)
            {
                FindElementnotRecursive<T>(pi, doc.RootElement, ref result);
            }

            JsonElement d = default;
            if (result.Equals(d))
            {
                throw new ArgumentNullException($"{nameof(result)} is null");
            }

            return result;
        }

        private void FindElementnotRecursive<T>(PropertyInfo pi, JsonElement parentNode, ref JsonElement result)
        {
            foreach (var node in parentNode.EnumerateObject())
            {
                JsonElement doc = default;
                if (node.Name == pi.Name && pi.PropertyType == typeof(T) && result.Equals(doc))
                {
                   
                    result = node.Value;


                }
            }
        }

        private void DeserializeRecursive(PropertyInfo pi, object parent, JsonElement parentNode)
        {
            foreach (JsonProperty node in parentNode.EnumerateObject())
            {
                if (node.Name == pi.Name)
                {
                    if (pi.PropertyType == typeof(string))
                    {
                        

                        pi.SetValue(parent, Convert.ChangeType(node.Value.ToString(), pi.PropertyType));
                    }
                    else if (pi.PropertyType.IsPrimitive)
                    {
                        // Console.WriteLine($"{  pi.PropertyType}");
                        pi.SetValue(parent, Convert.ChangeType(node.Value.ToString(), pi.PropertyType));
                    }
                    else if (pi.PropertyType.IsEnum)
                    {

                        pi.SetValue(parent, Enum.Parse(pi.PropertyType, node.ToString()));
                    }
                    else
                    {
                        Type subType = pi.PropertyType;
                        object subObj = Activator.CreateInstance(subType);

                        pi.SetValue(parent, subObj);

                        PropertyInfo[] props = subType.GetProperties();
                        foreach (PropertyInfo ppi in props)
                        {
                            DeserializeRecursive(ppi, subObj, node.Value);
                        }
                    }
                }
            }
        }


    }
    //xml parser
    class XmlParser : IParser
    {
        private string path;

        private Type ttype;

        public XmlParser(string path, Type ttype)
        {
            this.path = path;
            this.ttype = ttype;
        }

        public T GetOptions<T>()
        {
            object result = Activator.CreateInstance(typeof(T));


            try
            {
                PropertyInfo[] properties = typeof(T).GetProperties();

                foreach (PropertyInfo pi in properties)
                {
                    DeserializeRecursive(pi, result, FindNode<T>());
                }
            }
            catch (Exception)
            {

                result = null;
            }


            return (T)result;
        }

        private void DeserializeRecursive(PropertyInfo pi, object parent, XmlNode parentNode)
        {
            foreach (XmlNode node in parentNode.ChildNodes)
            {
                if (node.Name == pi.Name)
                {
                    if (pi.PropertyType.IsPrimitive || pi.PropertyType == typeof(string))
                    {
                        pi.SetValue(parent, Convert.ChangeType(node.InnerText, pi.PropertyType));
                    }
                    else if (pi.PropertyType.IsEnum)
                    {
                        pi.SetValue(parent, Enum.Parse(pi.PropertyType, node.InnerText));
                    }
                    else
                    {
                        Type subType = pi.PropertyType;
                        object subObj = Activator.CreateInstance(subType);

                        pi.SetValue(parent, subObj);

                        PropertyInfo[] subPIs = subType.GetProperties();
                        foreach (PropertyInfo spi in subPIs)
                        {
                            DeserializeRecursive(spi, subObj, node);
                        }
                    }
                }
            }
        }

        private XmlNode FindNode<T>()
        {
            XmlDocument doc = new XmlDocument();

            doc.Load(path);

            if (typeof(T) == ttype)
            {
                return doc.DocumentElement;
            }

            PropertyInfo[] properties = ttype.GetProperties();

            XmlNode result = null;

            foreach (PropertyInfo ppi in properties)
            {
                FindNodeRecursive<T>(ppi, doc.DocumentElement, ref result);
            }

            if (result is null)
            {
                throw new ArgumentNullException($"{nameof(result)} is null");
            }

            return result;
        }

        private void FindNodeRecursive<T>(PropertyInfo pi, XmlNode parentNode, ref XmlNode result)
        {
            foreach (XmlNode node in parentNode.ChildNodes)
            {
                if (node.Name == pi.Name && pi.PropertyType == typeof(T) && result == null)
                {
                    result = node;

                    if (!pi.PropertyType.IsPrimitive && !(pi.PropertyType == typeof(string)))
                    {
                        Type subt = pi.PropertyType;

                        PropertyInfo[] props = subt.GetProperties();
                        foreach (PropertyInfo ppi in props)
                        {
                            FindNodeRecursive<T>(ppi, node, ref result);
                        }
                    }
                }
            }
        }
    }
}
