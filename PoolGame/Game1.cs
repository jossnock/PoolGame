using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace PoolGame
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        Texture2D tableRimTexture;
        Vector2 tableRimPosition;
        float tableRimScaleX;
        float tableRimScaleY;
        float tableRimScale;
        float tableScaledWidth;
        float tableScaledHeight;


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
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here

            // tableRim properties:
            tableRimTexture = Content.Load<Texture2D>("2-1 rectangle (transparent)");
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            if (Keyboard.GetState().IsKeyDown(Keys.F))
            {
                _graphics.IsFullScreen = !_graphics.IsFullScreen;
                _graphics.ApplyChanges();
            }

            int windowWidth = GraphicsDevice.Viewport.Width;
            int windowHeight = GraphicsDevice.Viewport.Height;

            tableRimScaleX = (float)windowWidth / tableRimTexture.Width;
            tableRimScaleY = (float)windowHeight / tableRimTexture.Height;
            tableRimScale = Math.Min(tableRimScaleX, tableRimScaleY);
            tableScaledWidth = tableRimTexture.Width * tableRimScale;
            tableScaledHeight = tableRimTexture.Height * tableRimScale;
            tableRimPosition = new Vector2(windowWidth / 2, windowHeight - (tableScaledHeight / 2));

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);

            // TODO: Add your drawing code here

            _spriteBatch.Begin();

            _spriteBatch.Draw(
                tableRimTexture,
                tableRimPosition,
                null,
                Color.White,
                0f,
                new Vector2(tableRimTexture.Width / 2, tableRimTexture.Height / 2),
                new Vector2(tableRimScale, tableRimScale),
                SpriteEffects.None,
                0f
            );

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
