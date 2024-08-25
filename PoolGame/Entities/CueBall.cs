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
    internal class CueBall
    {
        private Texture2D texture;
        private Vector2 position;
        private Vector2 velocity;

        public CueBall(Texture2D texture, Vector2 initPosition)
        {
            this.texture = texture;
            this.position = initPosition;
        }

        public void MoveTo(Vector2 newPosition)
        {
            this.position = newPosition;
        }

        public void Velocity()
        {

        }

        public void Update(GameTime gameTime)
        {

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
                Vector2.One,
                SpriteEffects.None,
                0f
            );
        }
    }
}
