using System.Collections.Generic;
using System.Linq;
using Studio;
using VNActor;

namespace VNEngine
{
    public class RoleTracker
    {
        private const int EMPTY = -1;

        public enum RoleTypes
        {
            Character = 0,
            Prop = 1
        }

        public RoleTracker()
        {
            CharacterRoles = new Dictionary<string, int>();
            PropRoles = new Dictionary<string, int>();
        }

        public RoleTracker(Dictionary<int, Dictionary<string, int>> roles) : this()
        {
            foreach (var type in new List<RoleTypes> {RoleTypes.Character, RoleTypes.Prop})
            foreach (var kv in roles[(int) type])
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

        public void RemoveRole(string roleName)
        {
            CharacterRoles.Remove(roleName);
            PropRoles.Remove(roleName);
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

        public bool ChangeRoleName(string roleName, string newRoleName, Dictionary<string, int> roleList)
        {
            if (!roleList.ContainsKey(roleName) || RoleExists(newRoleName)) return false;

            roleList[newRoleName] = roleList[roleName];
            roleList.Remove(roleName);

            return true;
        }

        public bool ChangeRoleName(string roleName, string newRoleName)
        {
            return IsCharacterRole(roleName)
                ? ChangeRoleName(roleName, newRoleName, CharacterRoles)
                : ChangeRoleName(roleName, newRoleName, PropRoles);
        }


        public Dictionary<int, Dictionary<string, int>> ExportRoles()
        {
            var roleList = new Dictionary<int, Dictionary<string, int>>();

            foreach (var type in new List<RoleTypes> {RoleTypes.Character, RoleTypes.Prop})
                roleList[(int) type] = new Dictionary<string, int>();

            foreach (var kv in CharacterRoles) roleList[(int) RoleTypes.Character][kv.Key] = kv.Value;

            foreach (var kv in PropRoles) roleList[(int) RoleTypes.Prop][kv.Key] = kv.Value;

            return roleList;
        }
    }
}