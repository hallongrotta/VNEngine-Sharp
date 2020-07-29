using System;
using System.Collections.Generic;
using System.Text;
using Studio;
using UnityEngine;
using UnityEngine.UI;
using VNActor;

namespace VNEngine
{
    public static class VNFrame
    {

        public static object _sh = null;

        // act function
        // this function read script and do the works
        // script should be dictionary data
        public static void act(VNController game, Dictionary<string, object> script)
        {
            // act script must be a dict
            // script: { 'tgt1' : {'tgt_fuc1' : tgt_func1_param, 'tgt_func2' : tgt_func2_param, ... }, 'tgt2' : {...}, ... }
            // tgt can be "cam" for camera, "sys" for system, actor name for actor or prop name for prop
            foreach (var tgt in script.Keys)
            {
                try
                {
                    // if param is a string
                    if (tgt is string s)
                    {
                        // Handle camera
                        if (s == "cam")
                        {
                            foreach (var f in script[tgt])
                            {
                                if (VNCamera.cam_act_funcs.Contains(f))
                                {
                                    VNCamera.cam_act_funcs[f][0](game, script[tgt][f]);
                                }
                                else
                                {
                                    Console.WriteLine(String.Format("act error: unknown function '%s' for 'cam'", f));
                                }
                            }
                        }
                        else if (s == "sys")
                        {
                            // Handle system
                            foreach (var f in script[tgt])
                            {
                                if (GameSystem.sys_act_funcs.ContainsKey(f))
                                {
                                    GameSystem.sys_act_funcs[f][0](game, script[tgt][f]);
                                }
                                else
                                {
                                    Console.WriteLine(String.Format("act error: unknown function '%s' for 'sys'", f));
                                }
                            }
                        }
                        else if (s == "game_func")
                        {
                            // Handle game_func call
                            var funcs = script[tgt];
                            if (object.ReferenceEquals(typeof(funcs), list))
                            {
                            }
                            else
                            {
                                funcs = new List<object> {
                                funcs
                            };
                            }
                            foreach (var f in funcs)
                            {
                                game.call_game_func(f);
                            }
                        }
                        else if (game.scenef_get_all_actors().Contains(tgt))
                        {
                            // If this is a character in this scene
                            game.scenef_get_actor(tgt).import_status_diff_optimized(script[tgt]);
                        }
                        else if (hasattr(game.scenedata, "actors") && game.scenedata.actors.Contains(tgt))
                        {
                            game.scenedata.actors[tgt].import_status_diff_optimized(script[tgt]);
                            // for f in script[tgt]:
                            //     if f in char_act_funcs:
                            //         char_act_funcs[f][0](game.scenedata.actors[tgt], script[tgt][f])
                            //     else:
                            //         print "act error: unknown function '%s' for '%s'!"%(f, tgt)
                            // If this is a prop in this scene
                        }
                        else if (game.scenef_get_all_props().Contains(tgt))
                        {
                            game.scenef_get_propf(tgt).import_status_diff_optimized(script[tgt]);
                        }
                        else if (hasattr(game.scenedata, "props") && game.scenedata.props.Contains(tgt))
                        {
                            game.scenedata.props[tgt].import_status_diff_optimized(script[tgt]);
                            // for f in script[tgt]:
                            //     if f in prop_act_funcs:
                            //         prop_act_funcs[f][0](game.scenedata.props[tgt], script[tgt][f])
                            //     else:
                            //         print "act error: unknown function '%s' for '%s'!"%(f, tgt)
                            // if this is a clip in the gdata
                        }
                        else if (hasattr(game.gdata, "kfaManagedClips"))
                        {
                            if (game.gdata.kfaManagedClips.Contains(tgt))
                            {
                                game.gdata.kfaManagedClips[tgt].import_status(script[tgt]);
                            }
                            else
                            {
                                Console.WriteLine(String.Format("act error: unknown target '%s'!", tgt));
                            }
                        }
                        else
                        {
                            // else act can not handle it
                            Console.WriteLine(String.Format("act error: unknown target '%s'!", tgt));
                        }
                    }
                    else
                    {
                        // we pass object with state
                        if (tgt is HSNeoOCIChar chara)
                        {
                            // character
                            var actor = chara.as_actor;
                            actor.import_status_diff_optimized(script[tgt]);
                        }
                        else if (tgt is HSNeoOCIProp prop)
                        {
                            // prop - folder or item
                            var obj2 = prop.as_prop;
                            obj2.import_status_diff_optimized(script[tgt]);
                        }
                        else if (tgt is KeyFrameClip clip)
                        {
                            // keyframe clip
                            clip.import_status(script[tgt]);
                        }
                        else
                        {
                            Console.WriteLine(String.Format("act error in process tgt='%s' script='%s'", tgt, script[tgt]) + " : " + e.ToString());
                        }
                    }
                }
                catch (Exception e)
                {
                    traceback.print_exc();
                    Console.WriteLine(String.Format("act error in process tgt='%s' script='%s'", tgt, script[tgt]) + " : " + e.ToString());
                }
            }
        }

        // anime function
        // this function read script and do animation works by sequence
        // script should be tuple data
        public static void anime(VNController game, Dictionary<string, object> script)
        {
            // script = (aniScene, aniScene, ... aniScene)
            // aniScene = (actScript, [duration], [style])
            // actScript like the script used by act function
            // if duration and style is obmitted, then aniScene = actScript, and will be pass to act function directly
            if (!(script is tuple))
            {
                Console.WriteLine("anime function request tuple as script");
                return;
            }
            // init scene anime if not
            init_scene_anime(game);
            // check is old animation done
            if (game.scnAnimeTID != -1)
            {
                Console.WriteLine(String.Format("Old animation TID = %d is running!", game.scnAnimeTID));
                animeClearner(game);
            }
            // save script into list
            game.aniList = new List<object>();
            game.aniIndex = 0;
            foreach (var aniScene in script)
            {
                game.aniList.append(aniScene);
            }
            // start worker
            //print "scene anime start: %d anime/act to do"%len(script)
            if (game.isLockWindowDuringSceneAnimation)
            {
                game.isHideGameButtons = true;
            }
            animeWorker(game);
        }

        public static void init_scene_anime(VNController game)
        {
            // init variables need for scene anime, do only once
            if (hasattr(game, "scnAnimeTID"))
            {
                return;
            }
            game.scnAnimeTID = -1;
            game.isLockWindowDuringSceneAnimation = false;
        }

        public static object animeWorker(VNController game)
        {
            object isRangeParam;
            object asEff;
            object asDur;
            object asAct;
            try
            {
                // take out script
                if (game.aniIndex >= game.aniList.Count)
                {
                    // sync with ScriptHelper?
                    game.scnAnimeTID = -1;
                    if (game.isLockWindowDuringSceneAnimation)
                    {
                        game.isHideGameButtons = false;
                    }
                    //print "scene anime done"
                    return;
                }
                var aniScene = game.aniList[game.aniIndex];
                game.aniIndex += 1;
                // get info and setup 
                if (!(aniScene is tuple) || aniScene.Count == 1)
                {
                    // process act
                    if (aniScene is tuple)
                    {
                        aniScene = aniScene[0];
                    }
                    //print "Act"+str(game.aniIndex-1)+": [Non-Anime]"+str(aniScene)
                    act(game, aniScene);
                    animeWorker(game);
                    return;
                }
                else if (aniScene.Count == 2)
                {
                    asAct = aniScene[0];
                    asDur = aniScene[1];
                    asEff = "linear";
                }
                else
                {
                    asAct = aniScene[0];
                    asDur = aniScene[1];
                    asEff = aniScene[2];
                }
                // auto build range param if it is not
                foreach (var tgt in asAct.Keys)
                {
                    foreach (var fnc in asAct[tgt].Keys)
                    {
                        var param = asAct[tgt][fnc];
                        try
                        {
                            if (param is tuple && param.Count == 2 && paramInterpolater(param[0], param[1], 0.5) != null)
                            {
                                isRangeParam = true;
                            }
                            else
                            {
                                isRangeParam = false;
                            }
                        }
                        catch (Exception e)
                        {
                            isRangeParam = false;
                        }
                        if (!isRangeParam)
                        {
                            var curParam = currentParamBuilder(game, tgt, fnc);
                            if (curParam != null)
                            {
                                asAct[tgt][fnc] = (curParam, param);
                                //print "param [%s][%s] is rebuild as range param"%(tgt, fnc)
                                //print curParam
                                //print "to"
                                //print param
                                //print ""
                            }
                            else
                            {
                                //print "param [%s][%s] is not a range param"%(tgt, fnc)
                            }
                        }
                    }
                }
                //print "Act"+str(game.aniIndex-1)+": "+str(asAct)+", Dur: "+str(asDur)+", Eff: "+str(asEff)
                game.curAnimeAct = asAct;
                game.curAnimeEff = asEff;
                // setup timer and go
                game.scnAnimeTID = game.set_timer(asDur, animeWorker, animeUpdater);
                if (game.scnAnimeTID == -1)
                {
                    Console.WriteLine("animeWorker error: run out for timer resource");
                    animeClearner(game);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("animeWorker error:", game.aniList[game.aniIndex - 1].ToString(), ":", e.ToString());
            }
        }

        public static void animeClearner(VNController game)
        {
            Console.WriteLine(String.Format("animeClearner: start collapse anime from Act%d", game.aniIndex - 1));
            // stop scene anime timer if exists
            if (game.scnAnimeTID != -1)
            {
                game.clear_timer(game.scnAnimeTID);
            }
            game.scnAnimeTID = -1;
            // stop cam anime timer if exists
            if (game.camAnimeTID != -1)
            {
                game.clear_timer(game.camAnimeTID);
            }
            game.camAnimeTID = -1;
            // collapse current and rest animes/acts into one act script
            var actScript = scriptCollapser(game.aniList);
            Console.WriteLine("animeClearner collapsed act: " + script2string(actScript));
            // call act to do collapsed script
            act(game, actScript);
        }

        public static object animeUpdater(VNController game, object dt, object time, float duration)
        {
            try
            {
                // calculate progress
                if (time > duration)
                {
                    time = duration;
                }
                var asProgress = time2progress(time, duration, game.curAnimeEff);
                // handle special system command
                if (game.curAnimeAct.Keys.Contains("sys"))
                {
                    if (game.curAnimeAct["sys"].Contains("wait_anime"))
                    {
                        if (sys_wait_anime(game, game.curAnimeAct["sys"]["wait_anime"]))
                        {
                            //print "stop anime here time = %f"%time
                            game.clear_timer(game.scnAnimeTID, true);
                        }
                    }
                }
                if (game.curAnimeAct.Keys.Contains("sys"))
                {
                    if (game.curAnimeAct["sys"].Contains("wait_voice"))
                    {
                        if (sys_wait_voice(game, game.curAnimeAct["sys"]["wait_voice"]))
                        {
                            //print "stop by voice at time = %f"%time
                            game.clear_timer(game.scnAnimeTID, true);
                        }
                    }
                }
                // set script
                var actScript = new Dictionary<object, object>
                {
                };
                foreach (var tgt in game.curAnimeAct)
                {
                    actScript[tgt] = new Dictionary<object, object>
                    {
                    };
                    foreach (var fnc in game.curAnimeAct[tgt])
                    {
                        var rangeParam = game.curAnimeAct[tgt][fnc];
                        if (rangeParam is tuple)
                        {
                            actScript[tgt][fnc] = paramInterpolater(rangeParam[0], rangeParam[1], asProgress);
                        }
                        else
                        {
                            actScript[tgt][fnc] = rangeParam;
                        }
                    }
                }
                // send to act
                //print actScript
                act(game, actScript);
                // sync with ScriptHelper
                if (_sh != null)
                {
                    _sh.animeTime = time;
                }
            }
            catch (Exception e)
            {
                traceback.print_exc();
                Console.WriteLine("animeUpdater error:", e.ToString());
            }
        }

        public static Dictionary<object, object> scriptCollapser(List<object> orgList)
        {
            object fp;
            object simpleAct;
            object asAct;
            // collapse orgList from colFrom into one act script dict
            //print ">>>>start collapse %d tuple/list"%len(orgList)
            var actScript = new Dictionary<object, object>
            {
            };
            try
            {
                foreach (var i in Enumerable.Range(0, orgList.Count))
                {
                    var aniScene = orgList[i];
                    //print aniScene
                    if (!(aniScene is tuple))
                    {
                        asAct = aniScene;
                        simpleAct = true;
                    }
                    else
                    {
                        asAct = aniScene[0];
                        simpleAct = false;
                    }
                    foreach (var tgt in asAct)
                    {
                        if (!actScript.Contains(tgt))
                        {
                            actScript[tgt] = new Dictionary<object, object>
                            {
                            };
                        }
                        foreach (var f in asAct[tgt])
                        {
                            // take out final param
                            if (simpleAct)
                            {
                                fp = asAct[tgt][f];
                            }
                            else if (asAct[tgt][f] is tuple)
                            {
                                fp = asAct[tgt][f][1];
                            }
                            else
                            {
                                fp = asAct[tgt][f];
                            }
                            // special process for cam
                            if (tgt == "cam")
                            {
                                //print ">> cam param : " + str(fp)
                                fp = camParamUnAnimator(f, fp);
                                //print ">> UnAnimate : " + str(fp)
                                // register final param
                            }
                            actScript[tgt][f] = fp;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(">>>>scriptCollapser Error: " + e.ToString() + "\n");
            }
            return actScript;
        }

        public static void camParamUnAnimator(object camFunc, object camParam)
        {
            // delete cam animation setting from cam act script param
            // WARNING! will delete onCamEnd function if setted!
            if (camFunc == "goto_preset")
            {
                if (!(camParam is tuple))
                {
                    return camParam;
                }
                else
                {
                    if (camParam.Count == 4)
                    {
                        Console.WriteLine(String.Format("Warning from camParamUnAnimator: cam goto_preset onCamEnd func '%s' will be lost!", camParam[3].ToString()));
                    }
                    return camParam[0];
                }
            }
            else if (camFunc == "goto_pos")
            {
                if (camParam.Count == 6)
                {
                    Console.WriteLine(String.Format("Warning from camParamUnAnimator: cam goto_pos onCamEnd func '%s' will be lost!", camParam[5].ToString()));
                }
                return camParam[::3];
            }
            else if (camFunc == "rotate")
            {
                if (camParam.Count == 3 && !(camParam[0] is tuple))
                {
                    return camParam;
                }
                else
                {
                    if (camParam.Count == 4)
                    {
                        Console.WriteLine(String.Format("Warning from camParamUnAnimator: cam rotate onCamEnd func '%s' will be lost!", camParam[3].ToString()));
                    }
                    return camParam[0];
                }
            }
            else if (camFunc == "zoom")
            {
                if (!(camParam is tuple))
                {
                    return camParam;
                }
                else
                {
                    if (camParam.Count == 4)
                    {
                        Console.WriteLine(String.Format("Warning from camParamUnAnimator: cam zoom onCamEnd func '%s' will be lost!", camParam[3].ToString()));
                    }
                    return camParam[0];
                }
            }
            else
            {
                throw new Exception("Unknown cam function " + camFunc);
            }
        }

        public static object currentParamBuilder(VNController game, object tgt, object fnc)
        {
            object caFuncScript;
            if (tgt == "sys" && ("wait_anime", "wait_voice").Contains(fnc))
            {
                return null;
            }
            if (tgt == "cam" && fnc == "goto_pos")
            {
                caFuncScript = ScriptHelper.get_cam_status()["cam"];
            }
            else if (tgt == "sys" && sys_act_funcs.Keys.Contains(fnc) && sys_act_funcs[fnc][1])
            {
                caFuncScript = export_sys_status(game);
            }
            else if (game.scenef_get_all_actors().Contains(tgt) && char_act_funcs.Keys.Contains(fnc) && char_act_funcs[fnc][1])
            {
                caFuncScript = game.scenef_get_actor(tgt).export_full_status();
            }
            else if (hasattr(game.scenedata, "actors") && game.scenedata.actors.Contains(tgt) && char_act_funcs.Keys.Contains(fnc) && char_act_funcs[fnc][1])
            {
                caFuncScript = game.scenedata.actors[tgt].export_full_status();
            }
            else if (game.scenef_get_all_props().Contains(tgt) && prop_act_funcs.Keys.Contains(fnc) && prop_act_funcs[fnc][1])
            {
                caFuncScript = game.scenef_get_propf(tgt).export_full_status();
            }
            else if (hasattr(game.scenedata, "props") && game.scenedata.props.Contains(tgt) && prop_act_funcs.Keys.Contains(fnc) && prop_act_funcs[fnc][1])
            {
                caFuncScript = game.scenedata.props[tgt].export_full_status();
            }
            else
            {
                Console.WriteLine(String.Format("currentParamBuilder error: param of '%s''%s' unknown or can not do animate!", tgt, fnc));
                return null;
            }
            //print "caFuncScript = " + script2string(caFuncScript)
            if (caFuncScript.Keys.Contains(fnc))
            {
                return caFuncScript[fnc];
            }
            else
            {
                Console.WriteLine(String.Format("currentParamBuilder error: '%s' not found in auto build attribute list of '%s'", fnc, tgt));
                return null;
            }
        }

        public static object paramInterpolater(int paramFrom, int paramTo, int progress)
        {
            object subParam;
            // calculate new parameter between paramFrom and paramTo by progress
            if (paramFrom == null || paramTo == null)
            {
                return null;
            }
            else if (paramFrom is tuple || paramFrom is list)
            {
                // interpolate tuple or list, special case: enable interpolate between Vector3 and tuple(3) or list(3)
                if (paramFrom.Count == 3 && paramTo is Vector3)
                {
                    paramTo = (paramTo.x, paramTo.y, paramTo.z);
                }
                else if (type(paramFrom) != type(paramTo) || paramFrom.Count != paramTo.Count)
                {
                    throw new Exception(String.Format("Parameter mismatch %s vs %s! From: ", type(paramFrom).ToString(), type(paramTo).ToString()) + paramFrom.ToString() + " To: " + paramTo.ToString());
                }
                var updParam = new List<object>();
                foreach (var i in Enumerable.Range(0, paramFrom.Count))
                {
                    subParam = paramInterpolater(paramFrom[i], paramTo[i], progress);
                    updParam.append(subParam);
                }
                return tuple(updParam);
            }
            else if (paramFrom is dict)
            {
                // interpolate dictionary
                if (!(paramTo is dict))
                {
                    throw new Exception(String.Format("Parameter mismatch %s vs %s! From: ", type(paramFrom).ToString(), type(paramTo).ToString()) + paramFrom.ToString() + " To: " + paramTo.ToString());
                }
                updParam = new Dictionary<object, object>
                {
                };
                foreach (var ikey in paramFrom.Keys)
                {
                    if (paramTo.Keys.Contains(ikey))
                    {
                        subParam = paramInterpolater(paramFrom[ikey], paramTo[ikey], progress);
                        updParam[ikey] = subParam;
                    }
                }
                return updParam;
            }
            else if (paramFrom is Vector3)
            {
                // interpolate Vector3, special case: enable interpolate between Vector3 and tuple(3) or list(3)
                if ((paramTo is tuple || paramTo is list) && paramTo.Count == 3)
                {
                    paramTo = Vector3(paramTo[0], paramTo[1], paramTo[2]);
                }
                else if (!(paramTo is Vector3))
                {
                    throw new Exception(String.Format("Parameter mismatch %s vs %s! From: ", type(paramFrom).ToString(), type(paramTo).ToString()) + paramFrom.ToString() + " To: " + paramTo.ToString());
                }
                return Vector3.Lerp(paramFrom, paramTo, progress);
            }
            else if (paramFrom is Vector2)
            {
                // interpolate Vector2
                if (paramTo is tuple && paramTo.Count == 2)
                {
                    paramTo = Vector2(paramTo[0], paramTo[1]);
                }
                if (!(paramTo is Vector2))
                {
                    throw new Exception(String.Format("Parameter mismatch %s vs %s! From: ", type(paramFrom).ToString(), type(paramTo).ToString()) + paramFrom.ToString() + " To: " + paramTo.ToString());
                }
                return Vector2.Lerp(paramFrom, paramTo, progress);
            }
            else if (paramFrom is Color)
            {
                // interpolate Color
                if (paramTo is tuple && paramTo.Count == 4)
                {
                    paramTo = Color(paramTo[0], paramTo[1], paramTo[2], paramTo[3]);
                }
                if (!(paramTo is Color))
                {
                    throw new Exception(String.Format("Parameter mismatch %s vs %s! From: ", type(paramFrom).ToString(), type(paramTo).ToString()) + paramFrom.ToString() + " To: " + paramTo.ToString());
                }
                return Color.Lerp(paramFrom, paramTo, progress);
            }
            else
            {
                // try to interpolate as float
                updParam = progress * (paramTo - paramFrom) + paramFrom;
                return updParam;
            }
        }

        public static int time2progress(int time, float duration, List<string> style)
        {
            // calculate progress
            if (time > duration)
            {
                time = duration;
            }
            var progress = time / duration;
            if (style == "slow-fast")
            {
                progress = Mathf.Pow(progress, 2);
            }
            else if (style == "fast-slow")
            {
                progress = 1 - Mathf.Pow(1 - progress, 2);
            }
            else if (style == "slow-fast3")
            {
                progress = Mathf.Pow(progress, 3);
            }
            else if (style == "fast-slow3")
            {
                progress = 1 - Mathf.Pow(1 - progress, 3);
            }
            else if (style == "slow-fast4")
            {
                progress = Mathf.Pow(progress, 4);
            }
            else if (style == "fast-slow4")
            {
                progress = 1 - Mathf.Pow(1 - progress, 4);
            }
            return progress;
        }

        public static void register_actor_prop_by_tag(VNController game)
        {
            // the same as register_actor_prop_by_tag - but do it by core engine calls
            game.scenef_register_actorsprops();
            @"
    tmpActors = game.scenef_get_all_actors()
    tmpProps = game.scenef_get_all_props()
    game.scenedata.actors = {}
    game.scenedata.props = {}
    if game.isStudioNEO:
        for a in tmpActors.Keys:
            game.scenedata.actors[a] = ActorHSNeo(tmpActors[a].objctrl)
        for p in tmpProps.Keys:
            game.scenedata.props[p] = PropHSNeo(tmpProps[p].objctrl)
    elif game.isPlayHomeStudio:
        for a in tmpActors.Keys:
            game.scenedata.actors[a] = ActorPHStudio(tmpActors[a].objctrl)
        for p in tmpProps.Keys:
            game.scenedata.props[p] = PropPHStudio(tmpProps[p].objctrl)
    elif game.isCharaStudio:
        for a in tmpActors.Keys:
            game.scenedata.actors[a] = ActorCharaStudio(tmpActors[a].objctrl)
        for p in tmpProps.Keys:
            game.scenedata.props[p] = PropCharaStudio(tmpProps[p].objctrl)
    else:
        print ""Classic studio not supported!""
    ";
            game.scenedata.actors = game.scenef_get_all_actors();
            var tmpProps = game.scenef_get_all_props();
            game.scenedata.props = new Dictionary<object, object>
            {
            };
            foreach (var p in tmpProps.Keys)
            {
                game.scenedata.props[p] = game.scenef_get_propf(p);
            }
        }

        public static object addProp(object no, VNController game = null, object id = null)
        {
            object ociitem;
            // for StudioNeo and PlayhomeStudio, just use 'no' to identify an item,
            // for CharaStudio, 'no' should be (group, category, no) to identify an item.
            var eid = get_engine_id();
            // add item
            if (eid == "neo" || eid == "phstudio")
            {
                ociitem = AddObjectItem.Add(no);
            }
            else
            {
                ociitem = AddObjectItem.Add(no[0], no[1], no[2]);
            }
            // build prop object
            if (eid == "neo")
            {
                var newprop = PropHSNeo(ociitem);
            }
            else if (eid == "phstudio")
            {
                newprop = PropPHStudio(ociitem);
            }
            else if (eid == "charastudio")
            {
                newprop = PropCharaStudio(ociitem);
            }
            else
            {
                throw new Exception("classic studio not supported");
            }
            // register to props if game and id are set
            if (game != null && id != null)
            {
                if (id == "cam" || id == "sys")
                {
                    throw new Exception("reversed id 'sys' and 'cam' can not be used by user");
                }
                else if (game.scenedata.props.Keys.Contains(id))
                {
                    throw new Exception(String.Format("ID '%s' is already used by prop '%s'", id, game.scenedata.props[id].name));
                }
                else if (game.scenedata.actors.Keys.Contains(id))
                {
                    throw new Exception(String.Format("ID '%s' is already used by actor '%s'", id, game.scenedata.actors[id].name));
                }
                game.scenedata.props[id] = newprop;
            }
            return newprop;
        }

        public static object delProp(object prop, VNController game = null, object id = null)
        {
            // delete from props
            if (game != null && id != null)
            {
                if (game.scenedata.props.Keys.Contains(id))
                {
                    game.scenedata.props.pop(id);
                }
            }
            // delete from studio
            var studio = Studio.Instance;
            studio.DeleteNode(prop.treeNodeObject);
        }

        public static object register_string_resource(VNController game)
        {
            // load string resource dictionary from -strings- folder
            // save game in a global variable for easy access
            game.scenedata.strings = new Dictionary<object, object>
            {
            };
            var strFolder = HSNeoOCIFolder.find_single("-strings-");
            if (strFolder != null)
            {
                foreach (var strTO in strFolder.treeNodeObject.child)
                {
                    try
                    {
                        //print strTO.textName
                        var ss = strTO.textName.split(":", 1);
                        var si = Convert.ToInt32(ss[0]);
                        if (game.scenedata.strings.Keys.Contains(si))
                        {
                            Console.WriteLine(String.Format("Duplicated string id = %d: '%s' was overwrited by 's'", si, game.scenedata.strings[si], ss[1]));
                        }
                        game.scenedata.strings[si] = ss[1];
                    }
                    catch
                    {
                    }
                }
            }
            Console.WriteLine(String.Format("-- Framework: Load %d string resources", game.scenedata.strings.Count));
        }

        public static void load_and_init_scene(VNController game, object pngFile, Action<Tuple<ScriptHelper, bool>> initFunc, int loadWait = 60)
        {
            game.scenePNG = pngFile;
            game.sceneInitFunc = initFunc;
            game.load_scene(pngFile);
            game.scnLoadTID = game.set_timer(loadWait, _load_scene_timeout, _load_scene_wait);
            if (game.scnLoadTID != -1)
            {
                Console.WriteLine("start load scene: " + pngFile);
            }
            else
            {
                game.set_text_s("Fail to load scene because of no more timer available.");
                game.set_buttons_end_game();
            }
        }

        public static object _load_scene_timeout(VNController game)
        {
            // this is called when load scene timeouted
            game.set_text_s("Fail to load scene because of timeout.");
            game.set_buttons_end_game();
        }

        public static object _load_scene_wait(VNController game, object dt, object time, float duration)
        {
            // check if scene is loaded
            if (game.isFuncLocked == false)
            {
                game.clear_timer(game.scnLoadTID);
                // load ext data
                try
                {
                    if (game.isStudioNEO)
                    {
                        var fullpath = path.join(game.get_scene_dir(), game.sceneDir + game.scenePNG);
                        var HSSNES = HSStudioNEOExtSave();
                        HSSNES.LoadExtData(fullpath);
                    }
                }
                catch (Exception e)
                {
                    // ext data may be not necessary, continue on error
                    traceback.print_exc();
                    Console.WriteLine("Unable to load ext data");
                }
                game.sceneInitFunc(game);
                Console.WriteLine("load and init scene done!\n");
            }
        }

        public static string ltext(VNController game, object index)
        {
            if (game == null)
            {
                throw new Exception(String.Format("Unexpected ltext() call (index = %d) when game is None. Do not use this function in other function's default parameter!", index));
            }
            if (!hasattr(game.scenedata, "strings"))
            {
                register_string_resource(game);
            }
            if (game.scenedata.strings.Keys.Contains(index))
            {
                var orgText = game.scenedata.strings[index];
                var tgtText = orgText;
                var tt = orgText.split("#");
                //print "ltext", tt
                foreach (var i in Enumerable.Range(0, Convert.ToInt32((tt.Count - 1) / 2)))
                {
                    var vword = tt[i * 2 + 1];
                    //print "vword", vword
                    if (game.registeredChars.Contains(vword))
                    {
                        var rep = game.registeredChars[vword][1];
                    }
                    else if (game.scenedata.props.Keys.Contains(vword))
                    {
                        rep = game.scenedata.props[vword].name;
                    }
                    else
                    {
                        try
                        {
                            var ivword = Convert.ToInt32(vword);
                            if (game.scenedata.strings.Keys.Contains(ivword))
                            {
                                rep = game.scenedata.strings[ivword];
                            }
                            else
                            {
                                rep = null;
                            }
                        }
                        catch
                        {
                            rep = null;
                        }
                    }
                    //print "rep", rep
                    if (rep != null)
                    {
                        tgtText = tgtText.replace("#" + vword + "#", rep, 1);
                    }
                }
                return tgtText;
            }
            else
            {
                return "Undefined text id = " + index.ToString();
            }
        }

        public static object debug_game_texts_next(VNController game, object startfrom, object nexttexts, object endfunc)
        {
            object actScript;
            // collapse all the script before startfrom, and then start at startfrom
            if (startfrom >= nexttexts.Count)
            {
                Console.WriteLine("debug_game_texts_next: startfrom out of range! Jump to end!");
                endfunc(game);
                reutrn;
            }
            // collapse each skipped step into dict and collect them
            var toColList = new List<object>();
            foreach (var i in Enumerable.Range(0, startfrom))
            {
                if (nexttexts[i].Count != 4)
                {
                    continue;
                }
                if (nexttexts[i][3] is tuple)
                {
                    actScript = scriptCollapser(nexttexts[i][3]);
                }
                else if (nexttexts[i][3] is dict)
                {
                    actScript = nexttexts[i][3];
                }
                else
                {
                    Console.WriteLine("Unknown param type: " + type(nexttexts[i][3]).ToString());
                }
                toColList.append(actScript);
            }
            // collapse skipped steps and act it
            actScript = scriptCollapser(toColList);
            if (actScript.Count > 0)
            {
                Console.WriteLine(String.Format("All step before step %d was collapsed into", startfrom));
                Console.WriteLine(actScript);
                act(game, actScript);
            }
            // prepare a new script list
            var newScriptList = new List<object>();
            foreach (var i in Enumerable.Range(startfrom, nexttexts.Count - startfrom))
            {
                newScriptList.append(nexttexts[i]);
            }
            // run new 
            game.texts_next(newScriptList, endfunc);
        }
    }
}
