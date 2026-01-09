using Haruka.Common.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Haruka.MonoGameUtils.UI.Resources;

public class Skin {
    public const string TAG = nameof(Skin);

    public string FolderName { get; }
    public string Name { get; }
    public IniFile Configuration { get; }
    public SpriteFont DefaultFont { get; set; }
    public SpriteFont DefaultFontSmall { get; set; }
    public int DefaultFontHeight { get; set; }
    public SpriteFont FallbackFont { get; private set; }

    public int DialogBorderSize { get; set; } = 5;

    public Color SystemBackgroundColor { get; set; } = Color.Black;
    public Color SystemBorderColor { get; set; } = Color.White;
    public Color SelectedItemColor { get; set; } = Color.Red;
    public Color UnselectedItemColor { get; set; } = Color.White;

    public Color TextColor {
        get { return UnselectedItemColor; }
    }

    public Color DisabledItemColor { get; set; } = Color.Gray;
    public Color ModifiedItemColor { get; set; } = Color.Green;
    public Color OriginalItemColor { get; set; } = Color.Yellow;

    public Texture2D NoTexture { get; private set; }
    public Texture2D BackgroundTexture { get; private set; }

    private readonly ExtendedGame ExtendedGame;

    public Skin(ExtendedGame ExtendedGame, string name) {
        FolderName = name;
        Configuration = IniFile.New("User/Skins/" + name + "/Skin.ini");
        Name = Configuration.ReadString("Name", TAG, "Unnamed Skin");
        this.ExtendedGame = ExtendedGame;
    }

    public void Load() {
        if (FolderName == null) {
            return;
        }

        int fontSize = Configuration.ReadInt("FontSize", TAG, 30);
        FallbackFont = ExtendedGame.Load<SpriteFont>("Fonts/Fallback");
        NoTexture = ExtendedGame.Load<Texture2D>("NoTexture");

        if (FallbackFont == null) {
            ExtendedGame.ResourceLog.LogError(" !!! FAILED TO LOAD FALLBACK FONT FILE !!!");
            ExtendedGame.ResourceLog.LogError(" !!! CAN'T CONTINUE !!!");
            throw new IOException("Failed to load fallback font file");
        }

        DefaultFont = ExtendedGame.LoadSpriteFont("Fonts/Default", fontSize, true);

        if (DefaultFont == null) {
            ExtendedGame.ResourceLog.LogWarning("Failed to load default font file, using fallback.");
            DefaultFont = FallbackFont;
        }

        DefaultFontSmall = ExtendedGame.LoadSpriteFont("Fonts/Default", Configuration.ReadInt("FontSizeSmall", TAG, 20));

        if (DefaultFontSmall == null) {
            ExtendedGame.ResourceLog.LogWarning("Failed to load default font file, using fallback.");
            DefaultFontSmall = FallbackFont;
        }

        DefaultFontHeight = (int)DefaultFont.MeasureString("ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789").Y;
        ExtendedGame.ResourceLog.LogDebug("Font Size = " + DefaultFontHeight);

        DialogBorderSize = Configuration.ReadInt("DialogBorderSize", TAG, 5);
        SystemBackgroundColor = new Color(ColorParse(Configuration.ReadString("SystemBackground", TAG, "0,0,0")));
        SystemBorderColor = new Color(ColorParse(Configuration.ReadString("SystemBorderColor", TAG, "255,255,255")));
        SelectedItemColor = new Color(ColorParse(Configuration.ReadString("SelectedItemColor", TAG, "255,0,0")));
        UnselectedItemColor = new Color(ColorParse(Configuration.ReadString("UnselectedItemColor", TAG, "255,255,255")));
        DisabledItemColor = new Color(ColorParse(Configuration.ReadString("DisabledItemColor", TAG, "128,128,128")));
        ModifiedItemColor = new Color(ColorParse(Configuration.ReadString("ModifiedItemColor", TAG, "255,255,0")));
        OriginalItemColor = new Color(ColorParse(Configuration.ReadString("OriginalItemColor", TAG, "0,255,0")));

        string bgtex = Configuration.ReadString("SystemBackgroundImage", TAG);
        if (bgtex != null) {
            BackgroundTexture = ExtendedGame.LoadTexture(bgtex);
        }

    }

    private static uint ColorParse(string str) {
        string[] parts = str.Split(',');
        if (parts.Length != 3) {
            throw new ArgumentException("invalid color format: " + str);
        }

        return UInt32.Parse(parts[0]) << 0 | UInt32.Parse(parts[1]) << 8 | UInt32.Parse(parts[2]) << 16 | ((uint)0xFF << 24);
    }

    internal class NullSkin : Skin {
        public NullSkin() : base(null, null) {
        }
    }
}