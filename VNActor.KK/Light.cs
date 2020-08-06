using Studio;
using System;
using UnityEngine;

namespace VNActor
{
    public partial class Light : HSNeoOCILight, IProp
    {

        new public OCILight objctrl;

        public struct LightData : IDataClass
        {
            public Color color;
            internal bool enable;
            internal float intensity;
            internal bool shadow;
            internal float range;
            internal float angle;

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
        }

        public Light(OCILight objctrl) : base(objctrl)
        {
            this.objctrl = objctrl;
        }

        override public IDataClass export_full_status()
        {
            return new LightData(this);
        }

        public Color color
        {
            get
            {
                return this.objctrl.lightInfo.color;
            }
            set
            {
                this.objctrl.SetColor(value);
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
                return this.objctrl.lightInfo.range;
            }
            set
            {
                objctrl.SetRange(value);
            }
        }
    }
}
