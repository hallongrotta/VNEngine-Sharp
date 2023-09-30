
using KKAPI.Studio;
using Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using VNEngine;
using static SceneSaveState.UI;

namespace SceneSaveState
{
    internal class SceneConsoleRoleComponent
    {
        internal bool isSysTracking = true;
        internal RoleTracker roleTracker;
        internal Manager<Chapter> chapterManager;

        internal string newid;

        internal SceneConsole sc;

        internal string SelectedRole = "";

        internal bool track_map = true;

        internal SceneConsoleRoleComponent(RoleTracker rt, Manager<Chapter> cm)
        {
            this.chapterManager = cm;
            this.roleTracker = rt;
        }

        internal void addSysTracking(StudioController game)
        {
            if (chapterManager.HasItems)
            {
                var curstatus = game.export_full_status();
                foreach (Chapter c in chapterManager)
                {
                    foreach (Scene s in c)
                    {
                        s.sys = curstatus;
                    }
                }
                isSysTracking = true;
            }
            else
            {
                sc.show_blocking_message_time_sc("Please, add at least 1 state to add system environment tracking");
            }
        }

        internal void delSysTracking()
        {
            isSysTracking = false;
        }

        internal void ChangeSelectedRoleName(string newRoleName)
        {
            if (SelectedRole == "")
            {
                sc.show_blocking_message("No role selected.");
            }
            else
            {
                if (ChangeRoleName(SelectedRole, newRoleName)) SelectedRole = newRoleName;
            }
        }

        internal void SetSceneState(Scene s, StudioController game)
        {
            if (isSysTracking) game.Apply(s.sys, track_map);

            //var watch = new Stopwatch();
            //watch.Start();
            s.SetCharacterState(roleTracker.AllCharacters);
            //watch.Stop();
            //Logger.LogInfo($"Loaded character data in {watch.ElapsedMilliseconds} ms.");

            s.SetPropState(roleTracker.AllProps);
        }

        internal void ClearRoleOfSelected()
        {
            var objects = StudioAPI.GetSelectedObjects();
            foreach (var oci in objects) roleTracker.RemoveFromRole(oci);
        }

        internal void AddSelectedToRole(string roleName)
        {
            var objects = StudioAPI.GetSelectedObjects();
            foreach (var oci in objects) roleTracker.AddToRole(oci, roleName);
        }

        internal void RemoveRoleOfSelected()
        {
            var objects = StudioAPI.GetSelectedObjects();

            if (!objects.Any())
            {
                sc.show_blocking_message_time_sc("Nothing selected");
                return;
            }

            foreach (var oci in objects)
            {
                var roleName = roleTracker.GetRoleName(oci);
                RemoveRole(roleName);
            }
        }

        internal void RemoveRole(string roleName)
        {
            if (roleName == "") return;

            roleTracker.RemoveRole(roleName);

            foreach (var chapter in chapterManager)
            {
                foreach (var scene in chapter) scene.Remove(roleName);
            }
        }

        internal bool ChangeRoleName(string roleName, string newRoleName)
        {
            if (roleName == "") return false;

            if (!roleTracker.ChangeRoleName(roleName, newRoleName)) return false;

            foreach (var chapter in chapterManager)
            {
                foreach (var scene in chapter) scene.ChangeRoleName(roleName, newRoleName);
            }

            return true;
        }

        internal void RemoveSelectedRole()
        {
            roleTracker.RemoveRole(SelectedRole);
        }

        internal void AddSelectedToRole()
        {
            var objects = StudioAPI.GetSelectedObjects();

            if (!objects.Any())
            {
                sc.show_blocking_message_time_sc("Nothing selected");
                return;
            }

            foreach (var objectCtrl in objects)
                try
                {
                    roleTracker.AddToRole(objectCtrl);
                }
                catch
                {
                }
        }

        internal Warning? sceneConsoleTrackable(StudioController game, bool promptOnDelete)
        {
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical(GUILayout.Width(ColumnWidth));
            GUILayout.Label("Character Roles");
            tracking_actors_scroll = GUILayout.BeginScrollView(tracking_actors_scroll);
            Warning? warning = null;

            var rt = roleTracker;

            foreach (var kv in rt.CharacterRoles)
            {
                render_ui_for_tracking(game, kv.Key, rt.RoleFilled(kv.Key));
            }

            GUILayout.EndScrollView();
            GUILayout.EndVertical();

            GUILayout.BeginVertical(GUILayout.Width(ColumnWidth));
            GUILayout.Label("Prop Roles");
            tracking_props_scroll = GUILayout.BeginScrollView(tracking_props_scroll);
            foreach (var kv in rt.PropRoles)
            {
                render_ui_for_tracking(game, kv.Key, rt.RoleFilled(kv.Key));
            }
            GUILayout.EndScrollView();

            GUILayout.EndVertical();
            GUILayout.BeginVertical(GUILayout.Width(ColumnWidth));
            GUILayout.Label("Role Controls");

            if (GUILayout.Button("Add selected to new role.", GUILayout.Height(RowHeight*2), GUILayout.Width(ColumnWidth)))
            {
                AddSelectedToRole();
            }

            if (GUILayout.Button("Remove selected from role.", GUILayout.Height(RowHeight), GUILayout.Width(ColumnWidth)))
            {
                ClearRoleOfSelected();
            }

            if (SelectedRole == "")
            {
                GUILayout.Space(50);
            }
            else
            {
                if (GUILayout.Button($"Assign selected to {SelectedRole}.", GUILayout.Height(RowHeight), GUILayout.Width(ColumnWidth)))
                {
                    if (SelectedRole != "")
                    {
                        AddSelectedToRole(SelectedRole);
                    }
                }

                if (GUILayout.Button("Remove role.", GUILayout.Height(RowHeight), GUILayout.Width(ColumnWidth)))
                {

                    if (promptOnDelete)
                    {
                        warning = new Warning("Remove role from all scenes?", false, RemoveSelectedRole);
                    }
                    else
                    {
                        RemoveSelectedRole();
                    }
                }
            }

            if (!isSysTracking)
            {
                if (GUILayout.Button("Track scene settings", GUILayout.Height(RowHeight), GUILayout.Width(ColumnWidth)))
                {
                    addSysTracking(game);
                }
            }
            else
            {
                if (GUILayout.Button("Untrack scene settings", GUILayout.Height(RowHeight), GUILayout.Width(ColumnWidth)))
                {
                    delSysTracking();
                }
            }
            if (track_map)
            {
                if (GUILayout.Button("Stop tracking map selection", GUILayout.Height(RowHeight), GUILayout.Width(ColumnWidth)))
                {
                    track_map = false;
                }
            }
            else
            {
                if (GUILayout.Button("Track map selection", GUILayout.Height(RowHeight), GUILayout.Width(ColumnWidth)))
                {
                    track_map = true;
                }
            }
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("Change selected role name to:");
            newid = GUILayout.TextField(newid);
            if (GUILayout.Button("Change", GUILayout.Width(60)))
            {
                ChangeSelectedRoleName(newid);
            }
            GUILayout.EndHorizontal();
            return warning;
        }

        internal void render_ui_for_tracking(StudioController game, string roleName, bool roleFilled)
        {
            GUILayout.BeginHorizontal();
            var isSelected = SelectedRole == roleName;

            // Role Button
            if (GUILayout.Button(Utils.btntext_get_if_selected(roleName, isSelected), GUILayout.Width(50)))
            {
                SelectedRole = isSelected ? "" : roleName;
            }

            // Object button
            if (roleFilled)
            {
                var elem = roleTracker.GetOCI(roleName);
                isSelected = game.treenode_check_select(elem.treeNodeObject);
                if (GUILayout.Button(Utils.btntext_get_if_selected(elem.text_name, isSelected)))
                {
                    game.SelectObject(elem);
                }
            }
            else
            {
                GUILayout.Button("Empty");
            }
            //GUILayout.Label(txt)
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }
    

}
}
