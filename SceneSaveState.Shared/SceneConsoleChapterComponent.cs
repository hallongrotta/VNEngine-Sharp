using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using VNEngine;
using static SceneSaveState.UI;

namespace SceneSaveState
{
    internal class SceneConsoleChapterComponent
    {

        internal Manager<Chapter> cm;
        internal SceneConsole sc;
        private string _sceneNameEntry = "";
        private SceneConsoleCamComponent camComponent;

        internal enum SelectedItem
        {
            Scene,
            Chapter
        }

        internal static SelectedItem selectedItem = SelectedItem.Chapter;

        public SceneConsoleChapterComponent(Manager<Chapter> cm, SceneConsoleCamComponent cp)
        {
            this.cm = cm;
            camComponent = cp;
        }

        internal void MoveChapterForward()
        {
            cm.MoveItemForward();
        }

        internal void MoveChapterBackward()
        {
            cm.MoveItemBack();
        }

        internal Scene GoToPreviousChapter()
        {
            return cm.Back().Last();
        }

        internal Scene GoToNextChapter()
        {
            return cm.Next().First();
        }

        internal Scene SetChapter(int chapterNumber)
        {
            return cm.SetCurrent(chapterNumber).First();
        }

        internal Scene LoadNextScene(Chapter c)
        {
           return c.HasNext? c.Next() : GoToNextChapter();
        }

        internal Scene LoadPreviousScene(Chapter c, Camera cam, bool lastcam = false)
        {

            Scene s = c.HasPrev ? c.Back() : GoToPreviousChapter();
            sc.LoadScene(s);
            if (!lastcam || s.Count <= 0) return s;
            var camData = s.Last();
            camData.setCamera(cam);
            return s;
        }

        internal Scene GoToPreviousScene(Chapter c, Camera cam)
        {
            var currentScene = c.Current;
            if (currentScene.HasPrev)
            {
                var camdata = currentScene.Back();
                camdata.setCamera(cam);
                return currentScene;
            }
            if (!c.HasPrev) return currentScene;
            else
            {
                return LoadPreviousScene(c, cam, true);
            }
        }

        internal Scene LoadPreviousScene(Chapter c, Camera cam)
        {
            return LoadPreviousScene(c, cam, false);
        }

        internal void MoveSceneForward(Chapter c)
        {
            if (!c.HasNext && cm.HasNext)
            {
                var scene = c.Remove();
                cm.Next().Prepend(scene);
            }
            else
            {
                c.MoveItemForward();
            }

        }

        internal void MoveSceneBackward(Chapter chapter)
        {

            if (!chapter.HasPrev && cm.HasPrev)
            {
                var scene = chapter.Remove();
                var prevChapter = cm.Back();
                prevChapter.Add(scene);
            }
            else
            {
                chapter.MoveItemBack();
            }
        }

        public void AddChapter()
        {
            cm.Add(new Chapter());
        }

        public Chapter InsertChapter()
        {
            var chapter = new Chapter();
            cm.Insert(chapter);
            return chapter;
        }

        public Chapter SplitChapter(Chapter c)
        {
            if (!c.HasItems) return c;
            var currentIndex = c.CurrentIndex;
            var scenes = c.RemoveUntilEnd(currentIndex + 1);
            var new_chapter = new Chapter(scenes, null);
            return cm.Insert(new_chapter);
        }

        public void MergeChapters()
        {
            var currentChapter = cm.Current;
            if (!cm.HasNext) return;

            var currentIndex = currentChapter.CurrentIndex;

            var nextChapter = cm[currentIndex + 1];

            currentChapter.AddRange(nextChapter.ExportItems().ToList());

            cm.Remove(currentIndex + 1);


        }

        public void DuplicateChapter()
        {
            cm.Duplicate();
        }

        public void RemoveChapter()
        {
            cm.Remove();
        }

        internal void DrawMoveUpDownButtons()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Chapter");
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Move ↑"))
            {
                MoveChapterBackward();
            }
            if (GUILayout.Button("Move ↓"))
            {
                MoveChapterForward();
            }
            GUILayout.EndHorizontal();
        }

        internal void DrawMoveUpDownButtons(Chapter c)
        {

            GUILayout.BeginHorizontal();
            GUILayout.Label("Scene");
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Move ↑"))
            {
                MoveSceneBackward(c);
            }
            if (GUILayout.Button("Move ↓"))
            {
                MoveSceneForward(c);
            }
            GUILayout.EndHorizontal();
        }

        internal void DrawNextPrevButtons(Chapter c, Camera cam)
        {
            Scene s;
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Prev scene", GUILayout.Height(UI.RowHeight), GUILayout.Width(UI.ColumnWidth * 0.5f)))
            {
                s = LoadPreviousScene(c, cam);
            }
            if (GUILayout.Button("Next scene", GUILayout.Height(UI.RowHeight), GUILayout.Width(UI.ColumnWidth * 0.5f)))
            {
                s = LoadNextScene(c);
            }
            GUILayout.EndHorizontal();
        }

        internal Warning? DrawChapterEditButtons(Chapter c, Camera cam, bool promptOnDelete)
        {
            Warning? warning = null;
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Add chapter", GUILayout.Height(RowHeight), GUILayout.Width(ColumnWidth * 0.5f)))
            {
                AddChapter();
            }
            if (GUILayout.Button("Insert chapter", GUILayout.Height(RowHeight), GUILayout.Width(ColumnWidth * 0.5f)))
            {
                InsertChapter();
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Copy chapter", GUILayout.Height(RowHeight), GUILayout.Width(ColumnWidth * 0.5f)))
            {
                DuplicateChapter();
            }
            if (GUILayout.Button("Delete chapter", GUILayout.Height(RowHeight), GUILayout.Width(ColumnWidth * 0.5f)))
            {
                if (promptOnDelete)
                {
                    warning = new Warning("Delete selected scene?", false, RemoveChapter);
                }
                else
                {
                    RemoveChapter();
                }
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Merge chapters", GUILayout.Height(RowHeight), GUILayout.Width(ColumnWidth * 0.5f)))
            {
                MergeChapters();
            }
            if (GUILayout.Button("Split chapter", GUILayout.Height(RowHeight), GUILayout.Width(ColumnWidth * 0.5f)))
            {
                Chapter new_chapter = SplitChapter(c);
                sc.LoadScene(new_chapter.Current, cam);
            }
            GUILayout.EndHorizontal();
            return warning;
        }

        public string SetNameOfSelected()
        {
            return _sceneNameEntry == "" ? null : _sceneNameEntry;
        }

        internal void DrawChapterButtons()
        {
            var i = 0;
            foreach (var chapter in cm)
            {

                var chapterSelected = i == cm.CurrentIndex;

                var col = chapterSelected
                    ? SelectedTextColor
                    : NormalTextColor;

                var chapterName = chapter.Name ?? $"Chapter {i + 1}";

                if (cm[i].HasItems)
                {   
                    chapterName += $" ({cm[i].Count})";
                }

                if (GUILayout.Button($"<color={col}>{chapterName}</color>"))
                {
                    var scene = SetChapter(i);
                    sc.LoadScene(scene);
                    selectedItem = SelectedItem.Chapter;
                    _sceneNameEntry = "";
                }

                if (chapterSelected)
                {
                    DrawSceneButtons(sc, chapter, i, true);
                }

                i++;

            }
        }

        internal void DrawSceneButtons(SceneConsole sc, Chapter c, int chapterNumber, bool chapterSelected)
        {
            for (var i = 0; i < c.Count; i++)
            {
                var sceneName = c[i].Name;
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
                    sc.SetChapterAndScene(chapterNumber, i);
                    _sceneNameEntry = "";
                }

                GUILayout.EndHorizontal();

            }
        }

        internal string GetSelectedName()
        {

            if (_sceneNameEntry != "") return _sceneNameEntry;
            switch (selectedItem)
            {
                case SelectedItem.Chapter when cm.HasItems:
                    return cm.ItemNames[cm.CurrentIndex];
                case SelectedItem.Scene when cm.Current.HasItems:
                    return cm.Current.ItemNames[cm.Current.CurrentIndex];
                default:
                    return "";
            }
        }

        internal void DrawSceneTab(Chapter c, Camera cam)
        {
            _sceneNameEntry = GetSelectedName();

            GUILayout.BeginHorizontal();
            _sceneNameEntry = GUILayout.TextField(_sceneNameEntry, GUILayout.Width(ColumnWidth * 0.8f));
            if (GUILayout.Button("Set", GUILayout.Width(ColumnWidth * 0.2f)))
            {
                if (selectedItem == SelectedItem.Chapter)
                {
                    c.Name = SetNameOfSelected();
                }
                else
                {
                    c.Current.Name = SetNameOfSelected();
                }
                _sceneNameEntry = "";
                c.ItemNames = c.RebuildItemNames();
            }
            GUILayout.EndHorizontal();

            scene_scroll = GUILayout.BeginScrollView(scene_scroll);

            DrawChapterButtons();

            GUILayout.EndScrollView();

            if (!c.HasItems) return;

            GUILayout.FlexibleSpace();

            DrawMoveUpDownButtons(c);
            DrawMoveUpDownButtons();
            DrawNextPrevButtons(c, cam);
        }

        // Goto next/prev
        internal void GoToFirstScene()
        {
            var chapter = cm.First();
            sc.LoadScene(chapter.First());
        }

    }
}
