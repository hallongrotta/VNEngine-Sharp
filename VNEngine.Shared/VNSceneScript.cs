using System;
using System.Collections.Generic;
using System.Linq;
using VNActor;
using VNEngine;

namespace VNEngine
{

    public static class VNSceneScript
    {

        //vngame;all;VN Scene Script Editor-Viewer
        // :type game: vngameengine.VNNeoController
        public static void start(VNNeoController game)
        {
            // -------- some options we want to init ---------
            game.btnNextText = ">>";
            //game.isHideWindowDuringCameraAnimation = True # this setting hide game window during animation between cameras
            if (game.isClassicStudio)
            {
                game.show_blocking_message_time("VN Scene Script can't run on Old Studio. Use NEO");
                return;
            }
            //game.set_text_s("Scene script test")
            //game.set_text_s(get_all_infos(game))
            //start_menu(game, {'mode': 'view'})
            //game.set_text_s("---")
            //game.set_buttons_end_game()
            // :paramint:maxstate:5
            // :param:test:testval
            game.set_text_s("Welcome to " + "<b>VN Scene Script</b>!" + "\nIt's a way to implement and run code, located in scene folders text");
            game.set_buttons_alt(new List<string> {
            "Run current scene as VN Scene Script scene",
            start_current_menu,
            "Demos",
            start_demos,
            "Developer utils",
            dev_utils
        });
        }

        public static void dev_utils(VNNeoController game)
        {
            game.set_text_s("Developer utils:\nModify scene in way:");
            game.set_buttons_alt(new List<object> {
            color_text_yellowlight("Add headers >>"),
            dev_utils_headers,
            "",
            null,
            color_text_green("vnscene_acode.txt > :acode"),
            (dutil_syncwithfile_full_acode, (":acode", "vnscene_acode.txt")),
            color_text_red(":acode > vnscene_acode.txt"),
            (dutil_syncwithfile_param_back, (":acode", "vnscene_acode.txt")),
            "vnscene_sync.txt > -syncfile-",
            (dutil_syncwithfile_param, ("-syncfile-", "vnscene_sync.txt")),
            "-syncfile- > vnscene_sync.txt",
            (dutil_syncwithfile_param_back, ("-syncfile-", "vnscene_sync.txt")),
            "vnscene_mline.txt > selected",
            (dutil_syncwithfile_loadmline_to_selected, "vnscene_mline.txt"),
            "<<",
            start
        }, "compact");
        }

        public static void dev_utils_headers(VNNeoController game)
        {
            game.set_text_s("Developer utils:\nHeaders:");
            game.set_buttons_alt(new List<object> {
            color_text_yellowlight("Add template for v25"),
            dutil_header20,
            "Tpl v10 (header+defpack)",
            dutil_header1,
            color_text_blue("AddExt 'vnframe11' and init"),
            dutil_header3,
            color_text_blue("AddExt 'flipsync' and init"),
            dutil_header4,
            "Add :acode",
            dutil_header2,
            "Add dummy text for 10 cameras",
            dutil_adddummytext_forcameras,
            color_text_blue("AddExt 'gagency01'"),
            (dutil_header_param, new List<string> {
                ":useext:gagency01"
            }),
            "<<",
            dev_utils
        }, "compact");
        }

        public static void dutil_header0(VNNeoController game)
        {
            var fold1 = HSNeoOCIFolder.add(":vnscenescript:v25:110");
        }

        public static void dutil_header00(VNNeoController game)
        {
            var fold1 = HSNeoOCIFolder.add(":vnscenescript:v25:30");
        }

        public static void dutil_header20(VNNeoController game)
        {
            var fold1 = HSNeoOCIFolder.add(":vnscenescript:v25:30");
            var lines = new List<string> {
            "next",
            "txtf:s::Some text on cam 1",
            "cam:1",
            "next",
            "txtf:Girl::Hey! This text show on cam 2!",
            "cam:2"
        };
            dutil_acode_addlines(game, (":acode", lines));
        }

        public static void dutil_header1(VNNeoController game)
        {
            var fold1 = HSNeoOCIFolder.add(":vnscenescript:v25:110");
            var fold2 = HSNeoOCIFolder.add(":a:i:util:defpack");
            fold2.set_parent(fold1);
        }

        public static void dutil_header2(VNNeoController game)
        {
            var fold1 = HSNeoOCIFolder.add(":acode");
        }

        public static void dutil_header3(VNNeoController game)
        {
            addaction_to_headerfolder(game, ":useext:vnframe12");
            addaction_to_headerfolder(game, ":a:i:f_stinit");
        }

        public static void dutil_header4(VNNeoController game)
        {
            addaction_to_headerfolder(game, ":useext:flipsync10");
            addaction_to_headerfolder(game, ":a:i:initflipsync:v10");
        }

        public static void dutil_header_param(VNNeoController game, object param)
        {
            foreach (var str in param)
            {
                addaction_to_headerfolder(game, str);
            }
        }

        // :type game: vngameengine.VNNeoController
        public static void start_file(VNNeoController game, object file)
        {
            game.load_scene(file);
            start_current_menu(game);
        }

        // :type game: vngameengine.VNNeoController
        public static void start_demos(VNNeoController game)
        {
            game.set_text_s("Available demos:");
            var btns = new List<object> {
            "0.Simplest demo",
            (start_file, "vnscscriptdemo0.png"),
            "1.Simple demo",
            (start_file, "vnscscriptdemo1.png"),
            "2.Demo for extension making (adv)",
            (start_file, "vnscscriptdemoext.png")
        };
            if (game.isCharaStudio)
            {
                btns += new List<object> {
                "Hot Story (CharaStudio, large story, long load)",
                (start_file, "vnschotstory.png")
            };
            }
            game.set_buttons_alt(btns);
        }

        public static void start_current_menu(VNNeoController game)
        {
            game.set_text_s("Choose mode for run story:");
            game.set_buttons_alt(new List<string> {
            "View mode >",
            start_cur_view,
            "Debug mode (for developers) >",
            start_cur_debug
        });
        }

        // :type game: vngameengine.VNNeoController
        public static void start_cur_view(VNNeoController game)
        {
            game.run_menu(start_menu, new Dictionary<object, object> {
            {
                "mode",
                "view"}}, game_end_buttons);
        }

        // :type game: vngameengine.VNNeoController
        public static void start_cur_debug(VNNeoController game)
        {
            game.run_menu(start_menu, new Dictionary<object, object> {
            {
                "mode",
                "debug"}}, game_end_buttons);
        }

        public static void game_end_buttons(VNNeoController game)
        {
            //game.set_text("s", "Demo finished here... hope you like it and will made something by yourself! :)")
            game.set_text_s("Game ended!");
            game.set_buttons(new List<string> {
            "Restart game >",
            "Return to title >"
        }, new List<object> {
            toRestart,
            toEnd2
        });
        }

        // :type game: vngameengine.VNNeoController
        public static void toEnd2(VNNeoController game)
        {
            cleanup(game);
            game.return_to_start_screen();
        }

        public static void toRestart(VNNeoController game)
        {
            game.run_menu(start_menu, game.scenedata.scRunParams, game_end_buttons);
        }

        // ------------------------ actual VN Scene Script functions ------------------
        // :type game: vngameengine.VNNeoController
        public static void start_menu(VNController game, Dictionary<string, string> param)
        {
            if (param == null)
            {
                param = new Dictionary<string, string> {
                {
                    "mode",
                    "view"}};
            }
            game.scenedata.scRunMode = param["mode"];
            game.scenedata.scRunParams = param;
            // remove frame for preview in CharaStudio
            if (game.scenedata.scRunMode == "view")
            {
                if (game.isCharaStudio)
                {
                    if (game.scene_get_framefile().ToString().startswith("vnscenescriptframe"))
                    {
                        game.scene_set_framefile("");
                    }
                }
            }
            // set all to default
            game.scenedata.scMaxState = 0;
            // initialize script
            try
            {
                game.scenedata.scScriptAll = scene_get_all_infos(game);
            }
            catch (Exception)
            {
                var err = "ERROR: can't analyze scene\n";
                err += String.Format("(details: %s)", e.ToString());
                game.show_blocking_message_time(err);
                return;
            }
            var checkerr = check_correct(game);
            if (checkerr == "")
            {
                txt_to_script_tree(game);
                @"
        print ""--- params ---""
        print game.scenedata.scScriptParams
        print ""--- actions ---""
        print game.scenedata.scScriptActions
        ";
                Console.WriteLine("--- extensions ---");
                Console.WriteLine(game.scenedata.scScriptExtsTxt);
                //print "--- show declares ---"
                //print game.scenedata.scScriptShowDeclares
                load_extensions(game);
                run_state_actions(game, -1);
                var startState = 0;
                if (param.Contains("startState"))
                {
                    startState = param["startState"];
                }
                run_state(game, startState, true);
            }
            else
            {
                game.show_blocking_message_time(checkerr);
            }
        }

        public static void check_correct(VNNeoController game)
        {
            var scrtxt = game.scenedata.scScriptAll;
            try
            {
                foreach (var obj in scrtxt)
                {
                    var txt = obj[0];
                    @":type txt: string";
                    if (txt.startswith(":vnscenescript:"))
                    {
                        var ar = txt.split(":");
                        if (ar[2][0] == "v")
                        {
                            var ver = Convert.ToInt32(ar[2][1]);
                            if (ver == 10 || ver == 20 || ver == 25 || ver == 30)
                            {
                                var maxstate = Convert.ToInt32(ar[3]);
                                if (maxstate > 0)
                                {
                                    game.scenedata.scMaxState = maxstate;
                                    game.scenedata.scVer = ver;
                                    return "";
                                }
                                else
                                {
                                    return "Incorrect maxstate in header (must be >0)\n\n(header must be like :vnscenescript:v10:<maxstate>)";
                                }
                            }
                            else
                            {
                                return "Incorrect VNSceneScript version in header (must be v10)\n\n(header must be like :vnscenescript:v10:<maxstate>)";
                            }
                        }
                        else
                        {
                            return "Sorry, can't find VNSceneScript version in header\n\n(header must be like :vnscenescript:v10:<maxstate>)";
                        }
                    }
                }
            }
            catch (Exception)
            {
                var err = "ERROR: can't check scene header correctness\n";
                err += String.Format("(details: %s)", e.ToString());
                return err;
            }
            return "Sorry, but this scene is not a VNSceneScript\n\n(can't find the :vnscenescript: header)";
        }

        public static void get_all_infos(VNNeoController game)
        {
            var str = "";
            //from UnityEngine import Object
            //from Studio import BackgroundCtrl
            //UnityEngine.Object.FindObjectsOfType < CharAnimeCtrl > ();
            var ar = game.scenedata.scScriptAll;
            foreach (var obj in ar)
            {
                str += obj[0] + "\n";
            }
            return str;
        }

        public static void load_extensions(VNNeoController game)
        {
            var exts = new List<object>();
            var error = "";
            foreach (var ext in game.scenedata.scScriptExtsTxt)
            {
                try
                {
                    var mod = vngameengine.import_or_reload("vnscenescriptext_" + ext);
                    if (mod)
                    {
                        exts.append(mod);
                    }
                    else
                    {
                        error += "Can't find or load extension '" + ext + "'\n";
                    }
                }
                catch (Exception)
                {
                    error += "Can't find or load extension '" + ext + "'\n";
                }
            }
            if (error != "")
            {
                game.show_blocking_message_time("ERROR in loading extensions:\n" + error + "\n(please, download fresh version of VNSceneScript - it may contain this extensions)");
            }
            game.scenedata.scScriptExts = exts;
            return error;
        }

        public static void txt_to_script_tree(VNNeoController game) { throw new NotImplementedException("fixme"); }
        /* {
             object act;
             var scrtxt = game.scenedata.scScriptAll;
             var params = new Dictionary<object, object>
             {
             };
             var actions = new List<object>();
             var showdeclares = new List<object>();
             var exts = new List<object>();
             foreach (var obj in scrtxt)
             {
                 var txt = obj[0];
                 @":type txt: string";
                 var ar = txt.split(":");
                 //print ar
                 if (ar.Count > 1)
                 {
                     if (ar[1] == "param")
                     {
                     params[ar[2]] = ar[3];
                 }
                 if (ar[1] == "paramint") {
                     params[ar[2]] = Convert.ToInt32(ar[3]);
                 }
                 if (ar[1] == "a") {
                     act = parse_action_string(txt);
 act["treeobj"] = obj[2];
                     actions.append(act);
                 }
                 if (ar[1] == "show" || ar[1] == "showch") {
                     act = new Dictionary<object, object> {
                     };
                     act["action"] = ar[1];
                     act["states"] = parse_states_str(ar[2]);
 act["treeobj"] = obj[2];
                     showdeclares.append(act);
                 }
                 if (ar[1] == "useext") {
                     exts.append(ar[2]);
                 }
                 // v2.0 feature
                 if (ar[1] == "acode") {
                     if (game.scenedata.scVer >= 20) {
                         //print game.scenedata.scVer
                         var curframe = 0;
                         if (ar.Count > 2) {
                             curframe = Convert.ToInt32(ar[2]);
                         }
                         //treeobj0 = obj[2]
                         @":type treeobj0: dummyneoclasses.TreeNodeObject";
                         //targ = treeobj0.child[0]
                         var targ = obj[2];
                         foreach (var treeobj in targ.child) {
                             var treeobj = treeobj;
                             @":type treeobj: dummyneoclasses.TreeNodeObject";
                             //ar2.append(HSNeoOCI.create_from_treenode(treeobj))
                             var res = acode_parse(treeobj, curframe);
                             if (res is int) {
                                 curframe = res;
                             } else {
                                 actions.append(res);
                             }
                             // process subchilds
                             foreach (var treeobj2 in treeobj.child) {
                                 res = acode_parse(treeobj2, curframe);
                                 if (res is int) {
                                     curframe = res;
                                 } else {
                                     actions.append(res);
                                 }
                             }
                         }
                     } else {
                         Console.WriteLine("Error: acode support for versions 2.0 and above, cur version is {0}".format(game.scenedata.scVer));
                     }
                 }
             }
         }
         game.scenedata.scScriptParams = params;
         game.scenedata.scScriptActions = actions;
         game.scenedata.scScriptShowDeclares = showdeclares;
         game.scenedata.scScriptExtsTxt = exts;
     }*/

        public static void acode_parse(object treeobj, object curframe)
{
    if (treeobj.textName == "next")
    {
        curframe += 1;
        return curframe;
    }
    else if (treeobj.textName.startswith("nextf:"))
    {
        var arr = treeobj.textName.split(":");
        curframe = Convert.ToInt32(arr[1]);
        return curframe;
    }
    else
    {
        var acttext = ":a:{0}:{1}".format(curframe, treeobj.textName);
        // print acttext
        var act = parse_action_string(acttext);
        act["treeobj"] = treeobj;
        //actions.append(act)
        return act;
    }
}

public static Dictionary<object, object> parse_action_string(object txt)
{
    var ar = txt.split(":", 6);
    var act = new Dictionary<object, object>
    {
    };
    // act["arcode"] = ar
    act["origintext"] = txt;
    act["states"] = parse_states_str(ar[2]);
    act["action"] = ar[3];
    if (ar.Count > 4)
    {
        act["actionparam"] = ar[4];
        if (ar.Count > 5)
        {
            act["actionparam2"] = ar[5];
            if (ar.Count > 6)
            {
                act["actionparam3"] = ar[6];
            }
        }
    }
    return act;
}

public static void util_action_append(VNNeoController game, object actiontxt)
{
    game.scenedata.scScriptActions.append(parse_action_string(actiontxt));
}

public static void parse_states_str(object str)
{
    if (str == "i")
    {
        return new List<int> {
                -1
            };
    }
    var result = new List<object>();
    var ar1 = str.split(",");
    foreach (var el in ar1)
    {
        var ar2 = el.split("-");
        if (ar2.Count == 1)
        {
            // no '-' - so simple case like 2
            result.append(Convert.ToInt32(ar2[0]));
        }
        else
        {
            // case 1-3
            result += Enumerable.Range(Convert.ToInt32(ar2[0]), Convert.ToInt32(ar2[1]) + 1 - Convert.ToInt32(ar2[0]));
        }
    }
    return result;
    // simple now
    //return [int(str)]
}

// :type game: vngameengine.VNNeoController
public static void scene_get_all_infos(VNNeoController game)
{
    var ar = new List<object>();
    var dobjctrl = game.studio.dicInfo;
    foreach (var key in dobjctrl.Keys)
    {
        var objctrl = dobjctrl[key];
        if (objctrl is OCIFolder)
        {
            var txt = objctrl.name;
            if (txt[0] == ":")
            {
                // all starting with :
                ar.append((objctrl.name, objctrl, key));
            }
        }
    }
    return ar;
}

public static void run_state_wr(VNNeoController game, object param)
{
    run_state(game, param[0], param[1]);
}

public static void run_ministate(VNNeoController game, object param)
{
    libministates.ministates_run_elem_by_name(game, param);
}

// :type game: vngameengine.VNNeoController
public static void run_state(VNNeoController game, object state, bool skipnull)
{
    var isRunCycle = true;
    while (isRunCycle)
    {
        // if finished - go out
        if (state == game.scenedata.scMaxState + 1 || state >= 1000000)
        {
            // 1000000 is enough for all
            if (state >= 1000000)
            {
                Console.WriteLine("VNGE: VNSceneScript: emergency out, state is over 1000000");
            }
            cleanup(game);
            game.menu_finish(state);
            return;
        }
        game.scenedata.scNextState = state + 1;
        game.scenedata.scIsTimerNext = false;
        game.scenedata.scACustomButtons = new List<object>();
        // running actions
        // at first - obviously actions
        var cntactions = run_state_actions(game, state);
        // hide unneeded
        var cntshow = 0;
        var cnthide = 0;
        foreach (var act in game.scenedata.scScriptShowDeclares)
        {
            if (act["action"] == "show")
            {
                if (act["states"].Contains(state))
                {
                    if (set_treeobj_visible(act["treeobj"], true))
                    {
                        cntshow += 1;
                    }
                }
            }
            if (act["action"] == "showch")
            {
                if (act["states"].Contains(state))
                {
                    if (set_treeobj_visible(act["treeobj"].parent.parent.parent, true))
                    {
                        cntshow += 1;
                    }
                }
            }
        }
        foreach (var act in game.scenedata.scScriptShowDeclares)
        {
            if (act["action"] == "show")
            {
                if (!act["states"].Contains(state))
                {
                    if (set_treeobj_visible(act["treeobj"], false))
                    {
                        cnthide += 1;
                    }
                }
            }
            if (act["action"] == "showch")
            {
                if (!act["states"].Contains(state))
                {
                    if (set_treeobj_visible(act["treeobj"].parent.parent.parent, false))
                    {
                        cnthide += 1;
                    }
                }
            }
        }
        //print "State %s: actions %s, show %s, hide %s"%(str(state),str(cntactions),str(cntshow),str(cnthide))
        var nextstate = game.scenedata.scNextState;
        isRunCycle = false;
        //... but....
        if (skipnull)
        {
            // if we have no actions - progress to next state
            if (cntactions == 0 && cntshow == 0)
            {
                //run_state(game, nextstate, True)
                state = nextstate;
                isRunCycle = true;
                //return
                // end IS RUN CYCLE to find first non-empty state
                // otherwise, set buttons
            }
        }
    }
    if (game.scenedata.scIsTimerNext)
    {
        // setted timer for next state, no need to buttons
        game.set_buttons_alt(new List<object>());
    }
    else if (game.scenedata.scACustomButtons.Count > 0)
    {
        // we have action-defined buttons - so, render them
        game.set_buttons_alt(game.scenedata.scACustomButtons, "compact");
    }
    else
    {
        if (game.scenedata.scRunMode == "view")
        {
            game.set_buttons_alt(new List<object> {
                    ">>",
                    (run_state_wr, (nextstate, true))
                });
        }
        if (game.scenedata.scRunMode == "debug")
        {
            var btnsalt = new List<object> {
                    String.Format("Cur state %s >>", state.ToString()),
                    (run_state_wr, (nextstate, true)),
                    String.Format("Cur state %s >> (no skip)", state.ToString()),
                    (run_state_wr, (nextstate, false)),
                    "Save :a:st:camo:<campos>",
                    (dutil_campos, state.ToString())
                };
            foreach (var ext in game.scenedata.scScriptExts)
            {
                try
                {
                    btnsalt += ext.debug_buttons(game, state);
                }
                catch (Exception)
                {
                }
            }
            game.set_buttons_alt(btnsalt, "compact");
        }
        if (game.scenedata.scRunMode == "hiddenscript")
        {
            // don't render buttons - it's up to main game
        }
    }
    game.scenedata.scLastRunnedState = state;
    game.scenedata.scNextExpectedState = nextstate;
}

public static int run_state_actions(VNNeoController game, object state)
{
    var cntprocessed = 0;
    foreach (var act in game.scenedata.scScriptActions)
    {
        if (act["states"].Contains(state))
        {
            var res = run_action(game, act);
            if (!res)
            {
                Console.WriteLine(String.Format("Error: Can't find action like '%s' in '%s'", act["action"], act["origintext"]));
            }
            cntprocessed += 1;
        }
    }
    return cntprocessed;
}

public static bool set_treeobj_visible(object treeobj, bool visible)
{
    if (treeobj.visible != visible)
    {
        treeobj.SetVisible(visible);
        return true;
    }
    return false;
}

// :type game: vngameengine.VNNeoController
public static bool run_action(VNNeoController game, object act)
{
    object st;
    object v3;
    object camobj;
    object text;
    // nul action in 2.0
    if (act["action"] == "nul")
    {
        return true;
    }
    // ------- texts --------
    if (act["action"] == "txt")
    {
        text = act["actionparam2"].replace("\\n", "\n");
        game.set_text(act["actionparam"], text);
        return true;
    }
    if (act["action"] == "txtf")
    {
        text = act["actionparam3"].replace("\\n", "\n");
        game.set_text(act["actionparam"], text);
        return true;
    }
    if (act["action"] == "txts")
    {
        text = act["actionparam"].replace("\\n", "\n");
        game.set_text_s(text);
        return true;
    }
    if (act["action"] == "regchar")
    {
        game.register_char(act["actionparam"], act["actionparam2"], act["actionparam3"]);
        return true;
    }
    // ------- cameras --------
    if (act["action"] == "cam")
    {
        game.to_camera(Convert.ToInt32(act["actionparam"]));
        return true;
    }
    if (act["action"] == "camanim")
    {
        game.anim_to_camera_num(1, Convert.ToInt32(act["actionparam"]), "fast-slow");
        return true;
    }
    if (act["action"] == "camanim2")
    {
        game.anim_to_camera_num(2, Convert.ToInt32(act["actionparam"]), new Dictionary<object, object> {
                {
                    "style",
                    "linear"},
                {
                    "target_camera_zooming_in",
                    float(act["actionparam2"])}});
        return true;
    }
    if (act["action"] == "camcuranim" || act["action"] == "camcur")
    {
        //game.anim_to_camera_num(1,int(act["actionparam"]),"fast-slow")
        camobj = game.get_camera_num(0);
        if (act["actionparam"] == "roty")
        {
            v3 = camobj["rotate"];
            camobj["rotate"] = game.vec3(v3.x, v3.y + float(act["actionparam2"]), v3.z);
        }
        if (act["actionparam"] == "rotz")
        {
            v3 = camobj["rotate"];
            camobj["rotate"] = game.vec3(v3.x, v3.y, v3.z + float(act["actionparam2"]));
        }
        if (act["actionparam"] == "rotx")
        {
            v3 = camobj["rotate"];
            camobj["rotate"] = game.vec3(v3.x + float(act["actionparam2"]), v3.y, v3.z);
        }
        if (act["actionparam"] == "disz")
        {
            v3 = camobj["distance"];
            camobj["distance"] = game.vec3(v3.x, v3.y, v3.z + float(act["actionparam2"]));
        }
        // target
        if (act["action"] == "camcuranim")
        {
            game.anim_to_camera_obj(1, camobj, "fast-slow");
        }
        if (act["action"] == "camcur")
        {
            game.move_camera_obj(camobj);
        }
        return true;
    }
    if (act["action"] == "camo" || act["action"] == "camoanim" || act["action"] == "camoanim2" || act["action"] == "camoanim3")
    {
        try
        {
            var ar = act["actionparam"].split(",");
            var pos = (float(ar[0]), float(ar[1]), float(ar[2]));
            var dist = (float(ar[3]), float(ar[4]), float(ar[5]));
            var rot = (float(ar[6]), float(ar[7]), float(ar[8]));
            var fov = float(ar[9]);
            camobj = game.camparams2vec(pos, dist, rot, fov);
            //print "camo: "
            //print camobj
            if (act["action"] == "camo")
            {
                game.move_camera_obj(camobj);
            }
            if (act["action"] == "camoanim")
            {
                game.anim_to_camera_obj(1, camobj, "fast-slow");
            }
            if (act["action"] == "camoanim2")
            {
                game.anim_to_camera_obj(2, camobj, new Dictionary<object, object> {
                        {
                            "style",
                            "linear"},
                        {
                            "target_camera_zooming_in",
                            float(act["actionparam2"])}});
            }
            if (act["action"] == "camoanim3")
            {
                var endparams = act["actionparam3"].split(":");
                var objrun = new Dictionary<object, object> {
                        {
                            "style",
                            endparams[0]}};
                var p1 = float(endparams[1]);
                var p2 = float(endparams[2]);
                var p3 = float(endparams[3]);
                if (p1 != 0.0)
                {
                    objrun["target_camera_zooming_in"] = p1 * 2;
                }
                if (p2 != 0.0)
                {
                    objrun["target_camera_rotating_x"] = p2 * 2;
                }
                if (p3 != 0.0)
                {
                    objrun["target_camera_rotating_z"] = p3 * 2;
                }
                game.anim_to_camera_obj(float(act["actionparam2"]), camobj, objrun);
            }
            return true;
        }
        catch (Exception)
        {
            Console.WriteLine("Error in camo action: " + e.ToString());
        }
    }
    // ------- utils --------
    if (act["action"] == "util")
    {
        run_util(game, act["actionparam"]);
        return true;
    }
    if (act["action"] == "nextstate")
    {
        game.scenedata.scNextState = statestr_to_int(game, act["actionparam"]);
        return true;
    }
    if (act["action"] == "timernext")
    {
        if (act.has_key("actionparam2") && act["actionparam2"].Count > 0)
        {
            game.scenedata.scNextState = statestr_to_int(game, act["actionparam2"]);
        }
        game.scenedata.scIsTimerNext = true;
        game.set_timer(float(act["actionparam"]), _on_timer_next);
        return true;
    }
    if (act["action"] == "addbtn")
    {
        st = statestr_to_int(game, act["actionparam2"]);
        game.scenedata.scACustomButtons.append(act["actionparam"]);
        game.scenedata.scACustomButtons.append((run_state_wr, (st, true)));
        return true;
    }
    if (act["action"] == "addbtnrnd")
    {
        //st = statestr_to_int(game,act["actionparam2"])
        var arr = act["actionparam2"].split(",");
        Console.WriteLine(arr);
        st = statestr_to_int(game, random_choice(arr));
        game.scenedata.scACustomButtons.append(act["actionparam"]);
        game.scenedata.scACustomButtons.append((run_state_wr, (st, true)));
        return true;
    }
    if (act["action"] == "addbtnms")
    {
        //st = statestr_to_int(game,act["actionparam2"])
        game.scenedata.scACustomButtons.append(act["actionparam"]);
        game.scenedata.scACustomButtons.append((run_ministate, act["actionparam2"]));
        return true;
    }
    if (act["action"] == "runms")
    {
        //st = statestr_to_int(game,act["actionparam2"])
        //game.scenedata.scACustomButtons.append(act["actionparam"])
        //game.scenedata.scACustomButtons.append((run_ministate, act["actionparam2"]))
        run_ministate(game, act["actionparam"]);
        return true;
    }
    if (act["action"] == "showui")
    {
        game.visible = true;
        return true;
    }
    if (act["action"] == "hideui")
    {
        game.visible = false;
        if (act.has_key("actionparam") && act["actionparam"].Count > 0)
        {
            //game.scenedata.scNextState = statestr_to_int(game,act["actionparam2"])
            game.set_timer(float(act["actionparam"]), show_ui);
        }
        return true;
    }
    if (act["action"] == "unlockui")
    {
        game.isHideGameButtons = false;
        return true;
    }
    if (act["action"] == "lockui")
    {
        game.isHideGameButtons = true;
        if (act.has_key("actionparam") && act["actionparam"].Count > 0)
        {
            game.set_timer(float(act["actionparam"]), unlock_ui);
        }
        return true;
    }
    // ------- extensions --------
    foreach (var ext in game.scenedata.scScriptExts)
    {
        if (ext.custom_action(game, act))
        {
            return true;
        }
    }
    return false;
}

public static void show_ui(VNNeoController game)
{
    game.visible = true;
}

public static void unlock_ui(VNNeoController game)
{
    game.isHideGameButtons = false;
}

public static int statestr_to_int(VNNeoController game, object statestr)
{
    if (statestr == "end")
    {
        //print "end, ", game.scenedata.scMaxState + 1
        return game.scenedata.scMaxState + 1;
    }
    if (statestr == "next")
    {
        //print "next: ", game.scenedata.scNextState
        return game.scenedata.scNextState;
    }
    return Convert.ToInt32(statestr);
}

// :type game: vngameengine.VNNeoController
public static void _on_timer_next(VNNeoController game)
{
    // simple hack - run 0 button in game
    //game.call_game_func(game._vnButtonsActions[0])
    run_state(game, game.scenedata.scNextExpectedState, true);
}

public static void run_util(VNNeoController game, string param)
{
    // utils for start
    if (param == "cam110")
    {
        foreach (var i in Enumerable.Range(0, 10))
        {
            var atxt = String.Format(":a:%s:cam:%s", ((i + 1) * 10).ToString(), (i + 1).ToString());
            util_action_append(game, atxt);
        }
    }
    if (param == "charsg4b4")
    {
        var gcolor = "ee99ee";
        var gcolor3 = "99ee99";
        var bcolor = "9999ee";
        game.register_char("q", "aaaaaa", "? ? ?");
        game.register_char("g", gcolor, "Girl");
        game.register_char("gs", gcolor, "Girls");
        game.register_char("g1", gcolor, "Girl 1");
        game.register_char("g2", gcolor, "Girl 2");
        game.register_char("g3", gcolor3, "Girl 3");
        game.register_char("b", bcolor, "Boy");
        game.register_char("bs", bcolor, "Boys");
        game.register_char("b1", bcolor, "Boy 1");
        game.register_char("b2", bcolor, "Boy 2");
    }
    if (param == "defpack")
    {
        run_util(game, "cam110");
        run_util(game, "charsg4b4");
    }
}

// new in 1.1.
// 
//     scrtxt = game.scenedata.scScriptAll
//     for obj in scrtxt:
//         txt = obj[0]
//         if txt.startswith(':vnscenescript:'):
//             return HSNeoOCIFolder(obj[1])
//     return None
//     
public static HSNeoOCIFolder get_headerfolder(VNNeoController game)
{
    return HSNeoOCIFolder.find_single_startswith(":vnscenescript:");
}

public static void addaction_to_headerfolder(VNNeoController game, object name)
{
    var fold = HSNeoOCIFolder.add(name);
    fold.set_parent(get_headerfolder(game));
}

public static void dutil_campos(VNNeoController game, object state)
{
    //game.show_blocking_message_time(game.camera_calcstr_for_vnscene())
    var c = game.studio.cameraCtrl;
    var cdata = c.cameraData;
    var s1 = String.Format("%s,%s,%s,23.0", cdata.pos.ToString(), cdata.distance.ToString(), cdata.rotate.ToString());
    var calcstr = String.Format(":a:%s:camo:%s", state.ToString(), s1.replace("(", "").replace(")", "").replace(" ", ""));
    //game.show_blocking_message_time(calcstr)
    var fold = HSNeoOCIFolder.add(calcstr);
    fold.set_parent(get_headerfolder(game));
    game.show_blocking_message_time(String.Format("Action saved for state %s under vnscenescript header!\n(%s)", state.ToString(), calcstr));
}

public static void dutil_adddummytext_forcameras(VNNeoController game)
{
    foreach (var i in Enumerable.Range(0, 10))
    {
        var atxt = String.Format(":a:%s:txt:s:(text on cam %s)", ((i + 1) * 10).ToString(), (i + 1).ToString());
        addaction_to_headerfolder(game, atxt);
    }
    game.show_blocking_message_time("Actions added to vnscenescript header!");
}

public static void dutil_syncwithfile(VNNeoController game)
{
    dutil_syncwithfile_full(game, "-syncfile-", "vnscene_sync.txt");
}

public static void dutil_syncwithfile_param(VNNeoController game, object param)
{
    dutil_syncwithfile_full(game, param[0], param[1]);
}

public static void dutil_syncwithfile_param_back(VNNeoController game, object param)
{
    dutil_syncwithfile_full_back(game, param[0], param[1]);
}

public static void dutil_syncwithfile_full(VNNeoController game, object foldername, object filename)
{
    var fold = HSNeoOCIFolder.find_single(foldername);
    if (fold == null)
    {
        // create
        var header = get_headerfolder(game);
        fold = HSNeoOCIFolder.add(foldername);
        if (header != null)
        {
            fold.set_parent(header);
        }
    }
    var fname = filename;
    try
    {
        using (var f = codecs.open(fname, encoding: "utf-8"))
        {
            content = f.readlines();
        }
        // remove whitespace characters like `\n` at the end of each line
        var content = (from x in content
                       select x.strip()).ToList();
        // remove old content
        fold.delete_all_children();
        foreach (var x in content)
        {
            if (!x.startswith("#"))
            {
                if (x != "")
                {
                    var newfld = HSNeoOCIFolder.add(x);
                    newfld.set_parent(fold);
                }
            }
        }
    }
    catch (Exception)
    {
        game.show_blocking_message_time(String.Format("Can't find or read file %s in game root folder", fname));
    }
}

public static void dutil_syncwithfile_full_acode(VNNeoController game, object param)
{
    var _tup_1 = param;
    var foldername = _tup_1.Item1;
    var filename = _tup_1.Item2;
    var fold = HSNeoOCIFolder.find_single(foldername);
    if (fold == null)
    {
        // create
        var header = get_headerfolder(game);
        fold = HSNeoOCIFolder.add(foldername);
        if (header != null)
        {
            fold.set_parent(header);
        }
    }
    var fname = filename;
    try
    {
        using (var f = codecs.open(fname, encoding: "utf-8"))
        {
            content = f.readlines();
        }
        // remove whitespace characters like `\n` at the end of each line
        var content = (from x in content
                       select x.strip()).ToList();
        // remove old content
        fold.delete_all_children();
        var fldnext = fold;
        foreach (var x in content)
        {
            if (!x.startswith("#"))
            {
                if (x != "")
                {
                    var newfld = HSNeoOCIFolder.add(x);
                    if (x == "next")
                    {
                        newfld.set_parent(fold);
                        fldnext = newfld;
                    }
                    else
                    {
                        newfld.set_parent(fldnext);
                    }
                }
            }
        }
    }
    catch (Exception)
    {
        game.show_blocking_message_time(String.Format("Can't find or read file %s in game root folder", fname));
    }
}

public static void dutil_acode_addlines(VNNeoController game, object param)
{
    var _tup_1 = param;
    var foldername = _tup_1.Item1;
    var content = _tup_1.Item2;
    var fold = HSNeoOCIFolder.find_single(foldername);
    if (fold == null)
    {
        // create
        //header = get_headerfolder(game)
        fold = HSNeoOCIFolder.add(foldername);
        //if header != None:
        //    fold.set_parent(header)
    }
    var fldnext = fold;
    foreach (var x in content)
    {
        if (!x.startswith("#"))
        {
            if (x != "")
            {
                var newfld = HSNeoOCIFolder.add(x);
                if (x == "next")
                {
                    newfld.set_parent(fold);
                    fldnext = newfld;
                }
                else
                {
                    newfld.set_parent(fldnext);
                }
            }
        }
    }
}

public static void dutil_syncwithfile_full_back(VNNeoController game, object foldername, object filename)
{
    var fold = HSNeoOCIFolder.find_single(foldername);
    if (fold == null)
    {
        game.show_blocking_message_time(String.Format("Can't find folder '%s'", foldername));
        return;
    }
    var fname = filename;
    try
    {
        var f = codecs.open(filename, "w+", encoding: "utf-8");
        foreach (var fch in fold.treeNodeObject.child)
        {
            var obj = HSNeoOCI.create_from_treenode(fch);
            if (obj is HSNeoOCIFolder)
            {
                f.write(String.Format("%s\n", obj.name));
                // process subchildren
                if (obj.treeNodeObject.childCount > 0)
                {
                    foreach (var fch2 in obj.treeNodeObject.child)
                    {
                        var obj2 = HSNeoOCI.create_from_treenode(fch2);
                        if (obj2 is HSNeoOCIFolder)
                        {
                            f.write(String.Format("%s\n", obj2.name));
                        }
                    }
                }
            }
        }
        f.close();
    }
    catch (Exception)
    {
        game.show_blocking_message_time(String.Format("Can't write to file %s in game root folder", fname));
        return;
    }
    game.show_blocking_message_time("Writed to file!");
}

public static void dutil_syncwithfile_loadmline_to_selected(VNNeoController game, object filename)
{
    var fold = HSNeoOCI.create_from_selected();
    if (fold is HSNeoOCIFolder)
    {
        var fname = filename;
        try
        {
            using (var f = codecs.open(fname, encoding: "utf-8"))
            {
                content = f.readlines();
            }
            // remove whitespace characters like `\n` at the end of each line
            var content = (from x in content
                           select x.strip()).ToList();
            fold.name = "\\n".join(content);
        }
        catch (Exception)
        {
            game.show_blocking_message_time(String.Format("Can't find or read file %s in game root folder", fname));
        }
    }
    else
    {
        game.show_blocking_message_time(String.Format("No selected folder; you must select folder to import", fname));
    }
}

public static void cleanup(VNNeoController game)
{
    game.scenedata.scScriptAll = null;
    game.scenedata.scScriptParams = null;
    game.scenedata.scScriptActions = null;
    game.scenedata.scACustomButtons = null;
    game.scenedata.scScriptExts = null;
    game.scenedata.scVer = null;
    //game.
    // util colors
}

public static string color_text(object text, string color)
{
    return "<color=#{1}ff>{0}</color>".format(text, color);
}

public static object color_text_green(object text)
{
    return color_text(text, "aaffaa");
}

public static object color_text_red(object text)
{
    return color_text(text, "ffaaaa");
}

public static object color_text_yellowlight(object text)
{
    return color_text(text, "f8e473");
}

public static object color_text_blue(object text)
{
    return color_text(text, "aaaaff");
}
}

}
