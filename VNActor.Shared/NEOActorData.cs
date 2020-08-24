using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static VNActor.Actor;

namespace VNActor
{
    abstract public class NEOActorData
    {

        public bool visible;

        public Vector3 position;

        public Vector3 scale;

        public Vector3 rotation;

        public int voiceRepeat;

        public List<int[]> voiceList;

        public Dictionary<int, Vector3> fk;

        public bool[] fkActive;

        public KinematicMode kinematicType;

        public Hands_s handMotions;

        public Dictionary<string, IK_node_s> ik;

        public bool[] ikActive;

        public bool lipSync;

        public float mouthOpen;

        public int mouthPattern;
   
        public bool blinking;
        
        public float eyesOpen;
      
        public int eyePattern;
        
        public int eyebrowPattern;
        
        public int neckPattern;
       
        public Vector3 eyeLookPos;
        
        public int eyeLookPattern;
       
        public Son_s son;
        
        public Animation_s anim;
      
        public bool simple;
       
        public Color simpleColor;
    
        public byte[] juice;

        public bool[] accessoryStatus;

        public byte[] cloth;

        public float animeSpeed;

        public bool forceLoop;

        public float animePattern;

        public AnimeOption_s animeOption;

        public float faceRedness;

        public float nippleHardness;

        public byte[] neck;

        public NEOActorData()
        {

        }

        public NEOActorData(Actor a)
        {
            visible = a.Visible;
            position = a.Position;
            rotation = a.Rotation;
            scale = a.Scale;
            animeSpeed = a.AnimeSpeed;
            animePattern = a.AnimePattern;
   
            forceLoop = a.AnimationForceLoop;

            accessoryStatus = new bool[a.Accessories.Length];
            Array.Copy(a.Accessories, accessoryStatus, accessoryStatus.Length);

            faceRedness = a.FaceRedness;
            son = a.Son;

            anim = a.Animation;

            animeOption = new AnimeOption_s { height = a.height, breast = a.Breast };

            cloth = a.cloth;

            juice = a.Juice;
            nippleHardness = a.NippleStand;

            simple = a.Simple;
            simpleColor = a.SimpleColor;

            eyeLookPattern = a.EyeLookPattern;
            eyeLookPos = a.EyeLookPos;
            neckPattern = a.LookNeckPattern;

            neck = a.look_neck_full2;
            eyebrowPattern = a.eyebrow_ptn;
            eyePattern = a.eyes_ptn;
            eyesOpen = a.EyesOpenLevel;
            blinking = a.EyesBlink;
            mouthPattern = a.mouth_ptn;
            mouthOpen = a.MouthOpenLevel;
            lipSync = a.LipSync;
            handMotions = a.HandPattern;
            kinematicType = a.Kinematic;

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

            voiceList = a.VoiceList;
            voiceRepeat = a.VoiceRepeat;            
        }

        virtual public void Apply(Actor a)
        {
            a.Visible = visible;
            a.Position = position;
            a.Rotation = rotation;
            a.Scale = scale;
            a.AnimeSpeed = animeSpeed;
            a.AnimePattern = animePattern;
            a.AnimationForceLoop = forceLoop;
            a.Accessories = accessoryStatus;
            a.FaceRedness = faceRedness;
            son = a.Son;

            if (anim.normalizedTime is float time)
            {
                a.SetAnimate(anim.group, anim.category, anim.no, time);
            }
            else
            {
                a.SetAnimate(anim.group, anim.category, anim.no);
            }


            //(height, breast) = a.animeOption;

            a.cloth = cloth;

            a.Juice = juice;
            a.NippleStand = nippleHardness;

            a.Simple = simple;
            a.SimpleColor = simpleColor;

            a.EyeLookPattern = eyeLookPattern;
            a.EyeLookPos = eyeLookPos;
            a.LookNeckPattern = neckPattern;

            a.look_neck_full2 = neck;
            a.eyebrow_ptn = eyebrowPattern;
            a.eyes_ptn = eyePattern;
            a.EyesOpenLevel = eyesOpen;
            a.EyesBlink = blinking;
            a.mouth_ptn = mouthPattern;
            a.MouthOpenLevel = mouthOpen;
            a.LipSync = lipSync;
            a.HandPattern = handMotions;
            a.set_kinematic(kinematicType);

            if (kinematicType == KinematicMode.IK)
            {
                a.set_IK_active(ikActive);
                a.import_ik_target_info(ik);
            }
            else if (kinematicType == KinematicMode.FK)
            {
                a.set_FK_active(fkActive);
                a.import_fk_bone_info(fk);
            }
            else if (kinematicType == KinematicMode.IKFK)
            {
                a.set_IK_active(ikActive);
                a.import_ik_target_info(ik);
                a.set_FK_active(fkActive);
                a.import_fk_bone_info(fk);
            }

            //voice_lst = voiceList;
            a.VoiceRepeat = voiceRepeat;
        } 
    }
}
