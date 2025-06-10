using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using PraktWS0708.Entities;
using Microsoft.Xna.Framework;
using PraktWS0708.Settings;

namespace PraktWS0708.Rendering
{
    public sealed class OldCameraEffect
    {
        public Texture2D ResolveTarget;

        private Effect effect;
        private EffectParameter scene;

        private EffectParameter scratchPosParameter;
        private EffectParameter scratchWidthParameter;

        private EffectParameter noisePosParameter;

        private Random random;

        float[] scratchPos = new float[4];
        float[] scratchWidth = new float[4];
        float[] scratchSpeed = new float[4];

        private static float changeSpeedPropability = 0.1f;
        private static float maxSpeed = 0.1f;
        
        private static float jumpPropability = 0.01f;
        private static float maxWidth = 0.004f;

        public OldCameraEffect()
        {
            effect = RenderManager.Instance.PersistentContent.Load<Effect>(Settings.Configuration.EngineSettings.ShaderDirectory + "OldCamera");
            effect.CurrentTechnique = effect.Techniques[0];
            scene = effect.Parameters["scene"];

            effect.Parameters["noise"].SetValue(RenderManager.Instance.PersistentContent.Load<Texture2D>(Settings.Configuration.EngineSettings.TextureDirectory + "Noise"));

            scratchPosParameter = effect.Parameters["scratchPos"];
            scratchWidthParameter = effect.Parameters["scratchWidth"];
            noisePosParameter = effect.Parameters["noisePos"];

            random = new Random((int)DateTime.Now.Ticks);

            for (int i = 0; i < 4; i++)
            {
                scratchPos[i] = MathHelper.Lerp(-0.5f, 1.5f, (float)random.NextDouble());
                scratchSpeed[i] = MathHelper.Lerp(-maxSpeed, maxSpeed, (float)random.NextDouble());
                scratchWidth[i] = MathHelper.Lerp(0f, maxWidth, (float)random.NextDouble());
            }
        }

        public void Update(GameTime gameTime)
        {
            float timePassed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            noisePosParameter.SetValue(new Vector2((float)random.NextDouble() / 2f, (float)random.NextDouble() / 2f));

            for (int i = 0; i < 4; i++)
            {
                if ((float)random.NextDouble() <= changeSpeedPropability)
                {
                    scratchSpeed[i] = MathHelper.Lerp(-maxSpeed, maxSpeed, (float)random.NextDouble());
                }

                if ((float)random.NextDouble() <= jumpPropability)
                {
                    scratchPos[i] = MathHelper.Lerp(-0.5f, 1.5f, (float)random.NextDouble());
                    scratchWidth[i] = MathHelper.Lerp(0f, maxWidth, (float)random.NextDouble());
                }

                scratchPos[i] += timePassed * scratchSpeed[i];
            }
        }

        public void Apply()
        {
            SpriteBatch b = RenderManager.Instance.SpriteBatch;
            GraphicsDevice d = RenderManager.Instance.GraphicsDevice;

            // Save current scene for effect pass
            d.ResolveBackBuffer(ResolveTarget);

            d.Clear (ClearOptions.DepthBuffer | ClearOptions.Target, Color.Black, 0f, 0);

            scene.SetValue(ResolveTarget);

            scratchPosParameter.SetValue(scratchPos);
            scratchWidthParameter.SetValue(scratchWidth);


            b.Begin(SpriteBlendMode.None, SpriteSortMode.Immediate, SaveStateMode.None);
            effect.Begin();
            effect.CurrentTechnique.Passes[0].Begin();
            b.Draw(ResolveTarget, Vector2.Zero, Color.White);
            b.End();
            effect.CurrentTechnique.Passes[0].End();
            effect.End();
        }
    }
}
