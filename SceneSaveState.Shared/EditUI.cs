using System;
using UnityEngine;
using VNEngine;
using static SceneSaveState.SceneConsole;

namespace SceneSaveState
{
    partial class UI
    {

        internal enum SelectedItem
        {
            Scene,
            Chapter
        }

        internal static SelectedItem selectedItem = SelectedItem.Chapter;

        public static void DrawVNDataOptions()
        {
            if (!Instance.currentVNData.enabled) return;
            GUILayout.BeginHorizontal();
            GUILayout.Label("Who say:", GUILayout.Width(90));
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("<", GUILayout.Width(20)))
            {
                Instance.currentVNData.whosay = Instance.get_next_speaker(Instance.currentVNData.whosay, false);
            }
            if (GUILayout.Button(">", GUILayout.Width(20)))
            {
                Instance.currentVNData.whosay = Instance.get_next_speaker(Instance.currentVNData.whosay, true);
            }
            GUILayout.EndHorizontal();
            Instance.currentVNData.whosay = GUILayout.TextField(Instance.currentVNData.whosay);

            GUILayout.BeginHorizontal();
            GUILayout.Label("What say:", GUILayout.Width(90));
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("X", GUILayout.Width(20)))
            {
                Instance.currentVNData.whatsay = "";
            }
            if (GUILayout.Button("...", GUILayout.Width(20)))
            {
                Instance.currentVNData.whatsay = "...";
            }
            GUILayout.EndHorizontal();
            Instance.currentVNData.whatsay = GUILayout.TextArea(Instance.currentVNData.whatsay, GUILayout.Height(85));
            /*GUILayout.BeginHorizontal();
                GUILayout.Label("  Adv VN cmds", GUILayout.Width(90));
                Instance.currentVNData.addvncmds = GUILayout.TextArea(Instance.currentVNData.addvncmds, GUILayout.Width(235), GUILayout.Height(55));
                if (GUILayout.Button("X", GUILayout.Width(20)))
                {
                    Instance.currentVNData.addvncmds = "";
                }
                // if GUILayout.Button("X", GUILayout.Width(20)):
                //     sc.cam_whatsay = ""
                // if GUILayout.Button("...", GUILayout.Width(20)):
                //     sc.cam_whatsay = "..."
                GUILayout.EndHorizontal();
                */
        }

        private static string _sceneNameEntry = "";


        internal static void DrawSceneButtons(Chapter c, int chapterNumber, bool chapterSelected)
        {
            for (var i = 0; i < c.Count; i++)
            {
                var sceneName = c[i].name;
                var col = i == c.CurrentIndex && chapterSelected
                    ? SelectedTextColor
                    : NormalTextColor;

                if (sceneName is null)
                {
                    sceneName = $"Scene {i + 1}";
                }

                GUILayout.BeginHorizontal();
                GUILayout.Space(10);

                if (GUILayout.Button($"<color={col}>{sceneName}</color>"))
                {
                    selectedItem = SelectedItem.Scene;
                    Instance.SetCurrentChapter(chapterNumber, i);
                    _sceneNameEntry = "";
                }

                GUILayout.EndHorizontal();

            }
        }

        internal static void DrawChapterButtons(Manager<Chapter> chapterManager)
        {
            var i = 0;
            foreach (var chapter in chapterManager)
            {

                var chapterSelected = i == chapterManager.CurrentIndex;

                var col = chapterSelected
                    ? SelectedTextColor
                    : NormalTextColor;

                var chapterName = chapter.name ?? $"Chapter {i + 1}";

                if (chapterManager[i].HasItems)
                {
                    chapterName += $" ({chapterManager[i].Count})";
                }

                if (GUILayout.Button($"<color={col}>{chapterName}</color>"))
                {
                    Instance.SetCurrentChapter(i);
                    selectedItem = SelectedItem.Chapter;
                    _sceneNameEntry = "";
                }

                if (chapterSelected)
                {
                    DrawSceneButtons(chapter, i, true);
                }
               
                i++;

            }
        }

        internal static void SetName<T>(IManaged<T> item, string name)
        {
            item.name = name == "" ? null : name;
            Instance.ChapterManager.RebuildItemNames();
        }

        public static string GetSelectedName()
        {

            if (_sceneNameEntry != "") return _sceneNameEntry;

            switch (selectedItem)
            {
                case SelectedItem.Chapter when Instance.ChapterManager.HasItems:
                    return Instance.ChapterManager.ItemNames[Instance.ChapterManager.CurrentIndex];
                case SelectedItem.Scene when Instance.CurrentChapter.HasItems:
                    return Instance.CurrentChapter.ItemNames[Instance.CurrentChapter.CurrentIndex];
                default:
                    return "";
            }
        }

        public static void SetNameOfSelected<T>(IManaged<T> item)
        {
            if (_sceneNameEntry == "")
            {
                item.name = null;
            }
            else
            {
                item.name = _sceneNameEntry;
                _sceneNameEntry = "";
            }
        }

        public static void DrawSceneTab()
        {
            _sceneNameEntry = GetSelectedName();

            GUILayout.BeginHorizontal();
            _sceneNameEntry = GUILayout.TextField(_sceneNameEntry, GUILayout.Width(ColumnWidth*0.8f));
            if (GUILayout.Button("Set", GUILayout.Width(ColumnWidth * 0.2f)))
            {
                if (selectedItem == SelectedItem.Chapter)
                {
                    SetNameOfSelected(Instance.CurrentChapter);
                    Instance.ChapterManager.RebuildItemNames();
                }
                else
                {
                    SetNameOfSelected(Instance.CurrentScene);
                    Instance.CurrentChapter.RebuildItemNames();
                }
            }
            GUILayout.EndHorizontal();

            scene_scroll = GUILayout.BeginScrollView(scene_scroll);

            DrawChapterButtons(Instance.ChapterManager);

            GUILayout.EndScrollView();

            if (!Instance.CurrentChapter.HasItems) return;

            GUILayout.FlexibleSpace();
            
            DrawMoveUpDownButtons();
            DrawNextPrevButtons();
        }

        public static void DrawCamSelect()
        {
            if (!Instance.CurrentChapter.HasItems) return; 

            cam_scroll = GUILayout.BeginScrollView(cam_scroll, GUILayout.Height(185));
            for (int i = 0; i < Instance.CurrentScene.cams.Count - 0; i++)
            {
                var col = i == Instance.CamManager.CurrentIndex ? SelectedTextColor : "#f9f9f9";
                var cam = Instance.CurrentScene.cams[i];
                var camText = cam.name ?? $"Cam {i + 1}";

                GUILayout.BeginHorizontal();

                if (GUILayout.Button($"<color={col}>{camText}, {(int)cam.fov}</color>", GUILayout.Width(ColumnWidth * 0.8f)))
                {
                    Instance.CamManager.SetCurrent(i);
                    Instance.setCamera(isAnimated: false);
                }
                if (GUILayout.Button($"<color={col}>a</color>", GUILayout.Width(ColumnWidth*0.2f)))
                {
                    Instance.CamManager.SetCurrent(i);
                    Instance.setCamera(isAnimated: true);
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndScrollView();
            GUILayout.FlexibleSpace();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Add", GUILayout.Width(ColumnWidth * 0.7f)))
            {
                Instance.changeSceneCam(CamTask.ADD);
            }
            if (Instance.CurrentScene.cams.Count > 0)
            {
                if (GUILayout.Button("Del", GUILayout.Width(ColumnWidth * 0.3f)))
                {
                    if (Instance.promptOnDelete.Value)
                    {
                        warning_action = Instance.DeleteSceneCam;
                        warning_param = new WarningParam_s("Delete selected cam?", false);
                    }
                    else
                    {
                        Instance.DeleteSceneCam();
                    }
                }
            }
            GUILayout.EndHorizontal();
            if (Instance.CurrentScene.cams.Count > 0)
            {
                if (GUILayout.Button("Update", GUILayout.Width(ColumnWidth + 5)))
                {
                    Instance.changeSceneCam(CamTask.UPDATE);
                }
            }
            GUILayout.BeginHorizontal();
            const string up = "\u2191";
            const string down = "\u2193";
            if (GUILayout.Button($"Cam {up}"))
            {
                Instance.CamManager.MoveItemBack();
            }
            if (GUILayout.Button($"Cam {down}"))
            {
                Instance.CamManager.MoveItemForward();
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            if (Instance.CurrentChapter.HasItems && Instance.CamManager.Count > 0)
            {
                if (GUILayout.Button("Copy cams"))
                {
                    Instance.copyCamSet();
                }
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            if (Instance.CurrentChapter.HasItems && Instance.camset != null)
            {
                if (GUILayout.Button("Paste cams"))
                {
                    Instance.pasteCamSet();
                }
            }
            GUILayout.EndHorizontal();
        }

        public static void DrawChapterEditButtons()
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Add chapter", GUILayout.Height(25), GUILayout.Width(ColumnWidth * 0.5f)))
            {
                Instance.AddChapter();
            }
            if (GUILayout.Button("Insert chapter", GUILayout.Height(25), GUILayout.Width(ColumnWidth * 0.5f)))
            {
                Instance.InsertChapter();
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Copy chapter", GUILayout.Height(25), GUILayout.Width(ColumnWidth * 0.5f)))
            {
                Instance.DuplicateChapter();
            }
            if (GUILayout.Button("Delete chapter", GUILayout.Height(25), GUILayout.Width(ColumnWidth * 0.5f)))
            {
                if (Instance.promptOnDelete.Value)
                {
                    warning_action = Instance.RemoveChapter;
                    warning_param = new WarningParam_s("Delete selected scene?", false);
                }
                else
                {
                    Instance.RemoveChapter();
                }
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Merge chapters", GUILayout.Height(25), GUILayout.Width(ColumnWidth * 0.5f)))
            {
                Instance.MergeChapters();
            }
            if (GUILayout.Button("Split chapter", GUILayout.Height(25), GUILayout.Width(ColumnWidth * 0.5f)))
            {
                Instance.SplitChapter();
            }
            GUILayout.EndHorizontal();
        }

        public static void DrawSceneEditButtons()
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Add scene", GUILayout.Height(55), GUILayout.Width(ColumnWidth * 0.5f)))
            {
                Instance.AddScene();
            }
            if (GUILayout.Button("Update scene", GUILayout.Height(55), GUILayout.Width(ColumnWidth * 0.5f)))
            {
                Instance.UpdateScene();
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();      
            if (GUILayout.Button("Insert scene", GUILayout.Height(25), GUILayout.Width(ColumnWidth * 0.5f)))
            {
                Instance.InsertScene();
            }
            if (GUILayout.Button("Dup scene", GUILayout.Height(25), GUILayout.Width(ColumnWidth * 0.5f)))
            {
                Instance.DuplicateScene();
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Delete scene"))
            {
                if (Instance.promptOnDelete.Value)
                {
                    warning_action = Instance.RemoveScene;
                    warning_param = new WarningParam_s("Delete selected scene?", false);
                }
                else
                {
                    Instance.RemoveScene();
                }
            }
            GUILayout.EndHorizontal();
        }

        public static void DrawMoveUpDownButtons()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Scene");
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Move ↑"))
            {
                Instance.MoveSceneBackward();
            }
            if (GUILayout.Button("Move ↓"))
            {
                Instance.MoveSceneForward();
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("Chapter");
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Move ↑"))
            {
                Instance.MoveChapterBackward();
            }
            if (GUILayout.Button("Move ↓"))
            {
                Instance.MoveChapterForward();
            }
            GUILayout.EndHorizontal();
        }

        public static void DrawNextPrevButtons()
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Prev scene", GUILayout.Height(25), GUILayout.Width(ColumnWidth * 0.5f)))
            {
                Instance.LoadPreviousScene();
            }
            if (GUILayout.Button("Next scene", GUILayout.Height(25), GUILayout.Width(ColumnWidth * 0.5f)))
            {
                Instance.LoadNextScene();
            }
            GUILayout.EndHorizontal();
        }

        public static void sceneConsoleEditUI()
        {
            GUILayout.BeginHorizontal();
            // Column 1
            GUILayout.BeginVertical(GUILayout.Width(ColumnWidth));
            GUILayout.Label("Scenes");
            // Scene tab
            DrawSceneTab();
            GUILayout.EndVertical();

            // Column 2
            GUILayout.BeginVertical(GUILayout.Width(ColumnWidth));
            GUILayout.Label($"Scene Cameras. FOV: {(int)Instance.CameraController.cameraData.parse}");

            // Camera and character selection tabs
            if (Instance.CurrentChapter.Count > 0)
            {
                DrawCamSelect();
            }
            GUILayout.EndVertical();

            // Column 3         
            GUILayout.BeginVertical(GUILayout.Width(ColumnWidth));
            GUILayout.Label("Scene Controls");

            DrawSceneEditButtons();
            DrawChapterEditButtons();

            Instance.currentVNData.enabled = GUILayout.Toggle(Instance.currentVNData.enabled, "Use cam in Visual Novel");
            GUILayout.FlexibleSpace();
            DrawVNDataOptions();
            GUILayout.FlexibleSpace();
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }
    }
}
