using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static VNEngine.VNCamera;

namespace SceneSaveState
{
    internal class SceneManager
    {

        private List<Scene> scenes;

        public string[] SceneStrings
        {
            get
            {
                if ((scene_str_array == null) || scene_str_array.Length != scenes.Count)
                {
                    scene_str_array = new string[scenes.Count];
                    for (int id = 0; id < scenes.Count; id++)
                    {
                        scene_str_array[id] = $"Scene {id + 1}";
                    }
                    return scene_str_array;
                }
                else
                {
                    return scene_str_array;
                }
            }
        }

        public string[] CamStrings
        {
            get
            {
                if ((scene_cam_str == null) || scene_cam_str.Length != currentCamCount)
                {
                    scene_cam_str = new string[currentCamCount];
                    for (int id = 0; id < currentCamCount; id++)
                    {
                        scene_cam_str[id] = $"Cam {id}";
                    }
                    return scene_cam_str;
                }
                else
                {
                    return scene_cam_str;
                }
            }
        }

        public string[] scene_cam_str;
        private string[] scene_str_array;

        public int sceneIndex;

        public int currentSceneIndex { 
            get 
            { 
                return sceneIndex; 
            }
            private set 
            { 
                if (value < scenes.Count)
                {
                    if (value == -1 && scenes.Count == 0)
                    {
                        sceneIndex = value;
                    }
                    else if (value == -1 && scenes.Count > 0)
                    {
                        return;
                    }
                    else
                    {
                        sceneIndex = value;
                    }
                }
            } 
        }

        public int currentCamIndex { get; private set; }

        public Scene CurrentScene { 
            get 
            { 
                if (HasScenes) 
                { 
                    return scenes[currentSceneIndex]; 
                } 
                else 
                { return null; 
                } 
            } 
            private set 
            { scenes[currentSceneIndex] = value; 
            } 
        }

        public CamData CurrentCam { get { return CurrentScene.cams[currentCamIndex]; } }

        public int currentCamCount
        {
            get
            {
                if (CurrentScene.cams != null)
                {
                    return CurrentScene.cams.Count;
                }
                else
                {
                    return 0;
                }
            }
        }

        public int Count { get { return scenes.Count; } }

        public SceneManager()
        {
            scenes = new List<Scene>();
            currentSceneIndex = -1;
            currentCamIndex = -1;
        }

        public SceneManager(Scene[] scenes)
        {
            this.scenes = scenes.ToList();
            currentSceneIndex = 0;
            currentCamIndex = 0;
        }

        public bool HasScenes { get { return currentSceneIndex > -1; } }

        public Scene this[int index]
        {
            get
            {
                return scenes[index];
            }
            set
            {
                scenes[index] = value;
            }
        }

        public void Add(Scene s)
        {
            scenes.Add(s);
            currentSceneIndex = scenes.Count - 1;
        }
        public void Insert(Scene s)
        {
            Insert(s, currentSceneIndex);
        }

        public void Insert(Scene s, int position)
        {
            scenes.Insert(position + 1, s);
            currentSceneIndex++;
        }

        public void RemoveScene()
        {
            RemoveScene(currentSceneIndex);
        }

        public void Update(Scene s)
        {
            Update(currentSceneIndex, s);
        }

        public void Update(int position, Scene newScene)
        {
            if (position < scenes.Count)
            {
                var oldScene = scenes[position];
                newScene.cams = oldScene.cams;
                scenes[position] = newScene;
            }
        }

        public void RemoveScene(int position)
        {
            if (scenes.Count > 0 && position < scenes.Count && position > -1)
            {
                scenes.RemoveAt(position);
                currentSceneIndex--;
            }
            //scene_strings.RemoveAt(position);
            //scene_str_array = scene_strings.ToArray();
        }

        public void Duplicate()
        {
            if (scenes.Count > 0)
            {
                scenes.Insert(currentSceneIndex, scenes[currentSceneIndex].copy());
            }
        }

        // Move scene(up/down)
        public void move_scene_up()
        {
            if (scenes.Count > 1)
            {
                var cursc = CurrentScene;
                CurrentScene = scenes[currentSceneIndex - 1];
                currentSceneIndex -= 1;
                CurrentScene = cursc;
            }
        }

        public void move_scene_down()
        {
            if (currentSceneIndex < scenes.Count - 1)
            {
                var cursc = CurrentScene;
                CurrentScene = scenes[currentSceneIndex + 1];
                currentSceneIndex += 1;
                CurrentScene = cursc;
            }
        }

        // Move cam (up/down)
        public void move_cam_up()
        {
            if (HasScenes && currentCamIndex > 0)
            {
                var curcam = CurrentScene.cams[currentCamIndex];
                CurrentScene.cams[currentCamIndex] = CurrentScene.cams[currentCamIndex - 1];
                currentCamIndex -= 1;
                CurrentScene.cams[currentCamIndex] = curcam;
            }
        }

        public void move_cam_down()
        {
            if (HasScenes && currentCamIndex < currentCamCount - 1)
            {
                var curcam = CurrentScene.cams[currentCamIndex];
                CurrentScene.cams[currentCamIndex] = CurrentScene.cams[currentCamIndex + 1];
                currentCamIndex += 1;
                CurrentScene.cams[currentCamIndex] = curcam;
            }
        }

        public bool HasNext
        {
            get
            {
                return currentSceneIndex < scenes.Count - 1;
            }
        }

        public bool HasNextCam
        {
            get
            {
                return currentCamIndex < currentCamCount - 1;
            }
        }

        public bool HasPrevCam
        {
            get
            {
                return currentCamCount > 1;
            }
        }

        public bool HasPrev
        {
            get
            {
                return currentSceneIndex > 0;
            }
        }

        public Scene SetCurrent(int index)
        {
            if (HasScenes)
            {
                if (index < scenes.Count)
                {
                    currentSceneIndex = index;
                    return CurrentScene;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        public Scene Next()
        {
            if (HasNext)
            {
                currentSceneIndex++;
                return CurrentScene;
            }
            else
            {
                return null;
            }
        }

        public Scene Back()
        {
            if (HasPrev)
            {
                currentSceneIndex--;
                return CurrentScene;
            }
            else
            {
                return null;
            }
        }

        public Scene First()
        {
            if (HasScenes)
            {
                currentSceneIndex = 0;
                return CurrentScene;
            }
            else
            {
                return null;
            }
        }

        public Scene[] ExportScenes()
        {
            return scenes.ToArray();
        }


        public int AddCam(CamData camData)
        {
            currentCamIndex = CurrentScene.addCam(camData);
            return currentCamIndex;
        }

        public void UpdateCam(CamData camData)
        {
            CurrentScene.updateCam(currentCamIndex, camData);
        }

        public int DeleteCam()
        {
            if (currentCamCount > 1)
            {
                currentCamIndex = CurrentScene.deleteCam(currentCamIndex);
            }
            return currentCamIndex;
        }

        public int NextCam()
        {
            if (HasNextCam)
            {
                currentCamIndex++;
                return currentCamIndex;
            }
            else
            {
                return currentCamIndex;
            }
        }
        public int PrevCam()
        {
            if (HasPrevCam)
            {
                currentCamIndex--;
                return currentCamIndex;
            }
            else
            {
                return currentCamIndex;
            }

        }

        public void FirstCam()
        {
            currentCamIndex = 0;
        }

        public void LastCam()
        {
            currentCamIndex = currentCamCount - 1;
        }

        public void SetCurrentCam(int index)
        {
            if (index < currentCamCount)
            {
                currentCamIndex = index;
            }
        }

        public void ImportCamTextsCustom()
        {
            try
            {
                string[] text = File.ReadAllLines("sss_camtexts.out");
                VNData[] vndata = new VNData[text.Length];
                int i = 0;
                foreach (string line in text)
                {
                    string[] entries = line.Split('\t');
                    if (entries.Length == 2)
                    {
                        VNData data = new VNData();
                        data.whosay = entries[1];
                        data.whatsay = entries[0];
                        data.enabled = true;
                        vndata[i] = data;
                    }
                    i++;
                }
                int j = 0;
                foreach (Scene scene in scenes)
                {
                    foreach (CamData cam in scene.cams)
                    {
                        cam.addata = vndata[j];
                        j++;
                        if (j >= text.Length) { break; }
                    }
                }
            } catch (Exception)
            {
                return;
            }       
        }

        // export cam texts
        public void exportCamTexts()
        {
            var filename = Path.Combine(Directory.GetCurrentDirectory(), "sss_camtexts.xml");
            System.Xml.Serialization.XmlSerializer writer =
            new System.Xml.Serialization.XmlSerializer(typeof(List<VNData>));
            List<VNData> data = new List<VNData>();
            foreach (Scene scene in scenes)
            {
                foreach (CamData cam in scene.cams)
                {
                    data.Add(cam.addata);
                }
            }
            FileStream file = File.Create(filename);
            writer.Serialize(file, data);
            file.Close();
        }

        // export cam texts
        public void importCamTexts()
        {
            var filename = "sss_camtexts.xml";
            try
            {
                System.Xml.Serialization.XmlSerializer reader =
                new System.Xml.Serialization.XmlSerializer(typeof(List<VNData>));
                StreamReader file = new StreamReader(
                    filename);
                int j = 0;
                List<VNData> data = reader.Deserialize(file) as List<VNData>;
                foreach (Scene scene in scenes)
                {
                    foreach (CamData cam in scene.cams)
                    {
                        cam.addata = data[j];
                        j++;
                        if (j >= data.Count) { break; }
                    }
                }
                file.Close();
            }
            catch (Exception)
            {
                return;
            }
        }

    }
}
