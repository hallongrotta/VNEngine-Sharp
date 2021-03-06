﻿using System.Collections.Generic;
using UnityEngine;
using static VNEngine.Utils;

namespace VNEngine
{
    abstract public class SkinBase
    {

        public bool isCustomFuncWindowGUI;

        public int maxButtonsCompact;

        public int maxButtonsNormal;

        public SkinBase()
        {
            this.isCustomFuncWindowGUI = false;
            this.maxButtonsNormal = 5;
            this.maxButtonsCompact = 8;
        }

        // :type controller:VNController
        public abstract void setup(VNController controller);

        abstract public void render_system(string sys_text);

        abstract public void render_main(
            string text_author,
            string text,
            List<Button_s> btns,
            string btnStyle);

        public VNController controller;
        public GUI.WindowFunction funcWindowGUI;
    }

}
