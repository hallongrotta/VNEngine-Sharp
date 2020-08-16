using MessagePack;
using Sirenix.OdinInspector.Demos;
using Studio;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace VNActor
{
    // Koikatsu Actor
    public partial class Actor
        : IVNObject
    {

        public struct IK_node_s
        {
            public Vector3 pos;
            public Vector3? rot;
        }

        [Serializable]
        [MessagePackObject]
        public struct Hands_s
        {
            [Key(0)]
            public int leftMotion;
            [Key(1)]
            public int rightMotion;
        }

        [Serializable]
        [MessagePackObject]
        public struct Son_s
        {
            [Key(0)]
            public bool visible;
            [Key(1)]
            public float length;
        }

        [Serializable]
        [MessagePackObject]
        public struct Animation_s
        {
            [Key(0)]
            public int group;
            [Key(1)]
            public int category;
            [Key(2)]
            public int no;
            [Key(3)]
            public float? normalizedTime;
        }

        public enum KinematicMode
        {
            None,
            FK,
            IK,
            IKFK
        }


        [MessagePackObject(keyAsPropertyName: true)]
        public class ActorData : IDataClass
        {
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
            [Key("tearLevel")]
            public int tearLevel;
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
            [Key("coordinateType")]
            public int coordinateType;
            [Key("faceRedness")]
            public float faceRedness;
            [Key("nippleHardness")]
            public float nippleHardness;
            [Key("neck")]
            public byte[] neck;
            [Key("shoesType")]
            public int shoesType;

            public ActorData()
            {

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

                coordinateType = a.coordinate_type;
                cloth = a.cloth;

                juice = a.juice;
                nippleHardness = a.nipple_stand;

                simple = a.simple;
                simpleColor = a.simple_color;

                eyeLookPattern = a.look_eye_ptn;
                eyeLookPos = a.look_eye_pos;
                neckPattern = a.look_neck;

                neck = a.get_look_neck_full2();
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
                shoesType = a.shoes_type;

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

        public int coordinate_type
        {
            set
            {
                // type: 0-School01, 1-School02, 2-Gym, 3-Swim, 4-Club, 5-Plain, 6-Pajamas
                if (coordinate_type != value)
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

        public int shoes_type
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

        public byte[] cloth
        {
            get
            {
                // return state index of (top, bottom, bra, shorts, grove, panst, sock, shoes) in tuple
                // NOTE: self.objctrl.charFileStatus.clothesState return list[9] with 2 shoes
                byte[] cloth = new byte[8];

                for (int i = 0; i < 8; i++)
                {
                    cloth[i] = this.objctrl.charFileStatus.clothesState[i];
                }

                return cloth;
            }
        }

        public void SetAccessory(int accIndex, bool accShow)
        {
            this.objctrl.ShowAccessory(accIndex, accShow);
        }

        public void SetAccessory(bool accShow)
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

        public void SetAccessory(bool[] accIndex)
        {
            for (int i = 0; i < accIndex.Length; i++)
            {
                this.objctrl.ShowAccessory(i, accIndex[i]);
            }
        }

        public bool[] accessory
        {
            get
            {
                // return accessory state on/off in tuple(20)
                return this.objctrl.charFileStatus.showAccessory;
            }
        }

        public byte[] juice
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

        public int eye_ptn
        {
            get
            {
                // return eye look at pattern: 0: front, 1: camera, 2: hide from camera, 3: fix, 4: operate
                return this.objctrl.charInfo.GetLookEyesPtn();
            }
        }

        public int eyebrow_ptn
        {
            get
            {
                // return eyebrow pattern
                return this.objctrl.charInfo.GetEyebrowPtn();
            }
            set
            {
                // ptn: 0 to 16
                this.objctrl.charInfo.ChangeEyebrowPtn(value);
            }
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
            var biDic = new Dictionary<int, Vector3>
            {
            };
            foreach (var binfo in this.objctrl.listBones)
            {
                if (!activedOnly || binfo.active)
                {
                    //posClone = Vector3(binfo.posision.x, binfo.posision.y, binfo.posision.z)
                    var rot = binfo.boneInfo.changeAmount.rot;
                    var rotClone = new Vector3(rot.x <= 180 ? rot.x : rot.x - 360, rot.y <= 180 ? rot.y : rot.y - 360, rot.z <= 180 ? rot.z : rot.z - 360);
                    //abDic[binfo.boneID] = (posClone, rotClone)
                    biDic[binfo.boneID] = rotClone;
                }
            }
            //print "exported", len(biDic), "bones"
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

        public byte[] get_look_neck_full2()
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

        public void set_look_neck_full2(byte[] str64)
        {
            // needed only to set Fixed state
            if (str64.Length > 0)
            {
                // if non-fixed-state - move to it!
                this.look_neck = 4;
            }
            if (this.look_neck == 4)
            {
                // print lst
                // print arrstate
                var binaryReader = new BinaryReader(new MemoryStream(str64));
                this.objctrl.neckLookCtrl.LoadNeckLookCtrl(binaryReader);
            }
        }

        public string curcloth_coordinate
        {
            get
            {
                var bytes = this.objctrl.charInfo.nowCoordinate.SaveBytes();
                return Utils.bytearray_to_str64(bytes);
            }
            set
            {
                var bytes = Utils.str64_to_bytearray(value);
                try
                {
                    this.objctrl.charInfo.nowCoordinate.LoadBytes(bytes, ChaFileDefine.ChaFileCoordinateVersion);
                    //self.objctrl.charInfo.Reload()
                    //self.objctrl.charInfo.AssignCoordinate(ChaFileDefine.CoordinateType[self.objctrl.charInfo.fileStatus.coordinateType])
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

        public void import_status(IDataClass tmp_status)
        {
            if (tmp_status is ActorData data)
            {
                import_status(data);
            }
        }

        public float[] body_shapes_all
        {
            get
            {
                return this.objctrl.oiCharInfo.charFile.custom.body.shapeValueBody;
            }
        }

        public string[] get_body_shape_names()
        {
            return ChaFileDefine.cf_bodyshapename;
        }

        public int body_shapes_count
        {
            get
            {
                return this.body_shapes_all.Length;
            }
        }

        public float get_face_shape(int p1)
        {
            return this.charInfo.GetShapeFaceValue(p1);
        }

        public void set_face_shape(int p1, float p2)
        {
            this.charInfo.SetShapeFaceValue(p1, p2);
        }

        public float[] face_shape
        {
            get
            {
                return this.objctrl.oiCharInfo.charFile.custom.face.shapeValueFace;
            }
        }

        public string[] face_shape_names
        {
            get
            {
                return ChaFileDefine.cf_headshapename;
            }
        }

        public int face_shapes_count
        {
            get
            {
                return this.face_shapes_all.Length;
            }
        }

        public IDataClass export_full_status()
        {
            return new ActorData(this);
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

        public void import_status(ActorData a)
        {
            visible = a.visible;
            pos = a.position;
            rot = a.rotation;
            scale = a.scale;
            animeSpeed = a.animeSpeed;
            animePattern = a.animePattern;
            tearLevel = a.tearLevel;
            anime_forceloop = a.forceLoop;
            SetAccessory(a.accessoryStatus);
            facered = a.faceRedness;
            son = a.son;

            setAnimate(a.anim.group, a.anim.category, a.anim.no);

            //(height, breast) = a.animeOption;

            coordinate_type = a.coordinateType;
            setCloth(a.cloth);

            juice = a.juice;
            nipple_stand = a.nippleHardness;

            simple = a.simple;
            simple_color = a.simpleColor;

            look_eye_ptn = a.eyeLookPattern;
            look_eye_pos = a.eyeLookPos;
            look_neck = a.neckPattern;

            set_look_neck_full2(a.neck);
            eyebrow_ptn = a.eyebrowPattern;
            eyes_ptn = a.eyePattern;
            eyes_open = a.eyesOpen;
            eyes_blink = a.blinking;
            mouth_ptn = a.mouthPattern;
            mouth_open = a.mouthOpen;
            lip_sync = a.lipSync;
            hand_ptn = a.handMotions;
            set_kinematic(a.kinematicType);

            if (a.kinematicType == KinematicMode.IK)
            {
                set_IK_active(a.ikActive);
                import_ik_target_info(a.ik);
            }
            else if (a.kinematicType == KinematicMode.FK)
            {
                set_FK_active(a.fkActive);
                import_fk_bone_info(a.fk);
            }
            else if (a.kinematicType == KinematicMode.IKFK)
            {
                set_IK_active(a.ikActive);
                import_ik_target_info(a.ik);
                set_FK_active(a.fkActive);
                import_fk_bone_info(a.fk);
            }

            //voice_lst = a.voiceList;
            voice_repeat = a.voiceRepeat;
            shoes_type = a.shoesType;
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
                mactor.setAnimate(3, validCategoryKey[hPosition], validNoKey[hStage]);
                factor.setAnimate(2, validCategoryKey[hPosition], validNoKey[hStage]);
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
                mactor.setAnimate(5, validCategoryKey[hPosition], validNoKey[hStage]);
                factor.setAnimate(4, validCategoryKey[hPosition], validNoKey[hStage]);
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
                mactor.setAnimate(9, validCategoryKey[hPosition], validNoKey[hStage]);
                factor.setAnimate(9, validCategoryKey[hPosition] + 1, validNoKey[hStage]);
            }
            // auto adjust anime param
            Console.WriteLine(String.Format("factor({0}): height={1} breast={2}", factor.text_name, factor.height, factor.breast));
            var anime_option_param = new AnimeOption_s { height = factor.height, breast = factor.breast };
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
                kkChara.coordinate_type = param.coordinateType;
            }
        }

        public int tearLevel
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
            chara.tearLevel = param.tearLevel;
        }

        public static void char_shoes(Actor chara, ActorData param)
        {
            // param = 0 or 1
            chara.shoes_type = param.shoesType;
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
            chara.SetAccessory(param.accessoryStatus);
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
        {
            "cloth",
            new FunctionBoolPair(char_cloth, false)},
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
