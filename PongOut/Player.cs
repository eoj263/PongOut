using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.IO;
using System;
using System.Diagnostics.CodeAnalysis;

namespace PongOut
{
    public class Player : PhysicsObject, IDamageable, IContent 
    {
        static readonly string CONTENT_PATH= "player";
        static readonly string STAND_TEXTURE_PATH = Path.Join(CONTENT_PATH, "defualt/stand");
        static readonly string GUN_TEXTURE_PATH = Path.Join(CONTENT_PATH, "defualt/gun");
        static readonly float DEFAULT_HEALTH = 30;

        static Texture2D standTexture;
        static Texture2D gunTexture;

        bool gunMode = false;


        Gun gun;

        public float Health { get; private set; }

        public Player(Vector2 position) : base(position, null)
        {
            Health = DEFAULT_HEALTH;
            gun = new Gun(this, 250, 30, 7);
            GameElements.World.LoadAndAddObject(gun);
        }

        public override void OnCollision(PhysicsObject obj) {
        }

        public void LoadContent(ContentManager cm)
        {
            if (gunTexture == null)
            {
                gunTexture = cm.Load<Texture2D>(GUN_TEXTURE_PATH);
            }
            
            if(standTexture == null)
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

            //Velocity = Vector2.Transform(wantedRelativeMovementDirection * speed, Matrix.CreateRotationZ(Rotation));
            base.Update(gw, gt);
        }

        private void UpdateTexture()
        {
            Texture = gunMode ? gunTexture : standTexture;
        }

        Vector2 looking = new Vector2(1,0);


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

            if(kbs.IsKeyDown(moveUpKey) && kbs.IsKeyDown(moveLeftKey) && kbs.IsKeyDown(moveRightKey))
            {
                wanted = Vector2.Zero;
                gunMode = true;
            } else gunMode = false;

            wantedRelativeMovementDirection = wanted;
        }

        //IWeapon equipedWeapon;
        //public bool Equip(IWeapon weapon)
        //{
        //    equipedWeapon = weapon;
        //}

        public bool Damage(float ammount)
        {
            Health -= ammount;
            if (Health < 0)
                IsAlive = false;
            return IsAlive;
        }
    }

    public abstract class Enemy : PhysicsObject, IContent 
    {
        public static readonly string CONTENT_PATH = "enemy";

        protected Enemy(Vector2 position) : base(position, null)
        {
        }

        public abstract void LoadContent(ContentManager cm);
    }

    public abstract class DamageableEnemy : Enemy, IDamageable
    {
        public float Health { get; private set; }

        public DamageableEnemy(Vector2 position, float health) : base(position)
        {
            Health = health;
        }

        public bool Damage(float ammount)
        {
            Health -= ammount;
            
            if(Health < 0)
                IsAlive = false;
            return true;
        }
    }

    public class Bullet : PhysicsObject, IContent
    {
        static readonly string CONTENT_PATH= "bullet";
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
        }

        public void LoadContent(ContentManager cm)
        {
            if(defaultTexture == null)
            {
                defaultTexture = cm.Load<Texture2D>(DEFAULT_TEXTURE_PATH);
            }

            Texture = defaultTexture;
            CenterOrigin();
        }

        public override void OnCollision(PhysicsObject other)
        {
            // Kan inte collidera med sig själv
            if (shooter == other)
                return;

            if (other is IDamageable)
                (other as IDamageable).Damage(dammageAmmount);
        }

        

        public override void Update(GameWindow gw, GameTime gt)
        {
            Vector2 worldSize = GameElements.World.Size;

            // X-led
            if (Position.X < -OFFSCREEN_DESTORY_DISTANCE || Position.X > worldSize.X + OFFSCREEN_DESTORY_DISTANCE)
                IsAlive = false;

            // Y-led
            if (Position.Y < -OFFSCREEN_DESTORY_DISTANCE || Position.Y > worldSize.Y + OFFSCREEN_DESTORY_DISTANCE)
                IsAlive = false;

            base.Update(gw, gt);
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
            gun = new Gun(this);
            // TODO make so that the gun is removed when the zombie dies
            GameElements.World.AddObject(gun);
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
