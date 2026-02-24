using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Haruka.MonoGameUtils.UI.Graphics;

public class RenderUtils {
    public static string WrapText(SpriteFont font, string text, int maxWidth = -1, int maxLines = 99) {
        ExtendedGame.ResourceLog.LogDebug("Wrap Text: " + maxWidth + "px, " + maxLines);
        if (maxWidth < 0) {
            maxWidth = ExtendedGame.Instance.Width;
        }

        if (font.MeasureString(text).X < maxWidth) {
            //return Text;
        }

        string[] words = text.Split(' ');
        StringBuilder wrappedText = new StringBuilder();
        float linewidth = 0f;
        float spaceWidth = font.MeasureString(" ").X;
        int lines = 1;
        foreach (string word in words) {
            Vector2 size = font.MeasureString(word);
            if (linewidth + size.X < maxWidth) {
                if (word.Contains('\n')) {
                    if (++lines > maxLines) {
                        wrappedText.Append(word.Substring(0, word.IndexOf('\n')));
                        wrappedText.Append("...");
                        break;
                    }

                    linewidth = size.X + spaceWidth;
                } else {
                    linewidth += size.X + spaceWidth;
                }
            } else {
                lines += 2;
                if (lines > maxLines) {
                    wrappedText.Append("...");
                    break;
                }

                wrappedText.Append('\n');
                linewidth = size.X + spaceWidth;
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