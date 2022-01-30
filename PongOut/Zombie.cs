using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using System;

namespace PongOut
{
    public class Zombie : DamageableEnemy 
    {
        public static new readonly string CONTENT_PATH = Path.Combine(Enemy.CONTENT_PATH, "zombie");
        public static readonly string WALK_TEXTURE_PATH = Path.Combine(CONTENT_PATH, "walk");

        static readonly float DEFAULT_HEALTH = 100;
        static readonly float DEFAULT_SPEED = 1;

        static readonly int ATTACK_SPEED = 250;
        static readonly int ATTACK_DAMAGE = 5;


        float timeSinceLastAttack;

        static Texture2D walk;


        protected Vector2 facing;
        private WorldObject target;

        public Zombie(Vector2 position, WorldObject target): base(position, DEFAULT_HEALTH)
        {
            this.target = target;

            // Zombies are able to attack as soon as they spawn
            timeSinceLastAttack = ATTACK_SPEED;
        }

        public override void Update(GameWindow gw, GameTime gt)
        {
            facing = Vector2.Normalize(target.Position - Position);
            Rotation = MathF.Atan2(facing.Y, facing.X);

            Velocity = facing * DEFAULT_SPEED;

            timeSinceLastAttack += gt.ElapsedGameTime.Milliseconds;
            base.Update(gw, gt);
        }

        public override void LoadContent(ContentManager cm)
        {
            if(walk == null)
            {
                walk = cm.Load<Texture2D>(WALK_TEXTURE_PATH);
            }

            Texture = walk;
            CenterOrigin();
        }

        public override void OnCollision(PhysicsObject other)
        {
            if (timeSinceLastAttack < ATTACK_SPEED)
                return;

            if(other is Player)
            {
                (other as Player).Damage(ATTACK_DAMAGE);
                timeSinceLastAttack = 0;
            }
        }
    }
}
