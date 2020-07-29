using System;
using System.Collections.Generic;
using System.Text;

namespace VNEngine
{

        public class ScriptHelper
        {

            public Tuple<string, string, string, string, string, string, string> _animeStyleTexts;

            public int _normalHeight;

            public int _normalWidth;

            public object _previewBackupCurCharText;

            public object _previewBackupEndNextTextFunc;

            public object _previewBackupNextTexts;

            public object _previewBackupVnText;

            public object _previewFromIndex;

            public object _previewStartCam;

            public object _previewStartScn;

            public int _shrinkHeight;

            public int _shrinkWidth;

            public List<ScriptClip> animeBuffer;

            public int animeBufferIndex;

            public int animeTime;

            public bool asEnable;

            public AutoScriptInfo asInfo;

            public bool asTemplate;

            public string baseNest;

            public object ch_base_actor;

            public object ch_base_pattern;

            public object ch_base_speed;

            public object ch_category_texts;

            public object ch_ct_sclpos;

            public List<object> ch_ext_actor;

            public object ch_gp_sclpos;

            public object ch_group_texts;

            public object ch_no_sclpos;

            public object ch_no_texts;

            public void ch_partner_actor;

            public object ch_partner_actor_sex;

            public object ch_sel_category;

            public object ch_sel_group;

            public int ch_sel_no;

            public bool ch_set_ext_actor;

            public bool createLocalizeStringOnBuild;

            public ScriptClipInfo curSCInfo;

            public bool dumpClipToFile;

            public object game;

            public int guiHeight;

            public bool guiOnShow;

            public int guiScreenIndex;

            public int guiWidth;

            public Dictionary<object, object> lhModifiedList;

            public object lhOrgInput;

            public string lhOrgTextType;

            public string lhPrevVnText;

            public string lhTgtID;

            public string lhTgtInput;

            public bool masterMode;

            public string nestWord;

            public int orgWHeight;

            public void orgWindowCallback;

            public int orgWWidth;

            public object prevCamStatus;

            public object prevScnStatus;

            public string pythonContent;

            public int sdIndex;

            public List<object> sdModifiedList;

            public string sdSearchKeyword;

            public List<object> sdSearchResult;

            public bool slowMotion;

            public int slowMotionRate;

            public ScriptHelper(object tgtGame)
            {
                // const
                this._normalWidth = 500;
                this._normalHeight = 265;
                this._shrinkWidth = 130;
                this._shrinkHeight = 55;
                this._animeStyleTexts = ("linear", "slow-fast", "fast-slow", "slow-fast3", "fast-slow3", "slow-fast4", "fast-slow4");
                // basic setting
                init_scene_anime(tgtGame);
                this.game = tgtGame;
                this.orgWWidth = 0;
                this.orgWHeight = 0;
                this.orgWindowCallback = null;
                this.guiOnShow = false;
                this.guiWidth = this._normalWidth;
                this.guiHeight = this._normalHeight;
                this.guiScreenIndex = 0;
                // utility setting
                this.baseNest = "        ";
                this.nestWord = "    ";
                this.masterMode = false;
                this.slowMotionRate = 10;
                this.createLocalizeStringOnBuild = true;
                // script builder setting
                this.curSCInfo = new ScriptClipInfo();
                // anime buffer and reference setting
                this.init_anime_buffer(false);
                // runtime
                this.dumpClipToFile = false;
                this.slowMotion = false;
                this.pythonContent = "";
                this.asTemplate = false;
                this.asEnable = false;
                this.asInfo = new AutoScriptInfo();
                // some game attribute used by script helper
                if (!hasattr(tgtGame, "endNextTextFunc"))
                {
                    tgtGame.endNextTextFunc = null;
                }
                if (!hasattr(tgtGame, "scenePNG"))
                {
                    tgtGame.scenePNG = "";
                }
            }

            public virtual void init_anime_buffer(bool rescan = true)
            {
                // rescan
                if (rescan || !hasattr(this.game.scenedata, "actors") || !hasattr(this.game.scenedata, "props"))
                {
                    register_actor_prop_by_tag(this.game);
                }
                if (rescan || !hasattr(this.game.scenedata, "strings"))
                {
                    register_string_resource(this.game);
                }
                // reset prev status
                this.prevScnStatus = ScriptHelper.get_scn_status(this.game);
                this.prevCamStatus = ScriptHelper.get_cam_status();
                // reset anime buffer
                var initSC = new ScriptClip();
                initSC.finalScnStatus = this.prevScnStatus;
                initSC.finalCamStatus = this.prevCamStatus;
                initSC.info = new ScriptClipInfo();
                this.animeBuffer = new List<ScriptClip> {
                initSC
            };
                this.animeBufferIndex = 0;
                this.animeTime = 0;
            }

            public virtual ScriptClip build_script_clip(void info = null, int refIndex = -1, int tgtIndex = -1)
            {
                object output;
                object dlgString;
                object pTo;
                object pFrom;
                object scnScript;
                object curCamStatus;
                object curScnStatus;
                object preCamStatus;
                object preScnStatus;
                // build script from refIndex clip to tgtIndex clip
                // if info set to None, use self.curSCInfo
                // if refIndex set to -1, diff from setted previous status
                // if tgtIndex set to -1, diff to current scene status
                if (info == null)
                {
                    info = this.curSCInfo;
                }
                if (refIndex == -1)
                {
                    preScnStatus = this.prevScnStatus;
                    preCamStatus = this.prevCamStatus;
                }
                else
                {
                    preScnStatus = this.animeBuffer[refIndex].finalScnStatus;
                    preCamStatus = this.animeBuffer[refIndex].finalCamStatus;
                }
                if (tgtIndex == -1)
                {
                    curScnStatus = ScriptHelper.get_scn_status(this.game);
                    if (info.includeCamera)
                    {
                        curCamStatus = ScriptHelper.get_cam_status();
                    }
                    else
                    {
                        curCamStatus = this.prevCamStatus;
                    }
                }
                else
                {
                    curScnStatus = this.animeBuffer[tgtIndex].finalScnStatus;
                    curCamStatus = this.animeBuffer[tgtIndex].finalCamStatus;
                }
                // prepare scene contents
                if (info.dumpTypeIndex == 0)
                {
                    // dump diff
                    scnScript = ScriptHelper.diffSceneWithPrev(curScnStatus, preScnStatus);
                }
                else
                {
                    // dump full
                    scnScript = curScnStatus;
                }
                // filter target ids
                if (info.dumpTgts != null)
                {
                    foreach (var tgt in scnScript.keys())
                    {
                        if (info.dumpTgts.count(tgt) == 0)
                        {
                            scnScript.pop(tgt);
                        }
                    }
                }
                // seperate by anime and non-anime
                var dmpAnimeScript = new Dictionary<object, object>
                {
                };
                var dmpNonAnimeScript = new Dictionary<object, object>
                {
                };
                var dmpTailScript = new Dictionary<object, object>
                {
                };
                if (info.dumpAsIndex == 0 || info.animeDuration == 0)
                {
                    // no scene anime
                    dmpNonAnimeScript.update(scnScript);
                }
                else
                {
                    // check every tgt(actor/prop) for every action, anime or non-anime
                    foreach (var tgt in scnScript.Keys)
                    {
                        if (this.game.scenedata.actors.Contains(tgt))
                        {
                            foreach (var func in scnScript[tgt].Keys)
                            {
                                if (char_act_funcs.Contains(func))
                                {
                                    if (char_act_funcs[func][1])
                                    {
                                        if (!dmpAnimeScript.Contains(tgt))
                                        {
                                            dmpAnimeScript[tgt] = new Dictionary<object, object>
                                            {
                                            };
                                        }
                                        dmpAnimeScript[tgt][func] = scnScript[tgt][func];
                                    }
                                    else
                                    {
                                        if (!dmpNonAnimeScript.Contains(tgt))
                                        {
                                            dmpNonAnimeScript[tgt] = new Dictionary<object, object>
                                            {
                                            };
                                        }
                                        dmpNonAnimeScript[tgt][func] = scnScript[tgt][func];
                                    }
                                }
                                else
                                {
                                    Console.WriteLine(String.Format("Unexpected actor function '%s'", func));
                                }
                            }
                        }
                        else if (this.game.scenedata.props.Contains(tgt))
                        {
                            foreach (var func in scnScript[tgt].Keys)
                            {
                                if (prop_act_funcs.Contains(func))
                                {
                                    if (prop_act_funcs[func][1])
                                    {
                                        if (!dmpAnimeScript.Contains(tgt))
                                        {
                                            dmpAnimeScript[tgt] = new Dictionary<object, object>
                                            {
                                            };
                                        }
                                        dmpAnimeScript[tgt][func] = scnScript[tgt][func];
                                    }
                                    else
                                    {
                                        if (!dmpNonAnimeScript.Contains(tgt))
                                        {
                                            dmpNonAnimeScript[tgt] = new Dictionary<object, object>
                                            {
                                            };
                                        }
                                        dmpNonAnimeScript[tgt][func] = scnScript[tgt][func];
                                    }
                                }
                                else
                                {
                                    Console.WriteLine(String.Format("Unexpected prop function '%s'", func));
                                }
                            }
                        }
                        else if (this.game.gdata.kfaManagedClips.Contains(tgt))
                        {
                            foreach (var func in scnScript[tgt].Keys)
                            {
                                if (clip_act_funcs.Contains(func))
                                {
                                    if (clip_act_funcs[func][1])
                                    {
                                        if (!dmpAnimeScript.Contains(tgt))
                                        {
                                            dmpAnimeScript[tgt] = new Dictionary<object, object>
                                            {
                                            };
                                        }
                                        dmpAnimeScript[tgt][func] = scnScript[tgt][func];
                                    }
                                    else
                                    {
                                        if (!dmpNonAnimeScript.Contains(tgt))
                                        {
                                            dmpNonAnimeScript[tgt] = new Dictionary<object, object>
                                            {
                                            };
                                        }
                                        dmpNonAnimeScript[tgt][func] = scnScript[tgt][func];
                                    }
                                }
                                else
                                {
                                    Console.WriteLine(String.Format("Unexpected clip function '%s'", func));
                                }
                            }
                        }
                        else if (tgt == "sys")
                        {
                            foreach (var func in scnScript[tgt].Keys)
                            {
                                if (sys_act_funcs.Contains(func))
                                {
                                    if (sys_act_funcs[func][1])
                                    {
                                        if (!dmpAnimeScript.Contains(tgt))
                                        {
                                            dmpAnimeScript[tgt] = new Dictionary<object, object>
                                            {
                                            };
                                        }
                                        dmpAnimeScript[tgt][func] = scnScript[tgt][func];
                                    }
                                    else
                                    {
                                        if (!dmpNonAnimeScript.Contains(tgt))
                                        {
                                            dmpNonAnimeScript[tgt] = new Dictionary<object, object>
                                            {
                                            };
                                        }
                                        dmpNonAnimeScript[tgt][func] = scnScript[tgt][func];
                                    }
                                }
                                else
                                {
                                    Console.WriteLine(String.Format("Unexpected sys function '%s'", func));
                                }
                            }
                        }
                        else
                        {
                            Console.WriteLine(String.Format("Unexpected alias '%s'", tgt));
                        }
                    }
                }
                // change anime script param to range param
                foreach (var tgt in dmpAnimeScript.Keys)
                {
                    foreach (var func in dmpAnimeScript[tgt].Keys)
                    {
                        pFrom = preScnStatus[tgt][func];
                        pTo = dmpAnimeScript[tgt][func];
                        dmpAnimeScript[tgt][func] = (pFrom, pTo);
                    }
                }
                // prepare camera contents
                if (info.includeCamera)
                {
                    // dump camera, whatever changed or not
                    var camScript = new Dictionary<object, object>
                    {
                    };
                    camScript["cam"] = new Dictionary<object, object>
                    {
                    };
                    camScript["cam"]["goto_pos"] = curCamStatus["cam"]["goto_pos"];
                    if (info.animateCamera && info.useCameraTimer && info.cameraDuration != 0)
                    {
                        // use camera timer to animate camera, add camera anime setting into camScript
                        var pList = camScript["cam"]["goto_pos"].ToList();
                        pList.append(info.cameraDuration);
                        pList.append(this._animeStyleTexts[info.cameraStyle]);
                        camScript["cam"]["goto_pos"] = tuple(pList);
                        // merge to non-anime script
                        dmpNonAnimeScript.update(camScript);
                    }
                    else if (info.animateCamera && !info.useCameraTimer && info.animeDuration != 0)
                    {
                        // use scene timer to animate camera, add camera range param into camScript
                        pFrom = preCamStatus["cam"]["goto_pos"];
                        pTo = camScript["cam"]["goto_pos"];
                        camScript["cam"]["goto_pos"] = (pFrom, pTo);
                        // merge to anime script
                        dmpAnimeScript.update(camScript);
                    }
                    else
                    {
                        // merge to non-anime script
                        dmpNonAnimeScript.update(camScript);
                    }
                }
                // create localize string if needed
                if (this.dumpClipToFile && info.dialogue.Count > 0 && this.createLocalizeStringOnBuild)
                {
                    this.sd_init();
                    var newStrId = this.sd_find_or_create(info.dialogue);
                    dlgString = String.Format("ltext(game, %d)", newStrId);
                }
                else
                {
                    dlgString = "\"" + info.dialogue + "\"";
                }
                // prepare output string
                if (info.dumpAsIndex == 0)
                {
                    // as new act
                    if (dmpNonAnimeScript.Count == 0)
                    {
                        output = this.baseNest + String.Format("[\"%s\", %s],\n", info.speakerAlias, dlgString);
                    }
                    else
                    {
                        output = this.baseNest + String.Format("[\"%s\", %s, act, {\n", info.speakerAlias, dlgString);
                        foreach (var tgt in dmpNonAnimeScript.keys().OrderBy(_p_1 => _p_1).ToList())
                        {
                            output += this.baseNest + this.nestWord + "'" + tgt + "': " + script2string(dmpNonAnimeScript[tgt]) + ",\n";
                        }
                        output += this.baseNest + "}],\n";
                    }
                }
                else if (info.dumpAsIndex == 1)
                {
                    // as new anime
                    if (dmpAnimeScript.Count == 0 && info.animeDuration > 0)
                    {
                        // add a sys.idle function to anime script if anime duration is setted but no animeable function
                        dmpAnimeScript["sys"] = new Dictionary<object, object>
                        {
                        };
                        dmpAnimeScript["sys"]["idle"] = (0, 0);
                    }
                    if (dmpAnimeScript.Count > 0)
                    {
                        if (info.hideWindowInAnime)
                        {
                            // add a sys.visible=0 in non-anime script, and sys.visible=1 in tail
                            if (!dmpNonAnimeScript.Keys.Contains("sys"))
                            {
                                dmpNonAnimeScript["sys"] = new Dictionary<object, object>
                                {
                                };
                            }
                            dmpNonAnimeScript["sys"]["visible"] = 0;
                            dmpTailScript["sys"] = new Dictionary<object, object>
                            {
                            };
                            dmpTailScript["sys"]["visible"] = 1;
                        }
                        else if (info.hideButtonInAnime)
                        {
                            // add a sys.lock=1 in non-anime script, and sys.lock=0 in tail
                            if (!dmpNonAnimeScript.Keys.Contains("sys"))
                            {
                                dmpNonAnimeScript["sys"] = new Dictionary<object, object>
                                {
                                };
                            }
                            dmpNonAnimeScript["sys"]["lock"] = 1;
                            dmpTailScript["sys"] = new Dictionary<object, object>
                            {
                            };
                            dmpTailScript["sys"]["lock"] = 0;
                        }
                    }
                    if (dmpNonAnimeScript.Count + dmpAnimeScript.Count == 0)
                    {
                        output = this.baseNest + String.Format("[\"%s\", %s],\n", info.speakerAlias, dlgString);
                    }
                    else
                    {
                        output = this.baseNest + String.Format("[\"%s\", %s, anime, (\n", info.speakerAlias, dlgString);
                        if (dmpNonAnimeScript.Count > 0)
                        {
                            output += this.baseNest + this.nestWord + "({\n";
                            foreach (var tgt in dmpNonAnimeScript.keys().OrderBy(_p_2 => _p_2).ToList())
                            {
                                output += this.baseNest + this.nestWord + this.nestWord + "'" + tgt + "': " + script2string(dmpNonAnimeScript[tgt]) + ",\n";
                            }
                            output += this.baseNest + this.nestWord + "}),\n";
                        }
                        if (dmpAnimeScript.Count > 0)
                        {
                            output += this.baseNest + this.nestWord + "({\n";
                            foreach (var tgt in dmpAnimeScript.keys().OrderBy(_p_3 => _p_3).ToList())
                            {
                                output += this.baseNest + this.nestWord + this.nestWord + "'" + tgt + "': " + script2string(dmpAnimeScript[tgt]) + ",\n";
                            }
                            output += this.baseNest + this.nestWord + String.Format("}, %.2f, \'%s\'),\n", info.animeDuration, this._animeStyleTexts[info.animeStyle]);
                        }
                        if (dmpTailScript.Count > 0)
                        {
                            output += this.baseNest + this.nestWord + "({\n";
                            foreach (var tgt in dmpTailScript.keys().OrderBy(_p_4 => _p_4).ToList())
                            {
                                output += this.baseNest + this.nestWord + this.nestWord + "'" + tgt + "': " + script2string(dmpTailScript[tgt]) + ",\n";
                            }
                            output += this.baseNest + this.nestWord + "}),\n";
                        }
                        output += this.baseNest + ")],\n";
                    }
                }
                else
                {
                    // as sub anime
                    if (info.dialogue.Count > 0 && this.dumpClipToFile)
                    {
                        // add a sys.text function to non-anime script if dialogue is setted, not necessary if not need to output to file
                        if (!dmpNonAnimeScript.Contains("sys"))
                        {
                            dmpNonAnimeScript["sys"] = new Dictionary<object, object>
                            {
                            };
                        }
                        dmpNonAnimeScript["sys"]["text"] = (info.speakerAlias, dlgString);
                    }
                    if (dmpAnimeScript.Count == 0 && info.animeDuration > 0)
                    {
                        // add a sys.idle function to anime script if anime duration is setted but no animeable function
                        dmpAnimeScript["sys"] = new Dictionary<object, object>
                        {
                        };
                        dmpAnimeScript["sys"]["idle"] = (0, 0);
                    }
                    if (dmpAnimeScript.Count > 0)
                    {
                        if (info.hideWindowInAnime)
                        {
                            // add a sys.visible=0 in non-anime script, and sys.visible=1 in tail
                            if (!dmpNonAnimeScript.Keys.Contains("sys"))
                            {
                                dmpNonAnimeScript["sys"] = new Dictionary<object, object>
                                {
                                };
                            }
                            dmpNonAnimeScript["sys"]["visible"] = 0;
                            dmpTailScript["sys"] = new Dictionary<object, object>
                            {
                            };
                            dmpTailScript["sys"]["visible"] = 1;
                        }
                        else if (info.hideButtonInAnime)
                        {
                            // add a sys.lock=1 in non-anime script, and sys.lock=0 in tail
                            if (!dmpNonAnimeScript.Keys.Contains("sys"))
                            {
                                dmpNonAnimeScript["sys"] = new Dictionary<object, object>
                                {
                                };
                            }
                            dmpNonAnimeScript["sys"]["lock"] = 1;
                            dmpTailScript["sys"] = new Dictionary<object, object>
                            {
                            };
                            dmpTailScript["sys"]["lock"] = 0;
                        }
                    }
                    if (dmpNonAnimeScript.Count + dmpAnimeScript.Count == 0)
                    {
                        Console.WriteLine("Nothing to dump...");
                        return;
                    }
                    else
                    {
                        output = "";
                        if (dmpNonAnimeScript.Count > 0)
                        {
                            output += this.baseNest + this.nestWord + "({\n";
                            foreach (var tgt in dmpNonAnimeScript.keys().OrderBy(_p_5 => _p_5).ToList())
                            {
                                output += this.baseNest + this.nestWord + this.nestWord + "'" + tgt + "': " + script2string(dmpNonAnimeScript[tgt]) + ",\n";
                            }
                            output += this.baseNest + this.nestWord + "}),\n";
                        }
                        if (dmpAnimeScript.Count > 0)
                        {
                            output += this.baseNest + this.nestWord + "({\n";
                            foreach (var tgt in dmpAnimeScript.keys().OrderBy(_p_6 => _p_6).ToList())
                            {
                                output += this.baseNest + this.nestWord + this.nestWord + "'" + tgt + "': " + script2string(dmpAnimeScript[tgt]) + ",\n";
                            }
                            output += this.baseNest + this.nestWord + String.Format("}, %.2f, \'%s\'),\n", info.animeDuration, this._animeStyleTexts[info.animeStyle]);
                        }
                        if (dmpTailScript.Count > 0)
                        {
                            output += this.baseNest + this.nestWord + "({\n";
                            foreach (var tgt in dmpTailScript.keys().OrderBy(_p_7 => _p_7).ToList())
                            {
                                output += this.baseNest + this.nestWord + this.nestWord + "'" + tgt + "': " + script2string(dmpTailScript[tgt]) + ",\n";
                            }
                            output += this.baseNest + this.nestWord + "}),\n";
                        }
                    }
                }
                // save current to previous
                this.set_ref_scene(curScnStatus, curCamStatus);
                // output dump to file
                if (this.dumpClipToFile)
                {
                    try
                    {
                        var f = codecs.open("dumppython.txt", "a+", "utf-8");
                        f.write(output);
                        f.close();
                    }
                    catch (Exception)
                    {
                        Console.WriteLine(e);
                    }
                }
                // VNSceneScript patch
                var sshelper = vngameengine.import_or_reload("vnframe_vnscenescripthelper");
                var error = sshelper.dumpclip_toscenescript(this, output);
                if (error == "")
                {
                    // all is ok
                }
                else
                {
                    // some error show
                }
                // VNSceneScript patch end
                // build script clip
                var newSC = new ScriptClip();
                newSC.finalScnStatus = curScnStatus;
                newSC.finalCamStatus = curCamStatus;
                newSC.nonAnimeScript = dmpNonAnimeScript;
                newSC.animeScript = dmpAnimeScript;
                newSC.tailScript = dmpTailScript;
                newSC.info = copy.copy(info);
                return newSC;
            }

            // script builder and anime buffer
            public virtual void append_clip()
            {
                var newSC = this.build_script_clip();
                this.animeBuffer.append(newSC);
                this.animeBufferIndex = this.animeBuffer.Count - 1;
                this.clear_dialogue();
            }

            public virtual void update_clip(bool useOldInfo = false)
            {
                object updInfo;
                if (this.animeBufferIndex == 0)
                {
                    throw new Exception("Invalide operation: try to update init scene!");
                }
                // update current clip
                if (useOldInfo)
                {
                    updInfo = this.animeBuffer[this.animeBufferIndex].info;
                }
                else
                {
                    updInfo = this.curSCInfo;
                }
                var updSC = this.build_script_clip(updInfo, this.animeBufferIndex - 1);
                this.animeBuffer[this.animeBufferIndex] = updSC;
                // update next clip if exist
                if (this.animeBufferIndex < this.animeBuffer.Count - 1)
                {
                    var updNextInfo = this.animeBuffer[this.animeBufferIndex + 1].info;
                    var updNextSc = this.build_script_clip(updNextInfo, this.animeBufferIndex, this.animeBufferIndex + 1);
                    this.animeBuffer[this.animeBufferIndex + 1] = updNextSc;
                }
            }

            public virtual void insert_clip(bool useOldInfo = false)
            {
                object newInfo;
                if (this.animeBufferIndex == 0)
                {
                    throw new Exception("Invalide operation: try to insert init scene!");
                }
                // check time status
                if (this.animeBuffer[this.animeBufferIndex].info.dumpAsIndex != 0)
                {
                    Console.WriteLine(String.Format("TODO: calculate insert anime progress at %.2f/%.2f", this.animeTime, this.animeBuffer[this.animeBufferIndex].info.animeDuration));
                }
                else
                {
                    Console.WriteLine("current is a actiong.");
                }
                // create a new clip
                if (useOldInfo)
                {
                    newInfo = this.animeBuffer[this.animeBufferIndex].info;
                }
                else
                {
                    newInfo = this.curSCInfo;
                }
                var newSC = this.build_script_clip(newInfo, this.animeBufferIndex - 1);
                this.animeBuffer.insert(this.animeBufferIndex, newSC);
                // update next clip
                var updNextInfo = this.animeBuffer[this.animeBufferIndex + 1].info;
                var updNextSc = this.build_script_clip(updNextInfo, this.animeBufferIndex, this.animeBufferIndex + 1);
                this.animeBuffer[this.animeBufferIndex + 1] = updNextSc;
            }

            public virtual void delete_clip(bool rollBack = false)
            {
                // if set rollBack, delete will set the the init status of current clip and delete current clip and all following clips
                if (this.animeBufferIndex == 0)
                {
                    throw new Exception("Invalide operation: try to delete init scene!");
                }
                if (rollBack || this.animeBufferIndex == this.animeBuffer.Count - 1)
                {
                    if (this.animeBufferIndex > 0)
                    {
                        this.animeBuffer = this.animeBuffer[::self.animeBufferIndex];
                        this.animeBufferIndex -= 1;
                    }
                    this.prevScnStatus = this.animeBuffer[this.animeBufferIndex].finalScnStatus;
                    this.prevCamStatus = this.animeBuffer[this.animeBufferIndex].finalCamStatus;
                }
                else
                {
                    //print "delete clip at", self.animeBufferIndex
                    var updInfo = this.animeBuffer[this.animeBufferIndex + 1].info;
                    var updSc = this.build_script_clip(updInfo, this.animeBufferIndex - 1, this.animeBufferIndex + 1);
                    this.animeBuffer.pop(this.animeBufferIndex);
                    this.animeBuffer[this.animeBufferIndex] = updSc;
                }
            }

            public virtual void set_ref_scene(void scnStatus = null, void camStatus = null)
            {
                // set default
                if (scnStatus == null)
                {
                    scnStatus = ScriptHelper.get_scn_status(this.game);
                }
                if (camStatus == null)
                {
                    camStatus = ScriptHelper.get_cam_status();
                }
                // save to prev
                this.prevScnStatus = scnStatus;
                this.prevCamStatus = camStatus;
            }

            public virtual void restore_scene_status(object tgtSceneStatus)
            {
                var curScn = ScriptHelper.get_scn_status(this.game);
                var difSpt = ScriptHelper.diffSceneWithPrev(tgtSceneStatus, curScn);
                //print "restore scene:", script2string(difSpt)
                act(this.game, difSpt);
            }

            public virtual void restore_camera_status(object tgtCameraStatus)
            {
                act(this.game, tgtCameraStatus);
            }

            public virtual void play_anime_clip(string mode)
            {
                object aniDur;
                object currSC;
                object prevSC;
                // stop cam anime timer if exists
                if (this.game.camAnimeTID != -1)
                {
                    this.game.clear_timer(this.game.camAnimeTID);
                }
                this.game.camAnimeTID = -1;
                if (this.animeBufferIndex == 0)
                {
                    prevSC = this.animeBuffer[0];
                    currSC = this.animeBuffer[0];
                }
                else
                {
                    prevSC = this.animeBuffer[this.animeBufferIndex - 1];
                    currSC = this.animeBuffer[this.animeBufferIndex];
                }
                if (mode == "tostart")
                {
                    // just restore init status = prev final status
                    this.restore_scene_status(prevSC.finalScnStatus);
                    if (currSC.info.includeCamera)
                    {
                        this.restore_camera_status(prevSC.finalCamStatus);
                    }
                    this.animeTime = 0;
                }
                else if (mode == "toend")
                {
                    // just restore final status
                    this.restore_scene_status(currSC.finalScnStatus);
                    if (currSC.info.includeCamera)
                    {
                        this.restore_camera_status(currSC.finalCamStatus);
                    }
                    this.animeTime = currSC.info.animeDuration;
                }
                else if (mode == "play" || mode == "play_and_next")
                {
                    if (this.animeBufferIndex == 0)
                    {
                        // restore init status only
                        this.restore_scene_status(prevSC.finalScnStatus);
                        this.restore_camera_status(prevSC.finalCamStatus);
                    }
                    else
                    {
                        // restore prev status
                        this.restore_scene_status(prevSC.finalScnStatus);
                        if (currSC.info.includeCamera)
                        {
                            this.restore_camera_status(prevSC.finalCamStatus);
                        }
                        // play non-anime scipte then anime scipte
                        if (currSC.nonAnimeScript.Count > 0)
                        {
                            // non-anime script can be act 
                            act(this.game, currSC.nonAnimeScript);
                        }
                        if (currSC.animeScript.Count > 0)
                        {
                            // anime script must turn to tuple before pass to anime
                            if (this.slowMotion)
                            {
                                aniDur = currSC.info.animeDuration * this.slowMotionRate;
                            }
                            else
                            {
                                aniDur = currSC.info.animeDuration;
                            }
                            var aniScript = (currSC.animeScript, aniDur, this._animeStyleTexts[currSC.info.animeStyle]);
                            aniScript = ValueTuple.Create(aniScript);
                            anime(this.game, aniScript);
                        }
                        if (currSC.tailScript.Count > 0)
                        {
                            // tail script can be act, it will be acted soon but not after anime done TODO?
                            act(this.game, currSC.tailScript);
                        }
                    }
                    if (mode == "play_and_next" && this.animeBufferIndex < this.animeBuffer.Count - 1)
                    {
                        this.animeBufferIndex += 1;
                    }
                }
                else if (mode == "stop")
                {
                    if (this.game.scnAnimeTID != -1)
                    {
                        this.game.clear_timer(this.game.scnAnimeTID);
                        this.game.scnAnimeTID = -1;
                    }
                    if (this.slowMotion)
                    {
                        this.animeTime /= this.slowMotionRate;
                    }
                }
                else if (mode == "toposition")
                {
                    var asProgress = time2progress(this.animeTime, currSC.info.animeDuration, this._animeStyleTexts[currSC.info.animeStyle]);
                    //print "play anime at %.2f/%.2f, progress %.3f"%(self.animeTime, currSC.info.animeDuration, asProgress)
                    // set script
                    var actScript = new Dictionary<object, object>
                    {
                    };
                    foreach (var char in currSC.animeScript) {
                        actScript[char] = new Dictionary<object, object>
                        {
                        };
                        foreach (var f in currSC.animeScript[char])
                        {
                            var rangeParam = currSC.animeScript[char][f];
                            if (rangeParam is tuple)
                            {
                                actScript[char][f] = paramInterpolater(rangeParam[0], rangeParam[1], asProgress);
                            }
                            else
                            {
                                actScript[char][f] = rangeParam;
                            }
                        }
                    }
                    act(this.game, actScript);
                }
                else
                {
                    Console.WriteLine("Unknown play mode:", mode);
                }
            }

            public virtual object build_anime()
            {
                object preNest;
                // output anime to file
                var allAnimeScript = this.output_anime_clips();
                // format the output string
                //output = script2string(allAnimeScript)
                var output = "";
                foreach (var ntScript in allAnimeScript)
                {
                    // each next text script
                    if (ntScript.Count == 2)
                    {
                        output += this.baseNest + String.Format("[\"%s\", %s],\n", ntScript[0], ntScript[1].startswith("ltext(") ? ntScript[1] : "\"" + ntScript[1] + "\"");
                    }
                    else if (ntScript[2] == act)
                    {
                        output += this.baseNest + String.Format("[\"%s\", %s, act, {\n", ntScript[0], ntScript[1].startswith("ltext(") ? ntScript[1] : "\"" + ntScript[1] + "\"");
                        foreach (var tgt in ntScript[3].Keys)
                        {
                            output += this.baseNest + this.nestWord + "'" + tgt + "': " + script2string(ntScript[3][tgt]) + ",\n";
                        }
                        output += this.baseNest + "}],\n";
                    }
                    else if (ntScript[2] == anime)
                    {
                        output += this.baseNest + String.Format("[\"%s\", %s, anime, (\n", ntScript[0], ntScript[1].startswith("ltext(") ? ntScript[1] : "\"" + ntScript[1] + "\"");
                        foreach (var subScript in ntScript[3])
                        {
                            if (subScript.Count == 1)
                            {
                                output += this.baseNest + this.nestWord + "({\n";
                                foreach (var tgt in subScript[0].Keys)
                                {
                                    output += this.baseNest + this.nestWord + this.nestWord + "'" + tgt + "': " + script2string(subScript[0][tgt]) + ",\n";
                                }
                                output += this.baseNest + this.nestWord + "}),\n";
                            }
                            else
                            {
                                output += this.baseNest + this.nestWord + "({\n";
                                foreach (var tgt in subScript[0].Keys)
                                {
                                    output += this.baseNest + this.nestWord + this.nestWord + "'" + tgt + "': " + script2string(subScript[0][tgt]) + ",\n";
                                }
                                output += this.baseNest + this.nestWord + String.Format("}, %.2f, \'%s\'),\n", subScript[1], subScript[2]);
                            }
                        }
                        output += this.baseNest + ")],\n";
                    }
                    else
                    {
                        throw new Exception("build_anime exception: Unknown ntScript format");
                    }
                }
                // write to python or dump file
                if (this.asTemplate && this.asEnable)
                {
                    return this.as_add_seq(output);
                }
                else
                {
                    if (this.baseNest.Count >= this.nestWord.Count)
                    {
                        preNest = this.baseNest[::(len(self.baseNest) - len(self.nestWord))];
                    }
                    else
                    {
                        preNest = "";
                    }
                    output = preNest + "[\n" + output + preNest + "]\n";
                    try
                    {
                        var f = codecs.open("dumppython.txt", "a+", "utf-8");
                        f.write(output);
                        f.write("\n");
                        f.close();
                        return "All script clips in anime buffer were dumped into dumppython.txt";
                    }
                    catch (Exception)
                    {
                        return "Write to dumppython.txt failed: " + e.ToString();
                    }
                }
            }

            public virtual void preview_anime_in_game(int fromIndex = 0)
            {
                object prevSC;
                // preview anime in game, set from clip index or from start
                // prepare preview scene
                if (fromIndex == 0)
                {
                    prevSC = this.animeBuffer[0];
                }
                else
                {
                    prevSC = this.animeBuffer[fromIndex - 1];
                }
                this._previewStartScn = prevSC.finalScnStatus;
                this._previewStartCam = prevSC.finalCamStatus;
                this._previewFromIndex = fromIndex;
                // backup old scene contents
                this._previewBackupNextTexts = this.game.nextTexts;
                this._previewBackupEndNextTextFunc = this.game.endNextTextFunc;
                this._previewBackupCurCharText = this.game.curCharText;
                this._previewBackupVnText = this.game.vnText;
                // start
                this._preview_anime_start();
            }

            public virtual void _preview_anime_start()
            {
                this.restore_scene_status(this._previewStartScn);
                this.restore_camera_status(this._previewStartCam);
                var previewScript = this.output_anime_clips(this._previewFromIndex, false);
                scriptHelperGUIClose();
                this.game.texts_next(previewScript, this._preview_anime_end_choice);
            }

            public virtual object _preview_anime_end_choice(object game)
            {
                scriptHelperGUIStart(game);
                scriptHelperGUIMessage("Preview is over", (("Again", _sh._preview_anime_start), ("Back to ScriptHelper", _sh._preview_anime_end, false), ("Back to Scene", _sh._preview_anime_end, true)));
            }

            public virtual object _preview_anime_end(object toScene)
            {
                this.game.nextTexts = this._previewBackupNextTexts;
                this.game.endNextTextFunc = this._previewBackupEndNextTextFunc;
                this.game.set_text(this._previewBackupCurCharText, this._previewBackupVnText);
                if (toScene)
                {
                    scriptHelperGUIClose();
                }
            }

            public virtual List<tuple> output_anime_clips(int fromIndex = 0, bool forDump = true)
            {
                object optScript;
                object ntScript;
                object dlgString;
                // merge all buffered anime clips to one list
                var aoc = new List<tuple>();
                object lastClipType = null;
                foreach (var i in Enumerable.Range(fromIndex, this.animeBuffer.Count - fromIndex))
                {
                    var clp = this.animeBuffer[i];
                    var clpInfo = clp.info;
                    if (i == 0)
                    {
                        // TODO: dump init status 
                        continue;
                    }
                    // create localize string if needed
                    if (forDump && clpInfo.dialogue.Count > 0 && this.createLocalizeStringOnBuild)
                    {
                        this.sd_init();
                        var newStrId = this.sd_find_or_create(clpInfo.dialogue);
                        dlgString = String.Format("ltext(game, %d)", newStrId);
                    }
                    else
                    {
                        dlgString = clpInfo.dialogue;
                    }
                    // choice dump type
                    if (clpInfo.dumpAsIndex == 0)
                    {
                        // as a new act
                        var actScript = clp.nonAnimeScript;
                        if (actScript.Count > 0)
                        {
                            ntScript = new List<Func<Tuple<object, Dictionary<object, object>>>> {
                            clpInfo.speakerAlias,
                            dlgString,
                            act,
                            actScript
                        };
                            lastClipType = 0;
                        }
                        else
                        {
                            ntScript = new List<object> {
                            clpInfo.speakerAlias,
                            dlgString
                        };
                            lastClipType = null;
                        }
                        aoc.append(ntScript);
                    }
                    else if (clpInfo.dumpAsIndex == 1 || clpInfo.dumpAsIndex == 2 && lastClipType != 1)
                    {
                        // as a new anime
                        var anmScript = new List<object>();
                        if (clp.nonAnimeScript.Count > 0)
                        {
                            anmScript.append(ValueTuple.Create(clp.nonAnimeScript));
                        }
                        if (clp.animeScript.Count > 0)
                        {
                            optScript = ScriptHelper.optimizeAnimeScript(clp.animeScript);
                            anmScript.append((optScript, clpInfo.animeDuration, this._animeStyleTexts[clpInfo.animeStyle]));
                        }
                        if (clp.tailScript.Count > 0)
                        {
                            anmScript.append(ValueTuple.Create(clp.tailScript));
                        }
                        anmScript = tuple(anmScript);
                        if (anmScript.Count > 0)
                        {
                            ntScript = new List<object> {
                            clpInfo.speakerAlias,
                            dlgString,
                            anime,
                            anmScript
                        };
                            lastClipType = 1;
                        }
                        else
                        {
                            ntScript = new List<object> {
                            clpInfo.speakerAlias,
                            dlgString
                        };
                            lastClipType = 0;
                        }
                        aoc.append(ntScript);
                    }
                    else if (clpInfo.dumpAsIndex == 2 && lastClipType == 1)
                    {
                        // as a sub anime
                        anmScript = aoc[-1][3].ToList();
                        if (clpInfo.dialogue.Count > 0)
                        {
                            if (!clp.nonAnimeScript.Contains("sys"))
                            {
                                clp.nonAnimeScript["sys"] = new Dictionary<object, object>
                                {
                                };
                            }
                            clp.nonAnimeScript["sys"]["text"] = (clpInfo.speakerAlias, dlgString);
                        }
                        if (clp.nonAnimeScript.Count > 0)
                        {
                            anmScript.append(ValueTuple.Create(clp.nonAnimeScript));
                        }
                        if (clp.animeScript.Count > 0)
                        {
                            optScript = ScriptHelper.optimizeAnimeScript(clp.animeScript);
                            anmScript.append((optScript, clpInfo.animeDuration, this._animeStyleTexts[clpInfo.animeStyle]));
                        }
                        if (clp.tailScript.Count > 0)
                        {
                            anmScript.append(ValueTuple.Create(clp.tailScript));
                        }
                        anmScript = tuple(anmScript);
                        aoc[-1][3] = anmScript;
                        lastClipType = 1;
                    }
                    else
                    {
                        throw new Exception(String.Format("Unexpected dump type %d and last dump type %d", clpInfo.dumpAsIndex, lastClipType));
                    }
                }
                return aoc;
            }

            // scene helper
            public virtual void reload_scene()
            {
                var checkext = path.splitext(this.game.scenePNG);
                if (checkext[1].lower() != ".png")
                {
                    this.game.scenePNG += ".png";
                }
                var fpath = path.join(this.game.get_scene_dir(), this.game.sceneDir + this.game.scenePNG);
                if (path.isfile(fpath))
                {
                    Console.WriteLine("Try reload " + this.game.sceneDir + this.game.scenePNG);
                    load_and_init_scene(this.game, this.game.scenePNG, this.init_anime_buffer);
                }
                else
                {
                    scriptHelperGUIMessage(String.Format("File '%s' does not existed!", fpath), ValueTuple.Create("OK"));
                }
            }

            public virtual string save_scene(bool showMessage = true)
            {
                object msg;
                object bckfile;
                object dstfile;
                object srcfile;
                try
                {
                    var studio = Studio.Instance;
                    var scene_dir = this.game.get_scene_dir();
                    var oldSceneFiles = os.listdir(scene_dir);
                    studio.SaveScene();
                    var newSceneFiles = os.listdir(scene_dir);
                    var newFiles = new List<object>();
                    foreach (var sf in newSceneFiles)
                    {
                        if (!oldSceneFiles.Contains(sf))
                        {
                            //print "new save scene:", sf
                            newFiles.append(sf);
                        }
                    }
                    if (newFiles.Count == 0)
                    {
                        throw new Exception("Can not found new saved file...");
                    }
                    var checkext = os.path.splitext(this.game.scenePNG);
                    if (checkext[1].lower() != ".png")
                    {
                        this.game.scenePNG += ".png";
                    }
                    object pngFile = null;
                    var nonpngs = new List<object>();
                    foreach (var newFile in newFiles)
                    {
                        if (os.path.splitext(newFile)[1].lower() != ".png")
                        {
                            nonpngs.append(newFile);
                            continue;
                        }
                        else
                        {
                            pngFile = newFile;
                        }
                        srcfile = os.path.join(scene_dir, newFile);
                        dstfile = os.path.join(scene_dir, this.game.sceneDir, this.game.scenePNG);
                        if (os.path.isfile(dstfile))
                        {
                            bckfile = dstfile + ".bak";
                            shutil.move(dstfile, bckfile);
                        }
                        shutil.move(srcfile, dstfile);
                        // save ext data
                        try
                        {
                            if (this.game.isStudioNEO)
                            {
                                var HSSNES = HSStudioNEOExtSave();
                                HSSNES.SaveExtData(dstfile);
                            }
                        }
                        catch (Exception)
                        {
                            // ext data may be not necessary, continue on error
                            traceback.print_exc();
                            Console.WriteLine("[NOT IMPORTANT] Unable to save ext data");
                        }
                        msg = String.Format("Scene file '%s' saved!", os.path.join(this.game.sceneDir, this.game.scenePNG));
                    }
                    if (pngFile == null)
                    {
                        throw new Exception("Can not found new saved PNG file...");
                    }
                    else
                    {
                        var oldPngNameNoExt = os.path.splitext(pngFile)[0];
                        var newPngNameNoExt = os.path.splitext(this.game.scenePNG)[0];
                    }
                    foreach (var nonpng in nonpngs)
                    {
                        if (nonpng.startswith(oldPngNameNoExt))
                        {
                            srcfile = os.path.join(scene_dir, nonpng);
                            dstfile = os.path.join(scene_dir, this.game.sceneDir, nonpng.replace(oldPngNameNoExt, newPngNameNoExt));
                            Console.WriteLine("save non-png file:", srcfile, "->", dstfile);
                            if (os.path.isfile(dstfile))
                            {
                                bckfile = dstfile + ".bak";
                                shutil.move(dstfile, bckfile);
                            }
                            shutil.move(srcfile, dstfile);
                        }
                        else
                        {
                            msg += "\nUnknown Non-PNG file detected: " + nonpng;
                        }
                    }
                }
                catch (Exception)
                {
                    msg = "Save Scene Failed: " + e.ToString();
                    if (!showMessage)
                    {
                        throw new Exception(msg);
                    }
                }
                if (showMessage)
                {
                    scriptHelperGUIMessage(msg);
                }
                return msg;
            }

            public virtual object load_python(string filename = "")
            {
                try
                {
                    if (filename == "")
                    {
                        filename = this.game.current_game;
                    }
                    var pyPathname = path.join(this.game.pygamepath, filename);
                    if (!pyPathname.lower().endswith(".py"))
                    {
                        pyPathname += ".py";
                    }
                    //self.pythonContent = self.game.file_get_content(pyPathname)
                    if (path.exists(pyPathname))
                    {
                        var fp = codecs.open(pyPathname, "r", "utf-8");
                        this.pythonContent = fp.read();
                        fp.close();
                    }
                    else
                    {
                        throw new Exception(String.Format("file <%s> not found.", pyPathname));
                    }
                    if (this.pythonContent.strip().Count == 0)
                    {
                        this.asTemplate = false;
                        throw new Exception("Empty python content");
                    }
                    else
                    {
                        this.asTemplate = this.pythonContent.find("#-VNFA:BuildFromAutoScriptTemplate-#") > 0;
                        Console.WriteLine(String.Format("load python: %s (%dbytes), Template = %s", pyPathname, this.pythonContent.Count, this.asTemplate.ToString()));
                    }
                }
                catch (Exception)
                {
                    this.pythonContent == "";
                    Console.WriteLine("load python failed:", e);
                }
            }

            public virtual string save_python(bool showMessage = true)
            {
                object msg;
                object fp = null;
                try
                {
                    var dstfile = path.join(this.game.pygamepath, this.game.current_game + ".py");
                    if (path.isfile(dstfile))
                    {
                        var bckfile = dstfile + ".bak";
                        shutil.move(dstfile, bckfile);
                    }
                    fp = codecs.open(dstfile, "w", "utf-8");
                    fp.write(this.pythonContent);
                    fp.close();
                    msg = String.Format("Python file '%s' saved!", this.game.current_game + ".py");
                }
                catch (Exception)
                {
                    if (fp != null)
                    {
                        fp.close();
                    }
                    msg = "Save Python Failed: " + e.ToString();
                    if (!showMessage)
                    {
                        throw new Exception(msg);
                    }
                }
                if (showMessage)
                {
                    scriptHelperGUIMessage(msg);
                }
                return msg;
            }

            public virtual object tag_select()
            {
                object sel;
                try
                {
                    sel = HSNeoOCI.create_from_selected();
                }
                catch
                {
                    sel = null;
                }
                if (sel != null)
                {
                    foreach (var akey in this.game.scenedata.actors.Keys)
                    {
                        if (this.game.scenedata.actors[akey].objctrl == sel.objctrl)
                        {
                            scriptHelperGUIMessage(String.Format("Selected character is already tagged as '%s'.", akey));
                            return;
                        }
                    }
                    foreach (var pkey in this.game.scenedata.props.Keys)
                    {
                        if (this.game.scenedata.props[pkey].objctrl == sel.objctrl)
                        {
                            scriptHelperGUIMessage(String.Format("Selected item/folder is already tagged as '%s'.", pkey));
                            return;
                        }
                    }
                }
                if (sel is HSNeoOCIProp)
                {
                    scriptHelperGUIMessage("Tag '" + sel.text_name + "' as a prop:", (("Tag It", _sh._tag_select_do, sel), "Cancel"), new Dictionary<object, object> {
                    {
                        "ID",
                        new List<object> {
                            "",
                            "txt",
                            60,
                            60
                        }}});
                }
                else if (sel is HSNeoOCIChar)
                {
                    scriptHelperGUIMessage("Tag '" + sel.text_name + "' as an actor:", (("Tag She/Him", _sh._tag_select_do, sel), "Cancel"), new Dictionary<object, object> {
                    {
                        "ID",
                        new List<object> {
                            "",
                            "txt",
                            60,
                            60
                        }},
                    {
                        "Color",
                        new List<object> {
                            "ffffff",
                            "txt",
                            60,
                            60,
                            "*Color of title, RRGGBB in Hex"
                        }},
                    {
                        "Title",
                        new List<object> {
                            "",
                            "txt",
                            60,
                            60,
                            "*Omit it to use char's own name"
                        }}});
                }
                else
                {
                    scriptHelperGUIMessage("Nothing selected or unknown object.\nSelect a character or an item/folder/route to add tag to.");
                }
            }

            public virtual object _tag_select_do(object param)
            {
                object tagFolder;
                object tagText;
                try
                {
                    var tagId = this.msgParam["ID"][0].strip();
                    this._tag_check_id(tagId);
                    if (param is HSNeoOCIChar)
                    {
                        tagText = "-actor:" + tagId + ":" + this.msgParam["Color"][0];
                        if (this.msgParam["Title"][0].strip() != "")
                        {
                            tagText += ":" + this.msgParam["Title"][0];
                        }
                        tagFolder = HSNeoOCIFolder.add(tagText);
                        tagFolder.set_pos((param.pos.x, param.pos.y, param.pos.z));
                        tagFolder.set_parent_treenodeobject(param.treeNodeObject.child[0].child[0]);
                    }
                    else if (param is HSNeoOCILight)
                    {
                        tagText = "-propchild:" + tagId;
                        tagFolder = HSNeoOCIFolder.add(tagText);
                        tagFolder.set_pos((param.pos.x, param.pos.y, param.pos.z));
                        param.set_parent(tagFolder);
                    }
                    else if (param is HSNeoOCIRoute)
                    {
                        tagText = "-propgrandpa:" + tagId;
                        tagFolder = HSNeoOCIFolder.add(tagText);
                        tagFolder.set_pos((param.pos.x, param.pos.y, param.pos.z));
                        tagFolder.set_parent_treenodeobject(param.treeNodeObject.child[0]);
                    }
                    else
                    {
                        tagText = "-prop:" + tagId;
                        tagFolder = HSNeoOCIFolder.add(tagText);
                        tagFolder.set_pos((param.pos.x, param.pos.y, param.pos.z));
                        tagFolder.set_parent(param);
                    }
                    register_actor_prop_by_tag(this.game);
                }
                catch (Exception)
                {
                    scriptHelperGUIMessage(String.Format("Fail to create TAG for %s: %s", param.text_name, e.ToString()), this.masterMode ? 3 : ValueTuple.Create("OK"));
                }
            }

            public virtual object _tag_check_id(object tagId)
            {
                if (tagId.Count == 0)
                {
                    throw new Exception("Null ID");
                }
                if (tagId.isunicode())
                {
                    throw new Exception("ID can not use UNICODE text");
                }
                if (this.game.scenedata.actors.Keys.Contains(tagId))
                {
                    throw new Exception(String.Format("ID '%s' is already used by an actor id", tagId));
                }
                if (this.game.scenedata.props.Keys.Contains(tagId))
                {
                    throw new Exception(String.Format("ID '%s' is already used by a prop id", tagId));
                }
                if (this.game.gdata.kfaManagedClips.Keys.Contains(tagId))
                {
                    throw new Exception(String.Format("ID '%s' is already used by a clip name", tagId));
                }
                if (tagId == "sys")
                {
                    throw new Exception("ID 'sys' is reserved for system");
                }
                if (tagId == "cam")
                {
                    throw new Exception("ID 'cam' is reserved for camera");
                }
            }

            public virtual void sd_init()
            {
                if (!hasattr(this.game.scenedata, "strings"))
                {
                    register_string_resource(this.game);
                }
                if (hasattr(this, "sdSearchKeyword"))
                {
                    return;
                }
                this.sdSearchKeyword = "";
                this.sd_search();
                this.sdModifiedList = new List<object>();
            }

            public virtual object sd_search()
            {
                this.sdSearchResult = new List<object>();
                try
                {
                    var searchAsKey = Convert.ToInt32(this.sdSearchKeyword);
                    if (this.game.scenedata.strings.Keys.Contains(searchAsKey))
                    {
                        this.sdSearchResult.append(searchAsKey);
                    }
                }
                catch
                {
                }
                foreach (var key in this.game.scenedata.strings)
                {
                    if (this.game.scenedata.strings[key].find(this.sdSearchKeyword) != -1 && !this.sdSearchResult.Contains(key))
                    {
                        this.sdSearchResult.append(key);
                    }
                }
                if (this.sdSearchResult.Count > 0)
                {
                    this.sdIndex = 0;
                }
                else
                {
                    this.sdIndex = -1;
                }
            }

            public virtual object sd_new(object id, string text = "")
            {
                object newID;
                try
                {
                    newID = Convert.ToInt32(id);
                }
                catch
                {
                    newID = -1;
                }
                newID = this.sd_new_id(newID);
                this.game.scenedata.strings[newID] = text;
                this.sdModifiedList.append(newID);
                this.sdSearchKeyword = newID.ToString();
                this.sd_search();
                return newID;
            }

            public virtual object sd_apply(object id = -1)
            {
                object toApply;
                try
                {
                    id = Convert.ToInt32(id);
                    if (id == -1)
                    {
                        toApply = this.sdModifiedList;
                    }
                    else if (this.game.scenedata.strings.Keys.Contains(id))
                    {
                        toApply = new List<int> {
                        id
                    };
                    }
                    else
                    {
                        throw new Exception(String.Format("id %d is not in strings dictionary.", id));
                    }
                    var applied = new List<object>();
                    var pFld = HSNeoOCIFolder.find_single("-strings-");
                    if (pFld == null)
                    {
                        pFld = HSNeoOCIFolder.add("-strings-");
                    }
                    foreach (var ta in toApply)
                    {
                        var tgtFlds = HSNeoOCIFolder.find_all_startswith(ta.ToString() + ":");
                        var found = false;
                        foreach (var tgtFld in tgtFlds)
                        {
                            if (tgtFld.treeNodeObject.parent == pFld.treeNodeObject)
                            {
                                tgtFld.name = ta.ToString() + ":" + this.game.scenedata.strings[ta];
                                applied.append(ta);
                                found = true;
                                break;
                            }
                        }
                        if (!found)
                        {
                            var newFld = HSNeoOCIFolder.add(ta.ToString() + ":" + this.game.scenedata.strings[ta]);
                            newFld.set_parent(pFld);
                            applied.append(ta);
                        }
                    }
                    if (applied.Count > 0)
                    {
                        foreach (var ta in applied)
                        {
                            this.sdModifiedList.remove(ta);
                        }
                        return String.Format("%d strings applied to -strings- folder. Save the scene!", applied.Count);
                    }
                    else
                    {
                        return "No string applied!";
                    }
                }
                catch (Exception)
                {
                    return "Apply string failed: " + e.ToString();
                }
            }

            public virtual void sd_find_or_create(string text)
            {
                // search for existed string
                foreach (var strId in this.game.scenedata.strings.Keys)
                {
                    if (this.game.scenedata.strings[strId] == text)
                    {
                        return strId;
                    }
                }
                // no match. create one
                var strId = this.sd_new(-1, text);
                this.sd_apply(strId);
                return strId;
            }

            public virtual void sd_revert()
            {
                register_string_resource(this.game);
                this.sdModifiedList = new List<object>();
                this.sd_search();
            }

            public virtual int sd_new_id(int id)
            {
                if (id == -1 || _sh.game.scenedata.strings.Keys.Contains(id))
                {
                    var idx = _sh.game.scenedata.strings.keys();
                    idx.sort();
                    if (idx.Count > 0)
                    {
                        return idx[-1] + 1;
                    }
                    else
                    {
                        return 1;
                    }
                }
                else
                {
                    return id;
                }
            }

            public virtual bool sd_is_modified(object id)
            {
                try
                {
                    id = Convert.ToInt32(id);
                    return this.sdModifiedList.Contains(id);
                }
                catch
                {
                    return false;
                }
            }

            public virtual void lh_init()
            {
                this.sd_init();
                if (this.pythonContent == "")
                {
                    this.load_python();
                }
                if (hasattr(this, "lhPrevVnText"))
                {
                    return;
                }
                this.lhPrevVnText = "";
                this.lhOrgTextType = "Unknown";
                this.lhOrgInput = "";
                this.lhTgtID = "";
                this.lhTgtInput = "";
                this.lhModifiedList = new Dictionary<object, object>
                {
                };
            }

            public virtual void lh_check_orgInput()
            {
                var pt = this.lhOrgInput.replace("\n", "\\n");
                var ct = this.pythonContent.count("\"" + pt + "\"");
                if (ct > 0)
                {
                    this.lhOrgTextType = String.Format("\"PlainText\" c=%d", ct);
                    this.lh_set_id(-1);
                    return;
                }
                ct = this.pythonContent.count("'" + pt + "'");
                if (ct > 0)
                {
                    this.lhOrgTextType = String.Format("'PlainText' c=%d", ct);
                    this.lh_set_id(-1);
                    return;
                }
                foreach (var si in this.game.scenedata.strings.Keys)
                {
                    if (this.lhOrgInput == this.game.scenedata.strings[si] || this.lhOrgInput == ltext(this.game, si))
                    {
                        this.lhOrgInput = this.game.scenedata.strings[si];
                        this.lhOrgTextType = String.Format("ltext id=%d", si);
                        this.lh_set_id(si);
                        return;
                    }
                }
                this.lhOrgTextType = "Unknown";
            }

            public virtual object lh_set_id(object id, int offset = 0)
            {
                object ididxs;
                try
                {
                    id = Convert.ToInt32(id);
                }
                catch
                {
                    id = -1;
                }
                if (_sh.game.scenedata.strings.Keys.Contains(id))
                {
                    if (offset != 0)
                    {
                        ididxs = _sh.game.scenedata.strings.keys();
                        ididxs.sort();
                        var ididx = ididxs.index(id);
                        ididx = (ididx + offset) % ididxs.Count;
                        id = ididxs[ididx];
                    }
                    this.lhTgtID = id.ToString();
                    this.lhTgtInput = _sh.game.scenedata.strings[id];
                    var idModify = true;
                }
                else if (offset != 0 && _sh.game.scenedata.strings.Count > 0)
                {
                    ididxs = _sh.game.scenedata.strings.keys();
                    ididxs.sort();
                    id = ididxs[0];
                    this.lhTgtID = id.ToString();
                    this.lhTgtInput = _sh.game.scenedata.strings[id];
                    idModify = true;
                }
                else
                {
                    this.lhTgtID = this.sd_new_id(id).ToString();
                    this.lhTgtInput = "";
                    idModify = false;
                }
                if (idModify)
                {
                    this.lh_modify();
                }
            }

            public virtual object lh_update_tgtInput()
            {
                object id;
                try
                {
                    id = Convert.ToInt32(this.lhTgtID);
                }
                catch
                {
                    id = -1;
                }
                if (!_sh.game.scenedata.strings.Keys.Contains(id))
                {
                    id = this.sd_new_id(id);
                    this.lhTgtID = id.ToString();
                }
                this.game.scenedata.strings[id] = this.lhTgtInput;
                if (!this.sdModifiedList.Contains(id))
                {
                    this.sdModifiedList.append(id);
                    this.lh_modify();
                }
            }

            public virtual string lh_build_replace_source()
            {
                if (this.lhOrgTextType.startswith("\"PlainText\""))
                {
                    return "\"" + this.lhOrgInput + "\"";
                }
                else if (this.lhOrgTextType.startswith("'PlainText'"))
                {
                    return "'" + this.lhOrgInput + "'";
                }
                else if (this.lhOrgTextType.startswith("ltext"))
                {
                    return String.Format("ltext(game, %s)", this.lhOrgTextType[9]);
                }
                else
                {
                    throw new Exception("Unable to build replace source from unknown type");
                }
            }

            public virtual void lh_modify()
            {
                var rplSrcText = this.lh_build_replace_source();
                var rplTgtText = String.Format("ltext(game, %s)", this.lhTgtID);
                if (rplSrcText == rplTgtText)
                {
                    if (this.lhModifiedList.Keys.Contains(rplSrcText))
                    {
                        this.lhModifiedList.pop(rplSrcText);
                    }
                }
                else
                {
                    this.lhModifiedList[rplSrcText] = rplTgtText;
                }
                //print "lh_modify src: " + rplSrcText
                //print "lh_modify tgt: " + rplTgtText
                //print "lhModifiedList :" + str(self.lhModifiedList)
                //print "sdModifiedList :" + str(self.sdModifiedList)
                //print ""
            }

            public virtual void lh_apply(object id = -1)
            {
                object msg;
                object repSrc;
                if (id == -1)
                {
                    var repLst = this.lhModifiedList.keys();
                }
                else
                {
                    id = Convert.ToInt32(id);
                    repSrc = this.lh_build_replace_source();
                    if (this.lhModifiedList.Contains(repSrc))
                    {
                        repLst = new List<object> {
                        repSrc
                    };
                    }
                    else
                    {
                        repLst = new List<object>();
                    }
                }
                foreach (var repSrc in repLst)
                {
                    var repTgt = this.lhModifiedList.pop(repSrc);
                    //print "replace [" + rplSrcText + "] to [" + rplTgtText + "]"
                    if (repSrc[0] == "'" || repSrc[0] == "\"")
                    {
                        repSrc = repSrc.replace("\n", "\\n");
                    }
                    this.pythonContent = this.pythonContent.replace(repSrc, repTgt);
                }
                if (repLst.Count > 0)
                {
                    msg = String.Format("%d plain-text or ltext in python file is replaced. Save the python!", repLst.Count);
                }
                else
                {
                    msg = "No python text replaced.";
                }
                if (id == -1 || this.sdModifiedList.Contains(id))
                {
                    msg += "\n" + this.sd_apply(id);
                }
                return msg;
            }

            public virtual void lh_revert()
            {
                this.sd_revert();
                this.load_python();
                this.lhModifiedList = new Dictionary<object, object>
                {
                };
                this.lh_check_orgInput();
            }

            public virtual bool lh_is_modified(string id)
            {
                if (this.sd_is_modified(id))
                {
                    return true;
                }
                else
                {
                    var rplSrcText = this.lh_build_replace_source();
                    return this.lhModifiedList.Keys.Contains(rplSrcText);
                }
            }

            public virtual void ch_init()
            {
                if (hasattr(this, "ch_sel_group"))
                {
                    return;
                }
                this.ch_base_actor = null;
                this.ch_base_speed = null;
                this.ch_base_pattern = null;
                this.ch_partner_actor = null;
                this.ch_partner_actor_sex = ValueTuple.Create("<Empty>");
                this.ch_ext_actor = new List<object>();
                this.ch_set_ext_actor = false;
                this.ch_sel_group = 0;
                this.ch_sel_category = 0;
                this.ch_sel_no = 0;
                this.ch_gp_sclpos = Vector2(0, 0);
                this.ch_ct_sclpos = Vector2(0, 0);
                this.ch_no_sclpos = Vector2(0, 0);
                this.ch_group_texts = get_hanime_group_names(this.game);
                this.ch_category_texts = get_hanime_category_names(this.game, this.ch_sel_group);
                this.ch_no_texts = get_hanime_no_names(this.game, this.ch_sel_group, this.ch_sel_category);
            }

            public virtual object ch_select_actor(int aIndex)
            {
                object sel;
                try
                {
                    sel = HSNeoOCI.create_from_selected();
                }
                catch
                {
                    sel = null;
                }
                if (!(sel is HSNeoOCIChar))
                {
                    scriptHelperGUIMessage("Nothing selected or unknown object.\nSelect a character then click the select button.");
                    return;
                }
                var selactor = sel.as_actor;
                if (aIndex == 0)
                {
                    this.ch_base_actor = selactor;
                    this.ch_base_speed = selactor.get_anime_speed();
                    this.ch_base_pattern = selactor.get_anime_pattern();
                    this.ch_partner_actor_sex = selactor.h_partner(this.ch_sel_group, this.ch_sel_category);
                    this.ch_check_partner();
                }
                else if (aIndex == 1)
                {
                    if (this.ch_partner_actor_sex[0] != -1 && this.ch_partner_actor_sex[0] != selactor.sex)
                    {
                        scriptHelperGUIMessage(String.Format("Partner actor must be a %s", this.ch_partner_actor_sex[0] ? "female" : "male"));
                    }
                    else if (this.ch_base_actor.treeNodeObject == selactor.treeNodeObject)
                    {
                        scriptHelperGUIMessage(String.Format("%s is already setted as base actor", selactor.text_name));
                    }
                    else
                    {
                        this.ch_partner_actor = selactor;
                    }
                }
                else if (this.ch_partner_actor_sex[aIndex - 1] != -1 && this.ch_partner_actor_sex[aIndex - 1] != selactor.sex)
                {
                    scriptHelperGUIMessage(String.Format("Extra actor %d must be a %s", aIndex - 1, this.ch_partner_actor_sex[aIndex - 1] ? "female" : "male"));
                }
                else if (this.ch_base_actor.treeNodeObject == selactor.treeNodeObject)
                {
                    scriptHelperGUIMessage(String.Format("%s is already setted as base actor", selactor.text_name));
                }
                else if (this.ch_partner_actor.treeNodeObject != null && this.ch_partner_actor.treeNodeObject == selactor.treeNodeObject)
                {
                    scriptHelperGUIMessage(String.Format("%s is already setted as parnter actor", selactor.text_name));
                }
                else
                {
                    this.ch_ext_actor[aIndex - 2] = selactor;
                }
            }

            public virtual void ch_check_partner()
            {
                if (this.ch_partner_actor != null)
                {
                    if (this.ch_partner_actor.treeNodeObject == this.ch_base_actor.treeNodeObject)
                    {
                        this.ch_partner_actor = null;
                    }
                    else if (this.ch_partner_actor_sex[0] != -1 && this.ch_partner_actor_sex[0] != this.ch_partner_actor.sex)
                    {
                        this.ch_partner_actor = null;
                    }
                }
                var new_ext_actor = new List<object>();
                foreach (var i in Enumerable.Range(0, this.ch_partner_actor_sex.Count - 1))
                {
                    if (this.ch_ext_actor.Count > i && this.ch_ext_actor[i] != null)
                    {
                        var ext_act = this.ch_ext_actor[i];
                        if (ext_act.treeNodeObject == this.ch_base_actor.treeNodeObject)
                        {
                            ext_act = null;
                        }
                        else if (this.ch_partner_actor_sex[i + 1] != -1 && this.ch_partner_actor_sex[i + 1] != ext_act.sex)
                        {
                            ext_act = null;
                        }
                        new_ext_actor.append(ext_act);
                    }
                    else
                    {
                        new_ext_actor.append(null);
                    }
                }
                this.ch_ext_actor = new_ext_actor;
                if (this.ch_ext_actor.Count <= 1)
                {
                    this.ch_set_ext_actor = false;
                }
            }

            public virtual void ch_change_group(object newGroup)
            {
                this.ch_sel_group = newGroup;
                this.ch_sel_category = 0;
                this.ch_sel_no = 0;
                this.ch_category_texts = get_hanime_category_names(this.game, this.ch_sel_group);
                this.ch_no_texts = get_hanime_no_names(this.game, this.ch_sel_group, this.ch_sel_category);
                if (this.ch_base_actor != null)
                {
                    this.ch_partner_actor_sex = this.ch_base_actor.h_partner(this.ch_sel_group, this.ch_sel_category);
                    this.ch_check_partner();
                }
            }

            public virtual void ch_change_category(object newCat)
            {
                this.ch_sel_category = newCat;
                this.ch_sel_no = 0;
                this.ch_no_texts = get_hanime_no_names(this.game, this.ch_sel_group, this.ch_sel_category);
                if (this.ch_base_actor != null)
                {
                    this.ch_partner_actor_sex = this.ch_base_actor.h_partner(this.ch_sel_group, this.ch_sel_category);
                    this.ch_check_partner();
                }
            }

            public virtual void ch_update_speed(double speed)
            {
                this.ch_base_actor.set_anime_speed(speed);
                this.ch_base_speed = this.ch_base_actor.get_anime_speed();
                if (this.ch_partner_actor != null)
                {
                    this.ch_partner_actor.set_anime_speed(speed);
                }
                foreach (var extActor in this.ch_ext_actor)
                {
                    if (extActor != null)
                    {
                        extActor.set_anime_speed(speed);
                    }
                }
            }

            public virtual void ch_update_pattern(double pattern)
            {
                this.ch_base_actor.set_anime_pattern(pattern);
                this.ch_base_pattern = this.ch_base_actor.get_anime_pattern();
                if (this.ch_partner_actor != null)
                {
                    this.ch_partner_actor.set_anime_pattern(pattern);
                }
                foreach (var extActor in this.ch_ext_actor)
                {
                    if (extActor != null)
                    {
                        extActor.set_anime_pattern(pattern);
                    }
                }
            }

            public virtual void ch_start()
            {
                var hasExt = false;
                foreach (var ext in this.ch_ext_actor)
                {
                    if (ext != null)
                    {
                        hasExt = true;
                        break;
                    }
                }
                if (this.ch_base_actor != null && this.ch_partner_actor != null)
                {
                    Console.WriteLine(String.Format("start h_with (%d, %d, %d) as %s > %s > %s", this.ch_sel_group, this.ch_sel_category, this.ch_sel_no, this.ch_group_texts[this.ch_sel_group], this.ch_category_texts[this.ch_sel_category], this.ch_no_texts[this.ch_sel_no]));
                    if (hasExt)
                    {
                        this.ch_base_actor.h_with(this.ch_partner_actor, this.ch_sel_group, this.ch_sel_category, this.ch_sel_no, tuple(this.ch_ext_actor));
                    }
                    else
                    {
                        this.ch_base_actor.h_with(this.ch_partner_actor, this.ch_sel_group, this.ch_sel_category, this.ch_sel_no);
                    }
                }
            }

            public virtual void ch_restart()
            {
                if (this.ch_base_actor != null)
                {
                    this.ch_base_actor.restart_anime();
                }
                if (this.ch_partner_actor != null)
                {
                    this.ch_partner_actor.restart_anime();
                }
                foreach (var extActor in this.ch_ext_actor)
                {
                    if (extActor != null)
                    {
                        extActor.restart_anime();
                    }
                }
            }

            public virtual string ch_actor_name(int index)
            {
                if (index == 0 && this.ch_base_actor != null)
                {
                    return this.ch_base_actor.text_name;
                }
                else if (index == 1 && this.ch_partner_actor != null)
                {
                    return this.ch_partner_actor.text_name;
                }
                else if (index > 1 && this.ch_ext_actor[index - 2] != null)
                {
                    return this.ch_ext_actor[index - 2].text_name;
                }
                else
                {
                    return "click to set";
                }
            }

            public virtual bool ch_get_overall_anime_option_visible()
            {
                var chAllActors = new List<object> {
                this.ch_base_actor,
                this.ch_partner_actor
            };
                chAllActors.extend(this.ch_ext_actor);
                foreach (var chActor in chAllActors)
                {
                    if (chActor != null && chActor.get_anime_option_visible())
                    {
                        return true;
                    }
                }
                return false;
            }

            public virtual void ch_set_overall_anime_option_visible(object visible)
            {
                var chAllActors = new List<object> {
                this.ch_base_actor,
                this.ch_partner_actor
            };
                chAllActors.extend(this.ch_ext_actor);
                foreach (var chActor in chAllActors)
                {
                    if (chActor != null)
                    {
                        chActor.set_anime_option_visible(visible);
                    }
                }
            }

            public virtual bool ch_get_overall_shoes()
            {
                var chAllActors = new List<object> {
                this.ch_base_actor,
                this.ch_partner_actor
            };
                chAllActors.extend(this.ch_ext_actor);
                foreach (var chActor in chAllActors)
                {
                    if (chActor != null && chActor.get_cloth()[-1] == 0)
                    {
                        return true;
                    }
                }
                return false;
            }

            public virtual void ch_set_overall_shoes(object shoes)
            {
                var chAllActors = new List<object> {
                this.ch_base_actor,
                this.ch_partner_actor
            };
                chAllActors.extend(this.ch_ext_actor);
                foreach (var chActor in chAllActors)
                {
                    if (chActor != null)
                    {
                        var allcloth = chActor.get_cloth().ToList();
                        allcloth[-1] = shoes ? 0 : 2;
                        chActor.set_cloth(tuple(allcloth));
                    }
                }
            }

            public virtual string as_create_new()
            {
                object msg;
                object defaultEnd;
                object defaultEndBtn;
                object defaultReloadBtn;
                object defaultNextBtn;
                try
                {
                    // check asInfo
                    if (this.asInfo.gameName.Count == 0 || this.asInfo.pythonName.Count == 0 || this.asInfo.sceneDir.Count == 0 || this.asInfo.scenePNG.Count == 0)
                    {
                        throw new Exception("Some basic infomation missed!");
                    }
                    if (this.asInfo.gameName.isunicode() || this.asInfo.pythonName.isunicode() || this.asInfo.sceneDir.isunicode() || this.asInfo.scenePNG.isunicode())
                    {
                        throw new Exception("Game name/scene dir/scene png/python file settings can not use UNICODE texts");
                    }
                    // prepare basic infomation
                    var gameTitleLine = String.Format("#vngame;%s;%s", this.game.engine_name, this.asInfo.gameName);
                    if (this.asInfo.pythonName.lower().endswith(".py"))
                    {
                        aelf.asInfo.pythonName = this.asInfo.pythonName[:: - 3];
                    }
                    if (this.asInfo.sceneDir[-1] != "\\" && this.asInfo.sceneDir[-1] != "/")
                    {
                        this.asInfo.sceneDir += "\\";
                    }
                    if (!this.asInfo.scenePNG.lower().endswith(".png"))
                    {
                        this.asInfo.scenePNG += ".png";
                    }
                    if (this.asInfo.createLocalizeString)
                    {
                        this.sd_init();
                        var strId = this.sd_find_or_create(this.asInfo.defaultNextBtnText);
                        defaultNextBtn = String.Format("ltext(game, %d)", strId);
                        strId = this.sd_find_or_create(this.asInfo.defaultReloadBtnText);
                        defaultReloadBtn = String.Format("ltext(game, %d)", strId);
                        strId = this.sd_find_or_create(this.asInfo.defaultEndBtnText);
                        defaultEndBtn = String.Format("ltext(game, %d)", strId);
                        strId = this.sd_find_or_create(this.asInfo.defaultEndText);
                        defaultEnd = String.Format("ltext(game, %d)", strId);
                    }
                    else
                    {
                        defaultNextBtn = "\"" + this.asInfo.defaultNextBtnText + "\"";
                        defaultReloadBtn = "\"" + this.asInfo.defaultReloadBtnText + "\"";
                        defaultEndBtn = "\"" + this.asInfo.defaultEndBtnText + "\"";
                        defaultEnd = "\"" + this.asInfo.defaultEndText + "\"";
                    }
                    // read template and apply setting
                    this.load_python("vnftemplate.py");
                    this.pythonContent = this.pythonContent.replace("#-VNFA:GameTitle-#", gameTitleLine);
                    this.pythonContent = this.pythonContent.replace("#-VNFA:SceneDir-#", "\"" + this.asInfo.sceneDir.replace("\\", "\\\\") + "\"");
                    this.pythonContent = this.pythonContent.replace("#-VNFA:ScenePNG-#", "\"" + this.asInfo.scenePNG + "\"");
                    this.pythonContent = this.pythonContent.replace("#-VNFA:EnableReload-#", this.asInfo.enableReload.ToString());
                    this.pythonContent = this.pythonContent.replace("#-VNFA:EnableQuickReload-#", this.asInfo.enableQuickReload.ToString());
                    this.pythonContent = this.pythonContent.replace("#-VNFA:HideWindow-#", this.asInfo.alwaysHideWindowInCameraAnime.ToString());
                    this.game.isHideWindowDuringCameraAnimation = this.asInfo.alwaysHideWindowInCameraAnime;
                    this.pythonContent = this.pythonContent.replace("#-VNFA:LockWindow-#", this.asInfo.alwaysLockWindowInSceneAnime.ToString());
                    this.game.isLockWindowDuringSceneAnimation = this.asInfo.alwaysLockWindowInSceneAnime;
                    this.pythonContent = this.pythonContent.replace("#-VNFA:SkinVersion-#", "\"" + this.asInfo.skinVersion + "\"");
                    this.pythonContent = this.pythonContent.replace("#-VNFA:FakeLipSyncEnable-#", this.asInfo.fakeLipSyncEnable.ToString());
                    this.game.isfAutoLipSync = this.asInfo.fakeLipSyncEnable;
                    this.pythonContent = this.pythonContent.replace("#-VNFA:FakeLipSyncVersion-#", "\"" + this.asInfo.fakeLipSyncVersion + "\"");
                    this.game.fAutoLipSyncVer = this.asInfo.fakeLipSyncVersion;
                    this.pythonContent = this.pythonContent.replace("#-VNFA:FakeLipSyncReadingSpeed-#", this.asInfo.fakeLipSyncReadingSpeed.ToString());
                    this.game.readingSpeed = this.asInfo.fakeLipSyncReadingSpeed;
                    this.pythonContent = this.pythonContent.replace("#-VNFA:DefaultNextText-#", defaultNextBtn);
                    this.game.btnNextText = this.asInfo.defaultNextBtnText;
                    this.pythonContent = this.pythonContent.replace("#-VNFA:CreateString-#", this.asInfo.createLocalizeString.ToString());
                    this.createLocalizeStringOnBuild = this.asInfo.createLocalizeString;
                    this.pythonContent = this.pythonContent.replace("#-VNFA:MasterMode-#", this.asInfo.masterMode.ToString());
                    this.masterMode = this.asInfo.masterMode;
                    this.pythonContent = this.pythonContent.replace("#-VNFA:DefaultEndText-#", defaultEnd);
                    this.pythonContent = this.pythonContent.replace("#-VNFA:DefaultRestartButton-#", defaultReloadBtn);
                    this.pythonContent = this.pythonContent.replace("#-VNFA:DefaultEndButton-#", defaultEndBtn);
                    // save python file
                    this.game.current_game = this.asInfo.pythonName;
                    var pyMsg = this.save_python(false);
                    // Save scene PNG
                    this.game.sceneDir = this.asInfo.sceneDir;
                    this.game.scenePNG = this.asInfo.scenePNG;
                    var pngMsg = this.save_scene(false);
                    // report
                    msg = String.Format("New game <%s> for %s created!\n", this.asInfo.gameName, this.game.engine_name);
                    msg += pyMsg + "\n";
                    msg += pngMsg + "\n";
                }
                catch (Exception)
                {
                    msg = "Fail to create new auto script: " + e.ToString();
                }
                return msg;
            }

            public virtual object as_add_seq(string scrString)
            {
                try
                {
                    if (this.pythonContent.Count == 0)
                    {
                        this.load_python();
                    }
                    if (!this.asTemplate)
                    {
                        throw new Exception("Python contents error, or not a template script");
                    }
                    // search for insert position
                    var spos = this.pythonContent.find("#-VNFA:seq:empty:");
                    if (spos == -1)
                    {
                        throw "Can not find insert anchor start position!";
                    }
                    var epos = this.pythonContent.find("-#", spos + 17);
                    if (epos == -1)
                    {
                        throw "Can not find insert anchor end position!";
                    }
                    var rplAnchor = this.pythonContent[spos::(epos + 2)];
                    var seqNo = Convert.ToInt32(this.pythonContent[(spos + 17)::epos]);
                    var seqNext = seqNo + 1;
                    //print "replace anchor = %s, no = %d"%(rplAnchor, seqNo)
                    // new sequence
                    var newSeq = String.Format("#-VNFA:seq:start:%d-#\n", seqNo);
                    newSeq += scrString;
                    newSeq += String.Format("        #-VNFA:seq:end:%d-#\n", seqNo);
                    newSeq += String.Format("    ], toSeq%d)\n", seqNext);
                    newSeq += "\n";
                    newSeq += String.Format("def toSeq%d(game):\n", seqNext);
                    newSeq += "    game.texts_next([\n";
                    newSeq += String.Format("        #-VNFA:seq:empty:%d-#", seqNext);
                    //print newSeq
                    this.pythonContent = this.pythonContent.replace(rplAnchor, newSeq);
                    return String.Format("All script clips in anime buffer were build into <color=#ff0000>toSeq%d()</color> function of current python contents.\nSave the python to save your work!", seqNo);
                }
                catch (Exception)
                {
                    return "Add seq failed: " + e.ToString();
                }
            }

            public virtual object as_load()
            {
                Func<object, object, object> ltext = (game, no) => {
                    return String.Format("ltext(game, %d)", no);
                };
                try
                {
                    if (this.pythonContent.Count == 0)
                    {
                        this.load_python();
                    }
                    if (!this.asTemplate)
                    {
                        throw new Exception("Python contents error, or not a template script");
                    }
                    // search for insert position
                    var searchStartPos = 0;
                    while (true)
                    {
                        var sspos = this.pythonContent.find("#-VNFA:seq:start:", searchStartPos);
                        if (sspos == -1)
                        {
                            break;
                        }
                        var sepos = this.pythonContent.find("-#", sspos + 17);
                        if (sepos == -1)
                        {
                            throw new Exception("Can not find start anchor end position!");
                        }
                        var seqNo = Convert.ToInt32(this.pythonContent[(sspos + 17)::sepos]);
                        var staAnchor = this.pythonContent[sspos::(sepos + 2)];
                        var endAnchor = String.Format("#-VNFA:seq:end:%d-#", seqNo);
                        var espos = this.pythonContent.find(endAnchor, sepos + 2);
                        if (espos == -1)
                        {
                            throw new Exception(String.Format("Can not find end anchor %s pair with start anchor %s!", endAnchor, staAnchor));
                        }
                        searchStartPos = espos + endAnchor.Count;
                        Console.WriteLine(String.Format("found anchor pair %d", seqNo));
                        var scriptTexts = "[" + this.pythonContent[(sepos + 2)::espos] + "]";
                        var game = this.game;
                        var script = eval(scriptTexts);
                        //print scriptTexts
                        Console.WriteLine(script);
                        Console.WriteLine(script2string(script));
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine("as_load failed: " + e.ToString());
                }
            }

            public virtual object as_rebuild_clips(object clipString)
            {
                try
                {
                    if (this.pythonContent.Count == 0)
                    {
                        this.load_python();
                    }
                    if (!this.asTemplate)
                    {
                        throw new Exception("Python contents error, or not a template script");
                    }
                    // search for insert position
                    var spos = this.pythonContent.find("#-VNFA:KeyFrameClips:start-#");
                    if (spos == -1)
                    {
                        throw new Exception("Can not find keyframe clips insert anchor start position!");
                    }
                    spos += "#-VNFA:KeyFrameClips:start-#".Count;
                    var epos = this.pythonContent.find("#-VNFA:KeyFrameClips:end-#");
                    if (epos == -1)
                    {
                        throw new Exception("Can not find keyframe clips insert anchor end position!");
                    }
                    if (spos > epos)
                    {
                        throw new Exception("Keyframe clips insert anchor start/end position mismatch!");
                    }
                    // insert all
                    this.pythonContent = this.pythonContent[::spos] + "\n" + clipString + this.pythonContent[epos];
                    return "All keyframe clips in keyframe clip manager were build into current python contents.\nSave the python to save your work!";
                }
                catch (Exception)
                {
                    return "Add seq failed: " + e.ToString();
                }
            }

            // utilities
            public virtual void shrink_mode(bool shrink)
            {
                this.guiWidth = shrink ? this._shrinkWidth : this._normalWidth;
                this.guiHeight = shrink ? this._shrinkHeight : this._normalHeight;
                this.game.windowRect = Rect(Screen.width / 2 - this.game.wwidth / 2, Screen.height - this.game.wheight - 10, this.game.wwidth, this.game.wheight);
            }

            public virtual void clear_dialogue()
            {
                // clear dialogue
                this.curSCInfo.dialogue = "";
            }

            public virtual string get_next_speaker(string curSpeakAlias, bool next)
            {
                // next from unknown speaker
                if (curSpeakAlias != "s" && !this.game.scenedata.actors.Keys.Contains(curSpeakAlias))
                {
                    return "s";
                }
                // next from s or actor
                if (curSpeakAlias == "s")
                {
                    if (this.game.scenedata.actors.Count > 0)
                    {
                        if (next)
                        {
                            return this.game.scenedata.actors.Keys[0];
                        }
                        else
                        {
                            return this.game.scenedata.actors.Keys[-1];
                        }
                    }
                    else
                    {
                        return "s";
                    }
                }
                else
                {
                    var nextIndex = this.game.scenedata.actors.Keys.IndexOf(curSpeakAlias);
                    if (next)
                    {
                        nextIndex += 1;
                    }
                    else
                    {
                        nextIndex -= 1;
                    }
                    if (Enumerable.Range(0, this.game.scenedata.actors.Count).Contains(nextIndex))
                    {
                        return this.game.scenedata.actors.Keys[nextIndex];
                    }
                    else
                    {
                        return "s";
                    }
                }
            }

            [staticmethod]
            public static object diffSceneWithPrev(ScriptHelper curScn, object preScn)
            {
                var ds = new Dictionary<object, object>
                {
                };
                foreach (var tgt in curScn.Keys)
                {
                    if (preScn.Keys.Contains(tgt))
                    {
                        foreach (var actFunc in curScn[tgt].Keys)
                        {
                            if (!preScn[tgt].Keys.Contains(actFunc) || curScn[tgt][actFunc] != preScn[tgt][actFunc])
                            {
                                if (!ds.Keys.Contains(tgt))
                                {
                                    ds[tgt] = new Dictionary<object, object>
                                    {
                                    };
                                }
                                ds[tgt][actFunc] = curScn[tgt][actFunc];
                            }
                        }
                    }
                    else
                    {
                        ds[tgt] = scriptCopy(curScn[tgt]);
                    }
                }
                return ds;
            }

            [staticmethod]
            public static Dictionary<object, object> optimizeAnimeScript(object orgScript)
            {
                var optScript = new Dictionary<object, object>
                {
                };
                try
                {
                    foreach (var tgt in orgScript.Keys)
                    {
                        optScript[tgt] = new Dictionary<object, object>
                        {
                        };
                        foreach (var actFunc in orgScript[tgt].Keys)
                        {
                            if ((actFunc == "ik_set" || actFunc == "fk_set") && orgScript[tgt][actFunc][0] is dict)
                            {
                                // skip duplicated bone info
                                var optFrom = new Dictionary<object, object>
                                {
                                };
                                var optTo = new Dictionary<object, object>
                                {
                                };
                                foreach (var bi in orgScript[tgt][actFunc][0].Keys)
                                {
                                    if (orgScript[tgt][actFunc][0][bi] != orgScript[tgt][actFunc][1][bi])
                                    {
                                        optFrom[bi] = orgScript[tgt][actFunc][0][bi];
                                        optTo[bi] = orgScript[tgt][actFunc][1][bi];
                                    }
                                }
                                optScript[tgt][actFunc] = (optFrom, optTo);
                            }
                            else
                            {
                                optScript[tgt][actFunc] = orgScript[tgt][actFunc];
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine("optimizeAnimeScript error:", e);
                }
                return optScript;
            }

            [staticmethod]
            public static Dictionary<object, object> get_cam_status()
            {
                var cdata = Studio.Instance.cameraCtrl.cameraData;
                var camDic = new Dictionary<object, object>
                {
                };
                camDic["cam"] = new Dictionary<object, object>
                {
                };
                camDic["cam"]["goto_pos"] = (cdata.pos, cdata.distance, cdata.rotate);
                //print ("# other one: 'cam': {'goto_pos': ((%.3f, %.3f, %.3f), (%.3f, %.3f, %.3f), (%.3f, %.3f, %.3f))}"%(cdata.pos.x, cdata.pos.y, cdata.pos.z, cdata.distance.x, cdata.distance.y, cdata.distance.z, cdata.rotate.x, cdata.rotate.y, cdata.rotate.z))
                return camDic;
            }

            [staticmethod]
            public static Dictionary<object, object> get_scn_status(object game)
            {
                var fs = new Dictionary<object, object>
                {
                };
                if (hasattr(game.scenedata, "actors"))
                {
                    foreach (var actorAlias in game.scenedata.actors.Keys)
                    {
                        fs[actorAlias] = game.scenedata.actors[actorAlias].export_full_status();
                    }
                }
                if (hasattr(game.scenedata, "props"))
                {
                    foreach (var propAlias in game.scenedata.props.Keys)
                    {
                        fs[propAlias] = game.scenedata.props[propAlias].export_full_status();
                    }
                }
                if (hasattr(game.gdata, "kfaManagedClips"))
                {
                    foreach (var clipName in game.gdata.kfaManagedClips.Keys)
                    {
                        fs[clipName] = game.gdata.kfaManagedClips[clipName].export_full_status();
                    }
                }
                fs["sys"] = export_sys_status(game);
                return fs;
            }
        


        // -------------------------- script helper --------------------------
        // global functions for ScriptHelper
        public static object init_script_helper(object game)
        {
            game.onDumpSceneOverride = scriptHelperGUIStart;
            _sh = new ScriptHelper(game);
            return _sh;
        }

        public static object toggle_devconsole(object game)
        {
            // initialize if not
            if (_sh == null)
            {
                _sh = new ScriptHelper(game);
            }
            // toggle script helper guiOnShow
            if (_sh.guiOnShow)
            {
                scriptHelperGUIClose();
            }
            else
            {
                scriptHelperGUIStart(game);
            }
        }

        // ----- some wraps for skin -----
        public static object scriptHelperGUIStart(object game)
        {
            // register actor/prop if not
            if (!hasattr(game.scenedata, "actors") || !hasattr(game.scenedata, "props"))
            {
                register_actor_prop_by_tag(game);
            }
            // init anime clip if not
            if (!check_keyframe_anime(game))
            {
                init_keyframe_anime(game);
            }
            _sh.game_skin_saved = game.skin;
            _sh.guiOnShow = true;
            scriptHelperGUIToSceen(_sh.guiScreenIndex);
            var skin = SkinCustomWindow();
            skin.funcSetup = scriptHelperSkinSetup;
            skin.funcWindowGUI = scriptHelperSkinWindowGUI;
            game.skin_set(skin);
        }

        public static object scriptHelperSkinSetup(object game)
        {
            game.wwidth = _sh.guiWidth;
            game.wheight = _sh.guiHeight;
            game.windowRect = Rect(Screen.width / 2 - game.wwidth / 2, Screen.height - game.wheight - 10, game.wwidth, game.wheight);
            //game.windowCallback = GUI.WindowFunction(scriptHelperWindowGUI)
            game.windowStyle = game.windowStyleDefault;
        }

        public static object scriptHelperSkinWindowGUI(object game, object windowid)
        {
            scriptHelperWindowGUI(windowid);
        }

        public static void scriptHelperGUIClose(bool toDevConsole = false)
        {
            _sh.guiOnShow = false;
            _sh.game.windowName = "";
            _sh.game.isShowDevConsole = toDevConsole;
            _sh.game.skin_set(_sh.game_skin_saved);
        }

        // ----- end wraps for skin ------
        public static void scriptHelperGUIToSceen(double toScreen)
        {
            var validScreen = new Dictionary<object, object> {
            {
                0,
                "Script Builder"},
            {
                0.1,
                "Script Builder"},
            {
                1,
                "Anime Buffer"},
            {
                2,
                "Key Frame Animation Clip Manager"},
            {
                3,
                "Scene Helper"},
            {
                10,
                "String Dictionary"},
            {
                11,
                "Localize Helper"},
            {
                12,
                "Couple Helper"},
            {
                13,
                "Couple Helper -adjust-"},
            {
                14,
                "VNActor Export Setting"},
            {
                20,
                "New Game Wizard 1/2"},
            {
                21,
                "New Game Wizard 2/2"},
            {
                31,
                "Select Dump Target"}};
            if (validScreen.Keys.Contains(toScreen))
            {
                _sh.guiScreenIndex = toScreen;
                _sh.game.windowName = validScreen[toScreen];
            }
            else
            {
                Console.WriteLine("Invalid screen index:", toScreen);
            }
        }

        public static void scriptHelperGUIMessage(object msg, object action = 2, object param = null)
        {
            //print "scriptHelperGUIMessage from", _sh.guiScreenIndex, "\nmsg:", msg, "action:", action, "param:", param
            _sh.prevMsgGuiScreenIndex = _sh.guiScreenIndex;
            _sh.msgTexts = msg;
            _sh.msgParam = param;
            if (action is tuple)
            {
                _sh.msgAction = action;
                _sh.guiScreenIndex = 99;
            }
            else
            {
                _sh.msgAction = null;
                var tid = _sh.game.set_timer(float(action), scriptHelperGUIMessageRtn);
                if (tid != -1)
                {
                    _sh.guiScreenIndex = 99;
                }
                else
                {
                    Console.WriteLine("run out of timer");
                }
            }
        }

        public static void scriptHelperGUIMessageRtn(object game = null)
        {
            //print "scriptHelperGUIMessageRtn to", _sh.prevMsgGuiScreenIndex
            _sh.guiScreenIndex = _sh.prevMsgGuiScreenIndex;
        }

        public static object scriptHelperWindowGUI(object windowid)
        {
            object maParam;
            object maFunc;
            object maText;
            object comment;
            object chk;
            object newPtn;
            object prevTexts;
            object resMsg;
            object pyInfo;
            object msg;
            object aniDur;
            object durStr;
            Func<object, object> checkId = tgtId => {
                return sd.localTgts.count(tgtId) == 1;
            };
            Func<object, object, object> toggleId = (tgtId, check) => {
                if (checkId(tgtId) && !check)
                {
                    sd.localTgts.remove(tgtId);
                }
                if (!checkId(tgtId) && check)
                {
                    sd.localTgts.append(tgtId);
                }
            };
            Func<object> quitTgtSelect = () => {
                WONKO_del(sd.localTgts);
                scriptHelperGUIToSceen(0);
            };
            try
            {
                var fullw = _sh.game.wwidth - 30;
                var customButton = GUIStyle("button");
                customButton.fontSize = 14;
                var dumpTgtTexts = Array[String](("Diff", "All"));
                var dumpAsTexts = Array[String](("new act", "new anime", "sub anime"));
                var aniStyleTexts = Array[String](("Linear", "S-F", "F-S", "S-F3", "F-S3", "S-F4", "F-S4"));
                GUILayout.BeginVertical(GUILayout.Width(fullw));
                if (_sh.guiScreenIndex == 0)
                {
                    // screen No.0: Script Builder
                    // dump scene setting
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Dump", GUILayout.Width(35));
                    dumpTgtTexts[_sh.curSCInfo.dumpTypeIndex] = "<color=#00ffff>" + dumpTgtTexts[_sh.curSCInfo.dumpTypeIndex] + "</color>";
                    _sh.curSCInfo.dumpTypeIndex = GUILayout.SelectionGrid(_sh.curSCInfo.dumpTypeIndex, dumpTgtTexts, 2, GUILayout.Width(80));
                    GUILayout.Label("of", GUILayout.Width(15));
                    if (GUILayout.Button(String.Format("<color=#00ffff>%s</color>", _sh.curSCInfo.dumpTgts == null ? "ALL objs" : String.Format("%d objs", _sh.curSCInfo.dumpTgts.Count)), GUILayout.Width(71)))
                    {
                        scriptHelperGUIToSceen(31);
                    }
                    GUILayout.Label("as", GUILayout.Width(15));
                    dumpAsTexts[_sh.curSCInfo.dumpAsIndex] = "<color=#00ffff>" + dumpAsTexts[_sh.curSCInfo.dumpAsIndex] + "</color>";
                    _sh.curSCInfo.dumpAsIndex = GUILayout.SelectionGrid(_sh.curSCInfo.dumpAsIndex, dumpAsTexts, 3, GUILayout.Width(230));
                    GUILayout.EndHorizontal();
                    // speaker/dialogue setting
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button("<", GUILayout.Width(20)))
                    {
                        _sh.curSCInfo.speakerAlias = _sh.get_next_speaker(_sh.curSCInfo.speakerAlias, false);
                    }
                    if (GUILayout.Button(">", GUILayout.Width(20)))
                    {
                        _sh.curSCInfo.speakerAlias = _sh.get_next_speaker(_sh.curSCInfo.speakerAlias, true);
                    }
                    _sh.curSCInfo.speakerAlias = GUILayout.TextField(_sh.curSCInfo.speakerAlias, GUILayout.Width(50));
                    GUILayout.Label("says:", GUILayout.Width(35));
                    _sh.curSCInfo.dialogue = GUILayout.TextField(_sh.curSCInfo.dialogue, GUILayout.Width(286));
                    if (GUILayout.Button("Clear", GUILayout.Width(45)))
                    {
                        _sh.clear_dialogue();
                    }
                    GUILayout.EndHorizontal();
                    // scene anime detail
                    if (_sh.curSCInfo.dumpAsIndex != 0)
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Anime Duration:", GUILayout.Width(110));
                        if (GUILayout.Button("-1s", GUILayout.Width(50)))
                        {
                            _sh.curSCInfo.animeDuration -= 1;
                        }
                        if (GUILayout.Button("-0.1s", GUILayout.Width(50)))
                        {
                            _sh.curSCInfo.animeDuration -= 0.1;
                        }
                        if (_sh.curSCInfo.animeDuration < 0)
                        {
                            _sh.curSCInfo.animeDuration = 0;
                        }
                        durStr = GUILayout.TextField(_sh.curSCInfo.animeDuration.ToString(), 10, GUILayout.Width(50));
                        try
                        {
                            _sh.curSCInfo.animeDuration = float(durStr);
                        }
                        catch (ValueError)
                        {
                            Console.WriteLine(String.Format("invalid input in 'anime duration': %s, float expected.", durStr));
                        }
                        if (GUILayout.Button("+0.1s", GUILayout.Width(50)))
                        {
                            _sh.curSCInfo.animeDuration += 0.1;
                        }
                        if (GUILayout.Button("+1s", GUILayout.Width(50)))
                        {
                            _sh.curSCInfo.animeDuration += 1;
                        }
                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Anime Style:", GUILayout.Width(110));
                        _sh.curSCInfo.animeStyle = GUILayout.SelectionGrid(_sh.curSCInfo.animeStyle, aniStyleTexts, 7);
                        GUILayout.EndHorizontal();
                    }
                    // camera anime setting
                    GUILayout.BeginHorizontal();
                    _sh.curSCInfo.includeCamera = GUILayout.Toggle(_sh.curSCInfo.includeCamera, "Include Camera", GUILayout.Width(fullw / 3 - 2));
                    if (_sh.curSCInfo.includeCamera)
                    {
                        _sh.curSCInfo.animateCamera = GUILayout.Toggle(_sh.curSCInfo.animateCamera, "Camera Anime", GUILayout.Width(fullw / 3 - 2));
                        if (_sh.curSCInfo.animateCamera)
                        {
                            _sh.curSCInfo.useCameraTimer = GUILayout.Toggle(_sh.curSCInfo.useCameraTimer, "Use Camera Timer", GUILayout.Width(fullw / 3 - 2)) || _sh.curSCInfo.dumpAsIndex == 0;
                        }
                    }
                    GUILayout.EndHorizontal();
                    // camera anime detail
                    if (_sh.curSCInfo.includeCamera && _sh.curSCInfo.animateCamera && _sh.curSCInfo.useCameraTimer)
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Camera Duration:", GUILayout.Width(110));
                        if (GUILayout.Button("-1s", GUILayout.Width(50)))
                        {
                            _sh.curSCInfo.cameraDuration -= 1;
                        }
                        if (GUILayout.Button("-0.1s", GUILayout.Width(50)))
                        {
                            _sh.curSCInfo.cameraDuration -= 0.1;
                        }
                        if (_sh.curSCInfo.cameraDuration < 0)
                        {
                            _sh.curSCInfo.cameraDuration = 0;
                        }
                        durStr = GUILayout.TextField(_sh.curSCInfo.cameraDuration.ToString(), 10, GUILayout.Width(50));
                        try
                        {
                            _sh.curSCInfo.cameraDuration = float(durStr);
                        }
                        catch (ValueError)
                        {
                            Console.WriteLine(String.Format("invalid input in 'anime duration': %s, float expected.", durStr));
                        }
                        if (GUILayout.Button("+0.1s", GUILayout.Width(50)))
                        {
                            _sh.curSCInfo.cameraDuration += 0.1;
                        }
                        if (GUILayout.Button("+1s", GUILayout.Width(50)))
                        {
                            _sh.curSCInfo.cameraDuration += 1;
                        }
                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Camera Style:", GUILayout.Width(110));
                        _sh.curSCInfo.cameraStyle = GUILayout.SelectionGrid(_sh.curSCInfo.cameraStyle, aniStyleTexts, 7);
                        GUILayout.EndHorizontal();
                    }
                    // clip setting
                    GUILayout.BeginHorizontal();
                    _sh.dumpClipToFile = GUILayout.Toggle(_sh.dumpClipToFile, "To file", GUILayout.Width(70));
                    if (_sh.curSCInfo.dumpAsIndex != 0)
                    {
                        _sh.curSCInfo.hideWindowInAnime = GUILayout.Toggle(_sh.curSCInfo.hideWindowInAnime, "Hide window in anime", GUILayout.Width(150));
                        if (!_sh.curSCInfo.hideWindowInAnime)
                        {
                            _sh.curSCInfo.hideButtonInAnime = GUILayout.Toggle(_sh.curSCInfo.hideButtonInAnime, "Hide button in anime", GUILayout.Width(150));
                        }
                    }
                    GUILayout.EndHorizontal();
                    // VNSceneScript patch
                    vnframe_vnscenescripthelper.sshelper_onelineinterface(_sh);
                    // VNSceneScript patch end
                    // Tail button
                    GUILayout.FlexibleSpace();
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button("-", customButton, GUILayout.Width(20), GUILayout.Height(24)))
                    {
                        _sh.shrink_mode(true);
                        scriptHelperGUIToSceen(0.1);
                    }
                    if (GUILayout.Button(String.Format("<color=#ff6666ff>%s</color>", _sh.dumpClipToFile ? "Dump" : "Build"), customButton, GUILayout.Width(100 - 24), GUILayout.Height(24)))
                    {
                        _sh.append_clip();
                        scriptHelperGUIMessage(String.Format("Script%s added into anime buffer.", _sh.dumpClipToFile ? " Dumped and " : ""));
                    }
                    if (GUILayout.Button("Anime Buffer", customButton, GUILayout.Width(100), GUILayout.Height(24)))
                    {
                        scriptHelperGUIToSceen(1);
                    }
                    if (GUILayout.Button("Clip Manager", customButton, GUILayout.Width(100), GUILayout.Height(24)))
                    {
                        scriptHelperGUIToSceen(2);
                    }
                    if (GUILayout.Button("Scene Helper", customButton, GUILayout.Width(100), GUILayout.Height(24)))
                    {
                        scriptHelperGUIToSceen(3);
                    }
                    if (GUILayout.Button("Back", customButton, GUILayout.Width(50), GUILayout.Height(24)))
                    {
                        scriptHelperGUIClose();
                    }
                    GUILayout.EndHorizontal();
                }
                else if (_sh.guiScreenIndex == 0.1)
                {
                    // screen No.0.1: Script Builder shunk mode
                    // Tail button
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button("+", customButton, GUILayout.Width(20)))
                    {
                        _sh.shrink_mode(false);
                        scriptHelperGUIToSceen(0);
                    }
                    if (GUILayout.Button(String.Format("<color=#ff6666ff>%s</color>", _sh.dumpClipToFile ? "Dump Script" : "Build Script")))
                    {
                        _sh.append_clip();
                        scriptHelperGUIMessage(String.Format("Script%s added into anime buffer.", _sh.dumpClipToFile ? " Dumped and " : ""));
                    }
                    GUILayout.EndHorizontal();
                }
                else if (_sh.guiScreenIndex == 1)
                {
                    // screen No.1: Anime Buffer
                    var animeStopped = _sh.game.scnAnimeTID == -1;
                    // replay control
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button("<", GUILayout.Width(20)))
                    {
                        if (_sh.animeBufferIndex > 1)
                        {
                            _sh.animeBufferIndex -= 1;
                        }
                    }
                    GUILayout.Label(String.Format("%d/%d", _sh.animeBufferIndex, _sh.animeBuffer.Count - 1), GUILayout.Width(40));
                    if (GUILayout.Button(">", GUILayout.Width(20)))
                    {
                        if (_sh.animeBufferIndex < _sh.animeBuffer.Count - 1)
                        {
                            _sh.animeBufferIndex += 1;
                        }
                    }
                    if (GUILayout.Button(String.Format("%s", animeStopped ? "Replay" : "Stop"), GUILayout.Width(70)))
                    {
                        _sh.play_anime_clip(animeStopped ? "play" : "stop");
                    }
                    if (GUILayout.Button("To Start"))
                    {
                        _sh.play_anime_clip("tostart");
                    }
                    if (GUILayout.Button("To End"))
                    {
                        _sh.play_anime_clip("toend");
                    }
                    if (GUILayout.Button("Play & Next"))
                    {
                        _sh.play_anime_clip("play_and_next");
                    }
                    _sh.slowMotion = GUILayout.Toggle(_sh.slowMotion, "Slow Motion");
                    GUILayout.EndHorizontal();
                    if (_sh.animeBufferIndex != 0)
                    {
                        var cInfo = _sh.animeBuffer[_sh.animeBufferIndex].info;
                        // anime speaker/dialogue info
                        GUILayout.BeginHorizontal();
                        if (GUILayout.Button("<", GUILayout.Width(20)))
                        {
                            cInfo.speakerAlias = _sh.get_next_speaker(cInfo.speakerAlias, false);
                        }
                        if (GUILayout.Button(">", GUILayout.Width(20)))
                        {
                            cInfo.speakerAlias = _sh.get_next_speaker(cInfo.speakerAlias, true);
                        }
                        cInfo.speakerAlias = GUILayout.TextField(cInfo.speakerAlias, GUILayout.Width(50));
                        GUILayout.Label("says:", GUILayout.Width(35));
                        cInfo.dialogue = GUILayout.TextField(cInfo.dialogue, GUILayout.Width(286));
                        if (GUILayout.Button("Clear", GUILayout.Width(45)))
                        {
                            cInfo.dialogue = "";
                        }
                        GUILayout.EndHorizontal();
                        // anime clip info and progress control
                        if (cInfo.dumpAsIndex != 0)
                        {
                            GUILayout.BeginHorizontal();
                            GUILayout.Label("Anime Duration:", GUILayout.Width(110));
                            if (GUILayout.Button("-1s", GUILayout.Width(50)))
                            {
                                cInfo.animeDuration -= 1;
                            }
                            if (GUILayout.Button("-0.1s", GUILayout.Width(50)))
                            {
                                cInfo.animeDuration -= 0.1;
                            }
                            if (cInfo.animeDuration < 0)
                            {
                                cInfo.animeDuration = 0;
                            }
                            durStr = GUILayout.TextField(cInfo.animeDuration.ToString(), 10, GUILayout.Width(50));
                            try
                            {
                                cInfo.animeDuration = float(durStr);
                            }
                            catch (ValueError)
                            {
                                Console.WriteLine(String.Format("invalid input in 'anime duration': %s, float expected.", durStr));
                            }
                            if (GUILayout.Button("+0.1s", GUILayout.Width(50)))
                            {
                                cInfo.animeDuration += 0.1;
                            }
                            if (GUILayout.Button("+1s", GUILayout.Width(50)))
                            {
                                cInfo.animeDuration += 1;
                            }
                            var subAnimeSet = GUILayout.Toggle(cInfo.dumpAsIndex == 2, "sub-anime");
                            cInfo.dumpAsIndex = subAnimeSet ? 2 : 1;
                            GUILayout.EndHorizontal();
                            GUILayout.BeginHorizontal();
                            GUILayout.Label("Anime Style:", GUILayout.Width(110));
                            cInfo.animeStyle = GUILayout.SelectionGrid(cInfo.animeStyle, aniStyleTexts, 7);
                            GUILayout.EndHorizontal();
                            GUILayout.BeginHorizontal();
                            var animeBigStep = cInfo.animeDuration / 10;
                            var animeLittleStep = cInfo.animeDuration / 100;
                            if (animeStopped && _sh.animeTime > cInfo.animeDuration)
                            {
                                _sh.animeTime = cInfo.animeDuration;
                            }
                            var animeTimePrev = float(_sh.animeTime);
                            if (GUILayout.Button("<<", GUILayout.Width(25)) && animeStopped)
                            {
                                if (_sh.animeTime > animeBigStep)
                                {
                                    _sh.animeTime -= animeBigStep;
                                }
                                else
                                {
                                    _sh.animeTime = 0;
                                }
                            }
                            if (GUILayout.Button("<", GUILayout.Width(20)) && animeStopped)
                            {
                                if (_sh.animeTime > animeLittleStep)
                                {
                                    _sh.animeTime -= animeLittleStep;
                                }
                                else
                                {
                                    _sh.animeTime = 0;
                                }
                            }
                            if (animeStopped)
                            {
                                _sh.animeTime = GUILayout.HorizontalSlider(_sh.animeTime, 0, cInfo.animeDuration);
                            }
                            else
                            {
                                if (_sh.slowMotion)
                                {
                                    aniDur = cInfo.animeDuration * _sh.slowMotionRate;
                                }
                                else
                                {
                                    aniDur = cInfo.animeDuration;
                                }
                                GUILayout.HorizontalSlider(_sh.animeTime, 0, aniDur);
                            }
                            if (GUILayout.Button(">", GUILayout.Width(20)) && animeStopped)
                            {
                                if (_sh.animeTime < cInfo.animeDuration - animeLittleStep)
                                {
                                    _sh.animeTime += animeLittleStep;
                                }
                                else
                                {
                                    _sh.animeTime = cInfo.animeDuration;
                                }
                            }
                            if (GUILayout.Button(">>", GUILayout.Width(25)) && animeStopped)
                            {
                                if (_sh.animeTime < cInfo.animeDuration - animeBigStep)
                                {
                                    _sh.animeTime += animeBigStep;
                                }
                                else
                                {
                                    _sh.animeTime = cInfo.animeDuration;
                                }
                            }
                            if (animeStopped && (animeTimePrev - _sh.animeTime > 0.001 || animeTimePrev - _sh.animeTime < -0.001))
                            {
                                //print "anime time from", animeTimePrev, "to", _sh.animeTime
                                _sh.play_anime_clip("toposition");
                            }
                            GUILayout.EndHorizontal();
                        }
                        else
                        {
                            GUILayout.BeginHorizontal();
                            GUILayout.Label("*This clip has no scene anime script");
                            GUILayout.EndHorizontal();
                        }
                    }
                    else
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("*No anime clip in buffer, use Script Builder first");
                        GUILayout.EndHorizontal();
                    }
                    // anime clip edit control
                    if (_sh.animeBufferIndex > 0)
                    {
                        GUILayout.BeginHorizontal();
                        if (GUILayout.Button("Update", GUILayout.Width(fullw / 3 - 2)))
                        {
                            if (_sh.masterMode)
                            {
                                _sh.update_clip();
                            }
                            else
                            {
                                scriptHelperGUIMessage(String.Format("Update clip #%d to current scene status. And update next clip (if existed) to start from current scene. Click [Update] button to update with clip's current anime/camera settings. Or if you want new camera/scene settings, set in <b>Script Builder</b> and then click [Rebuild] button. Next clip (if existed) always be updated with its own camera/scene setting.", _sh.animeBufferIndex), (("Update", _sh.update_clip, true), ("Rebuild", _sh.update_clip, false), "Cancel"));
                            }
                        }
                        if (GUILayout.Button("Insert", GUILayout.Width(fullw / 3 - 2)))
                        {
                            if (_sh.masterMode)
                            {
                                _sh.insert_clip();
                            }
                            else
                            {
                                scriptHelperGUIMessage(String.Format("Insert a clip at #%d, move current clip to next and update it to start from current status. Click [Insert] button to insert clip with current clip's anime/carema settings. Or you can set new camera/scene settings in <b>Script Builder</b>, and  then click [Build & Insert].", _sh.animeBufferIndex), (("Insert", _sh.insert_clip, true), ("Build & Insert", _sh.insert_clip, false), "Cancel"));
                            }
                        }
                        if (GUILayout.Button("Delete", GUILayout.Width(fullw / 3 - 2)))
                        {
                            if (_sh.masterMode)
                            {
                                _sh.delete_clip();
                            }
                            else
                            {
                                scriptHelperGUIMessage("Use [Delete] button to delete current clip, and next clip (if existed) will be update to start from previous end state. Or [Roll back] button to delete not only current scene but also following clips, and set previous end state as the reference state.", (("Delete", _sh.delete_clip, false), ("Roll back", _sh.delete_clip, true), "Cancel"));
                            }
                        }
                        GUILayout.EndHorizontal();
                    }
                    // advanced control
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button("Preview", GUILayout.Width(fullw / 3 - 2)))
                    {
                        if (_sh.animeBufferIndex > 0)
                        {
                            if (_sh.masterMode)
                            {
                                _sh.preview_anime_in_game(_sh.animeBufferIndex);
                            }
                            else
                            {
                                scriptHelperGUIMessage("Preview the anime in game. Choice preview from beginning, or from current script clip.", (("Preview All", _sh.preview_anime_in_game), (String.Format("Preview From #%d", _sh.animeBufferIndex), _sh.preview_anime_in_game, _sh.animeBufferIndex), "Cancel"));
                            }
                        }
                        else
                        {
                            scriptHelperGUIMessage("No anime clip in buffer to preview, use Script Builder first.");
                        }
                    }
                    if (GUILayout.Button("Ref to current", GUILayout.Width(fullw / 3 - 2)))
                    {
                        if (_sh.masterMode)
                        {
                            _sh.set_ref_scene();
                            scriptHelperGUIMessage("Reference set to current scene.");
                        }
                        else
                        {
                            scriptHelperGUIMessage("Set reference to current scene, so next builded script will take diff from current status. Reference status will not effect existed anime clips.", (("Set as Ref", _sh.set_ref_scene), "Cancel"));
                        }
                    }
                    if (GUILayout.Button("Rescan & Reset", GUILayout.Width(fullw / 3 - 2)))
                    {
                        if (_sh.masterMode)
                        {
                            _sh.init_anime_buffer();
                            scriptHelperGUIMessage("All TAGs rescaned and anime buffer resetted.");
                        }
                        else
                        {
                            scriptHelperGUIMessage("Rescan the TAGs in the scene and reset anime buffer, set reference at the same time. Use this function when you want to start from beginning. All unsaved work lost!", (("Rescan & Reset", _sh.init_anime_buffer), "Cancel"));
                        }
                    }
                    GUILayout.EndHorizontal();
                    // Tail button
                    GUILayout.FlexibleSpace();
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button("Script Builder", customButton, GUILayout.Width(100), GUILayout.Height(24)))
                    {
                        scriptHelperGUIToSceen(0);
                    }
                    if (GUILayout.Button(String.Format("<color=#ff6666ff>%s</color>", _sh.asTemplate && _sh.asEnable ? "Build Anime" : "Dump Anime"), customButton, GUILayout.Width(100), GUILayout.Height(24)))
                    {
                        if (_sh.animeBuffer.Count > 1)
                        {
                            msg = _sh.build_anime();
                            scriptHelperGUIMessage(msg);
                        }
                        else
                        {
                            scriptHelperGUIMessage("No anime clip in buffer to dump, use script builder first.");
                        }
                    }
                    if (GUILayout.Button("Clip Manager", customButton, GUILayout.Width(100), GUILayout.Height(24)))
                    {
                        scriptHelperGUIToSceen(2);
                    }
                    if (GUILayout.Button("Scene Helper", customButton, GUILayout.Width(100), GUILayout.Height(24)))
                    {
                        scriptHelperGUIToSceen(3);
                    }
                    if (GUILayout.Button("Back", customButton, GUILayout.Width(50), GUILayout.Height(24)))
                    {
                        scriptHelperGUIClose();
                    }
                    GUILayout.EndHorizontal();
                }
                else if (_sh.guiScreenIndex == 2)
                {
                    // Screen No.2: Key Frame Animation Clip Manager
                    kfam_GUI(_sh);
                }
                else if (_sh.guiScreenIndex == 3)
                {
                    // screen No.3: Scene Helper
                    // scene file
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Scene File: (*for non-vnframe game, scene info may be wrong!)");
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("PNG:", GUILayout.Width(40));
                    var oldPngPathName = _sh.game.sceneDir + _sh.game.scenePNG;
                    var newPngPathName = GUILayout.TextField(oldPngPathName, GUILayout.Width(120));
                    if (oldPngPathName != newPngPathName)
                    {
                        var pathname = path.split(newPngPathName);
                        if (pathname[0] != "")
                        {
                            _sh.game.sceneDir = pathname[0] + "\\";
                        }
                        else
                        {
                            _sh.game.sceneDir = "";
                        }
                        _sh.game.scenePNG = pathname[1];
                    }
                    if (GUILayout.Button("Create TAG", GUILayout.Width(100)))
                    {
                        _sh.tag_select();
                    }
                    if (_sh.game.scenePNG.strip().Count > 0 && GUILayout.Button("Reload Scene", GUILayout.Width(100)))
                    {
                        if (_sh.masterMode)
                        {
                            _sh.reload_scene();
                        }
                        else
                        {
                            scriptHelperGUIMessage(String.Format("Reload png file '%s' will revert all unsaved changes and reset anime buffer. Are you sure?", newPngPathName), (("Reload", _sh.reload_scene), "Cancel"));
                        }
                    }
                    if (_sh.game.scenePNG.strip().Count > 0 && GUILayout.Button("Save Scene", GUILayout.Width(100)))
                    {
                        if (_sh.masterMode)
                        {
                            _sh.save_scene();
                        }
                        else
                        {
                            scriptHelperGUIMessage("Save current scene and overwrite old scene file?\n(Old scene will be backuped to .bak file)", (("Save", _sh.save_scene), "Cancel"));
                        }
                    }
                    GUILayout.EndHorizontal();
                    // python file
                    GUILayout.BeginHorizontal();
                    if (_sh.pythonContent.Count > 0)
                    {
                        if (_sh.asTemplate)
                        {
                            pyInfo = "[Loaded][AutoScript]";
                        }
                        else
                        {
                            pyInfo = "[Loaded]";
                        }
                    }
                    else
                    {
                        pyInfo = "[N/A]";
                    }
                    GUILayout.Label("Python script: " + pyInfo);
                    if (_sh.asTemplate)
                    {
                        _sh.asEnable = GUILayout.Toggle(_sh.asEnable, "Enable Auto Script", GUILayout.Width(130));
                    }
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("PY:", GUILayout.Width(40));
                    _sh.game.current_game = GUILayout.TextField(_sh.game.current_game, GUILayout.Width(120));
                    if (GUILayout.Button("New Python", GUILayout.Width(100)))
                    {
                        _sh.asInfo.pythonName = _sh.game.current_game;
                        _sh.asInfo.sceneDir = _sh.game.sceneDir;
                        _sh.asInfo.scenePNG = _sh.game.scenePNG;
                        scriptHelperGUIToSceen(20);
                    }
                    if (_sh.game.current_game.Count > 0 && GUILayout.Button(_sh.pythonContent == "" ? " Load Python " : "Revert Python", GUILayout.Width(100)))
                    {
                        if (_sh.masterMode || _sh.pythonContent == "")
                        {
                            _sh.load_python();
                        }
                        else
                        {
                            scriptHelperGUIMessage("Revert python script to last saved status?\nUnsaved auto script or localization works will be lost!", (("Revert", _sh.load_python), "Cancel"));
                        }
                    }
                    if (_sh.game.current_game.Count > 0 && _sh.pythonContent != "" && GUILayout.Button("Save Python", GUILayout.Width(100)))
                    {
                        if (_sh.masterMode)
                        {
                            _sh.save_python();
                        }
                        else
                        {
                            scriptHelperGUIMessage("Save current python script and overwrite old python file?\n(Old python will be backuped to .bak file)", (("Save", _sh.save_python), "Cancel"));
                        }
                    }
                    GUILayout.EndHorizontal();
                    // localization
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Utilities:");
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button("String Dictionary", GUILayout.Width(fullw / 3 - 2)))
                    {
                        _sh.sd_init();
                        scriptHelperGUIToSceen(10);
                    }
                    if (GUILayout.Button("Localize Helper", GUILayout.Width(fullw / 3 - 2)))
                    {
                        _sh.lh_init();
                        scriptHelperGUIToSceen(11);
                    }
                    if (GUILayout.Button("Couple Helper", GUILayout.Width(fullw / 3 - 2)))
                    {
                        _sh.ch_init();
                        scriptHelperGUIToSceen(12);
                    }
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button("VNActor Setting", GUILayout.Width(fullw / 3 - 2)))
                    {
                        scriptHelperGUIToSceen(14);
                    }
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                    _sh.createLocalizeStringOnBuild = GUILayout.Toggle(_sh.createLocalizeStringOnBuild, "Create localize string on script clip building.");
                    GUILayout.EndHorizontal();
                    // Tail button
                    GUILayout.FlexibleSpace();
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button("Script Builder", customButton, GUILayout.Width(100), GUILayout.Height(24)))
                    {
                        scriptHelperGUIToSceen(0);
                    }
                    if (GUILayout.Button("Anime Buffer", customButton, GUILayout.Width(100), GUILayout.Height(24)))
                    {
                        scriptHelperGUIToSceen(1);
                    }
                    if (GUILayout.Button("Clip Manager", customButton, GUILayout.Width(100), GUILayout.Height(24)))
                    {
                        scriptHelperGUIToSceen(2);
                    }
                    if (GUILayout.Button("<color=#ff6666ff>Reset</color>", customButton, GUILayout.Width(100), GUILayout.Height(24)))
                    {
                        scriptHelperGUIMessage("Reset the keyframe clips, all unsaved work lost!", (("Reset keyframe clips", init_keyframe_anime, _sh.game), "Cancel"));
                    }
                    if (GUILayout.Button("Back", customButton, GUILayout.Width(50), GUILayout.Height(24)))
                    {
                        scriptHelperGUIClose();
                    }
                    GUILayout.EndHorizontal();
                }
                else if (_sh.guiScreenIndex == 10)
                {
                    // Screen No.10: string dictionary
                    // search
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Search:", GUILayout.Width(50));
                    var prevSearch = _sh.sdSearchKeyword;
                    _sh.sdSearchKeyword = GUILayout.TextField(_sh.sdSearchKeyword, GUILayout.Width(60));
                    if (GUILayout.Button("Clear", GUILayout.Width(40)))
                    {
                        _sh.sdSearchKeyword = "";
                    }
                    if (_sh.sdSearchKeyword != prevSearch)
                    {
                        _sh.sd_search();
                    }
                    GUILayout.Label("Found:", GUILayout.Width(50));
                    if (_sh.sdSearchResult.Count > 0 && GUILayout.Button("<", GUILayout.Width(20)))
                    {
                        if (_sh.sdIndex == 0)
                        {
                            _sh.sdIndex = _sh.sdSearchResult.Count - 1;
                        }
                        else
                        {
                            _sh.sdIndex -= 1;
                        }
                    }
                    GUILayout.Label(String.Format("%d/%d", _sh.sdIndex + 1, _sh.sdSearchResult.Count), GUILayout.Width(50));
                    if (_sh.sdSearchResult.Count > 0 && GUILayout.Button(">", GUILayout.Width(20)))
                    {
                        if (_sh.sdIndex == _sh.sdSearchResult.Count - 1)
                        {
                            _sh.sdIndex = 0;
                        }
                        else
                        {
                            _sh.sdIndex += 1;
                        }
                    }
                    if (_sh.sdIndex >= 0)
                    {
                        GUILayout.Label(String.Format("ID = %d", _sh.sdSearchResult[_sh.sdIndex]));
                        if (_sh.sd_is_modified(_sh.sdSearchResult[_sh.sdIndex]) && GUILayout.Button("Apply"))
                        {
                            resMsg = _sh.sd_apply(_sh.sdSearchResult[_sh.sdIndex]);
                            if (_sh.masterMode)
                            {
                                scriptHelperGUIMessage(resMsg);
                            }
                            else
                            {
                                scriptHelperGUIMessage(resMsg, ValueTuple.Create("OK"));
                            }
                        }
                    }
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                    if (_sh.sdIndex >= 0)
                    {
                        prevTexts = _sh.game.scenedata.strings[_sh.sdSearchResult[_sh.sdIndex]];
                    }
                    else
                    {
                        prevTexts = "";
                    }
                    var inputTexts = GUILayout.TextArea(prevTexts, GUILayout.Width(fullw));
                    if (prevTexts != inputTexts && _sh.sdIndex >= 0)
                    {
                        _sh.game.scenedata.strings[_sh.sdSearchResult[_sh.sdIndex]] = inputTexts;
                        if (!_sh.sd_is_modified(_sh.sdSearchResult[_sh.sdIndex]))
                        {
                            _sh.sdModifiedList.append(_sh.sdSearchResult[_sh.sdIndex]);
                        }
                    }
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(String.Format("Totally %d strings in dictionary, %d modified.", _sh.game.scenedata.strings.Count, _sh.sdModifiedList.Count));
                    GUILayout.EndHorizontal();
                    // Tail button
                    GUILayout.FlexibleSpace();
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button("New", customButton, GUILayout.Width(fullw / 4 - 2), GUILayout.Height(24)))
                    {
                        _sh.sd_new(_sh.sdSearchKeyword);
                    }
                    if (GUILayout.Button("Apply All", customButton, GUILayout.Width(fullw / 4 - 2), GUILayout.Height(24)))
                    {
                        resMsg = _sh.sd_apply();
                        if (_sh.masterMode)
                        {
                            scriptHelperGUIMessage(resMsg);
                        }
                        else
                        {
                            scriptHelperGUIMessage(resMsg, ValueTuple.Create("OK"));
                        }
                    }
                    if (GUILayout.Button("Revert All", customButton, GUILayout.Width(fullw / 4 - 2), GUILayout.Height(24)))
                    {
                        if (_sh.masterMode)
                        {
                            _sh.sd_revert();
                        }
                        else
                        {
                            scriptHelperGUIMessage("Revert to last applied status? All non-applied changes will be lost.", (("Revert", _sh.sd_revert), "Cancel"));
                        }
                    }
                    if (GUILayout.Button("Back", customButton, GUILayout.Width(fullw / 4 - 2), GUILayout.Height(24)))
                    {
                        scriptHelperGUIToSceen(3);
                    }
                    GUILayout.EndHorizontal();
                }
                else if (_sh.guiScreenIndex == 11)
                {
                    // Screen No.11: localize helper
                    // Org
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(String.Format("Original texts: %s", _sh.lhOrgTextType));
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                    if (_sh.game.vnText != _sh.lhPrevVnText)
                    {
                        _sh.lhOrgInput = _sh.game.vnText;
                        _sh.lhTgtInput = "";
                        _sh.lh_check_orgInput();
                    }
                    var usrOrgInput = GUILayout.TextArea(_sh.lhOrgInput, GUILayout.Width(fullw));
                    if (usrOrgInput != _sh.lhOrgInput)
                    {
                        _sh.lhOrgInput = usrOrgInput;
                        _sh.lh_check_orgInput();
                    }
                    GUILayout.EndHorizontal();
                    // Tgt
                    if (_sh.lhOrgTextType[1::10] == "PlainText" || _sh.lhOrgTextType[::5] == "ltext")
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Select ID:", GUILayout.Width(70));
                        if (GUILayout.Button("<", GUILayout.Width(20)))
                        {
                            _sh.lh_set_id(_sh.lhTgtID, -1);
                        }
                        var usrTgtIndex = GUILayout.TextField(_sh.lhTgtID, GUILayout.Width(50));
                        if (usrTgtIndex != _sh.lhTgtID)
                        {
                            _sh.lh_set_id(usrTgtIndex);
                        }
                        if (GUILayout.Button(">", GUILayout.Width(20)))
                        {
                            _sh.lh_set_id(_sh.lhTgtID, +1);
                        }
                        if (GUILayout.Button("New", GUILayout.Width(50)))
                        {
                            _sh.lh_set_id(-1);
                        }
                        if (_sh.lh_is_modified(_sh.lhTgtID) && GUILayout.Button("Apply", GUILayout.Width(50)))
                        {
                            resMsg = _sh.lh_apply(_sh.lhTgtID);
                            if (_sh.masterMode)
                            {
                                scriptHelperGUIMessage(resMsg);
                            }
                            else
                            {
                                scriptHelperGUIMessage(resMsg, ValueTuple.Create("OK"));
                            }
                        }
                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal();
                        var usrTgtInput = GUILayout.TextArea(_sh.lhTgtInput, GUILayout.Width(fullw));
                        if (usrTgtInput != _sh.lhTgtInput)
                        {
                            _sh.lhTgtInput = usrTgtInput;
                            _sh.lh_update_tgtInput();
                        }
                        GUILayout.EndHorizontal();
                    }
                    else
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Unable to localize unknown string. Try input the plain text manually.");
                        GUILayout.EndHorizontal();
                    }
                    // Tail button
                    GUILayout.FlexibleSpace();
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button("Apply All", customButton, GUILayout.Width(fullw / 4 - 2), GUILayout.Height(24)))
                    {
                        resMsg = _sh.lh_apply(-1);
                        if (_sh.masterMode)
                        {
                            scriptHelperGUIMessage(resMsg);
                        }
                        else
                        {
                            scriptHelperGUIMessage(resMsg, ValueTuple.Create("OK"));
                        }
                    }
                    if (GUILayout.Button("Revert All", customButton, GUILayout.Width(fullw / 4 - 2), GUILayout.Height(24)))
                    {
                        if (_sh.masterMode)
                        {
                            _sh.lh_revert();
                        }
                        else
                        {
                            scriptHelperGUIMessage("Revert to last applied status? All non-applied changes will be lost.", (("Revert", _sh.lh_revert), "Cancel"));
                        }
                    }
                    if (GUILayout.Button("Back", customButton, GUILayout.Width(fullw / 4 - 2), GUILayout.Height(24)))
                    {
                        scriptHelperGUIToSceen(3);
                    }
                    if (GUILayout.Button("Back to Scene", customButton, GUILayout.Width(fullw / 4 - 2), GUILayout.Height(24)))
                    {
                        scriptHelperGUIClose();
                    }
                    GUILayout.EndHorizontal();
                }
                else if (_sh.guiScreenIndex == 12)
                {
                    // Screen No.12: Couple helper
                    // Actor select
                    GUILayout.BeginHorizontal();
                    if (!_sh.ch_set_ext_actor)
                    {
                        GUILayout.Label("Base Actor:", GUILayout.Width(70));
                        if (GUILayout.Button(_sh.ch_actor_name(0), GUILayout.Width(150)))
                        {
                            _sh.ch_select_actor(0);
                        }
                        if (_sh.ch_partner_actor_sex.Count > 0)
                        {
                            GUILayout.Label("Partner:", GUILayout.Width(50));
                            if (GUILayout.Button(_sh.ch_actor_name(1), GUILayout.Width(150)))
                            {
                                _sh.ch_select_actor(1);
                            }
                        }
                        if (_sh.ch_partner_actor_sex.Count > 1)
                        {
                            if (GUILayout.Button(">>", GUILayout.Width(30)))
                            {
                                _sh.ch_set_ext_actor = true;
                            }
                        }
                    }
                    else
                    {
                        if (GUILayout.Button("<<", GUILayout.Width(30)))
                        {
                            _sh.ch_set_ext_actor = false;
                        }
                        foreach (var i in Enumerable.Range(0, _sh.ch_partner_actor_sex.Count - 1))
                        {
                            GUILayout.Label(String.Format("Ext%d:", i + 1), GUILayout.Width(40));
                            if (GUILayout.Button(_sh.ch_actor_name(i + 2), GUILayout.Width(100)))
                            {
                                _sh.ch_select_actor(i + 2);
                            }
                        }
                    }
                    GUILayout.EndHorizontal();
                    // Speed and pattern
                    GUILayout.BeginHorizontal();
                    if (_sh.ch_base_speed != null && _sh.ch_base_pattern != null)
                    {
                        // Spd
                        GUILayout.Label("Spd:", GUILayout.Width(30));
                        var newSpd = GUILayout.HorizontalSlider(_sh.ch_base_speed, 0, 3, GUILayout.Width(95));
                        var newSpdTxt = GUILayout.TextField(String.Format("%.2f", newSpd), GUILayout.Width(30));
                        try
                        {
                            newSpd = float(newSpdTxt);
                            if (newSpd != _sh.ch_base_speed)
                            {
                                _sh.ch_update_speed(newSpd);
                            }
                        }
                        catch
                        {
                        }
                        if (_sh.ch_base_pattern is Vector2)
                        {
                            GUILayout.Label("Ptn:", GUILayout.Width(30));
                            var newPtn1 = GUILayout.HorizontalSlider(_sh.ch_base_pattern.x, -1, 1, GUILayout.Width(95));
                            var newPtnTxt1 = GUILayout.TextField(String.Format("%.2f", newPtn1), GUILayout.Width(30));
                            var newPtn2 = GUILayout.HorizontalSlider(_sh.ch_base_pattern.y, -1, 1, GUILayout.Width(95));
                            var newPtnTxt2 = GUILayout.TextField(String.Format("%.2f", newPtn2), GUILayout.Width(30));
                            try
                            {
                                newPtn = Vector2(float(newPtn1), float(newPtn2));
                                if (newPtn != _sh.ch_base_pattern)
                                {
                                    _sh.ch_update_pattern(newPtn);
                                }
                            }
                            catch
                            {
                            }
                        }
                        else
                        {
                            GUILayout.Label("Ptn:", GUILayout.Width(30));
                            newPtn = GUILayout.HorizontalSlider(_sh.ch_base_pattern, -1, 1, GUILayout.Width(95));
                            var newPtnTxt = GUILayout.TextField(String.Format("%.2f", newPtn), GUILayout.Width(30));
                            try
                            {
                                newPtn = float(newPtnTxt);
                                if (newPtn != _sh.ch_base_pattern)
                                {
                                    _sh.ch_update_pattern(newPtn);
                                }
                            }
                            catch
                            {
                            }
                        }
                    }
                    else
                    {
                        GUILayout.Label("*Select an actor first");
                    }
                    GUILayout.EndHorizontal();
                    // Motion select
                    GUILayout.BeginHorizontal();
                    _sh.ch_gp_sclpos = GUILayout.BeginScrollView(_sh.ch_gp_sclpos, GUILayout.Width(100), GUILayout.Height(150));
                    var newGroup = GUILayout.SelectionGrid(_sh.ch_sel_group, Array[String](_sh.ch_group_texts), 1);
                    if (newGroup != _sh.ch_sel_group)
                    {
                        _sh.ch_change_group(newGroup);
                    }
                    GUILayout.EndScrollView();
                    _sh.ch_ct_sclpos = GUILayout.BeginScrollView(_sh.ch_ct_sclpos, GUILayout.Width(190), GUILayout.Height(150));
                    var newCat = GUILayout.SelectionGrid(_sh.ch_sel_category, Array[String](_sh.ch_category_texts), 1);
                    if (newCat != _sh.ch_sel_category)
                    {
                        _sh.ch_change_category(newCat);
                    }
                    GUILayout.EndScrollView();
                    _sh.ch_no_sclpos = GUILayout.BeginScrollView(_sh.ch_no_sclpos, GUILayout.Width(190), GUILayout.Height(150));
                    var newNo = GUILayout.SelectionGrid(_sh.ch_sel_no, Array[String](_sh.ch_no_texts), 1);
                    if (newNo != _sh.ch_sel_no)
                    {
                        _sh.ch_sel_no = newNo;
                    }
                    GUILayout.EndScrollView();
                    GUILayout.EndHorizontal();
                    // Tail button
                    GUILayout.FlexibleSpace();
                    GUILayout.BeginHorizontal();
                    if (_sh.ch_base_actor == null || _sh.ch_partner_actor == null)
                    {
                        if (GUILayout.Button("Help", customButton, GUILayout.Width(fullw / 4 - 2), GUILayout.Height(24)))
                        {
                            scriptHelperGUIMessage("Set base actor and partner to make them couple! Partner char will be move to base char. And anime speed, anime pattern will be synchronized too.\nSelect a charater in studio and press <b>'click to set'</b> button to set actor.", ValueTuple.Create("OK"));
                        }
                        if (GUILayout.Button("", customButton, GUILayout.Width(fullw / 4 - 2), GUILayout.Height(24)))
                        {
                        }
                        if (GUILayout.Button("", customButton, GUILayout.Width(fullw / 4 - 2), GUILayout.Height(24)))
                        {
                        }
                    }
                    else
                    {
                        if (GUILayout.Button("Start", customButton, GUILayout.Width(fullw / 4 - 2), GUILayout.Height(24)))
                        {
                            _sh.ch_start();
                        }
                        if (GUILayout.Button("Restart", customButton, GUILayout.Width(fullw / 4 - 2), GUILayout.Height(24)))
                        {
                            _sh.ch_restart();
                        }
                        if (GUILayout.Button("Adjust", customButton, GUILayout.Width(fullw / 4 - 2), GUILayout.Height(24)))
                        {
                            scriptHelperGUIToSceen(13);
                        }
                    }
                    if (GUILayout.Button("Back", customButton, GUILayout.Width(fullw / 4 - 2), GUILayout.Height(24)))
                    {
                        scriptHelperGUIToSceen(3);
                    }
                    GUILayout.EndHorizontal();
                }
                else if (_sh.guiScreenIndex == 13)
                {
                    // Screen No.13: Couple helper - Adjust
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("", GUILayout.Width(130));
                    GUILayout.Label("Param1", GUILayout.Width(170));
                    GUILayout.Label("Param2", GUILayout.Width(160));
                    GUILayout.EndHorizontal();
                    var chAllActors = new List<void> {
                    _sh.ch_base_actor,
                    _sh.ch_partner_actor
                };
                    chAllActors.extend(_sh.ch_ext_actor);
                    foreach (var i in Enumerable.Range(0, chAllActors.Count))
                    {
                        var chActor = chAllActors[i];
                        if (chActor != null && chActor.isHAnime)
                        {
                            GUILayout.BeginHorizontal();
                            GUILayout.Label(_sh.ch_actor_name(i), GUILayout.Width(120));
                            var oldParam = chActor.get_anime_option_param();
                            var newParam1 = oldParam[0];
                            if (GUILayout.Button("<", GUILayout.Width(20)))
                            {
                                if (newParam1 > 0.1)
                                {
                                    newParam1 -= 0.1;
                                }
                                else
                                {
                                    newParam1 = 0;
                                }
                            }
                            newParam1 = GUILayout.HorizontalSlider(newParam1, 0, 1, GUILayout.Width(90));
                            if (GUILayout.Button(">", GUILayout.Width(20)))
                            {
                                if (newParam1 < 0.9)
                                {
                                    newParam1 += 0.1;
                                }
                                else
                                {
                                    newParam1 = 1;
                                }
                            }
                            GUILayout.Label(String.Format("%.2f", newParam1), GUILayout.Width(30));
                            var newParam2 = oldParam[1];
                            if (GUILayout.Button("<", GUILayout.Width(20)))
                            {
                                if (newParam2 > 0.1)
                                {
                                    newParam2 -= 0.1;
                                }
                                else
                                {
                                    newParam2 = 0;
                                }
                            }
                            newParam2 = GUILayout.HorizontalSlider(newParam2, 0, 1, GUILayout.Width(90));
                            if (GUILayout.Button(">", GUILayout.Width(20)))
                            {
                                if (newParam2 < 0.9)
                                {
                                    newParam2 += 0.1;
                                }
                                else
                                {
                                    newParam2 = 1;
                                }
                            }
                            GUILayout.Label(String.Format("%.2f", newParam2), GUILayout.Width(30));
                            if (newParam1 != oldParam[0] || newParam2 != oldParam[1])
                            {
                                chActor.set_anime_option_param((newParam1, newParam2));
                            }
                            GUILayout.EndHorizontal();
                        }
                    }
                    GUILayout.BeginHorizontal();
                    var allOption = _sh.ch_get_overall_anime_option_visible();
                    var newAllOption = GUILayout.Toggle(allOption, "Anime option item visible");
                    if (allOption != newAllOption)
                    {
                        _sh.ch_set_overall_anime_option_visible(newAllOption);
                    }
                    var allShoes = _sh.ch_get_overall_shoes();
                    var newAllShoes = GUILayout.Toggle(allShoes, "Wear shoes");
                    if (allShoes != newAllShoes)
                    {
                        _sh.ch_set_overall_shoes(newAllShoes);
                    }
                    GUILayout.EndHorizontal();
                    // Tail button
                    GUILayout.FlexibleSpace();
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button("Help", customButton, GUILayout.Width(fullw / 4 - 2), GUILayout.Height(24)))
                    {
                        scriptHelperGUIMessage("Adjust anime option param to fix anime position mismatch.\nParam1 almost about height, Param2 almost about breast.\nIf the female wears high-heel, y position may be offseted. You need to take off it or adjust y position by youself.", ValueTuple.Create("OK"));
                    }
                    if (GUILayout.Button("", customButton, GUILayout.Width(fullw / 4 - 2), GUILayout.Height(24)))
                    {
                    }
                    if (GUILayout.Button("", customButton, GUILayout.Width(fullw / 4 - 2), GUILayout.Height(24)))
                    {
                    }
                    if (GUILayout.Button("Back", customButton, GUILayout.Width(fullw / 4 - 2), GUILayout.Height(24)))
                    {
                        scriptHelperGUIToSceen(12);
                    }
                    GUILayout.EndHorizontal();
                }
                else if (_sh.guiScreenIndex == 14)
                {
                    // Screen No.14: VNActor Setting
                    // option list
                    GUILayout.Label("Check the extend data you want to export with VNActor:");
                    if (!hasattr(_sh, "guiScrollPos14"))
                    {
                        _sh.guiScrollPos14 = Vector2.zero;
                    }
                    _sh.guiScrollPos14 = GUILayout.BeginScrollView(_sh.guiScrollPos14, GUILayout.Height(170));
                    foreach (var optname in get_ini_options())
                    {
                        var optdesp = get_ini_exportOptionDesp(optname);
                        if (optdesp == null)
                        {
                            optdesp = "<color=#00ff00>" + optname + "</color>";
                        }
                        else
                        {
                            optdesp = "<color=#00ff00>" + optname + "</color>: " + optdesp;
                        }
                        var optold = is_ini_value_true(optname);
                        var optnew = GUILayout.Toggle(optold, optdesp);
                        if (optold != optnew)
                        {
                            set_ini_value(optname, optnew);
                        }
                    }
                    GUILayout.EndScrollView();
                    // Tail button
                    GUILayout.FlexibleSpace();
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button("OK", GUILayout.Width(fullw / 3 - 2), GUILayout.Height(24)))
                    {
                        scriptHelperGUIToSceen(3);
                    }
                    if (GUILayout.Button("Readme", GUILayout.Width(fullw / 3 - 2), GUILayout.Height(24)))
                    {
                        msg = "VNActor extend export settings, which are wrtten in vnactor.ini file, can be modified here in run time. These settings affect the behavior of other components that depends on vnactor (such as VNFrame, VNAnime and SceneSaveState).\nTo keep you setting permanently, please edit them in vnactor.ini under <color=#00ff00>[" + get_engine_id() + "]</color> category and then click <Reload>.";
                        scriptHelperGUIMessage(msg, ValueTuple.Create("OK"));
                    }
                    if (GUILayout.Button("Reload", GUILayout.Width(fullw / 3 - 2), GUILayout.Height(24)))
                    {
                        load_ini_file(true);
                    }
                    GUILayout.EndHorizontal();
                }
                else if (_sh.guiScreenIndex == 20)
                {
                    // Screen No.20: New auto script wizard, basic setting
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Setup a new auto script game:");
                    GUILayout.EndHorizontal();
                    // python info
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Game name:", GUILayout.Width(100));
                    _sh.asInfo.gameName = GUILayout.TextField(_sh.asInfo.gameName, GUILayout.Width(120));
                    GUILayout.Label("Python file:", GUILayout.Width(100));
                    _sh.asInfo.pythonName = GUILayout.TextField(_sh.asInfo.pythonName, GUILayout.Width(120));
                    GUILayout.EndHorizontal();
                    // Scene PNG info
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Scene folder:", GUILayout.Width(100));
                    _sh.asInfo.sceneDir = GUILayout.TextField(_sh.asInfo.sceneDir, GUILayout.Width(120));
                    GUILayout.Label("PNG file:", GUILayout.Width(100));
                    _sh.asInfo.scenePNG = GUILayout.TextField(_sh.asInfo.scenePNG, GUILayout.Width(120));
                    GUILayout.EndHorizontal();
                    // Settings
                    GUILayout.BeginHorizontal();
                    _sh.asInfo.enableReload = GUILayout.Toggle(_sh.asInfo.enableReload, "Enable reload button at end of the game.");
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                    _sh.asInfo.enableQuickReload = GUILayout.Toggle(_sh.asInfo.enableQuickReload, "Enable quick reload (Scene sould be initialized by script).");
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                    _sh.asInfo.alwaysHideWindowInCameraAnime = GUILayout.Toggle(_sh.asInfo.alwaysHideWindowInCameraAnime, "Always hide window when camera anime is playing.");
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                    _sh.asInfo.alwaysLockWindowInSceneAnime = GUILayout.Toggle(_sh.asInfo.alwaysLockWindowInSceneAnime, "Always hide buttons when scene anime is playing.");
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                    _sh.asInfo.createLocalizeString = GUILayout.Toggle(_sh.asInfo.createLocalizeString, "Create localize string in scene file.");
                    GUILayout.EndHorizontal();
                    //GUILayout.BeginHorizontal()
                    //_sh.asInfo.masterMode = GUILayout.Toggle(_sh.asInfo.masterMode, "I have mastered the ScriptHelper, so no prompt message needed!")
                    //GUILayout.EndHorizontal()
                    // Tail button
                    GUILayout.FlexibleSpace();
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button("Create New!", GUILayout.Width(fullw / 3 - 2), GUILayout.Height(24)))
                    {
                        msg = _sh.as_create_new();
                        scriptHelperGUIToSceen(3);
                        scriptHelperGUIMessage(msg, ValueTuple.Create("OK"));
                    }
                    if (GUILayout.Button("More settings", GUILayout.Width(fullw / 3 - 2), GUILayout.Height(24)))
                    {
                        scriptHelperGUIToSceen(21);
                    }
                    if (GUILayout.Button("Cancel", GUILayout.Width(fullw / 3 - 2), GUILayout.Height(24)))
                    {
                        scriptHelperGUIToSceen(3);
                    }
                    GUILayout.EndHorizontal();
                }
                else if (_sh.guiScreenIndex == 21)
                {
                    // Screen No.21: New auto script wizard, more setting
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Setup a new auto script game: - More Settings -");
                    GUILayout.EndHorizontal();
                    // settings
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("UI Skin:", GUILayout.Width(60));
                    var skinVersionTexts = Array[String](("skin_default", "skin_renpy"));
                    var skinVerIndex = Array.IndexOf(skinVersionTexts, _sh.asInfo.skinVersion);
                    skinVerIndex = GUILayout.SelectionGrid(skinVerIndex, skinVersionTexts, skinVersionTexts.Count);
                    _sh.asInfo.skinVersion = skinVersionTexts[skinVerIndex];
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Lip sync:", GUILayout.Width(60));
                    _sh.asInfo.fakeLipSyncEnable = GUILayout.Toggle(_sh.asInfo.fakeLipSyncEnable, "Enable", GUILayout.Width(110));
                    GUILayout.Label("Version:", GUILayout.Width(50));
                    var flsVersionTexts = Array[String](("v10", "v11"));
                    var flsVerIndex = Array.IndexOf(flsVersionTexts, _sh.asInfo.fakeLipSyncVersion);
                    flsVerIndex = GUILayout.SelectionGrid(flsVerIndex, flsVersionTexts, flsVersionTexts.Count, GUILayout.Width(80));
                    _sh.asInfo.fakeLipSyncVersion = flsVersionTexts[flsVerIndex];
                    GUILayout.Label("Speed:", GUILayout.Width(45));
                    var rspd = GUILayout.TextField(_sh.asInfo.fakeLipSyncReadingSpeed.ToString(), GUILayout.Width(40));
                    try
                    {
                        _sh.asInfo.fakeLipSyncReadingSpeed = float(rspd);
                    }
                    catch
                    {
                    }
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Default 'next' button text:", GUILayout.Width(170));
                    _sh.asInfo.defaultNextBtnText = GUILayout.TextField(_sh.asInfo.defaultNextBtnText, GUILayout.Width(200));
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Default 'reload' button text:", GUILayout.Width(170));
                    _sh.asInfo.defaultReloadBtnText = GUILayout.TextField(_sh.asInfo.defaultReloadBtnText, GUILayout.Width(200));
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Default 'end' button text:", GUILayout.Width(170));
                    _sh.asInfo.defaultEndBtnText = GUILayout.TextField(_sh.asInfo.defaultEndBtnText, GUILayout.Width(200));
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Default end scene text:", GUILayout.Width(170));
                    _sh.asInfo.defaultEndText = GUILayout.TextField(_sh.asInfo.defaultEndText, GUILayout.Width(200));
                    GUILayout.EndHorizontal();
                    // Tail button
                    GUILayout.FlexibleSpace();
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button("Create New!", GUILayout.Width(fullw / 3 - 2), GUILayout.Height(24)))
                    {
                        msg = _sh.as_create_new();
                        scriptHelperGUIToSceen(3);
                        scriptHelperGUIMessage(msg, ValueTuple.Create("OK"));
                    }
                    if (GUILayout.Button("Basic settings", GUILayout.Width(fullw / 3 - 2), GUILayout.Height(24)))
                    {
                        scriptHelperGUIToSceen(20);
                    }
                    if (GUILayout.Button("Cancel", GUILayout.Width(fullw / 3 - 2), GUILayout.Height(24)))
                    {
                        scriptHelperGUIToSceen(3);
                    }
                    GUILayout.EndHorizontal();
                }
                else if (_sh.guiScreenIndex == 31)
                {
                    // Screen No.31: Select dump target
                    // Data
                    var sheight = 150;
                    var sd = _sh.game.scenedata;
                    if (!hasattr(sd, "localTgts"))
                    {
                        sd.dtsActorPos = Vector2.zero;
                        sd.dtsPropPos = Vector2.zero;
                        sd.dtsClipPos = Vector2.zero;
                        if (_sh.curSCInfo.dumpTgts != null)
                        {
                            sd.localTgts = copy.copy(_sh.curSCInfo.dumpTgts);
                        }
                        else
                        {
                            sd.localTgts = new List<string> {
                            "sys"
                        };
                            foreach (var aid in sd.actors.keys())
                            {
                                sd.localTgts.append(aid);
                            }
                            foreach (var pid in sd.props.keys())
                            {
                                sd.localTgts.append(pid);
                            }
                            foreach (var cid in _sh.game.gdata.kfaManagedClips.keys())
                            {
                                sd.localTgts.append(cid);
                            }
                        }
                    }
                    // Title
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Select target objects to be dump into script:");
                    GUILayout.EndHorizontal();
                    // Actors, Props, Clips
                    GUILayout.BeginHorizontal();
                    GUILayout.BeginVertical(GUILayout.Width(180), GUILayout.Height(sheight));
                    GUILayout.Label("Actors:");
                    sd.dtsActorPos = GUILayout.BeginScrollView(sd.dtsActorPos);
                    if (sd.actors.Count)
                    {
                        foreach (var aid in sd.actors.keys())
                        {
                            chk = GUILayout.Toggle(checkId(aid), String.Format("%s (%s)", sd.actors[aid].text_name, aid));
                            toggleId(aid, chk);
                        }
                    }
                    else
                    {
                        GUILayout.Label("Not found");
                    }
                    GUILayout.EndScrollView();
                    GUILayout.EndVertical();
                    GUILayout.BeginVertical(GUILayout.Width(180), GUILayout.Height(sheight));
                    GUILayout.Label("Props:");
                    sd.dtsPropPos = GUILayout.BeginScrollView(sd.dtsPropPos);
                    if (sd.props.Count)
                    {
                        foreach (var pid in sd.props.keys())
                        {
                            chk = GUILayout.Toggle(checkId(pid), String.Format("%s (%s)", sd.props[pid].text_name, pid));
                            toggleId(pid, chk);
                        }
                    }
                    else
                    {
                        GUILayout.Label("Not found");
                    }
                    GUILayout.EndScrollView();
                    GUILayout.EndVertical();
                    GUILayout.BeginVertical(GUILayout.Width(100), GUILayout.Height(sheight));
                    GUILayout.Label("Clips:");
                    sd.dtsClipPos = GUILayout.BeginScrollView(sd.dtsClipPos);
                    if (_sh.game.gdata.kfaManagedClips.Count)
                    {
                        foreach (var cid in _sh.game.gdata.kfaManagedClips.keys())
                        {
                            chk = GUILayout.Toggle(checkId(cid), String.Format("%s", cid));
                            toggleId(cid, chk);
                        }
                    }
                    else
                    {
                        GUILayout.Label("Not found");
                    }
                    GUILayout.EndScrollView();
                    GUILayout.EndVertical();
                    GUILayout.EndHorizontal();
                    // System
                    GUILayout.BeginHorizontal();
                    chk = GUILayout.Toggle(checkId("sys"), "Dump system settings. (such as map, bgm...)");
                    toggleId("sys", chk);
                    GUILayout.EndHorizontal();
                    // Tail button
                    GUILayout.FlexibleSpace();
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button(String.Format("Select (%d)", sd.localTgts.Count), GUILayout.Width(fullw / 4 - 2), GUILayout.Height(24)))
                    {
                        _sh.curSCInfo.dumpTgts = copy.copy(sd.localTgts);
                        quitTgtSelect();
                    }
                    if (GUILayout.Button("All", GUILayout.Width(fullw / 4 - 2), GUILayout.Height(24)))
                    {
                        _sh.curSCInfo.dumpTgts = null;
                        quitTgtSelect();
                    }
                    if (GUILayout.Button("None", GUILayout.Width(fullw / 4 - 2), GUILayout.Height(24)))
                    {
                        sd.localTgts = new List<object>();
                    }
                    if (GUILayout.Button("Back", GUILayout.Width(fullw / 4 - 2), GUILayout.Height(24)))
                    {
                        quitTgtSelect();
                    }
                    GUILayout.EndHorizontal();
                }
                else if (_sh.guiScreenIndex == 99)
                {
                    // Screen No.99: Message Screen
                    var style = GUIStyle("label");
                    style.richText = true;
                    style.fontSize = 18;
                    style.wordWrap = true;
                    // Message Texts
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(_sh.msgTexts, style, GUILayout.Width(fullw));
                    GUILayout.EndHorizontal();
                    // Message Params
                    if (_sh.msgParam != null && _sh.msgParam is dict)
                    {
                        foreach (var pkey in _sh.msgParam)
                        {
                            var keyName = pkey;
                            var value = _sh.msgParam[pkey][0];
                            var valueType = _sh.msgParam[pkey][1];
                            var keyWidth = _sh.msgParam[pkey][2];
                            var valueWidth = _sh.msgParam[pkey][3];
                            if (_sh.msgParam[pkey].Count > 4)
                            {
                                comment = _sh.msgParam[pkey][4];
                            }
                            else
                            {
                                comment = "";
                            }
                            if (valueType == "txt")
                            {
                                GUILayout.BeginHorizontal();
                                GUILayout.Label(keyName, GUILayout.Width(keyWidth));
                                _sh.msgParam[pkey][0] = GUILayout.TextField(_sh.msgParam[pkey][0], GUILayout.Width(valueWidth));
                                GUILayout.Label(comment);
                                GUILayout.EndHorizontal();
                            }
                            else
                            {
                                Console.WriteLine("unsupported key type:", valueType);
                            }
                        }
                    }
                    // Tail button
                    GUILayout.FlexibleSpace();
                    if (_sh.msgAction != null)
                    {
                        GUILayout.BeginHorizontal();
                        foreach (var ma in _sh.msgAction)
                        {
                            if (ma is tuple && ma.Count > 1)
                            {
                                maText = ma[0].ToString();
                                maFunc = ma[1];
                                maParam = ma.Count == 2 ? null : ma[2];
                            }
                            else
                            {
                                maText = ma.ToString();
                                maFunc = null;
                                maParam = null;
                            }
                            if (_sh.msgAction == null)
                            {
                                break;
                            }
                            if (GUILayout.Button(maText, customButton, GUILayout.Width(fullw / _sh.msgAction.Count - 2), GUILayout.Height(32)))
                            {
                                scriptHelperGUIMessageRtn(_sh.game);
                                if (maFunc != null)
                                {
                                    if (maParam != null)
                                    {
                                        maFunc(maParam);
                                    }
                                    else
                                    {
                                        maFunc();
                                    }
                                }
                            }
                        }
                        GUILayout.EndHorizontal();
                    }
                }
                else
                {
                    throw new Exception("Unexpected guiScreenIndex = " + _sh.guiScreenIndex.ToString());
                }
                GUILayout.EndVertical();
                GUI.DragWindow();
            }
            catch (Exception)
            {
                Console.WriteLine("scriptHelperWindowGUI Exception:");
                traceback.print_exc();
                scriptHelperGUIClose();
                _sh.game.show_blocking_message_time("Script helper error: " + e.ToString());
            }
        }

        public static string script2string(object script)
        {
            object ret;
            if (script is list)
            {
                ret = "[";
                foreach (var subElm in script)
                {
                    ret += script2string(subElm) + ", ";
                }
                if (ret.Count > 1)
                {
                    ret = ret[::(len(ret) - 2)];
                }
                ret += "]";
                return ret;
            }
            else if (script is tuple)
            {
                ret = "(";
                foreach (var subElm in script)
                {
                    ret += script2string(subElm) + ", ";
                }
                if (script.Count > 1)
                {
                    ret = ret[::(len(ret) - 2)];
                }
                ret += ")";
                return ret;
            }
            else if (script is dict)
            {
                ret = "{";
                foreach (var subKey in script.keys().OrderBy(_p_1 => _p_1).ToList())
                {
                    ret += script2string(subKey) + ": ";
                    ret += script2string(script[subKey]) + ", ";
                }
                if (ret.Count > 1)
                {
                    ret = ret[::(len(ret) - 2)];
                }
                ret += "}";
                return ret;
            }
            else if (script is Vector3)
            {
                //ret = "Vector3(%.3f, %.3f, %.3f)"%(script.x, script.y, script.z)
                ret = String.Format("(%.3f, %.3f, %.3f)", script.x, script.y, script.z);
                return ret;
            }
            else if (script is Vector2)
            {
                ret = String.Format("(%.3f, %.3f)", script.x, script.y);
                return ret;
            }
            else if (script is Color)
            {
                ret = String.Format("(%.2f, %.2f, %.2f, %.2f)", script.r, script.g, script.b, script.a);
                return ret;
            }
            else if (script is str)
            {
                if (script.startswith("ltext(") && script.endswith(")"))
                {
                    // special for ltext() function
                    return script;
                }
                else
                {
                    return "'" + script + "'";
                }
            }
            else if (script is float || script is Single)
            {
                return String.Format("%.3f", script);
            }
            else if (script is @bool)
            {
                if (script)
                {
                    return "1";
                }
                else
                {
                    return "0";
                }
            }
            else if (script is int || script is Byte)
            {
                return script.ToString();
            }
            else if (script == act)
            {
                return "act";
            }
            else if (script == anime)
            {
                return "anime";
            }
            else if (script == null)
            {
                return "None";
            }
            else
            {
                throw new Exception("script2string: Unknown type " + type(script).ToString() + " of " + script.ToString());
            }
        }

        public static object scriptCopy(object script)
        {
            object ret;
            if (script is list)
            {
                ret = new List<object>();
                foreach (var subElm in script)
                {
                    ret.append(scriptCopy(subElm));
                }
                return ret;
            }
            else if (script is tuple)
            {
                ret = new List<object>();
                foreach (var subElm in script)
                {
                    ret.append(scriptCopy(subElm));
                }
                return tuple(ret);
            }
            else if (script is dict)
            {
                ret = new Dictionary<object, object>
                {
                };
                foreach (var subKey in script.keys())
                {
                    ret[subKey] = scriptCopy(script[subKey]);
                }
                return ret;
            }
            else if (script is Vector3)
            {
                ret = Vector3(script.x, script.y, script.z);
                return ret;
            }
            else if (script is Vector2)
            {
                ret = Vector2(script.x, script.y);
                return ret;
            }
            else if (script is Color)
            {
                ret = Color(script.r, script.g, script.b, script.a);
                return ret;
            }
            else
            {
                return script;
            }
        }
    }
}
