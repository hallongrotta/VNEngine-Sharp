using System;
using System.Collections.Generic;
using System.Text;

namespace VNEngine
{

    public class ActorHSNeo
        : actor
    {

        public object sex
        {
            get
            {
                // get sex: 0-male, 1-female
                return this.objctrl.charInfo.Sex;
            }
        }

        public object height
        {
            get
            {
                // get height:
                return this.objctrl.oiCharInfo.charFile.femaleCustomInfo.shapeValueBody[0];
            }
        }

        public object breast
        {
            get
            {
                // get breast:
                return this.objctrl.oiCharInfo.charFile.femaleCustomInfo.shapeValueBody[1];
            }
        }

        public object set_coordinate_type(object type)
        {
            // type: 0-normal, 1-room, 2-swim
            if (type == 0)
            {
                type = CoordinateType.type00;
            }
            else if (type == 1)
            {
                type = CoordinateType.type01;
            }
            else
            {
                type = CoordinateType.type02;
            }
            this.objctrl.SetCoordinateInfo(type);
        }

        public int get_coordinate_type()
        {
            // return coordinate type
            if (this.objctrl.charFileInfoStatus.coordinateType == CoordinateType.type00)
            {
                return 0;
            }
            else if (this.objctrl.charFileInfoStatus.coordinateType == CoordinateType.type01)
            {
                return 1;
            }
            else
            {
                return 2;
            }
        }

        public object set_cloth(object clothIndex, object clothState = null)
        {
            // param format 1: set one cloth
            // clothIndex: 0-top, 1-bottom, 2-bra, 3-shorts, 4-swim-up, 5-swim-down, 6-swim-top, 7-swim-bottom, 8-grove, 9-panst, 10-sock, 11-shoes
            // clothState: 0-put on, 1-half off, 2-off 
            // param format 2: set all clothes, like the return value of get_cloth()
            // clothIndex: state for (top, bottom, bra, shorts, swim-up, swim-down, swim-top, swim-bottom, grove, panst, sock, shoes) in tuple
            // clothState: must be None
            // param format 3: set all cloth to same state
            // clothIndex: state of all clothes
            // clothState: must be None
            if (clothState != null)
            {
                this.objctrl.SetClothesState(clothIndex, clothState);
            }
            else if (clothIndex is tuple)
            {
                foreach (var i in Enumerable.Range(0, clothIndex.Count <= 12 ? clothIndex.Count : 12))
                {
                    this.objctrl.SetClothesState(i, clothIndex[i]);
                }
            }
            else
            {
                //self.objctrl.SetClothesStateAll(clothIndex) # this function may crash the game
                foreach (var i in Enumerable.Range(0, 12))
                {
                    this.objctrl.SetClothesState(i, clothIndex);
                }
            }
        }

        public tuple get_cloth()
        {
            // return state index of (top, bottom, bra, shorts, swim-up, swim-down, swim-top, swim-bottom, grove, panst, sock, shoes) in tuple
            return tuple(this.objctrl.charFileInfoStatus.clothesState);
        }

        public object set_accessory(object accIndex, object accShow = null)
        {
            // param format 1: set one accessory
            // accIndex: 0~9
            // accShow: 0(hide)/1(visible)
            // param format 2: set all accessory, like the return value of get_accessory()
            // accIndex: 0/1 for each acessories in tuple(10)
            // accShow: must be None
            // param format 3: hide/show all accessory
            // accIndex: 0/1 for all
            // accShow: must be None
            if (accShow != null)
            {
                this.objctrl.ShowAccessory(accIndex, accShow);
            }
            else if (accIndex is tuple)
            {
                foreach (var i in Enumerable.Range(0, accIndex.Count <= 10 ? accIndex.Count : 10))
                {
                    this.objctrl.ShowAccessory(i, accIndex[i]);
                }
            }
            else
            {
                foreach (var i in Enumerable.Range(0, 10))
                {
                    this.objctrl.ShowAccessory(i, accIndex);
                }
            }
        }

        public tuple get_accessory()
        {
            // return accessory state on/off in tuple(10)
            return tuple(this.objctrl.charFileInfoStatus.showAccessory);
        }

        public object set_juice(object juices)
        {
            // juices: level on (face, FrontUp, BackUp, FrontDown, BackDown) when 0-none, 1-few, 2-lots
            // use self.objctrl.SetSiruFlags in console will cause the the game crash, but seems ok in frame
            this.objctrl.SetSiruFlags(SiruParts.SiruKao, juices[0]);
            this.objctrl.SetSiruFlags(SiruParts.SiruFrontUp, juices[1]);
            this.objctrl.SetSiruFlags(SiruParts.SiruBackUp, juices[2]);
            this.objctrl.SetSiruFlags(SiruParts.SiruFrontDown, juices[3]);
            this.objctrl.SetSiruFlags(SiruParts.SiruBackDown, juices[4]);
        }

        public tuple get_juice()
        {
            // return juice level of (face, FrontUp, BackUp, FrontDown, BackDown) in tuple
            var jInfo = new List<object>();
            jInfo.append(this.objctrl.GetSiruFlags(SiruParts.SiruKao));
            jInfo.append(this.objctrl.GetSiruFlags(SiruParts.SiruFrontUp));
            jInfo.append(this.objctrl.GetSiruFlags(SiruParts.SiruBackUp));
            jInfo.append(this.objctrl.GetSiruFlags(SiruParts.SiruFrontDown));
            jInfo.append(this.objctrl.GetSiruFlags(SiruParts.SiruBackDown));
            return tuple(jInfo);
        }

        public object set_tuya(object level)
        {
            // level: tuya 0~1
            this.objctrl.SetTuyaRate(level);
        }

        public void get_tuya()
        {
            // return tuya rate
            return this.objctrl.oiCharInfo.skinRate;
        }

        public void get_look_eye_ptn()
        {
            // return eye look at pattern: 0: front, 1: camera, 2: hide from camera, 3: fix, 4: operate
            return this.objctrl.charFileInfoStatus.eyesLookPtn;
        }

        public void get_look_neck()
        {
            // return neck look pattern: 0: front, 1: camera, 2: by anime, 3: fix
            return this.objctrl.charFileInfoStatus.neckLookPtn;
        }

        // keitaro 2.3.1
        public object get_look_neck_full()
        {
            // needed only to save Fixed state
            if (this.get_look_neck() == 4)
            {
                var memoryStream = MemoryStream();
                var binaryWriter = BinaryWriter(memoryStream);
                this.objctrl.neckLookCtrl.SaveNeckLookCtrl(binaryWriter);
                binaryWriter.Close();
                memoryStream.Close();
                return tuple(bytearray_to_list(memoryStream.ToArray()));
            }
            else
            {
                return ValueTuple.Create("<Empty>");
            }
        }

        public object set_look_neck_full(object arrstatetuple)
        {
            // needed only to set Fixed state
            if (arrstatetuple.Count > 0)
            {
                // if non-fixed-state - move to it!
                this.set_look_neck(4);
            }
            if (this.get_look_neck() == 4)
            {
                var lst = arrstatetuple.ToList();
                // print lst
                var arrstate = list_to_bytearray(lst);
                // print arrstate
                var binaryReader = BinaryReader(MemoryStream(arrstate));
                this.objctrl.neckLookCtrl.LoadNeckLookCtrl(binaryReader);
            }
        }

        public string get_look_neck_full2()
        {
            // needed only to save Fixed state
            if (this.get_look_neck() == 4)
            {
                var memoryStream = MemoryStream();
                var binaryWriter = BinaryWriter(memoryStream);
                this.objctrl.neckLookCtrl.SaveNeckLookCtrl(binaryWriter);
                binaryWriter.Close();
                memoryStream.Close();
                return bytearray_to_str64(memoryStream.ToArray());
            }
            else
            {
                return "";
            }
        }

        public object set_look_neck_full2(object str64)
        {
            // needed only to set Fixed state
            if (str64.Count > 0)
            {
                // if non-fixed-state - move to it!
                this.set_look_neck(4);
            }
            if (this.get_look_neck() == 4)
            {
                // print lst
                var arrstate = str64_to_bytearray(str64);
                // print arrstate
                var binaryReader = BinaryReader(MemoryStream(arrstate));
                this.objctrl.neckLookCtrl.LoadNeckLookCtrl(binaryReader);
            }
        }

        public void get_curcloth_coordinate()
        {
            var chaFile = this.objctrl.charInfo.chaFile;
            var memoryStream = MemoryStream();
            var binaryWriter = BinaryWriter(memoryStream);
            chaFile.clothesInfo.Save(binaryWriter);
            binaryWriter.Close();
            memoryStream.Close();
            return bytearray_to_str64(memoryStream.ToArray());
        }

        public object set_curcloth_coordinate(object str64)
        {
            var bytes = str64_to_bytearray(str64);
            //import ChaFileDefine
            var charInfo = this.objctrl.charInfo;
            try
            {
                charInfo.chaFile.clothesInfo.Load(MemoryStream(bytes), true);
                //charInfo.chaFile.SetCoordinateInfo(coordinateType);
                charInfo.Reload(false, true, true);
                if (charInfo.Sex == 1)
                {
                    // only female
                    charInfo.UpdateBustSoftnessAndGravity();
                }
            }
            catch (Exception)
            {
                Console.WriteLine(String.Format("Exception in set_curcloth_coordinate, %s", e.ToString()));
            }
        }

        public void get_eyes_open()
        {
            // return eyes open
            return this.objctrl.charFileInfoStatus.eyesOpenMax;
        }

        public void get_eyes_blink()
        {
            // return eyes blink flag
            return this.objctrl.charFileInfoStatus.eyesBlink;
        }

        public object set_kinematic(object mode, object force = 0)
        {
            // mode: 0-none, 1-IK, 2-FK, 3-IK&FK(need plugin)
            if (mode == 3)
            {
                try
                {
                    var hsa = HSStudioNEOAddon();
                    hsa.activateFKIK(this);
                }
                catch (Exception)
                {
                    Console.WriteLine("Fail to set IK&FK:", e);
                }
            }
            else if (mode == 2)
            {
                if (this.objctrl.oiCharInfo.enableIK)
                {
                    this.objctrl.ActiveKinematicMode(OICharInfo.KinematicMode.IK, 0, force);
                }
                if (!this.objctrl.oiCharInfo.enableFK)
                {
                    this.objctrl.ActiveKinematicMode(OICharInfo.KinematicMode.FK, 1, force);
                }
            }
            else if (mode == 1)
            {
                if (this.objctrl.oiCharInfo.enableFK)
                {
                    this.objctrl.ActiveKinematicMode(OICharInfo.KinematicMode.FK, 0, force);
                }
                if (!this.objctrl.oiCharInfo.enableIK)
                {
                    this.objctrl.ActiveKinematicMode(OICharInfo.KinematicMode.IK, 1, force);
                }
            }
            else
            {
                if (this.objctrl.oiCharInfo.enableIK)
                {
                    this.objctrl.ActiveKinematicMode(OICharInfo.KinematicMode.IK, 0, force);
                }
                if (this.objctrl.oiCharInfo.enableFK)
                {
                    this.objctrl.ActiveKinematicMode(OICharInfo.KinematicMode.FK, 0, force);
                }
            }
        }

        public string get_hspedata()
        {
            if (extplugins.ExtPlugin.exists("HSPENeo"))
            {
                return extplugins.HSPENeo().GetCharaSettingsText(this.objctrl);
            }
            return "";
        }

        public object set_hspedata(object hspedata)
        {
            //print "in hspedata"
            if (hspedata != "")
            {
                if (extplugins.ExtPlugin.exists("HSPENeo"))
                {
                    //print "import hspedata"
                    extplugins.HSPENeo().SetCharaSettingsText(this.objctrl, hspedata);
                    //print "import hspedata2"
                }
            }
        }

        public object get_body_shape(object p1)
        {
            return this.objctrl.charBody.charCustom.GetShapeBodyValue(p1);
        }

        public object set_body_shape(object p1, object p2)
        {
            this.objctrl.charBody.charCustom.SetShapeBodyValue(p1, p2);
        }

        public tuple get_body_shapes_all()
        {
            if (this.sex == 0)
            {
                return tuple(this.objctrl.oiCharInfo.charFile.maleCustomInfo.shapeValueBody);
            }
            else
            {
                return tuple(this.objctrl.oiCharInfo.charFile.femaleCustomInfo.shapeValueBody);
            }
        }

        public object get_body_shape_names()
        {
            if (this.sex == 0)
            {
                return CharDefine.cm_bodyshapename;
            }
            else
            {
                return CharDefine.cf_bodyshapename;
            }
        }

        public object get_body_shapes_count()
        {
            return this.get_body_shapes_all().Count;
        }

        public object get_face_shape(object p1)
        {
            return this.objctrl.charBody.charCustom.GetShapeFaceValue(p1);
        }

        public object set_face_shape(object p1, object p2)
        {
            this.objctrl.charBody.charCustom.SetShapeFaceValue(p1, p2);
        }

        public tuple get_face_shapes_all()
        {
            if (this.sex == 0)
            {
                return tuple(this.objctrl.oiCharInfo.charFile.maleCustomInfo.shapeValueFace);
            }
            else
            {
                return tuple(this.objctrl.oiCharInfo.charFile.femaleCustomInfo.shapeValueFace);
            }
        }

        public object get_face_shape_names()
        {
            if (this.sex == 0)
            {
                return CharDefine.cm_headshapename;
            }
            else
            {
                return CharDefine.cf_headshapename;
            }
        }

        public object get_face_shapes_count()
        {
            return this.get_face_shapes_all().Count;
        }

        public object export_full_status()
        {
            // export a dict contains all actor status
            var fs = new Dictionary<object, object>
            {
            };
            fs["visible"] = this.visible;
            fs["move_to"] = this.pos;
            fs["rotate_to"] = this.rot;
            fs["scale_to"] = this.scale;
            fs["anim"] = this.get_animate();
            fs["anim_spd"] = this.get_anime_speed();
            fs["anim_ptn"] = this.get_anime_pattern();
            fs["anim_lp"] = this.get_anime_forceloop();
            fs["cloth_type"] = this.get_coordinate_type();
            fs["cloth_all"] = this.get_cloth();
            fs["acc_all"] = this.get_accessory();
            if (this.sex == 1)
            {
                fs["juice"] = this.get_juice();
                fs["tear"] = this.get_tear();
                fs["face_red"] = this.get_facered();
                fs["nip_stand"] = this.get_nipple_stand();
                fs["skin_tuya"] = this.get_tuya();
            }
            else
            {
                fs["son"] = this.get_son();
                fs["simple"] = this.get_simple();
                fs["simple_color"] = this.get_simple_color();
            }
            fs["look_at_ptn"] = this.get_look_eye_ptn();
            fs["look_at_pos"] = this.get_look_eye_pos();
            fs["face_to"] = this.get_look_neck();
            fs["face_to_full2"] = this.get_look_neck_full2();
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
            if (is_ini_value_true("ExportChara_CurClothesCoord"))
            {
                fs["ext_curclothcoord"] = this.get_curcloth_coordinate();
            }
            if (is_ini_value_true("ExportChara_BodyShapes"))
            {
                fs["ext_bodyshapes"] = this.get_body_shapes_all();
            }
            if (is_ini_value_true("ExportChara_FaceShapes"))
            {
                fs["ext_faceshapes"] = this.get_face_shapes_all();
            }
            if (is_ini_value_true("ExportChara_AnimeAuxParam"))
            {
                fs["anim_optprm"] = this.get_anime_option_param();
            }
            // plugin data, enable by ini setting
            try
            {
                if (extplugins.ExtPlugin.exists("HSPENeo"))
                {
                    if (is_ini_value_true("ExportChara_HSPENeo"))
                    {
                        fs["pl_hspedata"] = this.get_hspedata();
                    }
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Error during get hspedata");
            }
            return fs;
        }

        public object h_partner(object hType = 0, object hPosition = 0)
        {
            // return partner sex for current h
            if (hType != 4)
            {
                if (this.sex == 0)
                {
                    return ValueTuple.Create(1);
                }
                else
                {
                    return ValueTuple.Create(0);
                }
            }
            else
            {
                //print "htype = 4: multi, hPosition =", hPosition
                if (Enumerable.Range(0, 4 - 0).Contains(hPosition))
                {
                    if (this.sex == 0)
                    {
                        return (1, 0);
                    }
                    else
                    {
                        return (0, 0);
                    }
                }
                else if (Enumerable.Range(4, 12 - 4).Contains(hPosition))
                {
                    if (this.sex == 0)
                    {
                        return (1, 1);
                    }
                    else
                    {
                        return (0, 1);
                    }
                }
                else if (this.sex == 0)
                {
                    return (1, 1);
                }
                else
                {
                    return ValueTuple.Create(1);
                }
            }
        }

        public object h_with(
            object partner,
            object hType = 0,
            object hPosition = 0,
            object hStage = 0,
            object extActors = ValueTuple.Create("<Empty>"))
        {
            object factor;
            object mactor;
            // partner: another actor as sex partner
            // hType: 1-11
            // hPosition:?
            // hStage:?
            // extActors = (Actor1, Actor2, Actor3)
            // sync with partner
            if (this.pos != partner.pos || this.rot != partner.rot || this.scale != partner.scale)
            {
                partner.move(pos: this.pos, rot: this.rot, scale: this.scale);
            }
            if (this.get_anime_speed() != partner.get_anime_speed())
            {
                partner.set_anime_speed(this.get_anime_speed());
            }
            if (this.get_anime_pattern() != partner.get_anime_pattern())
            {
                partner.set_anime_pattern(this.get_anime_pattern());
            }
            if (this.get_anime_forceloop() != partner.get_anime_forceloop())
            {
                partner.set_anime_forceloop(this.get_anime_forceloop());
            }
            foreach (var extActor in extActors)
            {
                if (extActor == null)
                {
                    continue;
                }
                //print "get ext actor " + extActor.text_name
                if (this.pos != extActor.pos || this.rot != extActor.rot || this.scale != extActor.scale)
                {
                    extActor.move(pos: this.pos, rot: this.rot, scale: this.scale);
                }
                if (this.get_anime_speed() != extActor.get_anime_speed())
                {
                    extActor.set_anime_speed(this.get_anime_speed());
                }
                if (this.get_anime_pattern() != extActor.get_anime_pattern())
                {
                    extActor.set_anime_pattern(this.get_anime_pattern());
                }
                if (this.get_anime_forceloop() != extActor.get_anime_forceloop())
                {
                    extActor.set_anime_forceloop(this.get_anime_forceloop());
                }
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
            // show son if not
            if (mactor.sex == 0 && !mactor.get_son())
            {
                mactor.set_son(1);
            }
            foreach (var extActor in extActors)
            {
                if (extActor != null && extActor.sex == 0 && !extActor.get_son())
                {
                    extActor.set_son(1);
                }
            }
            // load anime
            var info = Info.Instance;
            var gcDic = info.dicFAGroupCategory;
            var aDic = info.dicFemaleAnimeLoadInfo;
            var gp = Enumerable.Range(3, 9 - 3)[hType];
            var validCategoryKey = gcDic[gp].dicCategory.Keys.ToList();
            if (!Enumerable.Range(0, validCategoryKey.Count).Contains(hPosition))
            {
                Console.WriteLine(String.Format("invalid hPosition %d, must be 0~%d", hPosition, validCategoryKey.Count - 1));
                return;
            }
            var cat = validCategoryKey[hPosition];
            var validNoKey = aDic[gp][cat].Keys.ToList();
            if (!Enumerable.Range(0, validNoKey.Count).Contains(hStage))
            {
                Console.WriteLine(String.Format("invalid hStage %d, must be 0~%d", hStage, validNoKey.Count - 1));
                return;
            }
            var no = validNoKey[hStage];
            if (gp != 7)
            {
                //print "do %s > %s > %s"%(gcDic[gp].name, gcDic[gp].dicCategory[cat], aDic[gp][cat][no].name)
                //print "mactor.animate(%d, %d, %d)"%(gp, cat, no)
                //print "factor.animate(%d, %d, %d)"%(gp, cat, no)
                mactor.animate(gp, cat, no);
                factor.animate(gp, cat, no);
            }
            else if (Enumerable.Range(130, 134 - 130).Contains(cat))
            {
                mactor.animate(gp, cat, no);
                factor.animate(gp, cat, no);
                if (extActors.Count > 0 && extActors[0] != null)
                {
                    extActors[0].animate(gp, cat + 24, no);
                }
            }
            else if (Enumerable.Range(134, 138 - 134).Contains(cat))
            {
                mactor.animate(gp, cat, no);
                factor.animate(gp, cat, no);
                if (extActors.Count > 0 && extActors[0] != null)
                {
                    extActors[0].animate(gp, cat + 24, no);
                }
            }
            else if (Enumerable.Range(158, 162 - 158).Contains(cat))
            {
                mactor.animate(gp, cat - 24, no);
                factor.animate(gp, cat, no);
                if (extActors.Count > 0 && extActors[0] != null)
                {
                    extActors[0].animate(gp, cat - 24, no);
                }
            }
            else if (cat == 138 || cat == 139)
            {
                mactor.animate(gp, cat + 24, no);
                factor.animate(gp, cat, no);
            }
            else
            {
                mactor.animate(gp, cat - 24, no);
                factor.animate(gp, cat, no);
            }
            // auto adjust anime param
            //print "factor(%s): height=%.2f breast=%.2f"%(factor.text_name, factor.height, factor.breast)
            var anime_option_param = (factor.height, factor.breast);
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

        [staticmethod]
        public static tuple get_hanime_group_names()
        {
            var info = Info.Instance;
            var gcDic = info.dicFAGroupCategory;
            return tuple(from i in Enumerable.Range(3, 9 - 3)
                         select gcDic[i].name);
        }

        [staticmethod]
        public static tuple get_hanime_category_names(object group)
        {
            var info = Info.Instance;
            group = Enumerable.Range(3, 9 - 3)[group];
            var cDic = info.dicFAGroupCategory[group].dicCategory;
            return tuple(cDic.Values);
        }

        [staticmethod]
        public static tuple get_hanime_no_names(object group, object category)
        {
            var info = Info.Instance;
            var nDic = info.dicFemaleAnimeLoadInfo;
            group = Enumerable.Range(3, 9 - 3)[group];
            category = tuple(info.dicFAGroupCategory[group].dicCategory.Keys)[category];
            var nName = new List<object>();
            foreach (var n in nDic[group][category].Keys)
            {
                nName.append(nDic[group][category][n].name);
            }
            return tuple(nName);
        }
    }

}
