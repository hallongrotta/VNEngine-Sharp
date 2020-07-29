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
            Utils.sceneConsole.guiOnShow = true;
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
            Utils.setWindowName(Utils.sceneConsole.windowindex);
            game.wwidth = Utils.sceneConsole.windowwidth;
            game.wheight = Utils.sceneConsole.windowheight;
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
            UnityEngine.Debug.Log("Showing main window.");
            Utils.sceneConsole.scene_str_array = Utils.sceneConsole.scene_strings.ToArray();
            Utils.sceneConsole.fset = new List<string>(Utils.sceneConsole.nameset[0]);
            Utils.sceneConsole.mset = new List<string>(Utils.sceneConsole.nameset[1]);
            // prev_cam_index = sc.cur_cam
            // prev_sc_index = sc.cur_index
            // sc.prev_index = sc.cur_index
            try
            {
                if (!(Utils.sceneConsole.warning_param is null))
                {
                    WarningParam_s warning_params = (WarningParam_s)Utils.sceneConsole.warning_param;
                    warningUI(Utils.sceneConsole.warning_action, warning_params.func_param, msg: warning_params.msg, single_op: warning_params.single_op);
                }
                else if (Utils.sceneConsole.isFuncLocked == true)
                {
                    GUILayout.BeginVertical();
                    GUILayout.Space(10);
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(10);
                    GUILayout.Label("<size=20>" + Utils.sceneConsole.funcLockedText + "</size>");
                    // GUILayout.Label(sc.funcLockedText)
                    GUILayout.Space(10);
                    GUILayout.EndHorizontal();
                    GUILayout.Space(10);
                    GUILayout.EndVertical();
                }
                else
                {
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button("-", GUILayout.Width(45)))
                    {
                        minimizeWindow();
                    }
                    Utils.sceneConsole.windowindex = GUILayout.Toolbar(Utils.sceneConsole.windowindex, Utils.sceneConsole.consolenames);
                    GUILayout.EndHorizontal();
                    GUILayout.Space(10);
                    Utils.setWindowName(Utils.sceneConsole.windowindex);
                    // Scene Console
                    if (Utils.sceneConsole.windowindex == 0)
                    {
                        GUILayout.BeginVertical();
                        Utils.sceneConsole.subwinindex = GUILayout.Toolbar(Utils.sceneConsole.subwinindex, Utils.sceneConsole.options);
                        GUILayout.Space(10);
                        // Edit window
                        if (Utils.sceneConsole.subwinindex == 0)
                        {
                            sceneConsoleEditUI();
                        }
                        else if (Utils.sceneConsole.subwinindex == 1)
                        {
                            // Trackable window
                            sceneConsoleTrackable();
                        }
                        else if (Utils.sceneConsole.subwinindex == 2)
                        {
                            // Load/Save window
                            sceneConsoleLdSvUI();
                        }
                        else if (Utils.sceneConsole.subwinindex == 3)
                        {
                            // --------- Advanced controls -------------
                            sceneConsoleAdvUI();
                        }
                        else if (Utils.sceneConsole.subwinindex == 4)
                        {
                            // Ministates window
                            sceneConsoleMinistates();
                        }
                        else if (Utils.sceneConsole.subwinindex == 100)
                        {
                            // Render for advanced cam properties
                            //VNExt.render_wizard_ui(Utils.sceneConsole); TODO
                        }
                        GUILayout.FlexibleSpace();
                        GUILayout.BeginHorizontal();
                        // GUILayout.Label("<b>Warning:</b> Closing console removes all console data")
                        if (GUILayout.Button("About v" + mod_version, GUILayout.Width(100)))
                        {
                            //resetConsole(sc.game)
                            Utils.sceneConsole.show_blocking_message_time_sc("SceneSaveState " + mod_version + "\n\nFrom @keitaro\nLightweight and crossplatform version of SceneConsole mod by @chickenManX\nOriginal code by @chickenManX\nSome cool features by @countd360\n\nAlso includes:\nPose Library (by @keitaro, original code by @chickenManX)\nScene Utils (by @keitaro)\n(with Body and Face Sliders) (by @countd360)\n", 5.0f);
                        }
                        GUILayout.FlexibleSpace();
                        if (GUILayout.Button("Reset console", GUILayout.Width(100)))
                        {
                            Utils.resetConsole(Utils.sceneConsole.game);
                        }
                        if (GUILayout.Button("Close console", GUILayout.Width(100)))
                        {
                            var col = Utils.sceneConsole.sel_font_col;
                            Utils.sceneConsole.warning_action = Utils.sceneConsoleGUIClose;
                            Utils.sceneConsole.warning_param = new SceneConsole.WarningParam_s(String.Format("Do you really want to close window? (<b><color=%s>Warning:</color> All current scenedata will be deleted</b>)", col), null, false);
                        }
                        GUILayout.EndHorizontal();
                        GUILayout.EndVertical();
                        GUI.DragWindow();
                    }
                    else if (Utils.sceneConsole.windowindex == 1)
                    {
                        // Pose Console
                        //var _pc = posesavestate.init_from_sc(Utils.sceneConsole.game); //TODO add posesavestate
                        //posesavestate.poseConsoleUIFuncs();
                        //GUILayout.Label("No poses console for now ))")
                    }
                    else if (Utils.sceneConsole.windowindex == 2)
                    {
                        Utils.sceneUtilsUI();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("sceneSaveStateWindowGUI Exception:");
                Utils.sceneConsoleGUIClose();
                Utils.sceneConsole.game.show_blocking_message_time("sceneSaveState error: " + e.ToString());
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
                    Utils.sceneConsole.warning_param = null;
                }
                if (GUILayout.Button("Hell No!", GUILayout.Height(100)))
                {
                    Utils.sceneConsole.warning_param = null;
                }
            }
            else
            {
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("OK!", GUILayout.Height(100)))
                {
                    Utils.sceneConsole.warning_param = null;
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
                    Utils.sceneConsole.warning_param = null;
                }
                if (GUILayout.Button("Hell No!", GUILayout.Height(100)))
                {
                    Utils.sceneConsole.warning_param = null;
                }
            }
            else
            {
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("OK!", GUILayout.Height(100)))
                {
                    Utils.sceneConsole.warning_param = null;
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
            //Utils.sceneConsole = Utils.sceneConsole;
            Utils.sceneConsole.adv_scroll = GUILayout.BeginScrollView(Utils.sceneConsole.adv_scroll);
            GUILayout.Label("<b>Advanced controls</b>");
            GUILayout.Space(10);
            GUILayout.Label("Change character name:");
            GUILayout.BeginHorizontal();
            Utils.sceneConsole.charname = GUILayout.TextField(Utils.sceneConsole.charname);
            if (GUILayout.Button("Change selected", GUILayout.Width(110)))
            {
                Utils.sceneConsole.changeCharName(Utils.sceneConsole.charname);
            }
            GUILayout.EndHorizontal();
            GUILayout.Label("Status operations:");
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Copy selected status"))
            {
                Utils.sceneConsole.copySelectedStatus();
            }
            if (GUILayout.Button("Paste selected status"))
            {
                Utils.sceneConsole.pasteSelectedStatus();
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Copy selected status 2"))
            {
                Utils.sceneConsole.copySelectedStatus2();
            }
            if (GUILayout.Button("Paste selected status 2"))
            {
                Utils.sceneConsole.pasteSelectedStatus2();
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Copy selected status to tracking chara with same name"))
            {
                Utils.sceneConsole.copySelectedStatusToTracking(null);
            }
            // if GUILayout.Button("(without Pos)"):
            //     sc.copySelectedStatusToTracking(["pos"])
            GUILayout.EndHorizontal();
            //GUILayout.Space(15)
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("VN: all cameras on"))
            {
                Utils.sceneConsole.camSetAll(true);
            }
            if (GUILayout.Button("VN: all cameras off"))
            {
                Utils.sceneConsole.camSetAll(false);
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("VN: add Fake Lip Sync Ext, if no"))
            {
                /* TODO
                //var header = VNSceneScript.get_headerfolder(Utils.sceneConsole.game); 
                if (!(header is null))
                {
                    // vnscenescript.addaction_to_headerfolder(sc.game, ":useext:flipsync10")
                    // vnscenescript.addaction_to_headerfolder(sc.game, ":a:i:initflipsync:v10")
                    Utils.add_folder_if_not_exists(":useext:flipsync10", ":useext:flipsync", header);
                    Utils.add_folder_if_not_exists(":a:i:initflipsync:v10", ":a:i:initflipsync:", header);
                    Utils.sceneConsole.show_blocking_message_time_sc("Done!");
                }
                else
                {
                    Utils.sceneConsole.show_blocking_message_time_sc("Please, export VN at least one time before add Fake Lip Sync");
                }
                */
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(20);
            Utils.sceneConsole.autoLoad = GUILayout.Toggle(Utils.sceneConsole.autoLoad, "Load scene on select");
            GUILayout.Space(10);
            Utils.sceneConsole.autoAddCam = GUILayout.Toggle(Utils.sceneConsole.autoAddCam, "Auto add cam for new scenes");
            GUILayout.Space(10);
            Utils.sceneConsole.promptOnDelete = GUILayout.Toggle(Utils.sceneConsole.promptOnDelete, "Prompt before delete (scene/cam/chars)");
            GUILayout.Space(10);
            Utils.sceneConsole.skipClothesChanges = GUILayout.Toggle(Utils.sceneConsole.skipClothesChanges, "Don't process clothes changes on scene change");
            GUILayout.Space(10);
            Utils.sceneConsole.paramAnimCamIfPossible = GUILayout.Toggle(Utils.sceneConsole.paramAnimCamIfPossible, "Animate cam if possible");
            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            GUILayout.Label("Camera anim params: duration ");
            //Utils.sceneConsole.paramAnimCamDuration = GUILayout.TextField(Utils.sceneConsole.paramAnimCamDuration.ToString(), GUILayout.Width(40)); //TODO
            GUILayout.Label(", zoom-out");
            //Utils.sceneConsole.paramAnimCamZoomOut = GUILayout.TextField(Utils.sceneConsole.paramAnimCamZoomOut.ToString(), GUILayout.Width(40)); //TODO
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
                var chara = Utils.getSelectedChar(Utils.sceneConsole.game);
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
                Prop obj = Utils.getSelectedItem(Utils.sceneConsole.game);
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
            foreach (var command in Utils.sceneConsole.shortcuts.Keys.OrderBy(_p_1 => _p_1).ToList())
            {
                GUILayout.Label(String.Format("%s:", command), GUILayout.Width(110));
                //Utils.sceneConsole.shortcuts[command] = GUILayout.TextField(Utils.sceneConsole.shortcuts[command], GUILayout.Width(120)); TODO
                GUILayout.FlexibleSpace();
                cnt += 1;
                if (cnt % 2 == 0)
                {
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                }
            }
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
                Utils.sceneConsole.addSelectedToTrack();
            }
            GUILayout.Space(15);
            if (GUILayout.Button("Del selected", GUILayout.Height(50), GUILayout.Width(160)))
            {
                Utils.sceneConsole.delSelectedFromTrack();
            }
            GUILayout.Space(15);
            if (GUILayout.Button("Refresh", GUILayout.Height(50), GUILayout.Width(80)))
            {
                Utils.sceneConsole.game.scenef_register_actorsprops();
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Space(40);
            if (!Utils.sceneConsole.isSysTracking())
            {
                if (GUILayout.Button("Track scene environment", GUILayout.Height(50), GUILayout.Width(160)))
                {
                    Utils.sceneConsole.addSysTracking();
                }
            }
            else if (GUILayout.Button("UnTrack scene environment", GUILayout.Height(50), GUILayout.Width(160)))
            {
                Utils.sceneConsole.delSysTracking();
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Space(40);
            GUILayout.Label("Pro: Change selected char ID to ", GUILayout.Width(210));
            //GUILayout.Label("  Who say:", GUILayout.Width(80))
            Utils.sceneConsole.newid = GUILayout.TextField(Utils.sceneConsole.newid, GUILayout.Width(120));
            if (GUILayout.Button("Change", GUILayout.Width(60)))
            {
                //sc.cam_whosay = sc.get_next_speaker(sc.cam_whosay, False)
                Utils.sceneConsole.changeSelTrackID(Utils.sceneConsole.newid);
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
            Utils.sceneConsole.tracking_scroll = GUILayout.BeginScrollView(Utils.sceneConsole.tracking_scroll);
            GUILayout.Label("Actors:");
            var actors = Utils.sceneConsole.game.scenef_get_all_actors();
            foreach (var actorid in actors.Keys)
            {
                //GUILayout.Label("  "+actorid+": "+actors[actorid].text_name)
                //txt += "  "+actorid+": "+actors[actorid].text_name+"\n"
                //GUILayout.Label(txt)
                HSNeoOCIChar actor = actors[actorid];
                render_ui_for_tracking(actorid, actor);
            }
            GUILayout.Label("Props:");
            var props = Utils.sceneConsole.game.scenef_get_all_props();
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
                Utils.sceneConsole.game.studio.treeNodeCtrl.SelectSingle(elem.treeNodeObject);
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
            if (Utils.sceneConsole.updAutoStatesTimer == 0)
            {
                Utils.recalc_autostates();
            }
            Utils.sceneConsole.updAutoStatesTimer = (Utils.sceneConsole.updAutoStatesTimer + 1) % 200;
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
                Utils.sceneConsole.addSelectedMini();
            }
            GUILayout.FlexibleSpace();
            GUILayout.Label("(optional) custom name: ");
            // GUILayout.Label("  Who say:", GUILayout.Width(80))
            Utils.sceneConsole.mininewid = GUILayout.TextField(Utils.sceneConsole.mininewid, GUILayout.Width(120));
            GUILayout.Space(tablePadding);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Space(tablePadding);
            Utils.sceneConsole.isUseMsAuto = GUILayout.Toggle(Utils.sceneConsole.isUseMsAuto, "Use auto-states (operations with selected props)");
            GUILayout.Space(tablePadding);
            GUILayout.EndHorizontal();
            if (Utils.sceneConsole.isUseMsAuto)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(tablePadding);
                if (GUILayout.Button("Add Show/Hide", GUILayout.Width(100)))
                {
                    Utils.sceneConsole.addSelectedAutoShow("vis");
                }
                if (GUILayout.Button("Add Choice", GUILayout.Width(100)))
                {
                    Utils.sceneConsole.addSelectedAutoShow("choice");
                }
                if (GUILayout.Button("Del selected", GUILayout.Width(100)))
                {
                    Utils.sceneConsole.delSelectedAutoShow();
                }
                GUILayout.FlexibleSpace();
                GUILayout.Label("(opt) name: ");
                // GUILayout.Label("  Who say:", GUILayout.Width(80))
                Utils.sceneConsole.autoshownewid = GUILayout.TextField(Utils.sceneConsole.autoshownewid, GUILayout.Width(100));
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
            Utils.sceneConsole.miniset_scroll = GUILayout.BeginScrollView(Utils.sceneConsole.miniset_scroll);
            //for i in range(500):
            //    GUILayout.Label("State %s" % (str(i)))
            var mslist = Ministates.ministates_get_list(Utils.sceneConsole.game);
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
                                Ministates.ministates_run_elem(Utils.sceneConsole.game, el.obj);
                            }
                            catch (Exception e)
                            {
                                Utils.sceneConsole.show_blocking_message_time_sc(String.Format("Error during set state: %s", e.ToString()));
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
                foreach (var el0 in Utils.sceneConsole.arAutoStatesItemsVis)
                {
                    TreeNodeObject el = el0.treeNodeObject.parent;
                    if (el.textName != "")
                    {
                    }
                }
                foreach (var el0 in Utils.sceneConsole.arAutoStatesItemsChoice)
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
                if (Utils.sceneConsole.arAutoStatesItemsVis.Count > 0)
                {
                    GUILayout.Space(6);
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("A SHOW/HIDE:", GUILayout.Width(tableLabelW + 5));
                    GUILayout.BeginVertical();
                    GUILayout.BeginHorizontal();
                    i = 0;
                    foreach (var vs in Utils.sceneConsole.arAutoStatesItemsVis)
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
                                            Utils.sceneConsole.game.studio.treeNodeCtrl.SelectSingle(vis.treeNodeObject.parent);
                                        }
                                    }
                                    else if (Utils.treenode_check_select(vis.treeNodeObject.parent))
                                    {
                                        Utils.sceneConsole.game.studio.treeNodeCtrl.SelectSingle(vis.treeNodeObject.parent);
                                    }
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            Utils.sceneConsole.show_blocking_message_time_sc(String.Format("Error during set visible: %s", e.ToString()));
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
                foreach (var ic in Utils.sceneConsole.arAutoStatesItemsChoice)
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
                                    Utils.sceneConsole.game.studio.treeNodeCtrl.SelectSingle(el);
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
            Utils.sceneConsole.saveload_scroll = GUILayout.BeginScrollView(Utils.sceneConsole.saveload_scroll);
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
                if (Utils.sceneConsole.block.Count > 0)
                {
                    Utils.sceneConsole.warning_action = Utils.sceneConsole.loadSceneData;
                    Utils.sceneConsole.warning_param = new SceneConsole.WarningParam_s("Do you wish to load scenedata from current scene? (Will overwrite console data)", null, false);
                }
                else
                {
                    Utils.sceneConsole.loadSceneData();
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
                    Utils.sceneConsole.warning_action = Utils.sceneConsole.saveSceneData;
                    Utils.sceneConsole.warning_param = new SceneConsole.WarningParam_s("Scenedata exists. Overwrite?", fld, false);
                }
                else
                {
                    Utils.sceneConsole.saveSceneData(backup: false);
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
            Utils.sceneConsole.isSaveVerify = GUILayout.Toggle(Utils.sceneConsole.isSaveVerify, "Verify save", GUILayout.Height(20), GUILayout.Width(80));
            Utils.sceneConsole.isSaveOld = GUILayout.Toggle(Utils.sceneConsole.isSaveOld, "Old save 100%OK", GUILayout.Height(20), GUILayout.Width(125));
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
            Utils.sceneConsole.svname = GUILayout.TextField(Utils.sceneConsole.svname, GUILayout.Width(300));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(20);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("<color=#00ff00>Load</color> from file", GUILayout.Height(btnBigHeight), GUILayout.Width(210)))
            {
                if (Utils.sceneConsole.block.Count > 0)
                {
                    Utils.sceneConsole.warning_action = Utils.sceneConsole.loadSceneData;
                    Utils.sceneConsole.warning_param = new WarningParam_s("Do you wish to load scenedata from file? (Will overwrite console data)", new bool[] { true, false }, false);
                }
                else
                {
                    Utils.sceneConsole.loadSceneData(file: true, backup: false);
                }
            }
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("<color=#ff0000>Save</color> to file", GUILayout.Height(btnBigHeight), GUILayout.Width(210)))
            {
                // delete existing scenedata fld
                var fld_str = "-scfile:" + Utils.sceneConsole.svname;
                fld = Utils.getFolder(Utils.sceneConsole.game, Utils.sceneConsole.svname, true);
                if (!(fld == null))
                {
                    Utils.sceneConsole.warning_action = Utils.sceneConsole.saveToFile;
                    Utils.sceneConsole.warning_param = new WarningParam_s("Scenedata exists. Overwrite?", false, false);
                }
                else
                {
                    Utils.sceneConsole.saveToFile(backup: false);
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
                fld = Utils.getFolder(Utils.sceneConsole.game, "-scfile:", false);
                if (Utils.sceneConsole.block.Count > 0)
                {
                    if (fld == null)
                    {
                        Utils.sceneConsole.warning_action = Utils.sceneConsole.loadSceneData;
                        Utils.sceneConsole.warning_param = new WarningParam_s("Do you wish to load backup scenedata from scene? (Will overwrite console data)", new bool[] { false, true }, false);
                    }
                    else
                    {
                        Utils.sceneConsole.warning_action = Utils.sceneConsole.loadSceneData;
                        Utils.sceneConsole.warning_param = new WarningParam_s("Do you wish to load backup scenedata from file? (Will overwrite console data)", new bool[] { true, true }, false);
                    }
                }
                else if (fld == null)
                {
                    Utils.sceneConsole.loadSceneData(backup: true);
                }
                else
                {
                    Utils.sceneConsole.loadSceneData(file: true, backup: true);
                }
            }
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Load auto-timer backup file", GUILayout.Height(btnSmallHeight), GUILayout.Width(210)))
            {
                //sc.exportToVNSS()
                if (Utils.sceneConsole.block.Count > 0)
                {
                    Utils.sceneConsole.warning_action = Utils.sceneConsole.loadSceneDataBackupTimer;
                    Utils.sceneConsole.warning_param = new SceneConsole.WarningParam_s("Do you wish to load backup scenedata from file auto-saved by timer? (Will overwrite console data)", null, false);
                }
                else
                {
                    Utils.sceneConsole.loadSceneDataBackupTimer();
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
                Utils.sceneConsole.exportToVNSS();
            }
            //GUILayout.Space(210)
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("...or <color=#00ff00>run</color> VNSceneScript\nfrom beginning", GUILayout.Height(btnSmallHeight), GUILayout.Width(210)))
            {
                Utils.sceneConsole.runVNSS();
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            //GUILayout.Space(210)
            Utils.sceneConsole.vnFastIsRunImmediately = GUILayout.Toggle(Utils.sceneConsole.vnFastIsRunImmediately, "And run from cur scene", GUILayout.Height(20), GUILayout.Width(210));
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("from scene", GUILayout.Height(20), GUILayout.Width(105)))
            {
                Utils.sceneConsole.runVNSS("scene");
            }
            if (GUILayout.Button("from cam", GUILayout.Height(20), GUILayout.Width(105)))
            {
                Utils.sceneConsole.runVNSS("cam");
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
                Utils.sceneConsole.exportCamTexts();
            }
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Import cam texts\nfrom sss_camtexts.txt", GUILayout.Height(btnSmallHeight), GUILayout.Width(210)))
            {
                Utils.sceneConsole.importCamTexts();
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.EndScrollView();
        }

        public static void sceneConsoleEditUI()
        {
            VNCamera.VNData.addprops_struct addprops;
            object col;
            List<string> fset = Utils.sceneConsole.fset;
            List<string> mset = Utils.sceneConsole.mset;
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical();
            // Scene tab
            Utils.sceneConsole.scene_scroll = GUILayout.BeginScrollView(Utils.sceneConsole.scene_scroll, GUILayout.Width(Utils.sceneConsole.viewwidth));
            if (Utils.sceneConsole.block.Count > 0)
            {
                foreach (var i in Enumerable.Range(0, Utils.sceneConsole.block.Count))
                {
                    if (i == Utils.sceneConsole.cur_index)
                    {
                        col = Utils.sceneConsole.sel_font_col;
                    }
                    else
                    {
                        col = Utils.sceneConsole.nor_font_col;
                    }
                    string scn_name = Utils.sceneConsole.scene_str_array[i];
                    if (Utils.sceneConsole.block[i].cams.Count > 0 && Utils.sceneConsole.block[i].cams[0].hasVNData && Utils.sceneConsole.block[i].cams[0].addata.addparam)
                    {
                        addprops = Utils.sceneConsole.block[i].cams[0].addata.addprops;
                        if (addprops.addprops["a1"])
                        {
                            string sname = addprops.a1o.name;
                            if (sname != null && sname.Trim().Length > 0)
                            {
                                scn_name = sname + String.Format(" (%d)", i + 1);
                            }
                        }
                    }
                    if (GUILayout.Button(String.Format("<color=%s>%s</color>", col, scn_name)))
                    {
                        Utils.sceneConsole.cur_index = i;
                        if (Utils.sceneConsole.autoLoad == true)
                        {
                            Utils.sceneConsole.loadCurrentScene();
                            // sc.cur_index = GUILayout.SelectionGrid(sc.cur_index,sc.scene_str_array,1)
                        }
                    }
                }
            }
            GUILayout.EndScrollView();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Move up"))
            {
                Utils.sceneConsole.move_scene_up();
            }
            if (GUILayout.Button("Move down"))
            {
                Utils.sceneConsole.move_scene_down();
            }
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            GUILayout.BeginVertical();
            // Camera and character selection tabs
            GUILayout.BeginHorizontal();
            if (Utils.sceneConsole.cur_index > -1)
            {
                GUILayout.BeginVertical();
                Utils.sceneConsole.cam_scroll = GUILayout.BeginScrollView(Utils.sceneConsole.cam_scroll, GUILayout.Height(185), GUILayout.Width(Utils.sceneConsole.camviewwidth));
                foreach (var i in Enumerable.Range(0, Utils.sceneConsole.block[Utils.sceneConsole.cur_index].cams.Count - 0))
                {
                    if (i == Utils.sceneConsole.cur_cam)
                    {
                        col = Utils.sceneConsole.sel_font_col;
                    }
                    else
                    {
                        col = "#f9f9f9";
                    }
                    var cam = Utils.sceneConsole.block[Utils.sceneConsole.cur_index].cams[i];
                    VNCamera.VNData addparams = cam.addata;
                    GUILayout.BeginHorizontal();
                    // show name if available
                    var camtxt = String.Format("Cam %s", i.ToString());
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
                    if (GUILayout.Button(String.Format("<color=%s>%s</color>", col, camtxt)))
                    {
                        Utils.sceneConsole.cur_cam = i;
                        Utils.sceneConsole.setCamera(false);
                    }
                    if (GUILayout.Button(String.Format("<color=%s>a</color>", col), GUILayout.Width(22)))
                    {
                        Utils.sceneConsole.cur_cam = i;
                        Utils.sceneConsole.setCamera(true);
                    }
                    GUILayout.EndHorizontal();
                }
                GUILayout.EndScrollView();
                // sc.cur_cam = GUILayout.SelectionGrid(sc.cur_cam,sc.scene_cam_str,1,GUILayout.Height(200),GUILayout.Width(125))
                // if not sc.cur_cam == prev_cam_index:
                // sc.setCamera()
                GUILayout.Space(15);
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Add", GUILayout.Width(Utils.sceneConsole.camviewwidth * 0.7f)))
                {
                    Utils.sceneConsole.changeSceneCam("add");
                }
                if (GUILayout.Button("Del", GUILayout.Width(Utils.sceneConsole.camviewwidth * 0.3f)))
                {
                    if (Utils.sceneConsole.promptOnDelete)
                    {
                        Utils.sceneConsole.warning_action = Utils.sceneConsole.changeSceneCam;
                        Utils.sceneConsole.warning_param = new WarningParam_s("Delete selected cam?", "del", false);
                    }
                    else
                    {
                        Utils.sceneConsole.changeSceneCam("del");
                    }
                }
                GUILayout.EndHorizontal();
                if (GUILayout.Button("Update", GUILayout.Width(Utils.sceneConsole.camviewwidth + 5)))
                {
                    Utils.sceneConsole.changeSceneCam("upd");
                }
                GUILayout.Label("Move cam:");
                GUILayout.BeginHorizontal();
                var up = "\u2191";
                var down = "\u2193";
                if (GUILayout.Button(up, GUILayout.Width(Utils.sceneConsole.camviewwidth / 2)))
                {
                    Utils.sceneConsole.move_cam_up();
                }
                if (GUILayout.Button(down, GUILayout.Width(Utils.sceneConsole.camviewwidth / 2)))
                {
                    Utils.sceneConsole.move_cam_down();
                }
                GUILayout.EndHorizontal();
                GUILayout.EndVertical();
            }
            GUILayout.BeginVertical();
            if (true)
            {
                // len(sc.nameset[0])>0:
                Utils.sceneConsole.fset_scroll = GUILayout.BeginScrollView(Utils.sceneConsole.fset_scroll, GUILayout.Width(Utils.sceneConsole.viewwidth), GUILayout.Height(Utils.sceneConsole.viewheight));
                foreach (var i in Enumerable.Range(0, fset.Count - 0))
                {
                    if (i == Utils.sceneConsole.fset_index)
                    {
                        col = Utils.sceneConsole.sel_font_col;
                    }
                    else
                    {
                        col = Utils.sceneConsole.nor_font_col;
                    }
                    if (GUILayout.Button(String.Format("<color=%s>%s</color>", col, fset[i]), GUILayout.Height(40)))
                    {
                        Utils.sceneConsole.fset_index = i;
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
                if (Utils.sceneConsole.cur_index > -1 && Utils.sceneConsole.cur_cam > -1)
                {
                    GUILayout.Space(25);
                    if (GUILayout.Button("Copy cam set"))
                    {
                        Utils.sceneConsole.copyCamSet();
                    }
                }
            }
            GUILayout.EndVertical();
            GUILayout.BeginVertical();
            if (true)
            {
                // len(sc.nameset[1])>0:
                Utils.sceneConsole.mset_scroll = GUILayout.BeginScrollView(Utils.sceneConsole.mset_scroll, GUILayout.Width(Utils.sceneConsole.viewwidth), GUILayout.Height(Utils.sceneConsole.viewheight));
                foreach (var i in Enumerable.Range(0, mset.Count - 0))
                {
                    if (i == Utils.sceneConsole.mset_index)
                    {
                        col = Utils.sceneConsole.sel_font_col;
                    }
                    else
                    {
                        col = Utils.sceneConsole.nor_font_col;
                    }
                    if (GUILayout.Button(String.Format("<color=%s>%s</color>", col, mset[i]), GUILayout.Height(40)))
                    {
                        Utils.sceneConsole.mset_index = i;
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
                if (Utils.sceneConsole.cur_index > -1 && !Utils.sceneConsole.camset.IsNullOrEmpty())
                {
                    GUILayout.Space(25);
                    if (GUILayout.Button("Paste cam set"))
                    {
                        Utils.sceneConsole.pasteCamSet();
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
                Utils.sceneConsole.addAuto(insert: true);
            }
            if (GUILayout.Button("Dup scene", GUILayout.Height(25)))
            {
                Utils.sceneConsole.dupScene();
            }
            GUILayout.EndVertical();
            if (GUILayout.Button("Add scene (auto)", GUILayout.Height(55), GUILayout.Width(175)))
            {
                Utils.sceneConsole.addAuto();
            }
            if (GUILayout.Button("Update scene", GUILayout.Height(55)))
            {
                Utils.sceneConsole.addAuto(addsc: false);
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
                if (Utils.sceneConsole.promptOnDelete == true)
                {
                    Utils.sceneConsole.warning_action = Utils.sceneConsole.removeScene;
                    Utils.sceneConsole.warning_param = new SceneConsole.WarningParam_s("Delete selected scene?", null, false);
                }
                else
                {
                    Utils.sceneConsole.removeScene();
                }
            }
            GUILayout.BeginHorizontal();
            // if GUILayout.Button("Add scene (selected only)"):
            //     sc.addAuto(allbase=False)
            // if GUILayout.Button("Delete duplicate characters"):
            //     sc.removeDuplicates()
            GUILayout.EndHorizontal();
            if (!(Utils.sceneConsole.autoLoad == true))
            {
                GUILayout.Space(10);
                if (GUILayout.Button("Load Scene", GUILayout.Height(35)))
                {
                    Utils.sceneConsole.loadCurrentScene();
                }
            }
            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Load prev scene", GUILayout.Height(30)))
            {
                Utils.sceneConsole.goto_prev_sc();
            }
            if (GUILayout.Button("Load next scene", GUILayout.Height(30)))
            {
                Utils.sceneConsole.goto_next_sc();
            }
            GUILayout.EndHorizontal();
            // char texts
            GUILayout.Space(25);
            GUILayout.BeginHorizontal();
            Utils.sceneConsole.cam_addparam = GUILayout.Toggle(Utils.sceneConsole.cam_addparam, "  Use cam in Visual Novel");
            GUILayout.FlexibleSpace();
            if (Utils.sceneConsole.cam_addparam)
            {
                var txt = Utils.btntext_get_if_selected2("More", Utils.sceneConsole.cam_addprops.addprops["a1"] || Utils.sceneConsole.cam_addprops.addprops["a2"]);
                if (GUILayout.Button(txt, GUILayout.Height(20)))
                {
                    Utils.sceneConsole.subwinindex = 100;
                }
            }
            GUILayout.EndHorizontal();
            //GUILayout.Label("  Replics for VN for cam (not necessary):")
            // if GUILayout.Button("Add scene (selected only)"):
            //     sc.addAuto(allbase=False)
            // if GUILayout.Button("Delete duplicate characters"):
            //     sc.removeDuplicates()
            if (Utils.sceneConsole.cam_addparam)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("  Who say:", GUILayout.Width(90));
                Utils.sceneConsole.cam_whosay = GUILayout.TextField(Utils.sceneConsole.cam_whosay, GUILayout.Width(210));
                if (GUILayout.Button("<", GUILayout.Width(20)))
                {
                    Utils.sceneConsole.cam_whosay = Utils.sceneConsole.get_next_speaker(Utils.sceneConsole.cam_whosay, false);
                }
                if (GUILayout.Button(">", GUILayout.Width(20)))
                {
                    Utils.sceneConsole.cam_whosay = Utils.sceneConsole.get_next_speaker(Utils.sceneConsole.cam_whosay, true);
                }
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                GUILayout.Label("  What say:", GUILayout.Width(90));
                Utils.sceneConsole.cam_whatsay = GUILayout.TextField(Utils.sceneConsole.cam_whatsay, GUILayout.Width(210));
                if (GUILayout.Button("X", GUILayout.Width(20)))
                {
                    Utils.sceneConsole.cam_whatsay = "";
                }
                if (GUILayout.Button("...", GUILayout.Width(20)))
                {
                    Utils.sceneConsole.cam_whatsay = "...";
                }
                GUILayout.EndHorizontal();
                GUILayout.Space(5);
                GUILayout.BeginHorizontal();
                GUILayout.Label("  Adv VN cmds", GUILayout.Width(90));
                Utils.sceneConsole.cam_addvncmds = GUILayout.TextArea(Utils.sceneConsole.cam_addvncmds, GUILayout.Width(235), GUILayout.Height(55));
                if (GUILayout.Button("X", GUILayout.Width(20)))
                {
                    Utils.sceneConsole.cam_addvncmds = "";
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
            SceneConsole sc = Utils.sceneConsole;
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
                    Utils.sceneConsole.game.windowRect.width = Utils.sceneConsole.consoleWidth;
                    Utils.sceneConsole.game.windowRect.height = Utils.sceneConsole.consoleHeight;
                    Utils.sceneConsole.game.skin.funcWindowGUI = sceneConsoleSkinWindowGUI;
                }
                GUI.DragWindow();
            }
            catch (Exception e)
            {
                Console.WriteLine("sceneSaveStateWindowGUI Exception: " + e.ToString());               
                Utils.sceneConsoleGUIClose();
                Utils.sceneConsole.game.show_blocking_message_time("sceneSaveState error: " + e.ToString());
            }
        }

    }
}
