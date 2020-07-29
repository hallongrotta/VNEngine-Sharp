using System;
using System.Collections.Generic;
using System.Text;

namespace VNEngine
{
    public class ScriptClip
    {

        public object animeScript;

        public object finalCamStatus;

        public object finalScnStatus;

        public object info;

        public object nonAnimeScript;

        public object tailScript;

        public ScriptClip()
        {
            this.finalScnStatus = null;
            this.finalCamStatus = null;
            this.nonAnimeScript = null;
            this.animeScript = null;
            this.tailScript = null;
            this.info = null;
        }
    }

    public class ScriptClipInfo
    {

        public bool animateCamera;

        public int animeDuration;

        public int animeStyle;

        public int cameraDuration;

        public int cameraStyle;

        public string dialogue;

        public int dumpAsIndex;

        public object dumpTgts;

        public int dumpTypeIndex;

        public bool hideButtonInAnime;

        public bool hideWindowInAnime;

        public bool includeCamera;

        public string speakerAlias;

        public bool useCameraTimer;

        public ScriptClipInfo()
        {
            // scene
            this.dumpTypeIndex = 0;
            this.dumpTgts = null;
            this.dumpAsIndex = 0;
            this.animeDuration = 1;
            this.animeStyle = 0;
            this.speakerAlias = "";
            this.dialogue = "";
            // camera
            this.includeCamera = false;
            this.animateCamera = false;
            this.useCameraTimer = true;
            this.cameraDuration = 1;
            this.cameraStyle = 0;
            // system
            this.hideWindowInAnime = false;
            this.hideButtonInAnime = false;
        }
    }
}
