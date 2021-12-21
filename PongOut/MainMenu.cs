using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.IO;

namespace PongOut
{
    public class MainMenu : Menu<GameElements.State>, IContent
    {
        public static new readonly string CONTENT_PATH = Path.Combine(Menu<GameElements.State>.CONTENT_PATH, "mainMenu");
        public static readonly string TEXTURES_PATH = Path.Combine(CONTENT_PATH, "textures");

        public static readonly string START_TEXTURE_PATH = Path.Combine(TEXTURES_PATH, "start");
        public static readonly string HIGHSCORE_TEXTURE_PATH = Path.Combine(TEXTURES_PATH, "highscore");
        public static readonly string EXIT_TEXTURE_PATH = Path.Combine(TEXTURES_PATH, "exit");


        public MainMenu() : base(0)
        { }

        public void LoadContent(ContentManager cm)
        {
            Texture2D startTexture = cm.Load<Texture2D>(START_TEXTURE_PATH);
            Texture2D exitTexture = cm.Load<Texture2D>(EXIT_TEXTURE_PATH);
            Texture2D highscoreTexture = cm.Load<Texture2D>(HIGHSCORE_TEXTURE_PATH);

            AddItem(startTexture, GameElements.State.Run);
            AddItem(highscoreTexture, GameElements.State.Highscore);
            AddItem(exitTexture, GameElements.State.Quit);
        }
    }
}
