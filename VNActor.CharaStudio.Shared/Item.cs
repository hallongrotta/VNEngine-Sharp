﻿using System;
using System.Collections.Generic;
using System.Linq;
using MessagePack;
using UnityEngine;

namespace VNActor
{
    public partial class Item : IVNObject<Item>
    {
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

        public Dictionary<int, Color> color
        {
            set
            {
                if (!IsColorable || value is null) return;
                var i = 0;
                if (objctrl.useColor[0] && i < color.Count && color.ContainsKey(i))
                    objctrl.itemInfo.color[0] = value[i];
                i = 1;
                if (objctrl.useColor[1] && i < color.Count && color.ContainsKey(i))
                    objctrl.itemInfo.color[1] = value[i];
                i = 2;
                if (objctrl.useColor[2] && i < color.Count && color.ContainsKey(i))
                    objctrl.itemInfo.color[2] = value[i];
                i = 3;
                if (objctrl.useColor4 && i < color.Count && color.ContainsKey(i))
                    objctrl.itemInfo.color[7] = value[i];
                objctrl.UpdateColor();
            }
            get
            {
                // return a tuple of used color
                if (!IsColorable) return null;
                var cl = new Dictionary<int, Color>();
                if (objctrl.useColor[0]) cl[0] = objctrl.itemInfo.color[0];
                if (objctrl.useColor[1]) cl[1] = objctrl.itemInfo.color[1];
                if (objctrl.useColor[2]) cl[2] = objctrl.itemInfo.color[2];
                if (objctrl.useColor4) cl[4] = objctrl.itemInfo.color[7];
                return cl;

            }
        }

        public bool hasPattern
        {
            get
            {
                return IsItem && objctrl.usePattern.Any(n => n);
            }
        }

        public Dictionary<int, Pattern> pattern
        {
            set
            {
                // param: a set of ((key, filepath, clamp), (key, filepath, clamp), (key, filepath, clamp))
                if (!hasPattern || value is null) return;
                foreach (var i in Enumerable.Range(0, objctrl.usePattern.Length))
                {
                    if (!objctrl.usePattern[i] || !value.ContainsKey(i)) continue;
                    objctrl.itemInfo.pattern[i].key = value[i].key;
                    objctrl.itemInfo.pattern[i].filePath = value[i].filepath;
                    objctrl.itemInfo.pattern[i].clamp = value[i].clamp;
                }

                objctrl.SetupPatternTex();
                objctrl.UpdateColor();
            }
            get
            {
                if (!hasPattern) return null;
                var pt = new Dictionary<int, Pattern>();
                foreach (var i in Enumerable.Range(0, objctrl.usePattern.Length))
                {
                    if (!objctrl.usePattern[i]) continue;
                    var pi = objctrl.itemInfo.pattern[i];
                    pt[i] = new Pattern {key = pi.key, filepath = pi.filePath, clamp = pi.clamp};
                }

                return pt;
            }
        }

        public Dictionary<int, PatternDetail_s> pattern_detail
        {
            set
            {
                // param: a set of ((color, ut, vt, us, vs, rot), (color, ut, vt, us, vs, rot), (color, ut, vt, us, vs, rot))
                if (!hasPattern || value is null) return;
                foreach (var i in Enumerable.Range(0, objctrl.usePattern.Length))
                {
                    if (!objctrl.usePattern[i] || !value.ContainsKey(i)) continue;
                    objctrl.itemInfo.color[i + 3] = value[i].color;
                    objctrl.itemInfo.pattern[i].ut = value[i].ut;
                    objctrl.itemInfo.pattern[i].vt = value[i].vt;
                    objctrl.itemInfo.pattern[i].us = value[i].us;
                    objctrl.itemInfo.pattern[i].vs = value[i].vs;
                    objctrl.itemInfo.pattern[i].rot = value[i].rot;
                }

                objctrl.UpdateColor();
            }
            get
            {
                if (!hasPattern) return null;
                var pt = new Dictionary<int, PatternDetail_s>();
                foreach (var i in Enumerable.Range(0, objctrl.usePattern.Length))
                {
                    if (!objctrl.usePattern[i]) continue;
                    var color = objctrl.itemInfo.color[i + 3];
                    var pi = objctrl.itemInfo.pattern[i];
                    pt[i] = new PatternDetail_s
                        {color = color, ut = pi.ut, vt = pi.vt, us = pi.us, vs = pi.vs, rot = pi.rot};
                }

                return pt;

            }
        }

        public bool hasPanel => IsItem && objctrl.checkPanel;

        public Panel panel
        {
            set
            {
                // param: a set of (filepath, clamp)
                if (!hasPanel) return;
                objctrl.SetMainTex(value.filepath);
                objctrl.SetPatternClamp(0, value.clamp);
            }
            get
            {
                if (!hasPanel) throw new Exception("Item does not have panel.");
                var pi = objctrl.itemInfo.panel;
                var p0 = objctrl.itemInfo.pattern[0];
                return new Panel {filepath = pi.filePath, clamp = p0.clamp};

            }
        }

        public PanelDetail_s panel_detail
        {
            set
            {
                // param: a set of (color, ut, vt, us, vs, rot)
                if (!hasPanel) return;
                var p0 = objctrl.itemInfo.pattern[0];
                objctrl.itemInfo.color[0] = value.color;
                p0.ut = value.ut;
                p0.vt = value.vt;
                p0.us = value.us;
                p0.vs = value.vs;
                p0.rot = value.rot;
                objctrl.UpdateColor();
            }
            get
            {
                if (!hasPanel) throw new Exception("Item does not have panel.");
                var p0 = objctrl.itemInfo.pattern[0];
                return new PanelDetail_s
                {
                    color = objctrl.itemInfo.color[0], ut = p0.ut, vt = p0.vt, us = p0.us, vs = p0.vs, rot = p0.rot
                };

            }
        }

        public bool hasEmission => IsItem && objctrl.checkEmission;

        public bool hasAlpha => IsColorable && objctrl.checkAlpha;

        public bool hasLine => IsItem && objctrl.checkLine;

        public Line_s line
        {
            set
            {
                // param: (lineColor, lineWidth)
                if (!hasLine) return;
                objctrl.SetLineColor(value.color);
                objctrl.SetLineWidth(value.lineWidth);
            }
            get
            {
                if (hasLine)
                    return new Line_s {color = objctrl.itemInfo.lineColor, lineWidth = objctrl.itemInfo.lineWidth};
                throw new Exception();
            }
        }

        public bool hasShadowColor => IsItem && objctrl.checkShadow;

        public Color shadow_color
        {
            get
            {
                if (hasShadowColor)
                    return objctrl.itemInfo.color[6];
                throw new Exception();
            }
            set
            {
                // param: color
                if (!hasShadowColor) return;
                objctrl.itemInfo.color[6] = value;
                objctrl.UpdateColor();
            }
        }

        public float light_cancel
        {
            set
            {
                // param: light cancel
                if (hasLightCancel) objctrl.SetLightCancel(value);
            }
            get
            {
                if (hasLightCancel)
                    return objctrl.itemInfo.lightCancel;
                throw new Exception();
            }
        }

        public bool hasLightCancel => IsItem && objctrl.checkLightCancel;

        public static void prop_line(Item prop, ItemData param)
        {
            // param: (color, width)
            prop.line = (Line_s) param.line;
        }

        public static void prop_shadow_color(Item prop, ItemData param)
        {
            // param: shadow color
            prop.shadow_color = (Color) param.shadow_color;
        }

        public static void prop_light_cancel(Item prop, ItemData param)
        {
            // param: light cancel
            prop.light_cancel = (float) param.light_cancel;
        }

        public static void prop_color(Item prop, Dictionary<int, Color> param)
        {
            prop.color = param;
        }


        [MessagePackObject(true)]
        public class ItemData : NEOItemData, IDataClass<Item>
        {
            public ItemData()
            {
            }

            public ItemData(Item i) : base(i)
            {
                if (i.hasLine) line = i.line;
                if (i.hasShadowColor) shadow_color = i.shadow_color;
                if (i.hasLightCancel) light_cancel = i.light_cancel;
                if (i.IsFk) fk_set = i.Fk;
            }

            public override void Apply(Item i)
            {
                base.Apply(i);
                if (i.hasLine)
                    if (line is Line_s line_s)
                        line = line_s;
                if (i.hasShadowColor)
                    if (shadow_color is Color c)
                        shadow_color = c;
                if (i.hasLightCancel)
                    if (light_cancel is float cancel)
                        light_cancel = cancel;
                if (i.IsFk) i.Fk = fk_set;
            }
        }

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