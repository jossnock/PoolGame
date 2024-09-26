using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace PoolGame.Entities
{
    internal class PoolBall : CollideObject
    {
        private Vector2 velocity;
        private Vector2 acceleration;
        public PoolBall(Texture2D texture, Vector2 position, float radius) : base(texture, position, radius)
        {
            this.acceleration = new Vector2(1f, 1f);
            this.position = new Vector2(1280 / 2, 720 / 2);
        }
        private MouseState previousMouseState;

        public void ChangeVelocity()
        {
            Vector2 destination = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);
            Vector2 movementVector = destination - this.position;

            const float VelocityMultiplier = 1280f / (1280f * 50f);

            this.velocity += Vector2.Multiply(movementVector, VelocityMultiplier);


            // doing friction:
            const float DecelerationDueToFriction = 0.01f;
            if (this.velocity.X > 0)
            {
                this.velocity.X -= DecelerationDueToFriction;
            }
            if (this.velocity.X < 0)
            {
                this.velocity.X += DecelerationDueToFriction;
            }
            if (this.velocity.Y > 0)
            {
                this.velocity.Y -= DecelerationDueToFriction;
            }
            if (this.velocity.Y < 0)
            {
                this.velocity.Y += DecelerationDueToFriction;
            }
        }

        

        public void ChangePosition()
        {
            this.position += this.velocity;
        }

        public void DoCircleBoundsCollision(int height, int width)
        {
            // with top:
            if (this.position.Y - this.radius < 0)
            {
                this.position = new Vector2(this.position.X, this.radius);
                this.velocity.Y = -this.velocity.Y;
            }

            // with bottom:
            if (this.position.Y + this.radius > height)
            {
                this.position = new Vector2(this.position.X, height - this.radius);
                this.velocity.Y = -this.velocity.Y;
            }

            // with left:
            if (this.position.X - this.radius < 0)
            {
                this.position = new Vector2(this.radius, this.position.Y);
                this.velocity.X = -this.velocity.X;
            }

            // with right:
            if (this.position.X + this.radius > width)
            {
                this.position = new Vector2(width - this.radius, this.position.Y);
                this.velocity.X = -this.velocity.X;
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            MouseState currentMouseState = Mouse.GetState();

            if ((currentMouseState.LeftButton == ButtonState.Pressed) & (this.velocity == Vector2.Zero))
            {
                ChangeVelocity();
            }

            previousMouseState = currentMouseState;


            ChangePosition();

            DoCircleBoundsCollision(720, 1280);
        }
    }
}

