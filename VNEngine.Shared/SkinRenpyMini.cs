using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static VNEngine.Utils;
using static VNEngine.VNController;

namespace VNEngine
{
    public class SkinRenPyMini
        : SkinBase
    {
        public int buttonFontSize;

        public int buttonHeight;

        public int calcWindowProp;

        public float contentHeight;

        public double contentWidthProp;

        public Action<VNNeoController> endButtonCall;

        public string endButtonTxt;

        public bool isEndButton;

        public int labelFontSize;

        public string name;

        public int wheight;

        public int wwidth;

        public SkinRenPyMini()
        {
            name = "skin_renpymini";
            contentHeight = 0.15f;
            //self.wheight = Screen.height / 4
            contentWidthProp = 0.7;
            calcWindowProp = Screen.width / 1920;
            labelFontSize = 28;
            buttonFontSize = 20;
            buttonHeight = 36;
            maxButtonsNormal = 3;
            maxButtonsCompact = 6;
            //boxstyle = GUIStyle("box");
            //boxstyle.
            isEndButton = false;
            endButtonTxt = ">>";
        }

        public override void setup(VNController controller)
        {
            //super(SkinDefault, self).setup(controller)
            this.controller = controller;
            wwidth = Screen.width;
            wheight = (int) (Screen.height * contentHeight);
            controller.wwidth = wwidth;
            controller.wheight = wheight;
            controller.windowName = "";
            controller.windowRect = new Rect(Screen.width / 2 - controller.wwidth / 2,
                Screen.height - controller.wheight, controller.wwidth, controller.wheight);
            //GUI.skin.panel.onActive.textColor
            var style = new GUIStyle("label");
            this.controller.windowStyle = style;
            //style.
            //GUI.skin.window = style
            //GUI.backgroundColor.a = 0.7
        }

        public void ren_start()
        {
            // this is an ugly way to draw gray alpha background
            GUI.Box(new Rect(-10, 0, wwidth + 10, wheight + 5), "");
            GUI.Box(new Rect(-10, 0, wwidth + 10, wheight + 5), "");
            GUI.Box(new Rect(-10, 0, wwidth + 10, wheight + 5), "");
            GUILayout.BeginHorizontal(GUILayout.Width(wwidth));
            GUILayout.Space((float) (wwidth * (1 - contentWidthProp) / 2));
        }

        public void ren_end()
        {
            if (!isEndButton)
            {
                GUILayout.Space((float) (wwidth * (1 - contentWidthProp) / 2));
            }
            else
            {
                GUILayout.BeginVertical();
                //GUILayout.FlexibleSpace()
                GUILayout.Space(20);
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (GUILayout.Button(endButtonTxt))
                    //print "AAAA!"
                    controller.call_game_func(endButtonCall);
                GUILayout.Space(20);
                GUILayout.EndHorizontal();
                GUILayout.Space(20);
                GUILayout.EndVertical();
            }

            GUILayout.EndHorizontal();
            GUI.DragWindow();
        }

        public override void render_main(
            string text_author,
            string text,
            List<Button_s> btnsActions,
            string btnStyle)
        {
            RegisteredChar_s charinfo;
            //stylebox = GUIStyle("box")
            //stylebox.border = 0
            //GUI.Box(Rect(0, 0, Screen.width / 7, Screen.height / 1), "");
            ren_start();
            // --------- calculate actual author ------------
            var char0 = text_author.Split('/')[0];
            if (controller.registeredChars.ContainsKey(char0))
                charinfo = controller.registeredChars[char0];
            else
                charinfo = new RegisteredChar_s("ffffff", char0);
            // --------- render ---------------
            var fullw = (float) (wwidth * contentWidthProp);
            var fullw0 = wwidth * (float) (contentWidthProp - 0.2f);
            var fullw1 = wwidth * 0.2f;
            //GUILayout.BeginVertical("box1", GUILayout.Width(fullw), GUILayout.Height(self.wheight))
            //GUILayout.BeginVertical("box1", GUILayout.Width(fullw), GUILayout.Height(self.wheight))

            GUILayout.BeginVertical(GUILayout.Width(fullw0), GUILayout.Height(wheight));

            GUILayout.Space(10);
            // ---- preparing styles ----------
            var style = new GUIStyle("label");
            style.richText = true;
            style.fontSize = labelFontSize;
            style.wordWrap = true;
            var customButton = new GUIStyle("button");
            customButton.fontSize = buttonFontSize;
            // ----------- render ---------
            /*
            if (btnStyle is List<string>)
            {
                // -------- custom render -----------
                // tuple is specific action
                if (btnStyle[0] == "function")
                {
                    try
                    {
                        btnStyle[1](this.controller, new Dictionary<object, object> {
                            {
                                "fwidth",
                                fullw},
                            {
                                "btnheight",
                                this.buttonHeight},
                            {
                                "btnstyle",
                                customButton},
                            {
                                "labelstyle",
                                style}});
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Error in call custom GUI buttons: " + e.ToString());
                    }
                }
            }
            else

            {
            */
            // --------- normal render ----------
            //GUILayout.BeginHorizontal()
            if (charinfo.showname != "")
            {
                GUILayout.Label(string.Format("<color=#{0}ff><b>{1}</b></color>", charinfo.color, charinfo.showname),
                    style, GUILayout.Width(fullw0));
                GUILayout.Space(0);
            }

            /*
            // -- special processing for translation --
            if (this.controller.engineOptions["usetranslator"] == "1" && this.controller.engineOptions["translatetexts"] == "1")
            {
                GUILayout.Label(translateText(text), style, GUILayout.Width(fullw0));
            }
            else 
            */

            GUILayout.Label(text, style, GUILayout.Width(fullw0));

            //GUILayout.FlexibleSpace()
            GUILayout.EndVertical();
            GUILayout.BeginVertical(GUILayout.Width(fullw1), GUILayout.Height(wheight));
            GUILayout.FlexibleSpace();
            // ---- show buttons ---
            if (!controller.isHideGameButtons)
            {
                if (btnStyle == "compact") GUILayout.BeginHorizontal();
                foreach (var i in Enumerable.Range(0, btnsActions.Count))
                {
                    // preparing button texts
                    var restext = btnsActions[i].label;
                    /*
                    if (this.controller.engineOptions["usetranslator"] == "1" && this.controller.engineOptions["translatebuttons"] == "1")
                    {
                        restext = translateText(restext);
                    }
                    */
                    var fintext = restext;
                    if (controller.GetConfigEntry("Skins", "usekeysforbuttons"))
                        if (controller.arKeyKodes.Length > i)
                            fintext = controller.arKeyKodes[i].ToUpper() + ": " + fintext;
                    // render button
                    if (btnStyle == "normal")
                    {
                        if (btnsActions.Count > 1)
                        {
                            if (GUILayout.Button(fintext, customButton, GUILayout.Width(fullw1),
                                    GUILayout.Height(buttonHeight))) controller.call_game_func(btnsActions[i]);
                        }
                        else
                        {
                            // special case for 1 button
                            GUILayout.BeginHorizontal();
                            GUILayout.FlexibleSpace();
                            customButton.fontSize = (int) (customButton.fontSize * 1.2);
                            if (GUILayout.Button(fintext, customButton, GUILayout.Width(fullw1),
                                    GUILayout.Height(buttonHeight * 1.4f))) controller.call_game_func(btnsActions[i]);
                            GUILayout.EndHorizontal();
                        }
                    }

                    if (btnStyle == "compact")
                    {
                        if (GUILayout.Button(fintext, customButton, GUILayout.Width(fullw1 / 2 - 2),
                                GUILayout.Height(buttonHeight))) controller.call_game_func(btnsActions[i]);
                        if (i % 2 == 1)
                        {
                            GUILayout.EndHorizontal();
                            GUILayout.BeginHorizontal();
                            // pass
                        }
                    }
                }

                if (btnStyle == "compact") GUILayout.EndHorizontal();
            }

            GUILayout.Space(16);
            GUILayout.EndVertical();
            ren_end();
        }

        public override void render_system(string sys_text)
        {
            ren_start();
            var fullw = (int) (wwidth * contentWidthProp);
            GUILayout.BeginVertical(GUILayout.Width(fullw));
            var style = new GUIStyle("label");
            style.richText = true;
            style.fontSize = labelFontSize;
            style.wordWrap = true;
            GUILayout.Label(sys_text, style, GUILayout.Width(fullw));
            GUILayout.EndVertical();
            ren_end();
        }
    }
}