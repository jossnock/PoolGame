using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoolGame.Classes
{
    public class Cushion : CollideObject
    {
        // for cushions in the shape of trapeziums with only horizontal, vertical, and diagonal edges and one line of symmetry
        // (technically the long side of the trapezium doesn't have collisions because it doesn't need them

        int length;
        int width;
        public int orientation; // int between 1 and 4
        // if orientation == 1, it will have the shape: /___\
        // if orientation == 2, it will have the shape: /___\ rotated 90 degrees anticlockwise
        // if orientation == 3, it will have the shape: /___\ rotated 180 degrees
        // if orientation == 4, it will have the shape: /___\ rotated 90 degrees clockwise

        public Cushion(Texture2D _texture, Vector2 _position, int _length, int _width, int _orientation) : base(_texture, _position, _orientation)
        {
            if (orientation < 1 | orientation > 4)
            {
                throw new ArgumentException("orientation must be between 1 and 4 inclusive");
            }

            texture = _texture;
            position = _position;
            length = _length;
            width = _width;
            orientation = _orientation;
        }

        public void DoCollisions()
        {
            switch (orientation)
            {
                case 1:
                    for (int i = 0; i < Game1.poolBalls.Count; i++)
                    {
                        // left triangle:

                        // right triangle:
                        
                        // middle:
                        if ((Game1.poolBalls[i].position.X > position.X - (length / 2) + width) 
                            & (Game1.poolBalls[i].position.X < position.X + (length / 2) - width)
                            & (Game1.poolBalls[i].position.Y > position.Y - (width / 2)))
                        {
                            Game1.poolBalls[i].position = new Vector2(Game1.poolBalls[i].position.X, position.Y - (width / 2)); // places it outside of hitbox
                            Game1.poolBalls[i].velocity = new Vector2(Game1.poolBalls[i].velocity.X, -Game1.poolBalls[i].velocity.Y); // reflected horizontally
                        }

                        // left corner:

                        // right corner:
                    }
                    break;
                case 2:
                    for (int i = 0; i < Game1.poolBalls.Count; i++)
                    {

                    }
                    break;
                case 3:
                    for (int i = 0; i < Game1.poolBalls.Count; i++)
                    {

                    }
                    break;
                case 4:
                    for (int i = 0; i < Game1.poolBalls.Count; i++)
                    {

                    }
                    break;
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            DoCollisions();
        }
    }
}
