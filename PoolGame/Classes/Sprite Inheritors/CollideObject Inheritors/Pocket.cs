using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PoolGame.Classes.Screens;

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
            for (int i = 0; i < MainMenu.poolBalls.Count; i++)
            {
                if (Vector2.Distance(position, MainMenu.poolBalls[i].position) < MainMenu.pocketRadius) // not Match1.pocketRadius + Match1.poolBallRadius,
                                                                                                    // otherwise it would delete PoolBalls before they would realistically fall in a Pocket
                {
                    MainMenu.poolBalls.RemoveAt(i);
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
