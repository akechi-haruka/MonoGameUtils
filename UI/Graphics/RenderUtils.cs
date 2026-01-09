using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using OAS.Util.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OAS.UI.Graphics {
    public class RenderUtils {
        
        public static String WrapText(SpriteFont Font, String Text, int maxWidth = -1, int maxLines = 99) {
            Log.Write("Wrap Text: " + maxWidth + "px, " + maxLines, "Debug");
            if (maxWidth < 0) {
                maxWidth = Program.Main.Width;
            }
            if (Font.MeasureString(Text).X < maxWidth) {
                //return Text;
            }

            string[] words = Text.Split(' ');
            StringBuilder wrappedText = new StringBuilder();
            float linewidth = 0f;
            float spaceWidth = Font.MeasureString(" ").X;
            int lines = 1;
            for (int i = 0; i < words.Length; ++i) {
                string word = words[i];
                Vector2 size = Font.MeasureString(word);
                if (linewidth + size.X < maxWidth) {
                    if (word.Contains('\n')) {
                        if (++lines > maxLines) {
                            wrappedText.Append(word.Substring(0, word.IndexOf("\n")));
                            wrappedText.Append("...");
                            break;
                        } else {
                            linewidth = size.X + spaceWidth;
                        }
                    } else {
                        linewidth += size.X + spaceWidth;
                    }
                } else {
                    lines += 2;
                    if (lines > maxLines) {
                        wrappedText.Append("...");
                        break;
                    } else {
                        wrappedText.Append('\n');
                        linewidth = size.X + spaceWidth;
                    }
                }
                wrappedText.Append(word);
                wrappedText.Append(' ');
            }

            return wrappedText.ToString();
        }

        public static int CalculateTextLines(int height, int fontSize) {
            return height / fontSize;
        }
    }
}
