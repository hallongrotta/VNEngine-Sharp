using System;
using System.Collections.Generic;
using System.Linq;
using MessagePack;
using Studio;
using UnityEngine;

namespace VNActor
{
    // Shared Item code
    public partial class Item : Prop, IVNObject<Item>
    {
        public new OCIItem objctrl;

        public Item(OCIItem objctrl) : base(objctrl)
        {
            this.objctrl = objctrl;
        }

        public override Vector3 Position
        {
            get => objctrl.objectInfo.changeAmount.pos;
            set => objctrl.objectInfo.changeAmount.pos = value;
        }

        public override Vector3 Rotation
        {
            get => objctrl.objectInfo.changeAmount.rot;
            set => objctrl.objectInfo.changeAmount.rot = value;
        }

        public Vector3 Scale
        {
            get
            {
                if (objctrl is OCIItem item)
                    return item.objectInfo.changeAmount.scale;
                throw new Exception("Can not scale this item");
            }
            set
            {
                if (objctrl is OCIItem item)
                    item.itemInfo.changeAmount.scale = value;
            }
        }

        public int No
        {
            get
            {
                var item = objctrl;
                return item.itemInfo.no;
            }
        }


        public string Name => objctrl.treeNodeObject.textName;
        /*
        public bool isFolder
        {
            get
            {
                return this.objctrl is OCIFolder;
            }
        }
        */

        public bool IsItem => objctrl is OCIItem;

        /*
        public bool isLight
        {
            get
            {
                return this.objctrl is OCILight;
            }
        }
        */

        public bool IsColorable
        {
            get
            {
                if (IsItem)
                {
                    var item = objctrl;
                    return item.isChangeColor;
                }

                return false;
            }
        }

        public bool IsAnime
        {
            get
            {
                if (IsItem)
                {
                    var item = objctrl;
                    return item.isAnime;
                }

                return false;
            }
        }

        public bool IsFk => objctrl.isFK;

        public float AnimeSpeed
        {
            set =>
                // speed: 0~1
                objctrl.animeSpeed = value;
            get =>
                // return anime speed
                objctrl.animeSpeed;
        }

        public Emission_s Emission
        {
            set
            {
                // param: (color, power)
                if (hasEmission)
                {
                    var eColor = value.color;
                    var ePower = value.power;
                    //this.objctrl.SetEmissionColor(eColor);
                    //this.objctrl.SetEmissionPower(ePower);
                    objctrl.itemInfo.emissionColor = eColor;
                    objctrl.itemInfo.emissionPower = ePower;
                    objctrl.UpdateColor();
                }
            }
            get
            {
                if (hasEmission)
                {
                    var eColor = objctrl.itemInfo.emissionColor;
                    var ePower = objctrl.itemInfo.emissionPower;
                    return new Emission_s {color = eColor, power = ePower};
                }

                throw new Exception("This item has no emission");
            }
        }

        public float Alpha
        {
            set =>
                // param: 0~1 for alpha
                objctrl.SetAlpha(value);
            get
            {
                if (hasAlpha)
                    return objctrl.itemInfo.alpha;
                throw new Exception();
            }
        }

        public bool DynamicBoneEnable
        {
            set
            {
                // param: dynamic bone (yure) enable/disable
                if (IsDynamicBone) objctrl.ActiveDynamicBone(value);
            }
            get
            {
                if (IsDynamicBone)
                    return objctrl.itemInfo.enableDynamicBone;
                throw new Exception();
            }
        }

        public bool IsDynamicBone
        {
            get
            {
                if (IsItem)
                    return objctrl.isDynamicBone;
                return false;
            }
        }

        public new IDataClass<Item> export_full_status()
        {
            return new ItemData(this);
        }

        public void import_status(IDataClass<Item> p)
        {
            if (p is ItemData id) import_status(id);
        }

        public static Item add_item(int group, int category, int no)
        {
            var objctrl = AddObjectItem.Add(group, category, no); //TODO make this right
            return new Item(objctrl);
        }

        public void pos_add(float[] param)
        {
            // param = (pos_delta_x, pos_delta_y, pos_delta_z)
            ObjectCtrlInfo item = objctrl;
            var cp = item.objectInfo.changeAmount.pos;
            var ncp = new Vector3(cp.x + param[0], cp.y + param[1], cp.z + param[2]);
            item.objectInfo.changeAmount.pos = ncp;
        }

        public void rot_add(float[] param)
        {
            // param = (rot_delta_x, rot_delta_y, rot_delta_z)
            ObjectCtrlInfo item = objctrl;
            var rt = item.objectInfo.changeAmount.rot;
            var nrt = new Vector3(rt.x + param[0], rt.y + param[1], rt.z + param[2]);
            item.objectInfo.changeAmount.rot = nrt;
        }

        public void scale_add(float[] param)
        {
            // param = (scale_x, scale_y, scale_z) or scale
            if (objctrl is OCIItem item)
            {
                // for item only, folder can not set scale
                var rt = item.itemInfo.changeAmount.scale;
                var nrt = new Vector3(rt.x + param[0], rt.y + param[1], rt.z + param[2]);
                item.itemInfo.changeAmount.scale = nrt;
            }
        }

        public void move(Vector3 pos, Vector3 rot, Vector3 scale)
        {
            if (pos != null) objctrl.objectInfo.changeAmount.pos = pos;
            if (rot != null) objctrl.objectInfo.changeAmount.rot = rot;
            if (scale != null && IsItem) objctrl.objectInfo.changeAmount.scale = scale;
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

        public List<Vector3> Fk
        {
            get
            {
                // return a tuple of FK bone rot
                if (!IsFk) return new List<Vector3>();
                var boneInfo = new List<Vector3>();
                foreach (var bi in objctrl.listBones)
                {
                    var rot = bi.boneInfo.changeAmount.rot;
                    var rotClone = new Vector3(rot.x <= 180 ? rot.x : rot.x - 360, rot.y <= 180 ? rot.y : rot.y - 360,
                        rot.z <= 180 ? rot.z : rot.z - 360);
                    boneInfo.Add(rotClone);
                }

                return boneInfo;
            }
            set
            {
                // import fk bone info from dic
                if (!IsFk || value is null) return;
                foreach (var i in Enumerable.Range(0, objctrl.listBones.Count))
                {
                    var bone = objctrl.listBones[i];
                    if (i < value.Count) bone.boneInfo.changeAmount.rot = value[i];
                }
            }
        }

        [MessagePackObject]
        public struct Panel
        {
            [Key(0)] public string filepath;
            [Key(1)] public bool clamp;
        }

        [MessagePackObject]
        public struct PanelDetail_s
        {
            [Key(0)] public Color color;
            [Key(1)] public float ut;
            [Key(2)] public float vt;
            [Key(3)] public float us;
            [Key(4)] public float vs;
            [Key(5)] public float rot;

            public PanelDetail_s(Color mainColor, float ut, float vt, float us, float vs, float rot) : this()
            {
                color = mainColor;
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
            [Key(0)] public Color color;
            [Key(1)] public float power;
        }

        [MessagePackObject]
        public struct Pattern
        {
            [Key(0)] public int key;
            [Key(1)] public string filepath;
            [Key(2)] public bool clamp;

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
            [Key(0)] public Color color;
            [Key(1)] public float ut;
            [Key(2)] public float vt;
            [Key(3)] public float us;
            [Key(4)] public float vs;
            [Key(5)] public float rot;

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
            [Key(0)] public Color color;
            [Key(1)] public float lineWidth;
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