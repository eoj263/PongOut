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

        float timeAnimatingDamage = 0;
        bool playingDamageAnimation = false;

        public Player(Vector2 position, float maxHealth = DEFAULT_MAX_HEALTH) : base(position, null)
        {
            this.maxHealth = maxHealth;
            Health = maxHealth;

            gun = new Gun(this, 250, 30, 7);
            GameElements.World.LoadAndAddObject(gun);
            ActiveCollisionLayer |= CollisionLayers.Bullet | CollisionLayers.PlayerEnemy | CollisionLayers.PlayerCollectable;
        }

        public override void OnCollision(PhysicsObject obj) {}

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
            HandleDamageAnimation(gt);

            base.Update(gw, gt);
        }

        private void HandleDamageAnimation(GameTime gt)
        {
            if (playingDamageAnimation)
            {
                timeAnimatingDamage += gt.ElapsedGameTime.Milliseconds;

                if(timeAnimatingDamage > 0.1f)
                    StopDamageAnimation();
            }
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

            // Check if we should ender gun mode
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
            StartDamageAnimation();

            if (Health <= 0)
                IsAlive = false;

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
}
