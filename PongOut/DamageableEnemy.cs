using Microsoft.Xna.Framework;

namespace PongOut
{
    public abstract class DamageableEnemy : Enemy, IDamageable
    {
        public float Health { get; private set; }

        public DamageableEnemy(Vector2 position, float health) : base(position)
        {
            Health = health;
        }


        float timeAnimatingDamage = 0;
        bool playingDamageAnimation = false;
        protected void StartDamageAnimation() { 
            playingDamageAnimation = true;
            Color = Color.Red;
            timeAnimatingDamage = 0;
        }

        protected void StopDamageAnimation()
        {
            playingDamageAnimation = false;
            Color = Color.White;
        }

        public bool Damage(float ammount)
        {
            Health -= ammount;
            StartDamageAnimation();
            
            if(Health < 0)
                IsAlive = false;
            return true;
        }

        public override void Update(GameWindow gw, GameTime gt)
        {
            if (playingDamageAnimation)
            {
                timeAnimatingDamage += gt.ElapsedGameTime.Milliseconds;

                if(timeAnimatingDamage > 0.1f)
                    StopDamageAnimation();
            }

            base.Update(gw, gt);
        }
    }
}
