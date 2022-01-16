using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.IO;

namespace PongOut
{
    public class ScreenText : UIComponent, IContent
    {
        public static readonly string FONT_CONTENT = "fonts";
        public static readonly string DEFAULT_FONT_NAME = "default";

        private ContentManager cm;

        private string fontPath;

        SpriteFont font;

        private Vector2 position;

        public ScreenText(Vector2 position, string fontName = default) {
            this.position = position;

            if (fontName == null) {
                fontName = DEFAULT_FONT_NAME;
                SetFont(fontName);
            }
        }

        void SetFont(string fontName)
        {
            fontPath = Path.Combine(FONT_CONTENT, fontName);
            TryLoadFont();
        }

        /// <summary>
        /// Loads the font if the ContentManager is available
        /// </summary>
        /// <returns></returns>
        bool TryLoadFont() {
            if(cm != null)
            {
                font = cm.Load<SpriteFont>(fontPath);
                return true;
            }
            return false;
        }


        public override void Draw(SpriteBatch sb)
        {
            sb.DrawString(font, Text, position, Color.White);
        }

        public string Text { get; set; } = "";


        public void LoadContent(ContentManager cm)
        {
            this.cm = cm;
            TryLoadFont();
        }
    }


}
