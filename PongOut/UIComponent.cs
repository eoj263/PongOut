using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PongOut
{
    public abstract class UIComponent : IUpdatable, IDrawable
    {
        public abstract void Draw(SpriteBatch sb);
        public void Update(GameWindow gw, GameTime gt)
        {}
    }


}
