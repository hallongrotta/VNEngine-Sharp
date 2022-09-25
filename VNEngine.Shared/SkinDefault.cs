using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static VNEngine.Utils;
using static VNEngine.VNController;

namespace VNEngine
{
    public class SkinDefault : SkinBase
    {
        public int buttonFontSize;

        public int buttonHeight;

        public new VNController controller;

        public int labelFontSize;

        public string name;

        public int wheight;

        public int wwidth;

        public SkinDefault()
        {
            name = "skin_default";
            wwidth = 500;
            wheight = 230;
            labelFontSize = 16;
            buttonFontSize = 16;
            buttonHeight = 30;
        }

        public static SkinDefault get_skin()
        {
            return new SkinDefault();
        }

        public override void setup(VNController controller)
        {
            //super(SkinDefault, self).setup(controller)
            this.controller = controller;
            controller.wwidth = wwidth;
            controller.wheight = wheight;
            controller.windowName = "";
            controller.windowRect = new Rect(Screen.width / 2 - controller.wwidth / 2,
                Screen.height - controller.wheight - 10, controller.wwidth, controller.wheight);
            this.controller.windowStyle = this.controller.windowStyleDefault;
        }

        public override void render_main(
            string text_author,
            string text,
            List<Button_s> btnsActions,
            string btnStyle)
        {
            RegisteredChar_s charinfo;
            // --------- calculate actual author ------------
            var char0 = text_author.Split('/')[0];
            if (controller.registeredChars.ContainsKey(char0))
                charinfo = controller.registeredChars[char0];
            else
                charinfo = new RegisteredChar_s("ffffff", char0);
            // --------- render ---------------
            var fullw = wwidth - 30;
            GUILayout.BeginVertical(GUILayout.Width(fullw));
            var style = new GUIStyle("label");
            style.richText = true;
            style.fontSize = labelFontSize;
            style.wordWrap = true;
            var customButton = new GUIStyle("button");
            customButton.fontSize = buttonFontSize;
            if (charinfo.showname != "")
                GUILayout.Label(string.Format("<color=#%sff><b>%s</b></color>", charinfo.color, charinfo.showname),
                    style, GUILayout.Width(fullw));

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
            if (!controller.isHideGameButtons)
            {
                if (btnStyle == "compact") GUILayout.BeginHorizontal();
                foreach (var i in Enumerable.Range(0, btnsActions.Count))
                {
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
                    if (btnStyle == "normal")
                        if (GUILayout.Button(fintext, customButton, GUILayout.Width(fullw),
                                GUILayout.Height(buttonHeight)))
                            controller.call_game_func(btnsActions[i]);
                    if (btnStyle == "compact")
                    {
                        if (GUILayout.Button(fintext, customButton, GUILayout.Width(fullw / 2 - 2),
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

            GUILayout.EndVertical();
            GUI.DragWindow();
        }

        public override void render_system(string sys_text)
        {
            var fullw = wwidth - 30;
            GUILayout.BeginVertical(GUILayout.Width(fullw));
            var style = new GUIStyle("label");
            style.richText = true;
            style.fontSize = labelFontSize;
            style.wordWrap = true;
            GUILayout.Label(sys_text, style, GUILayout.Width(fullw));
            GUILayout.EndVertical();
            GUI.DragWindow();
        }
    }
}