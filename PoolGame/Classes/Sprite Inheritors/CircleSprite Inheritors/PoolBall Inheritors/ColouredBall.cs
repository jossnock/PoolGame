using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoolGame.Classes
{
    public class ColouredBall : PoolBall
    {
        public enum ballColour
        {
            red,
            yellow,
        }
        public ballColour colour;

        public ColouredBall(Texture2D texture, Vector2 initialPosition, float radius, bool isRed) : base(texture, initialPosition, radius)
        {
            if (isRed)
            {
                colour = ballColour.red;
            }
            else
            {
                colour = ballColour.yellow;
            }
        }
    }
}
