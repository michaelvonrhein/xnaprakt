using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using PraktWS0708.Entities;
using Microsoft.Xna.Framework;

namespace PraktWS0708.Rendering.Effects
{
    class ToonEffect : ManagedEffect
    {
        protected EffectParameter world;
        protected EffectParameter worldView;
        protected EffectParameter worldViewProjection;
        protected EffectParameter lightPosition;
        protected EffectParameter lightAmbient;
        protected EffectParameter lightSpecular;
        protected EffectParameter lightDiffuse;

        protected EffectParameter viewPosition;

        protected EffectParameter baseMap;

        protected EffectParameter layerOneSharp;
        protected EffectParameter layerOneRough;
        protected EffectParameter layerOneContrib;
        protected EffectParameter layerTwoSharp;
        protected EffectParameter layerTwoRough;
        protected EffectParameter layerTwoContrib;

        protected EffectParameter edgeOffset;


        public ToonEffect()
            : base("Toon")
        {
            world = effect.Parameters["world"];
            worldView = effect.Parameters["worldView"];
            worldViewProjection = effect.Parameters["worldViewProjection"];

            lightPosition = effect.Parameters["lightPosition"];
            lightAmbient = effect.Parameters["lightAmbient"];
            lightSpecular = effect.Parameters["lightSpecular"];
            lightDiffuse = effect.Parameters["lightDiffuse"];

            viewPosition = effect.Parameters["viewPosition"];

            layerOneSharp = effect.Parameters["fLayerOneSharp"];
            layerOneRough = effect.Parameters["fLayerOneRough"];
            layerOneContrib = effect.Parameters["fLayerOneContrib"];
            layerTwoSharp = effect.Parameters["fLayerTwoSharp"];
            layerTwoRough = effect.Parameters["fLayerTwoRough"];
            layerTwoContrib = effect.Parameters["fLayerTwoContrib"];

            edgeOffset = effect.Parameters["fEdgeOffset"];
            
            
            //lightAmbient.SetValue(World.Instance.Sunlight.AmbientColor);
            //lightSpecular.SetValue(World.Instance.Sunlight.SpecularColor);
            lightDiffuse.SetValue(new Vector4(0.8f, 0.8f, 0.8f, 1f));

            viewPosition.SetValue(World.Instance.Camera.Position);

            baseMap = effect.Parameters["baseMap"];

            layerOneSharp.SetValue(0.6f);
            layerOneRough.SetValue(0.98f);
            layerOneContrib.SetValue(0.1f);
            layerTwoSharp.SetValue(0.85f);
            layerTwoRough.SetValue(10f);
            layerTwoContrib.SetValue(0.3f);

            edgeOffset.SetValue(0.1f);        
        }

        public override void Update(BaseMeshPart mesh)
        {
            Camera c = World.Instance.Camera;
            Matrix wv = Orientation * mesh.Entity.WorldMatrix * c.View;
            Matrix wvp = wv * c.Projection;
            world.SetValue(mesh.Entity.WorldMatrix);
            worldView.SetValue(wv);
            worldViewProjection.SetValue(wvp);
            lightPosition.SetValue(World.Instance.Sunlight.Position);
            baseMap.SetValue(mesh.TextureParameters["baseMap"]);

            viewPosition.SetValue(c.Position);
        }
    }
}
