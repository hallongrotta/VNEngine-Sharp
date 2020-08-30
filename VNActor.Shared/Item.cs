using MessagePack;
using Studio;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace VNActor
{

    // Shared Item code
    public partial class Item : Prop, IVNObject
    {

        [MessagePackObject]
        public struct Panel
        {
            [Key(0)]
            public string filepath;
            [Key(1)]
            public bool clamp;
        }

        [MessagePackObject]
        public struct PanelDetail_s
        {
            [Key(0)]
            public Color color;
            [Key(1)]
            public float ut;
            [Key(2)]
            public float vt;
            [Key(3)]
            public float us;
            [Key(4)]
            public float vs;
            [Key(5)]
            public float rot;

            public PanelDetail_s(Color mainColor, float ut, float vt, float us, float vs, float rot) : this()
            {
                this.color = mainColor;
                this.ut = ut;
                this.vt = vt;
                this.us = us;
                this.vs = vs;
                this.rot = rot;
            }
        }

        [MessagePackObject]
        public struct Emission_s
        {
            [Key(0)]
            public Color color;
            [Key(1)]
            public float power;
        }

        [MessagePackObject]
        public struct Pattern
        {
            [Key(0)]
            public int key;
            [Key(1)]
            public string filepath;
            [Key(2)]
            public bool clamp;

            public Pattern(int key, string filepath, bool clamp)
            {
                this.key = key;
                this.filepath = filepath;
                this.clamp = clamp;
            }
        }


        [MessagePackObject]
        public struct PatternDetail_s
        {
            [Key(0)]
            public Color color;
            [Key(1)]
            public float ut;
            [Key(2)]
            public float vt;
            [Key(3)]
            public float us;
            [Key(4)]
            public float vs;
            [Key(5)]
            public float rot;

            public PatternDetail_s(Color color, float ut, float vt, float us, float vs, float rot)
            {
                this.color = color;
                this.ut = ut;
                this.vt = vt;
                this.us = us;
                this.vs = vs;
                this.rot = rot;
            }
        }

        [MessagePackObject]
        public struct Line_s
        {
            [Key(0)]
            public Color color;
            [Key(1)]
            public float lineWidth;
        }

        new public OCIItem objctrl;

        public Item(OCIItem objctrl) : base(objctrl)
        {
            this.objctrl = objctrl;
        }

        public static Item add_item(int group, int category, int no)
        {
            var objctrl = AddObjectItem.Add(group, category, no); //TODO make this right
            return new Item(objctrl);
        }

        public void pos_add(float[] param)
        {
            // param = (pos_delta_x, pos_delta_y, pos_delta_z)
            ObjectCtrlInfo item = this.objctrl;
            Vector3 cp = item.objectInfo.changeAmount.pos;
            Vector3 ncp = new Vector3(cp.x + param[0], cp.y + param[1], cp.z + param[2]);
            item.objectInfo.changeAmount.pos = ncp;
        }

        public void rot_add(float[] param)
        {
            // param = (rot_delta_x, rot_delta_y, rot_delta_z)
            ObjectCtrlInfo item = this.objctrl;
            Vector3 rt = item.objectInfo.changeAmount.rot;
            Vector3 nrt = new Vector3(rt.x + param[0], rt.y + param[1], rt.z + param[2]);
            item.objectInfo.changeAmount.rot = nrt;
        }

        public void scale_add(float[] param)
        {
            // param = (scale_x, scale_y, scale_z) or scale
            if (this.objctrl is OCIItem item)
            {
                // for item only, folder can not set scale
                Vector3 rt = item.itemInfo.changeAmount.scale;
                Vector3 nrt = new Vector3(rt.x + param[0], rt.y + param[1], rt.z + param[2]);
                item.itemInfo.changeAmount.scale = nrt;
            }
        }

        override public Vector3 Position
        {
            get
            {
                return this.objctrl.objectInfo.changeAmount.pos;
            }
            set
            {
                this.objctrl.objectInfo.changeAmount.pos = value;
            }
        }

        override public Vector3 Rotation
        {
            get
            {
                return this.objctrl.objectInfo.changeAmount.rot;
            }
            set
            {
                this.objctrl.objectInfo.changeAmount.rot = value;
            }
        }

        public Vector3 scale
        {
            get
            {
                if (this.objctrl is OCIItem item)
                    return item.objectInfo.changeAmount.scale;
                else
                {
                    throw new Exception("Can not scale this item");
                }
            }
            set
            {
                if (this.objctrl is OCIItem item)
                    item.itemInfo.changeAmount.scale = value;
            }
        }

        public int no
        {
            get
            {
                OCIItem item = (OCIItem)this.objctrl;
                return item.itemInfo.no;
            }
        }



        public string name
        {
            get
            {
                return this.objctrl.treeNodeObject.textName;
            }
        }
        /*
        public bool isFolder
        {
            get
            {
                return this.objctrl is OCIFolder;
            }
        }
        */

        public bool isItem
        {
            get
            {
                return this.objctrl is OCIItem;
            }
        }

        /*
        public bool isLight
        {
            get
            {
                return this.objctrl is OCILight;
            }
        }
        */

        public bool isColorable
        {
            get
            {
                if (this.isItem)
                {
                    OCIItem item = (OCIItem)this.objctrl;
                    return item.isChangeColor;
                }
                else
                {
                    return false;
                }
            }
        }

        public bool isAnime
        {
            get
            {
                if (this.isItem)
                {
                    OCIItem item = (OCIItem)this.objctrl;
                    return item.isAnime;
                }
                else
                {
                    return false;
                }
            }
        }

        public bool isFK
        {
            get
            {
                if (this.isItem)
                {
                    OCIItem item = (OCIItem)this.objctrl;
                    return item.isFK;
                }
                else
                {
                    return false;
                }
            }
        }

        public void move(Vector3 pos, Vector3 rot, Vector3 scale)
        {
            if (pos != null)
            {
                this.objctrl.objectInfo.changeAmount.pos = pos;
            }
            if (rot != null)
            {
                this.objctrl.objectInfo.changeAmount.rot = rot;
            }
            if (scale != null && this.isItem)
            {
                this.objctrl.objectInfo.changeAmount.scale = scale;
            }
        }

        public float anime_speed
        {
            set
            {
                // speed: 0~1
                this.objctrl.animeSpeed = value;
            }
            get
            {
                // return anime speed
                return this.objctrl.animeSpeed;
            }
        }

        public Emission_s emission
        {
            set
            {
                // param: (color, power)
                if (this.hasEmission)
                {
                    var eColor = value.color;
                    var ePower = value.power;
                    //this.objctrl.SetEmissionColor(eColor);
                    //this.objctrl.SetEmissionPower(ePower);
                    this.objctrl.itemInfo.emissionColor = eColor;
                    this.objctrl.itemInfo.emissionPower = ePower;
                    this.objctrl.UpdateColor();
                }
            }
            get
            {
                if (this.hasEmission)
                {
                    var eColor = this.objctrl.itemInfo.emissionColor;
                    var ePower = this.objctrl.itemInfo.emissionPower;
                    return new Emission_s { color = eColor, power = ePower };
                }
                else
                {
                    throw new Exception("This item has no emission");
                }
            }
        }

        public float alpha
        {
            set
            {
                // param: 0~1 for alpha
                this.objctrl.SetAlpha(value);
            }
            get
            {
                if (this.hasAlpha)
                {
                    return this.objctrl.itemInfo.alpha;
                }
                else
                {
                    throw new Exception();
                }
            }
        }

        public bool dynamicbone_enable
        {
            set
            {
                // param: dynamic bone (yure) enable/disable
                if (this.isDynamicBone)
                {
                    this.objctrl.ActiveDynamicBone(value);
                }
            }
            get
            {
                if (this.isDynamicBone)
                {
                    return this.objctrl.itemInfo.enableDynamicBone;
                }
                else
                {
                    throw new Exception();
                }
            }
        }

        public bool isDynamicBone
        {
            get
            {
                if (this.isItem)
                {
                    return this.objctrl.isDynamicBone;
                }
                else
                {
                    return false;
                }
            }
        }

        override public IDataClass export_full_status()
        {
            return new ItemData(this);
        }

        override public void import_status(IDataClass p)
        {
            if (p is ItemData id)
            {
                import_status(id);
            }
        }

        public void import_status(ItemData p)
        {
            p.Apply(this);
        }

        /*
        public void set_color(((Color, float color, float intensity, float sharpness), (Color, float color, float intensity, float sharpness)) color)
        {
            // color : a tuple of ((UnityEngine.Color, Color, Intensity, Sharpness), (UnityEngine.Color, Color, Intensity, Sharpness))
            // color (light): UnityEngine.Color (r,g,b,a)
            if (this.isColorable)
            {
                OCIItem item = (OCIItem)this.objctrl;

                item.SetColor(Utils.tuple4_2_color(color.Item1.Item1));
                item.SetGlossiness(Utils.tuple4_2_color(color.Item1.Item2));
                set_intensity(color.Item1.Item3);
                item.SetSharpness(color.Item1.Item4);
                if (item.isColor2 && color.Count > 1)
                {
                    item.SetColor2(Utils.tuple4_2_color(color.Item2.Item1));
                    item.SetGloss2(Utils.tuple4_2_color(color.Item2.Item2));
                    item.SetSharpness2(color.Item2.Item4);
                }
                item.UpdateColor();
            }
        }

        public void set_color((Color color, float color, float intensity, float sharpness) color)
        {
            if (this.isColorable)
            {
                OCIItem item = (OCIItem)this.objctrl;

                item.SetColor(Utils.tuple4_2_color(color.));
                item.SetGlossiness(Utils.tuple4_2_color(color.Item1.Item2));
                set_intensity(color.Item1.Item3);
                item.SetSharpness(color.Item1.Item4);
            }
        }
        */

        /*
        public void set_color((float, float, float, float) color)
        {
            if (this.isLight)
            {
                OCILight light = (OCILight)this.objctrl;
                light.SetColor(Utils.tuple4_2_color(color));
            }
        }

        public Color get_color()
        {
            // return a tuple of used color
            if (this.isColorable)
            {
                OCIItem item = (OCIItem)this.objctrl;
                var cl = new List<Tuple<object, object, object, object>> {
                        (item.itemInfo.color.rgbaDiffuse, item.itemInfo.color.rgbSpecular, item.itemInfo.color.specularIntensity, item.itemInfo.color.specularSharpness)
                    };
                if (item.GetProperty(isColor2))
                {
                    cl.append((this.objctrl.itemInfo.color2.rgbaDiffuse, this.objctrl.itemInfo.color2.rgbSpecular, this.objctrl.itemInfo.color2.specularIntensity, this.objctrl.itemInfo.color2.specularSharpness));
                }
                return tuple(cl);
            }
            else if (this.isLight)
            {
                OCILight light = (OCILight)this.objctrl;
                return light.lightInfo.color;
            }
        }*/

        public List<Vector3> export_fk_bone_info()
        {
            // return a tuple of FK bone rot
            if (this.isFK)
            {
                var boneinfo = new List<Vector3>();
                OCIItem item = objctrl;
                foreach (var bi in item.listBones)
                {
                    var rot = bi.boneInfo.changeAmount.rot;
                    Vector3 rotClone = new Vector3(rot.x <= 180 ? rot.x : rot.x - 360, rot.y <= 180 ? rot.y : rot.y - 360, rot.z <= 180 ? rot.z : rot.z - 360);
                    boneinfo.Add(rotClone);
                }
                return boneinfo;
            }
            else
            {
                return new List<Vector3>();
            }
        }

        public void import_fk_bone_info(List<Vector3> biList)
        {
            // import fk bone info from dic
            if (this.isFK)
            {
                OCIItem item = objctrl;
                foreach (var i in Enumerable.Range(0, item.listBones.Count))
                {
                    var binfo = item.listBones[i];
                    if (i < biList.Count)
                    {
                        binfo.boneInfo.changeAmount.rot = biList[i];
                    }
                }
            }
        }


        /*
        public void import_status(IDataClass status)
        {
            foreach (var f in status.Keys)
            {
                if (prop_act_funcs.Keys.Contains(f))
                {
                    prop_act_funcs[f].Item1(this, status[f]);
                }
                else
                {
                    Console.WriteLine(String.Format("act error: unknown function '%s' for prop", f));
                }
            }
        }
        */

        /*

        public void import_status_diff_optimized(Dictionary<string, object> status)
        {
            Dictionary<string, object> ofs = this.export_full_status();
            Dictionary<string, object> dfs = new Dictionary<string, object>
            {
            };
            foreach (string key in status.Keys)
            {
                if (!ofs.ContainsKey(key) || ofs[key] != status[key])
                {
                    dfs[key] = status[key];
                }
            }
            //return dfs
            //print "Optimized import status diff, ", dfs
            this.import_status(dfs);
        }

      */



    }
}
