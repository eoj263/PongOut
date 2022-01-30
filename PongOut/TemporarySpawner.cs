using Microsoft.Xna.Framework;
using System;

namespace PongOut
{
    public class TemporarySpawner : Spawner
    {
        const float Compensation = 2;
        int targetSpawnCount;

        float timeLeftToLive => maxTimeAlive - timeAlive;
        float maxTimeAlive;

        /// <summary>
        /// Creates a spawner that should spawn a certain ammount of things and be destroyed after a certain time
        /// </summary>
        /// <param name="spawn"></param>
        /// <param name="genSpawnLocation"></param>
        /// <param name="maxTimeAlive"></param>
        /// <param name="targetSpawnCount"></param>
        public TemporarySpawner(Action<Vector2> spawn, Func<Vector2> genSpawnLocation, float maxTimeAlive, int targetSpawnCount) : base(spawn, genSpawnLocation)
        {
            spawnCount = 0;
            this.maxTimeAlive = maxTimeAlive;
            this.targetSpawnCount= targetSpawnCount;
        }

        protected override float GenerateTimeToNextSpawn()
        {
            if(targetSpawnCount - spawnCount > 0) // Försök se till att rätt mängd saker spawnat innan tiden går ut
                return (float)rand.NextDouble() * timeLeftToLive / (targetSpawnCount - spawnCount) * Compensation;

            // Har vi spawnat fler enheter än vad som behövs
            return (float)rand.NextDouble() * maxTimeAlive / targetSpawnCount * Compensation;
        }

        public override void Update(GameWindow gw, GameTime gt)
        {
            base.Update(gw, gt);
            if (timeAlive >= maxTimeAlive)
                IsAlive = false;
        }
    }
}
