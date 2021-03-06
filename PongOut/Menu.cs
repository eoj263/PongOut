using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace PongOut
{
    ///<summary> 
    /// Since different menues have different choices, the Menu class is generic. This also allows for different menu choices to include data using classes.
    ///</summary>
    public class Menu<T> 
    {
        public readonly static string CONTENT_PATH = "menues";

        List<MenuItem<T>> menu;
        int selected = 0;

        /// <summary>
        /// Where to add the next element
        /// </summary>
        float currentHeight = 0;

        double lastChange = 0;
        T defaultMenuState;

        float timeoutDelay = 130;

        public Menu(T defaultMenuState)
        {
            menu = new List<MenuItem<T>>();
            this.defaultMenuState = defaultMenuState;
        }

        public void AddItem(Texture2D itemTexture, T state)
        {
            float X = 100;
            float Y = 100 + currentHeight;

            currentHeight += itemTexture.Height + 20;

            MenuItem<T> created = new MenuItem<T>(itemTexture, new Vector2(X, Y), state);
            menu.Add(created);
        }

        public virtual T Update(GameTime gameTime)
        {
            KeyboardState kbs = Keyboard.GetState();
            if (gameTime.TotalGameTime.TotalMilliseconds - lastChange >= timeoutDelay)
            {
                if (kbs.IsKeyDown(Keys.Down))
                {
                    selected++;
                    if (menu.Count <= selected)
                        selected = 0;
                }
                else if (kbs.IsKeyDown(Keys.Up))
                {
                    selected--;
                    if (selected < 0)
                        selected = menu.Count - 1;
                }

                lastChange = gameTime.TotalGameTime.TotalMilliseconds;
            }

            if (kbs.IsKeyDown(Keys.Enter))
                return menu[selected].State;
            return defaultMenuState;
        }

        public void Draw(SpriteBatch sb)
        {
            for (int i = 0; i < menu.Count; i++)
            {
                Color drawColor = i == selected ? Color.Red : Color.White;
                sb.Draw(menu[i].Texture, menu[i].Position, drawColor);
            }
        }
    }
}
