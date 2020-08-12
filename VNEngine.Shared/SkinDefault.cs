using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static VNEngine.Utils;
using static VNEngine.VNController;

namespace VNEngine
{
    public class SkinDefault : SkinBase
    {

        public static SkinDefault get_skin()
        {
            return new SkinDefault();
        }

        public int buttonFontSize;

        public int buttonHeight;

        public int labelFontSize;

        public string name;

        public int wheight;

        public int wwidth;

        new public VNNeoController controller;

        public SkinDefault() : base()
        {
            this.name = "skin_default";
            this.wwidth = 500;
            this.wheight = 230;
            this.labelFontSize = 16;
            this.buttonFontSize = 16;
            this.buttonHeight = 30;
        }

        override public void setup(VNController controller)
        {
            //super(SkinDefault, self).setup(controller)
            this.controller = (VNNeoController)controller;
            controller.wwidth = this.wwidth;
            controller.wheight = this.wheight;
            controller.windowName = "";
            controller.windowRect = new Rect(Screen.width / 2 - controller.wwidth / 2, Screen.height - controller.wheight - 10, controller.wwidth, controller.wheight);
            this.controller.windowStyle = this.controller.windowStyleDefault;
        }

        override public void render_main(
            string text_author,
            string text,
            List<string> btnsTexts,
            List<ButtonFunc_s> btnsActions,
            string btnStyle)
        {
            RegisteredChar_s charinfo;
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
            var fullw = this.wwidth - 30;
            GUILayout.BeginVertical(GUILayout.Width(fullw));
            var style = new GUIStyle("label");
            style.richText = true;
            style.fontSize = this.labelFontSize;
            style.wordWrap = true;
            var customButton = new GUIStyle("button");
            customButton.fontSize = this.buttonFontSize;
            if (charinfo.showname != "")
            {
                GUILayout.Label(String.Format("<color=#%sff><b>%s</b></color>", charinfo.color, charinfo.showname), style, GUILayout.Width(fullw));
            }

            /*
            if (this.controller.engineOptions["usetranslator"] == "1" && this.controller.engineOptions["translatetexts"] == "1")
            {
                GUILayout.Label(translateText(text), style, GUILayout.Width(fullw));
            }
            */

            GUILayout.Label(text, style, GUILayout.Width(fullw));

            GUILayout.FlexibleSpace();
            /*
                if (btnStyle is string[])
                {
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
                }*/
            if (!this.controller.isHideGameButtons)
            {
                if (btnStyle == "compact")
                {
                    GUILayout.BeginHorizontal();
                }
                foreach (var i in Enumerable.Range(0, btnsTexts.Count))
                {
                    var restext = btnsTexts[i];
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
                    if (btnStyle == "normal")
                    {
                        if (GUILayout.Button(fintext, customButton, GUILayout.Width(fullw), GUILayout.Height(this.buttonHeight)))
                        {
                            this.controller.call_game_func(btnsActions[i]);
                        }
                    }
                    if (btnStyle == "compact")
                    {
                        if (GUILayout.Button(fintext, customButton, GUILayout.Width(fullw / 2 - 2), GUILayout.Height(this.buttonHeight)))
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
            GUILayout.EndVertical();
            GUI.DragWindow();
        }

        override public void render_system(string sys_text)
        {
            var fullw = this.wwidth - 30;
            GUILayout.BeginVertical(GUILayout.Width(fullw));
            var style = new GUIStyle("label");
            style.richText = true;
            style.fontSize = this.labelFontSize;
            style.wordWrap = true;
            GUILayout.Label(sys_text, style, GUILayout.Width(fullw));
            GUILayout.EndVertical();
            GUI.DragWindow();
        }

        public void render_dev_console()
        {
            // require implement only in SkinDefault
            var fullw = this.wwidth - 30;
            GUILayout.BeginVertical(GUILayout.Width(fullw));
            // guistyle1 = GUIStyle;
            // guistyle1.wordwrap = True;
            // GUILayout.Label("my test text bla-bla-bla ake a repeating button. The button returns true as long as the user holds down the mo", guistyle1, GUILayout.Width(260))
            var style = new GUIStyle("label");
            style.richText = true;
            style.fontSize = this.labelFontSize;
            style.wordWrap = true;
            var customButton = new GUIStyle("button");
            customButton.fontSize = this.buttonFontSize;
            //GUILayout.Label(sys_text, style, GUILayout.Width(fullw))
            GUILayout.Label("<color=#ffaaaaff><b>Developer console</b></color>", style, GUILayout.Width(fullw));
            GUILayout.FlexibleSpace();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Dump camera", customButton, GUILayout.Width(fullw / 2 - 2), GUILayout.Height(this.buttonHeight)))
            {
                this.controller.dump_camera();
            }
            if (GUILayout.Button("Dump scene", customButton, GUILayout.Width(fullw / 2 - 2), GUILayout.Height(this.buttonHeight)))
            {
                this.controller.dump_scene();
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("List chars > console ", customButton, GUILayout.Width(fullw), GUILayout.Height(this.buttonHeight)))
            {
                this.controller.debug_print_all_chars();
            }
            GUILayout.EndHorizontal();
            if (!this.controller.isClassicStudio)
            {
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Dump selected item/folder tree", customButton, GUILayout.Width(fullw), GUILayout.Height(this.buttonHeight)))
                {
                    Utils.dump_selected_item_tree();
                    this.controller.show_blocking_message_time("Tree dumped!");
                }
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("VNFrame scene dump", customButton, GUILayout.Width(fullw / 2 - 2), GUILayout.Height(this.buttonHeight)))
                {
                    this.controller.dump_scene_vnframe(controller);
                    // self.show_blocking_message_time("VNFrame scene dumped!")
                }
                if (GUILayout.Button("VNFActor select dump", customButton, GUILayout.Width(fullw / 2 - 2), GUILayout.Height(this.buttonHeight)))
                {
                    this.controller.dump_selected_vnframe(controller);
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();
            GUI.DragWindow();
        }
    }
}

