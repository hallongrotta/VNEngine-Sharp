using ExtensibleSaveFormat;
using KKAPI.Studio.SaveLoad;
using KKAPI.Utilities;
using Studio;
using System;
using System.Collections.Generic;
using System.Text;
using VNEngine;

namespace SceneSaveState
{
    class SaveLoadController : SceneCustomFunctionController
    {



        protected override void OnSceneSave()
        {          
            SetExtendedData(SceneConsole.Instance.GetPluginData());          
        }

        public void deleteSaveData()
        {
            SceneConsole sc = SceneConsole.Instance;
            SetExtendedData(new PluginData() { data = null });
            sc.block = new SceneManager();
        }       

        protected override void OnSceneLoad(SceneOperationKind operation, ReadOnlyDictionary<int, ObjectCtrlInfo> loadedItems)
        {
            SceneConsole.Instance?.LoadPluginData(GetExtendedData());
        }
    }
}
