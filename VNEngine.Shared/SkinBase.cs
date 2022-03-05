using System.Collections.Generic;
using UnityEngine;
using static VNEngine.Utils;

namespace VNEngine
{
    public abstract class SkinBase
    {
        public VNController controller;
        public GUI.WindowFunction funcWindowGUI;

        public bool isCustomFuncWindowGUI;

        public int maxButtonsCompact;

        public int maxButtonsNormal;

        public SkinBase()
        {
            isCustomFuncWindowGUI = false;
            maxButtonsNormal = 5;
            maxButtonsCompact = 8;
        }

        // :type controller:VNController
        public abstract void setup(VNController controller);

        public abstract void render_system(string sys_text);

        public abstract void render_main(
            string text_author,
            string text,
            List<Button_s> btns,
            string btnStyle);
    }
}