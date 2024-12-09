﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using PoolGame.Classes;
using Myra;
using Myra.Graphics2D.UI;
using System.IO;
using System.Diagnostics;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Data.Common;
using Myra.Graphics2D;

namespace PoolGame
{
    public class Game1 : Game
    {
        enum ScreenState // screen states
        {
            MainMenu,
            Match,
            Settings
        }
        ScreenState _screenState;

        public static GraphicsDeviceManager _graphics;
        public static SpriteBatch _spriteBatch;
        public static Desktop _desktop;

        public static int windowWidth { get; set; }
        public static int windowHeight { get; set; }

        private KeyboardState previousKeyboardState;

        // sizing:
        // using the approx. scale 1cm : 8px
        public static int poolBallRadius;
        public static int pocketRadius;
        public static int tablePocketSpacing;

        // pool balls:

        public static CueBall _cueBall;
        public Texture2D cueBallTexture; // [placeholder texture], should be ~1/20 the height of the table, currently 40x40 pixels

        public ObjectBall _eightBall;
        public Texture2D eightBallTexture; // [placeholder texture], should be ~1/20 the height of the table, currently 40x40 pixels

        public ObjectBall _solidObjectBall1;
        public ObjectBall _solidObjectBall2;
        public ObjectBall _solidObjectBall3;
        public ObjectBall _solidObjectBall4;
        public ObjectBall _solidObjectBall5;
        public ObjectBall _solidObjectBall6;
        public ObjectBall _solidObjectBall7;
        public Texture2D solidObjectBallTexture; // [placeholder texture], should be ~1/20 the height of the table, currently 40x40 pixels

        public ObjectBall _stripedObjectBall1;
        public ObjectBall _stripedObjectBall2;
        public ObjectBall _stripedObjectBall3;
        public ObjectBall _stripedObjectBall4;
        public ObjectBall _stripedObjectBall5;
        public ObjectBall _stripedObjectBall6;
        public ObjectBall _stripedObjectBall7;
        public Texture2D stripedObjectBallTexture; // [placeholder texture], should be ~1/20 the height of the table, currently 40x40 pixels

        public static List<PoolBall> poolBalls; // List (not array) so that PoolBalls can be removed


        // pockets:
        public Pocket _pocketTopLeft;
        public Pocket _pocketTopMiddle;
        public Pocket _pocketTopRight;
        public Pocket _pocketBottomLeft;
        public Pocket _pocketBottomMiddle;
        public Pocket _pocketBottomRight;
        public Texture2D pocketTexture;
        public static Pocket[] pockets;


        // cushions:

        public Cushion _cushionTopLeft;
        public Cushion _cushionTopRight;
        public Texture2D cushionTopTexture;

        public Cushion _cushionMiddleLeft;
        public Texture2D cushionMddleLeftTexture;

        public Cushion _cushionMiddleRight;
        public Texture2D cushionMddleRightTexture;

        public Cushion _cushionBottomLeft;
        public Cushion _cushionBottomRight;
        public Texture2D cushionBottomTexture;

        public static Cushion[] cushions;


        // misc sprites:

        public static Sprite baulkLine;
        public Texture2D baulkLineTexture;

        public static Sprite bottomRightTriangle;
        public Texture2D bottomRightTriangleTexture;
        public static Sprite topRightTriangle;
        public Texture2D topRightTriangleTexture;
        public static Sprite topLeftTriangle;
        public Texture2D topLeftTriangleTexture;
        public static Sprite bottomLeftTriangle;
        public Texture2D bottomLeftTriangleTexture;


        //public Texture2D testTexture;
        //public static Sprite testSprite;


        // constants:
        public const float sqrt_2 = 1.41421356237f; // to not need to repeatedly use Math.Sqrt(2), plus C# doesn't like the results of Math.Sqrt() being assigned as a constant
        public const float sqrt_3 = 1.73205080757f; // to not need to repeatedly use Math.Sqrt(3), plus C# doesn't like the results of Math.Sqrt() being assigned as a constant

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            // [placeholder resolution]
            windowWidth = 1280; // Convert.ToInt32(Console.ReadLine()); ; // default 1280
            windowHeight = (9 * windowWidth) / 16; // 720 if windowWidth = 1280
            _graphics.PreferredBackBufferWidth = windowWidth; // default window width
            _graphics.PreferredBackBufferHeight = windowHeight; // default window height
            _graphics.ApplyChanges();

            Window.AllowUserResizing = true;
        }


        // Match screen methods:

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
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
                        if (Vector2.Distance(poolBalls[i].position, poolBalls[j].position) <= poolBalls[i].radius * 2) // [todo: try using < rather than <=]
                        {
                            Vector2 relativePositionVector = poolBalls[i].position - poolBalls[j].position; // the vector that is normal to the collision surface (aka other ball), pointing towards poolBalls[i]
                            Vector2 unitNormalVector = relativePositionVector / relativePositionVector.Length(); // normalised to have a magnitude of 1

                            Vector2 newVelocity = poolBalls[i].velocity + (Vector2.Dot(poolBalls[j].velocity - poolBalls[i].velocity, unitNormalVector) * unitNormalVector);
                            Vector2 newOtherVelocity = poolBalls[j].velocity + (Vector2.Dot(poolBalls[i].velocity - poolBalls[j].velocity, unitNormalVector) * unitNormalVector);

                            poolBalls[j].velocity = newOtherVelocity;
                            poolBalls[i].velocity = newVelocity;
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
        private Texture2D CreateBlockColouredCircleTexture(int radius, Color textureColour)
        {
            int diameter = radius * 2;
            Texture2D circleTexture = new(GraphicsDevice, diameter, diameter); // empty texture the size of the circle
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
        private Texture2D CreateEightBallTexture(int radius)
        {
            int diameter = radius * 2;
            Texture2D circleTexture = new(GraphicsDevice, diameter, diameter); // empty texture the size of the circle
            Color[] pixelColours = new Color[diameter * diameter];


            circleTexture.SetData(pixelColours); // maps the array of colours onto the texture
            return circleTexture;
        }

        /// <summary>
        /// Creates a vertical line texture with a specified radius and colour.
        /// </summary>
        /// <returns>The vertical line texture.</returns>
        private Texture2D CreateVerticalLineTexture(int length, int thickness, Color textureColour)
        {
            Texture2D lineTexture = new(GraphicsDevice, thickness, length); // empty texture the size of the line
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
        private Texture2D CreateTrapeziumTexture(int length, int width, Color textureColour, int orientation) 
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
                    trapeziumTexture = new(GraphicsDevice, length, width); // empty texture the size of the trapezium
                    for (int column = 0; column < length; column++)
                    {
                        for (int row = 0; row < width; row++)
                        {
                            if ((column >= width - row) & (column <= length - (width - row)))
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
                    trapeziumTexture = new(GraphicsDevice, width, length); // empty texture the size of the trapezium
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
                    trapeziumTexture = new(GraphicsDevice, length, width); // empty texture the size of the trapezium
                    for (int column = 0; column < length; column++)
                    {
                        for (int row = 0; row < width; row++)
                        {
                            if ((column >= row) & (column <= length - row))
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
                    trapeziumTexture = new(GraphicsDevice, width, length); // empty texture the size of the trapezium
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
        private Texture2D CreateRightAngledTriangleTexture(int sideLength, Color textureColour, int orientation)
        {

            Texture2D lineTexture = new(GraphicsDevice, sideLength, sideLength); // empty texture the size of the line
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

        /// <summary>
        /// Called once when <see cref="Game.Run"/> is called.
        /// Handles initialisation logic, loads any non-graphical resources, and calls <see cref="LoadContent"/>
        /// </summary>
        protected override void Initialize()
        {
            previousKeyboardState = Keyboard.GetState(); // getting the starting state of the keyboard, so that fullscreen can be used
            _screenState = ScreenState.MainMenu;

            base.Initialize();
        }

        /// <summary>
        /// Called once when <see cref="Game.Run"/> is called.
        /// Loads anything not loaded by <see cref="LoadContent"/>
        /// </summary>
        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            previousKeyboardState = Keyboard.GetState(); // getting the starting state of the keyboard so that hotkeys can be used

            // UI Setup:

            MyraEnvironment.Game = this; // initialising Myra

            var grid = new Grid
            {
                RowSpacing = 8,
                ColumnSpacing = 8,
                ShowGridLines = true,
            };

            grid.ColumnsProportions.Add(new Proportion(ProportionType.Auto));
            grid.ColumnsProportions.Add(new Proportion(ProportionType.Auto));
            grid.RowsProportions.Add(new Proportion(ProportionType.Auto));
            grid.RowsProportions.Add(new Proportion(ProportionType.Auto));


            var mainMenuTitle = new Label
            {
                Id = "label",
                Text = "Main Menu"
            };
            grid.Widgets.Add(mainMenuTitle);

            // Button
            var button = new Button
            {
                Content = new Label
                {
                    Text = "Start Match"
                }
            };
            Grid.SetColumn(button, 0);
            Grid.SetRow(button, 1);

            button.Click += (s, a) =>
            {
                _screenState = ScreenState.Match;
            };

            grid.Widgets.Add(button);

            // Add it to the desktop
            _desktop = new Desktop();
            _desktop.Root = grid;

            // sizing:
            poolBallRadius = windowWidth / 64; // 20 for windowWidth = 1280
            pocketRadius = (9 * windowWidth) / 320; // 36 for windowWidth = 1280
            tablePocketSpacing = (windowWidth / 80); // 16 for windowWidth = 1280


            // PoolBalls:

            cueBallTexture = CreateBlockColouredCircleTexture(poolBallRadius, Color.White);
            _cueBall = new(cueBallTexture, poolBallRadius);

            eightBallTexture = CreateBlockColouredCircleTexture(poolBallRadius, Color.Black);
            _eightBall = new(eightBallTexture, poolBallRadius);

            solidObjectBallTexture = CreateBlockColouredCircleTexture(poolBallRadius, Color.Yellow);
            stripedObjectBallTexture = CreateBlockColouredCircleTexture(poolBallRadius, Color.Red);

            const int spacing = 3; // to reduce messy collisions

            // (derivation for exact positions in writeup):

            // first column:
            _solidObjectBall1 = new(solidObjectBallTexture, new Vector2(_eightBall.position.X - (2 * poolBallRadius * sqrt_3) - (2 * spacing), _eightBall.position.Y), poolBallRadius, false);

            // second column:
            _solidObjectBall2 = new(solidObjectBallTexture, new Vector2(_eightBall.position.X - (poolBallRadius * sqrt_3) - spacing, _eightBall.position.Y + poolBallRadius + spacing), poolBallRadius, false);
            _stripedObjectBall1 = new(stripedObjectBallTexture, new Vector2(_eightBall.position.X - (poolBallRadius * sqrt_3) - spacing, _eightBall.position.Y - poolBallRadius - spacing), poolBallRadius, false);

            // third column:
            _stripedObjectBall2 = new(stripedObjectBallTexture, new Vector2(_eightBall.position.X, _eightBall.position.Y + (2 * poolBallRadius) + spacing), poolBallRadius, false);
            // (eight ball goes here)
            _solidObjectBall3 = new(solidObjectBallTexture, new Vector2(_eightBall.position.X, _eightBall.position.Y - (2 * poolBallRadius) - spacing), poolBallRadius, false);

            // fourth column:
            _solidObjectBall4 = new(solidObjectBallTexture, new Vector2(_eightBall.position.X + (poolBallRadius * sqrt_3) + spacing, _eightBall.position.Y + (3 * poolBallRadius) + (2 * spacing)), poolBallRadius, false);
            _stripedObjectBall3 = new(stripedObjectBallTexture, new Vector2(_eightBall.position.X + (poolBallRadius * sqrt_3) + spacing, _eightBall.position.Y + poolBallRadius + spacing), poolBallRadius, false);
            _solidObjectBall5 = new(solidObjectBallTexture, new Vector2(_eightBall.position.X + (poolBallRadius * sqrt_3) + spacing, _eightBall.position.Y - poolBallRadius - spacing), poolBallRadius, false);
            _stripedObjectBall4 = new(stripedObjectBallTexture, new Vector2(_eightBall.position.X + (poolBallRadius * sqrt_3) + spacing, _eightBall.position.Y - (3 * poolBallRadius) - (2 * spacing)), poolBallRadius, false);

            // fith column:
            _stripedObjectBall5 = new(stripedObjectBallTexture, new Vector2(_eightBall.position.X + (2 * poolBallRadius * sqrt_3) + (2 * spacing), _eightBall.position.Y + (4 * poolBallRadius) + (2 * spacing)), poolBallRadius, false);
            _solidObjectBall6 = new(solidObjectBallTexture, new Vector2(_eightBall.position.X + (2 * poolBallRadius * sqrt_3) + (2 * spacing), _eightBall.position.Y + (2 * poolBallRadius) + spacing), poolBallRadius, false);
            _stripedObjectBall6 = new(stripedObjectBallTexture, new Vector2(_eightBall.position.X + (2 * poolBallRadius * sqrt_3) + (2 * spacing), _eightBall.position.Y), poolBallRadius, false);
            _stripedObjectBall7 = new(stripedObjectBallTexture, new Vector2(_eightBall.position.X + (2 * poolBallRadius * sqrt_3) + (2 * spacing), _eightBall.position.Y - (2 * poolBallRadius) - spacing), poolBallRadius, false);
            _solidObjectBall7 = new(solidObjectBallTexture, new Vector2(_eightBall.position.X + (2 * poolBallRadius * sqrt_3) + (2 * spacing), _eightBall.position.Y - (4 * poolBallRadius) - (2 * spacing)), poolBallRadius, false);

            poolBalls = new List<PoolBall> { _cueBall, _eightBall,
                                             _solidObjectBall1, _solidObjectBall2, _solidObjectBall3, _solidObjectBall4, _solidObjectBall5, _solidObjectBall6, _solidObjectBall7,
                                             _stripedObjectBall1, _stripedObjectBall2, _stripedObjectBall3, _stripedObjectBall4, _stripedObjectBall5, _stripedObjectBall6, _stripedObjectBall7};


            // Pockets:

            //pocketRadius
            //tablePocketSpacing

            pocketTexture = CreateBlockColouredCircleTexture(pocketRadius, Color.Black);
            _pocketTopLeft = new Pocket(pocketTexture, new Vector2(pocketRadius + tablePocketSpacing, pocketRadius + tablePocketSpacing), pocketRadius);
            _pocketTopMiddle = new Pocket(pocketTexture, new Vector2(windowWidth / 2, pocketRadius + tablePocketSpacing), pocketRadius);
            _pocketTopRight = new Pocket(pocketTexture, new Vector2(windowWidth - pocketRadius - tablePocketSpacing, pocketRadius + tablePocketSpacing), pocketRadius);
            _pocketBottomLeft = new Pocket(pocketTexture, new Vector2(pocketRadius + tablePocketSpacing, windowHeight - pocketRadius - tablePocketSpacing), pocketRadius);
            _pocketBottomMiddle = new Pocket(pocketTexture, new Vector2(windowWidth / 2, windowHeight - pocketRadius - tablePocketSpacing), pocketRadius);
            _pocketBottomRight = new Pocket(pocketTexture, new Vector2(windowWidth - pocketRadius - tablePocketSpacing, windowHeight - pocketRadius - tablePocketSpacing), pocketRadius);

            pockets = new Pocket[6] { _pocketTopLeft,    _pocketTopMiddle,    _pocketTopRight,
                                      _pocketBottomLeft, _pocketBottomMiddle, _pocketBottomRight };


            // cushions:

            Color cushionColor = Color.Sienna;

            int topBottomCushionLength = (int)((windowWidth / 2) + tablePocketSpacing - (sqrt_2 * pocketRadius));
            int middleCushionLength = (int)(windowHeight - (2 * sqrt_2 * pocketRadius));
            int cushionWidth = (2 * pocketRadius) + tablePocketSpacing;
            int topBottomLeftCushionCentreX = (int)(((windowWidth / 2) + tablePocketSpacing + (sqrt_2 * pocketRadius)) / 2);
            int topBottomRightCushionCentreX = windowWidth - topBottomLeftCushionCentreX;

            cushionTopTexture = CreateTrapeziumTexture(topBottomCushionLength, cushionWidth, cushionColor, 3);
            _cushionTopLeft = new(cushionTopTexture, new Vector2(topBottomLeftCushionCentreX, cushionWidth / 2), topBottomCushionLength, cushionWidth, 3);
            _cushionTopRight = new(cushionTopTexture, new Vector2(topBottomRightCushionCentreX, cushionWidth / 2), topBottomCushionLength, cushionWidth, 3);

            cushionMddleLeftTexture = CreateTrapeziumTexture(middleCushionLength, cushionWidth, cushionColor, 4);
            _cushionMiddleLeft = new(cushionMddleLeftTexture, new Vector2(cushionWidth / 2, windowHeight / 2), middleCushionLength, cushionWidth, 4);

            cushionMddleRightTexture = CreateTrapeziumTexture(middleCushionLength, cushionWidth, cushionColor, 2);
            _cushionMiddleRight = new(cushionMddleRightTexture, new Vector2(windowWidth - (cushionWidth / 2), windowHeight / 2), middleCushionLength, cushionWidth, 2);

            cushionBottomTexture = CreateTrapeziumTexture(topBottomCushionLength, cushionWidth, cushionColor, 1);
            _cushionBottomLeft = new(cushionBottomTexture, new Vector2(topBottomLeftCushionCentreX, windowHeight - (cushionWidth / 2)), topBottomCushionLength, cushionWidth, 1);
            _cushionBottomRight = new(cushionBottomTexture, new Vector2(topBottomRightCushionCentreX, windowHeight - (cushionWidth / 2)), topBottomCushionLength, cushionWidth, 1);

            cushions = new Cushion[] { _cushionTopLeft,    _cushionTopRight, 
                                       _cushionMiddleLeft, _cushionMiddleRight, 
                                       _cushionBottomLeft, _cushionBottomRight };

            // triangles fill in the rest of the sides, they don't need collisions because they are always behind pockets:

            bottomRightTriangleTexture = CreateRightAngledTriangleTexture(2 * (pocketRadius + tablePocketSpacing), cushionColor, 1);
            bottomRightTriangle = new(bottomRightTriangleTexture, new Vector2(windowWidth - (pocketRadius + tablePocketSpacing), windowHeight - (pocketRadius + tablePocketSpacing)));

            topLeftTriangleTexture = CreateRightAngledTriangleTexture(2 * (pocketRadius + tablePocketSpacing), cushionColor, 2);
            topLeftTriangle = new(topLeftTriangleTexture, new Vector2(windowWidth - (pocketRadius + tablePocketSpacing), pocketRadius + tablePocketSpacing));

            topRightTriangleTexture = CreateRightAngledTriangleTexture(2 * (pocketRadius + tablePocketSpacing), cushionColor, 3);
            topRightTriangle = new(topRightTriangleTexture, new Vector2(pocketRadius + tablePocketSpacing, pocketRadius + tablePocketSpacing));

            bottomLeftTriangleTexture = CreateRightAngledTriangleTexture(2 * (pocketRadius + tablePocketSpacing), cushionColor, 4);
            bottomLeftTriangle = new(bottomLeftTriangleTexture, new Vector2(pocketRadius + tablePocketSpacing, windowHeight - (pocketRadius + tablePocketSpacing)));


            // misc. sprites:

            baulkLineTexture = CreateVerticalLineTexture(windowHeight - (4 * pocketRadius) - (2 * tablePocketSpacing), 3, Color.Black);
            int baulkLineCentreX = (windowWidth + (6 * pocketRadius) + (3 * tablePocketSpacing)) / 5; // 1/5th across the playing surface (not 1/5th across entire table)
            baulkLine = new(baulkLineTexture, new Vector2(baulkLineCentreX, windowHeight / 2));



            //testTexture = CreateRightAngledTriangleTexture(GraphicsDevice, 50, Color.Blue, 3);
            //testSprite = new(testTexture, new Vector2(100, 100));

            base.LoadContent();
        }


        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // exit with 'Back' or 'Escape':
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            // toggle fullscreen with 'F':
            if (Keyboard.GetState().IsKeyDown(Keys.F))
            {
                if (!previousKeyboardState.IsKeyDown(Keys.F))
                {
                    _graphics.IsFullScreen = !_graphics.IsFullScreen;
                    _graphics.ApplyChanges();
                }
            }

            previousKeyboardState = Keyboard.GetState(); // re-assign for the next Update()

            switch (_screenState)
            {
            case ScreenState.MainMenu:
                MainMenu _mainMenu = new();
                _mainMenu.Update(gameTime);
                break;

            case ScreenState.Match:
                Match _match = new();
                _match.Update(gameTime);
                break;
            case ScreenState.Settings:
                Match _setings = new();
                _setings.Update(gameTime);
                break;
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            switch (_screenState)
            {
                case ScreenState.MainMenu:
                    GraphicsDevice.Clear(Color.Gray); // background colour
                    MainMenu _mainMenu = new();
                    _mainMenu.Draw(gameTime);
                    break;

                case ScreenState.Match:
                    GraphicsDevice.Clear(new Color(21,88,67)); // background colour
                    Match _match = new();
                    _match.Draw(gameTime);
                    break;
                case ScreenState.Settings:
                    Match _setings = new();
                    _setings.Draw(gameTime);
                    break;
            }
        }
    }
}
