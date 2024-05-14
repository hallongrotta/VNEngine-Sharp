using HSPE.AMModules;
using HSPE;
using Studio;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.ConstrainedExecution;
using System.Text;
using static RootMotion.FinalIK.IKSolver;
using System.Xml;
using Housing;
using System.Reflection;
using JetBrains.Annotations;

namespace VNActor
{
    public static class XXPE
    {



        public static string GetCharaSettingsText(OCIChar ocichar)
        {
            var stringWriter = new StringWriter();
            var xmlTextWriter = new XmlTextWriter(stringWriter);

            xmlTextWriter.WriteStartElement("characterInfo");
            // here we work with OCIChar. OCIChar passed to function - this is chara control object
            // we SaveXML - it data in XML in xmlTextWriter
            GetPoseController(ocichar).SaveXml(xmlTextWriter);
            xmlTextWriter.WriteEndElement();

            // here we return result as string
            // vnactor expect you will provide result in some of standart datatypes:
            // lists, arrays, int, string, float, Vector3D etc.
            // NOT in XML structures!
            // so good way is to convert it
            return stringWriter.ToString();
        }


        // setting chara data
        public static void SetCharaSettingsText(OCIChar ocichar, string text)
        {
            var xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(text);
            var node = xmlDocument.FirstChild;
            GetPoseController(ocichar).ScheduleLoad(node, null);
        }


        public static PoseController GetPoseController(OCIChar ociobj)
        {
            return ociobj.charInfo.gameObject.GetComponent<PoseController>();
        }

        public static PoseController GetPoseController(OCIItem ociobj)
        {
            return ociobj.objectItem.GetComponent<PoseController>();
        }

        public static BlendShapesEditor GetBlendShapesEditor(OCIChar ocichar)
        {
            var poseController = GetPoseController(ocichar);
            var type = poseController.GetType();
            var fieldinfo = type.GetField("_blendShapesEditor", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
            var editor = fieldinfo.GetValue(poseController) as BlendShapesEditor;
            return editor;
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


        public static Dictionary<string, Dictionary<string, string>> GetBlendShapesObj(OCIChar ocichar) {

            var settingData = GetBlendShapesXML(ocichar);

            var fullObj = new Dictionary<string, Dictionary<string, string>>();
            try
            {
                // load contents of setting data
                var xDoc = new XmlDocument();
                xDoc.LoadXml(settingData);

                // find each setting and adjust
                var xRoot = xDoc.FirstChild;
                foreach (XmlElement nc in xRoot.FirstChild.ChildNodes) {
                    if (nc.Name == "skinnedMesh") {
                        var obj = new Dictionary<string, string>();
                        foreach (XmlElement blsh in nc.ChildNodes)
                        {
                            var ind = blsh.GetAttribute("index");
                            var w =   blsh.GetAttribute("weight");
                            obj[ind] = w;
                        }
                        fullObj[nc.GetAttribute("name")] = obj;
                    }
                    else
                    {
                        //print("XXPEBlendShapes Warning: unknown setting node '%s'" % nc.Name)
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"An error occurred while getting BlenShapes {e.Message}");
            }

            return fullObj;
        }


        public static void SetBlendShapesXML(OCIChar ocichar, string text)
        {
            var xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(text);
            var node = xmlDocument.FirstChild;
            GetBlendShapesEditor(ocichar).LoadXml(node);
        }




        public static void SetBlendShapesObj(OCIChar ocichar, Dictionary<string, Dictionary<string, string>> fullObj) {

            var stringWriter = new StringWriter();
            var xmlTextWriter = new XmlTextWriter(stringWriter);

            xmlTextWriter.WriteStartElement("blendShapes");
            xmlTextWriter.WriteStartElement("skinnedMeshes");
            foreach (var smeshes in fullObj.Keys) {
                xmlTextWriter.WriteStartElement("skinnedMesh");
                xmlTextWriter.WriteAttributeString("name", smeshes);
            foreach (var smesh in fullObj[smeshes].Keys) {
                    xmlTextWriter.WriteStartElement("blendShape");
                    xmlTextWriter.WriteAttributeString("index", smesh);
                    xmlTextWriter.WriteAttributeString("weight", fullObj[smeshes][smesh]);
                    xmlTextWriter.WriteEndElement();
    }
                xmlTextWriter.WriteEndElement();
}

            xmlTextWriter.WriteEndElement();
        xmlTextWriter.WriteEndElement();


            var text = stringWriter.ToString();

            SetBlendShapesXML(ocichar, text);

    }
    }
}
