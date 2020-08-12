﻿using System;
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
            Instance.scene_scroll = GUILayout.BeginScrollView(Instance.scene_scroll, GUILayout.Width(Instance.viewwidth));
            if (Instance.block.Count > 0)
            {
                for (int i = 0; i < Instance.block.Count; i++)
                {
                    if (i == Instance.currentSceneIndex)
                    {
                        col = Instance.sel_font_col;
                    }
                    else
                    {
                        col = Instance.nor_font_col;
                    }
                    string scn_name = Instance.scene_str_array[i];
                    if (Instance.block[i].cams.Count > 0 && Instance.block[i].cams[0].hasVNData && Instance.block[i].cams[0].addata.addparam)
                    {
                        addprops = Instance.block[i].cams[0].addata.addprops;
                        if (addprops.addprops["a1"])
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
                        Instance.currentSceneIndex = i;
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
                Instance.move_scene_up();
            }
            if (GUILayout.Button("Move down"))
            {
                Instance.move_scene_down();
            }
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            GUILayout.BeginVertical();
            // Camera and character selection tabs
            GUILayout.BeginHorizontal();
            if (Instance.currentSceneIndex > -1)
            {
                GUILayout.BeginVertical();
                Instance.cam_scroll = GUILayout.BeginScrollView(Instance.cam_scroll, GUILayout.Height(185), GUILayout.Width(Instance.camviewwidth));
                for ( int i = 0; i < Instance.block[Instance.currentSceneIndex].cams.Count - 0; i++)
                {
                    if (i == Instance.cur_cam)
                    {
                        col = Instance.sel_font_col;
                    }
                    else
                    {
                        col = "#f9f9f9";
                    }
                    var cam = Instance.block[Instance.currentSceneIndex].cams[i];
                    VNCamera.VNData addparams = cam.addata;
                    GUILayout.BeginHorizontal();
                    // show name if available
                    var camtxt = String.Format("Cam {0}", i.ToString());
                    if (addparams.addparam)
                    {
                        addprops = addparams.addprops;
                        if (addprops.addprops["a1"])
                        {
                            if (addprops.a1o.name != "")
                            {
                                camtxt = addprops.a1o.name;
                            }
                        }
                    }
                    if (GUILayout.Button(String.Format("<color={0}>{1}</color>", col, camtxt)))
                    {
                        Instance.cur_cam = i;
                        Instance.setCamera(false);
                    }
                    if (GUILayout.Button(String.Format("<color={0}>a</color>", col), GUILayout.Width(22)))
                    {
                        Instance.cur_cam = i;
                        Instance.setCamera(true);
                    }
                    GUILayout.EndHorizontal();
                }
                GUILayout.EndScrollView();
                // sc.cur_cam = GUILayout.SelectionGrid(sc.cur_cam,sc.scene_cam_str,1,GUILayout.Height(200),GUILayout.Width(125))
                // if not sc.cur_cam == prev_cam_index:
                // sc.setCamera()
                GUILayout.Space(15);
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Add", GUILayout.Width(Instance.camviewwidth * 0.7f)))
                {
                    Instance.changeSceneCam(CamTask.ADD);
                }
                if (GUILayout.Button("Del", GUILayout.Width(Instance.camviewwidth * 0.3f)))
                {
                    if (Instance.promptOnDelete)
                    {
                        Instance.warning_action = Instance.deleteSceneCam;
                        Instance.warning_param = new WarningParam_s("Delete selected cam?", CamTask.DELETE, false);
                    }
                    else
                    {
                        Instance.changeSceneCam(CamTask.DELETE);
                    }
                }
                GUILayout.EndHorizontal();
                if (GUILayout.Button("Update", GUILayout.Width(Instance.camviewwidth + 5)))
                {
                    Instance.changeSceneCam(CamTask.UPDATE);
                }
                GUILayout.Label("Move cam:");
                GUILayout.BeginHorizontal();
                var up = "\u2191";
                var down = "\u2193";
                if (GUILayout.Button(up, GUILayout.Width(Instance.camviewwidth / 2)))
                {
                    Instance.move_cam_up();
                }
                if (GUILayout.Button(down, GUILayout.Width(Instance.camviewwidth / 2)))
                {
                    Instance.move_cam_down();
                }
                GUILayout.EndHorizontal();
                GUILayout.EndVertical();
            }
            GUILayout.BeginVertical();
            if (true)
            {
                // len(sc.nameset[0])>0:
                Instance.fset_scroll = GUILayout.BeginScrollView(Instance.fset_scroll, GUILayout.Width(Instance.viewwidth), GUILayout.Height(Instance.viewheight));
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
                if (Instance.currentSceneIndex > -1 && Instance.cur_cam > -1)
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
                Instance.mset_scroll = GUILayout.BeginScrollView(Instance.mset_scroll, GUILayout.Width(Instance.viewwidth), GUILayout.Height(Instance.viewheight));
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
                if (Instance.currentSceneIndex > -1 && !Instance.camset.IsNullOrEmpty())
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
                Instance.addAuto(addsc: false);
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
                    Instance.warning_action = Instance.removeScene;
                    Instance.warning_param = new SceneConsole.WarningParam_s("Delete selected scene?", null, false);
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
            Instance.cam_addparam = GUILayout.Toggle(Instance.cam_addparam, "  Use cam in Visual Novel");
            GUILayout.FlexibleSpace();
            if (Instance.cam_addparam)
            {
                var txt = Utils.btntext_get_if_selected2("More", Instance.cam_addprops.addprops["a1"] || Instance.cam_addprops.addprops["a2"]);
                if (GUILayout.Button(txt, GUILayout.Height(20)))
                {
                    Instance.subwinindex = 100;
                }
            }
            GUILayout.EndHorizontal();
            //GUILayout.Label("  Replics for VN for cam (not necessary):")
            // if GUILayout.Button("Add scene (selected only)"):
            //     sc.addAuto(allbase=False)
            // if GUILayout.Button("Delete duplicate characters"):
            //     sc.removeDuplicates()
            if (Instance.cam_addparam)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("  Who say:", GUILayout.Width(90));
                Instance.cam_whosay = GUILayout.TextField(Instance.cam_whosay, GUILayout.Width(210));
                if (GUILayout.Button("<", GUILayout.Width(20)))
                {
                    Instance.cam_whosay = Instance.get_next_speaker(Instance.cam_whosay, false);
                }
                if (GUILayout.Button(">", GUILayout.Width(20)))
                {
                    Instance.cam_whosay = Instance.get_next_speaker(Instance.cam_whosay, true);
                }
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                GUILayout.Label("  What say:", GUILayout.Width(90));
                Instance.cam_whatsay = GUILayout.TextField(Instance.cam_whatsay, GUILayout.Width(210));
                if (GUILayout.Button("X", GUILayout.Width(20)))
                {
                    Instance.cam_whatsay = "";
                }
                if (GUILayout.Button("...", GUILayout.Width(20)))
                {
                    Instance.cam_whatsay = "...";
                }
                GUILayout.EndHorizontal();
                GUILayout.Space(5);
                GUILayout.BeginHorizontal();
                GUILayout.Label("  Adv VN cmds", GUILayout.Width(90));
                Instance.cam_addvncmds = GUILayout.TextArea(Instance.cam_addvncmds, GUILayout.Width(235), GUILayout.Height(55));
                if (GUILayout.Button("X", GUILayout.Width(20)))
                {
                    Instance.cam_addvncmds = "";
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
