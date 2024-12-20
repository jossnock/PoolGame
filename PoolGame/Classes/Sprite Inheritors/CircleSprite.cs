using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PoolGame.Classes
{
    /// <summary>
    /// A subclass of <see cref="Sprite"></see>, used for circular sprites.
    /// </summary>
    /// <remarks>This exists because multiple different objects need circular sprites, but some don't.</remarks>
    public class CircleSprite : Sprite
    {
        public float radius { get; set; }

        public CircleSprite(Texture2D _texture, Vector2 _position, float _radius)
        {
            texture = _texture;
            position = _position;
            radius = _radius;
        }

        public CircleSprite(Texture2D _texture, float _radius) // allowing CueBall to have a constructor that doesn't need initialPosition
        {
            texture = _texture;
            position = Vector2.Zero;
            radius = _radius;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
    }
}
