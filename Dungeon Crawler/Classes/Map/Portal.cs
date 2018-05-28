using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dungeon_Crawler
{
    public class Portal:Sprite
    {
        float BloomSatPulse = 1f, bloomSatDir = 0.09f;
        public Portal(Vector2 pos, Texture2D tex) : base(pos, tex)
        {
        }
        public Portal(Texture2D tex) : base(tex)
        {
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            Global.Effects.setBlurCalculations(Texture.Width, Texture.Height);
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, Global.Camera.TranslationMatrix);
            Global.Effects.bloomCombineEffect.CurrentTechnique.Passes[0].Apply();
            spriteBatch.Draw(Texture, Position, null, Color.White, 0.0f, Vector2.One, Size, SpriteEffects.None, Layers.Items);
            spriteBatch.End();
        }
        public void Update()
        {
            BloomSatPulse += bloomSatDir;
            if (BloomSatPulse > 2.5f) bloomSatDir = -0.09f;
            if (BloomSatPulse < 0.1f) bloomSatDir = 0.09f;
            Global.Effects.setPulse(BloomSatPulse);
        }
    }
}
