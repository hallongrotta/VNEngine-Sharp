using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;

namespace ExtPlugins
{
    public class AIPE
        : ExtPlugin
    {

        public AIPE()
            : base("AIPE")
        {
        }

        public static string GetCharaSettingsText(object ocichar)
        {
            clr.AddReference("System.Xml");
            var stringWriter = new StringWriter();
            var xmlTextWriter = new XmlTextWriter(stringWriter);
            xmlTextWriter.WriteStartElement("characterInfo");
            ocichar.charInfo.gameObject.GetComponent(PoseController).SaveXml(xmlTextWriter);
            xmlTextWriter.WriteEndElement();
            return stringWriter.ToString();
        }

        public static void SetCharaSettingsText(object ocichar, object text)
        {
            clr.AddReference("System.Xml");
            var xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(text);
            var node = xmlDocument.FirstChild;
            ocichar.charInfo.gameObject.GetComponent(PoseController).ScheduleLoad(node, null);
        }
    }
}
