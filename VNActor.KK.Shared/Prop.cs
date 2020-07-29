using Studio;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace VNActor
{
    public partial class Prop : IProp
    {

        new public OCIItem objctrl;

        public struct PropData : IDataClass
        {
            // Prop
            internal bool visible;
            internal Vector3 move_to;
            internal Vector3 rotate_to;
            internal Vector3 scale_to;
            internal Dictionary<int, Color> color;
            internal float? alpha;
            internal (string, bool)? pnl_set;
            public (Color color, float ut, float vt, float us, float vs, float rot)? pnl_dtl;
            internal (Color, float)? emission;
            public List<Vector3> fk_set;
            internal float? anim_spd;
            public Dictionary<int, (int key, string filepath, bool clamp)> ptn_set;
            public Dictionary<int, (Color color, float ut, float vt, float us, float vs, float rot)> ptn_dtl;
            public float? light_cancel;
            internal (Color color, float lineWidth)? line;
            internal Color? shadow_color;
            public bool? db_active;

            public void Remove(string key)
            {
                throw new NotImplementedException();
            }

            public PropData(Prop p)
            {
                // export full status of prop
                visible = p.visible;
                move_to = p.pos;
                rotate_to = p.rot;
                scale_to = p.scale;
                if (p.isAnime)
                {
                    anim_spd = p.get_anime_speed();
                }
                else
                {
                    anim_spd = null;
                }
                if (p.isColorable)
                {
                    color = p.color;
                }
                else
                {
                    color = null;
                }
                if (p.hasPattern)
                {
                    ptn_set = p.pattern;
                    ptn_dtl = p.pattern_detail;
                }
                else
                {
                    ptn_set = null;
                    ptn_dtl = null;
                }

                if (p.hasPanel)
                {
                    pnl_set = p.panel;
                    pnl_dtl = p.panel_detail;
                }
                else
                {
                    pnl_set = null;
                    pnl_dtl = null;
                }
                if (p.hasEmission)
                {
                    emission = p.emission;
                }
                else
                {
                    emission = null;
                }
                if (p.hasAlpha)
                {
                    alpha = p.alpha;
                }
                else
                {
                    alpha = null;
                }
                if (p.hasLine)
                {
                    line = p.line;
                }
                else
                {
                    line = null;
                }
                if (p.hasShadowColor)
                {
                    shadow_color = p.shadow_color;
                }
                else
                {
                    shadow_color = null;
                }
                if (p.hasLightCancel)
                {
                    light_cancel = p.light_cancel;
                }
                else
                {
                    light_cancel = null;
                }
                if (p.isFK)
                {
                    fk_set = p.export_fk_bone_info();
                }
                else
                {
                    fk_set = null;
                }
                if (p.isDynamicBone)
                {
                    db_active = p.dynamicbone_enable;
                }
                else
                {
                    db_active = null;
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
                        this.objctrl.itemInfo.color[0] = color[i];
                    }
                    i = 1;
                    if (this.objctrl.useColor[1] && i < color.Count && color.ContainsKey(i))
                    {
                        this.objctrl.itemInfo.color[1] = color[i];
                    }
                    i = 2;
                    if (this.objctrl.useColor[2] && i < color.Count && color.ContainsKey(i))
                    {
                        this.objctrl.itemInfo.color[2] = color[i];
                    }
                    i = 3;
                    if (this.objctrl.useColor4 && i < color.Count && color.ContainsKey(i))
                    {
                        this.objctrl.itemInfo.color[7] = color[i];
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

        public Dictionary<int, (int key, string filepath, bool clamp)> pattern
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
                    var pt = new Dictionary<int, (int key, string filepath, bool clamp)>();
                    foreach (var i in Enumerable.Range(0, this.objctrl.usePattern.Length))
                    {
                        if (this.objctrl.usePattern[i])
                        {
                            var pi = this.objctrl.itemInfo.pattern[i];
                            pt[i] = (pi.key, pi.filePath, pi.clamp);
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

        public Dictionary<int, (Color color, float ut, float vt, float us, float vs, float rot)> pattern_detail
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
                    var pt = new Dictionary<int, (Color color, float ut, float vt, float us, float vs, float rot)>();
                    foreach (var i in Enumerable.Range(0, this.objctrl.usePattern.Length))
                    {
                        if (this.objctrl.usePattern[i])
                        {
                            var color = this.objctrl.itemInfo.color[i + 3];
                            var pi = this.objctrl.itemInfo.pattern[i];
                            pt[i] = (color, pi.ut, pi.vt, pi.us, pi.vs, pi.rot);
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

        public (string filepath, bool clamp) panel
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
                    return (pi.filePath, p0.clamp);
                }
                else
                {
                    throw new Exception("Prop does not have panel.");
                }
            }
        }

        public (Color color, float ut, float vt, float us, float vs, float rot) panel_detail
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
                    return (this.objctrl.itemInfo.color[0], p0.ut, p0.vt, p0.us, p0.vs, p0.rot);
                }
                else
                {
                    throw new Exception("Prop does not have panel.");
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

        public (Color color, float power) emission
        {
            set
            {
                // param: (color, power)
                if (this.hasEmission)
                {
                    var eColor = value.color;
                    var ePower = value.power;
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
                    return (eColor, ePower);
                }
                else
                {
                    throw new Exception();
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

        public bool hasLine
        {
            get
            {
                return this.isItem && this.objctrl.checkLine;
            }
        }

        public (Color color, float lineWidth) line
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
                    return (this.objctrl.itemInfo.lineColor, this.objctrl.itemInfo.lineWidth);
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

        public bool isRoute
        {
            get
            {
                return base.objctrl is OCIRoute;
            }
        }


        public IDataClass export_full_status()
        {
            return new PropData(this);
        }

        public static void prop_line(Prop prop, PropData param)
        {
            // param: (color, width)
            prop.line = ((Color color, float lineWidth))param.line;
        }

        public static void prop_shadow_color(Prop prop, PropData param)
        {
            // param: shadow color
            prop.shadow_color = (Color)param.shadow_color;
        }

        public static void prop_light_cancel(Prop prop, PropData param)
        {
            // param: light cancel
            prop.light_cancel = (float)param.light_cancel;
        }

        public static void prop_color(Prop prop, Dictionary<int, Color> param)
        {
            prop.color = param;
        }

        /* TODO

        public static void prop_color(Prop prop, Color param)
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

        public delegate void PropActFunction(Prop p, PropData data);

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
