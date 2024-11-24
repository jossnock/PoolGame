using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using PoolGame.Classes;
using PoolGame.Classes.Screens;
using Myra;
using Myra.Graphics2D.UI;
using System.IO;
using System.Diagnostics;

namespace PoolGame
{
    public class MainMenu : Game
    {
        public GraphicsDeviceManager _graphics;
        public SpriteBatch _spriteBatch;
        public Desktop _desktop;

        public static int windowWidth {  get;  set; }
        public static int windowHeight {  get;  set; }

        private KeyboardState previousKeyboardState;

        public MainMenu()
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

        protected override void Initialize()
        {
            previousKeyboardState = Keyboard.GetState(); // getting the starting state of the keyboard, so that fullscreen can be used

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);


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
                Match1 match = new Match1();
                match.Run();
            };

            grid.Widgets.Add(button);

            // Add it to the desktop
            _desktop = new Desktop();
            _desktop.Root = grid;
        }

        protected override void Update(GameTime gameTime)
        {
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
            base.Update(gameTime);

        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Gray); // background colour

            _desktop.Render(); // renders Myra UI elements

            base.Draw(gameTime);
        }
    }
}
