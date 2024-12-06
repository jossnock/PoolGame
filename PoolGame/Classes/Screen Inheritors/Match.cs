using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PoolGame.Classes
{
    internal class Match : Screen
    {
        public Match() { }

        public override void Update(GameTime gameTime)
        {
            // Pocket-PoolBall collisions before PoolBall-PoolBall collisions because deleting is less intensive than calculating all collisions
            // and, if a PoolBall is deleted, less collision work needs to be done
            foreach (Pocket _pocket in Game1.pockets)
            {
                _pocket.Update(gameTime);
            }

            Game1.DoAllPoolBallPoolBallCollisions();

            // foreach means that PoolBalls removed from the array aren't updated:
            foreach (PoolBall _poolBall in Game1.poolBalls)
            {
                _poolBall.Update(gameTime);
            }

            foreach (Cushion _cushion in Game1.cushions)
            {
                _cushion.Update(gameTime);
            }

            Game1.testCushion.Update(gameTime);
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
            Game1.testCushion.Draw(Game1._spriteBatch);

            Game1._spriteBatch.End();
        }
    }
}
