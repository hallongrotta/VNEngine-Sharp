using System;
using System.IO;
using BepInEx;
using Studio;
using UnityEngine;
using static VNEngine.Utils;

namespace VNEngine
{


    public partial class StudioController
    {
        public StudioController()
        {
            if (Instance != null) throw new InvalidOperationException("Can only create one instance of Controller");
            Instance = this;
        }

        public static StudioController Instance { get; private set; }

        public string FrameFile
        {
            get => StudioScene.frame;
            set
            {
                var obj = FindObjectOfType<FrameCtrl>();
                obj.Load(value);
            }
        }

        public System.ColorCorrection colorCorrection
        {
            get
            {
                var sceneInfo = StudioScene;
                return new System.ColorCorrection
                {
                    no = sceneInfo.cgLookupTexture, blend = sceneInfo.cgBlend, contrast = sceneInfo.cgContrast,
                    brightness = sceneInfo.cgBrightness, saturation = sceneInfo.cgSaturation
                };
            }
            set
            {
                StudioScene.cgLookupTexture = value.no;
                StudioScene.cgBlend = value.blend;
                StudioScene.cgBrightness = value.brightness;
                StudioScene.cgContrast = value.contrast;
                StudioScene.cgSaturation = value.saturation;
                Studio.Studio.Instance.systemButtonCtrl.UpdateInfo();
            }
        }

        public string Frame
        {
            // set frame png, param = png file name, CharaStudio only
            set
            {
                var pngName = value.Trim();
                if (pngName != "")
                {
                    if (!pngName.ToLower().EndsWith(".png")) pngName += ".png";
                    // load png in game scene folder if existed
                    var pngInScene = combine_path(SceneDir(), sceneDir, pngName);
                    if (File.Exists(pngInScene))
                    {
                        var pngRevPath = combine_path("..", "studio", "scene", sceneDir, pngName);
                        FrameFile = pngRevPath;
                        return;
                    }

                    // load png in game default background folder if existed
                    var pngInDefault =
                        Path.GetFullPath(combine_path(Application.dataPath, "..", "UserData", "frame", pngName));
                    if (File.Exists(pngInDefault))
                    {
                        FrameFile = pngName;
                        return;
                    }
                }

                // remove if param == "" or file not existed
                FrameFile = "";
            }
        }

        public Vector3 MapRot
        {
            get => StudioScene.mapInfo.ca.rot;
            set => StudioScene.mapInfo.ca.rot = value;
        }

        public Vector3 MapPos
        {
            get => StudioScene.mapInfo.ca.pos;
            set => StudioScene.mapInfo.ca.pos = value;
        }

        public int MapNumber
        {
            set
            {
                if (value != StudioScene.mapInfo.no) studio.AddMap(value);
            }
            get => StudioScene.mapInfo.no;
        }

        public bool MapLight
        {
            set => StudioScene.mapInfo.light = value;
            get => StudioScene.mapInfo.light;
        }

        public bool MapOption
        {
            set
            {
                Studio.Map map;
                // set map option visible: param = 1/0
                map = Studio.Map.Instance;
                map.VisibleOption = value;
            }
            get => StudioScene.mapInfo.option;
        }

        public string SceneDir()
        {
            // return path to "scene" folder
            return Path.GetFullPath(Path.Combine(Application.dataPath, "..", "UserData", "Studio", "scene"));
        }

        public System.SystemData export_full_status()
        {
            return new System.SystemData(this);
        }

        public void Apply(System.SystemData data, bool change_map = true)
        {
            BGM = data.bgm;

            if (!(data.wav is null)) WAV = (System.Wav_s) data.wav;

            if (change_map) MapNumber = data.map;

            MapPos = data.map_pos;
            MapRot = data.map_rot;
            MapOption = data.map_opt;
            BackgroundImage = data.bg_png;
            Frame = data.fm_png;
            MapLight = data.map_light;
            CharLight = data.char_light;
            if (!colorCorrection.Equals(data.colorCorrection)) colorCorrection = data.colorCorrection;
        }
    }
}
