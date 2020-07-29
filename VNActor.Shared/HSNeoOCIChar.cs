using Studio;
using System;
using UnityEngine;

namespace VNActor
{
    public class HSNeoOCIChar
        : HSNeoOCI
    {

        new public OCIChar objctrl;


        public HSNeoOCIChar(OCIChar objctrl) : base(objctrl)
        {
            this.objctrl = objctrl;
        }

        public static HSNeoOCIChar add_female(string path)
        {
            var objctrl = AddObjectFemale.Add(path);
            return new HSNeoOCIChar(objctrl);
        }

        public static HSNeoOCIChar add_male(string path)
        {
            var objctrl = AddObjectMale.Add(path);
            return new HSNeoOCIChar(objctrl);
        }

        public ChaControl charInfo
        {
            get
            {
                return this.objctrl.charInfo;
            }
        }

        public OICharInfo oiCharInfo
        {
            get
            {
                return this.objctrl.oiCharInfo;
            }
        }

        public Vector3 pos
        {
            get
            {
                return this.charInfo.transform.localPosition;
            }
        }

        public Vector3 rot
        {
            get
            {
                return this.charInfo.transform.localRotation.eulerAngles;
            }
        }

        public Vector3 scale
        {
            get
            {
                return this.charInfo.transform.localScale;
            }
        }

        public int look_eyes_ptn
        {
            get
            {
                return this.objctrl.charInfo.GetLookEyesPtn();
            }
            set
            {
                this.objctrl.charInfo.ChangeLookEyesPtn(value);
            }
        }

        public int look_neck_ptn
        {
            get
            {
                return this.objctrl.charInfo.GetLookNeckPtn();
            }
            set
            {
                this.objctrl.charInfo.ChangeLookNeckPtn(value);
            }
        }

        public int mouth_ptn
        {
            get
            {
                return this.objctrl.charInfo.GetMouthPtn();
            }
            set
            {
                this.objctrl.charInfo.ChangeMouthPtn(value);
            }
        }

        public float mouth_openmax
        {
            get
            {
                return this.objctrl.charInfo.GetMouthOpenMax();
            }
            set
            {
                this.objctrl.charInfo.ChangeMouthOpenMax(value);
            }
        }

        public int eyes_ptn
        {
            get
            {
                return this.objctrl.charInfo.GetEyesPtn();
            }
            set
            {
                this.objctrl.charInfo.ChangeEyesPtn(value);
            }
        }

        public float eyes_openmax
        {
            get
            {
                return this.objctrl.charInfo.GetEyesOpenMax();

            }
            set
            {
                this.objctrl.charInfo.ChangeEyesOpenMax(value);
            }
        }
        /*
        public int eyebrow_ptn
        {
            get
            {

                return this.objctrl.charInfo.GetEyebrowPtn();

            }
            set
            {
                this.objctrl.charInfo.ChangeEyebrowPtn(value);
            }
        }*/

        public Actor as_actor { get { return (Actor)this; } }

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
            this.objctrl.RestartAnime();
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
            foreach (var key in dic0.Keys)
            {
                Console.WriteLine(key);
            }
            var d1 = dic0[category];
            Console.WriteLine(d1.ToString());
            var d2 = d1[group];
            Console.WriteLine(d2.ToString());
            var animeLoadInfo = d2[no];
            Console.WriteLine(animeLoadInfo.ToString());
            Console.WriteLine(String.Format("%s %s %s", animeLoadInfo.bundlePath, animeLoadInfo.fileName, animeLoadInfo.clip));
        }

        public void female_all_clothes_state(int state)
        {
            this.objctrl.SetClothesStateAll(state);
        }

        public void dump_obj()
        {
            //print "objctrlchar.move(pos=%s, rot=%s)"%(str(self.charInfo.GetPosition()), str(self.charInfo.GetRotation()))
            try
            {
                Console.WriteLine(String.Format("objctrlchar.move(pos=%s, rot=%s, scale=%s)", this.charInfo.transform.localPosition.ToString(), this.charInfo.transform.localRotation.eulerAngles.ToString(), this.charInfo.transform.localScale.ToString()));
                Console.WriteLine(String.Format("objctrlchar.animate(%s, %s, %s, %s, %s)", this.oiCharInfo.animeInfo.group.ToString(), this.oiCharInfo.animeInfo.category.ToString(), this.oiCharInfo.animeInfo.no.ToString(), this.oiCharInfo.animePattern.ToString(), this.oiCharInfo.animeSpeed.ToString()));
                //print "objctrlchar.tears_level = %s" % (str(self.tears_level))
            }
            catch (Exception e)
            {
                Console.WriteLine(String.Format("# oops, error happened %s", e.ToString()));
            }
            return;
        }
    }

}
