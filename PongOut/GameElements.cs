using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace PongOut
{

    public class FlashedMessage
    {
        public static readonly Color DEFAULT_COLOR = Color.White;
        const float DEFAULT_TARGET_TIME_ALIVE = 2000;

        public string Text { get; private set; }

        float timeAlive = 0;
        float targetTimeAlive;

        public FlashedMessage(string text, float time = DEFAULT_TARGET_TIME_ALIVE) { 
            Text = text;
            targetTimeAlive = time;
        }

        public bool Kill { get; private set; } = false;

        public void Update(GameTime gt)
        {
            timeAlive += gt.ElapsedGameTime.Milliseconds;
            if (timeAlive >= targetTimeAlive)
                Kill = true;
        }

        public override string ToString()
        {
            return Text; 
        }

    }

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
        static ScreenText flashedText;

        static List<FlashedMessage> flashedMessages;

        public static void FlashMessage(FlashedMessage msg)
        {
            flashedMessages.Add(msg);
        }


        public static void UpdateOverlay(GameTime gt)
        {
            for(int i = flashedMessages.Count - 1; i >= 0; i--)
            {
                flashedMessages[i].Update(gt);
                if (flashedMessages[i].Kill)
                    flashedMessages.RemoveAt(i);
            }

            flashedText.Text = String.Join("\n", flashedMessages);
        }

        public static void DrawOverlay(SpriteBatch sb) {
            flashedText.Draw(sb);
            
            if (DebugMode)
                debugText.Draw(sb);
        }

        public static void WriteDebugLine(string text)
        {
            if (!GameElements.DebugMode)
                return;

            debugText.Log(text);
        }
        public static void Initialize()
        {
            if (DebugMode)
            {
                debugText = new DebugText(Vector2.One, 10);
                LoadContentsOf(debugText);
            }

            highScore = new HighScore();
            highScore.LoadFromFile();

            flashedMessages = new List<FlashedMessage>();
            flashedText = new ScreenText(new Vector2(300, 50));
            LoadContentsOf(flashedText);

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
            highScore.ListDraw(sb);
        }

        public static State HighScoreEnterNameUpdate(GameTime gt)
        {
            KeyboardState kbs = Keyboard.GetState();

            if (kbs.IsKeyDown(Keys.Escape))
                return GameElements.State.MainMenu;

            bool done = highScore.EnterNameUpdate(gt);
            if (done) return State.Highscore; 
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
