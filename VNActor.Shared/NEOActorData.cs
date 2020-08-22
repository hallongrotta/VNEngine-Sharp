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
            visible = a.visible;
            position = a.pos;
            rotation = a.rot;
            scale = a.scale;
            animeSpeed = a.animeSpeed;
            animePattern = a.animePattern;
   
            forceLoop = a.anime_forceloop;

            accessoryStatus = new bool[a.accessory.Length];
            Array.Copy(a.accessory, accessoryStatus, accessoryStatus.Length);

            faceRedness = a.facered;
            son = a.son;

            anim = a.animate;

            animeOption = new AnimeOption_s { height = a.height, breast = a.breast };

            cloth = a.cloth;

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
        }
    }
}
