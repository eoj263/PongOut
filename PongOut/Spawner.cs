using Microsoft.Xna.Framework;
using System;

namespace PongOut
{
    public abstract class Spawner : GameObject
    {
        Action<Vector2> spawn;
        Func<Vector2> genSpawnLocation;

        protected float timeAlive = 0;
        float timeToNextSpawn = 0;

        protected int spawnCount;

        protected Random rand = new Random();
        
        /// <summary>
        /// Creates a spawner that should spawn a certain ammount of things and be destroyed after a certain time
        /// </summary>
        /// <param name="spawn"></param>
        /// <param name="genSpawnLocation"></param>
        /// <param name="maxTimeAlive"></param>
        /// <param name="targetSpawnCount"></param>
        public Spawner(Action<Vector2> spawn, Func<Vector2> genSpawnLocation)
        {
            this.spawn = spawn;
            this.genSpawnLocation = genSpawnLocation;
        }

        public override void Update(GameWindow gw, GameTime gt)
        {
            timeAlive += gt.ElapsedGameTime.Milliseconds;
            timeToNextSpawn -= gt.ElapsedGameTime.Milliseconds;
            if (timeToNextSpawn <= 0)
            {
                spawn(genSpawnLocation());
                spawnCount++;

                timeToNextSpawn = GenerateTimeToNextSpawn();
            }
        }

        protected abstract float GenerateTimeToNextSpawn();
    }
}
