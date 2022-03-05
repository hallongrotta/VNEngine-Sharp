using System;
using System.Collections.Generic;
using MessagePack;
using UnityEngine;
using static VNActor.Actor;

namespace VNActor
{
    public abstract class NEOActorData
    {
        public bool[] accessoryStatus;

        [Key("AdvIKData")] public AdvIKData advIKData;

        public Animation_s anim;

        public AnimeOption_s animeOption;

        public float animePattern;

        public float animeSpeed;

        public bool blinking;

        public byte[] cloth;

        public byte[] eyeAngles;

        public int eyebrowPattern;

        public EyeLookState eyeLookPattern;

        public Vector3 eyeLookPos;

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

        public float eyesOpen;

        public float faceRedness;

        public Dictionary<int, Vector3> fk;

        public bool[] fkActive;

        public bool forceLoop;

        public Hands_s handMotions;

        public Dictionary<string, IK_node_s> ik;

        public bool[] ikActive;

        public byte[] juice;

        public KinematicMode kinematicType;

        public bool lipSync;

        public float mouthOpen;

        public int mouthPattern;

        public byte[] neck;

        public NeckPattern neckPattern;

        public float nippleHardness;

        public Vector3 position;

        public Vector3 rotation;

        public Vector3 scale;

        public bool simple;

        public Color simpleColor;


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

        public bool visible;

        public List<int[]> voiceList;

        public int voiceRepeat;

        public NEOActorData()
        {
        }

        public NEOActorData(Actor a)
        {
            visible = a.Visible;
            if (visible)
            {
                position = a.Position;
                rotation = a.Rotation;
                scale = a.Scale;
                animeSpeed = a.AnimeSpeed;
                animePattern = a.AnimePattern;

                forceLoop = a.AnimationForceLoop;

                accessoryStatus = a.Accessories;

                faceRedness = a.FaceRedness;
                son = a.Son;

                anim = a.Animation;

                animeOption = new AnimeOption_s {height = a.Height, breast = a.Breast};

                cloth = a.Clothes;

                juice = a.Juice;
                nippleHardness = a.NippleStand;

                simple = a.Simple;
                simpleColor = a.SimpleColor;

                eyeLookPattern = a.Gaze;
                eyeLookPos = a.GazeTarget;
                neckPattern = a.LookNeckPattern;

                neck = a.LookNeckFull2;
                eyebrowPattern = a.EyebrowPattern;
                eyePattern = a.EyePattern;
                eyesOpen = a.EyesOpenLevel;
                eyeAngles = a.EyeAngles;
                blinking = a.EyesBlink;
                mouthPattern = a.MouthPattern;
                mouthOpen = a.MouthOpenLevel;
                lipSync = a.LipSync;
                handMotions = a.HandPattern;
                kinematicType = a.Kinematic;
                fkActive = a.get_FK_active();
                ikActive = a.get_IK_active();

                if (kinematicType == KinematicMode.FK || kinematicType == KinematicMode.IKFK)
                    fk = a.export_fk_bone_info();
                if (kinematicType == KinematicMode.IK || kinematicType == KinematicMode.IKFK)
                    ik = a.export_ik_target_info();

                voiceList = a.VoiceList;
                voiceRepeat = a.VoiceRepeat;

                // External plugin data
                try
                {
                    if (kinematicType == KinematicMode.IK) advIKData = new AdvIKData(a);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }

        public virtual void Apply(Actor a)
        {
            a.Visible = visible;
            if (visible)
            {
                //if (this.kinematicType != KinematicMode.IK)
                //{
                if (a.Position != position || a.Animation.no != anim.no) a.BreastDBEnable = false;
                //}


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
                    a.SetAnimate(anim.@group, anim.category, anim.no, time);
                else
                    a.SetAnimate(anim.@group, anim.category, anim.no);


                //(height, breast) = a.animeOption;

                a.Clothes = cloth;

                a.Juice = juice;
                a.NippleStand = nippleHardness;

                a.Simple = simple;
                a.SimpleColor = simpleColor;

                a.Gaze = eyeLookPattern;
                a.GazeTarget = eyeLookPos;
                a.LookNeckPattern = neckPattern;

                a.LookNeckFull2 = neck;
                a.EyebrowPattern = eyebrowPattern;
                a.EyePattern = eyePattern;
                a.EyeAngles = eyeAngles;
                a.EyesOpenLevel = eyesOpen;
                a.EyesBlink = blinking;
                a.MouthPattern = mouthPattern;
                a.MouthOpenLevel = mouthOpen;
                a.LipSync = lipSync;
                a.HandPattern = handMotions;

                if (a.Kinematic != kinematicType) a.set_kinematic(kinematicType);

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
                    a.set_FK_active(fkActive);
                    a.import_ik_target_info(ik);
                    a.import_fk_bone_info(fk);
                }


                //voice_lst = voiceList;
                a.VoiceRepeat = voiceRepeat;

                if (a.Position != position || a.Animation.no != anim.no) a.BreastDBEnable = true;

                //External plugin data

                try
                {
                    if (kinematicType == KinematicMode.IK) advIKData.Apply(a);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }
    }
}