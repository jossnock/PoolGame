using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Myra.Graphics2D.UI;

namespace PoolGame.Classes
{
    internal class MainMenu : Screen
    {
        public MainMenu() { }

        public override void Update(GameTime gameTime)
        {

        }

        public override void Draw(GameTime gameTime)
        {
            Game1._desktop.Render(); // renders Myra UI elements
        }
    }
}
