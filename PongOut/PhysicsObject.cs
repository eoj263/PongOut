using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace PongOut
{
    public abstract class PhysicsObject : MovingObject
    {

        public bool HitboxActive { get; protected set; } = true;

        protected PhysicsObject(Vector2 position, Texture2D texture = null) : base(position, texture)
        {
            collidingWith = new Queue<PhysicsObject>();
        }

        public Rectangle Rect
        {
            get => new Rectangle(Position.ToPoint(), Texture.Bounds.Size);
        }

        protected void RestrictToScreenBounds() { 
            if(Position.X < 0 && Velocity.X < 0)
            {
                Velocity = new Vector2(0.1f, Velocity.Y);
            }
            else if(Position.X > GameElements.World.Size.X && Velocity.X > 0)
            {
                Velocity = new Vector2(-0.1f, Velocity.Y);
            }

            if(Position.Y < 0 && Velocity.Y < 0)
            {
                Velocity = new Vector2(Velocity.X, 0.1f);
            }
            else if(Position.Y > GameElements.World.Size.Y && Velocity.Y > 0)
            {
                Velocity = new Vector2(Velocity.X, -0.1f);
            }
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