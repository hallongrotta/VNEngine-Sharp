using MessagePack;
using Studio;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace VNActor
{
    // Koikatsu Actor
    public partial class Actor
        : NeoOCI, IVNObject<Actor>
    {

        [MessagePackObject(keyAsPropertyName: true)]
        public class ActorData : NEOActorData, IDataClass<Actor>
        {

            [Key("ShoesType")]
            public int shoesType;
            [Key("CoordinateType")]
            public int coordinateType;
            [Key("TearLevel")]
            public int tearLevel;

            public ActorData() : base()
            {

            }

            override public void Apply(Actor a)
            {
                if (visible)
                {
                    a.TearLevel = tearLevel;
                    a.CoordinateType = coordinateType;
                    a.ShoesType = shoesType;

                }
                base.Apply(a);
            }

            public ActorData(Actor a, ActorData prevStatus) : this(a)
            {
                if (Utils.FKDictionariesEqual(fk, prevStatus.fk))
                {
                    fk = null;
                }
                if (Utils.IKDictionariesEqual(ik, prevStatus.ik))
                {
                    ik = null;
                }
            }



            public ActorData(Actor a) : base(a)
            {
                if (visible)
                {
                    tearLevel = a.TearLevel;
                    coordinateType = a.CoordinateType;
                    shoesType = a.ShoesType;
                    eyeAngles = a.EyeAngles;


                }

                /* TODO implement KKPE

                // ext data, enable by ini setting
                if (is_ini_value_true("ExportChara_CurClothesCoord"))
                {
                    fs["ext_curclothcoord"] = a.get_curcloth_coordinate();
                }
                if (is_ini_value_true("ExportChara_CurClothesCoordNoAcc"))
                {
                    fs["ext_curclothcoordnoacc"] = a.get_curcloth_coordinate();
                }
                if (is_ini_value_true("ExportChara_BodyShapes"))
                {
                    fs["ext_bodyshapes"] = a.get_body_shapes_all();
                }
                if (is_ini_value_true("ExportChara_FaceShapes"))
                {
                    fs["ext_faceshapes"] = a.get_face_shapes_all();
                }
                if (is_ini_value_true("ExportChara_AnimeAuxParam"))
                {
                    fs["anim_optprm"] = a.get_anime_option_param();
                }
                // plugin data, enable by ini setting
                try
                {
                    if (extplugins.ExtPlugin.exists("KKPE"))
                    {
                        if (is_ini_value_true("ExportChara_KKPE"))
                        {
                            fs["pl_kkpedata"] = a.get_kkpedata();
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error during get kkpedata");
                }
                */
            }

            public void Remove(string key)
            {
                return;
            }
        }

        public float Breast
        {
            get
            {
                // get breast:
                return this.objctrl.oiCharInfo.charFile.custom.body.shapeValueBody[4];
            }
        }

        public ChaFileDefine.CoordinateType coordinate_type_int_to_enum(int type) // TODO
        {
            ChaFileDefine.CoordinateType coordinateType;
            if (type == 0)
            {
                coordinateType = ChaFileDefine.CoordinateType.School01;
            }
            else if (type == 1)
            {
                coordinateType = ChaFileDefine.CoordinateType.School02;
            }
            else if (type == 2)
            {
                coordinateType = ChaFileDefine.CoordinateType.Gym;
            }
            else if (type == 3)
            {
                coordinateType = ChaFileDefine.CoordinateType.Swim;
            }
            else if (type == 4)
            {
                coordinateType = ChaFileDefine.CoordinateType.Club;
            }
            else if (type == 5)
            {
                coordinateType = ChaFileDefine.CoordinateType.Plain;
            }
            else
            {
                coordinateType = ChaFileDefine.CoordinateType.Pajamas;
            }
            return coordinateType;
        }

        public int CoordinateType
        {
            set
            {
                // type: 0-School01, 1-School02, 2-Gym, 3-Swim, 4-Club, 5-Plain, 6-Pajamas
                if (CoordinateType != value)
                {
                    ChaFileDefine.CoordinateType coordinateType = this.coordinate_type_int_to_enum(value);
                    this.objctrl.charInfo.ChangeCoordinateTypeAndReload(coordinateType);
                }
            }
            get
            {
                // return coordinate type
                return this.objctrl.charInfo.fileStatus.coordinateType;
            }
        }

        public int ShoesType
        {
            set
            {
                // type: 0-indoor, 1-outdoor
                this.objctrl.SetShoesType(value);
            }
            get
            {
                // return shoes type
                return this.objctrl.charFileStatus.shoesType;
            }
        }

        public byte[] Juice
        {
            set
            {
                // juices: level on (face, FrontUp, BackUp, FrontDown, BackDown) when 0-none, 1-few, 2-lots
                this.objctrl.SetSiruFlags(ChaFileDefine.SiruParts.SiruKao, value[0]);
                this.objctrl.SetSiruFlags(ChaFileDefine.SiruParts.SiruFrontUp, value[1]);
                this.objctrl.SetSiruFlags(ChaFileDefine.SiruParts.SiruBackUp, value[2]);
                this.objctrl.SetSiruFlags(ChaFileDefine.SiruParts.SiruFrontDown, value[3]);
                this.objctrl.SetSiruFlags(ChaFileDefine.SiruParts.SiruBackDown, value[4]);
            }
            get
            {
                // return juice level of (face, FrontUp, BackUp, FrontDown, BackDown) in tuple
                var jInfo = new byte[5] {
                this.objctrl.GetSiruFlags(ChaFileDefine.SiruParts.SiruKao),
                this.objctrl.GetSiruFlags(ChaFileDefine.SiruParts.SiruFrontUp),
                this.objctrl.GetSiruFlags(ChaFileDefine.SiruParts.SiruBackUp),
                this.objctrl.GetSiruFlags(ChaFileDefine.SiruParts.SiruFrontDown),
                this.objctrl.GetSiruFlags(ChaFileDefine.SiruParts.SiruBackDown)};
                return jInfo;
            }
        }

        public bool BreastDBEnable
        {
            set
            {
                var breastL = CharInfo.getDynamicBoneBust(ChaInfo.DynamicBoneKind.BreastL);
                var breastR = CharInfo.getDynamicBoneBust(ChaInfo.DynamicBoneKind.BreastR);
                breastL.enabled = value;
                breastR.enabled = value;
            }
        }
     
        private void LoadEyeAngle(BinaryReader reader)
        {
            CharInfo.eyeLookCtrl.eyeLookScript.LoadAngle(reader, new Version(0, 0, 8)); //Don't ask me why the version has to be this, it just is.
        }


        /* TODO KKPE stuff

        public string get_kkpedata()
        {
            if (extplugins.ExtPlugin.exists("KKPE"))
            {
                return extplugins.KKPE().GetCharaSettingsText(this.objctrl);
            }
            return "";
        }

        public object set_kkpedata(object kkpedata)
        {
            //print "in kkpedata"
            if (kkpedata != "")
            {
                if (extplugins.ExtPlugin.exists("KKPE"))
                {
                    //print "import kkpedata"
                    extplugins.KKPE().SetCharaSettingsText(this.objctrl, kkpedata);
                    //print "import kkpedata2"
                }
            }
        }

        */

        public bool IsRotatableIK(string ikNodeName)
        {
            return ikNodeName.Contains("_hand_") || ikNodeName.Contains("_leg03_");
        }

        public byte[] ClothCoordinate
        {
            get
            {
                return this.objctrl.charInfo.nowCoordinate.SaveBytes();
            }
            set
            {
                try
                {
                    this.objctrl.charInfo.nowCoordinate.LoadBytes(value, ChaFileDefine.ChaFileCoordinateVersion);
                    this.objctrl.charInfo.AssignCoordinate(this.coordinate_type_int_to_enum(this.objctrl.charInfo.fileStatus.coordinateType));
                    this.objctrl.charInfo.Reload(false, true, true, true);
                }
                catch (Exception e)
                {
                    Console.WriteLine(String.Format("Exception in set_curcloth_coordinate, {0}", e.ToString()));
                }
            }
        }

        public void set_curcloth_coordinate_no_accessory(string str64)
        {
            var bytes = Utils.str64_to_bytearray(str64);
            try
            {
                var nowCoord = this.objctrl.charInfo.nowCoordinate;

                var array2 = MessagePackSerializer.Serialize(nowCoord.accessory);
                nowCoord.LoadBytes(bytes, ChaFileDefine.ChaFileCoordinateVersion);
                //self.objctrl.charInfo.Reload()
                //self.objctrl.charInfo.AssignCoordinate(ChaFileDefine.CoordinateType[self.objctrl.charInfo.fileStatus.coordinateType])
                nowCoord.accessory = MessagePackSerializer.Deserialize<ChaFileAccessory>(array2);
                this.objctrl.charInfo.AssignCoordinate(this.coordinate_type_int_to_enum(this.objctrl.charInfo.fileStatus.coordinateType));
                //self.objctrl.charInfo.AssignCoordinate(0)
                this.objctrl.charInfo.Reload(false, true, true, true);
            }
            catch (Exception e)
            {
                Console.WriteLine(String.Format("Exception in set_curcloth_coordinate_no_accessory, {0}", e.ToString()));
            }
        }

        public void import_status(ActorData a, ActorData prevStatus)
        {
            if (a.fk is null)
            {
                a.fk = prevStatus.fk;
            }
            if (a.ik is null)
            {
                a.ik = prevStatus.ik;
            }

            import_status(a);
        }


        public int h_partner(int hType = 0, int hPosition = 0)
        {
            // return tuple of valid partner for current actor
            // 0: male, 1: female, -1: both
            return -1;
        }

        public void h_with(
            Actor partner,
            int hType = 0,
            int hPosition = 0,
            int hStage = 0,
            List<Actor> extActors = null)
        {
            Actor factor;
            Actor mactor;
            List<int> validCategoryKey;
            List<int> validNoKey;
            // partner: another actor as sex partner
            // hType: 0-serve, 1-insert, 2-yuri
            // hPosition:
            // hStage:
            // extActors: always (), no multi h in koikatu now
            // sync with partner
            if (this.Position != partner.Position || this.Rotation != partner.Rotation || this.Scale != partner.Scale)
            {
                partner.move(pos: this.Position, rot: this.Rotation, scale: this.Scale);
            }
            if (this.AnimeSpeed != partner.AnimeSpeed)
            {
                partner.AnimeSpeed = this.AnimeSpeed;
            }
            if (this.AnimePattern != partner.AnimePattern)
            {
                partner.AnimePattern = this.AnimePattern;
            }
            if (this.AnimationForceLoop != partner.AnimationForceLoop)
            {
                partner.AnimationForceLoop = this.AnimationForceLoop;
            }
            // decide sex role
            if (this.Sex == 0)
            {
                mactor = this;
                factor = partner;
            }
            else
            {
                mactor = partner;
                factor = this;
            }
            // show son for male
            var mss = mactor.Son;
            if (!mss.visible && mactor.Sex == 0)
            {
                mactor.Son = new Son_s { visible = true, length = mss.length };
            }
            // load anime
            var info = Info.Instance;
            var gcDic = info.dicAGroupCategory;
            var aDic = info.dicAnimeLoadInfo;
            if (hType == 0)
            {
                // serve
                validCategoryKey = gcDic[3].dicCategory.Keys.ToList();
                if (!Enumerable.Range(0, validCategoryKey.Count).Contains(hPosition))
                {
                    Console.WriteLine(String.Format("invalid hPosition %d, must be 0~%d", hPosition, validCategoryKey.Count - 1));
                    return;
                }
                validNoKey = aDic[3][validCategoryKey[hPosition]].Keys.ToList();
                if (!Enumerable.Range(0, validNoKey.Count).Contains(hStage))
                {
                    Console.WriteLine(String.Format("invalid hStage %d, must be 0~%d", hStage, validNoKey.Count - 1));
                    return;
                }
                //print "a.animate(3, %d, %d)"%(validCategoryKey[hPosition], validNoKey[hStage])
                //print "b.animate(2, %d, %d)"%(validCategoryKey[hPosition], validNoKey[hStage])
                mactor.SetAnimate(3, validCategoryKey[hPosition], validNoKey[hStage]);
                factor.SetAnimate(2, validCategoryKey[hPosition], validNoKey[hStage]);
            }
            else if (hType == 1)
            {
                // insert
                validCategoryKey = gcDic[5].dicCategory.Keys.ToList();
                if (!Enumerable.Range(0, validCategoryKey.Count).Contains(hPosition))
                {
                    Console.WriteLine(String.Format("invalid hPosition %d, must be 0~%d", hPosition, validCategoryKey.Count - 1));
                    return;
                }
                validNoKey = aDic[5][validCategoryKey[hPosition]].Keys.ToList();
                if (!Enumerable.Range(0, validNoKey.Count).Contains(hStage))
                {
                    Console.WriteLine(String.Format("invalid hStage %d, must be 0~%d", hStage, validNoKey.Count - 1));
                    return;
                }
                //print "a.animate(5, %d, %d)"%(validCategoryKey[hPosition], validNoKey[hStage])
                //print "b.animate(4, %d, %d)"%(validCategoryKey[hPosition], validNoKey[hStage])
                mactor.SetAnimate(5, validCategoryKey[hPosition], validNoKey[hStage]);
                factor.SetAnimate(4, validCategoryKey[hPosition], validNoKey[hStage]);
            }
            else
            {
                // yuri
                validCategoryKey = new List<int> {
                        179,
                        181,
                        183
                    };
                if (!Enumerable.Range(0, validCategoryKey.Count).Contains(hPosition))
                {
                    Console.WriteLine(String.Format("invalid hPosition %d, must be 0~%d", hPosition, validCategoryKey.Count - 1));
                    return;
                }
                validNoKey = aDic[9][validCategoryKey[hPosition]].Keys.ToList();
                if (!Enumerable.Range(0, validNoKey.Count).Contains(hStage))
                {
                    Console.WriteLine(String.Format("invalid hStage %d, must be 0~%d", hStage, validNoKey.Count - 1));
                    return;
                }
                //print "a.animate(9, %d, %d)"%(validCategoryKey[hPosition], validNoKey[hStage])
                //print "b.animate(9, %d, %d)"%(validCategoryKey[hPosition]+1, validNoKey[hStage])
                mactor.SetAnimate(9, validCategoryKey[hPosition], validNoKey[hStage]);
                factor.SetAnimate(9, validCategoryKey[hPosition] + 1, validNoKey[hStage]);
            }
            // auto adjust anime param
            Console.WriteLine(String.Format("factor({0}): height={1} breast={2}", factor.text_name, factor.Height, factor.Breast));
            var anime_option_param = new AnimeOption_s { height = factor.Height, breast = factor.Breast };
            if (factor.IsHAnime)
            {
                factor.AnimationOption = anime_option_param;
            }
            if (mactor.IsHAnime)
            {
                mactor.AnimationOption = anime_option_param;
            }
            foreach (var extActor in extActors)
            {
                if (extActor != null && extActor.IsHAnime)
                {
                    extActor.AnimationOption = anime_option_param;
                }
            }
        }


        public static string[] get_hanime_group_names()
        {
            var info = Info.Instance;
            var gcDic = info.dicAGroupCategory;
            return new string[] { gcDic[3].name, gcDic[5].name, gcDic[9].name };
        }


        public static List<string> get_hanime_category_names(int group_index)
        {
            var info = Info.Instance;
            int[] groups = new int[] { 3, 5, 9 };
            int group = groups[group_index];
            var cDic = info.dicAGroupCategory[group].dicCategory;
            if (group == 9)
            {
                return new List<string>() { cDic[179].Remove(cDic[179].Length - 1), cDic[181].Remove(cDic[181].Length - 1), cDic[183].Remove(cDic[183].Length - 1) };
            }
            else
            {
                return cDic.Values.ToList();
            }
        }


        public static List<string> get_hanime_no_names(int group_index, int category_index)
        {
            var info = Info.Instance;
            var nDic = info.dicAnimeLoadInfo;
            int[] groups = new int[] { 3, 5, 9 };
            int group = groups[group_index];
            int category = info.dicAGroupCategory[group].dicCategory.Keys.ToList()[category_index];
            var nName = new List<string>();
            foreach (var n in nDic[group][category].Keys)
            {
                nName.Add(nDic[group][category][n].name);
            }
            return nName;
        }

        public static void char_cloth_type(Actor chara, ActorData param)
        {
            if (chara is Actor kkChara)
            {
                kkChara.CoordinateType = param.coordinateType;
            }
        }

        public int TearLevel
        {
            set
            {
                // level: tears level 0,1,2,3 or 0~1
                this.objctrl.SetTearsLv((byte)value);
            }
            get
            {
                // return tear levelS
                return this.objctrl.GetTearsLv();
            }
        }

        public static void char_tear(Actor chara, ActorData param)
        {
            // param = tear level(0,1,2,3) or (0~1 for PH)
            chara.TearLevel = param.tearLevel;
        }

        public static void char_shoes(Actor chara, ActorData param)
        {
            // param = 0 or 1
            chara.ShoesType = param.shoesType;
        }

        /* TODO check if used
        public static void char_ext_curclothcoordnoacc(Actor chara, ActorData param)
        {
            // param = voice repeat flag
            try
            {
                chara.set_curcloth_coordinate_no_accessory(param.accessoryStatus);
            }
            catch (Exception e)
            {
                Console.WriteLine("VNGE VNActor error: can't set curcloth coord", e);
            }
        }
        */

        /* TODO KKPE stuff

        public static void char_pl_kkpedata(Actor chara, int[] param)
        {
            // param = kkpe data
            try
            {
                chara.set_kkpedata(param);
            }
            catch (Exception e)
            {
                Console.WriteLine("VNGE VNActor error: can't set KKPE data", e);
            }
        }

        */

        public static void char_accessory(Actor chara, ActorData param)
        {
            chara.Accessories = param.accessoryStatus;
        }

        /* TODO add this back
        public static void char_all_accessories(Actor chara, ActorData param)
        {
            // param = 0(hide all)/1(show all)
            // or
            // param = (accShow0, accShow1, ... accShow19)
            chara.SetAccessory(param.showAllAccessories);
        }
        */


        public struct FunctionBoolPair
        {
            CharaActFunction func;
            bool active;


            public FunctionBoolPair(CharaActFunction func, bool active)
            {
                this.func = func;
                this.active = active;
            }
        }


        private static Dictionary<string, FunctionBoolPair> char_act_funcs = new Dictionary<string, FunctionBoolPair> {
        {
            "anim",
            new FunctionBoolPair(char_anime, false)},
        {
            "anim_spd",
            new FunctionBoolPair(char_anime_speed, true)},
        {
            "anim_ptn",
            new FunctionBoolPair(char_anime_pattern, true)},
        {
            "anim_lp",
            new FunctionBoolPair(char_anime_forceloop, false)},
        {
            "anim_optprm",
            new FunctionBoolPair(char_anime_optionparam, true)},
        {
            "anim_restart",
            new FunctionBoolPair(char_anime_restart, false)},
        {
            "load_cloth",
            new FunctionBoolPair(char_load_cloth, false)},
        /*{
                
            "cloth",
            new FunctionBoolPair(char_cloth, false)},*/
        {
            "cloth_all",
            new FunctionBoolPair(char_all_clothes, false)},
        {
            "cloth_type",
            new FunctionBoolPair(char_cloth_type, false)},
        {
            "acc_all",
            new FunctionBoolPair(char_accessory, false)},
        {
            "juice",
            new FunctionBoolPair(char_juice, false)},
        {
            "tear",
            new FunctionBoolPair(char_tear, false)},
        {
            "face_red",
            new FunctionBoolPair(char_face_red, true)},
        {
            "nip_stand",
            new FunctionBoolPair(char_nip_stand, true)},
        {
            "face_opt",
            new FunctionBoolPair(char_face_option, false)},
        {
            "son",
            new FunctionBoolPair(char_son, true)},
        {
            "simple",
            new FunctionBoolPair(char_simple, false)},
        {
            "simple_color",
            new FunctionBoolPair(char_simple_color, true)},
        {
            "look_at",
            new FunctionBoolPair(char_eyes_look, true)},
        {
            "look_at_ptn",
            new FunctionBoolPair(char_eyes_look_ptn, false)},
        {
            "look_at_pos",
            new FunctionBoolPair(char_eyes_look_pos, true)},
        {
            "face_to",
            new FunctionBoolPair(char_neck_look, false)},
        {
            "face_to_full",
            new FunctionBoolPair(char_neck_look_full, false)},
        {
            "face_to_full2",
            new FunctionBoolPair(char_neck_look_full2, false)},
        {
            "eyebrow",
            new FunctionBoolPair(char_eyebrow, false)},
        {
            "eyes",
            new FunctionBoolPair(char_eyes, false)},
        {
            "eyes_open",
            new FunctionBoolPair(char_eyes_open, true)},
        {
            "eyes_blink",
            new FunctionBoolPair(char_eyes_blink, false)},
        {
            "mouth",
            new FunctionBoolPair(char_mouth, false)},
        {
            "mouth_open",
            new FunctionBoolPair(char_mouth_open, true)},
        {
            "lip_sync",
            new FunctionBoolPair(char_lip_sync, false)},
        {
            "hands",
            new FunctionBoolPair(char_hands, false)},
        {
            "move",
            new FunctionBoolPair(char_move, false)},
        {
            "move_to",
            new FunctionBoolPair(char_move_to, true)},
        /* TODO
        {
            "turn",
            (char_turn, false)},
        {
            "turn_to",
            (char_turn_to, true)}, */
        {
            "rotate_to",
            new FunctionBoolPair(char_rotate_to, true)},
        {
            "scale_to",
            new FunctionBoolPair(char_scale_to, true)},
        {
            "kinematic",
            new FunctionBoolPair(char_kinematic, false)},
        {
            "fk_active",
            new FunctionBoolPair(char_fk_active, false)},
        {
            "fk_set",
            new FunctionBoolPair(char_fk_set, true)},
        {
            "ik_active",
            new FunctionBoolPair(char_ik_active, false)},
        {
            "ik_set",
            new FunctionBoolPair(char_ik_set, true)},
        {
            "voice_lst",
            new FunctionBoolPair(char_voice_lst, false)},
        {
            "voice_rpt",
            new FunctionBoolPair(char_voice_rpt, false)},
        {
            "visible",
            new FunctionBoolPair(char_visible, false)},
        {
            "shoes",
            new FunctionBoolPair(char_shoes, false)},
        /* TODO KKPE stuff
        {
            "ext_curclothcoord",
            (char_ext_curclothcoord, false)},
        {
            "ext_curclothcoordnoacc",
            (char_ext_curclothcoordnoacc, false)},
        {
            "ext_bodyshapes",
            (char_ext_bodyshapes, true)},
        {
            "ext_faceshapes",
            (char_ext_faceshapes, true)},
        {
            "pl_kkpedata",
            (char_pl_kkpedata, false)},
        */
        };
    }
}
