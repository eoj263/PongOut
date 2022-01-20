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

            Window.AllowUserResizing = true;
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
}
