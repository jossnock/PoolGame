using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using PoolGame;
using PoolGame.Classes.Screens;
using System.Text.RegularExpressions;

namespace PoolGame.Classes
{
    internal class PoolBall : CollideObject
    {
        // all PoolBall objects have a mass of 1,
        // meaning that a PoolBall's velocity is always equal to its momentum
        // and its acceleration/deceleration is always equal to the resultant force acting upon it,
        // so I will only use velocityt and acceleration/deceleration in calculations for simplicity.

        public Vector2 velocity { get; set; }
        public const float VelocityMultiplier = 2 / (50f);

        public Vector2 acceleration { get; set; }
        public Vector2 decelerationDueToFriction { get; set; }
        public const float coefficientOfFriction = 0.01f;

        public PoolBall(Texture2D texture, Vector2 initialPosition, float radius) : base(texture, initialPosition, radius)
        {
            acceleration = Vector2.Zero;
            position = initialPosition;
        }

        public PoolBall(Texture2D texture, float radius) : base(texture, radius) // allowing CueBall to have a constructor that doesn't need initialPosition
        {
            acceleration = Vector2.Zero;
            position = Vector2.Zero;
        }

        public void DoFriction()
        {
            // horizontal:
            if (Math.Abs(velocity.X) > 0.1) // if x velocity is less than x decelerationDueToFriction,
                                            // decelerating causes the velocity to change sign,
                                            // meaning it moves backwards (relative to its original direction) which isn't how friction works
            {
                velocity = new Vector2(velocity.X - decelerationDueToFriction.X, velocity.Y); // decelerating
            }
            else
            {
                velocity = new Vector2(0, velocity.Y); // stops PoolBall if it is moving very slowly to stop friction from making it go backwards
                decelerationDueToFriction = new Vector2(0, decelerationDueToFriction.Y); // prevents friction from changing x component of velocity when it's zero
            }

            // vertical:
            if (Math.Abs(velocity.Y) > 0.1) // if x velocity is less than x decelerationDueToFriction,
                                            // decelerating causes the velocity to change sign,
                                            // meaning it moves backwards (relative to its original direction) which isn't how friction works
            {
                velocity = new Vector2(velocity.X, velocity.Y - decelerationDueToFriction.Y); // decelerating
            }
            else
            {
                velocity = new Vector2(velocity.X, 0); // stops PoolBall if it is moving very slowly to stop friction from making it go backwards
                decelerationDueToFriction = new Vector2(decelerationDueToFriction.X, 0); // prevents friction from changing y component of velocity when it's zero
            }
        }

        public void ChangePosition()
        {
            decelerationDueToFriction = velocity * coefficientOfFriction;

            position += velocity - decelerationDueToFriction;
        }

        public void DoBoundsCollision()
        {
            // with top:
            // -------------
            // |     O     |
            // |           |
            // |           |
            // -------------
            if (position.Y - radius < 0)
            {
                position = new Vector2(position.X, radius); // keeping in bounds if it clips out
                velocity = new Vector2(velocity.X, -velocity.Y); // reversing part of it to give the effect of an elastic collision
                decelerationDueToFriction = new Vector2(decelerationDueToFriction.X, -decelerationDueToFriction.Y);
            }

            // with bottom:
            // -------------
            // |           |
            // |           |
            // |     O     |
            // -------------
            if (position.Y + radius > MainMenu.windowHeight)
            {
                position = position = new Vector2(position.X, MainMenu.windowHeight - radius); // keeping in bounds if it clips out
                velocity = new Vector2(velocity.X, -velocity.Y); // reversing part of it to give the effect of an elastic collision
                decelerationDueToFriction = new Vector2(decelerationDueToFriction.X, -decelerationDueToFriction.Y); // reversing part of it to prevent it from speeding up PoolBall
            }

            // with left:
            // -------------
            // |           |
            // | O         |
            // |           |
            // -------------
            if (position.X - radius < 0)
            {
                position = new Vector2(radius, position.Y); // keeping in bounds if it clips out
                velocity = new Vector2(-velocity.X, velocity.Y); // reversing part of it to give the effect of an elastic collision
                decelerationDueToFriction = new Vector2(-decelerationDueToFriction.X, decelerationDueToFriction.Y); // reversing part of it to prevent it from speeding up PoolBall
            }

            // with right:
            // -------------
            // |           |
            // |         O |
            // |           |
            // -------------
            if (position.X + radius > MainMenu.windowWidth)
            {
                position = new Vector2(MainMenu.windowWidth - radius, position.Y); // keeping in bounds if it clips out
                velocity = new Vector2(-velocity.X, velocity.Y); // reversing part of it to give the effect of an elastic collision
                decelerationDueToFriction = new Vector2(-decelerationDueToFriction.X, decelerationDueToFriction.Y); // reversing part of it to prevent it from speeding up PoolBall
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            MouseState currentMouseState = Mouse.GetState();

            DoBoundsCollision();

            DoFriction();

            ChangePosition();
        }
    }
}

