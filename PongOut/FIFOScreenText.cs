using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace PongOut
{
    /// <summary>
    /// First in first out screen text. When text is added it will be displayed untill there is too much text and then old messages will be removed
    /// </summary>
    public class FIFOScreenText : ScreenText
    {

        Queue<string> buffer;
        private int bufferSize;

        public FIFOScreenText(Vector2 position, int bufferSize) : base(position)
        {
            this.bufferSize = bufferSize;
            buffer = new Queue<string>(bufferSize);
        }

        public void Log(string message)
        {
            buffer.Enqueue(message);
            while (buffer.Count > bufferSize)
                buffer.Dequeue();
        }

        public override void Draw(SpriteBatch sb)
        {
            Text = string.Join('\n', buffer);
            base.Draw(sb);
        }
   }
}
