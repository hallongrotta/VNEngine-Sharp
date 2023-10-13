
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
        internal const string defaultSpeakerAlias = "s";

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
        private VNData viewVNData;


        public View(CamData camData)
        {
            this.camData = camData;
            paramAnimCamDuration = 1.5f;
            paramAnimCamStyle = "fast-slow";
            paramAnimCamZoomOut = 0.0f;


            viewVNData = new VNData
            {
                enabled = false,
                whosay = "",
                whatsay = "",
                addvncmds = "",
                addprops = new addprops_struct()
            };

            viewVNData.addprops.a1 = false;
            viewVNData.addprops.a2 = false;

            Add(viewVNData);

            viewVNData = Current;

            // Transfer old data
            if (camData != null && camData.addata.enabled)
            { 
                Update(camData.addata);
            }

        }

        public View(float paramAnimCamDuration, string paramAnimCamStyle, float paramAnimCamZoomOut, VNData viewVNData, CamData camData, List<VNData> vNDatas)
        {
            this.paramAnimCamDuration = paramAnimCamDuration;
            this.paramAnimCamStyle = paramAnimCamStyle;
            this.paramAnimCamZoomOut = paramAnimCamZoomOut;
            this.viewVNData = viewVNData;
            this.camData = camData;
            this.vNDatas = vNDatas;
        }

        public View Copy()
        {
            throw new NotImplementedException();
        }

        internal void setCamera(Camera c, VNController gameController)
        {
            setCamera(c, gameController, false);
        }

        internal void setCamera(Camera c, VNController gameController, bool isAnimated)
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
            }
            else
            {
                ResetVNData();
            }
        }

        internal void DrawVNDataOptions(RoleTracker roleTracker)
        {
            viewVNData.enabled = true;
            if (!viewVNData.enabled) return;
            GUILayout.BeginHorizontal();
            GUILayout.Label("Who say:", GUILayout.Width(90));
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("<", GUILayout.Width(20)))
            {
                viewVNData.whosay = get_next_speaker(roleTracker.CharacterRoles, viewVNData.whosay, false);
            }
            if (GUILayout.Button(">", GUILayout.Width(20)))
            {
                viewVNData.whosay = get_next_speaker(roleTracker.CharacterRoles, viewVNData.whosay, true);
            }
            GUILayout.EndHorizontal();
            viewVNData.whosay = GUILayout.TextField(viewVNData.whosay);

            GUILayout.BeginHorizontal();
            GUILayout.Label("What say:", GUILayout.Width(90));
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("X", GUILayout.Width(20)))
            {
                viewVNData.whatsay = "";
            }
            if (GUILayout.Button("...", GUILayout.Width(20)))
            {
                viewVNData.whatsay = "...";
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginVertical();
            viewVNData.whatsay = GUILayout.TextArea(viewVNData.whatsay, GUILayout.Height(85));
            if (GUILayout.Button("Save", GUILayout.Width(40)))
            {
                Update(viewVNData);
            }
            GUILayout.EndVertical();

            /*GUILayout.BeginHorizontal();
                GUILayout.Label("  Adv VN cmds", GUILayout.Width(90));
                Instance.currentVNData.addvncmds = GUILayout.TextArea(Instance.currentVNData.addvncmds, GUILayout.Width(235), GUILayout.Height(55));
                if (GUILayout.Button("X", GUILayout.Width(20)))
                {
                    Instance.currentVNData.addvncmds = "";
                }
                // if GUILayout.Button("X", GUILayout.Width(20)):
                //     sc.cam_whatsay = ""
                // if GUILayout.Button("...", GUILayout.Width(20)):
                //     sc.cam_whatsay = "..."
                GUILayout.EndHorizontal();
                */
        }

        internal string get_next_speaker(Dictionary<string, int> allActors, string curSpeakAlias, bool next)
        {
            // next from unknown speaker
            var keylist = allActors.Keys.ToList();
            if (curSpeakAlias != defaultSpeakerAlias && !allActors.ContainsKey(curSpeakAlias))
                return defaultSpeakerAlias;
            // next from s or actor
            if (curSpeakAlias == defaultSpeakerAlias)
            {
                if (allActors.Count > 0)
                {
                    if (next)
                        return keylist[0];
                    return keylist.Last();
                }

                return defaultSpeakerAlias;
            }

            var nextIndex = keylist.IndexOf(curSpeakAlias);
            if (next)
                nextIndex += 1;
            else
                nextIndex -= 1;
            return Enumerable.Range(0, allActors.Count).Contains(nextIndex) ? keylist[nextIndex] : defaultSpeakerAlias;
        }

        internal void SetVNData(VNController gameController, VNData vnData)
        {
            gameController.SetText(vnData.whosay, vnData.whatsay);
            viewVNData = vnData;
        }

        internal void ResetVNData()
        {
            viewVNData.enabled = false;
            viewVNData.whosay = "";
            viewVNData.whatsay = "";
            viewVNData.addvncmds = "";
            viewVNData.addprops.a1 = false;
            viewVNData.addprops.a2 = false;
        }

    }
}
