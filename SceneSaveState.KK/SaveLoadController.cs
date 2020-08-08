using KKAPI.Studio.SaveLoad;
using KKAPI.Utilities;
using Studio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BepInEx;
using ExtensibleSaveFormat;

namespace SceneSaveState
{
    class SaveLoadController : SceneCustomFunctionController
    {

        public byte[] actorData;
        public byte[] camData;
        public byte[] propData;

        public List<Scene> scenes;

        public SaveLoadController(ref List<Scene> scenes)
        {
            this.scenes = scenes;
        }

        protected override void OnSceneSave()
        {
            var pluginData = new PluginData();
            pluginData.data["actors"] = actorData;
            pluginData.data["cams"] = camData;
            pluginData.data["props"] = propData;
            SetExtendedData(pluginData);
        }

        protected override void OnSceneLoad(SceneOperationKind operation, ReadOnlyDictionary<int, ObjectCtrlInfo> loadedItems)
        {
            var data = GetExtendedData();

        }
    }
}
