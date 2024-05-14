using MessagePack;
using Studio;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace VNActor
{
    public partial class Item : IVNObject<Item>
    {

        public class ItemData : NEOItemData, IDataClass<Item>
        {
            // Distinct
            public bool option;
            public bool fk_active;
            public Metallic_s[] metallic;
            public int anim_ptn;

            public ItemData() : base()
            {

            }

            public override void Apply(Item i)
            {
                base.Apply(i);
                if (i.hasOption)
                {
                    i.option = option;
                }
                if (i.hasMetallic)
                {
                    i.metallic = metallic;
                }
                if (i.IsFk)
                {
                    i.fk_enable = fk_active;
                    if (fk_active)
                    {
                        i.Fk = fk_set;
                    }
                }
            }

            public ItemData(Item i) : base(i)
            {
                if (i.hasMetallic)
                {
                    metallic = i.metallic;
                }
                if (i.hasOption)
                {
                    option = i.option;
                }
                if (i.IsFk)
                {
                    fk_active = i.fk_enable;
                    fk_set = fk_active ? i.Fk : null;
                }
                if (i.hasAnimePattern)
                {
                    anim_ptn = i.anime_pattern;
                }
            }
            /*
            if (i.isLight)
            {
                color = i.color();
                enable = i.enable;
                intensity = i.intensity;
                shadow = i.shadow;
                if (i.hasRange)
                {
                    range = i.range();
                }
                if (i.hasAngle)
                {
                    angle = i.angle();
                }
            }
            if (i.isRoute)
            {
                if (Utils.is_ini_value_true("ExportProp_RouteFull"))
                {
                    route_f = i.route_full();
                }
                else
                {
                    route_p = i.route_play();
                }
            }
            */
        }


        [MessagePackObject]
        public struct Metallic_s
        {
            [Key(0)]
            public float metallic;
            [Key(1)]
            public float glossiness;
        }

        public Dictionary<int, Color> color
        {
            set
            {
                // color : a tuple of UnityEngine.Color
                if (this.IsColorable)
                {
                    var i = 0;
                    if (objctrl.useColor[0] && color.ContainsKey(i))
                    {
                        objctrl.itemInfo.colors[0].mainColor = value[i];
                    }
                    i = 1;
                    if (objctrl.useColor[1] && color.ContainsKey(i))
                    {
                        objctrl.itemInfo.colors[1].mainColor = value[i];
                    }
                    i = 2;
                    if (objctrl.useColor[2] && color.ContainsKey(i))
                    {
                        objctrl.itemInfo.colors[2].mainColor = value[i];
                    }
                    i = 3;
                    if (objctrl.useColor4 && color.ContainsKey(i))
                    {
                        objctrl.itemInfo.colors[3].mainColor = value[i];
                    }
                    objctrl.UpdateColor();
                }
            }
            get
            {
                // return a tuple of used color
                if (this.IsColorable)
                {
                    var cl = new Dictionary<int, Color>();
                    if (this.objctrl.useColor[0])
                    {
                        cl[0] = this.objctrl.itemInfo.colors[0].mainColor;
                    }
                    if (this.objctrl.useColor[1])
                    {
                        cl[1] = this.objctrl.itemInfo.colors[1].mainColor;
                    }
                    if (this.objctrl.useColor[2])
                    {
                        cl[2] = this.objctrl.itemInfo.colors[2].mainColor;
                    }
                    if (this.objctrl.useColor4)
                    {
                        cl[3] = this.objctrl.itemInfo.colors[3].mainColor;
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
                if (!this.IsItem)
                {
                    return false;
                }
                foreach (var n in this.objctrl.usePattern)
                {
                    if (n)
                    {
                        return true;
                    }
                    return false;
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
                        if (this.objctrl.usePattern[i])
                        {
                            this.objctrl.itemInfo.colors[i].pattern.key = value[i].key;
                            this.objctrl.itemInfo.colors[i].pattern.filePath = value[i].filepath;
                            this.objctrl.itemInfo.colors[i].pattern.clamp = value[i].clamp;
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
                            var pi = this.objctrl.itemInfo.colors[i].pattern;
                            pt[i] = (new Pattern(pi.key, pi.filePath, pi.clamp));
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
                        if (this.objctrl.usePattern[i])
                        {
                            this.objctrl.itemInfo.colors[i].pattern.color = value[i].color;
                            this.objctrl.itemInfo.colors[i].pattern.ut = value[i].ut;
                            this.objctrl.itemInfo.colors[i].pattern.vt = value[i].vt;
                            this.objctrl.itemInfo.colors[i].pattern.us = value[i].us;
                            this.objctrl.itemInfo.colors[i].pattern.vs = value[i].vs;
                            this.objctrl.itemInfo.colors[i].pattern.rot = value[i].rot;
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
                            var pi = this.objctrl.itemInfo.colors[i].pattern;
                            pt[i] = new PatternDetail_s(pi.color, pi.ut, pi.vt, pi.us, pi.vs, pi.rot);
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
                return this.IsItem && this.objctrl.checkPanel;
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
                    PatternInfo p0 = this.objctrl.itemInfo.colors[0].pattern;
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
                    var p0 = this.objctrl.itemInfo.colors[0].pattern;
                    this.objctrl.itemInfo.colors[0].mainColor = value.color;
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
                    var p0 = this.objctrl.itemInfo.colors[0].pattern;
                    return new PanelDetail_s(this.objctrl.itemInfo.colors[0].mainColor, p0.ut, p0.vt, p0.us, p0.vs, p0.rot);
                }
                else
                {
                    throw new Exception("no panel to get");
                }
            }
        }

        public bool hasMetallic
        {
            get
            {
                if (!this.IsColorable)
                {
                    return false;
                }
                foreach (var n in this.objctrl.useMetallic)
                {
                    if (n)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        public Metallic_s[] metallic
        {
            get
            {
                if (hasMetallic)
                {
                    var length = this.objctrl.useMetallic.Length;
                    var mv = new Metallic_s[length];
                    for (int i = 0; i < length; i++)
                    {
                        if (objctrl.useMetallic[i])
                        {
                            mv[i] = new Metallic_s { metallic = objctrl.itemInfo.colors[i].metallic, glossiness = objctrl.itemInfo.colors[i].glossiness };
                        }
                    }
                    return mv;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                // param: a set of ((metallic, glossiness), (metallic, glossiness), ...)

                if (value is null) return;

                if (this.hasMetallic)
                {
                    for (int i = 0; i < objctrl.useMetallic.Length; i++)
                    {
                        if (this.objctrl.useMetallic[i])
                        {
                            this.objctrl.itemInfo.colors[i].metallic = value[i].metallic;
                            this.objctrl.itemInfo.colors[i].glossiness = value[i].glossiness;
                        }
                    }
                    this.objctrl.UpdateColor();
                }
            }
        }

        public bool hasEmission
        {
            get
            {
                if (!this.IsItem)
                {
                    return false;
                }
                else
                {
                    return this.objctrl.CheckEmission;
                }
            }
        }

        public bool hasAlpha
        {
            get
            {
                return this.IsColorable && this.objctrl.CheckAlpha;
            }
        }

        public bool option
        {
            set
            {
                // param: True/False for item option setting
                this.objctrl.SetOptionVisible(value);
            }
            get
            {
                if (hasOption && objctrl.itemInfo.option != null && this.objctrl.itemInfo.option.Count > 0)
                {
                    return this.objctrl.itemInfo.option[0];
                }
                else
                {
                    throw new Exception("no option");
                }
            }
        }

        public bool hasOption
        {
            get
            {
                return this.IsItem && this.objctrl.CheckOption;
            }
        }

        // fk enable

        public bool fk_enable
        {
            set
            {
                // param: fk enable/disable
                if (this.IsFk)
                {
                    this.objctrl.ActiveFK(value);
                }
            }
            get
            {
                if (this.IsFk)
                {
                    return this.objctrl.itemInfo.enableFK;
                }
                else
                {
                    return false;
                }
            }
        }

        public bool hasAnimePattern
        {
            get
            {
                return this.IsItem && this.IsAnime && this.objctrl.CheckAnimePattern;
            }
        }

        public int anime_pattern
        {
            get
            {
                if (this.hasAnimePattern)
                {
                    return this.objctrl.itemInfo.animePattern;
                }
                else
                {
                    throw new Exception("No pattern");
                }
            }
            set
            {
                // param: anime pattern no
                if (this.hasAnimePattern)
                {
                    this.objctrl.SetAnimePattern(value);
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

        public static void prop_pattern(Item prop, Dictionary<int, Pattern> param)
        {
            // param: a set of ((key, filepath, clamp), (key, filepath, clamp), (key, filepath, clamp))
            prop.pattern = param;
        }

        public static void prop_pattern_detail(Item prop, Dictionary<int, PatternDetail_s> param)
        {
            // param: a set of ((color, ut, vt, us, vs, rot), (color, ut, vt, us, vs, rot), (color, ut, vt, us, vs, rot))
            prop.pattern_detail = param;
        }

        public static void prop_panel(Item prop, Panel param)
        {
            // param: a set of (filepath, clamp)
            prop.panel = param;
        }

        public static void prop_panel_detail(Item prop, PanelDetail_s param)
        {
            // param: a set of (color, ut, vt, us, vs, rot)
            prop.panel_detail = param;
        }

        public static void prop_metallic(Item prop, Metallic_s[] param)
        {
            // param: a list of ((metallic, glossiness), (metallic, glossiness), ...)
            prop.metallic = param;
        }

        public static void prop_emission(Item prop, Emission_s param)
        {
            // param: (color, power)
            prop.Emission = param;
        }

        public static void prop_alpha(Item prop, float param)
        {
            // param = 0~1
            prop.Alpha = param;
        }

        public static void prop_option(Item prop, object param)
        {
            // param = 0(hide)/1(show)
            prop.option = (bool)param;
        }

        public static void prop_fk_enable(Item prop, object param)
        {
            // param = 0/1
            prop.fk_enable = (bool)param;
        }

        public static void prop_anime_pattern(Item prop, int param)
        {
            // param = pattern index
            prop.anime_pattern = param;
        }

        /*
        public delegate void PropActFunction(Item p, PropData data);

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
            "color_neo",
            (prop_color_neo, true)},
        {
            "color_cs",
            (prop_color_charastudio, true)},
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
