using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace PoolGame.Classes
{
    public class Pocket : CollideObject
    {
        public Pocket(Texture2D _texture, Vector2 _position, float _radius) : base(_texture, _position, _radius)
        {
            texture = _texture;
            position = _position;
            radius = _radius;
            Type = ObjectType.Circle;
        }

        public void DoCollisions()
        {
            for (int i = 0; i < Game1.poolBalls.Count; i++)
            {
                if (Vector2.Distance(position, Game1.poolBalls[i].position) < Game1.pocketRadius) // not Match1.pocketRadius + Match1.poolBallRadius,
                                                                                                    // otherwise it would delete PoolBalls before they would realistically fall in a Pocket
                {
                    if (Game1.poolBalls[i] is CueBall)
                    {
                        Game1._cueBall.velocity = Vector2.Zero; // stops it after it's pocketed so that IsAllStationary() doesn't think it's still moving
                        Game1._cueBall.isPotted = true; // removing _cueBall from poolBalls means it can't be updated so it can't reappear
                    }
                    else
                    {
                        Game1.poolBalls.RemoveAt(i);
                    }
                }
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            DoCollisions();
        }
    }
}
