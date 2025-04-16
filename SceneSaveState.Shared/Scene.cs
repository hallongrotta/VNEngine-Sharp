using System.Collections.Generic;
using VNActor;
#if KKS
using VNActor.KKS;
#endif
using VNEngine;
using static VNActor.Character;
using static VNActor.Light;
using static VNEngine.System;

using static VNActor.Item;
using static SceneSaveState.Camera;
using ADV;
using static SceneSaveState.VNDataComponent;
using MessagePack;
using UnityEngine;
using static SceneSaveState.UI;

namespace SceneSaveState
{
    [MessagePackObject]
    public partial class Scene : Manager<View>, IManaged<Scene>
    {

        [Key("Name")]
        public string Name { get; set; }

        [Key("actors")]
        public Dictionary<string, ActorData> actors;

        [Key("cams")]
        public List<CamData> cams;

        [Key("items")]
        public Dictionary<string, ItemData> items;

        [Key("lights")]
        public Dictionary<string, LightData> lights;

        [IgnoreMember]
        public string TypeName => "Scene";

        [Key("props")]
        public Dictionary<string, NEOPropData> props;

        [Key("sys")]
        public SystemData sys;

        [Key("views")]
        public List<View> views {get => Items ; private set => ImportItems(value); }

        public Scene(
     string Name,
     List<View> views,
     Dictionary<string, ActorData> actors,
     List<CamData> cams,
     Dictionary<string, ItemData> items,
     Dictionary<string, LightData> lights,
     Dictionary<string, NEOPropData> props,
     SystemData sys
     ) : base()
        {
            this.cams = null;
            if (cams != null)
            {
                foreach (var c in cams)
                {
                    Add(new View(c));
                }
            }
            this.actors = actors;
            this.props = props;
            this.items = items;
            this.lights = lights;
            this.sys = sys;
            this.Name = Name;
            if (views != null)
            {
                this.views = views;
            }
        }

        internal Scene()
        {
            this.actors = new Dictionary<string, ActorData>();
            this.props = new Dictionary<string, NEOPropData>();
            this.items = new Dictionary<string, ItemData>();
            this.lights = new Dictionary<string, LightData>();
#if KKS
            this.texts = new Dictionary<string, Text.TextData>();
#endif
        }

        internal Scene(SystemData sysData, Dictionary<string, Character> actors, Dictionary<string, Prop> props,
            bool importSys) : this()
        {
            foreach (var kv in actors) this.actors[kv.Key] = (ActorData) kv.Value.export_full_status();

            foreach (var kv in props) AddProp(kv.Key, kv.Value);

            if (importSys) sys = sysData;
        }

        internal void ApplyStatus<T>(T obj, IDataClass<T> status) where T : NeoOCI
        {
            if (status != null)
                status.Apply(obj);
            else
                obj.Visible = false;
        }

        internal void DeleteSceneCam()
        {
            Remove();
        }

        internal void AddProp(string key, IVNObject<Prop> p)
        {
            switch (p)
            {
                case Item i:
                    items[key] = i.export_full_status() as ItemData;
                    break;
                case VNActor.Light l:
                    lights[key] = l.export_full_status() as LightData;
                    break;
#if KKS
                case Text t:
                    texts[key] = t.export_full_status() as Text.TextData;
                    break;
#endif
                // Generic prop
                default:
                    props[key] = p.export_full_status() as NEOPropData;
                    break;
            }
        }


        internal void Remove(string roleName)
        {
            if (actors.ContainsKey(roleName))
                actors.Remove(roleName);
            else
                RemoveProp(roleName);
        }

        internal void RemoveProp(string key)
        {
            if (items.ContainsKey(key))
                items.Remove(key);
            else if (lights.ContainsKey(key))
                lights.Remove(key);
            else if (props.ContainsKey(key)) props.Remove(key);
#if KKS
            else if (texts.ContainsKey(key)) texts.Remove(key);
#endif
        }

        public Scene Copy()
        {
            var bytes = Utils.SerializeData(this);
            var s = Utils.DeserializeData<Scene>(bytes);
            return s;
        }

        internal bool IsEqual(Scene other)
        {
            /* TODO
            if (!Utils.is_arr_equal(cams, other.cams))
                return false;
            if (!Utils.is_status_statuses_equal(actors, other.actors))
                return false;
            if (!Utils.is_status_statuses_equal(props, other.props))
                return false;

            return true;
            */
            return false;
        }


        internal void ChangeRoleName<T>(string roleName, string newRoleName, Dictionary<string, T> data)
        {
            if (!data.ContainsKey(roleName)) return;

            data[newRoleName] = data[roleName];
            data.Remove(roleName);
        }

        internal void ChangeRoleName(string roleName, string newRoleName)
        {
            ChangeRoleName(roleName, newRoleName, actors);
            ChangeRoleName(roleName, newRoleName, items);
            ChangeRoleName(roleName, newRoleName, props);
            ChangeRoleName(roleName, newRoleName, lights);
#if KKS
            ChangeRoleName(roleName, newRoleName, texts);
#endif

            VNData data;
            foreach (var cam in this)
                for (var i = 0; i < cam.Count; i++)
                    if (cam[i].whosay == roleName)
                    {
                        data = cam[i];
                        data.whosay = newRoleName;
                        cam[i] = data;
                    }   
        }

        internal void SetCharacterState(string roleName, Character character)
        {
            actors.TryGetValue(roleName, out var actorData);
            if (actorData is null)
                character.Visible = false;
            else
                actorData.Apply(character);
        }

        internal void SetCharacterState(Dictionary<string, Character> roles)
        {
            foreach (var kvp in roles) SetCharacterState(kvp.Key, kvp.Value);
        }

        internal void SetPropState(string roleName, Prop actor)
        {
            switch (actor)
            {
                case Item i:
                {
                    items.TryGetValue(roleName, out var status);
                    ApplyStatus(i, status);
                    break;
                }
                case VNActor.Light l:
                {
                    lights.TryGetValue(roleName, out var status);
                    ApplyStatus(l, status);
                    break;
                }
#if KKS
                case Text t:
                {
                    texts.TryGetValue(roleName, out var status);
                    ApplyStatus(t, status);
                    break;
                }
#endif
                case Prop p:
                {
                    props.TryGetValue(roleName, out var status);
                    ApplyStatus(p, status);
                    break;
                }
            }
        }

        internal void SetPropState(Dictionary<string, Prop> roles)
        {
            foreach (var kvp in roles) SetPropState(kvp.Key, kvp.Value);
        }
        internal void SetSceneState(StudioController game, RoleTracker roleTracker)
        {
            if (roleTracker.isSysTracking) game.Apply(sys, roleTracker.track_map);

            //var watch = new Stopwatch();
            //watch.Start();
            SetCharacterState(roleTracker.AllCharacters);
            //watch.Stop();
            //Logger.LogInfo($"Loaded character data in {watch.ElapsedMilliseconds} ms.");

            SetPropState(roleTracker.AllProps);
        }


        // Views

        private View[] camset;

        internal void DeleteSceneCam(Camera c, VNController gc)
        {
            Remove();
            if (HasItems) Current.setCamera(c, gc);
        }

        internal void addSceneCam(Camera c)
        {
            var camData = c.export();
            Add(new View(camData));
        }

        internal void updateSceneCam(Camera c)
        {
            var camData = c.export();
            Update(new View(camData));
        }

        // Copy/paste cam set
        internal void copyCamSet()
        {
            camset = ExportItems();
        }

        internal void pasteCamSet()
        {
            ImportItems(camset);
        }

        internal Warning? DrawCamSelect(Camera c, VNController gameController, bool promptOnDelete)
        {

            Warning? warning = null;
            cam_scroll = GUILayout.BeginScrollView(cam_scroll, GUILayout.Height(185));
            for (int i = 0; i < Count - 0; i++)
            {
                var col = i == CurrentIndex ? SelectedTextColor : "#f9f9f9";
                var cam = this[i];
                var camText = $"{cam.TypeName} {i + 1}";

                GUILayout.BeginHorizontal();

                if (GUILayout.Button($"<color={col}>{camText}, {(int)cam.camData.fov}</color>", GUILayout.Width(ColumnWidth * 0.8f)))
                {
                    var camData = SetCurrent(i);
                    camData.setCamera(c, gameController, isAnimated: false);
                }
                if (GUILayout.Button($"<color={col}>a</color>", GUILayout.Width(ColumnWidth * 0.2f)))
                {
                    var camData = SetCurrent(i);
                    camData.setCamera(c, gameController, isAnimated: true);
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndScrollView();
            GUILayout.FlexibleSpace();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Add", GUILayout.Width(ColumnWidth * 0.7f)))
            {
                addSceneCam(c);
            }


            if (HasItems)
            {
                if (GUILayout.Button("Del", GUILayout.Width(ColumnWidth * 0.3f)))
                {
                    if (promptOnDelete)
                    {
                        warning = new Warning("Delete selected cam?", false, DeleteSceneCam);
                    }
                    else
                    {
                        DeleteSceneCam();
                    }
                }
            }
            GUILayout.EndHorizontal();
            if (Count > 0)
            {
                if (GUILayout.Button("Update", GUILayout.Width(ColumnWidth + 5)))
                {
                    updateSceneCam(c);
                }
            }
            GUILayout.BeginHorizontal();
            const string up = "\u2191";
            const string down = "\u2193";
            if (GUILayout.Button($"Cam {up}"))
            {
                MoveItemBack();
            }
            if (GUILayout.Button($"Cam {down}"))
            {
                MoveItemForward();
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            if (HasItems)
            {
                if (GUILayout.Button("Copy cams"))
                {
                    copyCamSet();
                }
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            if (camset != null)
            {
                if (GUILayout.Button("Paste cams"))
                {
                    pasteCamSet();
                }
            }
            GUILayout.EndHorizontal();
            return warning;
        }

    }
}