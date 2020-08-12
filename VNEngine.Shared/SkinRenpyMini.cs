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
            this.name = "skin_renpymini";
            this.contentHeight = 0.15f;
            //self.wheight = Screen.height / 4
            this.contentWidthProp = 0.7;
            this.calcWindowProp = Screen.width / 1920;
            this.labelFontSize = 28;
            this.buttonFontSize = 20;
            this.buttonHeight = 36;
            this.maxButtonsNormal = 3;
            this.maxButtonsCompact = 6;
            //boxstyle = GUIStyle("box");
            //boxstyle.
            this.isEndButton = false;
            this.endButtonTxt = ">>";
        }

        override public void setup(VNController controller)
        {
            //super(SkinDefault, self).setup(controller)
            this.controller = controller;
            this.wwidth = Screen.width;
            this.wheight = (int)(Screen.height * this.contentHeight);
            controller.wwidth = this.wwidth;
            controller.wheight = this.wheight;
            controller.windowName = "";
            controller.windowRect = new Rect(Screen.width / 2 - controller.wwidth / 2, Screen.height - controller.wheight, controller.wwidth, controller.wheight);
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
            GUI.Box(new Rect(-10, 0, this.wwidth + 10, this.wheight + 5), "");
            GUI.Box(new Rect(-10, 0, this.wwidth + 10, this.wheight + 5), "");
            GUI.Box(new Rect(-10, 0, this.wwidth + 10, this.wheight + 5), "");
            GUILayout.BeginHorizontal("", GUILayout.Width(this.wwidth));
            GUILayout.Space((float)(this.wwidth * (1 - this.contentWidthProp) / 2));
        }

        public void ren_end()
        {
            if (!this.isEndButton)
            {
                GUILayout.Space((float)(this.wwidth * (1 - this.contentWidthProp) / 2));
            }
            else
            {
                GUILayout.BeginVertical();
                //GUILayout.FlexibleSpace()
                GUILayout.Space(20);
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (GUILayout.Button(this.endButtonTxt))
                {
                    //print "AAAA!"
                    this.controller.call_game_func(this.endButtonCall);
                }
                GUILayout.Space(20);
                GUILayout.EndHorizontal();
                GUILayout.Space(20);
                GUILayout.EndVertical();
            }
            GUILayout.EndHorizontal();
            GUI.DragWindow();
        }

        override public void render_main(
            string text_author,
            string text,
            List<Button_s> btnsActions,
            string btnStyle)
        {
            RegisteredChar_s charinfo;
            //stylebox = GUIStyle("box")
            //stylebox.border = 0
            //GUI.Box(Rect(0, 0, Screen.width / 7, Screen.height / 1), "");
            this.ren_start();
            // --------- calculate actual author ------------
            var char0 = text_author.Split('/')[0];
            if (this.controller.registeredChars.ContainsKey(char0))
            {
                charinfo = this.controller.registeredChars[char0];
            }
            else
            {
                charinfo = new RegisteredChar_s("ffffff", char0);
            }
            // --------- render ---------------
            float fullw = (float)(this.wwidth * this.contentWidthProp);
            float fullw0 = this.wwidth * (float)(this.contentWidthProp - 0.2f);
            float fullw1 = this.wwidth * 0.2f;
            //GUILayout.BeginVertical("box1", GUILayout.Width(fullw), GUILayout.Height(self.wheight))
            //GUILayout.BeginVertical("box1", GUILayout.Width(fullw), GUILayout.Height(self.wheight))

            GUILayout.BeginVertical(GUILayout.Width(fullw0), GUILayout.Height(this.wheight));

            GUILayout.Space(10);
            // ---- preparing styles ----------
            var style = new GUIStyle("label");
            style.richText = true;
            style.fontSize = this.labelFontSize;
            style.wordWrap = true;
            var customButton = new GUIStyle("button");
            customButton.fontSize = this.buttonFontSize;
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
                GUILayout.Label(String.Format("<color=#{0}ff><b>{1}</b></color>", charinfo.color, charinfo.showname), style, GUILayout.Width(fullw0));
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
            GUILayout.BeginVertical(GUILayout.Width(fullw1), GUILayout.Height(this.wheight));
            GUILayout.FlexibleSpace();
            // ---- show buttons ---
            if (!this.controller.isHideGameButtons)
            {
                if (btnStyle == "compact")
                {
                    GUILayout.BeginHorizontal();
                }
                var isOneButton = false;
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
                    if (this.controller.GetConfigEntry("Skins", "usekeysforbuttons"))
                    {
                        if (this.controller.arKeyKodes.Length > i)
                        {
                            fintext = this.controller.arKeyKodes[i].ToUpper() + ": " + fintext;
                        }
                    }
                    // render button
                    if (btnStyle == "normal")
                    {
                        if (btnsActions.Count > 1)
                        {
                            if (GUILayout.Button(fintext, customButton, GUILayout.Width(fullw1), GUILayout.Height(this.buttonHeight)))
                            {
                                this.controller.call_game_func(btnsActions[i]);
                            }
                        }
                        else
                        {
                            // special case for 1 button
                            isOneButton = true;
                            GUILayout.BeginHorizontal();
                            GUILayout.FlexibleSpace();
                            customButton.fontSize = (int)(customButton.fontSize * 1.2);
                            if (GUILayout.Button(fintext, customButton, GUILayout.Width(fullw1), GUILayout.Height(this.buttonHeight * 1.4f)))
                            {
                                this.controller.call_game_func(btnsActions[i]);
                            }
                            GUILayout.EndHorizontal();
                        }
                    }
                    if (btnStyle == "compact")
                    {
                        if (GUILayout.Button(fintext, customButton, GUILayout.Width(fullw1 / 2 - 2), GUILayout.Height(this.buttonHeight)))
                        {
                            this.controller.call_game_func(btnsActions[i]);
                        }
                        if (i % 2 == 1)
                        {
                            GUILayout.EndHorizontal();
                            GUILayout.BeginHorizontal();
                            // pass
                        }
                    }
                }
                if (btnStyle == "compact")
                {
                    GUILayout.EndHorizontal();
                }
            }
            GUILayout.Space(16);
            GUILayout.EndVertical();
            this.ren_end();
        }

        override public void render_system(string sys_text)
        {
            this.ren_start();
            var fullw = (int)(this.wwidth * this.contentWidthProp);
            GUILayout.BeginVertical(GUILayout.Width(fullw));
            var style = new GUIStyle("label");
            style.richText = true;
            style.fontSize = this.labelFontSize;
            style.wordWrap = true;
            GUILayout.Label(sys_text, style, GUILayout.Width(fullw));
            GUILayout.EndVertical();
            this.ren_end();
        }


    }
}
