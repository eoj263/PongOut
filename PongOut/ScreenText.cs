using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.IO;

namespace PongOut
{
    public class ScreenText : UIComponent, IContent {
        public const string FONT_CONTENT = "fonts";
        public const string DEFAULT_FONT_NAME = "default";
        public const int DEFAULT_FONT_SCALE = 1;
        public static readonly Color DEFAULT_COLOR = Color.White;

        private ContentManager cm;

        private string fontPath;

        SpriteFont font;

        private Vector2 position;

        int Scale;

        public Color Color { get; set; }
        public ScreenText(Vector2 position, int fontScale = DEFAULT_FONT_SCALE, Color? color = null, string fontName = default) {
            if(!color.HasValue)
                color = DEFAULT_COLOR;

            Color = color.Value;

            this.Scale = fontScale;
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
            sb.DrawString(font, Text, position, Color, 0, Vector2.Zero, Scale, SpriteEffects.None, 1);
            //sb.DrawString(font, Text, position, Color.White);
        }

        public string Text { get; set; } = "";

        public void LoadContent(ContentManager cm)
        {
            this.cm = cm;
            TryLoadFont();
        }
    }


}
