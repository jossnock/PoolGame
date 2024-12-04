using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoolGame.Classes
{
    public class CollideObject : Sprite
    {
        public float radius { get; set; }
        public Rectangle boundingBox { get; set; }
        public enum ObjectType
        {
            Rectangle,
            Circle,
        }
        public ObjectType Type { get; set; }

        // constructor for rectangles:
        public CollideObject(Texture2D _texture, Vector2 _initialPosition, Rectangle _boundingBox)
        {
            texture = _texture;
            position = _initialPosition;
            boundingBox = _boundingBox;
            Type = ObjectType.Rectangle;
        }

        // constructor for circles:
        public CollideObject(Texture2D _texture, Vector2 _position, float _radius)
        {
            texture = _texture;
            position = _position;
            radius = _radius;
            Type = ObjectType.Circle;
        }

        public CollideObject(Texture2D _texture, float _radius) // allowing CueBall to have a constructor that doesn't need initialPosition
        {
            texture = _texture;
            position = Vector2.Zero;
            radius = _radius;
            Type = ObjectType.Circle;
        }

        public override void Update(GameTime gameTime)
        {
            if (Type == ObjectType.Rectangle)
            {
                boundingBox = new Rectangle((int)position.X, (int)position.Y, boundingBox.Width, boundingBox.Height);
            }

            base.Update(gameTime);
        }
    }
}