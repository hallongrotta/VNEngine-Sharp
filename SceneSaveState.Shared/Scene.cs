using System;
using System.Collections.Generic;
using VNActor;
using VNEngine;
using static VNActor.Actor;
using static VNActor.Item;
using static VNActor.Light;
using static VNEngine.System;
using static VNEngine.VNCamera;

namespace SceneSaveState
{

    public class Scene
    {

        public List<CamData> cams;
        public Dictionary<string, ActorData> actors;
        public Dictionary<string, NEOItemData> items;
        public Dictionary<string, LightData> lights;
        public Dictionary<string, NEOPropData> props;
        public SystemData sys;

        public Scene(Dictionary<string, ActorData> actors, Dictionary<string, NEOItemData> items, Dictionary<string, LightData> lights, Dictionary<string, NEOPropData> props, List<CamData> cams)
        {
            this.cams = cams;
            this.actors = actors;
            this.props = props;
            this.items = items;
            this.lights = lights;
        }

        public Scene()
        {
        }

        public Scene(VNNeoController game, bool importSys) : this(new Dictionary<string, ActorData>(), new Dictionary<string, NEOItemData>(), new Dictionary<string, LightData>(), new Dictionary<string, NEOPropData>(), new List<CamData>())
        {
            SceneFolders.LoadTrackedActorsAndProps();
            Dictionary<string, VNActor.Actor> actors = game.AllActors;
            Dictionary<string, Prop> props = game.AllProps;
            foreach (string actid in actors.Keys)
            {
                this.actors[actid] = (ActorData)actors[actid].export_full_status();
            }
            foreach (string propid in props.Keys)
            {
                var prop = props[propid];
                AddProp(propid, prop);
            }
            if (importSys)
            {
                this.sys = (VNEngine.System.SystemData)export_full_status();
            }
        }

        public void AddProp(string key, IVNObject<Prop> p)
        {
            if (p is Item i)
            {
                this.items[key] = i.export_full_status() as NEOItemData;
            }
            else if (p is Light l)
            {
                this.lights[key] = l.export_full_status() as LightData;
            }
            else // Generic prop
            {
                this.props[key] = p.export_full_status() as NEOPropData;
            }
        }

        public void RemoveProp(string key)
        {
            if (this.items.ContainsKey(key))
            {
                items.Remove(key);
            }
            else if (lights.ContainsKey(key))
            {
                lights.Remove(key);
            }
            else if (props.ContainsKey(key))
            {
                props.Remove(key);
            }
        }

        public Scene copy()
        {         
            byte[] bytes = Utils.SerializeData(this);
            Scene s = Utils.DeserializeData<Scene>(bytes);
            return s;
        }

        public bool isEqual(Scene other)
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


        


        // Camera Manip
        public int addCam(CamData cam_data)
        {
            this.cams.Add(cam_data);
            return this.cams.Count - 1;
        }

        public void updateCam(int index, CamData cam_data)
        {
            if (cams.Count > 0)
            {
                this.cams[index] = cam_data;
            }
        }

        public int deleteCam(int index)
        {
            if (this.cams.Count > 1)
            {
                this.cams.RemoveAt(index);
                if (index == 0 && this.cams.Count > 0)
                {
                    return index;
                }
                return index - 1;
            }
            else
            {
                return 0;
            }
        }
    }
}
