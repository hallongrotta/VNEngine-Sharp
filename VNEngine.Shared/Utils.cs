using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using VNActor;
using System.Reflection;
using UnityEngine.SceneManagement;
using System.Linq;
using System.IO;

namespace VNEngine
{
    public static partial class Utils
    {

        public struct adv_properties
        {
            public string name;
            public bool isTime;
            public string time;
            public bool isTAnimCam;
            public bool isTHideUI;
            public bool isTTimerNext;
            public string tacStyle;
            public string tacZOut;
            public string tacRotX;
            public string tacRotZ;
            public bool keepcam;
        }

        public static int vnge_version;
        private static Dictionary<string, string> _engineoptions;
        private static Dictionary<string, KeyCode_s> _keycodes;

        public static string get_engine_id()
        {
            string dpath = Application.dataPath;
            string[] ar = dpath.Split('/');
            string gameId = ar[ar.Length - 1];
            if (gameId == "HoneyStudio_32_Data" || gameId == "HoneyStudio_64_Data")
            {
                return "studio";
            }
            if (gameId == "StudioNEO_32_Data" || gameId == "StudioNEO_64_Data")
            {
                return "neo";
            }
            if (gameId == "PlayHomeStudio32bit_Data" || gameId == "PlayHomeStudio64bit_Data")
            {
                return "phstudio";
            }
            if (gameId == "CharaStudio_Data")
            {
                return "charastudio";
            }
            if (gameId == "StudioNEOV2_Data")
            {
                return "neov2";
            }
            return "";
        }


        public static bool inited = false;

        public static void vngame_window_autogames_uni_1init()
        {
            if (!inited)
            {
                inited = true;
                vngame_window_autogames_uni();
            }
        }


        public struct KeyCode_s {
            string keycode;
            public KeyCode code;
            bool ctrl;
            bool alt;
            bool shift;
            private string s;

            public KeyCode_s(string s, KeyCode code, bool ctrl, bool alt, bool shift) : this()
            {
                this.s = s;
                this.code = code;
                this.ctrl = ctrl;
                this.alt = alt;
                this.shift = shift;
            }
        }


        public static KeyCode_s ParseKeyCode(string s)
        {
            try
            {

                bool ctrl = false;
                bool alt = false;
                bool shift = false;
                string[] keys_array = s.ToLower().Split('+');
                List<string> keys = new List<string>(keys_array);
                shift = keys.Contains("shift");
                ctrl = keys.Contains("ctrl") || keys.Contains("control");
                alt = keys.Contains("alt") || keys.Contains("meta");
                KeyCode code = (KeyCode)Enum.Parse(typeof(KeyCode), keys[-1], true);
                return new KeyCode_s(s, code, ctrl, alt, shift);
                
            }
            catch
            {
            }
            return new KeyCode_s(s, new KeyCode(), false, false, false);
        }

        public static void reloadKeyCodes()
        {
            _keycodes = new Dictionary<string, KeyCode_s>();
        }

        public static void parseIniFile()
        {
            //string v;
            //string k;

            if (_keycodes == null)
            {
                reloadKeyCodes();
            }

            if (_engineoptions == null)
            {
                _engineoptions = new Dictionary<string, string>();

            }


            try
            {
                /* TODO
                //var config = ConfigParser.SafeConfigParser();
                //config.read("vnengine_config.ini");
                foreach (var _tup_1 in config.items("Shortcuts"))
                {
                    k = _tup_1.Item1;
                    v = _tup_1.Item2;
                    _keycodes[k.ToLower()] = ParseKeyCode(v);
                }
                foreach (var _tup_2 in config.items("Options"))
                {
                    k = _tup_2.Item1;
                    v = _tup_2.Item2;
                    _engineoptions[k.ToLower()] = v;
                }
                */
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.ToString());
            }

            _keycodes["hide"] = new KeyCode_s("Ctrl+F8", KeyCode.F8, true, false, false);
            
        }

        public static Dictionary<string, KeyCode_s> getKeyCodes()
        {
            return _keycodes;
        }

        public static bool checkKeyCode(string iniparam)
        {
            try
            {
                iniparam = iniparam.ToLower();
                if (_keycodes.ContainsKey(iniparam))
                {
                    //(_, KeyCode icode, bool ictrl, bool ialt, bool ishift) = 
                    KeyCode_s keyCode = _keycodes[iniparam];
                    if (Input.GetKeyDown(keyCode.code))
                    {
                        // unity sucks for checking meta keys
                        /*var _tup_1 = unity_util.metakey_state(); TODO
                        var ctrl = _tup_1.Item1;
                        var alt = _tup_1.Item2;
                        var shift = _tup_1.Item3;
                        if (ctrl == ictrl && alt == ialt && shift == ishift)
                        {
                            return true;
                        }
                        */
                    }
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
            }
            return false;
        }

        public static Dictionary<string, string> getEngineOptions()
        {
            return _engineoptions;
        }

        //import GameCursor, CameraControl

        /*
        public static object vngame_window_studio(object vnButtonsStart, object vnButtonsActionsStart)
        {
            var game = vngameenginestudio.vngame_window_studio(vnButtonsStart, vnButtonsActionsStart);
            return game;
        }
        */
        
        //changed by chickenman
        /*
        public static object vngame_window_neo(object vnButtonsStart, object vnButtonsActionsStart)
        {
            //unity_util.clean_behaviors();
            //var game = unity_util.create_gui_behavior(NeoController);
            return new NeoController;
        }

        public static object vngame_window_phstudio(object vnButtonsStart, object vnButtonsActionsStart)
        {
            var game = vngameenginephstudio.vngame_window_phstudio(vnButtonsStart, vnButtonsActionsStart);
            return game;
        }
        */




        

        // ---------------- --- dumping item tree -----------------
        public static void dump_selected_item_tree()
        {
            HSNeoOCI item = HSNeoOCI.create_from_selected();
            using (StreamWriter file =
                new StreamWriter(@"dump_selected_items.txt"))
            {
                _dump_item_tree(file, item, 0);
            }
        }

        public static string item_dump(HSNeoOCIItem item)
        {
            var addparams = "";
            try
            {
                var tmp = String.Format(", 'anSp': {0}", item.objctrl.animeSpeed);
                addparams += tmp;
            }
            catch (Exception)
            {
            }
            string txt1 = String.Format("{" + "'no': {0}, 'prs': ({1},{2},{3})", item.no, item.pos, item.rot, item.scale);
            txt1 += addparams;
            return txt1;
        }

        public static string folder_dump(HSNeoOCIFolder item)
        {
            var value = item.name;
            if (!only_roman_chars(value))
            {
                value = "nonlatinname";
            }
            string txt1 = String.Format("{" + "'no': 'fold', 'name': '{2}', 'pr': ({0},{1})", item.pos, item.rot, value);
            return txt1;
        }

        public static void _dump_item_tree(StreamWriter f, HSNeoOCI item, int level)
        {
            var txt1 = "";
            if (item is HSNeoOCIItem)
            {
                txt1 = item_dump((HSNeoOCIItem)item);
            } else if (item is HSNeoOCIFolder)
            {
                txt1 = folder_dump((HSNeoOCIFolder)item);
            }

            if (item.treeNodeObject.childCount > 0)
            {
                txt1 += ", 'ch': [";
                _print_dump(f, txt1, level);
                // print all child
                foreach (var childt in item.treeNodeObject.child)
                {
                    var child = HSNeoOCI.create_from_treenode(childt);
                    _dump_item_tree(f, child, level + 1);
                }
                _print_dump(f, "]},", level);
            }
            else
            {
                _print_dump(f, txt1 + "}", level);
            }
        }

        public static void _print_dump(StreamWriter f, string txt, int level)
        {
            //print(" "*level*4+txt)
            f.Write(new String(' ', level * 4) + txt + "\n");
        }

        public struct pr_s
        {
            internal Vector3 pos;
            internal Vector3 rot;
        }

        public struct prs_s
        {
            internal Vector3 pos;
            internal Vector3 rot;
            internal Vector3 scale;
        }

        public struct ItemTreeItem
        {
            public pr_s pr;
            public prs_s prs;
            public object no;
            public string name;
            public int group;
            public int category;
            public float? anSp;
            public List<ItemTreeItem> ch;
        }

        public static HSNeoOCI load_item_tree(ItemTreeItem obj, Studio.TreeNodeObject itemparenttobj)
        {
            HSNeoOCI return_item;
            if (obj.no is string)
            {
                HSNeoOCIFolder folder = HSNeoOCIFolder.add(obj.name);
                if (itemparenttobj != null)
                {
                    folder.set_parent_treenodeobject(itemparenttobj);
                }
                folder.pos = obj.pr.pos;
                folder.rot = obj.pr.rot;
                return_item = folder;
            }
            else
            {
                HSNeoOCIItem item = HSNeoOCIItem.add_item(obj.group, obj.category, (int)obj.no);
                if (itemparenttobj != null)
                {
                    item.set_parent_treenodeobject(itemparenttobj);
                }
                item.pos = obj.prs.pos;
                item.rot = obj.prs.rot;
                item.scale = obj.prs.scale;
                if (obj.anSp != null)
                {
                    item.objctrl.animeSpeed = (float)obj.anSp;
                }
                return_item = item;
            }
            if (obj.ch != null)
            {
                foreach (var objch in obj.ch)
                {
                    load_item_tree(objch, return_item.treeNodeObject);
                }
            }
            return return_item;
        }
        public static string combine_path(params string[] paths)
        {
            if (paths.Length == 2)
                return Path.Combine(paths[0], paths[1]);
            else
            {
                return Path.Combine(paths[0], combine_path(paths.Skip(1).Take(paths.Length - 1).ToArray()));
            }
        }


        public static bool only_roman_chars(string s)
        {
            try
            {
                //s.Encode("ascii"); TODO look back at this
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static string to_roman(string s)
        {
            // potentially convert all symbols to latin
            // but now only return s
            // this is used in console, so we don't always convert it to latin
            return s;
        }

        public static string to_roman_file(string s)
        {
            // when we write to file, we want always be latin
            if (only_roman_chars(s))
            {
                return s;
            }
            return "nonlatin";
        }

        public static int random_choice(int[] ar)
        {
            try
            {
                int item = UnityEngine.Random.Range(0, ar.Length - 1) + 1;
                //print "random_choice: ",item
                int res = ar[item];
                return res;
            }
            catch (Exception e)
            {
                Console.WriteLine(String.Format("Error in random_choice, %s ", e.ToString()));
            }
            return ar[0];
        }

        public static GUISkin CloneSkin(GUISkin skin)
        {
            GUISkin newskin = new GUISkin();

     

            var props = newskin.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.SetProperty);

            foreach (var prop in props)
            {
                try
                {
                    string name = prop.Name;
                    var oldvalue = skin.GetType().GetProperty(name).GetValue(skin, null);
                    newskin.GetType().GetProperty(name).SetValue(newskin, oldvalue, null);
                }
                catch
                {
                }
            }
            return newskin;
        }


        public static void console_show(bool visible)
        {
            return;
        }
        /* TODO

        public static void console_show(bool visible)
        {
            // crt_win_hwid = ctypes.windll.user32.FindWindowA(0, 'Unity Console')
            var crt_win_hwid = ctypes.windll.kernel32.GetConsoleWindow();
            //game.show_blocking_message_time(str(crt_win_hwid))
            if (visible)
            {
                ctypes.windll.user32.ShowWindow(crt_win_hwid, 1);
            }
            else
            {
                ctypes.windll.user32.ShowWindow(crt_win_hwid, 0);
            }
        }
        */

        // -- studio_wait_for_load trick - for NEO and Old Studio--
        public static void studio_wait_for_load()
        {
            parseIniFile();
            var option = getEngineOptions();
            if (option.ContainsKey("hideconsoleafterstart"))
            {
                if (option["hideconsoleafterstart"] == "1")
                {
                    console_show(false);
                }
            }
            if (get_engine_id() == "")
            {
                Console.WriteLine("VN Game Engine not for this EXE file");
                return;
            }
            //print "SceneManager: ",SceneManager.GetActiveScene().name
            // we wait for 300 seconds before scene will set to Studio - and run after that
            foreach (var i in Enumerable.Range(0, 300 - 0))
            {
                //Sleep(1000);
                //print "!!"
                //print "SceneManager: ", i, SceneManager.GetActiveScene().name
                if (SceneManager.GetActiveScene().name.ToLower() == "studio")
                {
                    vngame_window_autogames_uni_1init();
                    return;
                }
            }
            Console.WriteLine("Studio loads more than 300 seconds... seems to be an error.");
            return;
            //yield WaitForSeconds(8)
            //print "SceneManager3: ", i,  SceneManager.GetActiveScene().name
            //import coroutine
            //coroutine.start_new_coroutine(neo_preload2, (), None)
            // -- studio_wait_for_load trick - for NEO and Old Studio--
            // def studio_wait_for_load2():
            //     if get_engine_id() == "":
            //         print "VN Game Engine not for this EXE file"
            //         return
            //     from UnityEngine.SceneManagement import SceneManager
            //     #print "SceneManager: ",SceneManager.GetActiveScene().name
            //     from System.Threading import Thread
            //
            //     # we wait for 300 seconds before scene will set to Studio - and run after that
            //     for i in range(0,300):
            //         Thread.Sleep(1000)
            //
            //
            //         #print "!!"
            //         #print "SceneManager: ", i, SceneManager.GetActiveScene().name
            //         if SceneManager.GetActiveScene().name.lower() == "studio":
            //             vngame_window_autogames_uni_1init()
            //             return
            //
            //     print "Studio loads more than 300 seconds... seems to be an error."
            //     return
            //     #yield WaitForSeconds(8)
            //     #print "SceneManager3: ", i,  SceneManager.GetActiveScene().name
            //     #import coroutine
            //     #coroutine.start_new_coroutine(neo_preload2, (), None)
        }
    }
}
