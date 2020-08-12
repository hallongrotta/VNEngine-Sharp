using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using VNEngine;
using static SceneSaveState.SceneConsole;

namespace SceneSaveState
{
    public static partial class UI
    {
        public static void sceneConsoleLdSvUI()
        {
            object fld;
            // sc.svname = GUILayout.TextField(sc.svname)
            // GUILayout.Space(35)
            var btnBigHeight = 60;
            var btnSmallHeight = 50;
            saveload_scroll = GUILayout.BeginScrollView(saveload_scroll);
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
            Instance.svname = GUILayout.TextField(Instance.svname, GUILayout.Width(300));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(20);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("<color=#00ff00>Load</color> from file", GUILayout.Height(btnBigHeight), GUILayout.Width(210)))
            {
                if (Instance.block.Count > 0)
                {
                    warning_action = Instance.loadSceneData;
                    warning_param = new WarningParam_s("Do you wish to load scenedata from file? (Will overwrite console data)", new bool[] { true, false }, false);
                }
                else
                {
                    Instance.loadSceneData(file: true, backup: false);
                }
            }
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("<color=#ff0000>Save</color> to file", GUILayout.Height(btnBigHeight), GUILayout.Width(210)))
            {
                // delete existing scenedata fld
                var fld_str = "-scfile:" + Instance.svname;
                fld = Utils.getFolder(Instance.game, Instance.svname, true);
                if (!(fld == null))
                {
                    warning_action = Instance.saveToFile;
                    warning_param = new WarningParam_s("Scenedata exists. Overwrite?", false, false);
                }
                else
                {
                    Instance.saveToFile(backup: false);
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
                fld = Utils.getFolder(Instance.game, "-scfile:", false);
                if (Instance.block.Count > 0)
                {
                    if (fld == null)
                    {
                        warning_action = Instance.loadSceneData;
                        warning_param = new WarningParam_s("Do you wish to load backup scenedata from scene? (Will overwrite console data)", new bool[] { false, true }, false);
                    }
                    else
                    {
                        warning_action = Instance.loadSceneData;
                        warning_param = new WarningParam_s("Do you wish to load backup scenedata from file? (Will overwrite console data)", new bool[] { true, true }, false);
                    }
                }
                else if (fld == null)
                {
                    Instance.loadSceneData(backup: true);
                }
                else
                {
                    Instance.loadSceneData(file: true, backup: true);
                }
            }
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Load auto-timer backup file", GUILayout.Height(btnSmallHeight), GUILayout.Width(210)))
            {
                //sc.exportToVNSS()
                if (Instance.block.Count > 0)
                {
                    warning_action = Instance.loadSceneDataBackupTimer;
                    warning_param = new WarningParam_s("Do you wish to load backup scenedata from file auto-saved by timer? (Will overwrite console data)", null, false);
                }
                else
                {
                    Instance.loadSceneDataBackupTimer();
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
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("<color=#00ff00>Export</color> scenes and cams\nto VNSceneScript", GUILayout.Height(btnSmallHeight), GUILayout.Width(210)))
            {
                Instance.exportToVNSS();
            }
            //GUILayout.Space(210)
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("...or <color=#00ff00>run</color> VNSceneScript\nfrom beginning", GUILayout.Height(btnSmallHeight), GUILayout.Width(210)))
            {
                Instance.runVNSS();
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            //GUILayout.Space(210)
            Instance.vnFastIsRunImmediately = GUILayout.Toggle(Instance.vnFastIsRunImmediately, "And run from cur scene", GUILayout.Height(20), GUILayout.Width(210));
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("from scene", GUILayout.Height(20), GUILayout.Width(105)))
            {
                Instance.runVNSS("scene");
            }
            if (GUILayout.Button("from cam", GUILayout.Height(20), GUILayout.Width(105)))
            {
                Instance.runVNSS("cam");
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
            if (GUILayout.Button("Export cam texts\nto sss_camtexts.txt", GUILayout.Height(btnSmallHeight), GUILayout.Width(210)))
            {
                Instance.exportCamTexts();
            }
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Import cam texts\nfrom sss_camtexts.txt", GUILayout.Height(btnSmallHeight), GUILayout.Width(210)))
            {
                Instance.importCamTexts();
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.EndScrollView();
        }
    }
}
