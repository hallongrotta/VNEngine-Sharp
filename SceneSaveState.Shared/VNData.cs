using MessagePack;
using System;
using System.Collections.Generic;
using System.Text;

namespace SceneSaveState
{
    public class VNDataComponent
    {

        public struct adv_properties
        {
            [Key(0)] public string name;
            [Key(1)] public bool isTime;
            [Key(2)] public string time;
            [Key(3)] public bool isTAnimCam;
            [Key(4)] public bool isTHideUI;
            [Key(5)] public bool isTTimerNext;
            [Key(6)] public string tacStyle;
            [Key(7)] public string tacZOut;
            [Key(8)] public string tacRotX;
            [Key(9)] public string tacRotZ;
            [Key(10)] public bool keepcam;
        }

        public struct VNData: IManaged<VNData>
        {
            public bool enabled; // formerly addparam
            public string whosay;
            public string whatsay;
            public string addvncmds;
            public bool add_props;
            public addprops_struct addprops;
            private string name;

            public string Name { get => name; set => name = value; }

            public string TypeName => "VNText";

            public struct addprops_struct
            {
                [Key(0)] public adv_properties a1o;

                //[Key(1)]
                //public Dictionary<string, bool> a2o;
                [Key(2)] public bool a1;
                [Key(3)] public bool a2;
            }

            public VNData(bool enabled, string whosay, string whatsay, string addvncmds, addprops_struct addprops)
            {
                this.enabled = enabled;
                this.whosay = whosay;
                this.whatsay = whatsay;
                this.addvncmds = addvncmds;
                this.addprops = addprops;
                this.name = "VNText";
                add_props = false;
            }

            public VNData Copy()
            {
                throw new NotImplementedException();
            }

            
        }
    }
}
