using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Configuration;
using BepInEx.Logging;
using ExtensibleSaveFormat;
using KKAPI;
using KKAPI.Studio;
using KKAPI.Studio.SaveLoad;
using KKAPI.Utilities;
using MessagePack;
using SceneSaveState;
using Studio;
using UnityEngine;
using VNActor;
using VNEngine;
using static SceneSaveState.UI;
using static SceneSaveState.VNDataComponent;
using static VNActor.Character;
using static VNActor.Item;
using static VNActor.Light;
using static VNEngine.Utils;
using KeyboardShortcut = BepInEx.Configuration.KeyboardShortcut;
using Light = VNActor.Light;

namespace SceneSaveState
{
    [BepInProcess(Constants.StudioProcessName)]
    [BepInPlugin(GUID, PluginName, Version)]
    [BepInDependency(ExtendedSave.GUID)]
    [BepInDependency(KoikatuAPI.GUID)]
    internal class SceneConsole : BaseUnityPlugin
    {
        internal const string PluginName = "SceneConsole";
        private const string GUID = "com.kasanari.bepinex.sceneconsole";
        private const string Version = "1.0";
        private const string about_text =
            "SceneSaveState/VNEngine C# rewrite by @kasanari\n" +
            "SceneSaveState/VNEngine by @keitaro\n" +
            "Original SceneConsole code by @chickenManX\n" +
            "Some features by @countd360\n";


        
        private new static ManualLogSource Logger;

        //private Manager<Chapter> ChapterManager;

        private Warning? warning;

        private string funcLockedText;

        private bool guiOnShow;

        private bool isFuncLocked;

        private const string mod_version = "1.0";

        private ConfigEntry<bool> autoAddCam;
        private ConfigEntry<bool> promptOnDelete;
        private ConfigEntry<bool> skipClothesChanges;
        private ConfigEntry<bool> paramAnimCamIfPossible;

        //private RoleTracker roleTracker;

        private GUI.WindowFunction windowCallback;

        private Rect windowRect;

        private bool showTextBox = false;

        private readonly SceneConsoleSaveLoadComponent saveLoadComponent;
        private readonly SceneConsoleCopyComponent copyPasteComponent;
        private readonly Camera camera;

        private ChapterManager ChapterManager {
            get
            {
                return GetSceneController().chapterManager;
            }
        }

        private RoleTracker roleTracker
        {
            get
            {
                return GetSceneController().roleTracker;
            }
        }


        internal VNController GameController
        { 
            get
            {
                return VNController.Instance;
            }
        }

        internal SceneConsole()
        {
            // init dict
            // initWordDict()
            // --- Some constants ---
            guiOnShow = false;
            // --- Essential Data ---
            //ChapterManager = new Manager<Chapter>();
            //roleTracker = new RoleTracker();

            copyPasteComponent = new SceneConsoleCopyComponent(this);
            camera = new Camera();

            //ChapterManager.Add(new Chapter());
            // blocking message
            funcLockedText = "...";
            isFuncLocked = false;

            uiVNData = VNData.empty();

            // skin_default internal

        }

        internal static ConfigEntry<KeyboardShortcut> SSSHotkey { get; private set; }

        internal StudioController game => StudioController.Instance;

        //internal static SceneConsole Instance { get; private set; }

        internal ManualLogSource GetLogger => Logger;

        internal void Start()
        {
            Logger = base.Logger;
            loadConfig();
            SceneConsoleSkinSetup();
            StudioSaveLoadApi.RegisterExtraBehaviour<SceneConsoleSaveLoadComponent>(GUID);
        }

        public static SceneConsoleSaveLoadComponent GetSceneController() => Chainloader.ManagerObject.transform.GetComponentInChildren<SceneConsoleSaveLoadComponent>();

        internal void loadConfig()
        {
            // Keyboard shortcuts
            SSSHotkey = Config.Bind("Keyboard Shortcuts", "Toggle VN Controller Window",
                new KeyboardShortcut(KeyCode.B), "Show or hide the VN Controller window in Studio");

            // Settings
            autoAddCam = Config.Bind("Scene Console Settings", "AutoAddCamera", true, "Auto add cam for new scenes");
            promptOnDelete = Config.Bind("Scene Console Settings", "AutoAddCamera", true,
                "Prompt before delete (scene/cam/chars)");
            skipClothesChanges = Config.Bind("Scene Console Settings", "SkipClothesChange", false,
                "Don't process clothes changes on scene change");
            paramAnimCamIfPossible = Config.Bind("Scene Console Settings", "AnimateCamsIfPossible", false,
                "Animate cam if possible");
        }

        internal void SafeSceneConsoleWindowFunc(int id)
        {
            try
            {
                sceneConsoleWindowFunc(id);
            }
            catch (Exception e)
            {
                Logger.LogError(e);
                this.guiOnShow = false;
            }
        }

        internal void sceneConsoleWindowFunc(int id)
        {
            var currentChapter = ChapterManager.Current;
            ColumnWidth = WindowWidth / 3;
            GUI.DragWindow(new Rect(0, 0, 10000, 20));

                if (warning is Warning w)
                {
                    warningUI(w, msg: w.msg, single_op: w.single_op);
                }
                else if (isFuncLocked == true)
                {
                    GUILayout.BeginVertical();
                    GUILayout.Space(10);
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(10);
                    GUILayout.Label("<size=20>" + funcLockedText + "</size>");
                    // GUILayout.Label(sc.funcLockedText)
                    GUILayout.Space(10);
                    GUILayout.EndHorizontal();
                    GUILayout.Space(10);
                    GUILayout.EndVertical();
                    if (GUILayout.Button("Ok.", GUILayout.Width(100)))
                    {
                        isFuncLocked = false;
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
                            
                            warning = SceneConsoleEditUI(currentChapter, camera);
                            break;
                        case 1:
                            // Trackable window
                            warning = roleTracker.sceneConsoleTrackable(game, ChapterManager, promptOnDelete.Value);
                            break;
                        case 2:
                            // Load/Save window
                            warning = saveLoadComponent.sceneConsoleLdSvUI(this, ChapterManager.Count > 0);
                            break;
                        case 3:
                            // --------- Advanced controls -------------
                            copyPasteComponent.sceneConsoleAdvUI(this.game, currentChapter);
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
                        warning = new Warning("Delete current scene data? This will not delete scene data saved to the card.", false, Reset);
                    }
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("Start VN from scene.", GUILayout.Width(100)))
                    {
                        runVNSS("scene");
                    }

                    var buttonText = showTextBox ? "Text off" : "Text on";
                    if (GUILayout.Button(buttonText, GUILayout.Width(100)))
                    {
                        runVNSS("scene");
                    }
                    if (GUILayout.Button("About v" + mod_version, GUILayout.Width(100)))
                    {
                        //resetConsole(sc.game)
                        show_blocking_message_time_sc($"SceneSaveState {mod_version}\n{about_text}", 5.0f);
                    }
                    if (GUILayout.Button("Close console", GUILayout.Width(100)))
                    {
                        sceneConsoleGUIClose();
                    }
                    GUILayout.EndHorizontal();
                    GUILayout.EndVertical();
                    GUI.DragWindow();
                }
            }
        

        internal void DrawConfigSettings()
        {
            autoAddCam.Value = GUILayout.Toggle(autoAddCam.Value, "Auto add cam for new scenes");
            GUILayout.Space(10);
            promptOnDelete.Value = GUILayout.Toggle(promptOnDelete.Value, "Prompt before delete (scene/cam/chars)");
            GUILayout.Space(10);
            skipClothesChanges.Value = GUILayout.Toggle(skipClothesChanges.Value, "Don't process clothes changes on scene change");
            GUILayout.Space(10);
            paramAnimCamIfPossible.Value = GUILayout.Toggle(paramAnimCamIfPossible.Value, "Animate cam if possible");
        }

        private VNData uiVNData;

        internal VNData? DrawVNDataOptions(VNData visibleVNData, RoleTracker roleTracker)
        {
            VNData? newVNData = null;
            var enabled = true;
            
            GUILayout.Label("Actor:");
            GUILayout.FlexibleSpace();

            string whosay = GUILayout.TextField(visibleVNData.whosay);

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("<", GUILayout.Width(20)))
            {
                whosay = roleTracker.get_next_speaker(visibleVNData.whosay, false);
            }
            if (GUILayout.Button(">", GUILayout.Width(20)))
            {
                whosay = roleTracker.get_next_speaker(visibleVNData.whosay, true);
            }
            GUILayout.EndHorizontal();

            GUILayout.FlexibleSpace();
            GUILayout.Label("Text:");

            string whatsay = GUILayout.TextArea(visibleVNData.whatsay, GUILayout.Height(85));

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("X", GUILayout.Width(20)))
            {
                whatsay = "";
            }
            else if (GUILayout.Button("...", GUILayout.Width(20)))
            {
                whatsay = "...";
            }
            if (GUILayout.Button("Save", GUILayout.Width(40)))
            {
                newVNData = new VNData(enabled, whosay, whatsay, visibleVNData.addvncmds, visibleVNData.addprops);
            }
            GUILayout.EndHorizontal();

            uiVNData.whosay = whosay;
            uiVNData.whatsay = whatsay;

            /*GUILayout.BeginHorizontal();
                GUILayout.Label("  Adv VN cmds", GUILayout.Width(90));
                Instance.currentVNData.addvncmds = GUILayout.TextArea(Instance.currentVNData.addvncmds, GUILayout.Width(235), GUILayout.Height(55));
                if (GUILayout.Button("X", GUILayout.Width(20)))
                {
                    Instance.currentVNData.addvncmds = "";
                }
                // if GUILayout.Button("X", GUILayout.Width(20)):
                //     sc.cam_whatsay = ""
                // if GUILayout.Button("...", GUILayout.Width(20)):
                //     sc.cam_whatsay = "..."
                GUILayout.EndHorizontal();
                */
            return newVNData;

        }

        internal VNData ResetVNData()
        {
            var oldData = uiVNData;
            uiVNData = VNData.empty();
            return oldData;
        }

        internal void DrawSceneSelector(Chapter c, Camera cam)
        {
            GUILayout.BeginVertical(GUILayout.Width(ColumnWidth));
            GUILayout.Label("Scenes");
            // Scene tab
            var selected_scene = c == null ? null : ChapterManager.DrawSceneTab(GameController, c, cam);
            var vnData = LoadScene(selected_scene, cam);
            if (vnData is VNData vn)
            {
                uiVNData = vn;
            }
            GUILayout.EndVertical();
        } 

        internal Warning? DrawEditButtons(Chapter c, Camera cam)
        {
            Warning? w = null;
            GUILayout.BeginVertical(GUILayout.Width(ColumnWidth));
            GUILayout.Label($"Scene Cameras. FOV: {(int)camera.cameraData.parse}");

            // Camera and character selection tabs
            Scene s = c?.Current;
            w = s is Scene ? s.DrawCamSelect(cam, GameController, promptOnDelete.Value) : w;
            GUILayout.EndVertical();

            // Column 3         
            GUILayout.BeginVertical(GUILayout.Width(ColumnWidth));
            GUILayout.Label("Scene Controls");

            w = c?.DrawSceneEditButtons(CreateScene, () => new View(cam.export(), ResetVNData()), promptOnDelete.Value, autoAddCam.Value) ?? w;
            w = ChapterManager.DrawChapterEditButtons(c, promptOnDelete.Value) ?? w;

            GUILayout.FlexibleSpace();

            if (s is Scene)
            {
                if (s.Current is View v)
                {
                    var vndata = DrawVNDataOptions(uiVNData, roleTracker);
                    if (vndata is VNData vn)
                    {
                        var result = v.HasItems ? v.Update(vn) : v.Add(vn);
                    }
                }
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndVertical();
            return w;
        }

        internal Warning? SceneConsoleEditUI(Chapter c, Camera cam)
        {
            GUILayout.BeginHorizontal();
            // Column 1
            DrawSceneSelector(c, cam);

            // Column 2
            var w = DrawEditButtons(c, cam);
            GUILayout.EndHorizontal();
            return w;
        }

        internal void SceneConsoleSkinSetup()
        {
            windowCallback = SafeSceneConsoleWindowFunc;
            var x = UI.defaultWindowX;
            var y = UI.defaultWindowY;
            var w = UI.WindowWidth;
            var h = UI.WindowHeight;
            windowRect = new Rect(x, y, w, h);
        }

        internal void OnGUI()
        {
            if (guiOnShow) windowRect = GUILayout.Window(34652, windowRect, windowCallback, "Scene Console");
        }

        internal void Update()
        {
            if (SSSHotkey.Value.IsDown())
                //UI.sceneConsoleGUIStart(game);
                guiOnShow = !guiOnShow;
        }



        //[Serializable]
        //[MessagePackObject]
        //internal class ChapterData
        //{
        //    [Key("Scenes")] internal Scene[] Scenes;

        //    internal ChapterData()
        //    {

        //    }

        //    internal ChapterData(Scene[] scenes)
        //    {
        //        Scenes = scenes;
        //    }
        //}





        // Blocking message functions
        internal void show_blocking_message(string text = "...")
        {
            funcLockedText = text;
            isFuncLocked = true;
        }


        internal Scene CreateScene()
        {
            return new Scene(game.export_full_status(), roleTracker.AllCharacters, roleTracker.AllProps,
                roleTracker.isSysTracking);
        }

        internal void hide_blocking_message(object game = null)
        {
            isFuncLocked = false;
        }

        internal void show_blocking_message_time_sc(string text = "...", float duration = 3f)
        {
            show_blocking_message(text);
            GameController.set_timer(duration, hide_blocking_message);
        }

        

        

        



        

        // Load scene







        

        

        internal VNData? LoadScene(Scene s, Camera c)
        {
            if (s is null) return null;
            s.SetSceneState(game, roleTracker);
            if (s.HasItems) return s.Current.setCamera(c, GameController); else return null;
        }

        internal void Reset()
        {
            //ChapterManager = new Manager<Chapter>();
            //ChapterManager.Add(new Chapter());
        }

        internal void NextSceneOrCamera(VNController vn, int i)
        {
            NextSceneOrCamera(vn, i, camera);
        }

        internal VNData? NextSceneOrCamera(VNController vn, int i, Camera c)
        {
            var chapter = ChapterManager.Current;
            var scene = ChapterManager.GoToNextSceneOrCam(vn, chapter, c);
            return LoadScene(scene, camera);
        }

        internal void runVNSS(string starfrom = "begin")
        {
            if (ChapterManager.Count == 0) return;
            GameController.ShowVNTextBox(new List<Button_s> { new Button_s(">>", NextSceneOrCamera, 1) });
            int calcPos;
            if (starfrom == "cam")
                //print self.cur_index, self.cur_cam
                calcPos = (ChapterManager.CurrentIndex + 1) * 100 + ChapterManager.Current.Current.CurrentIndex;
            else if (starfrom == "scene")
                calcPos = (ChapterManager.CurrentIndex + ChapterManager.Current.CurrentIndex + 1) * 100;
            else
                calcPos = 0;
            var chapter = ChapterManager.SetCurrent(calcPos);
            LoadScene(chapter.Current, camera);
            Console.WriteLine("Run VNSS from state {0}", calcPos.ToString());
            //game.vnscenescript_run_current(onEndVNSS, calcPos.ToString());
        }

        //def _exportAddBlock(self,fld_acode,):
        

        // Set scene chars with state data from dictionary



        internal string GetIDOfSelectedObject()
        {
            var objects = StudioAPI.GetSelectedObjects();

            foreach (var objectCtrl in objects)
                try
                {
                    if (objectCtrl is OCIChar c) return SceneFolders.GetActorID(c);
                }
                catch
                {
                }

            return null;
        }



        internal void warningUI(Warning w, string msg = "", bool single_op = false)
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
                    w.warningFunc();
                    warning = null;
                }
                if (GUILayout.Button("Hell No!", GUILayout.Height(100)))
                {
                    warning = null;
                }
            }
            else
            {
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("OK!", GUILayout.Height(100)))
                {
                    warning = null;
                }
                GUILayout.FlexibleSpace();
            }
            GUILayout.EndHorizontal();
            // GUILayout.FlexibleSpace()
        }

        internal void sceneConsoleGUIClose(object param)
        {
            sceneConsoleGUIClose();
        }

        internal void sceneConsoleGUIClose()
        {
            guiOnShow = false;
            GameController.windowName = "";
        }
    }
}