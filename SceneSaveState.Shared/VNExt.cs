using System;
using System.Collections.Generic;
using System.Text;
using VNEngine;
using VNActor;
using UnityEngine;
using System.Linq;
using static VNEngine.Utils;
using static VNEngine.VNCamera;
using Studio;
using static VNActor.Actor;
using AIProject;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace SceneSaveState
{
    public static class VNExt
    {

            // export to VNSceneScript function
            public static object add_folder_if_not_exists(string foldertxt, string folderfind, HSNeoOCI parentifcreate, bool overwrite = false)
            {
                var vnext = HSNeoOCIFolder.find_single_startswith(folderfind);
                if (vnext is HSNeoOCIFolder)
                {
                    if (overwrite)
                    {
                        vnext.name = foldertxt;
                    }
                    return vnext;
                }
                else
                {
                    return Utils.folder_add_child(parentifcreate, foldertxt);
                }
            }

            public static object add_folder_if_not_exists_dup(string foldertxt, HSNeoOCI parentifcreate)
            {
                return add_folder_if_not_exists(foldertxt, foldertxt, parentifcreate);
            }

            public static void exportToVNSS(SceneConsole sc, params object[] param)
            {
                string cmd;
                string fulltext;
                string action;
                VNEngine.VNCamera.VNData.addprops_struct addprops;
                VNEngine.VNCamera.VNData addparams;
                VNCamera.CamData cam;
                Scene scene;
                // parameters will be needed later
                // arCamMoveStyle = (
                //     "linear", "slow-fast", "fast-slow", "slow-fast3", "fast-slow3", "slow-fast4", "fast-slow4")
                // ---------------- making headers ----------------
                var vnss = HSNeoOCIFolder.find_single_startswith(":vnscenescript:");
                var calcMaxFrame = ((sc.block.Count + 1) * 100).ToString();
                // calcMaxFrame = 100000
                if (vnss == null)
                {
                    vnss = HSNeoOCIFolder.add(":vnscenescript:v30:" + calcMaxFrame);
                    //Utils.folder_add_child(vnss, ":useext:vnframe12")
                    //Utils.folder_add_child(vnss, ":a:i:f_stinit")
                    // Utils.folder_add_child(vnss, ":acode")
                }
                else
                {
                    // updating max frame
                    vnss.name = ":vnscenescript:v30:" + calcMaxFrame;
                    //vnext = HSNeoOCIFolder.find_single_startswith(":useext:vnframe")
                    //if vnext:
                    //    vnext.name = ":useext:vnframe12"
                    // add vnframe
                }
                add_folder_if_not_exists(":useext:vnframe12", ":useext:vnframe", vnss);
                add_folder_if_not_exists_dup(":a:i:f_stinit", vnss);
                // add vnanime
                add_folder_if_not_exists(":useext:vnanime10", ":useext:vnanime", vnss);
                add_folder_if_not_exists_dup(":a:i:f_clipinit", vnss);
                // add lipsync
                // add_folder_if_not_exists(":useext:flipsync10", ":useext:flipsync", vnss)
                // add_folder_if_not_exists(":a:i:initflipsync:v10", ":a:i:initflipsync:", vnss)
                // add object camera support for KK and AI
                if (sc.game.isCharaStudio || sc.game.isNEOV2)
                {
                    add_folder_if_not_exists(":useext:objcam10", ":useext:objcam", vnss);
                }
                // add acode folder
                var fld_acode = HSNeoOCIFolder.find_single(":acode");
                if (fld_acode == null)
                {
                    fld_acode = Utils.folder_add_child(vnss, ":acode");
                }
                // removing old code
                fld_acode.delete_all_children();
                // ------------ calculating list of names --------------
                var dictNames = new Dictionary<string, string>
                {
                };
                foreach (var i in Enumerable.Range(0, sc.block.Count))
                {
                    scene = sc.block[i];
                    foreach (var j in Enumerable.Range(0, scene.cams.Count))
                    {
                        cam = scene.cams[j];
                        addparams = cam.addata;
                        if (addparams.addparam)
                        {
                            addprops = addparams.addprops;
                            if (addprops.addprops.ContainsKey("a1") && addprops.addprops["a1"])
                            {
                                if (addprops.a1o.name != "")
                                {
                                    dictNames[addprops.a1o.name] = ((i + 1) * 100 + j).ToString();
                                }
                            }
                        }
                    }
                }
                //print "DictNames: ", dictNames
                // ---------------- actual render ------------------------
                Dictionary<string, ActorData> latestRenderScene = null;
                // adding new elems
                foreach (var i in Enumerable.Range(0, sc.block.Count))
                {
                    scene = sc.block[i];

                    // only process scene if 1 cam is VN cam - other, skip
                    cam = scene.cams[0];
                    addparams = cam.addata;
                    if (addparams.addparam)
                    {
                        // only process if 1 cam is VN cam
                        var fld_next = Utils.folder_add_child(fld_acode, "nextf:" + ((i + 1) * 100).ToString());
                        // making actions for switch to scene
                        foreach (var actid in scene.actors.Keys)
                        {
                            // calc optimized status
                            var optstatus = scene.actors[actid];
                            // delete cloth status if set in advance - by @countd360
                            if (actid != "sys")
                            {
                                try
                                {
                                    if (sc.skipClothesChanges)
                                    {
                                        optstatus.Remove("acc_all");
                                        optstatus.Remove("cloth_all");
                                        optstatus.Remove("cloth_type");
                                    }
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine("Error in skip cloth process when export to VNSceneScript:", e);
                                }
                            }
                            var isRenderOptimized = false;
                            if (latestRenderScene.IsNullOrEmpty() && isRenderOptimized)
                            {
                                optstatus = Utils.get_status_diff_optimized(latestRenderScene[actid], scene.actors[actid]);
                            }
                            if (actid != "sys")
                            {
                                action = ScriptHelper.script2string(optstatus);
                                fulltext = "f_actm:" + actid + "::" + action;
                            }
                            else
                            {
                                action = JsonSerializer.Serialize(optstatus);
                                fulltext = "f_actm_j:" + actid + "::" + action;
                            }
                            Utils.folder_add_child(fld_next, fulltext);
                        }
                        latestRenderScene = scene.actors;
                        foreach (var propid in scene.props.Keys)
                        {
                            action = ScriptHelper.script2string(scene.props[propid]);
                            fulltext = "f_actm:" + propid + "::" + action;
                            Utils.folder_add_child(fld_next, fulltext);
                        }
                        // make actions for switch to cam
                        foreach (var j in Enumerable.Range(0, scene.cams.Count))
                        {
                            cam = scene.cams[j];
                            addparams = cam.addata;
                            if (addparams.addparam)
                            {
                                // trick to render next
                                if (fld_next == null)
                                {
                                    //fld_next = Utils.folder_add_child(fld_acode, "next")
                                    fld_next = Utils.folder_add_child(fld_acode, "nextf:" + ((i + 1) * 100 + j).ToString());
                                }
                                // calc cam str:
                                var scamstr = String.Format("%0.3f,%0.3f,%0.3f,%0.3f,%0.3f,%0.3f,%0.3f,%0.3f,%0.3f,%0.2f", cam.position.x, cam.position.y, cam.position.z, cam.rotation.x, cam.rotation.y, cam.rotation.z, cam.distance.x, cam.distance.y, cam.distance.z, cam.fov);
                                var isCamAddRendered = false;
                                // calc wizard props
                                if (addparams.addparam)
                                {
                                    if (addparams.Contains("addprops"))
                                    {
                                        addprops = addparams.addprops;
                                        var a1o = addprops.a1o;
                                        if (a1o.isTime)
                                        {
                                            var time = a1o.time;
                                            if (a1o.isTAnimCam)
                                            {
                                                cmd = "camoanim3:" + scamstr + ":" + time + ":" + a1o.tacStyle;
                                                cmd += ":" + a1o.tacZOut + ":" + a1o.tacRotX + ":" + a1o.tacRotZ;
                                                isCamAddRendered = true;
                                                Utils.folder_add_child(fld_next, cmd);
                                            }
                                            if (a1o.isTHideUI)
                                            {
                                                cmd = "hideui:" + time;
                                                Utils.folder_add_child(fld_next, cmd);
                                            }
                                            if (a1o.isTTimerNext)
                                            {
                                                cmd = "timernext:" + (float.Parse(time) + 0.2).ToString();
                                                Utils.folder_add_child(fld_next, cmd);
                                            }
                                        }
                                        if (a1o.keepcam)
                                        {
                                            // keeping cam
                                            isCamAddRendered = true;
                                        }                                       
                                    }
                                }
                                // calc and add additional commands
                                if (addparams.Contains("addvncmds"))
                                {
                                    var addvncmds = addparams.addvncmds];
                                    // replacing names
                                    foreach (var key in dictNames.Keys)
                                    {
                                        addvncmds = addvncmds.Replace(String.Format("{{%s}}", key), dictNames[key]);
                                        addvncmds = addvncmds.Replace(String.Format("{%s}", key), dictNames[key]);
                                    }
                                    // is cam set in addvncmds? old version vnss 'camo:{camstr}'
                                    if (addvncmds.Contains("{camstr}"))
                                    {
                                        addvncmds = addvncmds.replace("{camstr}", scamstr);
                                        isCamAddRendered = true;
                                    }
                                    // save all vnss cmd
                                    var acmds = analysis_vnss_cmd(addvncmds);
                                    foreach (var acmd in acmds)
                                    {
                                        if (!acmd["error"])
                                        {
                                            Utils.folder_add_child(fld_next, acmd["org"]);
                                        }
                                        if (acmd["action"] == "objcam" && acmd["ap1"].Count > 0)
                                        {
                                            isCamAddRendered = true;
                                        }
                                    }
                                }
                                // making text action
                                var act = "txtf:" + addparams.whosay + "::" + addparams.whatsay;
                                Utils.folder_add_child(fld_next, act);
                                // making cam action
                                if (!isCamAddRendered)
                                {
                                    Utils.folder_add_child(fld_next, "camo:" + scamstr);
                                }
                                fld_next = null;
                                // tricky making special state to end - not needed
                                // fldlast = Utils.folder_add_child(fld_acode, "nextf:100000")
                                // Utils.folder_add_child(fldlast, "nextstate:"+calcMaxFrame)
                                // Utils.folder_add_child(fldlast, "timernext:0.1")
                            }
                        }
                    }
                }
            }

            public static bool runAdvVNSS(SceneConsole sc, VNData addata)
            {
                object vncmds;
                // run Adv VNSS
                // pick adv param 2 option
                var runvnframe = Utils.is_ini_value_true("RunVNFrameExtDefault");
                var runvnanime = Utils.is_ini_value_true("RunVNAnimeExtDefault");
                var runobjcam = Utils.is_ini_value_true("RunObjCamExtDefault");
                var keepCamera = false;
                try
                {
                    // check option
                    if (addata.add_props)
                    {
                        var addprops = addata.addprops;
                        if (addprops.addprops.ContainsKey("a1") && addprops.addprops["a1"])
                        {
                            var a1o = addprops.a1o;
                            keepCamera = a1o.keepcam;
                                //print "found a1:keepcam, keep camera = %s"%keepCamera                         
                        }
                        if (addprops.addprops.ContainsKey("a2") && addprops.addprops["a2"])
                        {
                            var a2o = addprops.a2o;
                            if (a2o.ContainsKey("run-vnframe-ext"))
                            {
                                runvnframe = a2o["run-vnframe-ext"];
                                //print "found a2:run-vnframe-ext, run vnframe ext = %s"%runvnframe
                            }
                            if (a2o.ContainsKey("run-vnanime-ext"))
                            {
                                runvnanime = a2o["run-vnanime-ext"];
                                //print "found a2:run-vnanime-ext, run vnanime ext = %s"%runvnanime
                            }
                            if (a2o.ContainsKey("run-objcam-ext"))
                            {
                                runobjcam = a2o["run-objcam-ext"];
                                //print "found a2:run-objcam-ext, run objcam ext = %s"%runobjcam
                                // prepare vncmds
                            }
                        }
                    }
                    if (addata.Contains("addvncmds"))
                    {
                        vncmds = analysis_vnss_cmd(addata["addvncmds"]);
                    }
                    else
                    {
                        vncmds = ValueTuple.Create("<Empty>");
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Fail to parse camera addate:", e);
                    return false;
                }
                foreach (var vncmd in vncmds)
                {
                    if (runvnframe && vncmd["catelog"] == "vnframe")
                    {
                        //print "Run vnframe-ext:", vncmd["org"]
                        runVNSSExtCmd(sc.game, vncmd);
                    }
                    if (runvnanime && vncmd["catelog"] == "vnanime")
                    {
                        //print "Run vnanime-ext:", vncmd["org"]
                        runVNSSExtCmd(sc.game, vncmd);
                    }
                    if (runobjcam && vncmd["catelog"] == "objcam")
                    {
                        //print "Run objcam-ext:", vncmd["org"]
                        runVNSSExtCmd(sc.game, vncmd);
                        if (vncmd["action"] == "objcam" && vncmd["ap1"].Count > 0)
                        {
                            keepCamera = true;
                        }
                    }
                }
                return keepCamera;
            }

            public static int tablePadding = 20;

            public static int btnBigHeight = 40;

            public static void render_wizard_ui(SceneConsole sc)
            {
                if (!hasattr(sc, "vnss_wizard_ui_scroll"))
                {
                    sc.vnss_wizard_ui_scroll = Vector2.zero;
                }
                sc.vnss_wizard_ui_scroll = GUILayout.BeginScrollView(sc.vnss_wizard_ui_scroll);
                // Title
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.Label("Advanced properties for cam for VNSceneScript");
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                GUILayout.Space(15);
                // vnss block
                GUILayout.BeginHorizontal();
                GUILayout.Space(tablePadding);
                GUILayout.BeginVertical();
                // who say line
                GUILayout.BeginHorizontal();
                GUILayout.Label("Who say:", GUILayout.Width(85));
                sc.cam_whosay = GUILayout.TextField(sc.cam_whosay, GUILayout.Width(210));
                if (GUILayout.Button("<", GUILayout.Width(20)))
                {
                    sc.cam_whosay = sc.get_next_speaker(sc.cam_whosay, false);
                }
                if (GUILayout.Button(">", GUILayout.Width(20)))
                {
                    sc.cam_whosay = sc.get_next_speaker(sc.cam_whosay, true);
                }
                GUILayout.EndHorizontal();
                // what say line
                GUILayout.BeginHorizontal();
                GUILayout.BeginVertical(GUILayout.Width(85));
                GUILayout.Label("What say:", GUILayout.Width(85));
                if (GUILayout.Button("clear", GUILayout.Width(60)))
                {
                    sc.cam_whatsay = "";
                }
                GUILayout.EndVertical();
                sc.cam_whatsay = GUILayout.TextArea(sc.cam_whatsay, GUILayout.Height(54));
                GUILayout.EndHorizontal();
                // VNSS line
                GUILayout.Space(5);
                GUILayout.BeginHorizontal();
                if (!hasattr(sc, "wiz_view_mode"))
                {
                    sc.wiz_view_mode = Utils.is_ini_value_true("VnssViewCmdModeDefault") ? "<color=#00ffff>cmds</color>" : "text";
                    //sc.wiz_cmd_view_sclpos = Vector2.zero
                }
                GUILayout.BeginVertical(GUILayout.Width(85));
                GUILayout.Label("VNSS cmds:", GUILayout.Width(85));
                if (GUILayout.Button("clear", GUILayout.Width(60)))
                {
                    sc.cam_addvncmds = "";
                }
                if (GUILayout.Button(sc.wiz_view_mode, GUILayout.Width(60)))
                {
                    if (sc.wiz_view_mode == "text")
                    {
                        sc.wiz_view_mode = "<color=#00ffff>cmds</color>";
                    }
                    else
                    {
                        sc.wiz_view_mode = "text";
                    }
                }
                GUILayout.EndVertical();
                if (sc.wiz_view_mode == "text")
                {
                    sc.cam_addvncmds = GUILayout.TextArea(sc.cam_addvncmds, GUILayout.Height(80));
                }
                else
                {
                    //sc.wiz_cmd_view_sclpos = GUILayout.BeginScrollView(sc.wiz_cmd_view_sclpos, GUILayout.Height(80))
                    GUILayout.BeginVertical();
                    var acmds = analysis_vnss_cmd(sc.cam_addvncmds);
                    foreach (var acmd in acmds)
                    {
                        GUILayout.BeginHorizontal();
                        if (!acmd["error"])
                        {
                            GUILayout.Label("<color=#00ffff>" + acmd["desp"] + "</color>");
                        }
                        else
                        {
                            GUILayout.Label("<color=#ff0000>" + acmd["desp"] + "</color>");
                        }
                        GUILayout.FlexibleSpace();
                        if (!acmd["error"] && acmd["runable"] && GUILayout.Button("<color=#00ffff>" + "\u25b6" + "</color>", GUILayout.Width(20)))
                        {
                            runVNSSExtCmd(sc.game, acmd);
                        }
                        if (GUILayout.Button("<color=#00ffff>X</color>", GUILayout.Width(20)))
                        {
                            acmds.remove(acmd);
                            sc.cam_addvncmds = "\n".join(from c in acmds
                                                          select c["org"]);
                        }
                        GUILayout.EndHorizontal();
                    }
                    if (acmds.Count == 0)
                    {
                        GUILayout.Label("<color=#00ffff>[NONE]</color>");
                    }
                    //GUILayout.EndScrollView()
                    GUILayout.EndVertical();
                }
                GUILayout.EndHorizontal();
                // VNSS wizard
                GUILayout.Space(5);
                render_vnss_wizard(sc);
                GUILayout.EndVertical();
                GUILayout.Space(tablePadding);
                GUILayout.EndHorizontal();
                GUILayout.Space(20);
                // adv parameters 1
                GUILayout.BeginHorizontal();
                GUILayout.Space(tablePadding);
                GUILayout.BeginVertical();
                sc.cam_addprops.addprops["a1"] = GUILayout.Toggle(sc.cam_addprops.addprops["a1"], "Advanced parameters 1");
                if (sc.cam_addprops.addprops["a1"])
                {
                    if (!(sc.cam_addprops.a1o is null))
                    {
                        adv_update(sc.cam_addprops.a1o, adv1_def_values());
                    }
                    else
                    {
                        sc.cam_addprops.a1o = adv1_def_values();
                    }
                    GUILayout.Space(5);
                    render_wizard_adv1(sc);
                }
                else if (sc.cam_addprops.Contains("a1o"))
                {
                    sc.cam_addprops.a1o = new adv_properties();
                }
                GUILayout.Space(10);
                sc.cam_addprops.addprops["a2"] = GUILayout.Toggle(sc.cam_addprops.addprops["a2"], "Advanced parameters 2");
                if (sc.cam_addprops.addprops["a2"])
                {
                    if (sc.cam_addprops.addprops.ContainsKey("a2o"))
                    {
                        adv_update(sc.cam_addprops.a2o, adv2_def_values());
                    }
                    else
                    {
                        sc.cam_addprops.a2o = adv2_def_values();
                    }
                    GUILayout.Space(5);
                    render_wizard_adv2(sc);
                }
                else if (!(sc.cam_addprops.a2o is null))
                {
                    sc.cam_addprops.a2o = null;
                }
                GUILayout.EndVertical();
                GUILayout.Space(tablePadding);
                GUILayout.EndHorizontal();
                GUILayout.Space(20);
                // save and exit
                GUILayout.BeginHorizontal();
                GUILayout.Space(tablePadding);
                if (GUILayout.Button("Save cam+parameters and return to main", GUILayout.Height(btnBigHeight)))
                {
                    sc.wiz_step = -1;
                    sc.changeSceneCam("upd");
                    sc.subwinindex = 0;
                }
                if (GUILayout.Button("Back", GUILayout.Height(btnBigHeight), GUILayout.Width(60)))
                {
                    sc.wiz_step = -1;
                    sc.subwinindex = 0;
                    var camera_data = sc.block[sc.cur_index].cams[sc.cur_cam];
                    var addata = camera_data.addata;
                    sc.cam_addparam = addata.addparam;
                    sc.cam_whosay = addata.whosay;
                    sc.cam_whatsay = addata.whatsay;
                    if (addata.Contains("addvncmds"))
                    {
                        sc.cam_addvncmds = addata["addvncmds"];
                    }
                    else
                    {
                        sc.cam_addvncmds = "";
                    }
                    if (addata.Contains("addprops"))
                    {
                        sc.cam_addprops = addata.addprops;
                    }
                    else
                    {
                        sc.cam_addprops.addprops = new Dictionary<string, bool> {
                    {
                        "a1",
                        false},
                    {
                        "a2",
                        false}};
                    }
                }
                GUILayout.Space(tablePadding);
                GUILayout.EndHorizontal();
                // end
                GUILayout.EndScrollView();
            }



        // --------- adv properties 1 --------
        public static adv_properties adv1_def_values()
        {
            adv_properties adv1 = new adv_properties
            {
                name = "",
                isTime = false,
                time = "2.0",
                isTAnimCam = false,
                isTHideUI = false,
                isTTimerNext = false,
                tacStyle = "fast-slow",
                tacZOut = "0.0",
                tacRotX = "0.0",
                tacRotZ = "0.0",
                keepcam = false,
            };
            return adv1;
        }

            public static object render_wizard_adv1(SceneConsole sc)
            {
                var cam_style_list = ("linear", "slow-fast", "fast-slow", "slow-fast3", "fast-slow3", "slow-fast4", "fast-slow4");
                GUILayout.BeginHorizontal();
                GUILayout.Label("Name (you can address by name):");
                sc.cam_addprops.a1o.name = GUILayout.TextField(sc.cam_addprops.a1o.name, GUILayout.Width(200));
                if (GUILayout.Button("X", GUILayout.Width(20)))
                {
                    sc.cam_addprops.a1o.name = "";
                }
                var tabWidth = 30;
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                sc.cam_addprops.a1o.isTime = GUILayout.Toggle(sc.cam_addprops.a1o.isTime, " In time ");
                sc.cam_addprops.a1o.time = GUILayout.TextField(sc.cam_addprops.a1o.time, GUILayout.Width(100));
                GUILayout.Label(" seconds do ");
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                if (sc.cam_addprops.a1o.isTime)
                {
                    //GUILayout.BeginHorizontal()
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(tabWidth);
                    sc.cam_addprops.a1o.isTAnimCam = GUILayout.Toggle(sc.cam_addprops.a1o.isTAnimCam, "Animate camera");
                    if (sc.cam_addprops.a1o.isTAnimCam)
                    {
                        GUILayout.Label(" with ");
                        if (GUILayout.Button(sc.cam_addprops.a1o.tacStyle))
                        {
                            var si = cam_style_list.IndexOf(sc.cam_addprops.a1o.tacStyle);
                            si += 1;
                            si %= cam_style_list.Count;
                            sc.cam_addprops.a1o.tacStyle = cam_style_list[si];
                        }
                        GUILayout.Label(" style.");
                    }
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();
                    if (sc.cam_addprops.a1o.isTAnimCam)
                    {
                        // GUILayout.BeginHorizontal()
                        // GUILayout.Space(tabWidth*2)
                        // GUILayout.Label("Move style:")
                        // aniStyleTexts = Array[String](("Linear", "S-F", "F-S", "S-F3", "F-S3", "S-F4", "F-S4"))
                        // self.cam_addprops["a1o"]["tacamStyle"] = GUILayout.SelectionGrid(self.cam_addprops["a1o"]["tacamStyle"], aniStyleTexts, 7)
                        // GUILayout.EndHorizontal()
                        GUILayout.BeginHorizontal();
                        GUILayout.Space(tabWidth * 2);
                        GUILayout.Label("Effects: zoom out (m): ");
                        //aniStyleTexts = Array[String](("Linear", "S-F", "F-S", "S-F3", "F-S3", "S-F4", "F-S4"))
                        sc.cam_addprops.a1o.tacZOut = GUILayout.TextField(sc.cam_addprops.a1o.tacZOut);
                        GUILayout.Label(" rotation x ");
                        sc.cam_addprops.a1o.tacRotX = GUILayout.TextField(sc.cam_addprops.a1o.tacRotX);
                        GUILayout.Label(" rot z (tilt)");
                        sc.cam_addprops.a1o.tacRotZ = GUILayout.TextField(sc.cam_addprops.a1o.tacRotZ);
                        GUILayout.EndHorizontal();
                    }
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(tabWidth);
                    sc.cam_addprops.a1o.isTHideUI = GUILayout.Toggle(sc.cam_addprops.a1o.isTHideUI, "Hide UI");
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(tabWidth);
                    sc.cam_addprops.a1o.isTTimerNext = GUILayout.Toggle(sc.cam_addprops.a1o.isTTimerNext, "Move to next state after that time");
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();
                }
                GUILayout.BeginHorizontal();
                //GUILayout.Label(" or just ")
                sc.cam_addprops.a1o.keepcam = GUILayout.Toggle(sc.cam_addprops.a1o.keepcam, " Skip camera update, just keep previous camera. (advanced)");
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                //self.cam_addprops["a1o"]["time"] = GUILayout.TextField(self.cam_addprops["a1o"]["time"], GUILayout.Width(100))
                // GUILayout.Label(" do ")
                // GUILayout.FlexibleSpace()
                // GUILayout.EndHorizontal()
                // --------- adv properties 2 --------
            }

            public static Dictionary<string, bool> adv2_def_values()
            {
                return new Dictionary<string, bool> {
            {
                "run-vnframe-ext",
                Utils.is_ini_value_true("RunVNFrameExtDefault")},
            {
                "run-vnanime-ext",
                Utils.is_ini_value_true("RunVNAnimeExtDefault")},
            {
                "run-objcam-ext",
                Utils.is_ini_value_true("RunObjCamExtDefault")}};
            }

            public static void render_wizard_adv2(SceneConsole sc)
            {
                // object camera support
                GUILayout.BeginHorizontal();
                GUILayout.Label("  On cam change:");
                sc.cam_addprops.a2o["run-vnframe-ext"] = GUILayout.Toggle(sc.cam_addprops.a2o["run-vnframe-ext"], "Run VNFrame ext");
                sc.cam_addprops.a2o["run-vnanime-ext"] = GUILayout.Toggle(sc.cam_addprops.a2o["run-vnanime-ext"], "Run VNAnime ext");
                sc.cam_addprops.a2o["run-objcam-ext"] = GUILayout.Toggle(sc.cam_addprops.a2o["run-objcam-ext"], "Run VNFrame ext");
                GUILayout.EndHorizontal();
            }

            // ---------- utils ------------------
            public static object adv_update(object o1, object o2)
            {
                foreach (var key in o2)
                {
                    if (o1.Contains(key))
                    {
                    }
                    else
                    {
                        o1[key] = o2[key];
                    }
                }
                return o1;
            }

            public static object render_vnss_wizard(SceneConsole sc)
            {
                List<string> ocams;
                int cInx;
                List<string> cnlist;
                object frame;
                object speed;
                object loop;
                bool synChk;
                Dictionary<string, object> ss;
                Dictionary<string, object> aniScript;
                string tmrTxt;
                int ci;
                string msname;
                List<object> mslist;
                string[] dtxts;
                float tmr;
                string cmd;
                int dno_scn;
                int dno_cam;
                int? dno;
                string dtxt;
                string btxt;
                // clear for initial
                if (!hasattr(sc, "wiz_step"))
                {
                    sc.wiz_step = 0;
                }
                // step 0
                if (sc.wiz_step == 0)
                {
                    // wait for start
                    if (GUILayout.Button("Wizard: Start adding a VNSS command.", GUILayout.Height(55)))
                    {
                        sc.wiz_step = 1;
                    }
                }
                else if (sc.wiz_step == 1)
                {
                    // select command menu
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Wizard: What do you want to do?");
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("<color=#ff0000>Cancel</color>", GUILayout.Width(60)))
                    {
                        sc.wiz_step = 0;
                    }
                    GUILayout.EndHorizontal();
                    if (GUILayout.Button("<color=#00ff00>addbtn</color>: Add a custom button to fork story line"))
                    {
                        sc.wiz_step = 10;
                        sc.wiz_data = new Dictionary<string, string> {
                    {
                        "text",
                        ""},
                    {
                        "jump",
                        ""}};
                        sc.wiz_error = null;
                    }
                    if (GUILayout.Button("<color=#00ff00>nextstate</color>: Make a jump when clicked 'Next' button"))
                    {
                        sc.wiz_step = 11;
                        sc.wiz_data = new Dictionary<string, string> {
                    {
                        "jump",
                        ""}};
                        sc.wiz_error = null;
                    }
                    if (GUILayout.Button("<color=#00ff00>timernext</color>: Goto next cam/scene automatically when time passed"))
                    {
                        sc.wiz_step = 12;
                        sc.wiz_data = new Dictionary<string, string> {
                    {
                        "time",
                        ""},
                    {
                        "jump",
                        ""}};
                        sc.wiz_error = null;
                    }
                    if (GUILayout.Button("<color=#00ff00>addbtnrnd</color>: Add a custom button jump to random scene/cam"))
                    {
                        sc.wiz_step = 13;
                        sc.wiz_data = new Dictionary<string, string> {
                    {
                        "text",
                        ""},
                    {
                        "jump",
                        ""}};
                        sc.wiz_error = null;
                    }
                    if (GUILayout.Button("<color=#00ff00>addbtnms</color>: Add a custom button to invoke ministate"))
                    {
                        sc.wiz_step = 14;
                        sc.wiz_data = new Dictionary<string, string> {
                    {
                        "text",
                        ""},
                    {
                        "ministate",
                        ""}};
                        sc.wiz_error = null;
                    }
                    if (GUILayout.Button("<color=#00ff00>showui/hideui</color>: Hide or show the text dialog (UI)"))
                    {
                        sc.wiz_step = 15;
                        sc.wiz_data = new Dictionary<string, string> {
                    {
                        "show",
                        "true"},
                    {
                        "time",
                        ""}};
                        sc.wiz_error = null;
                    }
                    if (GUILayout.Button("<color=#00ff00>lockui/unlockui</color>: Hide or show the buttons on dialog (UI)"))
                    {
                        sc.wiz_step = 16;
                        sc.wiz_data = new Dictionary<string, string> {
                    {
                        "lock",
                        "false"},
                    {
                        "time",
                        ""}};
                        sc.wiz_error = null;
                    }
                    if (GUILayout.Button("<color=#00ff00>runms</color>: Invoke a ministate on this cam"))
                    {
                        sc.wiz_step = 17;
                        sc.wiz_data = new Dictionary<string, string> {
                    {
                        "ministate",
                        ""}};
                        sc.wiz_error = null;
                    }
                    if (GUILayout.Button("<color=#00ff00>synch</color>: Sync H animation for charators"))
                    {
                        sc.wiz_step = 18;
                        sc.wiz_data = new Dictionary<string, string> {
                    {
                        "fid",
                        ""},
                    {
                        "mid",
                        ""},
                    {
                        "rs",
                        "false"}}; //TODO make a bool again
                        sc.wiz_error = null;
                    }
                    if (GUILayout.Button("<color=#00ff00>VNFrame ext</color>: Change the status of actor/props"))
                    {
                        sc.wiz_step = 20;
                        sc.wiz_data = new Dictionary<string, object> {
                    {
                        "script",
                        new Dictionary<string, object> {
                        }},
                    {
                        "anime",
                        false},
                    {
                        "time",
                        "1"},
                    {
                        "style",
                        "linear"}};
                        sc.wiz_error = null;
                        //if not hasattr(self.game.scenedata, "actors"):
                        //    print "Initialize VNFrame animation framework for VNSS ext"
                        //    from vnframe import init_scene_anime, register_actor_prop_by_tag
                        //    init_scene_anime(self.game)
                        //    register_actor_prop_by_tag(self.game)
                    }
                    if (GUILayout.Button("<color=#00ff00>VNAnime ext</color>: Control the playback of Keyframe clips"))
                    {
                        sc.wiz_step = 21;
                        sc.wiz_data = new Dictionary<string, string> {
                    {
                        "action",
                        "play"},
                    {
                        "clip",
                        ""},
                    {
                        "loop",
                        ""},
                    {
                        "speed",
                        ""},
                    {
                        "frame",
                        ""}};
                        sc.wiz_error = null;
                    }
                    if ((sc.game.isCharaStudio || sc.game.isNEOV2) && GUILayout.Button("<color=#00ff00>ObjCam ext</color>: switch to object camera"))
                    {
                        sc.wiz_step = 22;
                        sc.wiz_data = new Dictionary<string, string> {
                    {
                        "name",
                        ""}};
                        sc.wiz_error = null;
                    }
                }
                else if (sc.wiz_step == 10)
                {
                    // addbtn command
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Wizard: Setup a custom button");
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("<color=#00ff00>OK</color>", GUILayout.Width(60)))
                    {
                        sc.wiz_error = null;
                        btxt = sc.wiz_data["text"].Trim();
                        if (btxt.Length == 0)
                        {
                            sc.wiz_error = "Input a string for button text!";
                        }
                        dtxt = sc.wiz_data["jump"].Trim();
                        if (dtxt != "end" && dtxt != "next" && (!dtxt.StartsWith("{") || !dtxt.EndsWith("}")))
                        {
                            try
                            {
                                dno = Convert.ToInt32(dtxt);
                            }
                            catch
                            {
                                sc.wiz_error = "Input a valid number for jump destination! Or 'next' for next cam, 'end' for end of game, '{camname}' for named cam.";
                                dno = null;
                            }
                            if (dno is int n)
                            {
                                dno_cam = n % 100;
                                dno_scn = Convert.ToInt32(dno / 100);
                                if (dno_cam != 0 && dno_scn != sc.cur_index + 1)
                                {
                                    sc.wiz_error = String.Format("%d is not a valid jump destination! You can only jump to cam0 of other scene (n00), or any cam of the same scene (%dnn).", dno, sc.cur_index + 1);
                                }
                            }
                        }
                        if (sc.wiz_error == null)
                        {
                            cmd = String.Format("addbtn:%s:%s", btxt, dtxt);
                            append_vnss_cmd(sc, cmd);
                            sc.wiz_step = 0;
                        }
                    }
                    if (GUILayout.Button("<color=#ff0000>Cancel</color>", GUILayout.Width(60)))
                    {
                        sc.wiz_step = 0;
                        sc.wiz_error = null;
                    }
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Button text:", GUILayout.Width(80));
                    sc.wiz_data["text"] = GUILayout.TextField(sc.wiz_data["text"], GUILayout.Width(120));
                    GUILayout.Space(10);
                    GUILayout.Label("Click the button jump to:", GUILayout.Width(150));
                    sc.wiz_data["jump"] = GUILayout.TextField(sc.wiz_data["jump"], GUILayout.Width(100));
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                    if (sc.wiz_error)
                    {
                        GUILayout.Label("<color=#ff0000>" + sc.wiz_error + "</color>");
                    }
                    else
                    {
                        GUILayout.Label("* Jump destination is <color=#00ff00>(scene number) * 100 + (cam number)</color>. For example, set 200 for first cam of scene2, 202 for scene2 cam2. Or set <color=#00ff00>{camname}</color> to jump to a named cam. Or set <color=#00ff00>next</color> for next cam, <color=#00ff00>end</color> for end of game.");
                    }
                    GUILayout.EndHorizontal();
                }
                else if (sc.wiz_step == 11)
                {
                    // nextstate command
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Wizard: Setup a jump when click next");
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("<color=#00ff00>OK</color>", GUILayout.Width(60)))
                    {
                        sc.wiz_error = null;
                        dtxt = sc.wiz_data["jump"].Trim();
                        if (dtxt != "end" && (!dtxt.StartsWith("{") || !dtxt.EndsWith("}")))
                        {
                            try
                            {
                                dno = Convert.ToInt32(dtxt);
                            }
                            catch
                            {
                                sc.wiz_error = "Input a valid number for jump destination! Or 'end' for end of game, '{camname}' for named cam.";
                                dno = null;
                            }
                            if (dno is int n)
                            {
                                dno_cam = n % 100;
                                dno_scn = Convert.ToInt32(dno / 100);
                                if (dno_cam != 0 && dno_scn != sc.cur_index + 1)
                                {
                                    sc.wiz_error = String.Format("%d is not a valid jump destination! You can only jump to cam0 of other scene (n00), or any cam of the same scene (%dnn).", dno, sc.cur_index + 1);
                                }
                            }
                        }
                        if (sc.wiz_error == null)
                        {
                            cmd = String.Format("nextstate:%s", dtxt);
                            append_vnss_cmd(sc, cmd);
                            sc.wiz_step = 0;
                        }
                    }
                    if (GUILayout.Button("<color=#ff0000>Cancel</color>", GUILayout.Width(60)))
                    {
                        sc.wiz_step = 0;
                        sc.wiz_error = null;
                    }
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Click the next then jump to:", GUILayout.Width(170));
                    sc.wiz_data["jump"] = GUILayout.TextField(sc.wiz_data["jump"], GUILayout.Width(50));
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                    if (sc.wiz_error)
                    {
                        GUILayout.Label("<color=#ff0000>" + sc.wiz_error + "</color>");
                    }
                    else
                    {
                        GUILayout.Label("* Jump destination is <color=#00ff00>(scene number) * 100 + (cam number)</color>. For example, set 200 for first cam of scene2, 202 for scene2 cam2. Or set <color=#00ff00>{camname}</color> to jump to a named cam. Or set <color=#00ff00>end</color> for end of game.");
                    }
                    GUILayout.EndHorizontal();
                }
                else if (sc.wiz_step == 12)
                {
                    // timernext command
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Wizard: Setup a auto next timer");
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("<color=#00ff00>OK</color>", GUILayout.Width(60)))
                    {
                        sc.wiz_error = null;
                        try
                        {
                            tmr = float.Parse(sc.wiz_data["time"]);
                        }
                        catch
                        {
                            sc.wiz_error = "Input a valid float number for timer in second!";
                            tmr = null;
                        }
                        dtxt = sc.wiz_data["jump"].Trim();
                        if (dtxt.Length > 0 && dtxt != "end" && (!dtxt.StartsWith("{") || !dtxt.EndsWith("}")))
                        {
                            try
                            {
                                dno = Convert.ToInt32(dtxt);
                            }
                            catch
                            {
                                sc.wiz_error = "Input a valid number for jump destination! Or 'end' for end of game, '{camname}' for named cam.";
                                dno = null;
                            }
                            if (dno is int no)
                            {
                                dno_cam = no % 100;
                                dno_scn = Convert.ToInt32(dno / 100);
                                if (dno_cam != 0 && dno_scn != sc.cur_index + 1)
                                {
                                    sc.wiz_error = String.Format("%d is not a valid jump destination! You can only jump to cam0 of other scene (n00), or any cam of the same scene (%dnn).", dno, sc.cur_index + 1);
                                }
                            }
                        }
                        if (sc.wiz_error == null)
                        {
                            if (dtxt.Length > 0)
                            {
                                cmd = String.Format("timernext:%.1f:%s", tmr, dtxt);
                            }
                            else
                            {
                                cmd = String.Format("timernext:%.1f", tmr);
                            }
                            append_vnss_cmd(sc, cmd);
                            sc.wiz_step = 0;
                        }
                    }
                    if (GUILayout.Button("<color=#ff0000>Cancel</color>", GUILayout.Width(60)))
                    {
                        sc.wiz_step = 0;
                        sc.wiz_error = null;
                    }
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Goto next scene after:", GUILayout.Width(150));
                    sc.wiz_data["time"] = GUILayout.TextField(sc.wiz_data["time"], GUILayout.Width(50));
                    GUILayout.Label("seconds", GUILayout.Width(60));
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Set jump destination:", GUILayout.Width(150));
                    sc.wiz_data["jump"] = GUILayout.TextField(sc.wiz_data["jump"], GUILayout.Width(100));
                    GUILayout.Label(", leave it blank to just go next.");
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                    if (sc.wiz_error)
                    {
                        GUILayout.Label("<color=#ff0000>" + sc.wiz_error + "</color>");
                    }
                    else
                    {
                        GUILayout.Label("* Jump destination is <color=#00ff00>(scene number) * 100 + (cam number)</color>. For example, set 200 for first cam of scene2, 202 for scene2 cam2. Or set <color=#00ff00>{camname}</color> to jump to a named cam. Or set <color=#00ff00>end</color> for end of game.");
                    }
                    GUILayout.EndHorizontal();
                }
                else if (sc.wiz_step == 13)
                {
                    // addbtnrnd command
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Wizard: Setup a random custom button");
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("<color=#00ff00>OK</color>", GUILayout.Width(60)))
                    {
                        sc.wiz_error = null;
                        var txt = sc.wiz_data["text"].Trim();
                        if (txt.Count == 0)
                        {
                            sc.wiz_error = "Input a string for button text!";
                        }
                        try
                        {
                            dtxts = sc.wiz_data["jump"].Trim().Split(',');
                            foreach (var dtxt in dtxts)
                            {
                                dtxt = dtxt.Trim();
                                if (dtxt != "end" && dtxt != "next" && (!dtxt.StartsWith("{") || !dtxt.EndsWith("}")))
                                {
                                    dno = Convert.ToInt32(dtxt);
                                    dno_cam = dno % 100;
                                    dno_scn = Convert.ToInt32(dno / 100);
                                    if (dno_cam != 0 && dno_scn != sc.cur_index + 1)
                                    {
                                        throw new Exception(String.Format("%d is not a valid jump destination! You can only jump to cam0 of other scene (n00), or any cam of the same scene (%dnn).", dno, sc.cur_index + 1));
                                    }
                                }
                            }
                            dtxts = string.Join(",", dtxts);
                        }
                        catch (Exception e)
                        {
                            sc.wiz_error = "Input a valid jump destination! " + e.ToString();
                            dtxts = null;
                        }
                        if (sc.wiz_error == null)
                        {
                            cmd = String.Format("addbtnrnd:%s:%s", txt, dtxts);
                            append_vnss_cmd(sc, cmd);
                            sc.wiz_step = 0;
                        }
                    }
                    if (GUILayout.Button("<color=#ff0000>Cancel</color>", GUILayout.Width(60)))
                    {
                        sc.wiz_step = 0;
                        sc.wiz_error = null;
                    }
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Button text:", GUILayout.Width(80));
                    sc.wiz_data["text"] = GUILayout.TextField(sc.wiz_data["text"], GUILayout.Width(120));
                    GUILayout.Space(10);
                    GUILayout.Label("Click the button jump to:", GUILayout.Width(150));
                    sc.wiz_data["jump"] = GUILayout.TextField(sc.wiz_data["jump"], GUILayout.Width(100));
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                    if (sc.wiz_error)
                    {
                        GUILayout.Label("<color=#ff0000>" + sc.wiz_error + "</color>");
                    }
                    else
                    {
                        GUILayout.Label("* Jump destination is <color=#00ff00>(scene number) * 100 + (cam number)</color>. For example, set 200 for first cam of scene2, 202 for scene2 cam2. Or set <color=#00ff00>{camname}</color> to jump to a named cam. Or set <color=#00ff00>next</color> for next cam, <color=#00ff00>end</color> for end of game. Set multiple jump destination separated by comma (ex: 201,202,203). When user click this button story will jump to one of them randomly.");
                    }
                    GUILayout.EndHorizontal();
                }
                else if (sc.wiz_step == 14)
                {
                    // addbtnms command
                    mslist = (from i in ministates_get_list(sc.game)
                              select i[0]).ToList();
                    if (mslist.Length > 0)
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Wizard: Setup a ministate button");
                        GUILayout.FlexibleSpace();
                        if (GUILayout.Button("<color=#00ff00>OK</color>", GUILayout.Width(60)))
                        {
                            sc.wiz_error = null;
                            btxt = sc.wiz_data["text"].Trim();
                            if (btxt.Length == 0)
                            {
                                sc.wiz_error = "Input a string for button text!";
                            }
                            var sts = sc.wiz_data["ministate"].Trim();
                            if (sc.wiz_error == null && (sts.Length == 0 || mslist.count(sts) == 0))
                            {
                                sc.wiz_error = String.Format("[%s] is not a valid ministate name!", sts);
                            }
                            if (sc.wiz_error == null)
                            {
                                cmd = String.Format("addbtnms:%s:%s", btxt, sts);
                                append_vnss_cmd(sc, cmd);
                                sc.wiz_step = 0;
                            }
                        }
                        if (GUILayout.Button("<color=#ff0000>Cancel</color>", GUILayout.Width(60)))
                        {
                            sc.wiz_step = 0;
                            sc.wiz_error = null;
                        }
                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Button text:", GUILayout.Width(80));
                        sc.wiz_data["text"] = GUILayout.TextField(sc.wiz_data["text"], GUILayout.Width(120));
                        GUILayout.Space(10);
                        GUILayout.Label("Click invokes:", GUILayout.Width(90));
                        sc.wiz_data["ministate"] = GUILayout.TextField(sc.wiz_data["ministate"], GUILayout.Width(120));
                        if (GUILayout.Button("<", GUILayout.Width(20)))
                        {
                            msname = sc.wiz_data["ministate"].Trim();
                            if (msname.Count == 0 || mslist.count(msname) == 0)
                            {
                                sc.wiz_data["ministate"] = mslist[0];
                            }
                            else
                            {
                                ci = mslist.IndexOf(msname);
                                sc.wiz_data["ministate"] = mslist[ci - 1];
                            }
                        }
                        if (GUILayout.Button(">", GUILayout.Width(20)))
                        {
                            msname = sc.wiz_data["ministate"].Trim();
                            if (msname.Count == 0 || mslist.count(msname) == 0)
                            {
                                sc.wiz_data["ministate"] = mslist[0];
                            }
                            else
                            {
                                ci = mslist.IndexOf(msname);
                                sc.wiz_data["ministate"] = mslist[(ci + 1) % mslist.Count];
                            }
                        }
                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal();
                        if (sc.wiz_error)
                        {
                            GUILayout.Label("<color=#ff0000>" + sc.wiz_error + "</color>");
                        }
                        else
                        {
                            GUILayout.Label("* Create a button to invoke a ministate by its name.");
                        }
                        GUILayout.EndHorizontal();
                    }
                    else
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Wizard: Invoke a ministate");
                        GUILayout.FlexibleSpace();
                        if (GUILayout.Button("<color=#ff0000>Cancel</color>", GUILayout.Width(60)))
                        {
                            sc.wiz_step = 0;
                            sc.wiz_error = null;
                        }
                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("No ministate found in this scene. Create one in Ministates panel first.");
                        GUILayout.EndHorizontal();
                    }
                }
                else if (sc.wiz_step == 15)
                {
                    // showui/hideui command
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Wizard: Show/Hide the dialog box (UI)");
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("<color=#00ff00>OK</color>", GUILayout.Width(60)))
                    {
                        sc.wiz_error = null;
                        cmd = sc.wiz_data["show"] ? "showui" : "hideui";
                        try
                        {
                            tmrTxt = sc.wiz_data["time"].Trim();
                            if (cmd == "hideui" && tmrTxt.Length > 0)
                            {
                                tmr = float.Parse(tmrTxt);
                                if (tmr <= 0)
                                {
                                    throw new Exception();
                                }
                                cmd += String.Format(":%.1f", tmr);
                            }
                        }
                        catch
                        {
                            sc.wiz_error = "Input a valid float number for timer in second!";
                            cmd = null;
                        }
                        if (sc.wiz_error == null)
                        {
                            append_vnss_cmd(sc, cmd);
                            sc.wiz_step = 0;
                        }
                    }
                    if (GUILayout.Button("<color=#ff0000>Cancel</color>", GUILayout.Width(60)))
                    {
                        sc.wiz_step = 0;
                        sc.wiz_error = null;
                    }
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                    sc.wiz_data["show"] = GUILayout.Toggle(sc.wiz_data["show"], "Show UI", GUILayout.Width(100));
                    sc.wiz_data["show"] = !GUILayout.Toggle(!sc.wiz_data["show"], "Hide UI", GUILayout.Width(60));
                    if (!sc.wiz_data["show"])
                    {
                        GUILayout.Label("for ", GUILayout.Width(25));
                        sc.wiz_data["time"] = GUILayout.TextField(sc.wiz_data["time"], GUILayout.Width(30));
                        GUILayout.Label("seconds, leave it blank for manual re-show");
                    }
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                    if (sc.wiz_error)
                    {
                        GUILayout.Label("<color=#ff0000>" + sc.wiz_error + "</color>");
                    }
                    else
                    {
                        GUILayout.Label("* You can't hit any button when UI was hidden. Set a timeout for hide UI command, or you must call show UI by yourself! In this case use it with <color=#00ff00>timernext</color>, and be sure to re-show it on the next scene/cam.");
                    }
                    GUILayout.EndHorizontal();
                }
                else if (sc.wiz_step == 16)
                {
                    // unlockui/lockui command
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Wizard: Show/Hide the dialog box (UI)");
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("<color=#00ff00>OK</color>", GUILayout.Width(60)))
                    {
                        sc.wiz_error = null;
                        cmd = sc.wiz_data["lock"] ? "lockui" : "unlockui";
                        try
                        {
                            tmrTxt = sc.wiz_data["time"].Trim();
                            if (cmd == "lockui" && tmrTxt.Count > 0)
                            {
                                tmr = (float)(tmrTxt);
                                if (tmr <= 0)
                                {
                                    throw new Exception();
                                }
                                cmd += String.Format(":%.1f", tmr);
                            }
                        }
                        catch
                        {
                            sc.wiz_error = "Input a valid float number for timer in second!";
                            cmd = null;
                        }
                        if (sc.wiz_error == null)
                        {
                            append_vnss_cmd(sc, cmd);
                            sc.wiz_step = 0;
                        }
                    }
                    if (GUILayout.Button("<color=#ff0000>Cancel</color>", GUILayout.Width(60)))
                    {
                        sc.wiz_step = 0;
                        sc.wiz_error = null;
                    }
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                    sc.wiz_data["lock"] = !GUILayout.Toggle(!sc.wiz_data["lock"], "Unlock UI", GUILayout.Width(100));
                    sc.wiz_data["lock"] = GUILayout.Toggle(sc.wiz_data["lock"], "Lock UI", GUILayout.Width(60));
                    if (sc.wiz_data["lock"])
                    {
                        GUILayout.Label("for ", GUILayout.Width(25));
                        sc.wiz_data["time"] = GUILayout.TextField(sc.wiz_data["time"], GUILayout.Width(30));
                        GUILayout.Label("seconds, leave it blank for manual unlock");
                    }
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                    if (!sc.wiz_error.IsNullOrEmpty())
                    {
                        GUILayout.Label("<color=#ff0000>" + sc.wiz_error + "</color>");
                    }
                    else
                    {
                        GUILayout.Label("* You can't hit any button when UI was locked! Set a timeout for lock UI command, or you must call unlock UI by yourself! In this case use it with <color=#00ff00>timernext</color>, and be sure to unlock it on the next scene/cam.");
                    }
                    GUILayout.EndHorizontal();
                }
                else if (sc.wiz_step == 17)
                {
                    // runms command
                    mslist = (from i in ministates_get_list(sc.game)
                              select i[0]).ToList();
                    if (mslist.Count > 0)
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Wizard: Invoke a ministate");
                        GUILayout.FlexibleSpace();
                        if (GUILayout.Button("<color=#00ff00>OK</color>", GUILayout.Width(60)))
                        {
                            sc.wiz_error = null;
                            msname = sc.wiz_data["ministate"].Trim();
                            if (msname.Length == 0 || mslist.count(msname) == 0)
                            {
                                sc.wiz_error = String.Format("[%s] is not a valid ministate name!", msname);
                            }
                            if (sc.wiz_error == null)
                            {
                                cmd = String.Format("runms:%s", msname);
                                append_vnss_cmd(sc, cmd);
                                sc.wiz_step = 0;
                            }
                        }
                        if (GUILayout.Button("<color=#ff0000>Cancel</color>", GUILayout.Width(60)))
                        {
                            sc.wiz_step = 0;
                            sc.wiz_error = null;
                        }
                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Ministate to run:", GUILayout.Width(150));
                        sc.wiz_data["ministate"] = GUILayout.TextField(sc.wiz_data["ministate"], GUILayout.Width(150));
                        if (GUILayout.Button("<", GUILayout.Width(20)))
                        {
                            msname = sc.wiz_data["ministate"].Trim();
                            if (msname.Count == 0 || mslist.count(msname) == 0)
                            {
                                sc.wiz_data["ministate"] = mslist[0];
                            }
                            else
                            {
                                ci = mslist.IndexOf(msname);
                                sc.wiz_data["ministate"] = mslist[ci - 1];
                            }
                        }
                        if (GUILayout.Button(">", GUILayout.Width(20)))
                        {
                            msname = sc.wiz_data["ministate"].Trim();
                            if (msname.Count == 0 || mslist.count(msname) == 0)
                            {
                                sc.wiz_data["ministate"] = mslist[0];
                            }
                            else
                            {
                                ci = mslist.IndexOf(msname);
                                sc.wiz_data["ministate"] = mslist[(ci + 1) % mslist.Count];
                            }
                        }
                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal();
                        if (sc.wiz_error)
                        {
                            GUILayout.Label("<color=#ff0000>" + sc.wiz_error + "</color>");
                        }
                        else
                        {
                            GUILayout.Label("* Set a ministate to run when this cam starts.");
                        }
                        GUILayout.EndHorizontal();
                    }
                    else
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Wizard: Invoke a ministate");
                        GUILayout.FlexibleSpace();
                        if (GUILayout.Button("<color=#ff0000>Cancel</color>", GUILayout.Width(60)))
                        {
                            sc.wiz_step = 0;
                            sc.wiz_error = null;
                        }
                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("No ministate found in this scene. Create one in Ministates panel first.");
                        GUILayout.EndHorizontal();
                    }
                }
                else if (sc.wiz_step == 18)
                {
                    // synch command
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Wizard: Sync H animation for charators");
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("<color=#00ff00>OK</color>", GUILayout.Width(60)))
                    {
                        sc.wiz_error = null;
                        cmd = sc.wiz_data["rs"] ? "synchr" : "synch";
                        var fid = sc.wiz_data["fid"];
                        var mid = sc.wiz_data["mid"];
                        try
                        {
                            if (!sc.game.scenef_get_actor(fid))
                            {
                                throw new Exception("<" + fid + "> is not a valid charactor id!");
                            }
                            if (mid.Length > 0 && !sc.game.scenef_get_actor(mid))
                            {
                                throw new Exception("<" + mid + "> is not a valid charactor id!");
                            }
                            if (fid == mid)
                            {
                                throw new Exception("Choice a different actor to sync with base actor!");
                            }
                            if (mid.Length > 0)
                            {
                                cmd += ":" + fid + ":" + mid;
                            }
                            else
                            {
                                cmd += ":" + fid;
                            }
                        }
                        catch (Exception e)
                        {
                            sc.wiz_error = "Wrong input: " + e.ToString();
                            cmd = null;
                        }
                        if (sc.wiz_error == null)
                        {
                            append_vnss_cmd(sc, cmd);
                            sc.wiz_step = 0;
                        }
                    }
                    if (GUILayout.Button("<color=#ff0000>Cancel</color>", GUILayout.Width(60)))
                    {
                        sc.wiz_step = 0;
                        sc.wiz_error = null;
                    }
                    GUILayout.EndHorizontal();
                    var curActors = sc.game.scenef_get_all_actors();
                    if (curActors.Count == 0)
                    {
                        sc.wiz_error = "No charactor tracked! Please track charactor first.";
                    }
                    else
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Base actor:", GUILayout.Width(70));
                        if (GUILayout.Button("<", GUILayout.Width(20)))
                        {
                            sc.wiz_data["fid"] = selectPrevNext(sc.wiz_data["fid"], -1);
                        }
                        if (GUILayout.Button(">", GUILayout.Width(20)))
                        {
                            sc.wiz_data["fid"] = selectPrevNext(sc.wiz_data["fid"], +1);
                        }
                        GUILayout.Space(24);
                        GUILayout.Label(dispNameOfId(sc.wiz_data["fid"]));
                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Partner:", GUILayout.Width(70));
                        if (GUILayout.Button("<", GUILayout.Width(20)))
                        {
                            sc.wiz_data["mid"] = selectPrevNext(sc.wiz_data["mid"], -1);
                        }
                        if (GUILayout.Button(">", GUILayout.Width(20)))
                        {
                            sc.wiz_data["mid"] = selectPrevNext(sc.wiz_data["mid"], +1);
                        }
                        if (GUILayout.Button("x", GUILayout.Width(20)))
                        {
                            sc.wiz_data["mid"] = "";
                        }
                        GUILayout.Label(dispNameOfId(sc.wiz_data["mid"]));
                        GUILayout.EndHorizontal();
                        sc.wiz_data["rs"] = GUILayout.Toggle(sc.wiz_data["rs"], "Restart anime *MAY CAUSE BUG IF RESTART JUST AFTER SET ANIME?");
                    }
                    GUILayout.BeginHorizontal();
                    if (sc.wiz_error)
                    {
                        GUILayout.Label("<color=#ff0000>" + sc.wiz_error + "</color>");
                    }
                    else
                    {
                        GUILayout.Label("* Sync H animation can automatically adjust actor's anime aux param according to base actor's height and breast. The base actor usually is the female actor.");
                    }
                    GUILayout.EndHorizontal();
                }
                else if (sc.wiz_step == 20)
                {
                    // VNFrame command
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Wizard: Addition VNFrame command to make scene variant");
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("<color=#00ff00>OK</color>", GUILayout.Width(60)))
                    {
                        if (sc.wiz_data["script"].Count > 0)
                        {
                            if (sc.wiz_data["anime"])
                            {
                                try
                                {
                                    tmr = (float)(sc.wiz_data["time"]);
                                    if (tmr <= 0)
                                    {
                                        throw new Exception();
                                    }
                                    sc.wiz_error = null;
                                    aniScript = ValueTuple.Create((sc.wiz_data["script"], tmr, sc.wiz_data["style"]));
                                    cmd = String.Format("f_anime:::%s", script2string(aniScript).replace(" ", "").Trim());
                                }
                                catch
                                {
                                    sc.wiz_error = "Input a valid time value in second for animation!";
                                }
                            }
                            else
                            {
                                cmd = String.Format("f_act:::%s", script2string(sc.wiz_data["script"]).replace(" ", "").Trim());
                            }
                            if (sc.wiz_error == null)
                            {
                                append_vnss_cmd(sc, cmd);
                                sc.wiz_step = 0;
                            }
                        }
                        else
                        {
                            sc.wiz_error = "No script found, make some change and take a snapshot first!";
                        }
                    }
                    if (GUILayout.Button("<color=#ff0000>Cancel</color>", GUILayout.Width(60)))
                    {
                        sc.wiz_step = 0;
                        sc.wiz_error = null;
                    }
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button("Snapshot", GUILayout.Width(100)))
                    {
                        sc.wiz_data["script"] = vnframe_take_snapshot(sc);
                        if (sc.wiz_data["script"].Count == 0)
                        {
                            sc.wiz_error = "No change on tracked actor/prop found, please make some change against base scene.";
                        }
                        else
                        {
                            sc.wiz_error = null;
                        }
                    }
                    if (sc.wiz_data["script"].Count > 0 && GUILayout.Button("Reset", GUILayout.Width(100)))
                    {
                        sc.block[sc.cur_index].setSceneState(sc.game);
                    }
                    if (sc.wiz_data["script"].Count > 0 && GUILayout.Button("Preview", GUILayout.Width(100)))
                    {
                        if (sc.wiz_data["anime"])
                        {
                            try
                            {
                                tmr = float.Parse(sc.wiz_data["time"]);
                                if (tmr <= 0)
                                {
                                    throw new Exception();
                                }
                                sc.wiz_error = null;
                            }
                            catch
                            {
                                sc.wiz_error = "Input a valid time value in second for animation!";
                            }
                            if (sc.wiz_error == null)
                            {
                                ss = scriptCopy(sc.wiz_data["script"]);
                                aniScript = ValueTuple.Create((ss, tmr, sc.wiz_data["style"]));
                            //print "anime script:", aniScript
                            VNFrame.anime(sc.game, aniScript);
                            }
                        }
                        else
                        {
                            ss = scriptCopy(sc.wiz_data["script"]);
                            //print "act script:", self.wiz_data["script"]
                            VNFrame.act(sc.game, ss);
                        }
                    }
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                    sc.wiz_data["anime"] = GUILayout.Toggle(sc.wiz_data["anime"], "Render in animation", GUILayout.Width(140));
                    if (sc.wiz_data["anime"])
                    {
                        GUILayout.Label("length:", GUILayout.Width(50));
                        sc.wiz_data["time"] = GUILayout.TextField(sc.wiz_data["time"], GUILayout.Width(40));
                        GUILayout.Label("sec, style:", GUILayout.Width(80));
                        if (GUILayout.Button(sc.wiz_data["style"], GUILayout.Width(100)))
                        {
                            var vnfa_style_list = ("linear", "slow-fast", "fast-slow", "slow-fast3", "fast-slow3", "slow-fast4", "fast-slow4");
                            var si = vnfa_style_list.IndexOf(sc.wiz_data["style"]);
                            si += 1;
                            si %= vnfa_style_list.Count;
                            sc.wiz_data["style"] = vnfa_style_list[si];
                        }
                    }
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                    if (sc.wiz_error)
                    {
                        GUILayout.Label("<color=#ff0000>" + sc.wiz_error + "</color>");
                    }
                    else
                    {
                        GUILayout.Label("* Setup you scene and click the <color=#00ff00>[Snapshot]</color> button to get a diff snapshot with the <color=#ff0000>base scene (not the last cam!)</color>. Then use <color=#00ff00>[Reset]</color> and <color=#00ff00>[Preview]</color> button to preview your vnframe act/anime. Hit <color=#00ff00>[OK]</color> if you are done. VNFrame anime is NOT RECOMMENDED for complicated animation, use VNAnime Keyframe Clip instead.");
                    }
                    GUILayout.EndHorizontal();
                }
                else if (sc.wiz_step == 21)
                {
                    // VNAnime command
                    // syntax check enable
                    if (!hasattr(sc.game, "gdata") || !hasattr(sc.game.gdata, "kfaManagedClips"))
                    {
                        sc.wiz_error = "WARNING: Keyframe function not initialized! Syntax check DISABLED! Set all parameters on your own risk!";
                        synChk = false;
                    }
                    else if (sc.game.gdata.kfaManagedClips.Count == 0)
                    {
                        sc.wiz_error = "WARNING: No keyframe clips found! Syntax check DISABLED! Set all parameters on your own risk!";
                        synChk = false;
                    }
                    else
                    {
                        synChk = true;
                    }
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Wizard: VNAnime command to control keyframe clip playback");
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("<color=#00ff00>OK</color>", GUILayout.Width(60)))
                    {
                        var clpname = sc.wiz_data["clip"].Trim();
                        cmd = null;
                        if (synChk)
                        {
                            if (clpname.Length > 0 && !sc.game.gdata.kfaManagedClips.has_key(clpname))
                            {
                                sc.wiz_error = String.Format("Keyframe clip [%s] not found! Input a valid clip name.", clpname);
                            }
                            else
                            {
                                sc.wiz_error = null;
                            }
                            if (sc.wiz_error == null && sc.wiz_data["action"] == "play")
                            {
                                if (clpname.Length > 0)
                                {
                                    try
                                    {
                                        if (sc.wiz_data["loop"].Trim().Count > 0)
                                        {
                                            loop = Convert.ToInt32(sc.wiz_data["loop"]);
                                            if (loop < -1)
                                            {
                                                throw new Exception();
                                            }
                                        }
                                        else
                                        {
                                            loop = null;
                                        }
                                    }
                                    catch
                                    {
                                        sc.wiz_error = "Input a valid number for loop count. -1 for infinite loop, 0 for non-loop, 1 for loop once and so on.";
                                    }
                                    try
                                    {
                                        if (sc.wiz_data["speed"].Trim().Count > 0)
                                        {
                                            speed = (float)(sc.wiz_data["speed"]);
                                            if (speed <= 0)
                                            {
                                                throw new Exception();
                                            }
                                        }
                                        else
                                        {
                                            speed = null;
                                        }
                                    }
                                    catch
                                    {
                                        sc.wiz_error = "Input a valid float number for speed rate. 1 for normal speed, 2 for 2x speed and so on.";
                                    }
                                    if (sc.wiz_error == null)
                                    {
                                        if (speed != null)
                                        {
                                            cmd = String.Format("f_clipplay:%s:%s:%.1f", clpname, loop == null ? "" : loop.ToString(), speed);
                                        }
                                        else if (loop != null)
                                        {
                                            cmd = String.Format("f_clipplay:%s:%d", clpname, loop);
                                        }
                                        else
                                        {
                                            cmd = String.Format("f_clipplay:%s", clpname);
                                        }
                                    }
                                }
                                else
                                {
                                    cmd = "f_clipplay";
                                }
                            }
                            if (sc.wiz_error == null && sc.wiz_data["action"] == "pause")
                            {
                                if (clpname.Length > 0)
                                {
                                    cmd = String.Format("f_clippause:%s", clpname);
                                }
                                else
                                {
                                    cmd = "f_clippause";
                                }
                            }
                            if (sc.wiz_error == null && sc.wiz_data["action"] == "stop")
                            {
                                if (clpname.Length > 0)
                                {
                                    cmd = String.Format("f_clipstop:%s", clpname);
                                }
                                else
                                {
                                    cmd = "f_clipstop";
                                }
                            }
                            if (sc.wiz_error == null && sc.wiz_data["action"] == "seek")
                            {
                                if (clpname.Length > 0)
                                {
                                    try
                                    {
                                        frame = Convert.ToInt32(sc.wiz_data["frame"]);
                                        if (frame < 0 || frame > sc.game.gdata.kfaManagedClips[clpname].frameLength)
                                        {
                                            throw new Exception();
                                        }
                                    }
                                    catch
                                    {
                                        sc.wiz_error = String.Format("Input a valid number for frame. Valid frame range of clip [%s] is 0-%d.", clpname, sc.game.gdata.kfaManagedClips[clpname].frameLength);
                                    }
                                    if (sc.wiz_error == null)
                                    {
                                        cmd = String.Format("f_clipseek:%s:%d", clpname, frame);
                                    }
                                }
                                else
                                {
                                    sc.wiz_error = "Clip name not setted!";
                                }
                            }
                        }
                        else
                        {
                            if (sc.wiz_data["action"] == "play")
                            {
                                if (clpname.Length > 0)
                                {
                                    if (sc.wiz_data["loop"].Trim().Count > 0)
                                    {
                                        loop = sc.wiz_data["loop"].Trim();
                                    }
                                    else
                                    {
                                        loop = null;
                                    }
                                    if (sc.wiz_data["speed"].Trim().Count > 0)
                                    {
                                        speed = sc.wiz_data["speed"].Trim();
                                    }
                                    else
                                    {
                                        speed = null;
                                    }
                                    if (speed != null)
                                    {
                                        cmd = String.Format("f_clipplay:%s:%s:%s", clpname, loop == null ? "" : loop, speed);
                                    }
                                    else if (loop != null)
                                    {
                                        cmd = String.Format("f_clipplay:%s:%s", clpname, loop);
                                    }
                                    else
                                    {
                                        cmd = String.Format("f_clipplay:%s", clpname);
                                    }
                                }
                                else
                                {
                                    cmd = "f_clipplay";
                                }
                            }
                            if (sc.wiz_data["action"] == "pause")
                            {
                                if (clpname.Length > 0)
                                {
                                    cmd = String.Format("f_clippause:%s", clpname);
                                }
                                else
                                {
                                    cmd = "f_clippause";
                                }
                            }
                            if (sc.wiz_data["action"] == "stop")
                            {
                                if (clpname.Length > 0)
                                {
                                    cmd = String.Format("f_clipstop:%s", clpname);
                                }
                                else
                                {
                                    cmd = "f_clipstop";
                                }
                            }
                            if (sc.wiz_data["action"] == "seek")
                            {
                                if (clpname.Length > 0)
                                {
                                    frame = sc.wiz_data["frame"];
                                    cmd = String.Format("f_clipseek:%s:%s", clpname, frame);
                                }
                                else
                                {
                                    sc.wiz_error = "Clip name not setted!";
                                }
                            }
                        }
                        if (cmd != null)
                        {
                            append_vnss_cmd(sc, cmd);
                            sc.wiz_step = 0;
                        }
                    }
                    if (GUILayout.Button("<color=#ff0000>Cancel</color>", GUILayout.Width(60)))
                    {
                        sc.wiz_step = 0;
                        sc.wiz_error = null;
                    }
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Select an action:", GUILayout.Width(100));
                    if (GUILayout.Toggle(sc.wiz_data["action"] == "play", "Play", GUILayout.Width(80)) && sc.wiz_data["action"] != "play")
                    {
                        sc.wiz_data["action"] = "play";
                        if (synChk)
                        {
                            sc.wiz_error = null;
                        }
                    }
                    if (GUILayout.Toggle(sc.wiz_data["action"] == "pause", "Pause", GUILayout.Width(80)) && sc.wiz_data["action"] != "pause")
                    {
                        sc.wiz_data["action"] = "pause";
                        if (synChk)
                        {
                            sc.wiz_error = null;
                        }
                    }
                    if (GUILayout.Toggle(sc.wiz_data["action"] == "stop", "Stop", GUILayout.Width(80)) && sc.wiz_data["action"] != "stop")
                    {
                        sc.wiz_data["action"] = "stop";
                        if (synChk)
                        {
                            sc.wiz_error = null;
                        }
                    }
                    if (GUILayout.Toggle(sc.wiz_data["action"] == "seek", "Seek", GUILayout.Width(80)) && sc.wiz_data["action"] != "seek")
                    {
                        sc.wiz_data["action"] = "seek";
                        if (synChk)
                        {
                            sc.wiz_error = null;
                        }
                    }
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Target clip name:", GUILayout.Width(100));
                    sc.wiz_data["clip"] = GUILayout.TextField(sc.wiz_data["clip"], GUILayout.Width(100));
                    if (synChk)
                    {
                        if (GUILayout.Button("<", GUILayout.Width(20)))
                        {
                            cnlist = sc.game.gdata.kfaManagedClips.keys().OrderBy(_p_1 => _p_1).ToList();
                            if (cnlist.count(sc.wiz_data["clip"].Trim()))
                            {
                                cInx = cnlist.IndexOf(sc.wiz_data["clip"].Trim());
                                cInx -= 1;
                                cInx %= cnlist.Count;
                                sc.wiz_data["clip"] = cnlist[cInx];
                            }
                            else
                            {
                                sc.wiz_data["clip"] = cnlist[0];
                            }
                        }
                        if (GUILayout.Button(">", GUILayout.Width(20)))
                        {
                            cnlist = sc.game.gdata.kfaManagedClips.keys().OrderBy(_p_2 => _p_2).ToList();
                            if (cnlist.count(sc.wiz_data["clip"].Trim()))
                            {
                                cInx = cnlist.IndexOf(sc.wiz_data["clip"].Trim());
                                cInx += 1;
                                cInx %= cnlist.Count;
                                sc.wiz_data["clip"] = cnlist[cInx];
                            }
                            else
                            {
                                sc.wiz_data["clip"] = cnlist[0];
                            }
                        }
                    }
                    if (sc.wiz_data["action"] != "seek")
                    {
                        if (GUILayout.Button("X", GUILayout.Width(20)))
                        {
                            sc.wiz_data["clip"] = "";
                        }
                        GUILayout.Label("* Leave blank to operate all clips");
                    }
                    else
                    {
                        GUILayout.Label(", seek to frame No:", GUILayout.Width(120));
                        sc.wiz_data["frame"] = GUILayout.TextField(sc.wiz_data["frame"], GUILayout.Width(50));
                    }
                    GUILayout.EndHorizontal();
                    if (sc.wiz_data["action"] == "play" && sc.wiz_data["clip"].Trim().Length > 0)
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Play loop:", GUILayout.Width(80));
                        sc.wiz_data["loop"] = GUILayout.TextField(sc.wiz_data["loop"], GUILayout.Width(40));
                        GUILayout.Label(", speed:", GUILayout.Width(60));
                        sc.wiz_data["speed"] = GUILayout.TextField(sc.wiz_data["speed"], GUILayout.Width(40));
                        GUILayout.Label("* Leave blank for clip's default");
                        GUILayout.EndHorizontal();
                    }
                    GUILayout.BeginHorizontal();
                    // Notification
                    if (sc.wiz_error)
                    {
                        GUILayout.Label("<color=#ff0000>" + sc.wiz_error + "</color>");
                    }
                    else if (sc.wiz_data["action"] == "play")
                    {
                        GUILayout.Label("* Play the specified clip or all clips. When you target one clip, you can control the loop count and play speed too.");
                    }
                    else if (sc.wiz_data["action"] == "pause")
                    {
                        GUILayout.Label("* Pause the specified clip or all clips. Resume from current frame when you play again.");
                    }
                    else if (sc.wiz_data["action"] == "stop")
                    {
                        GUILayout.Label("* Stop the specified clip or all clips. Start from beginning when you play again.");
                    }
                    else if (sc.wiz_data["action"] == "seek")
                    {
                        GUILayout.Label("* Seek to the specified frame of a clip. Start from there when you play it.");
                    }
                    else
                    {
                        sc.wiz_error = "Unexpected action!?";
                    }
                    GUILayout.EndHorizontal();
                }
                else if (sc.wiz_step == 22)
                {
                    // ObjCam command
                    // list object camera and set synChk
                    try
                    {
                    ocams = new List<string>();
                        foreach (OCICamera cam in sc.game.studio.dicObjectCtrl.Values)
                    {
                        ocams.Add(cam.name);
                    }
                        ocams.Add("");
                    }
                    catch
                    {
                        ocams = null;
                    }
                    if (ocams == null)
                    {
                        sc.wiz_error = "ERROR: Fail to get OCICamera! Object camera doesn't work!";
                        synChk = false;
                    }
                    else if (ocams.Count == 1)
                    {
                        sc.wiz_error = "WARNING: No object camera found! Syntax check DISABLED! Set all parameters on your own risk!";
                        synChk = false;
                    }
                    else
                    {
                        synChk = true;
                    }
                    // header line
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Wizard: Switch between normal camera and object camera");
                    GUILayout.FlexibleSpace();
                    if (!ocams.IsNullOrEmpty() && GUILayout.Button("<color=#00ff00>OK</color>", GUILayout.Width(60)))
                    {
                        sc.wiz_error = null;
                        var cname = sc.wiz_data["name"].Trim();
                        cmd = String.Format("objcam:%s", cname);
                        append_vnss_cmd(sc, cmd);
                        sc.wiz_step = 0;
                    }
                    if (GUILayout.Button("<color=#ff0000>Cancel</color>", GUILayout.Width(60)))
                    {
                        sc.wiz_step = 0;
                        sc.wiz_error = null;
                    }
                    GUILayout.EndHorizontal();
                    // contents
                    GUILayout.BeginHorizontal();
                    if (synChk)
                    {
                        GUILayout.Label("Target camera:", GUILayout.Width(100));
                        GUILayout.Label(sc.wiz_data["name"] != "" ? "<color=#ff0000>" + sc.wiz_data["name"] + "</color>" : "<color=#00ff00>normal camera</color>", GUILayout.Width(150));
                        if (GUILayout.Button("<", GUILayout.Width(20)))
                        {
                            cInx = ocams.IndexOf(sc.wiz_data["name"]);
                            cInx -= 1;
                            cInx %= ocams.Count;
                            sc.wiz_data["name"] = ocams[cInx];
                        }
                        if (GUILayout.Button(">", GUILayout.Width(20)))
                        {
                            cInx = ocams.IndexOf(sc.wiz_data["name"]);
                            cInx += 1;
                            cInx %= ocams.Count;
                            sc.wiz_data["name"] = ocams[cInx];
                        }
                    }
                    else if (!ocams.IsNullOrEmpty())
                    {
                        GUILayout.Label("Target camera:", GUILayout.Width(100));
                        sc.wiz_data["name"] = GUILayout.TextField(sc.wiz_data["name"], GUILayout.Width(150)).Trim();
                        GUILayout.Label("* Leave blank switch to normal camera");
                    }
                    GUILayout.EndHorizontal();
                    // Notification
                    if (!sc.wiz_error.IsNullOrEmpty())
                    {
                        GUILayout.Label("<color=#ff0000>" + sc.wiz_error + "</color>");
                    }
                    else
                    {
                        GUILayout.Label("* Switch to selected object camera or back to normal camera. Remember when object camera actived, it will overwrite normal camera setting <color=#ff0000>UNTIL</color> you switch back to normal camera.");
                    }
                }
                else
                {
                    GUILayout.Label("What?");
                    Console.WriteLine("Unknown wizard step:", sc.wiz_step);
                    sc.wiz_step = 0;
                }
                Func<object, object, object> selectPrevNext = (sid, dir) => {
                    var cids = curActors.keys().OrderBy(_p_1 => _p_1).ToList();
                    if (cids.Contains(sid))
                    {
                        return cids[(cids.IndexOf(sid) + dir) % cids.Count];
                    }
                    else
                    {
                        return cids[0];
                    }
                };
                Func<object, object> dispNameOfId = sid => {
                    if (curActors.keys().Contains(sid))
                    {
                        return "<color=#00ff00>" + sid + ": " + curActors[sid].text_name + "</color>";
                    }
                    else
                    {
                        return "<color=#ff0000>Not Set</color>";
                    }
                };
            }

            public static void append_vnss_cmd(SceneConsole sc, object cmd)
            {
                if (sc.cam_addvncmds.Length > 0 && !sc.cam_addvncmds.EndsWith("\n"))
                {
                    sc.cam_addvncmds += "\n";
                }
                sc.cam_addvncmds += cmd;
            }

            public static object analysis_vnss_cmd(string cmd)
            {
                var cmds = cmd.Trim().Split('\n');
                var acmds = new List<object>();
                foreach (var c in cmds)
                {
                    string org = c.Trim();
                    if (org.Length == 0)
                    {
                        continue;
                    }
                    var parameters = org.Split(':');
                    var action = parameters[0].ToLower();
                    var aplen = parameters.Length - 1;
                    var ap1 = aplen <= 0 ? null : parameters[1];
                    var ap2 = aplen <= 1 ? null : parameters[2];
                    var ap3 = aplen <= 2 ? null : parameters[3];
                    object desp = null;
                    var runable = false;
                    var catelog = "other";
                    var error = false;
                    if (action == "addbtn")
                    {
                        if (aplen == 2)
                        {
                            desp = String.Format("Custom button [%s] jumps to [%s]", ap1, ap2);
                        }
                        else
                        {
                            error = true;
                        }
                    }
                    else if (action == "nextstate")
                    {
                        if (aplen == 1)
                        {
                            desp = String.Format("Next button jumps to [%s]", ap1);
                        }
                        else
                        {
                            error = true;
                        }
                    }
                    else if (action == "timernext")
                    {
                        if (aplen == 2)
                        {
                            desp = String.Format("Auto jumps to [%s] after [%s] second", ap2, ap1);
                        }
                        else if (aplen == 1)
                        {
                            desp = String.Format("Auto go next after [%s] second", ap1);
                        }
                        else
                        {
                            error = true;
                        }
                    }
                    else if (action == "addbtnrnd")
                    {
                        if (aplen == 2)
                        {
                            desp = String.Format("Random button [%s] jumps to [%s]", ap1, ap2);
                        }
                        else
                        {
                            error = true;
                        }
                    }
                    else if (action == "addbtnms")
                    {
                        if (aplen == 2)
                        {
                            desp = String.Format("Ministate button [%s] invokes [%s]", ap1, ap2);
                        }
                        else
                        {
                            error = true;
                        }
                    }
                    else if (action == "showui")
                    {
                        if (aplen == 0)
                        {
                            desp = "Show UI";
                        }
                        else
                        {
                            error = true;
                        }
                    }
                    else if (action == "hideui")
                    {
                        if (aplen == 0)
                        {
                            desp = "Hide UI";
                        }
                        else if (aplen == 1)
                        {
                            desp = String.Format("Hide UI for [%s] second", ap1);
                        }
                        else
                        {
                            error = true;
                        }
                    }
                    else if (action == "lockui")
                    {
                        if (aplen == 0)
                        {
                            desp = "Lock UI";
                        }
                        else if (aplen == 1)
                        {
                            desp = String.Format("Lock UI for [%s] second", ap1);
                        }
                        else
                        {
                            error = true;
                        }
                    }
                    else if (action == "unlockui")
                    {
                        if (aplen == 0)
                        {
                            desp = "Unlock UI";
                        }
                        else
                        {
                            error = true;
                        }
                    }
                    else if (action == "runms")
                    {
                        if (aplen == 1)
                        {
                            desp = String.Format("Invoke ministate [%s]", ap1);
                        }
                        else
                        {
                            error = true;
                        }
                    }
                    else if (action == "synch" || action == "f_synch" || action == "synchr" || action == "f_synchr")
                    {
                        runable = true;
                        catelog = "vnframe";
                        if (aplen == 1)
                        {
                            desp = String.Format("Sync anime for [%s]", ap1) + action.EndsWith("r") ? " and restart anime" : "";
                        }
                        else if (aplen == 2)
                        {
                            desp = String.Format("Sync anime for [%s] with [%s]", ap1, ap2) + action.EndsWith("r") ? " and restart anime" : "";
                        }
                        else
                        {
                            error = true;
                        }
                    }
                    else if (action == "f_animclipnum")
                    {
                        runable = true;
                        catelog = "vnframe";
                        if (aplen == 2)
                        {
                            desp = "VNFrame: '" + org.getRange(0,35) + "...'";
                        }
                        else
                        {
                            error = true;
                        }
                    }
                    else if (action == "f_acts" || action == "f_actm" || action == "f_act" || action == "f_anime" || action == "f_actm_j")
                    {
                        runable = true;
                        catelog = "vnframe";
                        if (aplen == 3)
                        {
                            desp = "VNFrame: '" + org.getRange(0, 35) + "...'";
                        }
                        else
                        {
                            error = true;
                        }
                    }
                    else if (action == "f_clipplay")
                    {
                        runable = true;
                        catelog = "vnanime";
                        if (aplen >= 1)
                        {
                            desp = String.Format("VNAnime: Play clip [%s]", ap1);
                        }
                        else
                        {
                            desp = "VNAnime: Play all clips";
                        }
                    }
                    else if (action == "f_clipstop")
                    {
                        runable = true;
                        catelog = "vnanime";
                        if (aplen == 0)
                        {
                            desp = "VNAnime: Stop all clips";
                        }
                        else if (aplen == 1)
                        {
                            desp = String.Format("VNAnime: Stop clip [%s]", ap1);
                        }
                        else
                        {
                            error = true;
                        }
                    }
                    else if (action == "f_clippause")
                    {
                        runable = true;
                        catelog = "vnanime";
                        if (aplen == 0)
                        {
                            desp = "VNAnime: Pause all clips";
                        }
                        else if (aplen == 1)
                        {
                            desp = String.Format("VNAnime: Pause clip [%s]", ap1);
                        }
                        else
                        {
                            error = true;
                        }
                    }
                    else if (action == "f_clipseek")
                    {
                        runable = true;
                        catelog = "vnanime";
                        if (aplen == 2)
                        {
                            desp = String.Format("VNAnime: Seek to frame [%s] of clip [%s]", ap2, ap1);
                        }
                        else
                        {
                            error = true;
                        }
                    }
                    else if (action == "objcam")
                    {
                        runable = true;
                        catelog = "objcam";
                        if (aplen == 1)
                        {
                            if (ap1.Length > 0)
                            {
                                desp = String.Format("objcam: switch to camera [%s]", ap1);
                            }
                            else
                            {
                                desp = "objcam: switch back to normal camera";
                            }
                        }
                        else
                        {
                            error = true;
                        }
                    }
                    else
                    {
                        desp = String.Format("Unknown command: %s", org);
                    }
                    if (error || desp == null)
                    {
                        desp = action + " command with wrong syntax";
                    }
                    acmds.Add(new Dictionary<string, object> {
                {
                    "desp",
                    desp},
                {
                    "org",
                    org},
                {
                    "runable",
                    runable},
                {
                    "catelog",
                    catelog},
                {
                    "error",
                    error},
                {
                    "action",
                    action},
                {
                    "ap1",
                    ap1},
                {
                    "ap2",
                    ap2},
                {
                    "ap3",
                    ap3},
                {
                    "aplen",
                    aplen}});
                }
                return acmds;
            }

            public static object vnframe_take_snapshot(SceneConsole sc)
            {
                object dfs;
                IDataClass nfs;
                object ofs;
                var outputScript = new Dictionary<string, object>
                {
                };
                var curscn = sc.block[sc.cur_index];
                // diff actors
                foreach (var actId in curscn.actors.Keys)
                {
                    ofs = curscn.actors[actId];
                    if (actId == "sys")
                    {
                        nfs = GameSystem.export_sys_status(sc.game);
                    }
                    else
                    {
                        nfs = sc.game.scenef_get_actor(actId).export_full_status();
                    }
                    dfs = new Dictionary<string, object>
                    {
                    };
                    foreach (var key in nfs.keys())
                    {
                        if (!ofs.ContainsKey(key) || ofs[key] != nfs[key])
                        {
                            dfs[key] = nfs[key];
                        }
                    }
                    if (dfs.Count > 0)
                    {
                        outputScript[actId] = dfs;
                    }
                }
                // diff props
                foreach (var prpId in curscn.props.keys())
                {
                    ofs = curscn.props[prpId];
                    nfs = sc.game.scenef_get_propf(prpId).export_full_status();
                    dfs = new Dictionary<string, object>
                    {
                    };
                    foreach (var key in nfs.Keys)
                    {
                        if (!ofs.ContainsKey(key) || ofs[key] != nfs[key])
                        {
                            dfs[key] = nfs[key];
                        }
                    }
                    if (dfs.Count > 0)
                    {
                        outputScript[prpId] = dfs;
                    }
                }
                //print "outputScript:", outputScript
                return outputScript;
            }

            public static object runVNSSExtCmd(VNController game, Dictionary<string, string> vncmd)
            {
                try
                {
                    if (!vncmd["runable"])
                    {
                        return false;
                    }
                    // parse act
                    var ar = vncmd["org"].Split(':');
                    var act = new Dictionary<string, object>
                    {
                    };
                    act["origintext"] = vncmd["org"];
                    act["action"] = ar[0];
                    if (ar.Length > 1)
                    {
                        act["actionparam"] = ar[1];
                        if (ar.Length > 2)
                        {
                            act["actionparam2"] = ar[2];
                            if (ar.Length > 3)
                            {
                                act["actionparam3"] = ar[3];
                            }
                        }
                    }
                    // run vnss
                    if (vncmd["catelog"] == "vnframe")
                    {
                        return custom_action(game, act);
                    }
                    if (vncmd["catelog"] == "vnanime")
                    {
                        return custom_action(game, act);
                    }
                    if (vncmd["catelog"] == "objcam")
                    {
                        return custom_action(game, act);
                    }
                    return false;
                }
                catch
                {
                    return false;
                }
            }
        }
    }
