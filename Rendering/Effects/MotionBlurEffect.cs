using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using PraktWS0708.Entities;
using Microsoft.Xna.Framework;
using PraktWS0708.Settings;

namespace PraktWS0708.Rendering
{
    public sealed class MotionBlurEffect
    {
        public Texture2D ResolveTarget;

        private Effect effect;

        private EffectParameter scene;
        private EffectParameter blurIntensity;
        private EffectParameter blurCenter;

        public MotionBlurEffect()
        {
            effect = RenderManager.Instance.PersistentContent.Load<Effect>(Settings.Configuration.EngineSettings.ShaderDirectory + "MotionBlur");
            effect.CurrentTechnique = effect.Techniques[0];
            scene = effect.Parameters["scene"];
            blurIntensity = effect.Parameters["blurIntensity"];
            blurCenter = effect.Parameters["blurCenter"];
        }

        public void ApplyMotionBlur()
        {
            SpriteBatch b = RenderManager.Instance.SpriteBatch;

            // Save current scene for blur pass
            RenderManager.Instance.GraphicsDevice.ResolveBackBuffer(ResolveTarget);

            float relativeSpeed = World.Instance.PlayersShip.PhysicsPlugin.Velocity / 0.0075f; // This is the max speed - better get this from physics...

            // This ensures that speed-based interpolations do not run wild
            relativeSpeed = MathHelper.Clamp(relativeSpeed, 0f, 1f);

            scene.SetValue(ResolveTarget);
            blurIntensity.SetValue(MathHelper.Lerp(0.0f, 0.5f, relativeSpeed));
            blurCenter.SetValue(new Vector2(0.5f, 0.5f));

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
