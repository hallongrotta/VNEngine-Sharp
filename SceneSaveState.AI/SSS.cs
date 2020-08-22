using BepInEx;
using BepInEx.Configuration;
using SceneSaveState;
using UnityEngine;
using VNEngine;

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

        NeoV2Controller game;
        private bool visible = false;
        private Rect windowRect;
        private GUIStyle windowStyle;
        private GUI.WindowFunction windowCallback;
        private int wwidth;
        private int wheight;

        public static ConfigEntry<BepInEx.Configuration.KeyboardShortcut> SSSHotkey { get; private set; }

        internal void Start()
        {
            UnityEngine.Debug.Log("SceneSaveState started.");
            SSSHotkey = Config.Bind("Keyboard Shortcuts", "Toggle VN Controller Window", new BepInEx.Configuration.KeyboardShortcut(KeyCode.B), "Show or hide the VN Controller window in Studio");
            game = NeoV2Controller.Instance;
            //KKAPI.Chara.CharacterApi.RegisterExtraBehaviour<CharaStudioController>(GUID);
            //StudioSaveLoadApi.RegisterExtraBehaviour<AnimationControllerSceneController>(GUID);

            SceneConsole sc = new SceneConsole();

            KKAPI.Studio.SaveLoad.StudioSaveLoadApi.RegisterExtraBehaviour<SceneConsole>(GUID);

            UI.setWindowName(UI.windowindex);

            this.windowStyle = new GUIStyle("window");
            this.windowCallback = UI.sceneConsoleWindowFunc;
            sceneConsoleSkinSetup();
        }

        public void sceneConsoleSkinSetup()
        {
            UI.setWindowName(UI.windowindex);
            this.wwidth = UI.windowwidth;
            this.wheight = UI.windowheight;
            // #game.windowRect = Rect (Screen.width / 2 - game.wwidth / 2, Screen.height - game.wheight - 10, game.wwidth, game.wheight)
            var x = Screen.width - game.wwidth * 1.1f;
            var y = Screen.height - game.wheight - 600;
            var w = game.wwidth + 50;
            var h = game.wheight + 450;
            // game.windowRect = Rect(Screen.width / 2 - game.wwidth * 1.5, Screen.height - game.wheight - 500,
            //                        game.wwidth + 50, game.wheight + 400)
            this.windowRect = new Rect(x, y, w, h);
            //game.windowCallback = GUI.WindowFunction(scriptHelperWindowGUI)
            this.windowStyle = game.windowStyleDefault;
        }

        public void OnGUI()
        {
            if (SceneConsole.Instance.guiOnShow)
            {
                windowRect = GUILayout.Window(34652, this.windowRect, this.windowCallback, "sceneconsole");
            }
        }

        internal void Update()
        {

            if (SSSHotkey.Value.IsDown())
            {
                //UI.sceneConsoleGUIStart(game);
                SceneConsole.Instance.guiOnShow = !SceneConsole.Instance.guiOnShow;
            }
        }
    }
}
