using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using VNEngine;
using static SceneSaveState.Camera;
using static SceneSaveState.SceneConsole;
using static SceneSaveState.UI;
using static SceneSaveState.VNDataComponent;


namespace SceneSaveState
{
    internal class SceneConsoleCamComponent
    {
        private SceneConsole sc;
        private View[] camset;

        internal Camera CameraController
        {
            get
            {
                return Camera.Instance;
            }

        }

        internal SceneConsoleCamComponent()
        {

        }

        internal void DeleteSceneCam(Scene s, Camera c)
        {
            s.Remove();
            if (s.HasItems) s.Current.setCamera(c);
        }

        internal void addSceneCam(Scene s)
        {
            var camData = CameraController.export();
            s.Add(new View(camData));
        }

        internal void updateSceneCam(Scene s)
        {
            var camData = CameraController.export();
            s.Update(new View(camData));
        }

        

        // Copy/paste cam set
        internal void copyCamSet(Scene s)
        {
            camset = s.ExportItems();
        }

        internal void pasteCamSet(Scene s)
        {
            s.ImportItems(camset);
        }

        internal Warning? DrawCamSelect(Scene s, Camera c, SceneConsoleVNComponent vnComp, bool promptOnDelete)
        {

            Warning? warning = null;
            cam_scroll = GUILayout.BeginScrollView(cam_scroll, GUILayout.Height(185));
            for (int i = 0; i < s.Count - 0; i++)
            {
                var col = i == s.CurrentIndex ? SelectedTextColor : "#f9f9f9";
                var cam = s[i];
                var camText = cam.Name ?? $"Cam {i + 1}";

                GUILayout.BeginHorizontal();

                if (GUILayout.Button($"<color={col}>{camText}, {(int)cam.camData.fov}</color>", GUILayout.Width(ColumnWidth * 0.8f)))
                {
                    var camData = s.SetCurrent(i);
                    camData.setCamera(c, vnComp, isAnimated: false);
                }
                if (GUILayout.Button($"<color={col}>a</color>", GUILayout.Width(ColumnWidth * 0.2f)))
                {
                    var camData = s.SetCurrent(i);
                    camData.setCamera(c, vnComp, isAnimated: true);
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndScrollView();
            GUILayout.FlexibleSpace();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Add", GUILayout.Width(ColumnWidth * 0.7f)))
            {
                addSceneCam(s);
            }


            if (s.HasItems)
            {
                if (GUILayout.Button("Del", GUILayout.Width(ColumnWidth * 0.3f)))
                {
                    if (promptOnDelete)
                    {
                        warning = new Warning("Delete selected cam?", false, s.DeleteSceneCam);
                    }
                    else
                    {
                        s.DeleteSceneCam();
                    }
                }
            }
            GUILayout.EndHorizontal();
            if (s.Count > 0)
            {
                if (GUILayout.Button("Update", GUILayout.Width(ColumnWidth + 5)))
                {
                    updateSceneCam(s);
                }
            }
            GUILayout.BeginHorizontal();
            const string up = "\u2191";
            const string down = "\u2193";
            if (GUILayout.Button($"Cam {up}"))
            {
                s.MoveItemBack();
            }
            if (GUILayout.Button($"Cam {down}"))
            {
                s.MoveItemForward();
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            if (s.HasItems)
            {
                if (GUILayout.Button("Copy cams"))
                {
                    copyCamSet(s);
                }
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            if (camset != null)
            {
                if (GUILayout.Button("Paste cams"))
                {
                    pasteCamSet(s);
                }
            }
            GUILayout.EndHorizontal();
            return warning;
        }

    }
}
