using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.IO;
using System;

namespace PongOut
{
    public class Paddle : WorldObject, IContent
    {
        public static readonly string CONTENT_PATH = "paddle";
        public static readonly string TEXTURES_PATH = Path.Combine(CONTENT_PATH, "textures");

        public static readonly string DEFAULT_TEXTURE_PATH = Path.Combine(TEXTURES_PATH, "flowerPaddle");

        static Texture2D defaultTexture;

        public Vector2 Normal { get; protected set; }


        public Paddle(Vector2 position) : base(position)
        {
        }

        public void LoadContent(ContentManager cm)
        {
            defaultTexture = cm.Load<Texture2D>(DEFAULT_TEXTURE_PATH);
            Texture = defaultTexture;
            Origin = new Vector2(Texture.Width / 2, Texture.Height / 2);
        }

        public override void Update(GameWindow gw, GameTime gt)
        {
            HandleUserInput();
            Position = requestedPosition;
            Rotation = requestedRotation;

            Normal = new Vector2(1, 0);
        }

        Vector2 requestedPosition;
        float requestedRotation;
        private void HandleUserInput()
        {
            MouseState mouseState = Mouse.GetState();

            KeyboardState keyboardState = Keyboard.GetState();


            if (keyboardState.IsKeyDown(Keys.LeftControl))
            {

                // 10 - 5 = 5 -> ta sig från 5  till 10
                // a - b = c -> ta sig från b till a


                requestedRotation = -MathF.Atan2(
                    mouseState.Position.X - Position.X,
                    mouseState.Position.Y - Position.Y 
                    ) + MathF.PI / 2;
            }
            else
            {
                requestedPosition = new Vector2(Position.X, mouseState.Position.Y);
                requestedRotation = 0;
            }

        }

    }
}
