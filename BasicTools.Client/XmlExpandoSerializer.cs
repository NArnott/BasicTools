using System.Collections;
using System.Dynamic;
using System.IO;
using System.Xml;

namespace BasicTools.Client
{
    static class XmlExpandoSerializer
    {
        public static string Serialize(object obj, Formatting formatting)
        {
            using StringWriter stringWriter = new();
            using XmlTextWriter writer = new(stringWriter)
            {
                Formatting = formatting
            };

            writer.WriteStartDocument();

            SerializeToXml(writer, "XmlDocument", obj);

            writer.WriteEndDocument();

            writer.Flush();
            stringWriter.Flush();

            return stringWriter.ToString();
        }

        static void SerializeToXml(XmlWriter writer, ExpandoObject obj)
        {
            foreach (var prop in obj)
            {
                SerializeToXml(writer, prop.Key, prop.Value);
            }
        }

        static void SerializeToXml(XmlWriter writer, string elementName, object obj)
        {
            if (obj != null)
            {
                switch (obj)
                {
                    case ExpandoObject eo:
                        writer.WriteStartElement(elementName);
                        SerializeToXml(writer, eo);
                        writer.WriteEndElement();
                        break;

                    case string s:
                        writer.WriteStartElement(elementName);
                        writer.WriteValue(s);
                        writer.WriteEndElement();
                        break;

                    case IEnumerable list:
                        SerializeToXml(writer, elementName, list);
                        break;
                }
            }
        }

        static void SerializeToXml(XmlWriter writer, string elementName, IEnumerable list)
        {
            foreach (object obj in list)
            {
                SerializeToXml(writer, elementName, obj);
            }
        }

    }
}
