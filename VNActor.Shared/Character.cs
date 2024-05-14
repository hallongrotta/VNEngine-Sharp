#if NEOV2
using AIChara;
#endif

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MessagePack;
using Studio;
using UnityEngine;

namespace VNActor
{
    // Shared Character code
    public partial class Character : NeoOCI, IVNObject<Character>
    {
        public delegate void CharaActFunction(Character chara, ActorData param);

        public enum EyeLookState
        {
            Front,
            Follow,
            Avert,
            Fixed,
            Target
        }

        public enum KinematicMode
        {
            None,
            FK,
            IK,
            IKFK
        }

        public enum NeckPattern
        {
            Front,
            Camera,
            Avert,
            Anime,
            Fixed
        }

        public static OIBoneInfo.BoneGroup[] BoneGroups =
        {
            OIBoneInfo.BoneGroup.Body, OIBoneInfo.BoneGroup.RightLeg, OIBoneInfo.BoneGroup.LeftLeg,
            OIBoneInfo.BoneGroup.RightArm, OIBoneInfo.BoneGroup.LeftArm
        };

        public new OCIChar objctrl;


        public Character(OCIChar objctrl) : base(objctrl)
        {
            this.objctrl = objctrl;
        }

        public bool[] Accessories
        {
            get
            {
                // return accessory state on/off in tuple(20)
                var accessories = new bool[objctrl.charFileStatus.showAccessory.Length];
                Array.Copy(objctrl.charFileStatus.showAccessory, accessories,
                    objctrl.charFileStatus.showAccessory.Length);
                return accessories;
            }
            set => Array.Copy(value, objctrl.charFileStatus.showAccessory, objctrl.charFileStatus.showAccessory.Length);
        }

        public ChaControl CharInfo => objctrl.charInfo;

        public OICharInfo OICharInfo => objctrl.oiCharInfo;

        public override Vector3 Position
        {
            get => CharInfo.transform.localPosition;
            set => CharInfo.transform.localPosition = value;
        }

        public override Vector3 Rotation
        {
            get => CharInfo.transform.localRotation.eulerAngles;
            set => CharInfo.transform.localRotation = Quaternion.Euler(value.x, value.y, value.z);
        }

        public Vector3 Scale
        {
            get => CharInfo.transform.localScale;
            set { }
        }

        public byte[] EyeAngles
        {
            get
            {
                if (Gaze == EyeLookState.Fixed)
                    using (var memoryStream = new MemoryStream())
                    {
                        using (var binaryWriter = new BinaryWriter(memoryStream))
                        {
                            CharInfo.eyeLookCtrl.eyeLookScript.SaveAngle(binaryWriter);
                            return memoryStream.ToArray();
                        }
                    }

                return null;
            }
            set
            {
                if (value != null)
                    if (Gaze == EyeLookState.Fixed)
                        using (var memoryStream = new MemoryStream(value))
                        {
                            using (var binaryReader = new BinaryReader(memoryStream))
                            {
                                LoadEyeAngle(binaryReader);
                            }
                        }
            }
        }

        public EyeLookState Gaze
        {
            get => (EyeLookState) objctrl.charInfo.GetLookEyesPtn();
            set => objctrl.ChangeLookEyesPtn((int) value);
        }

        public int MouthPattern
        {
            set =>
                // ptn: 0 to x (depend on engine)
                objctrl.charInfo.ChangeMouthPtn(value);
            get =>
                // return mouth pattern
                objctrl.charInfo.GetMouthPtn();
        }

        public float MouthOpenMax
        {
            get => objctrl.charInfo.GetMouthOpenMax();
            set => objctrl.charInfo.ChangeMouthOpenMax(value);
        }

        public int EyePattern
        {
            set => objctrl.charInfo.ChangeEyesPtn(value);
            get => objctrl.charInfo.GetEyesPtn();
        }

        public float EyesOpenMax
        {
            get => objctrl.charInfo.GetEyesOpenMax();
            set => objctrl.charInfo.ChangeEyesOpenMax(value);
        }


        public int EyebrowPattern
        {
            get => objctrl.charInfo.GetEyebrowPtn();
            set => objctrl.charInfo.ChangeEyebrowPtn(value);
        }

        public byte[] LookNeckFull2
        {
            get
            {
                if (LookNeckPattern != NeckPattern.Fixed)
                    return null;

                // needed only to save Fixed state
                using (var memoryStream = new MemoryStream())
                {
                    using (var binaryWriter = new BinaryWriter(memoryStream))
                    {
                        objctrl.neckLookCtrl.SaveNeckLookCtrl(binaryWriter);
                        return memoryStream.ToArray();
                    }
                }
            }
            set
            {
                if (LookNeckPattern != NeckPattern.Fixed)
                    return;
                // print lst
                // print arrstate
                using (var memoryStream = new MemoryStream(value))
                {
                    using (var binaryReader = new BinaryReader(memoryStream))
                    {
                        objctrl.neckLookCtrl.LoadNeckLookCtrl(binaryReader);
                    }
                }
            }
        }

        public bool[] JointCorrection
        {
            get
            {
                var jointInfo = this.OICharInfo.expression;
                var data = new bool[jointInfo.Length];
                jointInfo.CopyTo(data, 0);
                return data;
            }
            set
            {
                if (value is null) return;
                var i = 0;
                foreach (var jointEnabled in value)
                {
                    this.objctrl.EnableExpressionCategory(i, jointEnabled);
                    i++;
                }
            }
        }

        public int FaceShapesCount => FaceShapesAll.Length;

        public string[] FaceShapesNames => ChaFileDefine.cf_headshapename;

        public float[] FaceShapesAll
        {
            get
            {
                var ct = FaceShapesCount;
                var res = new float[ct];
                for (var i = 0; i < ct; i++) res[i] = get_face_shape(i);
                return res;
            }
            set
            {
                for (var i = 0; i < value.Length; i++) set_face_shape(i, value[i]);
            }
        }

        public float[] BodyShapesAll => objctrl.oiCharInfo.charFile.custom.body.shapeValueBody;

        public int BodyShapesCount => BodyShapesAll.Length;

        public string[] BodyShapesNames => ChaFileDefine.cf_bodyshapename;

        public float Height =>
            // get height:
            objctrl.oiCharInfo.charFile.custom.body.shapeValueBody[0];

        public byte[] Clothes
        {
            get
            {
                // return state index of (top, bottom, bra, shorts, grove, panst, sock, shoes) in tuple
                var cloth = new byte[objctrl.charFileStatus.clothesState.Length];

                for (var i = 0; i < objctrl.charFileStatus.clothesState.Length; i++)
                    cloth[i] = objctrl.charFileStatus.clothesState[i];

                return cloth;
            }
            set
            {
                for (var i = 0; i < value.Length; i++)
                    if (objctrl.charFileStatus.clothesState[i] != value[i])
                        objctrl.SetClothesState(i, value[i]);
            }
        }
        //private Dictionary<string, (ActorData, bool)> char_act_funcs;

        public int Sex =>
            // get sex: 0-male, 1-female
            objctrl.sex;

        public bool IsVoicePlay =>
            // get voice play status
            objctrl.voiceCtrl.isPlay;

        public bool IsAnimeOver =>
            // get if anime played once. 
            // note, anime may still playing because of it is looping anime or force-loop sets true
            // for those non-looping anime, it may be stopped after played once if force-loop not set.
            objctrl.charAnimeCtrl.normalizedTime >= 1;

        public bool IsHAnime =>
            // get isHAnime status, use by anime option param
            objctrl.isHAnime;

        public Animation_s Animation
        {
            get
            {
                // return (group, category, no) in tuple         
                if (AnimeSpeed == 0.0)
                    return new Animation_s
                    {
                        group = objctrl.oiCharInfo.animeInfo.group, category = objctrl.oiCharInfo.animeInfo.category,
                        no = objctrl.oiCharInfo.animeInfo.no, normalizedTime = objctrl.charAnimeCtrl.normalizedTime
                    };
                return new Animation_s
                {
                    group = objctrl.oiCharInfo.animeInfo.group, category = objctrl.oiCharInfo.animeInfo.category,
                    no = objctrl.oiCharInfo.animeInfo.no
                };
            }
        }

        public float AnimeSpeed
        {
            get => objctrl.animeSpeed;
            set =>
                // speed: 0~3
                objctrl.animeSpeed = value;
        }

        public float AnimePattern
        {
            get => objctrl.animePattern;
            set =>
                // pattern: 0~1
                objctrl.animePattern = value;
        }

        public AnimeOption_s AnimationOption
        {
            set
            {
                // option: (param1, param2)
                objctrl.animeOptionParam1 = value.height;
                objctrl.animeOptionParam2 = value.breast;
            }
            get =>
                // return anime option param
                new AnimeOption_s {height = objctrl.animeOptionParam1, breast = objctrl.animeOptionParam2};
        }

        public bool AnimationItemVisible
        {
            set =>
                // visible: true/false
                objctrl.optionItemCtrl.visible = value;
            get =>
                // return anime option visible
                objctrl.optionItemCtrl.visible;
        }

        public bool AnimationForceLoop
        {
            get => objctrl.charAnimeCtrl.isForceLoop;
            set =>
                // loop: 0(false)/1(true)
                objctrl.charAnimeCtrl.isForceLoop = value;
        }

        public float FaceRedness
        {
            get =>
                // return face red level
                objctrl.GetHohoAkaRate();
            set =>
                // level: face red level 0~1
                objctrl.SetHohoAkaRate(value);
        }

        public float NippleStand
        {
            get =>
                // return nipple stand level
                objctrl.oiCharInfo.nipple;
            set =>
                // level: nipple stand level 0~1
                objctrl.SetNipStand(value);
        }

        public Son_s Son
        {
            get =>
                new Son_s
                {
                    visible = objctrl.oiCharInfo.visibleSon,
                    length = objctrl.oiCharInfo.sonLength
                };
            set
            {
                objctrl.SetVisibleSon(value.visible);
                objctrl.SetSonLength(value.length);
            }
        }

        public bool Simple
        {
            get =>
                // return simple state, simple color) in tuple
                objctrl.oiCharInfo.visibleSimple;
            set
            {
                // simple = one color, for male only: 1(true)/0(false)
                if (Sex == 0) objctrl.SetVisibleSimple(value);
            }
        }

        public Color SimpleColor
        {
            set
            {
                // simple color, for male only
                if (Sex == 0) objctrl.SetSimpleColor(value);
            }
            get =>
                // get simple color
                objctrl.oiCharInfo.simpleColor;
        }

        /*
        public Vector3 get_look_eye()
        {
            // return look eye ptn or look eye pos when ptn == 4
            var ptn = this.look_eye_ptn;

            if (ptn is int)
            {
                return ptn;
            }
            else
            {
                return this.look_eye_pos;
            }
        }
        */


        public Vector3 GazeTarget
        {
            set => objctrl.lookAtInfo.target.localPosition = value;
            get => objctrl.lookAtInfo.target.localPosition;
        }

        public NeckPattern LookNeckPattern
        {
            set =>
                // ptn for CharaStudio: 0: front, 1: camera, 2: hide from camera, 3: by anime, 4: fix
                objctrl.ChangeLookNeckPtn((int) value);
            get =>
                // return neck look pattern: 0: front, 1: camera, 2: hide from camera, 3: by anime, 4: fix
                (NeckPattern) objctrl.charInfo.GetLookNeckPtn();
        }

        public float EyesOpenLevel
        {
            set =>
                // open: 0~1
                objctrl.ChangeEyesOpen(value);
            get =>
                // return eyes open
                objctrl.charInfo.fileStatus.eyesOpenMax;
        }

        public bool EyesBlink
        {
            set =>
                // flag: 0(false)/1(True)
                objctrl.ChangeBlink(value);
            get =>
                // return eyes blink flag
                objctrl.charInfo.GetEyesBlinkFlag();
        }

        public float MouthOpenLevel
        {
            set =>
                // open: 0~1
                objctrl.ChangeMouthOpen(value);
            get =>
                // return mouth open
                objctrl.oiCharInfo.mouthOpen;
        }

        public bool LipSync
        {
            set =>
                // flag: 0/1. 
                objctrl.ChangeLipSync(value);
            get =>
                // return lip sync status
                OICharInfo.lipSync;
        }

        public Hands_s HandPattern
        {
            set
            {
                // ptn: (left hand ptn, right hand ptn)
                objctrl.ChangeHandAnime(0, value.leftMotion);
                objctrl.ChangeHandAnime(1, value.rightMotion);
            }
            get =>
                // return (lptn, rptn) in tuple
                new Hands_s {leftMotion = objctrl.oiCharInfo.handPtn[0], rightMotion = objctrl.oiCharInfo.handPtn[1]};
        }

        public List<int[]> VoiceList
        {
            get
            {
                // return a tuple of current loaded voice: ((group, category, no), (group, category, no), ...)
                return objctrl.voiceCtrl.list.Select(x => { return new int[] { x.group, x.category, x.no }; }).ToList();
            }
        }


        public int VoiceRepeat
        {
            set
            {
                // set voice repeat: 0: no repeat, 1: repeat selected one, 2: repeat all
                if (value == 2)
                    objctrl.voiceCtrl.repeat = VoiceCtrl.Repeat.All;
                else if (value == 1)
                    objctrl.voiceCtrl.repeat = VoiceCtrl.Repeat.Select;
                else
                    objctrl.voiceCtrl.repeat = VoiceCtrl.Repeat.None;
            }
            get
            {
                switch (objctrl.voiceCtrl.repeat)
                {
                    // return the status of voice repeat setting: 0: no repeat, 1: repeat selected one, 2: repeat all
                    case VoiceCtrl.Repeat.All:
                        return 2;
                    case VoiceCtrl.Repeat.Select:
                        return 1;
                    default:
                        return 0;
                }
            }
        }

        public KinematicMode Kinematic
        {
            get
            {
                // return current kinematice mode: 0-none, 1-IK, 2-FK, 3-IK&FK
                switch (objctrl.oiCharInfo.enableIK)
                {
                    case true when objctrl.oiCharInfo.enableFK:
                        return KinematicMode.IKFK;
                    case true:
                        return KinematicMode.IK;
                    default:
                    {
                        return objctrl.oiCharInfo.enableFK ? KinematicMode.FK : KinematicMode.None;
                    }
                }
            }
            set
            {
                switch (value)
                {
                    // mode: 0-none, 1-IK, 2-FK, 3-IK&FK
                    case KinematicMode.IKFK:
                    {
                        EnableIkFk();
                        break;
                    }
                    case KinematicMode.FK:
                    {
                        EnableFKAndDisableIK();
                        break;
                    }
                    case KinematicMode.IK:
                    {
                        EnableIKAndDisableFK();
                        break;
                    }
                    case KinematicMode.None:
                    default:
                    {
                        IKEnabled = false;
                        FKEnabled = false;
                        break;
                    }
                }
            }
        }

        public Dictionary<int, Vector3> FK
        {
            set
            {
                if (value is null) return;
                foreach (var boneInfo in objctrl.listBones.Where(boneInfo =>
                             value.ContainsKey(boneInfo.boneID) && boneInfo.active))
                    boneInfo.boneInfo.changeAmount.rot = value[boneInfo.boneID];
            }

            get
            {
                var activeBoneInfo = objctrl.listBones.Where(boneInfo => boneInfo.active);
                Dictionary<int, Vector3> fkBoneInfo = activeBoneInfo.ToDictionary(boneInfo => boneInfo.boneID,
                                       boneInfo =>
                                       {
                                        var rot = boneInfo.boneInfo.changeAmount.rot;
                                        return new Vector3(rot.x, rot.y, rot.z);
                                        });
                return fkBoneInfo;
            }
        }

        private IK_node_s makeIKNode(OCIChar.IKInfo itInfo)
        {
            var tgtName = itInfo.boneObject.name;
            var pos = itInfo.targetInfo.changeAmount.pos;
            var posClone = new Vector3(pos.x, pos.y, pos.z);
            if (IsRotatableIK(tgtName))
            {
                var rot = itInfo.targetInfo.changeAmount.rot;
                var rotClone = new Vector3(rot.x, rot.y, rot.z);
                //var rotClone = new Vector3(NormalizeAngle(rot.x), NormalizeAngle(rot.y), NormalizeAngle(rot.z));
                return new IK_node_s { pos = posClone, rot = rotClone };
            }
            else
            {
                return new IK_node_s { pos = posClone, rot = null };
            }
        }

        public Dictionary<string, IK_node_s> IK
        {
            set
            {
                if (value is null) return;
                var validTargets = objctrl.listIKTarget.Where(ikTgt => ikTgt.active && value.ContainsKey(ikTgt.boneObject.name));
                foreach (var ikTgt in validTargets)
                {
                    var nodeName = ikTgt.boneObject.name;
                    ikTgt.targetInfo.changeAmount.pos = value[nodeName].pos;
                    if (IsRotatableIK(nodeName) && value[nodeName].rot is Vector3 ikRot)
                        ikTgt.targetInfo.changeAmount.rot = ikRot;
                }
            }
            get
            {
                var activeInfo = objctrl.listIKTarget.Where(itInfo => itInfo.active);
                Dictionary<string, IK_node_s> ik = activeInfo.ToDictionary(itInfo => itInfo.boneObject.name, makeIKNode);
                return ik;
            }
        }

        public void import_status(IDataClass<Character> tmp_status)
        {
            if (tmp_status is ActorData data) import_status(data);
        }

        public IDataClass<Character> export_full_status()
        {
            return new ActorData(this);
        }

        public static Character add_female(string path)
        {
            var objctrl = AddObjectFemale.Add(path);
            return new Character(objctrl);
        }

        public static Character add_male(string path)
        {
            var objctrl = AddObjectMale.Add(path);
            return new Character(objctrl);
        }

        public void SetAccessory(int accIndex, bool accShow)
        {
            objctrl.ShowAccessory(accIndex, accShow);
        }

        public void reset_fk_bone_info()
        {
            // import fk bone info from dic
            foreach (var binfo in objctrl.listBones) binfo.boneInfo.changeAmount.rot = Vector3.zero;
        }

        private float NormalizeAngle(float angle)
        {
            return angle <= 180 ? angle : angle - 360;
        }

        public void EnableIkFk()
        {
            // enable IK
            objctrl.finalIK.enabled = true;
            objctrl.oiCharInfo.enableIK = true;
            objctrl.ActiveIK(OIBoneInfo.BoneGroup.Body, objctrl.oiCharInfo.activeIK[0], true);
            objctrl.ActiveIK(OIBoneInfo.BoneGroup.RightLeg, objctrl.oiCharInfo.activeIK[1], true);
            objctrl.ActiveIK(OIBoneInfo.BoneGroup.LeftLeg, objctrl.oiCharInfo.activeIK[2], true);
            objctrl.ActiveIK(OIBoneInfo.BoneGroup.RightArm, objctrl.oiCharInfo.activeIK[3], true);
            objctrl.ActiveIK(OIBoneInfo.BoneGroup.LeftArm, objctrl.oiCharInfo.activeIK[4], true);
            // enable FK, disable "body" because it should be controlled by IK
            objctrl.oiCharInfo.activeFK[3] = false;
            objctrl.fkCtrl.enabled = true;
            objctrl.oiCharInfo.enableFK = true;
            foreach (var i in Enumerable.Range(0, FKCtrl.parts.Length))
                objctrl.ActiveFK(FKCtrl.parts[i], objctrl.oiCharInfo.activeFK[i], true);

            // call ActiveKinematicMode to set pvCopy?
            objctrl.ActiveKinematicMode(OICharInfo.KinematicMode.IK, true, false);
        }

        public void EnableIKAndDisableFK()
        {

            FKEnabled = false;
            IKEnabled = true;

        }

        public void EnableFKAndDisableIK()
        {
            IKEnabled = false;
            FKEnabled = true;

        }


        public bool IKEnabled
        {
            set
            {
                if (IKEnabled == value) return;
                objctrl.ActiveKinematicMode(OICharInfo.KinematicMode.IK, value, false);
            }
            get
            {
                return objctrl.oiCharInfo.enableIK;
            }
        }

        public bool FKEnabled
        {
            set
            {
                if (FKEnabled == value) return;
                try
                {
                    objctrl.ActiveKinematicMode(OICharInfo.KinematicMode.FK, value, false);
                }
                    
                catch
                {
                    return;
                }
            }
            get
            {
                return objctrl.oiCharInfo.enableFK;
            }
        }


        public float get_face_shape(int p1)
        {
            return CharInfo.GetShapeFaceValue(p1);
        }

        public void set_face_shape(int p1, float p2)
        {
            CharInfo.SetShapeFaceValue(p1, p2);
        }

        public void import_status(ActorData a)
        {
            a.Apply(this);
        }

        /*
        public void move(Vector3 pos, Vector3 rot, Vector3 scale)
        {
            //if not self.stChara: return False
            if (pos != null)
            {
                this.charInfo.transform.localPosition = pos;
            }
            if (rot != null)
            {
                this.charInfo.transform.localRotation = Quaternion.Euler(rot);
                this.charInfo.transform.localRotation = Quaternion.Euler(rot);
            }
            if (scale != null)
            {
                this.charInfo.transform.localScale = scale;
            }
        } */

        /*
        public void animate(
            int group,
            int category,
            int no,
            float animePattern,
            float animeSpeed)
        {
            //print "1"
            try
            {
                this.oiCharInfo.animePattern = animePattern;
            }
            catch (Exception)
            {
                // passing for PlayHome Studio - it has another pattern
            }
            this.objctrl.LoadAnime(group, category, no, animeSpeed);
            //print "3"
            this.objctrl.animeSpeed = animeSpeed;
            //self.charInfo.chaBody.animBody.speed = animeSpeed
            //print "4"
            //self.charInfo.chaBody.animBody.speed = animeSpeed
            //print "5"
            //self.objctrl.RestartAnime()
            //print "6"
        }

        public void animate2(int group, int category, int no, float animeSpeed)
        {
            this.objctrl.LoadAnime(group, category, no);
            this.objctrl.animeSpeed = animeSpeed;
        }
        */

        public void restart_anime()
        {
            objctrl.RestartAnime();
        }

        //def animate2(self,category,group,no,animePattern,animeSpeed):
        // debug purpose - load info from dictionary
        public void animate_some_info(
            int group,
            int category,
            int no,
            float animePattern,
            float animeSpeed)
        {
            //from Studio import Info
            //Info.Instance.LoadExcelData()
            Console.WriteLine("21");
            //print str(Info.Instance.dicFemaleAnimeLoadInfo)
            var dic0 = Info.Instance.dicAnimeLoadInfo;
            foreach (var key in dic0.Keys) Console.WriteLine(key);
            var d1 = dic0[category];
            Console.WriteLine(d1.ToString());
            var d2 = d1[group];
            Console.WriteLine(d2.ToString());
            var animeLoadInfo = d2[no];
            Console.WriteLine(animeLoadInfo.ToString());
            Console.WriteLine("%s %s %s", animeLoadInfo.bundlePath, animeLoadInfo.fileName, animeLoadInfo.clip);
        }

        public void female_all_clothes_state(int state)
        {
            objctrl.SetClothesStateAll(state);
        }

        public void dump_obj()
        {
            //print "objctrlchar.move(pos=%s, rot=%s)"%(str(self.charInfo.GetPosition()), str(self.charInfo.GetRotation()))
            try
            {
                Console.WriteLine("objctrlchar.move(pos=%s, rot=%s, scale=%s)",
                    CharInfo.transform.localPosition.ToString(),
                    CharInfo.transform.localRotation.eulerAngles.ToString(), CharInfo.transform.localScale.ToString());
                Console.WriteLine("objctrlchar.animate(%s, %s, %s, %s, %s)", OICharInfo.animeInfo.group.ToString(),
                    OICharInfo.animeInfo.category.ToString(), OICharInfo.animeInfo.no.ToString(),
                    OICharInfo.animePattern.ToString(), OICharInfo.animeSpeed.ToString());
                //print "objctrlchar.tears_level = %s" % (str(self.tears_level))
            }
            catch (Exception e)
            {
                Console.WriteLine("# oops, error happened %s", e);
            }
        }

        /* TODO
        public object animeLength
        {
            get
            {
                // get current anime length in (second, frame, fps) format
                var cis = this.objctrl.charAnimeCtrl.animator.GetCurrentAnimatorClipInfo(0);
                var clip = cis[0].clip;
                return (clip.length, Convert.ToInt32(Math.Round(clip.length * clip.frameRate)), clip.frameRate);
            }
        }

        */

        /*
        new public Vector3 pos
        {
            get
            {
                return this.objctrl.oiCharInfo.changeAmount.pos;
            }
            set
            {
                this.objctrl.oiCharInfo.changeAmount.pos = pos;
            }
        }

        new public Vector3 rot
        {
            get
            {
                return this.objctrl.oiCharInfo.changeAmount.rot;
            }
            set
            {
                this.objctrl.oiCharInfo.changeAmount.rot = rot;
            }
        }

        new public Vector3 scale
        {
            get
            {
                return this.objctrl.oiCharInfo.changeAmount.scale;
            }
            set
            {
                this.objctrl.oiCharInfo.changeAmount.scale = value;
            }
        }
        */

        public void move(Vector3 pos, Vector3 rot, Vector3 scale)
        {
            if (pos != null) objctrl.oiCharInfo.changeAmount.pos = pos;
            if (rot != null) objctrl.oiCharInfo.changeAmount.rot = rot;
            if (scale != null) objctrl.oiCharInfo.changeAmount.scale = scale;
        }

        public void SetAnimate(
            int group,
            int category,
            int no,
            float normalizedTime = -1f,
            bool force = false)
        {
            // group, category, no: key to anime clip, dump from scene please
            // normalizedTime: start time? 0~1?
            // force reload: true/false
            // check if no normalized time passed to function
            var noNormalizedTime = normalizedTime == -1;
            if (noNormalizedTime) normalizedTime = 0;
            var curAnime = Animation;
            if (force || curAnime.group != group || curAnime.category != category || curAnime.no != no ||
                AnimeSpeed == 0.0 && !noNormalizedTime && objctrl.charAnimeCtrl.normalizedTime != normalizedTime)
                objctrl.LoadAnime(group, category, no, normalizedTime);
        }

        public void set_look_eye(EyeLookState ptn_dir, Vector3 dir)
        {
            Gaze = ptn_dir;
            if (ptn_dir == EyeLookState.Target) GazeTarget = dir;
        }

        public void set_look_eye(Vector3 dir)
        {
            // ptn_dir: 0: front, 1: camera, 2: hide from camera, 3: fix, 4: operate, or use Vector3 or tuple(3) to set a direction
            // when ptn_dir is Vector3 or (x, y, z), +x look right, -x look left, +y look up, -y look down
            objctrl.ChangeLookEyesPtn(4);
            objctrl.lookAtInfo.target.localPosition = dir;
        }

        public void add_voice(int group, int category, int no)
        {
            // group, category, no: index of voice
            // refer to objctrl.charInfo.charFile.parameter.personality? aggressive? diligence?
            objctrl.AddVoice(group, category, no);
        }

        public void del_voice(int index)
        {
            // delete voice by index
            objctrl.DeleteVoice(index);
        }

        public void del_all_voice()
        {
            // stop and delete all voice
            if (IsVoicePlay) stop_voice();
            objctrl.DeleteAllVoice();
        }

        public void set_voice_lst(List<int[]> voiceList, bool autoplay = true)
        {
            // set a list of voice, load and play it
            // voiceList: tuple of voice: ((group, category, no), (group, category, no), ...)
            // autoplay: play voice
            del_all_voice();
            foreach (var v in voiceList) add_voice(v[0], v[1], v[2]);
            if (autoplay) play_voice();
        }

        public void play_voice(int index = 0)
        {
            // index = which voice to play
            if (IsVoicePlay) stop_voice();
            objctrl.PlayVoice(index);
        }

        public void stop_voice()
        {
            // stop play voice
            objctrl.StopVoice();
        }

        public void SetActiveFK(int group, bool active = false, bool force = false)
        {
            OIBoneInfo.BoneGroup[] bis =
            {
                OIBoneInfo.BoneGroup.Hair, OIBoneInfo.BoneGroup.Neck, OIBoneInfo.BoneGroup.Breast,
                OIBoneInfo.BoneGroup.Body, OIBoneInfo.BoneGroup.RightHand, OIBoneInfo.BoneGroup.LeftHand,
                OIBoneInfo.BoneGroup.Skirt
            };

            objctrl.ActiveFK(bis[group], active, force);
        }

        public void SetActiveFK(bool group, bool force = false)
        {
            // param pattern 1: set one group
            // group: FK group: 0=hair, 1=neck, 2=Breast, 3=body, 4=right hand, 5=left hand, 6=skirt
            // active: 0/1
            // force: 0/1
            // param pattern 2: set all group
            // group: FK group 0/1 in tuple (hair, neck, Breast, body, right hand, left hand, skirt)
            // active: must be None
            // force: 0/1
            // param pattern 3: set all group to same state
            // group: 0/1 for all FK group
            // active: must be None
            // force: 0/1
            OIBoneInfo.BoneGroup[] bis =
            {
                OIBoneInfo.BoneGroup.Hair, OIBoneInfo.BoneGroup.Neck, OIBoneInfo.BoneGroup.Breast,
                OIBoneInfo.BoneGroup.Body, OIBoneInfo.BoneGroup.RightHand, OIBoneInfo.BoneGroup.LeftHand,
                OIBoneInfo.BoneGroup.Skirt
            };

            foreach (var i in Enumerable.Range(0, 7)) objctrl.ActiveFK(bis[i], group, force);
        }

        public void SetActiveFK(bool[] group, bool active = false, bool force = false)
        {
            OIBoneInfo.BoneGroup[] bis =
            {
                OIBoneInfo.BoneGroup.Hair, OIBoneInfo.BoneGroup.Neck, OIBoneInfo.BoneGroup.Breast,
                OIBoneInfo.BoneGroup.Body, OIBoneInfo.BoneGroup.RightHand, OIBoneInfo.BoneGroup.LeftHand,
                OIBoneInfo.BoneGroup.Skirt
            };

            for (var i = 0; i < bis.Length; i++)
                try
                {
                    objctrl.ActiveFK(bis[i], group[i], force);
                } catch (NullReferenceException)
                {
                    Console.WriteLine("SetActiveFK: failed for {0}, {1}, {2}", i, bis[i], group[i]);
                }
               
        }

        public bool[] get_FK_active()
        {
            // return active status for FK part, (hair, neck, Breast, body, right hand, left hand, skirt) in tuple
            var activeFK = new bool[objctrl.oiCharInfo.activeFK.Length];
            Array.Copy(objctrl.oiCharInfo.activeFK, activeFK, objctrl.oiCharInfo.activeFK.Length);
            return activeFK;
        }

        public void SetActiveIK(bool[] group, bool force = false)
        {
            // param pattern 1: set one group
            // group: IK group: 0=body, 1=right leg, 2=left leg, 3=right arm, 4=left arm
            // active: 0/1
            // force: 0/1
            // param pattern 2: set all group
            // group: IK group 0/1 in tuple (body, right leg, left leg, right arm, left arm)
            // active: must be None
            // force: 0/1
            // param pattern 3: set all group to same state
            // group: 0/1 for all IK group
            // active: must be None
            // force: 0/1
            foreach (var i in Enumerable.Range(0, group.Length <= 5 ? group.Length : 5))
                objctrl.ActiveIK(BoneGroups[i], group[i], force);
        }

        public void SetActiveIK(int group, bool active = false, bool force = false)
        {
            objctrl.ActiveIK(BoneGroups[group], active, force);
        }

        public void SetActiveIK(bool group, bool force = false)
        {
            // param pattern 1: set one group
            // group: IK group: 0=body, 1=right leg, 2=left leg, 3=right arm, 4=left arm
            // active: 0/1
            // force: 0/1
            // param pattern 2: set all group
            // group: IK group 0/1 in tuple (body, right leg, left leg, right arm, left arm)
            // active: must be None
            // force: 0/1
            // param pattern 3: set all group to same state
            // group: 0/1 for all IK group
            // active: must be None
            // force: 0/1
            OIBoneInfo.BoneGroup[] bis =
            {
                OIBoneInfo.BoneGroup.Body, OIBoneInfo.BoneGroup.RightLeg, OIBoneInfo.BoneGroup.LeftLeg,
                OIBoneInfo.BoneGroup.RightArm, OIBoneInfo.BoneGroup.LeftArm
            };

            foreach (var i in Enumerable.Range(0, 5)) objctrl.ActiveIK(bis[i], group, force);
        }

        public bool[] get_IK_active()
        {
            // return active status for IK part, (body, right leg, left leg, right arm, left arm) in tuple
            var IKActive = new bool[objctrl.oiCharInfo.activeIK.Length];
            Array.Copy(objctrl.oiCharInfo.activeIK, IKActive, objctrl.oiCharInfo.activeIK.Length);
            return IKActive;
        }

        /*

        public Dictionary<string, object> export_ik_target_info(bool activedOnly = true)
        {
            // export a dic contents IK target info
            var itDic = new Dictionary<string, object>();
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
                        //rotClone = Vector3(rot.x if rot.x <= 180 else rot.x - 360, rot.y if rot.y <= 180 else rot.y - 360, rot.z if rot.z <= 180 else rot.z - 360)
                        itDic[tgtName] = (posClone, rotClone);
                    }
                    else
                    {
                        itDic[tgtName] = ValueTuple.Create(posClone);
                    }
                }
            }
            //print "exported", len(itDic), "IK Targets"
            return itDic;
        }
        */

        /*
        public void import_ik_target_info(Dictionary<string, IK_node_s> itDic)
        {
            // import IK target info from dic 
            foreach (OCIChar.IKInfo ikTgt in this.objctrl.listIKTarget)
            {
                string ikTgName = ikTgt.boneObject.name;
                if (itDic.Keys.Contains(ikTgName))
                {

                    ikTgt.targetInfo.changeAmount.pos = itDic[ikTgName].Item1;

                    if ((ikTgName.Contains("_Hand_") || ikTgName.Contains("_Foot01_")))
                    {

                        ikTgt.targetInfo.changeAmount.rot = itDic[ikTgName].Item2;
                    }
                }
            }
        }
        */
        /*
        public void import_ik_target_info(Dictionary<string, ((float, float, float), (float, float, float))> itDic)
        {
            // import IK target info from dic 
            foreach (OCIChar.IKInfo ikTgt in this.objctrl.listIKTarget)
            {
                string ikTgName = ikTgt.boneObject.name;
                if (itDic.Keys.Contains(ikTgName))
                {

                    ikTgt.targetInfo.changeAmount.pos = new Vector3(itDic[ikTgName].Item1.Item1, itDic[ikTgName].Item1.Item2, itDic[ikTgName].Item1.Item3);

                    if ((ikTgName.Contains("_Hand_") || ikTgName.Contains("_Foot01_")))
                    {

                        ikTgt.targetInfo.changeAmount.rot = new Vector3(itDic[ikTgName].Item2.Item1, itDic[ikTgName].Item2.Item2, itDic[ikTgName].Item2.Item3);
                    }
                }
            }
        }
        */

        /*
        public void import_status(IDataClass status)
        {
            foreach (string f in status.Keys)
            {
                if (char_act_funcs.ContainsKey(f))
                {
                    CharaActFunction func = char_act_funcs[f].Item1;
                    func(this, status[f]);
                }
                else
                {
                    Console.WriteLine(String.Format("act error: unknown function '%s' for actor!", f));
                }
            }
        }
        */

        public void import_status_diff_optimized<T>(Dictionary<string, IDataClass<T>> status)
        {
        }

        /*
        public void import_status_diff_optimized(Dictionary<string, ActorData> status)
        {
            var ofs = this.export_full_status();
            var dfs = new Dictionary<string, ActorData>
            {
            };
            foreach (var key in status.Keys)
            {
                if (!ofs.Keys.Contains(key) || ofs[key] != status[key])
                {
                    dfs[key] = status[key];
                }
            }
            //return dfs
            //print "Optimized import status diff, ", dfs
            this.import_status(dfs);
        }

        public Dictionary<string, object> export_diff_status(Dictionary<string, object> diff)
        {
            // export a dict contains diff from another status
            var ofs = this.export_full_status();
            var dfs = new Dictionary<string, object>
            {
            };
            foreach (var key in ofs.Keys)
            {
                if (!diff.Keys.Contains(key) || ofs[key] != diff[key])
                {
                    dfs[key] = ofs[key];
                }
            }
            return dfs;
        }
        */

        public void load_clothes_file(string file)
        {
            // load a clothes file
            objctrl.LoadClothesFile(file);
        }

        // body sliders
        // see sceneutils.py for realization of this props
        public float get_body_shape(int p1)
        {
            return CharInfo.GetShapeBodyValue(p1);
        }

        public void set_body_shape(int p1, float p2)
        {
            CharInfo.SetShapeBodyValue(p1, p2);
        }


        public void set_body_shapes_all(List<float> values)
        {
            foreach (var i in Enumerable.Range(0, values.Count)) set_body_shape(i, values[i]);
        }
        /*
get
{
    int ct = this.body_shapes_count;
    var res = new List<float>();
    foreach (int i in Enumerable.Range(0, ct))
    {
        res.Add(this.get_body_shape(i));
    }
    return res;
}
}
*/

        public void setCloth(int clothIndex)
        {
            objctrl.SetClothesStateAll(clothIndex);
        }

        public void setCloth(int clothIndex, byte clothState)
        {
            // param format 1: set one cloth
            // clothIndex: 0-top, 1-bottom, 2-bra, 3-shorts, 4-grove!!, 5-panst!!, 6-sock, 7-shoes
            // clothState: 0-put on, 1-half off 1, 2--off 
            // param format 2: set all clothes, like the return value of get_cloth()
            // clothIndex: state for (top, bottom, bra, shorts, grove, panst, sock, shoes) in tuple
            // clothState: must be None
            // param format 3: set all cloth to same state
            // clothIndex: state of all clothes
            // clothState: must be None
            objctrl.SetClothesState(clothIndex, clothState);
        }

        [Serializable]
        [MessagePackObject]
        public struct Hands_s
        {
            [Key(0)] public int leftMotion;
            [Key(1)] public int rightMotion;
        }

        [Serializable]
        [MessagePackObject]
        public struct Son_s
        {
            [Key(0)] public bool visible;
            [Key(1)] public float length;
        }

        [Serializable]
        [MessagePackObject]
        public struct Animation_s
        {
            [Key(0)] public int group;
            [Key(1)] public int category;
            [Key(2)] public int no;
            [Key(3)] public float? normalizedTime;
        }

        public struct IK_node_s
        {
            public Vector3 pos;
            public Vector3? rot;
        }

        public struct AnimeOption_s
        {
            public float height;
            public float breast;
        }


        // face sliders
    }
}