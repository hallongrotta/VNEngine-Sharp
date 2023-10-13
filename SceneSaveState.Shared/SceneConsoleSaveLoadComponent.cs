using BepInEx.Logging;
using ExtensibleSaveFormat;
using KKAPI.Studio.SaveLoad;
using KKAPI.Utilities;
using Studio;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using VNEngine;
using static SceneSaveState.UI;
using Logger = BepInEx.Logging.Logger;

namespace SceneSaveState
{ 
    public class SceneConsoleSaveLoadComponent : SceneCustomFunctionController
    {

        internal SceneConsole sc;
        internal static ManualLogSource logger;
        internal const string backup_folder_name = "sssdata";
        internal string svname;
        internal Vector2 saveload_scroll = new Vector2(0, 0);
        internal const string defaultSaveName = "SSS.dat";
        internal const string defaultBackupName = "SSS.dat.backup";
        internal double saveDataSize { get; private set; }

        internal ChapterManager chapterManager;
        internal RoleTracker roleTracker;
        private bool trackMap;

        public SceneConsoleSaveLoadComponent()
        {
            this.chapterManager = new ChapterManager(new Chapter());
            this.roleTracker = new RoleTracker();
            this.trackMap = true;
            logger = new ManualLogSource("SceneConsoleSaveLoad"); // The source name is shown in BepInEx log
            BepInEx.Logging.Logger.Sources.Add(logger);
        }

        protected override void OnSceneSave()
        {
            SetExtendedData(GetPluginData(chapterManager, roleTracker, trackMap));
        }

        internal ChapterManager deleteSaveData()
        {
            SetExtendedData(new PluginData() { data = null });
            return new ChapterManager();
        }

        protected override void OnSceneLoad(SceneOperationKind operation, ReadOnlyDictionary<int, ObjectCtrlInfo> loadedItems)
        {
            var pluginData = GetExtendedData();
            chapterManager = LoadChaptersFromPluginData(pluginData);
            roleTracker = LoadRoleTrackerFromPluginData(pluginData);
            //sc.LoadScene(scene);
        }

   
        

        internal double CalculateSaveDataSize(byte[] bytes)
        {
            return (double)bytes.Length / 1000;
        }

        internal PluginData GetPluginData(ChapterManager cm, RoleTracker roleTracker, bool trackMap)
        {
            var currentChapter = cm.Current;
            var pluginData = new PluginData();
            saveDataSize = 0;
            if (!cm.HasItems) return null;
            try
            {

                var chapters = cm.ExportItems();
                var chapterData = cm.ExportItems().Select(c => c.ExportItems()).ToArray();
                var serializedChapters = Utils.SerializeData(chapterData);
                pluginData.data["chapters"] = serializedChapters;
                pluginData.data["currentScene"] = currentChapter.CurrentIndex;
                pluginData.data["currentChapter"] = cm.CurrentIndex;
                pluginData.data["sceneNames"] = currentChapter.ExportItemNames();
                pluginData.data["currentChapter"] = cm.CurrentIndex;
                pluginData.data["chapterNames"] = cm.ExportItemNames();
                pluginData.data["trackMap"] = trackMap;
                pluginData.data["roles"] = Utils.SerializeData(roleTracker.ExportRoles());
                var saveDataSizeKb = CalculateSaveDataSize(serializedChapters);
                logger.LogMessage($"Saved {saveDataSizeKb:N} Kb of scene state data.");
                saveDataSize = saveDataSizeKb;
                return pluginData;
            }
            catch (Exception e)
            {
                logger.LogError("Error occurred while saving scene data: " + e);
                logger.LogMessage("Failed to save scene data, check debug log for more info.");
                return null;
            }
        }

        internal RoleTracker LoadRoleTrackerFromPluginData(PluginData pluginData)
        {
            RoleTracker roleTracker = new RoleTracker();

            if (pluginData.data.ContainsKey("roles") && pluginData.data["roles"] is byte[] roleData && roleData.Length > 0)
                try
                {
                    var roles = Utils.DeserializeData<Dictionary<int, Dictionary<string, int>>>(roleData);
                    roleTracker = new RoleTracker(roles);
                }
                catch (Exception e)
                {
                    logger.LogError("Error occurred while loading role data: " + e);
                    logger.LogMessage("Failed to load role data, check debug log for more info.");
                }
            else
            {
                roleTracker = new RoleTracker();
            }

            // For scenes that still use SceneFolders
            SceneFolders.LoadTrackedActorsAndProps();
            if (SceneFolders.AllActors.Any() || SceneFolders.AllProps.Any())
                roleTracker.AddFrom(SceneFolders.AllActors, SceneFolders.AllProps);

            return roleTracker;
        }

        internal ChapterManager LoadChaptersFromPluginData(PluginData pluginData)
        {

            ChapterManager chapterManager = new ChapterManager();
            bool trackMap;

            if (pluginData?.data == null)
            {
                chapterManager.Add(new Chapter());
                saveDataSize = 0;
                return chapterManager;
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
                trackMap = temp as bool? ?? true;


                if (pluginData.data.ContainsKey("scenes") && pluginData.data["scenes"] is byte[] sceneData &&
                    sceneData.Length > 0 && !pluginData.data.ContainsKey("chapters"))
                {
                    var sceneArray = Utils.DeserializeData<Scene[]>(sceneData);
                    chapterManager = new ChapterManager(new Chapter(sceneArray.ToList(), sceneStrings));
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

                        var chapterStrings = ChapterManager.DeserializeItemNames(chapterNames);

                        chapterManager = new ChapterManager(chapters, currentIndex: chapterIndex, itemNames: chapterStrings);

                        var saveDataSizeKb = CalculateSaveDataSize(chapterData);
                        logger.LogMessage($"Loaded {saveDataSizeKb:N} Kb of scene state data.");
                        saveDataSize = saveDataSizeKb;
                    }
                    catch (Exception e)
                    {
                        logger.LogError("Error occurred while loading scene data: " + e);
                        logger.LogMessage("Failed to load scene data, check debug log for more info.");
                    }

                

                chapterManager.SetCurrent(chapterIndex);
                chapterManager.Current.SetCurrent(sceneIndex);
                return chapterManager;
            }
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
            var data = Utils.SerializeData(chapterManager.ExportItems());
            File.WriteAllBytes(abs_file_path, data);
        }

        internal ChapterManager LoadFromFile(string filename)
        {
            var script_dir = Path.GetDirectoryName(Application.dataPath);
            var file_path = Path.Combine(backup_folder_name, filename);
            var abs_file_path = Path.Combine(script_dir, file_path);
            if (File.Exists(abs_file_path))
            {
                var data = File.ReadAllBytes(abs_file_path);
                var chapters = Utils.DeserializeData<Chapter[]>(data);
                return new ChapterManager(chapters.ToList());
            }
            return null;
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
                if (chapterManager.Current.HasItems)
                {
                    var chapter = chapterManager.First();
                    chapter.Current.First();
                }
        }

        internal Warning? sceneConsoleLdSvUI(SceneConsole sc, bool warnOnLoad)
        {
            object fld;
            // sc.svname = GUILayout.TextField(sc.svname)
            // GUILayout.Space(35)
            var btnBigHeight = 60;
            var btnSmallHeight = 50;
            saveload_scroll = GUILayout.BeginScrollView(saveload_scroll);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label(" ------------------------------------------    Data in card   ------------------------------------------");
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(5);
            GUILayout.BeginHorizontal();
            GUILayout.Label($"Storing <b>{saveDataSize:N} Kb</b> of saved scene data.");
            GUILayout.EndHorizontal();
            GUILayout.Space(5);
            /*
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Reload Scene Data.", GUILayout.Height(btnBigHeight), GUILayout.Width(210)))
            {
                if (Instance.block.Count > 0)
                {
                    //warning_action = SaveLoadController.LoadPluginData;
                    //warning_param = new WarningParam_s("Reload scene data from card? This will overwrite current scenes.", false);
                }
                else
                {
                    //SaveLoadController.LoadPluginData();
                }
            }
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Delete Saved \n Scene Data.", GUILayout.Height(btnBigHeight), GUILayout.Width(210)))
            {
                var msg = "";
                if (Instance.block.Count > 0)
                {
                    msg = "Delete saved scene data and reset current scenes?";
                }
                else
                {
                    msg = "Delete saved scene data?";
                }
                //warning_action = SaveLoadController.deleteSaveData;
                //warning_param = new WarningParam_s(msg, false);
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            */

            Warning? warning = null;
            GUILayout.Space(20);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label(" ----------------------------------------    Data on external file    ----------------------------------------");
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(15);
            GUILayout.BeginHorizontal();
            GUILayout.Space(40);
            GUILayout.Label("File name:");
            GUILayout.Space(20);
            svname = GUILayout.TextField(svname, GUILayout.Width(300));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(20);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("<color=#00ff00>Load</color> from file", GUILayout.Height(btnBigHeight), GUILayout.Width(210)))
            {
                if (warnOnLoad)
                {
                    warning = new Warning("Do you wish to load scenedata from file? (Will overwrite console data)", false, loadSceneData);
                }
                else
                {
                   loadSceneData(backup: false);
                }
            }
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("<color=#ff0000>Save</color> to file", GUILayout.Height(btnBigHeight), GUILayout.Width(210)))
            {
                // delete existing scenedata fld
                var fld_str = "-scfile:" + svname;
                fld = sc.game.getFolder(svname, true);
                if (!(fld == null))
                {
                    warning = new Warning("Scenedata exists. Overwrite?", false, SaveToFile);
                }
                else
                {
                    SaveToFile("SSS.dat");
                }
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(30);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label(" ----------------------------    Load from backup (scene/external file)   ---------------------------");
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(15);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Load backup scene data\n(scene/external file)", GUILayout.Height(btnSmallHeight), GUILayout.Width(210)))
            {
                fld = sc.game.getFolder("-scfile:", false);
                if (warnOnLoad)
                {
                    if (fld == null)
                    {

                        warning = new Warning("Do you wish to load backup scenedata from scene? (Will overwrite console data)", false, loadSceneData);
                    }
                    else
                    {

                        warning = new Warning("Do you wish to load backup scenedata from file? (Will overwrite console data)", false, loadSceneData);
                    }
                }
                else if (fld == null)
                {
                    loadSceneData(backup: true);
                }
                else
                {
                    loadSceneData(backup: true);
                }
            }
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Load auto-timer backup file", GUILayout.Height(btnSmallHeight), GUILayout.Width(210)))
            {
                //sc.exportToVNSS()
                if (warnOnLoad)
                {
                    warning = new Warning("Do you wish to load backup scenedata from file auto-saved by timer? (Will overwrite console data)", false, loadSceneDataBackupTimer);
                }
                else
                {
                    loadSceneDataBackupTimer();
                }
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            // GUILayout.Label(
            //     " ----------------------------    Load from backup (scene/external file)   ---------------------------")
            GUILayout.Label(" -------------------------------    VN Export   ------------------------------");
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(15);
            GUILayout.BeginHorizontal();
            /*
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("<color=#00ff00>Export</color> scenes and cams\nto VNSceneScript", GUILayout.Height(btnSmallHeight), GUILayout.Width(210)))
            {
                Instance.exportToVNSS();
            }
            */
            //GUILayout.Space(210)
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("<color=#00ff00>Run</color> VN \nfrom beginning", GUILayout.Height(btnSmallHeight), GUILayout.Width(210)))
            {
                sc.runVNSS();
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            //GUILayout.Space(210)
            /*
            Instance.vnFastIsRunImmediately = GUILayout.Toggle(Instance.vnFastIsRunImmediately, "And run from cur scene", GUILayout.Height(20), GUILayout.Width(210));
            GUILayout.FlexibleSpace();
            */
            if (GUILayout.Button("from scene", GUILayout.Height(20), GUILayout.Width(105)))
            {
                sc.runVNSS("scene");
            }
            if (GUILayout.Button("from cam", GUILayout.Height(20), GUILayout.Width(105)))
            {
                sc.runVNSS("cam");
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            // GUILayout.Label(
            //     " ----------------------------    Load from backup (scene/external file)   ---------------------------")
            GUILayout.Label(" -------------------------------    Cam VN texts export/import   ------------------------------");
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Space(5);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Export cam texts\nto sss_camtexts.xml", GUILayout.Height(btnSmallHeight), GUILayout.Width(210)))
            {
                //Instance.ChapterManager.exportCamTexts();
            }
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Import cam texts\nfrom sss_camtexts.xml", GUILayout.Height(btnSmallHeight), GUILayout.Width(210)))
            {
                //Instance.ChapterManager.importCamTexts();
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Import cam texts\nfrom sss_camtexts.txt (Python)", GUILayout.Height(btnSmallHeight), GUILayout.Width(210)))
            {
                //Instance.ChapterManager.ImportCamTextsCustom();
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.EndScrollView();
            return warning;
        }

    }
}
