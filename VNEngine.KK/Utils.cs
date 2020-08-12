using Studio;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace VNEngine
{
    partial class Utils
    {

        public struct Button_s
        {
            public string label;
            public Action<VNController, int> btnCallFull;
            public int v;

            public Button_s(string title, Action<VNController, int> btnCallFull, int v) : this()
            {
                this.label = title;
                this.btnCallFull = btnCallFull;
                this.v = v;
            }
        }

        public static void sys_map_sun(CharaStudioController game, int param)
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

        public static CharaStudioController vngame_window_charastudio(List<Button_s> vnButtonsActionsStart)
        {
            //unity_util.clean_behaviors();
            //var game = unity_util.create_gui_behavior(CharaStudioController);
            return new CharaStudioController(vnButtonsActionsStart);
        }
    }
}
