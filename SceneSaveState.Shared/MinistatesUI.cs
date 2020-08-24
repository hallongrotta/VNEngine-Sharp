using Studio;
using System;
using System.Collections.Generic;
using UnityEngine;
using VNActor;
using VNEngine;
using static SceneSaveState.SceneConsole;

namespace SceneSaveState
{
    public static partial class UI
    {
        public static void sceneConsoleMinistates()
        {
            int i;
            string[] ar;
            // updating autostates - every 200 step
            if (Instance.updAutoStatesTimer == 0)
            {
                Utils.recalc_autostates();
            }
            Instance.updAutoStatesTimer = (Instance.updAutoStatesTimer + 1) % 200;
            var tableLabelW = 90;
            var tableBtnW = 125;
            var tablePadding = 10;
            // sc.svname = GUILayout.TextField(sc.svname)
            // GUILayout.Space(35)
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Ministates - add states only for selected actors+props. No tracking. Auto-save into scene.\nYou can use prefixes, naming by \"prefix-name\"");
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(15);
            GUILayout.BeginHorizontal();
            GUILayout.Space(tablePadding);
            if (GUILayout.Button("Add state", GUILayout.Width(200)))
            {
                Instance.addSelectedMini();
            }
            GUILayout.FlexibleSpace();
            GUILayout.Label("(optional) custom name: ");
            // GUILayout.Label("  Who say:", GUILayout.Width(80))
            Instance.mininewid = GUILayout.TextField(Instance.mininewid, GUILayout.Width(120));
            GUILayout.Space(tablePadding);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Space(tablePadding);
            Instance.isUseMsAuto = GUILayout.Toggle(Instance.isUseMsAuto, "Use auto-states (operations with selected props)");
            GUILayout.Space(tablePadding);
            GUILayout.EndHorizontal();
            if (Instance.isUseMsAuto)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(tablePadding);
                if (GUILayout.Button("Add Show/Hide", GUILayout.Width(100)))
                {
                    Instance.addSelectedAutoShow("vis");
                }
                if (GUILayout.Button("Add Choice", GUILayout.Width(100)))
                {
                    Instance.addSelectedAutoShow("choice");
                }
                if (GUILayout.Button("Del selected", GUILayout.Width(100)))
                {
                    Instance.delSelectedAutoShow();
                }
                GUILayout.FlexibleSpace();
                GUILayout.Label("(opt) name: ");
                // GUILayout.Label("  Who say:", GUILayout.Width(80))
                Instance.autoshownewid = GUILayout.TextField(Instance.autoshownewid, GUILayout.Width(100));
                GUILayout.Space(tablePadding);
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                GUILayout.Space(tablePadding);
                GUILayout.FlexibleSpace();
                GUILayout.Space(tablePadding);
                GUILayout.EndHorizontal();
            }
            GUILayout.Space(20);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label(" ----------------------------------------    Ministates     ----------------------------------------");
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(15);
            GUILayout.BeginHorizontal();
            GUILayout.Space(tablePadding);
            miniset_scroll = GUILayout.BeginScrollView(miniset_scroll);
            //for i in range(500):
            //    GUILayout.Label("State %s" % (str(i)))
            var mslist = Ministates.ministates_get_list(Instance.game);
            // calculating prfixes
            var arPrefixes = new List<string> {
            ""
        };
            foreach (var el in mslist)
            {
                //mstate = HSNeoOCIFolder.create_from_treenode(fldMiniState)
                ar = Ministates.ministates_calc_prefix(el.name);
                if (arPrefixes.Contains(ar[0]))
                {
                }
                else
                {
                    arPrefixes.Add(ar[0]);
                }
            }
            // rendering ministates
            foreach (var prefix in arPrefixes)
            {
                GUILayout.Space(6);
                GUILayout.BeginHorizontal();
                var prefixtxt = prefix;
                if (prefixtxt == "")
                {
                    prefixtxt = "(default)";
                }
                GUILayout.Label(prefixtxt + ":", GUILayout.Width(tableLabelW));
                GUILayout.BeginVertical();
                GUILayout.BeginHorizontal();
                i = 0;
                foreach (var el in mslist)
                {
                    ar = Ministates.ministates_calc_prefix(el.name);
                    if (ar[0] == prefix)
                    {
                        i += 1;
                        if (GUILayout.Button(ar[1], GUILayout.Width(tableBtnW)))
                        {
                            try
                            {
                                Ministates.ministates_run_elem(Instance.game, el.obj);
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.ToString());
                                Instance.show_blocking_message_time_sc(String.Format("Error during set state: {0}", e.ToString()));
                                //return
                                //if i != 0 and (i % 3 == 0):
                            }
                        }
                    }
                    if (i % 3 == 0)
                    {
                        GUILayout.FlexibleSpace();
                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal();
                    }
                }
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                GUILayout.EndVertical();
                GUILayout.EndHorizontal();
            }
            try
            {
                // trying auto states - to avoid errors during making UI
                foreach (var el0 in Instance.arAutoStatesItemsVis)
                {
                    TreeNodeObject el = el0.treeNodeObject.parent;
                    if (el.textName != "")
                    {
                    }
                }
                foreach (var el0 in Instance.arAutoStatesItemsChoice)
                {
                    TreeNodeObject el = el0.treeNodeObject.parent;
                    if (el.textName != "")
                    {
                    }
                    foreach (var el2 in el.child)
                    {
                        if (el2.textName != "")
                        {
                        }
                        if (el2.visible)
                        {
                        }
                    }
                }
                // rendering auto vis
                if (Instance.arAutoStatesItemsVis.Count > 0)
                {
                    GUILayout.Space(6);
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("A SHOW/HIDE:", GUILayout.Width(tableLabelW + 5));
                    GUILayout.BeginVertical();
                    GUILayout.BeginHorizontal();
                    i = 0;
                    foreach (var vs in Instance.arAutoStatesItemsVis)
                    {
                        Folder vis = (Folder)vs;
                        ar = vis.text_name.Split(':');
                        i += 1;
                        try
                        {
                            if (ar[1] == "vis")
                            {
                                if (GUILayout.Button(Utils.btntext_get_if_selected2(ar[2], vis.treeNodeObject.parent.visible), GUILayout.Width(tableBtnW)))
                                {
                                    vis.treeNodeObject.parent.visible = !vis.treeNodeObject.parent.visible;
                                    if (vis.treeNodeObject.parent.visible)
                                    {
                                        if (Utils.treenode_check_select(vis.treeNodeObject.parent))
                                        {
                                        }
                                        else
                                        {
                                            Instance.game.studio.treeNodeCtrl.SelectSingle(vis.treeNodeObject.parent);
                                        }
                                    }
                                    else if (Utils.treenode_check_select(vis.treeNodeObject.parent))
                                    {
                                        Instance.game.studio.treeNodeCtrl.SelectSingle(vis.treeNodeObject.parent);
                                    }
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            Instance.show_blocking_message_time_sc(String.Format("Error during set visible: {0}", e.ToString()));
                            // return
                            // if i != 0 and (i % 3 == 0):
                        }
                        if (i % 3 == 0)
                        {
                            GUILayout.FlexibleSpace();
                            GUILayout.EndHorizontal();
                            GUILayout.BeginHorizontal();
                        }
                    }
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();
                    GUILayout.EndVertical();
                    GUILayout.EndHorizontal();
                }
                // rendering choices
                foreach (var ic in Instance.arAutoStatesItemsChoice)
                {
                    GUILayout.Space(6);
                    GUILayout.BeginHorizontal();
                    var lbname = "--tmp--";
                    Folder itchoice = (Folder)ic;
                    try
                    {
                        ar = itchoice.text_name.Split(':');
                        lbname = ar[2];
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Err during calc label name: " + e.ToString());
                    }
                    GUILayout.Label(lbname, GUILayout.Width(tableLabelW));
                    GUILayout.BeginVertical();
                    GUILayout.BeginHorizontal();
                    i = 0;
                    foreach (TreeNodeObject el in itchoice.treeNodeObject.parent.child)
                    {
                        if (!el.textName.StartsWith("-msauto:"))
                        {
                            i += 1;
                            try
                            {
                                var btntext = Utils.btntext_get_if_selected2(el.textName, el.visible);
                                if (GUILayout.Button(btntext, GUILayout.Width(tableBtnW)))
                                {
                                    if (el.visible)
                                    {
                                        el.visible = false;
                                    }
                                    else
                                    {
                                        el.visible = true;
                                        foreach (var el2 in itchoice.treeNodeObject.parent.child)
                                        {
                                            if (el2 != el)
                                            {
                                                el2.visible = false;
                                            }
                                        }
                                    }
                                    Instance.game.studio.treeNodeCtrl.SelectSingle(el);
                                }
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine("Err during render button choice: " + e.ToString());
                            }
                            if (i % 3 == 0)
                            {
                                GUILayout.FlexibleSpace();
                                GUILayout.EndHorizontal();
                                GUILayout.BeginHorizontal();
                            }
                        }
                    }
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();
                    GUILayout.EndVertical();
                    GUILayout.EndHorizontal();
                }
            }
            catch (Exception e)
            {
                //print "VNGE SSS: try to recalc autostates...."
                GUILayout.Label(Utils.color_text_red("Trying to get autostates folders: " + e.ToString()));
                Utils.recalc_autostates();
                //return
                // end of all elements
            }
            GUILayout.EndScrollView();
            GUILayout.Space(tablePadding);
            GUILayout.EndHorizontal();
        }
    }
}
