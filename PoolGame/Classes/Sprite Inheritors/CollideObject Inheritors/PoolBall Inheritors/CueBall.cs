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
        public bool isPotted;
        public CueBall(Texture2D texture, float radius) : base(texture, radius)
        {
            acceleration = Vector2.Zero;
            int cueBallCentreX = (Game1.windowWidth + (6 * Game1.pocketRadius) + (3 * Game1.tablePocketSpacing)) / 5; // 1/5th across the playing surface (not 1/5th across entire table)
            position = new Vector2(cueBallCentreX, Game1.windowHeight / 2);
            isPotted = false;
        }

        public void Shoot(Vector2 mousePosition)
        {
            Vector2 movementVector = mousePosition - position; // calculating the distance between the cue ball and the mouse to form a direction vector
            velocity += movementVector * VelocityMultiplier;
        }

        /// <summary>
        /// Moves the cue ball to the specified position
        /// </summary>
        public void DebugMove(Vector2 _position)
        {
            velocity = Vector2.Zero;
            position = _position;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            Vector2 currentMousePosition = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);

            if (!isPotted) // can only move when not potted
            {
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

            if (isPotted & Game1.IsAllStationary() & (Mouse.GetState().LeftButton == ButtonState.Pressed)) // replace cue ball after the table is stationary and LMB is pressed
            {
                int cueBallCentreX = (Game1.windowWidth + (6 * Game1.pocketRadius) + (3 * Game1.tablePocketSpacing)) / 5; // 1/5th across the playing surface (not 1/5th across entire table)
                Vector2 inputPosition = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);

                // checking if it's in the left-most 5th of the board
                if ((inputPosition.X > (2 * Game1.pocketRadius) + Game1.tablePocketSpacing)
                  & (inputPosition.X < (Game1.windowWidth + (6 * Game1.pocketRadius) + (3 * Game1.tablePocketSpacing)) / 5)
                  & (inputPosition.Y > (2 * Game1.pocketRadius) + Game1.tablePocketSpacing)
                  & (inputPosition.Y < Game1.windowHeight - ((2 * Game1.pocketRadius) + Game1.tablePocketSpacing)))
                {
                    // checking if it would be inside of another PoolBall:
                    bool isColliding = false;
                    for (int i = 1; i < Game1.poolBalls.Count; i++) // starting at i = 1 skips the cue ball from checking itself
                    {
                        if (Vector2.Distance(inputPosition, Game1.poolBalls[i].position) < radius * 2) 
                        {
                            isColliding = true;
                            break; // once one is colliding, there's no point checking any more
                        }
                    }
                    if (!isColliding)
                    {
                        position = inputPosition;
                        isPotted = false;
                    }
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!isPotted) // can only be drawn when not potted
            {
                base.Draw(spriteBatch);
            }
        }
    }
}
