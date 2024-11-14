using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace PoolGame.Classes
{
    internal class PoolBall : CollideObject
    {
        private Vector2 velocity;
        private Vector2 acceleration;
        public Vector2 decelerationDueToFriction;

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

            this.decelerationDueToFriction = this.velocity * 0.01f;

        }

        public void DoFriction()
        {
            // doing friction:

            // horizontal:
            if (this.velocity.X > 0.1f | this.velocity.X < -0.1f) // checking if PoolBall is moving a notable amount, if so it does friction
            {
                if (this.velocity.X > 0)
                {
                    this.velocity.X -= decelerationDueToFriction.X;
                }
                if (this.velocity.X < 0)
                {
                    this.velocity.X -= decelerationDueToFriction.X;
                }
            }
            else
            {
                this.velocity.X = 0f; // stops PoolBall if it is moving very slowly to stop friction from making it go backwards
            }

            // vertical:
            if (this.velocity.Y > 0.1f | this.velocity.Y < -0.1f) // checking if PoolBall is moving a notable amount, if so it does friction
            {
                if (this.velocity.Y > 0)
                {
                    this.velocity.Y -= decelerationDueToFriction.Y;
                }
                if (this.velocity.Y < 0)
                {
                    this.velocity.Y -= decelerationDueToFriction.Y;
                }
            }
            else
            {
                this.velocity.Y = 0f; // stops PoolBall if it is moving very slowly to stop friction from making it go backwards
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
                this.decelerationDueToFriction.Y = -this.decelerationDueToFriction.Y;
            }

            // with bottom:
            if (this.position.Y + this.radius > height)
            {
                this.position = new Vector2(this.position.X, height - this.radius);
                this.velocity.Y = -this.velocity.Y;
                this.decelerationDueToFriction.Y = -this.decelerationDueToFriction.Y;
            }

            // with left:
            if (this.position.X - this.radius < 0)
            {
                this.position = new Vector2(this.radius, this.position.Y);
                this.velocity.X = -this.velocity.X;
                this.decelerationDueToFriction.X = -this.decelerationDueToFriction.X;
            }

            // with right:
            if (this.position.X + this.radius > width)
            {
                this.position = new Vector2(width - this.radius, this.position.Y);
                this.velocity.X = -this.velocity.X;
                this.decelerationDueToFriction.X = -this.decelerationDueToFriction.X;
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

            DoFriction();

            ChangePosition();

            DoCircleBoundsCollision(720, 1280);
        }
    }
}

