using Studio;
using System;
using System.IO;
using UnityEngine;
using VNActor;

namespace VNEngine
{
    public static partial class System
    {
        public static void sys_map_option(StudioController game, bool param)
        {
            Studio.Map map;
            // set map option visible: param = 1/0
            map = Studio.Map.Instance;
            map.VisibleOption = param;
        }

        public static void sys_fm_png(StudioController game, string param = "")
        {
            // set frame png, param = png file name, CharaStudio only

            var pngName = param.Trim();
            if (pngName != "")
            {
                if (!pngName.ToLower().EndsWith(".png"))
                {
                    pngName += ".png";
                }
                // load png in game scene folder if existed
                var pngInScene = Utils.combine_path(game.get_scene_dir(), game.sceneDir, pngName);
                if (File.Exists(pngInScene))
                {
                    var pngRevPath = Utils.combine_path("..", "studio", "scene", game.sceneDir, pngName);
                    game.scene_set_framefile(pngRevPath);
                    return;
                }
                // load png in game default background folder if existed
                var pngInDefault = Path.GetFullPath(Utils.combine_path(Application.dataPath, "..", "UserData", "frame", pngName));
                if (File.Exists(pngInDefault))
                {
                    game.scene_set_framefile(pngName);
                    return;
                }
            }
            // remove if param == "" or file not existed
            game.scene_set_framefile("");
        }

        public static void sys_map_rot(VNNeoController game, Vector3 param)
        {
            game.studio_scene.mapInfo.ca.rot = param;
        }

        public static void sys_map_pos(VNNeoController game, Vector3 param)
        {
            game.studio_scene.mapInfo.ca.pos = param;
        }

        public static void sys_map(VNNeoController game, int param)
        {
            // set map
            if (param != game.studio_scene.mapInfo.no)
            {
                game.studio.AddMap(param);
            }
        }

        public static void sys_map_light(StudioController game, bool param)
        {
            game.studio_scene.mapInfo.light = param;
        }

        public struct ColorCorrection : IEquatable<ColorCorrection>
        {
            public int no;
            public float blend;
            public int saturation;
            public int brightness;
            public int contrast;

            public bool Equals(ColorCorrection other)
            {
                bool equal = true;
                equal &= no == other.no;
                equal &= (blend - other.blend) < 0.001;
                equal &= saturation == other.saturation;
                equal &= brightness == other.brightness;
                equal &= contrast == other.contrast;
                return equal;
            }
        }

        private static ColorCorrection colorCorrection
        {
            get
            {
                SceneInfo sceneInfo = StudioController.Instance.studio_scene;
                return new ColorCorrection { no = sceneInfo.cgLookupTexture, blend = sceneInfo.cgBlend, contrast = sceneInfo.cgContrast, brightness = sceneInfo.cgBrightness, saturation = sceneInfo.cgSaturation };
            }
            set
            {
                StudioController.Instance.studio_scene.cgLookupTexture = value.no;
                StudioController.Instance.studio_scene.cgBlend = value.blend;
                StudioController.Instance.studio_scene.cgBrightness = value.brightness;
                StudioController.Instance.studio_scene.cgContrast = value.contrast;
                StudioController.Instance.studio_scene.cgSaturation = value.saturation;
                Studio.Studio.Instance.systemButtonCtrl.UpdateInfo();
            }
        }

        public class SystemData
        {

            public BGM_s bgm;

            public Wav_s? wav;
            public int map;
            public Vector3 map_pos;
            public Vector3 map_rot;

            public bool map_light;

            public bool map_opt;

            public string bg_png;
            public string fm_png;

            public CharLight_s char_light;

            public ColorCorrection colorCorrection;

            public void Remove(string key)
            {
                throw new NotImplementedException();
            }

            public void Apply(StudioController game)
            {
                sys_bgm(game, this);
                sys_wav(game, this);
                sys_map(game, this);
                sys_map_pos(game, this);
                sys_map_rot(game, this);
                sys_map_option(game, this.map_opt);
                sys_bg_png(game, this);
                sys_fm_png(game, this.fm_png);
                sys_char_light(game, this);
                if (!System.colorCorrection.Equals(colorCorrection))
                {
                    System.colorCorrection = this.colorCorrection;
                }            
            }

            public SystemData()
            {

            }

            public SystemData(StudioController game)
            {
                // export a dict contains all system status
                //from Studio import Studio
                //studio = Studio.Instance

                bgm = new BGM_s { no = game.studio.bgmCtrl.no, play = game.studio.bgmCtrl.play };

                if (game.studio.outsideSoundCtrl.fileName != "")
                {
                    wav = new Wav_s { fileName = game.studio.outsideSoundCtrl.fileName, play = game.studio.outsideSoundCtrl.play, repeat = game.studio.outsideSoundCtrl.repeat == BGMCtrl.Repeat.All };
                }
                else
                {
                    wav = null;
                }

                map = game.studio_scene.mapInfo.no;

                map_pos = game.studio_scene.mapInfo.ca.pos;
                map_rot = game.studio_scene.mapInfo.ca.rot;
                map_light = game.studio_scene.mapInfo.light;
                map_opt = game.studio_scene.mapInfo.option;

                bg_png = game.scene_get_bg_png_orig();

                fm_png = game.scene_get_framefile();

                var cl = game.studio_scene.charaLight;
                char_light = new CharLight_s { rgbDiffuse = cl.color, cameraLightIntensity = cl.intensity, rot_y = cl.rot[0], rot_x = cl.rot[1], cameraLightShadow = cl.shadow };

                colorCorrection = System.colorCorrection;

                /* TODO
                if (game.isStudioNEO || game.isCharaStudio)
                {
                    if (extplugins.ExtPlugin.exists("NodesConstraints"))
                    {
                        if (is_ini_value_true("ExportSys_NodesConstraints"))
                        {
                            var pl_nodescon = extplugins.NodesConstraints();
                            pl_nodescon = pl_nodescon.GetSysSettingsText();
                        }
                    }
                }
                if (game.isNEOV2)
                {
                    if (extplugins.ExtPlugin.exists("DHH_AI4"))
                    {
                        if (is_ini_value_true("ExportSys_DHH"))
                        {
                            var pl_dhh = extplugins.DHH_AI();
                            pl_dhh = (pl_dhh.getEnable(), pl_dhh.exportGraphSetting());
                        }
                    }
                }
                */
            }

            /*
            public static Dictionary<string, (SystemFunc, bool)> sys_act_funcs = new Dictionary<string, (SystemFunc, bool)> {
        {
            "idle",
            (sys_idle, true)},
        {
            "next",
            (sys_next, false)},
        {
            "skip",
            (sys_skip, false)},
        {
            "branch",
            (sys_branch, false)},
        {
            "set_vari",
            (sys_set_variable, false)},
        {
            "visible",
            (sys_visible, false)},
        {
            "lock",
            (sys_lock, false)},
        {
            "text",
            (sys_text, false)},
        {
            "lipsync",
            (sys_lipsync, false)},
        {
            "btn_next",
            (sys_btn_next, false)},
        {
            "wait_anime",
            (sys_wait_anime, true)},
        {
            "wait_voice",
            (sys_wait_voice, true)},
        {
            "bgm",
            (sys_bgm, false)},
        {
            "env",
            (sys_env, false)},
        {
            "wav",
            (sys_wav, false)},
        {
            "map",
            (sys_map, false)},
        {
            "map_pos",
            (sys_map_pos, true)},
        {
            "map_rot",
            (sys_map_rot, true)},
        {
            "map_sun",
            (sys_map_sun, false)},
        {
            "map_opt",
            (sys_map_option, false)},
        {
            "skybox",
            (sys_skybox, false)},
        {
            "bg_png",
            (sys_bg_png, false)},
        {
            "fm_png",
            (sys_fm_png, false)},
        {
            "char_light",
            (sys_char_light, false)},
        {
            "pl_neoextsave",
            (sys_pl_neoextsave, false)},
        {
            "pl_nodescon",
            (sys_pl_nodescon, false)},
        {
            "pl_dhh",
            (sys_pl_dhh, false)}};
            */
        }

    }
}
