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
            GUILayout.BeginHorizontal();          
            GUILayout.BeginVertical(GUILayout.Width(ColumnWidth));
            GUILayout.Label("Actors:");
            tracking_actors_scroll = GUILayout.BeginScrollView(tracking_actors_scroll);
            var actors = Instance.game.AllActors;
            foreach (var actorid in actors.Keys)
            {
                VNActor.Actor actor = actors[actorid];
                render_ui_for_tracking(actorid, actor);
            }
            GUILayout.EndScrollView();
            GUILayout.EndVertical();

            GUILayout.BeginVertical(GUILayout.Width(ColumnWidth));
            GUILayout.Label("Props:");
            tracking_props_scroll = GUILayout.BeginScrollView(tracking_props_scroll);
            var props = Instance.game.AllProps;
            foreach (var propid in props.Keys)
            {
                render_ui_for_tracking(propid, props[propid]);
            }
            GUILayout.EndScrollView();

            GUILayout.EndVertical();
            GUILayout.BeginVertical(GUILayout.Width(ColumnWidth));
            if (GUILayout.Button("Track selected", GUILayout.Height(50), GUILayout.Width(ColumnWidth)))
            {
                Instance.addSelectedToTrack();
            }
            if (GUILayout.Button("Untrack selected", GUILayout.Height(25), GUILayout.Width(ColumnWidth)))
            {
                if (Instance.promptOnDelete)
                {
                    warning_action = Instance.delSelectedFromTrack;
                    warning_param = new WarningParam_s("Untrack and delete selected from scenes?", false);
                }
                else
                {
                    Instance.delSelectedFromTrack();
                }
            }
            if (GUILayout.Button("Refresh", GUILayout.Height(25), GUILayout.Width(ColumnWidth)))
            {
                SceneFolders.LoadTrackedActorsAndProps();
            }
            if (!Instance.isSysTracking)
            {
                if (GUILayout.Button("Track scene settings", GUILayout.Height(25), GUILayout.Width(ColumnWidth)))
                {
                    Instance.addSysTracking();
                }
            }
            else
            {
                if (GUILayout.Button("Untrack scene settings", GUILayout.Height(25), GUILayout.Width(ColumnWidth)))
                {
                    Instance.delSysTracking();
                }
            }
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.BeginHorizontal();
            GUILayout.Label("Pro: Change selected char ID to ");
            //GUILayout.Label("  Who say:", GUILayout.Width(80))
            Instance.newid = GUILayout.TextField(Instance.newid);
            if (GUILayout.Button("Change", GUILayout.Width(60)))
            {
                //sc.cam_whosay = sc.get_next_speaker(sc.cam_whosay, False)
                Instance.changeSelTrackID(Instance.newid);
            }
            GUILayout.EndHorizontal();
        }

        // :type elem:HSNeoOCI
        public static void render_ui_for_tracking(string id, NeoOCI elem)
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
