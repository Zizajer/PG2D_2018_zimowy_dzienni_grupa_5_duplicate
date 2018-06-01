using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Dungeon_Crawler
{
    public class Effects
    {
        public Effect HitEffect;
        public Effect BurnEffect;
        public Effect FreezeEffect;
        public Effects(ContentManager Content)
        {
            HitEffect = Content.Load<Effect>("shaders/HitEffect");
            BurnEffect = Content.Load<Effect>("shaders/BurnEffect");
            FreezeEffect = Content.Load<Effect>("shaders/FreezeEffect");
        }
    }
}