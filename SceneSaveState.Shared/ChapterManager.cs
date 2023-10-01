using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using VNEngine;
using static SceneSaveState.UI;

namespace SceneSaveState
{
    internal class ChapterManager : Manager<Chapter>
    {

        private string _sceneNameEntry = "";

        internal enum SelectedItem
        {
            Scene,
            Chapter
        }

        internal static SelectedItem selectedItem = SelectedItem.Chapter;

        internal ChapterManager() { }

        internal ChapterManager(Chapter c) : base()
        {
            Add(c);
        }

        internal ChapterManager(List<Chapter> items, string[] itemNames, int currentIndex = 0) : base(items, itemNames, currentIndex)
        {
        }

        internal ChapterManager(List<Chapter> items, int currentIndex = 0) : base(items, currentIndex)
        {
        }

        internal void MoveChapterForward()
        {
            MoveItemForward();
        }

        internal void MoveChapterBackward()
        {
            MoveItemBack();
        }

        internal Scene GoToPreviousChapter()
        {
            return Back().Last();
        }

        internal Scene GoToNextChapter()
        {
            return Next().First();
        }

        internal Scene SetChapter(int chapterNumber)
        {
            return SetCurrent(chapterNumber).First();
        }

        internal Scene LoadNextScene(Chapter c)
        {
           return c.HasNext? c.Next() : GoToNextChapter();
        }

        internal Scene LoadPreviousScene(SceneConsole sc, VNController gc, Chapter c, Camera cam, bool lastcam = false)
        {

            Scene s = c.HasPrev ? c.Back() : GoToPreviousChapter();
            sc.LoadScene(s);
            if (!lastcam || s.Count <= 0) return s;
            var camData = s.Last();
            camData.setCamera(cam, gc);
            return s;
        }

        internal Scene GoToPreviousScene(SceneConsole sc, VNController gc, Chapter c, Camera cam)
        {
            var currentScene = c.Current;
            if (currentScene.HasPrev)
            {
                var camdata = currentScene.Back();
                camdata.setCamera(cam, gc);
                return currentScene;
            }
            if (!c.HasPrev) return currentScene;
            else
            {
                return LoadPreviousScene(sc, gc, c, cam, true);
            }
        }

        internal Scene LoadPreviousScene(SceneConsole sc, VNController gc, Chapter c, Camera cam)
        {
            return LoadPreviousScene(sc, gc, c, cam, false);
        }

        internal void MoveSceneForward(Chapter c)
        {
            if (!c.HasNext && HasNext)
            {
                var scene = c.Remove();
                Next().Prepend(scene);
            }
            else
            {
                c.MoveItemForward();
            }

        }

        internal void MoveSceneBackward(Chapter chapter)
        {

            if (!chapter.HasPrev && HasPrev)
            {
                var scene = chapter.Remove();
                var prevChapter = Back();
                prevChapter.Add(scene);
            }
            else
            {
                chapter.MoveItemBack();
            }
        }

        public void AddChapter()
        {
            Add(new Chapter());
        }

        public Chapter InsertChapter()
        {
            var chapter = new Chapter();
            Insert(chapter);
            return chapter;
        }

        public Chapter SplitChapter(Chapter c)
        {
            if (!c.HasItems) return c;
            var currentIndex = c.CurrentIndex;
            var scenes = c.RemoveUntilEnd(currentIndex + 1);
            var new_chapter = new Chapter(scenes, null);
            return Insert(new_chapter);
        }

        public void MergeChapters()
        {
            var currentChapter = Current;
            if (!HasNext) return;

            var currentIndex = currentChapter.CurrentIndex;

            var nextChapter = Items[currentIndex + 1];

            currentChapter.AddRange(nextChapter.ExportItems().ToList());

            Remove(currentIndex + 1);


        }

        public void DuplicateChapter()
        {
            Duplicate();
        }

        public void RemoveChapter()
        {
            Remove();
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

        internal Scene DrawNextPrevButtons(SceneConsole sc, VNController gc, Chapter c, Camera cam)
        {
            Scene s = null;
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Prev scene", GUILayout.Height(UI.RowHeight), GUILayout.Width(UI.ColumnWidth * 0.5f)))
            {
                s = LoadPreviousScene(sc, gc, c, cam);
            }
            if (GUILayout.Button("Next scene", GUILayout.Height(UI.RowHeight), GUILayout.Width(UI.ColumnWidth * 0.5f)))
            {
                s = LoadNextScene(c);
            }
            GUILayout.EndHorizontal();
            return s;
        }

        internal Warning? DrawChapterEditButtons(SceneConsole sc, Chapter c, Camera cam, bool promptOnDelete)
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

        internal void DrawChapterButtons(SceneConsole sc)
        {
            var i = 0;
            foreach (var chapter in this)
            {

                var chapterSelected = i == CurrentIndex;

                var col = chapterSelected
                    ? SelectedTextColor
                    : NormalTextColor;

                var chapterName = chapter.Name ?? $"Chapter {i + 1}";

                if (Items[i].HasItems)
                {   
                    chapterName += $" ({Items[i].Count})";
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
                case SelectedItem.Chapter when HasItems:
                    return ItemNames[CurrentIndex];
                case SelectedItem.Scene when Current.HasItems:
                    return Current.ItemNames[Current.CurrentIndex];
                default:
                    return "";
            }
        }

        internal void DrawSceneTab(SceneConsole sc, VNController gc, Chapter c, Camera cam)
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

            DrawChapterButtons(sc);

            GUILayout.EndScrollView();

            if (!c.HasItems) return;

            GUILayout.FlexibleSpace();

            DrawMoveUpDownButtons(c);
            DrawMoveUpDownButtons();
            DrawNextPrevButtons(sc, gc, c, cam);
        }
    }
}
