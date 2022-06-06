using System;
using System.Collections.Generic;
using UnityEngine;
using VNEngine;
using static SceneSaveState.SceneConsole;

namespace SceneSaveState
{
    public static partial class UI
    {

        private const string about_text =
            "SceneSaveState C# rewrite by @kasanari\n" +
            "SceneSaveState/VNEngine by @keitaro\n" +
            "Original SceneConsole code by @chickenManX\n" +
            "Some cool features by @countd360\n";

        private static string mod_version = "1.0";
        public static int subwinindex = 0;

        private static int ColumnWidth;

        internal const int defaultWindowX = 1000;
        internal const int defaultWindowY = 70;

        internal static int windowindex = 0;

        internal static int WindowHeight = 550;
        internal static int WindowWidth = 550;

        public static Vector2 cam_scroll = new Vector2(0, 0);
        public static Vector2 fset_scroll = new Vector2(0, 0);
        public static Vector2 miniset_scroll = new Vector2(0, 0);
        public static Vector2 mset_scroll = new Vector2(0, 0);
        public static Vector2 saveload_scroll = new Vector2(0, 0);
        public static Vector2 scene_scroll = new Vector2(0, 0);
        public static Vector2 tracking_actors_scroll = new Vector2(0, 0);
        public static Vector2 tracking_props_scroll = new Vector2(0, 0);
        public static Vector2 adv_scroll = new Vector2(0, 0);

        public static string[] consolenames = new string[] { "SceneSaveState" };
        public static string[] options = new string[] { "Edit", "Tracking", "Load/Save", "Advanced"};


        public const string SelectedTextColor = "#f24115";
        public const string NormalTextColor = "#f9f9f9";

        public struct WarningParam_s
        {
            public string msg;
            public object func_param;
            public bool single_op;

            public WarningParam_s(string msg, bool v2) : this()
            {
                this.msg = msg;
                single_op = v2;
            }
        }

        public static WarningParam_s? warning_param;
        public static Action warning_action;

        public static void setWindowName(int index)
        {
            var names = new Dictionary<int, string> {
            {
                0,
                "SceneSaveState"},
            {
                1,
                "Pose Library"},
            {
                2,
                "Scene Utils"}};
            if (names.ContainsKey(index))
            {
                UI.windowindex = index;
                SceneConsole.Instance.game.windowName = names[index];
            }
            else
            {
                Console.WriteLine("Invalid index:", index);
            }
        }

        public static void sceneConsoleGUIStart(VNNeoController game)
        {
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
            //Utils.loadConfig();
            // game.windowCallback = GUI.WindowFunction(sceneConsoleWindowFunc)
        }

        public static void sceneConsoleSkinSetup(VNNeoController game)
        {
            setWindowName(windowindex);
            game.wwidth = WindowWidth;
            game.wheight = WindowHeight;
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
            ColumnWidth = WindowWidth / 3;
            GUI.DragWindow(new Rect(0, 0, 10000, 20));
            try
            {
                if (!(warning_param is null))
                {
                    WarningParam_s warning_params = (WarningParam_s)warning_param;
                    warningUI(warning_action, msg: warning_params.msg, single_op: warning_params.single_op);
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
                    GUILayout.BeginVertical();
                            subwinindex = GUILayout.Toolbar(subwinindex, options);
                            GUILayout.Space(10);
                            switch (subwinindex)
                            {
                                // Edit window
                                case 0:
                                    sceneConsoleEditUI();
                                    break;
                                case 1:
                                    // Trackable window
                                    sceneConsoleTrackable();
                                    break;
                                case 2:
                                    // Load/Save window
                                    sceneConsoleLdSvUI();
                                    break;
                                case 3:
                                    // --------- Advanced controls -------------
                                    sceneConsoleAdvUI();
                                    break;
                                case 4:
                                    // Ministates window
                                    //sceneConsoleMinistates();
                                    break;
                                case 100:
                                    // Render for advanced cam properties
                                    //VNExt.render_wizard_ui(SceneConsole.Instance); TODO
                                    break;
                            }
                            GUILayout.FlexibleSpace();
                            GUILayout.BeginHorizontal();
                            // GUILayout.Label("<b>Warning:</b> Closing console removes all console data")
                            if (GUILayout.Button("Reset scenes", GUILayout.Width(100)))
                            {
                                warning_action = Instance.Reset;
                                warning_param = new WarningParam_s("Delete current scene data? This will not delete scene data saved to the card.", false);
                            }
                            GUILayout.FlexibleSpace();
                            if (GUILayout.Button("Start VN from scene.", GUILayout.Width(100)))
                            {
                                Instance.runVNSS("scene");
                            }
                            if (GUILayout.Button("About v" + mod_version, GUILayout.Width(100)))
                            {
                                //resetConsole(sc.game)
                                Instance.show_blocking_message_time_sc($"SceneSaveState {mod_version}\n{about_text}", 5.0f);
                            }                   
                            if (GUILayout.Button("Close console", GUILayout.Width(100)))
                            {
                                Utils.sceneConsoleGUIClose();
                            }
                            GUILayout.EndHorizontal();
                            GUILayout.EndVertical();
                            GUI.DragWindow();
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
                    warning_param = null;
                }
                if (GUILayout.Button("Hell No!", GUILayout.Height(100)))
                {
                    warning_param = null;
                }
            }
            else
            {
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("OK!", GUILayout.Height(100)))
                {
                    warning_param = null;
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
