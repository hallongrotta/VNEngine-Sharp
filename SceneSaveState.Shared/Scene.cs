using System;
using System.Collections.Generic;
using VNEngine;
using VNActor;
using CamData = VNEngine.VNCamera.CamData;
using Newtonsoft.Json;
using static VNActor.Actor;
using static VNActor.Prop;

namespace SceneSaveState
{

    public class Scene
    {
        private SceneConsole sc;
        public List<CamData> cams;
        public Dictionary<string, ActorData> actors;
        public Dictionary<string, PropData> props;
        public VNEngine.System.SystemData sys;

        public Scene(SceneConsole sc, Dictionary<string, ActorData> actors, Dictionary<string, PropData> props, List<CamData> cams)
        {
            this.sc = sc;
            this.cams = cams;
            this.actors = actors;
            this.props = props;
        }

        public Scene(SceneConsole sc)
        {
            new Scene(sc, new Dictionary<string, ActorData>(), new Dictionary<string, PropData>(), new List<CamData>());
        }


        public Scene copy()
        {

            
            string stractors = JsonConvert.SerializeObject(this.actors);
            string strprops = JsonConvert.SerializeObject(this.props);
            string strcams = JsonConvert.SerializeObject(this.cams);

            Dictionary<string, ActorData> copied_actors = JsonConvert.DeserializeObject<Dictionary<string, ActorData>>(stractors);
            Dictionary<string, PropData> copied_props = JsonConvert.DeserializeObject<Dictionary<string, PropData>>(strprops);
            List<CamData> copied_cams = JsonConvert.DeserializeObject<List<CamData>>(strcams);
            return new Scene(sc, copied_actors, copied_props, copied_cams);
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

        public void importCurScene(VNNeoController game, Dictionary<string, object> options)
        {
            game.scenef_register_actorsprops();
            this.actors = new Dictionary<string, ActorData>();
            this.props = new Dictionary<string, PropData>();
            Dictionary<string, Actor> actors = game.scenef_get_all_actors();
            Dictionary<string, Prop> props = game.scenef_get_all_props();
            foreach (string actid in actors.Keys)
            {
                this.actors[actid] = (ActorData)actors[actid].export_full_status();
            }
            foreach (string propid in props.Keys)
            {
                this.props[propid] = (PropData)props[propid].export_full_status();
            }
            if (options.ContainsKey("sys"))
            {
                this.sys = (VNEngine.System.SystemData)VNEngine.System.export_sys_status(game);
            }
        }


        // Set scene chars with state data from dictionary

        public void setSceneState(VNNeoController game)
        {
            foreach (var actid in this.actors.Keys)
            {
                if (actid == "sys")
                {
                    //vnframe.act(game, {'sys': self.actors[actid]})
                    Utils.sys_import_status_diff_optimized(game, this.actors[actid]);
                }
                else
                {
                    //char_import_status_diff_optimized(game.scenef_get_actor(actid),self.actors[actid])
                    ActorData char_status = this.actors[actid];
                    try
                    {
                        if (sc != null)
                        {
                            if (sc.skipClothesChanges)
                            {
                                    char_status.Remove("acc_all");
                                    char_status.Remove("cloth_all");
                                    char_status.Remove("cloth_type");
                            }
                        }
                    }
                    catch (Exception)
                    {
                    }
                    Utils.char_import_status_diff_optimized(game.scenef_get_actor(actid), char_status);
                }
            }
            foreach (var propid in this.props.Keys)
            {
                //vnframe.act(game, {propid: self.props[propid]})
                //print propid
                //print game.scenef_get_all_props()
                game.scenef_get_propf(propid).import_status(this.props[propid]);
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
            this.cams[index] = cam_data;
        }

        public int deleteCam(int index)
        {
            if (this.cams.Count > 0)
            {
                this.cams.RemoveAt(index);
                if (index == 0 && this.cams.Count > 0)
                {
                    return index;
                }
                return index - 1;
            } else
            {
                throw new Exception("No camera to delete!");
            }
        }
    }
}
