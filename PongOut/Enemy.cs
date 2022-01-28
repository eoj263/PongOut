using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace PongOut
{
    public abstract class Enemy : PhysicsObject, IContent 
    {
        public static readonly string CONTENT_PATH = "enemy";
         
        public int PointsWhenKilled { get; protected set; } = 1;

        protected Enemy(Vector2 position) : base(position, null)
        {
            this.ActiveCollisionLayer |= CollisionLayers.PlayerEnemy | CollisionLayers.Bullet;
        }

        public abstract void LoadContent(ContentManager cm);
    }
}
