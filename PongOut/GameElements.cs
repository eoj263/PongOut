using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace PongOut
{

    public static class GameElements
    {
        public static bool DebugMode { get; private set; } = false;

        public static HighScore highScore;
        public enum State { MainMenu, Run, Highscore, EnterHighScoreName, Quit, HowToPlay };
        public static State CurrentState = State.MainMenu;
        public static State PreviousState;

        public static bool StateChanged => CurrentState != PreviousState;

        public static World World { get; private set; } 

        static FIFOScreenText debugText;
        static ScreenText flashedText;

        static List<FlashedMessage> flashedMessages;

        /// <summary>
        /// Flashes a message to the user. 
        /// </summary>
        /// <param name="msg"></param>
        public static void FlashMessage(FlashedMessage msg)
        {
            flashedMessages.Add(msg);
        }


        /// <summary>
        /// Update overlay ( e.g flashed messages & debug test )
        /// </summary>
        /// <param name="gt"></param>
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

        /// <summary>
        /// Writes a line that will persist untill the queue is full
        /// </summary>
        /// <param name="text"></param>
        public static void WriteDebugLine(string text)
        {
            if (!GameElements.DebugMode)
                return;

            debugText.Log(text);
        }



        private static ScreenText howToPlayText;



        public static void Initialize()
        {
            if (DebugMode)
            {
                debugText = new FIFOScreenText(Vector2.One, 10);
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

            howToPlayText = new ScreenText(new Vector2(50, 100));
            LoadContentsOf(howToPlayText);

            howToPlayText.Text = "Ditt mål är att få så många poäng som möjligt. \nDu får poäng av att döda fiender. Olika fiender ger olika många poäng. Förflytta dig genom W, A, S och D.\nFör att skjuta trycker du på W, A och D tillsammans sedan siktar du med musen och skjuter med vänsterclick\n\nTryck på ESC för att gå tillbaka";

            LoadMenues(content);
            ResetWorld(content, window);
        }

        /// <summary>
        /// Loads the content of an object that implements the IContent interface
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="thing"></param>
        /// <returns></returns>
        public static T LoadContentsOf<T>(T thing) where T : IContent
        {
            thing.LoadContent(content);
            return thing;
        }

        static MainMenu mainMenu;

        /// <summary>
        /// Loads all menues
        /// </summary>
        /// <param name="content"></param>
        private static void LoadMenues(ContentManager content)
        {
            mainMenu = new MainMenu();
            LoadContentsOf(mainMenu);
        }

        public static State HowToPlayUpdate()
        {
            KeyboardState kbs = Keyboard.GetState();
            if (kbs.IsKeyDown(Keys.Escape))
                return State.MainMenu;
            return State.HowToPlay;
        }

        public static void HowToPlayDraw(SpriteBatch sb)
        {
            howToPlayText.Draw(sb);
        }

        /// <summary>
        /// Runs the game
        /// </summary>
        /// <param name="sb"></param>
        public static void RunDraw(SpriteBatch sb)
        {
            World.Draw(sb);
        }


        public static int lastScore { get; private set; }
        
        /// <summary>
        /// Updates the game
        /// </summary>
        /// <param name="content"></param>
        /// <param name="window"></param>
        /// <param name="gameTime"></param>
        /// <returns></returns>
        public static State RunUpdate(ContentManager content, GameWindow window, GameTime gameTime)
        {
            World.Update(window, gameTime);
            lastScore = World.Score;

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

        /// <summary>
        /// Sets the state that the next frame should have
        /// </summary>
        /// <param name="nextState"></param>
        public static void SetState(State nextState)
        {
            PreviousState = CurrentState;
            CurrentState = nextState;
        }
    }
}
