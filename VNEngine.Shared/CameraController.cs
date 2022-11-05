using HarmonyLib;
using Studio;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static VNEngine.VNCamera;
using static VNEngine.VNController;
using BepInEx;

namespace VNEngine
{
    public class CameraController
    {


        public int camAnimeTID;

        public Dictionary<string, float> camAnimFullStyle;

        public string camAnimStyle;

        public Vector3 camSAngle;

        public Vector3 camSDir;

        public float camSFOV;

        public Vector3 camSPos;

        public Vector3 camTAngle;

        public Vector3 camTDir;

        public float camTFOV;

        public Vector3 camTPos;

        public static CameraController Instance { get; private set; }
          
        public CameraController()
        {
            Instance = this;
        }

        protected Studio.Studio GameStudio
        {
            get
            {
                var studio = Studio.Studio.Instance;
                return studio;
            }
        }

        public Studio.CameraControl.CameraData cameraData
        {
            get
            {
                var c = GameStudio.cameraCtrl;
                var trav = Traverse.Create(c);
                var cdata = trav.Field("cameraData").GetValue<Studio.CameraControl.CameraData>();
                return cdata;
            }
        }



        public void _anim_to_cam_upd(VNController game, float dt, float time, float duration)
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



        public void to_camera(int camnum)
        {
            var si = GameStudio.sceneInfo;
            var cdatas = si.cameraData;

            var cdata = cameraData;
            //var targetInfos = trav.Field("listBones");

            //CameraData cdata = c.cameraData;
            cdata.Copy(cdatas[camnum - 1]);
        }

        public void move_camera_direct(CamData cam)
        {
            var cdata = cameraData;
            var c = GameStudio.cameraCtrl;

            cdata.pos = cam.position;

            cdata.distance = cam.distance;

            cdata.rotate = cam.rotation;

            cdata.parse = cam.fov;
        }

        public void move_camera_direct(Vector3 pos, Vector3 distance, Vector3 rotate, float fov)
        {
            var cd = new CamData(pos, rotate, distance, fov);
            move_camera_direct(cd);
        }

        public string CameraName
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

        public CamData get_camera_num(int camnum)
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

        public void _anim_to_camera_savecurrentpos()
        {
            var camobj = get_camera_num(0);
            camSPos = camobj.position;
            camSDir = camobj.distance;
            camSAngle = camobj.rotation;
            camSFOV = camobj.fov;
        }

        public void anim_to_camera_num(float duration, int camnum, string style = "linear", GameFunc onCameraEnd = null)
        {
            anim_to_camera_obj(duration, get_camera_num(camnum), style, onCameraEnd);
        }

        public void anim_to_camera(
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



        public void cam_rotate(CamData param)
        {
            var camobj = get_camera_num(0);
            var v3 = camobj.rotation;
            // param = ((rot_delta_x, rot_delta_y, rot_delta_z), duration)
            camobj.rotation = new Vector3(v3.x + param.rotation.x, v3.y + param.rotation.y, v3.z + param.rotation.z);
            anim_to_camera_obj(param.duration, camobj, param.style);
        }

        public void cam_zoom(CamData param)
        {
            var camobj = get_camera_num(0);
            var dv3 = camobj.distance;
            // param = zoom_delta, use positive value to zoom in, and negative value for zoom out 
            camobj.distance = new Vector3(dv3.x, dv3.y, dv3.z + param.zoom_delta);
            anim_to_camera_obj(param.duration, camobj, param.style);
        }

        public void animation_cam_timer(float duration, GameFunc onCameraEnd)
        {
            // camera animation one timer only
            if (camAnimeTID != -1) VNController.Instance.clear_timer(camAnimeTID);
            camAnimeTID = VNController.Instance.set_timer(duration, _anim_to_cam_end, _anim_to_cam_upd);
            VNController.Instance._onCameraEnd = onCameraEnd;
            if (VNController.Instance.isHideWindowDuringCameraAnimation) VNController.Instance.visible = false;
        }

        public static void _anim_to_cam_end(VNController controller)
        {
            if (controller.isHideWindowDuringCameraAnimation) controller.visible = false;
            controller.cameraController.camAnimeTID = -1;
            if (controller._onCameraEnd != null) controller.call_game_func(controller._onCameraEnd);
        }

        public void anim_to_camera_obj(float duration, CamData camobj, string style = "linear",
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

        public void move_camera(CamData cam)
        {
            move_camera_direct(cam);
        }

        public void cam_zoom(float zoom_delta)
        {
            var camobj = get_camera_num(0);
            var dv3 = camobj.distance;
            // param = zoom_delta, use positive value to zoom in, and negative value for zoom out 
            camobj.distance = new Vector3(dv3.x, dv3.y, dv3.z + zoom_delta);
            move_camera_obj(camobj); 
        }

        public void move_camera(Vector3 pos, Vector3 distance, Vector3 rotate, float fov = 23.0f)
        {
            //self.show_blocking_message_time("ERROR: move_camera was not implemented")
            var camobj = new CamData(pos, rotate, distance, fov);
            move_camera_obj(camobj);
        }

        public void move_camera_obj(CamData camobj)
        {
            move_camera_direct(camobj);
        }

        public void cam_goto_pos(CamData param)
        {
            anim_to_camera(param.duration, param.position, param.distance, param.rotation, param.fov, param.style);
        }

        public void cam_rotate(Vector3 param)
        {
            var camobj = get_camera_num(0);
            var v3 = camobj.rotation;
            camobj.rotation = new Vector3(v3.x + param.x, v3.y + param.y, v3.z + param.z);
            move_camera_obj(camobj);
        }

        public void cam_goto_preset(VNController game, CamData param)
        {
            // param = (set, duration)
            anim_to_camera_num(param.duration, param.camnum);
        }

        public void cam_goto_preset(StudioController game, int param)
        {
           to_camera(param);
        }

    }
}
