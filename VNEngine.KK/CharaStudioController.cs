using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using Studio;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static VNEngine.Utils;

namespace VNEngine
{

    [BepInProcess(Constants.StudioProcessName)]
    //[BepInDependency(GUID)]
    [BepInPlugin(GUID, PluginName, Version)]
    public class CharaStudioController
        : VNNeoController
    {

        new public static CharaStudioController Instance { get; private set; }

        public const string PluginName = "VN Controller";
        public const string GUID = "com.kasanari.bepinex.vncontroller";
        public const string Version = "1.0";
        internal new static ManualLogSource Logger;



        public CharaStudioController() : base()
        {

            if (Instance != null)
            {
                throw new InvalidOperationException("Can only create one instance of Controller");
            }
            this.engine_name = "charastudio";
            Instance = this;
        }

        public CharaStudioController(List<Button_s> vnButtonsStart) : base()
        {
            if (Instance != null)
            {
                throw new InvalidOperationException("Can only create one instance of Controller");
            }
            this.engine_name = "charastudio";
            this._vnButtons = vnButtonsStart;
            Instance = this;

        }

        public static ConfigEntry<KeyboardShortcut> VNControllerHotkey { get; private set; }

        internal void Main()
        {
            //Logger = base.Logger;
            //VNControllerHotkey = Config.Bind("Keyboard Shortcuts", "Toggle VN Controller Window", new KeyboardShortcut(KeyCode.Plus), "Show or hide the VN Controller window in Studio");
            //CharacterApi.RegisterExtraBehaviour<AnimationControllerCharaController>(GUID);
            //StudioSaveLoadApi.RegisterExtraBehaviour<AnimationControllerSceneController>(GUID);
        }

        // --- support functions ----
        public override void load_scene(string file)
        {
            this._load_scene_before(file);
            var studio = Studio.Studio.Instance;
            this.change_map_to(-1);
            //studio.InitScene(False) # or init scene to false
            this.updFuncParam = file;
            this.updFunc = this.load_scene2;
            this.funcLockedText = "Loading scene...";
            this.isFuncLocked = true;
            //self.saveTChar = 
        }

        public void load_scene2(string file)
        {
            this.updFunc = this.load_scene3;
        }

        public void load_scene3(string file)
        {
            this.updFunc = this.load_scene_immediately;
        }

        public void load_scene_immediately(string file)
        {
            var studio = Studio.Studio.Instance;
            //return Path.Combine(get_scene_dir(),file)
            var fpath = Path.Combine(this.get_scene_dir(), this.sceneDir + file);
            studio.LoadScene(fpath);
            //self.change_map_to(-1)
            //self.change_map_to(studio.sceneInfo.map)
            //self.updFunc = self.testupdfunc
            // -------- this loading scene certainly work, but show scene unimmersive, step-by-step -----------
            //studio.StartCoroutine(studio.LoadSceneCoroutine(fpath))
            this.isFuncLocked = false;
        }

        public override string get_scene_dir()
        {
            return Path.GetFullPath(Path.Combine(Path.Combine(Path.Combine(Path.Combine(Application.dataPath, ".."), "UserData"), "Studio"), "scene"));
        }

        public void change_map_to(int mapnum)
        {
            var studio = Studio.Studio.Instance;
            studio.AddMap(mapnum, true, false, false);
        }

        // 
        //             try:
        //                 ffile = "..\\studio\\scene\\" + self.sceneDir + filepng
        //                 print ffile
        //                 from Studio import BackgroundCtrl
        //                 from UnityEngine import GameObject
        //                 #print self.studio.m_BackgroundCtrl.Load(ffile)
        //                 #for obj in GameObject.FindObjectOfType(BackgroundCtrl):
        //                 obj = GameObject.FindObjectOfType(BackgroundCtrl)
        //                 print obj.Load(ffile)
        //             except Exception, e:
        //                 print("Error: " + str(e))
        //             
        public void scene_set_bg_png(string filepng)
        {
            var ffile = "..\\studio\\scene\\" + this.sceneDir + filepng;
            this.scene_set_bg_png_orig(ffile);
        }

        public string scene_get_framefile()
        {
            return this.studio_scene.frame;
        }

        public bool scene_set_framefile(string ffile)
        {
            var obj = GameObject.FindObjectOfType<FrameCtrl>();
            return obj.Load(ffile);
        }

    }

}
