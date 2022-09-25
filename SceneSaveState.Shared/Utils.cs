using MessagePack;
using MessagePack.Resolvers;
using Studio;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using VNActor;
using VNEngine;

namespace SceneSaveState
{
    public static class Utils
    {

        //public static SceneConsole.Instance SceneConsole.Instance;

        public static Dictionary<object, object> _conv_dict = new Dictionary<object, object>
        {
        };

        public static int _step = 0;

        //import_or_reload("sceneutils")
        //import_or_reload("posesavestate")
        // :::::::::: JSON Derulo ::::::::::::
        // :::::::::: For debug only ::::::::::::
        // :type game:vngameengine.StudioController
        public static void start(VNController game)
        {
            if (SceneConsole.Instance.ChapterManager.Count == 0)
            {
                return;
            }
            UI.sceneConsoleGUIStart(game);
        }


        public static byte[] SerializeData<T>(T item)
        {
            try
            {
                return MessagePackSerializer.Serialize(item, StandardResolver.Instance);
            }
            catch (FormatterNotRegisteredException)
            {
                return MessagePackSerializer.Serialize(item, ContractlessStandardResolver.Instance);
            }
            catch (InvalidOperationException)
            {
                throw;
            }
        }


        public static T DeserializeData<T>(byte[] s)
        {
            try
            {
                return MessagePackSerializer.Deserialize<T>(s, StandardResolver.Instance);
            }
            catch (FormatterNotRegisteredException)
            {
                return MessagePackSerializer.Deserialize<T>(s, ContractlessStandardResolver.Instance);
            }
            catch (InvalidOperationException)
            {
                throw;
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

        public static bool is_status_statuses_equal<T>(Dictionary<string, IDataClass<T>> oldstatus, Dictionary<string, IDataClass<T>> status)
        {
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
        public static void toggle_scene_console(VNController game)
        {
            if (SceneConsole.Instance.guiOnShow)
            {
                sceneConsoleGUIClose();
            }
            else
            {
                UI.sceneConsoleGUIStart(game);
            }
        }

        //public static void loadConfig()
        //{
            // Shortcuts
            /* TODO
            SceneConsole.Instance.game.gdata.sc_shortcuts = new Dictionary<string, (Action, string)> {
            {
                "Load Next",
                (
                    SceneConsole.Instance.goto_next,
                    null
                )},
            {
                "Load Prev",
                (
                    SceneConsole.Instance.goto_prev,
                    null
                )},
            {
                "Load Next Scene",
                (
                    SceneConsole.Instance.goto_next_sc,
                    null
                )},
            {
                "Load Prev Scene",
                (
                    SceneConsole.Instance.goto_prev_sc,
                    null
                )},
            {
                "Load First Scene",
               (
                    SceneConsole.Instance.goto_first,
                    null
                )},
            {
                "Add Scene (Auto)",
                (
                    SceneConsole.Instance.addAutoWithMsg,
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
                    SceneConsole.Instance.saveSceneData,
                    null
                )},
            {
                "Add Camera",
                (
                    SceneConsole.Instance.changeSceneCam,
                    null
                )}};
            */
            /*
            SceneConsole.Instance.shortcuts = new Dictionary<string, (string,string)>
            {
            };
            */
            /* TODO
            var config = ConfigParser.SafeConfigParser();
            config.read("scenesavestate.ini");
            foreach (var _tup_1 in config.items("Shortcuts"))
            {
                var command = _tup_1.Item1;
                var key = _tup_1.Item2;
                foreach (var com in SceneConsole.Instance.game.gdata.sc_shortcuts.Keys)
                {
                    if (command == com.ToLower())
                    {
                        SceneConsole.Instance.game.gdata.sc_shortcuts[com].key = VNEngine.Utils.ParseKeyCode(key);
                        SceneConsole.Instance.shortcuts[com] = key;
                        break;
                    }
                }
            } */
        //}

        public static void sceneConsoleGUIClose(object param)
        {
            sceneConsoleGUIClose();
        }

        public static void sceneConsoleGUIClose()
        {
            // applying backup
            /*
            if (!(SceneConsole.Instance.svname == ""))
            {
                SceneConsole.Instance.saveToFile(backup: true);
            }
            else
            {
                //if sc.verify_load() != 0:
                //sc.saveSceneData(backup=True)
                SceneConsole.Instance.svname = "backup";
                SceneConsole.Instance.saveToFile(backup: true);
            }
            */
            //SceneConsole.Instance.game.event_unreg_listener("update", hook_update);
            SceneConsole.Instance.guiOnShow = false;
            SceneConsole.Instance.GameController.windowName = "";
            //sc.game.skin_set(sc.game_skin_saved)
            //SceneConsole.Instance.game.skin_set(SceneConsole.Instance.game.gdata.sss_game_skin_saved);
            //SceneConsole.Instance.game.gdata.sss_game_skin_saved = null;
            // sc.game.isShowDevConsole = False
            // sc.game.wwidth = sc.originalwindowwidth
            // sc.game.wheight = sc.originalwindowheight
            // sc.game.windowRect = Rect(Screen.width / 2 - sc.game.wwidth / 2, Screen.height - sc.game.wheight - 10,
            //                            sc.game.wwidth, sc.game.wheight)
            // sc.game.windowCallback = sc.originalWindowCallback
        }



        public static string btntext_get_if_selected(string btntext, bool isSelected)
        {
            var col = isSelected ? UI.SelectedTextColor : UI.NormalTextColor;
            return $"<color={col}>{btntext}</color>";
        }

        public static string sort_by_textname(NeoOCI el)
        {
            return el.treeNodeObject.textName;
        }

        // ::::: Essential functions :::::
        public static Folder getFolder(StudioController game, string name, bool exact = false)
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
        public static void char_import_status_diff_optimized(VNActor.Character chara, VNActor.Character.ActorData status)
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

        public static IDataClass<T> get_status_diff_optimized<T>(IDataClass<T> oldstatus, IDataClass<T> status)
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
            if (Utils.SerializeData(el1) == Utils.SerializeData(el2))
            {
                return true;
            }
            return false;
        }

        public static bool is_status_equal<T>(IDataClass<T> oldstatus, IDataClass<T> status)
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
            var str1 = Utils.SerializeData(oldstatus);
            var str2 = Utils.SerializeData(status);
            if (str1 != str2)
            {
                //print "neq json: ", str1, str2
                return false;
            }
            return true;
        }

        public static object is_arr_statuses_equal<T>(IDataClass<T>[] ar1, IDataClass<T>[] ar2)
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

        public static void sys_import_status_diff_optimized(StudioController game, object status)
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

        public static Folder folder_add_child(NeoOCI parent, string childtext)
        {
            var fold = Folder.add(childtext);
            fold.set_parent(parent);
            return fold;
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

        public static string color_text_yellowlight(string text)
        {
            return color_text(text, "f8e473");
        }

        public static string color_text_blue(string text)
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
        public static object add_folder_if_not_exists(string foldertxt, string folderfind, NeoOCI parentifcreate, bool overwrite = false)
        {
            var vnext = Folder.find_single_startswith(folderfind);
            if (vnext is Folder)
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
