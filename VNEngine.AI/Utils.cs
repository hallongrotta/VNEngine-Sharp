using System;
using System.Collections.Generic;

namespace VNEngine
{
    class Utils
    {
        public static object vngame_window_neov2(object vnButtonsStart, object vnButtonsActionsStart)
        {
            //unity_util.clean_behaviors();
            //var game = unity_util.create_gui_behavior(NeoV2Controller);
            return new NeoV2Controller(vnButtonsStart, vnButtonsActionsStart);
        }

        public static bool vngame_window_autogames_uni()
        {
            string dpath = Application.dataPath;
            string[] ar = dpath.Split('/');
            string gameId = ar[ar.Length - 1];

            if (gameId == "StudioNEOV2_Data")
            {
                Console.WriteLine("VNGE", vnge_version, "starting...");
                vngame_window_neov2(new List<string> {
                "autogames"
            }, new List<object>());
                Console.WriteLine("VNGE", vnge_version, "inited!");
                return true;
            }
            Console.WriteLine("VN Game engine is not for this EXE file");
            return false;
        }
    }
}
