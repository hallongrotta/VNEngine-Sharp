using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace VNActor
{
    //===============================================================================================
    // character action wrapper functions list
    // All scripts: func(char, param)
    public partial class Actor
    {

        public static void char_anime(Actor chara, ActorData param)
        {

            if (param.anim.normalizedTime is float n)
            {
                chara.SetAnimate(param.anim.group, param.anim.category, param.anim.no, n);
            }
            else
            {
                chara.SetAnimate(param.anim.group, param.anim.category, param.anim.no);
            }


        }

        /*
        public static void char_anime(Actor chara, ActorData param)
        {
            if (param.Length == 3)
            {
                // param = (group, category, no)
                chara.setAnimate(param[0], param[1], param[2]);
            }
            else if (param.Length == 4)
            {
                // param = (group, category, no, normalizedTime)
                chara.setAnimate(param[0], param[1], param[2], param[3]);
            }
            else if (param.Length == 5)
            {
                // param = (group, category, no, normalizedTime, forceload)
                chara.setAnimate(param[0], param[1], param[2], param[3], param[4]);
            }
            else
            {
                Console.WriteLine("char_anime param format error");
            }
        }
        */

        public static void char_anime_speed(Actor chara, ActorData param)
        {
            // param = speed (0~3)
            chara.AnimeSpeed = param.animeSpeed;
        }

        public static void char_anime_pattern(Actor chara, ActorData param)
        {
            // param = pattern (0~1)
            chara.AnimePattern = param.animePattern;
        }

        public static void char_anime_forceloop(Actor chara, ActorData param)
        {
            // param = force loop (0/1)
            chara.AnimationForceLoop = param.forceLoop;
        }

        public static void char_anime_optionparam(Actor chara, ActorData param)
        {
            // param = (aux value 1, aux value 2)
            chara.AnimationOption = param.animeOption;
        }

        public static void char_anime_restart(Actor chara, ActorData param)
        {
            // param ignore
            chara.restart_anime();
        }

        public static void char_load_cloth(Actor chara, ActorData param)
        {
            // load cloth

            return; // TODO
            //chara.load_clothes_file(param.clothLoad);
        }

        /* TODO
        public static void char_cloth(Actor chara, ActorData param)
        {
            // param = (clothIndex, clothState)
            chara.setCloth(param.cloth);
        }
        */

        public static void char_all_clothes(Actor chara, ActorData param)
        {
            // param = 0(all), 1(half), 2(nude)
            // or
            // param = (top, bottom, bra, shorts, grove, panst, sock, shoe)
            chara.Clothes = param.cloth;
        }

        public static void char_juice(Actor chara, ActorData param)
        {
            // param = juice level on (face, FrontUp, BackUp, FrontDown, BackDown) where 0-none, 1-few, 2-lots, or just on int to set all
            chara.Juice = param.juice;
        }

        public static void char_face_red(Actor chara, ActorData param)
        {
            // param = hohoAka level(0~1)
            chara.FaceRedness = param.faceRedness;
        }

        public static void char_nip_stand(Actor chara, ActorData param)
        {
            // param = nipple stand level (0~1)
            chara.NippleStand = param.nippleHardness;
        }

        public static void char_face_option(Actor chara, ActorData param)
        {
            // param = 0-none, 1-ball, 2-tape
            // chara.set_face_option(param);
        }

        public static void char_son(Actor chara, ActorData param)
        {
            // param = visible or (visible(0/1), length(0~3))
            chara.Son = param.son;
        }

        public static void char_simple(Actor chara, ActorData param)
        {
            // param = simple visible
            chara.Simple = param.simple;
        }

        public static void char_simple_color(Actor chara, ActorData param)
        {
            // param = simple color
            chara.SimpleColor = param.simpleColor;
        }

        public static void char_eyes_look(Actor chara, ActorData param)
        {
            chara.set_look_eye(param.eyeLookPattern, param.eyeLookPos);
        }

        public static void char_eyes_look_ptn(Actor chara, ActorData param)
        {
            // param = 0, 1, 2, 3, 4
            chara.EyeLookPattern = param.eyeLookPattern;
        }

        public static void char_eyes_look_pos(Actor chara, ActorData param)
        {
            // param = Vector3 or (x, y, z)
            chara.EyeLookPos = param.eyeLookPos;
        }

        public static void char_neck_look(Actor chara, ActorData param)
        {
            // param = 0, 1, 2, 3, 4
            chara.LookNeckPattern = param.neckPattern;
        }

        public static void char_neck_look_full2(Actor chara, ActorData param)
        {
            // param = array of bytes, use dump to get it
            try
            {
                chara.LookNeckFull2 = param.neck;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in setting char neck in Fixed state (2): " + e.ToString());
                Console.WriteLine("Sorry, we just pass it...");
            }
        }

        public static void char_neck_look_full(Actor chara, ActorData param)
        {
            // param = array of bytes, use dump to get it
            try
            {
                //chara.set_look_neck_full(param); todo
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in setting char neck in Fixed state: " + e.ToString());
                Console.WriteLine("Sorry, we just pass it...");
            }
        }

        public static void char_eyebrow(Actor chara, ActorData param)
        {
            // param = eyebrow pattern
            chara.EyebrowPattern = param.eyebrowPattern;
        }

        public static void char_eyes(Actor chara, ActorData param)
        {
            // param = eye pattern
            chara.EyePattern = param.eyePattern;
        }

        public static void char_eyes_open(Actor chara, ActorData param)
        {
            // param = 0~1
            chara.EyesOpenLevel = param.eyesOpen;
        }

        public static void char_eyes_blink(Actor chara, ActorData param)
        {
            // param = 0(False)/1(True)
            chara.EyesBlink = param.blinking;
        }

        public static void char_mouth(Actor chara, ActorData param)
        {
            // param = mouth pattern
            chara.MouthPattern = param.mouthPattern;
        }

        public static void char_mouth_open(Actor chara, ActorData param)
        {
            // param = 0~1
            chara.MouthOpenLevel = param.mouthOpen;
        }

        public static void char_lip_sync(Actor chara, ActorData param)
        {
            // param = 0/1
            chara.LipSync = param.lipSync;
        }

        public static void char_hands(Actor chara, ActorData param)
        {
            // param = (left hand ptn, right hand ptn)
            chara.HandPattern = param.handMotions;
        }

        public static void char_move(Actor chara, ActorData param)
        {
            // param = (pos_delta_x, pos_delta_y, pos_delta_z)
            var cp = chara.Position;
            var ncp = new Vector3(cp.x + param.position.x, cp.y + param.position.y, cp.z + param.position.z);
            chara.Position = ncp;
        }

        public static void char_move_to(Actor chara, ActorData param)
        {
            // param = (pos_dst_x, pos_dst_y, pos_dst_z)
            chara.Position = param.position;
        }

        /* TODO
        public static void char_turn(Actor chara, ActorData param)
        {
            // param = rot_delta_y
            Vector3 rt = chara.rot;
            Vector3 nrt = new Vector3(rt.x, rt.y + param.y_rotation, rt.z);
            chara.rot = nrt;
        }

        public static void char_turn_to(Actor chara, ActorData param)
        {
            // param = rot_dst_y
            Vector3 rt = chara.rot;
            Vector3 nrt = new Vector3(rt.x, param.y_rotation, rt.z);
            chara.rot = nrt;
        }
        */

        public static void char_rotate_to(Actor chara, ActorData param)
        {
            // param = (rot_x, rot_y, rot_z)
            // for rotate x and z
            chara.Rotation = param.rotation;
        }

        public static void char_scale_to(Actor chara, ActorData param)
        {
            chara.Scale = param.scale;
        }

        public static void char_kinematic(Actor chara, ActorData param)
        {
            // param = 0-none, 1-IK, 2-FK
            chara.set_kinematic(param.kinematicType);
        }

        public static void char_fk_active(Actor chara, ActorData param)
        {
            // param = 0/1 flag in tuple (hair, neck, Breast, body, right hand, left hand, skirt)
            bool[] curFk = chara.get_FK_active();
            foreach (var i in Enumerable.Range(0, 7))
            {
                if (param.fkActive[i] != curFk[i])
                {
                    chara.set_FK_active(i, param.fkActive[i]);
                }
            }
        }

        public static void char_fk_set(Actor chara, ActorData param)
        {
            // param = fk bones info dict
            chara.import_fk_bone_info(param.fk);
        }

        public static void char_ik_active(Actor chara, ActorData param)
        {
            // param = 0/1 flag in tuple (body, right leg, left leg, right arm, left arm)
            var curIk = chara.get_IK_active();
            foreach (var i in Enumerable.Range(0, 5))
            {
                if (param.ikActive[i] != curIk[i])
                {
                    chara.set_IK_active(i, param.ikActive[i]);
                }
            }
        }

        public static void char_ik_set(Actor chara, ActorData param)
        {
            // param = ik target info dict
            chara.import_ik_target_info(param.ik);
        }

        public static void char_voice_lst(Actor chara, ActorData param)
        {
            // param = voice list
            // always play the voice
            chara.set_voice_lst(param.voiceList);
        }

        public static void char_voice_rpt(Actor chara, ActorData param)
        {
            // param = voice repeat flag
            chara.VoiceRepeat = param.voiceRepeat;
        }

        public static void char_visible(Actor chara, ActorData param)
        {
            // param = 0 or 1
            chara.Visible = param.visible;
        }

        public static void char_ext_bodyshapes(Actor chara, List<float> param)
        {
            // param = body shape array
            try
            {
                chara.set_body_shapes_all(param);
            }
            catch (Exception e)
            {
                Console.WriteLine("VNGE VNActor error: can't set body shapes", e);
            }
        }

        public static void char_ext_faceshapes(Actor chara, float[] param)
        {
            // param = face shape array
            try
            {
                chara.FaceShapesAll = param;
            }
            catch (Exception e)
            {
                Console.WriteLine("VNGE VNActor error: can't set face shapes", e);
            }
        }

        public static void char_pl_hspedata(Actor chara, int[] param)
        {
            // param = hspe data
            try
            {
                // chara.set_hspedata(param); TODO
            }
            catch (Exception e)
            {
                Console.WriteLine("VNGE VNActor error: can't set HSPE data", e);
            }
        }

        public static void char_pl_aipedata(Actor chara, int[] param)
        {
            // param = aipe data
            try
            {
                // chara.set_aipedata(param); TODO
            }
            catch (Exception e)
            {
                Console.WriteLine("VNGE VNActor error: can't set AIPE data", e);
            }
        }

        /*
        private static bool char_act_funcs(string key, Actor actor, ActorData param)
        {

            CharaActFunction func;

            switch (key)
            {
                case "anim":
                    func = char_anime;
                    break;

                case "anim_spd":
                    func = char_anime_speed;
                    break;

                case "anim_ptn":
                    func = char_anime_pattern, true;

                case "anim_lp":
                    char_anime_forceloop, false;

                case "anim_optprm":
                    char_anime_optionparam, true;

                case "anim_restart":
                    char_anime_restart, false;

                case "load_cloth":
                    char_load_cloth, false;

                case "cloth":
                    char_cloth, false;

                case "cloth_all":
                    char_all_clothes, false;

                case "acc":
                    char_accessory, false;

                case "acc_all":
                    char_all_accessories, false;

                case "juice":
                    char_juice, false;

                case "tear":
                    char_tear, false;

                case "face_red":
                    char_face_red, true;

                case "nip_stand":
                    char_nip_stand, true;

                case "skin_tuya":
                    char_tuya, true;

                case "skin_wet":
                    char_wet, true;

                case "face_opt":
                    char_face_option, false;

                case "son":
                    char_son, true;

                case "simple":
                    char_simple, false;

                case "simple_color":
                    char_simple_color, true;

                case "look_at":
                    char_eyes_look, true;

                case "look_at_ptn":
                    char_eyes_look_ptn, false;

                case "look_at_pos":
                    char_eyes_look_pos, true;

                case "face_to":
                    char_neck_look, false;

                case "face_to_full":
                    char_neck_look_full, false;

                case "face_to_full2":
                    char_neck_look_full2, false;

                case "eyebrow":
                    char_eyebrow, false;

                case "eyes":
                    char_eyes, false;

                case "eyes_open":
                    char_eyes_open, true;

                case "eyes_blink":
                    char_eyes_blink, false;

                case "mouth":
                    char_mouth, false;

                case "mouth_open":
                    char_mouth_open, true;

                case "lip_sync":
                    char_lip_sync, false;

                case "hands":
                    char_hands, false;

                case "move":
                    char_move, false;

                case "move_to":
                    char_move_to, true;

                case "turn":
                    char_turn, false;

                case "turn_to":
                    char_turn_to, true;

                case "rotate_to":
                    char_rotate_to, true;

                case "scale_to":
                    char_scale_to, true;

                case "kinematic":
                    char_kinematic, false;

                case "fk_active":
                    char_fk_active, false;

                case "fk_set":
                    char_fk_set, true;

                case "ik_active":
                    char_ik_active, false;

                case "ik_set":
                    char_ik_set, true;

                case "voice_lst":
                    char_voice_lst, false;

                case "voice_rpt":
                    char_voice_rpt, false;

                case "visible":
                    char_visible, false;

                case "ext_bodyshapes":
                    char_ext_bodyshapes, true;

                case "ext_faceshapes":
                    char_ext_faceshapes, true;

                case "pl_kkpedata":
                    char_pl_kkpedata, false;

                case "pl_aipedata":
                    (char_pl_aipedata, false;

                default:
                    break;
            }
         */
    }

}
