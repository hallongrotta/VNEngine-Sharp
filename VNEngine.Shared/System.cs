using MessagePack;
using UnityEngine;

namespace VNEngine
{
    public static partial class System
    {
        [MessagePackObject]
        public struct Wav_s
        {
            [Key(0)] public string fileName;
            [Key(1)] public bool play;
            [Key(2)] public bool repeat;
        }

        [MessagePackObject]
        public struct BGM_s
        {
            [Key(0)] public int no;
            [Key(1)] public bool play;
        }

        [MessagePackObject]
        public struct CharLight_s
        {
            [Key(0)] public Color rgbDiffuse;
            [Key(2)] public float cameraLightIntensity;
            [Key(3)] public float rot_y;
            [Key(4)] public float rot_x;
            [Key(5)] public bool cameraLightShadow;
        }
    }
}