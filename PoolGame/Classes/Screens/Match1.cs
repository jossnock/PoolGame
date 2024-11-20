using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace PoolGame.Classes.Screens
{
    internal class Match1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private KeyboardState previousKeyboardState;

        public CueBall _cueBall;
        public Texture2D cueBallTexture;

        public PoolBall _poolBall1;

        public static PoolBall[] poolBalls;

        public Match1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            _graphics.PreferredBackBufferWidth = MainMenu.windowWidth; // default window width
            _graphics.PreferredBackBufferHeight = MainMenu.windowHeight; // default window height
            _graphics.ApplyChanges();
        }

        protected override void Initialize()
        {
            previousKeyboardState = Keyboard.GetState(); // getting the starting state of the keyboard, so that fullscreen can be used

            // initialising objects:
            cueBallTexture = Content.Load<Texture2D>("circle 99x99");
            _cueBall = new CueBall(cueBallTexture, 50);

            _poolBall1 = new PoolBall(cueBallTexture, new Vector2(MainMenu.windowWidth / 2, MainMenu.windowHeight / 2), 50);

            poolBalls = new PoolBall[] { _cueBall, _poolBall1 };

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Enter))
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


            // updating objects:

            _cueBall.Update(gameTime);
            _poolBall1.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White); // background colour

            // main sprite batch:

            _spriteBatch.Begin();

            _cueBall.Draw(_spriteBatch);
            _poolBall1.Draw(_spriteBatch);

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
