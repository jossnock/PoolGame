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
    /// <summary>
    /// The screen that sets up everything for a match, renders everything during a match, and calculates collisions between PoolBalls
    /// </summary>
    public class Match : Screen
    {
        // sizing:
        // using the approx. scale 1cm : 8px for 1280x720
        public static int poolBallRadius;
        public static int pocketRadius;
        public static int tablePocketSpacing;

        // pool balls:

        public static CueBall _cueBall;
        public static Texture2D cueBallTexture;

        public static EightBall _eightBall;
        public static Texture2D eightBallTexture;

        public static ColouredBall _yellowObjectBall1;
        public static ColouredBall _yellowObjectBall2;
        public static ColouredBall _yellowObjectBall3;
        public static ColouredBall _yellowObjectBall4;
        public static ColouredBall _yellowObjectBall5;
        public static ColouredBall _yellowObjectBall6;
        public static ColouredBall _yellowObjectBall7;
        public static Texture2D yellowObjectBallTexture;

        public static ColouredBall _redObjectBall1;
        public static ColouredBall _redObjectBall2;
        public static ColouredBall _redObjectBall3;
        public static ColouredBall _redObjectBall4;
        public static ColouredBall _redObjectBall5;
        public static ColouredBall _redObjectBall6;
        public static ColouredBall _redObjectBall7;
        public static Texture2D redObjectBallTexture;

        public static List<PoolBall> poolBalls; // List (not array) so that PoolBalls can be removed when potted

        // pockets:
        public static Pocket _pocketTopLeft;
        public static Pocket _pocketTopMiddle;
        public static Pocket _pocketTopRight;
        public static Pocket _pocketBottomLeft;
        public static Pocket _pocketBottomMiddle;
        public static Pocket _pocketBottomRight;
        public static Texture2D pocketTexture;
        public static Pocket[] pockets;


        // cushions:

        public static Cushion _cushionTopLeft;
        public static Cushion _cushionTopRight;
        public static Texture2D cushionTopTexture;

        public static Cushion _cushionMiddleLeft;
        public static Texture2D cushionMddleLeftTexture;

        public static Cushion _cushionMiddleRight;
        public static Texture2D cushionMddleRightTexture;

        public static Cushion _cushionBottomLeft;
        public static Cushion _cushionBottomRight;
        public static Texture2D cushionBottomTexture;

        public static Cushion[] cushions;


        // misc sprites:

        public static Sprite baulkLine;
        public static Texture2D baulkLineTexture;

        public static Sprite bottomRightTriangle;
        public static Texture2D bottomRightTriangleTexture;
        public static Sprite topRightTriangle;
        public static Texture2D topRightTriangleTexture;
        public static Sprite topLeftTriangle;
        public static Texture2D topLeftTriangleTexture;
        public static Sprite bottomLeftTriangle;
        public static Texture2D bottomLeftTriangleTexture;


        // constants:
        // rounded up so that things don't overlap (since they are mainly used for positioning)
        public const float sqrt_2 = 1.4142135624f; // √2, to not need to repeatedly use Math.Sqrt(2), plus C# doesn't like the results of Math.Sqrt() being assigned as a constant
        public const float sqrt_3 = 1.7320508076f; // √3, to not need to repeatedly use Math.Sqrt(3), plus C# doesn't like the results of Math.Sqrt() being assigned as a constant


        public Match()
        {
            backgroundColor = new Color(21, 88, 67); // pale dark green
        }

        public override void LoadInitialContent(GraphicsDevice graphicsDevice)
        {
            // sizing:
            poolBallRadius = Game1.windowWidth / 64; // 20 for Game1.windowWidth = 1280
            pocketRadius = (9 * Game1.windowWidth) / 320; // 36 for Game1.windowWidth = 1280
            tablePocketSpacing = (Game1.windowWidth / 80); // 16 for Game1.windowWidth = 1280


            // PoolBalls:

            cueBallTexture = CreateBlockColouredCircleTexture(graphicsDevice, poolBallRadius, Color.White);
            _cueBall = new(cueBallTexture, poolBallRadius);

            eightBallTexture = CreateBlockColouredCircleTexture(graphicsDevice, poolBallRadius, Color.Black);
            _eightBall = new(eightBallTexture, poolBallRadius);

            yellowObjectBallTexture = CreateBlockColouredCircleTexture(graphicsDevice, poolBallRadius, Color.Yellow);
            redObjectBallTexture = CreateBlockColouredCircleTexture(graphicsDevice, poolBallRadius, Color.Red);

            const int spacing = 3; // to reduce messy collisions

            // (derivation for exact positions in writeup):

            // first column:
            _yellowObjectBall1 = new(yellowObjectBallTexture, new Vector2(_eightBall.position.X - (2 * poolBallRadius * sqrt_3) - (2 * spacing), _eightBall.position.Y), poolBallRadius, false);

            // second column:
            _yellowObjectBall2 = new(yellowObjectBallTexture, new Vector2(_eightBall.position.X - (poolBallRadius * sqrt_3) - spacing, _eightBall.position.Y + poolBallRadius + spacing), poolBallRadius, false);
            _redObjectBall1 = new(redObjectBallTexture, new Vector2(_eightBall.position.X - (poolBallRadius * sqrt_3) - spacing, _eightBall.position.Y - poolBallRadius - spacing), poolBallRadius, false);

            // third column:
            _redObjectBall2 = new(redObjectBallTexture, new Vector2(_eightBall.position.X, _eightBall.position.Y + (2 * poolBallRadius) + spacing), poolBallRadius, false);
            // (eight ball goes here)
            _yellowObjectBall3 = new(yellowObjectBallTexture, new Vector2(_eightBall.position.X, _eightBall.position.Y - (2 * poolBallRadius) - spacing), poolBallRadius, false);

            // fourth column:
            _yellowObjectBall4 = new(yellowObjectBallTexture, new Vector2(_eightBall.position.X + (poolBallRadius * sqrt_3) + spacing, _eightBall.position.Y + (3 * poolBallRadius) + (2 * spacing)), poolBallRadius, false);
            _redObjectBall3 = new(redObjectBallTexture, new Vector2(_eightBall.position.X + (poolBallRadius * sqrt_3) + spacing, _eightBall.position.Y + poolBallRadius + spacing), poolBallRadius, false);
            _yellowObjectBall5 = new(yellowObjectBallTexture, new Vector2(_eightBall.position.X + (poolBallRadius * sqrt_3) + spacing, _eightBall.position.Y - poolBallRadius - spacing), poolBallRadius, false);
            _redObjectBall4 = new(redObjectBallTexture, new Vector2(_eightBall.position.X + (poolBallRadius * sqrt_3) + spacing, _eightBall.position.Y - (3 * poolBallRadius) - (2 * spacing)), poolBallRadius, false);

            // fith column:
            _redObjectBall5 = new(redObjectBallTexture, new Vector2(_eightBall.position.X + (2 * poolBallRadius * sqrt_3) + (2 * spacing), _eightBall.position.Y + (4 * poolBallRadius) + (2 * spacing)), poolBallRadius, false);
            _yellowObjectBall6 = new(yellowObjectBallTexture, new Vector2(_eightBall.position.X + (2 * poolBallRadius * sqrt_3) + (2 * spacing), _eightBall.position.Y + (2 * poolBallRadius) + spacing), poolBallRadius, false);
            _redObjectBall6 = new(redObjectBallTexture, new Vector2(_eightBall.position.X + (2 * poolBallRadius * sqrt_3) + (2 * spacing), _eightBall.position.Y), poolBallRadius, false);
            _redObjectBall7 = new(redObjectBallTexture, new Vector2(_eightBall.position.X + (2 * poolBallRadius * sqrt_3) + (2 * spacing), _eightBall.position.Y - (2 * poolBallRadius) - spacing), poolBallRadius, false);
            _yellowObjectBall7 = new(yellowObjectBallTexture, new Vector2(_eightBall.position.X + (2 * poolBallRadius * sqrt_3) + (2 * spacing), _eightBall.position.Y - (4 * poolBallRadius) - (2 * spacing)), poolBallRadius, false);

            poolBalls = new List<PoolBall> { _cueBall, _eightBall,
                                             _yellowObjectBall1,   _yellowObjectBall2,   _yellowObjectBall3,   _yellowObjectBall4,   _yellowObjectBall5,   _yellowObjectBall6,   _yellowObjectBall7,
                                             _redObjectBall1, _redObjectBall2, _redObjectBall3, _redObjectBall4, _redObjectBall5, _redObjectBall6, _redObjectBall7};

            // Pockets:

            pocketTexture = CreateBlockColouredCircleTexture(graphicsDevice, pocketRadius, Color.Black);
            _pocketTopLeft = new(pocketTexture, new Vector2(pocketRadius + tablePocketSpacing, pocketRadius + tablePocketSpacing), pocketRadius);
            _pocketTopMiddle = new(pocketTexture, new Vector2(Game1.windowWidth / 2, pocketRadius + tablePocketSpacing), pocketRadius);
            _pocketTopRight = new(pocketTexture, new Vector2(Game1.windowWidth - pocketRadius - tablePocketSpacing, pocketRadius + tablePocketSpacing), pocketRadius);
            _pocketBottomLeft = new(pocketTexture, new Vector2(pocketRadius + tablePocketSpacing, Game1.windowHeight - pocketRadius - tablePocketSpacing), pocketRadius);
            _pocketBottomMiddle = new(pocketTexture, new Vector2(Game1.windowWidth / 2, Game1.windowHeight - pocketRadius - tablePocketSpacing), pocketRadius);
            _pocketBottomRight = new(pocketTexture, new Vector2(Game1.windowWidth - pocketRadius - tablePocketSpacing, Game1.windowHeight - pocketRadius - tablePocketSpacing), pocketRadius);

            pockets = new Pocket[6] { _pocketTopLeft,    _pocketTopMiddle,    _pocketTopRight,
                                      _pocketBottomLeft, _pocketBottomMiddle, _pocketBottomRight };


            // cushions:

            Color cushionColor = Color.Sienna;

            int topBottomCushionLength = (int)((Game1.windowWidth / 2) + tablePocketSpacing - (sqrt_2 * pocketRadius));
            int middleCushionLength = (int)(Game1.windowHeight - (2 * sqrt_2 * pocketRadius));
            int cushionWidth = (2 * pocketRadius) + tablePocketSpacing;
            int topBottomLeftCushionCentreX = (int)(((Game1.windowWidth / 2) + tablePocketSpacing + (sqrt_2 * pocketRadius)) / 2);
            int topBottomRightCushionCentreX = Game1.windowWidth - topBottomLeftCushionCentreX;

            cushionTopTexture = CreateTrapeziumTexture(graphicsDevice, topBottomCushionLength, cushionWidth, cushionColor, 3);
            _cushionTopLeft = new(cushionTopTexture, new Vector2(topBottomLeftCushionCentreX, cushionWidth / 2), topBottomCushionLength, cushionWidth, 3);
            _cushionTopRight = new(cushionTopTexture, new Vector2(topBottomRightCushionCentreX, cushionWidth / 2), topBottomCushionLength, cushionWidth, 3);

            cushionMddleLeftTexture = CreateTrapeziumTexture(graphicsDevice, middleCushionLength, cushionWidth, cushionColor, 4);
            _cushionMiddleLeft = new(cushionMddleLeftTexture, new Vector2(cushionWidth / 2, Game1.windowHeight / 2), middleCushionLength, cushionWidth, 4);

            cushionMddleRightTexture = CreateTrapeziumTexture(graphicsDevice, middleCushionLength, cushionWidth, cushionColor, 2);
            _cushionMiddleRight = new(cushionMddleRightTexture, new Vector2(Game1.windowWidth - (cushionWidth / 2), Game1.windowHeight / 2), middleCushionLength, cushionWidth, 2);

            cushionBottomTexture = CreateTrapeziumTexture(graphicsDevice, topBottomCushionLength, cushionWidth, cushionColor, 1);
            _cushionBottomLeft = new(cushionBottomTexture, new Vector2(topBottomLeftCushionCentreX, Game1.windowHeight - (cushionWidth / 2)), topBottomCushionLength, cushionWidth, 1);
            _cushionBottomRight = new(cushionBottomTexture, new Vector2(topBottomRightCushionCentreX, Game1.windowHeight - (cushionWidth / 2)), topBottomCushionLength, cushionWidth, 1);

            cushions = new Cushion[] { _cushionTopLeft,    _cushionTopRight,
                                       _cushionMiddleLeft, _cushionMiddleRight,
                                       _cushionBottomLeft, _cushionBottomRight };

            // triangles fill in the rest of the sides, they don't need collisions because they are always behind pockets:

            bottomRightTriangleTexture = CreateRightAngledTriangleTexture(graphicsDevice, 2 * (pocketRadius + tablePocketSpacing), cushionColor, 1);
            bottomRightTriangle = new(bottomRightTriangleTexture, new Vector2(Game1.windowWidth - (pocketRadius + tablePocketSpacing), Game1.windowHeight - (pocketRadius + tablePocketSpacing)));

            topLeftTriangleTexture = CreateRightAngledTriangleTexture(graphicsDevice, 2 * (pocketRadius + tablePocketSpacing), cushionColor, 2);
            topLeftTriangle = new(topLeftTriangleTexture, new Vector2(Game1.windowWidth - (pocketRadius + tablePocketSpacing), pocketRadius + tablePocketSpacing));

            topRightTriangleTexture = CreateRightAngledTriangleTexture(graphicsDevice, 2 * (pocketRadius + tablePocketSpacing), cushionColor, 3);
            topRightTriangle = new(topRightTriangleTexture, new Vector2(pocketRadius + tablePocketSpacing, pocketRadius + tablePocketSpacing));

            bottomLeftTriangleTexture = CreateRightAngledTriangleTexture(graphicsDevice, 2 * (pocketRadius + tablePocketSpacing), cushionColor, 4);
            bottomLeftTriangle = new(bottomLeftTriangleTexture, new Vector2(pocketRadius + tablePocketSpacing, Game1.windowHeight - (pocketRadius + tablePocketSpacing)));


            // misc. sprites:

            baulkLineTexture = CreateVerticalLineTexture(graphicsDevice, Game1.windowHeight - (4 * pocketRadius) - (2 * tablePocketSpacing), 3, Color.Black);
            int baulkLineCentreX = (Game1.windowWidth + (6 * pocketRadius) + (3 * tablePocketSpacing)) / 5; // 1/5th across the playing surface (not 1/5th across entire table)
            baulkLine = new(baulkLineTexture, new Vector2(baulkLineCentreX, Game1.windowHeight / 2));
        }


        // Game-loop methods for use in Game1:

        public override void Update(GraphicsDevice graphicsDevice, GameTime gameTime)
        {
            // restart match with 'R':
            if (Keyboard.GetState().IsKeyDown(Keys.R))
            {
                LoadInitialContent(graphicsDevice);
            }

            // Pocket-PoolBall collisions before PoolBall-PoolBall collisions because deleting is less intensive than calculating all collision trajectories
            // and if a PoolBall is deleted, less collision work needs to be done
            foreach (Pocket _pocket in pockets)
            {
                _pocket.Update(gameTime);
            }

            DoAllPoolBallPoolBallCollisions();

            // foreach means that PoolBalls removed from the array aren't updated:
            foreach (PoolBall _poolBall in poolBalls)
            {
                _poolBall.Update(gameTime);
            }

            foreach (Cushion _cushion in cushions)
            {
                _cushion.Update(gameTime);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            Game1.instance.GraphicsDevice.Clear(backgroundColor); // background colour

            Game1._spriteBatch.Begin();

            baulkLine.Draw(Game1._spriteBatch);

            bottomRightTriangle.Draw(Game1._spriteBatch);
            topRightTriangle.Draw(Game1._spriteBatch);
            topLeftTriangle.Draw(Game1._spriteBatch);
            bottomLeftTriangle.Draw(Game1._spriteBatch);

            foreach (Cushion _cushion in cushions)
            {
                _cushion.Draw(Game1._spriteBatch);
            }

            foreach (Pocket _pocket in pockets)
            {
                _pocket.Draw(Game1._spriteBatch);
            }

            // foreach means that PoolBalls removed from the array aren't drawn
            foreach (PoolBall _poolBall in poolBalls)
            {
                _poolBall.Draw(Game1._spriteBatch);
            }

            Game1._spriteBatch.End();
        }


        // Misc. methods:

        /// <summary>
        /// Checks if all PoolBalls are stationary (i.e. not moving).
        /// </summary>
        /// <returns>true if all PoolBalls are stationary, false if not.</returns>
        /// <remarks>Since PoolBalls are the only things that move, it only needs to check all PoolBalls to make sure everything is stationary.</remarks>
        public static bool IsAllStationary()
        {
            foreach (PoolBall poolBall in poolBalls)
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
            for (int i = 0; i < poolBalls.Count - 1; i++)
            {
                for (int j = i + 1; j < poolBalls.Count; j++) // j = i + 1 prevents repeatedly checking the same pairs
                {
                    if (poolBalls[i] == poolBalls[j]) // no need to check if it collides with itself
                    { continue; }
                    else
                    {
                        if (Vector2.Distance(poolBalls[i].position, poolBalls[j].position) < poolBalls[i].radius * 2)
                        {
                            Vector2 relativePositionVector = poolBalls[i].position - poolBalls[j].position; // the vector that is normal to the collision surface (aka other ball), pointing towards poolBalls[i]
                            Vector2 unitNormalVector = relativePositionVector / relativePositionVector.Length(); // normalised to have a magnitude of 1

                            // adjusting velocities:
                            Vector2 newVelocity = poolBalls[i].velocity + (Vector2.Dot(poolBalls[j].velocity - poolBalls[i].velocity, unitNormalVector) * unitNormalVector);
                            Vector2 newOtherVelocity = poolBalls[j].velocity + (Vector2.Dot(poolBalls[i].velocity - poolBalls[j].velocity, unitNormalVector) * unitNormalVector);
                            poolBalls[j].velocity = newOtherVelocity; //* PoolBall.poolBallpoolBallCoefficientOfRestitution;
                            poolBalls[i].velocity = newVelocity; //* PoolBall.poolBallpoolBallCoefficientOfRestitution;

                            // adjusting positions to prevent PoolBalls overlapping and getting stuck inside of each other:
                            poolBalls[j].position += ((0.5f * relativePositionVector.Length()) - poolBalls[i].radius) * unitNormalVector; // moving it half the distance they overlap
                            poolBalls[i].position -= ((0.5f * relativePositionVector.Length()) - poolBalls[i].radius) * unitNormalVector; // moving it half the distance they overlap
                        }
                    }
                }
            }
        }


        // Texture creating methods:

        /// <summary>
        /// Creates a texture for a circle with a specified radius and colour.
        /// </summary>
        /// <returns>The circle texture.</returns>
        /// <remarks>This is relatively intensive since it iterates through an array whose size is the number of pixels in the texture (e.g. size 1600 for a radius of 20px), but it's only called a few times and only during <see cref="Initialize"/> so it shouldn't notably impact performance.</remarks>
        public static Texture2D CreateBlockColouredCircleTexture(GraphicsDevice graphicsDevice, int radius, Color textureColour)
        {
            int diameter = radius * 2;
            Texture2D circleTexture = new(graphicsDevice, diameter, diameter); // empty texture the size of the circle
            Color[] pixelColours = new Color[diameter * diameter];

            // iterating through array as if it were a coordinate grid to find points easier:
            for (int xCoordinate = 0; xCoordinate < diameter; xCoordinate++)
            {
                for (int yCoordinate = 0; yCoordinate < diameter; yCoordinate++)
                {
                    // centre is at the point: (radius, radius)
                    int distanceSquared = ((radius - xCoordinate) * (radius - xCoordinate)) + ((radius - yCoordinate) * (radius - yCoordinate)); // square root isn't necessary because it can be compared to radius^2

                    if (distanceSquared < radius * radius) // note: <= produces some spikey edges whereas < doesn't
                    {
                        pixelColours[xCoordinate + (yCoordinate * diameter)] = textureColour;
                    }
                    else
                    {
                        pixelColours[xCoordinate + (yCoordinate * diameter)] = Color.Transparent;
                    }
                }
            }
            circleTexture.SetData(pixelColours); // maps the array of colours onto the texture
            return circleTexture;
        }

        /// <summary>
        /// <para>[unfinished, not implemented]</para>
        /// Creates a texture for the eight ball with a specified radius.
        /// </summary>
        /// <returns>The eight ball texture.</returns>
        /// <remarks>This is relatively intensive since it iterates through an array whose size is the number of pixels in the texture (e.g. size 1600 for a radius of 20px), but it's only called a few times and only during <see cref="Initialize"/> so it shouldn't notably impact performance.</remarks>
        public static Texture2D CreateEightBallTexture(GraphicsDevice graphicsDevice, int radius)
        {
            int diameter = radius * 2;
            Texture2D circleTexture = new(graphicsDevice, diameter, diameter); // empty texture the size of the circle
            Color[] pixelColours = new Color[diameter * diameter];


            circleTexture.SetData(pixelColours); // maps the array of colours onto the texture
            return circleTexture;
        }

        /// <summary>
        /// Creates a vertical line texture with a specified radius and colour.
        /// </summary>
        /// <returns>The vertical line texture.</returns>
        public static Texture2D CreateVerticalLineTexture(GraphicsDevice graphicsDevice, int length, int thickness, Color textureColour)
        {
            Texture2D lineTexture = new(graphicsDevice, thickness, length); // empty texture the size of the line
            Color[] pixelColours = new Color[length * thickness];

            for (int i = 0; i < length * thickness; i++)
            {
                pixelColours[i] = textureColour;
            }

            lineTexture.SetData(pixelColours); // maps the array of colours onto the texture
            return lineTexture;
        }

        /// <summary>
        /// Creates a trapezium texture with a specified radius and colour. 
        /// Its sides with be parallel to the vertical and horizonal, and 45 degrees from the vertical and horizontal 
        /// (e.g. if orientation == 1, it will look like: ◢■◣)
        /// </summary>
        /// <param name="length">The length of the bottom side when orientation == 1</param>
        /// <param name="width">The height when orientation == 1</param>
        /// <param name="orientation">
        /// <para>if orientation == 1, it will look like: ◢■◣</para>
        /// <para>if orientation == 2, it will look like: ◢■◣ rotated 90 degrees anticlockwise</para>
        /// <para>if orientation == 3, it will look like: ◥■◤</para>
        /// <para>if orientation == 4, it will look like: ◥■◤ rotated 90 degrees anticlockwise</para>
        /// </param>
        /// <returns>The trapezium texture.</returns>
        /// <remarks>This is relatively intensive since it iterates through an array whose size is the number of pixels in the texture (e.g. size 50000 for a length of 500px and a width of 100px), but it's only called a few times and only during <see cref="Initialize"/> so it shouldn't notably impact performance.</remarks>
        public static Texture2D CreateTrapeziumTexture(GraphicsDevice graphicsDevice, int length, int width, Color textureColour, int orientation)
        {
            if (orientation < 1 | orientation > 4)
            {
                throw new ArgumentException("orientation must be between 1 and 4 inclusive");
            }

            Color[] pixelColours = new Color[length * width];
            Texture2D trapeziumTexture;

            switch (orientation)
            {
                case 1:
                    trapeziumTexture = new(graphicsDevice, length, width); // empty texture the size of the trapezium
                    for (int column = 0; column < length; column++)
                    {
                        for (int row = 0; row < width; row++)
                        {
                            if ((column >= width - row) && (column <= length - (width - row)))
                            {
                                pixelColours[column + (row * length)] = textureColour;
                            }
                            else
                            {
                                pixelColours[column + (row * length)] = Color.Transparent;
                            }
                        }
                    }
                    break;
                case 2:
                    trapeziumTexture = new(graphicsDevice, width, length); // empty texture the size of the trapezium
                    for (int row = 0; row < length; row++)
                    {
                        for (int column = 0; column < width; column++)
                        {
                            if (row <= width) // upper section
                            {
                                if (column >= width - row)
                                {
                                    pixelColours[column + (row * width)] = textureColour;
                                }
                                else
                                {
                                    pixelColours[column + (row * width)] = Color.Transparent;
                                }
                            }
                            else if (row >= length - width) // lower section
                            {
                                if (column >= row - (length - width))
                                {
                                    pixelColours[column + (row * width)] = textureColour;
                                }
                                else
                                {
                                    pixelColours[column + (row * width)] = Color.Transparent;
                                }
                            }
                            else // middle section
                            {
                                pixelColours[column + (row * width)] = textureColour;
                            }
                        }
                    }
                    break;
                case 3:
                    trapeziumTexture = new(graphicsDevice, length, width); // empty texture the size of the trapezium
                    for (int column = 0; column < length; column++)
                    {
                        for (int row = 0; row < width; row++)
                        {
                            if ((column >= row) && (column <= length - row))
                            {
                                pixelColours[column + (row * length)] = textureColour;
                            }
                            else
                            {
                                pixelColours[column + (row * length)] = Color.Transparent;
                            }
                        }
                    }
                    break;
                case 4:
                    trapeziumTexture = new(graphicsDevice, width, length); // empty texture the size of the trapezium
                    for (int row = 0; row < length; row++)
                    {
                        for (int column = 0; column < width; column++)
                        {
                            if (row <= width) // upper section
                            {
                                if (column <= row)
                                {
                                    pixelColours[column + (row * width)] = textureColour;
                                }
                                else
                                {
                                    pixelColours[column + (row * width)] = Color.Transparent;
                                }
                            }
                            else if (row >= length - width) // lower section
                            {
                                if (column <= length - row)
                                {
                                    pixelColours[column + (row * width)] = textureColour;
                                }
                                else
                                {
                                    pixelColours[column + (row * width)] = Color.Transparent;
                                }
                            }
                            else // middle section
                            {
                                pixelColours[column + (row * width)] = textureColour;
                            }
                        }
                    }
                    break;
                default: // unreachable, but necessary so that C# thinks that trapeziumTexture is always defined
                    trapeziumTexture = null;
                    break;
            }

            trapeziumTexture.SetData(pixelColours); // maps the array of colours onto the texture
            return trapeziumTexture;
        }

        /// <summary>
        /// Creates a right-angles triangle texture with two equal-length sides and a specified side length and colour.
        /// Its sides with be parallel to the vertical and horizonal, and 45 degrees from either the vertical or horizontal.
        /// (e.g. if orientation == 1, it will look like: ◢)
        /// </summary>
        /// <param name="orientation">
        /// <para>if orientation == 1, it will look like: ◢</para>
        /// <para>if orientation == 2, it will look like: ◥</para>
        /// <para>if orientation == 3, it will look like: ◤</para>
        /// <para>if orientation == 4, it will look like: ◣</para>
        /// </param>
        /// <param name="sideLength">The length of the triangle's two perpendicular</param>
        /// <returns>The triangle texture.</returns>
        /// <remarks>This is relatively intensive since it iterates through an array whose size is the number of pixels in the texture (e.g. size 2500 for a sideLength of 50px), but it's only called a few times and only during <see cref="Initialize"/> so it shouldn't notably impact performance.</remarks>
        public static Texture2D CreateRightAngledTriangleTexture(GraphicsDevice graphicsDevice, int sideLength, Color textureColour, int orientation)
        {

            Texture2D lineTexture = new(graphicsDevice, sideLength, sideLength); // empty texture the size of the line
            Color[] pixelColours = new Color[sideLength * sideLength];

            switch (orientation)
            {
                case 1:
                    for (int row = 0; row < sideLength; row++)
                    {
                        for (int column = 0; column < sideLength; column++)
                        {
                            if (column >= sideLength - row)
                            {
                                pixelColours[column + (row * sideLength)] = textureColour;
                            }
                            else
                            {
                                pixelColours[column + (row * sideLength)] = Color.Transparent;
                            }
                        }
                    }
                    break;
                case 2:
                    for (int row = 0; row < sideLength; row++)
                    {
                        for (int column = 0; column < sideLength; column++)
                        {
                            if (column >= row)
                            {
                                pixelColours[column + (row * sideLength)] = textureColour;
                            }
                            else
                            {
                                pixelColours[column + (row * sideLength)] = Color.Transparent;
                            }
                        }
                    }
                    break;
                case 3:
                    for (int row = 0; row < sideLength; row++)
                    {
                        for (int column = 0; column < sideLength; column++)
                        {
                            if (column <= sideLength - row)
                            {
                                pixelColours[column + (row * sideLength)] = textureColour;
                            }
                            else
                            {
                                pixelColours[column + (row * sideLength)] = Color.Transparent;
                            }
                        }
                    }
                    break;
                case 4:
                    for (int row = 0; row < sideLength; row++)
                    {
                        for (int column = 0; column < sideLength; column++)
                        {
                            if (column <= row)
                            {
                                pixelColours[column + (row * sideLength)] = textureColour;
                            }
                            else
                            {
                                pixelColours[column + (row * sideLength)] = Color.Transparent;
                            }
                        }
                    }
                    break;
            }

            lineTexture.SetData(pixelColours); // maps the array of colours onto the texture
            return lineTexture;
        }
    }
}
