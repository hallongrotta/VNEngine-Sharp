﻿using System;
using MessagePack;
using UnityEngine;

namespace VNActor
{
    [Serializable]
    [MessagePackObject]
    public class NEOPropData : IDataClass<Prop>
    {
        [Key("Position")] public Vector3 position;

        [Key("Rotation")] public Vector3 rotation;

        [Key("Visible")] public bool visible;

        public NEOPropData()
        {
        }

        public NEOPropData(Prop p)
        {
            visible = p.Visible;
            if (!visible) return;
            position = p.Position;
            rotation = p.Rotation;
        }

        public void Apply(Prop p)
        {
            p.Visible = visible;
            if (!visible) return;
            p.Position = position;
            p.Rotation = rotation;
        }
    }
}