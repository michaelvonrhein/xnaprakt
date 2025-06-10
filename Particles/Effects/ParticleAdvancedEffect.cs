using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using PraktWS0708.Entities;
using Microsoft.Xna.Framework;
using PraktWS0708.Particles;

namespace PraktWS0708.Rendering.Effects
{
    class ParticleAdvancedEffect : ManagedEffect
    {
        protected EffectParameter worldViewParam;
        protected EffectParameter worldViewProjectionParam;
        //protected EffectParameter noiseTextureParam;
        protected EffectParameter colorTextureParam;
        protected EffectParameter widthParam;
        protected EffectParameter heightParam;
        protected EffectParameter speedParam;
        protected EffectParameter timeParam;
        protected EffectParameter positionParam;
        protected int m_iParticleCount;

        protected ParticleMapEffect m_ParticleMapEffect;

        public ParticleAdvancedEffect(int iParticleCount)
            : base("ParticleAdvanced")
        {
            m_iParticleCount = iParticleCount;
            if (m_iParticleCount < 1) m_iParticleCount = 1;

            worldViewParam = effect.Parameters["worldView"];
            worldViewProjectionParam = effect.Parameters["worldViewProjection"];

            positionParam = effect.Parameters["PositionMap"];
            colorTextureParam = effect.Parameters["ColorMap"];
            widthParam = effect.Parameters["width"];
            heightParam = effect.Parameters["height"];
            speedParam = effect.Parameters["speed"];
            timeParam = effect.Parameters["time"];

            heightParam.SetValue(1.0f);
            speedParam.SetValue(1.0f);

            m_ParticleMapEffect = new ParticleMapEffect();
            m_ParticleMapEffect.CreateRenderTarget();
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

            // update time
            fTime = (float)World.Instance.GameTime.ElapsedGameTime.Milliseconds / 10000f;
            while (fTime > 1f) fTime -= 1f;
            timeParam.SetValue(fTime);

            m_ParticleMapEffect.Update(mesh);
            positionParam.SetValue(m_ParticleMapEffect.PositionTexture);
            widthParam.SetValue(m_ParticleMapEffect.Width);
            heightParam.SetValue(m_ParticleMapEffect.Height);
        }
    }
}
