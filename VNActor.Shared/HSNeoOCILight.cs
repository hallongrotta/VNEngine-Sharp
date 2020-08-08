using Studio;
using UnityEngine;

namespace VNActor
{

    abstract public class HSNeoOCILight
        : HSNeoOCIProp
    {

        new protected OCILight objctrl;

        public HSNeoOCILight(OCILight objctrl) : base(objctrl)
        {
            this.objctrl = objctrl;
        }

        public static HSNeoOCILight add_light(int no)
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

        public Vector3 pos
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

        public Vector3 rot
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
    }
}
