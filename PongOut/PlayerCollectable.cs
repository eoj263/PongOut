using Microsoft.Xna.Framework;

namespace PongOut
{
    /// <summary>
    /// Collectables that can only be collected by a player
    /// </summary>
    public abstract class PlayerCollectable : Collectable
    {
        protected PlayerCollectable(Vector2 position) : base(position)
        {
            ActiveCollisionLayer = CollisionLayers.PlayerCollectable;
        }

        protected override sealed bool OnCollected(PhysicsObject collector)
        {
            if(collector is Player)
                return OnCollected(collector as Player);
            return false;
        }

        protected abstract bool OnCollected(Player p);
    }
}
