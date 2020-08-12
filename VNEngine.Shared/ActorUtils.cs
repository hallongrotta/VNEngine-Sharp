using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace VNEngine
{
    public class ActorUtils
    {

        /*
        public static object get_hanime_group_names(VNNeoController game)
        {
            if (game.isStudioNEO)
            {
                return Actor.hanime_group_names;
            }
            else if (game.isPlayHomeStudio)
            {
                return ActorPHStudio.get_hanime_group_names();
            }
            else if (game.isCharaStudio)
            {
                return ActorCharaStudio.get_hanime_group_names();
            }
            else if (game.isNEOV2)
            {
                return ActorNeoV2.get_hanime_group_names();
            }
            else
            {
                throw new Exception("Classic studio not supported!");
            }
        }

        public static object get_hanime_category_names(VNNeoController game, object group)
        {
            if (game.isStudioNEO)
            {
                return ActorHSNeo.get_hanime_category_names(group);
            }
            else if (game.isPlayHomeStudio)
            {
                return ActorPHStudio.get_hanime_category_names(group);
            }
            else if (game.isCharaStudio)
            {
                return ActorCharaStudio.get_hanime_category_names(group);
            }
            else if (game.isNEOV2)
            {
                return ActorNeoV2.get_hanime_category_names(group);
            }
            else
            {
                throw new Exception("Classic studio not supported!");
            }
        }

        public static object get_hanime_no_names(VNNeoController game, object group, object category)
        {
            if (game.isStudioNEO)
            {
                return ActorHSNeo.get_hanime_no_names(group, category);
            }
            else if (game.isPlayHomeStudio)
            {
                return ActorPHStudio.get_hanime_no_names(group, category);
            }
            else if (game.isCharaStudio)
            {
                return ActorCharaStudio.get_hanime_no_names(group, category);
            }
            else if (game.isNEOV2)
            {
                return ActorNeoV2.get_hanime_no_names(group, category);
            }
            else
            {
                throw new Exception("Classic studio not supported!");
            }
        }
        */

        // ----- support functions ----------
        public static List<byte> bytearray_to_list(byte[] ba)
        {
            return ba.ToList();
        }

        public static byte[] list_to_bytearray(List<byte> list)
        {
            //return Array[Byte]([Byte(i) for i in list])
            //return Array[Byte](list)
            //print bytearray(list)
            var memoryStream = new MemoryStream();
            var binaryWriter = new BinaryWriter(memoryStream);
            foreach (var i in list)
            {
                //binaryWriter.Write(bytes([i]))
                //binaryWriter.Write(str(i).encode())
                binaryWriter.Write(i);
            }
            //binaryWriter.Write(list[0])
            binaryWriter.Close();
            memoryStream.Close();
            return memoryStream.ToArray();
        }

        public static Dictionary<string, string> _iniOptions;

        public static Dictionary<string, string> _iniTranslation;

        public static Dictionary<string, string> _iniExportOptDesp;

        public static void load_ini_file(bool forceReload = false)
        {
            return;
        }

        /* TODO
        public static void load_ini_file(bool forceReload = false)
        {
            object v;
            string k;
            if (forceReload || _iniOptions == null || _iniTranslation == null || _iniExportOptDesp == null)
            {
                // load only needed
                _iniOptions = new Dictionary<string, string>
                {
                };
                _iniTranslation = new Dictionary<string, string>
                {
                };
                _iniExportOptDesp = new Dictionary<string, string>
                {
                };
                var config = ConfigParser.SafeConfigParser();
                config.read(os.path.splitext(@__file__)[0] + ".ini");
                // option for current engine
                var engineid = get_engine_id();
                foreach (var _tup_1 in config.items(engineid))
                {
                    k = _tup_1.Item1;
                    v = _tup_1.Item2;
                    _iniOptions[k.ToLower()] = v;
                }
                //print _iniOptions
                // translation strings for all engine
                foreach (var _tup_2 in config.items("Translation"))
                {
                    k = _tup_2.Item1;
                    v = _tup_2.Item2;
                    _iniTranslation[k.ToLower()] = v;
                }
                // description strings for all engine
                foreach (var _tup_3 in config.items("ExportOptionDescription"))
                {
                    k = _tup_3.Item1;
                    v = _tup_3.Item2;
                    _iniExportOptDesp[k.ToLower()] = v;
                }
            }
            else
            {
                // already parsed
            }
        }
        */

        public static string get_ini_value(string elem)
        {
            // get ini value for cur engine
            load_ini_file();
            // main code
            var elemlower = elem.ToLower();
            if (_iniOptions.ContainsKey(elemlower))
            {
                return _iniOptions[elemlower];
            }
            return null;
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

        public static void set_ini_value(string elem, int val)
        {
            bool value = val == 1 ? true : false;
            if (is_ini_value_true(elem) != value)
            {
                var elemlower = elem.ToLower();
                _iniOptions[elemlower] = value ? "1" : "0";
            }
        }

        public static List<string> get_ini_options()
        {
            load_ini_file();
            // return all keys
            return _iniOptions.Keys.ToList();
        }

        public static string get_ini_translation(string elem)
        {
            // get ini translation data
            load_ini_file();
            // main code
            var elemlower = elem.ToLower();
            if (_iniTranslation.ContainsKey(elemlower))
            {
                return _iniTranslation[elemlower];
            }
            return null;
        }

        public static string get_ini_exportOptionDesp(string elem)
        {
            // get ini export option description
            load_ini_file();
            // main code
            var elemlower = elem.ToLower();
            if (_iniExportOptDesp.ContainsKey(elemlower))
            {
                return _iniExportOptDesp[elemlower];
            }
            return null;
        }
    }
}
