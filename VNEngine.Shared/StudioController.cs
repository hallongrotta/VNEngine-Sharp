using System;
using System.Collections.Generic;
using System.IO;
using Studio;
using UnityEngine;
using VNActor;
using static VNEngine.Utils;

namespace VNEngine
{
    public partial class StudioController : MonoBehaviour
    {

        public string sceneDir;

        protected Studio.Studio studio
        {
            get
            {
                var studio = Studio.Studio.Instance;
                return studio;
            }
        }

        public void Update()
        {
            return;
        }

        protected SceneInfo StudioScene => studio.sceneInfo;

        public string WavFileName => studio.outsideSoundCtrl.fileName;

        public List<Character> scene_get_all_females()
        {
            var ar = new List<Character>();
            var dobjctrl = studio.dicObjectCtrl;
            foreach (var key in dobjctrl.Keys)
            {
                var objctrl = dobjctrl[key];
                if (objctrl is OCICharFemale chara)
                {
                    var pctrl = new Character(chara);
                    ar.Add(pctrl);
                }
            }

            return ar;
        }

        public bool treenode_check_select(TreeNodeObject treenode)
        {
            return studio.treeNodeCtrl.CheckSelect(treenode);
        }

        public void SelectNothing()
        {
            studio.treeNodeCtrl.RemoveNode();
        }

        public void SelectObject(NeoOCI elem)
        {
            studio.treeNodeCtrl.SelectSingle(elem.treeNodeObject);
        }

        public List<Character> scene_get_all_males()
        {
            var ar = new List<Character>();
            var dobjctrl = studio.dicObjectCtrl;
            foreach (var key in dobjctrl.Keys)
            {
                var objctrl = dobjctrl[key];
                if (objctrl is OCICharMale chara)
                {
                    var pctrl = new Character(chara);
                    ar.Add(pctrl);
                }
            }

            return ar;
        }

        public List<OCIItem> scene_get_all_items_raw()
        {
            var ar = new List<OCIItem>();
            var dobjctrl = studio.dicObjectCtrl;
            foreach (var key in dobjctrl.Keys)
            {
                var objctrl = dobjctrl[key];
                if (objctrl is OCIItem item)
                {
                    var pctrl = objctrl;
                    ar.Add(item);
                }
            }

            return ar;
        }

        public List<Item> scene_get_all_items()
        {
            var ar = new List<Item>();
            var dobjctrl = studio.dicObjectCtrl;
            foreach (var key in dobjctrl.Keys)
            {
                var objctrl = dobjctrl[key];
                if (objctrl is OCIItem item) ar.Add(new Item(item));
            }

            return ar;
        }

        public List<Folder> scene_get_all_folders()
        {
            var ar = new List<Folder>();
            var dobjctrl = studio.dicObjectCtrl;
            foreach (var key in dobjctrl.Keys)
            {
                var objctrl = dobjctrl[key];
                if (objctrl is OCIFolder fld) ar.Add(new Folder(fld));
            }

            return ar;
        }

        public string scene_get_bg_png_orig()
        {
            return studio.sceneInfo.background;
        }

        public List<Item> SelectedItems
        {
            get
            {
                var mtreeman = studio.treeNodeCtrl;
                var ar = new List<Item>();
                foreach (var node in mtreeman.selectNodes)
                {
                    var oitem = NeoOCI.create_from_treenode(node);
                    if (oitem.objctrl is OCIItem)
                    {
                        var prop = (Item)oitem;
                        ar.Add(prop);
                    }
                }

                if (ar.Count > 0)
                    return ar;
                throw new Exception("No items selected");
            }
        }

        public Item SelectedItem
        {
            get
            {
                var mtreeman = studio.treeNodeCtrl;
                var ar = new List<Item>();
                foreach (var node in mtreeman.selectNodes)
                {
                    var oitem = NeoOCI.create_from_treenode(node);
                    if (oitem.objctrl is OCIItem)
                    {
                        var prop = (Item)oitem;
                        ar.Add(prop);
                    }
                }

                if (ar.Count > 0)
                    return ar[0];
                throw new Exception("No items selected");
            }
        }

        public List<List<Character>> SelectedChars
        {
            get
            {
                var mtreeman = studio.treeNodeCtrl;
                var ar = new List<Character>();
                foreach (var node in mtreeman.selectNodes)
                {
                    var ochar = NeoOCI.create_from_treenode(node);
                    if (ochar.objctrl is OCIChar)
                    {
                        var chara = (Character)ochar;
                        ar.Add(chara);
                    }
                }

                var am = new List<Character>();
                var af = new List<Character>();
                foreach (var chara in ar)
                    if (chara.Sex == 0)
                        am.Add(chara);
                    else
                        af.Add(chara);
                return new List<List<Character>>
                {
                    af,
                    am
                };
            }
        }

        public Character SelectedChar
        {
            get
            {
                var mtreeman = studio.treeNodeCtrl;
                var ar = new List<Character>();
                foreach (var node in mtreeman.selectNodes)
                {
                    var ochar = NeoOCI.create_from_treenode(node);
                    if (ochar.objctrl is OCIChar)
                    {
                        var chara = (Character)ochar;
                        ar.Add(chara);
                    }
                }

                return ar[0];
            }
        }

        public ObjectCtrlInfo get_objctrl_num(int num)
        {
            // return ObjectCtrlInfo object from dicObjectCtrl
            //si = self.studio_scene
            //dobj = si.dicObject
            var dobjctrl = studio.dicObjectCtrl;
            return dobjctrl[num];
        }

        public Prop GetProp(string id)
        {
            return SceneFolders.scenef_get_propf(id);
        }

        public Character GetActor(string id)
        {
            return SceneFolders.scenef_get_actor(id);
        }





        public Character get_objctrl_num_tochar(int num)
        {
            // return VNActor.Character by num
            return new Character((OCIChar)get_objctrl_num(num));
        }

        public bool scene_set_bg_png_orig(string filepng)
        {
            if (scene_get_bg_png_orig() != filepng)
            {
                var obj = (BackgroundCtrl)FindObjectOfType(typeof(BackgroundCtrl));
                return obj.Load(filepng);
            }

            return true;
        }

        public void sync_h(Character female, Character male)
        {
            // if factor.isHAnime:
            female.AnimationOption = new Character.AnimeOption_s { height = female.Height, breast = female.Breast };
            // if mactor.isHAnime:
            male.AnimationOption = new Character.AnimeOption_s { height = female.Height, breast = female.Breast };
        }

        public System.Wav_s WAV
        {
            set
            {
                {
                    string wavRevPath;
                    // set outside wav sound, value = (wav file, play, repeat)
                    var wavName = value.fileName.Trim();
                    if (wavName != "")
                    {
                        if (!wavName.ToLower().EndsWith(".wav")) wavName += ".wav";
                        // load wav in game scene folder if existed
                        var wavInScene = combine_path(SceneDir(), sceneDir, wavName);
                        if (File.Exists(wavInScene))
                        {
                            wavRevPath = combine_path("..", "studio", "scene", sceneDir, wavName);

                            if (studio.outsideSoundCtrl.fileName != wavRevPath)
                                studio.outsideSoundCtrl.Play(wavRevPath);
                        }
                        else
                        {
                            // load wav in game default audio folder if existed
                            var wavInDefault =
                                Path.GetFullPath(combine_path(Application.dataPath, "..", "UserData", "audio",
                                    wavName));
                            if (File.Exists(wavInDefault))
                                if (studio.outsideSoundCtrl.fileName != wavName)
                                    studio.outsideSoundCtrl.Play(wavName);
                        }
                    }

                    if (studio.outsideSoundCtrl.play != value.play || wavName == "")
                    {
                        if (value.play)
                            studio.outsideSoundCtrl.Play();
                        else
                            studio.outsideSoundCtrl.Stop();
                    }

                    if (value.repeat)
                        studio.outsideSoundCtrl.repeat = BGMCtrl.Repeat.All;
                    else
                        studio.outsideSoundCtrl.repeat = BGMCtrl.Repeat.None;
                }
            }
            get =>
                new System.Wav_s
                {
                    fileName = studio.outsideSoundCtrl.fileName,
                    play = studio.outsideSoundCtrl.play,
                    repeat = studio.outsideSoundCtrl.repeat == BGMCtrl.Repeat.All
                };
        }

        public System.BGM_s BGM
        {
            set
            {
                if (studio.bgmCtrl.no != value.no) studio.bgmCtrl.Play(value.no);
                if (studio.bgmCtrl.play == value.play) return;
                if (value.play)
                    studio.bgmCtrl.Play();
                else
                    studio.bgmCtrl.Stop();
            }
            get => new System.BGM_s {no = studio.bgmCtrl.no, play = studio.bgmCtrl.play};
        }

        public System.CharLight_s CharLight
        {
            set
            {
                var cl = StudioScene.charaLight;
                cl.color = value.rgbDiffuse;
                cl.intensity = value.cameraLightIntensity;
                cl.rot[0] = value.rot_y;
                cl.rot[1] = value.rot_x;
                cl.shadow = value.cameraLightShadow;
                studio.cameraLightCtrl.Reflect();
            }
            get
            {
                var cl = StudioScene.charaLight;
                return new System.CharLight_s
                {
                    rgbDiffuse = cl.color,
                    cameraLightIntensity = cl.intensity,
                    rot_y = cl.rot[0],
                    rot_x = cl.rot[1],
                    cameraLightShadow = cl.shadow
                };
            }
        }

        public Folder getFolder(string name, bool exact = false)
        {
            var flds = scene_get_all_folders();
            foreach (var fld in flds)
            {
                if (exact == false)
                {
                    if (fld.text_name.Contains(name))
                    {
                        return fld;
                    }
                }
                else if (name == fld.text_name)
                {
                    return fld;
                }
            }
            return null;
        }

        public string BackgroundImage
        {
            set
            {
                var pngName = value;
                if (value is null)
                {
                    value = "";
                }
                else
                {
                    var pngInDefault =
                        Path.GetFullPath(combine_path(Application.dataPath, "..", "UserData", "bg", pngName));
                    if (!File.Exists(pngInDefault)) pngName = "";
                }

                scene_set_bg_png_orig(pngName);
            }
        }
    }
}