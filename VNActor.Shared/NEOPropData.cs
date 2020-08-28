using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace VNActor
{
    public class NEOPropData : IDataClass
    {
        public Vector3 position;
        public Vector3 rotation;
        public bool visible;

        public NEOPropData()
        {

        }

        public NEOPropData(Prop p)
        {
            visible = p.Visible;
            position = p.Position;
            rotation = p.Rotation;
        }

        public void Apply(Prop p)
        {                  
            p.Position = position;
            p.Rotation = rotation;
            p.Visible = visible;
        }
    }
}
