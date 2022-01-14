using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;

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

    public interface IDrawable
    {
        void Draw(SpriteBatch sb);
    }

    public interface IUpdatable
    {
        void Update(GameWindow gw, GameTime gt);
    }

    public abstract class GameObject : IUpdatable, IComparable<GameObject>
    {
        public int UpdatePriority { get; set; } = 0;

        public bool IsAlive
        {
            get; protected set;
        } = true;

        public int CompareTo([AllowNull] GameObject other)
        {
            return UpdatePriority - other.UpdatePriority;
        }

        public abstract void Update(GameWindow gw, GameTime gt);
    }

    public abstract class WorldObject : GameObject, IDrawable
    {
        protected Texture2D Texture { get; set; }
        public Vector2 Position { get; protected set; } = Vector2.Zero;

        protected Color Color { get; set; } = Color.White;

        protected float Rotation { get; set; } = 0;

        protected Rectangle? SourceRectanlge { get; set; } = null;

        protected Vector2 Origin { get; set; } = Vector2.Zero;
        protected Vector2 Scale { get; set; } = Vector2.One;

        protected float LayerDepth = 0;


        public WorldObject(Vector2 position, Texture2D texture = default)
        {
            Texture = texture;
            Position = position;
        }


        /// <summary>
        /// Is called when this object should be drawn 
        /// </summary>
        /// <param name="sb"></param>
        public void Draw(SpriteBatch sb)
        {
            sb.Draw(Texture, Position, null, Color, Rotation, Origin, Scale, SpriteEffects.None, LayerDepth);
        }
    }


    //public abstract class Ball : MovingObject, IContent
    //{
    
    //    public Ball()
    //    {

    //    }
    //}

    public abstract class MovingObject : WorldObject
    {
        protected MovingObject(Vector2 position, Texture2D texture = null) : base(position, texture)
        {
        }


        protected Vector2 velocity = Vector2.Zero;

        public override void Update(GameWindow gw, GameTime gt)
        {
            Position += velocity;
        }

    }
}
