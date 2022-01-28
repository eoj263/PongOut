using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.IO;
using System;
using System.Diagnostics.CodeAnalysis;

namespace PongOut
{
    public class Player : PhysicsObject, IDamageable, IContent, IHealable
    {
        static readonly string CONTENT_PATH = "player";
        static readonly string STAND_TEXTURE_PATH = Path.Join(CONTENT_PATH, "defualt/stand");
        static readonly string GUN_TEXTURE_PATH = Path.Join(CONTENT_PATH, "defualt/gun");
        const float DEFAULT_MAX_HEALTH = 30;

        static Texture2D standTexture;
        static Texture2D gunTexture;

        bool gunMode = false;

        public int Score { get; set; } = 0;

        public float maxHealth { get; private set; }

        Gun gun;
        public float Health { get; private set; }

        public Player(Vector2 position, float maxHealth = DEFAULT_MAX_HEALTH) : base(position, null)
        {
            this.maxHealth = maxHealth;
            Health = maxHealth;

            gun = new Gun(this, 250, 30, 7);
            GameElements.World.LoadAndAddObject(gun);
            ActiveCollisionLayer |= CollisionLayers.Bullet | CollisionLayers.Collectable | CollisionLayers.PlayerEnemy | CollisionLayers.PlayerCollectable;
        }

        public override void OnCollision(PhysicsObject obj) {}

        public void LoadContent(ContentManager cm)
        {
            if (gunTexture == null)
            {
                gunTexture = cm.Load<Texture2D>(GUN_TEXTURE_PATH);
            }

            if (standTexture == null)
            {
                standTexture = cm.Load<Texture2D>(STAND_TEXTURE_PATH);
            }

            Texture = standTexture;
            CenterOrigin();
        }


        Vector2 wantedRelativeMovementDirection;

        float speed = 5;
        public override void Update(GameWindow gw, GameTime gt)
        {
            HandleInput();
            UpdateTexture();

            Rotation = MathF.Atan2(looking.Y, looking.X);

            Velocity = wantedRelativeMovementDirection * speed;
            RestrictToScreenBounds();

            base.Update(gw, gt);
        }

        private void UpdateTexture()
        {
            Texture = gunMode ? gunTexture : standTexture;
        }

        Vector2 looking = new Vector2(1, 0);

        Keys moveUpKey = Keys.W;
        Keys moveDownKey = Keys.S;
        Keys moveLeftKey = Keys.A;
        Keys moveRightKey = Keys.D;
        public void HandleInput()
        {
            HandleKeyboardInput();
            HandleMouseInput();
        }

        private void HandleMouseInput()
        {
            MouseState ms = Mouse.GetState();
            var diff = -Position + ms.Position.ToVector2();

            this.looking = Vector2.Normalize(diff);

            if (gunMode && ms.LeftButton == ButtonState.Pressed)
            {
                gun.Use(this.looking);
            }
        }

        private void HandleKeyboardInput()
        {
            Vector2 wanted = Vector2.Zero;
            KeyboardState kbs = Keyboard.GetState();
            if (kbs.IsKeyDown(moveLeftKey))
            {
                wanted.X = -1;
            }

            if (kbs.IsKeyDown(moveRightKey))
            {
                wanted.X = 1;
            }

            if (kbs.IsKeyDown(moveDownKey))
            {
                wanted.Y = 1;
            }

            if (kbs.IsKeyDown(moveUpKey))
            {
                wanted.Y = -1;
            }


            if (wanted.X != 0 && wanted.Y != 0)
            {
                wanted.Normalize();
            }

            if (kbs.IsKeyDown(moveUpKey) && kbs.IsKeyDown(moveLeftKey) && kbs.IsKeyDown(moveRightKey))
            {
                wanted = Vector2.Zero;
                gunMode = true;
            } else gunMode = false;

            wantedRelativeMovementDirection = wanted;
        }

        public bool Damage(float ammount)
        {
            Health -= ammount;
            return true;
        }

        public float Heal(float ammount)
        {
            float toHeal = Math.Min(maxHealth - Health, ammount);
            Health += toHeal;
            return toHeal;
        }

        public void IncreaseMaxHealth(float ammount)
        {
            maxHealth += ammount;
        }
    }

    public interface ICollector
    {
        /// <summary>
        /// Collect an item
        /// </summary>
        /// <param name="c">The item to collect</param>
        /// <returns>It the item was able to be collected</returns>
        bool Collect(Collectable c);
    }


    public class Coin : PlayerCollectable
    {
        const int DEFAULT_SCORE_TO_GIVE = 4; 
        int scoreToGive;
        public Coin(Vector2 position, int scoreAdd = DEFAULT_SCORE_TO_GIVE) : base(position) {
            this.scoreToGive = scoreAdd;
        }

        protected override void OnCollected(Player p)
        {
            p.Score += scoreToGive;
        }
    }

    public abstract class PlayerCollectable : Collectable
    {
        protected PlayerCollectable(Vector2 position) : base(position)
        {
            ActiveCollisionLayer = CollisionLayers.PlayerCollectable;
        }

        protected override sealed void OnCollected(ICollector collector)
        {
            OnCollected(collector as Player);
        }

        protected abstract void OnCollected(Player p);
    }

    public class MaxHealthRaiser : PlayerCollectable
    {
        const float DEFAULT_AMMOUNT = 5;

        float ammount;
        public MaxHealthRaiser(Vector2 position, float ammount = DEFAULT_AMMOUNT) : base(position)
        {
            this.ammount = ammount;
        }

        protected override void OnCollected(Player p)
        {
            p.IncreaseMaxHealth(ammount);
        }
    }


    public class HealthPack : PlayerCollectable
    {

        const float DEFAULT_HEAL_AMMOUNT = 10;

        float healAmmount;
        public HealthPack(Vector2 position, float healAmmount = DEFAULT_HEAL_AMMOUNT) : base(position)
        {
            this.healAmmount = healAmmount;
        }

        protected override void OnCollected(Player p)
        {
            p.Heal(healAmmount);
        }
    }




    //public class Gun 
    //{
    //    public void Attatch(Player p);

    //    public bool Use(Player user)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}


    //public interface IWeapon 
    //{

    //    public Enum Kind
    //    {
    //        gun;
    //    }

    //    /// <summary>
    //    /// Uses a weapon
    //    /// </summary>
    //    /// <param name="positoin"></param>
    //    /// <param name="facing"></param>
    //    /// <returns>If the weapon was able to be used or not</returns>
    //    public abstract bool Use(Player user);
    //}


    public interface IHealable
    {
        /// <summary>
        /// Heal a thing
        /// </summary>
        /// <param name="ammount">The ammount to heal</param>
        /// <returns>The ammount that was healed</returns>
        float Heal(float ammount);
    }

    public interface IDamageable
    {
        /// <summary>
        /// Damage the object
        /// </summary>
        /// <param name="ammount"></param>
        /// <returns>True if the damage was given, false if not</returns>
        bool Damage(float ammount);
    }

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


    public class ShootingZombie : Zombie
    {

        public static new readonly string CONTENT_PATH = Path.Combine(Enemy.CONTENT_PATH, "shootingZombie");
        public static readonly string TEXTURE_PATH = Path.Combine(CONTENT_PATH, "defaultTexture");

        public static Texture2D defaultTexture;


        Gun gun;
        public ShootingZombie(Vector2 position, WorldObject target) : base(position, target)
        {
            PointsWhenKilled = 2;
            gun = new Gun(this);
            // TODO make so that the gun is removed when the zombie dies
            GameElements.World.LoadAndAddObject(gun);
        }

        public override void LoadContent(ContentManager cm)
        {
            if(defaultTexture == null)
            {
                defaultTexture = cm.Load<Texture2D>(TEXTURE_PATH);
            }

            Texture = defaultTexture;
            CenterOrigin();
        }

        public override void Update(GameWindow gw, GameTime gt)
        {
            gun.Use(facing);
            base.Update(gw, gt);
        }
    }


    public class Zombie : DamageableEnemy 
    {
        public static new readonly string CONTENT_PATH = Path.Combine(Enemy.CONTENT_PATH, "zombie");
        public static readonly string WALK_TEXTURE_PATH = Path.Combine(CONTENT_PATH, "walk");

        static readonly float DEFAULT_HEALTH = 100;
        static readonly float DEFAULT_SPEED = 1;
        
        static Texture2D walk;


        protected Vector2 facing;
        private WorldObject target;

        public Zombie(Vector2 position, WorldObject target): base(position, DEFAULT_HEALTH)
        {
            this.target = target;
        }

        public override void Update(GameWindow gw, GameTime gt)
        {
            facing = Vector2.Normalize(target.Position - Position);
            Rotation = MathF.Atan2(facing.Y, facing.X);

            Velocity = facing * DEFAULT_SPEED;


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
            if(other is Player)
            {
                (other as Player).Damage(20);
            }
        }
    }
}
