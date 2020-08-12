using Studio;
using UnityEngine;

namespace VNActor
{
    public partial class Route : HSNeoOCIProp
    {

        new public OCIRoute objctrl;

        public Route(ObjectCtrlInfo objctrl) : base(objctrl)
        {
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

    }
}
