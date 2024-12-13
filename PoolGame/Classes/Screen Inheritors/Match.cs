using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace PoolGame.Classes
{
    internal class Match : Screen
    {
        public Match() { }

        public static void LoadInitialContent(GraphicsDevice graphicsDevice)
        {
            // sizing:
            Game1.poolBallRadius = Game1.windowWidth / 64; // 20 for Game1.windowWidth = 1280
            Game1.pocketRadius = (9 * Game1.windowWidth) / 320; // 36 for Game1.windowWidth = 1280
            Game1.tablePocketSpacing = (Game1.windowWidth / 80); // 16 for Game1.windowWidth = 1280


            // PoolBalls:

            Game1.cueBallTexture = Game1.CreateBlockColouredCircleTexture(graphicsDevice, Game1.poolBallRadius, Color.White);
            Game1._cueBall = new(Game1.cueBallTexture, Game1.poolBallRadius);

            Game1.eightBallTexture = Game1.CreateBlockColouredCircleTexture(graphicsDevice, Game1.poolBallRadius, Color.Black);
            Game1._eightBall = new(Game1.eightBallTexture, Game1.poolBallRadius);

            Game1.solidObjectBallTexture = Game1.CreateBlockColouredCircleTexture(graphicsDevice, Game1.poolBallRadius, Color.Yellow);
            Game1.stripedObjectBallTexture = Game1.CreateBlockColouredCircleTexture(graphicsDevice, Game1.poolBallRadius, Color.Red);

            const int spacing = 3; // to reduce messy collisions

            // (derivation for exact positions in writeup):

            // first column:
            Game1._solidObjectBall1 = new(Game1.solidObjectBallTexture, new Vector2(Game1._eightBall.position.X - (2 * Game1.poolBallRadius * Game1.sqrt_3) - (2 * spacing), Game1._eightBall.position.Y), Game1.poolBallRadius, false);

            // second column:
            Game1._solidObjectBall2 = new(Game1.solidObjectBallTexture, new Vector2(Game1._eightBall.position.X - (Game1.poolBallRadius * Game1.sqrt_3) - spacing, Game1._eightBall.position.Y + Game1.poolBallRadius + spacing), Game1.poolBallRadius, false);
            Game1._stripedObjectBall1 = new(Game1.stripedObjectBallTexture, new Vector2(Game1._eightBall.position.X - (Game1.poolBallRadius * Game1.sqrt_3) - spacing, Game1._eightBall.position.Y - Game1.poolBallRadius - spacing), Game1.poolBallRadius, false);

            // third column:
            Game1._stripedObjectBall2 = new(Game1.stripedObjectBallTexture, new Vector2(Game1._eightBall.position.X, Game1._eightBall.position.Y + (2 * Game1.poolBallRadius) + spacing), Game1.poolBallRadius, false);
            // (eight ball goes here)
            Game1._solidObjectBall3 = new(Game1.solidObjectBallTexture, new Vector2(Game1._eightBall.position.X, Game1._eightBall.position.Y - (2 * Game1.poolBallRadius) - spacing), Game1.poolBallRadius, false);

            // fourth column:
            Game1._solidObjectBall4 = new(Game1.solidObjectBallTexture, new Vector2(Game1._eightBall.position.X + (Game1.poolBallRadius * Game1.sqrt_3) + spacing, Game1._eightBall.position.Y + (3 * Game1.poolBallRadius) + (2 * spacing)), Game1.poolBallRadius, false);
            Game1._stripedObjectBall3 = new(Game1.stripedObjectBallTexture, new Vector2(Game1._eightBall.position.X + (Game1.poolBallRadius * Game1.sqrt_3) + spacing, Game1._eightBall.position.Y + Game1.poolBallRadius + spacing), Game1.poolBallRadius, false);
            Game1._solidObjectBall5 = new(Game1.solidObjectBallTexture, new Vector2(Game1._eightBall.position.X + (Game1.poolBallRadius * Game1.sqrt_3) + spacing, Game1._eightBall.position.Y - Game1.poolBallRadius - spacing), Game1.poolBallRadius, false);
            Game1._stripedObjectBall4 = new(Game1.stripedObjectBallTexture, new Vector2(Game1._eightBall.position.X + (Game1.poolBallRadius * Game1.sqrt_3) + spacing, Game1._eightBall.position.Y - (3 * Game1.poolBallRadius) - (2 * spacing)), Game1.poolBallRadius, false);

            // fith column:
            Game1._stripedObjectBall5 = new(Game1.stripedObjectBallTexture, new Vector2(Game1._eightBall.position.X + (2 * Game1.poolBallRadius * Game1.sqrt_3) + (2 * spacing), Game1._eightBall.position.Y + (4 * Game1.poolBallRadius) + (2 * spacing)), Game1.poolBallRadius, false);
            Game1._solidObjectBall6 = new(Game1.solidObjectBallTexture, new Vector2(Game1._eightBall.position.X + (2 * Game1.poolBallRadius * Game1.sqrt_3) + (2 * spacing), Game1._eightBall.position.Y + (2 * Game1.poolBallRadius) + spacing), Game1.poolBallRadius, false);
            Game1._stripedObjectBall6 = new(Game1.stripedObjectBallTexture, new Vector2(Game1._eightBall.position.X + (2 * Game1.poolBallRadius * Game1.sqrt_3) + (2 * spacing), Game1._eightBall.position.Y), Game1.poolBallRadius, false);
            Game1._stripedObjectBall7 = new(Game1.stripedObjectBallTexture, new Vector2(Game1._eightBall.position.X + (2 * Game1.poolBallRadius * Game1.sqrt_3) + (2 * spacing), Game1._eightBall.position.Y - (2 * Game1.poolBallRadius) - spacing), Game1.poolBallRadius, false);
            Game1._solidObjectBall7 = new(Game1.solidObjectBallTexture, new Vector2(Game1._eightBall.position.X + (2 * Game1.poolBallRadius * Game1.sqrt_3) + (2 * spacing), Game1._eightBall.position.Y - (4 * Game1.poolBallRadius) - (2 * spacing)), Game1.poolBallRadius, false);

            Game1.poolBalls = new List<PoolBall> { Game1._cueBall, Game1._eightBall,
                                                   Game1._solidObjectBall1,   Game1._solidObjectBall2,   Game1._solidObjectBall3,   Game1._solidObjectBall4,   Game1._solidObjectBall5,   Game1._solidObjectBall6,   Game1._solidObjectBall7,
                                                   Game1._stripedObjectBall1, Game1._stripedObjectBall2, Game1._stripedObjectBall3, Game1._stripedObjectBall4, Game1._stripedObjectBall5, Game1._stripedObjectBall6, Game1._stripedObjectBall7};


            // Pockets:

            Game1.pocketTexture = Game1.CreateBlockColouredCircleTexture(graphicsDevice, Game1.pocketRadius, Color.Black);
            Game1._pocketTopLeft = new Pocket(Game1.pocketTexture, new Vector2(Game1.pocketRadius + Game1.tablePocketSpacing, Game1.pocketRadius + Game1.tablePocketSpacing), Game1.pocketRadius);
            Game1._pocketTopMiddle = new Pocket(Game1.pocketTexture, new Vector2(Game1.windowWidth / 2, Game1.pocketRadius + Game1.tablePocketSpacing), Game1.pocketRadius);
            Game1._pocketTopRight = new Pocket(Game1.pocketTexture, new Vector2(Game1.windowWidth - Game1.pocketRadius - Game1.tablePocketSpacing, Game1.pocketRadius + Game1.tablePocketSpacing), Game1.pocketRadius);
            Game1._pocketBottomLeft = new Pocket(Game1.pocketTexture, new Vector2(Game1.pocketRadius + Game1.tablePocketSpacing, Game1.windowHeight - Game1.pocketRadius - Game1.tablePocketSpacing), Game1.pocketRadius);
            Game1._pocketBottomMiddle = new Pocket(Game1.pocketTexture, new Vector2(Game1.windowWidth / 2, Game1.windowHeight - Game1.pocketRadius - Game1.tablePocketSpacing), Game1.pocketRadius);
            Game1._pocketBottomRight = new Pocket(Game1.pocketTexture, new Vector2(Game1.windowWidth - Game1.pocketRadius - Game1.tablePocketSpacing, Game1.windowHeight - Game1.pocketRadius - Game1.tablePocketSpacing), Game1.pocketRadius);

            Game1.pockets = new Pocket[6] { Game1._pocketTopLeft,    Game1._pocketTopMiddle,    Game1._pocketTopRight,
                                            Game1._pocketBottomLeft, Game1._pocketBottomMiddle, Game1._pocketBottomRight };


            // cushions:

            Color cushionColor = Color.Sienna;

            int topBottomCushionLength = (int)((Game1.windowWidth / 2) + Game1.tablePocketSpacing - (Game1.sqrt_2 * Game1.pocketRadius));
            int middleCushionLength = (int)(Game1.windowHeight - (2 * Game1.sqrt_2 * Game1.pocketRadius));
            int cushionWidth = (2 * Game1.pocketRadius) + Game1.tablePocketSpacing;
            int topBottomLeftCushionCentreX = (int)(((Game1.windowWidth / 2) + Game1.tablePocketSpacing + (Game1.sqrt_2 * Game1.pocketRadius)) / 2);
            int topBottomRightCushionCentreX = Game1.windowWidth - topBottomLeftCushionCentreX;

            Game1.cushionTopTexture = Game1.CreateTrapeziumTexture(graphicsDevice, topBottomCushionLength, cushionWidth, cushionColor, 3);
            Game1._cushionTopLeft = new(Game1.cushionTopTexture, new Vector2(topBottomLeftCushionCentreX, cushionWidth / 2), topBottomCushionLength, cushionWidth, 3);
            Game1._cushionTopRight = new(Game1.cushionTopTexture, new Vector2(topBottomRightCushionCentreX, cushionWidth / 2), topBottomCushionLength, cushionWidth, 3);

            Game1.cushionMddleLeftTexture = Game1.CreateTrapeziumTexture(graphicsDevice, middleCushionLength, cushionWidth, cushionColor, 4);
            Game1._cushionMiddleLeft = new(Game1.cushionMddleLeftTexture, new Vector2(cushionWidth / 2, Game1.windowHeight / 2), middleCushionLength, cushionWidth, 4);

            Game1.cushionMddleRightTexture = Game1.CreateTrapeziumTexture(graphicsDevice, middleCushionLength, cushionWidth, cushionColor, 2);
            Game1._cushionMiddleRight = new(Game1.cushionMddleRightTexture, new Vector2(Game1.windowWidth - (cushionWidth / 2), Game1.windowHeight / 2), middleCushionLength, cushionWidth, 2);

            Game1.cushionBottomTexture = Game1.CreateTrapeziumTexture(graphicsDevice, topBottomCushionLength, cushionWidth, cushionColor, 1);
            Game1._cushionBottomLeft = new(Game1.cushionBottomTexture, new Vector2(topBottomLeftCushionCentreX, Game1.windowHeight - (cushionWidth / 2)), topBottomCushionLength, cushionWidth, 1);
            Game1._cushionBottomRight = new(Game1.cushionBottomTexture, new Vector2(topBottomRightCushionCentreX, Game1.windowHeight - (cushionWidth / 2)), topBottomCushionLength, cushionWidth, 1);

            Game1.cushions = new Cushion[] { Game1._cushionTopLeft,    Game1._cushionTopRight,
                                             Game1._cushionMiddleLeft, Game1._cushionMiddleRight,
                                             Game1._cushionBottomLeft, Game1._cushionBottomRight };

            // triangles fill in the rest of the sides, they don't need collisions because they are always behind pockets:

            Game1.bottomRightTriangleTexture = Game1.CreateRightAngledTriangleTexture(graphicsDevice, 2 * (Game1.pocketRadius + Game1.tablePocketSpacing), cushionColor, 1);
            Game1.bottomRightTriangle = new(Game1.bottomRightTriangleTexture, new Vector2(Game1.windowWidth - (Game1.pocketRadius + Game1.tablePocketSpacing), Game1.windowHeight - (Game1.pocketRadius + Game1.tablePocketSpacing)));

            Game1.topLeftTriangleTexture = Game1.CreateRightAngledTriangleTexture(graphicsDevice, 2 * (Game1.pocketRadius + Game1.tablePocketSpacing), cushionColor, 2);
            Game1.topLeftTriangle = new(Game1.topLeftTriangleTexture, new Vector2(Game1.windowWidth - (Game1.pocketRadius + Game1.tablePocketSpacing), Game1.pocketRadius + Game1.tablePocketSpacing));

            Game1.topRightTriangleTexture = Game1.CreateRightAngledTriangleTexture(graphicsDevice, 2 * (Game1.pocketRadius + Game1.tablePocketSpacing), cushionColor, 3);
            Game1.topRightTriangle = new(Game1.topRightTriangleTexture, new Vector2(Game1.pocketRadius + Game1.tablePocketSpacing, Game1.pocketRadius + Game1.tablePocketSpacing));

            Game1.bottomLeftTriangleTexture = Game1.CreateRightAngledTriangleTexture(graphicsDevice, 2 * (Game1.pocketRadius + Game1.tablePocketSpacing), cushionColor, 4);
            Game1.bottomLeftTriangle = new(Game1.bottomLeftTriangleTexture, new Vector2(Game1.pocketRadius + Game1.tablePocketSpacing, Game1.windowHeight - (Game1.pocketRadius + Game1.tablePocketSpacing)));


            // misc. sprites:

            Game1.baulkLineTexture = Game1.CreateVerticalLineTexture(graphicsDevice, Game1.windowHeight - (4 * Game1.pocketRadius) - (2 * Game1.tablePocketSpacing), 3, Color.Black);
            int baulkLineCentreX = (Game1.windowWidth + (6 * Game1.pocketRadius) + (3 * Game1.tablePocketSpacing)) / 5; // 1/5th across the playing surface (not 1/5th across entire table)
            Game1.baulkLine = new(Game1.baulkLineTexture, new Vector2(baulkLineCentreX, Game1.windowHeight / 2));
        }

        /// <summary>
        /// Checks if all PoolBalls are stationary (i.e. not moving).
        /// </summary>
        /// <returns>true if all PoolBalls are stationary, false if not.</returns>
        /// <remarks>Since PoolBalls are the only moving entities, it only needs to check all PoolBalls to make sure everything is stationary.</remarks>
        public static bool IsAllStationary()
        {
            foreach (PoolBall poolBall in Game1.poolBalls)
            {
                if (poolBall.velocity != Vector2.Zero)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Checks if any PoolBalls are colliding.
        /// If they are, their velocities are altered accordingly
        /// and their positions are moved to make sure they aren't stuck overlapping
        /// </summary>
        /// <remarks>
        /// This can't be in the PoolBall class because both velocities need to be changed at the same time for it to work.
        /// If one PoolBall's velocity is changed and then another calls its collision detection function,
        /// the second PoolBall with alter its velocity with the wrong pre-collision velocity for the first PoolBall.
        /// </remarks>
        public static void DoAllPoolBallPoolBallCollisions() // in Match1.cs rather than PoolBall.cs so that all velocities can be changed on the same frame
        {
            for (int i = 0; i < Game1.poolBalls.Count - 1; i++)
            {
                for (int j = i + 1; j < Game1.poolBalls.Count; j++) // j = i + 1 prevents repeatedly checking the same pairs
                {
                    if (Game1.poolBalls[i] == Game1.poolBalls[j]) // no need to check if it collides with itself
                    { continue; }
                    else
                    {
                        if (Vector2.Distance(Game1.poolBalls[i].position, Game1.poolBalls[j].position) < Game1.poolBalls[i].radius * 2)
                        {
                            Vector2 relativePositionVector = Game1.poolBalls[i].position - Game1.poolBalls[j].position; // the vector that is normal to the collision surface (aka other ball), pointing towards poolBalls[i]
                            Vector2 unitNormalVector = relativePositionVector / relativePositionVector.Length(); // normalised to have a magnitude of 1

                            // adjusting velocities:
                            Vector2 newVelocity = Game1.poolBalls[i].velocity + (Vector2.Dot(Game1.poolBalls[j].velocity - Game1.poolBalls[i].velocity, unitNormalVector) * unitNormalVector);
                            Vector2 newOtherVelocity = Game1.poolBalls[j].velocity + (Vector2.Dot(Game1.poolBalls[i].velocity - Game1.poolBalls[j].velocity, unitNormalVector) * unitNormalVector);
                            Game1.poolBalls[j].velocity = newOtherVelocity;
                            Game1.poolBalls[i].velocity = newVelocity;

                            // adjusting positions to prevent PoolBalls overlapping and getting stuck inside of each other:
                            Game1.poolBalls[j].position += ((0.5f * relativePositionVector.Length()) - Game1.poolBalls[i].radius) * unitNormalVector; // moving it half the distance they overlap
                            Game1.poolBalls[i].position -= ((0.5f * relativePositionVector.Length()) - Game1.poolBalls[i].radius) * unitNormalVector; // moving it half the distance they overlap
                        }
                    }
                }
            }
        }



        public override void Update(GraphicsDevice graphicsDevice, GameTime gameTime)
        {
            // restart match with 'R':
            if (Keyboard.GetState().IsKeyDown(Keys.R))
            {
                LoadInitialContent(graphicsDevice);
            }

            // Pocket-PoolBall collisions before PoolBall-PoolBall collisions because deleting is less intensive than calculating all collision trajectories
            // and if a PoolBall is deleted, less collision work needs to be done
            foreach (Pocket _pocket in Game1.pockets)
            {
                _pocket.Update(gameTime);
            }

            DoAllPoolBallPoolBallCollisions();

            // foreach means that PoolBalls removed from the array aren't updated:
            foreach (PoolBall _poolBall in Game1.poolBalls)
            {
                _poolBall.Update(gameTime);
            }

            foreach (Cushion _cushion in Game1.cushions)
            {
                _cushion.Update(gameTime);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            Game1._spriteBatch.Begin();

            Game1.baulkLine.Draw(Game1._spriteBatch);

            Game1.bottomRightTriangle.Draw(Game1._spriteBatch);
            Game1.topRightTriangle.Draw(Game1._spriteBatch);
            Game1.topLeftTriangle.Draw(Game1._spriteBatch);
            Game1.bottomLeftTriangle.Draw(Game1._spriteBatch);

            foreach (Cushion _cushion in Game1.cushions)
            {
                _cushion.Draw(Game1._spriteBatch);
            }

            foreach (Pocket _pocket in Game1.pockets)
            {
                _pocket.Draw(Game1._spriteBatch);
            }

            // foreach means that PoolBalls removed from the array aren't drawn
            foreach (PoolBall _poolBall in Game1.poolBalls)
            {
                _poolBall.Draw(Game1._spriteBatch);
            }

            //Game1.testSprite.Draw(Game1._spriteBatch);

            Game1._spriteBatch.End();
        }
    }
}
