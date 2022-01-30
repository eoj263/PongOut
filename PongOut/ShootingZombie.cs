using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.IO;

namespace PongOut
{
    public class ShootingZombie : Zombie, IOnRemove
    {

        public static new readonly string CONTENT_PATH = Path.Combine(Enemy.CONTENT_PATH, "shootingZombie");
        public static readonly string TEXTURE_PATH = Path.Combine(CONTENT_PATH, "defaultTexture");

        public static Texture2D defaultTexture;

        Gun gun;
        public ShootingZombie(Vector2 position, WorldObject target) : base(position, target)
        {
            PointsWhenKilled = 2;
            gun = new Gun(this);

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

        public void OnRemove()
        {
            this.gun.Kill();
        }

        public override void Update(GameWindow gw, GameTime gt)
        {
            gun.Use(facing);
            base.Update(gw, gt);
        }
    }
}
