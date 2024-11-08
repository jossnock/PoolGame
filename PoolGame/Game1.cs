using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using PoolGame.Entities;
using GeonBit.UI;
using GeonBit.UI.Entities;

namespace PoolGame
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private KeyboardState previousKeyboardState;

        PoolBall _cueBall;
        Texture2D cueBallTexture;


        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            _graphics.PreferredBackBufferWidth = 1280; // default window width
            _graphics.PreferredBackBufferHeight = 720; // default window height
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
            UserInterface.Initialize(Content, BuiltinThemes.hd); // Initialises UI


            // loading objects:
            cueBallTexture = Content.Load<Texture2D>("circle 99x99");
            _cueBall = new PoolBall(cueBallTexture, new Vector2(_graphics.PreferredBackBufferWidth / 2, _graphics.PreferredBackBufferHeight / 2), 50);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

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


            // updating objects:

            _cueBall.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White); // background colour

            // main sprite batch:

            _spriteBatch.Begin();

            _cueBall.Draw(_spriteBatch);

            _spriteBatch.End();


            base.Draw(gameTime);
        }
    }
}
