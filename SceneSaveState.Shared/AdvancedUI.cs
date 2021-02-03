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
            if (GUILayout.Button("Set current map to all scenes."))
            {
                Instance.block.SetCurrentMapForAllScenes();
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
            GUILayout.Space(20);
            Instance.autoLoad.Value = GUILayout.Toggle(Instance.autoLoad.Value, "Load scene on select");
            GUILayout.Space(10);
            Instance.autoAddCam.Value = GUILayout.Toggle(Instance.autoAddCam.Value, "Auto add cam for new scenes");
            GUILayout.Space(10);
            Instance.promptOnDelete.Value = GUILayout.Toggle(Instance.promptOnDelete.Value, "Prompt before delete (scene/cam/chars)");
            GUILayout.Space(10);
            Instance.skipClothesChanges.Value = GUILayout.Toggle(Instance.skipClothesChanges.Value, "Don't process clothes changes on scene change");
            GUILayout.Space(10);
            Instance.paramAnimCamIfPossible.Value = GUILayout.Toggle(Instance.paramAnimCamIfPossible.Value, "Animate cam if possible");
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
