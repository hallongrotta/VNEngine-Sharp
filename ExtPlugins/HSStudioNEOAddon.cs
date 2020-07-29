using System;
using System.Collections.Generic;
using System.Text;

namespace ExtPlugins
{
    public class HSStudioNEOAddon
        : ExtPlugin
    {

        public object vmdMgr;

        public HSStudioNEOAddon()
        {
            //super(HSStudioNEOAddon, self).__init__("HSStudioNEOAddon") #can't get it to work
            ExtPlugin.@__init__(this, "HSStudioNEOAddon");
            if (this.isDetected)
            {
                this.vmdMgr = BaseMgr[HSVMDAnimationMgr].Instance;
            }
        }

        // Activate simulataneous FK/IK (char: HSNeoOCIChar, OCIChar)
        public object activateFKIK(object char)
        {
            if (this.isDetected == true && !(char == null))
            {
                if (char is HSNeoOCIChar)
                {
                    FKIKAssist.FKIKAssistMgr.ActivateFKIK(char.objctrl);
                }
                else if (char is OCIChar)
                {
                    //might be redundant, not sure
                    FKIKAssist.FKIKAssistMgr.ActivateFKIK(char);
                }
            }
        }

        // MMD play ALL
        public object MMDPlayAll()
        {
            if (this.isDetected)
            {
                this.vmdMgr.PlayAll();
            }
        }

        // MMD Pause ALL
        public object MMDPauseAll()
        {
            if (this.isDetected)
            {
                this.vmdMgr.PauseAll();
            }
        }

        // MMD play ALL
        public object MMDStopAll()
        {
            if (this.isDetected)
            {
                this.vmdMgr.StopAll();
            }
        }

        // MMD set anime position
        public object MMDSetAnimPositionAll(object time)
        {
            if (this.isDetected)
            {
                this.vmdMgr.PlayAll();
                this.vmdMgr.PauseAll();
                foreach (var vc in this.vmdMgr.controllers)
                {
                    vc.SetAnimPosition(time);
                }
                this.vmdMgr.SoundMgr.AnimePosition = time;
            }
        }

        // MMD get controller for char
        public void MMDGetCharVmdController(object char)
        {
            object tgtOCIChar;
            if (this.isDetected && char)
            {
                if (char is HSNeoOCIChar)
                {
                    tgtOCIChar = char.objctrl;
                }
                else if (char is OCIChar)
                {
                    //might be redundant, not sure
                    tgtOCIChar = char;
                }
                else
                {
                    return null;
                }
                foreach (var vc in this.vmdMgr.controllers)
                {
                    if (vc.studioChara == tgtOCIChar)
                    {
                        return vc;
                    }
                }
            }
            return null;
        }

        // MMD export setting for char
        public object MMDExportCharVmdSetting(object char)
        {
            var vc = this.MMDGetCharVmdController(char);
            if (vc && vc.VMDAnimEnabled)
            {
                var exset = new Dictionary<object, object>
                {
                };
                exset["VMDAnimEnabled"] = "1";
                exset["lastLoadedVMD"] = vc.lastLoadedVMD ? vc.lastLoadedVMD : "";
                exset["speed"] = vc.speed.ToString();
                exset["Loop"] = vc.Loop ? "1" : "0";
                exset["centerBasePos"] = String.Format("(%.3f, %.3f, %.3f)", vc.centerBasePos.x, vc.centerBasePos.y, vc.centerBasePos.z);
                return exset;
            }
            else
            {
                return null;
            }
        }

        // MMD import setting for char
        public object MMDImportCharVmdSetting(object char, object imSet)
        {
            var vc = this.MMDGetCharVmdController(char);
            if (vc == null)
            {
                return;
                // TODO: load settings
                // --- HSStudioNEOExtSave ---
            }
        }
    }
}
