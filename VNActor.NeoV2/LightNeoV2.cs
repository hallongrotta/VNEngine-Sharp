using Studio;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace VNActor
{
    class LightNeoV2 : Prop
    {

        new OCILight objctrl;
        public LightNeoV2(ObjectCtrlInfo objctrl) : base(objctrl)
        {
        }

        new public void set_color(Color color)
        {
            var c = Utils.tuple4_2_color(color);

            this.objctrl.SetColor(c);
        
        }

        new public Color get_color()
        {
            return this.objctrl.lightInfo.color;
        }

    }
}
