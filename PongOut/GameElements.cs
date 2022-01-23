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
        public static World World { get; private set; }

        static DebugText debugText;

        public static void DrawOverlay(SpriteBatch sb) {
            if (DebugMode)
                debugText.Draw(sb);
        }

        public static void WriteDebugLine(string text)
        {
            if (!GameElements.DebugMode)
            {
                return;
            }

            debugText.Log(text);
        }
        public static void Initialize()
        {
            debugText = new DebugText(Vector2.One, 10);
            LoadContentsOf(debugText);
        }

        public static void LoadWorld(ContentManager content, GameWindow window)
        {
            World = new World();
            World.LoadInitialState(window);
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
            World.Draw(sb);
            DrawOverlay(sb);
        }


        public static State RunUpdate(ContentManager content, GameWindow window, GameTime gameTime)
        {
            World.Update(window, gameTime);

            if (World.GameOver)
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
            DrawOverlay(sb);
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
            DrawOverlay(sb);
        }

        public static void SetState(State nextState)
        {
            CurrentState = nextState;
        }
    }
}
