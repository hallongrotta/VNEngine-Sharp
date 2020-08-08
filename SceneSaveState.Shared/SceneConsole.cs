using System;
using System.Collections.Generic;
using UnityEngine;
using VNEngine;
using VNActor;
using System.Linq;
using MessagePack;
using static VNEngine.VNCamera;
using System.IO;
using Studio;
using static VNActor.Actor;
using static VNActor.Prop;
using RootMotion;
using System.Text;
using KKAPI.Studio.SaveLoad;
using KKAPI.Utilities;
using ExtensibleSaveFormat;
using NodeCanvas.Tasks.Conditions;

namespace SceneSaveState
{

    public class SceneConsole : SceneCustomFunctionController
    {

        public bool isSysTracking = true;

        public int _normalheight;

        public int _normalwidth;

        public Dictionary<object, object> accstate;

        public Vector2 adv_scroll;

        public Dictionary<object, object> all_acc;

        public List<HSNeoOCIFolder> arAutoStatesItemsChoice;

        public List<HSNeoOCIFolder> arAutoStatesItemsVis;

        public bool autoAddCam;

        public bool autoLoad;

        public string autoshownewid;

        public double backupTimeCur;

        public double backupTimeDuration;

        public Dictionary<object, object> baseacc;

        public List<List<object>> basechars;

        public Dictionary<object, object> basepropflds;

        public Dictionary<object, object> baseprops;

        public List<Scene> block;

        public bool cam_addparam;

        public VNData.addprops_struct cam_addprops;

        public string cam_addvncmds;

        public Vector2 cam_scroll;

        public string cam_whatsay;

        public string cam_whosay;

        public List<CamData> camset;

        public int camviewwidth;

        public string charname;

        public IDataClass clipboard_status;

        public IDataClass clipboard_status2;

        public string[] consolenames;

        public int cur_cam;

        public int cur_index;

        public List<object> dict;

        public Dictionary<object, object> dictparse;

        public List<List<object>> dupchars;

        public int fset_index;

        public Vector2 fset_scroll;

        public string funcLockedText;

        //public VNNeoController game;

        public bool guiOnShow;

        public bool isFuncLocked;

        public bool isSaveCompact;

        public bool isSaveOld;

        public bool isSaveVerify;

        public bool isUseMsAuto;

        public int last_acc_id;

        public string ldname;

        public string mininewid;

        public Vector2 miniset_scroll;

        public int mset_index;

        public Vector2 mset_scroll;

        public List<List<string>> nameset;

        public string newid;

        public string nor_font_col;

        public string[] options;

        public Action originalWindowCallback;

        public int originalwindowheight;

        public int originalwindowwidth;

        public float paramAnimCamDuration;

        public bool paramAnimCamIfPossible;

        public string paramAnimCamStyle;

        public float paramAnimCamZoomOut;

        public int prev_cam;

        public int prev_index;

        public bool promptOnDelete;

        public List<object> propfldtag;

        public List<object> proptag;

        public Vector2 saveload_scroll;

        public string[] scene_cam_str;

        public Vector2 scene_scroll;

        public string[] scene_str_array;

        public List<string> scene_strings;

        public string scenefile;

        public List<object> sdict;

        public string sel_font_col;

        public Dictionary<string, KeyValuePair<string, string>> shortcuts;

        public SkinDefault skinDefault;

        public string skinDefault_sideApp;

        public bool skipClothesChanges;

        public int subwinindex;

        public string svname = "";

        public object temp_states;

        public Vector2 tracking_scroll;

        public int updAutoStatesTimer;

        public Action<object> warning_action;

        public struct WarningParam_s
        {
            public string msg;
            public object func_param;
            public bool single_op;
            private string v1;
            private object p;
            private bool v2;

            public WarningParam_s(string v1, object p, bool v2) : this()
            {
                this.v1 = v1;
                this.p = p;
                this.v2 = v2;
            }
        }

        public WarningParam_s? warning_param;

        public string versionSceneDataParsing;

        public int viewheight;

        public int viewwidth;

        public int windowheight;

        public int windowindex;

        public int windowwidth;

        public bool vnFastIsRunImmediately;
        internal float consoleWidth;
        internal float consoleHeight;
        internal List<string> fset;
        internal List<string> mset;
        internal int wiz_step;
        internal Dictionary<string, string> wiz_data;
        internal string wiz_error;
        internal string wiz_view_mode;
        internal Vector2 vnss_wizard_ui_scroll;


        public VNNeoController game
        {
            get
            {
                return VNNeoController.Instance;
            }
        }

        public static SceneConsole Instance { get; private set; }

        public SceneConsole()
        {
            // init dict
            // initWordDict()
            // --- Some constants ---
            this._normalwidth = 500;
            this._normalheight = 350;
            // self.drag_rect = Rect(0, 0, 10000, 50)
            // --- Basic settings ---
            this.originalwindowwidth = 0;
            this.originalwindowheight = 0;
            this.originalWindowCallback = null;
            this.guiOnShow = false;
            this.windowwidth = this._normalwidth;
            this.windowheight = this._normalheight;
            this.windowindex = 0;
            this.subwinindex = 0;
            // --- Essential Data ---
            this.versionSceneDataParsing = "7.0";
            this.dict = new List<object>();
            this.sdict = new List<object>();
            this.dictparse = new Dictionary<object, object>
            {
            };
            this.scenefile = "";
            this.block = new List<Scene>();
            this.basechars = new List<List<object>> {
                new List<object>(),
                new List<object>()
            };
            this.dupchars = new List<List<object>> {
                new List<object>(),
                new List<object>()
            };
            this.last_acc_id = 0;
            this.all_acc = new Dictionary<object, object>
            {
            };
            this.baseacc = new Dictionary<object, object>
            {
            };
            this.accstate = new Dictionary<object, object>
            {
            };
            this.propfldtag = new List<object>();
            this.basepropflds = new Dictionary<object, object>
            {
            };
            this.proptag = new List<object>();
            this.baseprops = new Dictionary<object, object>
            {
            };
            // self.basechars = self.getAllBaseChars()
            // self.dupchars = self.getAllDupChars()
            // self.updateNameset()
            // :::: UI Data ::::
            this.consolenames = new string[] { "SceneSaveState", "Pose Library", "Scene Utils" };
            this.options = new string[] { "Edit", "Tracking", "Load/Save", "Advanced", "Ministates" };
            // -- Main --
            this.sel_font_col = "#f24115";
            this.nor_font_col = "#f9f9f9";
            this.scene_scroll = new Vector2(0, 0);
            this.cam_scroll = new Vector2(0, 0);
            this.mset_scroll = new Vector2(0, 0);
            this.fset_scroll = new Vector2(0, 0);
            this.miniset_scroll = new Vector2(0, 0);
            this.tracking_scroll = new Vector2(0, 0);
            this.saveload_scroll = new Vector2(0, 0);
            this.fset_index = 0;
            this.mset_index = 0;
            this.viewwidth = 120;
            this.viewheight = 200;
            this.camviewwidth = 120;
            // self.char_name = ""
            this.cam_whosay = "";
            this.cam_whatsay = "";
            this.cam_addvncmds = "";
            this.cam_addparam = false;
            this.cam_addprops.addprops = new Dictionary<string, bool> {
                {
                    "a1",
                    false},
                {
                    "a2",
                    false}};
            this.newid = "";
            this.mininewid = "";
            this.autoshownewid = "";
            this.isUseMsAuto = false;
            // -- Load/Save --
            this.svname = "";
            this.ldname = "";
            // self.optionint = 0
            // -- Advanced --
            this.adv_scroll = new Vector2(0, 0);
            this.temp_states = null;
            this.charname = "";
            this.autoLoad = true;
            this.autoAddCam = true;
            this.promptOnDelete = true;
            this.skipClothesChanges = false;
            //this.shortcuts = new Dictionary<string, (string, string)>();
            this.paramAnimCamDuration = 1.5f;
            this.paramAnimCamStyle = "fast-slow";
            this.paramAnimCamZoomOut = 0.0f;
            this.paramAnimCamIfPossible = Utils.is_ini_value_true("AnimateCamIfPossible");
            // self.nwindowRect = None
            // -- Edit window --
            // Common data
            this.nameset = new List<List<string>> {
                new List<string>(),
                new List<string>()
            };
            this.scene_strings = new List<string>();
            this.scene_str_array = new string[] { "<Empty>" };
            this.scene_cam_str = new string[] {"<Empty>" };
            this.cur_index = -1;
            this.prev_index = -1;
            this.cur_cam = -1;
            this.prev_cam = -1;
            this.camset = new List<CamData>();
            this.isSaveCompact = true;
            this.isSaveVerify = true;
            this.isSaveOld = false;
            this.updAutoStatesTimer = 0;
            this.arAutoStatesItemsVis = new List<HSNeoOCIFolder>();
            this.arAutoStatesItemsChoice = new List<HSNeoOCIFolder>();
            this.backupTimeDuration = Utils.get_ini_value_def_int("AutoBackupTimeInSeconds", 600);
            this.backupTimeCur = this.backupTimeDuration;
            this.vnFastIsRunImmediately = false;
            // blocking message
            this.funcLockedText = "...";
            this.isFuncLocked = false;
            // skin_default internal
            this.skinDefault = new SkinDefault
            {
                controller = game
            };
            this.skinDefault_sideApp = "";
            Instance = this;
        }


        // Blocking message functions
        public void show_blocking_message(string text = "...")
        {
            this.funcLockedText = text;
            this.isFuncLocked = true;
        }

        public void hide_blocking_message(object game = null)
        {
            this.isFuncLocked = false;
        }

        public void show_blocking_message_time_sc(string text = "...", float duration = 3f)
        {
            this.show_blocking_message(text);
            this.game.set_timer(duration, this.hide_blocking_message);
        }

        // other
        public void updateSceneStrings()
        {
            this.scene_strings = new List<string>();
            foreach (var id in Enumerable.Range(0, this.block.Count - 0))
            {
                this.scene_strings.Add(String.Format("Scene {0}", id + 1));
            }
            this.scene_str_array = this.scene_strings.ToArray();
        }

        // ---------- ministates ------------
        public void addSelectedAutoShow(string param)
        {
            // get list of sel objs
            var arSel = this.get_selected_objs();
            if (arSel.Count == 0)
            {
                this.show_blocking_message_time_sc("No selection!");
                return;
            }
            foreach (var actprop in arSel)
            {
                //print actprop
                //if hasattr(actprop, 'as_prop'):
                if (actprop is HSNeoOCIChar chara)
                {
                    //id = self.find_item_in_objlist(actprop.objctrl)
                }
                else
                {
                    var txtname = this.autoshownewid;
                    if (txtname == "")
                    {
                        txtname = actprop.text_name;
                    }
                    var fld = HSNeoOCIFolder.add("-msauto:" + param + ":" + txtname);
                    //objSave["__id{0}"%(str(id))] = actprop.export_full_status()
                    fld.set_parent(actprop);
                }
            }
            Utils.recalc_autostates();
            this.autoshownewid = "";
        }

        public void delSelectedAutoShow()
        {
            // get list of sel objs
            var arSel = this.get_selected_objs();
            if (arSel.Count == 0)
            {
                this.show_blocking_message_time_sc("No selection!");
                return;
            }
            var arSel0 = arSel[0];
            var folders = HSNeoOCIFolder.find_all_startswith("-msauto:");
            foreach (var folder in folders)
            {
                if (folder.treeNodeObject.parent == arSel0.treeNodeObject)
                {
                    folder.delete();
                }
            }
            Utils.recalc_autostates();
        }

        public void addSelectedMini()
        {
            // find mini
            var fld = HSNeoOCIFolder.find_single("-ministates:1.0");
            if (fld == null)
            {
                fld = HSNeoOCIFolder.add("-ministates:1.0");
            }
            // calc name
            var name = this.mininewid;
            if (name == "")
            {
                name = "state";
            }
            // get list of sel objs
            var arSel = this.get_selected_objs();
            if (arSel.Count == 0)
            {
                this.show_blocking_message_time_sc("No selection!");
                return;
            }
            var objSave = new Dictionary<string, IDataClass>
            {
            };
            foreach (Actor actprop in arSel)
            {
                //if isinstance(actprop, Actor):
                var id = this.find_item_in_objlist(actprop.objctrl);
                objSave[String.Format("__id{0}", id.ToString())] = actprop.export_full_status();
                //elif isinstance(actprop, Actor):
                //print objSave
            }
            var fldName = Utils.folder_add_child(fld, name);
            foreach (var k in objSave.Values)
            {
                //fldObj = folder_add_child(fldName,MessagePackSerializer.Serialize(objSave))
                Utils.folder_add_child(fldName, MessagePackSerializer.Serialize<IDataClass>(k).ToString());
            }
            // fldName = HSNeoOCIFolder.add(name)
            // fldName.set_parent(fld)
            //
            // fldObj = HSNeoOCIFolder.add(MessagePackSerializer.Serialize(objSave))
            // fldObj.set_parent(fldName)
            this.mininewid = "";
        }

        public int find_item_in_objlist(ObjectCtrlInfo obj)
        {
            var dobjctrl = this.game.studio.dicObjectCtrl;
            foreach (var key in dobjctrl.Keys)
            {
                var objctrl = dobjctrl[key];
                if (objctrl == obj)
                {
                    return key;
                }
            }
            throw new Exception("Item does not exist");
        }

        public List<HSNeoOCI> get_selected_objs()
        {
            var mtreeman = this.game.studio.treeNodeCtrl;
            var ar = new List<HSNeoOCI>();
            foreach (var node in mtreeman.selectNodes)
            {
                var ochar = HSNeoOCI.create_from_treenode(node);
                if (ochar is HSNeoOCIChar chara)
                {
                    ar.Add(chara.as_actor);
                }
                else if (ochar is HSNeoOCIProp prop)
                {
                    ar.Add(prop.as_prop);
                }
                else
                {
                    throw new Exception("Invalid object");
                }
            }
            return ar;
        }

        // Add stuff
        public void addScene(bool insert = false)
        {
            if (insert == false)
            {
                this.cur_index = this.block.Count;
                this.prev_index = this.cur_index;
            }
            else
            {
                this.cur_index += 1;
            }
            this.block.Insert(this.cur_index, new Scene());
            this.updateSceneStrings();
        }

        public void getSceneCamString()
        {
            var cam_str = new List<string>();
            foreach (var i in Enumerable.Range(0, this.block[this.cur_index].cams.Count - 0))
            {
                cam_str.Add("Cam " + i.ToString());
            }
            this.scene_cam_str = cam_str.ToArray();
        }

        public void changeSceneCam(object param)
        {
            changeSceneCam((string)param);
        }

        public void changeSceneCam()
        {
            changeSceneCam(task: "");
        }

        public void changeSceneCam(string task = "")
        {
            var cdata = VNNeoController.cameraData;
            var addata = new VNData(this.cam_addparam, this.cam_whosay, this.cam_whatsay, this.cam_addvncmds, this.cam_addprops);
            var cam_data = new CamData(cdata.pos, cdata.rotate, cdata.distance, cdata.parse, addata);
            if (task == "" || task == "add")
            {
                this.cur_cam = this.block[this.cur_index].addCam(cam_data);
            }
            else if (task == "upd")
            {
                this.block[this.cur_index].updateCam(this.cur_cam, cam_data);
            }
            else if (task == "del")
            {
                this.cur_cam = this.block[this.cur_index].deleteCam(this.cur_cam);
                if (this.cur_cam > -1)
                {
                    this.setCamera();
                }
            }
            if (!(task == "upd"))
            {
                this.getSceneCamString();
            }
        }

        public void setCamera()
        {
            setCamera(this.paramAnimCamIfPossible);
        }

        public void setCamera(bool isAnimated)
        {
            VNCamera.CamData camera_data = this.block[this.cur_index].cams[this.cur_cam];
            // check and run adv command
            var keepCamera = false;
            if (camera_data.hasVNData)
            {
                //keepCamera = VNExt.runAdvVNSS(this, camera_data.addata); TODO
            }
            // actual set
            if (keepCamera)
            {
            }
            else if (isAnimated)
            {
                // self.game.anim_to_camera(1.5, pos=camera_data[0], distance=camera_data[1], rotate=camera_data[2], fov=camera_data[3], style={'style': "fast-slow",'target_camera_zooming_in': 2})
                /*var style = new Dictionary<string, object> {
                    {
                        "style",
                        "fast-slow"}};
                if (this.paramAnimCamZoomOut != 0.0)
                {
                    style["target_camera_zooming_in"] = this.paramAnimCamZoomOut;
                } */ //TODO fix this
                var style = "linear";
                this.game.anim_to_camera(this.paramAnimCamDuration, pos: camera_data.position, distance: camera_data.distance, rotate: camera_data.rotation, fov: camera_data.fov, style: style);
            }
            else
            {
                this.game.move_camera(camera_data);
                //this.game.move_camera(pos: camera_data.position, distance: camera_data.distance, rotate: camera_data.rotation, fov: camera_data.fov);
            }
            if (camera_data.addata is VNCamera.VNData addata)
            {
                this.cam_addparam = addata.addparam;
                this.cam_whosay = addata.whosay;
                this.cam_whatsay = addata.whatsay;
                if (addata.addvncmds != null)
                {
                    this.cam_addvncmds = addata.addvncmds;
                }
                else
                {
                    this.cam_addvncmds = "";
                }
                if (addata.addprops.addprops != null)
                {
                    this.cam_addprops = addata.addprops;
                }
                else
                {
                    this.cam_addprops.addprops = new Dictionary<string, bool> {
                        {
                            "a1",
                            false},
                        {
                            "a2",
                            false}};
                }
            }
            else
            {
                this.cam_addparam = false;
                this.cam_whosay = "";
                this.cam_whatsay = "";
                this.cam_addvncmds = "";
                this.cam_addprops.addprops = new Dictionary<string, bool> {
                    {
                        "a1",
                        false},
                    {
                        "a2",
                        false}};
            }
        }

        public void addAutoWithMsg()
        {
            this.addAuto();
            this.show_blocking_message_time_sc("Scene added!", 2.0f);
        }

        public void addAuto(bool insert = false, bool addsc = true, bool allbase = true)
        {
            if (addsc == true)
            {
                this.addScene(insert);
                if (this.autoAddCam == true)
                {
                    this.changeSceneCam("add");
                }
            }
            var curscene = this.block[this.cur_index];
            curscene.importCurScene(this.game, this.isSysTracking);
        }

        // Remove stuff

        public void removeScene(object param)
        {
            removeScene();
        }
        public void removeScene()
        {
            if (this.block.Count > 0)
            {
                this.block.RemoveAt(this.cur_index);
                this.cur_index = this.cur_index - 1;
                this.scene_strings.RemoveAt(scene_strings.Count - 1);
                this.scene_str_array = this.scene_strings.ToArray();
            }
        }

        // Load scene
        public void loadCurrentScene()
        {
            this.setSceneState();
            if (block.Count > 0 && this.block[this.cur_index].cams.Count > 0)
            {
                this.cur_cam = 0;
                this.setCamera();
            }
        }

        public void setSceneState()
        {
            setSceneState(this.cur_index);
        }

        public void setSceneState(int index)
        {
            var curscene = this.block[index];
            var game = VNNeoController.Instance;
            if (isSysTracking)
            {
                VNEngine.System.import_status(curscene.sys);
            }
            curscene.setSceneState(game);
        }

        public void copySelectedStatusToTracking(List<string> exclude)
        {
            var elem = HSNeoOCI.create_from_selected();
            if (elem is HSNeoOCIChar chara)
            {
                var tmp_status = chara.as_actor.export_full_status();
                var actors = this.game.scenef_get_all_actors();
                foreach (var key in actors.Keys)
                {
                    Actor actor = (Actor)actors[key];
                    if (actor.text_name == chara.text_name)
                    {
                        foreach (var keyEx in exclude)
                        {
                            tmp_status.Remove(keyEx);
                        }
                        actor.import_status(tmp_status);
                        return;
                    }
                }
                this.show_blocking_message_time_sc("Can't find tracking char with same name");
            }
            else
            {
                this.show_blocking_message_time_sc("Can't copy status");
            }
        }

        public void copySelectedStatus()
        {
            var elem = HSNeoOCI.create_from_selected();
            if (elem is HSNeoOCIChar chara)
            {
                this.clipboard_status = ((Actor)chara).export_full_status();
            }
            else if (elem is HSNeoOCIProp prop)
            {
                this.clipboard_status = prop.as_prop.export_full_status();
            }
            else
            {
                this.show_blocking_message_time_sc("Can't copy status");
            }
        }

        public void pasteSelectedStatus()
        {
            var elem = HSNeoOCI.create_from_selected();
            if (elem is HSNeoOCIChar chara)
            {
                chara.as_actor.import_status(this.clipboard_status);
            }
            else if (elem is HSNeoOCIProp prop)
            {
                prop.as_prop.import_status((PropData)this.clipboard_status);
            }
            else
            {
                this.show_blocking_message_time_sc("Can't paste status");
            }
        }

        public void copySelectedStatus2()
        {
            var elem = HSNeoOCI.create_from_selected();
            if (elem is HSNeoOCIChar chara)
            {
                this.clipboard_status2 = chara.as_actor.export_full_status();
            }
            else if (elem is HSNeoOCIProp prop)
            {
                this.clipboard_status2 = prop.as_prop.export_full_status();
            }
            else
            {
                this.show_blocking_message_time_sc("Can't copy status 2");
            }
        }

        public void pasteSelectedStatus2()
        {
            var elem = HSNeoOCI.create_from_selected();
            if (elem is HSNeoOCIChar chara)
            {
                chara.as_actor.import_status((ActorData)this.clipboard_status2);
            }
            else if (elem is HSNeoOCIProp prop)
            {
                prop.as_prop.import_status((PropData)this.clipboard_status2);
            }
            else
            {
                this.show_blocking_message_time_sc("Can't paste status 2");
            }
        }   

        public void addSysTracking()
        {
            if (this.block.Count > 0)
            {
                var curstatus = VNEngine.System.export_sys_status(this.game);
                foreach (var i in Enumerable.Range(0, this.block.Count))
                {
                    Scene scene = this.block[i];
                    //scene.actors["sys"] = curstatus;
                    scene.sys = (VNEngine.System.SystemData)curstatus;
                }
                isSysTracking = true; 
            }
            else
            {
                show_blocking_message_time_sc("Please, add at least 1 state to add system environment tracking");
            }
        }

        public void delSysTracking()
        {
            foreach (var i in Enumerable.Range(0, this.block.Count))
            {
                var scene = this.block[i];
                scene.actors.Remove("sys");
            }
            isSysTracking = false;
        }


        public void addSelectedToTrack(HSNeoOCIProp prop)
        {
            HSNeoOCIFolder tagfld;
            var props = this.game.scenef_get_all_props();

            foreach (Prop p in props.Values)
            {
                if (p.objctrl == prop.objctrl)
                {
                    return;
                }
            }

            string id = "";
            foreach (var i in Enumerable.Range(0, 1000 - 0))
            {
                id = "prp" + i.ToString();
                if (props.ContainsKey(id))
                {
                }
                else
                {
                    break;
                }
            }
            if (prop is HSNeoOCILight)
            {
                tagfld = HSNeoOCIFolder.add("-propchild:" + id);
                prop.set_parent(tagfld);
            }
            else if (prop is HSNeoOCIRoute)
            {
                tagfld = HSNeoOCIFolder.add("-propgrandpa:" + id);
                tagfld.set_parent_treenodeobject(prop.treeNodeObject.child[0]);
            }
            else
            {
                tagfld = HSNeoOCIFolder.add(VNNeoController.prop_folder_prefix + id);
                tagfld.set_parent_treenodeobject(prop.treeNodeObject);
            }
            var curstatus = prop.export_full_status();
            foreach (var i in Enumerable.Range(0, this.block.Count))
            {
                Scene scene = this.block[i];
                scene.props[id] = (PropData)curstatus;
                // updating set
            }
        }

        public void addSelectedToTrack(HSNeoOCIChar chara)
        {
            var actors = this.game.scenef_get_all_actors();

            foreach (Actor actor in actors.Values)
            {
                if (actor.objctrl == chara.objctrl)
                {
                    return;
                }
            }

            var id = "";
            foreach (var i in Enumerable.Range(0, 1000 - 0))
            {
                id = "act" + i.ToString();
                if (actors.ContainsKey(id))
                {
                }
                else
                {
                    break;
                }
            }
            var tagfld = HSNeoOCIFolder.add(VNNeoController.actor_folder_prefix + id);
            tagfld.set_parent_treenodeobject(chara.treeNodeObject.child[0].child[0]);
            var curstatus = (ActorData)chara.export_full_status();
            foreach (var i in Enumerable.Range(0, this.block.Count))
            {
                Scene scene = this.block[i];
                scene.actors[id] = curstatus;
            }
        }

        public void addSelectedToTrack()
        {
            var objects = KKAPI.Studio.StudioAPI.GetSelectedObjects();

            if (objects == null)
            {
                this.show_blocking_message_time_sc("Nothing selected");
                return;
            } 
            else
            {
                foreach (ObjectCtrlInfo objectCtrl in objects)
                {
                    if (objectCtrl is OCIItem item)
                    {
                        var prop = HSNeoOCI.create_from(item);
                        addSelectedToTrack(prop);
                    }
                    else if (objectCtrl is OCIChar chara)
                    {
                        var actor = HSNeoOCI.create_from(chara);
                        addSelectedToTrack(actor);
                    }
                    else if (objectCtrl is OCILight oLight)
                    {
                        var light = HSNeoOCI.create_from(oLight);
                        addSelectedToTrack(light);
                    }
                    else if (objectCtrl is OCIRoute oRoute)
                    {
                        var route = HSNeoOCI.create_from(oRoute);
                        addSelectedToTrack(route);
                    } else
                    {
                        return;
                    }
                }
                game.LoadTrackedActorsAndProps();
            }
        }

        /*
        public void addSelectedToTrack()
        {
            Scene scene;
            IDataClass curstatus;
            HSNeoOCIFolder tagfld;
            var elem = HSNeoOCI.create_from_selected();
            if (elem == null)
            {
                this.show_blocking_message_time_sc("Nothing selected");
                return;
            }
            if (elem is HSNeoOCIChar chara)
            {
                var actors = this.game.scenef_get_all_actors();
                var id = "";
                foreach (var i in Enumerable.Range(0, 1000 - 0))
                {
                    id = "act" + i.ToString();
                    if (actors.ContainsKey(id))
                    {
                    }
                    else
                    {
                        break;
                    }
                }
                tagfld = HSNeoOCIFolder.add("-actor:" + id);
                tagfld.set_parent_treenodeobject(elem.treeNodeObject.child[0].child[0]);
                this.game.scenef_register_actorsprops();
                curstatus = chara.as_actor.export_full_status();
                foreach (var i in Enumerable.Range(0, this.block.Count))
                {
                    scene = this.block[i];
                    scene.actors[id] = (ActorData)curstatus;
                }
            }
            else if (elem is HSNeoOCIProp prop)
            {
                var props = this.game.scenef_get_all_props();
                string id = "";
                foreach (var i in Enumerable.Range(0, 1000 - 0))
                {
                    id = "prp" + i.ToString();
                    if (props.ContainsKey(id))
                    {
                    }
                    else
                    {
                        break;
                    }
                }
                if (elem is HSNeoOCILight)
                {
                    tagfld = HSNeoOCIFolder.add("-propchild:" + id);
                    elem.set_parent(tagfld);
                }
                else if (elem is HSNeoOCIRoute)
                {
                    tagfld = HSNeoOCIFolder.add("-propgrandpa:" + id);
                    tagfld.set_parent_treenodeobject(elem.treeNodeObject.child[0]);
                }
                else
                {
                    tagfld = HSNeoOCIFolder.add("-prop:" + id);
                    tagfld.set_parent_treenodeobject(elem.treeNodeObject);
                }
                this.game.scenef_register_actorsprops();
                curstatus = prop.as_prop.export_full_status();
                foreach (var i in Enumerable.Range(0, this.block.Count))
                {
                    scene = this.block[i];
                    scene.props[id] = (PropData)curstatus;
                    // updating set
                }
            }
        }
        */

        public void changeSelTrackID(string toId)
        {
            if (toId == "")
            {
                this.show_blocking_message_time_sc("Please, set ID to change to first");
                return;
            }
            var elem = HSNeoOCI.create_from_selected();
            if (elem == null)
            {
                this.show_blocking_message_time_sc("Nothing selected");
                return;
            }
            if (elem is HSNeoOCIChar chara)
            {
                var actors = this.game.scenef_get_all_actors();
                string id = "";
                foreach (var actid in actors.Keys)
                {
                    if (actors[actid].objctrl == elem.objctrl)
                    {
                        // found
                        break;
                    }
                }
                //self.delActorFromTrack(actid)
                if (id == "")
                {
                    this.show_blocking_message_time_sc("Can't find actor to change ID");
                    return;
                }
                // actually changing ID
                this.changeActorTrackId(id, toId);
            }
            // updating set
            this.game.LoadTrackedActorsAndProps();
        }
        
        public void delSelectedFromTrack()
        {
            var elem = HSNeoOCI.create_from_selected();
            if (elem == null)
            {
                this.show_blocking_message_time_sc("Nothing selected");
                return;
            }
            if (elem is HSNeoOCIChar chara)
            {
                var actors = this.game.scenef_get_all_actors();
                var id = "";
                foreach (var actid in actors.Keys)
                {
                    if (actors[actid].objctrl == elem.objctrl)
                    {
                        // found
                        break;
                    }
                }
                if (id == "")
                {
                    this.show_blocking_message_time_sc("Can't delete; seems this actor is not tracking yet");
                    return;
                }
                this.delActorFromTrack(id);
            }
            else if (elem is HSNeoOCIProp)
            {
                var props = this.game.scenef_get_all_props();
                var id = "";
                foreach (var propid in props.Keys)
                {
                    if (props[propid].objctrl == elem.objctrl)
                    {
                        // found
                        break;
                    }
                }
                this.delPropFromTrack(id);
            }
            // updating set
            this.game.LoadTrackedActorsAndProps();
        }

        public void delActorFromTrack(string actid)
        {
            if (actid != "")
            {
                // we found this char
                var fld = HSNeoOCIFolder.find_single(VNNeoController.actor_folder_prefix + actid);
                if (fld == null)
                {
                    fld = HSNeoOCIFolder.find_single_startswith(VNNeoController.actor_folder_prefix + actid + ":");
                }
                // found
                if (fld != null)
                {
                    fld.delete();
                }
                foreach (var i in Enumerable.Range(0, this.block.Count))
                {
                    var scene = this.block[i];
                    scene.actors.Remove(actid);
                }
            }
        }

        public void changeActorTrackId(string actid, string toid)
        {
            if (actid != "")
            {
                // we found this char
                var fld = HSNeoOCIFolder.find_single(VNNeoController.actor_folder_prefix + actid);
                if (fld == null)
                {
                    fld = HSNeoOCIFolder.find_single_startswith(VNNeoController.actor_folder_prefix + actid + ":");
                }
                // found
                //if fld != None:
                //    fld.delete()
                var fldoldname = fld.name;
                var lastelems = fldoldname[(VNNeoController.actor_folder_prefix + actid).Length];
                //print lastelems
                fld.name = VNNeoController.actor_folder_prefix + toid + lastelems;
                //
                foreach (var i in Enumerable.Range(0, this.block.Count))
                {
                    var scene = this.block[i];
                    scene.actors[toid] = scene.actors[actid];
                    scene.actors.Remove(actid);
                    foreach (var camid in Enumerable.Range(0, scene.cams.Count))
                    {
                        var cam = scene.cams[camid];
                        var info = cam.addata;
                        if (info.whosay == actid)
                        {
                            info.whosay = toid;
                        }
                    }
                }
            }
        }

        public void delPropFromTrack(string propid)
        {
            if (propid != "")
            {
                // we found this prop
                var fld = HSNeoOCIFolder.find_single(VNNeoController.prop_folder_prefix + propid);
                // found
                if (fld != null)
                {
                    fld.delete();
                }
                foreach (var i in Enumerable.Range(0, this.block.Count))
                {
                    var scene = this.block[i];
                    scene.props.Remove(propid);
                }
            }
        }

        public HSNeoOCIFolder createFld(string txt, HSNeoOCI parent = null, bool ret = true)
        {
            var fld = HSNeoOCIFolder.add(txt);
            if (parent is HSNeoOCIFolder)
            {
                fld.set_parent(parent);
            }
            if (ret == true)
            {
                return fld;
            }
            else
            {
                throw new Exception("create folder failed");
            }
        }

        public HSNeoOCIFolder createFldIfNo(string txt, HSNeoOCIFolder parent, int childNum)
        {
            HSNeoOCIFolder fld;

                if (parent.treeNodeObject.child.Count <= childNum)
                {
                    //print "create folder! %s" % txt
                    fld = HSNeoOCIFolder.add(txt);
                    fld.set_parent(parent);
                    return fld;
                }
                else
                {
                    var chld = parent.treeNodeObject.child[childNum];
                    fld = HSNeoOCI.create_from_treenode<HSNeoOCIFolder>(chld);
                    if (chld.textName != txt)
                    {
                        //print "hit! upd folder! %s" % txt
                        fld.name = txt;
                        //return fld
                    }
                    else
                    {
                        //print "hit!! no creation! %s" % txt
                    }
                    return fld;
                }
       
        }



        // Parse/Save/Load
        public Dictionary<string, Scene> findAndLoadSceneData(bool backup = false)
        {
            Dictionary<string, ActorData> char_data;
            var flds = this.game.scene_get_all_folders_raw();
            foreach (var fld in flds)
            {
                if (fld.name.StartsWith("-scenesavestate:") && backup == false)
                {
                    var ver = fld.name.Split(':')[1];
                    if (ver == "1.0" || ver == "2.5" || ver == "4.0" || ver == "5.0" || ver == "7.0")
                    {
                        //return self.parseFlds(fld)  # returns dict
                        var block_dict = parseSSSFlds(fld);
                        // using compact resolve
                        /*if (ver == "4.0" || ver == "5.0" || ver == "7.0") //TODO add this back
                        {
                            foreach (var key in block_dict.Keys.ToList())
                            {
                                //print 'key, %s' % str(key)
                                //print isinstance(key, int)
                                //print isinstance(key, str)
                                Scene block = block_dict[key];
                                foreach (var actorkey in block.actors.Keys)
                                {
                                    char_data = block.actors[actorkey];
                                    if (char_data.ContainsKey("_diff"))
                                    {
                                        ind = char_data["_diff"][0];
                                        patch = char_data["_diff"][1];
                                        //print
                                        res = Utils.merge_two_dicts(block_dict[(Convert.ToInt32(ind) + 1).ToString()].actors[actorkey], patch);
                                        block.actors[actorkey] = res;
                                    }
                                }
                                foreach (var propkey in block.props.Keys)
                                {
                                    char_data = block.props[propkey];
                                    if (char_data.ContainsKey("_diff"))
                                    {
                                        ind = char_data["_diff"][0];
                                        patch = char_data["_diff"][1];
                                        //print
                                        res = Utils.merge_two_dicts(block_dict[(Convert.ToInt32(ind) + 1).ToString()].props[propkey], patch);
                                        block.props[propkey] = res;
                                    }
                                }
                            }
                        } */
                        return block_dict;
                    }
                    else
                    {
                        this.show_blocking_message_time_sc("Error: unknown version " + ver + " of scene data.\nPlease, upgrade SceneSaveState to load it.");
                        return null;
                        // break
                    }
                }
                if (fld.name == "-scenesavestatebackup" && backup == true)
                {
                    return this.parseSSSFlds(fld, backup: true);
                    // break
                }
            }
            this.show_blocking_message_time_sc("Error: can't find data to load");
            return null;
        }

        public Dictionary<string, Scene> parseSSSFlds(OCIFolder fld, bool backup = false)
        {
            if (fld.name.StartsWith("-scenesavestate:") && backup == false || fld.name == "-scenesavestatebackup" && backup == true)
            {
                //print "ok"
                var dict = new Dictionary<string, Scene>
                {
                };
                foreach (TreeNodeObject scene_fld in fld.treeNodeObject.child)
                {
                    HSNeoOCIFolder sceneFld = HSNeoOCI.create_from_treenode<HSNeoOCIFolder>(scene_fld);
                    // scene_id = int(scene_fld.name.strip("-scene:"))
                    KeyValuePair<string, object> kv = this.parseFlds(sceneFld);
                    dict[kv.Key] = (Scene)kv.Value;
                }
                return dict;
            }
            else
            {
                throw new Exception("Invalid SSS folder");
            } 
        }

        // parseFlds works for 1.0 or 2.5 version
        public KeyValuePair<string, object> parseFlds(HSNeoOCIFolder fld, bool backup = false)
        {
            if (fld.name.Contains("-scene:"))
            {
                var sc_dict = new Scene();
                foreach (var child in fld.treeNodeObject.child)
                {
                    HSNeoOCIFolder sc_elements = HSNeoOCIFolder.create_from_treenode<HSNeoOCIFolder>(child);

                    KeyValuePair<string, object> kv = this.parseFlds(sc_elements);

                    if (kv.Key == "actors")
                    {
                        sc_dict.actors = (Dictionary<string, ActorData>)kv.Value;
                    } else if (kv.Key == "props")
                    {
                        sc_dict.props = (Dictionary<string, PropData>)kv.Value;
                    } else if (kv.Key == "cams")
                    {
                        sc_dict.cams = (List<CamData>)kv.Value;
                    }
                }
                return new KeyValuePair<string, object>(fld.name.Substring("-scene:".Length), sc_dict);
            }
            else if (fld.name == "-actors")
            {
                var char_dict = new Dictionary<string, ActorData>();
                foreach (var child in fld.treeNodeObject.child) {
                    var chara = HSNeoOCIFolder.create_from_treenode<HSNeoOCIFolder>(child);
                    if (chara.name.StartsWith("{"))
                    {
                        //char_data = self.parseFlds(chara)
                        var char_data = Utils.DeserializeData<Dictionary<string, ActorData>>(chara.name);

                        foreach (var key in char_dict.Keys)
                        {
                            char_dict[key] = char_data[key];
                        }
                    }
                    else
                    {
                        var char_fld = HSNeoOCIFolder.create_from_treenode<HSNeoOCIFolder>(chara.treeNodeObject.child[0]);
                        ActorData char_data = Utils.DeserializeData<ActorData>(char_fld.name);
                        char_dict[chara.name] = char_data;
                    }
                }
                return new KeyValuePair<string, object>(fld.name.Substring("-".Length), char_dict);
            }
            else if (fld.name == "-props")
            {
                // Props
                var prop_dict = new Dictionary<string, PropData>();
                foreach (var child in fld.treeNodeObject.child)
                {
                    var prop = HSNeoOCIFolder.create_from_treenode<Prop>(child);
                    var prop_name = prop.name;
                    if (prop_name.StartsWith("{"))
                    {
                        var prop_data = Utils.DeserializeData<Dictionary<string, PropData>>(prop_name);

                        foreach (var key in prop_dict.Keys)
                        {
                            prop_dict[key] = prop_data[key];
                        }
                    }
                    else
                    {
                        var prop_fld = HSNeoOCIFolder.create_from_treenode<HSNeoOCIFolder>(prop.treeNodeObject.child[0]);

                        var prop_data = Utils.DeserializeData<PropData>(prop_fld.name);
                        prop_dict[prop_name] = prop_data;
                    }
                }
                return new KeyValuePair<string, object>(fld.name.Substring("-".Length), prop_dict);
            }
            else if (fld.name == "-cams")
            {
                // elif "-propitem:" in fld.name:
                //     prop_state = {}
                //     #id = fld.name.strip("-propitem:")
                //     id = fld.name[10:]
                //     state_fld = HSNeoOCIFolder.create_from_treenode(fld.treeNodeObject.child[0])
                //     prop_state = Utils.DeserializeData(state_fld.name, object_hook=sceneDecoder)
                //     return (id, prop_state)
                // cams
                var cams_dict = new Dictionary<object, object>
                {
                };
                foreach (var child in fld.treeNodeObject.child)
                {
                    var cam = HSNeoOCIFolder.create_from_treenode<HSNeoOCIFolder>(child);
                    KeyValuePair<string, object> kv = this.parseFlds(cam);
                    int id = Convert.ToInt32(kv.Key);
                    cams_dict[id] = (CamData)kv.Value;
                }
                return new KeyValuePair<string, object>(fld.name.Substring("-".Length), cams_dict);
            }
            else if (fld.name.Contains("-cam:"))
            {
                string key = fld.name.Substring("-cam:".Length);
                var state_fld = HSNeoOCIFolder.create_from_treenode<HSNeoOCIFolder>(fld.treeNodeObject.child[0]);
                CamData cam_state = Utils.DeserializeData<CamData>(state_fld.name);
                return new KeyValuePair<string, object>(key, cam_state);
            } 
            else
            {
                throw new Exception("Failed to parse folder");
            }
        }

        public void restrict_to_child(HSNeoOCIFolder fld, int numchilds)
        {
            if (fld.treeNodeObject.child.Count > numchilds)
            {
                var ar = fld.treeNodeObject.child;
                var ar2 = new List<HSNeoOCI>();
                foreach (var treeobj in ar)
                {
                    ar2.Add(HSNeoOCI.create_from_treenode(treeobj));
                }
                foreach (var i in Enumerable.Range(0, ar2.Count))
                {
                    if (i >= numchilds)
                    {
                        Console.WriteLine(String.Format("deleted! {0}", i.ToString()));
                        ar2[i].delete();
                    }
                }
            }
        }

        public void saveSceneData(object param)
        {
            saveSceneData((bool)param);
        }

        public void saveSceneData()
        {
            saveSceneData(backup: false);
        }

        public void saveSceneData(bool backup = false)
        {
            ActorData diff;
            ActorData bestResDiff;
            int bestRes;
            int bestInd;
            HSNeoOCIFolder data_fld;
            // self.saveSceneDataOld(fld,backup)
            // return
            // delete existing scenedata fld
            if (backup)
            {
                data_fld = HSNeoOCIFolder.find_single("-scenesavestatebackup");
                // correct... but be super-confident
                // if data_fld:
                //     pass
                // else:
                //     data_fld = self.createFld("-scenesavestatebackup")
                if (data_fld != null)
                {
                    data_fld.delete();
                }
                data_fld = this.createFld("-scenesavestatebackup");
            }
            else
            {
                data_fld = HSNeoOCIFolder.find_single_startswith("-scenesavestate:");
                var txt = "-scenesavestate:" + this.versionSceneDataParsing;
                if (data_fld != null)
                {
                    data_fld.name = txt;
                }
                else
                {
                    data_fld = this.createFld(txt);
                }
            }
            // save data as flds
            // template = {"fchars":{}, "mchars":{}, "propflds":{}, "cams":[], "accs":{}, "props":{}}
            var sdict = new List<Scene>();
            foreach (var scene in this.block)
            {
                sdict.Add(scene);
            }
            // Create folders as scenedata
            if (this.isSaveOld)
            {
                // txt = data_fld.name
                // data_fld.delete()
                // data_fld = HSNeoOCIFolder.add(txt)
                data_fld.delete_all_children();
            }
            else
            {
                this.restrict_to_child(data_fld, sdict.Count);
            }
            //self.restrict_to_child(data_fld, 0)
            // Create scene data
            //print len(sdict)
            var scene_id = 1;
            foreach (Scene scene in sdict)
            {
                // Making parent folders
                var scene_fld = this.createFldIfNo("-scene:" + scene_id.ToString(), data_fld, scene_id - 1);
                //scene_fld.delete_all_children()
                var actor_fld = this.createFldIfNo("-actors", scene_fld, 0);
                var props_fld = this.createFldIfNo("-props", scene_fld, 1);
                var cams_fld = this.createFldIfNo("-cams", scene_fld, 2);
                // Making children folders
                this.restrict_to_child(actor_fld, scene.actors.Keys.Count);
                var k = 0;
                foreach (var ch_name in scene.actors.Keys)
                {
                    var ch_content = scene.actors[ch_name];
                    //ch_name_fld = self.createFld(ch_name, actor_fld)
                    bestInd = -1;
                    bestRes = 100000;
                    /* TODO
                    if (this.isSaveCompact && !backup)
                    {
                        // optimization - calc diff, only for normal saves
                        if (scene_id > 1)
                        {
                            foreach (var j in Enumerable.Range(0, scene_id - 1))
                            {
                                if (sdict[j].actors.ContainsKey(ch_name))
                                {
                                    diff = Utils.get_status_diff_optimized(sdict[j].actors[ch_name], ch_content);
                                    if (diff.Count < bestRes)
                                    {
                                        bestInd = j;
                                        bestRes = diff.Count;
                                        bestResDiff = diff;
                                    }
                                    if (diff.Count == 0)
                                    {
                                        // ideal - immediately break
                                        break;
                                    }
                                }
                            }
                        }
                    } */
                    if (bestInd == -1)
                    {
                        // no optimization
                        //print "non-opt"
                        //self.createFld(MessagePackSerializer.Serialize(ch_content, cls=SceneEncoder), ch_name_fld)
                        //this.createFldIfNo((new Dictionary<string, ActorData> { { ch_name, ch_content}}), actor_fld, k);
                        this.createFldIfNo(Utils.SerializeData(scene.actors["act0"]), actor_fld, k);
                    }
                    /* TODO
                    else
                    {
                        //self.createFld(MessagePackSerializer.Serialize({'_diff':[bestInd, bestResDiff]}, cls=SceneEncoder), ch_name_fld)
                        this.createFldIfNo(Utils.SerializeData<Dictionary<string, ActorData>>(new Dictionary<string, ActorData> {
                            {
                                ch_name,
                                new Dictionary<string, ActorData> {
                                    {
                                        "_diff",
                                        new List<ActorData> {
                                            bestInd,
                                            bestResDiff
                                        }}}}}), actor_fld, k);
                    }
                    */
                    k += 1;
                }
                this.restrict_to_child(cams_fld, scene.cams.Count);
                foreach (var i in Enumerable.Range(0, scene.cams.Count - 0))
                {
                    var cam_id_fld = this.createFldIfNo("-cam:" + i.ToString(), cams_fld, i);
                    this.createFldIfNo(Utils.SerializeData(scene.cams[i]), cam_id_fld, 0);
                }
                this.restrict_to_child(props_fld, scene.props.Count);
                k = 0;
                foreach (var prop_id in scene.props.Keys)
                {
                    var prop_state = scene.props[prop_id];
                    //print "prop"
                    //prop_id_fld = self.createFld(prop_id, props_fld)
                    bestInd = -1;
                    bestRes = 100000;

                    /* TODO
                    if (this.isSaveCompact && !backup)
                    {
                        // optimization - calc diff, only for normal saves
                        if (scene_id > 1)
                        {
                            foreach (var j in Enumerable.Range(0, scene_id - 1))
                            {
                                if (sdict[j].props.ContainsKey(prop_id))
                                {
                                    diff = Utils.get_status_diff_optimized(sdict[j].props[prop_id], prop_state);
                                    if (diff.Count < bestRes)
                                    {
                                        bestInd = j;
                                        bestRes = diff.Count;
                                        bestResDiff = diff;
                                    }
                                    if (diff.Count == 0)
                                    {
                                        // ideal - immediately break
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    */
                    if (bestInd == -1)
                    {
                        // no optimization
                        // print "non-opt"
                        //self.createFld(MessagePackSerializer.Serialize(prop_state, cls=SceneEncoder), prop_id_fld)
                        this.createFldIfNo(Utils.SerializeData(new Dictionary<object, object> {
                            {
                                prop_id,
                                prop_state}}), props_fld, k);
                    }
                    /* TODO
                    else
                    {
                        //self.createFld(MessagePackSerializer.Serialize({'_diff': [bestInd, bestResDiff]}, cls=SceneEncoder), prop_id_fld)
                        this.createFldIfNo(Utils.SerializeData(new Dictionary<object, object> {
                            {
                                prop_id,
                                new Dictionary<object, object> {
                                    {
                                        "_diff",
                                        new List<object> {
                                            bestInd,
                                            bestResDiff
                                        }}}}}), props_fld, k);
                    }
                    */
                    k += 1;
                }
                scene_id = scene_id + 1;
            }
            data_fld.visible_treenode = false;
            this.game.set_timer(0.1f, this.onDataSaved);
        }

        public int verify_load()
        {
            try
            {
                var blockold = this.block;
                List<Scene> blocknew;
                try
                {
                    this.loadSceneData(setToFirst: false);
                    blocknew = this.block;
                    this.block = blockold;
                }
                catch (Exception e)
                {
                    this.block = blockold;
                    Console.WriteLine(String.Format("Error verify loading data - {0}", e.ToString()));
                    return -100000;
                }
                var diffs = 0;
                if (blocknew.Count == blockold.Count)
                {
                    foreach (var i in Enumerable.Range(0, blocknew.Count))
                    {
                        // diff = get_status_diff_optimized(blocknew[i],blockold[i])
                        // if len(diff) > 0:
                        //     diffs += 1
                        if (!blocknew[i].isEqual(blockold[i]))
                        {
                            Console.WriteLine(String.Format("Verify: Non-eq scene: {0}", (i + 1).ToString()));
                            diffs += 1;
                        }
                    }
                }
                else
                {
                    diffs = blocknew.Count - blockold.Count;
                    Console.WriteLine(String.Format("Diff scene length: {0}", diffs.ToString()));
                }
                if (diffs == 0)
                {
                    // self.show_blocking_message_time_sc("Data saved! (Verify error: %s differences!)" % (str(len(diff))))
                    //self.show_blocking_message_time_sc("Data saved! (Verify: OK)")
                    return 0;
                }
                else
                {
                    // self.show_blocking_message_time_sc(
                    //     "Data saved! (Verify: %s potential problems, see Console!)" % (str(diffs)))
                    return diffs;
                }
            }
            catch (Exception e)
            {
                //self.show_blocking_message_time_sc("Data saved! (Verify: INTERNAL ERROR!)")
                Console.WriteLine(String.Format("Error verify {0}", e.ToString()));
                return -100000;
            }
        }

        public void onDataSaved(VNController game)
        {
            //dt = Time.deltaTime
            //self.show_blocking_message_time_sc("Data saved in %.1f s!" % (dt))
            if (!this.isSaveVerify)
            {
                this.show_blocking_message_time_sc("Data saved!");
            }
            else
            {
                //diff = get_status_diff_optimized(blockold,blocknew)
                var diffs = this.verify_load();
                if (diffs == 0)
                {
                    this.show_blocking_message_time_sc("Data saved! (Verify: OK)");
                }
                else if (diffs == -100000)
                {
                    this.show_blocking_message_time_sc("Data saved! (Verify: INTERNAL ERROR!)");
                }
                else if (this.isSaveOld)
                {
                    this.show_blocking_message_time_sc(String.Format("Data saved! (Verify: OK)\n(some {0} potential misequals, seems be ok)", diffs.ToString()));
                }
                else
                {
                    this.show_blocking_message_time_sc(String.Format("Data saved! (Verify: {0} potential problems, see Console!)", diffs.ToString()));
                }
            }
        }

        public void saveToFile(object param)
        {
            saveToFile((bool)param);
        }

        public void saveToFile(bool backup = false)
        {
            string filename;
            HSNeoOCIFolder fld;
            // Template
            // template = {"fchars":{}, "mchars":{}, "propflds":{}, "cams":[], "accs":{}, "props":{}}
            if (this.svname == "")
            {
                fld = Utils.getFolder(this.game, "-scfile:", false);
                if (!(fld == null))
                {
                    this.svname = fld.text_name.Substring("-scfile:".Length);
                }
            }
            else
            {
                fld = Utils.getFolder(this.game, "-scfile:" + this.svname, true);
                if (fld == null)
                {
                    // create pointer fld
                    this.createFld("-scfile:" + this.svname, parent: null, ret: false);
                }
            }
            if (!(this.svname == ""))
            {
                if (backup == false)
                {
                    filename = this.svname.ToString();
                }
                else
                {
                    filename = this.svname.ToString() + "_backup";
                }
                this.saveToFileDirect(filename);
            }
        }

        public class save_data
        {
            public Dictionary<string, ActorData> actors;
            public List<CamData> cams;
            public Dictionary<string, PropData> props;

            public save_data()
            {

            }
        }

        public void saveToFileDirect(string filename)
        {
            var save_data = new List<save_data>();
            foreach (var i in Enumerable.Range(0, this.block.Count - 0))
            {
                save_data[i].actors = block[i].actors;
                save_data[i].cams = block[i].cams;
                save_data[i].props = block[i].props;
            }
            // save file
            var script_dir = Path.GetDirectoryName(Application.dataPath);
            var folder_path = "sssdata/";
            //if backup == False:
            var file_path = folder_path + filename.ToString() + ".txt";
            //else:
            //file_path = folder_path + str(self.svname) + "_backup.txt"
            var abs_file_path = Path.Combine(script_dir, file_path);  
            /*
            System.Xml.Serialization.XmlSerializer writer =
            new System.Xml.Serialization.XmlSerializer(typeof(List<save_data>));
            FileStream file = System.IO.File.Create(abs_file_path);
            writer.Serialize(file, save_data);
            */
            /*
            using (System.IO.StreamWriter file =
            new System.IO.StreamWriter(abs_file_path))
            {

                file.Write(MessagePackSerializer.Serialize(save_data));
            }
            */
        }

        public Dictionary<string, Scene> loadFromFileDirect(string filename)
        {
            var script_dir = Path.GetDirectoryName(Application.dataPath);
            var folder_path = "sssdata/";
            var file_path = folder_path + filename.ToString() + ".txt";
            var abs_file_path = Path.Combine(script_dir, file_path);
            if (File.Exists(abs_file_path))
            {
                string text = File.ReadAllText(abs_file_path);
                var block_dict = Utils.DeserializeData<Dictionary<string, Scene>>(text);
                return block_dict;
            }
            return null;
        }

        public void loadSceneDataInternalDict(Dictionary<string, Scene> block_dict, bool file)
        {
            if (!(block_dict == null))
            {
                // init zero
                this.dict = new List<object>();
                this.dictparse = new Dictionary<object, object>
                {
                };
                this.scenefile = "";
                this.block = new List<Scene>();
                this.baseacc = new Dictionary<object, object>
                {
                };
                this.accstate = new Dictionary<object, object>
                {
                };
                this.nameset = new List<List<string>> {
                    new List<string>(),
                    new List<string>()
                };
                this.scene_strings = new List<string>();
                // attaining data
                foreach (var key in block_dict.Keys.ToList())
                {
                    var actors = block_dict[key].actors;
                    var props = block_dict[key].props;
                    var cams = new List<CamData>();
                    if (file == true)
                    {
                        cams = block_dict[key].cams;
                        // cams.Add(block_dict[key]["cams"][id])
                    }
                    else
                    {
                        foreach (var cam in block_dict[key].cams)
                        {
                            //print key, id
                            cams.Add(cam);
                        }
                    }
                    //print actors
                    //print props
                    //print cams
                    this.block.Add(new Scene(actors, props, cams));
                    this.scene_strings.Add("Scene " + key.ToString());
                    // id = int(key)
                    // for id in range(0,len(block_dict)):
                    // fchars = block_dict[key]["fchars"]
                    // mchars = block_dict[key]["mchars"]
                    // propflds = block_dict[key]["propflds"]
                    // accs = block_dict[key]["accs"]
                    // props = block_dict[key]["props"]
                    // cams = []
                    // if file == True:
                    //     cams = block_dict[key]["cams"]
                    //     # cams.Add(block_dict[key]["cams"][id])
                    // else:
                    //     for id in sorted(block_dict[key]["cams"]):
                    //         cams.Add(block_dict[key]["cams"][id])
                    // self.block.Add(Scene(fchars, mchars, propflds, accs, cams, props))
                    // self.scene_strings.Add("Scene " + str(key))
                    // self.updateAllSceneChars(tag, newch)
                    // self.updateNameset()
                    // self.updateTagChars()
                    // self.updateTagItems()
                    // self.updateTagPropFlds()
                    // self.updateTagProps()
                    //self.show_blocking_message_time_sc("Scene data loaded!")
                }
            }
        }

        public void loadSceneDataBackupTimer(object param)
        {
            loadSceneDataBackupTimer();
        }

        public void loadSceneDataBackupTimer()
        {
            var block_dict = this.loadFromFileDirect("_backuptimer");
            this.loadSceneDataInternalDict(block_dict, true);
        }

        public void loadSceneData(object param)
        {
            bool[] bool_params = (bool[])param;
            loadSceneData(bool_params[1], bool_params[2], bool_params[3]);
        }

        public void loadSceneData(bool file = false, bool backup = false, bool setToFirst = true)
        {
            string filename;
            this.game.LoadTrackedActorsAndProps();
            Dictionary<string, Scene> block_dict = null;
            if (file == false)
            {
                // get scenedata
                block_dict = this.findAndLoadSceneData(backup: backup);
            }
            else
            {
                if (this.svname == "")
                {
                    var fld = Utils.getFolder(this.game, "-scfile:", false);
                    if (!(fld == null))
                    {
                        this.svname = fld.text_name.Substring("-scfile:".Length);
                    }
                }
                if (backup == false)
                {
                    filename = this.svname.ToString();
                }
                else
                {
                    filename = this.svname.ToString() + "_backup";
                }
                // abs_file_path = os.Path.Combine(script_dir, file_path)
                // if os.File.Exists(abs_file_path):
                //     f = open(abs_file_path, "r")
                //     block_dict = Utils.DeserializeData(f.read(), object_hook=sceneDecoder)  # , indent = 4, separators = (","," : ")))
                //     f.close()
                block_dict = this.loadFromFileDirect(filename);
            }
            this.loadSceneDataInternalDict(block_dict, file);
            // loading
            if (setToFirst)
            {
                if (this.block.Count > 0)
                {
                    this.cur_index = 0;
                    this.cur_cam = 0;
                }
            }
        }

        // Change name
        public void changeCharName(string name)
        {
            var chara = Utils.getSelectedChar(this.game);
            var old_name = chara.text_name;
            chara.objctrl.treeNodeObject.textName = name;
            // for sex in range(len(self.basechars)):
            //     if old_name in self.nameset[sex]:
            //         self.changeSceneChars((1 - sex), tag="upd")
            //         break
            // Duplicate scene
        }

        public void dupScene()
        {
            if (this.block.Count > 0)
            {
                //import copy
                // we have a problem with copy, so... just serialize and back it
                //objstr = MessagePackSerializer.Serialize(self.block[self.cur_index])
                this.block.Insert(this.cur_index, this.block[this.cur_index].copy());
                this.updateSceneStrings();
            }
        }

        // Copy/paste cam set
        public void copyCamSet()
        {
            if (this.cur_index > -1)
            {
                if (this.camset is null)
                {
                    this.camset = new List<CamData>();
                }
                this.camset = this.block[this.cur_index].cams;
            }
        }

        public void pasteCamSet()
        {
            if (this.cur_index > -1)
            {
                this.block[this.cur_index].cams.AddRange(this.camset);
            }
        }

        // Move cam (up/down)
        public void move_cam_up()
        {
            if (this.cur_index > -1 && this.cur_cam > 0)
            {
                var curcam = this.block[this.cur_index].cams[this.cur_cam];
                this.block[this.cur_index].cams[this.cur_cam] = this.block[this.cur_index].cams[this.cur_cam - 1];
                this.cur_cam -= 1;
                this.block[this.cur_index].cams[this.cur_cam] = curcam;
            }
        }

        public void move_cam_down()
        {
            if (this.cur_index > -1 && this.cur_cam < this.block[this.cur_index].cams.Count - 1)
            {
                var curcam = this.block[this.cur_index].cams[this.cur_cam];
                this.block[this.cur_index].cams[this.cur_cam] = this.block[this.cur_index].cams[this.cur_cam + 1];
                this.cur_cam += 1;
                this.block[this.cur_index].cams[this.cur_cam] = curcam;
            }
        }

        // Move scene(up/down)
        public void move_scene_up()
        {
            if (this.cur_index > 0)
            {
                var cursc = this.block[this.cur_index];
                this.block[this.cur_index] = this.block[this.cur_index - 1];
                this.cur_index -= 1;
                this.block[this.cur_index] = cursc;
            }
        }

        public void move_scene_down()
        {
            if (this.cur_index < this.block.Count - 1)
            {
                var cursc = this.block[this.cur_index];
                this.block[this.cur_index] = this.block[this.cur_index + 1];
                this.cur_index += 1;
                this.block[this.cur_index] = cursc;
            }
        }

        // Goto next/prev
        public void goto_first()
        {
            this.cur_index = 0;
            this.loadCurrentScene();
            this.prev_index = this.cur_index;
        }

        public void goto_next()
        {
            if (this.block.Count > 0)
            {
                if (this.block[this.cur_index].cams.Count > 0 && this.cur_cam < this.block[this.cur_index].cams.Count - 1)
                {
                    this.cur_cam += 1;
                    this.setCamera();
                }
                else
                {
                    // elif self.cur_index < (len(self.block) - 1):
                    // self.cur_index += 1
                    this.goto_next_sc();
                }
            }
        }

        public void goto_prev()
        {
            if (this.block.Count > 0)
            {
                this.prev_index = this.cur_index;
                this.prev_cam = this.cur_cam;
                if (this.cur_cam > 0)
                {
                    this.cur_cam -= 1;
                    this.setCamera();
                }
                else
                {
                    // elif self.cur_index > 0:
                    // self.cur_index -= 1
                    this.goto_prev_sc(lastcam: true);
                }
            }
        }

        public void goto_next_sc()
        {
            if (this.block.Count > 0 && this.cur_index < this.block.Count - 1)
            {
                this.cur_index += 1;
                this.loadCurrentScene();
                this.prev_index = this.cur_index;
            }
        }

        public void goto_prev_sc()
        {
            goto_prev_sc(false);
        }

        public void goto_prev_sc(bool lastcam = false)
        {
            if (this.block.Count > 0 && this.cur_index > 0)
            {
                this.cur_index -= 1;
                this.loadCurrentScene();
                this.prev_index = this.cur_index;
                if (lastcam == true && this.block[this.cur_index].cams.Count > 0)
                {
                    this.cur_cam = this.block[this.cur_index].cams.Count - 1;
                    this.setCamera();
                }
            }
        }

        // export cam texts
        public void exportCamTexts()
        {
            var filename = "sss_camtexts.txt";
            try
            {
                using (var f = File.OpenWrite(filename))
                using (var w = new StreamWriter(f))
                {

// var f = codecs.open(filename, "w+", encoding: "utf-8");
                    w.Write("[\n");
                foreach (var i in Enumerable.Range(0, this.block.Count))
                {
                    Scene scene = this.block[i];
                    // only process scene if 1 cam is VN cam - other, skip
                    //cam = scene.cams[0]
                    foreach (var j in Enumerable.Range(0, scene.cams.Count))
                    {
                        var cam = scene.cams[j];
                        var addparams = cam.addata;
                        //if addparams["addparam"]:  # only process if 1 cam is VN cam
                        //fullobj = {'z_sc': i+1, 'z_cam': j, 'who': addparams["whosay"], "say": addparams["whatsay"] }
                        /* TODO
                        var fullobj = (
                            i + 1,
                            j,
                            addparams.addparam,
                            addparams.whosay,
                            addparams.whatsay
                        );*/
                        var res = MessagePackSerializer.Serialize(addparams);
                        w.Write(String.Format("{0},\n", res));
                    }
                }
                w.Write("{}\n");
                w.Write("]\n");
                }
            }
            catch (Exception e)
            {
                this.show_blocking_message_time_sc(String.Format("Can't write to file {0} in game root folder\nerr: {1}", filename, e.ToString()));
                return;
            }
            this.show_blocking_message_time_sc("Cam VN texts exported to sss_camtexts.txt!\nFormat: scene, cam, isVNcam, whosay, whatsay");
        }

        // export cam texts
        public void importCamTexts()
        {
            var filename = "sss_camtexts.txt";
            try
            {
                string filecont = this.game.file_get_content_utf8(filename);
                var arr = Utils.DeserializeData<List<KeyValuePair<int, CamData>>>(filecont);
                foreach (KeyValuePair<int, CamData> kv in arr)
                {
                    var elem = kv.Value;
                    var scenenum = kv.Key;
                        var scene = this.block[scenenum];
                        var cam = scene.cams[elem.camnum];
                        cam.addata = elem.addata;
                        
                        cam.addata.whosay = elem.addata.whosay;
                        cam.addata.whatsay = elem.addata.whatsay;
                }
            }
            catch (Exception e)
            {
                this.show_blocking_message_time_sc(String.Format("Can't import file {0} in game root folder\nerr: {1}", filename, e.ToString()));
                return;
            }
            this.show_blocking_message_time_sc("Cam VN texts import success!");
        }

        public void camSetAll(bool state)
        {
            foreach (var i in Enumerable.Range(0, this.block.Count))
            {
                var scene = this.block[i];
                // only process scene if 1 cam is VN cam - other, skip
                // cam = scene.cams[0]
                foreach (var j in Enumerable.Range(0, scene.cams.Count))
                {
                    CamData cam = scene.cams[j];
                    cam.hasVNData = state;
                }
            }
            this.show_blocking_message_time_sc("Cams changed!");
        }

        // export to VNSceneScript
        public void exportToVNSS()
        {
            //VNExt.exportToVNSS(this, new Dictionary<object, object>()); TODO
            if (this.vnFastIsRunImmediately)
            {
                this.runVNSS("cam");
            }
        }

        public void runVNSS(string starfrom = "begin")
        {
            this.game.gdata.vnbupskin = this.game.skin;
            //self.game.skin_set_byname("skin_renpy")
            //from skin_renpy import SkinRenPy
            var rpySkin = new SkinRenPyMini();
            int calcPos;
            rpySkin.isEndButton = true;
            rpySkin.endButtonTxt = "End script";
            rpySkin.endButtonCall = this.endVNSSbtn;
            this.game.skin_set(rpySkin);
            if (starfrom == "cam")
            {
                //print self.cur_index, self.cur_cam
                calcPos = (this.cur_index + 1) * 100 + this.cur_cam;
            }
            else if (starfrom == "scene")
            {
                calcPos = (this.cur_index + 1) * 100;
            }
            else
            {
                calcPos = 0;
            }
            Console.WriteLine(String.Format("Run VNSS from state {0}", calcPos.ToString()));
            this.game.vnscenescript_run_current(this.onEndVNSS, calcPos.ToString());
        }

        public void endVNSSbtn(VNNeoController game)
        {
            //VNSceneScript.run_state(this.game, this.game.scenedata.scMaxState + 1, true); TODO
        }

        public void onEndVNSS(VNController game = null)
        {
            this.game.skin_set(this.game.gdata.vnbupskin);
        }

        //def _exportAddBlock(self,fld_acode,):
        public string get_next_speaker(string curSpeakAlias, bool next)
        {
            // next from unknown speaker
            var all_actors = this.game.scenef_get_all_actors();
            var keylist = all_actors.Keys.ToList();
            if (curSpeakAlias != "s" && !all_actors.ContainsKey(curSpeakAlias))
            {
                return "s";
            }
            // next from s or actor
            if (curSpeakAlias == "s")
            {
                if (all_actors.Count > 0)
                {
                    if (next)
                    {
                        return keylist[0];
                    }
                    else
                    {
                        return keylist[-1];
                    }
                }
                else
                {
                    return "s";
                }
            }
            else
            {
                var nextIndex = keylist.IndexOf(curSpeakAlias);
                if (next)
                {
                    nextIndex += 1;
                }
                else
                {
                    nextIndex -= 1;
                }
                if (Enumerable.Range(0, all_actors.Count).Contains(nextIndex))
                {
                    return keylist[nextIndex];
                }
                else
                {
                    return "s";
                }
            }
        }

        protected override void OnSceneSave()
        {
            var pluginData = new PluginData();
            pluginData.data["scenes"] = MessagePackSerializer.Serialize(this.block, MessagePack.Resolvers.ContractlessStandardResolver.Instance);
            SetExtendedData(pluginData);
            Console.WriteLine("Saved scene data.");
        }

        protected override void OnSceneLoad(SceneOperationKind operation, ReadOnlyDictionary<int, ObjectCtrlInfo> loadedItems)
        {
            var pluginData = GetExtendedData();
            if (pluginData is null)
            {
                this.block = new List<Scene>();
                this.cur_index = -1;
            }
            else
            {
                var sceneData = pluginData.data["scenes"] as byte[];
                if (!sceneData.IsNullOrEmpty())
                {
                    this.block = MessagePackSerializer.Deserialize<List<Scene>>(sceneData, MessagePack.Resolvers.ContractlessStandardResolver.Instance);
                    Console.WriteLine("Loaded scene data.");
                    this.cur_index = 0;
                }
            }
            updateSceneStrings();
            game.LoadTrackedActorsAndProps();
        }
    }
}
