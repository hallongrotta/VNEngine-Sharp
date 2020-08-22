using MessagePack;
using Studio;
using UnityEngine;

namespace VNActor
{
    partial class Light
    {

        public class LightData
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
            [Key(6)]
            public Vector3 position;
            [Key(7)]
            public Vector3 rotation;
        }

        public Light(ObjectCtrlInfo objctrl) : base(objctrl)
        {
        }

        public Color color {
            set
            {
                this.objctrl.SetColor(value);
            }
            get
            {
                return this.objctrl.lightInfo.color;
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

        public override IDataClass export_full_status()
        {
            throw new System.NotImplementedException();
        }

        public override void import_status(IDataClass status)
        {
            throw new System.NotImplementedException();
        }
    }
}
