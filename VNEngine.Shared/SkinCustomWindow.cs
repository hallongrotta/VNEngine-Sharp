using System;
using System.Collections.Generic;

namespace VNEngine
{
    public class SkinCustomWindow : SkinBase
    {

        public Action<VNController> funcSetup;

        //public Action<StudioController, int> funcWindowGUI;

        public string name;

        public SkinCustomWindow()
        {
            this.name = "skin_customwindow";
            this.isCustomFuncWindowGUI = true;
        }

        override public void setup(VNController controller)
        {
            this.controller = controller;
            //controller.call_game_func(this.funcSetup);
        }

        public void customWindowGUI(int windowid)
        {

            this.funcWindowGUI(windowid);

        }

        public override void render_main(string text_author, string text, List<Utils.Button_s> btnsActions, string btnStyle)
        {
            throw new NotImplementedException();
        }

        public override void render_system(string sys_text)
        {
            throw new NotImplementedException();
        }


    }
}
