using System;
using System.Collections.Generic;
using System.Text;
using static VNEngine.VNController;

namespace VNEngine
{
    public class GData
    {

        public struct ShortCut_s {
            internal Action command;
            string key;
        }

        //public object scScriptAll;
        //public object scScriptParams;
        //public object scScriptActions;
        //public object scACustomButtons;
        //public object scScriptExts;
        //public object scVer;
        public SkinBase sss_game_skin_saved;
        public SkinBase vnbupskin;
        public int scMaxState;
        public bool hook_update_allowed;
        public Dictionary<string, ShortCut_s> sc_shortcuts;
        internal Dictionary<string, Checkpoint> _check_list;

        public GData()
        {
        }
    }
}
