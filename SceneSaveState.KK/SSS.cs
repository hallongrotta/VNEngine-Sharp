using BepInEx;
using BepInEx.Logging;
using BepInEx.Configuration;
using UnityEngine;
using VNEngine;
using RootMotion.FinalIK;
using System;

namespace SceneSaveState
{
    [BepInProcess(VNEngine.Constants.StudioProcessName)]
    //[BepInDependency(GUID)]
    [BepInPlugin(GUID, PluginName, Version)]
    public class SSS : BaseUnityPlugin
    {

        public const string PluginName = "SceneConsole";
        public const string GUID = "com.kasanari.bepinex.sceneconsole";
        public const string Version = "1.0";

        CharaStudioController game;
        private bool visible = false;
        private Rect windowRect;
        private GUIStyle windowStyle;
        private GUI.WindowFunction windowCallback;
        private int wwidth;
        private int wheight;

        public static ConfigEntry<KeyboardShortcut> SSSHotkey { get; private set; }

        internal void Start()
        {
            UnityEngine.Debug.Log("SceneSaveState started.");
            SSSHotkey = Config.Bind("Keyboard Shortcuts", "Toggle VN Controller Window", new KeyboardShortcut(KeyCode.B), "Show or hide the VN Controller window in Studio");
            game = new CharaStudioController();
            //KKAPI.Chara.CharacterApi.RegisterExtraBehaviour<CharaStudioController>(GUID);
            //StudioSaveLoadApi.RegisterExtraBehaviour<AnimationControllerSceneController>(GUID);

            Utils.sceneConsole = new SceneConsole(game);

            Utils.setWindowName(Utils.sceneConsole.windowindex);

            this.windowStyle = new GUIStyle("window");
            this.windowCallback = new GUI.WindowFunction(UI.sceneConsoleWindowFunc);
            sceneConsoleSkinSetup();
        }

        public void sceneConsoleSkinSetup()
        {
            Utils.setWindowName(Utils.sceneConsole.windowindex);
            this.wwidth = Utils.sceneConsole.windowwidth;
            this.wheight = Utils.sceneConsole.windowheight;
            // #game.windowRect = Rect (Screen.width / 2 - game.wwidth / 2, Screen.height - game.wheight - 10, game.wwidth, game.wheight)
            var x = Screen.width - game.wwidth * 1.3f;
            var y = Screen.height - game.wheight - 650;
            var w = game.wwidth + 50;
            var h = game.wheight + 400;
            // game.windowRect = Rect(Screen.width / 2 - game.wwidth * 1.5, Screen.height - game.wheight - 500,
            //                        game.wwidth + 50, game.wheight + 400)
            this.windowRect = new Rect(x, y, w, h);
            //game.windowCallback = GUI.WindowFunction(scriptHelperWindowGUI)
            this.windowStyle = game.windowStyleDefault;
        }

        public void OnGUI()
        {
            if (Utils.sceneConsole.guiOnShow)
            {
                GUILayout.Window(0, this.windowRect, this.windowCallback, "sceneconsole");
            }    
        }

        internal void Update()
        {
            
            if (SSSHotkey.Value.IsDown())
            {
                UnityEngine.Debug.Log("UI Toggle");
                //UI.sceneConsoleGUIStart(game);
                Utils.sceneConsole.guiOnShow = !Utils.sceneConsole.guiOnShow;
            }              
        }
    }
}
