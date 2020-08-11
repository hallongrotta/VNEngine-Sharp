using MessagePack;
using Studio;
using System;
using UnityEngine;

namespace VNActor
{
    public partial class Light : HSNeoOCIProp, IVNObject
    {

        [MessagePackObject]
        public struct LightData : IDataClass
        {
            [Key(0)]
            public Color color;
            [Key(1)]
            public bool enable;
            [Key(2)]
            public float intensity;
            [Key(3)]
            public bool shadow;
            [Key(4)]
            public float range;
            [Key(5)]
            public float angle;

            public Vector3 Position { get; }

            public Vector3 Rotation { get; }

            public void Remove(string key)
            {
                throw new NotImplementedException();
            }

            public LightData(Light l)
            {
                Position = l.pos;
                Rotation = l.rot;
                color = l.color;
                enable = l.enable;
                intensity = l.intensity;
                shadow = l.shadow;
                range = l.range;
                angle = l.angle;
            }

            [SerializationConstructor]
            public LightData(Vector3 pos, Vector3 rot, Color color, bool enable, float intensity, bool shadow, float range, float angle)
            {
                Position = pos;
                Rotation = rot;
                this.color = color;
                this.enable = enable;
                this.intensity = intensity;
                this.shadow = shadow;
                this.range = range;
                this.angle = angle;
            }
        }

        override public IDataClass export_full_status()
        {
            return new LightData(this);
        }

        public void import_status(LightData l)
        {
            color = l.color;
            enable = l.enable;
            intensity = l.intensity;
            shadow = l.shadow;
            range = l.range;
            angle = l.angle;
        }

        override public void import_status(IDataClass status)
        {
            if (status is LightData)
            {
                import_status(status);
            }
        }

        public Color color
        {
            get
            {
                return objctrl.lightInfo.color;
            }
            set
            {
                objctrl.SetColor(value);
            }
        }

        public float intensity
        {
            get
            {

                return objctrl.lightInfo.intensity;
            }
            set
            {
                objctrl.SetIntensity(value);
            }
        }

        public bool shadow
        {
            get
            {
                return objctrl.lightInfo.shadow;
            }
            set
            {
                objctrl.SetShadow(value);
            }
        }

        public float angle
        {
            get
            {
                return objctrl.lightInfo.spotAngle;
            }
            set
            {
                objctrl.SetSpotAngle(value);
            }

        }

        public float range
        {
            get
            {
                return objctrl.lightInfo.range;
            }
            set
            {
                objctrl.SetRange(value);
            }
        }
    }
}
