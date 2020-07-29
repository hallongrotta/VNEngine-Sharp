using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using BepInEx;

namespace VNEngine
{

    public class BaseController
    {
        //private Component component;
        //private int counter;
        //private bool show_buttons;
        public bool visible;
        //private GUIStyle style;
        private CameraControl cameraControl;
        private Rect windowRect;
        private bool lastCameraState = true;
        private GameCursor gameCursor;
        public string windowName;
        //private GUI.WindowFunction windowCallback;
        //private bool isClassicStudio;
        protected Dictionary<string, string> engineOptions;
        protected string engine_name;
        protected string pygamepath;

        public BaseController()
        {
            //component = null; // will be assigned if exists as member
            //counter = 1;
            //show_buttons = false;
            visible = true;
            cameraControl = null;
            windowRect = new Rect(Screen.width / 2 - 50, Screen.height / 2 - 50, 400, 400);
            lastCameraState = true;
            gameCursor = null;
            windowName = "Window";
            //loading options
            Utils.parseIniFile();
            engineOptions = Utils.getEngineOptions();
            //print self.engineOptions;

        }

        public void EnableCamera(bool value) {
            if (this.lastCameraState != value)
            {
                lastCameraState = value;
                if (cameraControl)
                {
                    cameraControl.enabled = value;
                }
                gameCursor.enabled = value;
            }
        }



        public void ResetWindow(int windowid)
        {
            //Workaround mouse/camera issues when dragging window
            if (GUIUtility.hotControl == 0)
            {
                EnableCamera(true);
            }

            if (Event.current.type == EventType.MouseDown)
            {
                GUI.FocusControl("");
                GUI.FocusWindow(windowid);
                EnableCamera(false);
            }
        }



        public void Start()
        {

            try
            {
                if (!gameCursor) {
                    gameCursor = UnityEngine.Object.FindObjectOfType<GameCursor>();
                }
            }


            catch (Exception exception) {
                //Logger.LogError("VNGE: passable error in Start:" + exception); //TODO
            }
        }
        /*
        public void OnGUI()
        {
            if (!visible)
            {
                return;
            }
            else
            {
                windowRect = GUI.Window(0, windowRect, windowCallback, windowName);
            }
        }
        */

        /*
        public void Update()
        {

            // Update is called less so better place to check keystate

            if (Utils.checkKeyCode("hide")) {
                visible = !visible;
            }
        }
        */
    }
}
