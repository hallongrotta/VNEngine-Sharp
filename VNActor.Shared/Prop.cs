using Studio;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace VNActor
{

    public partial class Prop : HSNeoOCIItem
    {
        
        public Prop(OCIItem objctrl) : base(objctrl)
        {
            this.objctrl = objctrl;
        }

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
        /*
        public Vector3 pos
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

        public Vector3 rot
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
                return this.objctrl.objectInfo.changeAmount.scale;
            }
            set
            {
                this.objctrl.objectInfo.changeAmount.scale = value;
            }
        }
        */

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
