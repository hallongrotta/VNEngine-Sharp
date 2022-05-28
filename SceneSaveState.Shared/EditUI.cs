using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using VNEngine;
using static SceneSaveState.SceneConsole;

namespace SceneSaveState
{
    partial class UI
    {

        public static void DrawVNDataOptions()
        {
            if (Instance.currentVNData.enabled)
            {
                GUILayout.BeginVertical();
                GUILayout.BeginHorizontal();
                GUILayout.Label("Who say:", GUILayout.Width(90));
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("<", GUILayout.Width(20)))
                {
                    Instance.currentVNData.whosay = Instance.get_next_speaker(Instance.currentVNData.whosay, false);
                }
                if (GUILayout.Button(">", GUILayout.Width(20)))
                {
                    Instance.currentVNData.whosay = Instance.get_next_speaker(Instance.currentVNData.whosay, true);
                }
                GUILayout.EndHorizontal();
                Instance.currentVNData.whosay = GUILayout.TextField(Instance.currentVNData.whosay);

                GUILayout.BeginHorizontal();
                GUILayout.Label("What say:", GUILayout.Width(90));
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("X", GUILayout.Width(20)))
                {
                    Instance.currentVNData.whatsay = "";
                }
                if (GUILayout.Button("...", GUILayout.Width(20)))
                {
                    Instance.currentVNData.whatsay = "...";
                }
                GUILayout.EndHorizontal();
                Instance.currentVNData.whatsay = GUILayout.TextArea(Instance.currentVNData.whatsay, GUILayout.Height(85));
                GUILayout.EndVertical();
                /*GUILayout.BeginHorizontal();
                GUILayout.Label("  Adv VN cmds", GUILayout.Width(90));
                Instance.currentVNData.addvncmds = GUILayout.TextArea(Instance.currentVNData.addvncmds, GUILayout.Width(235), GUILayout.Height(55));
                if (GUILayout.Button("X", GUILayout.Width(20)))
                {
                    Instance.currentVNData.addvncmds = "";
                }
                // if GUILayout.Button("X", GUILayout.Width(20)):
                //     sc.cam_whatsay = ""
                // if GUILayout.Button("...", GUILayout.Width(20)):
                //     sc.cam_whatsay = "..."
                GUILayout.EndHorizontal();
                */
            }
        }

        static string scene_name = null;

        public static void DrawSceneTab()
        {
            GUILayout.BeginVertical();
            string col;
            
            if (Instance.block.Count > 0)
            {
                if (scene_name is null) {
                    scene_name = Instance.block.SceneStrings[Instance.block.currentSceneIndex];
                }

                scene_name = GUILayout.TextField(scene_name, GUILayout.Width(ColumnWidth));

                scene_scroll = GUILayout.BeginScrollView(scene_scroll, GUILayout.Width(ColumnWidth));

                if (Event.current.isKey && Event.current.keyCode == KeyCode.Return)
                {
                    if (scene_name == "")
                    {
                        Instance.block.CurrentScene.name = null;                    
                    }
                    else
                    {
                        Instance.block.CurrentScene.name = scene_name;
                        scene_name = null;
                    }
                    Instance.block.RebuildSceneStrings();
                }
              
                for (int i = 0; i < Instance.block.Count; i++)
                {
                    col = i == Instance.block.currentSceneIndex ? UI.SelectedTextColor : UI.NormalTextColor;
                    if (Instance.block.SceneStrings[i] is null)
                    {
                        Instance.block.SceneStrings[i] = $"Scene {i + 1}";
                    }
                    string scn_name = Instance.block.SceneStrings[i];
                    if (Instance.block[i].cams.Count > 0 && Instance.block[i].cams[0].addata.enabled)
                    {
                        VNCamera.VNData.addprops_struct addprops = Instance.block[i].cams[0].addata.addprops;
                        if (addprops.a1)
                        {
                            string sname = addprops.a1o.name;
                            if (sname != null && sname.Trim().Length > 0)
                            {
                                scn_name = sname + String.Format(" ({0})", i + 1);
                            }
                        }
                    }
                    if (GUILayout.Button(String.Format("<color={0}>{1}</color>", col, scn_name)))
                    {
                        scene_name = scn_name;
                        Instance.block.SetCurrent(i);
                        if (Instance.autoLoad.Value)
                        {
                            Instance.LoadCurrentScene();
                            // sc.cur_index = GUILayout.SelectionGrid(sc.cur_index,sc.scene_str_array,1)
                        }
                    }
                }
                GUILayout.EndScrollView();
            }
            
            if (Instance.block.HasScenes)
            {
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Move up"))
                {
                    Instance.block.move_scene_up();
                }
                if (GUILayout.Button("Move down"))
                {
                    Instance.block.move_scene_down();
                }
                DrawNextPrevButtons();
            }           
            GUILayout.EndVertical();           
        }

        public static void DrawCamSelect()
        {
            GUILayout.BeginVertical();
            string col;
            cam_scroll = GUILayout.BeginScrollView(cam_scroll, GUILayout.Height(185), GUILayout.Width(camviewwidth));
            for (int i = 0; i < Instance.block.CurrentScene.cams.Count - 0; i++)
            {
                if (i == Instance.block.currentCamIndex)
                {
                    col = UI.SelectedTextColor;
                }
                else
                {
                    col = "#f9f9f9";
                }
                var cam = Instance.block.CurrentScene.cams[i];
                VNCamera.VNData addparams = cam.addata;
                GUILayout.BeginHorizontal();
                // show name if available
                var camtxt = Instance.block.CamStrings[i];
                if (addparams.enabled)
                {
                    var addprops = addparams.addprops;
                    if (addprops.a1)
                    {
                        if (addprops.a1o.name != "")
                        {
                            camtxt = addprops.a1o.name;
                        }
                    }
                }
                if (GUILayout.Button(String.Format("<color={0}>{1}</color>", col, camtxt)))
                {
                    Instance.block.SetCurrentCam(i);
                    Instance.setCamera(isAnimated: false);
                }
                if (GUILayout.Button(String.Format("<color={0}>a</color>", col), GUILayout.Width(22)))
                {
                    Instance.block.SetCurrentCam(i);
                    Instance.setCamera(isAnimated: true);
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndScrollView();
            GUILayout.FlexibleSpace();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Add", GUILayout.Width(camviewwidth * 0.7f)))
            {
                Instance.changeSceneCam(CamTask.ADD);
            }
            if (Instance.block.CurrentScene.cams.Count > 0)
            {
                if (GUILayout.Button("Del", GUILayout.Width(camviewwidth * 0.3f)))
                {
                    if (Instance.promptOnDelete.Value)
                    {
                        warning_action = Instance.deleteSceneCam;
                        warning_param = new WarningParam_s("Delete selected cam?", false);
                    }
                    else
                    {
                        Instance.changeSceneCam(CamTask.DELETE);
                    }
                }
            }
            GUILayout.EndHorizontal();
            if (Instance.block.CurrentScene.cams.Count > 0)
            {
                if (GUILayout.Button("Update", GUILayout.Width(camviewwidth + 5)))
                {
                    Instance.changeSceneCam(CamTask.UPDATE);
                }
            }
            GUILayout.Label("Move cam:");
            GUILayout.BeginHorizontal();
            var up = "\u2191";
            var down = "\u2193";
            if (GUILayout.Button(up, GUILayout.Width(camviewwidth / 2)))
            {
                Instance.block.move_cam_up();
            }
            if (GUILayout.Button(down, GUILayout.Width(camviewwidth / 2)))
            {
                Instance.block.move_cam_down();
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            if (Instance.block.HasScenes && Instance.block.currentCamCount > 0)
            {
                if (GUILayout.Button("Copy cams", GUILayout.Width(camviewwidth)))
                {
                    Instance.copyCamSet();
                }
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            if (Instance.block.HasScenes && Instance.camset != null)
            {
                if (GUILayout.Button("Paste cams", GUILayout.Width(camviewwidth)))
                {
                    Instance.pasteCamSet();
                }
            }
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }

        public static void DrawEditButtons()
        {
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Add scene", GUILayout.Height(55), GUILayout.Width(ColumnWidth / 2)))
            {
                Instance.AddScene();
            }
            if (GUILayout.Button("Update scene", GUILayout.Height(55), GUILayout.Width(ColumnWidth / 2)))
            {
                Instance.UpdateScene();
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();      
            if (GUILayout.Button("Insert scene", GUILayout.Height(25), GUILayout.Width(ColumnWidth/2)))
            {
                Instance.InsertScene();
            }
            if (GUILayout.Button("Dup scene", GUILayout.Height(25), GUILayout.Width(ColumnWidth / 2)))
            {
                Instance.DuplicateScene();
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Delete scene"))
            {
                if (Instance.promptOnDelete.Value)
                {
                    warning_action = Instance.removeScene;
                    warning_param = new WarningParam_s("Delete selected scene?", false);
                }
                else
                {
                    Instance.removeScene();
                }
            }
            GUILayout.EndHorizontal();           
            GUILayout.EndVertical();
        }

        public static void DrawNextPrevButtons()
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Prev scene", GUILayout.Height(25), GUILayout.Width(ColumnWidth / 2)))
            {
                Instance.LoadPreviousScene();
            }
            if (GUILayout.Button("Next scene", GUILayout.Height(25), GUILayout.Width(ColumnWidth / 2)))
            {
                Instance.LoadNextScene();
            }
            GUILayout.EndHorizontal();
        }

        public static void sceneConsoleEditUI()
        {
            GUILayout.BeginHorizontal();
            // Column 1
            GUILayout.BeginVertical(GUILayout.Width(ColumnWidth));
            GUILayout.Label("Scenes");
            // Scene tab
            DrawSceneTab();
            GUILayout.EndVertical();

            // Column 2
            GUILayout.BeginVertical(GUILayout.Width(ColumnWidth));
            GUILayout.Label("Scene Cameras");
            GUILayout.BeginHorizontal();
            // Camera and character selection tabs
            if (Instance.block.Count > 0)
            {
                DrawCamSelect();            
            }
            else
            {
                GUILayout.FlexibleSpace();
            }
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            // Column 3         
            GUILayout.BeginVertical(GUILayout.Width(ColumnWidth));
            GUILayout.Label("Scene Controls");
            // Add scene, Load scene
            DrawEditButtons();
            if (!Instance.autoLoad.Value)
            {
                GUILayout.Space(10);
                if (GUILayout.Button("Load Scene", GUILayout.Height(35)))
                {
                    Instance.LoadCurrentScene();
                }
            }
            Instance.currentVNData.enabled = GUILayout.Toggle(Instance.currentVNData.enabled, "Use cam in Visual Novel");
            GUILayout.FlexibleSpace();
            DrawVNDataOptions();
            GUILayout.FlexibleSpace();
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }
    }
}
