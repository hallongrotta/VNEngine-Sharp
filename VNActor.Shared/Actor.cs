using Studio;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace VNActor
{

    public partial class Actor : HSNeoOCIChar, IVNObject
    {
        public struct AnimeOption_s
        {
            public float height;
            public float breast;
        }

        public delegate void CharaActFunction(Actor chara, ActorData param);
        //private Dictionary<string, (ActorData, bool)> char_act_funcs;

        public Actor(OCIChar objctrl) : base(objctrl) { }

        public int sex
        {
            get
            {
                // get sex: 0-male, 1-female
                return this.objctrl.sex;
            }
        }

        // value: 0(hide)/1(visible)
        public bool visible
        {
            get
            {
                // get visible status
                return this.objctrl.treeNodeObject.visible;
            }
            set
            {
                this.objctrl.treeNodeObject.visible = value;
            }
        }

        public bool isVoicePlay
        {
            get
            {
                // get voice play status
                return this.objctrl.voiceCtrl.isPlay;
            }
        }

        public bool isAnimeOver
        {
            get
            {
                // get if anime played once. 
                // note, anime may still playing because of it is looping anime or force-loop sets true
                // for those non-looping anime, it may be stopped after played once if force-loop not set.
                return this.objctrl.charAnimeCtrl.normalizedTime >= 1;
            }
        }

        public bool isHAnime
        {
            get
            {
                // get isHAnime status, use by anime option param
                return this.objctrl.isHAnime;
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
            if (pos != null)
            {
                this.objctrl.oiCharInfo.changeAmount.pos = pos;
            }
            if (rot != null)
            {
                this.objctrl.oiCharInfo.changeAmount.rot = rot;
            }
            if (scale != null)
            {
                this.objctrl.oiCharInfo.changeAmount.scale = scale;
            }
        }

        public void setAnimate(
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
            if (noNormalizedTime)
            {
                normalizedTime = 0;
            }
            var curAnime = this.animate;
            if (force || curAnime.group != group || curAnime.category != category || curAnime.no != no || this.animeSpeed == 0.0 && !noNormalizedTime && this.objctrl.charAnimeCtrl.normalizedTime != normalizedTime)
            {
                this.objctrl.LoadAnime(group, category, no, normalizedTime);
            }
        }

        public Animation_s animate
        {
            get
            {
                // return (group, category, no) in tuple         
                if (this.animeSpeed == 0.0)
                {
                    return new Animation_s { group = this.objctrl.oiCharInfo.animeInfo.group, category = this.objctrl.oiCharInfo.animeInfo.category, no = this.objctrl.oiCharInfo.animeInfo.no, normalizedTime = this.objctrl.charAnimeCtrl.normalizedTime };
                }
                else
                {
                    return new Animation_s { group = this.objctrl.oiCharInfo.animeInfo.group, category = this.objctrl.oiCharInfo.animeInfo.category, no = this.objctrl.oiCharInfo.animeInfo.no};
                }
            }
        }

        public float animeSpeed
        {
            get
            {
                return this.objctrl.animeSpeed;
            }
            set
            {
                // speed: 0~3
                this.objctrl.animeSpeed = value;
            }
        }

        public float animePattern
        {
            get
            {

                return this.objctrl.animePattern;
            }
            set
            {
                // pattern: 0~1
                this.objctrl.animePattern = value;
            }
        }

        public AnimeOption_s anime_option_param
        {
            set
            {
                // option: (param1, param2)
                this.objctrl.animeOptionParam1 = value.height;
                this.objctrl.animeOptionParam2 = value.breast;
            }
            get
            {
                // return anime option param
                return new AnimeOption_s { height = this.objctrl.animeOptionParam1, breast = this.objctrl.animeOptionParam2 };
            }
        }

        public bool anime_option_visible
        {
            set
            {
                // visible: true/false
                this.objctrl.optionItemCtrl.visible = value;
            }
            get
            {
                // return anime option visible
                return this.objctrl.optionItemCtrl.visible;
            }
        }

        public bool anime_forceloop
        {
            get
            {
                return this.objctrl.charAnimeCtrl.isForceLoop;
            }
            set
            {
                // loop: 0(false)/1(true)
                this.objctrl.charAnimeCtrl.isForceLoop = value;
            }
        }
        public float facered
        {
            get
            {
                // return face red level
                return this.objctrl.GetHohoAkaRate();
            }
            set
            {
                // level: face red level 0~1
                this.objctrl.SetHohoAkaRate(value);
            }
        }

        public float nipple_stand
        {
            get
            {
                // return nipple stand level
                return this.objctrl.oiCharInfo.nipple;
            }
            set
            {
                // level: nipple stand level 0~1
                this.objctrl.SetNipStand(value);
            }
        }

        public Son_s son
        {
            get
            {
                return new Son_s {visible = this.objctrl.oiCharInfo.visibleSon,
                length = this.objctrl.oiCharInfo.sonLength };
            }
            set
            {
                this.objctrl.SetVisibleSon(value.visible);
                this.objctrl.SetSonLength(value.length);
            }
        }

        /*
        public bool son
        {
            get
            {
                // return son visible
                return this.objctrl.oiCharInfo.visibleSon;
            }
            set
            {
                // son visible: 0(False)/1(True)
                if (this.sex == 0)
                {
                    this.objctrl.SetVisibleSon(value);
                }
            }
        }
        */

        public bool simple
        {
            get
            {
                // return simple state, simple color) in tuple
                return this.objctrl.oiCharInfo.visibleSimple;
            }
            set
            {
                // simple = one color, for male only: 1(true)/0(false)
                if (this.sex == 0)
                {
                    this.objctrl.SetVisibleSimple(value);
                }
            }
        }

        public Color simple_color
        {
            set
            {
                // simple color, for male only
                if (this.sex == 0)
                {
                    this.objctrl.SetSimpleColor(value);
                }
            }
            get
            {
                // get simple color
                return this.objctrl.oiCharInfo.simpleColor;
            }
        }

        public void set_look_eye(int ptn_dir, Vector3 dir)
        {
            this.look_eyes_ptn = ptn_dir;
            if (ptn_dir == 4)
            {
                this.look_eye_pos = dir;
            }
        }

        public void set_look_eye(Vector3 dir)
        {
            // ptn_dir: 0: front, 1: camera, 2: hide from camera, 3: fix, 4: operate, or use Vector3 or tuple(3) to set a direction
            // when ptn_dir is Vector3 or (x, y, z), +x look right, -x look left, +y look up, -y look down
            this.objctrl.ChangeLookEyesPtn(4);
            this.objctrl.lookAtInfo.target.localPosition = dir;
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

        public int look_eye_ptn
        {
            get
            {
                // return eye look at pattern: 0: front, 1: camera, 2: hide from camera, 3: fix, 4: operate
                return this.objctrl.charInfo.GetLookEyesPtn();
            }
            set
            {
                // eye look at pattern: 0: front, 1: camera, 2: hide from camera, 3: fix, 4: operate
                this.objctrl.ChangeLookEyesPtn(value);
            }
        }

        public Vector3 look_eye_pos
        {
            set
            {
                this.objctrl.lookAtInfo.target.localPosition = value;
            }
            get
            {
                return this.objctrl.lookAtInfo.target.localPosition;
            }
        }

        public int look_neck
        {
            set
            {
                // ptn for CharaStudio: 0: front, 1: camera, 2: hide from camera, 3: by anime, 4: fix
                // ptn for PHStudio: 0: front, 1: camera, 2: by anime, 3: fix
                this.objctrl.ChangeLookNeckPtn(value);
            }
            get
            {
                // return neck look pattern: 0: front, 1: camera, 2: hide from camera, 3: by anime, 4: fix
                return this.objctrl.charInfo.GetLookNeckPtn();
            }
        }

        public float eyes_open
        {
            set
            {
                // open: 0~1
                this.objctrl.ChangeEyesOpen(value);
            }
            get
            {
                // return eyes open
                return this.objctrl.charInfo.fileStatus.eyesOpenMax;
            }
        }

        public bool eyes_blink
        {
            set
            {
                // flag: 0(false)/1(True)
                this.objctrl.ChangeBlink(value);
            }
            get
            {
                // return eyes blink flag
                return this.objctrl.charInfo.GetEyesBlinkFlag();
            }
        }

        public float mouth_open
        {
            set
            {
                // open: 0~1
                this.objctrl.ChangeMouthOpen(value);
            }
            get
            {
                // return mouth open
                return this.objctrl.oiCharInfo.mouthOpen;
            }
        }

        public bool lip_sync
        {
            set
            {
                // flag: 0/1. 
                // this is the lip sync option for voice play, not for VNGameEngine
                this.objctrl.ChangeLipSync(value);
            }
            get
            {
                // return lip sync status
                return this.oiCharInfo.lipSync;
            }
        }

        public Hands_s hand_ptn
        {
            set
            {
                // ptn: (left hand ptn, right hand ptn)
                this.objctrl.ChangeHandAnime(0, value.leftMotion);
                this.objctrl.ChangeHandAnime(1, value.rightMotion);
            }
            get
            {
                // return (lptn, rptn) in tuple
                return new Hands_s { leftMotion = this.objctrl.oiCharInfo.handPtn[0], rightMotion = this.objctrl.oiCharInfo.handPtn[1] };
            }
        }

        public void add_voice(int group, int category, int no)
        {
            // group, category, no: index of voice
            // refer to objctrl.charInfo.charFile.parameter.personality? aggressive? diligence?
            this.objctrl.AddVoice(group, category, no);
        }

        public void del_voice(int index)
        {
            // delete voice by index
            this.objctrl.DeleteVoice(index);
        }

        public void del_all_voice()
        {
            // stop and delete all voice
            if (this.isVoicePlay)
            {
                this.stop_voice();
            }
            this.objctrl.DeleteAllVoice();
        }

        public void set_voice_lst(List<int[]> voiceList, bool autoplay = true)
        {
            // set a list of voice, load and play it
            // voiceList: tuple of voice: ((group, category, no), (group, category, no), ...)
            // autoplay: play voice
            this.del_all_voice();
            foreach (var v in voiceList)
            {
                this.add_voice(v[0], v[1], v[2]);
            }
            if (autoplay)
            {
                this.play_voice();
            }
        }

        public List<int[]> voice_lst
        {
            get
            {
                // return a tuple of current loaded voice: ((group, category, no), (group, category, no), ...)
                var vlist = new List<int[]>();
                foreach (var v in this.objctrl.voiceCtrl.list)
                {
                    int[] vi = new int[] { v.group, v.category, v.no };
                    vlist.Add(vi);
                }
                return vlist;
            }
        }


        public int voice_repeat
        {
            set
            {
                // set voice repeat: 0: no repeat, 1: repeat selected one, 2: repeat all
                if (value == 2)
                {
                    this.objctrl.voiceCtrl.repeat = VoiceCtrl.Repeat.All;
                }
                else if (value == 1)
                {
                    this.objctrl.voiceCtrl.repeat = VoiceCtrl.Repeat.Select;
                }
                else
                {
                    this.objctrl.voiceCtrl.repeat = VoiceCtrl.Repeat.None;
                }
            }
            get
            {
                // return the status of voice repeat setting: 0: no repeat, 1: repeat selected one, 2: repeat all
                if (this.objctrl.voiceCtrl.repeat == VoiceCtrl.Repeat.All)
                {
                    return 2;
                }
                else if (this.objctrl.voiceCtrl.repeat == VoiceCtrl.Repeat.Select)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
        }

        public void play_voice(int index = 0)
        {
            // index = which voice to play
            if (this.isVoicePlay)
            {
                this.stop_voice();
            }
            this.objctrl.PlayVoice(index);
        }

        public void stop_voice()
        {
            // stop play voice
            this.objctrl.StopVoice();
        }

        public KinematicMode kinematic
        {
            get
            {
                // return current kinematice mode: 0-none, 1-IK, 2-FK, 3-IK&FK
                KinematicMode kmode;
                if (this.objctrl.oiCharInfo.enableIK && this.objctrl.oiCharInfo.enableFK)
                {
                    kmode = KinematicMode.IKFK;
                }
                else if (this.objctrl.oiCharInfo.enableIK)
                {
                    kmode = KinematicMode.IK;
                }
                else if (this.objctrl.oiCharInfo.enableFK)
                {
                    kmode = KinematicMode.FK;
                }
                else
                {
                    kmode = KinematicMode.None;
                }
                return kmode;
            }
        }

        public void set_FK_active(int group, bool active = false, bool force = false)
        {
            OIBoneInfo.BoneGroup[] bis = new OIBoneInfo.BoneGroup[]
                { OIBoneInfo.BoneGroup.Hair, OIBoneInfo.BoneGroup.Neck, OIBoneInfo.BoneGroup.Breast, OIBoneInfo.BoneGroup.Body, OIBoneInfo.BoneGroup.RightHand, OIBoneInfo.BoneGroup.LeftHand, OIBoneInfo.BoneGroup.Skirt };

            this.objctrl.ActiveFK(bis[group], active, force);
        }

        public void set_FK_active(bool group, bool force = false)
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
            OIBoneInfo.BoneGroup[] bis = new OIBoneInfo.BoneGroup[]
                { OIBoneInfo.BoneGroup.Hair, OIBoneInfo.BoneGroup.Neck, OIBoneInfo.BoneGroup.Breast, OIBoneInfo.BoneGroup.Body, OIBoneInfo.BoneGroup.RightHand, OIBoneInfo.BoneGroup.LeftHand, OIBoneInfo.BoneGroup.Skirt };

            foreach (var i in Enumerable.Range(0, 7))
            {
                this.objctrl.ActiveFK(bis[i], group, force);
            }
        }

        public void set_FK_active(bool[] group, bool active = false, bool force = false)
        {
            OIBoneInfo.BoneGroup[] bis = new OIBoneInfo.BoneGroup[]
                { OIBoneInfo.BoneGroup.Hair, OIBoneInfo.BoneGroup.Neck, OIBoneInfo.BoneGroup.Breast, OIBoneInfo.BoneGroup.Body, OIBoneInfo.BoneGroup.RightHand, OIBoneInfo.BoneGroup.LeftHand, OIBoneInfo.BoneGroup.Skirt };

            foreach (var i in Enumerable.Range(0, group.Length <= 7 ? group.Length : 7))
            {
                this.objctrl.ActiveFK(bis[i], group[i], force);
            }
        }

        public bool[] get_FK_active()
        {
            // return active status for FK part, (hair, neck, Breast, body, right hand, left hand, skirt) in tuple
            return this.objctrl.oiCharInfo.activeFK;
        }

        /*
        public Dictionary<int, Vector3> export_fk_bone_info(bool activedOnly = true)
        {
            // export a dic contents FK bone info
            Dictionary<int, Vector3> biDic = new Dictionary<int, Vector3>();
            foreach (var i in Enumerable.Range(0, this.objctrl.listBones.Count))
            {
                OCIChar.BoneInfo binfo = this.objctrl.listBones[i];
                if (!activedOnly || binfo.active)
                {
                    //posClone = Vector3(binfo.posision.x, binfo.posision.y, binfo.posision.z)
                    Vector3 rot = binfo.boneInfo.changeAmount.rot;
                    Vector3 rotClone = new Vector3(rot.x <= 180 ? rot.x : rot.x - 360, rot.y <= 180 ? rot.y : rot.y - 360, rot.z <= 180 ? rot.z : rot.z - 360);
                    //abDic[binfo.boneID] = (posClone, rotClone)
                    biDic[i] = rotClone;
                }
            }
            //print "exported", len(biDic), "bones"
            return biDic;
        }

        public void import_fk_bone_info(Dictionary<int, Vector3> biDic)
        {
            // import fk bone info from dic
            foreach (var i in Enumerable.Range(0, this.objctrl.listBones.Count))
            {
                OCIChar.BoneInfo binfo = this.objctrl.listBones[i];
                if (biDic[i] != null)
                {
                    binfo.boneInfo.changeAmount.rot = biDic[i];
                }
            }
        }
        */

        public void set_IK_active(bool[] group, bool force = false)
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
            OIBoneInfo.BoneGroup[] bis = new OIBoneInfo.BoneGroup[] { OIBoneInfo.BoneGroup.Body, OIBoneInfo.BoneGroup.RightLeg, OIBoneInfo.BoneGroup.LeftLeg, OIBoneInfo.BoneGroup.RightArm, OIBoneInfo.BoneGroup.LeftArm };

            foreach (var i in Enumerable.Range(0, group.Length <= 5 ? group.Length : 5))
            {
                this.objctrl.ActiveIK(bis[i], group[i], force);
            }

        }

        public void set_IK_active(int group, bool active = false, bool force = false)
        {
            OIBoneInfo.BoneGroup[] bis = new OIBoneInfo.BoneGroup[] { OIBoneInfo.BoneGroup.Body, OIBoneInfo.BoneGroup.RightLeg, OIBoneInfo.BoneGroup.LeftLeg, OIBoneInfo.BoneGroup.RightArm, OIBoneInfo.BoneGroup.LeftArm };

            this.objctrl.ActiveIK(bis[group], active, force);
        }

        public void set_IK_active(bool group, bool force = false)
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
            OIBoneInfo.BoneGroup[] bis = new OIBoneInfo.BoneGroup[] { OIBoneInfo.BoneGroup.Body, OIBoneInfo.BoneGroup.RightLeg, OIBoneInfo.BoneGroup.LeftLeg, OIBoneInfo.BoneGroup.RightArm, OIBoneInfo.BoneGroup.LeftArm };

            foreach (var i in Enumerable.Range(0, 5))
            {
                this.objctrl.ActiveIK(bis[i], group, force);
            }
        }

        public bool[] get_IK_active()
        {
            // return active status for IK part, (body, right leg, left leg, right arm, left arm) in tuple
            return this.objctrl.oiCharInfo.activeIK;
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

        public void import_status_diff_optimized(Dictionary<string, IDataClass> status)
        {
            return;
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
            this.objctrl.LoadClothesFile(file);
        }

        // body sliders
        // see sceneutils.py for realization of this props
        public float get_body_shape(int p1)
        {
            return this.charInfo.GetShapeBodyValue(p1);
        }

        public void set_body_shape(int p1, float p2)
        {
            this.charInfo.SetShapeBodyValue(p1, p2);
        }



        public void set_body_shapes_all(List<float> values)
        {

            foreach (var i in Enumerable.Range(0, values.Count))
            {
                this.set_body_shape(i, values[i]);
            }
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
            this.objctrl.SetClothesStateAll(clothIndex);
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
            this.objctrl.SetClothesState(clothIndex, clothState);
        }

        public void setCloth(byte[] clothIndex)
        {
            for (int i = 0; i < clothIndex.Length; i++)
            {
                this.objctrl.SetClothesState(i, clothIndex[i]);
            }
        }

        // face sliders

        public float[] face_shapes_all
        {
            get
            {
                var ct = this.face_shapes_count;
                var res = new float[ct];
                for (int i = 0; i < ct; i++)
                {
                    res[i] = this.face_shape[i];
                }
                return res;
            }
            set
            {
                for (int i = 0; i < value.Length; i++)
                {
                    this.set_face_shape(i, value[i]);
                }
            }
        }
    }
}
