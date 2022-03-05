using MessagePack;
using Studio;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using VNActor;

namespace VNEngine
{
    public static partial class System
    {

        public struct Ace : IEquatable<Ace>
        {
            public float blend;
            public int no;

            public bool Equals(Ace other)
            {
                bool equal = true;
                equal &= no == other.no;
                equal &= (blend - other.blend) < 0.001;
                return equal;
            }
        }


        [MessagePackObject(keyAsPropertyName: true)]
        public class SystemData
        {
            public BGM_s bgm;

            public Wav_s? wav;
            public int map;
            public Vector3 map_pos;
            public Vector3 map_rot;
            public int sun;
            public bool map_opt;

            public string bg_png;
            public string fm_png;

            public CharLight_s char_light;

            public Ace ace;
            public void Remove(string key)
            {
                throw new NotImplementedException();
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
                map = game.studio_scene.map;
                map_pos = game.studio_scene.caMap.pos;
                map_rot = game.studio_scene.caMap.rot;

                sun = game.studio_scene.sunLightType;


                map_opt = game.studio_scene.mapOption;

                bg_png = game.scene_get_bg_png_orig();

                fm_png = game.scene_get_framefile();


                var cl = game.studio_scene.charaLight;
                char_light = new CharLight_s { rgbDiffuse = cl.color, cameraLightIntensity = cl.intensity, rot_y = cl.rot[0], rot_x = cl.rot[1], cameraLightShadow = cl.shadow };

                ace = System.ace;

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

            internal void Apply(StudioController game, bool change_map = true)
            {
                sys_bgm(game, this);
                sys_wav(game, this);

                if (change_map)
                {
                    sys_map(game, this);
                }

                sys_map_pos(game, this);
                sys_map_rot(game, this);
                map_sun(game, this);
                map_option(game, this);
                sys_bg_png(game, this);
                sys_fm_png(game, this);
                sys_char_light(game, this);
                if (!System.ace.Equals(ace))
                {
                    System.ace = this.ace;
                }             
            }
        }

        private static Ace ace
        {
            get
            {
                return new Ace { no = StudioController.Instance.studio_scene.aceNo, blend = StudioController.Instance.studio_scene.aceBlend };
            }
            set
            {
                StudioController.Instance.studio_scene.aceNo = value.no;
                StudioController.Instance.studio_scene.aceBlend = value.blend;
                Studio.Studio.Instance.systemButtonCtrl.UpdateInfo();
            }
        }

        public static void sys_map_rot(VNNeoController game, Vector3 param)
        {
            game.studio_scene.caMap.rot = param;
        }

        public static void map_sun(StudioController game, SystemData param)
        {
            map_sun(game, param.sun);
        }

        public static void sys_map_pos(VNNeoController game, Vector3 param)
        {
            game.studio_scene.caMap.pos = param;
        }

        public static void sys_map(VNNeoController game, int param)
        {
            // set map
            if (param != game.studio_scene.map)
            {
                game.studio.AddMap(param);
            }
        }



        public static void map_sun(StudioController game, int param)
        {
            // set sunLightType, param = sunLightType index, CharaStudio only

            var st = new SunLightInfo.Info.Type[] { SunLightInfo.Info.Type.DayTime, SunLightInfo.Info.Type.Evening, SunLightInfo.Info.Type.Night };
            var map = Map.Instance;
            map.sunType = st[param];

        }

        public static void map_option(VNNeoController game, SystemData param)
        {
            map_option(game, param.map_opt);
        }

        public static void map_option(VNNeoController game, bool param)
        {
            Map map;
            // set map option visible: param = 1/0
            map = Map.Instance;
            map.visibleOption = param;
        }

        public static void sys_fm_png(StudioController game, SystemData param)
        {
            sys_fm_png(game, param.fm_png);
        }

        public static void sys_fm_png(StudioController game, string pngName)
        {
            // set frame png, param = png file name, CharaStudio only
            if (pngName is null)
            {
                pngName = "";
            }
            else {
                // try to load png in "frame" folder
                var path = Utils.combine_path(Application.dataPath, "..", "UserData", "frame", pngName);
                var pngInDefault = Path.GetFullPath(path);
                if (!File.Exists(pngInDefault))
                {
                    pngName = "";
                }
            }
            game.scene_set_framefile(pngName);
        }

        public static void sys_map(StudioController game, SystemData s)
        {
            sys_map(game, s.map);
        }
        public static void sys_map(StudioController game, int param)
        {
            // set map
            if (param != game.studio_scene.map)
            {
                game.change_map_to(param);
            }
        }

        public delegate void SystemFunc(StudioController game, SystemData param);

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
