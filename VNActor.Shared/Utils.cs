using System;
using UnityEngine;



namespace VNActor
{

    public interface IDataClass
    {
        void Remove(string key);

    }

    class Utils
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

    }
}