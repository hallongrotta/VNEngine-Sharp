using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SceneSaveState
{
    internal class SceneManager
    {

        private List<Scene> scenes;

        private int sceneIndex;
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
        private string[] scene_str_array;

        public int currentSceneIndex { get { return sceneIndex; } private set { sceneIndex = value; } }

        public Scene CurrentScene { get { return scenes[currentSceneIndex]; } private set { scenes[currentSceneIndex] = value; } }

        public int Count { get { return scenes.Count; } }

        public SceneManager()
        {
            scenes = new List<Scene>();
            currentSceneIndex = -1;
        }

        public SceneManager(Scene[] scenes)
        {
            this.scenes = scenes.ToList();
            currentSceneIndex = 0;
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
            currentSceneIndex++;
        }
        public void Insert(Scene s)
        {
            Insert(s, currentSceneIndex);
        }

        public void Insert(Scene s, int position)
        {
            scenes.Insert(position, s);
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

        public bool HasNext
        {
            get
            {
                return currentSceneIndex < scenes.Count - 1;
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
    }
}
