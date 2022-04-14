using System.IO;
using Studio;
using UnityEngine;
using static VNEngine.Utils;

namespace VNEngine
{
    public partial class StudioController
    {
        public string WavFileName => studio.outsideSoundCtrl.fileName;

        public System.Wav_s WAV
        {
            set
            {
                {
                    string wavRevPath;
                    // set outside wav sound, value = (wav file, play, repeat)
                    var wavName = value.fileName.Trim();
                    if (wavName != "")
                    {
                        if (!wavName.ToLower().EndsWith(".wav")) wavName += ".wav";
                        // load wav in game scene folder if existed
                        var wavInScene = combine_path(SceneDir(), sceneDir, wavName);
                        if (File.Exists(wavInScene))
                        {
                            wavRevPath = combine_path("..", "studio", "scene", sceneDir, wavName);

                            if (studio.outsideSoundCtrl.fileName != wavRevPath)
                                studio.outsideSoundCtrl.Play(wavRevPath);
                        }
                        else
                        {
                            // load wav in game default audio folder if existed
                            var wavInDefault =
                                Path.GetFullPath(combine_path(Application.dataPath, "..", "UserData", "audio",
                                    wavName));
                            if (File.Exists(wavInDefault))
                                if (studio.outsideSoundCtrl.fileName != wavName)
                                    studio.outsideSoundCtrl.Play(wavName);
                        }
                    }

                    if (studio.outsideSoundCtrl.play != value.play || wavName == "")
                    {
                        if (value.play)
                            studio.outsideSoundCtrl.Play();
                        else
                            studio.outsideSoundCtrl.Stop();
                    }

                    if (value.repeat)
                        studio.outsideSoundCtrl.repeat = BGMCtrl.Repeat.All;
                    else
                        studio.outsideSoundCtrl.repeat = BGMCtrl.Repeat.None;
                }
            }
            get =>
                new System.Wav_s
                {
                    fileName = studio.outsideSoundCtrl.fileName,
                    play = studio.outsideSoundCtrl.play,
                    repeat = studio.outsideSoundCtrl.repeat == BGMCtrl.Repeat.All
                };
        }

        public System.BGM_s BGM
        {
            set
            {
                if (studio.bgmCtrl.no != value.no) studio.bgmCtrl.Play(value.no);
                if (studio.bgmCtrl.play == value.play) return;
                if (value.play)
                    studio.bgmCtrl.Play();
                else
                    studio.bgmCtrl.Stop();
            }
            get => new System.BGM_s {no = studio.bgmCtrl.no, play = studio.bgmCtrl.play};
        }

        public System.CharLight_s CharLight
        {
            set
            {
                var cl = studio_scene.charaLight;
                cl.color = value.rgbDiffuse;
                cl.intensity = value.cameraLightIntensity;
                cl.rot[0] = value.rot_y;
                cl.rot[1] = value.rot_x;
                cl.shadow = value.cameraLightShadow;
                studio.cameraLightCtrl.Reflect();
            }
            get
            {
                var cl = studio_scene.charaLight;
                return new System.CharLight_s
                {
                    rgbDiffuse = cl.color,
                    cameraLightIntensity = cl.intensity,
                    rot_y = cl.rot[0],
                    rot_x = cl.rot[1],
                    cameraLightShadow = cl.shadow
                };
            }
        }

        public string BackgroundImage
        {
            set
            {
                var pngName = value;
                if (value is null)
                {
                    value = "";
                }
                else
                {
                    var pngInDefault =
                        Path.GetFullPath(combine_path(Application.dataPath, "..", "UserData", "bg", pngName));
                    if (!File.Exists(pngInDefault)) pngName = "";
                }

                scene_set_bg_png_orig(pngName);
            }
        }
    }
}