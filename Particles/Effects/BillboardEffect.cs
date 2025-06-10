using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using PraktWS0708.Entities;
using Microsoft.Xna.Framework;

namespace PraktWS0708.Rendering.Effects
{
    class BillboardEffect : ManagedEffect
    {
        protected EffectParameter worldViewParam;
        protected EffectParameter worldViewProjectionParam;
        protected EffectParameter texWidthParam;
        protected EffectParameter texHeightParam;
        protected EffectParameter baseMapParam;
        protected EffectParameter widthParam;
        protected EffectParameter heightParam;
        protected EffectParameter aspectRatioParam;
        protected EffectParameter timeParam;

        public BillboardEffect()
            : base("ParticleBillboard")
        {
            worldViewParam = effect.Parameters["worldView"];
            worldViewProjectionParam = effect.Parameters["worldViewProjection"];

            texWidthParam = effect.Parameters["texWidth"]; // number of textures vertically
            texHeightParam = effect.Parameters["texHeight"]; // number of textures horizontally
            baseMapParam = effect.Parameters["baseMap"];
            widthParam = effect.Parameters["width"];    // width of drawing rectangle
            heightParam = effect.Parameters["height"];  // height of drawing rectangle
            aspectRatioParam = effect.Parameters["aspectRatio"];
            timeParam = effect.Parameters["time"];  // should be in [0, 1]

            texWidthParam.SetValue(20);
            texHeightParam.SetValue(1);
            heightParam.SetValue(10.0f);
            widthParam.SetValue(10.0f);

            aspectRatioParam.SetValue(4f / 3f);
            //aspectRatioParam.SetValue(World.Instance.Camera.Aspect);
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

            baseMapParam.SetValue(mesh.TextureParameters[textureName]);

            fTime += (float)World.Instance.GameTime.ElapsedGameTime.Milliseconds / 10000f;

            while (fTime > 1f) fTime -= 1f;

            timeParam.SetValue(fTime);
        }
    }
}
