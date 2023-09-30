
using System;
using System.Collections.Generic;
using System.Text;
using static SceneSaveState.Camera;
using static SceneSaveState.VNDataComponent;

namespace SceneSaveState
{
    public class View: Manager<VNData>, IManaged<View>
    {
        internal CamData camData;
        private float paramAnimCamDuration;
        private string paramAnimCamStyle;
        private float paramAnimCamZoomOut;

        public string Name { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public string TypeName => throw new NotImplementedException();

        public View(CamData camData)
        {
            this.camData = camData;
            paramAnimCamDuration = 1.5f;
            paramAnimCamStyle = "fast-slow";
            paramAnimCamZoomOut = 0.0f;
        }

        public View Copy()
        {
            throw new NotImplementedException();
        }

        internal void setCamera(Camera c, bool animCam)
        {
            setCamera(c, animCam);
        }

        internal void setCamera(Camera c)
        {
            setCamera(c, false);
        }

        internal void setCamera(Camera c, SceneConsoleVNComponent vnComponent, bool isAnimated)
        {
            // check and run adv command
            var keepCamera = false;
            if (camData.addata.enabled)
            {
                //keepCamera = VNExt.runAdvVNSS(this, camera_data.addata); TODO
            }

            // actual set
            if (keepCamera)
            {
            }
            else if (isAnimated)
            {
                // self.game.anim_to_camera(1.5, pos=camera_data[0], distance=camera_data[1], rotate=camera_data[2], fov=camera_data[3], style={'style': "fast-slow",'target_camera_zooming_in': 2})
                /*var style = new Dictionary<string, object> {
                    {
                        "style",
                        "fast-slow"}};
                if (this.paramAnimCamZoomOut != 0.0)
                {
                    style["target_camera_zooming_in"] = this.paramAnimCamZoomOut;
                } */ //TODO fix this
                var style = "linear";
                c.anim_to_camera(paramAnimCamDuration, camData.position, camData.distance,
                    camData.rotation, camData.fov, style);
            }
            else
            {
                c.move_camera(camData);
                //this.game.move_camera(pos: camera_data.position, distance: camera_data.distance, rotate: camera_data.rotation, fov: camera_data.fov);
            }

            if (camData.addata is VNData addata)
            {
                vnComponent.SetVNData(addata);
            }
            else
            {
                vnComponent.ResetVNData();
            }
        }

    }
}
