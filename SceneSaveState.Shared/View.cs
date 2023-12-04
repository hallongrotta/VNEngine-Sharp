
using MessagePack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using VNEngine;
using static SceneSaveState.Camera;
using static SceneSaveState.VNDataComponent;
using static SceneSaveState.VNDataComponent.VNData;

namespace SceneSaveState
{
    [MessagePackObject]
    public class View: Manager<VNData>, IManaged<View>
    {


        [Key("paramAnimCamDuration")]
        public float paramAnimCamDuration;
        [Key("paramAnimCamStyle")]
        public string paramAnimCamStyle;
        [Key("paramAnimCamZoomOut")]
        public float paramAnimCamZoomOut;
        [Key("camData")]
        public CamData camData;
        [Key("vnData")]
        public List<VNData> vNDatas { get => Items; private set => ImportItems(value); }
        [Key("Name")]
        public string Name { get => null; set { } }

        [IgnoreMember]
        public string TypeName => "Cam";


        internal View(CamData camData)
        {
            this.camData = camData;
            paramAnimCamDuration = 1.5f;
            paramAnimCamStyle = "fast-slow";
            paramAnimCamZoomOut = 0.0f;

            // Transfer old data
            if (camData != null && camData.addata.enabled)
            { 
                Update(camData.addata);
            }

        }

        internal View(CamData camData, VNData vnData) : this(camData)
        {
            Add(vnData);
        }

        public View(List<VNData> vnData, string Name, float paramAnimCamDuration, string paramAnimCamStyle, float paramAnimCamZoomOut, CamData camData) : this(camData)
        {
            this.paramAnimCamDuration = paramAnimCamDuration;
            this.paramAnimCamStyle = paramAnimCamStyle;
            this.paramAnimCamZoomOut = paramAnimCamZoomOut;
            this.camData = camData;
            this.vNDatas = vnData;
            this.Name = Name;
        }

        public View Copy()
        {
            throw new NotImplementedException();
        }

        internal VNData? setCamera(Camera c, VNController gameController)
        {
            return setCamera(c, gameController, false);
        }

        internal VNData? setCamera(Camera c, VNController gameController, bool isAnimated)
        {
         
            if (isAnimated)
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

            if (HasItems)
            {
                SetVNData(gameController, Current);
                return Current;
            }
            else return null;
        }

        internal void SetVNData(VNController gameController, VNData vnData)
        {
            gameController.SetText(vnData.whosay, vnData.whatsay);
        }

        

    }
}
