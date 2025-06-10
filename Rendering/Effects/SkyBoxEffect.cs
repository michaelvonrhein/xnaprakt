using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using PraktWS0708.Entities;
using Microsoft.Xna.Framework;

namespace PraktWS0708.Rendering.Effects
{
    sealed class SkyBoxEffect
    {
        private Effect effect;
        private EffectParameter matrix;

        public SkyBoxEffect()
        {
            effect = World.Instance.WorldContent.Load<Effect>(Settings.Configuration.EngineSettings.ShaderDirectory + "SkyBox");
            effect.CurrentTechnique = effect.Techniques[0];
            matrix = effect.Parameters["matWorldViewProjection"];
            effect.Parameters["skyCubeTex"].SetValue(World.Instance.WorldContent.Load<Texture2D>(Settings.Configuration.EngineSettings.TextureDirectory + "Environment\\SkyBox"));
        }

        public void Begin()
        {
            Camera c = World.Instance.Camera;
            matrix.SetValue(Matrix.CreateLookAt(Vector3.Zero, c.LookAt - c.Position, c.Up) * c.Projection);
            effect.Begin();
            effect.CurrentTechnique.Passes[0].Begin();
        }

        public void End()
        {
            effect.CurrentTechnique.Passes[0].End();
            effect.End();
        }
    }
}
