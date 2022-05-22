using MessagePack;
using Studio;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using VNActor;

namespace VNEngine
{
    public static partial class System
    {

        public struct Ace : IEquatable<Ace>
        {
            public float blend;
            public int no;

            public bool Equals(Ace other)
            {
                bool equal = true;
                equal &= no == other.no;
                equal &= (blend - other.blend) < 0.001;
                return equal;
            }
        }


        [MessagePackObject(keyAsPropertyName: true)]
        public class SystemData
        {
            public BGM_s bgm;

            public Wav_s? wav;
            public int map;
            public string MapFilename;
            public Vector3 map_pos;
            public Vector3 map_rot;
            public int sun;
            public bool map_opt;

            public string bg_png;
            public string fm_png;

            public CharLight_s char_light;

            public Ace ace;
            public void Remove(string key)
            {
                throw new NotImplementedException();
            }

            public SystemData()
            {

            }

            public string GetMapFilename(int mapNumber)
            {
                return mapNumber == -1 ? null : Singleton<Info>.Instance.dicMapLoadInfo[mapNumber].fileName;
            }

            public SystemData(StudioController game)
            {
                // export a dict contains all system status
                //from Studio import Studio
                //studio = Studio.Instance

                bgm = game.BGM;

                if (game.WavFileName != "")
                {
                    wav = game.WAV;
                }
                else
                {
                    wav = null;
                }

                map = game.MapNumber;
                MapFilename = GetMapFilename(map);
                map_pos = game.MapPos;
                map_rot = game.MapRot;

                sun = game.Sun;

                bg_png = game.scene_get_bg_png_orig(); 

                fm_png = game.FrameFile;

                char_light = game.CharLight;

                ace = game.Ace;
            }
        }
    }
}
