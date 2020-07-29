using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using UnityEngine;
using Studio;

namespace VNEngine
{
    public class NeoV2Controller
        : VNNeoController
    {

        public NeoV2Controller(List<string> vnButtonsStart, List<(Action<object>, object)> vnButtonsActionsStart) : base()
        {
            this.engine_name = "neov2";        
            this._vnButtons = vnButtonsStart;
            this._vnButtonsActions = vnButtonsActionsStart;
        }

        // --- support functions ----
        public void load_scene(string file)
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

        public string get_scene_dir()
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
