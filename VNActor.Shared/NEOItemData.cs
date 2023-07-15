using System.Collections.Generic;
using MessagePack;
using UnityEngine;
using static VNActor.Item;

namespace VNActor
{
    [MessagePackObject(true)]
    public abstract class NEOItemData : NEOPropData, IDataClass<Item>
    {
        public float? alpha;
        public float? anim_spd;
        public Dictionary<int, Color> color;
        public bool? db_active;
        public Emission_s? emission;
        public List<Vector3> fk_set;
        public float? light_cancel;
        public Line_s? line;
        public PanelDetail_s? pnl_dtl;
        public Panel? pnl_set;
        public Dictionary<int, PatternDetail_s> ptn_dtl;
        public Dictionary<int, Pattern> ptn_set;
        public Vector3 scale;
        public Color? shadow_color;
       
        public NEOItemData()
        {
        }

        public NEOItemData(Item i) : base(i)
        {
            // export full status of prop
            if (!visible) return;
            scale = i.Scale;
            if (i.IsColorable) color = i.color;
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

            if (i.hasEmission) emission = i.Emission;
            if (i.hasAlpha) alpha = i.Alpha;
            if (i.IsDynamicBone) db_active = i.DynamicBoneEnable;
            if (i.IsAnime) anim_spd = i.AnimeSpeed;
            if (i.IsDynamicBone) db_active = i.DynamicBoneEnable;
        }

        public virtual void Apply(Item i)
        {
            // export full status of prop
            base.Apply(i);
            if (visible)
            {
                i.Scale = scale;
                if (anim_spd is float f) i.AnimeSpeed = f;
                if (i.IsColorable && color is Dictionary<int, Color>) i.color = color;
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

                if (i.hasEmission && emission is Emission_s e) i.Emission = e;
                if (i.hasAlpha && alpha is float alpha_set) i.Alpha = alpha_set;
                if (i.IsDynamicBone)
                    if (db_active is bool b)
                        i.DynamicBoneEnable = b;
            }
        }
    }
}