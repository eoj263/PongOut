using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace PongOut
{
    public class World
    {
        public bool GameOver { get; private set; }

        private SortedSet<GameObject> gameObjects;
        private List<WorldObject> worldObjects;

        public Paddle paddle;

        public void Draw(SpriteBatch sb)
        {
            worldObjects.ForEach((o) => o.Draw(sb));
        }

        public void LoadInitialState(GameWindow window)
        {
            gameObjects = new SortedSet<GameObject>();
            worldObjects = new List<WorldObject>();


            paddle = new Paddle(new Vector2(20, window.ClientBounds.Y / 2));
            LoadAndAddObject(paddle);
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
}
