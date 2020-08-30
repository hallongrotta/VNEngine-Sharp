using BepInEx;
using BepInEx.Configuration;
using UnityEngine;
using VNEngine;
using KKAPI;

namespace SceneSaveState
{
    [BepInProcess(Constants.StudioProcessName)]
    //[BepInDependency(GUID)]
    [BepInPlugin(GUID, PluginName, Version)]
    [BepInDependency(ExtensibleSaveFormat.ExtendedSave.GUID)]
    [BepInDependency(KoikatuAPI.GUID)]
    public class SSS : BaseUnityPlugin
    {

        public const string PluginName = "SceneConsole";
        public const string GUID = "com.kasanari.bepinex.sceneconsole";
        public const string Version = "1.0";

        StudioController game;
        private Rect windowRect;
        private GUI.WindowFunction windowCallback;

        public static ConfigEntry<BepInEx.Configuration.KeyboardShortcut> SSSHotkey { get; private set; }

        internal void Start()
        {
            UnityEngine.Debug.Log("SceneSaveState started.");
            SSSHotkey = Config.Bind("Keyboard Shortcuts", "Toggle VN Controller Window", new BepInEx.Configuration.KeyboardShortcut(KeyCode.B), "Show or hide the VN Controller window in Studio");
            game = StudioController.Instance;
            //KKAPI.Chara.CharacterApi.RegisterExtraBehaviour<CharaStudioController>(GUID);
            //StudioSaveLoadApi.RegisterExtraBehaviour<AnimationControllerSceneController>(GUID);

            SceneConsole sc = new SceneConsole();

            KKAPI.Studio.SaveLoad.StudioSaveLoadApi.RegisterExtraBehaviour<SceneConsole>(GUID);

            UI.setWindowName(UI.windowindex);
            this.windowCallback = UI.sceneConsoleWindowFunc;
            sceneConsoleSkinSetup();
        }

        public void sceneConsoleSkinSetup()
        {
            UI.setWindowName(UI.windowindex);
            var x = UI.defaultWindowX;
            var y = UI.defaultWindowY;
            var w = UI.WindowWidth;
            var h = UI.WindowHeight;
            this.windowRect = new Rect(x, y, w, h);
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
