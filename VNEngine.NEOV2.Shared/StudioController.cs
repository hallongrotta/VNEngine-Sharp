using System;
using System.Collections.Generic;
using System.IO;
using BepInEx;
using Studio;
using UnityEngine;
using static VNEngine.Utils;

namespace VNEngine
{
    [BepInProcess(Constants.StudioProcessName)]
    //[BepInDependency(GUID)]
    [BepInPlugin(GUID, PluginName, Version)]
    public partial class StudioController
        : VNNeoController
    {
        public StudioController()
        {
            if (Instance != null) throw new InvalidOperationException("Can only create one instance of Controller");
            Instance = this;
        }

        public StudioController(List<Button_s> vnButtonsStart)
        {
            _vnButtons = vnButtonsStart;
        }

        public new static StudioController Instance { get; private set; }

        public string FrameFile
        {
            get => studio_scene.frame;
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
                var sceneInfo = studio_scene;
                return new System.ColorCorrection
                {
                    no = sceneInfo.cgLookupTexture, blend = sceneInfo.cgBlend, contrast = sceneInfo.cgContrast,
                    brightness = sceneInfo.cgBrightness, saturation = sceneInfo.cgSaturation
                };
            }
            set
            {
                studio_scene.cgLookupTexture = value.no;
                studio_scene.cgBlend = value.blend;
                studio_scene.cgBrightness = value.brightness;
                studio_scene.cgContrast = value.contrast;
                studio_scene.cgSaturation = value.saturation;
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
            get => studio_scene.mapInfo.ca.rot;
            set => studio_scene.mapInfo.ca.rot = value;
        }

        public Vector3 MapPos
        {
            get => studio_scene.mapInfo.ca.pos;
            set => studio_scene.mapInfo.ca.pos = value;
        }

        public int MapNumber
        {
            set
            {
                if (value != studio_scene.mapInfo.no) studio.AddMap(value);
            }
            get => studio_scene.mapInfo.no;
        }

        public bool MapLight
        {
            set => studio_scene.mapInfo.light = value;
            get => studio_scene.mapInfo.light;
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
            get => studio_scene.mapInfo.option;
        }

        public override string SceneDir()
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
