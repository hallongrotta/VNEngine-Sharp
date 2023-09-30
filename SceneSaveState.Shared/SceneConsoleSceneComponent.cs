
using Manager;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using VNEngine;
using static SceneSaveState.SceneConsole;
using static SceneSaveState.UI;

namespace SceneSaveState
{
    internal class SceneConsoleSceneComponent
    {

        private readonly SceneConsole sc;
        private readonly bool autoAddCam;


        internal SceneConsoleSceneComponent(SceneConsole sceneConsole,  bool autoAddCam)
        {
            this.sc = sceneConsole;
            this.autoAddCam = autoAddCam;
        }

        internal Scene UpdateScene(Chapter c, Scene s)
        {
            if (!c.HasItems) return null;
            var oldscene = c.Update(s);
            s.Name = oldscene.Name;
            return s;
        }


        internal Scene InsertScene(Chapter chapter, Scene s, Camera c)
        {
            return AddScene(chapter, s, c, insert: true);
        }

        internal Scene AddScene(Chapter chapter, Scene s, Camera c, bool insert = false)
        {
            if (insert)
                chapter.Insert(s);
            else
                chapter.Add(s);

            if (autoAddCam)
                s.Add(new View(c.export()));

            return s;
        }

        // Remove stuff

        internal void RemoveScene(Chapter c)
        {
            if (c.HasItems) c.Remove();
        }

        internal Scene SetCurrentScene(Chapter currentChapter, int i)
        {
            var scene = currentChapter.SetCurrent(i);
            sc.LoadScene(scene);
            return scene;
        }











       









        internal Scene DuplicateScene(Chapter c)
        {
            var new_scene = c.Current.Copy();
            c.Insert(new_scene);
            return new_scene;
        }

        public Warning? DrawSceneEditButtons(Chapter c, Camera cam, bool promptOnDelete)
        {
            GUILayout.BeginHorizontal();
            Warning? warning = null;
            if (GUILayout.Button("Add scene", GUILayout.Height(RowHeight*2), GUILayout.Width(ColumnWidth * 0.5f)))
            {
                AddScene(c, sc.CreateScene(), cam, autoAddCam);
            }
            if (GUILayout.Button("Update scene", GUILayout.Height(RowHeight*2), GUILayout.Width(ColumnWidth * 0.5f)))
            {
                UpdateScene(c, sc.CreateScene());
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Insert scene", GUILayout.Height(RowHeight), GUILayout.Width(ColumnWidth * 0.5f)))
            {
                InsertScene(c, sc.CreateScene(), cam);
            }
            if (GUILayout.Button("Dup scene", GUILayout.Height(RowHeight), GUILayout.Width(ColumnWidth * 0.5f)))
            {
                DuplicateScene(c);
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Delete scene"))
            {
                if (promptOnDelete)
                {

                    warning = new Warning("Delete selected scene?", false, c.RemoveScene);
                }
                else
                {
                    c.RemoveScene();
                }
            }
            GUILayout.EndHorizontal();
            return warning;
        }

        




    }
}
