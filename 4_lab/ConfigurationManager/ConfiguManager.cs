using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml;

namespace ConfigurationMeneger
{
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
