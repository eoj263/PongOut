using Microsoft.Xna.Framework;
using System;
using System.Diagnostics.CodeAnalysis;

namespace PongOut
{
    /// <summary>
    /// An object that can be added to the game. It is also updateable and Killable
    /// </summary>
    public abstract class GameObject 
    {
        public bool IsAlive
        {
            get; protected set;
        } = true;

        public void Kill()
        {
            this.IsAlive = false;
        }

        public abstract void Update(GameWindow gw, GameTime gt);
    }
}
