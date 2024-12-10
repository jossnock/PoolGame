using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PoolGame;

namespace PoolGame.Classes
{
    public class CueBall : PoolBall
    {

        public CueBall(Texture2D texture, float radius) : base(texture, radius)
        {
            acceleration = Vector2.Zero;
            int cueBallCentreX = (Game1.windowWidth + (6 * Game1.pocketRadius) + (3 * Game1.tablePocketSpacing)) / 5; // 1/5th across the playing surface (not 1/5th across entire table)
            position = new Vector2(cueBallCentreX, Game1.windowHeight / 2);
        }

        public void Shoot(Vector2 mousePosition)
        {
            Vector2 movementVector = mousePosition - position; // calculatingh the distance between the cue ball and the mouse to form a direction with some magnitude

            velocity += movementVector * VelocityMultiplier;
        }

        public void DebugMove(Vector2 mousePosition)
        {
            velocity = Vector2.Zero;
            position = mousePosition;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);


            Vector2 currentMousePosition = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);

            if ((Mouse.GetState().LeftButton == ButtonState.Pressed) 
              & (Game1.IsAllStationary() == true)) // only allowed to shoot again when everything is stationary
              // [todo: keep in window]: & )currentMousePosition.X > 0 & currentMousePosition.X < Game1.windowWidth & currentMousePosition.Y > 0 & currentMousePosition.Y < Game1.windowHeight)
            {
                Shoot(currentMousePosition);
            };

            if (Mouse.GetState().RightButton == ButtonState.Pressed)
            {
                DebugMove(currentMousePosition);
            }
        }
    }
}
