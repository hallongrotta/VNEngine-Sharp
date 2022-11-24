using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using ExtensibleSaveFormat;
using KKAPI;
using KKAPI.Studio;
using KKAPI.Studio.SaveLoad;
using MessagePack;
using Studio;
using UnityEngine;
using VNActor;
using VNEngine;
using static VNActor.Character;
using static VNActor.Item;
using static VNActor.Light;
using static VNEngine.Utils;
using static VNEngine.VNCamera;
using static VNEngine.VNCamera.VNData;
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
        internal const string GUID = "com.kasanari.bepinex.sceneconsole";
        internal const string Version = "1.0";

        internal const string backup_folder_name = "sssdata";
        internal const string defaultSpeakerAlias = "s";
        internal const string defaultSaveName = "SSS.dat";
        internal const string defaultBackupName = "SSS.dat.backup";
        internal new static ManualLogSource Logger;

        internal List<Folder> arAutoStatesItemsChoice;

        internal List<Folder> arAutoStatesItemsVis;

        internal ConfigEntry<bool> autoAddCam;

        internal string autoshownewid;

        internal Manager<Chapter> ChapterManager;

        internal List<CamData> camset;

        internal string charname;

        internal IDataClass<IVNObject<object>> clipboard_status;

        internal IDataClass<IVNObject<object>> clipboard_status2;
        internal float consoleHeight;

        internal float consoleWidth;

        internal VNData currentVNData;

        //internal int cur_cam;

        internal string funcLockedText;

        internal bool guiOnShow;

        internal bool isFuncLocked;

        internal bool isSysTracking = true;

        internal bool isUseMsAuto;

        internal int last_acc_id;

        internal string mininewid;

        internal string newid;

        internal float paramAnimCamDuration;

        internal ConfigEntry<bool> paramAnimCamIfPossible;

        internal string paramAnimCamStyle;

        internal float paramAnimCamZoomOut;

        internal ConfigEntry<bool> promptOnDelete;

        internal RoleTracker roleTracker;
        internal string SelectedRole = "";

        //internal Dictionary<string, KeyValuePair<string, string>> shortcuts;

        internal string skinDefault_sideApp;

        internal ConfigEntry<bool> skipClothesChanges;

        internal string svname = "";
        internal bool track_map = true;

        internal int updAutoStatesTimer;
        private GUI.WindowFunction windowCallback;

        private Rect windowRect;

        internal Manager<CamData> CamManager;

        internal bool showTextBox = false; 

        internal CameraController CameraController { 
        get
            {
                return CameraController.Instance;
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
            last_acc_id = 0;
            ChapterManager = new Manager<Chapter>();

            ChapterManager.Add(new Chapter());

            CamManager = new Manager<CamData>(new List<CamData>());

            // self.basechars = self.getAllBaseChars()
            // self.dupchars = self.getAllDupChars()
            // self.updateNameset()
            // :::: UI Data ::::

            // self.char_name = ""

            currentVNData = new VNData
            {
                enabled = false,
                whosay = "",
                whatsay = "",
                addvncmds = "",
                addprops = new addprops_struct()
            };

            currentVNData.addprops.a1 = false;
            currentVNData.addprops.a2 = false;

            newid = "";
            mininewid = "";
            autoshownewid = "";
            isUseMsAuto = false;

            // -- Advanced --
            charname = "";
            //this.shortcuts = new Dictionary<string, (string, string)>();
            paramAnimCamDuration = 1.5f;
            paramAnimCamStyle = "fast-slow";
            paramAnimCamZoomOut = 0.0f;
            // self.nwindowRect = None
            // -- Edit window --
            camset = new List<CamData>();
            updAutoStatesTimer = 0;
            arAutoStatesItemsVis = new List<Folder>();
            arAutoStatesItemsChoice = new List<Folder>();
            // blocking message
            funcLockedText = "...";
            isFuncLocked = false;
            // skin_default internal
            skinDefault_sideApp = "";
            Instance = this;
            roleTracker = new RoleTracker();
        }

        internal static ConfigEntry<KeyboardShortcut> SSSHotkey { get; private set; }

        internal double saveDataSize { get; private set; }

        internal StudioController game => StudioController.Instance;

        internal static SceneConsole Instance { get; private set; }

        internal ManualLogSource GetLogger => Logger;

        internal void Awake()
        {
            StudioSaveLoadApi.RegisterExtraBehaviour<SaveLoadController>(GUID);
        }

        internal Scene CurrentScene => CurrentChapter.Current;

        internal Chapter CurrentChapter => ChapterManager.Current;

        internal void Start()
        {
            Logger = base.Logger;
            loadConfig();
            sceneConsoleSkinSetup();
        }

        public void loadConfig()
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

        internal void sceneConsoleSkinSetup()
        {
            windowCallback = UI.sceneConsoleWindowFunc;
            UI.setWindowName(UI.windowindex);
            var x = UI.defaultWindowX;
            var y = UI.defaultWindowY;
            var w = UI.WindowWidth;
            var h = UI.WindowHeight;
            windowRect = new Rect(x, y, w, h);
        }

        internal void OnGUI()
        {
            if (Instance.guiOnShow) windowRect = GUILayout.Window(34652, windowRect, windowCallback, "Scene Console");
        }

        internal void Update()
        {
            if (SSSHotkey.Value.IsDown())
                //UI.sceneConsoleGUIStart(game);
                Instance.guiOnShow = !Instance.guiOnShow;
        }

        internal double CalculateSaveDataSize(byte[] bytes)
        {
            return (double) bytes.Length / 1000;
        }

        //[Serializable]
        //[MessagePackObject]
        //public class ChapterData
        //{
        //    [Key("Scenes")] public Scene[] Scenes;

        //    public ChapterData()
        //    {

        //    }

        //    public ChapterData(Scene[] scenes)
        //    {
        //        Scenes = scenes;
        //    }
        //}


        internal PluginData GetPluginData()
        {
            var pluginData = new PluginData();
            if (ChapterManager.Count > 0) 
                try
                {

                    var chapters = ChapterManager.ExportItems();
                    var chapterData = new Scene[chapters.Length][];
                    for (var index = 0; index < chapters.Length; index++)
                    {
                        chapterData[index] = chapters[index].ExportItems();
                    }

                    var serializedChapters = Utils.SerializeData(chapterData);
                    pluginData.data["chapters"] = serializedChapters;
                    pluginData.data["currentScene"] = CurrentChapter.CurrentIndex;
                    pluginData.data["currentChapter"] = ChapterManager.CurrentIndex;
                    pluginData.data["sceneNames"] = CurrentChapter.ExportItemNames();
                    pluginData.data["currentChapter"] = ChapterManager.CurrentIndex;
                    pluginData.data["chapterNames"] = ChapterManager.ExportItemNames();
                    pluginData.data["trackMap"] = track_map;
                    pluginData.data["roles"] = Utils.SerializeData(roleTracker.ExportRoles());
                    var saveDataSizeKb = CalculateSaveDataSize(serializedChapters);
                    Logger.LogMessage($"Saved {saveDataSizeKb:N} Kb of scene state data.");
                    saveDataSize = saveDataSizeKb;
                    return pluginData;
                }
                catch (Exception e)
                {
                    Logger.LogError("Error occurred while saving scene data: " + e);
                    Logger.LogMessage("Failed to save scene data, check debug log for more info.");
                    return null;
                }

            saveDataSize = 0;
            return null;
        }

        internal void LoadPluginData(PluginData pluginData)
        {
            if (pluginData?.data == null)
            {
                roleTracker = new RoleTracker();
                ChapterManager = new Manager<Chapter>();
                ChapterManager.Add(new Chapter());
                saveDataSize = 0;
            }
            else
            {

                pluginData.data.TryGetValue("currentScene", out var temp);
                var sceneIndex = temp as int? ?? 0;

                pluginData.data.TryGetValue("currentChapter", out temp);
                var chapterIndex = temp as int? ?? 0;

                pluginData.data.TryGetValue("sceneNames", out temp);
                var sceneNames = temp as string;
                var sceneStrings = Manager<Scene>.DeserializeItemNames(sceneNames);

                pluginData.data.TryGetValue("sceneNames", out temp);
                var chapterNames = temp as string;

                pluginData.data.TryGetValue("trackMap", out temp);
                track_map = temp as bool? ?? true;


                if (pluginData.data.ContainsKey("scenes") && pluginData.data["scenes"] is byte[] sceneData &&
                    sceneData.Length > 0 && !pluginData.data.ContainsKey("chapters"))
                {
                    var sceneArray = Utils.DeserializeData<Scene[]>(sceneData);
                    ChapterManager = new Manager<Chapter>();
                    ChapterManager.Add(new Chapter(sceneArray.ToList(), sceneStrings));
                }

                if (pluginData.data.ContainsKey("chapters") && pluginData.data["chapters"] is byte[] chapterData && chapterData.Length > 0)
                    try
                    {
                        var chapterDataArray = Utils.DeserializeData<Scene[][]>(chapterData);

                        var chapters = new List<Chapter>(chapterDataArray.Length);
 
                        foreach (var t in chapterDataArray)
                        {
                            chapters.Add(new Chapter(t.ToList(), sceneStrings));
                        }

                        var chapterStrings = Manager<Chapter>.DeserializeItemNames(chapterNames);
                        
                        ChapterManager = new Manager<Chapter>(chapters, currentIndex: chapterIndex, itemNames: chapterStrings);

                        var saveDataSizeKb = CalculateSaveDataSize(chapterData);
                        Logger.LogMessage($"Loaded {saveDataSizeKb:N} Kb of scene state data.");
                        saveDataSize = saveDataSizeKb;
                    }
                    catch (Exception e)
                    {
                        Logger.LogError("Error occurred while loading scene data: " + e);
                        Logger.LogMessage("Failed to load scene data, check debug log for more info.");
                    }

                if (pluginData.data.ContainsKey("roles") && pluginData.data["roles"] is byte[] roleData && roleData.Length > 0)
                    try
                    {
                        var roles = Utils.DeserializeData<Dictionary<int, Dictionary<string, int>>>(roleData);
                        roleTracker = new RoleTracker(roles);
                    }
                    catch (Exception e)
                    {
                        Logger.LogError("Error occurred while loading role data: " + e);
                        Logger.LogMessage("Failed to load role data, check debug log for more info.");
                    }
                else
                {
                    roleTracker = new RoleTracker();
                }

                // For scenes that still use SceneFolders
                SceneFolders.LoadTrackedActorsAndProps();
                if (SceneFolders.AllActors.Any() || SceneFolders.AllProps.Any())
                    roleTracker.AddFrom(SceneFolders.AllActors, SceneFolders.AllProps);

                ChapterManager.SetCurrent(chapterIndex);
                CurrentChapter.SetCurrent(sceneIndex);

                LoadCurrentScene();
            }
        }


        // Blocking message functions
        internal void show_blocking_message(string text = "...")
        {
            funcLockedText = text;
            isFuncLocked = true;
        }


        internal void MoveChapterForward()
        {
            ChapterManager.MoveItemForward();
        }

        internal void MoveChapterBackward()
        {
            ChapterManager.MoveItemBack();
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

        internal void getSceneCamString()
        {
        }

        internal void DeleteSceneCam()
        {
            var curCam = CamManager.Remove();
            if (curCam > -1) setCamera();
        }

        internal void changeSceneCam(CamTask task)
        {
            var cdata = CameraController.cameraData;
            var addata = currentVNData;
            var camData = new CamData(cdata.pos, cdata.rotate, cdata.distance, cdata.parse, addata);
            switch (task)
            {
                case CamTask.ADD:
                    CamManager.Add(camData);
                    break;
                case CamTask.UPDATE:
                    CamManager.Update(camData);
                    break;
            }

            if (task != CamTask.UPDATE) getSceneCamString();
        }

        internal void setCamera()
        {
            setCamera(paramAnimCamIfPossible.Value);
        }

        internal void SetVNData(VNData vnData)
        {
            currentVNData.enabled = vnData.enabled;
            currentVNData.whosay = vnData.whosay is null ? "" : vnData.whosay;
            currentVNData.whatsay = vnData.whatsay is null ? "" : vnData.whatsay;
            if (vnData.addvncmds != null)
                currentVNData.addvncmds = vnData.addvncmds;
            else
                currentVNData.addvncmds = "";

            currentVNData.addprops = vnData.addprops;

            GameController.SetText(currentVNData.whosay, currentVNData.whatsay);
        }

        internal void ResetVNData()
        {
            currentVNData.enabled = false;
            currentVNData.whosay = "";
            currentVNData.whatsay = "";
            currentVNData.addvncmds = "";
            currentVNData.addprops.a1 = false;
            currentVNData.addprops.a2 = false;
        }

        internal void setCamera(bool isAnimated)
        {
            var camera_data = CamManager.Current;
            // check and run adv command
            var keepCamera = false;
            if (camera_data.addata.enabled)
            {
                //keepCamera = VNExt.runAdvVNSS(this, camera_data.addata); TODO
            }

            // actual set
            if (keepCamera)
            {
            }
            else if (isAnimated)
            {
                // self.game.anim_to_camera(1.5, pos=camera_data[0], distance=camera_data[1], rotate=camera_data[2], fov=camera_data[3], style={'style': "fast-slow",'target_camera_zooming_in': 2})
                /*var style = new Dictionary<string, object> {
                    {
                        "style",
                        "fast-slow"}};
                if (this.paramAnimCamZoomOut != 0.0)
                {
                    style["target_camera_zooming_in"] = this.paramAnimCamZoomOut;
                } */ //TODO fix this
                var style = "linear";
                CameraController.anim_to_camera(paramAnimCamDuration, camera_data.position, camera_data.distance,
                    camera_data.rotation, camera_data.fov, style);
            }
            else
            {
                CameraController.move_camera(camera_data);
                //this.game.move_camera(pos: camera_data.position, distance: camera_data.distance, rotate: camera_data.rotation, fov: camera_data.fov);
            }

            if (camera_data.addata is VNData addata)
            {
                SetVNData(addata);
            }
            else
            {
                ResetVNData();
            }
        }

        internal void addAutoWithMsg()
        {
            AddScene();
            show_blocking_message_time_sc("Scene added!", 2.0f);
        }

        internal void UpdateScene()
        {
            if (!CurrentChapter.HasItems) return;

            var scene = new Scene(game.export_full_status(), roleTracker.AllCharacters, roleTracker.AllProps,
                isSysTracking);

            CurrentChapter.Update(scene);
        }


        internal void InsertScene()
        {
            AddScene(insert:true);
        }

        internal void AddScene(bool insert = false)
        {
            var scene = new Scene(game.export_full_status(), roleTracker.AllCharacters, roleTracker.AllProps,
                isSysTracking);
            if (insert)
                CurrentChapter.Insert(scene);
            else
                CurrentChapter.Add(scene);

            CamManager = new Manager<CamData>(scene.cams);

            if (autoAddCam.Value)
                changeSceneCam(CamTask.ADD);
        }

        // Remove stuff

        internal void RemoveScene()
        {
            if (CurrentChapter.HasItems) CurrentChapter.Remove();
        }

        // Load scene

        internal void SetCurrentScene(int i)
        {
            CurrentChapter.SetCurrent(i);
            LoadCurrentScene();
        }

        internal void SetCurrentChapter(int chapterNumber, int sceneNumber)
        {
            Instance.ChapterManager.SetCurrent(chapterNumber);
            SetCurrentScene(sceneNumber);
        }

        internal void SetCurrentChapter(int chapterNumber)
        {
            Instance.ChapterManager.SetCurrent(chapterNumber);
            CurrentChapter.First();
            if (CurrentChapter.HasItems)
            {
                LoadCurrentScene();
            }
        }

        internal bool LoadCurrentScene()
        {
            if (ChapterManager.Count <= 0 || CurrentChapter.Count <= 0) return false;
            SetSceneState(CurrentScene);
            CamManager = new Manager<CamData>(CurrentScene.cams);
            if (CamManager.Count <= 0) return false;
            setCamera();
            return true;
        }

        internal void copySelectedStatusToTracking(List<string> exclude)
        {
            var elem = NeoOCI.create_from_selected();
            if (elem is Character chara)
            {
                var tmp_status = chara.export_full_status();
                var actors = roleTracker.AllCharacters;
                foreach (var key in actors.Keys)
                {
                    var character = actors[key];
                    if (character.text_name == chara.text_name)
                    {
                        /* TODO
                        foreach (var keyEx in exclude)
                        {
                            tmp_status.Remove(keyEx);
                        }
                        */
                        character.import_status(tmp_status);
                        return;
                    }
                }

                show_blocking_message_time_sc("Can't find tracking char with same name");
            }
            else
            {
                show_blocking_message_time_sc("Can't copy status");
            }
        }

        internal void copySelectedStatus()
        {
            var elem = NeoOCI.create_from_selected();
            if (elem is Character chara)
                clipboard_status = (IDataClass<IVNObject<object>>) chara.export_full_status();
            else if (elem is Prop prop)
                clipboard_status = (IDataClass<IVNObject<object>>) prop.export_full_status();
            else
                show_blocking_message_time_sc("Can't copy status");
        }

        internal void pasteSelectedStatus()
        {
            var elem = NeoOCI.create_from_selected();
            if (elem is Character chara)
            {
                chara.import_status((ActorData) clipboard_status);
            }
            else if (elem is Prop prop)
            {
                if (elem is Item i)
                    i.import_status((ItemData) clipboard_status2);
                else if (elem is Light l)
                    prop.import_status((LightData) clipboard_status2);
                else
                    prop.import_status((NEOPropData) clipboard_status2);
            }
            else
            {
                show_blocking_message_time_sc("Can't paste status");
            }
        }

        internal void copySelectedStatus2()
        {
            var elem = NeoOCI.create_from_selected();
            if (elem is Character chara)
                clipboard_status2 = (IDataClass<IVNObject<object>>) chara.export_full_status();
            else if (elem is Prop prop)
                clipboard_status2 = (IDataClass<IVNObject<object>>) prop.export_full_status();
            else
                show_blocking_message_time_sc("Can't copy status 2");
        }

        internal void pasteSelectedStatus2()
        {
            var elem = NeoOCI.create_from_selected();
            if (elem is Character chara)
            {
                chara.import_status((ActorData) clipboard_status2);
            }
            else if (elem is Prop prop)
            {
                if (elem is Item i)
                    i.import_status((ItemData) clipboard_status2);
                else if (elem is Light l)
                    prop.import_status((LightData) clipboard_status2);
                else
                    prop.import_status((NEOPropData) clipboard_status2);
            }
            else
            {
                show_blocking_message_time_sc("Can't paste status 2");
            }
        }

        internal void addSysTracking()
        {
            if (ChapterManager.Count > 0)
            {
                var curstatus = game.export_full_status();
                foreach (var scene in CurrentChapter)
                {
                    //scene.actors["sys"] = curstatus;
                    scene.sys = curstatus;
                }

                isSysTracking = true;
            }
            else
            {
                show_blocking_message_time_sc("Please, add at least 1 state to add system environment tracking");
            }
        }

        internal void delSysTracking()
        {
            isSysTracking = false;
        }

        internal void AddSelectedToRole()
        {
            var objects = StudioAPI.GetSelectedObjects();

            if (!objects.Any())
            {
                show_blocking_message_time_sc("Nothing selected");
                return;
            }

            foreach (var objectCtrl in objects)
                try
                {
                    roleTracker.AddToRole(objectCtrl);
                }
                catch
                {
                }
        }

        internal void ChangeSelectedRoleName(string newRoleName)
        {
            if (SelectedRole == "")
            {
                show_blocking_message("No role selected.");
            }
            else
            {
                if (ChangeRoleName(SelectedRole, newRoleName)) SelectedRole = newRoleName;
            }
        }

        internal void ClearRoleOfSelected()
        {
            var objects = StudioAPI.GetSelectedObjects();

            foreach (var oci in objects) roleTracker.RemoveFromRole(oci);
        }

        internal void AddSelectedToRole(string roleName)
        {
            var objects = StudioAPI.GetSelectedObjects();
            foreach (var oci in objects) roleTracker.AddToRole(oci, roleName);
        }

        internal void RemoveRoleOfSelected()
        {
            var objects = StudioAPI.GetSelectedObjects();

            if (!objects.Any())
            {
                show_blocking_message_time_sc("Nothing selected");
                return;
            }

            foreach (var oci in objects)
            {
                var roleName = roleTracker.GetRoleName(oci);
                RemoveRole(roleName);
            }
        }

        internal void RemoveRole(string roleName)
        {
            if (roleName == "") return;

            roleTracker.RemoveRole(roleName);

            foreach (var scene in CurrentChapter) scene.Remove(roleName);
        }

        internal bool ChangeRoleName(string roleName, string newRoleName)
        {
            if (roleName == "") return false;

            if (!roleTracker.ChangeRoleName(roleName, newRoleName)) return false;

            foreach (var scene in CurrentChapter) scene.ChangeRoleName(roleName, newRoleName);

            return true;
        }

        internal void Reset()
        {
            ChapterManager = new Manager<Chapter>();
            ChapterManager.Add(new Chapter());
        }

        internal void SaveToFile()
        {
            if (svname == "") SaveToFile(defaultSaveName);
        }

        internal void SaveToFile(string filename)
        {
            var app_dir = Path.GetDirectoryName(Application.dataPath);

            if (!Directory.Exists(backup_folder_name)) Directory.CreateDirectory(backup_folder_name);

            var file_path = Path.Combine(backup_folder_name, filename);
            var abs_file_path = Path.Combine(app_dir, file_path);
            var data = Utils.SerializeData(ChapterManager.ExportItems());
            File.WriteAllBytes(abs_file_path, data);
        }

        internal void LoadFromFile(string filename)
        {
            var script_dir = Path.GetDirectoryName(Application.dataPath);
            var file_path = Path.Combine(backup_folder_name, filename);
            var abs_file_path = Path.Combine(script_dir, file_path);
            if (File.Exists(abs_file_path))
            {
                var data = File.ReadAllBytes(abs_file_path);
                var chapters = Utils.DeserializeData<Chapter[]>(data);
                ChapterManager = new Manager<Chapter>(chapters.ToList());
            }
        }

        internal void loadSceneDataBackupTimer(object param)
        {
            loadSceneDataBackupTimer();
        }

        internal void loadSceneDataBackupTimer()
        {
            LoadFromFile("_backuptimer");
        }

        internal void loadSceneData()
        {
            loadSceneData(false);
        }

        internal void loadSceneData(bool backup = false, bool setToFirst = true)
        {
            string filename;
            SceneFolders.LoadTrackedActorsAndProps();

            if (backup)
            {
                if (svname == "")
                    filename = defaultBackupName;
                else
                    filename = svname + ".backup";
            }
            else
            {
                if (svname == "")
                    filename = defaultSaveName;
                else
                    filename = svname;
            }

            // abs_file_path = os.Path.Combine(script_dir, file_path)
            // if os.File.Exists(abs_file_path):
            //     f = open(abs_file_path, "r")
            //     block_dict = Utils.DeserializeData(f.read(), object_hook=sceneDecoder)  # , indent = 4, separators = (","," : ")))
            //     f.close()
            LoadFromFile(filename);

            // loading
            if (setToFirst)
                if (CurrentChapter.HasItems)
                {
                    ChapterManager.First();
                    CamManager.First();
                }
        }

        // Change name
        internal static void changeCharName(StudioController game, string name)
        {
            var chara = game.SelectedChar;
            var old_name = chara.text_name;
            chara.objctrl.treeNodeObject.textName = name;
            // for sex in range(len(self.basechars)):
            //     if old_name in self.nameset[sex]:
            //         self.changeSceneChars((1 - sex), tag="upd")
            //         break
            // Duplicate scene
        }

        internal void DuplicateScene()
        {
            if (ChapterManager.Count > 0)
                CurrentChapter.Insert(CurrentChapter.Current.Copy());
        }

        // Copy/paste cam set
        internal void copyCamSet()
        {
            if (!CurrentChapter.HasItems) return;
            if (camset is null) camset = new List<CamData>();
            camset = CurrentChapter.Current.cams;
        }

        internal void pasteCamSet()
        {
            if (CurrentChapter.HasItems) CurrentChapter.Current.cams.AddRange(camset);
        }


        // Goto next/prev
        internal void goto_first()
        {
            ChapterManager.First();
            LoadCurrentScene();
        }

        internal void NextSceneOrCamera(VNController vn, int i)
        {
            NextSceneOrCamera();
        }

        internal void NextSceneOrCamera()
        {

            if (ChapterManager.Count == 1 && CurrentChapter.Count == 0)
            {
                return;
            }

            if (ChapterManager.Count <= 0) return;
            if (CamManager.Count > 0 && CamManager.HasNext)
            {
                CamManager.Next();
                setCamera();
            }
            else
            {
                LoadNextScene();
            }
        }

        internal void goto_prev()
        {
            if (!CurrentChapter.HasPrev) return;
            if (CamManager.CurrentIndex > 0)
            {
                CamManager.Back();
                setCamera();
            }
            else
            {
                // elif self.cur_index > 0:
                // self.cur_index -= 1
                LoadPreviousScene(true);
            }
        }

        internal void LoadNextScene()
        {
            if (!CurrentChapter.HasNext)
            {
                GoToNextChapter();
            }
            else
            {
                CurrentChapter.Next();
            }
            LoadCurrentScene();
        }

        internal void LoadPreviousScene()
        {
            LoadPreviousScene(false);
        }

        internal void GoToPreviousChapter()
        {
            if (!ChapterManager.HasPrev) return;
            ChapterManager.Back();
            CurrentChapter.Last();
        }

        internal void GoToNextChapter()
        {
            if (!ChapterManager.HasNext) return;
            ChapterManager.Next();
            CurrentChapter.First();
        }

        internal void MoveSceneForward()
        {
            if (!CurrentChapter.HasNext && ChapterManager.HasNext)
            {
                var scene = CurrentScene;
                CurrentChapter.Remove();
                ChapterManager.Next();
                CurrentChapter.Prepend(scene);
            }
            else
            {
                CurrentChapter.MoveItemForward();
            }
            
        }

        internal void MoveSceneBackward()
        {

            if (!CurrentChapter.HasPrev && ChapterManager.HasPrev)
            {
                var scene = CurrentScene;
                CurrentChapter.Remove();
                ChapterManager.Back();
                CurrentChapter.Add(scene);
            }
            else
            {
                CurrentChapter.MoveItemBack();
            }
        }
        


        internal void LoadPreviousScene(bool lastcam = false)
        {

            if (!CurrentChapter.HasPrev)
            {
                GoToPreviousChapter();
            }
            else
            {
                CurrentChapter.Back();
            }

            LoadCurrentScene();
            if (!lastcam || CamManager.Count <= 0) return;
            CamManager.Last();
            setCamera();

        }

        internal void camSetAll(bool state)
        {
            foreach (var i in Enumerable.Range(0, ChapterManager.Count))
            {
                var scene = CurrentChapter[i];
                // only process scene if 1 cam is VN cam - other, skip
                // cam = scene.cams[0]
                foreach (var j in Enumerable.Range(0, scene.cams.Count))
                {
                    var cam = scene.cams[j];
                    cam.addata.enabled = state;
                }
            }

            show_blocking_message_time_sc("Cams changed!");
        }

        internal void runVNSS(string starfrom = "begin")
        {
            if (ChapterManager.Count == 0) return;
            GameController.ShowVNTextBox(new List<Button_s> { new Button_s(">>", NextSceneOrCamera, 1) });
            int calcPos;
            if (starfrom == "cam")
                //print self.cur_index, self.cur_cam
                calcPos = (ChapterManager.CurrentIndex + 1) * 100 + CamManager.CurrentIndex;
            else if (starfrom == "scene")
                calcPos = (ChapterManager.CurrentIndex + CurrentChapter.CurrentIndex + 1) * 100;
            else
                calcPos = 0;
            ChapterManager.SetCurrent(calcPos);
            LoadCurrentScene();
            Console.WriteLine("Run VNSS from state {0}", calcPos.ToString());
            //game.vnscenescript_run_current(onEndVNSS, calcPos.ToString());
        }

        //def _exportAddBlock(self,fld_acode,):
        internal string get_next_speaker(string curSpeakAlias, bool next)
        {
            // next from unknown speaker
            var all_actors = roleTracker.CharacterRoles;
            var keylist = all_actors.Keys.ToList();
            if (curSpeakAlias != defaultSpeakerAlias && !all_actors.ContainsKey(curSpeakAlias))
                return defaultSpeakerAlias;
            // next from s or actor
            if (curSpeakAlias == defaultSpeakerAlias)
            {
                if (all_actors.Count > 0)
                {
                    if (next)
                        return keylist[0];
                    return keylist.Last();
                }

                return defaultSpeakerAlias;
            }

            var nextIndex = keylist.IndexOf(curSpeakAlias);
            if (next)
                nextIndex += 1;
            else
                nextIndex -= 1;
            return Enumerable.Range(0, all_actors.Count).Contains(nextIndex) ? keylist[nextIndex] : defaultSpeakerAlias;
        }

        // Set scene chars with state data from dictionary

        internal void SetSceneState(Scene s)
        {
            if (isSysTracking) game.Apply(s.sys, track_map);

            //var watch = new Stopwatch();
            //watch.Start();
            s.SetCharacterState(roleTracker.AllCharacters);
            //watch.Stop();
            //Logger.LogInfo($"Loaded character data in {watch.ElapsedMilliseconds} ms.");

            s.SetPropState(roleTracker.AllProps);
        }

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

        public void RemoveSelectedRole()
        {
            roleTracker.RemoveRole(SelectedRole);
        }

        internal enum CamTask
        {
            UPDATE,
            ADD
        }

        public void AddChapter()
        {
            ChapterManager.Add(new Chapter());
        }

        public void InsertChapter()
        {
            ChapterManager.Insert(new Chapter());
        }

        public void SplitChapter()
        {

            if (!CurrentChapter.HasItems) return;

            var currentIndex = CurrentChapter.CurrentIndex;

            var scenes = CurrentChapter.RemoveUntilEnd(currentIndex+1);
            ChapterManager.Insert(new Chapter(scenes, null));
            LoadCurrentScene();

        }

        public void MergeChapters()
        {

            if (!ChapterManager.HasNext) return;

            var currentIndex = CurrentChapter.CurrentIndex;

            var nextChapter = ChapterManager[currentIndex + 1]; 

            CurrentChapter.AddRange(nextChapter.ExportItems().ToList());

            ChapterManager.Remove(currentIndex + 1);


        }

        public void DuplicateChapter()
        {
            ChapterManager.Duplicate();
        }

        public void RemoveChapter()
        {
            ChapterManager.Remove();
        }
    }
}