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

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            MouseState currentMouseState = Mouse.GetState();

            if ((currentMouseState.LeftButton == ButtonState.Pressed) & (velocity == Vector2.Zero)) // only allowed to input movement when stationary
            {
                Shoot();
            }

            previousMouseState = currentMouseState;
        }
    }
}
