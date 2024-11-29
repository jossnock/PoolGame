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
            if (this.Type == ObjectType.Rectangle)
            {
                this.boundingBox = new Rectangle((int)this.position.X, (int)this.position.Y, this.boundingBox.Width, this.boundingBox.Height);
            }

            base.Update(gameTime);
        }

        /*

        public void CheckAndDoCollision(CollideObject otherObject)
        {
            if (this.Type == ObjectType.Rectangle & otherObject.Type == ObjectType.Rectangle)
            {
                if (IsCollidingRectangleRectangle(this, otherObject))
                {

                }
            }

            else if (this.Type == ObjectType.Rectangle & otherObject.Type == ObjectType.Circle)
            {
                if (IsCollidingRectangleCircle(this, otherObject))
                {

                }
            }

            else if (this.Type == ObjectType.Circle & otherObject.Type == ObjectType.Rectangle)
            {
                if (IsCollidingRectangleCircle(otherObject, this))
                {

                }
            }

            else if (this.Type == ObjectType.Circle & otherObject.Type == ObjectType.Circle)
            {
                if (IsCollidingCircleCircle(this, otherObject))
                {

                }
            }
        }


        // methods for checking if CollideObject is colliding with another CollideObject:

        private bool IsCollidingRectangleRectangle(CollideObject rectangle1, CollideObject rectangle2)
        {
            if (rectangle1.boundingBox.Intersects(rectangle2.boundingBox))
            {
                return true;
            }
            return false;
        }

        private bool IsCollidingRectangleCircle(CollideObject rectangle, CollideObject circle)
        {
            // finding closest point on rectangle to circle:
            float closestX = MathHelper.Clamp(circle.position.X, rectangle.boundingBox.Left, rectangle.boundingBox.Right);
            float closestY = MathHelper.Clamp(circle.position.Y, rectangle.boundingBox.Top, rectangle.boundingBox.Bottom);

            // checking if it's within the circle's radius:
            if (Vector2.Distance(new Vector2(closestX, closestY), circle.position) < circle.radius)
            {
                return true;
            }

            return false;
        }

        private bool IsCollidingCircleCircle(CollideObject circle1, CollideObject circle2)
        {
            if (Vector2.Distance(circle1.position, circle2.position) < (circle1.radius + circle2.radius))
            {
                return true;
            }
            return false;
        }

        */
    }
}