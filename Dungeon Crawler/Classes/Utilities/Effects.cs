using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Dungeon_Crawler
{
    public class Effects
    {
        public Effect hitEffect;
        public Effects(ContentManager Content)
        {
            hitEffect = Content.Load<Effect>("shaders/Effec1");
        }
    }
}