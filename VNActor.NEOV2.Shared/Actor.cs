using AIChara;
using Studio;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace VNActor
{

    // AI Actor
    public partial class Actor : IVNObject<Actor>
    {

        public class ActorData : NEOActorData, IDataClass<Actor>
        {
            public float tearLevel;
            public byte[] coordinate;
            public float tuya;
            public float wetness;

            public string aipedata;
            public Dictionary<string, Dictionary<string, float>> aipeblendshapes;

            public ActorData() : base() { }

            public ActorData(Actor a) : base(a)
            {
                if (visible)
                {
                    tuya = a.SkinGloss;
                    wetness = a.SkinWetness;
                    tearLevel = a.TearLevel;
                    coordinate = a.ClothCoordinate;

                    try
                    {
                        //aipedata = a.aipedata;
                        aipeblendshapes = a.xxpeblendshapes;
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("Error during get aipedata");
                    }

                }

                /*
                // ext data, enable by ini setting
                if (Utils.is_ini_value_true("ExportChara_CurClothesCoord"))
                {
                    fs["ext_curclothcoord"] = a.curcloth_coordinate;
                }
                if (Utils.is_ini_value_true("ExportChara_BodyShapes"))
                {
                    fs["ext_bodyshapes"] = a.body_shapes_all;
                }
                if (Utils.is_ini_value_true("ExportChara_FaceShapes"))
                {
                    fs["ext_faceshapes"] = a.face_shapes_all;
                }
                if (Utils.is_ini_value_true("ExportChara_AnimeAuxParam"))
                {
                    fs["anim_optprm"] = a.anime_option_param;
                }
                // plugin data, enable by ini setting
                if (Utils.is_ini_value_true("ExportChara_AIPE"))
                {
                    try
                    {
                        fs["pl_aipedata"] = a.aipedata;
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("Error during get aipedata");
                    }
                }
                return fs;
                */
            }

            override public void Apply(Actor a)
            {
                base.Apply(a);
                if (visible)
                {
                    a.SkinGloss = tuya;
                    a.SkinWetness = wetness;
                    a.TearLevel = tearLevel;
                    a.ClothCoordinate = coordinate;

                    try
                    {
                        //a.aipedata = aipedata;
                        a.xxpeblendshapes = aipeblendshapes;
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("Error during set aipedata");
                    }

                }            
            }
        }

        public float Breast
        {
            get
            {
                // get breast:
                return this.objctrl.oiCharInfo.charFile.custom.body.shapeValueBody[1];
            }
        }

        
        
        private void LoadEyeAngle(BinaryReader reader)
        {
            CharInfo.eyeLookCtrl.eyeLookScript.LoadAngle(reader);
        }

        public void set_accessory(int accIndex, bool accShow)
        {
            this.objctrl.ShowAccessory(accIndex, accShow);
        }

        public void set_accessory(bool accShow)
        {
            // param format 1: set one accessory
            // accIndex: 0~19
            // accShow: 0(hide)/1(visible)
            // param format 2: set all accessory, like the return value of get_accessory()
            // accIndex: 0/1 for each acessories in tuple(20)
            // accShow: must be None
            // param format 3: hide/show all accessory
            // accIndex: 0/1 for all
            // accShow: must be None
            for (int i = 0; i < 20; i++)
            {
                this.objctrl.ShowAccessory(i, accShow);
            }
        }

        /// <summary>
        /// Yoghurt splash effects on character.
        /// </summary>
        public byte[] Juice
        {
            set
            {
                // juices: level on (face, FrontUp, BackUp, FrontDown, BackDown) when 0-none, 1-few, 2-lots
                this.objctrl.SetSiruFlags(ChaFileDefine.SiruParts.SiruKao, value[0]);
                this.objctrl.SetSiruFlags(ChaFileDefine.SiruParts.SiruFrontTop, value[1]);
                this.objctrl.SetSiruFlags(ChaFileDefine.SiruParts.SiruBackTop, value[2]);
                this.objctrl.SetSiruFlags(ChaFileDefine.SiruParts.SiruFrontBot, value[3]);
                this.objctrl.SetSiruFlags(ChaFileDefine.SiruParts.SiruBackBot, value[4]);
            }
            get
            {
                // return juice level of (face, FrontUp, BackUp, FrontDown, BackDown) in tuple
                var jInfo = new byte[5]
                {
                this.objctrl.GetSiruFlags(ChaFileDefine.SiruParts.SiruKao),
                this.objctrl.GetSiruFlags(ChaFileDefine.SiruParts.SiruFrontTop),
                this.objctrl.GetSiruFlags(ChaFileDefine.SiruParts.SiruBackTop),
                this.objctrl.GetSiruFlags(ChaFileDefine.SiruParts.SiruFrontBot),
                this.objctrl.GetSiruFlags(ChaFileDefine.SiruParts.SiruBackBot)
            };

                return jInfo;
            }
        }

        /// <summary>
        /// Amount of tears in characters eyes. Set in a range of [0, 1].
        /// </summary>
        public float TearLevel
        {
            get
            {
                // return tear level
                return this.objctrl.GetTears();
            }
            set
            {
                // level: 0~1
                this.objctrl.SetTears(value);
            }
        }

        /// <summary>
        /// Skin glossiness effect. Set in a range of [0, 1].
        /// </summary>
        public float SkinGloss
        {
            set
            {
                // level: tuya 0~1
                this.objctrl.SetTuyaRate(value);
            }
            get
            {
                // return tuya rate
                return this.objctrl.oiCharInfo.SkinTuyaRate;
            }
        }

        /// <summary>
        /// Skin wetness effect. Set in a range of [0, 1].
        /// </summary>
        public float SkinWetness
        {
            get
            {
                // return wet rate
                return this.objctrl.oiCharInfo.WetRate;
            }
            set
            {
                // level: wet 0~1
                this.objctrl.SetWetRate(value);
            }
        }

        public bool IsRotatableIK(string ikNodeName)
        {
            return ikNodeName.Contains("_Hand_") || ikNodeName.Contains("_Foot01_");
        }

        /// <summary>
        /// Full clothing data for character. Can be used to change outfits. 
        /// Will only reload character if clothing is different than what is currently being worn. 
        /// </summary>
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
                    if (!value.SequenceEqual(ClothCoordinate))
                    {
                        this.objctrl.charInfo.nowCoordinate.LoadBytes(value, ChaFileDefine.ChaFileCoordinateVersion);
                        this.objctrl.charInfo.Reload(false, true, true);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(String.Format("Exception in set_curcloth_coordinate, {0}", e.ToString()));
                }
            }
        }

        /// <summary>
        /// Enable or disable dynamic bones in character breasts.
        /// </summary>
        public bool BreastDBEnable
        {
            set
            {
                var breastL = CharInfo.GetDynamicBoneBustAndHip(ChaControlDefine.DynamicBoneKind.BreastL);
                var breastR = CharInfo.GetDynamicBoneBustAndHip(ChaControlDefine.DynamicBoneKind.BreastR);
                breastL.enabled = value;
                breastR.enabled = value;
            }          
        }

        /*
        public float get_body_shape(int p1)
        {
            return this.charInfo.GetShapeBodyValue(p1);
        }

        public void set_body_shape(int p1, float p2)
        {
            this.charInfo.SetShapeBodyValue(p1, p2);
        }

        public float[] get_body_shapes_all()
        {
            return this.objctrl.oiCharInfo.charFile.custom.body.shapeValueBody;
        }

        public object get_body_shape_names()
        {
            return ChaFileDefine.cf_bodyshapename;
        }

        public int get_body_shapes_count()
        {
            return this.get_body_shapes_all().Length;
        }
                */

        

        public string aipedata
        {
            get
            {
                return AIPEUtils.GetCharaSettingsText(this.objctrl);
            }
            set
            {
                if (aipedata != "")
                {

                    AIPEUtils.SetCharaSettingsText(this.objctrl, aipedata);

                }
            }
        }

       public Dictionary<string, Dictionary<string, float>> xxpeblendshapes
        {
            get
            {
                return AIPEUtils.GetBlendShapesObj(this.objctrl);
            }
            set
            {
                AIPEUtils.SetBlendShapesObj(this.objctrl, value);
            }
        }              
      
        public static void char_tuya(Actor chara, ActorData param)
        {
            // param = skin tuya 0~1
            chara.SkinGloss = param.tuya;
        }

        public static void char_wet(Actor chara, ActorData param)
        {
            // param = skin wet 0~1
            chara.SkinWetness = param.wetness;
        }

        public object h_partner(int hType = 0, int hPosition = 0)
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
            // hType: 0-touth, 1-serve, 2-insert, 3-special, 4-yuri
            // hPosition:
            // hStage:
            // extActors: 
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
                // touch
                validCategoryKey = gcDic[1].dicCategory.Keys.ToList();
                if (!Enumerable.Range(0, validCategoryKey.Count).Contains(hPosition))
                {
                    Console.WriteLine(String.Format("invalid hPosition %d, must be 0~%d", hPosition, validCategoryKey.Count - 1));
                    return;
                }
                validNoKey = aDic[1][validCategoryKey[hPosition]].Keys.ToList();
                if (!Enumerable.Range(0, validNoKey.Count).Contains(hStage))
                {
                    Console.WriteLine(String.Format("invalid hStage %d, must be 0~%d", hStage, validNoKey.Count - 1));
                    return;
                }
                Console.WriteLine(String.Format("%s anime(%d, %d, %d)", mactor.text_name, 2, validCategoryKey[hPosition], validNoKey[hStage]));
                Console.WriteLine(String.Format("%s anime(%d, %d, %d)", factor.text_name, 1, validCategoryKey[hPosition], validNoKey[hStage]));
                mactor.SetAnimate(2, validCategoryKey[hPosition], validNoKey[hStage]);
                factor.SetAnimate(1, validCategoryKey[hPosition], validNoKey[hStage]);
            }
            else if (hType == 1)
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
                mactor.SetAnimate(4, validCategoryKey[hPosition], validNoKey[hStage]);
                factor.SetAnimate(3, validCategoryKey[hPosition], validNoKey[hStage]);
            }
            else if (hType == 2)
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
                mactor.SetAnimate(6, validCategoryKey[hPosition], validNoKey[hStage]);
                factor.SetAnimate(5, validCategoryKey[hPosition], validNoKey[hStage]);
            }
            else if (hType == 3)
            {
                // insert
                validCategoryKey = gcDic[7].dicCategory.Keys.ToList();
                if (!Enumerable.Range(0, validCategoryKey.Count).Contains(hPosition))
                {
                    Console.WriteLine(String.Format("invalid hPosition %d, must be 0~%d", hPosition, validCategoryKey.Count - 1));
                    return;
                }
                validNoKey = aDic[7][validCategoryKey[hPosition]].Keys.ToList();
                if (!Enumerable.Range(0, validNoKey.Count).Contains(hStage))
                {
                    Console.WriteLine(String.Format("invalid hStage %d, must be 0~%d", hStage, validNoKey.Count - 1));
                    return;
                }
                mactor.SetAnimate(8, validCategoryKey[hPosition], validNoKey[hStage]);
                factor.SetAnimate(7, validCategoryKey[hPosition], validNoKey[hStage]);
            }
            else
            {
                // yuri
                validCategoryKey = new List<int> {
                        215,
                        217,
                        219,
                        221,
                        223
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
                mactor.SetAnimate(9, validCategoryKey[hPosition], validNoKey[hStage]);
                factor.SetAnimate(9, validCategoryKey[hPosition] + 1, validNoKey[hStage]);
            }
            // auto adjust anime param
            Console.WriteLine(String.Format("factor(%s): height=%.2f breast=%.2f", factor.text_name, factor.Height, factor.Breast));
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
            return new string[] { gcDic[1].name, gcDic[3].name, gcDic[5].name, gcDic[7].name, gcDic[9].name };
        }


        public static List<string> get_hanime_category_names(int group)
        {
            var info = Info.Instance;
            var groups = new int[] { 1, 3, 5, 7, 9 };
            group = groups[group];
            var cDic = info.dicAGroupCategory[group].dicCategory;
            if (group == 9)
            {
                return new List<string>() { cDic[215].name, cDic[217].name, cDic[219].name, cDic[221].name, cDic[223].name };
            }
            else
            {
                return (from v in cDic.Values select v.name).ToList();
            }
        }

        public static List<string> get_hanime_no_names(int group, int category)
        {
            var info = Info.Instance;
            var nDic = info.dicAnimeLoadInfo;
            var groups = new int[] { 1, 3, 5, 7, 9 };
            group = groups[group];
            var cat = info.dicAGroupCategory[group].dicCategory.Keys.ToList()[category];
            var nName = new List<string>();
            foreach (var n in nDic[group][cat].Keys)
            {
                nName.Add(nDic[group][cat][n].name);
            }
            return nName;
        }

        public static void char_tear(Actor chara, ActorData param)
        {
            // param = tear level(0,1,2,3) or (0~1 for PH)
            chara.TearLevel = param.tearLevel;
        }

       

        /* TODO
        public static void char_accessory(Actor chara, ActorData param)
        {
            // param = (accIndex, accShow)
            if (chara is Actor aiChara)
            {
                aiChara.set_accessory(param.accessory.accIndex, param.accessory.accShow);
            }
        }

        public static void char_accessory(Actor chara, ActorData param)
        {
            // param = (accIndex, accShow)
            chara.set_accessory(param.accessory.accIndex, param.accessory.accShow);
        }
        */

        /*
        public static void char_all_accessories(Actor chara, ActorData param)
        {
            // param = 0(hide all)/1(show all)
            // or
            // param = (accShow0, accShow1, ... accShow19)
            chara.set_accessory(param.showAllAccessories);
        }
        */

        /*
        public static void char_neck_look_full2(Actor chara, ActorData param)
        {
            // param = array of bytes, use dump to get it
            try
            {
                chara.look_neck_full2 = param.neck;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in setting char neck in Fixed state (2): " + e.ToString());
                Console.WriteLine("Sorry, we just pass it...");
            }
        }
        */

        /*

        private static Dictionary<string, (CharaActFunction, bool)> char_act_funcs = new Dictionary<string, (CharaActFunction, bool)> {
        {
            "anim",
            (char_anime, false)},
        {
            "anim_spd",
            (char_anime_speed, true)},
        {
            "anim_ptn",
            (char_anime_pattern, true)},
        {
            "anim_lp",
            (char_anime_forceloop, false)},
        {
            "anim_optprm",
            (char_anime_optionparam, true)},
        {
            "anim_restart",
            (char_anime_restart, false)},
        {
            "load_cloth",
            (char_load_cloth, false)},
        {
            "cloth",
            (char_cloth, false)},
        {
            "cloth_all",
            (char_all_clothes, false)},
        {
            "acc",
            (char_accessory, false)},
        {
            "acc_all",
            (char_all_accessories, false)},
        {
            "juice",
            (char_juice, false)},
        {
            "tear",
            (char_tear, false)},
        {
            "face_red",
            (char_face_red, true)},
        {
            "nip_stand",
            (char_nip_stand, true)},
        {
            "skin_tuya",
            (char_tuya, true)},
        {
            "skin_wet",
            (char_wet, true)},
        {
            "face_opt",
            (char_face_option, false)},
        {
            "son",
            (char_son, true)},
        {
            "simple",
            (char_simple, false)},
        {
            "simple_color",
            (char_simple_color, true)},
        {
            "look_at",
            (char_eyes_look, true)},
        {
            "look_at_ptn",
            (char_eyes_look_ptn, false)},
        {
            "look_at_pos",
            (char_eyes_look_pos, true)},
        {
            "face_to",
            (char_neck_look, false)},
        {
            "face_to_full",
            (char_neck_look_full, false)},
        {
            "face_to_full2",
            (char_neck_look_full2, false)},
        {
            "eyebrow",
            (char_eyebrow, false)},
        {
            "eyes",
            (char_eyes, false)},
        {
            "eyes_open",
            (char_eyes_open, true)},
        {
            "eyes_blink",
            (char_eyes_blink, false)},
        {
            "mouth",
            (char_mouth, false)},
        {
            "mouth_open",
            (char_mouth_open, true)},
        {
            "lip_sync",
            (char_lip_sync, false)},
        {
            "hands",
            (char_hands, false)},
        {
            "move",
            (char_move, false)},
        {
            "move_to",
            (char_move_to, true)},
        {
            "turn",
            (char_turn, false)},
        {
            "turn_to",
            (char_turn_to, true)},
        {
            "rotate_to",
            (char_rotate_to, true)},
        {
            "scale_to",
            (char_scale_to, true)},
        {
            "kinematic",
            (char_kinematic, false)},
        {
            "fk_active",
            (char_fk_active, false)},
        {
            "fk_set",
            (char_fk_set, true)},
        {
            "ik_active",
            (char_ik_active, false)},
        {
            "ik_set",
            (char_ik_set, true)},
        {
            "voice_lst",
            (char_voice_lst, false)},
        {
            "voice_rpt",
            (char_voice_rpt, false)},
        {
            "visible",
            (char_visible, false)},
        {
            "ext_bodyshapes",
            (char_ext_bodyshapes, true)},
        {
            "ext_faceshapes",
            (char_ext_faceshapes, true)},
        {
            "pl_kkpedata",
            (char_pl_kkpedata, false)},
        {
            "pl_aipedata",
            (char_pl_aipedata, false)}};

        */
    }
}
