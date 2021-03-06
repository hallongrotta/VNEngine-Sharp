﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Studio;
using VNEngine;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Linq;
using System.Linq.Expressions;
using System.IO;
using VNActor;

namespace VNEngine
{
    partial class Utils
    {

        public static SceneConsole sceneConsole;

        public static Dictionary<object, object> _conv_dict = new Dictionary<object, object>
        {
        };

        public static int _step = 0;

        //import_or_reload("sceneutils")
        //import_or_reload("posesavestate")
        // :::::::::: JSON Derulo ::::::::::::
        // :::::::::: For debug only ::::::::::::
        // :type game:vngameengine.VNNeoController
        public static void start(VNNeoController game)
        {
            if (game.isClassicStudio)
            {
                game.show_blocking_message_time("This only for NEO-engines, sorry");
                return;
            }
            game.gdata.hook_update_allowed = true;
            if (sceneConsole == null)
            {
                sceneConsole = new SceneConsole(game);
            }
            UI.sceneConsoleGUIStart(game);
            game.scenef_register_actorsprops();
            // if no blocks - autoload
            if (sceneConsole.block.Count == 0)
            {
                if (HSNeoOCIFolder.find_single_startswith("-scenesavestate:") != null)
                {
                    sceneConsole.loadSceneData();
                    sceneConsole.show_blocking_message_time_sc("Scene data was auto-loaded!");
                }
            }
        }

        public static bool is_arr_equal(List<object> ar1, List<object> ar2)
        {
            if (ar1.Count != ar2.Count)
            {
                return false;
            }
            foreach (var i in Enumerable.Range(0, ar1.Count))
            {
                if (is_local_equal(ar1[i], ar2[i]))
                {
                }
                else
                {
                    return false;
                }
            }
            return true;
        }

        public static bool is_status_statuses_equal(Dictionary<string, IDataClass> oldstatus, Dictionary<string, IDataClass> status) {
            /*
        if (oldstatus.Count != status.Count) {
            return false;
        }
        var ofs = oldstatus;
        var dfs = new Dictionary<object, object> {
        };
        foreach (var key in status.Keys) {
            if (!ofs.ContainsKey(key) || !is_status_equal(ofs[key], status[key])) {
                //dfs[key] = status[key]
                return false;
            }
        }
        // return dfs
        // print "Optimized import status diff, ", dfs
        return true;
            */
            return false;
    }


        // main class
        // ::::: Console init and window :::::
        public static void toggle_scene_console(VNNeoController game)
        {
            if (sceneConsole == null)
            {
                sceneConsole = new SceneConsole(game);
            }
            if (sceneConsole.guiOnShow)
            {
                sceneConsoleGUIClose();
            }
            else
            {
                UI.sceneConsoleGUIStart(game);
            }
        }

        public static void resetConsole(VNNeoController game)
        {
            sceneConsoleGUIClose();
            sceneConsole = null;
            sceneConsole = new SceneConsole(game);
            UI.sceneConsoleGUIStart(game);
        }

        public static void loadConfig()
        {
            // Shortcuts
            sceneConsole.game.gdata.sc_shortcuts = new Dictionary<string, (Action, string)> {
            {
                "Load Next",
                (
                    sceneConsole.goto_next,
                    null
                )},
            {
                "Load Prev",
                (
                    sceneConsole.goto_prev,
                    null
                )},
            {
                "Load Next Scene",
                (
                    sceneConsole.goto_next_sc,
                    null
                )},
            {
                "Load Prev Scene",
                (
                    sceneConsole.goto_prev_sc,
                    null
                )},
            {
                "Load First Scene",
               (
                    sceneConsole.goto_first,
                    null
                )},
            {
                "Add Scene (Auto)",
                (
                    sceneConsole.addAutoWithMsg,
                    null
                )},
            {
                "Expand/collapse window",
                (
                    UI.minimizeWindow,
                    null
                )},
            {
                "Save Scenedata",
                (
                    sceneConsole.saveSceneData,
                    null
                )},
            {
                "Add Camera",
                (
                    sceneConsole.changeSceneCam,
                    null
                )}};
            sceneConsole.shortcuts = new Dictionary<string, (string,string)>
            {
            };
            /* TODO
            var config = ConfigParser.SafeConfigParser();
            config.read("scenesavestate.ini");
            foreach (var _tup_1 in config.items("Shortcuts"))
            {
                var command = _tup_1.Item1;
                var key = _tup_1.Item2;
                foreach (var com in sceneConsole.game.gdata.sc_shortcuts.Keys)
                {
                    if (command == com.ToLower())
                    {
                        sceneConsole.game.gdata.sc_shortcuts[com].key = VNEngine.Utils.ParseKeyCode(key);
                        sceneConsole.shortcuts[com] = key;
                        break;
                    }
                }
            } */
        }

        public static void saveConfig()
        {
            var cfpath = "scenesavestate.ini";
            var content = "[Shortcuts]\n";
            foreach (var _tup_1 in sceneConsole.shortcuts.Values)
            {
                var command = _tup_1.command;
                var key = _tup_1.key;
                content += command + " = " + key + "\n";
            }
            File.WriteAllText(cfpath, content);
            // reinit config
            sceneConsole.game.event_unreg_listener("update", hook_update);
            loadConfig();
            sceneConsole.game.event_reg_listener("update", hook_update);
        }

        public static void hook_update(VNController game)
        {
            if (!game.gdata.hook_update_allowed)
            {
                return;
            }
            var dt = Time.deltaTime;
            if (sceneConsole.game.visible)
            {
                // count only time when SSS is visible
                sceneConsole.backupTimeCur -= dt;
                //print sc.backupTimeCur
                if (sceneConsole.backupTimeCur <= 0)
                {
                    sceneConsole.backupTimeCur = sceneConsole.backupTimeDuration;
                    if (sceneConsole.block.Count > 0)
                    {
                        //print len(sc.block)
                        Console.WriteLine(String.Format("VNGE SSS: try backup by timer (every %s seconds)... (%s scenes)", sceneConsole.backupTimeDuration.ToString(), sceneConsole.block.Count.ToString()));
                        try
                        {
                            sceneConsole.saveToFileDirect("_backuptimer");
                            Console.WriteLine("VNGE SSS: made backup by timer!");
                        }
                        catch
                        {
                            Console.WriteLine("VNGE SSS: timer backup FAILED!... ");
                        }
                    }
                }
            }
            // param = sc.game.gdata.sc_shortcuts["loadnext"]
           /* foreach (var _tup_1 in sceneConsole.game.gdata.sc_shortcuts.Values) //TODO look into this
            {
                var commands = _tup_1.Item1;
                var param = _tup_1.Item2;
                (_, icode, ictrl, ialt, ishift) = param[1];
                if (Input.GetKeyDown(icode))
                {
                    // unity sucks for checking meta keys
                    var _tup_2 = unity_util.metakey_state();
                    var ctrl = _tup_2.Item1;
                    var alt = _tup_2.Item2;
                    var shift = _tup_2.Item3;
                    // print "detected key down:"
                    // print "tgt:", game.gdata.startShortcut
                    // print "cur: ctrl=%s, alt=%s, shift=%s"%(str(ctrl), str(alt), str(shift))
                    if (ctrl == ictrl && alt == ialt && shift == ishift)
                    {
                        // need to stop processing for about 0.5 seconds - avoid duplicate key proc
                        game.gdata.hook_update_allowed = false;
                        game.set_timer(0.3f, hook_upd_restore);
                        param[0]();
                        break;
                    }
                }
            } */
        }

        public static void hook_upd_restore(VNController game)
        {
            game.gdata.hook_update_allowed = true;
        }

        public static void sceneConsoleGUIClose(object param)
        {
            sceneConsoleGUIClose();
        }

        public static void sceneConsoleGUIClose()
        {
            // applying backup
            if (!(sceneConsole.svname == ""))
            {
                sceneConsole.saveToFile(backup: true);
            }
            else
            {
                //if sc.verify_load() != 0:
                //sc.saveSceneData(backup=True)
                sceneConsole.svname = "backup";
                sceneConsole.saveToFile(backup: true);
            }
            sceneConsole.game.event_unreg_listener("update", hook_update);
            sceneConsole.guiOnShow = false;
            sceneConsole.game.windowName = "";
            //sc.game.skin_set(sc.game_skin_saved)
            sceneConsole.game.skin_set(sceneConsole.game.gdata.sss_game_skin_saved);
            sceneConsole.game.gdata.sss_game_skin_saved = null;
            // sc.game.isShowDevConsole = False
            // sc.game.wwidth = sc.originalwindowwidth
            // sc.game.wheight = sc.originalwindowheight
            // sc.game.windowRect = Rect(Screen.width / 2 - sc.game.wwidth / 2, Screen.height - sc.game.wheight - 10,
            //                            sc.game.wwidth, sc.game.wheight)
            // sc.game.windowCallback = sc.originalWindowCallback
        }

        public static void setWindowName(int index)
        {
            var names = new Dictionary<int, string> {
            {
                0,
                "SceneSaveState"},
            {
                1,
                "Pose Library"},
            {
                2,
                "Scene Utils"}};
            if (names.ContainsKey(index))
            {
                sceneConsole.windowindex = index;
                sceneConsole.game.windowName = names[index];
            }
            else
            {
                Console.WriteLine("Invalid index:", index);
            }
        }

        public static string btntext_get_if_selected(string btntext, bool isSelected)
        {
            object col;
            if (isSelected)
            {
                col = sceneConsole.sel_font_col;
            }
            else
            {
                col = sceneConsole.nor_font_col;
            }
            return String.Format("<color=%s>%s</color>", col, btntext);
        }

        public static string btntext_get_if_selected2(string btntext, bool isSelected)
        {
            object col;
            if (isSelected)
            {
                col = "#f8e473";
            }
            else
            {
                col = sceneConsole.nor_font_col;
            }
            return String.Format("<color=%s>%s</color>", col, btntext);
        }

        public static void recalc_autostates()
        {
            var ar = HSNeoOCIFolder.find_all_startswith("-msauto:vis:");
            ar.Sort();
            sceneConsole.arAutoStatesItemsVis = ar;
            var ar2 = HSNeoOCIFolder.find_all_startswith("-msauto:choice:");
            ar2.Sort();
            sceneConsole.arAutoStatesItemsChoice = ar2;
        }

        public static string sort_by_textname(HSNeoOCI el)
        {
            return el.treeNodeObject.textName;
        }



        // ::::: Essential functions :::::
        public static HSNeoOCIFolder getFolder(VNNeoController game, string name, bool exact = false)
        {
            var flds = game.scene_get_all_folders();
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

        public static List<List<Actor>> getSelectedChars(VNNeoController game)
        {
            var mtreeman = game.studio.treeNodeCtrl;
            var ar = new List<Actor>();
            foreach (var node in mtreeman.selectNodes)
            {
                var ochar = HSNeoOCI.create_from_treenode(node);
                if (ochar.objctrl is OCIChar)
                {
                    HSNeoOCIChar chara = (HSNeoOCIChar)ochar;
                    ar.Add(chara.as_actor);
                }
            }
                var am = new List<Actor>();
                var af = new List<Actor>();
                foreach (var chara in ar)
                {
                    if (chara.sex == 0)
                    {
                        am.Add(chara);
                    }
                    else
                    {
                        af.Add(chara);
                    }
                }
                return new List<List<Actor>> {
                af,
                am
            };  
        }

        public static Actor getSelectedChar(VNNeoController game)
        {
            var mtreeman = game.studio.treeNodeCtrl;
            var ar = new List<Actor>();
            foreach (var node in mtreeman.selectNodes)
            {
                var ochar = HSNeoOCI.create_from_treenode(node);
                if (ochar.objctrl is OCIChar)
                {
                    HSNeoOCIChar chara = (HSNeoOCIChar)ochar;
                    ar.Add(chara.as_actor);
                }
            }
            return ar[0];
        }

        public static Prop getSelectedItem(VNNeoController game)
        {
            var mtreeman = game.studio.treeNodeCtrl;
            var ar = new List<Prop>();
            foreach (var node in mtreeman.selectNodes)
            {
                var oitem = HSNeoOCI.create_from_treenode(node);
                if (oitem.objctrl is OCIItem)
                {
                    HSNeoOCIProp prop = (HSNeoOCIProp)oitem;
                    ar.Add(prop.as_prop);
                }
            }
            if (ar.Count > 0)
            {
                return ar[0];
            }
            else
            {
                throw new Exception("No items selected");
            }
        }

        public static List<Prop> getSelectedItems(VNNeoController game)
        {
            var mtreeman = game.studio.treeNodeCtrl;
            var ar = new List<Prop>();
            foreach (var node in mtreeman.selectNodes)
            {
                var oitem = HSNeoOCI.create_from_treenode(node);
                if (oitem.objctrl is OCIItem)
                {
                    HSNeoOCIProp prop = (HSNeoOCIProp)oitem;
                    ar.Add(prop.as_prop);
                }
            }
            if (ar.Count > 0)
            {
                return ar;
            }
            else
            {
                throw new Exception("No items selected");
            }
        }

        // def getSelectedFolder(game, all=False):
        //     mtreeman = game.studio.treeNodeCtrl
        //     ar = []
        //     for node in mtreeman.selectNodes:
        //         ofld = HSNeoOCI.create_from_treenode(node)
        //         if isinstance(ofld.objctrl, OCIFolder):
        //             ar.Add(PropSC(ofld.objctrl))
        //     if len(ar) > 0:
        //         if all == False:
        //             return ar[0]
        //         else:
        //             return ar
        //     # else:
        //     # raise Exception("No folders selected")
        // chara functions
        public static void char_import_status_diff_optimized(Actor chara, Actor.ActorData status)
        {
            /* TODO
            var ofs = chara.export_full_status();
            var dfs = new Dictionary<string, object>
            {
            };
            foreach (var key in status.Keys)
            {
                if (!ofs.ContainsKey(key) || ofs[key] != status[key])
                {
                    dfs[key] = status[key];
                }
            }
            //return dfs
            //print "Optimized import status diff, ", dfs
            chara.import_status(dfs);
            */
            chara.import_status(status);
        }

        public static new IDataClass get_status_diff_optimized(IDataClass oldstatus, IDataClass status)
        {
            /* TODO
            var ofs = oldstatus;
            var dfs = new Dictionary<string, object>
            {
            };
            foreach (var key in status.Keys)
            {
                if (!ofs.ContainsKey(key) || ofs[key] != status[key])
                {
                    dfs[key] = status[key];
                }
            }
            //return dfs
            //print "Optimized import status diff, ", dfs
            */
            return status;
        }

        public static bool is_local_equal(object el1, object el2)
        {
            if (el1 == el2)
            {
                return true;
            }
            // if isinstance(el1, Vector3) or isinstance(el1, Vector2) or isinstance(el1, Color) or isinstance(
            //         el1, tuple):
            if (JsonSerializer.Serialize(el1) == JsonSerializer.Serialize(el2))
            {
                return true;
            }
            return false;
        }

        public static bool is_status_equal(IDataClass oldstatus, IDataClass status)
        {
            if (oldstatus != status)
            {
                return false;
            }
            /* TODO
            var ofs = oldstatus;
            var dfs = new Dictionary<string, object>
            {
            };
            foreach (var key in status.Keys)
            {
                if (!ofs.ContainsKey(key) || ofs[key] != status[key])
                {
                    //dfs[key] = status[key]
                    if (is_local_equal(ofs[key], status[key]))
                    {
                    }
                    else
                    {
                        Console.WriteLine("non-eq: ", ofs[key], status[key]);
                        return false;
                    }
                }
            }
            // return dfs
            // print "Optimized import status diff, ", dfs
            */
            return true;
        }

        public static bool is_status_statuses_equal(Dictionary<string, object> oldstatus, Dictionary<string, object> status)
        {

            return oldstatus == status;
        }

        public static bool is_status_equal_json(Dictionary<string, object> oldstatus, Dictionary<string, object> status)
        {
            if (oldstatus.Count != status.Count)
            {
                return false;
            }
            var str1 = JsonSerializer.Serialize(oldstatus);
            var str2 = JsonSerializer.Serialize(status);
            if (str1 != str2)
            {
                //print "neq json: ", str1, str2
                return false;
            }
            return true;
        }

        public static object is_arr_statuses_equal(IDataClass[] ar1, IDataClass[] ar2)
        {
            if (ar1.Length != ar2.Length)
            {
                return false;
            }
            foreach (var i in Enumerable.Range(0, ar1.Length))
            {
                if (is_status_equal(ar1[i], ar2[i]))
                {
                }
                else
                {
                    return false;
                }
            }
            return true;
        }

        public static void sys_import_status_diff_optimized(VNNeoController game, object status)
        { /* //TODO
            var ofs = GameSystem.export_sys_status(game);
            var dfs = new Dictionary<object, object>
            {
            };
            foreach (var key in status.Keys)
            {
                if (!ofs.ContainsKey(key) || ofs[key] != status[key])
                {
                    dfs[key] = status[key];
                }
            }
            
            VNFrame.act(game, new Dictionary<string, object> {
            {
                "sys",
                dfs}});
            VNFrame.act(game, new Dictionary<string, object> {
            {
                "sys",
                status}});
            */
        }

        public static HSNeoOCIFolder folder_add_child(HSNeoOCI parent, string childtext)
        {
            var fold = HSNeoOCIFolder.add(childtext);
            fold.set_parent(parent);
            return fold;
        }

        // scene utils UI
        public static void sceneUtilsUI()
        {
            var game = sceneConsole.game;
            var skin_def = sceneConsole.skinDefault;
            // run scene utils if needed
            if (sceneConsole.skinDefault_sideApp != "sceneutils")
            {
                sceneConsole.skinDefault_sideApp = "sceneutils";
                Utils.start(sceneConsole.game);
            }
            if (!game.isFuncLocked)
            {
                if (!game.isShowDevConsole)
                {
                    try
                    {
                        skin_def.render_main(game.curCharFull, game.vnText, game.vnButtons, game._vnButtonsActions, game.vnButtonsStyle);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Error in skin.render_main, ", e.ToString());
                    }
                }
                else
                {
                    // show dev console
                    try
                    {
                        skin_def.render_dev_console();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Error in skin.render_dev_console, ", e.ToString());
                    }
                }
            }
            else
            {
                // render system message
                skin_def.render_system(sceneConsole.funcLockedText);
            }
        }

        public static object merge_two_dicts<T>(Dictionary<string, T> x, Dictionary<string, T> y)
        {
            foreach (KeyValuePair<string, T> kv in x)
            {
                y[kv.Key] = kv.Value;
            }
            return y;
        }

        // tree node
        public static bool treenode_check_select(TreeNodeObject treenode)
        {
            VNNeoController game = sceneConsole.game;
            if (game.isCharaStudio)
            {
                return game.studio.treeNodeCtrl.CheckSelect(treenode);
            }
            else
            {
                return false;
            }
        }

        // util colors
        public static string color_text(string text, string color)
        {
            return string.Format("<color=#{1}ff>{0}</color>", text, color);
        }

        public static string color_text_green(string text)
        {
            return color_text(text, "aaffaa");
        }

        public static string color_text_red(string text)
        {
            return color_text(text, "ffaaaa");
        }

        public static object color_text_yellowlight(string text)
        {
            return color_text(text, "f8e473");
        }

        public static object color_text_blue(string text)
        {
            return color_text(text, "aaaaff");
        }

        public static Dictionary<string, string> _iniOptions = null;

        public static string get_ini_value(string elem)
        {
            // get ini value for cur engine
            if (_iniOptions != null)
            {
                // already parsed
            }
            else
            {
                // need to parse and cache
                _iniOptions = new Dictionary<string, string>
                {
                };
                // TODO:
                /*
                var config = ConfigParser.SafeConfigParser();
                config.read("scenesavestate_config.ini");
                foreach (var _tup_1 in config.items("Options"))
                {
                    var k = _tup_1.Item1;
                    var v = _tup_1.Item2;
                    _iniOptions[k.lower()] = v;
                } */
            }
            // main code
            //print _iniOptions
            var elemlower = elem.ToLower();
            if (_iniOptions.ContainsKey(elemlower))
            {
                return _iniOptions[elemlower];
            }
            return null;
        }

        public static int get_ini_value_def_int(string elem, int defint)
        {
            var val = get_ini_value(elem);
            if (val == null)
            {
                return defint;
            }
            else
            {
                var val2 = Convert.ToInt32(val);
                if (val2 == -1)
                {
                    return defint;
                }
                else
                {
                    return val2;
                }
            }
        }

        public static bool is_ini_value_true(string elem)
        {
            var val = get_ini_value(elem);
            if (val != null && val != "0")
            {
                return true;
            }
            return false;
        }

        // folders
        public static object add_folder_if_not_exists(string foldertxt, string folderfind, HSNeoOCI parentifcreate, bool overwrite = false)
        {
            var vnext = HSNeoOCIFolder.find_single_startswith(folderfind);
            if (vnext is HSNeoOCIFolder)
            {
                if (overwrite)
                {
                    vnext.name = foldertxt;
                }
                return vnext;
            }
            else
            {
                return folder_add_child(parentifcreate, foldertxt);
            }
        }

    }
}
