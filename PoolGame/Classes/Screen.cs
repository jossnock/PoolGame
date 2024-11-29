using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace PoolGame.Classes
{
    public abstract class Screen
    {
        public abstract void Update(GameTime gameTime);

        public abstract void Draw(GameTime gameTime);
    }
}
