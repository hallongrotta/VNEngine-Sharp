using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HarmonyLib;
using Studio;
using UnityEngine;
using VNActor;
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

        protected Studio.Studio studio
        {
            get
            {
                var studio = Studio.Studio.Instance;
                return studio;
            }
        }

        protected SceneInfo studio_scene => studio.sceneInfo;

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
                        var chara = (Character) ochar;
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
                        var chara = (Character) ochar;
                        ar.Add(chara);
                    }
                }

                return ar[0];
            }
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
                        var prop = (Item) oitem;
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
                        var prop = (Item) oitem;
                        ar.Add(prop);
                    }
                }

                if (ar.Count > 0)
                    return ar[0];
                throw new Exception("No items selected");
            }
        }

        public string CameraName
        {
            get
            {
                // return the current active camera's name, or return None if no camera actived.
                if (studio.ociCamera != null) return studio.ociCamera.name;
                return null;
            }
            set
            {
                // set the named camera as active camera, if name is None or not found, switch to default camera
                // if active an object camera, return true. Or return false if non object camera actived.
                foreach (var ociobj in studio.dicObjectCtrl.Values)
                    if (ociobj is OCICamera cam)
                    {
                        if (cam.name == name)
                            if (studio.ociCamera != cam)
                                studio.ChangeCamera(cam);
                        return;
                    }

                studio.ChangeCamera(null);
            }
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
            anim_to_camera_obj(duration, get_camera_num(camnum), style, onCameraEnd);
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

        public void _anim_to_camera_savecurrentpos()
        {
            var camobj = get_camera_num(0);
            camSPos = camobj.position;
            camSDir = camobj.distance;
            camSAngle = camobj.rotation;
            camSFOV = camobj.fov;
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
            anim_to_camera_obj(duration, camobj, style, onCameraEnd);
        }

        public void anim_to_camera_obj(float duration, CamData camobj, string style = "linear",
            GameFunc onCameraEnd = null)
        {
            _anim_to_camera_savecurrentpos();
            // print "Anim to cam 1"
            // print "Anim to cam 2"
            var camobjv = camobj;
            camTPos = camobjv.position;
            camTDir = camobjv.distance;
            camTAngle = camobjv.rotation;
            camTFOV = camobjv.fov;
            camAnimStyle = style;
            camAnimFullStyle = null;
            // camera animation one timer only
            animation_cam_timer(duration, onCameraEnd);
        }

        public void debug_print_all_chars()
        {
            var fems = scene_get_all_females();
            Console.WriteLine("-- Female scene chars: --");
            foreach (var i in Enumerable.Range(0, fems.Count))
                Console.WriteLine("{0}: {1}", i.ToString(), fems[i].text_name);
            fems = scene_get_all_males();
            Console.WriteLine("-- Male scene chars: --");
            foreach (var i in Enumerable.Range(0, fems.Count))
                Console.WriteLine("{0}: {1}", i.ToString(), fems[i].text_name);
            show_blocking_message_time("Debug: list of chars printed in console!");
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

        public bool scene_set_bg_png_orig(string filepng)
        {
            if (scene_get_bg_png_orig() != filepng)
            {
                var obj = (BackgroundCtrl) FindObjectOfType(typeof(BackgroundCtrl));
                return obj.Load(filepng);
            }

            return true;
        }

        public void sync_h(Character female, Character male)
        {
            // if factor.isHAnime:
            female.AnimationOption = new Character.AnimeOption_s {height = female.Height, breast = female.Breast};
            // if mactor.isHAnime:
            male.AnimationOption = new Character.AnimeOption_s {height = female.Height, breast = female.Breast};
        }
    }
}