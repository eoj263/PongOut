using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace PongOut
{

    public abstract class PhysicsObject : MovingObject
    {
        [Flags]
        public enum CollisionLayers
        {
            PlayerEnemy,
            PlayerCollectable,
            Bullet,
        }

        /// <summary>
        /// Only things that have at least one layer in common will collide. If there are no mutual layers, there will be no collision
        /// </summary>
        public CollisionLayers ActiveCollisionLayer { get; protected set; }

        protected PhysicsObject(Vector2 position, Texture2D texture = null) : base(position, texture)
        {
            collidingWith = new Queue<PhysicsObject>();
        }

        public Rectangle Rect
        {
            get => new Rectangle(Position.ToPoint(), Texture.Bounds.Size);
        }

        /// <summary>
        /// Restricts movement to within the screen bounds
        /// </summary>
        protected void RestrictToScreenBounds() { 
            if(Position.X < 0 && Velocity.X < 0)
            {
                Velocity = new Vector2(0, Velocity.Y);
            }
            else if(Position.X > GameElements.World.Size.X && Velocity.X > 0)
            {
                Velocity = new Vector2(0, Velocity.Y);
            }

            if(Position.Y < 0 && Velocity.Y < 0)
            {
                Velocity = new Vector2(Velocity.X, 0);
            }
            else if(Position.Y > GameElements.World.Size.Y && Velocity.Y > 0)
            {
                Velocity = new Vector2(Velocity.X, 0);
            }
        }

        /// <summary>
        /// Is called by 
        /// </summary>
        /// <param name="other"></param>
        public abstract void OnCollision(PhysicsObject other);

        public bool CheckCollision(PhysicsObject other)
        {
            if ((ActiveCollisionLayer & other.ActiveCollisionLayer) == 0)
                return false;

            return Rect.Intersects(other.Rect);
        }

        public override void Update(GameWindow gw, GameTime gt)
        {
            base.Update(gw, gt);
        }

        /// <summary>
        /// Disbatches all the queued collision events. Calls OnCollisions(collision) for each queued collision
        /// </summary>
        public void DispatchCollisions()
        {
            while(collidingWith.Count > 0)
            {
                OnCollision(collidingWith.Dequeue());
            }
        }

        Queue<PhysicsObject> collidingWith;

        /// <summary>
        /// Registers (queues) a collision event. The collision is then processed when DisbatchCollisions is called
        /// </summary>
        /// <param name="with"></param>
        public void AddCollision(PhysicsObject with)
        {
            collidingWith.Enqueue(with);
        }
    }
}