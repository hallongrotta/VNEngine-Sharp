using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;
using Studio;

using HSPE;
using System.Reflection;

using BlendShapesEditor = HSPE.AMModules.BlendShapesEditor;
using HSPE.AMModules;

namespace VNActor
{
    public static class AIPEUtils
    {

        public static T GetPrivate<T>(PoseController obj, string name)
        {
            var type = obj.GetType();
            var typefield = type.GetField(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
            return (T)typefield.GetValue(obj);
        }

        public static string GetCharaSettingsText(OCIChar ocichar)
            {
                var stringWriter = new StringWriter();
                var xmlTextWriter = new XmlTextWriter(stringWriter);
                xmlTextWriter.WriteStartElement("characterInfo");
                ocichar.charInfo.gameObject.GetComponent<PoseController>().SaveXml(xmlTextWriter);
                xmlTextWriter.WriteEndElement();
                return stringWriter.ToString();
            }

            public static void SetCharaSettingsText(OCIChar ocichar, string text)
            {
                //from System.IO import StringWriter
                var xmlDocument = new XmlDocument();
                xmlDocument.LoadXml(text);
                var node = xmlDocument.FirstChild;
                //from UnityEngine import GameObject
                ocichar.charInfo.gameObject.GetComponent<PoseController>().ScheduleLoad(node, null);
            }

            public static PoseController GetPoseController(OCIChar ocichar)
            {
                return ocichar.charInfo.gameObject.GetComponent<PoseController>();
            }

            public static List<AdvancedModeModule> GetModules(OCIChar ocichar)
            {
                var poseController = GetPoseController(ocichar);
                return GetPrivate<List<AdvancedModeModule>>(poseController, "_modules");
            }

            public static BlendShapesEditor GetBlendShapesEditor(OCIChar ocichar)
            {
                var modules = GetModules(ocichar);
                foreach (var mod in modules)
                {
                    if (mod is BlendShapesEditor bse)
                    {
                        return bse;
                    }
                }
                return null;
            }

            public static string GetBlendShapesXML(OCIChar ocichar)
            {
                var stringWriter = new StringWriter();
                var xmlTextWriter = new XmlTextWriter(stringWriter);
                xmlTextWriter.WriteStartElement("blendShapes");
                GetBlendShapesEditor(ocichar).SaveXml(xmlTextWriter);
                xmlTextWriter.WriteEndElement();
                return stringWriter.ToString();
            }

            public static Dictionary<string, Dictionary<string, float>> GetBlendShapesObj(OCIChar ocichar)
            {
                var settingData = GetBlendShapesXML(ocichar);
                var fullObj = new Dictionary<string, Dictionary<string, float>>
                {
                };
                try
                {
                    // load contents of setting data
                    var xDoc = new XmlDocument();
                    xDoc.LoadXml(settingData);
                    // find each setting and adjust
                    var xRoot = xDoc.FirstChild;
                    foreach (XmlElement nc in xRoot.FirstChild.ChildNodes)
                    {
                        if (nc.Name == "skinnedMesh")
                        {
                            var obj = new Dictionary<string, float>
                            {
                            };
                            foreach (XmlElement blsh in nc.ChildNodes)
                            {
                                var ind = blsh.GetAttribute("index").ToString();
                                var w = float.Parse(blsh.GetAttribute("weight"));
                                obj[ind] = w;
                            }
                            fullObj[nc.GetAttribute("name").ToString()] = obj;
                        }
                        else
                        {
                            Console.WriteLine(String.Format("XXPEBlendShapes Warning: unknown setting node '%s'", nc.Name));
                        }
                    }
                }
                catch
                {
                }
                return fullObj;
            }

            public static void SetBlendShapesXML(OCIChar ocichar, string text)
            {
                //from System.IO import StringWriter
                var xmlDocument = new XmlDocument();
                xmlDocument.LoadXml(text);
                var node = xmlDocument.FirstChild;
                GetBlendShapesEditor(ocichar).LoadXml(node);
            }

            public static void SetBlendShapesObj(OCIChar ocichar, Dictionary<string, Dictionary<string, float>> fullObj)
            {
                var stringWriter = new StringWriter();
                var xmlTextWriter = new XmlTextWriter(stringWriter);
                xmlTextWriter.WriteStartElement("blendShapes");
                //self.GetBlendShapesEditor(ocichar).SaveXml(xmlTextWriter)
                xmlTextWriter.WriteStartElement("skinnedMeshes");
                foreach (var smeshes in fullObj.Keys)
                {
                    xmlTextWriter.WriteStartElement("skinnedMesh");
                    xmlTextWriter.WriteAttributeString("name", smeshes.ToString());
                    foreach (var smesh in fullObj[smeshes].Keys)
                    {
                        xmlTextWriter.WriteStartElement("blendShape");
                        xmlTextWriter.WriteAttributeString("index", smesh.ToString());
                        xmlTextWriter.WriteAttributeString("weight", fullObj[smeshes][smesh].ToString());
                        xmlTextWriter.WriteEndElement();
                    }
                    xmlTextWriter.WriteEndElement();
                }
                xmlTextWriter.WriteEndElement();
                xmlTextWriter.WriteEndElement();
                //print stringWriter.ToString()
                var text = stringWriter.ToString();
                //pass
                SetBlendShapesXML(ocichar, text);
            }
        }
}
