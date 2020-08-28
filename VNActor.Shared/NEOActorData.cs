using System;
using System.Collections.Generic;
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


/* Unmerged change from project 'VNActor.AI'
Before:
        public int eyePattern;
        
        public int eyebrowPattern;
        
        public int neckPattern;
After:
        public int eyePattern;

        public int eyebrowPattern;

        public int neckPattern;
*/

/* Unmerged change from project 'VNActor.HS2'
Before:
        public int eyePattern;
        
        public int eyebrowPattern;
        
        public int neckPattern;
After:
        public int eyePattern;

        public int eyebrowPattern;

        public int neckPattern;
*/
        public int eyePattern;

        public int eyebrowPattern;

        public int neckPattern;

        public Vector3 eyeLookPos;

        public int eyeLookPattern;


/* Unmerged change from project 'VNActor.AI'
Before:
        public Son_s son;
        
        public Animation_s anim;
After:
        public Son_s son;

        public Animation_s anim;
*/

/* Unmerged change from project 'VNActor.HS2'
Before:
        public Son_s son;
        
        public Animation_s anim;
After:
        public Son_s son;

        public Animation_s anim;
*/
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

            animeOption = new AnimeOption_s { height = a.Height, breast = a.Breast };

            cloth = a.Clothes;

            juice = a.Juice;
            nippleHardness = a.NippleStand;

            simple = a.Simple;
            simpleColor = a.SimpleColor;

            eyeLookPattern = a.EyeLookPattern;
            eyeLookPos = a.EyeLookPos;
            neckPattern = a.LookNeckPattern;

            neck = a.LookNeckFull2;
            eyebrowPattern = a.EyebrowPattern;
            eyePattern = a.EyePattern;
            eyesOpen = a.EyesOpenLevel;
            blinking = a.EyesBlink;
            mouthPattern = a.MouthPattern;
            mouthOpen = a.MouthOpenLevel;
            lipSync = a.LipSync;
            handMotions = a.HandPattern;
            kinematicType = a.Kinematic;
            fkActive = new bool[a.get_FK_active().Length];
            ikActive = new bool[a.get_IK_active().Length];
            Array.Copy(a.get_FK_active(), fkActive, fkActive.Length);
            Array.Copy(a.get_IK_active(), ikActive, ikActive.Length);

            if (kinematicType == KinematicMode.FK || kinematicType == KinematicMode.IKFK)
            {
                fk = a.export_fk_bone_info();
            }
            if (kinematicType == KinematicMode.IK || kinematicType == KinematicMode.IKFK)
            {
                ik = a.export_ik_target_info();
            }

            voiceList = a.VoiceList;
            voiceRepeat = a.VoiceRepeat;
        }

        virtual public void Apply(Actor a)
        {


            //if (this.kinematicType != KinematicMode.IK)
            //{
            if (a.Position != position || a.Animation.no != anim.no)
            {
                a.set_kinematic(KinematicMode.FK); // Enable FK by default so that stuff does not jiggle when moved.
                //a.reset_fk_bone_info();
                a.set_FK_active(new bool[] { true, false, true, false, false, false, false });
            }

            //} 


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

            a.Clothes = cloth;

            a.Juice = juice;
            a.NippleStand = nippleHardness;

            a.Simple = simple;
            a.SimpleColor = simpleColor;

            a.EyeLookPattern = eyeLookPattern;
            a.EyeLookPos = eyeLookPos;
            a.LookNeckPattern = neckPattern;

            a.LookNeckFull2 = neck;
            a.EyebrowPattern = eyebrowPattern;
            a.EyePattern = eyePattern;
            a.EyesOpenLevel = eyesOpen;
            a.EyesBlink = blinking;
            a.MouthPattern = mouthPattern;
            a.MouthOpenLevel = mouthOpen;
            a.LipSync = lipSync;
            a.HandPattern = handMotions;
            a.set_kinematic(kinematicType);
            a.set_IK_active(ikActive);
            a.set_FK_active(fkActive);

            if (kinematicType == KinematicMode.IK)
            {
                a.import_ik_target_info(ik);
            }
            else if (kinematicType == KinematicMode.FK)
            {
                a.import_fk_bone_info(fk);
            }
            else if (kinematicType == KinematicMode.IKFK)
            {
                a.import_ik_target_info(ik);
                a.import_fk_bone_info(fk);
            }

            //voice_lst = voiceList;
            a.VoiceRepeat = voiceRepeat;
        }
    }
}
