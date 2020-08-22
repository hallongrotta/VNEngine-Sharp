using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using VNActor;
using VNEngine.Shared;
using static VNEngine.Utils;
using static VNEngine.VNCamera;
//using WindowsInput;
//using WindowsInput.Native;

namespace VNEngine
{
    public abstract class VNController
       : BaseController
    {

        public List<Button_s> btnsFull;

        public int wwidth;
        public int _btnSepCounter;

        public Dictionary<string, List<GameFunc>> _eventListenerDic;

        public List<GameFunc> _menuStack;

        public GameFunc _onCameraEnd;

        protected Dictionary<string, Actor> _scenef_actors;

        protected Dictionary<string, HSNeoOCIProp> _scenef_props;

        public List<Button_s> _vnButtons;

        public string _vnStText;

        public string _vnText;

        public string[] arKeyKodes;

        public string btnNextText;

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

        public GData gdata;

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

        public Timer[] timers;
        private List<Button_s> _vnStButtons;

        public delegate void TimerUpdateFunc(VNController controller, float deltaTime, float timeLapsed, float timeLeft);

        public class Timer
        {
            public float timeLeft;
            public float duration;
            public GameFunc funcEnd;
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

        public Dictionary<string, RegisteredChar_s> registeredChars;

        public GData scenedata;

        public string sceneDir;

        public SkinBase skin;

        public SkinBase skin_default;

        public SkinBase skin_saved;

        public delegate void GameFunc(VNController controller);

        public delegate void UpdateFunc(string file);

        public UpdateFunc updFunc;

        public string updFuncParam;

        public GUI.WindowFunction windowCallback;

        public Rect windowRect;

        public GUIStyle windowStyle;

        public GUIStyle windowStyleDefault;

        public string vnButtonsStyle;
        public int wheight;

        public struct Checkpoint
        {
            public string id;
            public GameFunc func;
        }

        public VNController() : base()
        {
            this.arKeyKodes = null; //Utils.getEngineOptions()["keysforbuttons"].Split(',');
            this.vnButtonsStyle = "normal";
            //this.visible = this.engineOptions["starthidden"] == "0";
            // self.wwidth = 500
            // self.wheight = 230
            //
            // self.windowName = ''
            // self.windowRect = Rect (Screen.width / 2 - self.wwidth / 2, Screen.height - self.wheight - 10, self.wwidth, self.wheight)
            //self.skin_panel_unity = CloneSkin(GUI.)
            this.windowStyle = null;
            this.windowStyleDefault = new GUIStyle("window");
            this.skin_set_byname("skin_default");
            this.skin_default = this.skin;
            this._vnText = "Welcome to <b>VN Game engine</b>!\n";
            try
            {
                //this._vnText = this._vnText;
                this._vnText += "\n";
            }
            catch (Exception)
            {
                this._vnText += "<color=red>Warning!</color> You have a problems with UTF-8 libs! See website.\n";
                Console.WriteLine("VNGE: problems with UTF-8 libs detected");
            }
            //this._vnText += "- " + Utils.getKeyCodes()["hide"].code + " to show/hide this window\n- " + Utils.getKeyCodes()["reset"].code + " to return to this main screen (more in INI file)";
            //self._vnButtons = ["Start >"]
            //self._vnButtonsActions = [self.StartAct]
            this.registeredChars = new Dictionary<string, RegisteredChar_s>();
            this.register_char("s", "ff5555", "");
            this.curCharText = "s";
            this.curCharFull = "s";

            //this.nextTexts = new Dictionary<int, List<Action>>(); TODO add this back

            this.updFunc = null;
            this.updFuncParam = "";
            this.timers = new Timer[8];

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
            this._vnStButtons = this._vnButtons;
            this._vnStText = this._vnText;
            this.maxBtnsBeforeSeparator = 5;
            this._btnSepCounter = 0;
            this.btnsFull = new List<Button_s>();
            //this.gdata = new GData();
            //this.scenedata = new GData();
            this.gpersdata = new Dictionary<string, object>();
            this._scenef_actors = new Dictionary<string, Actor>();
            this._scenef_props = new Dictionary<string, HSNeoOCIProp>();
            this.current_game = "";
            this._eventListenerDic = new Dictionary<string, List<GameFunc>>();
            this.isfAutoLipSync = false;
            this.fAutoLipSyncVer = "v10";
            this.init_start_params();
            // autoloading feature
            this.windowCallback = new GUI.WindowFunction(this.FuncWindowGUI);
        }

        public void init_start_params()
        {
            this.isShowDevConsole = false;
            // menu
            this._menuStack = new List<GameFunc>();
            this.isHideGameButtons = false;
            this.onSetTextCallback = null;
            this.camAnimeTID = -1;
            this.onDumpSceneOverride = null;
            this.isHideWindowDuringCameraAnimation = false;
            this.isFuncLocked = false;
            this.funcLockedText = "SYSTEM: Unknown default lock";
            // some settings - may be localized
            this.btnNextText = "Next >";
            //self.autostart = False
            //self.isDevDumpButtons = False - no use
            this.sceneDir = "";
            //this.gdata = new GData();
            this.gpersdata = new Dictionary<string, object>();
            //this.scenedata = new GData();
            this._scenef_actors = new Dictionary<string, Actor>();
            this._scenef_props = new Dictionary<string, HSNeoOCIProp>();
            this.current_game = "";
            // lip sync
            this.isfAutoLipSync = false;
            this.fAutoLipSyncVer = "v10";
            this.readingChar = null;
            this.readingSpeed = 12.0;
            this.readingProgress = 0;
            this.lipAnimeTID = -1;
            this._eventListenerDic = new Dictionary<string, List<GameFunc>>();
            this.windowStyle = this.windowStyleDefault;
            this.skin_set(this.skin_default);
        }

        public void _sup_hide_window()
        {
            this.hide_window();
        }

        public void FuncWindowGUI(int windowid)
        {
            if (this.skin is SkinCustomWindow customWindow)
            {
                // skin has it's own WindowGUI func
                customWindow.customWindowGUI(windowid);
                return;
            }
            if (!this.isFuncLocked)
            {
                if (!this.isShowDevConsole)
                {
                    try
                    {
                        this.skin.render_main(this.curCharFull, this.vnText, this.vnButtons, this.vnButtonsStyle);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Error in skin.render_main: " + e.ToString());
                        this.visible = false;
                    }
                }
                else
                {
                    // show dev console
                    try
                    {
                        if (this.skin is SkinDefault skinDefault)
                        {
                            skinDefault.render_dev_console();
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Error in skin.render_dev_console: " + e.ToString());
                        this.visible = false;
                    }
                }
            }
            else
            {
                // render system message
                this.skin.render_system(this.funcLockedText);
            }
        }

        public void OnGUI()
        {
            //r = self.windowRect
            //self.windowRect = Rect(r.x, r.y, r.width, 70 if self.show_buttons else 50)
            //if self.windowStyle:
            //    GUI.skin.window = self.windowStyle
            //BaseController.OnGUI(self)
            if (!this.visible)
            {
                return;
            }
            this.windowRect = GUI.Window(0, this.windowRect, this.windowCallback, this.windowName, this.windowStyle);
        }



        new public void Update()
        {
            base.Update();
            if (this.updFunc != null)
            {
                var func = this.updFunc;
                this.updFunc = null;
                func(this.updFuncParam);
            }
            for (int i = 0; i < timers.Length; i++)
            {
                if (timers[i] is Timer timer)
                {
                    if (timer.timeLeft > 0)
                    {
                        timer.timeLeft -= Time.deltaTime;

                        if (timer.updateFunc is TimerUpdateFunc func)
                        {
                            func(this, Time.deltaTime, timer.duration - timer.timeLeft, timer.duration);
                        }

                        if (timer.timeLeft <= 0)
                        {
                            this.call_game_func(timer.funcEnd);
                        }
                    }
                }
            }
            this.UpdateKeyChecks();
            this.event_dispatch("update", null);
        }


        public void UpdateKeyChecks()
        {
            if (checkKeyCode("Reset"))
            {
                this.return_to_start_screen_clear();
            }
            if (checkKeyCode("ReloadCurrentGame"))
            {
                if (this.current_game != "")
                {
                    this.game_start_fromfile(this, this.current_game);
                }
            }
            if (checkKeyCode("VNFrameDeveloperConsole"))
            {
                try
                {
                    //ScriptHelper.toggle_devconsole(this); TODO
                }
                catch (Exception e)
                {
                    this.show_blocking_message_time(String.Format("Error: can't start VNFrame developer console: {0}", e.ToString()));
                }
            }
            if (this.GetConfigEntry("Skins", "usekeysforbuttons"))
            {
                if (this.visible && !this.isFuncLocked && !this.isHideGameButtons)
                {
                    foreach (var i in Enumerable.Range(0, this.vnButtons.Count))
                    {
                        if (Input.GetKeyDown(this.arKeyKodes[i]))
                        {
                            //self._vnButtonsActions[i](self)
                            this.call_game_func(this._vnButtons[i]);
                        }
                    }
                }
            }
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
                if (this.isShowDevConsole)
                {
                    // restoring old window
                    this.isShowDevConsole = false;
                    this.skin_set(this.skin_saved);
                }
                else
                {
                    // set default skin and set console flag to show
                    // console must be rendered only in default skin
                    this.skin_saved = this.skin;
                    this.skin_set(this.skin_default);
                    this.isShowDevConsole = true;
                }
            }
            if (checkKeyCode("dumpcamera"))
            {
                this.dump_camera();
            }
            if (checkKeyCode("dumpscene"))
            {
                this.dump_scene();
            }
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

            ConfigEntryBase value = Config[new BepInEx.Configuration.ConfigDefinition(section, key)];

            if (value.BoxedValue is bool option)
            {
                return option;
            }
            else
            {
                return (bool)value.DefaultValue;
            }
        }

        public void return_to_start_screen_clear()
        {
            this.clear_timers();
            //self.reset() # before init_start_params to call before_scene_unload event
            this._unload_scene_before();
            // no resetting scene!
            this.init_start_params();
            this.return_to_start_screen();
        }

        public int set_timer(float duration, GameFunc timerFuncEnd, TimerUpdateFunc timerFuncUpd = null)
        {
            Logger.LogDebug("Start set_timer!");
            int i;
            for (i = 0; i < timers.Length; i++)
            {
                if (timers[i] is null)
                {
                    Timer timer = new Timer
                    {
                        timeLeft = duration,
                        duration = duration,
                        funcEnd = timerFuncEnd,
                        updateFunc = timerFuncUpd
                    };

                    timers[i] = timer;
                    return i;
                }
            }
            return -1;
        }

        public void clear_timer(int index, bool runEndFunc = false)
        {
            if (index < this.timers.Length)
            {
                if (runEndFunc && this.timers[index] is Timer t)
                {
                    t.funcEnd(this);
                }
                this.timers[index] = null;
            }
        }

        public void return_to_start_screen()
        {
            this.skin_set_byname("skin_default");
            this.set_text("s", this._vnStText);
            //this.set_buttons(this._vnStButtons, this._vnStButtonsActions); TODO
        }

        public void clear_timers()
        {
            // not calling end function
            this.timers = new Timer[8];
        }

        //self.OnGUI(self)
        public string vnText
        {
            get
            {
                return this._vnText;
            }
            set
            {
                this._vnText = value;
            }
        }

        //self.OnGUI(self)
        // ---- external game functions ---------
        public List<Button_s> vnButtons
        {
            get
            {
                return this._vnButtons;
            }
            set
            {
                this._vnButtons = value;
            }
        }

        public void set_buttons(List<Button_s> buttons, string style = "normal")
        {
            this.vnButtonsStyle = style;
            if (style == "normal")
            {
                this.maxBtnsBeforeSeparator = this.skin.maxButtonsNormal;
            }
            if (style == "compact")
            {
                this.maxBtnsBeforeSeparator = this.skin.maxButtonsCompact;
            }
            if (buttons.Count <= this.maxBtnsBeforeSeparator)
            {
                // normal case, not so much btns
                this._btnSepCounter = 0;
                this.vnButtons = buttons;
            }
            else
            {
                this._btnSepCounter = 0;
                this.btnsFull = buttons;
                this._btnCallSepCounter(this, 0);
                //self.OnGUI(self)
            }
        }

        public void _btnCallFull(VNController game, int param)
        {
            this.call_game_func(this.btnsFull[param]);
        }

        public void _btnCallSepCounter(VNController game, int param)
        {
            // wrapping over list
            if (param > this.btnsFull.Count - 1)
            {
                param = 0;
            }
            // get sublist
            var endindex = param + this.maxBtnsBeforeSeparator - 1;
            if (endindex > this.btnsFull.Count)
            {
                endindex = this.btnsFull.Count;
            }
            var ar1 = this.btnsFull.GetRange(param, this.maxBtnsBeforeSeparator - 1);
            //print param
            //print endindex
            //print ar1
            var ar2 = new List<Button_s>();
            foreach (var i in Enumerable.Range(0, ar1.Count))
            {
                ar2.Add(new Button_s("Button", this._btnCallFull, param + i));
            }
            // add button to move forward
            ar2.Add(new Button_s(">>", this._btnCallSepCounter, param + this.maxBtnsBeforeSeparator - 1));
            // setting buttons
            this.set_buttons(ar2, this.vnButtonsStyle);
        }

        
        public void set_buttons_alt(List<Button_s> arButTextsActions, string style = "normal")
        {
            this.set_buttons(arButTextsActions, style);
        }
        
        
        public void set_buttons_end_game()
        {
            var buttons = new List<Button_s>() { new Button_s("End Game & Return >>", this._onEndGame, -1) };
            this.set_buttons(buttons);
        }       

        public void _onEndGame(VNController game, int i)
        {
            this.return_to_start_screen_clear();
        }

        public void set_text(string character, string text)
        {
            var char0 = character.Split('/')[0];
            this.curCharText = char0;
            this.curCharFull = character;
            if (text.StartsWith("!"))
            {
                this.vnText = text.Substring(1);
            }
            else
            {
                this.vnText = text;
            }
            //self.OnGUI(self)
            if (this.onSetTextCallback != null)
            {
                this.onSetTextCallback(this, character, text);
            }
            this.event_dispatch("set_text", new RegisteredChar_s(character, text));
        }

        public void set_text_s(string text)
        {
            this.set_text("s", text);
        }

        public void register_char(string name, string color, string showname)
        {
            this.registeredChars[name] = new RegisteredChar_s(color, showname);
        }

        public void texts_next(Dictionary<int, List<Action>> nexttexts, GameFunc endfunc)
        {
            //this.nextTexts = nexttexts; TODO
            this.endNextTextFunc = endfunc;
            this.NextText(this);
        }

        public void NextText(VNController game)
        {
            return;
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

        public void show_window()
        {
            this.visible = true;
        }

        public void hide_window()
        {
            this.visible = false;
        }

        public void show_blocking_message(string text = "...")
        {
            this.funcLockedText = text;
            this.isFuncLocked = true;
        }

        public void hide_blocking_message(VNController game = null)
        {
            this.isFuncLocked = false;
        }

        public void show_blocking_message_time(string text = "...", int duration = 3)
        {
            this.show_blocking_message(text);
            this.set_timer(duration, this.hide_blocking_message);
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
        public void reset()
        {
            return;
        }

        public void call_game_func(Utils.Button_s param)
        {
            try
            {
                param.btnCallFull(this, param.v);
            }
            catch (Exception e)
            {
                Logger.LogError("Error in call_game_func: " + e.ToString());
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
                return;
            }
            catch (Exception e)
            {
                Logger.LogError("Error in call_game_func: " + e.ToString());
            }
        }

        public void call_game_func(Action<VNNeoController> a)
        {
            try
            {
                a((VNNeoController)this);
                return;
            }
            catch (Exception e)
            {
                Logger.LogError("Error in call_game_func: " + e.ToString());
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
                Logger.LogError("Error in call_game_func: " + e.ToString());
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
                Logger.LogError("Error in call_game_func: " + e.ToString());
            }
        }

        // Load scene from file
        public abstract void load_scene(string file);

        // Return dir, where engine saves scenes
        public abstract string get_scene_dir();

        public abstract void dump_camera();

        public abstract void dump_scene();

        public CamData get_camera_num(int camnum)
        {
            this.show_blocking_message_time("ERROR: get_camera_num was not implemented");
            return this.camparams2vec(new Vector3(), new Vector3(), new Vector3());
        }

        // 
        //         Made animation movement to camera with some number
        //         :param float duration: Duration of animation in seconds
        //         :param int camnum: Camera number to animate from current position
        //         :param * style: may be an object or string. String can be linear,slow-fast,fast-slow,slow-fast3,fast-slow3,slow-fast4,fast-slow4. Object may vary
        //         :param Callable onCameraEnd: function that wil called after animation end
        //         
        public void anim_to_camera_num(float duration, int camnum, string style = "linear", GameFunc onCameraEnd = null)
        {
            //self.show_blocking_message_time("ERROR: anim_to_camera_num was not implemented")
            this.anim_to_camera_obj(duration, this.get_camera_num(camnum), style, onCameraEnd);
        }

        public List<Actor> scene_get_all_females()
        {
            return new List<Actor>();
        }

        public List<Actor> scene_get_all_males()
        {
            return new List<Actor>();
        }

        public void debug_print_all_chars()
        {
            var fems = this.scene_get_all_females();
            Console.WriteLine("-- Female scene chars: --");
            foreach (var i in Enumerable.Range(0, fems.Count))
            {
                Console.WriteLine(String.Format("{0}: {1}", i.ToString(), fems[i].text_name));
            }
            fems = this.scene_get_all_males();
            Console.WriteLine("-- Male scene chars: --");
            foreach (var i in Enumerable.Range(0, fems.Count))
            {
                Console.WriteLine(String.Format("{0}: {1}", i.ToString(), fems[i].text_name));
            }
            this.show_blocking_message_time("Debug: list of chars printed in console!");
        }

        public delegate void MenuFunc(VNController controller, Dictionary<string, string> param);

        // ---------- menu functions -------------------------
        public void run_menu(MenuFunc menufunc, Dictionary<string, string> menuparam, GameFunc onEndFunc)
        {
            this._menuStack.Add(onEndFunc);
            menufunc(this, menuparam);
        }

        public void menu_finish(int result)
        {
            this.menu_result = result;
            GameFunc endFunc = _menuStack.Last();
            _menuStack.RemoveAt(-1);
            this.call_game_func(endFunc);
        }

        public bool isClassicStudio
        {
            get
            {
                return base.engine_name == "studio";
            }
        }

        public bool isStudioNEO
        {
            get
            {
                return this.engine_name == "neo";
            }
        }

        public bool isNEOV2
        {
            get
            {
                return this.engine_name == "neov2";
            }
        }

        public bool isCharaStudio
        {
            get
            {
                return this.engine_name == "charastudio";
            }
        }

        public bool isPlayHomeStudio
        {
            get
            {
                return this.engine_name == "phstudio";
            }
        }

        // ---------- checking for engine types --------------
        // -------- other ----------
        public void scene_set_bg_png(object filepng)
        {
            this.show_blocking_message_time("ERROR: scene_set_bg_png was not implemented");
        }

        // ---------- cameras ----------

        public void move_camera(CamData cam)
        {
            this.move_camera_direct(cam);
        }

        public void move_camera(Vector3? pos = null, Vector3? distance = null, Vector3? rotate = null, float fov = 23.0f)
        {
            //self.show_blocking_message_time("ERROR: move_camera was not implemented")
            CamData camobj = this.camparams2vec(pos, distance, rotate, fov);
            this.move_camera_obj(camobj);
        }

        public abstract void move_camera_direct(CamData cam);

        public abstract void move_camera_direct(Vector3? pos = null, Vector3? distance = null, Vector3? rotate = null, float? fov = null);

        public void move_camera_obj(CamData camobj)
        {
            var camv = this.cam2vec(camobj);
            this.move_camera_direct(camv.position, camv.distance, camv.rotation, camv.fov);
        }

        public CamData camparams2vec(Vector3? pos, Vector3? distance, Vector3? rotate, float fov = 23.0f)
        {
            var obj = new CamData();
            if (pos is Vector3 pos_vec)
            {
                obj.position = pos_vec;
            }
            if (distance is Vector3 dist_vec)
            {
                obj.distance = dist_vec;
            }
            if (rotate is Vector3 rot_vec)
            {
                obj.rotation = rot_vec;
            }
            obj.fov = fov;
            return obj;
        }

        public CamData cam2vec(CamData camobj)
        {
            return this.camparams2vec(camobj.position, camobj.distance, camobj.rotation, camobj.fov);
        }

        public void anim_to_camera(
            float duration,
            Vector3? pos = null,
            Vector3? distance = null,
            Vector3? rotate = null,
            float fov = 23.0f,
            string style = "linear",
            GameFunc onCameraEnd = null)
        {
            var camobj = this.camparams2vec(pos, distance, rotate, fov);
            this.anim_to_camera_obj(duration, camobj, style, onCameraEnd);
        }

        public void anim_to_camera_obj(float duration, CamData camobj, string style = "linear", GameFunc onCameraEnd = null)
        {
            this._anim_to_camera_savecurrentpos();
            // print "Anim to cam 1"
            // print "Anim to cam 2"
            var camobjv = this.cam2vec(camobj);
            this.camTPos = camobjv.position;
            this.camTDir = camobjv.distance;
            this.camTAngle = camobjv.rotation;
            this.camTFOV = camobjv.fov;
            this.camAnimStyle = style;
            this.camAnimFullStyle = null;
            // camera animation one timer only
            animation_cam_timer(duration, onCameraEnd);
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
            if (this.camAnimeTID != -1)
            {
                this.clear_timer(this.camAnimeTID);
            }
            this.camAnimeTID = this.set_timer(duration, this._anim_to_cam_end, this._anim_to_cam_upd);
            this._onCameraEnd = onCameraEnd;
            if (this.isHideWindowDuringCameraAnimation)
            {
                this.hide_window();
            }
        }

        public void _anim_to_cam_upd(VNController game, float dt, float time, float duration)
        {
            var camProgress = time / duration;
            if (this.camAnimStyle == "linear")
            {
                camProgress = time / duration;
            }
            if (this.camAnimStyle == "slow-fast")
            {
                camProgress = Mathf.Pow(camProgress, 2);
            }
            if (this.camAnimStyle == "fast-slow")
            {
                camProgress = 1 - Mathf.Pow(1 - camProgress, 2);
            }
            if (this.camAnimStyle == "slow-fast3")
            {
                camProgress = Mathf.Pow(camProgress, 3);
            }
            if (this.camAnimStyle == "fast-slow3")
            {
                camProgress = 1 - Mathf.Pow(1 - camProgress, 3);
            }
            if (this.camAnimStyle == "slow-fast4")
            {
                camProgress = Mathf.Pow(camProgress, 4);
            }
            if (this.camAnimStyle == "fast-slow4")
            {
                camProgress = 1 - Mathf.Pow(1 - camProgress, 4);
            }
            var TPos = this.camTPos;
            var TDir = this.camTDir;
            Vector3 TAngle = this.camTAngle;
            if (this.camAnimFullStyle != null)
            {
                if (this.camAnimFullStyle.ContainsKey("target_camera_zooming_in"))
                {
                    TDir = new Vector3(TDir.x, TDir.y, TDir.z - this.camAnimFullStyle["target_camera_zooming_in"] * (1 - time / duration));
                }
                if (this.camAnimFullStyle.ContainsKey("target_camera_rotating_z"))
                {
                    TAngle = new Vector3(TAngle.x, TAngle.y, TAngle.z + this.camAnimFullStyle["target_camera_rotating_z"] * (1 - time / duration));
                }
                if (this.camAnimFullStyle.ContainsKey("target_camera_rotating_x"))
                {
                    TAngle = new Vector3(TAngle.x + this.camAnimFullStyle["target_camera_rotating_x"] * (1 - time / duration), TAngle.y, TAngle.z);
                }
                if (this.camAnimFullStyle.ContainsKey("target_camera_rotating_y"))
                {
                    TAngle = new Vector3(TAngle.x, TAngle.y + this.camAnimFullStyle["target_camera_rotating_y"] * (1 - time / duration), TAngle.z);
                }
                if (this.camAnimFullStyle.ContainsKey("target_camera_posing_y"))
                {
                    TPos = new Vector3(TPos.x, TPos.y + this.camAnimFullStyle["target_camera_posing_y"] * (1 - time / duration), TPos.z);
                    // TDir.z = TDir.z + self.camAnimFullStyle["move_distance"] * time / duration
                    // TDir.z = TDir.z + (-20)
                    // print "z: %s"%(str(TDir.z))
                }
            }
            var pos = Vector3.Lerp(this.camSPos, TPos, camProgress);
            var distance = Vector3.Lerp(this.camSDir, TDir, camProgress);
            var rotate = Vector3.Slerp(this.camSAngle, TAngle, camProgress);
            var fov = Mathf.Lerp(this.camSFOV, this.camTFOV, camProgress);
            //print fov, self.camSFOV, self.camTFOV, camProgress
            this.move_camera_direct(pos, distance, rotate, fov);
        }

        public void _anim_to_cam_end(VNController game)
        {
            // game.set_text("Anim camera end!")
            // print "Anim camera end!"
            if (this.isHideWindowDuringCameraAnimation)
            {
                this.show_window();
            }
            this.camAnimeTID = -1;
            if (this._onCameraEnd != null)
            {
                this.call_game_func(this._onCameraEnd);
            }
            return;
        }

        public void _anim_to_camera_savecurrentpos()
        {
            CamData camobj = this.get_camera_num(0);
            this.camSPos = camobj.position;
            this.camSDir = camobj.distance;
            this.camSAngle = camobj.rotation;
            this.camSFOV = camobj.fov;
        }

        public Vector3 vec3(float x, float y, float z)
        {
            return new Vector3(x, y, z);
        }

        // ---- automaking list of games -----
        public void prepare_auto_games()
        {
            this.prepare_auto_games_prefix(this, "");
        }

        public void prepare_auto_games_prefix(VNController game, string prefix)
        {
            return;
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


        public void game_start_fromfile() { }

        public void game_start_fromfile(object game, string gamefilestr) { }
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
            this._event_create_lisarray_ifneeded(eventid);
            this._eventListenerDic[eventid].Add(func);
        }

        public void _event_create_lisarray_ifneeded(string eventid)
        {
            if (this._eventListenerDic.ContainsKey(eventid))
            {
                return;
            }
            else
            {
                //this._eventListenerDic[eventid] = new List<Action>(); TODO
            }
        }

        public object event_unreg_listener(string eventid, GameFunc func)
        {
            this._event_create_lisarray_ifneeded(eventid);
            if (this._eventListenerDic[eventid].Contains(func))
            {
                this._eventListenerDic[eventid].Remove(func);
                return true;
            }
            return false;
        }

        public void event_dispatch(string eventid, object param)
        {
            if (this._eventListenerDic.ContainsKey(eventid))
            {
                foreach (var func in this._eventListenerDic[eventid])
                {
                    //func(this, eventid, param); TODO
                }
            }
        }

        public void _load_scene_before(string file)
        {
            this._unload_scene_before();
            this.event_dispatch("before_scene_load", file);
        }

        public void _unload_scene_before()
        {
            this.event_dispatch("before_scene_unload", null);
            this.scenedata = new GData();
        }

        // -------- game persistent data ----------
        public string gpersdata_getfilename()
        {
            var dstfile = Utils.combine_path(this.pygamepath, "Gpdata", this.current_game + "_p.dat");
            return dstfile;
        }

        public bool gpersdata_exists()
        {
            var dstfile = this.gpersdata_getfilename();
            return File.Exists(dstfile);
        }

        public void gpersdata_load()
        {
            object msg;
            try
            {
                var dstfile = this.gpersdata_getfilename();
                if (this.gpersdata_exists())
                {

                    //this.gpersdata = pickle.load(f2);

                    msg = "gpersdata loaded!";
                }
                else
                {
                    msg = "gpersdata not exists!";
                }
            }
            catch (Exception e)
            {
                msg = "gpersdata load Failed: " + e.ToString();
            }
            Console.WriteLine(msg);
        }

        public object gpersdata_save()
        {
            try
            {
                var dstfile = this.gpersdata_getfilename();

                //pickle.dump(this.gpersdata, f);
                return "";

            }
            catch (Exception e)
            {
                var msg = "gpersdata save Failed: " + e.ToString();
                Console.WriteLine(msg);
                return msg;
                // game.show_blocking_message_time(msg)
            }
        }

        public object gpersdata_set(string param, List<object> val)
        {
            this.gpersdata[param] = val;
            return this.gpersdata_save();
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
                var dstfile = this.gpersdata_getfilename();
                File.Delete(dstfile);
                this.gpersdata = new Dictionary<string, object>();
                msg = "gpersdata cleared!";
            }
            catch (Exception e)
            {
                msg = "gpersdata clear Failed: " + e.ToString();
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
            this.visible = true;
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
                this.skin_set(skin);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error loading skin ", skinname, ", error: ", e);
            }
        }

        public SkinBase skin_get_current()
        {
            return this.skin;
        }
    }

}
