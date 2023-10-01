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

        internal Scene GoToNextScene(Chapter c)
        {
           return c.HasNext? c.Next() : Next().First();
        }

        internal Scene GoToPreviousScene(Chapter c)
        {

            return c.HasPrev ? c.Back() : Back().Last();
        }

        internal Scene GoToPreviousSceneOrCam(VNController gc, Chapter c, Camera cam)
        {
            var currentScene = c.Current;
            if (currentScene.HasPrev)
            {
                var camdata = currentScene.Back();
                camdata.setCamera(cam, gc);
                return null;
            }
            return GoToPreviousScene(c);
        }

        internal Scene GoToNextSceneOrCam(VNController gc, Chapter c, Camera cam)
        {
            var currentScene = c.Current;
            if (currentScene.HasPrev)
            {
                var camdata = currentScene.First();
                camdata.setCamera(cam, gc);
                return null;
            }
            return GoToNextScene(c);
        }

        internal void MoveSceneForward(Chapter c)
        {
            if (!c.HasNext && HasNext)
            {
                Next().Prepend(c.Remove());
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

        internal void DrawMoveUpDownButtons()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Chapter");
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Move ↑"))
            {
                MoveItemBack();
            }
            if (GUILayout.Button("Move ↓"))
            {
                MoveItemForward();
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

        internal Scene DrawNextPrevButtons(VNController gc, Chapter c, Camera cam)
        {
            Scene s = null;
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Prev scene", GUILayout.Height(UI.RowHeight), GUILayout.Width(UI.ColumnWidth * 0.5f)))
            {
                s = GoToPreviousSceneOrCam(gc, c, cam);
            }
            if (GUILayout.Button("Next scene", GUILayout.Height(UI.RowHeight), GUILayout.Width(UI.ColumnWidth * 0.5f)))
            {
                s = GoToNextSceneOrCam(gc, c, cam);
            }
            GUILayout.EndHorizontal();
            return s;
        }

        private void RemoveChapter()
        {
            Remove();
        }   

        internal Warning? DrawChapterEditButtons(Chapter c, Camera cam, bool promptOnDelete)
        {
            Warning? warning = null;
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Add chapter", GUILayout.Height(RowHeight), GUILayout.Width(ColumnWidth * 0.5f)))
            {
                Add(new Chapter());
            }
            if (GUILayout.Button("Insert chapter", GUILayout.Height(RowHeight), GUILayout.Width(ColumnWidth * 0.5f)))
            {
                Insert(new Chapter());
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Copy chapter", GUILayout.Height(RowHeight), GUILayout.Width(ColumnWidth * 0.5f)))
            {
                Duplicate();
            }
            if (GUILayout.Button("Delete chapter", GUILayout.Height(RowHeight), GUILayout.Width(ColumnWidth * 0.5f)))
            {
                if (promptOnDelete)
                {
                    warning = new Warning("Delete selected scene?", false, RemoveChapter);
                }
                else
                {
                    Remove();
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
                SplitChapter(c);
            }
            GUILayout.EndHorizontal();
            return warning;
        }

        public string SetNameOfSelected()
        {
            return _sceneNameEntry == "" ? null : _sceneNameEntry;
        }

        internal Scene DrawChapterButtons(SceneConsole sc)
        {
            Scene s = null;
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
                    s = SetCurrent(i).First();
                    selectedItem = SelectedItem.Chapter;
                    _sceneNameEntry = "";
                }

                if (chapterSelected)
                {
                    s = DrawSceneButtons(sc, chapter, i, true);
                }
                i++;
            }
            return s;
        }

        internal Scene DrawSceneButtons(SceneConsole sc, Chapter c, int chapterNumber, bool chapterSelected)
        {
            Scene s = null;
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
                    s = SetChapterAndScene(chapterNumber, i);
                    _sceneNameEntry = "";
                    
                }

                GUILayout.EndHorizontal();

            }
            return s;
        }

        internal Scene SetChapterAndScene(int chapterNumber, int sceneNumber)
        {
            var chapter = SetCurrent(chapterNumber);
            return chapter.SetCurrentScene(sceneNumber);
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
            Scene s = null;

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

            s = DrawChapterButtons(sc);

            GUILayout.EndScrollView();

            if (!c.HasItems) return;

            GUILayout.FlexibleSpace();

            DrawMoveUpDownButtons(c);
            DrawMoveUpDownButtons();
            
            s = DrawNextPrevButtons(gc, c, cam) is Scene s2? s2 : s;

            sc.LoadScene(s, cam);

        }
    }
}
