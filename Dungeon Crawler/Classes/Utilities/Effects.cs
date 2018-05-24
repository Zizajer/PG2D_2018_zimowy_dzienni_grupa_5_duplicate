using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Dungeon_Crawler
{
    public class Effects
    {
        public Effect hitEnemyEffect;
        public Effect hitPlayerEffect;
        public Effects(ContentManager Content)
        {
            hitEnemyEffect = Content.Load<Effect>("shaders/Effec1");
            hitPlayerEffect = Content.Load<Effect>("shaders/Effec2");
        }
    }
}