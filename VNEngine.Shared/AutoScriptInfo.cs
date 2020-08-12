namespace VNEngine
{
    public class AutoScriptInfo
    {

        public bool alwaysHideWindowInCameraAnime;

        public bool alwaysLockWindowInSceneAnime;

        public bool createLocalizeString;

        public string defaultEndBtnText;

        public string defaultEndText;

        public string defaultNextBtnText;

        public string defaultReloadBtnText;

        public bool enableQuickReload;

        public bool enableReload;

        public bool fakeLipSyncEnable;

        public double fakeLipSyncReadingSpeed;

        public string fakeLipSyncVersion;

        public string gameName;

        public bool masterMode;

        public string pythonName;

        public string sceneDir;

        public string scenePNG;

        public string skinVersion;

        public AutoScriptInfo()
        {
            this.gameName = "";
            this.pythonName = "";
            this.sceneDir = "";
            this.scenePNG = "";
            this.enableReload = true;
            this.enableQuickReload = false;
            this.alwaysHideWindowInCameraAnime = false;
            this.alwaysLockWindowInSceneAnime = false;
            this.createLocalizeString = false;
            this.defaultNextBtnText = "Next >>";
            this.defaultReloadBtnText = "Restart <<";
            this.defaultEndBtnText = "End Game >>";
            this.defaultEndText = "<size=32>THE END</size>";
            this.skinVersion = "skin_renpy";
            this.fakeLipSyncEnable = true;
            this.fakeLipSyncVersion = "v11";
            this.fakeLipSyncReadingSpeed = 12.0;
            this.masterMode = false;
        }
    }
}
