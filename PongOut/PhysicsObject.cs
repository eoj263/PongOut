using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace PongOut
{
    public abstract class PhysicsObject : MovingObject
    {

        public bool HitboxActive { get; protected set; }

        protected PhysicsObject(Vector2 position, Texture2D texture = null) : base(position, texture)
        {
            collidingWith = new Queue<PhysicsObject>();
        }

        public Rectangle Rect
        {
            get => new Rectangle(Position.ToPoint(), Texture.Bounds.Size);
        }

        public abstract void OnCollision(PhysicsObject other);

        public bool CheckCollision(PhysicsObject other)
        {
            if (!other.HitboxActive || !HitboxActive)
                return false;

            return Rect.Intersects(other.Rect);
        }

        public override void Update(GameWindow gw, GameTime gt)
        {
            base.Update(gw, gt);
        }

        public void DispatchCollisions()
        {
            while(collidingWith.Count > 0)
            {
                OnCollision(collidingWith.Dequeue());
            }
        }


        Queue<PhysicsObject> collidingWith;
        public void AddCollision(PhysicsObject with)
        {
            collidingWith.Enqueue(with);
        }

    }
}