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
using static VNActor.Item;
using RootMotion;
using System.Text;
using KKAPI.Studio.SaveLoad;
using KKAPI.Utilities;
using ExtensibleSaveFormat;
using NodeCanvas.Tasks.Conditions;
using static VNActor.Light;
using static VNEngine.Utils;
using static VNEngine.VNCamera.VNData;

namespace SceneSaveState
{

    internal class SceneConsole : SceneCustomFunctionController
    {

        public const string defaultSpeakerAlias = "s";

        public bool isSysTracking = true;

        public Dictionary<string, object> accstate;

        public Vector2 adv_scroll;

        public Dictionary<string, object> all_acc;

        public List<HSNeoOCIFolder> arAutoStatesItemsChoice;

        public List<HSNeoOCIFolder> arAutoStatesItemsVis;

        public bool autoAddCam;

        public bool autoLoad;

        public string autoshownewid;

        public double backupTimeCur;

        public double backupTimeDuration;

        public Dictionary<string, object> baseacc;

        public List<List<object>> basechars;

        public Dictionary<string, object> basepropflds;

        public Dictionary<string, object> baseprops;

        public SceneManager block;

        public VNData currentVNData;

        public List<CamData> camset;

        public string charname;

        public IDataClass clipboard_status;

        public IDataClass clipboard_status2;

        public string[] consolenames;

        //public int cur_cam;

        public List<object> dict;

        public Dictionary<string, object> dictparse;

        public List<List<object>> dupchars;

        public int fset_index;

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

        public int mset_index;

        public List<List<string>> nameset;

        public string newid;

        public string nor_font_col;

        public string[] options;

        public float paramAnimCamDuration;

        public bool paramAnimCamIfPossible;

        public string paramAnimCamStyle;

        public float paramAnimCamZoomOut;

        public bool promptOnDelete;

        public List<object> propfldtag;

        public List<object> proptag;

        public string scenefile;

        public List<object> sdict;

        public string sel_font_col;

        //public Dictionary<string, KeyValuePair<string, string>> shortcuts;

        public SkinDefault skinDefault;

        public string skinDefault_sideApp;

        public bool skipClothesChanges;

        public string svname = "";

        public object temp_states;

        public int updAutoStatesTimer;

        public string versionSceneDataParsing;

        public bool vnFastIsRunImmediately;
        internal float consoleWidth;
        internal float consoleHeight;
        internal List<string> fset;
        internal List<string> mset;
        /*
        internal int wiz_step;
        internal Dictionary<string, string> wiz_data;
        internal string wiz_error;
        internal string wiz_view_mode;
        internal Vector2 vnss_wizard_ui_scroll;
        */


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
            guiOnShow = false;
            // --- Essential Data ---
            versionSceneDataParsing = "7.0";
            dict = new List<object>();
            sdict = new List<object>();
            dictparse = new Dictionary<string, object>();
            scenefile = "";
            basechars = new List<List<object>> {
                new List<object>(),
                new List<object>()
            };
            dupchars = new List<List<object>> {
                new List<object>(),
                new List<object>()
            };
            last_acc_id = 0;
            block = new SceneManager();
            all_acc = new Dictionary<string, object>();
            baseacc = new Dictionary<string, object>();
            accstate = new Dictionary<string, object>();
            propfldtag = new List<object>();
            basepropflds = new Dictionary<string, object>();
            proptag = new List<object>();
            baseprops = new Dictionary<string, object>
            {
            };
            // self.basechars = self.getAllBaseChars()
            // self.dupchars = self.getAllDupChars()
            // self.updateNameset()
            // :::: UI Data ::::
            consolenames = new string[] { "SceneSaveState" };
            //this.consolenames = new string[] { "SceneSaveState", "Pose Library", "Scene Utils" };
            options = new string[] { "Edit", "Tracking", "Load/Save", "Advanced", "Ministates" };
            // -- Main --
            sel_font_col = "#f24115";
            nor_font_col = "#f9f9f9";

            fset_index = 0;
            mset_index = 0;

            // self.char_name = ""

            currentVNData = new VNData()
            {
                enabled = false,
                whosay = "",
                whatsay = "",
                addvncmds = "",
                addprops = new addprops_struct()
            };

            currentVNData.addprops.a1 = false;
            currentVNData.addprops.a2 = false;

            newid = "";
            mininewid = "";
            autoshownewid = "";
            isUseMsAuto = false;
            // -- Load/Save --
            svname = "";
            ldname = "";
            // self.optionint = 0
            // -- Advanced --
            adv_scroll = new Vector2(0, 0);
            temp_states = null;
            charname = "";
            autoLoad = true;
            autoAddCam = true;
            promptOnDelete = true;
            skipClothesChanges = false;
            //this.shortcuts = new Dictionary<string, (string, string)>();
            paramAnimCamDuration = 1.5f;
            paramAnimCamStyle = "fast-slow";
            paramAnimCamZoomOut = 0.0f;
            paramAnimCamIfPossible = Utils.is_ini_value_true("AnimateCamIfPossible");
            // self.nwindowRect = None
            // -- Edit window --
            // Common data
            nameset = new List<List<string>> {
                new List<string>(),
                new List<string>()
            };
            camset = new List<CamData>();
            isSaveCompact = true;
            isSaveVerify = true;
            isSaveOld = false;
            updAutoStatesTimer = 0;
            arAutoStatesItemsVis = new List<HSNeoOCIFolder>();
            arAutoStatesItemsChoice = new List<HSNeoOCIFolder>();
            backupTimeDuration = Utils.get_ini_value_def_int("AutoBackupTimeInSeconds", 600);
            backupTimeCur = backupTimeDuration;
            vnFastIsRunImmediately = false;
            // blocking message
            funcLockedText = "...";
            isFuncLocked = false;
            // skin_default internal
            skinDefault = new SkinDefault
            {
                controller = game
            };
            skinDefault_sideApp = "";
            Instance = this;
        }


        // Blocking message functions
        public void show_blocking_message(string text = "...")
        {
            funcLockedText = text;
            isFuncLocked = true;
        }

        public void hide_blocking_message(object game = null)
        {
            isFuncLocked = false;
        }

        public void show_blocking_message_time_sc(string text = "...", float duration = 3f)
        {
            show_blocking_message(text);
            game.set_timer(duration, hide_blocking_message);
        }

        // ---------- ministates ------------
        public void addSelectedAutoShow(string param)
        {
            // get list of sel objs
            var arSel = get_selected_objs();
            if (arSel.Count == 0)
            {
                show_blocking_message_time_sc("No selection!");
                return;
            }
            foreach (var actprop in arSel)
            {
                //print actprop
                //if hasattr(actprop, 'as_prop'):
                if (actprop is Actor chara)
                {
                    //id = self.find_item_in_objlist(actprop.objctrl)
                }
                else
                {
                    var txtname = autoshownewid;
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
            autoshownewid = "";
        }

        public void delSelectedAutoShow()
        {
            // get list of sel objs
            var arSel = get_selected_objs();
            if (arSel.Count == 0)
            {
                show_blocking_message_time_sc("No selection!");
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
            var name = mininewid;
            if (name == "")
            {
                name = "state";
            }
            // get list of sel objs
            var arSel = get_selected_objs();
            if (arSel.Count == 0)
            {
                show_blocking_message_time_sc("No selection!");
                return;
            }
            var objSave = new Dictionary<string, IDataClass>
            {
            };
            foreach (Actor actprop in arSel)
            {
                //if isinstance(actprop, Actor):
                var id = find_item_in_objlist(actprop.objctrl);
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
            mininewid = "";
        }

        public int find_item_in_objlist(ObjectCtrlInfo obj)
        {
            var dobjctrl = game.studio.dicObjectCtrl;
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
            var mtreeman = game.studio.treeNodeCtrl;
            var ar = new List<HSNeoOCI>();
            foreach (var node in mtreeman.selectNodes)
            {
                var ochar = HSNeoOCI.create_from_treenode(node);
                if (ochar is Actor chara)
                {
                    ar.Add(chara);
                }
                else if (ochar is HSNeoOCIProp prop)
                {
                    ar.Add(prop);
                }
                else
                {
                    throw new Exception("Invalid object");
                }
            }
            return ar;
        }

        public void getSceneCamString()
        {

        }

        public void deleteSceneCam(object param)
        {
            changeSceneCam(CamTask.DELETE);
        }
             
        public void changeSceneCam()
        {
            changeSceneCam(task: CamTask.ADD);
        }

        public enum CamTask
        {
            UPDATE,
            DELETE,
            ADD
        }

        public void changeSceneCam(CamTask task)
        {
            var cdata = VNNeoController.cameraData;
            var addata = currentVNData;
            var cam_data = new CamData(cdata.pos, cdata.rotate, cdata.distance, cdata.parse, addata);
            if (task == CamTask.ADD)
            {
                block.AddCam(cam_data);
            }
            else if (task == CamTask.UPDATE)
            {
                block.UpdateCam(cam_data);
            }
            else if (task == CamTask.DELETE)
            {
                var cur_cam = block.DeleteCam();
                if (cur_cam > -1)
                {
                    setCamera();
                }
            }
            if (!(task == CamTask.UPDATE))
            {
                getSceneCamString();
            }
        }

        public void setCamera()
        {
            setCamera(paramAnimCamIfPossible);
        }

        public void setCamera(bool isAnimated)
        {
            VNCamera.CamData camera_data = block.CurrentCam;
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
                game.anim_to_camera(paramAnimCamDuration, pos: camera_data.position, distance: camera_data.distance, rotate: camera_data.rotation, fov: camera_data.fov, style: style);
            }
            else
            {
                game.move_camera(camera_data);
                //this.game.move_camera(pos: camera_data.position, distance: camera_data.distance, rotate: camera_data.rotation, fov: camera_data.fov);
            }
            if (camera_data.addata is VNData addata)
            {
                currentVNData.enabled = addata.enabled;
                currentVNData.whosay = addata.whosay;
                currentVNData.whatsay = addata.whatsay;
                if (addata.addvncmds != null)
                {
                    currentVNData.addvncmds = addata.addvncmds;
                }
                else
                {
                    currentVNData.addvncmds = "";
                }

                currentVNData.addprops = addata.addprops;               

                game.set_text(camera_data.addata.whosay, camera_data.addata.whatsay);
            }
            else
            {
                currentVNData.enabled = false;
                currentVNData.whosay = "";
                currentVNData.whatsay = "";
                currentVNData.addvncmds = "";
                currentVNData.addprops.a1 = false;
                currentVNData.addprops.a2 = false;
            }
        }

        public void addAutoWithMsg()
        {
            addAuto();
            show_blocking_message_time_sc("Scene added!", 2.0f);
        }

        public void UpdateScene()
        {
            if (block.HasScenes)
            {
                Scene scene = new Scene(game, isSysTracking);
                block.Update(scene);
            }
        }

        public void addAuto(bool insert = false, bool addsc = true, bool allbase = true)
        {
            Scene scene = new Scene(game, isSysTracking);
            if (insert)
            {
                block.Insert(scene);
            }
            else
            {
                block.Add(scene);
            }
            if (addsc == true)
            {
                if (autoAddCam == true)
                {
                    changeSceneCam(CamTask.ADD);
                }
            }           
        }

        // Remove stuff

        public void removeScene(object param)
        {
            removeScene();
        }
        public void removeScene()
        {
            if (block.HasScenes)
            {
                block.RemoveScene();
            }
        }

        // Load scene

        public void loadCurrentScene()
        {
            setSceneState(block.CurrentScene);
            if (block.Count > 0 && block.currentCamCount > 0)
            {
                block.FirstCam();
                setCamera();
            }
        }

        public void setSceneState(Scene s)
        {
            var game = VNNeoController.Instance;
            if (isSysTracking)
            {
                VNEngine.System.import_status(s.sys);
            }
            s.setSceneState(game);
        }

        public void copySelectedStatusToTracking(List<string> exclude)
        {
            var elem = HSNeoOCI.create_from_selected();
            if (elem is Actor chara)
            {
                var tmp_status = chara.export_full_status();
                var actors = game.scenef_get_all_actors();
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
                show_blocking_message_time_sc("Can't find tracking char with same name");
            }
            else
            {
                show_blocking_message_time_sc("Can't copy status");
            }
        }

        public void copySelectedStatus()
        {
            var elem = HSNeoOCI.create_from_selected();
            if (elem is Actor chara)
            {
                clipboard_status = ((Actor)chara).export_full_status();
            }
            else if (elem is HSNeoOCIProp prop)
            {
                clipboard_status = prop.export_full_status();
            }
            else
            {
                show_blocking_message_time_sc("Can't copy status");
            }
        }

        public void pasteSelectedStatus()
        {
            var elem = HSNeoOCI.create_from_selected();
            if (elem is Actor chara)
            {
                chara.import_status(clipboard_status);
            }
            else if (elem is HSNeoOCIProp prop)
            {
                prop.import_status((ItemData)clipboard_status);
            }
            else
            {
                show_blocking_message_time_sc("Can't paste status");
            }
        }

        public void copySelectedStatus2()
        {
            var elem = HSNeoOCI.create_from_selected();
            if (elem is Actor chara)
            {
                clipboard_status2 = chara.export_full_status();
            }
            else if (elem is HSNeoOCIProp prop)
            {
                clipboard_status2 = prop.export_full_status();
            }
            else
            {
                show_blocking_message_time_sc("Can't copy status 2");
            }
        }

        public void pasteSelectedStatus2()
        {
            var elem = HSNeoOCI.create_from_selected();
            if (elem is Actor chara)
            {
                chara.import_status((ActorData)clipboard_status2);
            }
            else if (elem is HSNeoOCIProp prop)
            {
                prop.import_status((ItemData)clipboard_status2);
            }
            else
            {
                show_blocking_message_time_sc("Can't paste status 2");
            }
        }   

        public void addSysTracking()
        {
            if (block.Count > 0)
            {
                var curstatus = VNEngine.System.export_sys_status(game);
                foreach (var i in Enumerable.Range(0, block.Count))
                {
                    Scene scene = block[i];
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
            foreach (var i in Enumerable.Range(0, block.Count))
            {
                var scene = block[i];
                scene.actors.Remove("sys");
            }
            isSysTracking = false;
        }


        public void addSelectedToTrack(HSNeoOCIProp prop)
        {
            HSNeoOCIFolder tagfld;
            var props = game.scenef_get_all_props();

            foreach (HSNeoOCIProp p in props.Values)
            {
                if (p.objctrl == prop.objctrl)
                {
                    return;
                }
            }

            string baseid = "";
            string newid = "";
            if (prop is Item)
            {
                baseid = "item";
            }
            else if (prop is VNActor.Light)
            {
                baseid = "light";
            }
            foreach (var i in Enumerable.Range(0, 1000 - 0))
            {
                var id = baseid + i;
                
                if (props.ContainsKey(id))
                {
                }
                else
                {
                    newid = id;
                    break;
                }
            }
            if (prop is VNActor.Light)
            {
                tagfld = HSNeoOCIFolder.add(VNNeoController.light_folder_prefix + newid);
                prop.set_parent(tagfld);
            }
            else if (prop is Route)
            {
                tagfld = HSNeoOCIFolder.add("-propgrandpa:" + newid);
                tagfld.set_parent_treenodeobject(prop.treeNodeObject.child[0]);
            }
            else
            {
                tagfld = HSNeoOCIFolder.add(VNNeoController.prop_folder_prefix + newid);
                tagfld.set_parent_treenodeobject(prop.treeNodeObject);
            }
            var curstatus = prop.export_full_status();
            for (int i = 0; i < block.Count; i++)
            {
                Scene scene = block[i];

                if (curstatus is ItemData propData)
                {
                    scene.props[newid] = propData;
                }
                else if (curstatus is LightData lightData)
                {
                    scene.lights[newid] = lightData;
                }             
                // updating set
            }
        }

        public void addSelectedToTrack(Actor chara)
        {
            var actors = game.scenef_get_all_actors();

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
            foreach (var i in Enumerable.Range(0, block.Count))
            {
                Scene scene = block[i];
                scene.actors[id] = curstatus;
            }
        }

        public void addSelectedToTrack()
        {
            var objects = KKAPI.Studio.StudioAPI.GetSelectedObjects();

            if (objects.Count() == 0)
            {
                show_blocking_message_time_sc("Nothing selected");
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

        public void changeSelTrackID(string toId)
        {
            if (toId == "")
            {
                show_blocking_message_time_sc("Please, set ID to change to first");
                return;
            }
            var elem = HSNeoOCI.create_from_selected();
            if (elem == null)
            {
                show_blocking_message_time_sc("Nothing selected");
                return;
            }
            if (elem is Actor chara)
            {
                var actors = game.scenef_get_all_actors();
                string id = "";
                foreach (var actid in actors.Keys)
                {
                    if (actors[actid].objctrl == elem.objctrl)
                    {
                        // found
                        id = actid;
                        break;
                    }
                }
                //self.delActorFromTrack(actid)
                if (id == "")
                {
                    show_blocking_message_time_sc("Can't find actor to change ID");
                    return;
                }
                // actually changing ID
                changeActorTrackId(id, toId);
            }
            // updating set
            game.LoadTrackedActorsAndProps();
        }

        public void delSelectedFromTrack(object o)
        {
            delSelectedFromTrack();
        } 


        public void delSelectedFromTrack()
        {
            var elem = HSNeoOCI.create_from_selected();
            if (elem == null)
            {
                show_blocking_message_time_sc("Nothing selected");
                return;
            }
            if (elem is Actor chara)
            {
                var actors = game.scenef_get_all_actors();
                var id = "";
                foreach (var actid in actors.Keys)
                {
                    if (actors[actid].objctrl == elem.objctrl)
                    {
                        // found
                        id = actid;
                        break;
                    }
                }
                if (id == "")
                {
                    show_blocking_message_time_sc("Can't delete; seems this actor is not tracking yet");
                    return;
                }
                delActorFromTrack(id);
            }
            else if (elem is HSNeoOCIProp)
            {
                var props = game.scenef_get_all_props();
                var id = "";
                foreach (var propid in props.Keys)
                {
                    if (props[propid].objctrl == elem.objctrl)
                    {
                        // found
                        break;
                    }
                }
                delPropFromTrack(id);
            }
            // updating set
            game.LoadTrackedActorsAndProps();
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
                foreach (var i in Enumerable.Range(0, block.Count))
                {
                    var scene = block[i];
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
                foreach (var i in Enumerable.Range(0, block.Count))
                {
                    var scene = block[i];
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
                foreach (var i in Enumerable.Range(0, block.Count))
                {
                    var scene = block[i];
                    scene.props.Remove(propid);
                }
            }
        }

        public static HSNeoOCIFolder createFld(string txt, HSNeoOCI parent = null, bool ret = true)
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

        public static HSNeoOCIFolder createFldIfNo(string txt, HSNeoOCIFolder parent, int childNum)
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
                    fld = HSNeoOCI.create_from_treenode(chld) as HSNeoOCIFolder;
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
        /*
        public int verify_load()
        {
            try
            {
                var blockold = block;
                List<Scene> blocknew;
                try
                {
                    loadSceneData(setToFirst: false);
                    blocknew = block;
                    block = blockold;
                }
                catch (Exception e)
                {
                    block = blockold;
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
        */

        /*

        public void onDataSaved(VNController game)
        {
            //dt = Time.deltaTime
            //self.show_blocking_message_time_sc("Data saved in %.1f s!" % (dt))
            if (!isSaveVerify)
            {
                show_blocking_message_time_sc("Data saved!");
            }
            else
            {
                //diff = get_status_diff_optimized(blockold,blocknew)
                var diffs = verify_load();
                if (diffs == 0)
                {
                    show_blocking_message_time_sc("Data saved! (Verify: OK)");
                }
                else if (diffs == -100000)
                {
                    show_blocking_message_time_sc("Data saved! (Verify: INTERNAL ERROR!)");
                }
                else if (isSaveOld)
                {
                    show_blocking_message_time_sc(String.Format("Data saved! (Verify: OK)\n(some {0} potential misequals, seems be ok)", diffs.ToString()));
                }
                else
                {
                    show_blocking_message_time_sc(String.Format("Data saved! (Verify: {0} potential problems, see Console!)", diffs.ToString()));
                }
            }
        }
        */

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
            if (svname == "")
            {
                fld = Utils.getFolder(game, "-scfile:", false);
                if (!(fld == null))
                {
                    svname = fld.text_name.Substring("-scfile:".Length);
                }
            }
            else
            {
                fld = Utils.getFolder(game, "-scfile:" + svname, true);
                if (fld == null)
                {
                    // create pointer fld
                    createFld("-scfile:" + svname, parent: null, ret: false);
                }
            }
            if (!(svname == ""))
            {
                if (backup == false)
                {
                    filename = svname.ToString();
                }
                else
                {
                    filename = svname.ToString() + "_backup";
                }
                saveToFileDirect(filename);
            }
        }

        public class save_data
        {
            public Dictionary<string, ActorData> actors;
            public List<CamData> cams;
            public Dictionary<string, ItemData> props;

            public save_data()
            {

            }
        }

        public void saveToFileDirect(string filename)
        {
            var save_data = new List<save_data>();
            foreach (var i in Enumerable.Range(0, block.Count - 0))
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

        public static Dictionary<string, Scene> loadFromFileDirect(string filename)
        {
            var script_dir = Path.GetDirectoryName(Application.dataPath);
            var folder_path = "sssdata/";
            var file_path = folder_path + filename.ToString() + ".txt";
            var abs_file_path = Path.Combine(script_dir, file_path);
            if (File.Exists(abs_file_path))
            {
                byte[] data = File.ReadAllBytes(abs_file_path);
                var block_dict = Utils.DeserializeData<Dictionary<string, Scene>>(data);
                return block_dict;
            }
            return null;
        }

        public void loadSceneDataInternalDict(Dictionary<string, Scene> block_dict, bool file)
        {
            if (!(block_dict == null))
            {
                // init zero
                dict = new List<object>();
                dictparse = new Dictionary<string, object>
                {
                };
                scenefile = "";
                baseacc = new Dictionary<string, object>
                {
                };
                accstate = new Dictionary<string, object>
                {
                };
                nameset = new List<List<string>> {
                    new List<string>(),
                    new List<string>()
                };
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
                    block.Add(new Scene(actors, props, null, cams));

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
            var block_dict = loadFromFileDirect("_backuptimer");
            loadSceneDataInternalDict(block_dict, true);
        }

        public void loadSceneData(object param)
        {
            bool[] bool_params = (bool[])param;
            loadSceneData(bool_params[1], bool_params[2], bool_params[3]);
        }

        public void loadSceneData(bool file = false, bool backup = false, bool setToFirst = true)
        {
            string filename;
            game.LoadTrackedActorsAndProps();
            Dictionary<string, Scene> block_dict = null;
            if (file == false)
            {
                // get scenedata
                //block_dict = this.findAndLoadSceneData(backup: backup);
            }
            else
            {
                if (svname == "")
                {
                    var fld = Utils.getFolder(game, "-scfile:", false);
                    if (!(fld == null))
                    {
                        svname = fld.text_name.Substring("-scfile:".Length);
                    }
                }
                if (backup == false)
                {
                    filename = svname.ToString();
                }
                else
                {
                    filename = svname.ToString() + "_backup";
                }
                // abs_file_path = os.Path.Combine(script_dir, file_path)
                // if os.File.Exists(abs_file_path):
                //     f = open(abs_file_path, "r")
                //     block_dict = Utils.DeserializeData(f.read(), object_hook=sceneDecoder)  # , indent = 4, separators = (","," : ")))
                //     f.close()
                block_dict = loadFromFileDirect(filename);
            }
            loadSceneDataInternalDict(block_dict, file);
            // loading
            if (setToFirst)
            {
                if (block.HasScenes)
                {
                    block.First();
                    block.FirstCam();
                }
            }
        }

        // Change name
        public static void changeCharName(VNNeoController game, string name)
        {
            var chara = Utils.getSelectedChar(game);
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
            if (block.Count > 0)
            {
                //import copy
                // we have a problem with copy, so... just serialize and back it
                //objstr = MessagePackSerializer.Serialize(self.block[self.cur_index])
                block.Insert(block.CurrentScene.copy());
            }
        }

        // Copy/paste cam set
        public void copyCamSet()
        {
            if (block.HasScenes)
            {
                if (camset is null)
                {
                    camset = new List<CamData>();
                }
                camset = block.CurrentScene.cams;
            }
        }

        public void pasteCamSet()
        {
            if (block.HasScenes)
            {
                block.CurrentScene.cams.AddRange(camset);
            }
        }



        // Goto next/prev
        public void goto_first()
        {
            block.First();
            loadCurrentScene();
        }

        public void goto_next(VNController game, int i)
        {
            goto_next();
        }

        public void goto_next()
        {
            if (block.Count > 0)
            {
                if (block.currentCamCount > 0 && block.HasNextCam)
                {
                    block.NextCam();
                    setCamera();
                }
                else
                {
                    // elif self.cur_index < (len(self.block) - 1):
                    // self.cur_index += 1
                    goto_next_sc();
                }
            }
        }

        public void goto_prev()
        {
            if (block.HasPrev)
            {
                if (block.currentCamIndex > 0)
                {
                    block.PrevCam();
                    setCamera();
                }
                else
                {
                    // elif self.cur_index > 0:
                    // self.cur_index -= 1
                    goto_prev_sc(lastcam: true);
                }
            }
        }

        public void goto_next_sc()
        {
            if (block.HasNext)
            {
                block.Next();
                loadCurrentScene();
            }
        }

        public void goto_prev_sc()
        {
            goto_prev_sc(false);
        }

        public void goto_prev_sc(bool lastcam = false)
        {
            if (block.HasPrev)
            {
                block.Back();
                loadCurrentScene();
                if (lastcam == true && block.currentCamCount > 0)
                {
                    block.LastCam();
                    setCamera();
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
                foreach (var i in Enumerable.Range(0, block.Count))
                {
                    Scene scene = block[i];
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
                show_blocking_message_time_sc(String.Format("Can't write to file {0} in game root folder\nerr: {1}", filename, e.ToString()));
                return;
            }
            show_blocking_message_time_sc("Cam VN texts exported to sss_camtexts.txt!\nFormat: scene, cam, isVNcam, whosay, whatsay");
        }

        // export cam texts
        public void importCamTexts()
        {
            var filename = "sss_camtexts.txt";
            try
            {
                string filecont = game.file_get_content_utf8(filename);
                List<CamData> arr = new List<CamData>(); // Utils.DeserializeData<List<KeyValuePair<int, CamData>>>(filecont); TODO
                for (int i = 0; i < arr.Count; i++)
                {
                    
                    var elem = arr[i];
                    var scenenum = i;
                        var scene = block[scenenum];
                        var cam = scene.cams[elem.camnum];
                        cam.addata = elem.addata;
                        
                        cam.addata.whosay = elem.addata.whosay;
                        cam.addata.whatsay = elem.addata.whatsay;
                }
            }
            catch (Exception e)
            {
                show_blocking_message_time_sc(String.Format("Can't import file {0} in game root folder\nerr: {1}", filename, e.ToString()));
                return;
            }
            show_blocking_message_time_sc("Cam VN texts import success!");
        }

        public void camSetAll(bool state)
        {
            foreach (var i in Enumerable.Range(0, block.Count))
            {
                var scene = block[i];
                // only process scene if 1 cam is VN cam - other, skip
                // cam = scene.cams[0]
                foreach (var j in Enumerable.Range(0, scene.cams.Count))
                {
                    CamData cam = scene.cams[j];
                    cam.hasVNData = state;
                }
            }
            show_blocking_message_time_sc("Cams changed!");
        }

        // export to VNSceneScript
        public void exportToVNSS()
        {
            //VNExt.exportToVNSS(this, new Dictionary<object, object>()); TODO
            if (vnFastIsRunImmediately)
            {
                runVNSS("cam");
            }
        }

        public void runVNSS(string starfrom = "begin")
        {
            //this.game.gdata.vnbupskin = this.game.skin;
            //self.game.skin_set_byname("skin_renpy")
            //from skin_renpy import SkinRenPy
            var rpySkin = new SkinRenPyMini();
            int calcPos;
            rpySkin.isEndButton = true;
            rpySkin.endButtonTxt = "X";
            rpySkin.endButtonCall = endVNSSbtn;
            game.set_text_s("...");
            game.set_buttons(new List<Button_s>() { new Button_s(">>", goto_next, 1) });
            game.skin_set(rpySkin);
            game.visible = true;
            if (starfrom == "cam")
            {
                //print self.cur_index, self.cur_cam
                calcPos = (block.currentSceneIndex + 1) * 100 + block.currentCamIndex;
            }
            else if (starfrom == "scene")
            {
                calcPos = (block.currentSceneIndex + 1) * 100;
            }
            else
            {
                calcPos = 0;
            }
            block.SetCurrent(calcPos);
            Console.WriteLine(String.Format("Run VNSS from state {0}", calcPos.ToString()));
            game.vnscenescript_run_current(onEndVNSS, calcPos.ToString());
        }

        public void endVNSSbtn(VNNeoController game)
        {
            this.game.visible = false;
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
            var all_actors = game.scenef_get_all_actors();
            var keylist = all_actors.Keys.ToList();
            if (curSpeakAlias != defaultSpeakerAlias && !all_actors.ContainsKey(curSpeakAlias))
            {
                return defaultSpeakerAlias;
            }
            // next from s or actor
            if (curSpeakAlias == defaultSpeakerAlias)
            {
                if (all_actors.Count > 0)
                {
                    if (next)
                    {
                        return keylist[0];
                    }
                    else
                    {
                        return keylist.Last();
                    }
                }
                else
                {
                    return defaultSpeakerAlias;
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
                    return defaultSpeakerAlias;
                }
            }
        }

        public void deleteSaveData()
        {
            SetExtendedData(new PluginData() { data = null });
            block = new SceneManager();
            game.LoadTrackedActorsAndProps();
        }

        protected override void OnSceneSave()
        {
            var pluginData = new PluginData();
            if (block.Count > 0)
            {
                byte[] sceneData = MessagePackSerializer.Serialize(block.ExportScenes(), MessagePack.Resolvers.ContractlessStandardResolver.Instance);
                pluginData.data["scenes"] = sceneData;
                SetExtendedData(pluginData);
                var logger = game.GetLogger;
                logger.LogDebug($"Saved {((double)sceneData.Length / 1000):N} Kbytes of scene data.");
            }
        }

        protected override void OnSceneLoad(SceneOperationKind operation, ReadOnlyDictionary<int, ObjectCtrlInfo> loadedItems)
        {
            var pluginData = GetExtendedData();
            if (pluginData == null || pluginData?.data == null)
            {
                block = new SceneManager();
            }
            else
            {
                byte[] sceneData = pluginData.data["scenes"] as byte[];
                if (!sceneData.IsNullOrEmpty())
                {
                    var logger = game.GetLogger;
                    try
                    {
                        var scenes = MessagePackSerializer.Deserialize<Scene[]>(sceneData, MessagePack.Resolvers.ContractlessStandardResolver.Instance);
                        block = new SceneManager(scenes);
                        logger.LogDebug($"Loaded {((double)sceneData.Length / 1000):N} Kbytes of scene data.");
                    }
                    catch (Exception e)
                    {
                        logger.LogError("Error occurred while loading scene data: " + e.ToString());
                    }
                }
            }
            game.LoadTrackedActorsAndProps();
        }
    }
}
