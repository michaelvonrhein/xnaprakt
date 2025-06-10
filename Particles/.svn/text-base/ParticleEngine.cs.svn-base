using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using PraktWS0708.Rendering;

namespace PraktWS0708.Particles
{
    public enum ParticleSystemType
    {
        Explosion,
        Fire,
        Thrust,
        Billboard,
        PowerUp,
        Advanced,
        Default
    };

    class ParticleEngine
    {
        public VertexBuffer VertexBuffer;
        public VertexDeclaration VertexDeclaration;

        private ParticleEngine()
        {
            VertexPositionNormalTexture[] oVertexPositionNormalTexture = new VertexPositionNormalTexture[3 * Settings.Configuration.EngineSettings.MaxParticleCount];

            Random oRandom = new Random();
            Vector3 vRandom = Vector3.Zero, vRandom2;

            for (int iIndex = 0; iIndex < oVertexPositionNormalTexture.Length; iIndex++)
            {
                if (iIndex % 3 == 0)
                {
                    do
                    {
                        vRandom = new Vector3(
                            (float)(oRandom.NextDouble() * 2.0 - 1.0),
                            (float)(oRandom.NextDouble() * 2.0 - 1.0),
                            (float)(oRandom.NextDouble() * 2.0 - 1.0));
                    }
                    while (vRandom.Length() > 0.5f);
                }

                do
                {
                    vRandom2 = new Vector3(
                        (float)(oRandom.NextDouble() * 2.0 - 1.0),
                        (float)(oRandom.NextDouble() * 2.0 - 1.0),
                        (float)(oRandom.NextDouble() * 2.0 - 1.0));
                }
                while (vRandom2.Length() > 0.5f);

                oVertexPositionNormalTexture[iIndex].Position = vRandom2;
                oVertexPositionNormalTexture[iIndex].Normal = vRandom;
                oVertexPositionNormalTexture[iIndex].TextureCoordinate = new Vector2(iIndex / 3, iIndex % 3);
            }

            VertexDeclaration = new VertexDeclaration(RenderManager.Instance.GraphicsDevice, VertexPositionNormalTexture.VertexElements);
            VertexBuffer = new VertexBuffer(RenderManager.Instance.GraphicsDevice, 3 * Settings.Configuration.EngineSettings.MaxParticleCount * VertexPositionNormalTexture.SizeInBytes, ResourceUsage.None, ResourceManagementMode.Automatic);

            VertexBuffer.SetData<VertexPositionNormalTexture>(oVertexPositionNormalTexture);
        }

        private static ParticleEngine oInstance;

        public static ParticleEngine Instance
        {
            get
            {
                if (oInstance == null)
                {
                    oInstance = new ParticleEngine();
                }
                return oInstance;
            }
        }
    }
}
