using Studio;
using UnityEngine;

namespace VNActor
{

    public class HSNeoOCIRoute
        : HSNeoOCIProp
    {

        new public OCILight objctrl;

        public HSNeoOCIRoute(ObjectCtrlInfo objctrl) : base(objctrl)
        {
        }

        public Vector3 pos
        {
            get
            {
                return this.objctrl.objectInfo.changeAmount.pos;
            }
        }

        public Vector3 rot
        {
            get
            {
                return this.objctrl.objectInfo.changeAmount.rot;
            }
        }
    }

}
