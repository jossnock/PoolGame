using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoolGame;

namespace PoolGame.Classes
{
    public class ObjectBall : PoolBall
    {
        // TODO: replace bools with an enum
        private bool isStriped; // if false, then the ObjectBall is solid
        private bool isEight; // if true, the ObjectBall is the eight-ball

        // coloured ball constructor:
        public ObjectBall(Texture2D texture, Vector2 initialPosition, float radius, bool _isStriped) : base(texture, initialPosition, radius)
        {
            isStriped = _isStriped;
        }

        // eight ball constructor:
        public ObjectBall(Texture2D texture, float radius) : base(texture, radius)
        {
            isEight = true;
            position = new Vector2((4f / 5) * ((Game1.windowWidth - (4 * Game1.pocketRadius) - (2 * Game1.tablePocketSpacing))), Game1.windowHeight / 2);
            // positioned 1/5th the width of the playing surface, (derivation in writeup)
        }
    }
}
