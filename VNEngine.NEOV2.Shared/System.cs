using System;
using UnityEngine;

namespace VNEngine
{
    public static partial class System
    {
        public struct ColorCorrection : IEquatable<ColorCorrection>
        {
            public int no;
            public float blend;
            public int saturation;
            public int brightness;
            public int contrast;

            public bool Equals(ColorCorrection other)
            {
                var equal = true;
                equal &= no == other.no;
                equal &= blend - other.blend < 0.001;
                equal &= saturation == other.saturation;
                equal &= brightness == other.brightness;
                equal &= contrast == other.contrast;
                return equal;
            }
        }


        public class SystemData
        {
            public string bg_png;

            public BGM_s bgm;

            public CharLight_s char_light;

            public ColorCorrection colorCorrection;
            public string fm_png;
            public int map;

            public bool map_light;

            public bool map_opt;
            public Vector3 map_pos;
            public Vector3 map_rot;

            public Wav_s? wav;


            public SystemData()
            {
            }


            public SystemData(StudioController game)
            {
                // export a dict contains all system status
                //from Studio import Studio
                //studio = Studio.Instance

                bgm = game.BGM;

                if (game.WavFileName != "")
                    wav = game.WAV;
                else
                    wav = null;

                map = game.MapNumber;

                map_pos = game.MapPos;
                map_rot = game.MapRot;
                map_light = game.MapLight;
                map_opt = game.MapOption;

                bg_png = game.scene_get_bg_png_orig();

                fm_png = game.FrameFile;

                char_light = game.CharLight;

                colorCorrection = game.colorCorrection;
            }

            public void Remove(string key)
            {
                throw new NotImplementedException();
            }
        }
    }
}