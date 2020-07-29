﻿using Studio;
using System.Collections.Generic;
using VNActor;

namespace VNEngine
{
    public class Ministates
    {

            public struct MiniState_s
        {
            public string name;
            public TreeNodeObject obj;
        }

            public static List<MiniState_s> ministates_get_list(VNController game)
            {
                var fld = HSNeoOCIFolder.find_single("-ministates:1.0");
                if (fld == null)
                {
                    return new List<MiniState_s>();
                }
                var ar = new List<MiniState_s>();
                foreach (var fldMiniState in fld.treeNodeObject.child)
                {
                    ar.Add(new MiniState_s { name = fldMiniState.textName, obj = fldMiniState });
                }
                return ar;
            }

            public static void ministates_run_elem(VNNeoController game, TreeNodeObject elem)
            {
                var state = ministates_get_elem(game, elem);
                ministates_run_savedstate(game, state);
            }

            public static Dictionary<string, TreeNodeObject> ministates_get_elem(VNNeoController game, TreeNodeObject elem)
            {
                var res = new Dictionary<string, TreeNodeObject>
                {
                };
                foreach (var elData in elem.child)
                {

                    var elDataObj = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, TreeNodeObject>>(elData.textName);

                    foreach (KeyValuePair<string, TreeNodeObject> kv in elDataObj)
                    {
                        res[kv.Key] = kv.Value;
                    }
                }
                return res;
            }

        public static void ministates_run_savedstate(VNNeoController game, Dictionary<string, TreeNodeObject> state)
        {
            return;
        }

        /* TODO
            public static void ministates_run_savedstate(VNNeoController game, Dictionary<string, TreeNodeObject> state)
            {
                var elDataObj = state;
                foreach (var key in elDataObj.Keys)
                {
                    var elId = Convert.ToInt32(key[4]);
                    var objctrl = game.studio.dicObjectCtrl[elId];
                    var actprop = HSNeoOCI.create_from(objctrl);
                    if (actprop is HSNeoOCIChar chara)
                    {
                        chara.as_actor.import_status_diff_optimized(elDataObj[key]);
                    }
                    else if (actprop is HSNeoOCIProp prop)
                    {
                        prop.as_prop.import_status_diff_optimized(elDataObj[key]);
                    }
                }
            }
        */

            public static TreeNodeObject ministates_get_elem_by_name(VNNeoController game, string name)
            {
                var list = ministates_get_list(game);
                foreach (var elemFull in list)
                {
                    if (elemFull.name == name)
                    {
                        return elemFull.obj;
                    }
                }
                return null;
            }

            public static void ministates_run_elem_by_name(VNNeoController game, string name)
            {
                var elem = ministates_get_elem_by_name(game, name);
                if (elem != null)
                {
                    ministates_run_elem(game, elem);
                }
            }

            public static string[] ministates_calc_prefix(string name)
            {
                var ar = name.Split('-');
                if (ar.Length == 1)
                {
                    return new string[] {
                "",
                name
            };
                }
                else
                {
                    return ar;
                }
            }
        }

    }
