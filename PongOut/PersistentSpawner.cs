using Microsoft.Xna.Framework;
using System;

namespace PongOut
{
    public class PersistentSpawner : Spawner
    {
        private float minSpawnTime, maxSpawnTime;

        /// <summary>
        /// Generates a spawner that will persist untill killed by external sources. 
        /// </summary>
        /// <param name="spawn"></param>
        /// <param name="genSpawnLocation"></param>
        /// <param name="minSpawnTime">The least ammount of time between spawns</param>
        /// <param name="maxSpawnTime">The max ammount of time between spawns</param>
        public PersistentSpawner(Action<Vector2> spawn, Func<Vector2> genSpawnLocation, float minSpawnTime, float maxSpawnTime) : base(spawn, genSpawnLocation)
        {
            if (minSpawnTime > maxSpawnTime)
                throw new ArgumentException("Max spawn time cannot be greater than min spawntime");

            this.minSpawnTime = minSpawnTime;
            this.maxSpawnTime = maxSpawnTime;
        }

        protected override float GenerateTimeToNextSpawn()
        {
            return (float)rand.NextDouble() * (maxSpawnTime - minSpawnTime) + minSpawnTime;
        }
    }
}
