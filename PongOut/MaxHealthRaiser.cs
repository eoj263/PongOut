using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.IO;

namespace PongOut
{
    /// <summary>
    /// Increases the max health of a player
    /// </summary>
    public class MaxHealthRaiser : PlayerCollectable, IContent
    {
        static readonly string TEXTURE_PATH = Path.Join(Collectable.CONTENT_PATH, "maxHealthRaiser");
        static Texture2D defaultTexture;


        const float DEFAULT_AMMOUNT = 5;

        float ammount;
        public MaxHealthRaiser(Vector2 position, float ammount = DEFAULT_AMMOUNT) : base(position)
        {
            this.ammount = ammount;
        }

        protected override bool OnCollected(Player p)
        {
            p.IncreaseMaxHealth(ammount);
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
