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
            }
            else
            {
                Console.WriteLine("Invalid index:", index);
            }
        }

        public static void sceneConsoleSkinWindowGUI(int windowid)
        {
            sceneConsoleWindowFunc(windowid);
        }

        public static void sceneConsoleSkinWindowGUIMin(int windowid)
        {
            minimizeWindowFunc(Instance.GameController, windowid);
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

                            var buttonText = Instance.showTextBox ? "Text off" : "Text on";
                            if (GUILayout.Button(buttonText, GUILayout.Width(100)))
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
                Instance.GameController.show_blocking_message_time("sceneSaveState error: " + e.ToString());
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

        public static void minimizeWindow(VNController controller)
        {
            SceneConsole sc = Instance;
            if (controller.windowRect.width > 200)
            {
                sc.consoleWidth = controller.windowRect.width;
                sc.consoleHeight = controller.windowRect.height;
                controller.windowRect.width = 120;
                controller.windowRect.height = 75;
                // Rect (Screen.width / 2 - sc.game.wwidth * 1.5, Screen.height - sc.game.wheight - 500,
                //               110, 75)
                //sc.game.windowCallback = GUI.WindowFunction(minimizeWindowFunc)
                controller.TextBox.funcWindowGUI = sceneConsoleSkinWindowGUIMin;
            }
            else
            {
                controller.windowRect.width = sc.consoleWidth;
                controller.windowRect.height = sc.consoleHeight;
                controller.TextBox.funcWindowGUI = sceneConsoleSkinWindowGUI;
                //sc.game.windowCallback = GUI.WindowFunction(sceneConsoleWindowFunc)
            }
        }

        public static void minimizeWindowFunc(VNController controller, object windowid)
        {
            try
            {
                if (GUILayout.Button("Expand", GUILayout.Width(100), GUILayout.Height(45)))
                {
                    controller.windowRect.width = Instance.consoleWidth;
                    controller.windowRect.height = Instance.consoleHeight;
                    controller.TextBox.funcWindowGUI = sceneConsoleSkinWindowGUI;
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
