using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PongOut
{
    public abstract class WorldObject : GameObject, IDrawable
    {
        protected Texture2D Texture { get; set; }
        public Vector2 Position { get; protected set; } = Vector2.Zero;

        protected Color Color { get; set; } = Color.White;

        protected float Rotation { get; set; } = 0;

        protected Rectangle? SourceRectanlge { get; set; } = null;

        protected Vector2 Origin { get; set; } = Vector2.Zero;
        protected Vector2 Scale { get; set; } = Vector2.One;

        protected float LayerDepth = 0;


        public WorldObject(Vector2 position, Texture2D texture = default)
        {
            Texture = texture;
            Position = position;
        }

        public void CenterOrigin()
        {
            this.Origin = Texture.Bounds.Size.ToVector2() / 2; 
        }



        /// <summary>
        /// Is called when this object should be drawn 
        /// </summary>
        /// <param name="sb"></param>
        public virtual void Draw(SpriteBatch sb)
        {
            sb.Draw(Texture, Position, null, Color, Rotation, Origin, Scale, SpriteEffects.None, LayerDepth);
        }
    }
}
