using MessagePack;
using Studio;
using UnityEngine;

namespace VNActor
{
    public partial class Light : HSNeoOCIProp, IVNObject
    {

        [MessagePackObject]
        public class LightData : IDataClass
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

            public LightData()
            {

            }

            public LightData(Light l)
            {
                position = l.pos;
                rotation = l.rot;
                color = l.color;
                enable = l.enable;
                intensity = l.intensity;
                shadow = l.shadow;
                range = l.range;
                angle = l.angle;
            }
        }

        new protected OCILight objctrl;

        public Light(OCILight objctrl) : base(objctrl)
        {
            this.objctrl = objctrl;
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
            Vector3 cp = this.objctrl.objectInfo.changeAmount.pos;
            Vector3 ncp = new Vector3(cp.x + param[0], cp.y + param[1], cp.z + param[2]);
            this.objctrl.objectInfo.changeAmount.pos = ncp;
        }

        public void rot_add(Vector3 param)
        {
            // param = (rot_delta_x, rot_delta_y, rot_delta_z)
            Vector3 rt = this.objctrl.objectInfo.changeAmount.rot;
            Vector3 nrt = new Vector3(rt.x + param[0], rt.y + param[1], rt.z + param[2]);
            this.objctrl.objectInfo.changeAmount.rot = nrt;
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

        public bool enable
        {
            get
            {
                return objctrl.lightInfo.enable;
            }
            set
            {
                objctrl.SetEnable(value);
            }
        }

        public LightType type
        {
            get
            {
                return objctrl.lightType;
            }
        }

        public int no
        {
            get
            {
                return objctrl.lightInfo.no;
            }
        }

        override public Vector3 pos
        {
            get
            {
                return this.objctrl.objectInfo.changeAmount.pos;
            }
            set
            {
                this.objctrl.objectInfo.changeAmount.pos = value;
            }
        }

        override public Vector3 rot
        {
            get
            {
                return this.objctrl.objectInfo.changeAmount.rot;
            }
            set
            {
                this.objctrl.objectInfo.changeAmount.rot = value;
            }
        }

        public static void prop_enable(Light prop, LightData param)
        {
            // param = 0(hide)/1(show)
            prop.enable = param.enable;
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

        override public IDataClass export_full_status()
        {
            return new LightData(this);
        }

        public void import_status(LightData l)
        {
            pos = l.position;
            rot = l.rotation;
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
    }
}
