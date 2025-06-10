using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using PraktWS0708.Entities;
using Microsoft.Xna.Framework;
using PraktWS0708.Settings;

namespace PraktWS0708.Rendering.Effects
{
    public class TrackBackEffect:ManagedTrackEffect
    {
       

        private EffectParameter worldView;
        private EffectParameter projection;

        private EffectParameter lightAmbient;         // Global ambient light color (rgb)

     
        private EffectParameter shadowMapSize;        // Shadow map resolution (texels)

        private EffectParameter baseMap;              // Base (diffuse & ambient) texture
       

        private EffectParameter tilesPerSegment;
        private EffectParameter segments;

       

        public TrackBackEffect():base("TrackBack")
        {
            
            worldView = effect.Parameters["worldView"];
            projection = effect.Parameters["projection"];

            lightAmbient = effect.Parameters["lightAmbient"];

            baseMap = effect.Parameters["baseMap"];
            

            tilesPerSegment = effect.Parameters["tilesPerSegment"];
            segments = effect.Parameters["segments"];
            baseMap = effect.Parameters["baseMap"];
            tilesPerSegment.SetValue(8);
            segments.SetValue(64);
        }

        public new EffectPassCollection Passes
        {
            get { return effect.CurrentTechnique.Passes; }
        }

        public override void Update(TrackMeshPart mesh)
        {
            

            World w = World.Instance;
            Camera c = w.Camera;
            worldView.SetValue(c.View);
            projection.SetValue(c.Projection);

            //shadowMapSize.SetValue(Configuration.EngineSettings.ShadowMapResolution);
            baseMap.SetValue(mesh.Textures[0]);
            lightAmbient.SetValue(World.Instance.Sunlight.AmbientColor);

           

        }

        public new void End()
        {
            effect.End();
        }
    }
}
