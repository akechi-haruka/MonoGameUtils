using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OAS.UI.Screens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OAS.UI.Graphics.Animators {
    public interface IAnimator {

        public bool Paused { get; set; }

        public void Update(Screen screen, GameTime gameTime);

    }
}
