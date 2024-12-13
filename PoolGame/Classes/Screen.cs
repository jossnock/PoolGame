using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PoolGame.Classes
{
    public abstract class Screen
    {
        public abstract void Update(GraphicsDevice graphicsDevice, GameTime gameTime);

        public abstract void Draw(GameTime gameTime);
    }
}
