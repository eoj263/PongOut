using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections;

namespace PongOut
{
    public class Game1 : Game
    {
        // Textures come from https://www.kenney.nl/assets/topdown-shooter
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
            GameElements.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            GameElements.LoadContent(Content, Window);
        }

        protected override void Update(GameTime gameTime)
        {
            GameElements.State nextState = GameElements.CurrentState;

            switch (GameElements.CurrentState)
            {
                case GameElements.State.Run:
                    if (GameElements.StateChanged) 
                        GameElements.ResetWorld(Content, Window);

                    nextState = GameElements.RunUpdate(Content, Window, gameTime);
                    break;

                case GameElements.State.Highscore:
                    nextState = GameElements.HighScoreUpdate(gameTime);
                    break;

                case GameElements.State.EnterHighScoreName:
                    if (GameElements.StateChanged)
                        GameElements.highScore.InitializeEnterName();
                    nextState = GameElements.HighScoreEnterNameUpdate(gameTime);
                    break;

                case GameElements.State.MainMenu:
                    nextState = GameElements.MainMenuUpdate(gameTime);
                    break;

                case GameElements.State.HowToPlay:
                    nextState = GameElements.HowToPlayUpdate();
                    break;

                case GameElements.State.Quit:
                    Exit();
                    break;
            }

            GameElements.UpdateOverlay(gameTime);
            GameElements.SetState(nextState);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Gray);

            _spriteBatch.Begin(SpriteSortMode.FrontToBack);
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
                case GameElements.State.EnterHighScoreName:
                    GameElements.HighScoreEnterNameDraw(_spriteBatch);
                    break;
                case GameElements.State.HowToPlay:
                    GameElements.HowToPlayDraw(_spriteBatch);
                    break;
                case GameElements.State.Quit:
                    Exit();
                    break;
            }
            GameElements.DrawOverlay(_spriteBatch);

            _spriteBatch.End();
            base.Draw(gameTime);
        }

        protected override void UnloadContent()
        {
            GameElements.highScore.SaveToFile();
            base.UnloadContent();
        }
    }
}
