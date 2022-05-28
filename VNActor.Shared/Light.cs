using MessagePack;
using Studio;
using UnityEngine;

namespace VNActor
{
    public class Light : Prop, IVNObject<Light>
    {
        protected new OCILight objctrl;

        public Light(OCILight objctrl) : base(objctrl)
        {
            this.objctrl = objctrl;
        }

        public float range
        {
            get => objctrl.lightInfo.range;
            set => objctrl.SetRange(value);
        }

        public bool shadow
        {
            get => objctrl.lightInfo.shadow;
            set => objctrl.SetShadow(value);
        }

        public float angle
        {
            get => objctrl.lightInfo.spotAngle;
            set => objctrl.SetSpotAngle(value);
        }

        public Color color
        {
            get => objctrl.lightInfo.color;
            set => objctrl.SetColor(value);
        }

        public float intensity
        {
            get => objctrl.lightInfo.intensity;
            set => objctrl.SetIntensity(value);
        }

        public override bool Visible
        {
            get => objctrl.lightInfo.enable;
            set => objctrl.SetEnable(value);
        }

        public LightType type => objctrl.lightType;

        public int no => objctrl.lightInfo.no;

        public override Vector3 Position
        {
            get => objctrl.objectInfo.changeAmount.pos;
            set => objctrl.objectInfo.changeAmount.pos = value;
        }

        public override Vector3 Rotation
        {
            get => objctrl.objectInfo.changeAmount.rot;
            set => objctrl.objectInfo.changeAmount.rot = value;
        }

        public new IDataClass<Light> export_full_status()
        {
            return new LightData(this);
        }

        public void import_status(IDataClass<Light> status)
        {
            if (status is LightData) import_status(status);
        }

        public static Light add_light(int no)
        {
            //no:0~8
            var objctrl = AddObjectLight.Add(no);
            return new Light(objctrl);
        }

        public void pos_add(Vector3 param)
        {
            // param = (pos_delta_x, pos_delta_y, pos_delta_z)
            var cp = objctrl.objectInfo.changeAmount.pos;
            var ncp = new Vector3(cp.x + param[0], cp.y + param[1], cp.z + param[2]);
            objctrl.objectInfo.changeAmount.pos = ncp;
        }

        public void rot_add(Vector3 param)
        {
            // param = (rot_delta_x, rot_delta_y, rot_delta_z)
            var rt = objctrl.objectInfo.changeAmount.rot;
            var nrt = new Vector3(rt.x + param[0], rt.y + param[1], rt.z + param[2]);
            objctrl.objectInfo.changeAmount.rot = nrt;
        }

        public static void prop_enable(Light prop, LightData param)
        {
            // param = 0(hide)/1(show)
            prop.Visible = param.Visible;
        }

        public static void prop_intensity(Light prop, LightData param)
        {
            prop.intensity = param.intensity;
        }

        public static void prop_shadow(Light prop, LightData param)
        {
            prop.shadow = param.shadow;
        }

        public static void prop_angle(Light prop, LightData param)
        {
            prop.angle = param.angle;
        }

        public static void prop_range(Light prop, LightData param)
        {
            prop.range = param.range;
        }

        public void import_status(LightData l)
        {
            l.Apply(this);
        }

        [MessagePackObject(true)]
        public class LightData : NEOPropData, IDataClass<Light>
        {
            [Key(4)] public float angle;

            [Key(0)] public Color color;

            [Key(1)] public float intensity;

            [Key(3)] public float range;

            [Key(2)] public bool shadow;

            public LightData()
            {
            }

            public LightData(Light l) : base(l)
            {
                color = l.color;
                shadow = l.shadow;
                range = l.range;
                angle = l.angle;
            }

            public void Apply(Light l)
            {
                base.Apply(l);
                color = l.color;
                intensity = l.intensity;
                shadow = l.shadow;
                range = l.range;
                angle = l.angle;
            }
        }
    }
}