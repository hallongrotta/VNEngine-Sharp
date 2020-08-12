using System;
using System.Collections.Generic;
using VNEngine;

namespace VNEngineNEOV2
{
    public partial class System
    {
        public struct SystemData : IDataClass
        {
            private (object no, object play) bgm;
            private (object no, object play) env;
            private (object fileName, object play, bool) wav;
            private object map;
            private object map_pos;
            private object map_rot;
            private object map_sun;
            private bool map_opt;
            private object skybox;
            private object bg_png;
            private object fm_png;
            private SceneInfo sc;
            private (object rgbDiffuse, object cameraLightIntensity, object, object, object cameraLightShadow) char_light;

            public void Remove(string key)
            {
                throw new NotImplementedException();
            }

            public static void sys_map_option(NeoV2Controller game, bool param)
            {
                Map map;
                // set map option visible: param = 1/0
                if (game.isNEOV2)
                {
                    map = Map.Instance;
                    map.VisibleOption = param;
                }
                else
                {
                    Console.WriteLine("sys_map_option only supports StudioNEOV2");
                }
            }
            public SystemData(VNNeoController game)
            {
                // export a dict contains all system status
                //from Studio import Studio
                //studio = Studio.Instance

                bgm = (game.studio.bgmCtrl.no, game.studio.bgmCtrl.play);
                if (game.isStudioNEO)
                {
                    env = (game.studio.envCtrl.no, game.studio.envCtrl.play);
                }
                wav = (game.studio.outsideSoundCtrl.fileName, game.studio.outsideSoundCtrl.play, game.studio.outsideSoundCtrl.repeat == BGMCtrl.Repeat.All);
                map = game.studio_scene.map;
                map_pos = game.studio_scene.caMap.pos;
                map_rot = game.studio_scene.caMap.rot;
                if (game.isCharaStudio)
                {
                    map_sun = game.studio_scene.sunLightType;
                }
                if (game.isCharaStudio || game.isNEOV2)
                {
                    map_opt = game.studio_scene.mapOption;
                }
                if (game.isPlayHomeStudio)
                {
                    skybox = game.studio_scene.skybox;
                }
                bg_png = game.scene_get_bg_png_orig();
                if (game.isCharaStudio || game.isNEOV2)
                {
                    fm_png = game.scene_get_framefile();
                }
                if (game.isStudioNEO)
                {
                    sc = game.studio_scene;
                    char_light = (sc.cameraLightColor.rgbDiffuse, sc.cameraLightIntensity, sc.cameraLightRot[0], sc.cameraLightRot[1], sc.cameraLightShadow);
                }
                else if (game.isPlayHomeStudio)
                {
                    sc = game.studio_scene;
                    char_light = (sc.cameraLightColor, sc.cameraLightIntensity, sc.cameraLightRot[0], sc.cameraLightRot[1], sc.cameraLightShadow, sc.cameraMethod);
                }
                else if (game.isCharaStudio || game.isNEOV2)
                {
                    var cl = game.studio_scene.charaLight;
                    char_light = (cl.color, cl.intensity, cl.rot[0], cl.rot[1], cl.shadow);
                }
                // external plugins data
                if (game.isStudioNEO)
                {
                    if (extplugins.ExtPlugin.exists("HSStudioNEOExtSave"))
                    {
                        if (is_ini_value_true("ExportSys_NeoExtSave"))
                        {
                            var pl_neoext = extplugins.HSStudioNEOExtSave();
                            pl_neoextsave = pl_neoext.ExtDataGet();
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
            }


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

        }
    }
}
