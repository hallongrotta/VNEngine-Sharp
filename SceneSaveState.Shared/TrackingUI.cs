using System.Linq;
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
            GUILayout.Label("Character Roles");
            tracking_actors_scroll = GUILayout.BeginScrollView(tracking_actors_scroll);

            var rt = Instance.roleTracker;
            var actors = Instance.roleTracker.AllCharacters;
            var props = Instance.roleTracker.AllProps;

            foreach (var kv in rt.CharacterRoles)
            {
                render_ui_for_tracking(kv.Key, !rt.RoleFilled(kv.Key) ? null : actors[kv.Key]);
            }

            GUILayout.EndScrollView();
            GUILayout.EndVertical();

            GUILayout.BeginVertical(GUILayout.Width(ColumnWidth));
            GUILayout.Label("Prop Roles");
            tracking_props_scroll = GUILayout.BeginScrollView(tracking_props_scroll);
            foreach (var kv in rt.PropRoles)
            {
                render_ui_for_tracking(kv.Key, !rt.RoleFilled(kv.Key) ? null : props[kv.Key]);
            }
            GUILayout.EndScrollView();

            GUILayout.EndVertical();
            GUILayout.BeginVertical(GUILayout.Width(ColumnWidth));
            GUILayout.Label("Role Controls");
            
            if (GUILayout.Button("Add selected to new role.", GUILayout.Height(50), GUILayout.Width(ColumnWidth)))
            {
                Instance.AddSelectedToRole();
            }

            if (GUILayout.Button("Remove selected from role.", GUILayout.Height(25), GUILayout.Width(ColumnWidth)))
            {
                Instance.ClearRoleOfSelected();
            }

            if (Instance.SelectedRole == "")
            {
                GUILayout.Space(50);
            }
            else
            {
                if (GUILayout.Button($"Assign selected to {Instance.SelectedRole}.", GUILayout.Height(25), GUILayout.Width(ColumnWidth)))
                {
                    if (Instance.SelectedRole != "")
                    {
                        Instance.AddSelectedToRole(Instance.SelectedRole);
                    }
                }

                if (GUILayout.Button("Remove role.", GUILayout.Height(25), GUILayout.Width(ColumnWidth)))
                {

                    if (Instance.promptOnDelete.Value)
                    {
                        warning_action = Instance.RemoveSelectedRole;
                        warning_param = new WarningParam_s("Remove role from all scenes?", false);
                    }
                    else
                    {
                        Instance.RemoveSelectedRole();
                    }
                }
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
            if (Instance.track_map)
            {
                if (GUILayout.Button("Stop tracking map selection", GUILayout.Height(25), GUILayout.Width(ColumnWidth))) 
                {
                    Instance.track_map = false;
                }
            }
            else
            {
                if (GUILayout.Button("Track map selection", GUILayout.Height(25), GUILayout.Width(ColumnWidth)))
                {
                    Instance.track_map = true;
                }
            }
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("Change selected role name to:");
            Instance.newid = GUILayout.TextField(Instance.newid);
            if (GUILayout.Button("Change", GUILayout.Width(60)))
            {
                Instance.ChangeSelectedRoleName(Instance.newid);
            }
            GUILayout.EndHorizontal();
        }

        public static void render_ui_for_tracking(string id, NeoOCI elem)
        {
            GUILayout.BeginHorizontal();
            var isSelected = Instance.SelectedRole == id;
            
            // Role Button
            if (GUILayout.Button(Utils.btntext_get_if_selected(id, isSelected), GUILayout.Width(50)))
            {
                Instance.SelectedRole = isSelected ? "" : id;
            }

            // Object button
            if (elem is null)
            {
                GUILayout.Button("Empty");
            }
            else
            {
                isSelected = Instance.game.treenode_check_select(elem.treeNodeObject);
                if (GUILayout.Button(Utils.btntext_get_if_selected(elem.text_name, isSelected)))
                {
                    Instance.game.SelectObject(elem);
                }
            }
            //GUILayout.Label(txt)
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }
    }
}
