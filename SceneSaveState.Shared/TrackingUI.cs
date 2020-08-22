using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using VNActor;
using VNEngine;
using static SceneSaveState.SceneConsole;
namespace SceneSaveState
{
    public static partial class UI
    {
        public static void sceneConsoleTrackable()
        {
            //if sc is SceneConsole:
            // sc.svname = GUILayout.TextField(sc.svname)
            // GUILayout.Space(35)
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label(" ---------------------------------    Operations    ------------------------------------");
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(15);
            GUILayout.BeginHorizontal();
            GUILayout.Space(40);
            if (GUILayout.Button("Add selected", GUILayout.Height(50), GUILayout.Width(160)))
            {
                Instance.addSelectedToTrack();
            }
            GUILayout.Space(15);
            if (GUILayout.Button("Del selected", GUILayout.Height(50), GUILayout.Width(160)))
            {
                if (Instance.promptOnDelete)
                {
                    warning_action = Instance.delSelectedFromTrack;
                    warning_param = new WarningParam_s("Delete selected actor from scenes?", null, false);
                }
                else
                {
                    Instance.delSelectedFromTrack();
                }
            }
            GUILayout.Space(15);
            if (GUILayout.Button("Refresh", GUILayout.Height(50), GUILayout.Width(80)))
            {
                Instance.game.LoadTrackedActorsAndProps();
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Space(40);
            if (!Instance.isSysTracking)
            {
                if (GUILayout.Button("Track scene environment", GUILayout.Height(50), GUILayout.Width(160)))
                {
                    Instance.addSysTracking();
                }
            }
            else if (GUILayout.Button("UnTrack scene environment", GUILayout.Height(50), GUILayout.Width(160)))
            {
                Instance.delSysTracking();
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Space(40);
            GUILayout.Label("Pro: Change selected char ID to ", GUILayout.Width(210));
            //GUILayout.Label("  Who say:", GUILayout.Width(80))
            Instance.newid = GUILayout.TextField(Instance.newid, GUILayout.Width(120));
            if (GUILayout.Button("Change", GUILayout.Width(60)))
            {
                //sc.cam_whosay = sc.get_next_speaker(sc.cam_whosay, False)
                Instance.changeSelTrackID(Instance.newid);
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(50);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label(" ----------------------------------------    Tracking chars/props     ----------------------------------------");
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(15);
            //GUILayout.BeginHorizontal()
            tracking_scroll = GUILayout.BeginScrollView(tracking_scroll);
            GUILayout.Label("Actors:");
            var actors = Instance.game.scenef_get_all_actors();
            foreach (var actorid in actors.Keys)
            {
                //GUILayout.Label("  "+actorid+": "+actors[actorid].text_name)
                //txt += "  "+actorid+": "+actors[actorid].text_name+"\n"
                //GUILayout.Label(txt)
                VNActor.Actor actor = actors[actorid];
                render_ui_for_tracking(actorid, actor);
            }
            GUILayout.Label("Props:");
            var props = Instance.game.scenef_get_all_props();
            foreach (var propid in props.Keys)
            {
                render_ui_for_tracking(propid, props[propid]);
            }
            GUILayout.EndScrollView();
        }

        // :type elem:HSNeoOCI
        public static void render_ui_for_tracking(string id, HSNeoOCI elem)
        {
            var txt = id + ": " + elem.text_name;
            GUILayout.BeginHorizontal();
            GUILayout.Space(20);
            var btntext = "";
            if (elem.visible_treenode)
            {
                btntext = "v";
            }
            if (GUILayout.Button(btntext, GUILayout.Width(22)))
            {
                elem.visible_treenode = !elem.visible_treenode;
            }
            bool isSelected = Utils.treenode_check_select(elem.treeNodeObject);
            if (GUILayout.Button(Utils.btntext_get_if_selected(txt, isSelected)))
            {
                Instance.game.studio.treeNodeCtrl.SelectSingle(elem.treeNodeObject);
            }
            //GUILayout.Label(txt)
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }
    }
}
