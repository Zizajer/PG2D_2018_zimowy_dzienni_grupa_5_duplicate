using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Dungeon_Crawler
{
    public class Effects
    {
        public Effect hitEnemyEffect;
        public Effect hitPlayerEffect;
        public Effect bloomCombineEffect;

        public Effects(ContentManager Content)
        {
            hitEnemyEffect = Content.Load<Effect>("shaders/Effec1");
            hitPlayerEffect = Content.Load<Effect>("shaders/Effec2");
            bloomCombineEffect = Content.Load<Effect>("shaders/BloomCombine");
        }

        public void setBlurCalculations(int width, int height)
        {
            // Pass 2: draw from rendertarget 1 into rendertarget 2, using a shader to apply a horizontal gaussian blur filter.
            // Pass 3: draw from rendertarget 2 back into rendertarget 1, using a shader to apply a vertical gaussian blur filter.
            SetBlurEffectParameters(1.0f / (float)width, 1.0f / (float)height);
        }

        // SET BLUR EFFECT PARAMETERS
        void SetBlurEffectParameters(float dx, float dy)
        {
            // Look up the sample weight and offset effect parameters.
            EffectParameter weightsParameter, offsetsParameter;

            weightsParameter = bloomCombineEffect.Parameters["SampleWeights"];
            offsetsParameter = bloomCombineEffect.Parameters["SampleOffsets"];

            // Look up how many samples our gaussian blur effect supports.
            int sampleCount = weightsParameter.Elements.Count;

            // Create temporary arrays for computing our filter settings.
            float[] sampleWeights = new float[sampleCount];
            Vector2[] sampleOffsets = new Vector2[sampleCount];

            // The first sample always has a zero offset.
            sampleWeights[0] = ComputeGaussian(0);
            sampleOffsets[0] = new Vector2(0);

            // Maintain a sum of all the weighting values.
            float totalWeights = sampleWeights[0];

            // Add pairs of additional sample taps, positioned along a line in both directions from the center.
            for (int i = 0; i < sampleCount / 2; i++)
            {
                // Store weights for the positive and negative taps.
                float weight = ComputeGaussian(i + 1);

                sampleWeights[i * 2 + 1] = weight;
                sampleWeights[i * 2 + 2] = weight;

                totalWeights += weight * 2;

                // To get the maximum amount of blurring from a limited number of pixel shader samples, we take advantage of the bilinear filtering
                // hardware inside the texture fetch unit. If we position our texture coordinates exactly halfway between two texels, the filtering unit
                // will average them for us, giving two samples for the price of one. This allows us to step in units of two texels per sample, rather
                // than just one at a time. The 1.5 offset kicks things off by positioning us nicely in between two texels.
                float sampleOffset = i * 2 + 1.5f;

                Vector2 delta = new Vector2(dx, dy) * sampleOffset;

                // Store texture coordinate offsets for the positive and negative taps.
                sampleOffsets[i * 2 + 1] = delta;
                sampleOffsets[i * 2 + 2] = -delta;
            }

            // Normalize the list of sample weightings, so they will always sum to one.
            for (int i = 0; i < sampleWeights.Length; i++)
            {
                sampleWeights[i] /= totalWeights;
            }

            weightsParameter.SetValue(sampleWeights);  // Tell the effect about our new filter settings.
            offsetsParameter.SetValue(sampleOffsets);
        }

        public void setPulse(float bloomSatPulse)
        {
            bloomCombineEffect.Parameters["BloomSaturation"].SetValue(bloomSatPulse);
        }


        // C O M P U T E   G A U S S I A N 
        float ComputeGaussian(float n)
        {
            float theta = 2f;
            return (float)((1.0 / Math.Sqrt(2 * Math.PI * theta)) *
                            Math.Exp(-(n * n) / (2 * theta * theta)));
        }
    }
}