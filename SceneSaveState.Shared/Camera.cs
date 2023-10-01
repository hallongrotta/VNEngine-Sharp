using System.Collections.Generic;
using HarmonyLib;
using MessagePack;
using Studio;
using UnityEngine;
using VNActor;
using VNEngine;
using static SceneSaveState.VNDataComponent;
using static VNEngine.VNController;
using NotImplementedException = System.NotImplementedException;

namespace SceneSaveState
{
    public class Camera
    {
     

        [MessagePackObject(true)]
        public class CamData : IDataClass<Camera>
        {
            public string Name { get; set; }

            public string TypeName => "Cam";

            public CamData Copy()
            {
                throw new NotImplementedException();
            }

            void IDataClass<Camera>.Apply(Camera item)
            {
                throw new NotImplementedException();
            }

            public VNData addata;
            public int camnum;
            public Vector3 distance;
            public float duration;
            public float fov;
            public Vector3 position;
            public Vector3 rotation;
            public string style;
            public float zoom_delta;

            public CamData(Vector3 position, Vector3 rotation, Vector3 distance, float fov)
            {
                camnum = -1; //TODO
                duration = -1;
                zoom_delta = -1;
                this.position = position;
                this.rotation = rotation;
                this.distance = distance;
                this.fov = fov;
                addata = new VNData();
                style = "";
            }

            public CamData(Vector3 position, Vector3 rotation, Vector3 distance, float fov, VNData addata)
            {
                camnum = -1; //TODO
                duration = -1;
                zoom_delta = -1;
                this.position = position;
                this.rotation = rotation;
                this.distance = distance;
                this.fov = fov;
                this.addata = addata;
                style = "";
            }

            public CamData()
            {
            }
        }

        internal int camAnimeTID;

        internal Dictionary<string, float> camAnimFullStyle;

        internal string camAnimStyle;

        internal Vector3 camSAngle;

        internal Vector3 camSDir;

        internal float camSFOV;

        internal Vector3 camSPos;

        internal Vector3 camTAngle;

        internal Vector3 camTDir;

        internal float camTFOV;

        internal Vector3 camTPos;

        protected Studio.Studio GameStudio
        {
            get
            {
                var studio = Studio.Studio.Instance;
                return studio;
            }
        }

        internal Studio.CameraControl.CameraData cameraData
        {
            get
            {
                var c = GameStudio.cameraCtrl;
                //var trav = Traverse.Create(c);
                // var cdata = trav.Field("cameraData").GetValue<Studio.CameraControl.CameraData>();
#if KKS || KK
                var cdata = Studio.Studio.Instance.m_CameraCtrl.cameraData;
#else
                var trav = Traverse.Create(c);
                var cdata = trav.Field("cameraData").GetValue<Studio.CameraControl.CameraData>();
#endif
                return cdata;
            }
        }


        internal CamData export() => new CamData(cameraData.pos, cameraData.rotate, cameraData.distance, cameraData.parse);


        internal void _anim_to_cam_upd(VNController game, float dt, float time, float duration)
        {
            var camProgress = time / duration;
            if (camAnimStyle == "linear") camProgress = time / duration;
            if (camAnimStyle == "slow-fast") camProgress = Mathf.Pow(camProgress, 2);
            if (camAnimStyle == "fast-slow") camProgress = 1 - Mathf.Pow(1 - camProgress, 2);
            if (camAnimStyle == "slow-fast3") camProgress = Mathf.Pow(camProgress, 3);
            if (camAnimStyle == "fast-slow3") camProgress = 1 - Mathf.Pow(1 - camProgress, 3);
            if (camAnimStyle == "slow-fast4") camProgress = Mathf.Pow(camProgress, 4);
            if (camAnimStyle == "fast-slow4") camProgress = 1 - Mathf.Pow(1 - camProgress, 4);
            var TPos = camTPos;
            var TDir = camTDir;
            var TAngle = camTAngle;
            if (camAnimFullStyle != null)
            {
                if (camAnimFullStyle.ContainsKey("target_camera_zooming_in"))
                    TDir = new Vector3(TDir.x, TDir.y,
                        TDir.z - camAnimFullStyle["target_camera_zooming_in"] * (1 - time / duration));
                if (camAnimFullStyle.ContainsKey("target_camera_rotating_z"))
                    TAngle = new Vector3(TAngle.x, TAngle.y,
                        TAngle.z + camAnimFullStyle["target_camera_rotating_z"] * (1 - time / duration));
                if (camAnimFullStyle.ContainsKey("target_camera_rotating_x"))
                    TAngle = new Vector3(
                        TAngle.x + camAnimFullStyle["target_camera_rotating_x"] * (1 - time / duration), TAngle.y,
                        TAngle.z);
                if (camAnimFullStyle.ContainsKey("target_camera_rotating_y"))
                    TAngle = new Vector3(TAngle.x,
                        TAngle.y + camAnimFullStyle["target_camera_rotating_y"] * (1 - time / duration), TAngle.z);
                if (camAnimFullStyle.ContainsKey("target_camera_posing_y"))
                    TPos = new Vector3(TPos.x,
                        TPos.y + camAnimFullStyle["target_camera_posing_y"] * (1 - time / duration), TPos.z);
                // TDir.z = TDir.z + self.camAnimFullStyle["move_distance"] * time / duration
                // TDir.z = TDir.z + (-20)
                // print "z: %s"%(str(TDir.z))
            }

            var pos = Vector3.Lerp(camSPos, TPos, camProgress);
            var distance = Vector3.Lerp(camSDir, TDir, camProgress);
            var rotate = Vector3.Slerp(camSAngle, TAngle, camProgress);
            var fov = Mathf.Lerp(camSFOV, camTFOV, camProgress);
            //print fov, self.camSFOV, self.camTFOV, camProgress
            move_camera_direct(pos, distance, rotate, fov);
        }



        internal void to_camera(int camnum)
        {
            var si = GameStudio.sceneInfo;
            var cdatas = si.cameraData;

            var cdata = cameraData;
            //var targetInfos = trav.Field("listBones");

            //CameraData cdata = c.cameraData;
            cdata.Copy(cdatas[camnum - 1]);
        }

        internal void Apply(CamData camData)
        {
            var cdata = cameraData;
            var c = GameStudio.cameraCtrl;

            cdata.pos = camData.position;

            cdata.distance = camData.distance;

            cdata.rotate = camData.rotation;

            cdata.parse = camData.fov;
            c.fieldOfView = camData.fov;
        }

        internal void move_camera_direct(Vector3 pos, Vector3 distance, Vector3 rotate, float fov)
        {
            var cd = new CamData(pos, rotate, distance, fov);
            Apply(cd);
        }

        internal string CameraName
        {
            get
            {
                // return the current active camera's name, or return None if no camera actived.
                if (GameStudio.ociCamera != null) return GameStudio.ociCamera.name;
                return null;
            }
            set
            {
                // set the named camera as active camera, if name is None or not found, switch to default camera
                // if active an object camera, return true. Or return false if non object camera actived.
                foreach (var ociobj in GameStudio.dicObjectCtrl.Values)
                    if (ociobj is OCICamera cam)
                    {
                        if (cam.name == value)
                            if (GameStudio.ociCamera != cam)
                                GameStudio.ChangeCamera(cam);
                        return;
                    }

                GameStudio.ChangeCamera(null);
            }
        }

        internal CamData get_camera_num(int camnum)
        {
            Studio.CameraControl.CameraData cdata;
            var studio = this.GameStudio;
            var si = studio.sceneInfo;
            var cdatas = si.cameraData;
            if (camnum == 0)
            {
                // 0 camera is current camera. It may be interested due to some reasons
                var c = studio.cameraCtrl;
                cdata = cameraData;
            }
            else
            {
                cdata = cdatas[camnum - 1];
            }

            var camobj = new CamData(cdata.pos, cdata.rotate, cdata.distance, cdata.parse);
            //print camobj
            return camobj;
        }

        internal void _anim_to_camera_savecurrentpos()
        {
            var camobj = get_camera_num(0);
            camSPos = camobj.position;
            camSDir = camobj.distance;
            camSAngle = camobj.rotation;
            camSFOV = camobj.fov;
        }

        internal void anim_to_camera_num(float duration, int camnum, string style = "linear", GameFunc onCameraEnd = null)
        {
            anim_to_camera_obj(duration, get_camera_num(camnum), style, onCameraEnd);
        }

        internal void anim_to_camera(
            float duration,
            Vector3 pos = new Vector3(),
            Vector3 distance = new Vector3(),
            Vector3 rotate = new Vector3(),
            float fov = 23.0f,
            string style = "linear",
            GameFunc onCameraEnd = null)
        {
            var camobj = new CamData(pos, rotate, distance, fov);
            anim_to_camera_obj(duration, camobj, style, onCameraEnd);
        }



        internal void cam_rotate(CamData param)
        {
            var camobj = get_camera_num(0);
            var v3 = camobj.rotation;
            // param = ((rot_delta_x, rot_delta_y, rot_delta_z), duration)
            camobj.rotation = new Vector3(v3.x + param.rotation.x, v3.y + param.rotation.y, v3.z + param.rotation.z);
            anim_to_camera_obj(param.duration, camobj, param.style);
        }

        internal void cam_zoom(CamData param)
        {
            var camobj = get_camera_num(0);
            var dv3 = camobj.distance;
            // param = zoom_delta, use positive value to zoom in, and negative value for zoom out 
            camobj.distance = new Vector3(dv3.x, dv3.y, dv3.z + param.zoom_delta);
            anim_to_camera_obj(param.duration, camobj, param.style);
        }

        internal void animation_cam_timer(float duration, GameFunc onCameraEnd)
        {
            // camera animation one timer only
            if (camAnimeTID != -1) VNController.Instance.clear_timer(camAnimeTID);
            camAnimeTID = VNController.Instance.set_timer(duration, _anim_to_cam_end, _anim_to_cam_upd);
            VNController.Instance._onCameraEnd = onCameraEnd;
            if (VNController.Instance.isHideWindowDuringCameraAnimation) VNController.Instance.visible = false;
        }

        internal void _anim_to_cam_end(VNController controller)
        {
            if (controller.isHideWindowDuringCameraAnimation) controller.visible = false;
            camAnimeTID = -1;
            if (controller._onCameraEnd != null) controller.call_game_func(controller._onCameraEnd);
        }

        internal void anim_to_camera_obj(float duration, CamData camobj, string style = "linear",
    GameFunc onCameraEnd = null)
        {
            _anim_to_camera_savecurrentpos();
            // print "Anim to cam 1"
            // print "Anim to cam 2"
            var camobjv = camobj;
            camTPos = camobjv.position;
            camTDir = camobjv.distance;
            camTAngle = camobjv.rotation;
            camTFOV = camobjv.fov;
            camAnimStyle = style;
            camAnimFullStyle = null;
            // camera animation one timer only
            animation_cam_timer(duration, onCameraEnd);
        }

        internal void move_camera(CamData cam)
        {
            Apply(cam);
        }

        internal void cam_zoom(float zoom_delta)
        {
            var camobj = get_camera_num(0);
            var dv3 = camobj.distance;
            // param = zoom_delta, use positive value to zoom in, and negative value for zoom out 
            camobj.distance = new Vector3(dv3.x, dv3.y, dv3.z + zoom_delta);
            move_camera_obj(camobj);
        }

        internal void move_camera(Vector3 pos, Vector3 distance, Vector3 rotate, float fov = 23.0f)
        {
            //self.show_blocking_message_time("ERROR: move_camera was not implemented")
            var camobj = new CamData(pos, rotate, distance, fov);
            move_camera_obj(camobj);
        }

        internal void move_camera_obj(CamData camobj)
        {
            Apply(camobj);
        }

        internal void cam_goto_pos(CamData param)
        {
            anim_to_camera(param.duration, param.position, param.distance, param.rotation, param.fov, param.style);
        }

        internal void cam_rotate(Vector3 param)
        {
            var camobj = get_camera_num(0);
            var v3 = camobj.rotation;
            camobj.rotation = new Vector3(v3.x + param.x, v3.y + param.y, v3.z + param.z);
            move_camera_obj(camobj);
        }

        internal void cam_goto_preset(VNController game, CamData param)
        {
            // param = (set, duration)
            anim_to_camera_num(param.duration, param.camnum);
        }

        internal void cam_goto_preset(StudioController game, int param)
        {
            to_camera(param);
        }


        /*
        internal delegate void CamOperation(StudioController game, CamData param);

        internal struct CamActFunc
        {
            internal CamOperation func;
            internal bool active;

            public CamActFunc(CamOperation func, bool active)
            {
                this.func = func;
                this.active = active;
            }
        }
        */

        /*
        public void cam_act_funcs(StudioController game, string func, CamData param)
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

        /*
        public static Dictionary<string, CamActFunc> cam_act_funcs = new Dictionary<string, CamActFunc>
        {
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
                new CamActFunc(cam_rotate, false)
            },
            {
                "zoom",
                new CamActFunc(cam_zoom, false)
            }
        };
        */



        /*
        public static void cam_goto_preset(StudioController game, (int camnum, int duration, string style) param)
        {
            game.anim_to_camera_num(param.duration, param.camnum, param.style);
        }


        public static void cam_goto_preset(StudioController game, (int camnum, int duration, string style, GameFunc onCamEnd) param)
        {
            game.anim_to_camera_num(param.duration, param.camnum, param.style, param.onCamEnd);
        }
        */



        /*
        public static void cam_goto_pos(StudioController game, (Vector3 pos, Vector3 distance, Vector3 rotate) param)
        {
            game.move_camera(pos: param.pos, distance: param.distance, rotate: param.rotate);
        }

        public static void cam_goto_pos(StudioController game, (Vector3 pos, Vector3 distance, Vector3 rotate, float duration, string style) param)
        {
            game.anim_to_camera(param.duration, pos: param.pos, distance: param.distance, rotate: param.rotate, style: param.style);
        }

        public static void cam_goto_pos(StudioController game, (Vector3 pos, Vector3 distance, Vector3 rotate, float duration) param)
        {
            game.anim_to_camera(param.duration, pos: param.pos, distance: param.distance, rotate: param.rotate);
        }

        public static void cam_goto_pos(StudioController game, (Vector3 pos, Vector3 distance, Vector3 rotate, float duration, string style, float fov) param)
        {
            game.anim_to_camera(param.duration, pos: param.pos, distance: param.distance, rotate: param.rotate, fov: param.fov, style: param.style);
        }
        */


        /*
        public static void cam_rotate(StudioController game, (Vector3 vec, float duration) param)
        {
            CamData camobj = game.get_camera_num(0);
            Vector3 v3 = camobj.rotation;
            // param = ((rot_delta_x, rot_delta_y, rot_delta_z), duration)
            camobj.rotation = new Vector3(v3.x + param.vec.x, v3.y + param.vec.y, v3.z + param.vec.z);
            game.anim_to_camera_obj(param.duration, camobj);
        }

        public static void cam_rotate(StudioController game, (Vector3 vec, float duration, string style) param)
        {
            CamData camobj = game.get_camera_num(0);
            Vector3 v3 = camobj.rotation;
            // param = ((rot_delta_x, rot_delta_y, rot_delta_z), duration)
            camobj.rotation = new Vector3(v3.x + param.vec.x, v3.y + param.vec.y, v3.z + param.vec.z);
            game.anim_to_camera_obj(param.duration, camobj, style: param.style);
        }

        public static void cam_rotate(StudioController game, (Vector3 vec, float duration, string style, GameFunc onCameraEnd) param)
        {
            CamData camobj = game.get_camera_num(0);
            Vector3 v3 = camobj.rotation;
            // param = ((rot_delta_x, rot_delta_y, rot_delta_z), duration)
            camobj.rotation = new Vector3(v3.x + param.vec.x, v3.y + param.vec.y, v3.z + param.vec.z);
            game.anim_to_camera_obj(param.duration, camobj, style: param.style, param.onCameraEnd);
        }
        */



        /*

        public static void cam_zoom(StudioController game, (float zoom_delta, float duration) param)
        {
            var camobj = game.get_camera_num(0);
            var dv3 = camobj.distance;
            // param = zoom_delta, use positive value to zoom in, and negative value for zoom out 
            camobj.distance = new Vector3(dv3.x, dv3.y, dv3.z + param.zoom_delta);
            game.anim_to_camera_obj(param.duration, camobj);
        }

        public static void cam_zoom(StudioController game, (float zoom_delta, float duration, string style) param)
        {
            var camobj = game.get_camera_num(0);
            var dv3 = camobj.distance;
            // param = zoom_delta, use positive value to zoom in, and negative value for zoom out 
            camobj.distance = new Vector3(dv3.x, dv3.y, dv3.z + param.zoom_delta);
            game.anim_to_camera_obj(param.duration, camobj, param.style);
        }
        */

        /*
        public static void cam_zoom(StudioController game, (float zoom_delta, float duration, string style, GameFunc onCameraEnd) param)
        {
            var camobj = game.get_camera_num(0);
            var dv3 = camobj.distance;
            // param = zoom_delta, use positive value to zoom in, and negative value for zoom out 
            camobj.distance = new Vector3(dv3.x, dv3.y, dv3.z + param.zoom_delta);
            game.anim_to_camera_obj(param.duration, camobj, param.style, param.onCameraEnd);
        }
        */

        // :type game: vngameengine.StudioController
    }
}