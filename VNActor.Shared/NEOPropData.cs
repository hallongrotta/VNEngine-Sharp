using MessagePack;
using UnityEngine;

namespace VNActor
{
    [MessagePackObject]
    public class NEOPropData : IDataClass<Prop>
    {
        [Key("Position")] public Vector3 position;

        [Key("Rotation")] public Vector3 rotation;

        [Key("Visible")] public bool Visible;

        public NEOPropData()
        {
        }

        public NEOPropData(Prop p)
        {
            Visible = p.Visible;
            if (!Visible) return;
            position = p.Position;
            rotation = p.Rotation;
        }

        public void Apply(Prop p)
        {
            p.Visible = Visible;
            if (Visible)
            {
                p.Position = position;
                p.Rotation = rotation;
            }
        }
    }
}