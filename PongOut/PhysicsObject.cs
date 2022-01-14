using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PongOut
{
    public abstract class PhysicsObject : MovingObject
    {
        protected PhysicsObject(Vector2 position, Texture2D texture = null) : base(position, texture)
        {
        }

        public override void Update(GameWindow gw, GameTime gt)
        {
            base.Update(gw, gt);
        }
    }
}