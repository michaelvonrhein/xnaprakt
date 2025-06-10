using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using PraktWS0708.Entities;
using Microsoft.Xna.Framework;

namespace PraktWS0708.Rendering.Effects
{
    sealed class PlanetEffect
    {
        private Effect effect;
        private EffectParameter worldViewProjection;
        private EffectParameter world;

        private EffectParameter lightPosition;
        private EffectParameter lightDiffuse;

        private EffectParameter textureParameter;
        private Texture2D texture;
        private EffectParameter nightTextureParameter;
        private Texture2D nightTexture;

        public bool ZTestEnable = true;

        private Matrix worldMatrix;

        public Matrix Worldmatrix
        {
            get { return worldMatrix; }
            set { worldMatrix = value; }
        }

        public PlanetEffect(Vector3 lightDirection, Vector3 diffuse, string texName, string nightTexName)
        {
            effect = World.Instance.WorldContent.Load<Effect>(Settings.Configuration.EngineSettings.ShaderDirectory + "Planet");
            worldViewProjection = effect.Parameters["matWorldViewProjection"];
            world = effect.Parameters["matWorld"];

            lightPosition = effect.Parameters["lightDirection"];
            lightPosition.SetValue(lightDirection);
            lightDiffuse = effect.Parameters["lightDiffuse"];
            lightDiffuse.SetValue(diffuse);

            textureParameter = effect.Parameters["planetTex"];
            texture = World.Instance.WorldContent.Load<Texture2D>(Settings.Configuration.EngineSettings.TextureDirectory + "Environment\\" + texName);
            nightTextureParameter = effect.Parameters["planetNightTex"];
            nightTexture = World.Instance.WorldContent.Load<Texture2D>(Settings.Configuration.EngineSettings.TextureDirectory + "Environment\\" + nightTexName);
            
            worldMatrix = Matrix.Identity;
        }

        public void Begin()
        {
            Camera c = World.Instance.Camera;
            worldViewProjection.SetValue(worldMatrix * Matrix.CreateLookAt(Vector3.Zero, c.LookAt - c.Position, c.Up) * c.Projection);
            world.SetValue(worldMatrix);
            textureParameter.SetValue(texture);
            nightTextureParameter.SetValue(nightTexture);

            if (ZTestEnable)
                effect.CurrentTechnique = effect.Techniques["Planet"];
            else
                effect.CurrentTechnique = effect.Techniques["PlanetZTestDisabled"];
            
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
