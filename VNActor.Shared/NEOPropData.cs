using MessagePack;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace VNActor
{
    [MessagePackObject]
    public class NEOPropData : IDataClass<Prop>
    {
        [Key("Position")]
        public Vector3 position;
        [Key("Rotation")]
        public Vector3 rotation;
        [Key("Visible")]
        public bool visible;

        public NEOPropData()
        {

        }

        public NEOPropData(Prop p)
        {
            visible = p.Visible;
            if (visible)
            {
                position = p.Position;
                rotation = p.Rotation;
            }
        }

        public void Apply(Prop p)
        {                  
            p.Visible = visible;
            if (visible)
            {
                p.Position = position;
                p.Rotation = rotation;
            }
        }
    }
}
