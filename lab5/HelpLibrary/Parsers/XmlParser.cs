using System;
using System.Reflection;
using System.Xml;

namespace HelpLibrary
{
    public class XmlParser : IParsable
    {
        private readonly string currentpath;
        private readonly Type currenttype;
        public Xml(string path, Type type)
        {
            currentpath = path;
            currenttype = type;
        }
        public T GetOptions<T>()
        {
            object result = Activator.CreateInstance(typeof(T));
            if (result is null)
            {
                throw new ArgumentNullException($"{nameof(result)} is null");
            }
            PropertyInfo[] properties = typeof(T).GetProperties();
            foreach (PropertyInfo propertyInfo in properties)
            {
                Deserialize(propertyInfo, result, SearchNode<T>());
            }
            return (T)result;
        }
        private void Deserialize(PropertyInfo propertyInfo, object parent, XmlNode parentNode)
        {
            foreach (XmlNode node in parentNode.ChildNodes)
            {
                if (node.Name == propertyInfo.Name)
                {
                    if (propertyInfo.PropertyType.IsPrimitive || propertyInfo.PropertyType == typeof(string))
                    {
                        propertyInfo.SetValue(parent, Convert.ChangeType(node.InnerText, propertyInfo.PropertyType));
                    }
                    else if (propertyInfo.PropertyType.IsEnum)
                    {
                        propertyInfo.SetValue(parent, Enum.Parse(propertyInfo.PropertyType, node.InnerText));
                    }
                    else
                    {
                        Type subType = propertyInfo.PropertyType;
                        object subObject = Activator.CreateInstance(subType);
                        propertyInfo.SetValue(parent, subObject);
                        PropertyInfo[] subPropertyInfo = subType.GetProperties();
                        foreach (PropertyInfo subpropertyinfo in subPropertyInfo)
                        {
                            Deserialize(subpropertyinfo, subObject, node);
                        }
                    }
                }
            }
        }
        private XmlNode SearchNode<T>()
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(currentpath);
            if (typeof(T) == currenttype)
            {
                return xmlDocument.DocumentElement;
            }
            PropertyInfo[] properties = currenttype.GetProperties();
            XmlNode result = null;
            foreach (PropertyInfo propertyInfo in properties)
            {
                SearchNode<T>(propertyInfo, xmlDocument.DocumentElement, ref result);
            }
            if (result is null)
            {
                
                throw new ArgumentNullException($"{nameof(result)} is null");
            }
            return result;
        }
        private void SearchNode<T>(PropertyInfo propertyInfo, XmlNode parentNode, ref XmlNode result)
        {
            foreach (XmlNode node in parentNode.ChildNodes)
            {
                if (node.Name == propertyInfo.Name && propertyInfo.PropertyType == typeof(T) && result == null)
                {
                    result = node;

                    if (!propertyInfo.PropertyType.IsPrimitive && !(propertyInfo.PropertyType == typeof(string)))
                    {
                        Type subType = propertyInfo.PropertyType;

                        PropertyInfo[] subPropertyInfo = subType.GetProperties();
                        foreach (PropertyInfo subpropertyinfo in subPropertyInfo)
                        {
                            SearchNode<T>(subpropertyinfo, node, ref result);
                        }
                    }
                }
            }
        }
    }
}
