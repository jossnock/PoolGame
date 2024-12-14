using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using PoolGame.Classes;
using Myra;
using Myra.Graphics2D.UI;

namespace PoolGame
{
    public class Game1 : Game
    {
        public static Game1 instance; // instance of Game1 so that Game1's static methods can be used in other classes

        public enum ScreenState // screen states
        {
            MainMenu,
            Match,
            Settings
        }
        public static ScreenState _screenState;

        public static GraphicsDeviceManager _graphics;
        public static SpriteBatch _spriteBatch;

        public static int windowWidth;
        public static int windowHeight;

        private KeyboardState previousKeyboardState;

        public Game1()
        {
            instance = this;
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


        /// <summary>
        /// Handles initialisation logic, loads any non-graphical resources, and calls <see cref="LoadContent"/>
        /// </summary>
        /// <remarks>Called when <see cref="Game.Run"/> is called (1 time on startup).</remarks>
        protected override void Initialize()
        {
            previousKeyboardState = Keyboard.GetState(); // getting the starting state of the keyboard, so that fullscreen can be used
            _screenState = ScreenState.MainMenu; // always dispays the main menu on startup

            base.Initialize();
        }

        /// <summary>
        /// Loads anything not loaded by <see cref="Initialize"/>
        /// </summary>
        /// <remarks>Called by <see cref="Initialize"/> (1 time on startup).</remarks>
        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            previousKeyboardState = Keyboard.GetState(); // getting the starting state of the keyboard so that hotkeys can be used

            MyraEnvironment.Game = this; // initialising Myra for the UI

            // MainMenu screen setup:
            var mainMenu = new MainMenu(); // since LoadInitialContent() isn't static, an instance is needed
            mainMenu.LoadInitialContent(GraphicsDevice);

            base.LoadContent();
        }

        /// <summary>
        /// Updates the game by (when specified) recieving input, altering attributes, and calling methods.
        /// </summary>
        /// <remarks>Called 1 time every frame.</remarks>
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


            switch (_screenState) // varies depending on which screen is active
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

        /// <summary>
        /// Draws all active sprites.
        /// </summary>
        /// <remarks>Called 1 time every frame.</remarks>
        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            switch (_screenState) // varies depending on which screen is active
            {
                case ScreenState.MainMenu:
                    MainMenu _mainMenu = new();
                    _mainMenu.Draw(gameTime);
                    break;

                case ScreenState.Match:
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
