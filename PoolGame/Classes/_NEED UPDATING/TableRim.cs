using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

// ALL OUT OF DATE

namespace PoolGame.Classes
{
    internal class TableRim
    {
        private Texture2D texture;
        private Vector2 position;
        private Vector2 velocity = Vector2.Zero;
        private GraphicsDevice graphicsDevice;

        // calculated variables not assigned by the initialiser:
        float scaleX;
        float scaleY;
        float scale;
        float scaledWidth;
        float scaledHeight;
        int windowWidth;
        int windowHeight;


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
            windowWidth = graphicsDevice.Viewport.Width;
            windowHeight = graphicsDevice.Viewport.Height;

            // adjusting the sprite size to fit with the window:
            scaleX = (float)windowWidth / texture.Width;
            scaleY = (float)windowHeight / texture.Height;
            scale = Math.Min(scaleX, scaleY); // makes sure it fits in the window even if the apect ratio is 1:2+
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
               new Vector2(texture.Width / 2, texture.Height / 2), // centre of sprite
               new Vector2(scale, scale), // enlarge by scale in x and y
               SpriteEffects.None,
               0f
           );
        }
    }
}
