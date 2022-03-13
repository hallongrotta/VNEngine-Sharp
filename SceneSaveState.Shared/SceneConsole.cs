using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using ExtensibleSaveFormat;
using KKAPI;
using KKAPI.Studio;
using KKAPI.Studio.SaveLoad;
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

        internal ConfigEntry<bool> autoLoad;

        internal string autoshownewid;

        internal SceneManager block;

        internal List<CamData> camset;

        internal string charname;

        internal IDataClass<IVNObject<object>> clipboard_status;

        internal IDataClass<IVNObject<object>> clipboard_status2;
        internal float consoleHeight;

        internal float consoleWidth;

        internal VNData currentVNData;

        //internal int cur_cam;

        internal string funcLockedText;

        //internal VNNeoController game;

        internal bool guiOnShow;

        internal bool isFuncLocked;

        internal bool isSysTracking = true;

        internal bool isUseMsAuto;

        internal int last_acc_id;

        internal string mininewid;

        internal string newid;

        internal string nor_font_col;

        internal float paramAnimCamDuration;

        internal ConfigEntry<bool> paramAnimCamIfPossible;

        internal string paramAnimCamStyle;

        internal float paramAnimCamZoomOut;

        internal ConfigEntry<bool> promptOnDelete;

        internal string sel_font_col;

        //internal Dictionary<string, KeyValuePair<string, string>> shortcuts;

        internal SkinDefault skinDefault;

        internal string skinDefault_sideApp;

        internal ConfigEntry<bool> skipClothesChanges;

        internal string svname = "";
        internal bool track_map = true;

        internal int updAutoStatesTimer;
        private GUI.WindowFunction windowCallback;

        private Rect windowRect;

        internal SceneConsole()
        {
            // init dict
            // initWordDict()
            // --- Some constants ---
            guiOnShow = false;
            // --- Essential Data ---
            last_acc_id = 0;
            block = new SceneManager();
            // self.basechars = self.getAllBaseChars()
            // self.dupchars = self.getAllDupChars()
            // self.updateNameset()
            // :::: UI Data ::::

            // -- Main --
            sel_font_col = "#f24115";
            nor_font_col = "#f9f9f9";

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
            skinDefault = new SkinDefault
            {
                controller = game
            };
            skinDefault_sideApp = "";
            Instance = this;
        }

        internal static ConfigEntry<KeyboardShortcut> SSSHotkey { get; private set; }

        internal double saveDataSize { get; private set; }

        internal VNNeoController game => VNNeoController.Instance;

        internal static SceneConsole Instance { get; private set; }

        internal ManualLogSource GetLogger => Logger;

        internal void Awake()
        {
            StudioSaveLoadApi.RegisterExtraBehaviour<SaveLoadController>(GUID);
        }

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
            autoLoad = Config.Bind("Scene Console Settings", "AutoLoadScene", true, "Load scenes when selected.");
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

        internal PluginData GetPluginData()
        {
            var pluginData = new PluginData();
            if (block.Count > 0)
                try
                {
                    var sceneData = Utils.SerializeData(block.ExportScenes());
                    pluginData.data["scenes"] = sceneData;
                    pluginData.data["currentScene"] = block.currentSceneIndex;
                    pluginData.data["sceneNames"] = block.ExportSceneStrings();
                    pluginData.data["trackMap"] = track_map;
                    var saveDataSizeKb = CalculateSaveDataSize(sceneData);
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
            if (pluginData == null || pluginData?.data == null)
            {
                block = new SceneManager();
                saveDataSize = 0;
            }
            else
            {
                var sceneData = pluginData.data["scenes"] as byte[];
                if (sceneData != null && sceneData.Length > 0)
                    try
                    {
                        var scenes = Utils.DeserializeData<Scene[]>(sceneData);

                        object temp;

                        pluginData.data.TryGetValue("currentScene", out temp);
                        var sceneIndex = temp as int? ?? 0;

                        pluginData.data.TryGetValue("sceneNames", out temp);
                        var sceneNames = temp as string;

                        pluginData.data.TryGetValue("trackMap", out temp);
                        track_map = temp as bool? ?? true;

                        var sceneStrings = SceneManager.DeserializeSceneStrings(sceneNames);

                        block = new SceneManager(scenes, currentSceneIndex: sceneIndex, sceneStrings: sceneStrings);

                        var saveDataSizeKb = CalculateSaveDataSize(sceneData);
                        Logger.LogMessage($"Loaded {saveDataSizeKb:N} Kb of scene state data.");
                        saveDataSize = saveDataSizeKb;
                    }
                    catch (Exception e)
                    {
                        Logger.LogError("Error occurred while loading scene data: " + e);
                        Logger.LogMessage("Failed to load scene data, check debug log for more info.");
                    }
            }

            SceneFolders.LoadTrackedActorsAndProps();
            loadCurrentScene();
        }


        // Blocking message functions
        internal void show_blocking_message(string text = "...")
        {
            funcLockedText = text;
            isFuncLocked = true;
        }

        internal void hide_blocking_message(object game = null)
        {
            isFuncLocked = false;
        }

        internal void show_blocking_message_time_sc(string text = "...", float duration = 3f)
        {
            show_blocking_message(text);
            game.set_timer(duration, hide_blocking_message);
        }

        internal void getSceneCamString()
        {
        }

        internal void deleteSceneCam()
        {
            changeSceneCam(CamTask.DELETE);
        }

        internal void changeSceneCam()
        {
            changeSceneCam(CamTask.ADD);
        }

        internal void changeSceneCam(CamTask task)
        {
            var cdata = VNNeoController.cameraData;
            var addata = currentVNData;
            var cam_data = new CamData(cdata.pos, cdata.rotate, cdata.distance, cdata.parse, addata);
            if (task == CamTask.ADD)
            {
                block.AddCam(cam_data);
            }
            else if (task == CamTask.UPDATE)
            {
                block.UpdateCam(cam_data);
            }
            else if (task == CamTask.DELETE)
            {
                var cur_cam = block.DeleteCam();
                if (cur_cam > -1) setCamera();
            }

            if (!(task == CamTask.UPDATE)) getSceneCamString();
        }

        internal void setCamera()
        {
            setCamera(paramAnimCamIfPossible.Value);
        }

        internal void setCamera(bool isAnimated)
        {
            var camera_data = block.CurrentCam;
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
                game.anim_to_camera(paramAnimCamDuration, camera_data.position, camera_data.distance,
                    camera_data.rotation, camera_data.fov, style);
            }
            else
            {
                game.move_camera(camera_data);
                //this.game.move_camera(pos: camera_data.position, distance: camera_data.distance, rotate: camera_data.rotation, fov: camera_data.fov);
            }

            if (camera_data.addata is VNData addata)
            {
                currentVNData.enabled = addata.enabled;
                currentVNData.whosay = addata.whosay is null ? "" : addata.whosay;
                currentVNData.whatsay = addata.whatsay is null ? "" : addata.whatsay;
                if (addata.addvncmds != null)
                    currentVNData.addvncmds = addata.addvncmds;
                else
                    currentVNData.addvncmds = "";

                currentVNData.addprops = addata.addprops;

                game.set_text(currentVNData.whosay, currentVNData.whatsay);
            }
            else
            {
                currentVNData.enabled = false;
                currentVNData.whosay = "";
                currentVNData.whatsay = "";
                currentVNData.addvncmds = "";
                currentVNData.addprops.a1 = false;
                currentVNData.addprops.a2 = false;
            }
        }

        internal void addAutoWithMsg()
        {
            addAuto();
            show_blocking_message_time_sc("Scene added!", 2.0f);
        }

        internal void UpdateScene()
        {
            if (block.HasScenes)
            {
                var scene = new Scene(game, isSysTracking);
                block.Update(scene);
            }
        }

        internal void addAuto(bool insert = false, bool addsc = true, bool allbase = true)
        {
            var scene = new Scene(game, isSysTracking);
            if (insert)
                block.Insert(scene);
            else
                block.Add(scene);
            if (addsc)
                if (autoAddCam.Value)
                    changeSceneCam(CamTask.ADD);
        }

        // Remove stuff
        internal void removeScene(object param)
        {
            removeScene();
        }

        internal void removeScene()
        {
            if (block.HasScenes) block.RemoveScene();
        }

        // Load scene

        internal void loadCurrentScene()
        {
            SetSceneState(block.CurrentScene);
            if (block.Count > 0 && block.currentCamCount > 0)
            {
                block.FirstCam();
                setCamera();
            }
        }

        internal void copySelectedStatusToTracking(List<string> exclude)
        {
            var elem = NeoOCI.create_from_selected();
            if (elem is Character chara)
            {
                var tmp_status = chara.export_full_status();
                var actors = game.AllActors;
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
            if (block.Count > 0)
            {
                var curstatus = VNEngine.System.export_full_status();
                foreach (var i in Enumerable.Range(0, block.Count))
                {
                    var scene = block[i];
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

        internal void addSelectedToTrack()
        {
            var objects = StudioAPI.GetSelectedObjects();

            if (objects.Count() == 0)
            {
                show_blocking_message_time_sc("Nothing selected");
                return;
            }

            foreach (var objectCtrl in objects)
                try
                {
                    SceneFolders.AddToTrack(objectCtrl);
                }
                catch
                {
                }

            SceneFolders.LoadTrackedActorsAndProps();
        }

        internal void changeSelTrackID(string toId)
        {
            if (toId == "")
            {
                show_blocking_message_time_sc("Please, set ID to change to first");
                return;
            }

            var elem = NeoOCI.create_from_selected();
            if (elem == null)
            {
                show_blocking_message_time_sc("Nothing selected");
                return;
            }

            if (elem is Character chara)
            {
                var actors = game.AllActors;
                var id = "";
                foreach (var actid in actors.Keys)
                    if (actors[actid].objctrl == elem.objctrl)
                    {
                        // found
                        id = actid;
                        break;
                    }

                //self.delActorFromTrack(actid)
                if (id == "")
                {
                    show_blocking_message_time_sc("Can't find actor to change ID");
                    return;
                }

                // actually changing ID
                changeActorTrackId(id, toId);
            }

            // updating set
            SceneFolders.LoadTrackedActorsAndProps();
        }

        internal void delSelectedFromTrack(object o)
        {
            delSelectedFromTrack();
        }


        internal void delSelectedFromTrack()
        {
            var elem = NeoOCI.create_from_selected();
            if (elem == null)
            {
                show_blocking_message_time_sc("Nothing selected");
                return;
            }

            if (elem is Character chara)
            {
                var actors = game.AllActors;
                var id = "";
                foreach (var actid in actors.Keys)
                    if (actors[actid].objctrl == elem.objctrl)
                    {
                        // found
                        id = actid;
                        break;
                    }

                if (id == "")
                {
                    show_blocking_message_time_sc("Can't delete; seems this actor is not tracking yet");
                    return;
                }

                delActorFromTrack(id);
            }
            else if (elem is Prop)
            {
                var props = game.AllProps;
                var id = "";
                foreach (var propid in props.Keys)
                    if (props[propid].objctrl == elem.objctrl)
                    {
                        id = propid; // found
                        break;
                    }

                delPropFromTrack(id);
            }

            // updating set
            SceneFolders.LoadTrackedActorsAndProps();
        }

        internal void delActorFromTrack(string actid)
        {
            if (actid != "")
            {
                // we found this char
                var fld = Folder.find_single(SceneFolders.actor_folder_prefix + actid);
                if (fld == null) fld = Folder.find_single_startswith(SceneFolders.actor_folder_prefix + actid + ":");
                // found
                if (fld != null) fld.delete();
                foreach (var i in Enumerable.Range(0, block.Count))
                {
                    var scene = block[i];
                    scene.actors.Remove(actid);
                }
            }
        }

        internal void changeActorTrackId(string actid, string toid)
        {
            if (actid != "")
            {
                // we found this char
                var fld = Folder.find_single(SceneFolders.actor_folder_prefix + actid);
                if (fld == null) fld = Folder.find_single_startswith(SceneFolders.actor_folder_prefix + actid + ":");
                // found
                //if fld != None:
                //    fld.delete()
                var fldoldname = fld.name;
                var lastelems = fldoldname.Substring((SceneFolders.actor_folder_prefix + actid).Length);
                //print lastelems
                fld.name = SceneFolders.actor_folder_prefix + toid + lastelems;
                //
                for (var i = 0; i < block.Count; i++)
                {
                    var scene = block[i];
                    scene.actors[toid] = scene.actors[actid];
                    scene.actors.Remove(actid);
                    foreach (var cam in scene.cams)
                    {
                        var info = cam.addata;
                        if (info.whosay == actid) info.whosay = toid;
                    }
                }
            }
        }

        internal void delPropFromTrack(string propid)
        {
            if (propid != "")
            {
                // we found this prop
                var fld = Folder.find_single(SceneFolders.prop_folder_prefix + propid);
                // found
                if (fld != null) fld.delete();
                foreach (var i in Enumerable.Range(0, block.Count))
                {
                    var scene = block[i];
                    scene.RemoveProp(propid);
                }
            }
        }

        internal void saveSceneData(object param)
        {
            saveSceneData((bool) param);
        }

        internal void Reset()
        {
            block = new SceneManager();
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
            var data = Utils.SerializeData(block.ExportScenes());
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
                var scenes = Utils.DeserializeData<Scene[]>(data);
                block = new SceneManager(scenes);
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
                if (block.HasScenes)
                {
                    block.First();
                    block.FirstCam();
                }
        }

        // Change name
        internal static void changeCharName(VNNeoController game, string name)
        {
            var chara = Utils.getSelectedChar(game);
            var old_name = chara.text_name;
            chara.objctrl.treeNodeObject.textName = name;
            // for sex in range(len(self.basechars)):
            //     if old_name in self.nameset[sex]:
            //         self.changeSceneChars((1 - sex), tag="upd")
            //         break
            // Duplicate scene
        }

        internal void dupScene()
        {
            if (block.Count > 0)
                //import copy
                // we have a problem with copy, so... just serialize and back it
                //objstr = MessagePackSerializer.Serialize(self.block[self.cur_index])
                block.Insert(block.CurrentScene.copy());
        }

        // Copy/paste cam set
        internal void copyCamSet()
        {
            if (block.HasScenes)
            {
                if (camset is null) camset = new List<CamData>();
                camset = block.CurrentScene.cams;
            }
        }

        internal void pasteCamSet()
        {
            if (block.HasScenes) block.CurrentScene.cams.AddRange(camset);
        }


        // Goto next/prev
        internal void goto_first()
        {
            block.First();
            loadCurrentScene();
        }

        internal void goto_next(VNController game, int i)
        {
            goto_next();
        }

        internal void goto_next()
        {
            if (block.Count > 0)
            {
                if (block.currentCamCount > 0 && block.HasNextCam)
                {
                    block.NextCam();
                    setCamera();
                }
                else
                {
                    // elif self.cur_index < (len(self.block) - 1):
                    // self.cur_index += 1
                    goto_next_sc();
                }
            }
        }

        internal void goto_prev()
        {
            if (block.HasPrev)
            {
                if (block.currentCamIndex > 0)
                {
                    block.PrevCam();
                    setCamera();
                }
                else
                {
                    // elif self.cur_index > 0:
                    // self.cur_index -= 1
                    goto_prev_sc(true);
                }
            }
        }

        internal void goto_next_sc()
        {
            if (block.HasNext)
            {
                block.Next();
                loadCurrentScene();
            }
        }

        internal void goto_prev_sc()
        {
            goto_prev_sc(false);
        }

        internal void goto_prev_sc(bool lastcam = false)
        {
            if (block.HasPrev)
            {
                block.Back();
                loadCurrentScene();
                if (lastcam && block.currentCamCount > 0)
                {
                    block.LastCam();
                    setCamera();
                }
            }
        }

        internal void camSetAll(bool state)
        {
            foreach (var i in Enumerable.Range(0, block.Count))
            {
                var scene = block[i];
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
            //this.game.gdata.vnbupskin = this.game.skin;
            //self.game.skin_set_byname("skin_renpy")
            //from skin_renpy import SkinRenPy

            if (block.Count == 0) return;

            var rpySkin = new SkinRenPyMini();
            int calcPos;
            rpySkin.isEndButton = true;
            rpySkin.endButtonTxt = "X";
            rpySkin.endButtonCall = endVNSSbtn;
            game.set_text_s("...");
            game.set_buttons(new List<Button_s> {new Button_s(">>", goto_next, 1)});
            game.skin_set(rpySkin);
            game.visible = true;
            if (starfrom == "cam")
                //print self.cur_index, self.cur_cam
                calcPos = (block.currentSceneIndex + 1) * 100 + block.currentCamIndex;
            else if (starfrom == "scene")
                calcPos = (block.currentSceneIndex + 1) * 100;
            else
                calcPos = 0;
            block.SetCurrent(calcPos);
            loadCurrentScene();
            Console.WriteLine("Run VNSS from state {0}", calcPos.ToString());
            game.vnscenescript_run_current(onEndVNSS, calcPos.ToString());
        }

        internal void endVNSSbtn(VNNeoController game)
        {
            this.game.visible = false;
            //VNSceneScript.run_state(this.game, this.game.scenedata.scMaxState + 1, true); TODO
        }

        internal void onEndVNSS(VNController game = null)
        {
            this.game.skin_set(this.game.skin_default);
        }

        //def _exportAddBlock(self,fld_acode,):
        internal string get_next_speaker(string curSpeakAlias, bool next)
        {
            // next from unknown speaker
            var all_actors = game.AllActors;
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
            if (Enumerable.Range(0, all_actors.Count).Contains(nextIndex))
                return keylist[nextIndex];
            return defaultSpeakerAlias;
        }

        // Set scene chars with state data from dictionary

        internal void SetSceneState(Scene s)
        {
            if (isSysTracking) VNEngine.System.import_status(s.sys, track_map);
            foreach (var actid in s.actors.Keys)
            {
                var actors = game.AllActors;

                foreach (var kvp in actors)
                {
                    s.actors.TryGetValue(kvp.Key, out var char_status);

                    if (char_status is null)
                        kvp.Value.Visible = false;
                    else
                        try
                        {
                            char_status.Apply(kvp.Value);
                        }
                        catch (Exception e)
                        {
                            Instance.game.GetLogger.LogError(
                                $"Error occurred when importing Character with id {actid}" + e);
                            Instance.game.GetLogger.LogMessage(
                                $"Error occurred when importing Character with id {actid}");
                            SceneFolders.LoadTrackedActorsAndProps();
                        }
                }
            }

            var propid = "";
            try
            {
                foreach (var kvp in game.AllProps)
                {
                    propid = kvp.Key;
                    if (kvp.Value is Item i)
                    {
                        s.items.TryGetValue(kvp.Key, out var status);
                        s.ApplyStatus(i, status);
                    }
                    else if (kvp.Value is Light l)
                    {
                        s.lights.TryGetValue(kvp.Key, out var status);
                        s.ApplyStatus(l, status);
                    }
                    else if (kvp.Value is Prop p)
                    {
                        s.props.TryGetValue(kvp.Key, out var status);
                        s.ApplyStatus(p, status);
                    }
                }
            }
            catch (Exception e)
            {
                game.GetLogger.LogError($"Error occurred when importing Prop with id {propid}" + e);
                SceneFolders.LoadTrackedActorsAndProps();
                Instance.game.GetLogger.LogMessage($"Error occurred when importing Prop with id {propid}");
            }
        }

        internal string GetIDOfSelectedObject()
        {
            var objects = StudioAPI.GetSelectedObjects();

            if (objects.Count() == 0)
                return null;
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

        internal enum CamTask
        {
            UPDATE,
            DELETE,
            ADD
        }
    }
}