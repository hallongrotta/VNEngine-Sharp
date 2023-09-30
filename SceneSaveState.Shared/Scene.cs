using System.Collections.Generic;
using VNActor;
#if KKS
using VNActor.KKS;
#endif
using VNEngine;
using static VNActor.Character;
using static VNActor.Light;
using static VNEngine.System;
using static VNEngine.VNCamera;

using static VNActor.Item;
using MessagePack;

namespace SceneSaveState
{
    [MessagePackObject(keyAsPropertyName: true)]
    public partial class Scene : IManaged<Scene>
    {

        public string Name { get; set; }

        public Dictionary<string, ActorData> actors;

        public List<CamData> cams;

        public Dictionary<string, ItemData> items;

        public Dictionary<string, LightData> lights;
        [IgnoreMember]
        public string TypeName => "Scene";



        public Dictionary<string, NEOPropData> props;
    
        public SystemData sys;

#if !KKS
        public Scene(Dictionary<string, ActorData> actors, Dictionary<string, ItemData> items,
        Dictionary<string, LightData> lights, Dictionary<string, NEOPropData> props, List<CamData> cams)
        {
            this.cams = cams;
            this.actors = actors;   
            this.props = props;
            this.items = items;
            this.lights = lights;

        }
#endif

        internal Scene()
        {
            this.cams = new List<CamData>();
            this.actors = new Dictionary<string, ActorData>();
            this.props = new Dictionary<string, NEOPropData>();
            this.items = new Dictionary<string, ItemData>();
            this.lights = new Dictionary<string, LightData>();
#if KKS
            this.texts = new Dictionary<string, Text.TextData>();
#endif
        }

        public Scene(SystemData sysData, Dictionary<string, Character> actors, Dictionary<string, Prop> props,
            bool importSys) : this()
        {
            foreach (var kv in actors) this.actors[kv.Key] = (ActorData) kv.Value.export_full_status();

            foreach (var kv in props) AddProp(kv.Key, kv.Value);

            if (importSys) sys = sysData;
        }

        public void ApplyStatus<T>(T obj, IDataClass<T> status) where T : NeoOCI
        {
            if (status != null)
                status.Apply(obj);
            else
                obj.Visible = false;
        }

        public void AddProp(string key, IVNObject<Prop> p)
        {
            switch (p)
            {
                case Item i:
                    items[key] = i.export_full_status() as ItemData;
                    break;
                case Light l:
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


        public void Remove(string roleName)
        {
            if (actors.ContainsKey(roleName))
                actors.Remove(roleName);
            else
                RemoveProp(roleName);
        }

        public void RemoveProp(string key)
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

        public bool IsEqual(Scene other)
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


        public void ChangeRoleName<T>(string roleName, string newRoleName, Dictionary<string, T> data)
        {
            if (!data.ContainsKey(roleName)) return;

            data[newRoleName] = data[roleName];
            data.Remove(roleName);
        }

        public void ChangeRoleName(string roleName, string newRoleName)
        {
            ChangeRoleName(roleName, newRoleName, actors);
            ChangeRoleName(roleName, newRoleName, items);
            ChangeRoleName(roleName, newRoleName, props);
            ChangeRoleName(roleName, newRoleName, lights);
#if KKS
            ChangeRoleName(roleName, newRoleName, texts);
#endif

            foreach (var cam in cams)
                if (cam.addata.whosay == roleName)
                    cam.addata.whosay = newRoleName;
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
                case Light l:
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
    }
}