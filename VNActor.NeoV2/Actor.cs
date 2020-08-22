using AIChara;
using MessagePack;
using Studio;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace VNActor
{

    // AI Actor
    public partial class Actor
    {

        public class ActorData : IDataClass
        {
            internal float tearLevel;
            internal float tuya;
            internal float wetness;

            [Key("visible")]
            public bool visible;
            [Key("position")]
            public Vector3 position;
            [Key("scale")]
            public Vector3 scale;
            [Key("rotation")]
            public Vector3 rotation;
            [Key("voiceRepeat")]
            public int voiceRepeat;
            //public Itembool shoesOn;
            [Key("voiceList")]
            public List<int[]> voiceList;
            [Key("fk")]
            public Dictionary<int, Vector3> fk;
            [Key("fkActive")]
            public bool[] fkActive;
            [Key("kinematicType")]
            public KinematicMode kinematicType;
            [Key("handMotions")]
            public Hands_s handMotions;
            [Key("ik")]
            public Dictionary<string, IK_node_s> ik;
            [Key("ikActive")]
            public bool[] ikActive;
            [Key("lipSync")]
            public bool lipSync;
            [Key("mouthOpen")]
            public float mouthOpen;
            [Key("mouthPattern")]
            public int mouthPattern;
            [Key("blinking")]
            public bool blinking;
            [Key("eyesOpen")]
            public float eyesOpen;
            [Key("eyePattern")]
            public int eyePattern;
            [Key("eyebrowPattern")]
            public int eyebrowPattern;
            [Key("neckPattern")]
            public int neckPattern;
            [Key("eyeLookPos")]
            public Vector3 eyeLookPos;
            [Key("eyeLookPattern")]
            public int eyeLookPattern;
            [Key("son")]
            public Son_s son;
            [Key("anim")]
            public Animation_s anim;
            [Key("simple")]
            public bool simple;
            [Key("simpleColor")]
            public Color simpleColor;
            [Key("juice")]
            public byte[] juice;
            //public bool showAllAccessories;
            [Key("accessoryStatus")]
            public bool[] accessoryStatus;
            [Key("cloth")]
            public byte[] cloth;
            [Key("animeSpeed")]
            public float animeSpeed;
            [Key("forceLoop")]
            public bool forceLoop;
            [Key("animePattern")]
            public float animePattern;
            [Key("animeOption")]
            public AnimeOption_s animeOption;
            [Key("faceRedness")]
            public float faceRedness;
            [Key("nippleHardness")]
            public float nippleHardness;
            [Key("neck")]
            public byte[] neck;

            public ActorData(Actor a)
            {

                visible = a.visible;
                position = a.pos;
                rotation = a.rot;
                scale = a.scale;
                animeSpeed = a.animeSpeed;
                animePattern = a.animePattern;
                tearLevel = a.tearLevel;
                forceLoop = a.anime_forceloop;

                accessoryStatus = new bool[a.accessory.Length];
                Array.Copy(a.accessory, accessoryStatus, accessoryStatus.Length);

                faceRedness = a.facered;
                son = a.son;

                anim = a.animate;

                animeOption = new AnimeOption_s { height = a.height, breast = a.breast };

                cloth = a.get_cloth();

                juice = a.juice;
                nippleHardness = a.nipple_stand;

                simple = a.simple;
                simpleColor = a.simple_color;

                eyeLookPattern = a.look_eye_ptn;
                eyeLookPos = a.look_eye_pos;
                neckPattern = a.look_neck;

                neck = a.look_neck_full2;
                eyebrowPattern = a.eyebrow_ptn;
                eyePattern = a.eyes_ptn;
                eyesOpen = a.eyes_open;
                blinking = a.eyes_blink;
                mouthPattern = a.mouth_ptn;
                mouthOpen = a.mouth_open;
                lipSync = a.lip_sync;
                handMotions = a.hand_ptn;
                kinematicType = a.kinematic;

                fkActive = null;
                fk = null;
                ikActive = null;
                ik = null;

                if (kinematicType == KinematicMode.FK || kinematicType == KinematicMode.IKFK)
                {
                    fkActive = new bool[a.get_FK_active().Length];
                    Array.Copy(a.get_FK_active(), fkActive, fkActive.Length);
                    fk = a.export_fk_bone_info();
                }
                if (kinematicType == KinematicMode.IK || kinematicType == KinematicMode.IKFK)
                {
                    ikActive = new bool[a.get_IK_active().Length];
                    Array.Copy(a.get_IK_active(), ikActive, ikActive.Length);
                    ik = a.export_ik_target_info();
                }

                voiceList = a.voice_lst;
                voiceRepeat = a.voice_repeat;


                tuya = a.tuya;
                wetness = a.wet;

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
        }

        public float height
        {
            get
            {
                // get height:
                return this.objctrl.oiCharInfo.charFile.custom.body.shapeValueBody[0];
            }
        }

        public float breast
        {
            get
            {
                // get breast:
                return this.objctrl.oiCharInfo.charFile.custom.body.shapeValueBody[1];
            }
        }

        public byte[] get_cloth()
        {
            // return state index of (top, bottom, bra, shorts, grove, panst, sock, shoes) in tuple
            return this.objctrl.charFileStatus.clothesState;
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

        public bool[] accessory
        {
            get
            {
                // return accessory state on/off in tuple(20)
                return this.objctrl.charFileStatus.showAccessory;
            }
            set
            {
                for (int i = 0; i < value.Length; i++)
                {
                    this.objctrl.ShowAccessory(i, value[i]);
                }
            }
        }

        public byte[] juice
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

        public float tearLevel
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

        public float tuya
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

        public float wet
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

        /*
        public void set_son((bool visible, float length) sonState)
        {
            // sonState: (0(False)/1(True), length(0~3))
            this.objctrl.SetVisibleSon(sonState.visible);
            this.objctrl.SetSonLength(sonState.length);
        }
        */

        /*
        public (bool visible, float length) get_son()
        {
            // return son (visible, length) in tuple
            return (this.objctrl.oiCharInfo.visibleSon, this.objctrl.oiCharInfo.sonLength);
        }
        */

        public int eyebrow_ptn
        {
            get
            {
                // return eyebrow pattern
                return this.objctrl.charInfo.GetEyebrowPtn();
            }
            set
            {
                this.objctrl.charInfo.ChangeEyebrowPtn(value);
            }
        }

        public void set_kinematic(KinematicMode mode, bool force = false)
        {
            // mode: 0-none, 1-IK, 2-FK, 3-IK&FK
            if (mode == KinematicMode.IKFK)
            {
                // enable IK
                this.objctrl.finalIK.enabled = true;
                this.objctrl.oiCharInfo.enableIK = true;
                this.objctrl.ActiveIK(OIBoneInfo.BoneGroup.Body, this.objctrl.oiCharInfo.activeIK[0], true);
                this.objctrl.ActiveIK(OIBoneInfo.BoneGroup.RightLeg, this.objctrl.oiCharInfo.activeIK[1], true);
                this.objctrl.ActiveIK(OIBoneInfo.BoneGroup.LeftLeg, this.objctrl.oiCharInfo.activeIK[2], true);
                this.objctrl.ActiveIK(OIBoneInfo.BoneGroup.RightArm, this.objctrl.oiCharInfo.activeIK[3], true);
                this.objctrl.ActiveIK(OIBoneInfo.BoneGroup.LeftArm, this.objctrl.oiCharInfo.activeIK[4], true);
                // enable FK, disable "body" because it should be controlled by IK
                this.objctrl.oiCharInfo.activeFK[3] = false;
                this.objctrl.fkCtrl.enabled = true;
                this.objctrl.oiCharInfo.enableFK = true;
                foreach (var i in Enumerable.Range(0, FKCtrl.parts.Length))
                {
                    try
                    {
                        this.objctrl.ActiveFK(FKCtrl.parts[i], this.objctrl.oiCharInfo.activeFK[i], true);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(String.Format($"Error set kinematic to 3(IK&FK), when ActiveFK[{i}: {FKCtrl.parts[i]}]. Error message = {e}"));
                    }
                }
                // call ActiveKinematicMode to set pvCopy?
                this.objctrl.ActiveKinematicMode(OICharInfo.KinematicMode.IK, true, false);
            }
            else if (mode == KinematicMode.FK)
            {
                if (this.objctrl.oiCharInfo.enableIK)
                {
                    try
                    {
                        this.objctrl.ActiveKinematicMode(OICharInfo.KinematicMode.IK, false, force);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Error set kinematic to 2(FK), when clear IK. Error message = {e}");
                    }
                }
                if (!this.objctrl.oiCharInfo.enableFK)
                {
                    try
                    {
                        this.objctrl.ActiveKinematicMode(OICharInfo.KinematicMode.FK, true, force);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Error set kinematic to 2(FK), when set FK. Error message = {e}");
                    }
                }
            }
            else if (mode == KinematicMode.IK)
            {
                if (this.objctrl.oiCharInfo.enableFK)
                {
                    try
                    {
                        this.objctrl.ActiveKinematicMode(OICharInfo.KinematicMode.FK, false, force);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Error set kinematic to 1(IK), when clear FK. Error message = {e}");
                    }
                }
                if (!this.objctrl.oiCharInfo.enableIK)
                {
                    try
                    {
                        this.objctrl.ActiveKinematicMode(OICharInfo.KinematicMode.IK, true, force);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Error set kinematic to 1(IK), when set IK. Error message = {e}");
                    }
                }
            }
            else
            {
                if (this.objctrl.oiCharInfo.enableIK)
                {
                    try
                    {
                        this.objctrl.ActiveKinematicMode(OICharInfo.KinematicMode.IK, false, force);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Error set kinematic to 0(None), when clear IK. Error message = {e}");
                    }
                }
                if (this.objctrl.oiCharInfo.enableFK)
                {
                    try
                    {
                        this.objctrl.ActiveKinematicMode(OICharInfo.KinematicMode.FK, false, force);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Error set kinematic to 0(None), when clear FK. Error message = {e}");
                    }
                }
            }
        }

        public Dictionary<int, Vector3> export_fk_bone_info(bool activedOnly = true)
        {
            // export a dic contents FK bone info
            var biDic = new Dictionary<int, Vector3>();
            foreach (var binfo in this.objctrl.listBones)
            {
                if (!activedOnly || binfo.active)
                {
                    // posClone = Vector3(binfo.posision.x, binfo.posision.y, binfo.posision.z)
                    var rot = binfo.boneInfo.changeAmount.rot;
                    var rotClone = new Vector3(rot.x <= 180 ? rot.x : rot.x - 360, rot.y <= 180 ? rot.y : rot.y - 360, rot.z <= 180 ? rot.z : rot.z - 360);
                    // abDic[binfo.boneID] = (posClone, rotClone)
                    biDic[binfo.boneID] = rotClone;
                }
            }
            // print "exported", len(biDic), "bones"
            return biDic;
        }

        public void import_fk_bone_info(Dictionary<int, Vector3> biDic)
        {
            // import fk bone info from dic
            foreach (var binfo in this.objctrl.listBones)
            {
                if (biDic.ContainsKey(binfo.boneID))
                {
                    binfo.boneInfo.changeAmount.rot = biDic[binfo.boneID];
                }
            }
        }

        public Dictionary<string, IK_node_s> export_ik_target_info(bool activedOnly = true)
        {
            // export a dic contents IK target info
            var itDic = new Dictionary<string, IK_node_s>
            {
            };
            foreach (var itInfo in this.objctrl.listIKTarget)
            {
                if (!activedOnly || itInfo.active)
                {
                    var tgtName = itInfo.boneObject.name;
                    var pos = itInfo.targetInfo.changeAmount.pos;
                    var posClone = new Vector3(pos.x, pos.y, pos.z);
                    if (tgtName.Contains("_hand_") || tgtName.Contains("_leg03_"))
                    {
                        var rot = itInfo.targetInfo.changeAmount.rot;
                        var rotClone = new Vector3(rot.x, rot.y, rot.z);
                        //rotClone = Vector3(rot.x if rot.x <= 180 else rot.x - 360, rot.y if rot.y <= 180 else rot.y - 360, rot.z if rot.z <= 180 else rot.z - 360)
                        itDic[tgtName] = new IK_node_s { pos = posClone, rot = rotClone };
                    }
                    else
                    {
                        itDic[tgtName] = new IK_node_s { pos = posClone, rot = null };
                    }
                }
            }
            //print "exported", len(itDic), "IK Targets"
            return itDic;
        }

        public void import_ik_target_info(Dictionary<string, IK_node_s> itDic)
        {
            // import IK target info from dic 
            foreach (var ikTgt in this.objctrl.listIKTarget)
            {
                var ikTgName = ikTgt.boneObject.name;
                if (itDic.ContainsKey(ikTgName))
                {

                    ikTgt.targetInfo.changeAmount.pos = itDic[ikTgName].pos;

                    if (ikTgName.Contains("_hand_") || ikTgName.Contains("_leg03_"))
                    {
                        if (itDic[ikTgName].rot is Vector3 ik_rot)
                        {
                            ikTgt.targetInfo.changeAmount.rot = ik_rot;
                        }
                    }
                }
            }
        }

        public byte[] look_neck_full2
        {
            get
            {
                // needed only to save Fixed state
                if (this.look_neck == 4)
                {
                    var memoryStream = new MemoryStream();
                    var binaryWriter = new BinaryWriter(memoryStream);
                    this.objctrl.neckLookCtrl.SaveNeckLookCtrl(binaryWriter);
                    binaryWriter.Close();
                    memoryStream.Close();
                    return memoryStream.ToArray();
                }
                else
                {
                    return null;
                }
            }
            set
            {
                // needed only to set Fixed state
                if (!value.IsNullOrEmpty())
                {
                    // if non-fixed-state - move to it!
                    this.look_neck = 4;
                }
                if (this.look_neck == 4)
                {
                    // print lst
                    // print arrstate
                    var binaryReader = new BinaryReader(new MemoryStream(value));
                    this.objctrl.neckLookCtrl.LoadNeckLookCtrl(binaryReader);
                }
            }
        }

        public byte[] curcloth_coordinate
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
                    this.objctrl.charInfo.Reload();
                }
                catch (Exception e)
                {
                    Console.WriteLine(String.Format("Exception in set_curcloth_coordinate, {0}", e.ToString()));
                }
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

        public object get_face_shape(int p1)
        {
            return this.charInfo.GetShapeFaceValue(p1);
        }

        public void set_face_shape(int p1, float p2)
        {
            this.charInfo.SetShapeFaceValue(p1, p2);
        }

        public float[] face_shapes_all
        {
            get
            {
                return this.objctrl.oiCharInfo.charFile.custom.face.shapeValueFace;
            }
            set
            {
                this.objctrl.oiCharInfo.charFile.custom.face.shapeValueFace = value;
            }
        }

        public object get_face_shape_names()
        {
            return ChaFileDefine.cf_headshapename;
        }

        public int face_shapes_count
        {
            get
            {
                return this.face_shapes_all.Length;
            }
        }
        

        /* TODO

        public string get_aipedata()
        {
            return AIPE.GetCharaSettingsText(this.objctrl);
        }

        public void set_aipedata(object aipedata)
        {
            if (aipedata != "")
            {

                AIPE.SetCharaSettingsText(this.objctrl, aipedata);

            }
        }
        */

        public static void char_tuya(Actor chara, ActorData param)
        {
            // param = skin tuya 0~1
            chara.tuya = param.tuya;
        }

        public static void char_wet(Actor chara, ActorData param)
        {
            // param = skin wet 0~1
            chara.wet = param.wetness;
        }

        public IDataClass export_full_status()
        {
            // export a dict contains all actor status
            return new ActorData(this);
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
            if (this.pos != partner.pos || this.rot != partner.rot || this.scale != partner.scale)
            {
                partner.move(pos: this.pos, rot: this.rot, scale: this.scale);
            }
            if (this.animeSpeed != partner.animeSpeed)
            {
                partner.animeSpeed = this.animeSpeed;
            }
            if (this.animePattern != partner.animePattern)
            {
                partner.animePattern = this.animePattern;
            }
            if (this.anime_forceloop != partner.anime_forceloop)
            {
                partner.anime_forceloop = this.anime_forceloop;
            }
            // decide sex role
            if (this.sex == 0)
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
            var mss = mactor.son;
            if (!mss.visible && mactor.sex == 0)
            {
                mactor.son = new Son_s { visible = true, length = mss.length };
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
                mactor.setAnimate(2, validCategoryKey[hPosition], validNoKey[hStage]);
                factor.setAnimate(1, validCategoryKey[hPosition], validNoKey[hStage]);
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
                mactor.setAnimate(4, validCategoryKey[hPosition], validNoKey[hStage]);
                factor.setAnimate(3, validCategoryKey[hPosition], validNoKey[hStage]);
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
                mactor.setAnimate(6, validCategoryKey[hPosition], validNoKey[hStage]);
                factor.setAnimate(5, validCategoryKey[hPosition], validNoKey[hStage]);
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
                mactor.setAnimate(8, validCategoryKey[hPosition], validNoKey[hStage]);
                factor.setAnimate(7, validCategoryKey[hPosition], validNoKey[hStage]);
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
                mactor.setAnimate(9, validCategoryKey[hPosition], validNoKey[hStage]);
                factor.setAnimate(9, validCategoryKey[hPosition] + 1, validNoKey[hStage]);
            }
            // auto adjust anime param
            Console.WriteLine(String.Format("factor(%s): height=%.2f breast=%.2f", factor.text_name, factor.height, factor.breast));
            var anime_option_param = new AnimeOption_s { height = factor.height, breast = factor.breast};
            if (factor.isHAnime)
            {
                factor.anime_option_param = anime_option_param;
            }
            if (mactor.isHAnime)
            {
                mactor.anime_option_param = anime_option_param;
            }
            foreach (var extActor in extActors)
            {
                if (extActor != null && extActor.isHAnime)
                {
                    extActor.anime_option_param = anime_option_param;
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
            chara.tearLevel = param.tearLevel;
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
