using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using VNEngine;
using static SceneSaveState.SceneConsole;

namespace SceneSaveState
{
    partial class UI
    {
        public static void sceneConsoleEditUI()
        {
            VNCamera.VNData.addprops_struct addprops;
            object col;
            List<string> fset = Instance.fset;
            List<string> mset = Instance.mset;
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical();
            // Scene tab
            scene_scroll = GUILayout.BeginScrollView(scene_scroll, GUILayout.Width(viewwidth));
            if (Instance.block.Count > 0)
            {
                for (int i = 0; i < Instance.block.Count; i++)
                {
                    if (i == Instance.block.currentSceneIndex)
                    {
                        col = Instance.sel_font_col;
                    }
                    else
                    {
                        col = Instance.nor_font_col;
                    }
                    string scn_name = Instance.block.SceneStrings[i];
                    if (Instance.block[i].cams.Count > 0 && Instance.block[i].cams[0].hasVNData && Instance.block[i].cams[0].addata.enabled)
                    {
                        addprops = Instance.block[i].cams[0].addata.addprops;
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
                        Instance.block.SetCurrent(i);
                        if (Instance.autoLoad == true)
                        {
                            Instance.loadCurrentScene();
                            // sc.cur_index = GUILayout.SelectionGrid(sc.cur_index,sc.scene_str_array,1)
                        }
                    }
                }
            }
            GUILayout.EndScrollView();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Move up"))
            {
                Instance.block.move_scene_up();
            }
            if (GUILayout.Button("Move down"))
            {
                Instance.block.move_scene_down();
            }
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            GUILayout.BeginVertical();
            // Camera and character selection tabs
            GUILayout.BeginHorizontal();
            if (Instance.block.Count > 0)
            {
                GUILayout.BeginVertical();
                cam_scroll = GUILayout.BeginScrollView(cam_scroll, GUILayout.Height(185), GUILayout.Width(camviewwidth));
                for ( int i = 0; i < Instance.block.CurrentScene.cams.Count - 0; i++)
                {
                    if (i == Instance.block.currentCamIndex)
                    {
                        col = Instance.sel_font_col;
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
                        addprops = addparams.addprops;
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
                // sc.cur_cam = GUILayout.SelectionGrid(sc.cur_cam,sc.scene_cam_str,1,GUILayout.Height(200),GUILayout.Width(125))
                // if not sc.cur_cam == prev_cam_index:
                // sc.setCamera()
                GUILayout.Space(15);
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Add", GUILayout.Width(camviewwidth * 0.7f)))
                {
                    Instance.changeSceneCam(CamTask.ADD);
                }
                if (Instance.block.CurrentScene.cams.Count > 0)
                {
                    if (GUILayout.Button("Del", GUILayout.Width(camviewwidth * 0.3f)))
                    {
                        if (Instance.promptOnDelete)
                        {
                            warning_action = Instance.deleteSceneCam;
                            warning_param = new WarningParam_s("Delete selected cam?", CamTask.DELETE, false);
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
                GUILayout.EndVertical();
            }
            GUILayout.BeginVertical();
            if (true)
            {
                // len(sc.nameset[0])>0:
                fset_scroll = GUILayout.BeginScrollView(fset_scroll, GUILayout.Width(viewwidth), GUILayout.Height(viewheight));
                for (int i = 0; i < fset.Count - 0; i++)
                {
                    if (i == Instance.fset_index)
                    {
                        col = Instance.sel_font_col;
                    }
                    else
                    {
                        col = Instance.nor_font_col;
                    }
                    if (GUILayout.Button(String.Format("<color={0}>{1}</color>", col, fset[i]), GUILayout.Height(40)))
                    {
                        Instance.fset_index = i;
                    }
                }
                GUILayout.EndScrollView();
                // sc.fset_index = GUILayout.SelectionGrid(sc.fset_index,fset,1,GUILayout.Height(200))
                // if GUILayout.Button("Change FChar"):
                //     sc.changeSceneChars(1, "upd")
                // if GUILayout.Button("Delete FChar"):
                //     if sc.promptOnDelete:
                //         sc.warning_param = (sc.changeSceneChars, "Delete selected female character?", (1, "del"), False)
                //     else:
                //         sc.changeSceneChars(1, "del")
                if (Instance.block.HasScenes && Instance.block.currentCamCount > 0)
                {
                    GUILayout.Space(25);
                    if (GUILayout.Button("Copy cam set"))
                    {
                        Instance.copyCamSet();
                    }
                }
            }
            GUILayout.EndVertical();
            GUILayout.BeginVertical();
            if (true)
            {
                // len(sc.nameset[1])>0:
                mset_scroll = GUILayout.BeginScrollView(mset_scroll, GUILayout.Width(viewwidth), GUILayout.Height(viewheight));
                for (int i = 0; i < mset.Count - 0; i++)
                {
                    if (i == Instance.mset_index)
                    {
                        col = Instance.sel_font_col;
                    }
                    else
                    {
                        col = Instance.nor_font_col;
                    }
                    if (GUILayout.Button(String.Format("<color={0}>{1}</color>", col, mset[i]), GUILayout.Height(40)))
                    {
                        Instance.mset_index = i;
                    }
                }
                GUILayout.EndScrollView();
                // sc.mset_index = GUILayout.SelectionGrid(sc.mset_index,mset,1,GUILayout.Height(200))
                // if GUILayout.Button("Change MChar"):
                //     sc.changeSceneChars(0, "upd")
                // if GUILayout.Button("Delete MChar"):
                //     if sc.promptOnDelete:
                //         sc.warning_param = (sc.changeSceneChars, "Delete selected male character?", (0, "del"), False)
                //     else:
                //         sc.changeSceneChars(0, "del")
                if (Instance.block.HasScenes && !Instance.camset.IsNullOrEmpty())
                {
                    GUILayout.Space(25);
                    if (GUILayout.Button("Paste cam set"))
                    {
                        Instance.pasteCamSet();
                    }
                }
            }
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
            GUILayout.Space(10);
            // Add scene, Load scene
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical();
            if (GUILayout.Button("Insert scene", GUILayout.Height(25)))
            {
                Instance.addAuto(insert: true);
            }
            if (GUILayout.Button("Dup scene", GUILayout.Height(25)))
            {
                Instance.dupScene();
            }
            GUILayout.EndVertical();
            if (GUILayout.Button("Add scene (auto)", GUILayout.Height(55), GUILayout.Width(175)))
            {
                Instance.addAuto();
            }
            if (GUILayout.Button("Update scene", GUILayout.Height(55)))
            {
                Instance.UpdateScene();
            }
            GUILayout.EndHorizontal();
            // if GUILayout.Button("Update props",GUILayout.Height(40)):
            // sc.addPropStates()
            GUILayout.BeginHorizontal();
            // if GUILayout.Button("Add scene (base)"):
            // sc.addAuto(allbase = True)
            // if GUILayout.Button("Add dup prop folders"):
            // sc.addPropFolders(dup=True)
            // if sc.cur_index > -1 and GUILayout.Button("Designate PropFolder", GUILayout.Height(25)):
            //     sc.addPropFolders()
            // if sc.cur_index > -1 and GUILayout.Button("Designate Container", GUILayout.Height(25)):
            //     sc.addProps()
            GUILayout.EndHorizontal();
            GUILayout.Space(10);
            if (GUILayout.Button("Delete scene"))
            {
                if (Instance.promptOnDelete == true)
                {
                    warning_action = Instance.removeScene;
                    warning_param = new WarningParam_s("Delete selected scene?", null, false);
                }
                else
                {
                    Instance.removeScene();
                }
            }
            GUILayout.BeginHorizontal();
            // if GUILayout.Button("Add scene (selected only)"):
            //     sc.addAuto(allbase=False)
            // if GUILayout.Button("Delete duplicate characters"):
            //     sc.removeDuplicates()
            GUILayout.EndHorizontal();
            if (!(Instance.autoLoad == true))
            {
                GUILayout.Space(10);
                if (GUILayout.Button("Load Scene", GUILayout.Height(35)))
                {
                    Instance.loadCurrentScene();
                }
            }
            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Load prev scene", GUILayout.Height(30)))
            {
                Instance.goto_prev_sc();
            }
            if (GUILayout.Button("Load next scene", GUILayout.Height(30)))
            {
                Instance.goto_next_sc();
            }
            GUILayout.EndHorizontal();
            // char texts
            GUILayout.Space(25);
            GUILayout.BeginHorizontal();
            Instance.currentVNData.enabled = GUILayout.Toggle(Instance.currentVNData.enabled, "  Use cam in Visual Novel");
            GUILayout.FlexibleSpace();
            if (Instance.currentVNData.enabled)
            {
                var txt = Utils.btntext_get_if_selected2("More", Instance.currentVNData.addprops.a1 || Instance.currentVNData.addprops.a2);
                if (GUILayout.Button(txt, GUILayout.Height(20)))
                {
                    subwinindex = 100;
                }
            }
            GUILayout.EndHorizontal();
            //GUILayout.Label("  Replics for VN for cam (not necessary):")
            // if GUILayout.Button("Add scene (selected only)"):
            //     sc.addAuto(allbase=False)
            // if GUILayout.Button("Delete duplicate characters"):
            //     sc.removeDuplicates()
            if (Instance.currentVNData.enabled)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("  Who say:", GUILayout.Width(90));
                Instance.currentVNData.whosay = GUILayout.TextField(Instance.currentVNData.whosay, GUILayout.Width(210));
                if (GUILayout.Button("<", GUILayout.Width(20)))
                {
                    Instance.currentVNData.whosay = Instance.get_next_speaker(Instance.currentVNData.whosay, false);
                }
                if (GUILayout.Button(">", GUILayout.Width(20)))
                {
                    Instance.currentVNData.whosay = Instance.get_next_speaker(Instance.currentVNData.whosay, true);
                }
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                GUILayout.Label("  What say:", GUILayout.Width(90));
                Instance.currentVNData.whatsay = GUILayout.TextField(Instance.currentVNData.whatsay, GUILayout.Width(210));
                if (GUILayout.Button("X", GUILayout.Width(20)))
                {
                    Instance.currentVNData.whatsay = "";
                }
                if (GUILayout.Button("...", GUILayout.Width(20)))
                {
                    Instance.currentVNData.whatsay = "...";
                }
                GUILayout.EndHorizontal();
                GUILayout.Space(5);
                GUILayout.BeginHorizontal();
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
            }
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
            // if not sc.prev_index == sc.cur_index and not sc.cur_index < 0:
            // sc.loadCurrentScene()
            // Minimize
        }
    }
}
