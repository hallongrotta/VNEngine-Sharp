using Studio;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System.Linq;

namespace VNActor
{
    public partial class Prop : IProp
    {
        public struct PropData : IDataClass
        {
            // Prop
            internal bool visible;
            internal bool option;
            internal bool db_active;
            internal Vector3 move_to;
            internal Vector3 rotate_to;
            internal Vector3 scale_to;
            internal Color[] color;
            internal float alpha;
            internal Pattern[] ptn_set;
            internal PatternDetail[] ptn_dtl;
            internal (string, bool) pnl_set;
            internal Prop.PatternDetail pnl_dtl;
            internal List<(float, float)> metallic;
            internal (Color, float) emission;
            internal bool fk_active;
            public List<Vector3> fk_set;
            internal float anim_spd;
            internal int anim_ptn;
            //Light
            internal object enable;
            internal float intensity;
            internal bool shadow;
            internal float range;
            internal float angle;
            //Route
            internal Route route_f;
            internal bool route_p;

            public void Remove(string key)
            {
                throw new NotImplementedException();
            }
        }

        OCIItem objctrl;
        private object enable;

        public void set_color(Color[] color)
        {
            // color : a tuple of UnityEngine.Color
            if (this.isColorable)
            {

                OCIItem item = (OCIItem)this.objctrl;

                var i = 0;
                if (item.useColor[0] && i < color.Length && color[i] != null)
                {
                    item.itemInfo.colors[0].mainColor = Utils.tuple4_2_color(color[i]);
                }
                i = 1;
                if (item.useColor[1] && i < color.Length && color[i] != null)
                {
                    item.itemInfo.colors[1].mainColor = Utils.tuple4_2_color(color[i]);
                }
                i = 2;
                if (item.useColor[2] && i < color.Length && color[i] != null)
                {
                    item.itemInfo.colors[2].mainColor = Utils.tuple4_2_color(color[i]);
                }
                i = 3;
                if (item.useColor4 && i < color.Length && color[i] != null)
                {
                    item.itemInfo.colors[3].mainColor = Utils.tuple4_2_color(color[i]);
                }
                item.UpdateColor();
            }
        }

        new public Color[] get_color()
        {
            // return a tuple of used color
            if (this.isColorable)
            {
                var cl = new Color[4];
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
            } else
            {
                throw new Exception("not colorable");
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
                    return false;
                }
                return false;
            }
        }

        public void set_pattern(Pattern[] param)
        {
            // param: a set of ((key, filepath, clamp), (key, filepath, clamp), (key, filepath, clamp))
            if (this.hasPattern)
            {
                foreach (var i in Enumerable.Range(0, this.objctrl.usePattern.Length))
                {
                    if (this.objctrl.usePattern[i])
                    {
                        this.objctrl.itemInfo.colors[i].pattern.key = param[i].key;
                        this.objctrl.itemInfo.colors[i].pattern.filePath = param[i].filepath;
                        this.objctrl.itemInfo.colors[i].pattern.clamp = param[i].clamp;
                    }
                }
                this.objctrl.SetupPatternTex();
                this.objctrl.UpdateColor();
            }
        }

        public Pattern[] get_pattern()
        {
            if (this.hasPattern)
            {
                var pt = new Pattern[this.objctrl.usePattern.Length];
                foreach (var i in Enumerable.Range(0, this.objctrl.usePattern.Length))
                {
                    if (this.objctrl.usePattern[i])
                    {
                        var pi = this.objctrl.itemInfo.colors[i].pattern;
                        pt[i] = (new Pattern(pi.key, pi.filePath, pi.clamp));
                    }
                }
                return pt;
            } else
            {
                return null;
            }
        }

        public void set_pattern_detail(PatternDetail[] param)
        {
            // param: a set of ((color, ut, vt, us, vs, rot), (color, ut, vt, us, vs, rot), (color, ut, vt, us, vs, rot))
            if (this.hasPattern)
            {
                foreach (var i in Enumerable.Range(0, this.objctrl.usePattern.Length))
                {
                    if (this.objctrl.usePattern[i])
                    {
                        this.objctrl.itemInfo.colors[i].pattern.color = param[i].color;
                        this.objctrl.itemInfo.colors[i].pattern.ut = param[i].ut;
                        this.objctrl.itemInfo.colors[i].pattern.vt = param[i].vt;
                        this.objctrl.itemInfo.colors[i].pattern.us = param[i].us;
                        this.objctrl.itemInfo.colors[i].pattern.vs = param[i].vs;
                        this.objctrl.itemInfo.colors[i].pattern.rot = param[i].rot;
                    }
                }
                this.objctrl.UpdateColor();
            }
        }

        public struct Pattern
        {
            public int key;
            public string filepath;
            public bool clamp;

            public Pattern(int key, string filepath, bool clamp)
            {
                this.key = key;
                this.filepath = filepath;
                this.clamp = clamp;
            }
        }

        public struct PatternDetail
        {
            public Color color;
            public float ut;
            public float vt; 
            public float us; 
            public float vs; 
            public float rot;

            public PatternDetail(Color color, float ut, float vt, float us, float vs, float rot)
            {
                this.color = color;
                this.ut = ut;
                this.vt = vt;
                this.us = us;
                this.vs = vs;
                this.rot = rot;
            }
        }

        public PatternDetail[] get_pattern_detail()
        {
            if (this.hasPattern)
            {
                var pt = new PatternDetail[this.objctrl.usePattern.Length];

                foreach (var i in Enumerable.Range(0, this.objctrl.usePattern.Length))
                {
                    if (this.objctrl.usePattern[i])
                    {
                        var pi = this.objctrl.itemInfo.colors[i].pattern;
                        pt[i] = new PatternDetail(pi.color, pi.ut, pi.vt, pi.us, pi.vs, pi.rot);
                    }
                }
                return pt;
            } else
            {
                return null;
            }
        }

        public bool hasPanel
        {
            get
            {
                return this.isItem && this.objctrl.checkPanel;
            }
        }

        public void set_panel((string, bool) param)
        {
            // param: a set of (filepath, clamp)
            if (this.hasPanel)
            {
                this.objctrl.SetMainTex(param.Item1);
                this.objctrl.SetPatternClamp(0, param.Item2);
            }
        }

        public (string, bool) get_panel()
        {
            if (this.hasPanel)
            {
                PatternInfo pi = this.objctrl.itemInfo.panel;
                PatternInfo p0 = this.objctrl.itemInfo.colors[0].pattern;
                return (pi.filePath, p0.clamp);
            } else
            {
                throw new Exception("no panel to get");
            }
        }

        public void set_panel_detail(PatternDetail param)
        {
            // param: a set of (color, ut, vt, us, vs, rot)
            if (this.hasPanel)
            {
                var p0 = this.objctrl.itemInfo.colors[0].pattern;
                this.objctrl.itemInfo.colors[0].mainColor = param.color;
                p0.ut = param.ut;
                p0.vt = param.vt;
                p0.us = param.us;
                p0.vs = param.vs;
                p0.rot = param.rot;
                this.objctrl.UpdateColor();
            }
        }

        public PatternDetail get_panel_detail()
        {
            if (this.hasPanel)
            {
                var p0 = this.objctrl.itemInfo.colors[0].pattern;
                return new PatternDetail(this.objctrl.itemInfo.colors[0].mainColor, p0.ut, p0.vt, p0.us, p0.vs, p0.rot);
            }
            else
            {
                throw new Exception("no panel to get");
            }
        }

        public bool hasMetallic
        {
            get
            {
                if (!this.isColorable)
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

        public void set_metallic(List<(float, float)> param)
        {
            // param: a set of ((metallic, glossiness), (metallic, glossiness), ...)
            if (this.hasMetallic)
            {
                foreach (var i in Enumerable.Range(0, this.objctrl.useMetallic.Length))
                {
                    if (this.objctrl.useMetallic[i])
                    {
                        this.objctrl.itemInfo.colors[i].metallic = param[i].Item1;
                        this.objctrl.itemInfo.colors[i].glossiness = param[i].Item2;
                    }
                }
                this.objctrl.UpdateColor();
            }
        }

        public List<(float, float)> get_metallic()
        {
            if (this.hasMetallic)
            {
                var mv = new List<(float, float)>();
                foreach (var i in Enumerable.Range(0, this.objctrl.useMetallic.Length))
                {
                    if (this.objctrl.useMetallic[i])
                    {
                        mv.Add((this.objctrl.itemInfo.colors[i].metallic, this.objctrl.itemInfo.colors[i].glossiness));
                    }
                    else
                    {
                        //mv.Add(null);
                    }
                }
                return mv;
            }
            else
            {
                return null;
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
                    return this.objctrl.CheckEmission;
                }
            }
        }

        public void set_emission((Color, float) param)
        {
            // param: (color, power)
            if (this.hasEmission)
            {
                var eColor = Utils.tuple4_2_color(param.Item1);
                var ePower = param.Item2;
                //self.objctrl.SetEmissionColor(eColor)
                //self.objctrl.SetEmissionPower(ePower)
                this.objctrl.itemInfo.emissionColor = eColor;
                this.objctrl.itemInfo.emissionPower = ePower;
                this.objctrl.UpdateColor();
            }
        }

        public (Color, float) get_emission()
        {
            if (this.hasEmission)
            {
                var eColor = this.objctrl.itemInfo.emissionColor;
                var ePower = this.objctrl.itemInfo.emissionPower;
                return (eColor, ePower);
            }
            else
            {
                throw new Exception("This item has no emission");
            }
        }

        public bool hasAlpha
        {
            get
            {
                return this.isColorable && this.objctrl.CheckAlpha;
            }
        }

        public void set_alpha(float param)
        {
            // param: 0~1 for alpha
            this.objctrl.SetAlpha(param);
        }

        public float get_alpha()
        {
            if (this.hasAlpha)
            {
                return this.objctrl.itemInfo.alpha;
            }
            else
            {
                throw new Exception("No alpha");
            }
        }

        public bool hasOption
        {
            get
            {
                return this.isItem && this.objctrl.CheckOption;
            }
        }

        public void set_option(bool param)
        {
            // param: True/False for item option setting
            this.objctrl.SetOptionVisible(param);
        }

        public bool get_option()
        {
            if (this.hasOption && this.objctrl.itemInfo.option != null && this.objctrl.itemInfo.option.Count > 0)
            {
                return this.objctrl.itemInfo.option[0];
            }
            else
            {
                throw new Exception("no option");
            }
        }

        // fk enable
        public void set_fk_enable(bool param)
        {
            // param: fk enable/disable
            if (this.isFK)
            {
                this.objctrl.ActiveFK(param);
            }
        }

        public bool get_fk_enable()
        {
            if (this.isFK)
            {
                return this.objctrl.itemInfo.enableFK;
            } else
            {
                return false;
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

        public void set_dynamicbone_enable(bool param)
        {
            // param: dynamic bone (yure) enable/disable
            if (this.isDynamicBone)
            {
                this.objctrl.ActiveDynamicBone(param);
            }
        }

        public bool get_dynamicbone_enable()
        {
            if (this.isDynamicBone)
            {
                return this.objctrl.itemInfo.enableDynamicBone;
            } else
            {
                return false;
            }
        }

        public bool hasAnimePattern
        {
            get
            {
                return this.isItem && this.isAnime && this.objctrl.CheckAnimePattern;
            }
        }

        public void set_anime_pattern(int param)
        {
            // param: anime pattern no
            if (this.hasAnimePattern)
            {
                this.objctrl.SetAnimePattern(param);
            }
        }

        public int get_anime_pattern()
        {
            if (this.hasAnimePattern)
            {
                return this.objctrl.itemInfo.animePattern;
            } else
            {
                throw new Exception("No pattern");
            }
        }

        public bool isRoute
        {
            get
            {
                return base.objctrl is OCIRoute;
            }
        }

        public void set_route_play(bool param)
        {
            // param: 1=start/0=stop
            if (this.isRoute)
            {
                OCIRoute route = (OCIRoute)base.objctrl;
                if (param)
                {
                    route.Play();
                }
                else
                {
                    route.Stop();
                }
            }
        }

        public bool get_route_play()
        {
            if (this.isRoute)
            {
                OCIRoute route = (OCIRoute)base.objctrl;
                return route.isPlay;
            } else
            {
                return false;
            }
        }

        public void set_route_full(bool param)
        {
            this.set_route_play(param);
            return;
        }

        public void set_route_full(Route param)
        {
            // route info, ref to get_route_full
            if (this.isRoute)
            {
                try
                {
                    OCIRoute route = (OCIRoute) base.objctrl;
                    // route_f, full route setting
                    route.Stop();
                    Route cur_status = this.get_route_full();
                    foreach (var i in Enumerable.Range(0, 4))
                    {
                        if (param.pts[i].Equals(cur_status.pts[i]))
                        {
                            continue;
                        }
                        var ri = route.routeInfo;
                        if (i == 0)
                        {
                            // orient
                            ri.orient = param.orient;
                        }
                        else if (i == 1)
                        {
                            // loop
                            ri.loop = param.loop;
                            route.UpdateLine();
                        }
                        else if (i == 2)
                        {
                            // points
                            foreach (var j in Enumerable.Range(0, cur_status.pts.Count))
                            {
                                var pt = param.pts[j];
                                var pt_cur = cur_status.pts[j];
                                if (pt.Equals(pt_cur))
                                {
                                    continue;
                                }
                                var pi = route.routeInfo.route[j];
                                        // pt pos
                                        pi.changeAmount.pos = pt.pt_pos;

                                        // pt rot
                                        pi.changeAmount.rot = pt.rot;

                                        // aid pos
                                        pi.aidInfo.changeAmount.pos = pt.aid_pos;

                                        // speed
                                        pi.speed = pt.speed;

                                        // easeType
                                        pi.easeType = pt.easeType;

                                        // connection
                                        pi.connection = pt.connection;

                                        // link
                                        pi.link = pt.link;
                            }
                            route.UpdateLine();
                        }
                        else if (i == 3)
                        {
                            // active
                            this.set_route_play(param.active);
                        }
                        else
                        {
                            throw new Exception("Unknown route info");
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("VNGE VNActor Error: Can not set route, ", e);
                }
            }
        }

        public struct Point : IEquatable<Point>
        {
            public Vector3 pt_pos;
            public Vector3 rot;
            public Vector3 aid_pos;
            public float speed;
            public StudioTween.EaseType easeType;
            public OIRoutePointInfo.Connection connection;
            public bool link;

            public Point(Vector3 pt_pos, Vector3 rot, Vector3 aid_pos, float speed, StudioTween.EaseType easeType, OIRoutePointInfo.Connection connection, bool link)
            {
                this.pt_pos = pt_pos;
                this.rot = rot;
                this.aid_pos = aid_pos;
                this.speed = speed;
                this.easeType = easeType;
                this.connection = connection;
                this.link = link;
            }

            public override bool Equals(object obj)
            {
                return obj is Point point && Equals(point);
            }

            public bool Equals(Point other)
            {
                return pt_pos.Equals(other.pt_pos) &&
                       rot.Equals(other.rot) &&
                       aid_pos.Equals(other.aid_pos) &&
                       speed == other.speed &&
                       easeType == other.easeType &&
                       connection == other.connection &&
                       link == other.link;
            }
        }

        public struct Route
        {
            public OIRouteInfo.Orient orient;
            public bool loop;
            public List<Point> pts;
            public bool active;

            public Route(OIRouteInfo.Orient orient, bool loop, List<Point> pts, bool active)
            {
                this.orient = orient;
                this.loop = loop;
                this.pts = pts;
                this.active = active;
            }
        }

        public Route get_route_full()
        {
            if (this.isRoute)
            {
                OCIRoute route = (OCIRoute)base.objctrl;
                OIRouteInfo ri = route.routeInfo;
                var pts = new List<Point>();
                foreach (OIRoutePointInfo pi in ri.route)
                {
                    // (pt pos, pt rot, aid pos, speed, easeType, connection, link)
                    pts.Add(new Point(pi.changeAmount.pos, pi.changeAmount.rot, pi.aidInfo.changeAmount.pos, pi.speed, pi.easeType, pi.connection, pi.link) );
                }
                // (orient, loop, points, active)
                Route rs = new Route(ri.orient, ri.loop, pts, ri.active);
                return rs;
            } else
            {
                throw new Exception("not a route");
            }
        }

        public PropData export_full_status()
        {
            // export full status of prop
            var fs = new PropData();
            fs.visible = this.visible;
            fs.move_to = this.pos;
            fs.rotate_to = this.rot;
            if (this.isItem)
            {
                fs.scale_to = this.scale;
                if (this.isColorable)
                {
                    fs.color = this.get_color();
                }
                if (this.hasPattern)
                {
                    fs.ptn_set = this.get_pattern();
                    fs.ptn_dtl = this.get_pattern_detail();
                }
                if (this.hasPanel)
                {
                    fs.pnl_set = this.get_panel();
                    fs.pnl_dtl = this.get_panel_detail();
                }
                if (this.hasMetallic)
                {
                    fs.metallic = this.get_metallic();
                }
                if (this.hasEmission)
                {
                    fs.emission = this.get_emission();
                }
                if (this.hasAlpha)
                {
                    fs.alpha = this.get_alpha();
                }
                if (this.hasOption)
                {
                    fs.option = this.get_option();
                }
                if (this.isFK)
                {
                    fs.fk_active = this.get_fk_enable();
                    if (fs.fk_active)
                    {
                        fs.fk_set = this.export_fk_bone_info();
                    }
                    else
                    {
                        fs.fk_set = null;
                    }
                }
                if (this.isDynamicBone)
                {
                    fs.db_active = this.get_dynamicbone_enable();
                }
                if (this.isAnime)
                {
                    fs.anim_spd = this.get_anime_speed();
                }
                if (this.hasAnimePattern)
                {
                    fs.anim_ptn = this.get_anime_pattern();
                }
            }
            if (this.isLight)
            {
                fs.color = this.get_color();
                fs.enable = this.enable;
                fs.intensity = this.get_intensity;
                fs.shadow = this.get_shadow;
                if (this.hasRange)
                {
                    fs.range = this.get_range();
                }
                if (this.hasAngle)
                {
                    fs.angle = this.get_angle();
                }
            }
            if (this.isRoute)
            {
                if (Utils.is_ini_value_true("ExportProp_RouteFull"))
                {
                    fs.route_f = this.get_route_full();
                }
                else
                {
                    fs.route_p = this.get_route_play();
                }
            }
            return fs;
        }

        public static void prop_pattern(Prop prop, Prop.Pattern[] param)
        {
            // param: a set of ((key, filepath, clamp), (key, filepath, clamp), (key, filepath, clamp))
            prop.set_pattern(param);
        }

        public static void prop_pattern_detail(Prop prop, Prop.PatternDetail[] param)
        {
            // param: a set of ((color, ut, vt, us, vs, rot), (color, ut, vt, us, vs, rot), (color, ut, vt, us, vs, rot))
            prop.set_pattern_detail(param);
        }

        public static void prop_panel(Prop prop, (string, bool) param)
        {
            // param: a set of (filepath, clamp)
            prop.set_panel(param);
        }

        public static void prop_panel_detail(Prop prop, PatternDetail param)
        {
            // param: a set of (color, ut, vt, us, vs, rot)
            prop.set_panel_detail(param);
        }

        public static void prop_metallic(Prop prop, List<(float metallic, float glossiness)> param)
        {
            // param: a list of ((metallic, glossiness), (metallic, glossiness), ...)
            prop.set_metallic(param);
        }

        public static void prop_emission(Prop prop, (Color, float) param)
        {
            // param: (color, power)
            prop.set_emission(param);
        }

        public static void prop_alpha(Prop prop, float param)
        {
            // param = 0~1
            prop.set_alpha(param);
        }

        public static void prop_option(Prop prop, object param)
        {
            // param = 0(hide)/1(show)
            prop.set_option((bool)param);
        }

        public static void prop_fk_enable(Prop prop, object param)
        {
            // param = 0/1
            prop.set_fk_enable((bool)param);
        }

        public static void prop_anime_pattern(Prop prop, int param)
        {
            // param = pattern index
            prop.anime_pattern = param;
        }

        public delegate void PropActFunction(Prop p, PropData data);

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

    }
}
