using Studio;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace VNEngine
{
    partial class Utils
    {

        public struct ButtonFunc_s
        {
            internal Action<object> func;
            internal object param;
            private Action<VNController, int> btnCallFull;
            private int v;

            public ButtonFunc_s(Action<VNController, int> btnCallFull, int v) : this()
            {
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

        public static CharaStudioController vngame_window_charastudio(List<string> vnButtonsStart, List<ButtonFunc_s> vnButtonsActionsStart)
        {
            //unity_util.clean_behaviors();
            //var game = unity_util.create_gui_behavior(CharaStudioController);
            return new CharaStudioController(vnButtonsStart, vnButtonsActionsStart);
        }

        public static bool vngame_window_autogames_uni()
        {
            string dpath = Application.dataPath;
            string[] ar = dpath.Split('/');
            string gameId = ar[ar.Length - 1];

            if (gameId == "CharaStudio_Data")
            {
                Console.WriteLine("VNGE", vnge_version, "starting...");
                vngame_window_charastudio(new List<string> {
                "autogames"
            }, new List<ButtonFunc_s>());
                Console.WriteLine("VNGE", vnge_version, "inited!");
                return true;
            }
            Console.WriteLine("VN Game engine is not for this EXE file");
            return false;
        }

    }
}
