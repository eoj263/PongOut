using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.IO;
using System;

namespace PongOut
{
    public class Player : PhysicsObject, IContent
    {
        static readonly string CONTENT_PATH= "player";
        static readonly string DEFAULT_STAND = Path.Join(CONTENT_PATH, "defualt/stand"); 
        public Player(Vector2 position) : base(position, null)
        {

        }

        public void LoadContent(ContentManager cm)
        {
            Texture = cm.Load<Texture2D>(DEFAULT_STAND);
            CenterOrigin();
        }


        Vector2 wantedRelativeMovementDirection;

        float speed = 5;
        public override void Update(GameWindow gw, GameTime gt)
        {
            HandleInput();

            // TODO Custom math and rotate
            Rotation = MathF.Atan2(looking.Y, looking.X);

            velocity = Vector2.Transform(wantedRelativeMovementDirection * speed, Matrix.CreateRotationZ(Rotation));
            base.Update(gw, gt);
        }


        Vector2 looking = new Vector2(1,0);


        Keys moveUpKey = Keys.W;
        Keys moveDownKey = Keys.S;
        Keys moveLeftKey = Keys.A;
        Keys moveRightKey = Keys.D;
        public void HandleInput()
        {
            KeyboardState kbs = Keyboard.GetState();
            Vector2 wanted = Vector2.Zero;

            if (kbs.IsKeyDown(moveUpKey))
            {
                wanted.Y = -1;
            }

            if (kbs.IsKeyDown(moveDownKey))
            {
                wanted.Y = 1;
            }

            if (kbs.IsKeyDown(moveLeftKey))
            {
                wanted.X = -1;
            }

            if (kbs.IsKeyDown(moveRightKey))
            {
                wanted.X = 1;
            }


            if(wanted.X != 0 && wanted.Y != 0)
            {
                wanted.Normalize();
            }
            wantedRelativeMovementDirection = wanted;

            MouseState ms = Mouse.GetState();

            var diff = -Position + ms.Position.ToVector2();

            this.looking = Vector2.Normalize(diff);
        }
    }
}
