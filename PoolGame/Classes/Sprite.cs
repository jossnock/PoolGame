using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using static PoolGame.Classes.CollideObject;

namespace PoolGame.Classes
{
    public class Sprite // internal (can't directly create a Sprite object)
    {
        public Texture2D texture {  get; set; }
        public Vector2 position { get; set; } // a sprite's position is at its centre unless otherwise specified

        // constructor for rectangles:
        public Sprite(Texture2D _texture, Vector2 _initialPosition)
        {
            texture = _texture;
            position = _initialPosition;
        }

        public Sprite() // allowing CueBall to have a constructor that doesn't need initialPosition
        {

        }

        public virtual void Update(GameTime gameTime)
        {

        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(
               texture,
               position,
               null,
               Color.White,
               0f,
               new Vector2(texture.Width / 2, texture.Height / 2),
               Vector2.One,
               SpriteEffects.None,
               0f
           );
        }
    }
}
