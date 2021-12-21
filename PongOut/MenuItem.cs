using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PongOut
{
    class MenuItem<T>
    {
        Texture2D texture;
        Vector2 position;
        T currentState;

        public MenuItem(Texture2D texture, Vector2 position, T currentState)
        {
            this.texture = texture;
            this.position = position;
            this.currentState = currentState;
        }

        public Texture2D Texture { get => texture; }
        public Vector2 Position { get => position; }
        public T State { get => currentState; }
    }
}
