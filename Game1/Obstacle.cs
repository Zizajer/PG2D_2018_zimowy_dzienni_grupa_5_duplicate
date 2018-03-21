using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Game1
{
    public class Obstacle
    {
        public Texture2D Texture { get; set; }
        public Vector2 Position { get; set; }
        public Obstacle(Vector2 pos,Texture2D tex)
        {
            Position = pos;
            Texture = tex;
        }
    }
}
