using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

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

        public float GenerateTimeToNextSpawn()
        {
            if(targetSpawnCount - spawnCount > 0) // Försök se till att rätt mängd saker spawnat innan tiden går ut
                return (float)rand.NextDouble() * timeLeftToLive / (targetSpawnCount - spawnCount) * Compensation;

            // Har vi spawnat fler enheter än vad som behövs
            GameElements.WriteDebugLine("Using the other thing");
            return (float)rand.NextDouble() * maxTimeAlive / targetSpawnCount * Compensation;

        }
    }

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

                GameElements.WriteDebugLine("spawnCount: " + spawnCount);
                timeToNextSpawn = GenerateTimeToNextSpawn();
            }
        }

        protected abstract float GenerateTimeToNextSpawn();
    }

    public class World
    {

        Random rand = new Random();

        public bool GameOver { get; private set; }

        // Each gameObject will have an id.
        private Dictionary<int, GameObject> gameObjects;

        private SortedVector<int> worldObjects;
        private SortedVector<int> physicsObjects;

        private List<UIComponent> UIComponents;

        private int currentGameObjectId = 0;

        private Player player;

        private ScreenText scoreText;

        private Queue<int> objectsToRemoveQueue = new Queue<int>();
        private Queue<object> objectsToAddQueue = new Queue<object>();

        public Vector2 Size { get; private set; }


        public int CurrentScore => player.Score;


        public Vector2 RandomOffscreenCoordinate(float offset) {
            int edge = rand.Next(4);

            Vector2 location = Vector2.Zero;
            switch (edge)
            {
                case 0: // Top
                    location = new Vector2(RandomXOnScreenCoordinate(), -offset);
                    break;
                case 1: // Right 
                    location = new Vector2(Size.X + offset, RandomYOnScreenCoordinate());
                    break;
                case 2: // Bottom 
                    location = new Vector2(RandomXOnScreenCoordinate(), Size.Y + offset);
                    break;
                case 3: // Left 
                    location = new Vector2(-offset, RandomYOnScreenCoordinate());
                    break;

            }

            return location;
        }

        public float RandomXOnScreenCoordinate()
        {
            return (float)rand.NextDouble() * Size.X;
        }
        public float RandomYOnScreenCoordinate()
        {
            return (float)rand.NextDouble() * Size.Y;
        }


        public void Draw(SpriteBatch sb)
        {
            scoreText.Draw(sb);

            for(int i = 0; i < worldObjects.Length; i++)
            {
                int id = worldObjects[i];
                (gameObjects[id] as WorldObject).Draw(sb);
            }
        }

        public void LoadInitialState(GameWindow window)
        {
            gameObjects = new Dictionary<int, GameObject>();

            worldObjects = new SortedVector<int>(false);
            physicsObjects = new SortedVector<int>(false);

            UIComponents = new List<UIComponent>();

            Size = window.ClientBounds.Size.ToVector2();

            player = new Player(new Vector2(500, 200));
            LoadAndAddObject(player);

            scoreText = new ScreenText(new Vector2(20, 20));
            LoadObject(scoreText);

            ShootingZombie zombie2 = new ShootingZombie(new Vector2(100, 100), player);
            LoadAndAddObject(zombie2);

            Zombie zombie1 = new Zombie(new Vector2(300, 300), player);
            LoadAndAddObject(zombie1);

            Spawner<Zombie> zombieSpawner = new Spawner<Zombie>((pos) =>
                LoadAndAddObject(new Zombie(pos, player)), () => RandomOffscreenCoordinate(50), 30000, 30);
            LoadAndAddObject(zombieSpawner);
        }

        public void LoadAndAddObject(object obj)
        {
            LoadObject(obj);
            AddObject(obj);
        }

        public void RemoveObject(int id)
        {
            gameObjects.Remove(id);
            worldObjects.Remove(id);
            physicsObjects.Remove(id);
        }

        void AddObject(object toAdd)
        {
            if (inGameObjectItteration) {
                QueueAddObject(toAdd);
                return;
            }

            if (toAdd is GameObject)
                gameObjects.Add(currentGameObjectId++, toAdd as GameObject);
            if (toAdd is WorldObject) worldObjects.Add(currentGameObjectId - 1);
            if (toAdd is PhysicsObject) physicsObjects.Add(currentGameObjectId - 1);
        }

        public void LoadObject(object toLoad)
        {
            if (toLoad is IContent) GameElements.LoadContentsOf(toLoad as IContent);
        }

        bool inGameObjectItteration = false;


        public void Update(GameWindow window, GameTime gameTime)
        {
            ProcessGameObjectQueues();

            inGameObjectItteration = true;
            foreach(KeyValuePair<int, GameObject> kv in gameObjects)
            {
                if (!kv.Value.IsAlive && !(kv.Value is Player))
                {
                    RemoveObject(kv.Key);
                    continue;
                }

                kv.Value.Update(window, gameTime);
            }
            inGameObjectItteration = false;

            DoCollisions();

            scoreText.Text = $"Poäng: {player.Score}";

            if (!player.IsAlive)
                GameOver = true;
        }

        void QueueRemoveObject(int id) {
            objectsToRemoveQueue.Enqueue(id);
        }
        void QueueAddObject(object obj) {
            objectsToAddQueue.Enqueue(obj);
        }

        void ProcessGameObjectQueues()
        {
            while(objectsToRemoveQueue.Count > 0)
            {
                RemoveObject(objectsToRemoveQueue.Dequeue());
            }

            while(objectsToAddQueue.Count > 0)
            {
                AddObject(objectsToAddQueue.Dequeue());
            }
        }


        public void DoCollisions() { 

            for(int i = 0; i < physicsObjects.Length; i++)
            {
                for(int j = i + 1; j < physicsObjects.Length; j++)
                {
                    PhysicsObject a = (PhysicsObject)gameObjects[physicsObjects[i]];
                    PhysicsObject b = (PhysicsObject)gameObjects[physicsObjects[j]];

                    bool areColliding = a.CheckCollision(b);
                    if (areColliding)
                    {
                        a.AddCollision(b);
                        b.AddCollision(a);
                    }
                }
            }

            for(int i = 0; i < physicsObjects.Length; i++)
            {
                int id = physicsObjects[i];
                ((PhysicsObject)gameObjects[id]).DispatchCollisions();
            }
        }
    }

    public class SortedVector<T> where T: IComparable<T> 
    {
        List<T> list;
        bool allowDuplicates;

        public SortedVector(bool allowDuplicates = true){
            this.allowDuplicates = allowDuplicates;
            list = new List<T>();
        }
        public int Length => list.Count; 
        
        public T this[int index] {
            get
            {
                return list[index];
            }
        }

        public void Add(T obj)
        {
            int index = Find(obj);
            if (index >= 0 && !allowDuplicates)
                throw new ArgumentException("Duplicate item");
            int insertAt = index;
            if(insertAt < 0)
                insertAt = ~index;

            list.Insert(insertAt, obj);
        }

        public int Find(T obj)
        {
            return list.BinarySearch(obj);
        }

        public bool Remove(T obj)
        {
            int idx = Find(obj);
            if (idx < 0)
                return false;

            list.RemoveAt(idx);
            return true;
        }

        //private class SortedVectorEnumerator : IEnumerator<T>
        //{

        //    private int currentIdx = -1;

        //    public T Current => vector[currentIdx];
        //    object IEnumerator.Current => vector[currentIdx];

        //    public void Dispose()

        //    {
        //    }

        //    public bool MoveNext()
        //    {
        //        currentIdx++;
        //        return (currentIdx < vector.Length); 
        //    }

        //    public void Reset()
        //    {
        //        currentIdx = -1;
        //    }

        //    SortedVector<T> vector;
        //    public SortedVectorEnumerator(SortedVector<T> vector) {
        //        this.vector = vector;
        //    }
        //}
    }


    public class DebugText : ScreenText
    {

        Queue<string> buffer;
        private int bufferSize;


        public DebugText(Vector2 position, int bufferSize) : base(position)
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
