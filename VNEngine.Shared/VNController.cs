using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BepInEx.Configuration;
using UnityEngine;
using static VNEngine.Utils;
using static VNEngine.VNCamera;

//using WindowsInput;
//using WindowsInput.Native;

namespace VNEngine
{
    public abstract class VNController
        : BaseController
    {
        public delegate void GameFunc(VNController controller);

        // 
        //         Made animation movement to camera with some number
        //         :param float duration: Duration of animation in seconds
        //         :param int camnum: Camera number to animate from current position
        //         :param * style: may be an object or string. String can be linear,slow-fast,fast-slow,slow-fast3,fast-slow3,slow-fast4,fast-slow4. Object may vary
        //         :param Callable onCameraEnd: function that wil called after animation end
        //         

        public delegate void MenuFunc(VNController controller, Dictionary<string, string> param);

        public delegate void TimerUpdateFunc(VNController controller, float deltaTime, float timeLapsed,
            float timeLeft);

        public delegate void UpdateFunc(string file);

        public int _btnSepCounter;

        public Dictionary<string, List<GameFunc>> _eventListenerDic;

        public List<GameFunc> _menuStack;

        public GameFunc _onCameraEnd;

        public List<Button_s> _vnButtons = new List<Button_s>();
        private List<Button_s> _vnStButtons;

        public string _vnStText;

        public string _vnText;

        public string[] arKeyKodes;

        public string btnNextText;

        public List<Button_s> btnsFull;

        public int camAnimeTID;

        public Dictionary<string, float> camAnimFullStyle;

        public string camAnimStyle;

        public Vector3 camSAngle;

        public Vector3 camSDir;

        public float camSFOV;

        public Vector3 camSPos;

        public Vector3 camTAngle;

        public Vector3 camTDir;

        public float camTFOV;

        public Vector3 camTPos;

        public string curCharFull;

        public string curCharText;

        public string current_game;

        public GameFunc endNextTextFunc;

        public string fAutoLipSyncVer;

        public string funcLockedText;

        public Dictionary<string, object> gpersdata;

        public bool isfAutoLipSync;

        public bool isFuncLocked;

        public bool isHideGameButtons;

        public bool isHideWindowDuringCameraAnimation;

        public bool isShowDevConsole;

        public int lipAnimeTID;

        public int maxBtnsBeforeSeparator;

        public int menu_result;

        //public Dictionary<int, (string character, string text, GameFunc func, object param)> nextTexts;

        public GameFunc onDumpSceneOverride;

        public Action<VNController, string, string> onSetTextCallback;

        public object readingChar;

        public int readingProgress;

        public double readingSpeed;

        public Dictionary<string, RegisteredChar_s> registeredChars;

        public string sceneDir;

        public SkinBase skin;

        public SkinBase skin_default;

        public SkinBase skin_saved;

        public Timer[] timers;

        public UpdateFunc updFunc;

        public string updFuncParam;

        public string vnButtonsStyle;
        public int wheight;

        public GUI.WindowFunction windowCallback;

        public Rect windowRect;

        public GUIStyle windowStyle;

        public GUIStyle windowStyleDefault;

        public int wwidth;

        public VNController()
        {
            arKeyKodes = null; //Utils.getEngineOptions()["keysforbuttons"].Split(',');
            vnButtonsStyle = "normal";
            //this.visible = this.engineOptions["starthidden"] == "0";
            // self.wwidth = 500
            // self.wheight = 230
            //
            // self.windowName = ''
            // self.windowRect = Rect (Screen.width / 2 - self.wwidth / 2, Screen.height - self.wheight - 10, self.wwidth, self.wheight)
            //self.skin_panel_unity = CloneSkin(GUI.)
            windowStyle = null;
            windowStyleDefault = new GUIStyle("window");
            skin_set_byname("skin_default");
            skin_default = skin;
            _vnText = "Welcome to <b>VN Game engine</b>!\n";
            try
            {
                //this._vnText = this._vnText;
                _vnText += "\n";
            }
            catch (Exception)
            {
                _vnText += "<color=red>Warning!</color> You have a problems with UTF-8 libs! See website.\n";
                Console.WriteLine("VNGE: problems with UTF-8 libs detected");
            }

            //this._vnText += "- " + Utils.getKeyCodes()["hide"].code + " to show/hide this window\n- " + Utils.getKeyCodes()["reset"].code + " to return to this main screen (more in INI file)";
            //self._vnButtons = ["Start >"]
            //self._vnButtonsActions = [self.StartAct]
            registeredChars = new Dictionary<string, RegisteredChar_s>();
            register_char("s", "ff5555", "");
            curCharText = "s";
            curCharFull = "s";

            //this.nextTexts = new Dictionary<int, List<Action>>(); TODO add this back

            updFunc = null;
            updFuncParam = "";
            timers = new Timer[8];

            /* TODO

            // preprocessing start options
            if (this._vnButtons[0] == "autogames")
            {
                //self._vnButtons = []
                //self._vnButtonsActions = []
                //self.prepare_auto_games()
                //self._vnButtons = ["All games list >>", "(hide this window)"]
                this._vnButtons = new List<string> {
                    "All games list >>",
                    "Simple novels list >>",
                    "(hide this window)"
                };
                this._vnButtonsActions = null;//new List<Action> { this.prepare_auto_games, this.game_start_fromfile, this._sup_hide_window }; TODO
            }

            */
            _vnStButtons = _vnButtons;
            _vnStText = _vnText;
            maxBtnsBeforeSeparator = 5;
            _btnSepCounter = 0;
            btnsFull = new List<Button_s>();
            //this.gdata = new GData();
            //this.scenedata = new GData();
            gpersdata = new Dictionary<string, object>();
            current_game = "";
            _eventListenerDic = new Dictionary<string, List<GameFunc>>();
            isfAutoLipSync = false;
            fAutoLipSyncVer = "v10";
            init_start_params();
            // autoloading feature
            windowCallback = FuncWindowGUI;
        }

        //self.OnGUI(self)
        public string vnText
        {
            get => _vnText;
            set => _vnText = value;
        }

        //self.OnGUI(self)
        // ---- external game functions ---------
        public List<Button_s> vnButtons
        {
            get => _vnButtons;
            set => _vnButtons = value;
        }

        public void init_start_params()
        {
            isShowDevConsole = false;
            // menu
            _menuStack = new List<GameFunc>();
            isHideGameButtons = false;
            onSetTextCallback = null;
            camAnimeTID = -1;
            onDumpSceneOverride = null;
            isHideWindowDuringCameraAnimation = false;
            isFuncLocked = false;
            funcLockedText = "SYSTEM: Unknown default lock";
            // some settings - may be localized
            btnNextText = "Next >";
            //self.autostart = False
            //self.isDevDumpButtons = False - no use
            sceneDir = "";
            //this.gdata = new GData();
            gpersdata = new Dictionary<string, object>();
            //this.scenedata = new GData();
            current_game = "";
            // lip sync
            isfAutoLipSync = false;
            fAutoLipSyncVer = "v10";
            readingChar = null;
            readingSpeed = 12.0;
            readingProgress = 0;
            lipAnimeTID = -1;
            _eventListenerDic = new Dictionary<string, List<GameFunc>>();
            windowStyle = windowStyleDefault;
            skin_set(skin_default);
        }

        public void FuncWindowGUI(int windowid)
        {
            if (skin is SkinCustomWindow customWindow)
            {
                // skin has it's own WindowGUI func
                customWindow.customWindowGUI(windowid);
                return;
            }

            if (!isFuncLocked)
            {
                if (!isShowDevConsole)
                    try
                    {
                        skin.render_main(curCharFull, vnText, vnButtons, vnButtonsStyle);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Error in skin.render_main: " + e);
                        visible = false;
                    }
                else
                    // show dev console
                    try
                    {
                        if (skin is SkinDefault skinDefault) skinDefault.render_dev_console();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Error in skin.render_dev_console: " + e);
                        visible = false;
                    }
            }
            else
            {
                // render system message
                skin.render_system(funcLockedText);
            }
        }

        public void OnGUI()
        {
            //r = self.windowRect
            //self.windowRect = Rect(r.x, r.y, r.width, 70 if self.show_buttons else 50)
            //if self.windowStyle:
            //    GUI.skin.window = self.windowStyle
            //BaseController.OnGUI(self)
            if (!visible) return;
            windowRect = GUI.Window(0, windowRect, windowCallback, windowName, windowStyle);
        }


        public new void Update()
        {
            base.Update();
            if (updFunc != null)
            {
                var func = updFunc;
                updFunc = null;
                func(updFuncParam);
            }

            for (var i = 0; i < timers.Length; i++)
                if (timers[i] is Timer timer)
                    if (timer.timeLeft > 0)
                    {
                        timer.timeLeft -= Time.deltaTime;

                        if (timer.updateFunc is TimerUpdateFunc func)
                            func(this, Time.deltaTime, timer.duration - timer.timeLeft, timer.duration);

                        if (timer.timeLeft <= 0) call_game_func(timer.funcEnd);
                    }

            UpdateKeyChecks();
            event_dispatch("update", null);
        }


        public void UpdateKeyChecks()
        {
            if (checkKeyCode("Reset")) return_to_start_screen_clear();
            if (checkKeyCode("ReloadCurrentGame"))
                if (current_game != "")
                    game_start_fromfile(this, current_game);
            if (checkKeyCode("VNFrameDeveloperConsole"))
                try
                {
                    //ScriptHelper.toggle_devconsole(this); TODO
                }
                catch (Exception e)
                {
                    show_blocking_message_time(string.Format("Error: can't start VNFrame developer console: {0}", e));
                }

            if (GetConfigEntry("Skins", "usekeysforbuttons"))
                if (visible && !isFuncLocked && !isHideGameButtons)
                    foreach (var i in Enumerable.Range(0, vnButtons.Count))
                        if (Input.GetKeyDown(arKeyKodes[i]))
                            //self._vnButtonsActions[i](self)
                            call_game_func(_vnButtons[i]);
            // running games from INI
            /*
            this._util_upd_check_and_start_game("game1");
            this._util_upd_check_and_start_game("game2");
            this._util_upd_check_and_start_game("game3");
            this._util_upd_check_and_start_game("game4");
            this._util_upd_check_and_start_game("game5");
            this._util_upd_check_and_start_game("game6");
            this._util_upd_check_and_start_game("game7");
            this._util_upd_check_and_start_game("game8");
            this._util_upd_check_and_start_game("game9");
            this._util_upd_check_and_start_game("game10");
            */
            if (checkKeyCode("developerconsole"))
            {
                if (isShowDevConsole)
                {
                    // restoring old window
                    isShowDevConsole = false;
                    skin_set(skin_saved);
                }
                else
                {
                    // set default skin and set console flag to show
                    // console must be rendered only in default skin
                    skin_saved = skin;
                    skin_set(skin_default);
                    isShowDevConsole = true;
                }
            }

            if (checkKeyCode("dumpcamera")) dump_camera();
            if (checkKeyCode("reloadvnengine"))
            {
                // reload engine
                Console.WriteLine("Try reloading engine...");
                try
                {
                    //reload(sys.modules["vngameengine"]);
                    //sys.modules["vngameengine"].vngame_window_autogames_uni();
                    Console.WriteLine("Reloading engine success!");
                }
                catch (Exception)
                {
                    Console.WriteLine("Error in reloading game engine");
                }
            }
        }

        /*
        public void _util_upd_check_and_start_game(string gamekey)
        {
            if (this.engineOptions.ContainsKey(gamekey))
            {
                if (this.engineOptions[gamekey] != "")
                {
                    if (checkKeyCode(gamekey))
                    {
                        this.game_start_fromfile(this, this.engineOptions[gamekey]);
                    }
                }
            }
        }
        */

        public bool GetConfigEntry(string section, string key)
        {
            var value = Config[new ConfigDefinition(section, key)];

            if (value.BoxedValue is bool option)
                return option;
            return (bool) value.DefaultValue;
        }

        public void return_to_start_screen_clear()
        {
            clear_timers();
            //self.reset() # before init_start_params to call before_scene_unload event
            _unload_scene_before();
            // no resetting scene!
            init_start_params();
            return_to_start_screen();
        }

        public int set_timer(float duration, GameFunc timerFuncEnd, TimerUpdateFunc timerFuncUpd = null)
        {
            Logger.LogDebug("Start set_timer!");
            int i;
            for (i = 0; i < timers.Length; i++)
                if (timers[i] is null)
                {
                    var timer = new Timer
                    {
                        timeLeft = duration,
                        duration = duration,
                        funcEnd = timerFuncEnd,
                        updateFunc = timerFuncUpd
                    };

                    timers[i] = timer;
                    return i;
                }

            return -1;
        }

        public void clear_timer(int index, bool runEndFunc = false)
        {
            if (index < timers.Length)
            {
                if (runEndFunc && timers[index] is Timer t) t.funcEnd(this);
                timers[index] = null;
            }
        }

        public void return_to_start_screen()
        {
            skin_set_byname("skin_default");
            set_text("s", _vnStText);
            //this.set_buttons(this._vnStButtons, this._vnStButtonsActions); TODO
        }

        public void clear_timers()
        {
            // not calling end function
            timers = new Timer[8];
        }

        public void set_buttons(List<Button_s> buttons, string style = "normal")
        {
            vnButtonsStyle = style;
            if (style == "normal") maxBtnsBeforeSeparator = skin.maxButtonsNormal;
            if (style == "compact") maxBtnsBeforeSeparator = skin.maxButtonsCompact;
            if (buttons.Count <= maxBtnsBeforeSeparator)
            {
                // normal case, not so much btns
                _btnSepCounter = 0;
                vnButtons = buttons;
            }
            else
            {
                _btnSepCounter = 0;
                btnsFull = buttons;
                _btnCallSepCounter(this, 0);
                //self.OnGUI(self)
            }
        }

        public void _btnCallFull(VNController game, int param)
        {
            call_game_func(btnsFull[param]);
        }

        public void _btnCallSepCounter(VNController game, int param)
        {
            // wrapping over list
            if (param > btnsFull.Count - 1) param = 0;
            // get sublist
            var endindex = param + maxBtnsBeforeSeparator - 1;
            if (endindex > btnsFull.Count) endindex = btnsFull.Count;
            var ar1 = btnsFull.GetRange(param, maxBtnsBeforeSeparator - 1);
            //print param
            //print endindex
            //print ar1
            var ar2 = new List<Button_s>();
            foreach (var i in Enumerable.Range(0, ar1.Count)) ar2.Add(new Button_s("Button", _btnCallFull, param + i));
            // add button to move forward
            ar2.Add(new Button_s(">>", _btnCallSepCounter, param + maxBtnsBeforeSeparator - 1));
            // setting buttons
            set_buttons(ar2, vnButtonsStyle);
        }


        public void set_buttons_alt(List<Button_s> arButTextsActions, string style = "normal")
        {
            set_buttons(arButTextsActions, style);

/* Unmerged change from project 'VNEngine.AI'
Before:
        }
        
        
        public void set_buttons_end_game()
After:
        }


        public void set_buttons_end_game()
*/

/* Unmerged change from project 'VNEngine.HS2'
Before:
        }
        
        
        public void set_buttons_end_game()
After:
        }


        public void set_buttons_end_game()
*/
        }


        public void set_buttons_end_game()
        {
            var buttons = new List<Button_s> {new Button_s("End Game & Return >>", _onEndGame, -1)};
            set_buttons(buttons);
        }

        public void _onEndGame(VNController game, int i)
        {
            return_to_start_screen_clear();
        }

        public void set_text(string character, string text)
        {
            var char0 = character.Split('/')[0];
            curCharText = char0;
            curCharFull = character;
            if (text.StartsWith("!"))
                vnText = text.Substring(1);
            else
                vnText = text;
            //self.OnGUI(self)
            if (onSetTextCallback != null) onSetTextCallback(this, character, text);
            event_dispatch("set_text", new RegisteredChar_s(character, text));
        }

        public void set_text_s(string text)
        {
            set_text("s", text);
        }

        public void register_char(string name, string color, string showname)
        {
            registeredChars[name] = new RegisteredChar_s(color, showname);
        }

        public void texts_next(Dictionary<int, List<Action>> nexttexts, GameFunc endfunc)
        {
            //this.nextTexts = nexttexts; TODO
            endNextTextFunc = endfunc;
            NextText(this);
        }

        public void NextText(VNController game)
        {
        }

        /* TODO
        public void NextText(VNController game)
        {
            if (this.nextTexts.Count > 0)
            {
                this.set_text(this.nextTexts[0].character, this.nextTexts[0].text);
                this.set_buttons(new List<string> { this.btnNextText }, new List<GameFunc> {  this.NextText  });

                if (!(this.nextTexts[0].func is null))
                {
                    var func = this.nextTexts[0].func;
                    func(this, this.nextTexts[0].param);
                }

                this.nextTexts.Remove(0);
            }
            else
            {
                this.endNextTextFunc(this);
            }
        }
        */

        public void show_blocking_message(string text = "...")
        {
            funcLockedText = text;
            isFuncLocked = true;
        }

        public void hide_blocking_message(VNController game = null)
        {
            isFuncLocked = false;
        }

        public void show_blocking_message_time(string text = "...", int duration = 3)
        {
            show_blocking_message(text);
            set_timer(duration, hide_blocking_message);
        }

        /* TODO replace with bepinex
        // simulating key presses
        public void anim_sim_zoom_in(float duration)
        {
            var sim = new InputSimulator();
            sim.Keyboard.KeyDown(VirtualKeyCode.UP);
            this.set_timer(duration, this._anim_sim_zoom_in_end);
        }

        public void _anim_sim_zoom_in_end(VNController game)
        {
            var sim = new InputSimulator();
            sim.Keyboard.KeyUp(VirtualKeyCode.UP);
        }

        public void anim_sim_zoom_out(float duration)
        {
            var sim = new InputSimulator();
            sim.Keyboard.KeyDown(VirtualKeyCode.DOWN);
            this.set_timer(duration, this._anim_sim_zoom_out_end);
        }

        public void _anim_sim_zoom_out_end(VNController game)
        {
            var sim = new InputSimulator();
            sim.Keyboard.KeyUp(VirtualKeyCode.DOWN);
        }
        */

        // reseting scene - must be overrided by engine

        public void call_game_func(Button_s param)
        {
            try
            {
                param.btnCallFull(this, param.v);
            }
            catch (Exception e)
            {
                Logger.LogError("Error in call_game_func: " + e);
            }
        }

        /*
        public void call_game_func(List<Utils.ButtonFunc_s> param)
        {
            try
            {
                    param.a[0](param.o);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in call_game_func: " + e.ToString());
            }
        }
        */

        public void call_game_func(Action<VNController, object> a, object o)
        {
            try
            {
                a(this, o);
            }
            catch (Exception e)
            {
                Logger.LogError("Error in call_game_func: " + e);
            }
        }

        public void call_game_func(Action<VNNeoController> a)
        {
            try
            {
                a((VNNeoController) this);
            }
            catch (Exception e)
            {
                Logger.LogError("Error in call_game_func: " + e);
            }
        }

        public void call_game_func(List<Action> param)
        {
            try
            {
                // [func]
                //print "new call"
                param[0]();
            }
            catch (Exception e)
            {
                Logger.LogError("Error in call_game_func: " + e);
            }
        }

        public void call_game_func(GameFunc param)
        {
            try
            {
                // default - call func(game)
                param(this);
            }
            catch (Exception e)
            {
                Logger.LogError("Error in call_game_func: " + e);
            }
        }

        // Return dir, where engine saves scenes
        public abstract string SceneDir();

        public abstract void dump_camera();

        // ---------- menu functions -------------------------
        public void run_menu(MenuFunc menufunc, Dictionary<string, string> menuparam, GameFunc onEndFunc)
        {
            _menuStack.Add(onEndFunc);
            menufunc(this, menuparam);
        }

        public void menu_finish(int result)
        {
            menu_result = result;
            var endFunc = _menuStack.Last();
            _menuStack.RemoveAt(-1);
            call_game_func(endFunc);
        }

        // ---------- checking for engine types --------------
        // -------- other ----------
        public void scene_set_bg_png(object filepng)
        {
            show_blocking_message_time("ERROR: scene_set_bg_png was not implemented");
        }

        // ---------- cameras ----------

        public void move_camera(CamData cam)
        {
            move_camera_direct(cam);
        }

        public void move_camera(Vector3 pos, Vector3 distance, Vector3 rotate, float fov = 23.0f)
        {
            //self.show_blocking_message_time("ERROR: move_camera was not implemented")
            var camobj = new CamData(pos, rotate, distance, fov);
            move_camera_obj(camobj);
        }

        public abstract void move_camera_direct(CamData cam);

        public abstract void move_camera_direct(Vector3 pos, Vector3 distance, Vector3 rotate, float fov);

        public void move_camera_obj(CamData camobj)
        {
            move_camera_direct(camobj);
        }

        /* TODO
        public void anim_to_camera_obj(float duration, CamData camobj, Dictionary<string, float> style, Action onCameraEnd = null)
        {
            this._anim_to_camera_savecurrentpos();
            // print "Anim to cam 1"
            // print "Anim to cam 2"
            var camobjv = this.cam2vec(camobj);
            this.camTPos = camobjv.position;
            this.camTDir = camobjv.distance;
            this.camTAngle = camobjv.rotation;
            this.camTFOV = camobjv.fov;

            this.camAnimStyle = style["style"];
            this.camAnimFullStyle = style;
            if (this.camAnimFullStyle.ContainsKey("add_distance_target_camera"))
            {
                this.camTDir = new Vector3(this.camTDir.x, this.camTDir.y, this.camTDir.z + this.camAnimFullStyle["add_distance_target_camera"]);
            }
            // camera animation one timer only
            animation_cam_timer(duration, onCameraEnd);
        }
        */

        public void animation_cam_timer(float duration, GameFunc onCameraEnd)
        {
            // camera animation one timer only
            if (camAnimeTID != -1) clear_timer(camAnimeTID);
            camAnimeTID = set_timer(duration, _anim_to_cam_end, _anim_to_cam_upd);
            _onCameraEnd = onCameraEnd;
            if (isHideWindowDuringCameraAnimation) visible = false;
        }

        public void _anim_to_cam_upd(VNController game, float dt, float time, float duration)
        {
            var camProgress = time / duration;
            if (camAnimStyle == "linear") camProgress = time / duration;
            if (camAnimStyle == "slow-fast") camProgress = Mathf.Pow(camProgress, 2);
            if (camAnimStyle == "fast-slow") camProgress = 1 - Mathf.Pow(1 - camProgress, 2);
            if (camAnimStyle == "slow-fast3") camProgress = Mathf.Pow(camProgress, 3);
            if (camAnimStyle == "fast-slow3") camProgress = 1 - Mathf.Pow(1 - camProgress, 3);
            if (camAnimStyle == "slow-fast4") camProgress = Mathf.Pow(camProgress, 4);
            if (camAnimStyle == "fast-slow4") camProgress = 1 - Mathf.Pow(1 - camProgress, 4);
            var TPos = camTPos;
            var TDir = camTDir;
            var TAngle = camTAngle;
            if (camAnimFullStyle != null)
            {
                if (camAnimFullStyle.ContainsKey("target_camera_zooming_in"))
                    TDir = new Vector3(TDir.x, TDir.y,
                        TDir.z - camAnimFullStyle["target_camera_zooming_in"] * (1 - time / duration));
                if (camAnimFullStyle.ContainsKey("target_camera_rotating_z"))
                    TAngle = new Vector3(TAngle.x, TAngle.y,
                        TAngle.z + camAnimFullStyle["target_camera_rotating_z"] * (1 - time / duration));
                if (camAnimFullStyle.ContainsKey("target_camera_rotating_x"))
                    TAngle = new Vector3(
                        TAngle.x + camAnimFullStyle["target_camera_rotating_x"] * (1 - time / duration), TAngle.y,
                        TAngle.z);
                if (camAnimFullStyle.ContainsKey("target_camera_rotating_y"))
                    TAngle = new Vector3(TAngle.x,
                        TAngle.y + camAnimFullStyle["target_camera_rotating_y"] * (1 - time / duration), TAngle.z);
                if (camAnimFullStyle.ContainsKey("target_camera_posing_y"))
                    TPos = new Vector3(TPos.x,
                        TPos.y + camAnimFullStyle["target_camera_posing_y"] * (1 - time / duration), TPos.z);
                // TDir.z = TDir.z + self.camAnimFullStyle["move_distance"] * time / duration
                // TDir.z = TDir.z + (-20)
                // print "z: %s"%(str(TDir.z))
            }

            var pos = Vector3.Lerp(camSPos, TPos, camProgress);
            var distance = Vector3.Lerp(camSDir, TDir, camProgress);
            var rotate = Vector3.Slerp(camSAngle, TAngle, camProgress);
            var fov = Mathf.Lerp(camSFOV, camTFOV, camProgress);
            //print fov, self.camSFOV, self.camTFOV, camProgress
            move_camera_direct(pos, distance, rotate, fov);
        }

        public void _anim_to_cam_end(VNController game)
        {
            // game.set_text("Anim camera end!")
            // print "Anim camera end!"
            if (isHideWindowDuringCameraAnimation) visible = false;
            camAnimeTID = -1;
            if (_onCameraEnd != null) call_game_func(_onCameraEnd);
        }

        public Vector3 vec3(float x, float y, float z)
        {
            return new Vector3(x, y, z);
        }

        // ---- automaking list of games -----
        public void prepare_auto_games()
        {
            prepare_auto_games_prefix(this, "");
        }

        public void prepare_auto_games_prefix(VNController game, string prefix)
        {
        }

        /*public object prepare_auto_games_prefix(VNNeoController game, string prefix)
        {
            var mypath = this.pygamepath;
            var onlyfiles = (from f in listdir(mypath) where isfile(join (mypath, f)) select f).ToList();
            var arTx = new List<object>();
            var arTx2 = new List<object>();
            var arAc = new List<object>();
            foreach (var fil in onlyfiles)
            {
                //print fil[:-3]
                if (fil.endswith(".py"))
                {
                    string firstline = this.file_get_firstline(mypath + "\\" + fil);
                    //print firstline
                    string[] ar = firstline.Split(';');
                    // checking header
                    if (ar.Length >= 3)
                    {
                        if (ar[0] == "#vngame")
                        {
                            if (ar[1] == "all" || ar[1] == this.engine_name)
                            {
                                // found game!
                                //print "-- Found game! --"
                                var gamename = ar[2];
                                if (gamename[-1] == "\n")
                                {
                                    gamename = gamename[:: - 1];
                                }
                                // Add game or folder if prefix matches. Unless the name already exists.
                                // (An empty prefix will match all games)
                                if (gamename.startswith(prefix))
                                {
                                    // Remove prefix from gamename
                                    if (prefix.Count > 0)
                                    {
                                        gamename = gamename[prefix.Count];
                                    }
                                    // If gamename looks like a folder, just keep the foldername
                                    if (gamename.ContainsKey("/"))
                                    {
                                        gamename = gamename.split("/")[0];
                                        gamename = gamename + "/";
                                    }
                                    if (!arTx2.ContainsKey(gamename))
                                    {
                                        arTx2.append(gamename);
                                        if (gamename.ContainsKey("/"))
                                        {
                                            arTx.append("<color=#aaaaaaff>" + gamename[:: - 1] + " ></color>");
                                            arAc.append((this.prepare_auto_games_prefix, prefix + gamename));
                                        }
                                        else
                                        {
                                            arTx.append(gamename);
                                            arAc.append((this.game_start_fromfile, fil[:: - 3]));
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            // IF we have a prefix, add button to go back one level
            if (prefix.Count > 0)
            {
                var parent = "/".join(prefix.split("/")[:: - 2]);
                // Add a / but only if the prefix we want to go to is non-empty
                if (parent)
                {
                    parent = parent + "/";
                }
                arTx.Add("<< Back");
                arAc.Add((this.prepare_auto_games_prefix, parent));
            }
            this.set_text_s("Available games:");
            this.set_buttons(arTx, arAc);
        }*/


        public void game_start_fromfile()
        {
        }

        public void game_start_fromfile(object game, string gamefilestr)
        {
        }
        //public void game_start_fromfile(object game, object gamefilestr) { }
        /*
    {
        var oldcurrentgame = this.current_game;
        Console.WriteLine(String.Format("-- Importing and starting game: %s --", gamefilestr));
        try
        {
            this.current_game = gamefilestr;
            sys.modules[gamefilestr].start(this);
        }
        catch (Exception e)
        {
            Console.WriteLine("Error in game loading: " + e.ToString());
            Console.WriteLine("details:");
            traceback.print_exc();
            this.show_blocking_message_time(String.Format("ERROR: can't load or start game '%s'", gamefilestr));
            this.current_game = oldcurrentgame;
            //__import__(gamefilestr).start(self)
        }
    }*/

        /*       public object file_get_firstline(object filename)
               {
                   if (os.path.exists(filename))
                   {
                       var fp = open(filename, "r");
                       var content = fp.readline();
                       fp.close();
                       return content;
                   }
                   return "";
               }

               public object file_get_firstline_utf8(object filename)
               {
                   if (os.path.exists(filename))
                   {
                       var fp = codecs.open(filename, "r", encoding: "utf-8");
                       var content = fp.readline();
                       fp.close();
                       return content;
                   }
                   return "";
               }

               public object file_get_content(object filename)
               {
                   if (os.path.exists(filename))
                   {
                       var fp = open(filename, "r");
                       var content = fp.read();
                       fp.close();
                       return content;
                   }
                   return "";
               }
        */
        public string file_get_content_utf8(string filename)
        {
            if (File.Exists(filename))
            {
                var content = File.ReadAllText(filename);
                return content;
            }

            return "";
        }

        // ------------ event system -----------
        public void event_reg_listener(string eventid, GameFunc func)
        {
            _event_create_lisarray_ifneeded(eventid);
            _eventListenerDic[eventid].Add(func);
        }

        public void _event_create_lisarray_ifneeded(string eventid)
        {
            if (_eventListenerDic.ContainsKey(eventid)) return;
        }

        public object event_unreg_listener(string eventid, GameFunc func)
        {
            _event_create_lisarray_ifneeded(eventid);
            if (_eventListenerDic[eventid].Contains(func))
            {
                _eventListenerDic[eventid].Remove(func);
                return true;
            }

            return false;
        }

        public void event_dispatch(string eventid, object param)
        {
            if (_eventListenerDic.ContainsKey(eventid))
                foreach (var func in _eventListenerDic[eventid])
                {
                    //func(this, eventid, param); TODO
                }
        }

        public void _load_scene_before(string file)
        {
            _unload_scene_before();
            event_dispatch("before_scene_load", file);
        }

        public void _unload_scene_before()
        {
            event_dispatch("before_scene_unload", null);
        }

        // -------- game persistent data ----------
        public string gpersdata_getfilename()
        {
            var dstfile = combine_path(pygamepath, "Gpdata", current_game + "_p.dat");
            return dstfile;
        }

        public bool gpersdata_exists()
        {
            var dstfile = gpersdata_getfilename();
            return File.Exists(dstfile);
        }

        public void gpersdata_load()
        {
            object msg;
            try
            {
                var dstfile = gpersdata_getfilename();
                if (gpersdata_exists())
                    //this.gpersdata = pickle.load(f2);

                    msg = "gpersdata loaded!";
                else
                    msg = "gpersdata not exists!";
            }
            catch (Exception e)
            {
                msg = "gpersdata load Failed: " + e;
            }

            Console.WriteLine(msg);
        }

        public object gpersdata_save()
        {
            try
            {
                var dstfile = gpersdata_getfilename();

                //pickle.dump(this.gpersdata, f);
                return "";
            }
            catch (Exception e)
            {
                var msg = "gpersdata save Failed: " + e;
                Console.WriteLine(msg);
                return msg;
                // game.show_blocking_message_time(msg)
            }
        }

        public object gpersdata_set(string param, List<object> val)
        {
            gpersdata[param] = val;
            return gpersdata_save();
        }


        public Dictionary<string, Checkpoint> gpersdata_get(string param)
        {
            /* TODO
            if (this.gpersdata.ContainsKey(param))
            {
                return this.gpersdata[param];
            }
            else
            {
                return null;
            }
            */
            return null;
        }

        public void gpersdata_clear()
        {
            object msg;
            try
            {
                var dstfile = gpersdata_getfilename();
                File.Delete(dstfile);
                gpersdata = new Dictionary<string, object>();
                msg = "gpersdata cleared!";
            }
            catch (Exception e)
            {
                msg = "gpersdata clear Failed: " + e;
            }

            Console.WriteLine(msg);
        }

        // ---------- checkpoints --------

        /* TODO

        public void checkpoint_set_list(string type, Checkpoint arr)
        {

            this.gdata._check_list = new Dictionary<string, List<object>>();
            this.gdata._check_list[type] = arr;
        }

        public object checkpoint_save(string type, string checkId)
        {
            var arr = this.checkpoint_loadall(type);
            if (arr.ContainsKey(checkId))
            {
                return "";
            }
            else
            {
                arr.Add(checkId);
                return this.gpersdata_set("_checkpoints_" + type, arr);
            }
        }

        public Dictionary<string, Checkpoint> checkpoint_loadall(string type)
        {
            var res = this.gpersdata_get("_checkpoints_" + type);
            if (res == null)
            {
                return new Dictionary<string, Checkpoint>();
            }
            else
            {
                return res;
            }
        }

        public void checkpoint_clean(string type)
        {
            this.gpersdata_set("_checkpoints_" + type, new List<object>());
        }

        public bool checkpoint_goto(string type, string checkId)
        {
            var arr = this.gdata._check_list[type];
            foreach (Checkpoint checkpoint in arr)
            {
                if (checkpoint.id == checkId)
                {
                    this.call_game_func(checkpoint.func);
                    return true;
                }
            }
            return false;
        }

        public object checkpoint_has_one(string type)
        {
            return this.checkpoint_loadall(type).Keys.Count > 0;
        }

        public bool checkpoint_goto_latest(string type)
        {
            var arr = this.checkpoint_loadall(type);
            if (arr.Count > 0)
            {
                return this.checkpoint_goto(type, arr[-1]);
            }
            return false;
        }

        public object checkpoint_rendergotomenu(string type, bool showall = true)
        {
            var btns = new List<object>();
            var arr = this.gdata._check_list[type];
            var arrpassed = this.checkpoint_loadall(type);
            foreach (Checkpoint checkpoint in arr)
            {
                var checkId = checkpoint.id;
                if (arrpassed.ContainsKey(checkId))
                {
                    btns += new List<Tuple<object, Tuple<Func<object>, object>>> {
                        checkpoint.func[0],
                        (this._checkpoint_goto_menu, (type, checkId))
                    };
                }
                else if (showall)
                {
                    var txt = "<color=#{1}ff>{0}</color>".format(checkpoint[2][0], "666666");
                    btns += new List<string> {
                        txt,
                        null
                    };
                }
            }
            return btns;
        }

        public void _checkpoint_goto_menu(object game, Checkpoint param)
        {
            this.checkpoint_goto(param.id, param.func);
        }
        */

        // --------- skin system ------------
        public void skin_set(SkinBase skin)
        {
            this.skin = skin;
            this.skin.setup(this);
        }

        public void skin_set_byname(string skinname)
        {
            try
            {
                SkinBase skin;
                switch (skinname)
                {
                    case "skin_default":
                        skin = new SkinDefault();
                        break;
                    case "skin_renpy":
                        skin = new SkinRenPy();
                        break;
                    case "skin_renpymini":
                        skin = new SkinRenPyMini();
                        break;
                    default:
                        skin = new SkinDefault();
                        break;
                }

                skin_set(skin);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error loading skin ", skinname, ", error: ", e);
            }
        }

        public SkinBase skin_get_current()
        {
            return skin;
        }

        public class Timer
        {
            public float duration;
            public GameFunc funcEnd;
            public float timeLeft;
            public TimerUpdateFunc updateFunc;
        }

        public struct RegisteredChar_s
        {
            public string color;
            public string showname;

            public RegisteredChar_s(string color, string showname)
            {
                this.color = color;
                this.showname = showname;
            }
        }

        public struct Checkpoint
        {
            public string id;
            public GameFunc func;
        }
    }
}