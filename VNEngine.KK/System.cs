using System;
using System.Collections.Generic;
using System.Text;
using VNEngine;
using Studio;
using System.IO;
using UnityEngine;
using VNActor;
using ADV.EventCG;
using MessagePack;

namespace VNEngine
{
    public partial class System
    {

        [MessagePackObject]
        public struct Wav_s
        {
            [Key(0)]
            public string fileName;
            [Key(1)]
            public bool play;
            [Key(2)]
            public bool repeat;
        }

        [MessagePackObject]
        public struct BGM_s
        {
            [Key(0)]
            public int no;
            [Key(1)]
            public bool play;
        }

        [MessagePackObject]
        public struct CharLight_s
        {
            [Key(0)]
            public Color rgbDiffuse;
            [Key(2)]
            public float cameraLightIntensity;
            [Key(3)]
            public float rot_y;
            [Key(4)]
            public float rot_x;
            [Key(5)]
            public bool cameraLightShadow;
        }

        [MessagePackObject(keyAsPropertyName: true)]
        public struct SystemData : IDataClass
        {
            public BGM_s bgm;

            public Wav_s? wav;
            public int map;
            public Vector3 map_pos;
            public Vector3 map_rot;
            public int map_sun;
            public bool map_opt;

            public string bg_png;
            public string fm_png;

            public CharLight_s char_light;

            public void Remove(string key)
            {
                throw new NotImplementedException();
            }

            public SystemData(CharaStudioController game)
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
                map = game.studio_scene.map;
                map_pos = game.studio_scene.caMap.pos;
                map_rot = game.studio_scene.caMap.rot;

                map_sun = game.studio_scene.sunLightType;
                

                map_opt = game.studio_scene.mapOption;
                
                bg_png = game.scene_get_bg_png_orig();

                fm_png = game.scene_get_framefile();
                

                var cl = game.studio_scene.charaLight;
                char_light = new CharLight_s { rgbDiffuse =  cl.color, cameraLightIntensity = cl.intensity, rot_y = cl.rot[0], rot_x = cl.rot[1], cameraLightShadow = cl.shadow };
                
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
                */
            }

            [SerializationConstructor]
            public SystemData(BGM_s bgm, Wav_s? wav, int map, Vector3 map_pos, Vector3 map_rot, int map_sun, bool map_opt, string bg_png, string fm_png, CharLight_s char_light)
            {
                this.bgm = bgm;
                this.wav = wav;
                this.map = map;
                this.map_pos = map_pos;
                this.map_rot = map_rot;
                this.map_sun = map_sun;
                this.map_opt = map_opt;
                this.bg_png = bg_png;
                this.fm_png = fm_png;
                this.char_light = char_light;
            }
        }

        public static void import_status(SystemData s)
        {
            var game = CharaStudioController.Instance;
            sys_bgm(game, s);
            sys_wav(game, s);
            sys_map(game, s);
            sys_map_pos(game, s);
            sys_map_rot(game, s);
            map_sun(game, s);
            map_option(game, s);
            sys_bg_png(game, s);
            sys_fm_png(game, s);
            sys_char_light(game, s);
        }

        public static void map_sun(CharaStudioController game, SystemData param)
        {
            map_sun(game, param.map_sun);
        }

        public static void map_sun(CharaStudioController game, int param)
        {
            // set sunLightType, param = sunLightType index, CharaStudio only
            if (game.isCharaStudio)
            {

                var st = new SunLightInfo.Info.Type[] { SunLightInfo.Info.Type.DayTime, SunLightInfo.Info.Type.Evening, SunLightInfo.Info.Type.Night };
                var map = Map.Instance;
                map.sunType = st[param];
            }
            else
            {
                Console.WriteLine("sys_map_sun only supports CharaStudio");
            }
        }

        public static void map_option(VNNeoController game, SystemData param)
        {
            map_option(game, param.map_opt);
        }

        public static void map_option(VNNeoController game, bool param)
        {
            Map map;
            // set map option visible: param = 1/0
            if (game.isCharaStudio)
            {
                map = Map.Instance;
                map.visibleOption = param;
            }
        }

        public static void sys_fm_png(CharaStudioController game, SystemData param)
        {
            sys_fm_png(game, param.fm_png);
        }

        public static void sys_fm_png(CharaStudioController game, string param = "")
        {
            // set frame png, param = png file name, CharaStudio only
            if (game.isCharaStudio || game.isNEOV2)
            {
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
            else
            {
                Console.WriteLine("sys_fm_png only supports CharaStudio");
            }
        }

        public static void sys_map(CharaStudioController game, int param)
        {
            // set map
            if (param != game.studio_scene.map)
            {
                if (game.isCharaStudio)
                {
                    game.change_map_to(param);
                }
            }
        }

        static public IDataClass export_sys_status(VNNeoController game)
        {
            return new SystemData((CharaStudioController)game);
        }

        public delegate void SystemFunc(CharaStudioController game, SystemData param);

        public struct ActFunc
        {
            SystemFunc func;
            bool active;

            public ActFunc(SystemFunc func, bool active)
            {
                this.func = func;
                this.active = active;
            }

        }

        public static Dictionary<string, ActFunc> sys_act_funcs = new Dictionary<string, ActFunc> {
        {
            "idle",
            new ActFunc(sys_idle, true)},
        {
            "next",
            new ActFunc(sys_next, false)},
        {
            "skip",
            new ActFunc(sys_skip, false)},
        {
            "branch",
            new ActFunc(sys_branch, false)},
        {
            "set_vari",
            new ActFunc(sys_set_variable, false)},
        {
            "visible",
            new ActFunc(sys_visible, false)},
        {
            "lock",
            new ActFunc(sys_lock, false)},
        {
            "text",
            new ActFunc(sys_text, false)},
        {
            "lipsync",
            new ActFunc(sys_lipsync, false)},
        {
            "btn_next",
            new ActFunc(sys_btn_next, false)},
        {
            "wait_anime",
            new ActFunc(sys_wait_anime, true)},
        {
            "wait_voice",
            new ActFunc(sys_wait_voice, true)},
        {
            "bgm",
            new ActFunc(sys_bgm, false)},
        {
            "wav",
            new ActFunc(sys_wav, false)},
        {
            "map",
            new ActFunc(sys_map, false)},
        {
            "map_pos",
            new ActFunc(sys_map_pos, true)},
        {
            "map_rot",
            new ActFunc(sys_map_rot, true)},
        {
            "map_sun",
            new ActFunc(map_sun, false)},
        {
            "map_opt",
            new ActFunc(map_option, false)},
        {
            "bg_png",
            new ActFunc(sys_bg_png, false)},
        {
            "fm_png",
            new ActFunc(sys_fm_png, false)},
        {
            "char_light",
            new ActFunc(sys_char_light, false)},
        };
    }
}
