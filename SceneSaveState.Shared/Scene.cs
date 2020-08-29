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
        public Dictionary<string, NEOPropData> props;
        public SystemData sys;

        public Scene(Dictionary<string, ActorData> actors, Dictionary<string, NEOPropData> props, List<CamData> cams)
        {
            this.cams = cams;
            this.actors = actors;
            this.props = props;
        }

        public Scene() : this(new Dictionary<string, ActorData>(), new Dictionary<string, NEOPropData>(), new List<CamData>())
        {
        }

        public Scene(VNNeoController game, bool importSys) : this()
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
                if (prop is Prop p)
                {
                    this.props[propid] = (NEOPropData)p.export_full_status();
                }
            }
            if (importSys)
            {
                this.sys = (VNEngine.System.SystemData)VNEngine.System.export_sys_status(game);
            }
        }

        public Scene copy()
        {

            var stractors = Utils.SerializeData(this.actors);
            var strprops = Utils.SerializeData(this.props);
            var strcams = Utils.SerializeData(this.cams);
            byte[] strsys = null;
            if (this.sys != null)
            {
                strsys = Utils.SerializeData(this.sys);
            }


            Dictionary<string, ActorData> copied_actors = Utils.DeserializeData<Dictionary<string, ActorData>>(stractors);
            Dictionary<string, NEOPropData> copied_props = Utils.DeserializeData<Dictionary<string, NEOPropData>>(strprops);
            List<CamData> copied_cams = Utils.DeserializeData<List<CamData>>(strcams);
            var copied_scene = new Scene(copied_actors, copied_props, copied_cams);
            if (this.sys != null && strsys != null)
            {
                SystemData copied_sys = Utils.DeserializeData<SystemData>(strsys);
                copied_scene.sys = copied_sys;
            }
            return copied_scene;
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


        // Set scene chars with state data from dictionary

        public void setSceneState(VNNeoController game)
        {
            foreach (var actid in this.actors.Keys)
            {
                //char_import_status_diff_optimized(game.scenef_get_actor(actid),self.actors[actid])
                ActorData char_status = this.actors[actid];
                try
                {
                    /* TODO
                    if (SceneConsole.Instance != null)
                    {
                        if (SceneConsole.Instance.skipClothesChanges)
                        {
                            char_status.Remove("acc_all");
                            char_status.Remove("cloth_all");
                            char_status.Remove("cloth_type");
                        }
                    }
                    */
                }
                catch (Exception)
                {
                }
                //Utils.char_import_status_diff_optimized(game.scenef_get_actor(actid), char_status);
                var actor = game.GetActor(actid);
                try
                {
                    actor?.import_status(char_status);
                }
                catch (Exception e)
                {
                    SceneConsole.Instance.game.GetLogger.LogError($"Error occurred when importing Actor with id {actid}" + e.ToString());
                    SceneConsole.Instance.game.GetLogger.LogMessage($"Error occurred when importing Actor with id {actid}");
                    SceneFolders.LoadTrackedActorsAndProps();
                }
            }
            foreach (var propid in this.props.Keys)
            {
                //vnframe.act(game, {propid: self.props[propid]})
                //print propid
                //print game.scenef_get_all_props()
                Prop prop = game.GetProp(propid);
                
                try
                {
                    if (prop is Item i)
                    {
                        ItemData status = this.props[propid] as ItemData;
                        i.import_status(status);

                    }
                    else if (prop is Light l)
                    {
                        LightData status = this.props[propid] as LightData;
                        l.import_status(status);
                    }
                    else
                    {
                        NEOPropData status = this.props[propid];
                        prop.import_status(status);
                    }
                }
                catch (Exception e)
                {
                    game.GetLogger.LogError($"Error occurred when importing Prop with id {propid}" + e.ToString());
                    SceneFolders.LoadTrackedActorsAndProps();
                    SceneConsole.Instance.game.GetLogger.LogMessage($"Error occurred when importing Prop with id {propid}");
                }
            }
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
