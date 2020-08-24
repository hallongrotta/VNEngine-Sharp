using MessagePack;
using Studio;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace VNActor
{
    public partial class Item : IVNObject
    {



        [MessagePackObject(keyAsPropertyName: true)]
        public class ItemData : NEOItemData, IDataClass
        {

            public ItemData()
            {

            }

            override public void Apply(Item i)
            {
                base.Apply(i);
                if (i.hasLine)
                {
                    if (line is Line_s line_s)
                    {
                        line = line_s;
                    }
                }
                if (i.hasShadowColor)
                {
                    if (shadow_color is Color c)
                    {
                        shadow_color = c;
                    }
                }
                if (i.hasLightCancel)
                {
                    if (light_cancel is float cancel)
                        light_cancel = cancel;
                }
                if (i.isFK)
                {
                    i.import_fk_bone_info(fk_set);
                }
            }

            public ItemData(Item i) : base(i)
            {
                if (i.hasLine)
                {
                    line = i.line;
                }
                if (i.hasShadowColor)
                {
                    shadow_color = i.shadow_color;
                }
                if (i.hasLightCancel)
                {
                    light_cancel = i.light_cancel;
                }
                if (i.isFK)
                {
                    fk_set = i.export_fk_bone_info();
                }
            }
        }

        public Dictionary<int, Color> color
        {
            set
            {
                if (this.isColorable)
                {
                    var i = 0;
                    if (this.objctrl.useColor[0] && i < color.Count && color.ContainsKey(i))
                    {
                        this.objctrl.itemInfo.color[0] = value[i];
                    }
                    i = 1;
                    if (this.objctrl.useColor[1] && i < color.Count && color.ContainsKey(i))
                    {
                        this.objctrl.itemInfo.color[1] = value[i];
                    }
                    i = 2;
                    if (this.objctrl.useColor[2] && i < color.Count && color.ContainsKey(i))
                    {
                        this.objctrl.itemInfo.color[2] = value[i];
                    }
                    i = 3;
                    if (this.objctrl.useColor4 && i < color.Count && color.ContainsKey(i))
                    {
                        this.objctrl.itemInfo.color[7] = value[i];
                    }
                    this.objctrl.UpdateColor();
                }
            }
            get
            {
                // return a tuple of used color
                if (this.isColorable)
                {
                    var cl = new Dictionary<int, Color>();
                    if (this.objctrl.useColor[0])
                    {
                        cl[0] = this.objctrl.itemInfo.color[0];
                    }
                    if (this.objctrl.useColor[1])
                    {
                        cl[1] = this.objctrl.itemInfo.color[1];
                    }
                    if (this.objctrl.useColor[2])
                    {
                        cl[2] = this.objctrl.itemInfo.color[2];
                    }
                    if (this.objctrl.useColor4)
                    {
                        cl[4] = this.objctrl.itemInfo.color[7];
                    }
                    return cl;
                }
                else
                {
                    return null;
                }
            }
        }

        public bool hasPattern
        {
            get
            {
                if (!this.isItem)
                {
                    return false;
                }
                foreach (var n in this.objctrl.usePattern)
                {
                    if (n)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        public Dictionary<int, Pattern> pattern
        {
            set
            {
                // param: a set of ((key, filepath, clamp), (key, filepath, clamp), (key, filepath, clamp))
                if (this.hasPattern)
                {
                    foreach (var i in Enumerable.Range(0, this.objctrl.usePattern.Length))
                    {
                        if (this.objctrl.usePattern[i] && value.ContainsKey(i))
                        {
                            this.objctrl.itemInfo.pattern[i].key = value[i].key;
                            this.objctrl.itemInfo.pattern[i].filePath = value[i].filepath;
                            this.objctrl.itemInfo.pattern[i].clamp = value[i].clamp;
                        }
                    }
                    this.objctrl.SetupPatternTex();
                    this.objctrl.UpdateColor();
                }
            }
            get
            {
                if (this.hasPattern)
                {
                    var pt = new Dictionary<int, Pattern>();
                    foreach (var i in Enumerable.Range(0, this.objctrl.usePattern.Length))
                    {
                        if (this.objctrl.usePattern[i])
                        {
                            var pi = this.objctrl.itemInfo.pattern[i];
                            pt[i] = new Pattern { key = pi.key, filepath = pi.filePath, clamp = pi.clamp };
                        }
                    }
                    return pt;
                }
                else
                {
                    return null;
                }
            }
        }

        public Dictionary<int, PatternDetail_s> pattern_detail
        {
            set
            {
                // param: a set of ((color, ut, vt, us, vs, rot), (color, ut, vt, us, vs, rot), (color, ut, vt, us, vs, rot))
                if (this.hasPattern)
                {
                    foreach (var i in Enumerable.Range(0, this.objctrl.usePattern.Length))
                    {
                        if (this.objctrl.usePattern[i] && value.ContainsKey(i))
                        {
                            this.objctrl.itemInfo.color[i + 3] = value[i].color;
                            this.objctrl.itemInfo.pattern[i].ut = value[i].ut;
                            this.objctrl.itemInfo.pattern[i].vt = value[i].vt;
                            this.objctrl.itemInfo.pattern[i].us = value[i].us;
                            this.objctrl.itemInfo.pattern[i].vs = value[i].vs;
                            this.objctrl.itemInfo.pattern[i].rot = value[i].rot;
                        }
                    }
                    this.objctrl.UpdateColor();
                }
            }
            get
            {
                if (this.hasPattern)
                {
                    var pt = new Dictionary<int, PatternDetail_s>();
                    foreach (var i in Enumerable.Range(0, this.objctrl.usePattern.Length))
                    {
                        if (this.objctrl.usePattern[i])
                        {
                            var color = this.objctrl.itemInfo.color[i + 3];
                            var pi = this.objctrl.itemInfo.pattern[i];
                            pt[i] = new PatternDetail_s { color = color, ut = pi.ut, vt = pi.vt, us = pi.us, vs = pi.vs, rot = pi.rot };
                        }
                    }
                    return pt;
                }
                else
                {
                    return null;
                }
            }
        }

        public bool hasPanel
        {
            get
            {
                return this.isItem && this.objctrl.checkPanel;
            }
        }

        public Panel panel
        {
            set
            {
                // param: a set of (filepath, clamp)
                if (this.hasPanel)
                {
                    this.objctrl.SetMainTex(value.filepath);
                    this.objctrl.SetPatternClamp(0, value.clamp);
                }
            }
            get
            {
                if (this.hasPanel)
                {
                    var pi = this.objctrl.itemInfo.panel;
                    var p0 = this.objctrl.itemInfo.pattern[0];
                    return new Panel { filepath = pi.filePath, clamp = p0.clamp };
                }
                else
                {
                    throw new Exception("Item does not have panel.");
                }
            }
        }

        public PanelDetail_s panel_detail
        {
            set
            {
                // param: a set of (color, ut, vt, us, vs, rot)
                if (this.hasPanel)
                {
                    PatternInfo p0 = this.objctrl.itemInfo.pattern[0];
                    this.objctrl.itemInfo.color[0] = value.color;
                    p0.ut = value.ut;
                    p0.vt = value.vt;
                    p0.us = value.us;
                    p0.vs = value.vs;
                    p0.rot = value.rot;
                    this.objctrl.UpdateColor();
                }
            }
            get
            {
                if (this.hasPanel)
                {
                    var p0 = this.objctrl.itemInfo.pattern[0];
                    return new PanelDetail_s { color = this.objctrl.itemInfo.color[0], ut = p0.ut, vt = p0.vt, us = p0.us, vs = p0.vs, rot = p0.rot };
                }
                else
                {
                    throw new Exception("Item does not have panel.");
                }
            }
        }

        public bool hasEmission
        {
            get
            {
                if (!this.isItem)
                {
                    return false;
                }
                else
                {
                    return this.objctrl.checkEmission;
                }
            }
        }

        public bool hasAlpha
        {
            get
            {
                return this.isColorable && this.objctrl.checkAlpha;
            }
        }

        public bool hasLine
        {
            get
            {
                return this.isItem && this.objctrl.checkLine;
            }
        }

        public Line_s line
        {
            set
            {
                // param: (lineColor, lineWidth)
                if (this.hasLine)
                {
                    this.objctrl.SetLineColor(value.color);
                    this.objctrl.SetLineWidth(value.lineWidth);
                }


            }
            get
            {
                if (this.hasLine)
                {
                    return new Line_s { color = this.objctrl.itemInfo.lineColor, lineWidth = this.objctrl.itemInfo.lineWidth };
                }
                else
                {
                    throw new Exception();
                }
            }
        }

        public bool hasShadowColor
        {
            get
            {
                return this.isItem && this.objctrl.checkShadow;
            }
        }

        public Color shadow_color
        {
            get
            {
                if (this.hasShadowColor)
                {
                    return this.objctrl.itemInfo.color[6];
                }
                else
                {
                    throw new Exception();
                }
            }
            set
            {
                // param: color
                if (this.hasShadowColor)
                {
                    this.objctrl.itemInfo.color[6] = value;
                    this.objctrl.UpdateColor();
                }
            }
        }

        public float light_cancel
        {
            set
            {
                // param: light cancel
                if (this.hasLightCancel)
                {
                    this.objctrl.SetLightCancel(value);
                }
            }
            get
            {
                if (this.hasLightCancel)
                {
                    return this.objctrl.itemInfo.lightCancel;
                }
                else
                {
                    throw new Exception();
                }
            }
        }

        public bool hasLightCancel
        {
            get
            {
                return this.isItem && this.objctrl.checkLightCancel;
            }
        }

        public static void prop_line(Item prop, ItemData param)
        {
            // param: (color, width)
            prop.line = (Line_s)param.line;
        }

        public static void prop_shadow_color(Item prop, ItemData param)
        {
            // param: shadow color
            prop.shadow_color = (Color)param.shadow_color;
        }

        public static void prop_light_cancel(Item prop, ItemData param)
        {
            // param: light cancel
            prop.light_cancel = (float)param.light_cancel;
        }

        public static void prop_color(Item prop, Dictionary<int, Color> param)
        {
            prop.color = param;
        }




        /* TODO

        public static void prop_color(Item prop, Color param)
        {
            object ncolor;
            // param = ((R, G, B, A), (R, G, B, A), ...) or ((R, G, B), (R, G, B), ...) or (R, G, B) or (R, G, B, A) or (Color, Color, ...) or Color
            var clist = new List<Color>();
            if (param is Color)
            {
                clist.Add(param);
            }
            else if (param[0] is Color)
            {
                foreach (var subc in param)
                {
                    clist.Add(subc);
                }
            }
            else if (param[0] is tuple)
            {
                foreach (var subc in param)
                {
                    if (subc.Count == 4)
                    {
                        ncolor = Color(subc[0], subc[1], subc[2], subc[3]);
                    }
                    else
                    {
                        ncolor = Color(subc[0], subc[1], subc[2]);
                    }
                    clist.append(ncolor);
                }
            }
            else
            {
                if (param.Count == 4)
                {
                    ncolor = new Color(param[0], param[1], param[2], param[3]);
                }
                else
                {
                    ncolor = new Color(param[0], param[1], param[2]);
                }
                clist.Add(ncolor);
            }
            prop.setColor(tuple(clist));
        }
        */

        public delegate void PropActFunction(Item p, ItemData data);

        /* TODO

        protected static Dictionary<string, (PropActFunction, bool)> prop_act_funcs = new Dictionary<string, (PropActFunction, bool)> {
        {
            "visible",
            (prop_visible, false)},
        {
            "move",
            (prop_move, false)},
        {
            "move_to",
            (prop_move_to, true)},
        {
            "rotate",
            (prop_rotate, false)},
        {
            "rotate_to",
            (prop_rotate_to, true)},
        {
            "scale_to",
            (prop_scale_to, true)},
        {
            "color",
            (prop_color, true)},
        {
            "ptn_set",
            (prop_pattern, false)},
        {
            "ptn_dtl",
            (prop_pattern_detail, true)},
        {
            "pnl_set",
            (prop_panel, false)},
        {
            "pnl_dtl",
            (prop_panel_detail, true)},
        {
            "metallic",
            (prop_metallic, true)},
        {
            "emission",
            (prop_emission, true)},
        {
            "alpha",
            (prop_alpha, true)},
        {
            "line",
            (prop_line, true)},
        {
            "shadow_c",
            (prop_shadow_color, true)},
        {
            "light_cancel",
            (prop_light_cancel, true)},
        {
            "option",
            (prop_option, false)},
        {
            "fk_active",
            (prop_fk_enable, false)},
        {
            "fk_set",
            (prop_fk_set, true)},
        {
            "db_active",
            (prop_dynamicbone_enable, false)},
        {
            "anim_spd",
            (prop_anime_speed, true)},
        {
            "anim_ptn",
            (prop_anime_pattern, false)},
        {
            "enable",
            (prop_enable, false)},
        {
            "intensity",
            (prop_intensity, true)},
        {
            "shadow",
            (prop_shadow, false)},
        {
            "range",
            (prop_range, true)},
        {
            "angle",
            (prop_angle, true)},
        {
            "route_p",
            (prop_route, false)},
        {
            "route_f",
            (prop_route, false)}};

    
        */
    }
}
