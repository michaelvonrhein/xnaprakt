using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using PraktWS0708.Entities;
using Microsoft.Xna.Framework;

namespace PraktWS0708.Rendering.Effects
{
    class ExplosionEffect : ManagedEffect
    {
        protected EffectParameter worldViewParam;
        protected EffectParameter worldViewProjectionParam;
        //protected EffectParameter noiseTextureParam;
        protected EffectParameter colorTextureParam;
        protected EffectParameter centerParam;
        protected EffectParameter amplitudeParam;
        protected EffectParameter speedParam;
        protected EffectParameter timeParam;

        //protected Texture2D colorTexture;
        protected Texture2D noiseTexture;

        public ExplosionEffect()
            : base("ParticleExplosion")
        {
            worldViewParam = effect.Parameters["worldView"];
            worldViewProjectionParam = effect.Parameters["worldViewProjection"];

            //noiseTextureParam = effect.Parameters["NoiseMap"];
            colorTextureParam = effect.Parameters["ColorMap"];
            centerParam = effect.Parameters["center"];
            amplitudeParam = effect.Parameters["amplitude"];
            speedParam = effect.Parameters["speed"];
            timeParam = effect.Parameters["time"];

            //noiseTexture = World.Instance.WorldContent.Load<Texture2D>(Settings.Configuration.EngineSettings.TextureDirectory + "particleNoise");
            //colorTexture = World.Instance.WorldContent.Load<Texture2D>(Settings.Configuration.EngineSettings.TextureDirectory + "particleColor");

            //noiseTextureParam.SetValue(noiseTexture);
            //colorTextureParam.SetValue(colorTexture);
            amplitudeParam.SetValue(1.0f);
            speedParam.SetValue(1.0f);
        }

        float fTime = 0;
        string textureName = "baseMap";

        public override void Update(BaseMeshPart mesh)
        {
            Camera c = World.Instance.Camera;
            Matrix matWorldView = mesh.Entity.WorldMatrix * c.View;
            Matrix matWorldViewProj = matWorldView * c.Projection;
            worldViewParam.SetValue(matWorldView);
            worldViewProjectionParam.SetValue(matWorldViewProj);

            colorTextureParam.SetValue(mesh.TextureParameters[textureName]);

            fTime += (float)World.Instance.GameTime.ElapsedGameTime.Milliseconds / 10000f;

            while (fTime > 1f) fTime -= 1f;

            timeParam.SetValue(fTime);
        }
    }
}
