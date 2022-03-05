using System;
using System.Collections.Generic;
using UnityEngine;
using static VNActor.Character;

namespace VNActor
{
    public interface IDataClass<T>
    {
        void Apply(T item);
    }

    internal class Utils
    {
        /*
        public static Color tuple4_2_color((float, float, float, float) param)
        {
            return new Color(param.Item1, param.Item2, param.Item3, param.Item4);
        }

        public static Color tuple4_2_color(Color param)
        {
            return param;
        }
        */

        public static string bytearray_to_str64(byte[] ba)
        {
            var res = Convert.ToBase64String(ba, 0, ba.Length);
            //print res
            return res;
        }

        public static byte[] str64_to_bytearray(string str)
        {
            return Convert.FromBase64String(str);
            // return Array[Byte]([Byte(i) for i in list])
            // return Array[Byte](list)
        }

        public static bool FKDictionariesEqual(Dictionary<int, Vector3> dict1, Dictionary<int, Vector3> dict2)
        {
            var equal = false;
            if (dict1.Count == dict2.Count) // Require equal count.
            {
                equal = true;
                foreach (var pair in dict1)
                    if (dict2.TryGetValue(pair.Key, out var value))
                    {
                        // Require value be equal.
                        if (value.Equals(pair.Value))
                        {
                            equal = false;
                            break;
                        }
                    }
                    else
                    {
                        // Require key be present.
                        equal = false;
                        break;
                    }
            }

            return equal;
        }

        public static bool IKDictionariesEqual(Dictionary<string, IK_node_s> dict1, Dictionary<string, IK_node_s> dict2)
        {
            var equal = false;
            if (dict1.Count == dict2.Count) // Require equal count.
            {
                equal = true;
                foreach (var pair in dict1)
                    if (dict2.TryGetValue(pair.Key, out var value))
                    {
                        if (value.Equals(pair.Value))
                        {
                            equal = false;
                            break;
                        }
                    }
                    else
                    {
                        equal = false;
                        break;
                    }
            }

            return equal;
        }
    }
}