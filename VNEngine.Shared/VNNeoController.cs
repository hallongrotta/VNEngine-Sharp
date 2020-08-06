using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using VNActor;
using Studio;
using HarmonyLib;
using System.IO;
using Manager;
using ADV.EventCG;
using static VNEngine.VNCamera;
using KKAPI.Studio;
using System.Linq;

namespace VNEngine
{
    public abstract class VNNeoController
        : VNController
    {

        public static string actor_folder_prefix = "vnactor:";
        public static string prop_folder_prefix = "vnprop:";

        //public Dictionary<string, Actor> _scenef_actors;

        //public Dictionary<string, Prop> _scenef_props;

        public GameFunc runScAct;
        private int? scLastRunnedState;
        public VNNeoController() : base()
        {
           
        }

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
            if (this.isCharaStudio)
            {
                /* TODO
                // old code simulating key press
                var ar = new List<VirtualKeyCode> {
                    VirtualKeyCode.VK_1,
                    VirtualKeyCode.VK_2,
                    VirtualKeyCode.VK_3,
                    VirtualKeyCode.VK_4,
                    VirtualKeyCode.VK_5,
                    VirtualKeyCode.VK_6,
                    VirtualKeyCode.VK_7,
                    VirtualKeyCode.VK_8,
                    VirtualKeyCode.VK_9,
                    VirtualKeyCode.VK_0
                };
                InputSimulator inputSimulator = new InputSimulator();
                inputSimulator.Keyboard.KeyPress(ar[camnum - 1]);
                // enable it due to fucking reason - in CharaStudio camera not always setting at correct position
                */
            }
            else
            {
                Studio.Studio studio = Studio.Studio.Instance;
                SceneInfo si = studio.sceneInfo;
                Studio.CameraControl.CameraData[] cdatas = si.cameraData;

                Studio.CameraControl.CameraData cdata = cameraData;
                //var targetInfos = trav.Field("listBones");

                //CameraData cdata = c.cameraData;
                cdata.Copy(cdatas[camnum - 1]);
            }
        }

        public static Studio.CameraControl.CameraData cameraData {
            get
            {
                Studio.Studio studio = Studio.Studio.Instance;
                Studio.CameraControl c = studio.cameraCtrl;
                Traverse trav = Traverse.Create(c);
                Studio.CameraControl.CameraData cdata = trav.Field("cameraData").GetValue<Studio.CameraControl.CameraData>();
                return cdata;
            }
        }

        override public void move_camera_direct(CamData cam)
        {
            Studio.CameraControl.CameraData cdata = cameraData;
            Studio.CameraControl c = studio.cameraCtrl;

            cdata.pos = cam.position;       

            cdata.distance = cam.distance;

            cdata.rotate = cam.rotation;         

            cdata.parse = cam.fov;             
            
        }

        override public void move_camera_direct(Vector3? pos = null, Vector3? distance = null, Vector3? rotate = null, float? fov = null)
        {
            Studio.CameraControl.CameraData cdata = cameraData;
            Studio.CameraControl c = studio.cameraCtrl;

            if (pos != null)
            {
                cdata.pos = (Vector3)pos;
            }
            if (distance != null)
            {
                cdata.distance = (Vector3)distance;
            }
            if (rotate != null)
            {
                cdata.rotate = (Vector3)rotate;
            }
            if (fov != null)
            {
                if (c.fieldOfView != fov)
                {
                    c.fieldOfView = (float)fov;
                }
            }
        }

        public override void dump_camera()
        {
            hsneo_dump_camera();
        }

        public void hsneo_dump_camera()
        {

            using (StreamWriter file =
                new StreamWriter(@"dumppython.txt", true))
            {
                file.WriteLine("---DUMP! Camera----");
                //hs.HSCamera.dump()
                this.hsneo_dump_camera2(file);
                file.WriteLine("");
            }

            this.show_blocking_message_time("Camera position dumped!");

        }

        public void hsneo_dump_camera2(StreamWriter file)
        {
            Studio.Studio studio = Studio.Studio.Instance;
            Studio.CameraControl c = studio.cameraCtrl;
            Studio.CameraControl.CameraData cdata = cameraData;
            file.WriteLine(String.Format("game.move_camera(pos=%s, distance=%s, rotate=%s)", cdata.pos.ToString(), cdata.distance.ToString(), cdata.rotate.ToString()));
            file.WriteLine(String.Format("# for VN Scene Script %s", this.camera_calcstr_for_vnscene()));
            file.WriteLine(String.Format("# other one: 'cam': {{ 'goto_pos': (({0:F3}, {1:F3}, {2:F3}), ({3:F3}, {4:F3}, {5:F3}), ({6:F3}, {7:F3}, {8:F3})) }}, ", cdata.pos.x, cdata.pos.y, cdata.pos.z, cdata.distance.x, cdata.distance.y, cdata.distance.z, cdata.rotate.x, cdata.rotate.y, cdata.rotate.z));
        }

        public string camera_calcstr_for_vnscene()
        {
            var st = 0;
            if (this.scLastRunnedState != null)
            {
                st = (int)this.scLastRunnedState;
            }
            var cdata = cameraData;
            var s1 = String.Format("%s,%s,%s,23.0", cdata.pos.ToString(), cdata.distance.ToString(), cdata.rotate.ToString());
            return String.Format("a:%s:camo:%s", st.ToString(), s1.Replace("(", "").Replace(")", "").Replace(" ", ""));
        }

        public void dump_scene_vnframe(VNController game)
        {
            IDataClass status;
            var output = "";
            this.LoadTrackedActorsAndProps();
            Dictionary<string, Actor> actors = this.scenef_get_all_actors();
            string id_global = "";
            try
            {
                foreach (var id in actors.Keys)
                {
                    id_global = id;
                    Actor actor = (Actor)this.scenef_get_actor(id);
                    status = actor.export_full_status();
                    //output += String.Format("'%s': ", id) + VNFrame.script2string(status) + ",\n"; TODO
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in status: ", e);
                game.show_blocking_message_time(String.Format("Error during dump actor %s!", id_global));
                return;
            }
            var props = this.scenef_get_all_props();
            try
            {
                foreach (var id in props.Keys)
                {
                    id_global = id;
                    Prop prop = this.scenef_get_propf(id);
                    status = prop.export_full_status();
                    //output += String.Format("'%s': ", id) + VNFrame.script2string(status) + ",\n"; TODO
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in status: ", e);
                game.show_blocking_message_time(String.Format("Error during dump prop %s!", id_global));
                return;
            }
            //output += "'sys': " + VNFrame.script2string(System.export_sys_status(this)) + ",\n"; TODO add vnframe
            var c = this.studio.cameraCtrl;
            var cdata = cameraData;
            output += String.Format("'cam': {{'goto_pos': (({0:F3}, {1:F3}, {2:F3}), ({3:F3}, {4:F3}, {5:F3}), ({6:F3}, {7:F3}, {8:F3}))}}\n", cdata.pos.x, cdata.pos.y, cdata.pos.z, cdata.distance.x, cdata.distance.y, cdata.distance.z, cdata.rotate.x, cdata.rotate.y, cdata.rotate.z);
            output = "{\n" + output + "}\n";
            try
            {
                using (StreamWriter file =
                    new StreamWriter(@"dumppython.txt", true))
                {
                    file.Write(output);
                    file.Write('\n');
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            this.show_blocking_message_time("VNFrame Scene dumped!");
        }

        public void dump_selected_vnframe(VNController game)
        {
            var output = "";
            string id = "";
            try
            {
                HSNeoOCIChar fem = (HSNeoOCIChar)HSNeoOCI.create_from_selected();
                var actor = (Actor)fem;
                id = actor.text_name;
                var status = actor.export_full_status();
                /* TODO
                output += VNFrame.script2string(new Dictionary<object, object> {
                    {
                        "selected",
                        status}}) + "\n";
                */
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in status: ", e);
                game.show_blocking_message_time(String.Format("Error during dump actor!", id));
                return;
            }
            try
            {
                using (StreamWriter file =
                    new StreamWriter(@"dumppython.txt", true))
                {
                    file.Write(output);
                    file.Write('\n');
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            this.show_blocking_message_time("VNFrame selected dumped!");
        }

        new public VNCamera.CamData get_camera_num(int camnum)
        {
            Studio.CameraControl.CameraData cdata;
            Studio.Studio studio = this.studio;
            SceneInfo si = studio.sceneInfo;
            Studio.CameraControl.CameraData[] cdatas = si.cameraData;
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
            var camobj = this.camparams2vec(cdata.pos, cdata.distance, cdata.rotate, cdata.parse);
            //print camobj
            return camobj;
        }

        public void reset()
        {
            this._unload_scene_before();
            this.studio.InitScene(false);
        }

        public Studio.Studio studio
        {
            get
            {
                var studio = Studio.Studio.Instance;
                return studio;
            }
        }

        public SceneInfo studio_scene
        {
            get
            {
                return this.studio.sceneInfo;
            }
        }

        public void hsneo_dump_scene()
        {

            using (StreamWriter file =
                new StreamWriter(@"dumppython.txt", true))
            {
                file.WriteLine("---DUMP! Scene----");
                this.hsneo_dump_scene2(file);
                file.WriteLine("");
            }
        }

        public void hsneo_dump_scene2(StreamWriter file)
        {
            //print("Dumping scene 1!")
            //si = self.studio_scene
            var dobjctrl = this.studio.dicObjectCtrl;
            //print("Dumping scene 2!")
            file.WriteLine("# we are not dumping objects because of number... but you can enable it in code of hsneo_dump_scene2");
            foreach (var key in dobjctrl.Keys)
            {
                var objctrl = dobjctrl[key];
                //print(key)
                if (objctrl is OCIChar chara)
                {
                    file.WriteLine(String.Format("objctrlchar = game.get_objctrl_num_tochar(%s) # char name %s, animid=%s", key, Utils.to_roman_file(objctrl.treeNodeObject.textName), Utils.to_roman_file(chara.charAnimeCtrl.name)));
                    //print("objctrlchar = game.get_objctrl_num_tochar(%s) # char name" % (key))
                    // objctrl.charAnimeCtrl.name
                    var pctrl = new Actor(chara);
                    pctrl.dump_obj();
                }
                else
                {
                    //uncomment here to dump not only chars in scene
                    //print("objctrl = game.get_objctrl_num(%s)"%(key))
                    //objctrl = objctrl;
                    //print key
                    //print("Dumping scene End!")
                }
            }
            this.show_blocking_message_time("Scene dumped!");
        }

        
        public override void dump_scene()
        {
            if (this.onDumpSceneOverride != null)
            {
                this.onDumpSceneOverride(this);
            }
            else
            {
                this.hsneo_dump_scene();
            }
        } 

        public ObjectCtrlInfo get_objctrl_num(int num)
        {
            // return ObjectCtrlInfo object from dicObjectCtrl
            //si = self.studio_scene
            //dobj = si.dicObject
            var dobjctrl = this.studio.dicObjectCtrl;
            return dobjctrl[num];
        }

        public HSNeoOCIChar get_objctrl_num_tochar(int num)
        {
            // return HSNeoOCIChar by num
            return new Actor((OCIChar)this.get_objctrl_num(num));
        }

        new public List<HSNeoOCIChar> scene_get_all_females()
        {
            var ar = new List<HSNeoOCIChar>();
            var dobjctrl = this.studio.dicObjectCtrl;
            foreach (var key in dobjctrl.Keys)
            {
                var objctrl = dobjctrl[key];
                if (objctrl is OCICharFemale chara)
                {
                    var pctrl = new Actor(chara);
                    ar.Add(pctrl);
                }
            }
            return ar;
        }

        new public List<HSNeoOCIChar> scene_get_all_males()
        {
            var ar = new List<HSNeoOCIChar>();
            var dobjctrl = this.studio.dicObjectCtrl;
            foreach (var key in dobjctrl.Keys)
            {
                var objctrl = dobjctrl[key];
                if (objctrl is OCICharMale chara)
                {
                    var pctrl = new Actor(chara);
                    ar.Add(pctrl);
                }
            }
            return ar;
        }

        public List<OCIItem> scene_get_all_items_raw()
        {
            var ar = new List<OCIItem>();
            var dobjctrl = this.studio.dicObjectCtrl;
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

        public List<HSNeoOCIItem> scene_get_all_items()
        {
            var ar = new List<HSNeoOCIItem>();
            var dobjctrl = this.studio.dicObjectCtrl;
            foreach (var key in dobjctrl.Keys)
            {
                var objctrl = dobjctrl[key];
                if (objctrl is OCIItem item)
                {
                    ar.Add(new Prop(item));
                }
            }
            return ar;
        }

        public List<OCIFolder> scene_get_all_folders_raw()
        {
            var ar = new List<OCIFolder>();
            var dobjctrl = this.studio.dicObjectCtrl;
            foreach (OCIFolder folder in dobjctrl.Values.OfType<OCIFolder>())
            {
               ar.Add(folder);
            }
            return ar;
        }

        public List<HSNeoOCIFolder> scene_get_all_folders()
        {
            var ar = new List<HSNeoOCIFolder>();
            var dobjctrl = this.studio.dicObjectCtrl;
            foreach (var key in dobjctrl.Keys)
            {
                var objctrl = dobjctrl[key];
                if (objctrl is OCIFolder fld)
                {
                    ar.Add(new HSNeoOCIFolder(fld));
                }
            }
            return ar;
        }



        public void vnscenescript_run_current(GameFunc onEnd, string startState = "0")
        {
            //print "Run current!"
            /* TODO
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
            this.runScAct = onEnd;
            this.load_scene(file);
            this.set_text_s("...");
            //this.set_buttons_alt(new List<string>()); TODO
            this.set_timer(0.5f, this._vnscenescript_run_filescene);
        }

        public void _vnscenescript_run_filescene(VNController game)
        {
            this.set_timer(0.5f, this._vnscenescript_run_filescene2);
        }

        public void _vnscenescript_run_filescene2(VNController game)
        {
            this.vnscenescript_run_current(this.runScAct);
        }

        public string scene_get_bg_png_orig()
        {
            return this.studio.sceneInfo.background;
        }

        public bool scene_set_bg_png_orig(string filepng)
        {
            if (this.scene_get_bg_png_orig() != filepng)
            {
                // return self.studio.sceneInfo.background
                // print self.studio.m_BackgroundCtrl.Load(ffile)
                // for obj in GameObject.FindObjectOfType(BackgroundCtrl):
                BackgroundCtrl obj = (BackgroundCtrl)GameObject.FindObjectOfType(typeof(BackgroundCtrl));
                return obj.Load(filepng);
                // print self.studio.m_BackgroundCtrl.Load(ffile)
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
                    this._scenef_props[id] = new Prop(item); // TODO dont make a new one each time
                }
                else if (obj is OCIChar chara) 
                {
                    string id = "act" + this._scenef_actors.Count;
                    this._scenef_actors[id] = new Actor(chara);
                }
            }
        }
        */

        public void LoadTrackedActorsAndProps()
        {
            List<OCIFolder> folders = scene_get_all_folders_raw();
            _scenef_actors = new Dictionary<string, Actor>();
            _scenef_props = new Dictionary<string, Prop>();

            foreach (OCIFolder fld in folders)
            {
                string fldName = fld.name;
                if (fldName.StartsWith(actor_folder_prefix))
                {

                    var hsociChar = HSNeoOCI.create_from_treenode(fld.treeNodeObject.parent.parent.parent);

                    if (hsociChar is HSNeoOCIChar chara)
                    {
                        string actorAlias;
                        string actorColor = "ffffff";
                        string actorTitle = hsociChar.text_name;

                        // analysis actor tag
                        var tagElements = fldName.Split(':');
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

                        _scenef_actors[actorAlias] = chara.as_actor;

                        register_char(actorAlias, actorColor, actorTitle);                      

                        Console.WriteLine("Registered actor: '" + actorAlias + "' as " + actorTitle + " (#" + actorColor + ")");
                    }
                }
                else if (fldName.StartsWith(prop_folder_prefix))
                {
                    // analysis props tag

                    string propAlias = fldName.Substring(prop_folder_prefix.Length).Trim();
                    // register props
                    HSNeoOCI oci = HSNeoOCI.create_from_treenode(fld.treeNodeObject.parent);

                    if (oci is Prop prop)
                    {
                        _scenef_props[propAlias] = prop;
                        Console.WriteLine("Registered prop: '" + Utils.to_roman(propAlias) + "' as " + Utils.to_roman(oci.text_name));
                    }
                }
            }
        }

        /*
        public void scenef_register_actorsprops()
        {
            string actorTitle = "";
            Console.WriteLine("-- Framework: register actors and props start --");
            var game = this;
            // search for tag folder (-actor:,-prop:,-dlgFolder-) and load them into game automaticlly
            // so this function must be called AFTER SCENE HAD BE LOADED!!
            game._scenef_actors = new Dictionary<string, Actor>
            {
            };
            game._scenef_props = new Dictionary<string, Prop>
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
                        if (hsociChar is HSNeoOCIChar chara)
                        {
                            if (actorTitle is null)
                            {
                                actorTitle = hsociChar.text_name;
                            }
                            //game._scenef_actors[actorAlias] = Actor(hsociChar.objctrl)
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

                    if (oci is Prop prop) {
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
                    game._scenef_props[propAlias] = (Prop)hsobj;
                    Console.WriteLine("Registered prop: '" + Utils.to_roman(propAlias) + "' as " + Utils.to_roman(hsobj.text_name));
                }
                else if (ftn.StartsWith("-propgrandpa:"))
                {
                    // analysis props tag
                    propAlias = ftn.Substring("-propgrandpa:".Length).Trim();
                    // register props
                    hsobj = HSNeoOCI.create_from_treenode(fld.treeNodeObject.parent.parent);
                    game._scenef_props[propAlias] = (Prop)hsobj;
                    Console.WriteLine("Registered prop: '" + Utils.to_roman(propAlias) + "' as " + Utils.to_roman(hsobj.text_name));
                }
            }
            Console.WriteLine("-- Framework: register actors and props end --");
        }
       */
        

        public Dictionary<string, Actor> scenef_get_all_actors()
        {
            return this._scenef_actors;
        }

        public Dictionary<string, Prop> scenef_get_all_props()
        {
            return this._scenef_props;
        }

        public HSNeoOCI scenef_get_prop(string id)
        {
            if (this.scenef_get_all_props().ContainsKey(id))
            {
                HSNeoOCI obj = this.scenef_get_all_props()[id];
                return obj;
            }
            return null;
        }

        public Prop scenef_get_propf(string id)
        {
            if (this.scenef_get_all_props()[id] != null)
            {
                HSNeoOCIProp obj = this.scenef_get_all_props()[id];
                return obj.as_prop;
            }
            return null;
        }

        public Actor scenef_get_actor(string id)
        {
            if (this.scenef_get_all_actors()[id] != null)
            {
                Actor obj = this.scenef_get_all_actors()[id];
                return obj;
            }
            return null;
        }

        public void scenef_reg_actor(string id, Actor actor)
        {
            this._scenef_actors[id] = actor;
        }

        public void scenef_reg_prop(string id, Prop prop)
        {
            this._scenef_props[id] = prop;
        }

        public void scenef_clean_actorsprops()
        {
            this._scenef_actors = new Dictionary<string, Actor>();
            this._scenef_props = new Dictionary<string, Prop>();
        }

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
        public void sync_h(IActor female, IActor male)
        {
            // if factor.isHAnime:
            female.anime_option_param = new Actor.AnimeOption_s { height = female.height, breast = female.breast };
            // if mactor.isHAnime:
            male.anime_option_param = new Actor.AnimeOption_s { height = female.height, breast = female.breast };
        }
    }

}
