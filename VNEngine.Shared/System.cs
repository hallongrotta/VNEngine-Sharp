using MessagePack;
using Studio;
using System.IO;
using System.Linq;
using UnityEngine;
using VNActor;
using static VNEngine.Utils;
using static VNEngine.VNCamera;

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


        //===============================================================================================
        // system action wrapper functions
        // All scripts: func(game, param)
        public static void sys_idle(VNNeoController game, object param)
        {
            // as name says, do nothing, using in anime to wait, param ignored
            return;
        }

        public static void sys_idle(VNNeoController game, SystemData param)
        {
            return;
        }

        public static void sys_next(VNNeoController game, object param)
        {
            // the same as click next, param ignored
            game.NextText(game);
        }
        public static void sys_next(VNNeoController game, SystemData param)
        {
            return;
        }

        public static void sys_skip(VNNeoController game, SystemData param)
        {
            return;
        }

        public static void sys_skip(VNNeoController game, int param)
        {
            // skip some steps, param: steps to skip, should be > 0
            foreach (var i in Enumerable.Range(0, param))
            {
                //game.nextTexts.Remove(0); TODO
            }
        }


        public static void sys_branch_skip(VNNeoController game, SystemData param)
        {
            return;
        }

        public static void sys_branch(VNNeoController game, SystemData param)
        {
            return;
        }

        /* TODO

        public static void sys_branch(VNNeoController game, (List<string> buttons, List<Action> funcs) param)
        {
            // start a select scene: param = ([button1, button2, ...], [func1, func2, ...])
            // if use int in func(n), will call the sys_branch_skip by skip(n)
            var funclist = new List<Action>();
            foreach (var f in param.funcs)
            {
                if (f is int)
                {
                    funclist.Add((sys_branch_skip, f));
                }
                else
                {
                    funclist.Add(f);
                }
            }
            game.set_buttons(param.buttons, funclist);
        }

        public static void sys_branch_skip(VNNeoController game, int param)
        {
            // skip some steps, param: steps to skip, should be >= 0. if param == 0 skip to next, just like sys_next()
            // function in step will be ignored
            if (game.nextTexts.Count > param)
            {
                //game.NextText(game) get target and run it
                var skptgt = game.nextTexts[param];
                //print "skip to ['%s', '%s']"%(skptgt[0], skptgt[1])
                game.set_text(skptgt.character, skptgt.text);
                game.set_buttons(new List<object> {
                game.btnNextText
            }, new List<string> {
                game.NextText(game)
            });
                if (skptgt.func is GameFunc)
                {
                    var func = skptgt.func;
                    func(game, skptgt.param);
                }
                // pop skipped one
                foreach (var i in Enumerable.Range(0, param + 1))
                {
                    game.nextTexts.pop(0);
                }
            }
            else
            {
                throw new Exception(String.Format("sys_branch_skip: unable to skip %d steps when only %d steps in queue!", param, game.nextTexts.Length));
            }
        }
        */

        public static void sys_set_variable(VNNeoController game, SystemData param)
        {
            return;
        }

        /* TODO

        public static void sys_set_variable(VNNeoController game, (Dictionary<string, object> variable, Dictionary<string, object> value) param)
        {
            // set some variables in act script, param = (variable, key, value) or (variable, value)
            // variable must be a mutable obj such like list and dict
            // if key is omitted, function set all values in 'variable' so make sure 'value' is a list or dict just like 'variable'
            foreach (var k in param.variable.Keys)
            {
                param.variable[k] = param.value[k];
            }
        }

        public static void sys_set_variable(VNNeoController game, (List<object> variable, List<object> value) param)
        {
            // set some variables in act script, param = (variable, key, value) or (variable, value)
            // variable must be a mutable obj such like list and dict
            // if key is omitted, function set all values in 'variable' so make sure 'value' is a list or dict just like 'variable'
            foreach (var i in Enumerable.Range(0, param.variable.Count))
            {
                param.variable[i] = param.value[i];
            }
        }

        public static void sys_set_variable(VNNeoController game, (Dictionary<string, object> variable, string key, object value) param)
        {
            // set some variables in act script, param = (variable, key, value) or (variable, value)
            // variable must be a mutable obj such like list and dict
            // if key is omitted, function set all values in 'variable' so make sure 'value' is a list or dict just like 'variable'

            param.variable[param.key] = param.value;
        }

        */

        public static void sys_visible(VNNeoController game, SystemData param)
        {
            return;
            //sys_visible(game, param.visible);
        }

        public static void sys_visible(VNNeoController game, bool param)
        {
            // param = 0(hide)/1(show)
            game.visible = param;
        }

        public static void sys_lock(VNNeoController game, SystemData param)
        {
            return;
        }

        public static void sys_lock(VNNeoController game, bool param)
        {
            // param = 0(unlock)/1(lock)
            game.isHideGameButtons = param;
        }

        public static void sys_text(VNNeoController game, SystemData param)
        {
            return;
        }

        public static void sys_text(VNNeoController game, VNData param)
        {
            // param = (char, text)
            game.set_text(param.whosay, param.whatsay);
        }

        public static void sys_lipsync(VNNeoController game, SystemData param)
        {
            return;
        }

        public static void sys_lipsync(VNNeoController game, bool param)
        {
            // param = 0(disable)/1(enable)
            game.isfAutoLipSync = param;
        }

        public static void sys_btn_next(VNNeoController game, SystemData param)
        {
            return;
        }

        public static void sys_btn_next(VNNeoController game, string param = "Next >>")
        {
            // param = "next button text"
            //game.btnNextText = param
            /* TODO
            game.set_buttons(new List<string> {
                param
            },
                new List<(Action<object>, object)> {
                (game.NextText, null)
        });
            */
        }

        public static void sys_wait_anime(VNNeoController game, SystemData param)
        {
            return;
        }

        public static bool sys_wait_anime(VNNeoController game, string param)
        {
            // wait anime of actor play once: param = actorID
            // return True if anime is over or actor not found
            Actor actor = game.scenef_get_actor(param);
            if (actor is Actor act)
            {
                return act.isAnimeOver;
            }
            else
            {
                return true;
            }
        }

        public static void sys_wait_voice(VNNeoController game, SystemData param)
        {
            return;
        }

        public static bool sys_wait_voice(VNNeoController game, string param)
        {
            // wait voice of actor over: param = actorID
            // return True if voice is over or actor not found
            Actor actor = game.scenef_get_actor(param);
            if (actor is Actor act)
            {
                return !act.isVoicePlay;
            }
            else
            {
                return true;
            }
        }

        public static void sys_bgm(VNNeoController game, SystemData param)
        {
            sys_bgm(game, param.bgm);
        }

        public static void sys_bgm(VNNeoController game, BGM_s param)
        {
            // set bgm, param = (bgm no, play)
            if (game.studio.bgmCtrl.no != param.no)
            {
                game.studio.bgmCtrl.Play(param.no);
            }
            if (game.studio.bgmCtrl.play != param.play)
            {
                if (param.play)
                {
                    game.studio.bgmCtrl.Play();
                }
                else
                {
                    game.studio.bgmCtrl.Stop();
                }
            }
        }

        /*

        public static void sys_env(VNNeoController game, SystemData param)
        {
            sys_env(game, param.env);
        }

        public static void sys_env(VNNeoController game, (int no, bool play) param)
        {
            // set evn sound, param = (evn no, play), StudioNeo only
            if (game.isStudioNEO)
            {
                if (game.studio.envCtrl.no != param.no)
                {
                    game.studio.envCtrl.Play(param.no);
                }
                if (game.studio.envCtrl.play != param.play)
                {
                    if (param.play)
                    {
                        game.studio.envCtrl.Play();
                    }
                    else
                    {
                        game.studio.envCtrl.Stop();
                    }
                }
            }
            else
            {
                Console.WriteLine("sys_env only supports HoneySelect Studio Neo");
            }
        }
        */

        public static void sys_wav(VNNeoController game, SystemData param)
        {
            if (param.wav is Wav_s wav)
            {
                sys_wav(game, wav);
            }
        }





        public static void sys_wav(VNNeoController game, Wav_s param)
        {
            string wavRevPath;
            // set outside wav sound, param = (wav file, play, repeat)
            var wavName = param.fileName.Trim();
            if (wavName != "")
            {
                if (!wavName.ToLower().EndsWith(".wav"))
                {
                    wavName += ".wav";
                }
                // load wav in game scene folder if existed
                var wavInScene = combine_path(game.get_scene_dir(), game.sceneDir, wavName);
                if (File.Exists(wavInScene))
                {
                    if (game.isStudioNEO)
                    {
                        wavRevPath = combine_path("..", "studioneo", "scene", game.sceneDir, wavName);
                    }
                    else
                    {
                        wavRevPath = combine_path("..", "studio", "scene", game.sceneDir, wavName);
                    }
                    if (game.studio.outsideSoundCtrl.fileName != wavRevPath)
                    {
                        game.studio.outsideSoundCtrl.Play(wavRevPath);
                    }
                }
                else
                {
                    // load wav in game default audio folder if existed
                    var wavInDefault = Path.GetFullPath(combine_path(Application.dataPath, "..", "UserData", "audio", wavName));
                    if (File.Exists(wavInDefault))
                    {
                        if (game.studio.outsideSoundCtrl.fileName != wavName)
                        {
                            game.studio.outsideSoundCtrl.Play(wavName);
                        }
                    }
                }
            }
            if (game.studio.outsideSoundCtrl.play != param.play || wavName == "")
            {
                if (param.play)
                {
                    game.studio.outsideSoundCtrl.Play();
                }
                else
                {
                    game.studio.outsideSoundCtrl.Stop();
                }
            }
            if (param.repeat)
            {
                game.studio.outsideSoundCtrl.repeat = BGMCtrl.Repeat.All;
            }
            else
            {
                game.studio.outsideSoundCtrl.repeat = BGMCtrl.Repeat.None;
            }
        }

        public static void sys_map(VNNeoController game, SystemData param)
        {
            sys_map(game, param.map);
        }

        public static void sys_map_pos(VNNeoController game, SystemData param)
        {
            sys_map_pos(game, param.map_pos);
        }

        public static void sys_map_rot(VNNeoController game, SystemData param)
        {
            sys_map_rot(game, param.map_rot);
        }


        /*        public static object sys_skybox(VNNeoController game, int[] param)
                {
                    // set sky box, param = sky box index, Playhome only
                    if (game.isPlayHomeStudio)
                    {
                        game.studio.AddSkybox(param);
                    }
                    else
                    {
                        Console.WriteLine("sys_skybox only supports PlayHome Studio");
                    }
                }
        */

        public static void sys_bg_png(VNNeoController game, SystemData param)
        {
            sys_bg_png(game, param.bg_png);
        }
        public static void sys_bg_png(VNNeoController game, string param = "")
        {
            string pngInDefault;
            // set background png, param = png file name
            var pngName = param.Trim();
            if (pngName != "")
            {
                if (!pngName.ToLower().EndsWith(".png"))
                {
                    pngName += ".png";
                }
                // load png in game scene folder if existed
                var pngInScene = combine_path(game.get_scene_dir(), game.sceneDir, pngName);
                if (File.Exists(pngInScene))
                {
                    game.scene_set_bg_png(pngName);
                    return;
                }
                // load png in game default background folder if existed
                if (game.isCharaStudio || game.isNEOV2)
                {
                    pngInDefault = Path.GetFullPath(combine_path(Application.dataPath, "..", "UserData", "bg", pngName));
                }
                else
                {
                    pngInDefault = Path.GetFullPath(combine_path(Application.dataPath, "..", "UserData", "background", pngName));
                }
                if (File.Exists(pngInDefault))
                {
                    game.scene_set_bg_png_orig(pngName);
                    return;
                }
            }
            // remove if param == "" or file not existed
            game.scene_set_bg_png_orig("");
        }

        public static void sys_char_light(VNNeoController game, SystemData param)
        {
            sys_char_light(game, param.char_light);
        }

        public static void sys_char_light(VNNeoController game, CharLight_s param)
        {
            //SceneInfo sc;
            // set chara light
            // param for HS, KK, AI = (color, intensity, rot_y, rot_x, shadow)
            // param for PH = (color, intensity, rot_y, rot_x, shadow, method)
            /*            if (game.isStudioNEO)
                        {
                            sc = game.studio_scene;
                            sc.cameraLightColor.SetDiffuseRGB(tuple4_2_color(param[0]));
                            sc.cameraLightIntensity = param[1];
                            sc.cameraLightRot[0] = param[2];
                            sc.cameraLightRot[1] = param[3];
                            sc.cameraLightShadow = param[4];
                            game.studio.cameraLightCtrl.Reflect();
                        }
                        else if (game.isPlayHomeStudio)
                        {
                            sc = game.studio_scene;
                            sc.cameraLightColor = tuple4_2_color(param[0]);
                            sc.cameraLightIntensity = param[1];
                            sc.cameraLightRot[0] = param[2];
                            sc.cameraLightRot[1] = param[3];
                            sc.cameraLightShadow = param[4];
                            sc.cameraMethod = param[5];
                            game.studio.cameraLightCtrl.Reflect();
                        }*/
            if (game.isCharaStudio || game.isNEOV2)
            {
                var cl = game.studio_scene.charaLight;
                cl.color = param.rgbDiffuse;
                cl.intensity = param.cameraLightIntensity;
                cl.rot[0] = param.rot_y;
                cl.rot[1] = param.rot_x;
                cl.shadow = param.cameraLightShadow;
                game.studio.cameraLightCtrl.Reflect();
            }
        }
        /*
                public static void sys_pl_neoextsave(VNNeoController game, int[] param)
                {
                    // HSStudioNEOExtSave plugin - for HSNeoAdvAddon
                    if (extplugins.ExtPlugin.exists("HSStudioNEOExtSave"))
                    {
                        var pl_neoext = ExtPlugins.HSStudioNEOExtSave();
                        pl_neoext.ExtDataSet(param);
                    }
                    else
                    {
                        Console.WriteLine("this require HSStudioNEOExtSave plugin");
                    }
                }

                public static object sys_pl_nodescon(VNNeoController game, int[] param)
                {
                    // NodesConstraints
                    if (extplugins.ExtPlugin.exists("NodesConstraints"))
                    {
                        var pl_neoext = extplugins.NodesConstraints();
                        pl_neoext.SetSysSettingsText(param);
                    }
                    else
                    {
                        Console.WriteLine("this require NodesConstraints plugin");
                    }
                }

                public static object sys_pl_dhh(VNNeoController game, int[] param)
                {
                    // DHH
                    if (game.isNEOV2 && extplugins.ExtPlugin.exists("DHH_AI4"))
                    {
                        var pl_dhh = extplugins.DHH_AI();
                        pl_dhh.setEnable(param[0]);
                        pl_dhh.importGraphSetting(param[1]);
                    }
                    else
                    {
                        Console.WriteLine("this require DHH_AI4 (for AI) plugin");
                    }
                }*/

        /*
        // :type game: vngameengine.VNNeoController
        public static SystemData export_sys_status(VNNeoController game)
        {
            object sc;
            // export a dict contains all system status
            //from Studio import Studio
            //studio = Studio.Instance
            var fs = new SystemData();
            {
            };
            fs["bgm"] = (game.studio.bgmCtrl.no, game.studio.bgmCtrl.play);
            if (game.isStudioNEO)
            {
                fs["env"] = (game.studio.envCtrl.no, game.studio.envCtrl.play);
            }
            fs["wav"] = (game.studio.outsideSoundCtrl.fileName, game.studio.outsideSoundCtrl.play, game.studio.outsideSoundCtrl.repeat == BGMCtrl.Repeat.All);
            fs["map"] = game.studio_scene.map;
            fs["map_pos"] = game.studio_scene.caMap.pos;
            fs["map_rot"] = game.studio_scene.caMap.rot;
            if (game.isCharaStudio)
            {
                fs["map_sun"] = game.studio_scene.sunLightType;
            }
            if (game.isCharaStudio || game.isNEOV2)
            {
                fs["map_opt"] = game.studio_scene.mapOption;
            }
            if (game.isPlayHomeStudio)
            {
                fs["skybox"] = game.studio_scene.skybox;
            }
            fs["bg_png"] = game.scene_get_bg_png_orig();
            if (game.isCharaStudio || game.isNEOV2)
            {
                fs["fm_png"] = game.scene_get_framefile();
            }
            if (game.isStudioNEO)
            {
                sc = game.studio_scene;
                fs["char_light"] = (sc.cameraLightColor.rgbDiffuse, sc.cameraLightIntensity, sc.cameraLightRot[0], sc.cameraLightRot[1], sc.cameraLightShadow);
            }
            else if (game.isPlayHomeStudio)
            {
                sc = game.studio_scene;
                fs["char_light"] = (sc.cameraLightColor, sc.cameraLightIntensity, sc.cameraLightRot[0], sc.cameraLightRot[1], sc.cameraLightShadow, sc.cameraMethod);
            }
            else if (game.isCharaStudio || game.isNEOV2)
            {
                var cl = game.studio_scene.charaLight;
                fs["char_light"] = (cl.color, cl.intensity, cl.rot[0], cl.rot[1], cl.shadow);
            }
            // external plugins data
            if (game.isStudioNEO)
            {
                if (extplugins.ExtPlugin.exists("HSStudioNEOExtSave"))
                {
                    if (is_ini_value_true("ExportSys_NeoExtSave"))
                    {
                        var pl_neoext = extplugins.HSStudioNEOExtSave();
                        fs["pl_neoextsave"] = pl_neoext.ExtDataGet();
                    }
                }
            }
            if (game.isStudioNEO || game.isCharaStudio)
            {
                if (extplugins.ExtPlugin.exists("NodesConstraints"))
                {
                    if (is_ini_value_true("ExportSys_NodesConstraints"))
                    {
                        var pl_nodescon = extplugins.NodesConstraints();
                        fs["pl_nodescon"] = pl_nodescon.GetSysSettingsText();
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
                        fs["pl_dhh"] = (pl_dhh.getEnable(), pl_dhh.exportGraphSetting());
                    }
                }
            }
            //print fs
            return fs;
        }
        */



    }
}
