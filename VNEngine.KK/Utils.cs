using Studio;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace VNEngine
{
    partial class Utils
    {
        public static void sys_map_sun(StudioController game, int param)
        {
            // set sunLightType, param = sunLightType index, CharaStudio only
            var st = new SunLightInfo.Info.Type[] { SunLightInfo.Info.Type.DayTime, SunLightInfo.Info.Type.Evening, SunLightInfo.Info.Type.Night };
            var map = Map.Instance;
            map.sunType = st[param];
        }

        public static StudioController vngame_window_charastudio(List<Button_s> vnButtonsActionsStart)
        {
            //unity_util.clean_behaviors();
            //var game = unity_util.create_gui_behavior(CharaStudioController);
            return new StudioController(vnButtonsActionsStart);
        }
    }
}
