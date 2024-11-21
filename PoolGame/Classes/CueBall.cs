using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PoolGame;
using PoolGame.Classes.Screens;

namespace PoolGame.Classes
{
    internal class CueBall : PoolBall
    {
        private MouseState previousMouseState;

        public CueBall(Texture2D texture, float radius) : base(texture, radius)
        {
            acceleration = Vector2.Zero;
            position = new Vector2(MainMenu.windowWidth / 5, MainMenu.windowHeight / 2);
        }

        public void Shoot()
        {
            Vector2 mousePosition = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);
            Vector2 movementVector = mousePosition - position;

            velocity += movementVector * VelocityMultiplier;

        }

        public void DoCircleCircleCollision()
        {
            foreach (PoolBall poolBall in Match1.poolBalls)
            {
                if (poolBall == this) // no need to check if it collides with itself
                { continue; }
                else
                {
                    if (Vector2.Distance(poolBall.position, position) < radius * 2)
                    {
                        Vector2 initVelocity = velocity;
                        Vector2 stationaryBallDirection = (poolBall.position - position);
                        float velocityMultiplier = ((initVelocity.X * stationaryBallDirection.X) + (initVelocity.Y * stationaryBallDirection.Y)) 
                            / ((stationaryBallDirection.X * stationaryBallDirection.X) +(stationaryBallDirection.Y * stationaryBallDirection.Y));
                        poolBall.velocity = stationaryBallDirection * velocityMultiplier;
                        velocity = initVelocity - poolBall.velocity;
                    }
                }
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            DoCircleCircleCollision();

            MouseState currentMouseState = Mouse.GetState();

            if ((currentMouseState.LeftButton == ButtonState.Pressed) & (velocity == Vector2.Zero)) // only allowed to input movement when stationary
            {
                Shoot();
            }

            previousMouseState = currentMouseState;
        }
    }
}
