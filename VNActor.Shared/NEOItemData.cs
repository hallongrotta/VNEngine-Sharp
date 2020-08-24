using System.Collections.Generic;
using UnityEngine;
using static VNActor.Item;

namespace VNActor
{
    public class NEOItemData : IDataClass
    {
        public bool visible;
        public Vector3 position;
        public Vector3 rotation;
        public Vector3 scale;
        public Dictionary<int, Color> color;
        public float? alpha;
        public Panel? pnl_set;
        public PanelDetail_s? pnl_dtl;
        public Emission_s? emission;
        public List<Vector3> fk_set;
        public float? anim_spd;
        public Dictionary<int, Pattern> ptn_set;
        public Dictionary<int, PatternDetail_s> ptn_dtl;
        public float? light_cancel;
        public Line_s? line;
        public Color? shadow_color;
        public bool? db_active;

        public NEOItemData()
        {

        }

        public NEOItemData(Item i)
        {
            // export full status of prop
            visible = i.Visible;
            position = i.Position;
            rotation = i.Rotation;
            scale = i.scale;
            if (i.isColorable)
            {
                color = i.color;
            }
            if (i.hasPattern)
            {
                ptn_set = i.pattern;
                ptn_dtl = i.pattern_detail;
            }
            if (i.hasPanel)
            {
                pnl_set = i.panel;
                pnl_dtl = i.panel_detail;
            }
            if (i.hasEmission)
            {
                emission = i.emission;
            }
            if (i.hasAlpha)
            {
                alpha = i.alpha;
            }
            if (i.isDynamicBone)
            {
                db_active = i.dynamicbone_enable;
            }
            if (i.isAnime)
            {
                anim_spd = i.anime_speed;
            }
            if (i.isDynamicBone)
            {
                db_active = i.dynamicbone_enable;
            }
        }

        virtual public void Apply(Item i)
        {
            // export full status of prop
            i.Visible = visible;
            i.Position = position;
            i.Rotation = rotation;
            i.scale = scale;
            if (anim_spd is float f)
            {
                i.anime_speed = f;
            }
            if (i.isColorable && (color is Dictionary<int, Color>))
            {
                i.color = color;
            }
            if (i.hasPattern)
            {
                i.pattern = ptn_set;
                i.pattern_detail = ptn_dtl;
            }

            if (i.hasPanel && pnl_set is Panel panel_set && pnl_dtl is PanelDetail_s detail)
            {
                i.panel = panel_set;
                i.panel_detail = detail;
            }
            if (i.hasEmission && emission is Emission_s e)
            {
                i.emission = e;
            }
            if (i.hasAlpha && alpha is float alpha_set)
            {
                i.alpha = alpha_set;
            }
            if (i.isDynamicBone)
            {
                if (db_active is bool b)
                {
                    i.dynamicbone_enable = b;
                }
            }
        }
    }
}
