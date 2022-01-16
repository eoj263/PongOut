using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PongOut
{
    public abstract class MovingObject : WorldObject
    {
        protected MovingObject(Vector2 position, Texture2D texture = null) : base(position, texture)
        {
        }


        protected Vector2 velocity = Vector2.Zero;

        public override void Update(GameWindow gw, GameTime gt)
        {
            Position += velocity;
        }

    }
}
