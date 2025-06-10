using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using PraktWS0708.Entities;
using Microsoft.Xna.Framework;
using PraktWS0708.Settings;

namespace PraktWS0708.Rendering.Effects
{
    class PhongEffect : ManagedEffect
    {
        protected EffectParameter worldView;
        protected EffectParameter view;
        protected EffectParameter projection;

        protected EffectParameter lightAmbient;         // Global ambient light color (rgb)

        protected EffectParameter lightDirection;       // Light Directions in view space
        protected EffectParameter lightProjection;      // Transformations from view space to light projection
      

        protected EffectParameter shadowMapSize;        // Shadow map resolution (texels)

        protected EffectParameter baseMap;              // Base (diffuse & ambient) texture
        protected EffectParameter bumpMap;              // Bump texture
        protected EffectParameter shadowMap0;           // Shadowmaps
        protected EffectParameter shadowMap1;
        protected EffectParameter shadowMap2;

        protected EffectParameter splitPlanes;	

        private string textureName = "baseMap";
        //private string bumpName = "bumpMap";

        private Matrix[] worldProjectionMatrices = new Matrix[Configuration.EngineSettings.shadowmap_splits];

        public PhongEffect()
            : base("Phong")
        {
            worldView = effect.Parameters["worldView"];
            view = effect.Parameters["view"];
            projection = effect.Parameters["projection"];

            lightAmbient = effect.Parameters["lightAmbient"];

            lightDirection = effect.Parameters["lightDirection"];
            lightProjection = effect.Parameters["lightProjection"];
            
            shadowMapSize = effect.Parameters["shadowMapSize"];

            baseMap = effect.Parameters["baseMap"];
            bumpMap = effect.Parameters["bumpMap"];
            shadowMap0 = effect.Parameters["shadowMap0"];
            shadowMap1 = effect.Parameters["shadowMap1"];
            shadowMap2 = effect.Parameters["shadowMap2"];

            splitPlanes = effect.Parameters["splitPlanes"];
        }

        public override void Update(BaseMeshPart mesh)
        {
            World w = World.Instance;
            Camera c = w.Camera;
            Sunlight s = w.Sunlight;
            worldView.SetValue(Orientation * mesh.Entity.WorldMatrix * c.View);
            view.SetValue(c.View);
            projection.SetValue(c.Projection);

            baseMap.SetValue(mesh.TextureParameters[textureName]);
            //bumpMap.SetValue(mesh.TextureParameters[bumpName]);

            shadowMapSize.SetValue(Configuration.EngineSettings.ShadowMapResolution);

            lightAmbient.SetValue(s.AmbientColor);

            shadowMap0.SetValue(s.ShadowMaps[0]);
            shadowMap1.SetValue(s.ShadowMaps[1]);
            shadowMap2.SetValue(s.ShadowMaps[2]);
            splitPlanes.SetValue(s.SplitPlanes);
       
            for (int i = 0; i < worldProjectionMatrices.Length; i++)
            {
                worldProjectionMatrices[i] = Orientation * mesh.Entity.WorldMatrix * s.ProjectionMatrices[i];
            }
            lightProjection.SetValue(worldProjectionMatrices);
            lightDirection.SetValue(s.Direction);
        }
    }
}
