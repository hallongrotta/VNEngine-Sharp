
/* Unmerged change from project 'SceneSaveState.AI'
Before:
using System;
using System.Collections.Generic;
using UnityEngine;
using VNEngine;
using VNActor;
using System.Linq;
using MessagePack;
using static VNEngine.VNCamera;
After:
using BepInEx.Logging;
using ExtensibleSaveFormat;
using KKAPI.Studio.SaveLoad;
using KKAPI.Utilities;
using MessagePack;
using Studio;
using System;
using System.VNCamera;
*/

/* Unmerged change from project 'SceneSaveState.HS2'
Before:
using System;
using System.Collections.Generic;
using UnityEngine;
using VNEngine;
using VNActor;
using System.Linq;
using MessagePack;
using static VNEngine.VNCamera;
After:
using BepInEx.Logging;
using ExtensibleSaveFormat;
using KKAPI.Studio.SaveLoad;
using KKAPI.Utilities;
using MessagePack;
using Studio;
using System;
using System.VNCamera;
*/
using ExtensibleSaveFormat;
using KKAPI.Studio.SaveLoad;
using KKAPI.Utilities;
using MessagePack;
using Studio;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

/* Unmerged change from project 'SceneSaveState.AI'
Before:
using static VNActor.Actor;
using static VNActor.Item;
using System.Text;
using KKAPI.Studio.SaveLoad;
using KKAPI.Utilities;
using ExtensibleSaveFormat;
After:
using System.Text;
using UnityEngine;
using VNActor;
using VNEngine;
using KKAPI.Actor;
using static VNActor.Item;
*/

/* Unmerged change from project 'SceneSaveState.HS2'
Before:
using static VNActor.Actor;
using static VNActor.Item;
using System.Text;
using KKAPI.Studio.SaveLoad;
using KKAPI.Utilities;
using ExtensibleSaveFormat;
After:
using System.Text;
using UnityEngine;
using VNActor;
using VNEngine;
using KKAPI.Actor;
using static VNActor.Item;
*/
using UnityEngine;
using VNActor;
using VNEngine;
using static VNActor.Actor;
using static VNActor.Item;
using static VNActor.Light;
using static VNEngine.Utils;
using static VNEngine.VNCamera;
using static VNEngine.VNCamera.VNData;

namespace SceneSaveState
{

    internal class SceneConsole : SceneCustomFunctionController
    {
        public const string backup_folder_name = "sssdata";
        public const string defaultSpeakerAlias = "s";
        public const string defaultSaveName = "SSS.dat";
        public const string defaultBackupName = "SSS.dat.backup";

        public bool isSysTracking = true;

        public List<Folder> arAutoStatesItemsChoice;

        public List<Folder> arAutoStatesItemsVis;

        public bool autoAddCam;

        public bool autoLoad;

        public string autoshownewid;

        public double backupTimeCur;

        public double backupTimeDuration;

        public SceneManager block;

        public VNData currentVNData;

        public List<CamData> camset;

        public string charname;

        public IDataClass clipboard_status;

        public IDataClass clipboard_status2;

        //public int cur_cam;

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

        public float paramAnimCamDuration;

        public bool paramAnimCamIfPossible;

        public string paramAnimCamStyle;

        public float paramAnimCamZoomOut;

        public bool promptOnDelete;

        public string sel_font_col;

        //public Dictionary<string, KeyValuePair<string, string>> shortcuts;

        public SkinDefault skinDefault;

        public string skinDefault_sideApp;

        public bool skipClothesChanges;

        public string svname = "";

        public int updAutoStatesTimer;

        public string versionSceneDataParsing;

        public bool vnFastIsRunImmediately;
        internal float consoleWidth;
        internal float consoleHeight;
        internal List<string> fset;
        internal List<string> mset;


        public double saveDataSize { get; private set; }

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
            last_acc_id = 0;
            block = new SceneManager();
            // self.basechars = self.getAllBaseChars()
            // self.dupchars = self.getAllDupChars()
            // self.updateNameset()
            // :::: UI Data ::::

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
            arAutoStatesItemsVis = new List<Folder>();
            arAutoStatesItemsChoice = new List<Folder>();
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
                if (actprop is VNActor.Actor chara)
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
                    var fld = Folder.add("-msauto:" + param + ":" + txtname);
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
            var folders = Folder.find_all_startswith("-msauto:");
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
            var fld = Folder.find_single("-ministates:1.0");
            if (fld == null)
            {
                fld = Folder.add("-ministates:1.0");
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
            foreach (VNActor.Actor actprop in arSel)
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

        public List<NeoOCI> get_selected_objs()
        {
            var mtreeman = game.studio.treeNodeCtrl;
            var ar = new List<NeoOCI>();
            foreach (var node in mtreeman.selectNodes)
            {
                var ochar = NeoOCI.create_from_treenode(node);
                if (ochar is VNActor.Actor chara)
                {
                    ar.Add(chara);
                }
                else if (ochar is Prop prop)
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

        public void deleteSceneCam()
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
            if (camera_data.addata.enabled)
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
            SetSceneState(block.CurrentScene);
            if (block.Count > 0 && block.currentCamCount > 0)
            {
                block.FirstCam();
                setCamera();
            }
        }

        public void copySelectedStatusToTracking(List<string> exclude)
        {
            var elem = NeoOCI.create_from_selected();
            if (elem is VNActor.Actor chara)
            {
                var tmp_status = chara.export_full_status();
                var actors = game.AllActors;
                foreach (var key in actors.Keys)
                {
                    VNActor.Actor actor = (VNActor.Actor)actors[key];
                    if (actor.text_name == chara.text_name)
                    {
                        /* TODO
                        foreach (var keyEx in exclude)
                        {
                            tmp_status.Remove(keyEx);
                        }
                        */
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
            var elem = NeoOCI.create_from_selected();
            if (elem is VNActor.Actor chara)
            {
                clipboard_status = ((VNActor.Actor)chara).export_full_status();
            }
            else if (elem is Prop prop)
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
            var elem = NeoOCI.create_from_selected();
            if (elem is VNActor.Actor chara)
            {
                chara.import_status(clipboard_status);
            }
            else if (elem is Prop prop)
            {
                prop.import_status(clipboard_status);
            }
            else
            {
                show_blocking_message_time_sc("Can't paste status");
            }
        }

        public void copySelectedStatus2()
        {
            var elem = NeoOCI.create_from_selected();
            if (elem is VNActor.Actor chara)
            {
                clipboard_status2 = chara.export_full_status();
            }
            else if (elem is Prop prop)
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
            var elem = NeoOCI.create_from_selected();
            if (elem is VNActor.Actor chara)
            {
                chara.import_status((ActorData)clipboard_status2);
            }
            else if (elem is Prop prop)
            {
                prop.import_status(clipboard_status2);
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


        public void addSelectedToTrack(Prop prop)
        {
            Folder tagfld;
            var props = game.AllProps;

            foreach (Prop p in props.Values)
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
            else if (prop is Folder)
            {
                baseid = "folder";
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
                tagfld = Folder.add(SceneFolders.light_folder_prefix + newid);
                prop.set_parent(tagfld);
            }
            else if (prop is Route)
            {
                tagfld = Folder.add("-propgrandpa:" + newid);
                tagfld.set_parent_treenodeobject(prop.treeNodeObject.child[0]);
            }
            else
            {
                tagfld = Folder.add(SceneFolders.prop_folder_prefix + newid);
                tagfld.set_parent_treenodeobject(prop.treeNodeObject);
            }
            for (int i = 0; i < block.Count; i++)
            {
                Scene scene = block[i];
                scene.AddProp(newid, prop);
            }
        }

        public void addSelectedToTrack(VNActor.Actor chara)
        {
            var actors = game.AllActors;

            foreach (VNActor.Actor actor in actors.Values)
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
            var tagfld = Folder.add(SceneFolders.actor_folder_prefix + id);
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
                        var prop = NeoOCI.create_from(item);
                        addSelectedToTrack(prop);
                    }
                    else if (objectCtrl is OCIChar chara)
                    {
                        var actor = NeoOCI.create_from(chara);
                        addSelectedToTrack(actor);
                    }
                    else if (objectCtrl is OCILight oLight)
                    {
                        var light = NeoOCI.create_from(oLight);
                        addSelectedToTrack(light);
                    }
                    else if (objectCtrl is OCIRoute oRoute)
                    {
                        var route = NeoOCI.create_from(oRoute);
                        addSelectedToTrack(route);
                    }
                    else if (objectCtrl is OCIFolder oFolder)
                    {
                        var fld = NeoOCI.create_from(oFolder);
                        addSelectedToTrack(fld);
                    }
                    else
                    {
                        return;
                    }
                }
                SceneFolders.LoadTrackedActorsAndProps();
            }
        }

        public void changeSelTrackID(string toId)
        {
            if (toId == "")
            {
                show_blocking_message_time_sc("Please, set ID to change to first");
                return;
            }
            var elem = NeoOCI.create_from_selected();
            if (elem == null)
            {
                show_blocking_message_time_sc("Nothing selected");
                return;
            }
            if (elem is VNActor.Actor chara)
            {
                var actors = game.AllActors;
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
            SceneFolders.LoadTrackedActorsAndProps();
        }

        public void delSelectedFromTrack(object o)
        {
            delSelectedFromTrack();
        }


        public void delSelectedFromTrack()
        {
            var elem = NeoOCI.create_from_selected();
            if (elem == null)
            {
                show_blocking_message_time_sc("Nothing selected");
                return;
            }
            if (elem is VNActor.Actor chara)
            {
                var actors = game.AllActors;
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
            else if (elem is Prop)
            {
                var props = game.AllProps;
                var id = "";
                foreach (var propid in props.Keys)
                {
                    if (props[propid].objctrl == elem.objctrl)
                    {
                        id = propid; // found
                        break;
                    }
                }
                delPropFromTrack(id);
            }
            // updating set
            SceneFolders.LoadTrackedActorsAndProps();
        }

        public void delActorFromTrack(string actid)
        {
            if (actid != "")
            {
                // we found this char
                var fld = Folder.find_single(SceneFolders.actor_folder_prefix + actid);
                if (fld == null)
                {
                    fld = Folder.find_single_startswith(SceneFolders.actor_folder_prefix + actid + ":");
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
                var fld = Folder.find_single(SceneFolders.actor_folder_prefix + actid);
                if (fld == null)
                {
                    fld = Folder.find_single_startswith(SceneFolders.actor_folder_prefix + actid + ":");
                }
                // found
                //if fld != None:
                //    fld.delete()
                string fldoldname = fld.name;
                string lastelems = fldoldname.Substring((SceneFolders.actor_folder_prefix + actid).Length);
                //print lastelems
                fld.name = SceneFolders.actor_folder_prefix + toid + lastelems;
                //
                for (int i = 0; i < block.Count; i++)
                {
                    var scene = block[i];
                    scene.actors[toid] = scene.actors[actid];
                    scene.actors.Remove(actid);
                    foreach (var cam in scene.cams)
                    {
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
                var fld = Folder.find_single(SceneFolders.prop_folder_prefix + propid);
                // found
                if (fld != null)
                {
                    fld.delete();
                }
                foreach (var i in Enumerable.Range(0, block.Count))
                {
                    var scene = block[i];
                    scene.RemoveProp(propid);
                }
            }
        }

        public static Folder createFld(string txt, NeoOCI parent = null, bool ret = true)
        {
            var fld = Folder.add(txt);
            if (parent is Folder)
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

        public static Folder createFldIfNo(string txt, Folder parent, int childNum)
        {
            Folder fld;

            if (parent.treeNodeObject.child.Count <= childNum)
            {
                //print "create folder! %s" % txt
                fld = Folder.add(txt);
                fld.set_parent(parent);
                return fld;
            }
            else
            {
                var chld = parent.treeNodeObject.child[childNum];
                fld = NeoOCI.create_from_treenode(chld) as Folder;
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

        public void restrict_to_child(Folder fld, int numchilds)
        {
            if (fld.treeNodeObject.child.Count > numchilds)
            {
                var ar = fld.treeNodeObject.child;
                var ar2 = new List<NeoOCI>();
                foreach (var treeobj in ar)
                {
                    ar2.Add(NeoOCI.create_from_treenode(treeobj));
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

        public void Reset()
        {
            block = new SceneManager();
        }

        public void SaveToFile()
        {
            if (svname == "")
            {
                SaveToFile(defaultSaveName);
            }
        }

        public void SaveToFile(string filename)
        {
            var app_dir = Path.GetDirectoryName(Application.dataPath);

            if (!Directory.Exists(backup_folder_name))
            {
                Directory.CreateDirectory(backup_folder_name);
            }

            var file_path = Path.Combine(backup_folder_name, filename);
            var abs_file_path = Path.Combine(app_dir, file_path);
            var data = Utils.SerializeData(this.block.ExportScenes());
            File.WriteAllBytes(abs_file_path, data);
        }

        public void LoadFromFile(string filename)
        {
            var script_dir = Path.GetDirectoryName(Application.dataPath);
            var file_path = Path.Combine(backup_folder_name, filename);
            var abs_file_path = Path.Combine(script_dir, file_path);
            if (File.Exists(abs_file_path))
            {
                byte[] data = File.ReadAllBytes(abs_file_path);
                var scenes = Utils.DeserializeData<Scene[]>(data);
                block = new SceneManager(scenes);
            }
        }        

        public void loadSceneDataBackupTimer(object param)
        {
            loadSceneDataBackupTimer();
        }

        public void loadSceneDataBackupTimer()
        {
            LoadFromFile("_backuptimer");
        }

        public void loadSceneData()
        {
            loadSceneData(false, true);
        }

        public void loadSceneData(bool backup = false, bool setToFirst = true)
        {
            string filename;
            SceneFolders.LoadTrackedActorsAndProps();

            if (backup)
            {
                if (svname == "")
                {
                    filename = defaultBackupName;
                }
                else
                {
                    filename = svname + ".backup";
                }
            }
            else
            {
                if (svname == "")
                {
                    filename = defaultSaveName;
                }
                else
                {
                    filename = svname;
                }
            }

            // abs_file_path = os.Path.Combine(script_dir, file_path)
            // if os.File.Exists(abs_file_path):
            //     f = open(abs_file_path, "r")
            //     block_dict = Utils.DeserializeData(f.read(), object_hook=sceneDecoder)  # , indent = 4, separators = (","," : ")))
            //     f.close()
            LoadFromFile(filename);
            
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
                    cam.addata.enabled = state;
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
            loadCurrentScene();
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
            this.game.skin_set(this.game.skin_default);
        }

        //def _exportAddBlock(self,fld_acode,):
        public string get_next_speaker(string curSpeakAlias, bool next)
        {
            // next from unknown speaker
            var all_actors = game.AllActors;
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

        public double CalculateSaveDataSize(byte[] bytes)
        {
            return (double)bytes.Length / 1000;
        }

        public void deleteSaveData()
        {
            SetExtendedData(new PluginData() { data = null });
            block = new SceneManager();
            SceneFolders.LoadTrackedActorsAndProps();
            this.saveDataSize = 0;
        }

        // Set scene chars with state data from dictionary

        public void SetSceneState(Scene s)
        {
            if (isSysTracking)
            {
                VNEngine.System.import_status(s.sys);
            }
            foreach (var actid in s.actors.Keys)
            {
                ActorData char_status = s.actors[actid];
                try
                {
                    /* TODO
                    if (SceneConsole.Instance != null)
                    {
                        if (SceneConsole.Instance.skipClothesChanges)
                        {
                            char_status.Remove("acc_all");
                            char_status.Remove("cloth_all");
                            char_status.Remove("cloth_type");
                        }
                    }
                    */
                }
                catch (Exception)
                {
                }
                var actor = game.GetActor(actid);
                try
                {
                    actor?.import_status(char_status);
                }
                catch (Exception e)
                {
                    SceneConsole.Instance.game.GetLogger.LogError($"Error occurred when importing Actor with id {actid}" + e.ToString());
                    SceneConsole.Instance.game.GetLogger.LogMessage($"Error occurred when importing Actor with id {actid}");
                    SceneFolders.LoadTrackedActorsAndProps();
                }
            }
            string propid = "";
            try
                {
                    
                    foreach (var id in s.items.Keys)
                    {
                        propid = id;
                        Item i = game.GetProp(id) as Item;
                        if (i != null)
                        {
                            i.import_status(s.items[id]);
                        }
                    }

                    foreach (var id in s.lights.Keys)
                    {
                    propid = id;
                    VNActor.Light l = game.GetProp(id) as VNActor.Light;
                    if (l != null)
                        {
                            l.import_status(s.lights[id]);
                        }
                    }

                    foreach (var id in s.props.Keys)
                    {
                    propid = id;
                    Prop p = game.GetProp(id) as Prop;
                        if (p != null)
                        {
                            p.import_status(s.props[id]);
                        }
                    }
            }
                catch (Exception e)
                {
                    game.GetLogger.LogError($"Error occurred when importing Prop with id {propid}" + e.ToString());
                    SceneFolders.LoadTrackedActorsAndProps();
                    Instance.game.GetLogger.LogMessage($"Error occurred when importing Prop with id {propid}");
                }           
        }

        protected override void OnSceneSave()
        {
            var logger = game.GetLogger;
            var pluginData = new PluginData();
            if (block.Count > 0)
            {
                try
                {
                    byte[] sceneData = MessagePackSerializer.Serialize(block.ExportScenes(), MessagePack.Resolvers.ContractlessStandardResolver.Instance);
                    pluginData.data["scenes"] = sceneData;
                    SetExtendedData(pluginData);
                    var saveDataSizeKb = CalculateSaveDataSize(sceneData);
                    logger.LogMessage($"Saved {(saveDataSizeKb):N} Kb of scene state data.");
                    this.saveDataSize = saveDataSizeKb;
                }
                catch (Exception e)
                {
                    logger.LogError("Error occurred while saving scene data: " + e.ToString());
                    logger.LogMessage("Failed to save scene data, check debug log for more info.");
                }
            }
        }

        internal void LoadPluginData()
        {
            var pluginData = GetExtendedData();
            if (pluginData == null || pluginData?.data == null)
            {
                block = new SceneManager();
            }
            else
            {
                byte[] sceneData = pluginData.data["scenes"] as byte[];
                if (sceneData != null && sceneData.Length > 0)
                {
                    var logger = game.GetLogger;
                    try
                    {
                        var scenes = MessagePackSerializer.Deserialize<Scene[]>(sceneData, MessagePack.Resolvers.ContractlessStandardResolver.Instance);
                        block = new SceneManager(scenes);
                        var saveDataSizeKb = CalculateSaveDataSize(sceneData);
                        logger.LogMessage($"Loaded {(saveDataSizeKb):N} Kb of scene state data.");
                        this.saveDataSize = saveDataSizeKb;
                    }
                    catch (Exception e)
                    {
                        logger.LogError("Error occurred while loading scene data: " + e.ToString());
                        logger.LogMessage("Failed to load scene data, check debug log for more info.");
                    }
                }
            }
            SceneFolders.LoadTrackedActorsAndProps();
        }

        protected override void OnSceneLoad(SceneOperationKind operation, ReadOnlyDictionary<int, ObjectCtrlInfo> loadedItems)
        {
            LoadPluginData();
        }
    }
}
