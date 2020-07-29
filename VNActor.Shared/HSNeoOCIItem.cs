using Studio;
using System;
using UnityEngine;

namespace VNActor
{

    public class HSNeoOCIItem
        : HSNeoOCIProp
    {

        public HSNeoOCIItem(OCIItem objctrl) : base(objctrl)
        {

        }

        public static HSNeoOCIItem add_item(int group, int category, int no)
        {
            var objctrl = AddObjectItem.Add(group, category, no); //TODO make this right
            return new HSNeoOCIItem(objctrl);
        }

        public void pos_add(float[] param)
        {
            // param = (pos_delta_x, pos_delta_y, pos_delta_z)
            ObjectCtrlInfo item = this.objctrl;
            Vector3 cp = item.objectInfo.changeAmount.pos;
            Vector3 ncp = new Vector3(cp.x + param[0], cp.y + param[1], cp.z + param[2]);
            item.objectInfo.changeAmount.pos = ncp;
        }

        public void rot_add(float[] param)
        {
            // param = (rot_delta_x, rot_delta_y, rot_delta_z)
            ObjectCtrlInfo item = this.objctrl;
            Vector3 rt = item.objectInfo.changeAmount.rot;
            Vector3 nrt = new Vector3(rt.x + param[0], rt.y + param[1], rt.z + param[2]);
            item.objectInfo.changeAmount.rot = nrt;
        }

        public void scale_add(float[] param)
        {
            // param = (scale_x, scale_y, scale_z) or scale
            if (this.objctrl is OCIItem item)
            {
                // for item only, folder can not set scale
                Vector3 rt = item.itemInfo.changeAmount.scale;
                Vector3 nrt = new Vector3(rt.x + param[0], rt.y + param[1], rt.z + param[2]);
                item.itemInfo.changeAmount.scale = nrt;
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
                this.objctrl.objectInfo.changeAmount.pos = pos;
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

        public Vector3 scale
        {
            get
            {
                if (this.objctrl is OCIItem item)
                    return item.objectInfo.changeAmount.scale;
                else
                {
                    throw new Exception("Can not scale this item");
                }
            }
            set
            {
                if (this.objctrl is OCIItem item)
                    item.itemInfo.changeAmount.scale = value;
            }
        }

        public int no
        {
            get
            {
                OCIItem item = (OCIItem)this.objctrl;
                return item.itemInfo.no;
            }
        }
    }

}
