using Microsoft.Xna.Framework;

namespace PongOut
{
    public class Gun : GameObject
    {

        static float DEFAULT_COOLDOWN = 6000;

        private float timeSinceUse;
        private float cooldown;

        private PhysicsObject user;

        float? bulletDamage;
        float? bulletSpeed; 


        public Gun(PhysicsObject user, float? cooldown = null, float? bulletDamage = null, float? bulletSpeed = null)
        {
            this.user = user;

            if (!cooldown.HasValue)
                cooldown = DEFAULT_COOLDOWN;

            this.cooldown = cooldown.Value;
            timeSinceUse = this.cooldown;

            this.bulletDamage = bulletDamage;
            this.bulletSpeed = bulletSpeed;
        }

        public override void Update(GameWindow gw, GameTime gt)
        {
            timeSinceUse += gt.ElapsedGameTime.Milliseconds;
        }

        public bool Use(Vector2 facing) {
            if (!AbleToFire())
                return false;

            Fire(facing);
            timeSinceUse = 0;
            return true;
        }

        public bool AbleToFire() {
            if(timeSinceUse >= cooldown)
                return true;

            return false;
        }

        void Fire(Vector2 facing)
        {
            Bullet b = new Bullet(user, user.Position, facing, bulletDamage, bulletSpeed);
            GameElements.World.LoadAndAddObject(b);
        }
    }
}
