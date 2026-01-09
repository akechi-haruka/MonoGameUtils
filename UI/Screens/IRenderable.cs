using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OAS.UI.Screens {
    public interface IRenderable<T> {

        public void Draw(Program game, SpriteBatch s, GameTime t, float xOffset, float yOffset, T owner, bool selected, int maxOptionNameWidth);

    }
}
