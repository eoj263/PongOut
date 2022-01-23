using Microsoft.Xna.Framework;
using System;
using System.Diagnostics.CodeAnalysis;

namespace PongOut
{
    public abstract class GameObject : IUpdatable
    {
        //public int UpdatePriority { get; set; } = 0;

        public bool IsAlive
        {
            get; protected set;
        } = true;

        //protected World world;
        //public GameObject(World w)
        //{
        //    world = w;
        //}

        //public int CompareTo([AllowNull] GameObject other)
        //{
        //    return UpdatePriority - other.UpdatePriority;
        //}

        public abstract void Update(GameWindow gw, GameTime gt);
    }
}
