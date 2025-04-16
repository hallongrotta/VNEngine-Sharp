using MessagePack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using UnityEngine;
using VNEngine;
using static SceneSaveState.UI;
using static SceneSaveState.VNDataComponent;

namespace SceneSaveState
{
    public class Chapter : Manager<Scene>, IManaged<Chapter>
    {
        public string Name { get; set; }

        public string TypeName => "Chapter";

        public Chapter Copy()
        {
            var bytes = Utils.SerializeData(this);
            return Utils.DeserializeData<Chapter>(bytes);
        }

        public Chapter() : base()
        {

        }

        public Chapter(List<Scene> scenes, string[] sceneNames) : base(scenes, sceneNames)
        {

        }

        internal void RemoveRole(string roleName)
        {
            foreach (var scene in this) scene.Remove(roleName);
        }

        internal override Scene Update(int position, Scene newScene)
        {
            var oldScene = base.Update(position, newScene);
            newScene.ImportItems(oldScene.ExportItems());
            return oldScene;
        }

        internal void RemoveScene()
        {
            if (HasItems) Remove();
        }

        internal void ImportCamTextsCustom()
        {
            try
            {
                var text = File.ReadAllLines("sss_camtexts.out");
                var vndata = new VNData[text.Length];
                var i = 0;
                foreach (var line in text)
                {
                    var entries = line.Split('\t');
                    if (entries.Length == 2)
                    {
                        var data = new VNData();
                        data.whosay = entries[1];
                        data.whatsay = entries[0];
                        data.enabled = true;
                        vndata[i] = data;
                    }

                    i++;
                }

                var j = 0;
                foreach (var scene in this)
                    foreach (var cam in scene)
                    {
                        cam.Add(vndata[j]);
                        j++;
                        if (j >= text.Length) break;
                    }
            }
            catch (Exception)
            {
            }
        }

        // export cam texts
        internal void exportCamTexts()
        {
            var filename = Path.Combine(Directory.GetCurrentDirectory(), "sss_camtexts.xml");
            var writer =
                new XmlSerializer(typeof(List<VNData>));
            var data = new List<VNData>();
            foreach (var scene in this)
                foreach (var cam in scene)
                    data.Add(cam.Current);
            var file = File.Create(filename);
            writer.Serialize(file, data);
            file.Close();
        }

        // export cam texts
        internal void importCamTexts()
        {
            var filename = "sss_camtexts.xml";
            try
            {
                var reader =
                    new XmlSerializer(typeof(List<VNData>));
                var file = new StreamReader(
                    filename);
                var j = 0;
                var data = reader.Deserialize(file) as List<VNData>;
                foreach (var scene in this)
                    foreach (var cam in scene)
                    {
                        cam.Add(data[j]);
                        j++;
                        if (j >= data.Count) break;
                    }

                file.Close();
            }
            catch (Exception)
            {
            }
        }

        internal void SetCurrentMapNumForAllScenes(int map_num)
        {
            foreach (var scene in this)
            {
                scene.sys.map = map_num;
            }
        }
        internal void SetCurrentMapTransformForAllScenes(Vector3 pos, Vector3 rot)
        {
            foreach (var scene in this)
            {
                scene.sys.map_pos = pos;
                scene.sys.map_rot = rot;
            }
        }

        internal Scene InsertScene(Scene s, CreateViewFunc viewFunc, bool autoAddCam)
        {
            return AddScene(s, viewFunc, autoAddCam, insert: true);
        }

        delegate Scene AddSceneFunc(Scene scene);
        internal Scene AddScene(Scene s, CreateViewFunc viewFunc, bool autoAddCam, bool insert = false)
        {
            var addSceneFunc = insert ? (AddSceneFunc)Insert : Add; 
            addSceneFunc(s);
            var _ = autoAddCam ? s.Add(viewFunc()) : null;
            return s;
        }

        internal Scene DuplicateScene()
        {
            if (!HasItems) return null;
            return DuplicateScene(Current);
        }

        private Scene DuplicateScene(Scene s)
        {
            var new_scene = s.Copy();
            Insert(new_scene);
            return new_scene;
        }


        internal Scene UpdateScene(Scene s)
        {
            if (!HasItems) return null;
            var oldscene = Update(s);
            s.Name = oldscene.Name;
            return s;
        }

        internal Scene SetCurrentScene(int i)
        {
            return SetCurrent(i);
        }

        internal View GoToNextView()
        {
            return Current.HasNext ? Current.Next() : Next().First();
        }

        internal View GoToPrevView()
        {
            return Current.HasNext ? Current.Back() : Back().Last();
        }

        public delegate Scene CreateSceneFunc();
        public delegate View CreateViewFunc();

        public Warning? DrawSceneEditButtons(CreateSceneFunc fn, CreateViewFunc viewFunc, bool promptOnDelete, bool autoAddCam)
        {
            GUILayout.BeginHorizontal();
            Warning? warning = null;
            var _ = GUILayout.Button("Add scene", GUILayout.Height(RowHeight * 2), GUILayout.Width(ColumnWidth * 0.5f)) ? AddScene(fn(), viewFunc, autoAddCam) : null;
            _ = GUILayout.Button("Update scene", GUILayout.Height(RowHeight * 2), GUILayout.Width(ColumnWidth * 0.5f)) ?  UpdateScene(fn()) : null;
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            _ = GUILayout.Button("Insert scene", GUILayout.Height(RowHeight), GUILayout.Width(ColumnWidth * 0.5f)) ? InsertScene(fn(), viewFunc, autoAddCam) : null;
            _ = GUILayout.Button("Dup scene", GUILayout.Height(RowHeight), GUILayout.Width(ColumnWidth * 0.5f)) ? DuplicateScene() : null;
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Delete scene"))
            {
                if (promptOnDelete)
                {

                    warning = new Warning("Delete selected scene?", false, RemoveScene);
                }
                else
                {
                    RemoveScene();
                }
            }
            GUILayout.EndHorizontal();
            return warning;
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
