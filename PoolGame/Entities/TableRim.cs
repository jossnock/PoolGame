using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace PoolGame.Entities
{
    internal class TableRim
    {
        private Texture2D texture;
        private Vector2 position;
        private Vector2 velocity;
        private GraphicsDevice graphicsDevice;

        float scaleX;
        float scaleY;
        float scale;
        float scaledWidth;
        float scaledHeight;

        public TableRim(Texture2D texture, Vector2 initPosition, Vector2 velocity, GraphicsDevice graphicsDevice)
        {
            this.texture = texture;
            this.position = initPosition;
            this.velocity = velocity;
            this.graphicsDevice = graphicsDevice;
        }

        public void MoveTo(Vector2 newPosition)
        {
            this.position = newPosition;
        }

        public void Update(GameTime gameTime)
        {
            int windowWidth = graphicsDevice.Viewport.Width;
            int windowHeight = graphicsDevice.Viewport.Height;

            scaleX = (float)windowWidth / texture.Width;
            scaleY = (float)windowHeight / texture.Height;
            scale = Math.Min(scaleX, scaleY);
            scaledWidth = texture.Width * scale;
            scaledHeight = texture.Height * scale;
            position = new Vector2(windowWidth / 2, windowHeight - (scaledHeight / 2));
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(
               texture,
               position,
               null,
               Color.White,
               0f,
               new Vector2(texture.Width / 2, texture.Height / 2),
               new Vector2(scale, scale),
               SpriteEffects.None,
               0f
           );
        }
    }
}
