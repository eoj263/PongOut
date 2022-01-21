using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PongOut
{
    public abstract class PhysicsObject : MovingObject
    {
        protected PhysicsObject(Vector2 position, Texture2D texture = null) : base(position, texture)
        {
        }


        public Rectangle Rect
        {
            get => new Rectangle(Position.ToPoint(), Texture.Bounds.Size);
        }

        public abstract bool OnCollision(PhysicsObject other);

        public bool CheckCollision(PhysicsObject other)
        {
            return Rect.Intersects(other.Rect);
        }

        public override void Update(GameWindow gw, GameTime gt)
        {
            base.Update(gw, gt);
        }
    }
}