using Microsoft.Xna.Framework;

namespace PongOut
{
    /// <summary>
    /// A message that persists a limited ammount of time
    /// </summary>
    public class FlashedMessage
    {
        public static readonly Color DEFAULT_COLOR = Color.White;
        const float DEFAULT_TARGET_TIME_ALIVE = 2000;

        public string Text { get; private set; } 

        float timeAlive = 0;
        float targetTimeAlive;

        public FlashedMessage(string text, float time = DEFAULT_TARGET_TIME_ALIVE) { 
            Text = text;
            targetTimeAlive = time;
        }


        public bool Kill { get; private set; } = false;

        public void Update(GameTime gt)
        {
            timeAlive += gt.ElapsedGameTime.Milliseconds;
            if (timeAlive >= targetTimeAlive)
                Kill = true;
        }

        public override string ToString()
        {
            return Text; 
        }

    }
}
