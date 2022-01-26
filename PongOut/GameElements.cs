using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace PongOut
{
    public static class GameElements
    {

        public static bool DebugMode { get; private set; } = true;

        public static HighScore highScore;
        public enum State { MainMenu, Run, Highscore, EnterHighScoreName, Quit };
        public static State CurrentState = State.MainMenu;
        public static State PreviousState;

        public static bool StateChanged => CurrentState != PreviousState;

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
            highScore = new HighScore();
            if (DebugMode)
            {
                debugText = new DebugText(Vector2.One, 10);
                LoadContentsOf(debugText);
            }
        }

        public static void ResetWorld(ContentManager content, GameWindow window)
        {
            lastScore = 0;
            World = new World();
            World.LoadInitialState(window);
        }

        private static ContentManager content;
        public static void LoadContent(ContentManager content, GameWindow window)
        {
            GameElements.content = content;
            LoadMenu(content);
            ResetWorld(content, window);
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

        public static int lastScore { get; private set; }
        
        public static State RunUpdate(ContentManager content, GameWindow window, GameTime gameTime)
        {
            World.Update(window, gameTime);
            lastScore = World.CurrentScore;

            if (World.GameOver)
                return State.EnterHighScoreName;
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
            highScore.ListDraw(sb);
        }

        public static State HighScoreEnterNameUpdate(GameTime gt)
        {
            bool done = highScore.EnterNameUpdate(gt);
            if (done)
                return State.MainMenu; // TODO CHANGE TO HIGHSCORE
            return State.EnterHighScoreName;
        }
        public static void HighScoreEnterNameDraw(SpriteBatch sb)
        {
            highScore.EnterNameDraw(sb);
        }


        public static void SetState(State nextState)
        {
            PreviousState = CurrentState;
            CurrentState = nextState;
        }
    }
}
