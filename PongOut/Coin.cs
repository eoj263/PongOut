using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.IO;

namespace PongOut
{

    public class Coin : PlayerCollectable, IContent
    {
        static readonly string TEXTURE_PATH = Path.Join(Collectable.CONTENT_PATH, "coin");
        static Texture2D defaultTexture;



        const int DEFAULT_SCORE_TO_GIVE = 4; 
        int scoreToGive;
        public Coin(Vector2 position, int scoreAdd = DEFAULT_SCORE_TO_GIVE) : base(position) {
            this.scoreToGive = scoreAdd;
        }

        protected override bool OnCollected(Player p)
        {
            p.Score += scoreToGive;
            return true;
        }

        public void LoadContent(ContentManager cm)
        {
            if (defaultTexture == null)
                defaultTexture = cm.Load<Texture2D>(TEXTURE_PATH);
            Texture = defaultTexture;
        }
    }
}
