using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using UnityEngine;

namespace VNEngine
{
    public class NeoController
        : VNNeoController
    {

        public NeoController() : base()
        {
            this.engine_name = "neo";
            this.pygamepath = "Plugins\\Console\\Lib";
            //this._vnButtons = vnButtonsStart;
            //this._vnButtonsActions = vnButtonsActionsStart;
        }

        // --- support functions ----
        public virtual void load_scene(string file)
        {
            this._load_scene_before(file);
            this.funcLockedText = "Loading scene...";
            this.isFuncLocked = true;
            this.updFuncParam = file;
            this.updFunc = this.load_scene2;
        }

        public virtual void load_scene2(string file)
        {
            this.updFunc = this.load_scene_immediately;
        }

        public virtual void load_scene_immediately(string file)
        {
            var studio = Studio.Studio.Instance;
            //return Path.Combine(get_scene_dir(),file)
            studio.LoadScene(Path.Combine(this.get_scene_dir(), this.sceneDir + file));
            this.isFuncLocked = false;
        }

        public virtual string get_scene_dir()
        {
            return Path.GetFullPath(Path.Combine(Application.dataPath, "..", "UserData", "studioneo", "scene"));
        }

        public virtual void scene_set_bg_png(string filepng)
        {
            var ffile = "..\\studioneo\\scene\\" + this.sceneDir + filepng;
            //print self.studio.m_BackgroundCtrl.Load(ffile)
            this.scene_set_bg_png_orig(ffile);
            // def Update(self):
            //     VNNeoController.Update(self)
            //     print "Upd!"
            // def OnDestroy(self):
            //     print "OnDestroy NEO!"
            //     import coroutine
            //     coroutine.start_new_coroutine(vngame_window_autogames_uni, (), None)
        }
    }

}
