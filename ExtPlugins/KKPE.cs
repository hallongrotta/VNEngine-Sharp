using System;
using System.Collections.Generic;
using System.Text;

namespace ExtPlugins
{
    public class KKPE
        : ExtPlugin
    {

        public KKPE()
        {
            //super(HSStudioNEOAddon, self).__init__("HSStudioNEOAddon") #can't get it to work
            ExtPlugin.@__init__(this, "KKPE");
        }

        public object GetCharaSettingsText(object ocichar)
        {
            clr.AddReference("System.Xml");
            var stringWriter = StringWriter();
            var xmlTextWriter = XmlTextWriter(stringWriter);
            // print self.studio.m_BackgroundCtrl.Load(ffile)
            // for obj in GameObject.FindObjectOfType(BackgroundCtrl):
            //obj = GameObject.FindObjectOfType(MainWindow)
            //obj.SaveChara(ocichar,xmlTextWriter)
            xmlTextWriter.WriteStartElement("characterInfo");
            ocichar.charInfo.gameObject.GetComponent(PoseController).SaveXml(xmlTextWriter);
            xmlTextWriter.WriteEndElement();
            return stringWriter.ToString();
        }

        public object SetCharaSettingsText(object ocichar, object text)
        {
            clr.AddReference("System.Xml");
            var xmlDocument = XmlDocument();
            xmlDocument.LoadXml(text);
            var node = xmlDocument.FirstChild;
            ocichar.charInfo.gameObject.GetComponent(PoseController).ScheduleLoad(node, null);
            // print self.studio.m_BackgroundCtrl.Load(ffile)
            // for obj in GameObject.FindObjectOfType(BackgroundCtrl):
            // obj = GameObject.FindObjectOfType(MainWindow)
            // obj.SaveChara(ocichar,xmlTextWriter)
            // xmlTextWriter.WriteStartElement("characterInfo")
            // ocichar.charInfo.gameObject.GetComponent(PoseController).SaveXml(xmlTextWriter)
            // xmlTextWriter.WriteEndElement()
            //
            // return stringWriter.ToString()
        }
    }
}
