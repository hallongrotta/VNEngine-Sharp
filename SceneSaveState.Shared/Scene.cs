﻿using System;
using System.Collections.Generic;
using VNEngine;
using VNActor;
using CamData = VNEngine.VNCamera.CamData;
using MessagePack;
using static VNActor.Actor;
using static VNActor.Prop;
using static VNActor.Light;

namespace SceneSaveState
{

    public class Scene
    {

        public List<CamData> cams;
        public Dictionary<string, ActorData> actors;
        public Dictionary<string, PropData> props;
        public Dictionary<string, LightData> lights;
        public VNEngine.System.SystemData sys;

        public Scene(Dictionary<string, ActorData> actors, Dictionary<string, PropData> props, Dictionary<string, LightData> lights, List<CamData> cams)
        {
            this.cams = cams;
            this.actors = actors;
            this.props = props;
            this.lights = lights;
        }

        public Scene() : this(new Dictionary<string, ActorData>(), new Dictionary<string, PropData>(), new Dictionary<string, LightData>(), new List<CamData>())
        {
        }

        public Scene copy()
        {

            var stractors = Utils.SerializeData(this.actors);
            var strprops = Utils.SerializeData(this.props);
            var strcams = Utils.SerializeData(this.cams);
            var strlights = Utils.SerializeData(this.lights);

            Dictionary<string, ActorData> copied_actors = Utils.DeserializeData<Dictionary<string, ActorData>>(stractors);
            Dictionary<string, PropData> copied_props = Utils.DeserializeData<Dictionary<string, PropData>>(strprops);
            Dictionary<string, LightData> copied_lights = Utils.DeserializeData<Dictionary<string, LightData>>(strlights);
            List<CamData> copied_cams = Utils.DeserializeData<List<CamData>>(strcams);

            return new Scene(copied_actors, copied_props, copied_lights, copied_cams);
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

        public void importCurScene(VNNeoController game, bool importSys)
        {
            game.LoadTrackedActorsAndProps();
            this.actors = new Dictionary<string, ActorData>();
            this.props = new Dictionary<string, PropData>();
            Dictionary<string, Actor> actors = game.scenef_get_all_actors();
            Dictionary<string, HSNeoOCIProp> props = game.scenef_get_all_props();
            foreach (string actid in actors.Keys)
            {
                this.actors[actid] = (ActorData)actors[actid].export_full_status();
            }
            foreach (string propid in props.Keys)
            {
                var prop = props[propid];
                if (prop is Prop p)
                {
                    this.props[propid] = (PropData)p.export_full_status();
                }
                else if (prop is Light l)
                {
                    this.lights[propid] = (LightData)l.export_full_status();
                }               
            }
            if (importSys)
            {
                this.sys = (VNEngine.System.SystemData)VNEngine.System.export_sys_status(game);
            }
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
                    if (SceneConsole.Instance != null)
                    {
                        if (SceneConsole.Instance.skipClothesChanges)
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
                //Utils.char_import_status_diff_optimized(game.scenef_get_actor(actid), char_status);
                var actor = game.scenef_get_actor(actid);
                try
                {
                    actor?.import_status(char_status);
                }
                catch (Exception e)
                {
                    SceneConsole.Instance.game.GetLogger.LogError($"Error occurred when importing Actor with id {actid}" + e.ToString());
                    game.LoadTrackedActorsAndProps();
                }                              
            }
            foreach (var propid in this.props.Keys)
            {
                //vnframe.act(game, {propid: self.props[propid]})
                //print propid
                //print game.scenef_get_all_props()
                Prop prop = game.scenef_get_propf(propid);
                PropData status = this.props[propid];
                try { 
                    prop?.import_status(status);
                }
                    catch (Exception e)
                {
                    game.GetLogger.LogError($"Error occurred when importing Item with id {propid}" + e.ToString());
                    game.LoadTrackedActorsAndProps();
                }
        }
            foreach (var lightid in this.lights.Keys)
            {
                //vnframe.act(game, {propid: self.props[propid]})
                //print propid
                //print game.scenef_get_all_props()
                Light light = game.scenef_get_light(lightid);
                LightData status = this.lights[lightid];         
                try
                {
                    light?.import_status(status);
                }
                catch (Exception e)
                {
                    SceneConsole.Instance.game.GetLogger.LogError($"Error occurred when importing Item with id {lightid}" + e.ToString());
                    game.LoadTrackedActorsAndProps();
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
