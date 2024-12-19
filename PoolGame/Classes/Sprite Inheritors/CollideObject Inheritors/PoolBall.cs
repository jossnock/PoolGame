using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using PoolGame;

namespace PoolGame.Classes
{
    public class PoolBall : CircleSprite
    {
        // all PoolBall objects have a mass of 1,
        // meaning that a PoolBall's velocity is always equal to its momentum
        // and its acceleration is always equal to the resultant force acting upon it,
        // so I will only use velocity and acceleration in calculations for simplicity.

        public Vector2 velocity { get; set; }
        public const float VelocityMultiplier = 1 / 25f;
        public const float ThresholdVelicity = 0.05f;

        public Vector2 acceleration { get; set; }

        public Vector2 decelerationDueToRollingResistance { get; set; }
        public const float coefficientOfRollingResistance = 3 * 0.01f; // between 0.005 - 0.015

        public const float poolBallpoolBallCoefficientOfRestitution = 0.9f; // between 0.92 - 0.98
        public const float poolBallCushionCoefficientOfRestitution = 0.8f; // between 0.75 - 0.85


        public PoolBall(Texture2D texture, Vector2 initialPosition, float radius) : base(texture, initialPosition, radius)
        {
            velocity = Vector2.Zero;
            acceleration = Vector2.Zero;
            position = initialPosition;
            radius = Match.poolBallRadius;
        }

        public PoolBall(Texture2D texture, float radius) : base(texture, radius) // allowing CueBall to have a constructor that doesn't need initialPosition
        {
            velocity = Vector2.Zero;
            acceleration = Vector2.Zero;
            position = Vector2.Zero;
            radius = Match.poolBallRadius;
        }

        public void ApplyRollingResistance() // [todo: separate into 2 methods, one for resistance, one to stop when slow enough]
        {
            // horizontal:
            if (Math.Abs(velocity.X) > 0) // if x velocity is less than 0,
                                          // decelerating causes the velocity to change sign,
                                          // meaning it moves backwards (relative to its original direction) which isn't how friction works
            {
                velocity = new Vector2(velocity.X - decelerationDueToRollingResistance.X, velocity.Y); // decelerating
            }
            else
            {
                decelerationDueToRollingResistance = new Vector2(0, decelerationDueToRollingResistance.Y); // prevents friction from changing x component of velocity when it's zero
            }

            // vertical:
            if (Math.Abs(velocity.Y) > 0) // if x velocity is less than 0,
                                          // decelerating causes the velocity to change sign,
                                          // meaning it moves backwards (relative to its original direction) which isn't how friction works
            {
                velocity = new Vector2(velocity.X, velocity.Y - decelerationDueToRollingResistance.Y); // decelerating
            }
            else
            {
                decelerationDueToRollingResistance = new Vector2(decelerationDueToRollingResistance.X, 0); // prevents friction from changing y component of velocity when it's zero
            }
        }

        /// <summary>
        /// If velocity in a certain direction is under a given threshold, the PoolBall will stop moving in that direction.
        /// </summary>
        public void StopWhenSlow()
        {
            // horizontal:
            if (Math.Abs(velocity.X) < ThresholdVelicity)
            {
                velocity = new Vector2(0, velocity.Y);
                decelerationDueToRollingResistance = new Vector2(0, decelerationDueToRollingResistance.Y);
            }

            // vertical:
            if (Math.Abs(velocity.Y) < ThresholdVelicity)
            {
                velocity = new Vector2(velocity.X, 0);
                decelerationDueToRollingResistance = new Vector2(decelerationDueToRollingResistance.X, 0);
            }
        }

        public void ChangePosition()
        {
            if (velocity.Length() > 0)
            {
                decelerationDueToRollingResistance = Vector2.Normalize(velocity) * coefficientOfRollingResistance; // friction = coefficientOfFriction * reaction force
                                                                                                                   // letting mass and gravitational field strength equal 1, friction = coefficientOfFriction (acting in the opposite direction to motion)
            }                                                                                                      // (normalising velocity allows only its direction to be used, with coefficientOfFriction as the magnitude)

            position += velocity;
        }

        public void DoBoundsCollision() // [remove, soon to be obsolete due to Cushions.cs]
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
                decelerationDueToRollingResistance = new Vector2(decelerationDueToRollingResistance.X, -decelerationDueToRollingResistance.Y);
            }

            // with bottom:
            // -------------
            // |           |
            // |           |
            // |     O     |
            // -------------
            if (position.Y + radius > Game1.windowHeight)
            {
                position = position = new Vector2(position.X, Game1.windowHeight - radius); // keeping in bounds if it clips out
                velocity = new Vector2(velocity.X, -velocity.Y); // reversing part of it to give the effect of an elastic collision
                decelerationDueToRollingResistance = new Vector2(decelerationDueToRollingResistance.X, -decelerationDueToRollingResistance.Y); // reversing part of it to prevent it from speeding up PoolBall
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
                decelerationDueToRollingResistance = new Vector2(-decelerationDueToRollingResistance.X, decelerationDueToRollingResistance.Y); // reversing part of it to prevent it from speeding up PoolBall
            }

            // with right:
            // -------------
            // |           |
            // |         O |
            // |           |
            // -------------
            if (position.X + radius > Game1.windowWidth)
            {
                position = new Vector2(Game1.windowWidth - radius, position.Y); // keeping in bounds if it clips out
                velocity = new Vector2(-velocity.X, velocity.Y); // reversing part of it to give the effect of an elastic collision
                decelerationDueToRollingResistance = new Vector2(-decelerationDueToRollingResistance.X, decelerationDueToRollingResistance.Y); // reversing part of it to prevent it from speeding up PoolBall
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            StopWhenSlow();

            DoBoundsCollision();

            ApplyRollingResistance();

            ChangePosition();

            
        }
    }
}

