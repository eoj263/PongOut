using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace PongOut
{
    public static class GameElements
    {

        public static bool DebugMode { get; private set; } = true;

        public enum State { MainMenu, Run, Highscore, Quit };
        public static State CurrentState = State.MainMenu;
        static World world;

        public static void Initialize()
        {
        }

        public static void LoadWorld(ContentManager content, GameWindow window)
        {
            world = new World();
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
}
