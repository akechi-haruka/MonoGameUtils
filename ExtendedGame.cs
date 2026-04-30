using Haruka.Common;
using Haruka.Common.Configuration;
using Haruka.MonoGameUtils.Input;
using Haruka.MonoGameUtils.Input.Api;
using Haruka.MonoGameUtils.Input.Builtin;
using Haruka.MonoGameUtils.UI.Elements;
using Haruka.MonoGameUtils.UI.Graphics.Animators;
using Haruka.MonoGameUtils.UI.Resources;
using Haruka.MonoGameUtils.UI.Screens;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using SpriteFontPlus;

namespace Haruka.MonoGameUtils;

public abstract class ExtendedGame : Game {
    public const string SECTION_MAIN_WINDOW = "MainWindow";

    public static ExtendedGame Instance { get; private set; }

    internal static ILogger ResourceLog;

    public IniFile Configuration { get; }
    public Screen CurrentScreen { get; private set; }
    public Screen Overlay { get; private set; }
    public Screen DrawScreen { get; private set; }
    public int Width { get; private set; }
    public int Height { get; private set; }
    public bool Running { get; private set; }
    public double Framerate { get; protected set; }
    public Skin Skin { get; set; } = new Skin.NullSkin();
    public InputManager InputManager { get; }

    public Thread LogicThread { get; private set; }
    public Thread RenderThread { get; private set; }

    internal DateTime LastSecond = DateTime.Now;

    private readonly GraphicsDeviceManager graphics;
    private SpriteBatch spriteBatch;
    private readonly Lock drawScreenLock = new Lock();
    private readonly Dictionary<string, object> resourceCache = new Dictionary<string, object>();
    private readonly List<Action> logicThreadInvoke = new List<Action>();
    private readonly bool dynamicResize;
    private readonly int originalWidth;
    private readonly int originalHeight;

    private int framesThisSecond = 1;
    private string currentMusic;
    private readonly ManualResetEvent gameExitWaiter = new ManualResetEvent(false);

    protected ExtendedGame(IniFile config, string windowTitle, int width = 0, int height = 0, bool borderless = false, params IInputAPI[] customInputs) {
        Instance = this;
        Configuration = config;
        ResourceLog = Log.GetOrCreate("Rsrc");

        graphics = new GraphicsDeviceManager(this) {
            IsFullScreen = false,
            HardwareModeSwitch = false
        };
        graphics.ApplyChanges();

        Running = true;
        Width = width;
        Height = height;
        if (Width <= 0 || Height <= 0) {
            Width = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            Height = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            borderless = true;
        }

        SetResolutionFull(Width, Height, borderless);
        UpdateAnchors();
        Log.Main.LogDebug("Graphics window created");

        InputManager = new InputManager(Configuration, customInputs);
        InputManager.StartAllInputs();

        Overlay = new EmptyScreen();

        IsMouseVisible = InputManager.GetInput<MouseInput>() != null || Configuration.ReadBool("ForceMouseVisible", SECTION_MAIN_WINDOW);

        Window.AllowAltF4 = Configuration.ReadBool("AllowAltF4", SECTION_MAIN_WINDOW, true);
        Window.AllowUserResizing = Configuration.ReadBool("AllowWindowResize", SECTION_MAIN_WINDOW, true);
        dynamicResize = Configuration.ReadBool("DynamicResize", SECTION_MAIN_WINDOW, true);
        SetFPS(Configuration.ReadInt("RefreshRate", SECTION_MAIN_WINDOW, 60));

        originalWidth = Width;
        originalHeight = Height;

        Window.Title = windowTitle;

        Window.TextInput += Window_TextInput;
        Deactivated += ExtendedGame_Deactivated;
        Activated += ExtendedGame_Activated;
        Exiting += ExtendedGame_Exiting;

        Content.RootDirectory = Directory.Exists("../Content") ? Path.GetFullPath("..\\Content") : "Content";

        new Thread(LogPerformance) {
            Name = "Watchdog"
        }.Start();
    }


    protected override void LoadContent() {
        Window.ClientSizeChanged += Window_ClientSizeChanged;

        spriteBatch = new SpriteBatch(GraphicsDevice);

        Skin = new Skin(this, Configuration.ReadString("Skin", SECTION_MAIN_WINDOW, "default"));
        Skin.Load();

        RecreateRenderPositions();

        ChangeScreen(GetInitialScreen());
    }

    protected abstract Screen GetInitialScreen();

    protected abstract void HandleError(Exception ex);

    #region Resource Load Functions

    public string GetSkinnedPath(string path) {
        return "User" + Path.DirectorySeparatorChar + "Skins" + Path.DirectorySeparatorChar + Skin.FolderName + Path.DirectorySeparatorChar + path;
    }

    private void CheckEvilThreads() {
        if (Thread.CurrentThread == RenderThread) {
            ResourceLog.LogWarning("Loading resource on render thread!!");
        } else if (Thread.CurrentThread == LogicThread) {
            ResourceLog.LogWarning("Loading resource on logic thread!!");
        }
    }

    public T Load<T>(string path) {
        if (resourceCache.TryGetValue(path, out object value)) {
            ResourceLog.LogTrace("Loading from cache: " + path);
            return (T)value;
        }

        ResourceLog.LogDebug("Loading: " + path);

        CheckEvilThreads();

        T result = default;
        try {
            result = Content.Load<T>(path);
        } catch (Exception e) when (e is ContentLoadException or InvalidDataException) {
            ResourceLog.LogError("Resource access failed: " + e.Message);
        }

        if (result == null) {
            ResourceLog.LogWarning("Load failed: " + path);
        } else {
            resourceCache.Add(path, result);
        }

        return result;
    }

    public Texture2D LoadTexture(string path) {
        Texture2D tex;
        string skinPath = GetSkinnedPath(path) + ".png";
        if (File.Exists(skinPath)) {
            ResourceLog.LogInformation("Loading (local): " + skinPath);
            FileStream fs = null;
            try {
                fs = new FileStream(skinPath, FileMode.Open);
                tex = Texture2D.FromStream(GraphicsDevice, fs);
            } catch (IOException ex) {
                ResourceLog.LogWarning("Failed reading skinned texture: " + ex.Message);
                ResourceLog.LogInformation("Loading regular texture...");
                tex = Load<Texture2D>(path);
            } finally {
                fs?.Close();
            }
        } else {
            tex = Load<Texture2D>(path);
        }

        if (tex == null) {
            tex = Skin.NoTexture;
        }

        return tex;
    }

    public Song LoadBGM(string path) {
        Song muz;
        string[] extList = new string[] { ".mp3", ".ogg", ".wav" };
        string skinPath = null;
        foreach (string ext in extList) {
            string pathTest = GetSkinnedPath(path) + ext;
            if (File.Exists(pathTest)) {
                skinPath = pathTest;
            }
        }

        if (skinPath != null) {
            ResourceLog.LogInformation("Loading (local): " + skinPath);
            try {
                muz = Song.FromUri(skinPath, new Uri(skinPath, UriKind.Relative));
            } catch (IOException ex) {
                ResourceLog.LogDebug(ex.ToString());
                ResourceLog.LogWarning("Failed reading skinned song: " + ex.Message);
                ResourceLog.LogInformation("Loading regular song...");
                muz = Load<Song>(path);
            }
        } else {
            muz = Load<Song>(path);
        }

        return muz;
    }

    public SoundEffect LoadSound(string path) {
        SoundEffect muz;
        string skinPath = GetSkinnedPath(path) + ".wav";
        if (File.Exists(skinPath)) {
            ResourceLog.LogInformation("Loading (local): " + skinPath);
            try {
                muz = SoundEffect.FromStream(new FileStream(skinPath, FileMode.Open, FileAccess.Read, FileShare.Read));
            } catch (IOException ex) {
                ResourceLog.LogDebug(ex.ToString());
                ResourceLog.LogWarning("Failed reading skinned sound effect: " + ex.Message);
                ResourceLog.LogInformation("Loading regular sound effect...");
                muz = Load<SoundEffect>(path);
            }
        } else {
            muz = Load<SoundEffect>(path);
        }

        return muz;
    }

    public SpriteFont LoadSpriteFont(string path, int size = 0, bool includeCJK = false, string ext = ".ttf") {
        if (size == 0) {
            size = Skin.DefaultFontHeight;
        }

        SpriteFont sf = null;
        string skinPath = GetSkinnedPath(path) + ext;
        ResourceLog.LogDebug("skinpath = " + skinPath);
        if (File.Exists(skinPath)) {
            ResourceLog.LogInformation("Loading (local): " + skinPath);
            path = skinPath;
        } else {
            string checkpath = Content.RootDirectory + "/" + path + ext;
            if (File.Exists(checkpath)) {
                ResourceLog.LogInformation("Loading: " + checkpath);
                path = checkpath;
            } else {
                return Load<SpriteFont>(path);
            }
        }

        CheckEvilThreads();

        try {
            List<CharacterRange> cr = new List<CharacterRange> {
                CharacterRange.BasicLatin,
                CharacterRange.Latin1Supplement
            };
            if (includeCJK) {
                cr.Add(CharacterRange.Hiragana);
                cr.Add(CharacterRange.Katakana);
            }

            TtfFontBakerResult fontBakeResult = TtfFontBaker.Bake(File.ReadAllBytes(path), size, 1024, 1024, cr);
            sf = fontBakeResult.CreateSpriteFont(GraphicsDevice);
        } catch (Exception ex) {
            ResourceLog.LogError("Resource access failed: " + ex.Message);
        }

        if (sf == null) {
            ResourceLog.LogWarning("Load failed: " + path);
        }

        return sf;
    }

    public void PlayMusic(string path) {
        if (currentMusic != path) {
            MediaPlayer.IsRepeating = true;
            Song muz = LoadBGM(path);
            if (muz != null) {
                MediaPlayer.Play(muz);
                currentMusic = path;
            }
        }
    }

    public void StopMusic() {
        currentMusic = null;
        MediaPlayer.Stop();
    }

    public void PlaySound(string path) {
        SoundEffect se = LoadSound(path);
        se?.Play();
    }

    #endregion

    #region Window Management

    public void SetResolutionFull(int w, int h, bool borderless = false, bool exclusiveFullscreen = false) {
        Log.Main.LogInformation("Setting resolution to " + w + "x" + h);
        SetExclusiveFullscreen(exclusiveFullscreen);
        SetBorderless(borderless);
        SetWindowSize(w, h, false);
        ApplyWindowChanges();
        RecreateRenderPositions();
    }

    public void SetWindowSize(int width, int height, bool immediatelyApply = true) {
        Log.Main.LogInformation("Setting window size to " + width + "x" + height);
        if (width > 0 && height > 0) {
            SetRenderSize(width, height);
            if (immediatelyApply) {
                ApplyWindowChanges();
            }
        }
    }

    public void SetExclusiveFullscreen(bool exclusiveFullscreen) {
        Log.Main.LogInformation("Setting exclusive fullscreen to " + exclusiveFullscreen);
        graphics.HardwareModeSwitch = exclusiveFullscreen;
    }

    public void SetRenderSize(int width, int height) {
        Log.Main.LogInformation("Setting render size to " + width + "x" + height);
        if (width > 0 && height > 0) {
            Width = graphics.PreferredBackBufferWidth = width;
            Height = graphics.PreferredBackBufferHeight = height;
        }
    }

    public void SetBorderless(bool borderless) {
        Log.Main.LogInformation("Setting borderless mode to " + borderless);
        Window.IsBorderless = borderless;
        if (borderless) {
            SetWindowPosition(new Point(0, 0));
        } else {
            int sw = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            int sh = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            SetWindowPosition(new Point(sw / 2 - Width / 2, sh / 2 - Height / 2));
        }
    }

    public void SetWindowPosition(Point position) {
        Log.Main.LogInformation("Setting window position to x=" + position.X + "/y=" + position.Y);
        Window.Position = position;
    }

    public void ApplyWindowChanges() {
        Log.Main.LogInformation("Applying graphic changes");
        graphics.ApplyChanges();
    }

    public void RecreateRenderPositions() {
        Log.Main.LogInformation("Re-calculating rendering positions");
        UpdateAnchors();
        QueueOnLogicThread(() => {
            CurrentScreen?.OnGameResized();
            Overlay?.OnGameResized();
        });
    }

    public void SetFPS(int val) {
        if (val <= 0) {
            Log.Main.LogInformation("Setting FPS limit to infinite");
            IsFixedTimeStep = false;
        } else {
            Log.Main.LogInformation("Setting FPS limit to " + val);
            IsFixedTimeStep = true;
            TargetElapsedTime = TimeSpan.FromSeconds(1D / val);
        }
    }

    private void Window_ClientSizeChanged(object sender, EventArgs e) {
        int w = Window.ClientBounds.Width;
        int h = Window.ClientBounds.Height;
        Log.Main.LogInformation("Window resized to " + w + "x" + h);
        if (dynamicResize) {
            SetRenderSize(w, h);
            RecreateRenderPositions();
        } else {
            SetRenderSize(originalWidth, originalHeight);
        }

        ApplyWindowChanges();
    }

    private void UpdateAnchors() {
        int w = Width;
        int h = Height;
        Screen.TopLeft = new Point(0, 0);
        Screen.TopCenter = new Point(w / 2, 0);
        Screen.TopRight = new Point(w, 0);
        Screen.MiddleLeft = new Point(0, h / 2);
        Screen.MiddleCenter = new Point(w / 2, h / 2);
        Screen.MiddleRight = new Point(w, h / 2);
        Screen.BottomLeft = new Point(0, h);
        Screen.BottomMiddle = new Point(w / 2, h);
        Screen.BottomRight = new Point(w, h);
        Screen.TopSafeFrame = new Point(Screen.TopLeft.X + Screen.SAFE_FRAME_SIZE, Screen.TopLeft.Y + Screen.SAFE_FRAME_SIZE);
        Screen.TopCenterSafeFrame = new Point(w / 2, Screen.TopSafeFrame.Y);
        Screen.BottomSafeFrame = new Point(Screen.BottomLeft.X + Screen.SAFE_FRAME_SIZE, Screen.BottomLeft.Y - Screen.SAFE_FRAME_SIZE);
        Screen.BottomCenterSafeFrame = new Point(w / 2, Screen.BottomSafeFrame.Y - Screen.SAFE_FRAME_SIZE);
        Screen.BottomCenterSafeFrameT = new Point(Screen.BottomCenterSafeFrame.X, Screen.BottomCenterSafeFrame.Y - Skin.DefaultFontHeight);
        Screen.BottomLeftSafeFrameT = new Point(Screen.SAFE_FRAME_SIZE, Screen.BottomCenterSafeFrame.Y);
        Screen.BottomRightSafeFrameT = new Point(w - Screen.SAFE_FRAME_SIZE, Screen.BottomCenterSafeFrame.Y);
        Screen.ScreenRect = new Rectangle(0, 0, w, h);
    }

    private void LogPerformance() {
        do {
            if (Framerate < 45) {
                Log.Main.LogWarning("Low FPS reported: " + Framerate.ToString("N1"));
            }

            gameExitWaiter.WaitOne(10000);
        } while (Running);
    }

    private void ExtendedGame_Activated(object sender, EventArgs e) {
        Log.Main.LogInformation("Got focus");
        InputManager.IsFocused = true;
    }

    private void ExtendedGame_Deactivated(object sender, EventArgs e) {
        Log.Main.LogInformation("Lost focus!");
        InputManager.IsFocused = false;
    }

    private void ExtendedGame_Exiting(object sender, ExitingEventArgs e) {
        Log.Main.LogInformation("Exit event received!");
        Running = false;
        gameExitWaiter.Set();
    }

    private void Window_TextInput(object sender, TextInputEventArgs e) {
        if (CurrentScreen != null) {
            Screen s = CurrentScreen;
            s.OnKeyboardTypeEvent(e);
        }
    }

    #endregion

    #region Game loops

    protected override void Update(GameTime gameTime) {
        if (LogicThread == null) {
            LogicThread = Thread.CurrentThread;
        }

        try {
            InputManager.EarlyUpdate(gameTime);

            lock (logicThreadInvoke) {
                while (logicThreadInvoke.Count > 0) {
                    Action lti = logicThreadInvoke[0];
                    logicThreadInvoke.RemoveAt(0);
                    lti.Invoke();
                }
            }

            CurrentScreen?.Update(gameTime);
            Overlay?.Update(gameTime);

            InputManager.LateUpdate(gameTime);
        } catch (Exception ex) {
            HandleError(ex);
        }

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime) {
        GraphicsDevice.Clear(Skin.SystemBackgroundColor);

        if (RenderThread == null) {
            RenderThread = Thread.CurrentThread;
        }

        DateTime currentSec = DateTime.Now;
        if (currentSec.Second != LastSecond.Second) {
            Framerate = framesThisSecond;
            LastSecond = currentSec;
            framesThisSecond = 1;
        } else {
            framesThisSecond++;
        }
        //Framerate = 1 / gameTime.ElapsedGameTime.TotalSeconds;

        try {
            lock (drawScreenLock) {
                spriteBatch.Begin();
                if (Skin?.BackgroundTexture != null) {
                    spriteBatch.Draw(Skin.BackgroundTexture, Screen.ScreenRect, Color.White);
                }

                DrawScreen?.Draw(gameTime, spriteBatch);
                Overlay?.Draw(gameTime, spriteBatch);
            }
        } catch (Exception ex) {
            HandleError(ex);
        } finally {
            spriteBatch.End();
        }

        base.Draw(gameTime);
    }

    public void QueueOnLogicThread(Action a) {
        lock (logicThreadInvoke) {
            logicThreadInvoke.Add(a);
        }
    }

    #endregion

    #region Screen navigation

    public void ChangeScreen(Screen next) {
        if (LogicThread != null && Thread.CurrentThread != LogicThread) {
            QueueOnLogicThread(() => ChangeScreen(next));
            return;
        }

        Log.Main.LogInformation("Moving to: " + next?.GetType());
        if (CurrentScreen != null) {
            Log.Main.LogInformation("Closing: " + CurrentScreen.GetType());
            CurrentScreen.CloseScreen();
        }

        Screen prev = CurrentScreen;

        CurrentScreen = next;
        if (CurrentScreen != null) {
            Log.Main.LogInformation("Opening: " + CurrentScreen.GetType());
            CurrentScreen.ResetScreenElements();
            CurrentScreen.OpenScreen(CurrentScreen.IgnoreOnScreenStack ? prev?.PreviousScreen : prev);
        } else {
            Log.Main.LogError("Now loading a null screen!");
        }

        lock (drawScreenLock) {
            DrawScreen = CurrentScreen;
        }
    }

    public void ChangeOverlay(Screen next) {
        if (LogicThread != null && Thread.CurrentThread != LogicThread) {
            QueueOnLogicThread(() => ChangeOverlay(next));
            return;
        }

        Log.Main.LogInformation("Overlay moving to: " + next?.GetType());
        if (Overlay != null) {
            Log.Main.LogInformation("Overlay closing: " + Overlay.GetType());
            Overlay.CloseScreen();
        }

        if (next != null) {
            Log.Main.LogInformation("Overlay opening: " + next.GetType());
            next.OpenScreen(null);
        } else {
            Log.Main.LogError("Now loading a null overlay!");
        }

        lock (drawScreenLock) {
            Overlay = next;
        }
    }

    #endregion

    #region Global UI Elements

    public void NotifyPopup(string header, string value) {
        ElementPopup popup = new ElementPopup(header, value);
        popup.AddAnimator(new FadeAnimator(popup, popup, 5000, 1000));
        Overlay?.AddElement(popup);
    }

    public void OpenDialog(String message, Action<int?> callback = null, params String[] options) {
        Dialog dlg = new Dialog(message, options);
        if (callback != null) {
            dlg.OnClose += callback;
        }

        OpenDialog(dlg);
    }

    public void OpenDialog(Dialog dlg) {
        if (LogicThread != null && Thread.CurrentThread != LogicThread) {
            QueueOnLogicThread(() => OpenDialog(dlg));
            return;
        }

        Overlay?.AddElement(dlg);
        InputManager.ResetInputStates();
    }

    public void OpenTextInput(string title, string @default, Action<string> callback, bool allowCancel = true) {
        OpenDialog(new TextInputDialog(title, @default, callback, allowCancel));
    }

    #endregion
}