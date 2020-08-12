using Studio;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VNActor;
using VNEngine;
using static SceneSaveState.SceneConsole;
using static VNActor.Item;

namespace SceneSaveState
{
    public static partial class UI
    {
        private static string mod_version = "1.0";
        public static int subwinindex = 0;

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
            Instance.guiOnShow = true;
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
            Utils.setWindowName(Instance.windowindex);
            game.wwidth = Instance.windowwidth;
            game.wheight = Instance.windowheight;
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
            Instance.scene_str_array = Instance.scene_strings.ToArray();
            Instance.fset = new List<string>(Instance.nameset[0]);
            Instance.mset = new List<string>(Instance.nameset[1]);
            // prev_cam_index = sc.cur_cam
            // prev_sc_index = sc.cur_index
            // sc.prev_index = sc.cur_index
            GUI.DragWindow(new Rect(0, 0, 10000, 20));
            try
            {
                if (!(Instance.warning_param is null))
                {
                    WarningParam_s warning_params = (WarningParam_s)Instance.warning_param;
                    warningUI(Instance.warning_action, warning_params.func_param, msg: warning_params.msg, single_op: warning_params.single_op);
                }
                else if (Instance.isFuncLocked == true)
                {
                    GUILayout.BeginVertical();
                    GUILayout.Space(10);
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(10);
                    GUILayout.Label("<size=20>" + Instance.funcLockedText + "</size>");
                    // GUILayout.Label(sc.funcLockedText)
                    GUILayout.Space(10);
                    GUILayout.EndHorizontal();
                    GUILayout.Space(10);
                    GUILayout.EndVertical();
                    if (GUILayout.Button("Ok.", GUILayout.Width(100)))
                    {
                        Instance.isFuncLocked = false;
                    }
                }
                else
                {
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button("-", GUILayout.Width(45)))
                    {
                        minimizeWindow();
                    }
                    Instance.windowindex = GUILayout.Toolbar(Instance.windowindex, Instance.consolenames);
                    GUILayout.EndHorizontal();
                    GUILayout.Space(10);
                    Utils.setWindowName(Instance.windowindex);
                    // Scene Console
                    if (Instance.windowindex == 0)
                    {
                        GUILayout.BeginVertical();
                        subwinindex = GUILayout.Toolbar(subwinindex, Instance.options);
                        GUILayout.Space(10);
                        // Edit window
                        if (subwinindex == 0)
                        {
                            sceneConsoleEditUI();
                        }
                        else if (subwinindex == 1)
                        {
                            // Trackable window
                            sceneConsoleTrackable();
                        }
                        else if (subwinindex == 2)
                        {
                            // Load/Save window
                            sceneConsoleLdSvUI();
                        }
                        else if (subwinindex == 3)
                        {
                            // --------- Advanced controls -------------
                            sceneConsoleAdvUI();
                        }
                        else if (subwinindex == 4)
                        {
                            // Ministates window
                            sceneConsoleMinistates();
                        }
                        else if (subwinindex == 100)
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
                            Instance.show_blocking_message_time_sc("SceneSaveState " + mod_version + "\n\nFrom @keitaro\nLightweight and crossplatform version of SceneConsole mod by @chickenManX\nOriginal code by @chickenManX\nSome cool features by @countd360\n\nAlso includes:\nPose Library (by @keitaro, original code by @chickenManX)\nScene Utils (by @keitaro)\n(with Body and Face Sliders) (by @countd360)\n", 5.0f);
                        }
                        GUILayout.FlexibleSpace();
                        if (GUILayout.Button("Reset console", GUILayout.Width(100)))
                        {
                            Utils.resetConsole(Instance.game);
                        }
                        if (GUILayout.Button("Delete saved data", GUILayout.Width(100)))
                        {
                            Instance.deleteSaveData();
                        }
                        if (GUILayout.Button("Close console", GUILayout.Width(100)))
                        {
                            var col = Instance.sel_font_col;
                            Instance.warning_action = Utils.sceneConsoleGUIClose;
                            Instance.warning_param = new SceneConsole.WarningParam_s(String.Format("Do you really want to close window? (<b><color={0}>Warning:</color> All current scenedata will be deleted</b>)", col), null, false);
                        }
                        GUILayout.EndHorizontal();
                        GUILayout.EndVertical();
                        GUI.DragWindow();
                    }
                    else if (Instance.windowindex == 1)
                    {
                        // Pose Console
                        //var _pc = posesavestate.init_from_sc(SceneConsole.Instance.game); //TODO add posesavestate
                        //posesavestate.poseConsoleUIFuncs();
                        //GUILayout.Label("No poses console for now ))")
                    }
                    else if (Instance.windowindex == 2)
                    {
                        //Utils.sceneUtilsUI(); TODO
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("sceneSaveStateWindowGUI Exception: " + e.ToString());
                //Utils.sceneConsoleGUIClose();
                Instance.game.show_blocking_message_time("sceneSaveState error: " + e.ToString());
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
                    Instance.warning_param = null;
                }
                if (GUILayout.Button("Hell No!", GUILayout.Height(100)))
                {
                    Instance.warning_param = null;
                }
            }
            else
            {
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("OK!", GUILayout.Height(100)))
                {
                    Instance.warning_param = null;
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
                    Instance.warning_param = null;
                }
                if (GUILayout.Button("Hell No!", GUILayout.Height(100)))
                {
                    Instance.warning_param = null;
                }
            }
            else
            {
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("OK!", GUILayout.Height(100)))
                {
                    Instance.warning_param = null;
                }
                GUILayout.FlexibleSpace();
            }
            GUILayout.EndHorizontal();
            // GUILayout.FlexibleSpace()
        }


        public static void minimizeWindow()
        {
            SceneConsole sc = Instance;
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
                    Instance.game.windowRect.width = Instance.consoleWidth;
                    Instance.game.windowRect.height = Instance.consoleHeight;
                    Instance.game.skin.funcWindowGUI = sceneConsoleSkinWindowGUI;
                }
                GUI.DragWindow();
            }
            catch (Exception e)
            {
                Console.WriteLine("sceneSaveStateWindowGUI Exception: " + e.ToString());
                //Utils.sceneConsoleGUIClose();
                //SceneConsole.Instance.game.show_blocking_message_time("sceneSaveState error: " + e.ToString()); TODO
            }
        }
    }
}
