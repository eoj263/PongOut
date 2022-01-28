using Microsoft.Xna.Framework;

namespace PongOut
{
    public abstract class Collectable : PhysicsObject
    {
        public Collectable(Vector2 position) : base(position, null)
        {
            Position = position;
        }

        public override void OnCollision(PhysicsObject other)
        {
            // Could be fired if the item has already been collected
            if (!IsAlive)
                return;

            if (other is ICollector)
            {
                OnCollected(other as ICollector);
            }
        }

        protected abstract void OnCollected(ICollector collector);

    }
}
