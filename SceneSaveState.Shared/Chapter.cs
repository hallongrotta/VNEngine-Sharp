using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using VNEngine;

namespace SceneSaveState
{
    public class Chapter : Manager<Scene>, IManaged<Chapter>
    {
        public string name { get; set; }

        public string TypeName => "Chapter";

        public Chapter Copy()
        {
            throw new NotImplementedException();
        }

        public Chapter()
        {

        }

        public Chapter(List<Scene> scenes, string[] sceneNames) : base(scenes, sceneNames)
        {

        }

        public override Scene Update(int position, Scene newScene)
        {
            var oldScene = base.Update(position, newScene);
            newScene.cams = oldScene.cams;
            return oldScene;
        }

        public void ImportCamTextsCustom()
        {
            try
            {
                var text = File.ReadAllLines("sss_camtexts.out");
                var vndata = new VNCamera.VNData[text.Length];
                var i = 0;
                foreach (var line in text)
                {
                    var entries = line.Split('\t');
                    if (entries.Length == 2)
                    {
                        var data = new VNCamera.VNData();
                        data.whosay = entries[1];
                        data.whatsay = entries[0];
                        data.enabled = true;
                        vndata[i] = data;
                    }

                    i++;
                }

                var j = 0;
                foreach (var scene in Items)
                    foreach (var cam in scene.cams)
                    {
                        cam.addata = vndata[j];
                        j++;
                        if (j >= text.Length) break;
                    }
            }
            catch (Exception)
            {
            }
        }

        // export cam texts
        public void exportCamTexts()
        {
            var filename = Path.Combine(Directory.GetCurrentDirectory(), "sss_camtexts.xml");
            var writer =
                new XmlSerializer(typeof(List<VNCamera.VNData>));
            var data = new List<VNCamera.VNData>();
            foreach (var scene in Items)
                foreach (var cam in scene.cams)
                    data.Add(cam.addata);
            var file = File.Create(filename);
            writer.Serialize(file, data);
            file.Close();
        }

        // export cam texts
        public void importCamTexts()
        {
            var filename = "sss_camtexts.xml";
            try
            {
                var reader =
                    new XmlSerializer(typeof(List<VNCamera.VNData>));
                var file = new StreamReader(
                    filename);
                var j = 0;
                var data = reader.Deserialize(file) as List<VNCamera.VNData>;
                foreach (var scene in Items)
                    foreach (var cam in scene.cams)
                    {
                        cam.addata = data[j];
                        j++;
                        if (j >= data.Count) break;
                    }

                file.Close();
            }
            catch (Exception)
            {
            }
        }

        internal void SetCurrentMapForAllScenes()
        {
            foreach (var scene in Items)
            {
                scene.sys.map = Current.sys.map;
                scene.sys.map_pos = Current.sys.map_pos;
                scene.sys.map_rot = Current.sys.map_rot;
            }
        }

#if HS2
        internal void SetCurrentAccessoriesForAllScenes(string id)
        {
            var acc = Current.actors[id].accessoryCoordinate;
            foreach (Scene scene in Items)
            {
                scene.actors[id].accessoryCoordinate = acc;
            }
        }

        internal void SetCurrentClothesForAllScenes(string id)
        {
            var clothes = Current.actors[id].clothesCoordinate;
            foreach (Scene scene in Items)
            {
                scene.actors[id].clothesCoordinate = clothes;
            }
        }
#endif
    }
}
