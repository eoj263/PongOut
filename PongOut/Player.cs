using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.IO;

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
        }

        Vector2 wantedMovementDirection;


        float speed = 20;
        public override void Update(GameWindow gw, GameTime gt)
        {
            velocity = wantedMovementDirection * speed;
            base.Update(gw, gt);
        }



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

            wantedMovementDirection = Vector2.Normalize(wanted);
        }
    }

}
