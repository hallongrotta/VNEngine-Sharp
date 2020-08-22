using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace VNEngine
{

    public class BaseController : BaseUnityPlugin
    {
        public const string PluginName = "VN Controller";
        public const string GUID = "com.kasanari.bepinex.vncontroller";
        public const string Version = "1.0";
        internal new static ManualLogSource Logger;

        //private Component component;
        //private int counter;
        //private bool show_buttons;
        public bool visible;
        //private GUIStyle style;
        private CameraControl cameraControl;
        private Rect windowRect;
        private bool lastCameraState = true;
        private GameCursor gameCursor;
        public string windowName;
        //private GUI.WindowFunction windowCallback;
        //private bool isClassicStudio;
        protected Dictionary<string, string> engineOptions;
        protected string engine_name;
        protected string pygamepath;

        public ManualLogSource GetLogger { get { return Logger; } }

        public bool checkKeyCode(string configkey)
        {
            var entry = new ConfigDefinition("Keyboard Shortcuts", configkey);
            if (entry != null)
            {
                if (Config.ContainsKey(entry))
                {
                    BepInEx.Configuration.KeyboardShortcut shortcut = (BepInEx.Configuration.KeyboardShortcut)Config[entry].BoxedValue;
                    return shortcut.IsDown();
                }
            }
            return false;
        }

        public bool CheckConfigEntry(string category, string key)
        {
            return true;
        }

        public void loadConfig()
        {
            Config.Bind("Keyboard Shortcuts", "ToggleVNControllerWindow", new BepInEx.Configuration.KeyboardShortcut(KeyCode.F8, KeyCode.LeftControl), "Show or hide the VN Controller window in Studio.");
            Config.Bind("Keyboard Shortcuts", "Reset", new BepInEx.Configuration.KeyboardShortcut(KeyCode.F3, KeyCode.LeftControl), "Reset VN Controller.");
            Config.Bind("Keyboard Shortcuts", "ReloadCurrentGame", new BepInEx.Configuration.KeyboardShortcut(KeyCode.F10), "Reload current game.");
            Config.Bind("Keyboard Shortcuts", "VNFrameDeveloperConsole", new BepInEx.Configuration.KeyboardShortcut(KeyCode.F5, KeyCode.LeftControl), "Show or hide the VN Controller window in Studio");
            Config.Bind("Keyboard Shortcuts", "DumpCamera", new BepInEx.Configuration.KeyboardShortcut(KeyCode.F4, KeyCode.LeftControl, KeyCode.LeftAlt), "Show or hide the VN Controller window in Studio");
            Config.Bind("Keyboard Shortcuts", "DeveloperConsole", new BepInEx.Configuration.KeyboardShortcut(KeyCode.F4, KeyCode.LeftControl), "Show or hide the VN Controller window in Studio");
            Config.Bind("Keyboard Shortcuts", "ReloadVNEngine", new BepInEx.Configuration.KeyboardShortcut(KeyCode.F1, KeyCode.LeftControl), "Show or hide the VN Controller window in Studio");
            Config.Bind("Skins", "usekeysforbuttons", false);
        }

        public BaseController()
        {
            Logger = base.Logger;
            //component = null; // will be assigned if exists as member
            //counter = 1;
            //show_buttons = false;
            visible = false;
            cameraControl = null;
            windowRect = new Rect(Screen.width / 2 - 50, Screen.height / 2 - 50, 400, 400);
            lastCameraState = true;
            gameCursor = null;
            windowName = "Window";
            //loading options
            loadConfig();
            //print self.engineOptions;

        }

        public void EnableCamera(bool value)
        {
            if (this.lastCameraState != value)
            {
                lastCameraState = value;
                if (cameraControl)
                {
                    cameraControl.enabled = value;
                }
                gameCursor.enabled = value;
            }
        }



        public void ResetWindow(int windowid)
        {
            //Workaround mouse/camera issues when dragging window
            if (GUIUtility.hotControl == 0)
            {
                EnableCamera(true);
            }

            if (Event.current.type == EventType.MouseDown)
            {
                GUI.FocusControl("");
                GUI.FocusWindow(windowid);
                EnableCamera(false);
            }
        }



        public void Start()
        {

            try
            {
                if (!gameCursor)
                {
                    gameCursor = UnityEngine.Object.FindObjectOfType<GameCursor>();
                }
            }


            catch (Exception)
            {
                //Logger.LogError("VNGE: passable error in Start:" + exception); //TODO
            }
        }
        /*
        public void OnGUI()
        {
            if (!visible)
            {
                return;
            }
            else
            {
                windowRect = GUI.Window(0, windowRect, windowCallback, windowName);
            }
        }
        */


        public void Update()
        {

            // Update is called less so better place to check keystate

            if (checkKeyCode("ToggleVNControllerWindow"))
            {
                visible = !visible;
            }
        }

    }
}
