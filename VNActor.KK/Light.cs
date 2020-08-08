using MessagePack;
using Studio;
using System;
using UnityEngine;

namespace VNActor
{
    public partial class Light : HSNeoOCILight, IProp
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

            public void Remove(string key)
            {
                throw new NotImplementedException();
            }

            public LightData(Light l)
            {
                color = l.color;
                enable = l.enable;
                intensity = l.intensity;
                shadow = l.shadow;
                range = l.range;
                angle = l.angle;
            }

            [SerializationConstructor]
            public LightData(Color color, bool enable, float intensity, bool shadow, float range, float angle)
            {
                this.color = color;
                this.enable = enable;
                this.intensity = intensity;
                this.shadow = shadow;
                this.range = range;
                this.angle = angle;
            }
        }

        public Light(OCILight objctrl) : base(objctrl)
        {

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
