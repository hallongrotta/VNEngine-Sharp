using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HarmonyLib;
using Studio;
using UnityEngine;
using VNActor;
using static VNEngine.Utils;
using static VNEngine.VNCamera;

namespace VNEngine
{
    public abstract class VNNeoController
        : VNController
    {
        //public Dictionary<string, VNActor.Character> _scenef_actors;

        //public Dictionary<string, Item> _scenef_props;

        public GameFunc runScAct;

        public VNNeoController()
        {
            Instance = this;
        }

        public static VNNeoController Instance { get; private set; }

        public static Studio.CameraControl.CameraData cameraData
        {
            get
            {
                var studio = Studio.Studio.Instance;
                var c = studio.cameraCtrl;
                var trav = Traverse.Create(c);
                var cdata = trav.Field("cameraData").GetValue<Studio.CameraControl.CameraData>();
                return cdata;
            }
        }

        public Dictionary<string, Character> AllActors => SceneFolders.AllActors;

        public Dictionary<string, Prop> AllProps => SceneFolders.AllProps;

        public Studio.Studio studio
        {
            get
            {
                var studio = Studio.Studio.Instance;
                return studio;
            }
        }

        public SceneInfo studio_scene => studio.sceneInfo;

        /*        public string calc_py_path()
                {
                    var rootfolder = Path.GetFullPath(Path.Combine(Application.dataPath, ".."));
                    // os.path.splitext(__file__)[0] + '.ini'
                    var pydirname = path.dirname(@__file__);
                    return path.relpath(pydirname, rootfolder);
                }
         */

        // 
        //         camobj = self.get_camera_num(camnum)
        //         self.move_camera_obj(camobj)
        //         
        public void to_camera(int camnum)
        {
            var studio = Studio.Studio.Instance;
            var si = studio.sceneInfo;
            var cdatas = si.cameraData;

            var cdata = cameraData;
            //var targetInfos = trav.Field("listBones");

            //CameraData cdata = c.cameraData;
            cdata.Copy(cdatas[camnum - 1]);
        }

        public override void move_camera_direct(CamData cam)
        {
            var cdata = cameraData;
            var c = studio.cameraCtrl;

            cdata.pos = cam.position;

            cdata.distance = cam.distance;

            cdata.rotate = cam.rotation;

            cdata.parse = cam.fov;
        }

        public override void move_camera_direct(Vector3 pos, Vector3 distance, Vector3 rotate, float fov)
        {
            var cd = new CamData(pos, rotate, distance, fov);
            move_camera_direct(cd);
        }

        public override void dump_camera()
        {
            hsneo_dump_camera();
        }

        public void hsneo_dump_camera()
        {
            using (var file =
                   new StreamWriter(@"dumppython.txt", true))
            {
                file.WriteLine("---DUMP! Camera----");
                //hs.HSCamera.dump()
                hsneo_dump_camera2(file);
                file.WriteLine("");
            }

            show_blocking_message_time("Camera position dumped!");
        }

        public void hsneo_dump_camera2(StreamWriter file)
        {
            var studio = Studio.Studio.Instance;
            var c = studio.cameraCtrl;
            var cdata = cameraData;
            file.WriteLine("game.move_camera(pos=%s, distance=%s, rotate=%s)", cdata.pos.ToString(),
                cdata.distance.ToString(), cdata.rotate.ToString());
            file.WriteLine("# for VN Scene Script %s", camera_calcstr_for_vnscene());
            file.WriteLine(
                "# other one: 'cam': {{ 'goto_pos': (({0:F3}, {1:F3}, {2:F3}), ({3:F3}, {4:F3}, {5:F3}), ({6:F3}, {7:F3}, {8:F3})) }}, ",
                cdata.pos.x, cdata.pos.y, cdata.pos.z, cdata.distance.x, cdata.distance.y, cdata.distance.z,
                cdata.rotate.x, cdata.rotate.y, cdata.rotate.z);
        }

        public string camera_calcstr_for_vnscene()
        {
            var st = 0;
            var cdata = cameraData;
            var s1 = string.Format("%s,%s,%s,23.0", cdata.pos.ToString(), cdata.distance.ToString(),
                cdata.rotate.ToString());
            return string.Format("a:%s:camo:%s", st.ToString(), s1.Replace("(", "").Replace(")", "").Replace(" ", ""));
        }

        public Prop GetProp(string id)
        {
            return SceneFolders.scenef_get_propf(id);
        }

        public Character GetActor(string id)
        {
            return SceneFolders.scenef_get_actor(id);
        }

        public CamData get_camera_num(int camnum)
        {
            Studio.CameraControl.CameraData cdata;
            var studio = this.studio;
            var si = studio.sceneInfo;
            var cdatas = si.cameraData;
            if (camnum == 0)
            {
                // 0 camera is current camera. It may be interested due to some reasons
                var c = studio.cameraCtrl;
                cdata = cameraData;
            }
            else
            {
                cdata = cdatas[camnum - 1];
            }

            var camobj = new CamData(cdata.pos, cdata.rotate, cdata.distance, cdata.parse);
            //print camobj
            return camobj;
        }

        public void anim_to_camera_num(float duration, int camnum, string style = "linear", GameFunc onCameraEnd = null)
        {
            this.anim_to_camera_obj(duration, this.get_camera_num(camnum), style, onCameraEnd);
        }

        public void reset()
        {
            _unload_scene_before();
            studio.InitScene(false);
        }

        public ObjectCtrlInfo get_objctrl_num(int num)
        {
            // return ObjectCtrlInfo object from dicObjectCtrl
            //si = self.studio_scene
            //dobj = si.dicObject
            var dobjctrl = studio.dicObjectCtrl;
            return dobjctrl[num];
        }

        public Character get_objctrl_num_tochar(int num)
        {
            // return VNActor.Character by num
            return new Character((OCIChar) get_objctrl_num(num));
        }

        public new List<Character> scene_get_all_females()
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

        public void _anim_to_camera_savecurrentpos()
        {
            CamData camobj = this.get_camera_num(0);
            this.camSPos = camobj.position;
            this.camSDir = camobj.distance;
            this.camSAngle = camobj.rotation;
            this.camSFOV = camobj.fov;
        }

        public void anim_to_camera(
            float duration,
            Vector3 pos = new Vector3(),
            Vector3 distance = new Vector3(),
            Vector3 rotate = new Vector3(),
            float fov = 23.0f,
            string style = "linear",
            GameFunc onCameraEnd = null)
        {
            var camobj = new CamData(pos, rotate, distance, fov);
            this.anim_to_camera_obj(duration, camobj, style, onCameraEnd);
        }

        public void anim_to_camera_obj(float duration, CamData camobj, string style = "linear", GameFunc onCameraEnd = null)
        {
            this._anim_to_camera_savecurrentpos();
            // print "Anim to cam 1"
            // print "Anim to cam 2"
            var camobjv = camobj;
            this.camTPos = camobjv.position;
            this.camTDir = camobjv.distance;
            this.camTAngle = camobjv.rotation;
            this.camTFOV = camobjv.fov;
            this.camAnimStyle = style;
            this.camAnimFullStyle = null;
            // camera animation one timer only
            animation_cam_timer(duration, onCameraEnd);
        }

        public void debug_print_all_chars()
        {
            var fems = this.scene_get_all_females();
            Console.WriteLine("-- Female scene chars: --");
            foreach (var i in Enumerable.Range(0, fems.Count))
            {
                Console.WriteLine(String.Format("{0}: {1}", i.ToString(), fems[i].text_name));
            }
            fems = this.scene_get_all_males();
            Console.WriteLine("-- Male scene chars: --");
            foreach (var i in Enumerable.Range(0, fems.Count))
            {
                Console.WriteLine(String.Format("{0}: {1}", i.ToString(), fems[i].text_name));
            }
            this.show_blocking_message_time("Debug: list of chars printed in console!");
        }

        public new List<Character> scene_get_all_males()
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

        public void vnscenescript_run_current(GameFunc onEnd, string startState = "0")
        {
            //print "Run current!"
            /*
            MenuFunc func = VNSceneScript.start_menu;
            this.run_menu(func, new Dictionary<string, string> {
                {
                    "mode",
                    "view"
                },
                {
                    "startState",
                    startState
                }
            }, onEnd);
            */
        }

        public void vnscenescript_run_filescene(string file, GameFunc onEnd)
        {
            runScAct = onEnd;
            load_scene(file);
            set_text_s("...");
            set_buttons_alt(new List<Button_s>());
            set_timer(0.5f, _vnscenescript_run_filescene);
        }

        public void _vnscenescript_run_filescene(VNController game)
        {
            set_timer(0.5f, _vnscenescript_run_filescene2);
        }

        public void _vnscenescript_run_filescene2(VNController game)
        {
            vnscenescript_run_current(runScAct);
        }

        public string scene_get_bg_png_orig()
        {
            return studio.sceneInfo.background;
        }

        public bool scene_set_bg_png_orig(string filepng)
        {
            if (scene_get_bg_png_orig() != filepng)
            {
                var obj = (BackgroundCtrl) FindObjectOfType(typeof(BackgroundCtrl));
                return obj.Load(filepng);
            }

            return true;
        }

        // -------- scene with framework ------------

        /*
        public void scenef_register_actorsprops()
        {
            foreach (var obj in StudioAPI.GetSelectedObjects())
            {
                if (obj is OCIItem item)
                {
                    string id = "prp" + this._scenef_props.Count;
                    this._scenef_props[id] = new Item(item); // TODO dont make a new one each time
                }
                else if (obj is OCIChar chara) 
                {
                    string id = "act" + this._scenef_actors.Count;
                    this._scenef_actors[id] = new Character(chara);
                }
            }
        }
        */


        /*
        public void scenef_register_actorsprops()
        {
            string actorTitle = "";
            Console.WriteLine("-- Framework: register actors and props start --");
            var game = this;
            // search for tag folder (-actor:,-prop:,-dlgFolder-) and load them into game automaticlly
            // so this function must be called AFTER SCENE HAD BE LOADED!!
            game._scenef_actors = new Dictionary<string, Character>
            {
            };
            game._scenef_props = new Dictionary<string, Item>
            {
            };
            // get all from scene
            var folders = game.scene_get_all_folders_raw();
            // load actors and props from -actor:/-prop: tag folder attach on char/item
            string actorAlias;
            string actorColor = "ffffff";
            string propAlias;
            HSNeoOCI hsobj;
            foreach (var fld in folders)
            {
                var ftn = fld.name;
                if (ftn.StartsWith("-actor:"))
                {
                    // analysis actor tag
                    var tagElements = ftn.Split(':');
                    if (tagElements.Length == 2)
                    {
                        actorAlias = tagElements[1];
                    }
                    else if (tagElements.Length == 3)
                    {
                        actorAlias = tagElements[1];
                        actorColor = tagElements[2];
                    }
                    else
                    {
                        actorAlias = tagElements[1];
                        actorColor = tagElements[2];
                        actorTitle = tagElements[3];
                    }
                    // register actor
                    try
                    {
                        var hsociChar = HSNeoOCI.create_from_treenode(fld.treeNodeObject.parent.parent.parent);
                        if (hsociChar is Character chara)
                        {
                            if (actorTitle is null)
                            {
                                actorTitle = hsociChar.text_name;
                            }
                            //game._scenef_actors[actorAlias] = Character(hsociChar.objctrl)
                            //adapted to multiple frameworks in 2.0
                            game._scenef_actors[actorAlias] = chara.as_actor;
                            if (actorColor is string)
                            {
                                game.register_char(actorAlias, actorColor, actorTitle);
                            }
                            else
                            {

                            }
                            Console.WriteLine("Registered actor: '" + Utils.to_roman(actorAlias) + "' as " + Utils.to_roman(actorTitle) + " (#" + actorColor.ToString() + ")");
                        }
                        else
                        {
                            Console.WriteLine("Error in register char tag (not correct child) <" + Utils.to_roman(ftn) + ">");
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("error in register char tag <" + Utils.to_roman(ftn) + ">: " + e.ToString());
                        continue;
                    }
                }
                else if (ftn.StartsWith("-prop:"))
                {
                    // analysis props tag

                    propAlias = ftn.Substring("-prop:".Length).Trim();
                    // register props
                    HSNeoOCI oci = HSNeoOCI.create_from_treenode(fld.treeNodeObject.parent);

                    if (oci is Item prop) {
                        game._scenef_props[propAlias] = prop;
                    }
                    
                    Console.WriteLine("Registered prop: '" + Utils.to_roman(propAlias) + "' as " + Utils.to_roman(oci.text_name));
                }
                else if (ftn.StartsWith("-propchild:"))
                {
                    // analysis props tag
                    propAlias = ftn.Substring("-propchild:".Length).Trim();
                    // register props
                    hsobj = HSNeoOCI.create_from_treenode(fld.treeNodeObject.child[0]);
                    game._scenef_props[propAlias] = (Item)hsobj;
                    Console.WriteLine("Registered prop: '" + Utils.to_roman(propAlias) + "' as " + Utils.to_roman(hsobj.text_name));
                }
                else if (ftn.StartsWith("-propgrandpa:"))
                {
                    // analysis props tag
                    propAlias = ftn.Substring("-propgrandpa:".Length).Trim();
                    // register props
                    hsobj = HSNeoOCI.create_from_treenode(fld.treeNodeObject.parent.parent);
                    game._scenef_props[propAlias] = (Item)hsobj;
                    Console.WriteLine("Registered prop: '" + Utils.to_roman(propAlias) + "' as " + Utils.to_roman(hsobj.text_name));
                }
            }
            Console.WriteLine("-- Framework: register actors and props end --");
        }
       */


        // ---- lip sync ------- TODO
        /*        new public void set_text(string character, string text)
                {
                    base.set_text(character, text);
                    if (this.isfAutoLipSync)
                    {
                        try
                        {
                            this._flipsync_text_handler(character, text);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Error in flipsync: " + e.ToString());
                        }
                    }
                }*/

        /*        public object _flipsync_text_handler(object character, object text)
                {
                    vngelipsync.flipsync_text_handler(character, text);
                }

                public object fake_lipsync_stop()
                {
                    vngelipsync.fake_lipsync_stop(this);
                }*/

        // --------- sync_h ---------
        public void sync_h(Character female, Character male)
        {
            // if factor.isHAnime:
            female.AnimationOption = new Character.AnimeOption_s {height = female.Height, breast = female.Breast};
            // if mactor.isHAnime:
            male.AnimationOption = new Character.AnimeOption_s {height = female.Height, breast = female.Breast};
        }
    }
}