﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PongOut
{
    public class World
    {
        public bool GameOver { get; private set; }

        // Each gameObject will have an id.
        private Dictionary<int, GameObject> gameObjects;

        private SortedVector<int> worldObjects;
        private SortedVector<int> physicsObjects;


        private List<UIComponent> UIComponents;


        private int currentGameObjectId = 0;

        private Player player;


        public void Draw(SpriteBatch sb)
        {
            for(int i = 0; i < worldObjects.Length; i++)
            {
                int id = worldObjects[i];
                (gameObjects[id] as WorldObject).Draw(sb);
            }
        }

        public void LoadInitialState(GameWindow window)
        {
            gameObjects = new Dictionary<int, GameObject>();
            worldObjects = new SortedVector<int>();
            physicsObjects = new SortedVector<int>();

            UIComponents = new List<UIComponent>();

            player = new Player(new Vector2(200, 200));
            LoadAndAddObject(player);

            Zombie zombie1 = new Zombie(new Vector2(300, 300));
            LoadAndAddObject(zombie1);
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
        
        public SortedVector(){
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
            if (index >= 0)
                throw new ArgumentException("Duplicate item");
            int insertAt = ~index;

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
