using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PongOut
{
    public class World
    {

        Random rand = new Random();

        public bool GameOver { get; private set; }

        /// <summary>
        ///  GameObjects are stored by their id
        /// </summary>
        private Dictionary<int, GameObject> gameObjects;


        /// <summary>
        /// Stores all ids of GameObjects that are also WorldObjects
        /// </summary>
        private SortedVector<int> worldObjects;

        /// <summary>
        /// Stores all ids of GameObjects that are also PhysicsObjects 
        /// </summary>
        private SortedVector<int> physicsObjects;

        /// <summary>
        /// Stores all ids of GameObjects that are also EnemySpawners. Note enemySpawners are not automatically added to this list by the AddObject function 
        /// </summary>
        private SortedVector<int> enemySpawners;

        /// <summary>
        /// So that we know which gameObjectId the next game object should have
        /// </summary>
        private int currentGameObjectId = 0;

        private Player player;
        private ScreenText infoText;


        /// <summary>
        /// Objects that should be removed next frame
        /// </summary>
        private Queue<int> objectsToRemoveQueue = new Queue<int>();

        /// <summary>
        /// Objects that should be added next frame
        /// </summary>
        private Queue<object> objectsToAddQueue = new Queue<object>();

        /// <summary>
        /// Size of the world
        /// </summary>
        public Vector2 Size { get; private set; }

        /// <summary>
        /// The current score
        /// </summary>
        public int Score => player.Score;

        int level = 0;

        /// <param name="offset">The minimum distance to the edge of the screen</param>
        public Vector2 RandomOnScreenCoordinate(float offset)
        {
            return new Vector2(
                RandomXOnScreenCoordinate(offset),
                RandomYOnScreenCoordinate(offset));
        }

        /// <param name="offset">How far away from the screen the object should spawn</param>
        public Vector2 RandomOffscreenCoordinate(float offset)
        {
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

        /// <summary>
        /// Generates an on screen x-cooordinate
        /// </summary>
        /// <param name="offset"> The minimum distance to the edge of the screen</param>
        public float RandomXOnScreenCoordinate(float offset = 0)
        {
            return (float)rand.NextDouble() * (Size.X - 2 * offset) + offset;
        }

        /// <summary>
        /// Generates an on screen y-cooordinate
        /// </summary>
        /// <param name="offset"> The minimum distance to the edge of the screen</param>
        public float RandomYOnScreenCoordinate(float offset = 0)
        {
            return (float)rand.NextDouble() * (Size.Y - 2 * offset) + offset;
        }


        public void Draw(SpriteBatch sb)
        {
            DrawUI(sb);

            for (int i = 0; i < worldObjects.Length; i++)
            {
                int id = worldObjects[i];
                (gameObjects[id] as WorldObject).Draw(sb);
            }
        }


        protected void DrawUI(SpriteBatch sb)
        {
            infoText.Draw(sb);
        }

        public void GenerateSpawnersForCurrentLevel() { 

            TemporarySpawner zombieSpawner = new TemporarySpawner(
                (pos) =>
                {
                    LoadAndAddObject(new Zombie(pos, player));
                },
                () => RandomOffscreenCoordinate(20),
                (10 + level * 2) * 1000, // How long this spawner should last 
                (int)(2 + level * 1.2) // How many zombies should spawn for this level
                );

            TemporarySpawner shootingZombieSpawner = new TemporarySpawner(
                (pos) =>
                {
                    LoadAndAddObject(new ShootingZombie(pos, player));
                },
                () => RandomOffscreenCoordinate(20),
                (12 + level * 3) * 1000,
                (int)(level * 1.5)
                );

            int zombieSpawnerId = LoadAndAddObject(zombieSpawner);
            int shootingZombieSpawnerId = LoadAndAddObject(shootingZombieSpawner);

            // Since AddObject does not manage the enemySpawnerList
            enemySpawners.Add(zombieSpawnerId); 
            enemySpawners.Add(shootingZombieSpawnerId);
        }


        public void LoadInitialState(GameWindow window)
        {
            gameObjects = new Dictionary<int, GameObject>();
            worldObjects = new SortedVector<int>(false);
            physicsObjects = new SortedVector<int>(false);
            enemySpawners = new SortedVector<int>(false);

            Size = window.ClientBounds.Size.ToVector2();

            player = new Player(new Vector2(500, 200));
            LoadAndAddObject(player);

            infoText = new ScreenText(new Vector2(20, 20));
            LoadObject(infoText);


            GenerateSpawnersForCurrentLevel(); 

            PersistentSpawner healthPackSpawner = new PersistentSpawner(
                (pos) => LoadAndAddObject(new HealthPack(pos)),
                () => RandomOnScreenCoordinate(20),
                5000, 10000);

            PersistentSpawner healthRaiserSpawner = new PersistentSpawner(
                (pos) => LoadAndAddObject(new MaxHealthRaiser(pos)),
                () => RandomOnScreenCoordinate(20),
                5000, 20000);

            PersistentSpawner coinSpawner = new PersistentSpawner(
                (pos) => LoadAndAddObject(new Coin(pos)),
                () => RandomOnScreenCoordinate(20),
                0, 15000);

            LoadAndAddObject(healthPackSpawner);
            LoadAndAddObject(healthRaiserSpawner);
            LoadAndAddObject(coinSpawner);
        }

        /// <param name="obj"></param>
        public int LoadAndAddObject(object obj)
        {
            LoadObject(obj);
            return AddObject(obj);
        }

        /// <summary>
        /// Removes an object from all lists where it is stored and calls tries to call the objects "OnRemove" function
        /// </summary>
        /// <param name="id"></param>
        private void RemoveObject(int id)
        {
            GameObject obj;
            if(gameObjects.TryGetValue(id, out obj))
            {
                if (obj is IOnRemove)
                    (obj as IOnRemove).OnRemove();
            }

            gameObjects.Remove(id);
            worldObjects.Remove(id);
            physicsObjects.Remove(id);
            enemySpawners.Remove(id);
        }

        /// <summary>
        /// Adds the object to the GameObjectList where the object gets an ID. Furthermore this function adds the object to relevant "convenience" lists
        /// </summary>
        /// <param name="toAdd"></param>
        /// <returns></returns>
        int AddObject(object toAdd)
        {
            if (inGameObjectItteration)
            {
                QueueAddObject(toAdd);
                return currentGameObjectId++;
            }

            if (toAdd is GameObject)
                gameObjects.Add(currentGameObjectId++, toAdd as GameObject);
            if (toAdd is WorldObject) worldObjects.Add(currentGameObjectId - 1);
            if (toAdd is PhysicsObject) physicsObjects.Add(currentGameObjectId - 1);
            return currentGameObjectId - 1;
        }

        /// <summary>
        /// If the object implements IContent the object will get a chans to load it's assets
        /// </summary>
        /// <param name="toLoad"></param>
        public void LoadObject(object toLoad)
        {
            if (toLoad is IContent) GameElements.LoadContentsOf(toLoad as IContent);
        }

        /// <summary>
        /// True when world is updating and removing GameObjects
        /// </summary>
        bool inGameObjectItteration = false;

        /// <summary>
        /// All logic of levels
        /// </summary>
        public void HandleLevels()
        {
            if (enemySpawners.Length > 0)
                return;

            level++; 
            int pointsToGrant = level * 10;
            GameElements.FlashMessage(
                new FlashedMessage($"Grattis du har nu nått nivå {level}. Du belönas därför med {pointsToGrant} poäng")
                );
            player.Score += pointsToGrant;

            GenerateSpawnersForCurrentLevel();
        }


        public void Update(GameWindow window, GameTime gameTime)
        {

            ProcessGameObjectQueues();
            HandleLevels();

            inGameObjectItteration = true;
            foreach (KeyValuePair<int, GameObject> kv in gameObjects)
            {
                if (!kv.Value.IsAlive)
                {
                    // Remove object if it is not a player
                    if (!(kv.Value is Player))
                        RemoveObject(kv.Key);
                    continue;
                }

                kv.Value.Update(window, gameTime);
            }
            inGameObjectItteration = false;

            DoCollisions();

            infoText.Text = $"Liv: {player.Health}/{player.maxHealth}\nPoäng: {player.Score}\nNivå: {level}";

            if (!player.IsAlive)
                GameOver = true;
        }

        /// <summary>
        /// Add an object next frame
        /// </summary>
        /// <param name="obj"></param>
        void QueueAddObject(object obj)
        {
            objectsToAddQueue.Enqueue(obj);
        }

        /// <summary>
        /// Removes and adds objects that are queued for removal/adding
        /// </summary>
        void ProcessGameObjectQueues()
        {
            while (objectsToRemoveQueue.Count > 0)
            {
                RemoveObject(objectsToRemoveQueue.Dequeue());
            }

            while (objectsToAddQueue.Count > 0)
            {
                AddObject(objectsToAddQueue.Dequeue());
            }
        }


        public void DoCollisions()
        {

            // Find all collisions
            for (int i = 0; i < physicsObjects.Length; i++)
            {
                for (int j = i + 1; j < physicsObjects.Length; j++)
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

            // Make all objects manage the collisions
            for (int i = 0; i < physicsObjects.Length; i++)
            {
                int id = physicsObjects[i];
                ((PhysicsObject)gameObjects[id]).DispatchCollisions();
            }
        }
    }
}
