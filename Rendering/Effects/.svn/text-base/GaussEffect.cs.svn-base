using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using PraktWS0708.Settings;
using Microsoft.Xna.Framework;

namespace PraktWS0708.Rendering.Effects
{
    public sealed class GaussEffect
    {
        public Texture2D ResolveTexture;
        private Effect effect;

        private EffectParameter texParam;

        public GaussEffect()
        {
            effect = RenderManager.Instance.PersistentContent.Load<Effect>(Settings.Configuration.EngineSettings.ShaderDirectory + "Gauss");
            effect.CurrentTechnique = effect.Techniques[0];

            texParam = effect.Parameters["tex"];
        }

        public void ApplyGauss(float amount)
        {
            GraphicsDevice d = RenderManager.Instance.GraphicsDevice;
            d.ResolveBackBuffer(ResolveTexture);
            SetBlurEffectParameters(1.0f / (float)ResolveTexture.Width, 0, amount);
            RenderManager.Instance.SpriteBatch.Begin(SpriteBlendMode.None, SpriteSortMode.Immediate, SaveStateMode.None);

            effect.Begin();
            effect.CurrentTechnique.Passes[0].Begin();

            // Draw the quad.
            RenderManager.Instance.SpriteBatch.Draw(ResolveTexture, Vector2.Zero, Color.White);
            RenderManager.Instance.SpriteBatch.End();

            effect.CurrentTechnique.Passes[0].End();
            effect.End();

            d.ResolveBackBuffer(ResolveTexture);
            SetBlurEffectParameters(0, 1.0f / (float)ResolveTexture.Height, amount);
            RenderManager.Instance.SpriteBatch.Begin(SpriteBlendMode.None, SpriteSortMode.Immediate, SaveStateMode.None);

            effect.Begin();
            effect.CurrentTechnique.Passes[0].Begin();

            // Draw the quad.
            RenderManager.Instance.SpriteBatch.Draw(ResolveTexture, Vector2.Zero, Color.White);
            RenderManager.Instance.SpriteBatch.End();

            effect.CurrentTechnique.Passes[0].End();
            effect.End();
        }

        void SetBlurEffectParameters(float dx, float dy, float amount)
        {
            // Look up the sample weight and offset effect parameters.
            EffectParameter weightsParameter, offsetsParameter;

            weightsParameter = effect.Parameters["SampleWeights"];
            offsetsParameter = effect.Parameters["SampleOffsets"];

            // Look up how many samples our gaussian blur effect supports.
            int sampleCount = weightsParameter.Elements.Count;

            // Create temporary arrays for computing our filter settings.
            float[] sampleWeights = new float[sampleCount];
            Vector2[] sampleOffsets = new Vector2[sampleCount];

            // The first sample always has a zero offset.
            sampleWeights[0] = ComputeGaussian(0, amount);
            sampleOffsets[0] = new Vector2(0);

            // Maintain a sum of all the weighting values.
            float totalWeights = sampleWeights[0];

            // Add pairs of additional sample taps, positioned
            // along a line in both directions from the center.
            for (int i = 0; i < sampleCount / 2; i++)
            {
                // Store weights for the positive and negative taps.
                float weight = ComputeGaussian(i + 1, amount);

                sampleWeights[i * 2 + 1] = weight;
                sampleWeights[i * 2 + 2] = weight;

                totalWeights += weight * 2;

                // To get the maximum amount of blurring from a limited number of
                // pixel shader samples, we take advantage of the bilinear filtering
                // hardware inside the texture fetch unit. If we position our texture
                // coordinates exactly halfway between two texels, the filtering unit
                // will average them for us, giving two samples for the price of one.
                // This allows us to step in units of two texels per sample, rather
                // than just one at a time. The 1.5 offset kicks things off by
                // positioning us nicely in between two texels.
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

            // Tell the effect about our new filter settings.
            weightsParameter.SetValue(sampleWeights);
            offsetsParameter.SetValue(sampleOffsets);
        }

        float ComputeGaussian(float n, float theta)
        {
            return (float)((1.0 / Math.Sqrt(2 * Math.PI * theta)) *
                           Math.Exp(-(n * n) / (2 * theta * theta)));
        }
    }
}
