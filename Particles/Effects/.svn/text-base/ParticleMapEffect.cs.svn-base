using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using PraktWS0708.Entities;
using Microsoft.Xna.Framework;
using PraktWS0708.Particles;
using PraktWS0708.ShipUI;

namespace PraktWS0708.Rendering.Effects
{
    public class ParticleMapEffect : ManagedEffect
    {
        protected EffectParameter colorTextureParam;
        protected EffectParameter widthParam;
        protected EffectParameter heightParam;
        protected EffectParameter timeParam;
        protected EffectParameter positionParam;

        protected Texture2D m_PositionTexture;
        protected RenderTarget2D RenderTarget;
        public int m_iWidth;
        public int m_iHeight;

        public Texture2D PositionTexture
        {
            get { return m_PositionTexture; }
        }

        public int Width
        {
            get { return m_iWidth; }
        }

        public int Height
        {
            get { return m_iHeight; }
        }

        public ParticleMapEffect()
            : base("ParticleMap")
        {
            positionParam = effect.Parameters["PositionMap"];
            colorTextureParam = effect.Parameters["ColorMap"];
            widthParam = effect.Parameters["width"];
            heightParam = effect.Parameters["height"];
            timeParam = effect.Parameters["time"];

            m_iWidth = Settings.Configuration.EngineSettings.ParticleMapWidth;
            m_iHeight = Settings.Configuration.EngineSettings.ParticleMapHeight;
        }

        float fTime = 0;
        string textureName = "baseMap";

        public override void Update(BaseMeshPart mesh)
        {
            GraphicsDevice oGraphicsDevice = RenderManager.Instance.GraphicsDevice;
            DepthStencilBuffer oBackDepthStencilBuffer = oGraphicsDevice.DepthStencilBuffer;
            oGraphicsDevice.SetRenderTarget(1, RenderTarget);

            colorTextureParam.SetValue(mesh.TextureParameters[textureName]);
            widthParam.SetValue(Width);
            heightParam.SetValue(Height);

            // update time
            fTime += (float)World.Instance.GameTime.ElapsedGameTime.Milliseconds / 10000f;
            while (fTime > 1f) fTime -= 1f;
            timeParam.SetValue(fTime);

            effect.GraphicsDevice.RenderState.PointSize = 3.0f;
            effect.CurrentTechnique = effect.Techniques["ParticleMap"];
            effect.CommitChanges();

            if (Settings.Configuration.EngineSettings.MaxEffectQuality == Rendering.RenderManager.EffectQuality.High)
            {
                // Update Particles
                effect.Begin();
                effect.Techniques["ParticleMap"].Passes["Particles"].Begin();

                oGraphicsDevice.VertexDeclaration = ParticleEngine.Instance.VertexDeclaration;
                oGraphicsDevice.Vertices[0].SetSource(ParticleEngine.Instance.VertexBuffer, 0, VertexPositionColor.SizeInBytes);
                oGraphicsDevice.DrawPrimitives(PrimitiveType.PointList, 0, Settings.Configuration.EngineSettings.MaxParticleCount * 3);

                effect.Techniques["ParticleMap"].Passes["Particles"].End();
                effect.End();
            }

            oGraphicsDevice.ResolveRenderTarget(1);   // rendering finished - prepare for GetTexture call
            m_PositionTexture = RenderTarget.GetTexture();    // store texture of render target
#if WINDOWS
            //m_PositionTexture.Save("ParticleMap.png", ImageFileFormat.Png);
#endif
            oGraphicsDevice.SetRenderTarget(1, null); // set default render target
            oGraphicsDevice.DepthStencilBuffer = oBackDepthStencilBuffer;   // set depth/stencil buffer to old one

            positionParam.SetValue(m_PositionTexture);
        }

        /**
         * creates a render target for the particle map effect
         */
        public void CreateRenderTarget()
        {
            GraphicsDevice GraphicsDevice = RenderManager.Instance.GraphicsDevice;
            RenderTarget = new RenderTarget2D(GraphicsDevice, Width, Height, 1, SurfaceFormat.Color, GraphicsDevice.PresentationParameters.MultiSampleType, GraphicsDevice.PresentationParameters.MultiSampleQuality);
        }
    }
}
