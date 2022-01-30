using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.IO;

namespace PongOut
{
    public class Bullet : PhysicsObject, IContent
    {
        static readonly string CONTENT_PATH = "bullet";
        static readonly string DEFAULT_TEXTURE_PATH = Path.Join(CONTENT_PATH, "defaultTexture");

        static Texture2D defaultTexture; 

        static readonly float DEFAULT_SPEED = 4;
        static readonly float DEFAULT_DAMAGE_AMMOUNT = 20;

        static readonly float OFFSCREEN_DESTORY_DISTANCE = 20; 
        PhysicsObject shooter;

        float dammageAmmount;

        public Bullet(PhysicsObject shooter, Vector2 position, Vector2 direction, float? dammageAmmount = null, float? speed = null) : base(position, null) {
            if (!speed.HasValue)
                speed = DEFAULT_SPEED;

            if (!dammageAmmount.HasValue)
                dammageAmmount = DEFAULT_DAMAGE_AMMOUNT;

            this.shooter = shooter;

            Position = position;
            Rotation = Rotation;

            Velocity = Vector2.Normalize(direction) * speed.Value;
            this.dammageAmmount = dammageAmmount.Value;

            ActiveCollisionLayer = CollisionLayers.Bullet;
        }

        public void LoadContent(ContentManager cm)
        {
            if(defaultTexture == null)
                defaultTexture = cm.Load<Texture2D>(DEFAULT_TEXTURE_PATH);

            Texture = defaultTexture;
            CenterOrigin();
        }

        public override void OnCollision(PhysicsObject other)
        {
            // Cannot collide with the shooter
            if (shooter == other)
                return;

            if (other is IDamageable)
            {
                bool damageDealt = (other as IDamageable).Damage(dammageAmmount);
                if (damageDealt) { 
                    IsAlive = false;

                    // Add points when a player kills an enemy 
                    if(shooter is Player && other is Enemy && !other.IsAlive)
                    {
                        (shooter as Player).Score += (other as Enemy).PointsWhenKilled;
                    }
                }
            }
        }

        

        public override void Update(GameWindow gw, GameTime gt)
        {
            Vector2 worldSize = GameElements.World.Size;

            // X
            if (Position.X < -OFFSCREEN_DESTORY_DISTANCE || Position.X > worldSize.X + OFFSCREEN_DESTORY_DISTANCE)
                IsAlive = false;

            // Y
            if (Position.Y < -OFFSCREEN_DESTORY_DISTANCE || Position.Y > worldSize.Y + OFFSCREEN_DESTORY_DISTANCE)
                IsAlive = false;

            base.Update(gw, gt);
        }
    }
}
