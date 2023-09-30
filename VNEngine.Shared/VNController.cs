using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BepInEx.Configuration;
using UnityEngine;
using static VNEngine.Utils;
using BepInEx.Logging;
using BepInEx;


//using WindowsInput;
//using WindowsInput.Native;

namespace VNEngine
{
    [BepInProcess(Constants.StudioProcessName)]
    [BepInPlugin(GUID, PluginName, Version)]
    public class VNController :
        BaseUnityPlugin
    {
        public const string PluginName = "VN Controller";
        public const string GUID = "com.kasanari.bepinex.vncontroller";
        public const string Version = "1.0";
        internal new static ManualLogSource Logger;
        public delegate void GameFunc(VNController controller);

        public GameFunc runScAct;

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


        public List<Button_s> _vnButtons = new List<Button_s>();
        private List<Button_s> _vnStButtons;

        public string _vnStText;

        public string _vnText;

        public string[] arKeyKodes;

        public string btnNextText;

        public List<Button_s> btnsFull;



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

        public Timer[] timers;

        public UpdateFunc updFunc;

        public string updFuncParam;

        public SkinBase textBox;

        public ButtonStyle vnButtonsStyle;
        public int wheight;

        public GUI.WindowFunction windowCallback;

        public Rect windowRect;

        public GUIStyle windowStyle;

        public GUIStyle windowStyleDefault;

        public int wwidth;

        public GameFunc _onCameraEnd;

        //private Component component;
        //private int counter;
        //private bool show_buttons;
        public bool visible;
        //private GUIStyle style;
        private CameraControl cameraControl;
        private bool lastCameraState = true;
        private GameCursor gameCursor;
        public string windowName;
        //private GUI.WindowFunction windowCallback;
        //private bool isClassicStudio;
        protected Dictionary<string, string> engineOptions;

        protected string pygamepath;

        public ManualLogSource GetLogger { get { return Logger; } }

        public bool checkKeyCode(string configkey)
        {
            var entry = new ConfigDefinition("Keyboard Shortcuts", configkey);
            if (entry != null)
            {
                if (Config.ContainsKey(entry))
                {
                    BepInEx.Configuration.KeyboardShortcut shortcut = (BepInEx.Configuration.KeyboardShortcut)Config[entry].BoxedValue;
                    return shortcut.IsDown();
                }
            }
            return false;
        }

        public bool CheckConfigEntry(string category, string key)
        {
            return true;
        }

        public void loadConfig()
        {
            Config.Bind("Keyboard Shortcuts", "ToggleVNControllerWindow", new BepInEx.Configuration.KeyboardShortcut(KeyCode.F8, KeyCode.LeftControl), "Show or hide the VN Controller window in Studio.");
            Config.Bind("Keyboard Shortcuts", "Reset", new BepInEx.Configuration.KeyboardShortcut(KeyCode.F3, KeyCode.LeftControl), "Reset VN Controller.");
            Config.Bind("Keyboard Shortcuts", "ReloadCurrentGame", new BepInEx.Configuration.KeyboardShortcut(KeyCode.F10), "Reload current game.");
            Config.Bind("Keyboard Shortcuts", "VNFrameDeveloperConsole", new BepInEx.Configuration.KeyboardShortcut(KeyCode.F5, KeyCode.LeftControl), "Show or hide the VN Controller window in Studio");
            Config.Bind("Keyboard Shortcuts", "DumpCamera", new BepInEx.Configuration.KeyboardShortcut(KeyCode.F4, KeyCode.LeftControl, KeyCode.LeftAlt), "Show or hide the VN Controller window in Studio");
            Config.Bind("Keyboard Shortcuts", "DeveloperConsole", new BepInEx.Configuration.KeyboardShortcut(KeyCode.F4, KeyCode.LeftControl), "Show or hide the VN Controller window in Studio");
            Config.Bind("Keyboard Shortcuts", "ReloadVNEngine", new BepInEx.Configuration.KeyboardShortcut(KeyCode.F1, KeyCode.LeftControl), "Show or hide the VN Controller window in Studio");
            Config.Bind("Skins", "usekeysforbuttons", false);
        }

        public void EnableCamera(bool value)
        {
            if (this.lastCameraState != value)
            {
                lastCameraState = value;
                if (cameraControl)
                {
                    cameraControl.enabled = value;
                }
                gameCursor.enabled = value;
            }
        }



        public void ResetWindow(int windowid)
        {
            //Workaround mouse/camera issues when dragging window
            if (GUIUtility.hotControl == 0)
            {
                EnableCamera(true);
            }

            if (Event.current.type == EventType.MouseDown)
            {
                GUI.FocusControl("");
                GUI.FocusWindow(windowid);
                EnableCamera(false);
            }
        }

        public void Start()
        {

            try
            {
                if (!gameCursor)
                {
                    gameCursor = UnityEngine.Object.FindObjectOfType<GameCursor>();
                }
            }


            catch (Exception)
            {
                //Logger.LogError("VNGE: passable error in Start:" + exception); //TODO
            }
        }
        /*
        public void OnGUI()
        {
            if (!visible)
            {
                return;
            }
            else
            {
                windowRect = GUI.Window(0, windowRect, windowCallback, windowName);
            }
        }
        */

    public VNController()
        {

            gameObject.AddComponent<StudioController>();

            Logger = base.Logger;
            //component = null; // will be assigned if exists as member
            //counter = 1;
            //show_buttons = false;
            visible = false;
            cameraControl = null;
            windowRect = new Rect(Screen.width / 2 - 50, Screen.height / 2 - 50, 400, 400);
            lastCameraState = true;
            gameCursor = null;
            windowName = "Window";
            //loading options
            loadConfig();
            //print self.engineOptions;
            Instance = this;
            arKeyKodes = null; //Utils.getEngineOptions()["keysforbuttons"].Split(',');
            vnButtonsStyle = ButtonStyle.Normal;
            //this.visible = this.engineOptions["starthidden"] == "0";
            // self.wwidth = 500
            // self.wheight = 230 
            //
            // self.windowName = ''
            // self.windowRect = Rect (Screen.width / 2 - self.wwidth / 2, Screen.height - self.wheight - 10, self.wwidth, self.wheight)
            //self.skin_panel_unity = CloneSkin(GUI.)
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
            _vnButtons = new List<Button_s>();
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

        public static VNController Instance { get; private set; }

        public void init_start_params()
        {
            isShowDevConsole = false;
            // menu
            _menuStack = new List<GameFunc>();
            isHideGameButtons = false;
            onSetTextCallback = null;
            onDumpSceneOverride = null;
            isHideWindowDuringCameraAnimation = false;
            isFuncLocked = false;
            funcLockedText = "SYSTEM: Unknown default lock";
            // some settings - may be localized
            btnNextText = "Next >";
            //self.autostart = False
            //self.isDevDumpButtons = False - no use
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
        }

        public void FuncWindowGUI(int windowid)
        {
            if (!isFuncLocked)
            {
                TextBox.render_main(curCharFull, vnText, vnButtons, vnButtonsStyle);
            }
            else
            {
                // render system message
                TextBox.render_system(funcLockedText);
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


        public void Update()
        {
            // Update is called less so better place to check keystate

            if (checkKeyCode("ToggleVNControllerWindow"))
            {
                visible = !visible;
            }

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
            event_dispatch("update", null);
        }


        public bool GetConfigEntry(string section, string key)
        {
            var value = Config[new ConfigDefinition(section, key)];

            if (value.BoxedValue is bool option)
                return option;
            return (bool) value.DefaultValue;
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

        public void clear_timers()
        {
            // not calling end function
            timers = new Timer[8];
        }

        public void set_buttons(List<Button_s> buttons, ButtonStyle style = ButtonStyle.Normal)
        {
            vnButtonsStyle = style;
            if (style == ButtonStyle.Normal) maxBtnsBeforeSeparator = TextBox.maxButtonsNormal;
            if (style == ButtonStyle.Compact) maxBtnsBeforeSeparator = TextBox.maxButtonsCompact;
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

        public void reset()
        {
            _unload_scene_before();
            //studio.InitScene(false);
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

        internal void HideVNBox(VNController game)
        {
            visible = false;
        }

        public void ShowVNTextBox(List<Button_s> buttons)
        {
            var rpySkin = new VNTextBox
            {
                isEndButton = true,
                endButtonTxt = "X",
                endButtonCall = HideVNBox
            };
            TextBox = rpySkin;
            set_text_s("");
            set_buttons(buttons);
            visible = true;
        }

        public void SetText(string character, string text)
        {
            var char0 = character.Split('/')[0];
            curCharText = char0;
            curCharFull = character;
            vnText = text.StartsWith("!") ? text.Substring(1) : text;
            //self.OnGUI(self)
            if (onSetTextCallback != null) onSetTextCallback(this, character, text);
            event_dispatch("set_text", new RegisteredChar_s(character, text));
        }

        public void set_text_s(string text)
        {
            SetText("s", text);
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

        public void call_game_func(Action<VNController> a)
        {
            try
            {
                a(this);
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

        // ---- automaking list of games -----
        public void prepare_auto_games()
        {
            prepare_auto_games_prefix(this, "");
        }

        public void prepare_auto_games_prefix(VNController game, string prefix)
        {
        }

        /*public object prepare_auto_games_prefix(StudioController game, string prefix)
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

        public SkinBase TextBox
        {
            set
            {
                this.textBox = value;
                this.textBox.setup(this);
            }
            get
            {
                return this.textBox;
            }
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