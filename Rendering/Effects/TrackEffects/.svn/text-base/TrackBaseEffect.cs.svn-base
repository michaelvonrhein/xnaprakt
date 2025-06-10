using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using PraktWS0708.Entities;
using Microsoft.Xna.Framework;
using PraktWS0708.Settings;

namespace PraktWS0708.Rendering.Effects
{
    public class TrackBaseEffect:ManagedTrackEffect
    {
       

        private EffectParameter worldView;
        private EffectParameter worldViewIT;
        private EffectParameter projection;

        private EffectParameter lightAmbient;         // Global ambient light color (rgb)

     
       

        private EffectParameter DiffuseDesc;
        private EffectParameter NormalDesc;
        private EffectParameter Specular;

        private EffectParameter Diffuse1;
        private EffectParameter Diffuse2;
        private EffectParameter Diffuse3;

        private EffectParameter Normal1;
        private EffectParameter Normal2;
        private EffectParameter Normal3;

        private EffectParameter LightMap;
        private EffectParameter LightDirectionMap;


        private EffectParameter lightDirection;
       

        private EffectParameter shadowMapSize;        // Shadow map resolution (texels)

        private EffectParameter shadowMap0;           // Shadowmaps
        private EffectParameter shadowMap1;
        private EffectParameter shadowMap2;

        private EffectParameter lightProjection;      // Transformations from view space to light projection
        private EffectParameter splitPlanes;
       

        private EffectParameter tilesPerSegment;
        private EffectParameter segments;

        

        public TrackBaseEffect(int tiles, int segments):base("TrackBase")
        {
            
            worldView = effect.Parameters["worldView"];
            worldViewIT = effect.Parameters["worldViewIT"];
            projection = effect.Parameters["projection"];

            splitPlanes = effect.Parameters["splitPlanes"];

            lightProjection = effect.Parameters["lightProjection"];


            LightMap = effect.Parameters["lightmap"];
            LightDirectionMap = effect.Parameters["lightdirmap"];
         
            

    
            DiffuseDesc = effect.Parameters["diffuseDefinition"];
            NormalDesc = effect.Parameters["normalDefinition"];
            Specular = effect.Parameters["specular"];  

            Diffuse1 = effect.Parameters["diffuse1"];
            Diffuse2 = effect.Parameters["diffuse2"];
            Diffuse3 = effect.Parameters["diffuse3"];
            Normal1 = effect.Parameters["normal1"];
            Normal2 = effect.Parameters["normal2"];
            Normal3 = effect.Parameters["normal3"];

            lightDirection = effect.Parameters["lightdir"];
            


            shadowMapSize = effect.Parameters["shadowMapSize"];

            shadowMap0 = effect.Parameters["shadowMap0"];
            shadowMap1 = effect.Parameters["shadowMap1"];
            shadowMap2 = effect.Parameters["shadowMap2"];
            

            tilesPerSegment = effect.Parameters["tilesPerSegment"];
            this.segments = effect.Parameters["segments"];

            tilesPerSegment.SetValue(tiles);
            this.segments.SetValue(segments);

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
            worldViewIT.SetValue(Matrix.Invert(Matrix.Transpose(c.View)));
            //worldViewIT.SetValue(c.View);
            LightMap.SetValue(World.Instance.Track.lightmap);
            LightDirectionMap.SetValue(World.Instance.Track.lightdirmap);
           
            DiffuseDesc.SetValue(mesh.Textures[0]);
            NormalDesc.SetValue(mesh.Textures[1]);
            Specular.SetValue(mesh.Textures[2]);
            Diffuse1.SetValue(mesh.Textures[3]);
            Diffuse2.SetValue(mesh.Textures[4]);
            Diffuse3.SetValue(mesh.Textures[5]);
            Normal1.SetValue(mesh.Textures[6]);
            Normal2.SetValue(mesh.Textures[7]);
            Normal3.SetValue(mesh.Textures[8]);

            shadowMapSize.SetValue(World.Instance.Sunlight.size);

            shadowMap0.SetValue(World.Instance.Sunlight.ShadowMaps[0]);
            shadowMap1.SetValue(World.Instance.Sunlight.ShadowMaps[1]);
            shadowMap2.SetValue(World.Instance.Sunlight.ShadowMaps[2]);
            splitPlanes.SetValue(World.Instance.Sunlight.SplitPlanes);
            lightProjection.SetValue(World.Instance.Sunlight.ProjectionMatrices);
            lightDirection.SetValue(World.Instance.Sunlight.Direction);
            TrackSegment segment = World.Instance.Track.SceneGraph.Segments[World.Instance.Track.SceneGraph.cameraSegment];

           

           

           

        }

        public new void End()
        {
            effect.End();
        }
    }
}
