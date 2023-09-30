using System;
using System.Collections.Generic;
using UnityEngine;
using VNEngine;
using static SceneSaveState.SceneConsole;

namespace SceneSaveState
{
    public static partial class UI
    {

        

        public static int subwinindex = 0;

        internal static int ColumnWidth;
        internal const int RowHeight = 25;

        internal const int defaultWindowX = 1000;
        internal const int defaultWindowY = 70;

        internal static int windowindex = 0;

        internal static int WindowHeight = 550;
        internal static int WindowWidth = 550;

        public static Vector2 cam_scroll = new Vector2(0, 0);
        public static Vector2 fset_scroll = new Vector2(0, 0);
        public static Vector2 miniset_scroll = new Vector2(0, 0);
        public static Vector2 mset_scroll = new Vector2(0, 0);

        public static Vector2 scene_scroll = new Vector2(0, 0);
        public static Vector2 tracking_actors_scroll = new Vector2(0, 0);
        public static Vector2 tracking_props_scroll = new Vector2(0, 0);
        

        public static string[] consolenames = new string[] { "SceneSaveState" };
        public static string[] options = new string[] { "Edit", "Tracking", "Load/Save", "Advanced"};

        public const string SelectedTextColor = "#f24115";
        public const string NormalTextColor = "#f9f9f9";

        public delegate void WarningFunc();

        public struct Warning
        {
            public string msg;
            public object func_param;
            public bool single_op;
            public WarningFunc warningFunc;
            

            public Warning(string msg, bool v2, WarningFunc warningFunc) : this()
            {
                this.msg = msg;
                single_op = v2;
                this.warningFunc = warningFunc;
            }
        }

        
    }
}
