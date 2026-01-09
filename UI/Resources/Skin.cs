using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OAS.Error;
using OAS.Util.Configuration;
using OAS.Util.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace OAS.UI.Resources {
    public class Skin {

        public const string TAG = nameof(Skin);

        public String FolderName { get; }
        public String Name { get; }
        public IniFile Configuration { get; }
        public SpriteFont DefaultFont { get; set; }
        public SpriteFont DefaultFontSmall { get; set; }
        public int DefaultFontHeight { get; set; }
        public SpriteFont FallbackFont { get; private set; }
        public SpriteFont CJKFont { get; set; }
        public SpriteFont CJKFontSmall { get; set; }
        public int CJKFontHeight { get; set; }
        public SpriteFont LogFont { get; private set; }

        public int DialogBorderSize { get; set; } = 5;

        public Color SystemBackgroundColor { get; set; } = Color.Black;
        public Color SystemBorderColor { get; set; } = Color.White;
        public Color SelectedItemColor { get; set; } = Color.Red;
        public Color UnselectedItemColor { get; set; } = Color.White;
        public Color TextColor {
            get {
                return UnselectedItemColor;
            }
        }
        public Color DisabledItemColor { get; set; } = Color.Gray;
        public Color ModifiedItemColor { get; set; } = Color.Green;
        public Color OriginalItemColor { get; set; } = Color.Yellow;
        public bool DefaultOverlayBottomIndicatorDisplay { get; set; } = true;
        public bool CardReaderIcon { get; set; } = true;
        public bool NetworkIcon { get; set; } = true;
        public bool DataIcon { get; set; } = true;
        public bool SignalIcon { get; set; } = true;
        public String BootSequenceScreen { get; set; }

        public Texture2D NoTexture;
        public Texture2D BackgroundTexture { get; private set; }

        private Program program;

        public Skin(Program program, String name) {

            this.FolderName = name;
            this.Configuration = IniFile.New("User/Skins/" + name + "/Skin.ini");
            this.Name = Configuration.ReadString("Name", TAG, "Unnamed Skin");
            this.program = program;
        }

        public void Load() {

            if (FolderName == null) {
                return;
            }

            int fontSize = Configuration.ReadInt("FontSize", TAG, 30);
            FallbackFont = program.Load<SpriteFont>("Fonts/Fallback");
            NoTexture = program.Load<Texture2D>("NoTexture");

            if (FallbackFont == null) {
                Log.WriteError(" !!! FAILED TO LOAD FALLBACK FONT FILE !!!");
                Log.WriteError(" !!! CAN'T CONTINUE !!!");
                throw new OASException(ErrorDictionary.Get(1000, 1009), "Fallback font file could not be loaded");
            }

            DefaultFont = program.LoadSpriteFont("Fonts/Default", fontSize, true);

            if (DefaultFont == null) {
                Log.WriteWarning("Failed to load font files, using fallback.");
                DefaultFont = FallbackFont;
            }

            DefaultFontSmall = program.LoadSpriteFont("Fonts/Default", Configuration.ReadInt("FontSizeSmall", TAG, 20));

            if (DefaultFont == null) {
                Log.WriteWarning("Failed to load font files, using fallback.");
                DefaultFont = FallbackFont;
            }

            DefaultFontHeight = (int)DefaultFont.MeasureString("ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789").Y;
            Log.Write("Font Size = " + DefaultFontHeight, "Debug");

            CJKFont = program.LoadSpriteFont("Fonts/SongNamesCJK", fontSize);

            if (CJKFont == null) {
                Log.WriteWarning("Failed to load CJK font, using the fallback font!");
                CJKFont = FallbackFont;
            }

            CJKFontHeight = (int)CJKFont.MeasureString("ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789").Y;

            CJKFontSmall = program.LoadSpriteFont("Fonts/CJKSmall");

            if (CJKFontSmall == null) {
                Log.WriteWarning("Failed to load small CJK font, using the fallback font!");
                CJKFontSmall = FallbackFont;
            }

            LogFont = program.LoadSpriteFont("Fonts/Log");

            if (LogFont == null) {
                Log.WriteWarning("Failed to load CJK font for logging, using the fallback font!");
                LogFont = FallbackFont;
            }

            DialogBorderSize = Configuration.ReadInt("DialogBorderSize", TAG, 5);
            SystemBackgroundColor = new Color(ColorParse(Configuration.ReadString("SystemBackground", TAG, "0,0,0")));
            SystemBorderColor = new Color(ColorParse(Configuration.ReadString("SystemBorderColor", TAG, "255,255,255")));
            SelectedItemColor = new Color(ColorParse(Configuration.ReadString("SelectedItemColor", TAG, "255,0,0")));
            UnselectedItemColor = new Color(ColorParse(Configuration.ReadString("UnselectedItemColor", TAG, "255,255,255")));
            DisabledItemColor = new Color(ColorParse(Configuration.ReadString("DisabledItemColor", TAG, "128,128,128")));
            ModifiedItemColor = new Color(ColorParse(Configuration.ReadString("ModifiedItemColor", TAG, "255,255,0")));
            OriginalItemColor = new Color(ColorParse(Configuration.ReadString("OriginalItemColor", TAG, "0,255,0")));
            DefaultOverlayBottomIndicatorDisplay = Configuration.ReadBool("DefaultOverlayBottomIndicatorDisplay", TAG, true);

            string bgtex = Configuration.ReadString("SystemBackgroundImage", TAG);
            if (bgtex != null) {
                BackgroundTexture = program.LoadTexture(bgtex);
            }

            CardReaderIcon = Configuration.ReadBool("CardReaderIcon", TAG, true);
            NetworkIcon = Configuration.ReadBool("NetworkIcon", TAG, true);
            DataIcon = Configuration.ReadBool("DataIcon", TAG, true);
            SignalIcon = Configuration.ReadBool("SignalIcon", TAG, true);

            BootSequenceScreen = Configuration.ReadString("BootSequenceScreen", TAG, null);
        }

        private static uint ColorParse(string str) {
            String[] parts = str.Split(',');
            if (parts.Length != 3) {
                throw new ArgumentException("invalid color format: " + str);
            }
            return (UInt32.Parse(parts[0])) << 0 | (UInt32.Parse(parts[1])) << 8 | (UInt32.Parse(parts[2])) << 16 | ((uint)0xFF << 24);
        }

        internal class NullSkin : Skin {

            public NullSkin() : base(null, null) {
            }

        }
    }
}
