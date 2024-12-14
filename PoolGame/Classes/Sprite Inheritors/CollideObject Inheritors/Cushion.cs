﻿using Microsoft.Xna.Framework.Graphics;
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

        public int length;
        public int width;
        public int orientation; // int between 1 and 4
        // if orientation == 1, it will have the shape: /___\
        // if orientation == 2, it will have the shape: /___\ rotated 90 degrees anticlockwise
        // if orientation == 3, it will have the shape: /___\ rotated 180 degrees
        // if orientation == 4, it will have the shape: /___\ rotated 90 degrees clockwise

        public Cushion(Texture2D _texture, Vector2 _position, int _length, int _width, int _orientation) : base(_texture, _position, _orientation)
        {
            if (_orientation < 1 | _orientation > 4)
            {
                throw new ArgumentException("orientation must be between 1 and 4 inclusive");
            }

            texture = _texture;
            position = _position;
            length = _length;
            width = _width;
            orientation = _orientation;
        }

        public void DoCollisions() // [todo: add repositioning outside of the hitbox after collisions
                                   //        add correct corner collisions]
        {
            
            switch (orientation)
            {
                // NOTE: directions are from viewing the trapezium as if it's orientation = 1
                case 1:
                    for (int i = 0; i < Match.poolBalls.Count; i++)
                    {
                        // middle:
                        if ((Match.poolBalls[i].position.X > position.X - (length / 2) + width)  // if it's in the column of the screen that has this section of the trapezium
                          & (Match.poolBalls[i].position.X < position.X + (length / 2) - width) // if it's in the column of the screen that has this section of the trapezium
                          & (Match.poolBalls[i].position.Y + Match.poolBalls[i].radius > position.Y - (width / 2))) // if it's below the top side of the trapezium
                        {
                            Match.poolBalls[i].position = new Vector2(Match.poolBalls[i].position.X, position.Y - Match.poolBalls[i].radius - (width / 2)); // places it outside of hitbox
                            Match.poolBalls[i].velocity = new Vector2(Match.poolBalls[i].velocity.X, -Match.poolBalls[i].velocity.Y); // reflected horizontally
                        }

                        // left triangle:
                        if ((Match.poolBalls[i].position.X > position.X - (length / 2) - Match.poolBalls[i].radius) // if in the column of the screen that has this section of the trapezium
                          & (Match.poolBalls[i].position.X < position.X - (length / 2) + width) // if in the column of the screen that has this section of the trapezium
                          & (Math.Abs(((position.X + position.Y) - (Match.poolBalls[i].position.X + Match.poolBalls[i].position.Y) + (0.5 * (width - length)))) / Match.sqrt_2 < Match.poolBalls[i].radius))
                        {
                            // [todo: add repositioning]: Match.poolBalls[i].position = new Vector2(); // places it outside of hitbox
                            Match.poolBalls[i].velocity = new Vector2(-Match.poolBalls[i].velocity.Y, -Match.poolBalls[i].velocity.X); // reflected
                        }

                        // right triangle:
                        if ((Match.poolBalls[i].position.X > position.X + (length / 2) - width) // if in the column of the screen that has this section of the trapezium
                          & (Match.poolBalls[i].position.X < position.X + (length / 2) + Match.poolBalls[i].radius) // if in the column of the screen that has this section of the trapezium
                          & (Math.Abs(((position.Y - position.X) + (Match.poolBalls[i].position.X - Match.poolBalls[i].position.Y) + (0.5 * (width - length)))) / Match.sqrt_2 < Match.poolBalls[i].radius))
                        {
                            // [todo: add repositioning]: Match.poolBalls[i].position = new Vector2(); // places it outside of hitbox
                            Match.poolBalls[i].velocity = new Vector2(Match.poolBalls[i].velocity.Y, Match.poolBalls[i].velocity.X); // reflected
                        }

                        // left corner:

                        // right corner:
                    }
                    break;
                case 2:
                    for (int i = 0; i < Match.poolBalls.Count; i++)
                    {
                        // left triangle:
                        if ((Match.poolBalls[i].position.Y > position.Y + (length / 2) - width) // if in the row of the screen that has this section of the trapezium
                          & (Match.poolBalls[i].position.Y < position.Y + (length / 2) + Match.poolBalls[i].radius) // if in the row of the screen that has this section of the trapezium
                          & (Math.Abs(((position.X - position.Y) + (Match.poolBalls[i].position.Y - Match.poolBalls[i].position.X) + (0.5 * (width - length)))) / Match.sqrt_2 < Match.poolBalls[i].radius))
                        {
                            // [todo: add repositioning]: Match.poolBalls[i].position = new Vector2(); // places it outside of hitbox
                            Match.poolBalls[i].velocity = new Vector2(Match.poolBalls[i].velocity.Y, Match.poolBalls[i].velocity.X); // reflected
                        }

                        // right triangle:
                        if ((Match.poolBalls[i].position.Y > position.Y - (length / 2) - Match.poolBalls[i].radius) // if in the row of the screen that has this section of the trapezium
                          & (Match.poolBalls[i].position.Y < position.Y - (length / 2) + width) // if in the row of the screen that has this section of the trapezium
                          & (Math.Abs(((position.X + position.Y) - (Match.poolBalls[i].position.Y + Match.poolBalls[i].position.X) + (0.5 * (width - length)))) / Match.sqrt_2 < Match.poolBalls[i].radius))
                        {
                            // [todo: add repositioning]: Match.poolBalls[i].position = new Vector2(); // places it outside of hitbox
                            Match.poolBalls[i].velocity = new Vector2(-Match.poolBalls[i].velocity.Y, -Match.poolBalls[i].velocity.X); // reflected
                        }

                        // middle:
                        if ((Match.poolBalls[i].position.Y > position.Y - (length / 2) + width)
                          & (Match.poolBalls[i].position.Y < position.Y + (length / 2) - width)
                          & (Match.poolBalls[i].position.X + Match.poolBalls[i].radius > position.X - (width / 2)))
                        {
                            Match.poolBalls[i].position = new Vector2(position.X - Match.poolBalls[i].radius - (width / 2), Match.poolBalls[i].position.Y); // places it outside of hitbox
                            Match.poolBalls[i].velocity = new Vector2(-Match.poolBalls[i].velocity.X, Match.poolBalls[i].velocity.Y); // reflected vertically
                        }

                        // left corner:

                        // right corner:
                    }
                    break;
                case 3:
                    for (int i = 0; i < Match.poolBalls.Count; i++)
                    {
                        // middle:
                        if ((Match.poolBalls[i].position.X > position.X - (length / 2) + width)
                          & (Match.poolBalls[i].position.X < position.X + (length / 2) - width)
                          & (Match.poolBalls[i].position.Y - Match.poolBalls[i].radius < position.Y + (width / 2)))
                        {
                            Match.poolBalls[i].position = new Vector2(Match.poolBalls[i].position.X, position.Y + Match.poolBalls[i].radius + (width / 2)); // places it outside of hitbox
                            Match.poolBalls[i].velocity = new Vector2(Match.poolBalls[i].velocity.X, -Match.poolBalls[i].velocity.Y); // reflected horizontally
                        }

                        // left triangle:
                        if ((Match.poolBalls[i].position.X > position.X + (length / 2) - width) // if in the column of the screen that has this section of the trapezium
                          & (Match.poolBalls[i].position.X < position.X + (length / 2) + Match.poolBalls[i].radius) // if in the column of the screen that has this section of the trapezium
                          & (Math.Abs(((position.X + position.Y) - (Match.poolBalls[i].position.X + Match.poolBalls[i].position.Y) + (0.5 * (length - width)))) / Match.sqrt_2 < Match.poolBalls[i].radius))
                        {
                            // [todo: add repositioning]: Match.poolBalls[i].position = new Vector2(); // places it outside of hitbox
                            Match.poolBalls[i].velocity = new Vector2(-Match.poolBalls[i].velocity.Y, -Match.poolBalls[i].velocity.X); // reflected
                        }

                        // right triangle:
                        if ((Match.poolBalls[i].position.X > position.X - (length / 2) - Match.poolBalls[i].radius) // if in the column of the screen that has this section of the trapezium
                          & (Match.poolBalls[i].position.X < position.X - (length / 2) + width) // if in the column of the screen that has this section of the trapezium
                          & (Math.Abs(((position.X - position.Y) + (Match.poolBalls[i].position.Y - Match.poolBalls[i].position.X) + (0.5 * (width - length)))) / Match.sqrt_2 < Match.poolBalls[i].radius))
                        {
                            // [todo: add repositioning]: Match.poolBalls[i].position = new Vector2(); // places it outside of hitbox
                            Match.poolBalls[i].velocity = new Vector2(Match.poolBalls[i].velocity.Y, Match.poolBalls[i].velocity.X); // reflected
                        }

                        // left corner:

                        // right corner:
                    }
                    break;
                case 4:
                    for (int i = 0; i < Match.poolBalls.Count; i++)
                    {
                        // left triangle:
                        if ((Match.poolBalls[i].position.Y > position.Y - (length / 2) - Match.poolBalls[i].radius) // if in the row of the screen that has this section of the trapezium
                          & (Match.poolBalls[i].position.Y < position.Y - (length / 2) + width) // if in the row of the screen that has this section of the trapezium
                          & (Math.Abs(((position.X - position.Y) + (Match.poolBalls[i].position.Y - Match.poolBalls[i].position.X) + (0.5 * (length - width)))) / Match.sqrt_2 < Match.poolBalls[i].radius))
                        {
                            // [todo: add repositioning]: Match.poolBalls[i].position = new Vector2(); // places it outside of hitbox
                            Match.poolBalls[i].velocity = new Vector2(Match.poolBalls[i].velocity.Y, Match.poolBalls[i].velocity.X); // reflected
                        }

                        // right triangle:
                        if ((Match.poolBalls[i].position.Y > position.Y + (length / 2) - width) // if in the row of the screen that has this section of the trapezium
                          & (Match.poolBalls[i].position.Y < position.Y + (length / 2) + Match.poolBalls[i].radius) // if in the row of the screen that has this section of the trapezium
                          & (Math.Abs(((position.X + position.Y) - (Match.poolBalls[i].position.Y + Match.poolBalls[i].position.X) + (0.5 * (length - width)))) / Match.sqrt_2 < Match.poolBalls[i].radius))
                        {
                            // [todo: add repositioning]: Match.poolBalls[i].position = new Vector2(); // places it outside of hitbox
                            Match.poolBalls[i].velocity = new Vector2(-Match.poolBalls[i].velocity.Y, -Match.poolBalls[i].velocity.X); // reflected
                        }

                        // middle:
                        if ((Match.poolBalls[i].position.Y > position.Y - (length / 2) + width)
                          & (Match.poolBalls[i].position.Y < position.Y + (length / 2) - width)
                          & (Match.poolBalls[i].position.X - Match.poolBalls[i].radius < position.X + (width / 2)))
                        {
                            Match.poolBalls[i].position = new Vector2(position.X + Match.poolBalls[i].radius + (width / 2), Match.poolBalls[i].position.Y); // places it outside of hitbox
                            Match.poolBalls[i].velocity = new Vector2(-Match.poolBalls[i].velocity.X, Match.poolBalls[i].velocity.Y); // reflected vertically
                        }

                        // left corner:

                        // right corner:
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
