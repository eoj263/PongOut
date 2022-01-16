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

        private SortedSet<GameObject> gameObjects;
        private List<WorldObject> worldObjects;

        private List<UIComponent> UIComponents;


        private DebugText debugText;
        private Player player;


        public void Draw(SpriteBatch sb)
        {
            worldObjects.ForEach((o) => o.Draw(sb));
        }

        public void LoadInitialState(GameWindow window)
        {
            gameObjects = new SortedSet<GameObject>();
            worldObjects = new List<WorldObject>();
            UIComponents = new List<UIComponent>();


            if (GameElements.DebugMode)
            {
                debugText = new DebugText(new Vector2(200, 200), 9);
            }

            


            player = new Player(new Vector2(200, 200));
            LoadAndAddObject(player);
        }


        public void WriteDebugLine(string text) {
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

        public void AddObject(object toAdd)
        {
            if (toAdd is GameObject) gameObjects.Add(toAdd as GameObject);
            if (toAdd is WorldObject) worldObjects.Add(toAdd as WorldObject);
        }

        public void LoadObject(object toLoad)
        {
            if (toLoad is IContent) GameElements.LoadContentsOf(toLoad as IContent);
        }

        public void Update(GameWindow window, GameTime gameTime)
        {

            foreach(GameObject o in gameObjects)
            {
                o.Update(window, gameTime);
            }
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
            while(buffer.Count > bufferSize)
                buffer.Pop();
        }

        public override void Draw(SpriteBatch sb)
        {
            Text = string.Join('\n', buffer);
            base.Draw(sb);
        }
    }

}
