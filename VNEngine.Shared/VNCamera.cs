using MessagePack;
using Studio;
using System.Collections.Generic;
using UnityEngine;

namespace VNEngine
{
    public class VNCamera
    {
        public struct adv_properties
        {
            [Key(0)]
            public string name;
            [Key(1)]
            public bool isTime;
            [Key(2)]
            public string time;
            [Key(3)]
            public bool isTAnimCam;
            [Key(4)]
            public bool isTHideUI;
            [Key(5)]
            public bool isTTimerNext;
            [Key(6)]
            public string tacStyle;
            [Key(7)]
            public string tacZOut;
            [Key(8)]
            public string tacRotX;
            [Key(9)]
            public string tacRotZ;
            [Key(10)]
            public bool keepcam;
        }

        public struct VNData
        {
            public bool enabled; // formerly addparam
            public string whosay;
            public string whatsay;
            public string addvncmds;
            public bool add_props;
            public addprops_struct addprops;

            public struct addprops_struct
            {
                [Key(0)]
                public adv_properties a1o;
                //[Key(1)]
                //public Dictionary<string, bool> a2o;
                [Key(2)]
                public bool a1;
                [Key(3)]
                public bool a2;
            }

            public VNData(bool enabled, string whosay, string whatsay, string addvncmds, addprops_struct addprops)
            {
                this.enabled = enabled;
                this.whosay = whosay;
                this.whatsay = whatsay;
                this.addvncmds = addvncmds;
                this.addprops = addprops;
                this.add_props = false;
            }
        }

        [MessagePackObject(keyAsPropertyName: true)]
        public class CamData
        {
            public Vector3 position;
            public Vector3 rotation;
            public Vector3 distance;
            public float fov;
            public VNData addata;
            public int camnum;
            public float duration;
            public string style;
            public float zoom_delta;
            public bool hasVNData;

            public CamData(Vector3 position, Vector3 rotation, Vector3 distance, float fov)
            {
                this.camnum = -1; //TODO
                this.duration = -1;
                this.zoom_delta = -1;
                this.position = position;
                this.rotation = rotation;
                this.distance = distance;
                this.fov = fov;
                this.addata = new VNData();
                this.style = "";
                this.hasVNData = false;
            }

            public CamData(Vector3 position, Vector3 rotation, Vector3 distance, float fov, VNData addata)
            {
                this.camnum = -1; //TODO
                this.duration = -1;
                this.zoom_delta = -1;
                this.position = position;
                this.rotation = rotation;
                this.distance = distance;
                this.fov = fov;
                this.addata = addata;
                this.style = "";
                this.hasVNData = false;
            }

            public CamData()
            {

            }
        }

        public delegate void CamOperation(VNNeoController game, CamData param);

        public static void cam_goto_preset(VNNeoController game, CamData param)
        {
            // param = (set, duration)
            game.anim_to_camera_num(param.duration, param.camnum);
        }

        /*
        public static void cam_goto_preset(VNNeoController game, (int camnum, int duration, string style) param)
        {
            game.anim_to_camera_num(param.duration, param.camnum, param.style);
        }


        public static void cam_goto_preset(VNNeoController game, (int camnum, int duration, string style, GameFunc onCamEnd) param)
        {
            game.anim_to_camera_num(param.duration, param.camnum, param.style, param.onCamEnd);
        }
        */

        public static void cam_goto_preset(VNNeoController game, int param)
        {
            game.to_camera(param);
        }

        /*
        public static void cam_goto_pos(VNNeoController game, (Vector3 pos, Vector3 distance, Vector3 rotate) param)
        {
            game.move_camera(pos: param.pos, distance: param.distance, rotate: param.rotate);
        }

        public static void cam_goto_pos(VNNeoController game, (Vector3 pos, Vector3 distance, Vector3 rotate, float duration, string style) param)
        {
            game.anim_to_camera(param.duration, pos: param.pos, distance: param.distance, rotate: param.rotate, style: param.style);
        }

        public static void cam_goto_pos(VNNeoController game, (Vector3 pos, Vector3 distance, Vector3 rotate, float duration) param)
        {
            game.anim_to_camera(param.duration, pos: param.pos, distance: param.distance, rotate: param.rotate);
        }

        public static void cam_goto_pos(VNNeoController game, (Vector3 pos, Vector3 distance, Vector3 rotate, float duration, string style, float fov) param)
        {
            game.anim_to_camera(param.duration, pos: param.pos, distance: param.distance, rotate: param.rotate, fov: param.fov, style: param.style);
        }
        */
        public static void cam_goto_pos(VNNeoController game, CamData param)
        {
            game.anim_to_camera(param.duration, pos: param.position, distance: param.distance, rotate: param.rotation, fov: param.fov, style: param.style);
        }


        public static void cam_rotate(VNNeoController game, Vector3 param)
        {
            CamData camobj = game.get_camera_num(0);
            Vector3 v3 = camobj.rotation;
            camobj.rotation = new Vector3(v3.x + param.x, v3.y + param.y, v3.z + param.z);
            game.move_camera_obj(camobj);
        }

        /*
        public static void cam_rotate(VNNeoController game, (Vector3 vec, float duration) param)
        {
            CamData camobj = game.get_camera_num(0);
            Vector3 v3 = camobj.rotation;
            // param = ((rot_delta_x, rot_delta_y, rot_delta_z), duration)
            camobj.rotation = new Vector3(v3.x + param.vec.x, v3.y + param.vec.y, v3.z + param.vec.z);
            game.anim_to_camera_obj(param.duration, camobj);
        }

        public static void cam_rotate(VNNeoController game, (Vector3 vec, float duration, string style) param)
        {
            CamData camobj = game.get_camera_num(0);
            Vector3 v3 = camobj.rotation;
            // param = ((rot_delta_x, rot_delta_y, rot_delta_z), duration)
            camobj.rotation = new Vector3(v3.x + param.vec.x, v3.y + param.vec.y, v3.z + param.vec.z);
            game.anim_to_camera_obj(param.duration, camobj, style: param.style);
        }

        public static void cam_rotate(VNNeoController game, (Vector3 vec, float duration, string style, GameFunc onCameraEnd) param)
        {
            CamData camobj = game.get_camera_num(0);
            Vector3 v3 = camobj.rotation;
            // param = ((rot_delta_x, rot_delta_y, rot_delta_z), duration)
            camobj.rotation = new Vector3(v3.x + param.vec.x, v3.y + param.vec.y, v3.z + param.vec.z);
            game.anim_to_camera_obj(param.duration, camobj, style: param.style, param.onCameraEnd);
        }
        */

        public static void cam_rotate(VNNeoController game, CamData param)
        {
            CamData camobj = game.get_camera_num(0);
            Vector3 v3 = camobj.rotation;
            // param = ((rot_delta_x, rot_delta_y, rot_delta_z), duration)
            camobj.rotation = new Vector3(v3.x + param.rotation.x, v3.y + param.rotation.y, v3.z + param.rotation.z);
            game.anim_to_camera_obj(param.duration, camobj, style: param.style);

        }

        public static void cam_zoom(VNNeoController game, float zoom_delta)
        {
            var camobj = game.get_camera_num(0);
            var dv3 = camobj.distance;
            // param = zoom_delta, use positive value to zoom in, and negative value for zoom out 
            camobj.distance = new Vector3(dv3.x, dv3.y, dv3.z + zoom_delta);
            game.move_camera_obj(camobj);
        }

        /*

        public static void cam_zoom(VNNeoController game, (float zoom_delta, float duration) param)
        {
            var camobj = game.get_camera_num(0);
            var dv3 = camobj.distance;
            // param = zoom_delta, use positive value to zoom in, and negative value for zoom out 
            camobj.distance = new Vector3(dv3.x, dv3.y, dv3.z + param.zoom_delta);
            game.anim_to_camera_obj(param.duration, camobj);
        }

        public static void cam_zoom(VNNeoController game, (float zoom_delta, float duration, string style) param)
        {
            var camobj = game.get_camera_num(0);
            var dv3 = camobj.distance;
            // param = zoom_delta, use positive value to zoom in, and negative value for zoom out 
            camobj.distance = new Vector3(dv3.x, dv3.y, dv3.z + param.zoom_delta);
            game.anim_to_camera_obj(param.duration, camobj, param.style);
        }
        */

        public static void cam_zoom(VNNeoController game, CamData param)
        {
            var camobj = game.get_camera_num(0);
            var dv3 = camobj.distance;
            // param = zoom_delta, use positive value to zoom in, and negative value for zoom out 
            camobj.distance = new Vector3(dv3.x, dv3.y, dv3.z + param.zoom_delta);
            game.anim_to_camera_obj(param.duration, camobj, param.style);
        }

        /*
        public static void cam_zoom(VNNeoController game, (float zoom_delta, float duration, string style, GameFunc onCameraEnd) param)
        {
            var camobj = game.get_camera_num(0);
            var dv3 = camobj.distance;
            // param = zoom_delta, use positive value to zoom in, and negative value for zoom out 
            camobj.distance = new Vector3(dv3.x, dv3.y, dv3.z + param.zoom_delta);
            game.anim_to_camera_obj(param.duration, camobj, param.style, param.onCameraEnd);
        }
        */

        // :type game: vngameengine.VNNeoController
        public static void set_camera_name(VNNeoController game, string name)
        {
            // set the named camera as active camera, if name is None or not found, switch to default camera
            // if active an object camera, return true. Or return false if non object camera actived.
            foreach (var ociobj in game.studio.dicObjectCtrl.Values)
            {
                if (ociobj is OCICamera cam)
                {
                    if (cam.name == name)
                    {
                        if (game.studio.ociCamera != cam)
                        {
                            game.studio.ChangeCamera(cam);
                        }

                    }
                    return;
                }
            }
            game.studio.ChangeCamera(null);
        }

        // :type game: vngameengine.VNNeoController
        public static string get_camera_name(VNNeoController game)
        {
            // return the current active camera's name, or return None if no camera actived.
            if (game.studio.ociCamera != null)
            {
                return game.studio.ociCamera.name;
            }
            return null;
        }

        public struct CamActFunc
        {
            internal CamOperation func;
            internal bool active;

            public CamActFunc(CamOperation func, bool active)
            {
                this.func = func;
                this.active = active;
            }
        }

        public static Dictionary<string, CamActFunc> cam_act_funcs = new Dictionary<string, CamActFunc> {
        {
            "goto_preset",
            new CamActFunc(cam_goto_preset, false)
        },
        {
            "goto_pos",
            new CamActFunc(cam_goto_pos, true)
        },
        {
            "rotate",
            new CamActFunc(cam_rotate, false)},
        {
            "zoom",
            new CamActFunc(cam_zoom, false)}

        };

        /*
        public void cam_act_funcs(VNNeoController game, string func, CamData param)
        {
            switch (func)
            {
                case "goto_preset":
                    game.anim_to_camera_num(param.duration, param.camnum);
                    break;
                case "goto_pos":
                    game.anim_to_camera(param.duration, pos: param.position, distance: param.distance, rotate: param.rotation, fov: param.fov, style: param.style);
                    break;
                case "rotate":
                    cam_rotate(game, param.rotation);
                    break;
                case "zoom":
                    cam_goto_pos(game, param.zoom);
                    break;
                default:
                    break;
            }
        }
        */
    }
}
