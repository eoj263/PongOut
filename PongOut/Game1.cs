using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;

namespace PongOut
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            GameElements.LoadContent(Content, Window);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();


            GameElements.State nextState = GameElements.CurrentState;
            switch (GameElements.CurrentState)
            {
                case GameElements.State.Run:
                    nextState = GameElements.RunUpdate(Content, Window, gameTime);
                    break;
                case GameElements.State.Highscore:
                    nextState = GameElements.HighScoreUpdate(gameTime);
                    break;
                case GameElements.State.MainMenu:
                    nextState = GameElements.MainMenuUpdate(gameTime);
                    break;
                case GameElements.State.Quit:
                    Exit();
                    break;
            }

            GameElements.SetState(nextState);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            _spriteBatch.Begin();
            switch (GameElements.CurrentState)
            {
                case GameElements.State.Run:
                    GameElements.RunDraw(_spriteBatch);
                    break;
                case GameElements.State.Highscore:
                    GameElements.HighScoreDraw(_spriteBatch);
                    break;
                case GameElements.State.MainMenu:
                    GameElements.MainMenuDraw(_spriteBatch);
                    break;
                case GameElements.State.Quit:
                    Exit();
                    break;
            }
            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }

    public static class GameElements
    {
        public enum State { MainMenu, Run, Highscore, Quit }; 
        public static State CurrentState = State.MainMenu;
        static World world;

        public static void Initialize()
        {
        }

        public static void LoadWorld(ContentManager content, GameWindow window)
        {
            world = new World();
            LoadContentsOf(world);
            world.LoadInitialState(window);
        }

        private static ContentManager content;
        public static void LoadContent(ContentManager content, GameWindow window)
        {
            GameElements.content = content;
            LoadMenu(content);
            LoadWorld(content, window);
        }

        public static T LoadContentsOf<T>(T thing) where T : IContent
        {
            thing.LoadContent(content);
            return thing;
        }

        static MainMenu mainMenu;

        private static void LoadMenu(ContentManager content)
        {
            mainMenu = new MainMenu();
            LoadContentsOf(mainMenu);
        }


        public static void RunDraw(SpriteBatch sb)
        {
            world.Draw(sb);
        }


        public static State RunUpdate(ContentManager content, GameWindow window, GameTime gameTime)
        {
            world.Update(window, gameTime);

            if (world.GameOver)
                return State.MainMenu;
            return State.Run;
        }

        public static State MainMenuUpdate(GameTime gameTime)
        {
            return mainMenu.Update(gameTime);
        }

        public static void MainMenuDraw(SpriteBatch sb)
        {
            mainMenu.Draw(sb);
        }

        public static State HighScoreUpdate(GameTime gt)
        {
            KeyboardState kbs = Keyboard.GetState();

            if (kbs.IsKeyDown(Keys.Escape))
                return State.MainMenu;

            return State.Highscore;
        }

        public static void HighScoreDraw(SpriteBatch sb)
        {
        }

        public static void SetState(State nextState)
        {
            CurrentState = nextState;
        }
    }

    public class MainMenu : Menu<GameElements.State>, IContent
    {
        public static new readonly string CONTENT_PATH = Path.Combine(Menu<GameElements.State>.CONTENT_PATH, "mainMenu");
        public static readonly string TEXTURES_PATH = Path.Combine(CONTENT_PATH, "textures");

        public static readonly string START_TEXTURE_PATH = Path.Combine(TEXTURES_PATH, "start");
        public static readonly string HIGHSCORE_TEXTURE_PATH = Path.Combine(TEXTURES_PATH, "highscore");
        public static readonly string EXIT_TEXTURE_PATH = Path.Combine(TEXTURES_PATH, "exit");


        public MainMenu() : base(0)
        { }

        public void LoadContent(ContentManager cm)
        {
            Texture2D startTexture = cm.Load<Texture2D>(START_TEXTURE_PATH);
            Texture2D exitTexture = cm.Load<Texture2D>(EXIT_TEXTURE_PATH);
            Texture2D highscoreTexture = cm.Load<Texture2D>(HIGHSCORE_TEXTURE_PATH);

            AddItem(startTexture, GameElements.State.Run);
            AddItem(highscoreTexture, GameElements.State.Highscore);
            AddItem(exitTexture, GameElements.State.Quit);
        }
    }

    // Since different menues have different choices, the abstract Menu class is generic. This also allows for different menu choices to include data using classes.

    public abstract class Menu<T> : Drawable
    {

        public readonly static string CONTENT_PATH = "menues";

        List<MenuItem<T>> menu;
        int selected = 0;

        float currentHeight = 0;

        double lastChange = 0;
        T defaultMenuState;

        protected Menu(T defaultMenuState)
        {
            menu = new List<MenuItem<T>>();
            this.defaultMenuState = defaultMenuState;
        }

        protected void AddItem(Texture2D itemTexture, T state)
        {
            float X = 100;
            float Y = 100 + currentHeight;

            currentHeight += itemTexture.Height + 20;

            MenuItem<T> created = new MenuItem<T>(itemTexture, new Vector2(X, Y), state);
            menu.Add(created);
        }

        public T Update(GameTime gameTime)
        {
            KeyboardState kbs = Keyboard.GetState();

            if (gameTime.TotalGameTime.TotalMilliseconds - lastChange > 130)
            {
                if (kbs.IsKeyDown(Keys.Down))
                {
                    selected++;
                    if (menu.Count <= selected)
                        selected = 0;
                }
                else if (kbs.IsKeyDown(Keys.Up))
                {
                    selected--;
                    if (selected < 0)
                        selected = menu.Count - 1;
                }

                lastChange = gameTime.TotalGameTime.TotalMilliseconds;
            }

            if (kbs.IsKeyDown(Keys.Enter))
                return menu[selected].State;
            return defaultMenuState;
        }

        public void Draw(SpriteBatch sb)
        {
            for (int i = 0; i < menu.Count; i++)
            {
                Color drawColor = i == selected ? Color.Red : Color.White;
                sb.Draw(menu[i].Texture, menu[i].Position, drawColor);
            }
        }
    }

    class MenuItem<T>
    {
        Texture2D texture;
        Vector2 position;
        T currentState;

        public MenuItem(Texture2D texture, Vector2 position, T currentState)
        {
            this.texture = texture;
            this.position = position;
            this.currentState = currentState;
        }

        public Texture2D Texture { get => texture; }
        public Vector2 Position { get => position; }
        public T State { get => currentState; }
    }


    public class Paddle : WorldObject 
    {
        public static readonly string CONTENT_PATH = "paddle";
        public static readonly string TEXTURES_PATH = Path.Combine(CONTENT_PATH, "textures");

        public static readonly string DEFAULT_TEXTURE_PATH = Path.Combine(TEXTURES_PATH, "defaultTexture");

        public void LoadContent(ContentManager cm)
        {
        }
    }

    public class World : IContent
    {
        public bool GameOver { get; private set; }

        public void Draw(SpriteBatch sb)
        {
        }

        public void LoadInitialState(GameWindow window)
        {
        }

        public void LoadContent(ContentManager content)
        {
        }

        public void Update(GameWindow window, GameTime gameTime)
        {
        }
    }

    public interface IContent
    {
        void LoadContent(ContentManager cm);
    }

    public interface Drawable
    {
        void Draw(SpriteBatch sb);
    }

    public interface Updatable
    {
        void Update(GameWindow gw, GameTime gt);
    }

    public abstract class GameObject
    {
        public bool IsAlive
        {
            get; protected set;
        } = true;
    }

    public abstract class WorldObject : GameObject, Drawable, Updatable
    {
        public void Draw(SpriteBatch sb)
        {
        }

        public void Update(GameWindow gw, GameTime gt)
        {
        }
    }

    public abstract class MovingObject
    {
    }
}
