using System;
using System.Collections.Generic;
using System.Text;

namespace VNEngine
{
    public class PropHSNeo : Prop
    {

        public object export_full_status()
        {
            // export full status of prop
            var fs = new Dictionary<object, object>
            {
            };
            fs["visible"] = this.visible;
            fs["move_to"] = this.pos;
            fs["rotate_to"] = this.rot;
            if (this.isItem)
            {
                fs["scale_to"] = this.scale;
            }
            if (this.isAnime)
            {
                fs["anim_spd"] = this.get_anime_speed();
            }
            if (this.isColorable || this.isLight)
            {
                fs["color"] = this.get_color();
            }
            if (this.isFK)
            {
                fs["fk_set"] = this.export_fk_bone_info();
            }
            if (this.isLight)
            {
                fs["enable"] = this.enable;
                fs["intensity"] = this.get_intensity;
                fs["shadow"] = this.get_shadow;
                if (this.hasRange)
                {
                    fs["range"] = this.get_range();
                }
                if (this.hasAngle)
                {
                    fs["angle"] = this.get_angle();
                }
            }
            return fs;
        }
    }
}
