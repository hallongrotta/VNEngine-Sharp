using KKAPI.Studio;
using Studio;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VNActor;
using VNEngine;
using static SceneSaveState.UI;
using static SceneSaveState.VNDataComponent;

namespace SceneSaveState
{
    public class RoleTracker
    {



        internal string newid;

        internal SceneConsole sc;

        internal string SelectedRole = "";

        internal bool track_map = true;

        internal bool isSysTracking = true;

        private const int EMPTY = -1;

        private VNData viewVNData;

        public enum RoleTypes
        {
            Character = 0,
            Prop = 1
        }

        internal void delSysTracking()
        {
            isSysTracking = false;
        }

        internal void addSysTracking(Manager<Chapter> chapterManager, StudioController game)
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


        internal void AddSelectedToRole()
        {
            var objects = StudioAPI.GetSelectedObjects();

            /*
            if (!objects.Any())
            {
                sc.show_blocking_message_time_sc("Nothing selected");
                return;
            }
            */

            foreach (var objectCtrl in objects)
                try
                {
                    AddToRole(objectCtrl);
                }
                catch
                {
                }
        }

        internal void RemoveRoleOfSelected(ChapterManager manager)
        {
            var objects = StudioAPI.GetSelectedObjects();

            if (!objects.Any())
            {
                //sc.show_blocking_message_time_sc("Nothing selected");
                return;
            }

            foreach (var oci in objects)
            {
                var roleName = GetRoleName(oci);
                RemoveRole(roleName, manager);
            }
        }

        internal bool ChangeRoleNameInAllChapters(string roleName, string newRoleName, Manager<Chapter> chapterManager)
        {
            if (roleName == "") return false;

            if (!ChangeRoleName(roleName, newRoleName)) return false;

            foreach (var chapter in chapterManager)
            {
                foreach (var scene in chapter) scene.ChangeRoleName(roleName, newRoleName);
            }

            return true;
        }

        internal void ChangeSelectedRoleName(string newRoleName, Manager<Chapter> chapterManager)
        {
            if (SelectedRole == "")
            {
                sc.show_blocking_message("No role selected.");
            }
            else
            {
                if (ChangeRoleNameInAllChapters(SelectedRole, newRoleName, chapterManager)) SelectedRole = newRoleName;
            }
        }

        internal void ClearRoleOfSelected()
        {
            var objects = StudioAPI.GetSelectedObjects();
            foreach (var oci in objects) RemoveFromRole(oci);
        }

        internal void AddSelectedToRole(string roleName)
        {
            var objects = StudioAPI.GetSelectedObjects();
            foreach (var oci in objects) AddToRole(oci, roleName);
        }

        public RoleTracker()
        {
            CharacterRoles = new Dictionary<string, int>();
            PropRoles = new Dictionary<string, int>();
        }

        public RoleTracker(Dictionary<int, Dictionary<string, int>> roles) : this()
        {
            foreach (var type in new List<RoleTypes> { RoleTypes.Character, RoleTypes.Prop })
                foreach (var kv in roles[(int)type])
                {
                    var oci = GetOCI(kv.Value);
                    if (oci is null)
                    {
                        AddRole(kv.Key, type);
                    }
                    else
                    {
                        AddToRole(oci, kv.Key);
                    }
                }
        }

        internal Dictionary<string, T> GenerateActorDict<T>(Dictionary<string, int> roleList) where T : NeoOCI
        {
            var allActors = new Dictionary<string, T>();
            var rolesToEmpty = new List<string>();
            foreach (var kv in roleList)
            {
                if (kv.Value == EMPTY) continue;
                var oci = GetOCI(kv.Value);
                if (oci is null)
                {
                    rolesToEmpty.Add(kv.Key);
                }
                else
                {
                    allActors[kv.Key] = (T)NeoOCI.createFromOCI(oci);
                }
            }

            foreach (var role in rolesToEmpty)
            {
                roleList[role] = EMPTY;
            }

            return allActors;
        }

        public Dictionary<string, Character> AllCharacters => GenerateActorDict<Character>(CharacterRoles);

        public Dictionary<string, Prop> AllProps => GenerateActorDict<Prop>(PropRoles);

        public Dictionary<string, int> CharacterRoles { get; }

        public Dictionary<string, int> PropRoles { get; }

        public void AddFrom(Dictionary<string, Character> characters, Dictionary<string, Prop> props)
        {
            foreach (var kv in characters)
            {
                CharacterRoles[kv.Key] = FindObjectIndex(kv.Value.objctrl);
            }

            foreach (var kv in props)
            {
                PropRoles[kv.Key] = FindObjectIndex(kv.Value.objctrl);
            }
        }

        public bool IsCharacterRole(string roleName)
        {
            return CharacterRoles.ContainsKey(roleName);
        }

        internal string GetBaseRoleName(ObjectCtrlInfo oci)
        {
            switch (oci)
            {
                case OCIItem i:
                    return "item";
                case OCIChar c:
                    return "act";
                case OCIFolder f:
                    return "folder";
                case OCILight l:
                    return "light";
                case OCIRoute r:
                    return "route";
#if KKS
                    case OCIText t:
                    return "text";          
#endif
            }

            return "role";
        }

        internal ObjectCtrlInfo GetOCI(int id)
        {
            var dicObjectCtrl = Studio.Studio.Instance.dicObjectCtrl;
            if (id == EMPTY)
                return null;
            dicObjectCtrl.TryGetValue(id, out var oci);
            return oci;
        }

        public NeoOCI GetOCI(string roleName)
        {
            return NeoOCI.createFromOCI(IsCharacterRole(roleName) ? GetOCI(CharacterRoles[roleName]) : GetOCI(PropRoles[roleName]));
        }


        public int FindObjectIndex(ObjectCtrlInfo oci)
        {
            foreach (var kv in Studio.Studio.Instance.dicObjectCtrl)
                if (kv.Value.Equals(oci))
                    return kv.Key;
            return EMPTY;
        }

        internal string CreateRoleName(string baseName)
        {
            foreach (var i in Enumerable.Range(0, int.MaxValue))
            {
                var id = baseName + i;
                if (!RoleExists(id)) return id;
            }

            return null;
        }

        public void AddToRole(ObjectCtrlInfo oci)
        {
            if (HasRole(oci)) return;
            var roleId = CreateRoleName(GetBaseRoleName(oci));

            AddToRole(oci, roleId);
        }

        public static string GetRoleName<T>(T obj, Dictionary<string, T> dict) where T : NeoOCI
        {
            foreach (var kv in dict)
                if (kv.Value.objctrl == obj.objctrl)
                    return kv.Key;
            return null;
        }

        public string GetRoleName(ObjectCtrlInfo oci)
        {
            if (!HasRole(oci)) return "";
            var id = FindObjectIndex(oci);
            foreach (var kv in CharacterRoles.Where(kv => kv.Value == id)) return kv.Key;

            foreach (var kv in PropRoles.Where(kv => kv.Value == id)) return kv.Key;

            return "";
        }

        public bool RoleExists(string roleName, Dictionary<string, int> roles)
        {
            return roles.ContainsKey(roleName);
        }

        public bool RoleFilled(string roleName, Dictionary<string, int> roles)
        {
            return RoleExists(roleName, roles) && roles[roleName] != EMPTY;
        }

        public bool RoleExists(string roleName)
        {
            return RoleExists(roleName, PropRoles) || RoleExists(roleName, CharacterRoles);
        }

        public bool RoleFilled(string roleName)
        {
            return RoleFilled(roleName, PropRoles) || RoleFilled(roleName, CharacterRoles);
        }

        public bool HasRole(ObjectCtrlInfo oci)
        {
            return PropRoles.Values.Any(p => oci == GetOCI(p)) || CharacterRoles.Values.Any(p => oci == GetOCI(p));
        }

        public int ClearRole(string roleName)
        {
            if (!RoleExists(roleName)) return EMPTY;

            var roleDict = IsCharacterRole(roleName) ? CharacterRoles : PropRoles;
            var status = ClearRole(roleName, roleDict);
            return status;
        }

        public int ClearRole(string roleName, Dictionary<string, int> roleDict)
        {
            var id = roleDict[roleName];
            roleDict[roleName] = EMPTY;
            return id;
        }

        public int RemoveFromRole(ObjectCtrlInfo oci)
        {
            var roleName = GetRoleName(oci);
            return ClearRole(roleName);
        }

        public void AddRole(string roleName, RoleTypes roleType)
        {
            switch (roleType)
            {
                case RoleTypes.Character:
                    CharacterRoles.Add(roleName, -1);
                    break;
                case RoleTypes.Prop:
                    PropRoles.Add(roleName, -1);
                    break;
                default:
                    break;
            }
        }

        public void AddToRole(ObjectCtrlInfo oci, string roleName)
        {
            var actor = NeoOCI.createFromOCI(oci);

            if (RoleExists(roleName))
            {
                var isChara = IsCharacterRole(roleName);
                if (isChara && !(actor is Character)) return;
                if (!isChara && actor is Character) return;
            }

            if (HasRole(oci))
            {
                var oldRole = GetRoleName(oci);
                ClearRole(oldRole);
            }

            switch (actor)
            {
                case Character character:
                    CharacterRoles[roleName] = FindObjectIndex(oci);
                    break;
                case Prop prop:
                    PropRoles[roleName] = FindObjectIndex(oci);
                    break;
            }
        }


        public bool ChangeRoleName(string roleName, string newRoleName)
        {
            return IsCharacterRole(roleName)
                ? ChangeRoleName(roleName, newRoleName, CharacterRoles)
                : ChangeRoleName(roleName, newRoleName, PropRoles);
        }

        public bool ChangeRoleName(string roleName, string newRoleName, Dictionary<string, int> roleList)
        {
            if (!roleList.ContainsKey(roleName) || RoleExists(newRoleName)) return false;

            roleList[newRoleName] = roleList[roleName];
            roleList.Remove(roleName);

            return true;
        }

        internal void RemoveRole(string roleName, ChapterManager chapterManager)
        {
            if (roleName == "") return;
            CharacterRoles.Remove(roleName);
            PropRoles.Remove(roleName);
            chapterManager.RemoveRole(roleName);
        }


        public Dictionary<int, Dictionary<string, int>> ExportRoles()
        {
            var roleList = new Dictionary<int, Dictionary<string, int>>();

            foreach (var type in new List<RoleTypes> { RoleTypes.Character, RoleTypes.Prop })
                roleList[(int)type] = new Dictionary<string, int>();

            foreach (var kv in CharacterRoles) roleList[(int)RoleTypes.Character][kv.Key] = kv.Value;

            foreach (var kv in PropRoles) roleList[(int)RoleTypes.Prop][kv.Key] = kv.Value;

            return roleList;
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
                var elem = GetOCI(roleName);
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

        internal Warning? sceneConsoleTrackable(StudioController game, ChapterManager chapterManager, bool promptOnDelete)
        {
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical(GUILayout.Width(ColumnWidth));
            GUILayout.Label("Character Roles");
            tracking_actors_scroll = GUILayout.BeginScrollView(tracking_actors_scroll);
            Warning? warning = null;

            foreach (var kv in CharacterRoles)
            {
                render_ui_for_tracking(game, kv.Key, RoleFilled(kv.Key));
            }

            GUILayout.EndScrollView();
            GUILayout.EndVertical();

            GUILayout.BeginVertical(GUILayout.Width(ColumnWidth));
            GUILayout.Label("Prop Roles");
            tracking_props_scroll = GUILayout.BeginScrollView(tracking_props_scroll);
            foreach (var kv in PropRoles)
            {
                render_ui_for_tracking(game, kv.Key, RoleFilled(kv.Key));
            }
            GUILayout.EndScrollView();

            GUILayout.EndVertical();
            GUILayout.BeginVertical(GUILayout.Width(ColumnWidth));
            GUILayout.Label("Role Controls");

            if (GUILayout.Button("Add selected to new role.", GUILayout.Height(RowHeight * 2), GUILayout.Width(ColumnWidth)))
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
                        var function = new WarningFunc(() => RemoveRole(SelectedRole, chapterManager));
                        warning = new Warning("Remove role from all scenes?", false, function);
                    }
                    else
                    {
                        RemoveRole(SelectedRole, chapterManager);
                    }
                }
            }

            if (!isSysTracking)
            {
                if (GUILayout.Button("Track scene settings", GUILayout.Height(RowHeight), GUILayout.Width(ColumnWidth)))
                {
                    addSysTracking(chapterManager, game);
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
                ChangeSelectedRoleName(newid, chapterManager);
            }
            GUILayout.EndHorizontal();
            return warning;
        }

        internal const string defaultSpeakerAlias = "s";

        internal string get_next_speaker(string curSpeakAlias, bool next)
        {
            return get_next_speaker(CharacterRoles, curSpeakAlias, next);
        }

        private string get_next_speaker(Dictionary<string, int> allActors, string curSpeakAlias, bool next)
        {
            // next from unknown speaker
            var keylist = allActors.Keys.ToList();
            if (curSpeakAlias != defaultSpeakerAlias && !allActors.ContainsKey(curSpeakAlias))
                return defaultSpeakerAlias;
            // next from s or actor
            if (curSpeakAlias == defaultSpeakerAlias)
            {
                if (allActors.Count > 0)
                {
                    if (next)
                        return keylist[0];
                    return keylist.Last();
                }

                return defaultSpeakerAlias;
            }

            var nextIndex = keylist.IndexOf(curSpeakAlias);
            if (next)
                nextIndex += 1;
            else
                nextIndex -= 1;
            return Enumerable.Range(0, allActors.Count).Contains(nextIndex) ? keylist[nextIndex] : defaultSpeakerAlias;
        }
    }
}