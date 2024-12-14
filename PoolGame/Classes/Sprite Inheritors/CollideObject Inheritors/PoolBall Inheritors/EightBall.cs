using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoolGame.Classes
{
    public class EightBall : PoolBall
    {
        public EightBall(Texture2D texture, float radius) : base(texture, radius)
        {
            position = new Vector2((4f / 5) * ((Game1.windowWidth - (4 * Match.pocketRadius) - (2 * Match.tablePocketSpacing))), Game1.windowHeight / 2);
            // positioned 1/5th the width of the playing surface, (derivation in writeup)
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            //[todo: insert game-end logic here]
        }
    }
}