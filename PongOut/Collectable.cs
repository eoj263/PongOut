using Microsoft.Xna.Framework;

namespace PongOut
{
    /// <summary>
    /// Something that can be picked up
    /// </summary>
    public abstract class Collectable : PhysicsObject
    {
        public static readonly string CONTENT_PATH = "collectable";

        public Collectable(Vector2 position) : base(position, null)
        {
            Position = position;

            // Collectables should be drawn behind other objects
            LayerDepth = 0;
        }

        public override void OnCollision(PhysicsObject other)
        {
            // Could be fired if the item has already been collected
            if (!IsAlive)
                return;
            bool collected = OnCollected(other);

            if (collected)
                IsAlive = false;
        }

        /// <summary>
        /// </summary>
        /// <param name="collector"></param>
        /// <returns>Wheter or not the item was able to be collected</returns>
        protected abstract bool OnCollected(PhysicsObject collector);

    }
}
