using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using VNActor;
using VNEngine;
using static VNActor.Character;
using static VNActor.Item;
using static VNActor.Light;

namespace SceneSaveState
{
    internal class SceneConsoleCopyComponent
    {

        internal SceneConsole sc;

        internal IDataClass<IVNObject<object>> clipboard_status;

        internal IDataClass<IVNObject<object>> clipboard_status2;

        internal static Vector2 adv_scroll = new Vector2(0, 0);

        RoleTracker roleTracker;

        internal string charname;

        internal SceneConsoleCopyComponent(SceneConsole sc)
        {
            this.sc = sc;
        }

        internal void copySelectedStatusToTracking(Dictionary<string, Character> actors, List<string> exclude)
        {
            var elem = NeoOCI.create_from_selected();
            if (elem is Character chara)
            {
                var tmp_status = chara.export_full_status();
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

                sc.show_blocking_message_time_sc("Can't find tracking char with same name");
            }
            else
            {
                sc.show_blocking_message_time_sc("Can't copy status");
            }
        }

        internal void copySelectedStatus()
        {
            var elem = NeoOCI.create_from_selected();
            if (elem is Character chara)
                clipboard_status = (IDataClass<IVNObject<object>>)chara.export_full_status();
            else if (elem is Prop prop)
                clipboard_status = (IDataClass<IVNObject<object>>)prop.export_full_status();
            else
                sc.show_blocking_message_time_sc("Can't copy status");
        }

        internal void pasteSelectedStatus()
        {
            var elem = NeoOCI.create_from_selected();
            if (elem is Character chara)
            {
                chara.import_status((ActorData)clipboard_status);
            }
            else if (elem is Prop prop)
            {
                if (elem is Item i)
                    i.import_status((ItemData)clipboard_status2);
                else if (elem is VNActor.Light l)
                    prop.import_status((LightData)clipboard_status2);
                else
                    prop.import_status((NEOPropData)clipboard_status2);
            }
            else
            {
                sc.show_blocking_message_time_sc("Can't paste status");
            }
        }

        internal void copySelectedStatus2()
        {
            var elem = NeoOCI.create_from_selected();
            if (elem is Character chara)
                clipboard_status2 = (IDataClass<IVNObject<object>>)chara.export_full_status();
            else if (elem is Prop prop)
                clipboard_status2 = (IDataClass<IVNObject<object>>)prop.export_full_status();
            else
                sc.show_blocking_message_time_sc("Can't copy status 2");
        }

        internal void pasteSelectedStatus2()
        {
            var elem = NeoOCI.create_from_selected();
            if (elem is Character chara)
            {
                chara.import_status((ActorData)clipboard_status2);
            }
            else if (elem is Prop prop)
            {
                if (elem is Item i)
                    i.import_status((ItemData)clipboard_status2);
                else if (elem is VNActor.Light l)
                    prop.import_status((LightData)clipboard_status2);
                else
                    prop.import_status((NEOPropData)clipboard_status2);
            }
            else
            {
                sc.show_blocking_message_time_sc("Can't paste status 2");
            }
        }

        // Change name
        internal void changeCharName(StudioController game, string name)
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

        internal void sceneConsoleAdvUI(StudioController game, Chapter c)
        {
            //SceneConsole.Instance = SceneConsole.Instance;
            adv_scroll = GUILayout.BeginScrollView(adv_scroll);
            GUILayout.Label("<b>Advanced controls</b>");
            GUILayout.Space(10);
            GUILayout.Label("Change character name:");
            GUILayout.BeginVertical();
            charname = GUILayout.TextField(charname);
            if (GUILayout.Button("Change selected", GUILayout.Width(110)))
            {
                changeCharName(game, charname);
            }
            GUILayout.Label("Status operations:");
            if (GUILayout.Button("Copy selected status"))
            {
                copySelectedStatus();
            }
            if (GUILayout.Button("Paste selected status"))
            {
                pasteSelectedStatus();
            }
            if (GUILayout.Button("Copy selected status 2"))
            {
                copySelectedStatus2();
            }
            if (GUILayout.Button("Paste selected status 2"))
            {
                pasteSelectedStatus2();
            }
            if (GUILayout.Button("Copy selected status to tracking chara with same name"))
            {
                copySelectedStatusToTracking(roleTracker.AllCharacters, null);
            }
            if (GUILayout.Button("Set current map to all scenes."))
            {
                c.SetCurrentMapNumForAllScenes(game.MapNumber);
            }
            if (GUILayout.Button("Set current map transform to all scenes."))
            {
                c.SetCurrentMapTransformForAllScenes(game.MapPos, game.MapRot);
            }
#if HS2
            if (GUILayout.Button("Set current actor's clothes to all scenes."))
            {
                string id = sc.GetIDOfSelectedObject();
                if (!(id is null))
                {
                    c.SetCurrentClothesForAllScenes(id);
                }
            }
            if (GUILayout.Button("Set current actor's accessories to all scenes."))
            {
                string id = sc.GetIDOfSelectedObject();
                if (!(id is null))
                {
                    c.SetCurrentAccessoriesForAllScenes(id);
                }
            }
            #endif
            // if GUILayout.Button("(without Pos)"):
            //     sc.copySelectedStatusToTracking(["pos"])
            GUILayout.EndVertical();
            //GUILayout.Space(15)
            GUILayout.BeginHorizontal();
            GUILayout.EndHorizontal();
            GUILayout.Space(20);
            sc.DrawConfigSettings();
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
