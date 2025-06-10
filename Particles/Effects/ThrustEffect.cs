using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using PraktWS0708.Entities;
using Microsoft.Xna.Framework;

namespace PraktWS0708.Rendering.Effects
{
    class ThrustEffect : ManagedEffect
    {
        protected EffectParameter worldViewParam;
        protected EffectParameter worldViewProjectionParam;
        //protected EffectParameter noiseTextureParam;
        protected EffectParameter baseMapParam;
        protected EffectParameter centerParam;
        protected EffectParameter amplitudeParam;
        protected EffectParameter speedParam;
        protected EffectParameter timeParam;
        protected EffectParameter gasParam;
        protected EffectParameter turboParam;
        protected EffectParameter scaleParam;

        //protected Texture2D colorTexture;
        //protected Texture2D noiseTexture;

        public ThrustEffect()
            : base("ParticleThrust")
        {
            worldViewParam = effect.Parameters["worldView"];
            worldViewProjectionParam = effect.Parameters["worldViewProjection"];

            //noiseTextureParam = effect.Parameters["NoiseMap"];
            baseMapParam = effect.Parameters["baseMap"];
            centerParam = effect.Parameters["center"];
            amplitudeParam = effect.Parameters["amplitude"];
            speedParam = effect.Parameters["speed"];
            timeParam = effect.Parameters["time"];
            gasParam = effect.Parameters["gas"];
            turboParam = effect.Parameters["turbo"];
            scaleParam = effect.Parameters["scale"];

            //noiseTexture = World.Instance.WorldContent.Load<Texture2D>(Settings.Configuration.EngineSettings.TextureDirectory + "particleNoise");
            //colorTexture = World.Instance.WorldContent.Load<Texture2D>(Settings.Configuration.EngineSettings.TextureDirectory + "particleColor");

            //noiseTextureParam.SetValue(noiseTexture);
            //colorTextureParam.SetValue(colorTexture);
        }

        float fTime = 0;
        const string textureName = "baseMap";
        const string turboName = "TURBO";
        const string gasName = "ACCELERATION";

        public override void Update(BaseMeshPart mesh)
        {
            Camera c = World.Instance.Camera;
            Matrix matWorldView = mesh.Entity.WorldMatrix * c.View;
            Matrix matWorldViewProj = matWorldView * c.Projection;
            worldViewParam.SetValue(matWorldView);
            worldViewProjectionParam.SetValue(matWorldViewProj);

            scaleParam.SetValue(mesh.VectorParameters["SCALE"]);
            baseMapParam.SetValue(mesh.TextureParameters[textureName]);
            turboParam.SetValue(mesh.BoolParameters[turboName]);
            gasParam.SetValue(mesh.FloatParameters[gasName]);
            amplitudeParam.SetValue(10f);
            speedParam.SetValue(1f);

            fTime += (float)World.Instance.GameTime.ElapsedGameTime.Milliseconds / 20000f;

            //while (fTime > 1f) fTime -= 1f;

            timeParam.SetValue(fTime);
        }
    }
}
