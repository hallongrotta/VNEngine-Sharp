using System;
using System.Collections.Generic;
using System.Text;

namespace ExtPlugins
{
    public class HSStudioNEOExtSave
        : ExtPlugin
    {

        public HSStudioNEOExtSave()
        {
            //super(HSStudioNEOExtSave, self).__init__("HSStudioNEOExtSave") #can't get it to work
            ExtPlugin.@__init__(this, "HSStudioNEOExtSave");
        }

        // Save ext data
        public object SaveExtData(object filePath)
        {
            StudioNEOExtendSaveMgr.Instance.SaveExtData(filePath);
        }

        // Load ext data
        public object LoadExtData(object filePath)
        {
            StudioNEOExtendSaveMgr.Instance.LoadExtData(filePath);
            StudioNEOExtendSaveMgr.Instance.LoadExtDataRaw(filePath);
        }

        public object ExtDataGet()
        {
            clr.AddReference("System.Xml");
            //handlers = StudioNEOExtendSaveMgr.Instance.GetField("handlers", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
            var handlers = GetPrivate(StudioNEOExtendSaveMgr.Instance, "handlers");
            if (handlers.Count > 0)
            {
                var xmlDocument = XmlDocument();
                var newChild = xmlDocument.CreateXmlDeclaration("1.0", "utf-8", null);
                xmlDocument.AppendChild(newChild);
                var xmlElement = xmlDocument.CreateElement("ExtSave");
                xmlDocument.AppendChild(xmlElement);
                foreach (var handler in handlers)
                {
                    try
                    {
                        handler.OnSave(xmlElement);
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("Error in ExtSave handler save:", e);
                    }
                }
                //return xmlDocument.ToString()
                //from System.Xml import XmlTextWriter
                var stringWriter = StringWriter();
                xmlDocument.Save(stringWriter);
                return stringWriter.ToString();
            }
            return "";
        }

        public object ExtDataSet(object datastring)
        {
            clr.AddReference("System.Xml");
            // handlers = StudioNEOExtendSaveMgr.Instance.GetField("handlers", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
            var handlers = GetPrivate(StudioNEOExtendSaveMgr.Instance, "handlers");
            if (handlers.Count > 0)
            {
                var xmlDocument = XmlDocument();
                xmlDocument.LoadXml(datastring);
                var documentElement = xmlDocument.DocumentElement;
                foreach (var handler in handlers)
                {
                    try
                    {
                        handler.OnLoad(documentElement);
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("Error in ExtSave handler load:", e);
                    }
                }
            }
        }
    }
}
