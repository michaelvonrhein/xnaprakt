using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using PraktWS0708.Settings;
using Microsoft.Xna.Framework;

namespace PraktWS0708.Rendering.Effects
{
    public sealed class BloomEffect
    {
        public Texture2D ResolveTarget;
        public RenderTarget2D[] RenderTargets = new RenderTarget2D[2];
        private Effect effect;
        private EffectTechnique extract;
        private EffectTechnique blur;
        private EffectTechnique combine;
        private EffectParameter texParam;
        private EffectParameter bloomParam;
        private EffectParameter baseParam;
        private EffectParameter bloomIntensity;
        private EffectParameter baseIntensity;
        private EffectParameter bloomSaturation;
        private EffectParameter baseSaturation;
        private EffectParameter bloomThreshold;


        public BloomEffect()
        {
            effect = RenderManager.Instance.PersistentContent.Load<Effect>(Settings.Configuration.EngineSettings.ShaderDirectory + "Bloom");
            extract = effect.Techniques["BloomExtract"];
            blur = effect.Techniques["GaussianBlur"];
            combine = effect.Techniques["BloomCombine"];
            texParam = effect.Parameters["tex"];
            bloomParam = effect.Parameters["bloomTex"];
            baseParam = effect.Parameters["baseTex"];

            bloomThreshold = effect.Parameters["BloomThreshold"];
            bloomIntensity = effect.Parameters["BloomIntensity"];
            baseIntensity = effect.Parameters["BaseIntensity"];
            bloomSaturation = effect.Parameters["BloomSaturation"];
            baseSaturation = effect.Parameters["BaseSaturation"];
        }

        /// <summary>
        /// This is where it all happens. Grabs a scene that has already been rendered,
        /// and uses postprocess magic to add a glowing bloom effect over the top of it.
        /// </summary>
        public void ApplyBloom()
        {
            GraphicsDevice d = RenderManager.Instance.GraphicsDevice;
            // Temporarily disable the depth stencil buffer.
            DepthStencilBuffer previousDepthStencil = d.DepthStencilBuffer;

            d.DepthStencilBuffer = null;

            // Resolve the scene into a texture, so we can
            // use it as input data for the bloom processing.
            d.ResolveBackBuffer(ResolveTarget);

            // Pass 1: draw the scene into rendertarget 1, using a
            // shader that extracts only the brightest parts of the image.
            bloomThreshold.SetValue(Configuration.EngineSettings.BloomThreshold);
            texParam.SetValue(ResolveTarget);
            DrawFullscreenQuad(ResolveTarget, RenderTargets[0], extract);

            // Pass 2: draw from rendertarget 1 into rendertarget 2,
            // using a shader to apply a horizontal gaussian blur filter.
            SetBlurEffectParameters(1.0f / (float)RenderTargets[0].Width, 0);
            texParam.SetValue(RenderTargets[0].GetTexture());
            DrawFullscreenQuad(RenderTargets[0].GetTexture(), RenderTargets[1], blur);

            // Pass 3: draw from rendertarget 2 back into rendertarget 1,
            // using a shader to apply a vertical gaussian blur filter.
            SetBlurEffectParameters(0, 1.0f / (float)RenderTargets[0].Height);
            texParam.SetValue(RenderTargets[1].GetTexture());
            DrawFullscreenQuad(RenderTargets[1].GetTexture(), RenderTargets[0], blur);

            // Pass 4: draw both rendertarget 1 and the original scene
            // image back into the main backbuffer, using a shader that
            // combines them to produce the final bloomed result.
            d.SetRenderTarget(0, null);

            bloomIntensity.SetValue(Configuration.EngineSettings.BloomIntensity);
            baseIntensity.SetValue(Configuration.EngineSettings.BaseIntensity);
            bloomSaturation.SetValue(Configuration.EngineSettings.BloomSaturation);
            baseSaturation.SetValue(Configuration.EngineSettings.BaseSaturation);

            Viewport viewport = d.Viewport;

            baseParam.SetValue(ResolveTarget);
            bloomParam.SetValue(RenderTargets[0].GetTexture());
            DrawFullscreenQuad(RenderTargets[0].GetTexture(), viewport.Width, viewport.Height, combine);

            // Restore the original depth stencil buffer.
            d.DepthStencilBuffer = previousDepthStencil;
        }

        /// <summary>
        /// Helper for drawing a texture into a rendertarget, using
        /// a custom shader to apply postprocessing effects.
        /// </summary>
        void DrawFullscreenQuad(Texture2D texture, RenderTarget2D renderTarget, EffectTechnique technique)
        {
            RenderManager.Instance.GraphicsDevice.SetRenderTarget(0, renderTarget);

            DrawFullscreenQuad(texture, renderTarget.Width, renderTarget.Height, technique);

            RenderManager.Instance.GraphicsDevice.ResolveRenderTarget(0);
        }


        /// <summary>
        /// Helper for drawing a texture into the current rendertarget,
        /// using a custom shader to apply postprocessing effects.
        /// </summary>
        void DrawFullscreenQuad(Texture2D texture, int width, int height, EffectTechnique technique)
        {
            RenderManager.Instance.SpriteBatch.Begin(SpriteBlendMode.None, SpriteSortMode.Immediate, SaveStateMode.None);

            effect.CurrentTechnique = technique;
            effect.Begin();
            technique.Passes[0].Begin();

            // Draw the quad.
            RenderManager.Instance.SpriteBatch.Draw(texture, new Rectangle(0, 0, width, height), Color.White);
            RenderManager.Instance.SpriteBatch.End();

            technique.Passes[0].End();
            effect.End();
        }

        /// <summary>
        /// Computes sample weightings and texture coordinate offsets
        /// for one pass of a separable gaussian blur filter.
        /// </summary>
        void SetBlurEffectParameters(float dx, float dy)
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
            sampleWeights[0] = ComputeGaussian(0);
            sampleOffsets[0] = new Vector2(0);

            // Maintain a sum of all the weighting values.
            float totalWeights = sampleWeights[0];

            // Add pairs of additional sample taps, positioned
            // along a line in both directions from the center.
            for (int i = 0; i < sampleCount / 2; i++)
            {
                // Store weights for the positive and negative taps.
                float weight = ComputeGaussian(i + 1);

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


        /// <summary>
        /// Evaluates a single point on the gaussian falloff curve.
        /// Used for setting up the blur filter weightings.
        /// </summary>
        float ComputeGaussian(float n)
        {
            float theta = Configuration.EngineSettings.BlurAmount;

            return (float)((1.0 / Math.Sqrt(2 * Math.PI * theta)) *
                           Math.Exp(-(n * n) / (2 * theta * theta)));
        }
    }
}
