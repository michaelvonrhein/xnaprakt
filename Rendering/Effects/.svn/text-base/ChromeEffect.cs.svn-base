using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using PraktWS0708.Entities;
using PraktWS0708.Settings;
using Microsoft.Xna.Framework;


namespace PraktWS0708.Rendering.Effects
{
    public class ChromeEffect : ManagedEffect
    {
        protected EffectParameter world;
        protected EffectParameter worldView;
        protected EffectParameter worldViewProjection;
        protected EffectParameter projection;

        protected EffectParameter lightAmbient;         // Global ambient light color (rgb)

        protected EffectParameter lightPosition;        // Light Positions in view space
        protected EffectParameter lightDirection;       // Light Directions in view space
        protected EffectParameter lightAngle;           // Cosine of the light angles
        protected EffectParameter lightSpecular;        // Specular colors (rgb)
        protected EffectParameter lightDiffuse;         // Diffuse colors (rgb)
        protected EffectParameter lightSpecularPower;   // Specular highlight sizes
        protected EffectParameter lightProjection;      // Transformations from view space to light projection
        protected EffectParameter lightShadowDepth;     // Maximum real depths of the shadowmaps
        protected EffectParameter lightShadow;          // Indicates whether the light has a shadowmap

        protected EffectParameter shadowMapSize;        // Shadow map resolution (texels)

        protected EffectParameter environmentMap;
        protected EffectParameter reflectivity;

        protected EffectParameter shadowMap0;           // Shadowmaps
        protected EffectParameter shadowMap1;
        protected EffectParameter shadowMap2;
        protected EffectParameter shadowMap3;

        private Vector3[] lightPositions = new Vector3[4];
        private Vector3[] lightDirections = new Vector3[4];
        private float[] lightAngles = new float[4];
        private Vector3[] lightSpeculars = new Vector3[4];
        private Vector3[] lightDiffuses = new Vector3[4];
        private float[] lightSpecularPowers = new float[4];
        private Matrix[] lightProjections = new Matrix[4];
        private float[] lightShadowDepths = new float[4];
        private float[] lightShadows = new float[4];

        TextureCube debugTex;

        public static RenderTargetCube RenderTarget;
        public static DepthStencilBuffer DepthStencilBuffer;



        public ChromeEffect()
            : base("Chrome")
        {
            world = effect.Parameters["world"];
            worldView = effect.Parameters["worldView"];
            worldViewProjection = effect.Parameters["worldViewProjection"];
            projection = effect.Parameters["projection"];

            lightAmbient = effect.Parameters["lightAmbient"];

            lightPosition = effect.Parameters["lightPosition"];
            lightDirection = effect.Parameters["lightDirection"];
            lightAngle = effect.Parameters["lightAngle"];
            lightSpecular = effect.Parameters["lightSpecular"];
            lightDiffuse = effect.Parameters["lightDiffuse"];
            lightSpecularPower = effect.Parameters["lightSpecularPower"];
            lightProjection = effect.Parameters["lightProjection"];
            lightShadowDepth = effect.Parameters["lightShadowDepth"];
            lightShadow = effect.Parameters["lightShadow"];

            shadowMapSize = effect.Parameters["shadowMapSize"];

            shadowMap0 = effect.Parameters["shadowMap0"];
            shadowMap1 = effect.Parameters["shadowMap1"];
            shadowMap2 = effect.Parameters["shadowMap2"];
            shadowMap3 = effect.Parameters["shadowMap3"];

            environmentMap = effect.Parameters["environmentMap"];
            reflectivity = effect.Parameters["reflectivity"];

            reflectivity.SetValue(0.8f);

            debugTex = World.Instance.WorldContent.Load<TextureCube>("Content/Textures/ColoredCube");

        }

        public override void Update(BaseMeshPart mesh)
        {
            Camera c = World.Instance.Camera;
            Matrix wv = Orientation * mesh.Entity.WorldMatrix * c.View;
            Matrix wvp = wv * c.Projection;
            
            world.SetValue(mesh.Entity.WorldMatrix);
            worldView.SetValue(wv);
            worldViewProjection.SetValue(wvp);
            projection.SetValue(c.Projection);

            shadowMapSize.SetValue(Configuration.EngineSettings.ShadowMapResolution);

            lightAmbient.SetValue(World.Instance.Sunlight.AmbientColor);

            TrackSegment s = World.Instance.Track.SceneGraph.Segments[mesh.Entity.CurrentSegment];

            /*
            if (s.DistanceSortedSpotlights[0].ShadowMap != null)
            {
                shadowMap0.SetValue(s.DistanceSortedSpotlights[0].ShadowMap);
            }
            if (s.DistanceSortedSpotlights[1].ShadowMap != null)
            {
                shadowMap1.SetValue(s.DistanceSortedSpotlights[1].ShadowMap);
            }
            if (s.DistanceSortedSpotlights[2].ShadowMap != null)
            {
                shadowMap2.SetValue(s.DistanceSortedSpotlights[2].ShadowMap);
            }
            if (s.DistanceSortedSpotlights[3].ShadowMap != null)
            {
                shadowMap3.SetValue(s.DistanceSortedSpotlights[3].ShadowMap);
            }
            

            for (int i = 0; i < 4; i++)
            {
                Spotlight l = s.DistanceSortedSpotlights[i];
                lightPositions[i] = Vector3.Transform(l.Camera.Position, c.View);
                lightDirections[i] = Vector3.Transform(l.Camera.LookAt, c.View) - lightPositions[i];
                lightAngles[i] = (float)Math.Cos(l.Camera.Fov * 0.5f);
                lightSpeculars[i] = l.Specular;
                lightDiffuses[i] = l.Diffuse;
                lightSpecularPowers[i] = l.SpecularPower;
                lightProjections[i] = mesh.Entity.WorldMatrix * l.Camera.ViewProjection;
                lightShadowDepths[i] = l.Camera.Far;
                lightShadows[i] = (l.ShadowMap != null) ? 1f : 0f;
            }
             */ 

            lightPosition.SetValue(lightPositions);
            lightDirection.SetValue(lightDirections);
            lightAngle.SetValue(lightAngles);
            lightSpecular.SetValue(lightSpeculars);
            lightDiffuse.SetValue(lightDiffuses);
            lightSpecularPower.SetValue(lightSpecularPowers);
            lightProjection.SetValue(lightProjections);
            lightShadowDepth.SetValue(lightShadowDepths);
            lightShadow.SetValue(lightShadows);

            //environmentMap.SetValue(debugTex);
            environmentMap.SetValue(mesh.Entity.RenderingPlugin.EnvironmentMap);
        }
    }
}
