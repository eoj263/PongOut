using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.IO;

namespace PongOut
{
    /// <summary>
    /// Heals a player
    /// </summary>
    public class HealthPack : PlayerCollectable, IContent
    {
        static readonly string TEXTURE_PATH = Path.Join(Collectable.CONTENT_PATH, "healthPack");

        const float DFAULT_HEAL_AMMOUNT = 10;


        static Texture2D defaultTexture;

        float healAmmount;
        public HealthPack(Vector2 position, float healAmmount = DFAULT_HEAL_AMMOUNT) : base(position)
        {
            this.healAmmount = healAmmount;
        }

        protected override bool OnCollected(Player p)
        {
            float ammountHealed = p.Heal(healAmmount);
            return ammountHealed > 0;
        }

        public void LoadContent(ContentManager cm)
        {
            if (defaultTexture == null)
                defaultTexture = cm.Load<Texture2D>(TEXTURE_PATH);
            Texture = defaultTexture;
        }
    }
}
