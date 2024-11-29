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

        public CueBall _cueBall;
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
        public static Pocket[] pockets; // Array (not list) because Pockets don't need to be removed

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            // [placeholder resolution]
            windowWidth = 1280;
            windowHeight = 720;
            _graphics.PreferredBackBufferWidth = windowWidth; // default window width
            _graphics.PreferredBackBufferHeight = windowHeight; // default window height
            _graphics.ApplyChanges();

            Window.AllowUserResizing = true;
        }

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
                for (int j = i + 1; j < poolBalls.Count; j++)
                {
                    if (poolBalls[i] == poolBalls[j]) // no need to check if it collides with itself
                    { continue; }
                    else
                    {
                        if (Vector2.Distance(poolBalls[i].position, poolBalls[j].position) <= poolBalls[i].radius * 2)
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

        protected override void Initialize()
        {
            previousKeyboardState = Keyboard.GetState(); // getting the starting state of the keyboard, so that fullscreen can be used
            _screenState = ScreenState.MainMenu;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            previousKeyboardState = Keyboard.GetState(); // getting the starting state of the keyboard so that hotkeys can be used

            // UI Setup:

            // initialising Myra and loading the MyraMain.xmmp file:
            MyraEnvironment.Game = this;

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
            poolBallRadius = 20;
            pocketRadius = 36;
            tablePocketSpacing = 16;


            // objects:

            // PoolBalls:

            cueBallTexture = Content.Load<Texture2D>("CueBall_transparent_40x40");
            _cueBall = new CueBall(cueBallTexture, poolBallRadius);

            eightBallTexture = cueBallTexture;
            _eightBall = new ObjectBall(cueBallTexture, poolBallRadius);

            float sqrt3 = (float)Math.Sqrt(3); // to not need to repeatedly use Math.Sqrt(3)
            const int spacing = 3; // to avoid frame-1 collisions

            solidObjectBallTexture = Content.Load<Texture2D>("ObjectBall_solid_transparent_40x40");
            stripedObjectBallTexture = Content.Load<Texture2D>("ObjectBall_striped_transparent_40x40");

            // (derivation for exact positions in writeup):

            // first column:
            _solidObjectBall1 = new ObjectBall(solidObjectBallTexture, new Vector2(_eightBall.position.X - (2 * poolBallRadius * sqrt3) - (2 * spacing), _eightBall.position.Y), poolBallRadius, false);

            // second column:
            _solidObjectBall2 = new ObjectBall(solidObjectBallTexture, new Vector2(_eightBall.position.X - (poolBallRadius * sqrt3) - spacing, _eightBall.position.Y + poolBallRadius + spacing), poolBallRadius, false);
            _stripedObjectBall1 = new ObjectBall(stripedObjectBallTexture, new Vector2(_eightBall.position.X - (poolBallRadius * sqrt3) - spacing, _eightBall.position.Y - poolBallRadius - spacing), poolBallRadius, false);

            // third column:
            _stripedObjectBall2 = new ObjectBall(stripedObjectBallTexture, new Vector2(_eightBall.position.X, _eightBall.position.Y + (2 * poolBallRadius) + spacing), poolBallRadius, false);
            // (eight ball goes here)
            _solidObjectBall3 = new ObjectBall(solidObjectBallTexture, new Vector2(_eightBall.position.X, _eightBall.position.Y - (2 * poolBallRadius) - spacing), poolBallRadius, false);

            // fourth column:
            _solidObjectBall4 = new ObjectBall(solidObjectBallTexture, new Vector2(_eightBall.position.X + (poolBallRadius * sqrt3) + spacing, _eightBall.position.Y + (3 * poolBallRadius) + (2 * spacing)), poolBallRadius, false);
            _stripedObjectBall3 = new ObjectBall(stripedObjectBallTexture, new Vector2(_eightBall.position.X + (poolBallRadius * sqrt3) + spacing, _eightBall.position.Y + poolBallRadius + spacing), poolBallRadius, false);
            _solidObjectBall5 = new ObjectBall(solidObjectBallTexture, new Vector2(_eightBall.position.X + (poolBallRadius * sqrt3) + spacing, _eightBall.position.Y - poolBallRadius - spacing), poolBallRadius, false);
            _stripedObjectBall4 = new ObjectBall(stripedObjectBallTexture, new Vector2(_eightBall.position.X + (poolBallRadius * sqrt3) + spacing, _eightBall.position.Y - (3 * poolBallRadius) - (2 * spacing)), poolBallRadius, false);

            // fith column:
            _stripedObjectBall5 = new ObjectBall(stripedObjectBallTexture, new Vector2(_eightBall.position.X + (2 * poolBallRadius * sqrt3) + (2 * spacing), _eightBall.position.Y + (4 * poolBallRadius) + (2 * spacing)), poolBallRadius, false);
            _solidObjectBall6 = new ObjectBall(solidObjectBallTexture, new Vector2(_eightBall.position.X + (2 * poolBallRadius * sqrt3) + (2 * spacing), _eightBall.position.Y + (2 * poolBallRadius) + spacing), poolBallRadius, false);
            _stripedObjectBall6 = new ObjectBall(stripedObjectBallTexture, new Vector2(_eightBall.position.X + (2 * poolBallRadius * sqrt3) + (2 * spacing), _eightBall.position.Y), poolBallRadius, false);
            _stripedObjectBall7 = new ObjectBall(stripedObjectBallTexture, new Vector2(_eightBall.position.X + (2 * poolBallRadius * sqrt3) + (2 * spacing), _eightBall.position.Y - (2 * poolBallRadius) - spacing), poolBallRadius, false);
            _solidObjectBall7 = new ObjectBall(solidObjectBallTexture, new Vector2(_eightBall.position.X + (2 * poolBallRadius * sqrt3) + (2 * spacing), _eightBall.position.Y - (4 * poolBallRadius) - (2 * spacing)), poolBallRadius, false);

            poolBalls = new List<PoolBall> { _cueBall, _eightBall,
                                         _solidObjectBall1, _solidObjectBall2, _solidObjectBall3, _solidObjectBall4, _solidObjectBall5, _solidObjectBall6, _solidObjectBall7,
                                         _stripedObjectBall1, _stripedObjectBall2, _stripedObjectBall3, _stripedObjectBall4, _stripedObjectBall5, _stripedObjectBall6, _stripedObjectBall7};


            // Pockets:

            //pocketRadius
            //tablePocketSpacing

            pocketTexture = Content.Load<Texture2D>("CueBall_transparent_72x72"); // [placeholder]
            _pocketTopLeft = new Pocket(pocketTexture, new Vector2(pocketRadius + tablePocketSpacing, pocketRadius + tablePocketSpacing), pocketRadius);
            _pocketTopMiddle = new Pocket(pocketTexture, new Vector2(Game1.windowWidth / 2, pocketRadius + tablePocketSpacing), pocketRadius);
            _pocketTopRight = new Pocket(pocketTexture, new Vector2(Game1.windowWidth - pocketRadius - tablePocketSpacing, pocketRadius + tablePocketSpacing), pocketRadius);
            _pocketBottomLeft = new Pocket(pocketTexture, new Vector2(pocketRadius + tablePocketSpacing, Game1.windowHeight - pocketRadius - tablePocketSpacing), pocketRadius);
            _pocketBottomMiddle = new Pocket(pocketTexture, new Vector2(Game1.windowWidth / 2, Game1.windowHeight - pocketRadius - tablePocketSpacing), pocketRadius);
            _pocketBottomRight = new Pocket(pocketTexture, new Vector2(Game1.windowWidth - pocketRadius - tablePocketSpacing, Game1.windowHeight - pocketRadius - tablePocketSpacing), pocketRadius);

            pockets = new Pocket[6] { _pocketTopLeft,    _pocketTopMiddle,    _pocketTopRight,
                                      _pocketBottomLeft, _pocketBottomMiddle, _pocketBottomRight };

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
