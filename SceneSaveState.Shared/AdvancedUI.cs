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
            // Debug purpose
            if (GUILayout.Button("Print block data"))
            {
                Console.WriteLine("::::::::::::Debug::::::::::::");
                // print "sc.cur.index :",sc.cur_index
                // print "sc.nameset :",sc.nameset
                // print "sc.block[sc.cur_index].mchars :",sc.block[sc.cur_index].mchars
                // print "sc.block[sc.cur_index].fchars :",sc.block[sc.cur_index].fchars
                // print "sc.block[sc.cur_index].props :", sc.block[sc.cur_index].props
            }
            if (GUILayout.Button("Print char data"))
            {
                Console.WriteLine("::::::::::::Debug::::::::::::");
                var chara = Utils.getSelectedChar(Instance.game);
                if (!(chara == null))
                {
                    VNActor.Actor.ActorData state = (VNActor.Actor.ActorData)chara.export_full_status();
                    var fk_dic = state.fk;
                    Console.WriteLine("fk_set = {");
                    foreach (KeyValuePair<int, Vector3> entry in fk_dic)
                    {
                        var k = entry.Key;
                        var v = entry.Value;
                        Console.WriteLine(k.ToString(), ":", v, ",");
                    }
                    Console.WriteLine("}");
                }
            }
            if (GUILayout.Button("Print Item FK"))
            {
                Console.WriteLine("::::::::::::Debug::::::::::::");
                Item obj = Utils.getSelectedItem(Instance.game);
                if (!(obj == null))
                {
                    ItemData obst = (ItemData)obj.export_full_status();
                    for (int i = 0; i < obst.fk_set.Count; i++)
                    {
                        Vector3 vector = obst.fk_set[i];
                        Console.WriteLine(i.ToString(), ":", vector, ",");
                    }
                }
            }
            GUILayout.Space(25);
            GUILayout.Label("<b>Shortcut settings</b>");
            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            //var cnt = 0;
            /* TODO
            foreach (var command in SceneConsole.Instance.shortcuts.Keys.OrderBy(_p_1 => _p_1).ToList())
            {
                GUILayout.Label(String.Format("%s:", command), GUILayout.Width(110));
                //SceneConsole.Instance.shortcuts[command] = GUILayout.TextField(SceneConsole.Instance.shortcuts[command], GUILayout.Width(120)); TODO
                GUILayout.FlexibleSpace();
                cnt += 1;
                if (cnt % 2 == 0)
                {
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                }
            }
            */
            GUILayout.EndHorizontal();
            if (GUILayout.Button("Save config", GUILayout.Height(50)))
            {
                Utils.saveConfig();
            }
            GUILayout.EndScrollView();
        }
    }
}
