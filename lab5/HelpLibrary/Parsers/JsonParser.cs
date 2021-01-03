using System;
using System.IO;
using System.Reflection;
using System.Text.Json;

namespace HelpLibrary
{
    public class JsonParser : IParsable
    {
        private readonly string currentpath;
        private readonly Type currenttype;
        public JsonParser(string path, Type type)
        {
            this.currentpath = currentpath;
            this.currenttype = currenttype;
        }
        public T GetOptions<T>()
        {
            string json;
            using (StreamReader sr = new StreamReader(currentpath))
            {
                json = sr.ReadToEnd();
            }
            object result = Activator.CreateInstance(typeof(T));
            if (result is null)
            {
                throw new ArgumentNullException($"{nameof(result)} is null");
            }
            PropertyInfo[] properties = typeof(T).GetProperties();
            JsonElement rootNode = FindNode<T>(json);
            foreach (PropertyInfo pi in properties)
            {
                DeserializeRecursive(pi, result, rootNode);
            }
            return (T)result;
        }
        private void DeserializeRecursive(PropertyInfo pi, object parent, JsonElement parentNode)
        {
            if (pi.PropertyType.IsPrimitive || pi.PropertyType == typeof(string))
            {
                pi.SetValue(parent, Convert.ChangeType(parentNode.GetProperty(pi.Name).GetRawText().Trim('"'), pi.PropertyType));
            }
            else if (pi.PropertyType.IsEnum)
            {
                pi.SetValue(parent, Enum.Parse(pi.PropertyType, parentNode.GetProperty(pi.Name).GetRawText().Trim('"')));
            }
            else
            {
                Type subType = pi.PropertyType;
                object subObj = Activator.CreateInstance(subType);
                pi.SetValue(parent, subObj);
                PropertyInfo[] subPIs = subType.GetProperties();
                foreach (PropertyInfo spi in subPIs)
                {
                    DeserializeRecursive(spi, subObj, parentNode.GetProperty(pi.Name));
                }
            }
        }
        private JsonElement FindNode<T>(string json)
        {
            JsonDocument jsonDocument = JsonDocument.Parse(json);
            JsonElement root = jsonDocument.RootElement;
            if (typeof(T) == currenttype)
            {
                return root;
            }
            PropertyInfo[] properties = currenttype.GetProperties();
            JsonElement? result = null;
            foreach (PropertyInfo pi in properties)
            {
                SearchNode<T>(pi, root, ref result);
            }
            if (result is null)
            {
                throw new ArgumentNullException($"{nameof(result)} is null");
            }
            return (JsonElement)result;
        }
        private void SearchNode<T>(PropertyInfo pi, JsonElement parentNode, ref JsonElement? result)
        {
            if (result == null)
            {
                if (parentNode.TryGetProperty(pi.Name, out JsonElement element) && pi.PropertyType == typeof(T))
                {
                    result = (JsonElement?)element;
                    return;
                }
                else if (!pi.PropertyType.IsPrimitive && pi.PropertyType != typeof(string))
                {
                    Type subType = pi.PropertyType;

                    PropertyInfo[] subPIs = subType.GetProperties();
                    foreach (PropertyInfo spi in subPIs)
                    {
                        SearchNode<T>(spi, parentNode.GetProperty(spi.Name), ref result);
                    }
                }
            }
        }
    }
}
