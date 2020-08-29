using System;
using System.Collections.Generic;
using UnityEngine;
using VNActor;
using static SceneSaveState.SceneConsole;
using static VNActor.Item;

namespace SceneSaveState
{
    public static partial class UI
    {

        public static void sceneConsoleAdvUI()
        {
            //SceneConsole.Instance = SceneConsole.Instance;
            adv_scroll = GUILayout.BeginScrollView(adv_scroll);
            GUILayout.Label("<b>Advanced controls</b>");
            GUILayout.Space(10);
            GUILayout.Label("Change character name:");
            GUILayout.BeginHorizontal();
            Instance.charname = GUILayout.TextField(Instance.charname);
            if (GUILayout.Button("Change selected", GUILayout.Width(110)))
            {
                changeCharName(Instance.game, Instance.charname);
            }
            GUILayout.EndHorizontal();
            GUILayout.Label("Status operations:");
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Copy selected status"))
            {
                Instance.copySelectedStatus();
            }
            if (GUILayout.Button("Paste selected status"))
            {
                Instance.pasteSelectedStatus();
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Copy selected status 2"))
            {
                Instance.copySelectedStatus2();
            }
            if (GUILayout.Button("Paste selected status 2"))
            {
                Instance.pasteSelectedStatus2();
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Copy selected status to tracking chara with same name"))
            {
                Instance.copySelectedStatusToTracking(null);
            }
            // if GUILayout.Button("(without Pos)"):
            //     sc.copySelectedStatusToTracking(["pos"])
            GUILayout.EndHorizontal();
            //GUILayout.Space(15)
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("VN: all cameras on"))
            {
                Instance.camSetAll(true);
            }
            if (GUILayout.Button("VN: all cameras off"))
            {
                Instance.camSetAll(false);
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("VN: add Fake Lip Sync Ext, if no"))
            {
                /* TODO
                //var header = VNSceneScript.get_headerfolder(SceneConsole.Instance.game); 
                if (!(header is null))
                {
                    // vnscenescript.addaction_to_headerfolder(sc.game, ":useext:flipsync10")
                    // vnscenescript.addaction_to_headerfolder(sc.game, ":a:i:initflipsync:v10")
                    Utils.add_folder_if_not_exists(":useext:flipsync10", ":useext:flipsync", header);
                    Utils.add_folder_if_not_exists(":a:i:initflipsync:v10", ":a:i:initflipsync:", header);
                    SceneConsole.Instance.show_blocking_message_time_sc("Done!");
                }
                else
                {
                    SceneConsole.Instance.show_blocking_message_time_sc("Please, export VN at least one time before add Fake Lip Sync");
                }
                */
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(20);
            Instance.autoLoad = GUILayout.Toggle(Instance.autoLoad, "Load scene on select");
            GUILayout.Space(10);
            Instance.autoAddCam = GUILayout.Toggle(Instance.autoAddCam, "Auto add cam for new scenes");
            GUILayout.Space(10);
            Instance.promptOnDelete = GUILayout.Toggle(Instance.promptOnDelete, "Prompt before delete (scene/cam/chars)");
            GUILayout.Space(10);
            Instance.skipClothesChanges = GUILayout.Toggle(Instance.skipClothesChanges, "Don't process clothes changes on scene change");
            GUILayout.Space(10);
            Instance.paramAnimCamIfPossible = GUILayout.Toggle(Instance.paramAnimCamIfPossible, "Animate cam if possible");
            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            GUILayout.Label("Camera anim params: duration ");
            //SceneConsole.Instance.paramAnimCamDuration = GUILayout.TextField(SceneConsole.Instance.paramAnimCamDuration.ToString(), GUILayout.Width(40)); //TODO
            GUILayout.Label(", zoom-out");
            //SceneConsole.Instance.paramAnimCamZoomOut = GUILayout.TextField(SceneConsole.Instance.paramAnimCamZoomOut.ToString(), GUILayout.Width(40)); //TODO
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();     
            GUILayout.EndScrollView();
        }
    }
}
