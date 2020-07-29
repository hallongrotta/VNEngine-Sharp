using Studio;
using System;
using System.Collections.Generic;
using AIChara;
using System.Linq;
using UnityEngine;
using System.IO;

namespace VNActor
{

    public partial class Actor : IActor
    {

        public struct ActorData : IDataClass
        {
            public bool visible;
            public Vector3 position;
            public Vector3 scale;
            public Vector3 rotation;
            public int voiceRepeat;
            public bool shoesOn;
            public List<int[]> voiceList;
            public Dictionary<string, (Vector3 pos, Vector3 rot)> ik;
            public bool[] ikActive;
            public Dictionary<int, Vector3> fk;
            public bool[] fkActive;
            public int kinematicType;
            public float y_rotation;
            public (int, int) handMotions;
            public bool lipSync;
            public float mouthOpen;
            public int mouthPattern;
            public bool blinking;
            public float eyesOpen;
            public float wetness;
            public int eyePattern;
            public int eyebrowPattern;
            public int neckPattern;
            public Vector3 eyeLookPos;
            public int eyeLookPattern;
            public bool son;
            public bool simple;
            public Color simpleColor;
            public int tearLevel;
            public byte[] juice;
            public bool showAllAccessories;
            public bool[] accessoryStatus;
            public (int clothIndex, int clothState) cloth;
            public (int accIndex, bool accShow) accessory;
            public float animeSpeed;
            public bool forceLoop;
            public float animePattern;
            public (float height, float breast) animeOption;
            public string clothLoad;
            public int coordinateType;
            public float faceRedness;
            public float nippleHardness;
            public float tuya;
            public string neck;

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

        public void set_accessory(bool[] accIndex)
        {
            for (int i = 0; i < accIndex.Length; i++)
            {
                this.objctrl.ShowAccessory(i, accIndex[i]);
            }
        }

        public bool[] get_accessory()
        {
            // return accessory state on/off in tuple(20)
            return this.objctrl.charFileStatus.showAccessory;
        }

        public void set_juice(byte[] juices)
        {
            // juices: level on (face, FrontUp, BackUp, FrontDown, BackDown) when 0-none, 1-few, 2-lots
            this.objctrl.SetSiruFlags(ChaFileDefine.SiruParts.SiruKao, juices[0]);
            this.objctrl.SetSiruFlags(ChaFileDefine.SiruParts.SiruFrontTop, juices[1]);
            this.objctrl.SetSiruFlags(ChaFileDefine.SiruParts.SiruBackTop, juices[2]);
            this.objctrl.SetSiruFlags(ChaFileDefine.SiruParts.SiruFrontBot, juices[3]);
            this.objctrl.SetSiruFlags(ChaFileDefine.SiruParts.SiruBackBot, juices[4]);
        }

        public List<byte> get_juice()
        {
            // return juice level of (face, FrontUp, BackUp, FrontDown, BackDown) in tuple
            var jInfo = new List<byte>();
            jInfo.Add(this.objctrl.GetSiruFlags(ChaFileDefine.SiruParts.SiruKao));
            jInfo.Add(this.objctrl.GetSiruFlags(ChaFileDefine.SiruParts.SiruFrontTop));
            jInfo.Add(this.objctrl.GetSiruFlags(ChaFileDefine.SiruParts.SiruBackTop));
            jInfo.Add(this.objctrl.GetSiruFlags(ChaFileDefine.SiruParts.SiruFrontBot));
            jInfo.Add(this.objctrl.GetSiruFlags(ChaFileDefine.SiruParts.SiruBackBot));
            return jInfo;
        }

        public void set_tear(float level)
        {
            // level: 0~1
            this.objctrl.SetTears(level);
        }

        public float get_tear()
        {
            // return tear level
            return this.objctrl.GetTears();
        }

        public void set_tuya(float level)
        {
            // level: tuya 0~1
            this.objctrl.SetTuyaRate(level);
        }

        public float get_tuya()
        {
            // return tuya rate
            return this.objctrl.oiCharInfo.SkinTuyaRate;
        }

        public void set_wet(float level)
        {
            // level: wet 0~1
            this.objctrl.SetWetRate(level);
        }

        public float get_wet()
        {
            // return wet rate
            return this.objctrl.oiCharInfo.WetRate;
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

        public void set_simple(bool simpleState)
        {
            // simple = one color, 1(true)/0(false)
            this.objctrl.SetVisibleSimple(simpleState);
        }

        public void set_simple_color(Color simpleColor)
        {
            // simple color
            this.objctrl.SetSimpleColor(simpleColor);
        }

        public Color get_simple_color()
        {
            // return simple color
            return this.objctrl.oiCharInfo.simpleColor;
        }

        public int get_look_eye_ptn()
        {
            // return eye look at pattern: 0: front, 1: camera, 2: hide from camera, 3: fix, 4: operate
            return this.objctrl.charInfo.GetLookEyesPtn();
        }

        public void set_look_neck(int ptn)
        {
            // ptn for CharaStudio: 0: front, 1: camera, 2: hide from camera, 3: by anime, string = 4: fix
            //if isinstance(ptn, str):
            //    self.set_look_neck_full2(ptn)
            //else:
            this.objctrl.ChangeLookNeckPtn(ptn);
        }

        public int get_look_neck()
        {
            // return neck look pattern: 0: front, 1: camera, 2: hide from camera, 3: by anime, 4: fix neck full as a string
            var ptn = this.objctrl.charInfo.GetLookNeckPtn();
            //if ptn == 4:
            //    return self.get_look_neck_full2()
            //else:
            //    return ptn
            return ptn;
        }

        public void set_eyebrow_ptn(int ptn)
        {
            // ptn: 0 to 16
            this.objctrl.charInfo.ChangeEyebrowPtn(ptn);
        }

        public int get_eyebrow_ptn()
        {
            // return eyebrow pattern
            return this.objctrl.charInfo.GetEyebrowPtn();
        }

        public void set_kinematic(int mode, bool force = false)
        {
            // mode: 0-none, 1-IK, 2-FK, 3-IK&FK
            if (mode == 3)
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
                        Console.WriteLine(String.Format("Error set kinematic to 3(IK&FK), when ActiveFK[%d: %s]. Error message = %s, Exception type = %s", i, FKCtrl.parts[i].ToString(), e.ToString(), e.ToString()));
                    }
                }
                // call ActiveKinematicMode to set pvCopy?
                this.objctrl.ActiveKinematicMode(OICharInfo.KinematicMode.IK, true, false);
            }
            else if (mode == 2)
            {
                if (this.objctrl.oiCharInfo.enableIK)
                {
                    try
                    {
                        this.objctrl.ActiveKinematicMode(OICharInfo.KinematicMode.IK, false, force);
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("Error set kinematic to 2(FK), when clear IK");
                    }
                }
                if (!this.objctrl.oiCharInfo.enableFK)
                {
                    try
                    {
                        this.objctrl.ActiveKinematicMode(OICharInfo.KinematicMode.FK, true, force);
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("Error set kinematic to 2(FK), when set FK");
                    }
                }
            }
            else if (mode == 1)
            {
                if (this.objctrl.oiCharInfo.enableFK)
                {
                    try
                    {
                        this.objctrl.ActiveKinematicMode(OICharInfo.KinematicMode.FK, false, force);
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("Error set kinematic to 1(IK), when clear FK");
                    }
                }
                if (!this.objctrl.oiCharInfo.enableIK)
                {
                    try
                    {
                        this.objctrl.ActiveKinematicMode(OICharInfo.KinematicMode.IK, true, force);
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("Error set kinematic to 1(IK), when set IK");
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
                    catch (Exception)
                    {
                        Console.WriteLine("Error set kinematic to 0(None), when clear IK");
                    }
                }
                if (this.objctrl.oiCharInfo.enableFK)
                {
                    try
                    {
                        this.objctrl.ActiveKinematicMode(OICharInfo.KinematicMode.FK, false, force);
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("Error set kinematic to 0(None), when clear FK");
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

        public Dictionary<string, (Vector3, Vector3?)> export_ik_target_info(bool activedOnly = true)
        {
            // export a dic contents IK target info
            var itDic = new Dictionary<string, (Vector3, Vector3?)>
            {
            };
            foreach (var itInfo in this.objctrl.listIKTarget)
            {
                if (!activedOnly || itInfo.active)
                {
                    string tgtName = itInfo.boneObject.name;
                    Vector3 pos = itInfo.targetInfo.changeAmount.pos;
                    Vector3 posClone = new Vector3(pos.x, pos.y, pos.z);
                    if (tgtName.Contains("_Hand_") || tgtName.Contains("_Foot01_"))
                    {
                        Vector3 rot = itInfo.targetInfo.changeAmount.rot;
                        Vector3 rotClone = new Vector3(rot.x, rot.y, rot.z);
                        // rotClone = Vector3(rot.x if rot.x <= 180 else rot.x - 360, rot.y if rot.y <= 180 else rot.y - 360, rot.z if rot.z <= 180 else rot.z - 360)
                        itDic[tgtName] = (posClone, rotClone);
                    }
                    else
                    {
                        itDic[tgtName] = (posClone, null);
                    }
                }
            }
            // print "exported", len(itDic), "IK Targets"
            return itDic;
        }

        public void import_ik_target_info(Dictionary<string, (Vector3 pos, Vector3? rot)> itDic)
        {
            // import IK target info from dic
            foreach (var ikTgt in this.objctrl.listIKTarget)
            {
                var ikTgName = ikTgt.boneObject.name;
                if (itDic.ContainsKey(ikTgName))
                {
                   ikTgt.targetInfo.changeAmount.pos = itDic[ikTgName].pos;

                    if ((ikTgName.Contains("_Hand_") || ikTgName.Contains("_Foot01_")))
                    {
                        if (itDic[ikTgName].rot is Vector3 vec)
                        {
                            ikTgt.targetInfo.changeAmount.rot = vec;
                        }
                    }
                }
            }
        }

        public string get_look_neck_full2()
        {
            // needed only to save Fixed state
            if (this.get_look_neck() == 4)
            {
                var memoryStream = new MemoryStream();
                var binaryWriter = new BinaryWriter(memoryStream);
                this.objctrl.neckLookCtrl.SaveNeckLookCtrl(binaryWriter);
                binaryWriter.Close();
                memoryStream.Close();
                return Utils.bytearray_to_str64(memoryStream.ToArray());
            }
            else
            {
                return "";
            }
        }

        public void set_look_neck_full2(string str64)
        {
            // needed only to set Fixed state
            if (str64.Length > 0)
            {
                // if non-fixed-state - move to it!
                this.set_look_neck(4);
            }
            if (this.get_look_neck() == 4)
            {
                // print lst
                var arrstate = Utils.str64_to_bytearray(str64);
                // print arrstate
                var binaryReader = new BinaryReader(new MemoryStream(arrstate));
                this.objctrl.neckLookCtrl.LoadNeckLookCtrl(binaryReader);
            }
        }

        public string get_curcloth_coordinate()
        {
            var bytes = this.objctrl.charInfo.nowCoordinate.SaveBytes();
            return Utils.bytearray_to_str64(bytes);
        }

        public void set_curcloth_coordinate(string str64)
        {
            var bytes = Utils.str64_to_bytearray(str64);
            try
            {
                this.objctrl.charInfo.nowCoordinate.LoadBytes(bytes, ChaFileDefine.ChaFileCoordinateVersion);
                this.objctrl.charInfo.Reload();
            }
            catch (Exception e)
            {
                Console.WriteLine(String.Format("Exception in set_curcloth_coordinate, %s", e.ToString()));
            }
        }

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

        public object get_face_shape(int p1)
        {
            return this.charInfo.GetShapeFaceValue(p1);
        }

        public void set_face_shape(int p1, float p2)
        {
            this.charInfo.SetShapeFaceValue(p1, p2);
        }

        public float[] get_face_shapes_all()
        {
            return this.objctrl.oiCharInfo.charFile.custom.face.shapeValueFace;
        }

        public object get_face_shape_names()
        {
            return ChaFileDefine.cf_headshapename;
        }

        public int get_face_shapes_count()
        {
            return this.face_shapes_all.Length;
        }

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

        public static void char_tuya(Actor chara, ActorData param)
        {
            // param = skin tuya 0~1
            chara.set_tuya(param.tuya);
        }

        public static void char_wet(Actor chara, ActorData param)
        {
            // param = skin wet 0~1
            chara.set_wet(param.wetness);
        }

        public ActorData export_full_status()
        {
            // export a dict contains all actor status
            var fs = new Dictionary<string, object>
            {
            };
            fs["visible"] = this.visible;
            fs["move_to"] = this.pos;
            fs["rotate_to"] = this.rot;
            fs["scale_to"] = this.scale;
            fs["anim"] = this.get_animate();
            fs["anim_spd"] = this._anime_speed;
            fs["anim_ptn"] = this.get_animePattern();
            fs["anim_lp"] = this.get_anime_forceloop();
            fs["cloth_all"] = this.get_cloth();
            fs["acc_all"] = this.get_accessory();
            if (this.sex == 1)
            {
                fs["juice"] = this.get_juice();
                fs["nip_stand"] = this.get_nipple_stand();
            }
            fs["tear"] = this.get_tear();
            fs["face_red"] = this.get_facered();
            fs["skin_tuya"] = this.get_tuya();
            fs["skin_wet"] = this.get_wet();
            fs["simple"] = this.get_simple();
            fs["simple_color"] = this.get_simple_color();
            fs["son"] = this.get_son();
            fs["look_at"] = this.get_look_eye();
            fs["face_to"] = this.get_look_neck();
            fs["eyebrow"] = this.get_eyebrow_ptn();
            fs["eyes"] = this.get_eyes_ptn();
            fs["eyes_open"] = this.get_eyes_open();
            fs["eyes_blink"] = this.get_eyes_blink();
            fs["mouth"] = this.get_mouth_ptn();
            fs["mouth_open"] = this.get_mouth_open();
            fs["lip_sync"] = this.get_lip_sync();
            fs["hands"] = this.get_hand_ptn();
            fs["kinematic"] = this.get_kinematic();
            fs["fk_active"] = this.get_FK_active();
            fs["fk_set"] = this.export_fk_bone_info();
            fs["ik_active"] = this.get_IK_active();
            fs["ik_set"] = this.export_ik_target_info();
            fs["voice_lst"] = this.get_voice_lst();
            fs["voice_rpt"] = this.get_voice_repeat();
            // ext data, enable by ini setting
            if (Utils.is_ini_value_true("ExportChara_CurClothesCoord"))
            {
                fs["ext_curclothcoord"] = this.get_curcloth_coordinate();
            }
            if (Utils.is_ini_value_true("ExportChara_BodyShapes"))
            {
                fs["ext_bodyshapes"] = this.get_body_shapes_all();
            }
            if (Utils.is_ini_value_true("ExportChara_FaceShapes"))
            {
                fs["ext_faceshapes"] = this.get_face_shapes_all();
            }
            if (Utils.is_ini_value_true("ExportChara_AnimeAuxParam"))
            {
                fs["anim_optprm"] = this.get_anime_option_param();
            }
            // plugin data, enable by ini setting
            if (Utils.is_ini_value_true("ExportChara_AIPE"))
            {
                try
                {
                    fs["pl_aipedata"] = this.get_aipedata();
                }
                catch (Exception)
                {
                    Console.WriteLine("Error during get aipedata");
                }
            }
            return fs;
        }

        public object h_partner(int hType = 0, int hPosition = 0)
        {
            // return tuple of valid partner for current actor
            // 0: male, 1: female, -1: both
            return ValueTuple.Create(-1);
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
            if (this._anime_speed != partner._anime_speed)
            {
                partner.set_animeSpeed(this._anime_speed);
            }
            if (this.get_animePattern() != partner.get_animePattern())
            {
                partner.set_animePattern(this.get_animePattern());
            }
            if (this.get_anime_forceloop() != partner.get_anime_forceloop())
            {
                partner.set_anime_forceloop(this.get_anime_forceloop());
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
            var mss = mactor.get_son();
            if (!mss.visible && mactor.sex == 0)
            {
                mactor.set_son((true, mss.length));
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
                mactor.animate(2, validCategoryKey[hPosition], validNoKey[hStage]);
                factor.animate(1, validCategoryKey[hPosition], validNoKey[hStage]);
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
                mactor.animate(4, validCategoryKey[hPosition], validNoKey[hStage]);
                factor.animate(3, validCategoryKey[hPosition], validNoKey[hStage]);
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
                mactor.animate(6, validCategoryKey[hPosition], validNoKey[hStage]);
                factor.animate(5, validCategoryKey[hPosition], validNoKey[hStage]);
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
                mactor.animate(8, validCategoryKey[hPosition], validNoKey[hStage]);
                factor.animate(7, validCategoryKey[hPosition], validNoKey[hStage]);
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
                mactor.animate(9, validCategoryKey[hPosition], validNoKey[hStage]);
                factor.animate(9, validCategoryKey[hPosition] + 1, validNoKey[hStage]);
            }
            // auto adjust anime param
            Console.WriteLine(String.Format("factor(%s): height=%.2f breast=%.2f", factor.text_name, factor.height, factor.breast));
            var anime_option_param = (factor.height, factor.breast);
            if (factor.isHAnime)
            {
                factor.set_anime_option_param(anime_option_param);
            }
            if (mactor.isHAnime)
            {
                mactor.set_anime_option_param(anime_option_param);
            }
            foreach (var extActor in extActors)
            {
                if (extActor != null && extActor.isHAnime)
                {
                    extActor.set_anime_option_param(anime_option_param);
                }
            }
        }

        public static void char_tuya(Actor chara, ActorData param)
        {
            // param = skin tuya 0~1
            chara.tuya = param.tuya;
        }

        public static void char_wet(Actor chara, ActorData param)
        {
            // param = skin wet 0~1
            chara.set_wet(param.wetness);
        }

        public static (string, string, string, string, string) get_hanime_group_names()
        {
            var info = Info.Instance;
            var gcDic = info.dicAGroupCategory;
            return (gcDic[1].name, gcDic[3].name, gcDic[5].name, gcDic[7].name, gcDic[9].name);
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

        public static void char_juice(Actor chara, ActorData param)
        {
            // param = juice level on (face, FrontUp, BackUp, FrontDown, BackDown) where 0-none, 1-few, 2-lots, or just on int to set all
            chara.set_juice(param.juice);
        }

        public static void char_tear(Actor chara, ActorData param)
        {
            // param = tear level(0,1,2,3) or (0~1 for PH)
            chara.set_tear(param.tearLevel);
        }

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

        public static void char_all_accessories(Actor chara, ActorData param)
        {
            if (chara is Actor aiChara)
            {
                char_all_accessories(aiChara, param);
            }
        }

        public static void char_all_accessories(Actor chara, ActorData param)
        {
            // param = 0(hide all)/1(show all)
            // or
            // param = (accShow0, accShow1, ... accShow19)
            chara.set_accessory(param.showAllAccessories);
        }

        public static void char_neck_look_full2(Actor chara, ActorData param)
        {
            if (chara is Actor aiChara)
            {
                char_neck_look_full2(aiChara, param);
            }
            else
            {
                return;
            }
        }

        public static void char_neck_look_full2(Actor chara, ActorData param)
        {
            // param = array of bytes, use dump to get it
            try
            {
                chara.set_look_neck_full2(param.neck);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in setting char neck in Fixed state (2): " + e.ToString());
                Console.WriteLine("Sorry, we just pass it...");
            }
        }

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


    }
}
