using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Studio;
using VNEngine;
using VNActor;
using System.Linq;
using ADV;
using static VNActor.Prop;
using static SceneSaveState.SceneConsole;

namespace SceneSaveState
{
    public static class UI
    {
        private static string mod_version = "1.0";

        public static void sceneConsoleGUIStart(VNNeoController game)
        {
            //sc.game_skin_saved = game.skin
            if (game.gdata.sss_game_skin_saved is null)
            {
                game.gdata.sss_game_skin_saved = game.skin;
            }

            var skin = new SkinCustomWindow();
            skin.funcSetup = sceneConsoleSkinSetup;
            //skin.funcWindowGUI = sceneConsoleSkinWindowGUI;
            game.skin_set(skin);
            // sc.originalWindowCallback = game.windowCallback
            // sc.originalwindowwidth = game.wwidth
            // sc.originalwindowheight = game.wheight
            SceneConsole.Instance.guiOnShow = true;
            // setWindowName(sc.windowindex)
            // game.wwidth = sc.windowwidth
            // game.wheight = sc.windowheight
            //
            // game.windowRect = Rect(Screen.width / 2 - game.wwidth * 1.5, Screen.height - game.wheight - 500,
            //                        game.wwidth + 50, game.wheight + 400)
            Utils.loadConfig();
            game.event_reg_listener("update", Utils.hook_update);
            // game.windowCallback = GUI.WindowFunction(sceneConsoleWindowFunc)
        }

        public static void sceneConsoleSkinSetup(VNNeoController game)
        {
            Utils.setWindowName(SceneConsole.Instance.windowindex);
            game.wwidth = SceneConsole.Instance.windowwidth;
            game.wheight = SceneConsole.Instance.windowheight;
            // #game.windowRect = Rect (Screen.width / 2 - game.wwidth / 2, Screen.height - game.wheight - 10, game.wwidth, game.wheight)
            var x = Utils.get_ini_value_def_int("WindowX", (int)(Screen.width - game.wwidth * 1.3));
            var y = Utils.get_ini_value_def_int("WindowY", Screen.height - game.wheight - 650);
            var w = Utils.get_ini_value_def_int("WindowWidth", game.wwidth + 50);
            var h = Utils.get_ini_value_def_int("WindowHeight", game.wheight + 400);
            // game.windowRect = Rect(Screen.width / 2 - game.wwidth * 1.5, Screen.height - game.wheight - 500,
            //                        game.wwidth + 50, game.wheight + 400)
            game.windowRect = new Rect(x, y, w, h);
            //game.windowCallback = GUI.WindowFunction(scriptHelperWindowGUI)
            game.windowStyle = game.windowStyleDefault;
        }

        public static void sceneConsoleSkinWindowGUI(int windowid)
        {
            sceneConsoleWindowFunc(windowid);
        }

        public static void sceneConsoleSkinWindowGUIMin(int windowid)
        {
            minimizeWindowFunc(windowid);
        }

        public static void sceneConsoleWindowFunc(int id)
        {
            SceneConsole.Instance.scene_str_array = SceneConsole.Instance.scene_strings.ToArray();
            SceneConsole.Instance.fset = new List<string>(SceneConsole.Instance.nameset[0]);
            SceneConsole.Instance.mset = new List<string>(SceneConsole.Instance.nameset[1]);
            // prev_cam_index = sc.cur_cam
            // prev_sc_index = sc.cur_index
            // sc.prev_index = sc.cur_index
            try
            {
                if (!(SceneConsole.Instance.warning_param is null))
                {
                    WarningParam_s warning_params = (WarningParam_s)SceneConsole.Instance.warning_param;
                    warningUI(SceneConsole.Instance.warning_action, warning_params.func_param, msg: warning_params.msg, single_op: warning_params.single_op);
                }
                else if (SceneConsole.Instance.isFuncLocked == true)
                {
                    GUILayout.BeginVertical();
                    GUILayout.Space(10);
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(10);
                    GUILayout.Label("<size=20>" + SceneConsole.Instance.funcLockedText + "</size>");
                    // GUILayout.Label(sc.funcLockedText)
                    GUILayout.Space(10);
                    GUILayout.EndHorizontal();
                    GUILayout.Space(10);
                    GUILayout.EndVertical();
                    if (GUILayout.Button("Ok.", GUILayout.Width(100)))
                    {
                        SceneConsole.Instance.isFuncLocked = false;
                    }
                }
                else
                {
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button("-", GUILayout.Width(45)))
                    {
                        minimizeWindow();
                    }
                    SceneConsole.Instance.windowindex = GUILayout.Toolbar(SceneConsole.Instance.windowindex, SceneConsole.Instance.consolenames);
                    GUILayout.EndHorizontal();
                    GUILayout.Space(10);
                    Utils.setWindowName(SceneConsole.Instance.windowindex);
                    // Scene Console
                    if (SceneConsole.Instance.windowindex == 0)
                    {
                        GUILayout.BeginVertical();
                        SceneConsole.Instance.subwinindex = GUILayout.Toolbar(SceneConsole.Instance.subwinindex, SceneConsole.Instance.options);
                        GUILayout.Space(10);
                        // Edit window
                        if (SceneConsole.Instance.subwinindex == 0)
                        {
                            sceneConsoleEditUI();
                        }
                        else if (SceneConsole.Instance.subwinindex == 1)
                        {
                            // Trackable window
                            sceneConsoleTrackable();
                        }
                        else if (SceneConsole.Instance.subwinindex == 2)
                        {
                            // Load/Save window
                            sceneConsoleLdSvUI();
                        }
                        else if (SceneConsole.Instance.subwinindex == 3)
                        {
                            // --------- Advanced controls -------------
                            sceneConsoleAdvUI();
                        }
                        else if (SceneConsole.Instance.subwinindex == 4)
                        {
                            // Ministates window
                            sceneConsoleMinistates();
                        }
                        else if (SceneConsole.Instance.subwinindex == 100)
                        {
                            // Render for advanced cam properties
                            //VNExt.render_wizard_ui(SceneConsole.Instance); TODO
                        }
                        GUILayout.FlexibleSpace();
                        GUILayout.BeginHorizontal();
                        // GUILayout.Label("<b>Warning:</b> Closing console removes all console data")
                        if (GUILayout.Button("About v" + mod_version, GUILayout.Width(100)))
                        {
                            //resetConsole(sc.game)
                            SceneConsole.Instance.show_blocking_message_time_sc("SceneSaveState " + mod_version + "\n\nFrom @keitaro\nLightweight and crossplatform version of SceneConsole mod by @chickenManX\nOriginal code by @chickenManX\nSome cool features by @countd360\n\nAlso includes:\nPose Library (by @keitaro, original code by @chickenManX)\nScene Utils (by @keitaro)\n(with Body and Face Sliders) (by @countd360)\n", 5.0f);
                        }
                        GUILayout.FlexibleSpace();
                        if (GUILayout.Button("Reset console", GUILayout.Width(100)))
                        {
                            Utils.resetConsole(SceneConsole.Instance.game);
                        }
                        if (GUILayout.Button("Close console", GUILayout.Width(100)))
                        {
                            var col = SceneConsole.Instance.sel_font_col;
                            SceneConsole.Instance.warning_action = Utils.sceneConsoleGUIClose;
                            SceneConsole.Instance.warning_param = new SceneConsole.WarningParam_s(String.Format("Do you really want to close window? (<b><color={0}>Warning:</color> All current scenedata will be deleted</b>)", col), null, false);
                        }
                        GUILayout.EndHorizontal();
                        GUILayout.EndVertical();
                        GUI.DragWindow();
                    }
                    else if (SceneConsole.Instance.windowindex == 1)
                    {
                        // Pose Console
                        //var _pc = posesavestate.init_from_sc(SceneConsole.Instance.game); //TODO add posesavestate
                        //posesavestate.poseConsoleUIFuncs();
                        //GUILayout.Label("No poses console for now ))")
                    }
                    else if (SceneConsole.Instance.windowindex == 2)
                    {
                        Utils.sceneUtilsUI();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("sceneSaveStateWindowGUI Exception: " + e.ToString());
                Utils.sceneConsoleGUIClose();
                SceneConsole.Instance.game.show_blocking_message_time("sceneSaveState error: " + e.ToString());
            }
        }

        public static void warningUI(Action func, string msg = "", bool single_op = false)
        {
            GUILayout.Space(125);
            // GUILayout.FlexibleSpace()
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label(msg);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(125);
            GUILayout.BeginHorizontal();
            if (!(single_op == true))
            {
                if (GUILayout.Button("Yes", GUILayout.Height(100)))
                {
                    func();
                    SceneConsole.Instance.warning_param = null;
                }
                if (GUILayout.Button("Hell No!", GUILayout.Height(100)))
                {
                    SceneConsole.Instance.warning_param = null;
                }
            }
            else
            {
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("OK!", GUILayout.Height(100)))
                {
                    SceneConsole.Instance.warning_param = null;
                }
                GUILayout.FlexibleSpace();
            }
            GUILayout.EndHorizontal();
            // GUILayout.FlexibleSpace()
        }

        public static void warningUI(Action<object> func, object func_param, bool single_op = false, string msg = "")
        {
            GUILayout.Space(125);
            // GUILayout.FlexibleSpace()
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label(msg);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(125);
            GUILayout.BeginHorizontal();
            if (!(single_op == true))
            {
                if (GUILayout.Button("Yes", GUILayout.Height(100)))
                {
                    func(func_param);
                    SceneConsole.Instance.warning_param = null;
                }
                if (GUILayout.Button("Hell No!", GUILayout.Height(100)))
                {
                    SceneConsole.Instance.warning_param = null;
                }
            }
            else
            {
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("OK!", GUILayout.Height(100)))
                {
                    SceneConsole.Instance.warning_param = null;
                }
                GUILayout.FlexibleSpace();
            }
            GUILayout.EndHorizontal();
            // GUILayout.FlexibleSpace()
        }

        public static void sceneConsoleAdvUI()
        {
            object v;
            int k;
            //SceneConsole.Instance = SceneConsole.Instance;
            SceneConsole.Instance.adv_scroll = GUILayout.BeginScrollView(SceneConsole.Instance.adv_scroll);
            GUILayout.Label("<b>Advanced controls</b>");
            GUILayout.Space(10);
            GUILayout.Label("Change character name:");
            GUILayout.BeginHorizontal();
            SceneConsole.Instance.charname = GUILayout.TextField(SceneConsole.Instance.charname);
            if (GUILayout.Button("Change selected", GUILayout.Width(110)))
            {
                SceneConsole.Instance.changeCharName(SceneConsole.Instance.charname);
            }
            GUILayout.EndHorizontal();
            GUILayout.Label("Status operations:");
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Copy selected status"))
            {
                SceneConsole.Instance.copySelectedStatus();
            }
            if (GUILayout.Button("Paste selected status"))
            {
                SceneConsole.Instance.pasteSelectedStatus();
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Copy selected status 2"))
            {
                SceneConsole.Instance.copySelectedStatus2();
            }
            if (GUILayout.Button("Paste selected status 2"))
            {
                SceneConsole.Instance.pasteSelectedStatus2();
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Copy selected status to tracking chara with same name"))
            {
                SceneConsole.Instance.copySelectedStatusToTracking(null);
            }
            // if GUILayout.Button("(without Pos)"):
            //     sc.copySelectedStatusToTracking(["pos"])
            GUILayout.EndHorizontal();
            //GUILayout.Space(15)
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("VN: all cameras on"))
            {
                SceneConsole.Instance.camSetAll(true);
            }
            if (GUILayout.Button("VN: all cameras off"))
            {
                SceneConsole.Instance.camSetAll(false);
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("VN: add Fake Lip Sync Ext, if no"))
            {
                /* TODO
                //var header = VNSceneScript.get_headerfolder(SceneConsole.Instance.game); 
                if (!(header is null))
                {
                    // vnscenescript.addaction_to_headerfolder(sc.game, ":useext:flipsync10")
                    // vnscenescript.addaction_to_headerfolder(sc.game, ":a:i:initflipsync:v10")
                    Utils.add_folder_if_not_exists(":useext:flipsync10", ":useext:flipsync", header);
                    Utils.add_folder_if_not_exists(":a:i:initflipsync:v10", ":a:i:initflipsync:", header);
                    SceneConsole.Instance.show_blocking_message_time_sc("Done!");
                }
                else
                {
                    SceneConsole.Instance.show_blocking_message_time_sc("Please, export VN at least one time before add Fake Lip Sync");
                }
                */
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(20);
            SceneConsole.Instance.autoLoad = GUILayout.Toggle(SceneConsole.Instance.autoLoad, "Load scene on select");
            GUILayout.Space(10);
            SceneConsole.Instance.autoAddCam = GUILayout.Toggle(SceneConsole.Instance.autoAddCam, "Auto add cam for new scenes");
            GUILayout.Space(10);
            SceneConsole.Instance.promptOnDelete = GUILayout.Toggle(SceneConsole.Instance.promptOnDelete, "Prompt before delete (scene/cam/chars)");
            GUILayout.Space(10);
            SceneConsole.Instance.skipClothesChanges = GUILayout.Toggle(SceneConsole.Instance.skipClothesChanges, "Don't process clothes changes on scene change");
            GUILayout.Space(10);
            SceneConsole.Instance.paramAnimCamIfPossible = GUILayout.Toggle(SceneConsole.Instance.paramAnimCamIfPossible, "Animate cam if possible");
            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            GUILayout.Label("Camera anim params: duration ");
            //SceneConsole.Instance.paramAnimCamDuration = GUILayout.TextField(SceneConsole.Instance.paramAnimCamDuration.ToString(), GUILayout.Width(40)); //TODO
            GUILayout.Label(", zoom-out");
            //SceneConsole.Instance.paramAnimCamZoomOut = GUILayout.TextField(SceneConsole.Instance.paramAnimCamZoomOut.ToString(), GUILayout.Width(40)); //TODO
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            // Debug purpose
            if (GUILayout.Button("Print block data"))
            {
                Console.WriteLine("::::::::::::Debug::::::::::::");
                // print "sc.cur.index :",sc.cur_index
                // print "sc.nameset :",sc.nameset
                // print "sc.block[sc.cur_index].mchars :",sc.block[sc.cur_index].mchars
                // print "sc.block[sc.cur_index].fchars :",sc.block[sc.cur_index].fchars
                // print "sc.block[sc.cur_index].props :", sc.block[sc.cur_index].props
            }
            if (GUILayout.Button("Print char data"))
            {
                Console.WriteLine("::::::::::::Debug::::::::::::");
                var chara = Utils.getSelectedChar(SceneConsole.Instance.game);
                if (!(chara == null))
                {
                    VNActor.Actor.ActorData state = (Actor.ActorData)chara.export_full_status();
                    var fk_dic = state.fk;
                    Console.WriteLine("fk_set = {");
                    foreach (KeyValuePair<int, Vector3> entry in fk_dic)
                    {
                        k = entry.Key;
                        v = entry.Value;
                        Console.WriteLine(k.ToString(), ":", v, ",");
                    }
                    Console.WriteLine("}");
                }
            }
            if (GUILayout.Button("Print Item FK"))
            {
                Console.WriteLine("::::::::::::Debug::::::::::::");
                Prop obj = Utils.getSelectedItem(SceneConsole.Instance.game);
                if (!(obj == null))
                {
                    PropData obst = (PropData)obj.export_full_status();
                    for (int i = 0; i < obst.fk_set.Count; i++)
                    {
                        Vector3 vector = obst.fk_set[i];
                        Console.WriteLine(i.ToString(), ":", vector, ",");
                    }
                }
            }
            GUILayout.Space(25);
            GUILayout.Label("<b>Shortcut settings</b>");
            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            var cnt = 0;
            /* TODO
            foreach (var command in SceneConsole.Instance.shortcuts.Keys.OrderBy(_p_1 => _p_1).ToList())
            {
                GUILayout.Label(String.Format("%s:", command), GUILayout.Width(110));
                //SceneConsole.Instance.shortcuts[command] = GUILayout.TextField(SceneConsole.Instance.shortcuts[command], GUILayout.Width(120)); TODO
                GUILayout.FlexibleSpace();
                cnt += 1;
                if (cnt % 2 == 0)
                {
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                }
            }
            */
            GUILayout.EndHorizontal();
            if (GUILayout.Button("Save config", GUILayout.Height(50)))
            {
                Utils.saveConfig();
            }
            GUILayout.EndScrollView();
        }

        public static void sceneConsoleTrackable()
        {
            //if sc is SceneConsole:
            // sc.svname = GUILayout.TextField(sc.svname)
            // GUILayout.Space(35)
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label(" ---------------------------------    Operations    ------------------------------------");
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(15);
            GUILayout.BeginHorizontal();
            GUILayout.Space(40);
            if (GUILayout.Button("Add selected", GUILayout.Height(50), GUILayout.Width(160)))
            {
                SceneConsole.Instance.addSelectedToTrack();
            }
            GUILayout.Space(15);
            if (GUILayout.Button("Del selected", GUILayout.Height(50), GUILayout.Width(160)))
            {
                SceneConsole.Instance.delSelectedFromTrack();
            }
            GUILayout.Space(15);
            if (GUILayout.Button("Refresh", GUILayout.Height(50), GUILayout.Width(80)))
            {
                SceneConsole.Instance.game.LoadTrackedActorsAndProps();
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Space(40);
            if (!SceneConsole.Instance.isSysTracking())
            {
                if (GUILayout.Button("Track scene environment", GUILayout.Height(50), GUILayout.Width(160)))
                {
                    SceneConsole.Instance.addSysTracking();
                }
            }
            else if (GUILayout.Button("UnTrack scene environment", GUILayout.Height(50), GUILayout.Width(160)))
            {
                SceneConsole.Instance.delSysTracking();
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Space(40);
            GUILayout.Label("Pro: Change selected char ID to ", GUILayout.Width(210));
            //GUILayout.Label("  Who say:", GUILayout.Width(80))
            SceneConsole.Instance.newid = GUILayout.TextField(SceneConsole.Instance.newid, GUILayout.Width(120));
            if (GUILayout.Button("Change", GUILayout.Width(60)))
            {
                //sc.cam_whosay = sc.get_next_speaker(sc.cam_whosay, False)
                SceneConsole.Instance.changeSelTrackID(SceneConsole.Instance.newid);
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(50);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label(" ----------------------------------------    Tracking chars/props     ----------------------------------------");
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(15);
            //GUILayout.BeginHorizontal()
            SceneConsole.Instance.tracking_scroll = GUILayout.BeginScrollView(SceneConsole.Instance.tracking_scroll);
            GUILayout.Label("Actors:");
            var actors = SceneConsole.Instance.game.scenef_get_all_actors();
            foreach (var actorid in actors.Keys)
            {
                //GUILayout.Label("  "+actorid+": "+actors[actorid].text_name)
                //txt += "  "+actorid+": "+actors[actorid].text_name+"\n"
                //GUILayout.Label(txt)
                HSNeoOCIChar actor = actors[actorid];
                render_ui_for_tracking(actorid, actor);
            }
            GUILayout.Label("Props:");
            var props = SceneConsole.Instance.game.scenef_get_all_props();
            foreach (var propid in props.Keys)
            {
                render_ui_for_tracking(propid, props[propid]);
            }
            GUILayout.EndScrollView();
        }

        // :type elem:HSNeoOCI
        public static void render_ui_for_tracking(object id, HSNeoOCI elem)
        {
            var txt = id + ": " + elem.text_name;
            GUILayout.BeginHorizontal();
            GUILayout.Space(20);
            var btntext = "";
            if (elem.visible_treenode)
            {
                btntext = "v";
            }
            if (GUILayout.Button(btntext, GUILayout.Width(22)))
            {
                elem.visible_treenode = !elem.visible_treenode;
            }
            bool isSelected = Utils.treenode_check_select(elem.treeNodeObject);
            if (GUILayout.Button(Utils.btntext_get_if_selected(txt, isSelected)))
            {
                SceneConsole.Instance.game.studio.treeNodeCtrl.SelectSingle(elem.treeNodeObject);
            }
            //GUILayout.Label(txt)
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        public static void sceneConsoleMinistates()
        {
            int i;
            string[] ar;
            // updating autostates - every 200 step
            if (SceneConsole.Instance.updAutoStatesTimer == 0)
            {
                Utils.recalc_autostates();
            }
            SceneConsole.Instance.updAutoStatesTimer = (SceneConsole.Instance.updAutoStatesTimer + 1) % 200;
            var tableLabelW = 90;
            var tableBtnW = 125;
            var tablePadding = 10;
            // sc.svname = GUILayout.TextField(sc.svname)
            // GUILayout.Space(35)
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Ministates - add states only for selected actors+props. No tracking. Auto-save into scene.\nYou can use prefixes, naming by \"prefix-name\"");
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(15);
            GUILayout.BeginHorizontal();
            GUILayout.Space(tablePadding);
            if (GUILayout.Button("Add state", GUILayout.Width(200)))
            {
                SceneConsole.Instance.addSelectedMini();
            }
            GUILayout.FlexibleSpace();
            GUILayout.Label("(optional) custom name: ");
            // GUILayout.Label("  Who say:", GUILayout.Width(80))
            SceneConsole.Instance.mininewid = GUILayout.TextField(SceneConsole.Instance.mininewid, GUILayout.Width(120));
            GUILayout.Space(tablePadding);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Space(tablePadding);
            SceneConsole.Instance.isUseMsAuto = GUILayout.Toggle(SceneConsole.Instance.isUseMsAuto, "Use auto-states (operations with selected props)");
            GUILayout.Space(tablePadding);
            GUILayout.EndHorizontal();
            if (SceneConsole.Instance.isUseMsAuto)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(tablePadding);
                if (GUILayout.Button("Add Show/Hide", GUILayout.Width(100)))
                {
                    SceneConsole.Instance.addSelectedAutoShow("vis");
                }
                if (GUILayout.Button("Add Choice", GUILayout.Width(100)))
                {
                    SceneConsole.Instance.addSelectedAutoShow("choice");
                }
                if (GUILayout.Button("Del selected", GUILayout.Width(100)))
                {
                    SceneConsole.Instance.delSelectedAutoShow();
                }
                GUILayout.FlexibleSpace();
                GUILayout.Label("(opt) name: ");
                // GUILayout.Label("  Who say:", GUILayout.Width(80))
                SceneConsole.Instance.autoshownewid = GUILayout.TextField(SceneConsole.Instance.autoshownewid, GUILayout.Width(100));
                GUILayout.Space(tablePadding);
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                GUILayout.Space(tablePadding);
                GUILayout.FlexibleSpace();
                GUILayout.Space(tablePadding);
                GUILayout.EndHorizontal();
            }
            GUILayout.Space(20);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label(" ----------------------------------------    Ministates     ----------------------------------------");
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(15);
            GUILayout.BeginHorizontal();
            GUILayout.Space(tablePadding);
            SceneConsole.Instance.miniset_scroll = GUILayout.BeginScrollView(SceneConsole.Instance.miniset_scroll);
            //for i in range(500):
            //    GUILayout.Label("State %s" % (str(i)))
            var mslist = Ministates.ministates_get_list(SceneConsole.Instance.game);
            // calculating prfixes
            var arPrefixes = new List<string> {
            ""
        };
            foreach (var el in mslist)
            {
                //mstate = HSNeoOCIFolder.create_from_treenode(fldMiniState)
                ar = Ministates.ministates_calc_prefix(el.name);
                if (arPrefixes.Contains(ar[0]))
                {
                }
                else
                {
                    arPrefixes.Add(ar[0]);
                }
            }
            // rendering ministates
            foreach (var prefix in arPrefixes)
            {
                GUILayout.Space(6);
                GUILayout.BeginHorizontal();
                var prefixtxt = prefix;
                if (prefixtxt == "")
                {
                    prefixtxt = "(default)";
                }
                GUILayout.Label(prefixtxt + ":", GUILayout.Width(tableLabelW));
                GUILayout.BeginVertical();
                GUILayout.BeginHorizontal();
                i = 0;
                foreach (var el in mslist)
                {
                    ar = Ministates.ministates_calc_prefix(el.name);
                    if (ar[0] == prefix)
                    {
                        i += 1;
                        if (GUILayout.Button(ar[1], GUILayout.Width(tableBtnW)))
                        {
                            try
                            {
                                Ministates.ministates_run_elem(SceneConsole.Instance.game, el.obj);
                            }
                            catch (Exception e)
                            {
                                SceneConsole.Instance.show_blocking_message_time_sc(String.Format("Error during set state: {0}", e.ToString()));
                                //return
                                //if i != 0 and (i % 3 == 0):
                            }
                        }
                    }
                    if (i % 3 == 0)
                    {
                        GUILayout.FlexibleSpace();
                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal();
                    }
                }
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                GUILayout.EndVertical();
                GUILayout.EndHorizontal();
            }
            try
            {
                // trying auto states - to avoid errors during making UI
                foreach (var el0 in SceneConsole.Instance.arAutoStatesItemsVis)
                {
                    TreeNodeObject el = el0.treeNodeObject.parent;
                    if (el.textName != "")
                    {
                    }
                }
                foreach (var el0 in SceneConsole.Instance.arAutoStatesItemsChoice)
                {
                    TreeNodeObject el = el0.treeNodeObject.parent;
                    if (el.textName != "")
                    {
                    }
                    foreach (var el2 in el.child)
                    {
                        if (el2.textName != "")
                        {
                        }
                        if (el2.visible)
                        {
                        }
                    }
                }
                // rendering auto vis
                if (SceneConsole.Instance.arAutoStatesItemsVis.Count > 0)
                {
                    GUILayout.Space(6);
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("A SHOW/HIDE:", GUILayout.Width(tableLabelW + 5));
                    GUILayout.BeginVertical();
                    GUILayout.BeginHorizontal();
                    i = 0;
                    foreach (var vs in SceneConsole.Instance.arAutoStatesItemsVis)
                    {
                        HSNeoOCIFolder vis = (HSNeoOCIFolder)vs;
                        ar = vis.text_name.Split(':');
                        i += 1;
                        try
                        {
                            if (ar[1] == "vis")
                            {
                                if (GUILayout.Button(Utils.btntext_get_if_selected2(ar[2], vis.treeNodeObject.parent.visible), GUILayout.Width(tableBtnW)))
                                {
                                    vis.treeNodeObject.parent.visible = !vis.treeNodeObject.parent.visible;
                                    if (vis.treeNodeObject.parent.visible)
                                    {
                                        if (Utils.treenode_check_select(vis.treeNodeObject.parent))
                                        {
                                        }
                                        else
                                        {
                                            SceneConsole.Instance.game.studio.treeNodeCtrl.SelectSingle(vis.treeNodeObject.parent);
                                        }
                                    }
                                    else if (Utils.treenode_check_select(vis.treeNodeObject.parent))
                                    {
                                        SceneConsole.Instance.game.studio.treeNodeCtrl.SelectSingle(vis.treeNodeObject.parent);
                                    }
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            SceneConsole.Instance.show_blocking_message_time_sc(String.Format("Error during set visible: {0}", e.ToString()));
                            // return
                            // if i != 0 and (i % 3 == 0):
                        }
                        if (i % 3 == 0)
                        {
                            GUILayout.FlexibleSpace();
                            GUILayout.EndHorizontal();
                            GUILayout.BeginHorizontal();
                        }
                    }
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();
                    GUILayout.EndVertical();
                    GUILayout.EndHorizontal();
                }
                // rendering choices
                foreach (var ic in SceneConsole.Instance.arAutoStatesItemsChoice)
                {
                    GUILayout.Space(6);
                    GUILayout.BeginHorizontal();
                    var lbname = "--tmp--";
                    HSNeoOCIFolder itchoice = (HSNeoOCIFolder)ic;
                    try
                    {
                        ar = itchoice.text_name.Split(':');
                        lbname = ar[2];
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Err during calc label name...");
                    }
                    GUILayout.Label(lbname, GUILayout.Width(tableLabelW));
                    GUILayout.BeginVertical();
                    GUILayout.BeginHorizontal();
                    i = 0;
                    foreach (TreeNodeObject el in itchoice.treeNodeObject.parent.child)
                    {
                        if (!el.textName.StartsWith("-msauto:"))
                        {
                            i += 1;
                            try
                            {
                                var btntext = Utils.btntext_get_if_selected2(el.textName, el.visible);
                                if (GUILayout.Button(btntext, GUILayout.Width(tableBtnW)))
                                {
                                    if (el.visible)
                                    {
                                        el.visible = false;
                                    }
                                    else
                                    {
                                        el.visible = true;
                                        foreach (var el2 in itchoice.treeNodeObject.parent.child)
                                        {
                                            if (el2 != el)
                                            {
                                                el2.visible = false;
                                            }
                                        }
                                    }
                                    SceneConsole.Instance.game.studio.treeNodeCtrl.SelectSingle(el);
                                }
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine("Err during render button choice...");
                            }
                            if (i % 3 == 0)
                            {
                                GUILayout.FlexibleSpace();
                                GUILayout.EndHorizontal();
                                GUILayout.BeginHorizontal();
                            }
                        }
                    }
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();
                    GUILayout.EndVertical();
                    GUILayout.EndHorizontal();
                }
            }
            catch (Exception e)
            {
                //print "VNGE SSS: try to recalc autostates...."
                GUILayout.Label(Utils.color_text_red("Trying to get autostates folders...."));
                Utils.recalc_autostates();
                //return
                // end of all elements
            }
            GUILayout.EndScrollView();
            GUILayout.Space(tablePadding);
            GUILayout.EndHorizontal();
        }

        public static void sceneConsoleLdSvUI()
        {
            object fld;
            // sc.svname = GUILayout.TextField(sc.svname)
            // GUILayout.Space(35)
            var btnBigHeight = 60;
            var btnSmallHeight = 50;
            SceneConsole.Instance.saveload_scroll = GUILayout.BeginScrollView(SceneConsole.Instance.saveload_scroll);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label(" ------------------------------------------    Data in scene folders    ------------------------------------------");
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(15);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("<color=#00ff00>Load</color> scene data", GUILayout.Height(btnBigHeight), GUILayout.Width(210)))
            {
                if (SceneConsole.Instance.block.Count > 0)
                {
                    SceneConsole.Instance.warning_action = SceneConsole.Instance.loadSceneData;
                    SceneConsole.Instance.warning_param = new SceneConsole.WarningParam_s("Do you wish to load scenedata from current scene? (Will overwrite console data)", null, false);
                }
                else
                {
                    SceneConsole.Instance.loadSceneData();
                }
            }
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("<color=#ff0000>Save</color> scene data", GUILayout.Height(btnBigHeight), GUILayout.Width(210)))
            {
                // delete existing scenedata fld
                //fld = getFolder(sc.game, "-scenesavestate", True)
                fld = HSNeoOCIFolder.find_single_startswith("-scenesavestate:");
                if (!(fld == null))
                {
                    SceneConsole.Instance.warning_action = SceneConsole.Instance.saveSceneData;
                    SceneConsole.Instance.warning_param = new SceneConsole.WarningParam_s("Scenedata exists. Overwrite?", fld, false);
                }
                else
                {
                    SceneConsole.Instance.saveSceneData(backup: false);
                }
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Space(210);
            GUILayout.FlexibleSpace();
            //sc.isSaveCompact = GUILayout.Toggle(sc.isSaveCompact, "Save compact (since 4.0)", GUILayout.Height(20), GUILayout.Width(210))
            //sc.isSaveCompact = GUILayout.Toggle(sc.isSaveCompact, "Save compact", GUILayout.Height(20),
            //                                     GUILayout.Width(100))
            SceneConsole.Instance.isSaveVerify = GUILayout.Toggle(SceneConsole.Instance.isSaveVerify, "Verify save", GUILayout.Height(20), GUILayout.Width(80));
            SceneConsole.Instance.isSaveOld = GUILayout.Toggle(SceneConsole.Instance.isSaveOld, "Old save 100%OK", GUILayout.Height(20), GUILayout.Width(125));
            // if GUILayout.Button("Save scene data", GUILayout.Height(80), GUILayout.Width(210)):
            //     # delete existing scenedata fld
            //     # fld = getFolder(sc.game, "-scenesavestate", True)
            //     fld = HSNeoOCIFolder.find_single_startswith("-scenesavestate:")
            //     if not fld == None:
            //         sc.warning_param = (sc.saveSceneData, 'Scenedata exists. Overwrite?', fld, False)
            //     else:
            //         sc.saveSceneData(fld=None, backup=False)
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(20);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label(" ----------------------------------------    Data on external file    ----------------------------------------");
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(15);
            GUILayout.BeginHorizontal();
            GUILayout.Space(40);
            GUILayout.Label("File name:");
            GUILayout.Space(20);
            SceneConsole.Instance.svname = GUILayout.TextField(SceneConsole.Instance.svname, GUILayout.Width(300));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(20);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("<color=#00ff00>Load</color> from file", GUILayout.Height(btnBigHeight), GUILayout.Width(210)))
            {
                if (SceneConsole.Instance.block.Count > 0)
                {
                    SceneConsole.Instance.warning_action = SceneConsole.Instance.loadSceneData;
                    SceneConsole.Instance.warning_param = new WarningParam_s("Do you wish to load scenedata from file? (Will overwrite console data)", new bool[] { true, false }, false);
                }
                else
                {
                    SceneConsole.Instance.loadSceneData(file: true, backup: false);
                }
            }
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("<color=#ff0000>Save</color> to file", GUILayout.Height(btnBigHeight), GUILayout.Width(210)))
            {
                // delete existing scenedata fld
                var fld_str = "-scfile:" + SceneConsole.Instance.svname;
                fld = Utils.getFolder(SceneConsole.Instance.game, SceneConsole.Instance.svname, true);
                if (!(fld == null))
                {
                    SceneConsole.Instance.warning_action = SceneConsole.Instance.saveToFile;
                    SceneConsole.Instance.warning_param = new WarningParam_s("Scenedata exists. Overwrite?", false, false);
                }
                else
                {
                    SceneConsole.Instance.saveToFile(backup: false);
                }
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(30);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label(" ----------------------------    Load from backup (scene/external file)   ---------------------------");
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(15);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Load backup scene data\n(scene/external file)", GUILayout.Height(btnSmallHeight), GUILayout.Width(210)))
            {
                fld = Utils.getFolder(SceneConsole.Instance.game, "-scfile:", false);
                if (SceneConsole.Instance.block.Count > 0)
                {
                    if (fld == null)
                    {
                        SceneConsole.Instance.warning_action = SceneConsole.Instance.loadSceneData;
                        SceneConsole.Instance.warning_param = new WarningParam_s("Do you wish to load backup scenedata from scene? (Will overwrite console data)", new bool[] { false, true }, false);
                    }
                    else
                    {
                        SceneConsole.Instance.warning_action = SceneConsole.Instance.loadSceneData;
                        SceneConsole.Instance.warning_param = new WarningParam_s("Do you wish to load backup scenedata from file? (Will overwrite console data)", new bool[] { true, true }, false);
                    }
                }
                else if (fld == null)
                {
                    SceneConsole.Instance.loadSceneData(backup: true);
                }
                else
                {
                    SceneConsole.Instance.loadSceneData(file: true, backup: true);
                }
            }
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Load auto-timer backup file", GUILayout.Height(btnSmallHeight), GUILayout.Width(210)))
            {
                //sc.exportToVNSS()
                if (SceneConsole.Instance.block.Count > 0)
                {
                    SceneConsole.Instance.warning_action = SceneConsole.Instance.loadSceneDataBackupTimer;
                    SceneConsole.Instance.warning_param = new SceneConsole.WarningParam_s("Do you wish to load backup scenedata from file auto-saved by timer? (Will overwrite console data)", null, false);
                }
                else
                {
                    SceneConsole.Instance.loadSceneDataBackupTimer();
                }
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            // GUILayout.Label(
            //     " ----------------------------    Load from backup (scene/external file)   ---------------------------")
            GUILayout.Label(" -------------------------------    VN Export   ------------------------------");
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(15);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("<color=#00ff00>Export</color> scenes and cams\nto VNSceneScript", GUILayout.Height(btnSmallHeight), GUILayout.Width(210)))
            {
                SceneConsole.Instance.exportToVNSS();
            }
            //GUILayout.Space(210)
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("...or <color=#00ff00>run</color> VNSceneScript\nfrom beginning", GUILayout.Height(btnSmallHeight), GUILayout.Width(210)))
            {
                SceneConsole.Instance.runVNSS();
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            //GUILayout.Space(210)
            SceneConsole.Instance.vnFastIsRunImmediately = GUILayout.Toggle(SceneConsole.Instance.vnFastIsRunImmediately, "And run from cur scene", GUILayout.Height(20), GUILayout.Width(210));
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("from scene", GUILayout.Height(20), GUILayout.Width(105)))
            {
                SceneConsole.Instance.runVNSS("scene");
            }
            if (GUILayout.Button("from cam", GUILayout.Height(20), GUILayout.Width(105)))
            {
                SceneConsole.Instance.runVNSS("cam");
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            // GUILayout.Label(
            //     " ----------------------------    Load from backup (scene/external file)   ---------------------------")
            GUILayout.Label(" -------------------------------    Cam VN texts export/import   ------------------------------");
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Space(5);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Export cam texts\nto sss_camtexts.txt", GUILayout.Height(btnSmallHeight), GUILayout.Width(210)))
            {
                SceneConsole.Instance.exportCamTexts();
            }
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Import cam texts\nfrom sss_camtexts.txt", GUILayout.Height(btnSmallHeight), GUILayout.Width(210)))
            {
                SceneConsole.Instance.importCamTexts();
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.EndScrollView();
        }

        public static void sceneConsoleEditUI()
        {
            VNCamera.VNData.addprops_struct addprops;
            object col;
            List<string> fset = SceneConsole.Instance.fset;
            List<string> mset = SceneConsole.Instance.mset;
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical();
            // Scene tab
            SceneConsole.Instance.scene_scroll = GUILayout.BeginScrollView(SceneConsole.Instance.scene_scroll, GUILayout.Width(SceneConsole.Instance.viewwidth));
            if (SceneConsole.Instance.block.Count > 0)
            {
                foreach (var i in Enumerable.Range(0, SceneConsole.Instance.block.Count))
                {
                    if (i == SceneConsole.Instance.cur_index)
                    {
                        col = SceneConsole.Instance.sel_font_col;
                    }
                    else
                    {
                        col = SceneConsole.Instance.nor_font_col;
                    }
                    string scn_name = SceneConsole.Instance.scene_str_array[i];
                    if (SceneConsole.Instance.block[i].cams.Count > 0 && SceneConsole.Instance.block[i].cams[0].hasVNData && SceneConsole.Instance.block[i].cams[0].addata.addparam)
                    {
                        addprops = SceneConsole.Instance.block[i].cams[0].addata.addprops;
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
                        SceneConsole.Instance.cur_index = i;
                        if (SceneConsole.Instance.autoLoad == true)
                        {
                            SceneConsole.Instance.loadCurrentScene();
                            // sc.cur_index = GUILayout.SelectionGrid(sc.cur_index,sc.scene_str_array,1)
                        }
                    }
                }
            }
            GUILayout.EndScrollView();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Move up"))
            {
                SceneConsole.Instance.move_scene_up();
            }
            if (GUILayout.Button("Move down"))
            {
                SceneConsole.Instance.move_scene_down();
            }
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            GUILayout.BeginVertical();
            // Camera and character selection tabs
            GUILayout.BeginHorizontal();
            if (SceneConsole.Instance.cur_index > -1)
            {
                GUILayout.BeginVertical();
                SceneConsole.Instance.cam_scroll = GUILayout.BeginScrollView(SceneConsole.Instance.cam_scroll, GUILayout.Height(185), GUILayout.Width(SceneConsole.Instance.camviewwidth));
                foreach (var i in Enumerable.Range(0, SceneConsole.Instance.block[SceneConsole.Instance.cur_index].cams.Count - 0))
                {
                    if (i == SceneConsole.Instance.cur_cam)
                    {
                        col = SceneConsole.Instance.sel_font_col;
                    }
                    else
                    {
                        col = "#f9f9f9";
                    }
                    var cam = SceneConsole.Instance.block[SceneConsole.Instance.cur_index].cams[i];
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
                        SceneConsole.Instance.cur_cam = i;
                        SceneConsole.Instance.setCamera(false);
                    }
                    if (GUILayout.Button(String.Format("<color={0}>a</color>", col), GUILayout.Width(22)))
                    {
                        SceneConsole.Instance.cur_cam = i;
                        SceneConsole.Instance.setCamera(true);
                    }
                    GUILayout.EndHorizontal();
                }
                GUILayout.EndScrollView();
                // sc.cur_cam = GUILayout.SelectionGrid(sc.cur_cam,sc.scene_cam_str,1,GUILayout.Height(200),GUILayout.Width(125))
                // if not sc.cur_cam == prev_cam_index:
                // sc.setCamera()
                GUILayout.Space(15);
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Add", GUILayout.Width(SceneConsole.Instance.camviewwidth * 0.7f)))
                {
                    SceneConsole.Instance.changeSceneCam("add");
                }
                if (GUILayout.Button("Del", GUILayout.Width(SceneConsole.Instance.camviewwidth * 0.3f)))
                {
                    if (SceneConsole.Instance.promptOnDelete)
                    {
                        SceneConsole.Instance.warning_action = SceneConsole.Instance.changeSceneCam;
                        SceneConsole.Instance.warning_param = new WarningParam_s("Delete selected cam?", "del", false);
                    }
                    else
                    {
                        SceneConsole.Instance.changeSceneCam("del");
                    }
                }
                GUILayout.EndHorizontal();
                if (GUILayout.Button("Update", GUILayout.Width(SceneConsole.Instance.camviewwidth + 5)))
                {
                    SceneConsole.Instance.changeSceneCam("upd");
                }
                GUILayout.Label("Move cam:");
                GUILayout.BeginHorizontal();
                var up = "\u2191";
                var down = "\u2193";
                if (GUILayout.Button(up, GUILayout.Width(SceneConsole.Instance.camviewwidth / 2)))
                {
                    SceneConsole.Instance.move_cam_up();
                }
                if (GUILayout.Button(down, GUILayout.Width(SceneConsole.Instance.camviewwidth / 2)))
                {
                    SceneConsole.Instance.move_cam_down();
                }
                GUILayout.EndHorizontal();
                GUILayout.EndVertical();
            }
            GUILayout.BeginVertical();
            if (true)
            {
                // len(sc.nameset[0])>0:
                SceneConsole.Instance.fset_scroll = GUILayout.BeginScrollView(SceneConsole.Instance.fset_scroll, GUILayout.Width(SceneConsole.Instance.viewwidth), GUILayout.Height(SceneConsole.Instance.viewheight));
                foreach (var i in Enumerable.Range(0, fset.Count - 0))
                {
                    if (i == SceneConsole.Instance.fset_index)
                    {
                        col = SceneConsole.Instance.sel_font_col;
                    }
                    else
                    {
                        col = SceneConsole.Instance.nor_font_col;
                    }
                    if (GUILayout.Button(String.Format("<color={0}>{1}</color>", col, fset[i]), GUILayout.Height(40)))
                    {
                        SceneConsole.Instance.fset_index = i;
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
                if (SceneConsole.Instance.cur_index > -1 && SceneConsole.Instance.cur_cam > -1)
                {
                    GUILayout.Space(25);
                    if (GUILayout.Button("Copy cam set"))
                    {
                        SceneConsole.Instance.copyCamSet();
                    }
                }
            }
            GUILayout.EndVertical();
            GUILayout.BeginVertical();
            if (true)
            {
                // len(sc.nameset[1])>0:
                SceneConsole.Instance.mset_scroll = GUILayout.BeginScrollView(SceneConsole.Instance.mset_scroll, GUILayout.Width(SceneConsole.Instance.viewwidth), GUILayout.Height(SceneConsole.Instance.viewheight));
                foreach (var i in Enumerable.Range(0, mset.Count - 0))
                {
                    if (i == SceneConsole.Instance.mset_index)
                    {
                        col = SceneConsole.Instance.sel_font_col;
                    }
                    else
                    {
                        col = SceneConsole.Instance.nor_font_col;
                    }
                    if (GUILayout.Button(String.Format("<color={0}>{1}</color>", col, mset[i]), GUILayout.Height(40)))
                    {
                        SceneConsole.Instance.mset_index = i;
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
                if (SceneConsole.Instance.cur_index > -1 && !SceneConsole.Instance.camset.IsNullOrEmpty())
                {
                    GUILayout.Space(25);
                    if (GUILayout.Button("Paste cam set"))
                    {
                        SceneConsole.Instance.pasteCamSet();
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
                SceneConsole.Instance.addAuto(insert: true);
            }
            if (GUILayout.Button("Dup scene", GUILayout.Height(25)))
            {
                SceneConsole.Instance.dupScene();
            }
            GUILayout.EndVertical();
            if (GUILayout.Button("Add scene (auto)", GUILayout.Height(55), GUILayout.Width(175)))
            {
                SceneConsole.Instance.addAuto();
            }
            if (GUILayout.Button("Update scene", GUILayout.Height(55)))
            {
                SceneConsole.Instance.addAuto(addsc: false);
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
                if (SceneConsole.Instance.promptOnDelete == true)
                {
                    SceneConsole.Instance.warning_action = SceneConsole.Instance.removeScene;
                    SceneConsole.Instance.warning_param = new SceneConsole.WarningParam_s("Delete selected scene?", null, false);
                }
                else
                {
                    SceneConsole.Instance.removeScene();
                }
            }
            GUILayout.BeginHorizontal();
            // if GUILayout.Button("Add scene (selected only)"):
            //     sc.addAuto(allbase=False)
            // if GUILayout.Button("Delete duplicate characters"):
            //     sc.removeDuplicates()
            GUILayout.EndHorizontal();
            if (!(SceneConsole.Instance.autoLoad == true))
            {
                GUILayout.Space(10);
                if (GUILayout.Button("Load Scene", GUILayout.Height(35)))
                {
                    SceneConsole.Instance.loadCurrentScene();
                }
            }
            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Load prev scene", GUILayout.Height(30)))
            {
                SceneConsole.Instance.goto_prev_sc();
            }
            if (GUILayout.Button("Load next scene", GUILayout.Height(30)))
            {
                SceneConsole.Instance.goto_next_sc();
            }
            GUILayout.EndHorizontal();
            // char texts
            GUILayout.Space(25);
            GUILayout.BeginHorizontal();
            SceneConsole.Instance.cam_addparam = GUILayout.Toggle(SceneConsole.Instance.cam_addparam, "  Use cam in Visual Novel");
            GUILayout.FlexibleSpace();
            if (SceneConsole.Instance.cam_addparam)
            {
                var txt = Utils.btntext_get_if_selected2("More", SceneConsole.Instance.cam_addprops.addprops["a1"] || SceneConsole.Instance.cam_addprops.addprops["a2"]);
                if (GUILayout.Button(txt, GUILayout.Height(20)))
                {
                    SceneConsole.Instance.subwinindex = 100;
                }
            }
            GUILayout.EndHorizontal();
            //GUILayout.Label("  Replics for VN for cam (not necessary):")
            // if GUILayout.Button("Add scene (selected only)"):
            //     sc.addAuto(allbase=False)
            // if GUILayout.Button("Delete duplicate characters"):
            //     sc.removeDuplicates()
            if (SceneConsole.Instance.cam_addparam)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("  Who say:", GUILayout.Width(90));
                SceneConsole.Instance.cam_whosay = GUILayout.TextField(SceneConsole.Instance.cam_whosay, GUILayout.Width(210));
                if (GUILayout.Button("<", GUILayout.Width(20)))
                {
                    SceneConsole.Instance.cam_whosay = SceneConsole.Instance.get_next_speaker(SceneConsole.Instance.cam_whosay, false);
                }
                if (GUILayout.Button(">", GUILayout.Width(20)))
                {
                    SceneConsole.Instance.cam_whosay = SceneConsole.Instance.get_next_speaker(SceneConsole.Instance.cam_whosay, true);
                }
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                GUILayout.Label("  What say:", GUILayout.Width(90));
                SceneConsole.Instance.cam_whatsay = GUILayout.TextField(SceneConsole.Instance.cam_whatsay, GUILayout.Width(210));
                if (GUILayout.Button("X", GUILayout.Width(20)))
                {
                    SceneConsole.Instance.cam_whatsay = "";
                }
                if (GUILayout.Button("...", GUILayout.Width(20)))
                {
                    SceneConsole.Instance.cam_whatsay = "...";
                }
                GUILayout.EndHorizontal();
                GUILayout.Space(5);
                GUILayout.BeginHorizontal();
                GUILayout.Label("  Adv VN cmds", GUILayout.Width(90));
                SceneConsole.Instance.cam_addvncmds = GUILayout.TextArea(SceneConsole.Instance.cam_addvncmds, GUILayout.Width(235), GUILayout.Height(55));
                if (GUILayout.Button("X", GUILayout.Width(20)))
                {
                    SceneConsole.Instance.cam_addvncmds = "";
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

        public static void minimizeWindow()
        {
            SceneConsole sc = SceneConsole.Instance;
            if (sc.game.windowRect.width > 200)
            {
                sc.consoleWidth = sc.game.windowRect.width;
                sc.consoleHeight = sc.game.windowRect.height;
                sc.game.windowRect.width = 120;
                sc.game.windowRect.height = 75;
                // Rect (Screen.width / 2 - sc.game.wwidth * 1.5, Screen.height - sc.game.wheight - 500,
                //               110, 75)
                //sc.game.windowCallback = GUI.WindowFunction(minimizeWindowFunc)
                sc.game.skin.funcWindowGUI = sceneConsoleSkinWindowGUIMin;
            }
            else
            {
                sc.game.windowRect.width = sc.consoleWidth;
                sc.game.windowRect.height = sc.consoleHeight;
                sc.game.skin.funcWindowGUI = sceneConsoleSkinWindowGUI;
                //sc.game.windowCallback = GUI.WindowFunction(sceneConsoleWindowFunc)
            }
        }

        public static void minimizeWindowFunc(object windowid)
        {
            try
            {
                if (GUILayout.Button("Expand", GUILayout.Width(100), GUILayout.Height(45)))
                {
                    SceneConsole.Instance.game.windowRect.width = SceneConsole.Instance.consoleWidth;
                    SceneConsole.Instance.game.windowRect.height = SceneConsole.Instance.consoleHeight;
                    SceneConsole.Instance.game.skin.funcWindowGUI = sceneConsoleSkinWindowGUI;
                }
                GUI.DragWindow();
            }
            catch (Exception e)
            {
                Console.WriteLine("sceneSaveStateWindowGUI Exception: " + e.ToString());               
                Utils.sceneConsoleGUIClose();
                //SceneConsole.Instance.game.show_blocking_message_time("sceneSaveState error: " + e.ToString()); TODO
            }
        }

    }
}
