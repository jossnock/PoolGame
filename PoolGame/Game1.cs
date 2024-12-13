using Microsoft.Xna.Framework;
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
        public enum ScreenState // screen states
        {
            MainMenu,
            Match,
            Settings
        }
        public static ScreenState _screenState;

        public static GraphicsDeviceManager _graphics;
        public static SpriteBatch _spriteBatch;
        public static Desktop _mainMenuDesktop;

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
        public static Texture2D cueBallTexture; // [placeholder texture], should be ~1/20 the height of the table, currently 40x40 pixels

        public static ObjectBall _eightBall;
        public static Texture2D eightBallTexture; // [placeholder texture], should be ~1/20 the height of the table, currently 40x40 pixels

        public static ObjectBall _solidObjectBall1;
        public static ObjectBall _solidObjectBall2;
        public static ObjectBall _solidObjectBall3;
        public static ObjectBall _solidObjectBall4;
        public static ObjectBall _solidObjectBall5;
        public static ObjectBall _solidObjectBall6;
        public static ObjectBall _solidObjectBall7;
        public static Texture2D solidObjectBallTexture; // [placeholder texture], should be ~1/20 the height of the table, currently 40x40 pixels

        public static ObjectBall _stripedObjectBall1;
        public static ObjectBall _stripedObjectBall2;
        public static ObjectBall _stripedObjectBall3;
        public static ObjectBall _stripedObjectBall4;
        public static ObjectBall _stripedObjectBall5;
        public static ObjectBall _stripedObjectBall6;
        public static ObjectBall _stripedObjectBall7;
        public static Texture2D stripedObjectBallTexture; // [placeholder texture], should be ~1/20 the height of the table, currently 40x40 pixels

        public static List<PoolBall> poolBalls; // List (not array) so that PoolBalls can be removed


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


        //public Texture2D testTexture;
        //public static Sprite testSprite;


        // constants:
        // rounded up so that things don't overlap because they are mainly used for positioning
        public const float sqrt_2 = 1.4142135624f; // to not need to repeatedly use Math.Sqrt(2), plus C# doesn't like the results of Math.Sqrt() being assigned as a constant
        public const float sqrt_3 = 1.7320508076f; // to not need to repeatedly use Math.Sqrt(3), plus C# doesn't like the results of Math.Sqrt() being assigned as a constant

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

            // screen setups:

            MyraEnvironment.Game = this; // initialising Myra
            MainMenu.LoadInitialContent(GraphicsDevice);

            // Add it to the screen:
            _mainMenuDesktop = new Desktop();
            _mainMenuDesktop.Root = MainMenu.grid;

            Match.LoadInitialContent(GraphicsDevice);

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
                _mainMenu.Update(GraphicsDevice, gameTime);
                break;

            case ScreenState.Match:
                Match _match = new();
                _match.Update(GraphicsDevice, gameTime);
                break;
            case ScreenState.Settings:
                Match _setings = new();
                _setings.Update(GraphicsDevice, gameTime);
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
