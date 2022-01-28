namespace PongOut
{
    public abstract class GunModifier {
        public int Priority { get; private set; } = 0;

        private Gun gun;

        public GunModifier(Gun gun)
        {
            this.gun = gun;
        }

        public abstract void Apply(ref float cooldown, float? bulletDamage, float bulletSpeed);

        //private float timeSinceUse;
        //private float cooldown;

        //private PhysicsObject user;

        //float? bulletDamage;
        //float? bulletSpeed; 
        public virtual void BeforeUse(ref float timeSinceUse) {
        }
    }
}
