using System;
using System.Collections.Generic;
using System.IO;
using BepInEx;
using Studio;
using UnityEngine;
using static VNEngine.Utils;

namespace VNEngine
{
    [BepInProcess(Constants.StudioProcessName)]
    //[BepInDependency(GUID)]
    [BepInPlugin(GUID, PluginName, Version)]
    public class NeoV2Controller
        : VNNeoController
    {

        new public static NeoV2Controller Instance { get; private set; }

        public NeoV2Controller() : base()
        {

            if (Instance != null)
            {
                throw new InvalidOperationException("Can only create one instance of Controller");
            }
            this.engine_name = "neov2";
            Instance = this;
        }

        public NeoV2Controller(List<Button_s> vnButtonsStart) : base()
        {
            this.engine_name = "neov2";
            this._vnButtons = vnButtonsStart;
        }

        // --- support functions ----
        override public void load_scene(string file)
        {
            this._load_scene_before(file);
            this.funcLockedText = "Loading scene...";
            this.isFuncLocked = true;
            this.updFuncParam = file;
            this.updFunc = this.load_scene2;
        }

        public void load_scene2(object file)
        {
            this.updFunc = this.load_scene_immediately;
        }

        public void load_scene_immediately(string file)
        {
            var studio = Studio.Studio.Instance;
            // return Path.Combine(get_scene_dir(),file)
            studio.LoadScene(Path.Combine(this.get_scene_dir(), this.sceneDir + file));
            this.isFuncLocked = false;
        }

        override public string get_scene_dir()
        {
            return Path.GetFullPath(Path.Combine(Application.dataPath, "..", "UserData", "Studio", "scene"));
        }

        public void scene_set_bg_png(object filepng)
        {
            var ffile = "..\\studio\\scene\\" + this.sceneDir + filepng;
            // print self.studio.m_BackgroundCtrl.Load(ffile)
            this.scene_set_bg_png_orig(ffile);
        }

        public string scene_get_framefile()
        {
            return this.studio_scene.frame;
        }

        public object scene_set_framefile(string ffile)
        {
            FrameCtrl obj = GameObject.FindObjectOfType<FrameCtrl>();
            return obj.Load(ffile);
        }
    }
}
