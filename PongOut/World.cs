using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PongOut
{
    public class World
    {
        public bool GameOver { get; private set; }

        // Each gameObject will have an id.
        private Dictionary<int, GameObject> gameObjects;

        private SortedSet<int> worldObjects;
        private SortedSet<int> physicsObjects;


        private List<UIComponent> UIComponents;


        private int currentGameObjectId = 0;


        private DebugText debugText;
        private Player player;


        public void Draw(SpriteBatch sb)
        {
            foreach (int id in worldObjects.AsEnumerable())
            {
                (gameObjects[id] as WorldObject).Draw(sb);
            }
        }

        public void LoadInitialState(GameWindow window)
        {
            gameObjects = new Dictionary<int, GameObject>();
            worldObjects = new SortedSet<int>();
            physicsObjects = new SortedSet<int>();

            UIComponents = new List<UIComponent>();

            if (GameElements.DebugMode)
            {
                debugText = new DebugText(new Vector2(200, 200), 9);
            }

            player = new Player(new Vector2(200, 200));
            LoadAndAddObject(player);

            Zombie zombie1 = new Zombie(new Vector2(300, 300));
            LoadAndAddObject(zombie1);
        }


        public void WriteDebugLine(string text)
        {
            if (!GameElements.DebugMode)
            {
                return;
            }
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


        public void AddObject(object toAdd)
        {
            if (toAdd is GameObject)
                gameObjects.Add(currentGameObjectId++, toAdd as GameObject);
            if (toAdd is WorldObject) worldObjects.Add(currentGameObjectId - 1);
            //if (toAdd is Enemy) enemies.Add(currentGameObjectId - 1);
            if (toAdd is PhysicsObject) physicsObjects.Add(currentGameObjectId - 1);
        }

        public void LoadObject(object toLoad)
        {
            if (toLoad is IContent) GameElements.LoadContentsOf(toLoad as IContent);
        }

        public void Update(GameWindow window, GameTime gameTime)
        {
            foreach (var o in gameObjects.Values)
            {
                o.Update(window, gameTime);
            }
            DoCollisions();
        }

        public void DoCollisions() { 

            foreach(int id in physicsObjects.AsEnumerable())
            {
                gameObjects.
            }

        
        }

    }

    public class SortedVector<T> where T: IComparable<T>
    {
        List<T> list; 
        

        public SortedVector(){
            list = new List<T>();
        }



        
        public T this[int index] {
            return list[index];
        }


        public void Add(T obj)
        {
            int index = Find(obj);
            if (index >= 0)
                throw new ArgumentException("Duplicate item");
            int insertAt = ~index;

            list.Insert(insertAt, obj);
        }

        public int Find(T obj)
        {
            return list.BinarySearch(obj);
        }






    }


    public class DebugText : ScreenText
    {

        Stack<string> buffer;
        private int bufferSize;


        public DebugText(Vector2 position, int bufferSize) : base(position)
        {
            this.bufferSize = bufferSize;
            buffer = new Stack<string>(bufferSize);
        }

        public void Log(string message)
        {
            buffer.Push(message);
            while (buffer.Count > bufferSize)
                buffer.Pop();
        }

        public override void Draw(SpriteBatch sb)
        {
            Text = string.Join('\n', buffer);
            base.Draw(sb);
        }
    }

}
