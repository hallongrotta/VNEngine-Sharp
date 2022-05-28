using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BepInEx;
using BepInEx.Configuration;
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
            if (Instance != null) throw new InvalidOperationException("Can only create one instance of Controller");
            _vnButtons = vnButtonsStart;
            Instance = this;
        }

        public new static StudioController Instance { get; private set; }

        public static ConfigEntry<KeyboardShortcut> VNControllerHotkey { get; private set; }

        public string FrameFile
        {
            get => studio_scene.frame;
            set
            {
                var obj = FindObjectOfType<FrameCtrl>();
                obj.Load(value);
            }
        }

        public int MapNumber
        {
            set => StartCoroutine(AddMapCoroutine(value));


            get => studio_scene.map;
        }

        internal System.Ace Ace
        {
            get => new System.Ace {no = studio_scene.aceNo, blend = studio_scene.aceBlend};
            set
            {
                studio_scene.aceNo = value.no;
                studio_scene.aceBlend = value.blend;
                Studio.Studio.Instance.systemButtonCtrl.UpdateInfo();
            }
        }

        public bool MapOption
        {
            set
            {
                Map map;
                // set map option visible: param = 1/0
                map = Map.Instance;
                map.visibleOption = value;
            }
            get => studio_scene.mapOption;
        }

        public Vector3 MapRot
        {
            set
            {
                if (studio_scene.caMap.rot == value) return;
                studio_scene.caMap.rot = value;
                MapCtrl.Instance.Reflect();
            }
           
            get => studio_scene.caMap.rot;
        }

        public Vector3 MapPos
        {
            set
            {
                if (studio_scene.caMap.pos == value) return;
                studio_scene.caMap.pos = value;
                MapCtrl.Instance.Reflect();
            }
            get => studio_scene.caMap.pos;
        }

    public int Sun
        {
            set
            {
                var map = Map.Instance;
                if ((int)map.sunType == value) return;
                map.sunType = (SunLightInfo.Info.Type)value;
            }
            get => studio_scene.sunLightType;
        }


        public string Frame
        {
            set
            {
                var pngName = value;
                if (pngName is null)
                {
                    pngName = "";
                }
                else
                {
                    // try to load png in "frame" folder
                    var path = combine_path(Application.dataPath, "..", "UserData", "frame", pngName);
                    var pngInDefault = Path.GetFullPath(path);
                    if (!File.Exists(pngInDefault)) pngName = "";
                }

                FrameFile = pngName;
            }
        }

        internal void Main()
        {
            //Logger = base.Logger;
            //VNControllerHotkey = Config.Bind("Keyboard Shortcuts", "Toggle VN Controller Window", new KeyboardShortcut(KeyCode.Plus), "Show or hide the VN Controller window in Studio");
            //CharacterApi.RegisterExtraBehaviour<AnimationControllerCharaController>(GUID);
            //StudioSaveLoadApi.RegisterExtraBehaviour<AnimationControllerSceneController>(GUID);
        }

        public override string SceneDir()
        {
            return Path.GetFullPath(Path.Combine(
                Path.Combine(Path.Combine(Path.Combine(Application.dataPath, ".."), "UserData"), "Studio"), "scene"));
        }

        private IEnumerator AddMapCoroutine(int mapNum)
        {
            yield return Singleton<Map>.Instance.LoadMapCoroutine(mapNum, true);
            studio_scene.map = mapNum;
            if (!(studio.onChangeMap is null)) studio.onChangeMap();
            studio.m_CameraCtrl.CloerListCollider();
            Info.MapLoadInfo mapLoadInfo = null;
            if (Singleton<Info>.Instance.dicMapLoadInfo.TryGetValue(Singleton<Map>.Instance.no, out mapLoadInfo))
                studio.m_CameraCtrl.LoadVanish(mapLoadInfo.vanish.bundlePath, mapLoadInfo.vanish.fileName,
                    Singleton<Map>.Instance.mapRoot);
            var mapctrl = MapCtrl.Instance;
            mapctrl.Reflect();
        }

        public System.SystemData export_full_status()
        {
            return new System.SystemData(this);
        }

        public static int GetMapNumberByFilename(string fileName)
        {
            var mapLoadInfoDictionary = Singleton<Info>.Instance.dicMapLoadInfo;

            foreach (var kv in mapLoadInfoDictionary.Where(kv => kv.Value.fileName == fileName))
            {
                return kv.Key;
            }

            return -1;
        }

        public void Apply(System.SystemData data, bool changeMap = true)
        {
            BGM = data.bgm;

            if (!(data.wav is null)) WAV = (System.Wav_s) data.wav;

            if (changeMap)
            {
                // MapNumber = data.map;
                var mapNum = GetMapNumberByFilename(data.MapFilename);

                // For backwards compatibility
                if (data.MapFilename is null)
                {
                    mapNum = data.map;
                }

                if (mapNum != MapNumber)
                {
                    MapNumber = mapNum;
                }
            }

            MapPos = data.map_pos;
            MapRot = data.map_rot;
            Sun = data.sun;
            MapOption = data.map_opt;
            BackgroundImage = data.bg_png;
            Frame = data.fm_png;
            CharLight = data.char_light;
            if (!Ace.Equals(data.ace)) Ace = data.ace;
        }
    }
}