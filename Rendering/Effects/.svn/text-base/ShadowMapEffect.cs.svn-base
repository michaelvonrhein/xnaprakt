using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using PraktWS0708.Entities;
using Microsoft.Xna.Framework;
using PraktWS0708.Settings;
using PraktWS0708.Utils;

namespace PraktWS0708.Rendering
{
    public sealed class ShadowMapEffect
    {
        public RenderTarget2D[][] RenderTargets = new RenderTarget2D[2][];
        public DepthStencilBuffer[] DepthStencilBuffers = new DepthStencilBuffer[Configuration.EngineSettings.shadowmap_splits];
        private Effect effect;
        private EffectParameter projectionParameter;
        private EffectParameter maxDepthParameter;

        private EffectParameter minDepthParameter;

        public enum QUALITY
        {
            HIGH=0,
            LOW=1
        };

        public ShadowMapEffect()
        {
            RenderTargets[0]=new RenderTarget2D[Configuration.EngineSettings.shadowmap_splits];
            RenderTargets[1] = new RenderTarget2D[Configuration.EngineSettings.shadowmap_splits];
            effect = RenderManager.Instance.PersistentContent.Load<Effect>(Settings.Configuration.EngineSettings.ShaderDirectory + "ShadowMap");
            projectionParameter = effect.Parameters["lightViewProjection"];
            maxDepthParameter = effect.Parameters["maxDepth"];
            minDepthParameter = effect.Parameters["minDepth"];
        }

        public static int getSize(QUALITY quality)
        {
            if(quality==QUALITY.HIGH)
                return Configuration.EngineSettings.ShadowMapResolution;
            else 
                return Configuration.EngineSettings.ShadowMapResolution/4;
                    
        }

        public void GenerateShadowMaps(QUALITY quality)
        {
            World w = World.Instance;
            Track t = w.Track;
            SceneGraph g = t.SceneGraph;
            GraphicsDevice d = RenderManager.Instance.GraphicsDevice;
            DepthStencilBuffer backDepthBuffer = d.DepthStencilBuffer;
            Matrix viewProjection;
            int qflag=(int)quality;
            w.Sunlight.buildProjectionMatrices(new MyFrustum(w.Camera), Configuration.EngineSettings.shadowmap_splits);
            for (int i = 0; i < w.Sunlight.ProjectionMatrices.Length; i++)
            {
                d.SetRenderTarget(0, RenderTargets[qflag][i]);
                
                d.DepthStencilBuffer = DepthStencilBuffers[i];
                d.Clear(Color.White);
                maxDepthParameter.SetValue(w.Sunlight.maxDepths[i]);
                minDepthParameter.SetValue(w.Sunlight.minDepths[i]);
                viewProjection = /*.Sunlight.ViewMatrix */ w.Sunlight.ProjectionMatrices[i];
                projectionParameter.SetValue(viewProjection);
                effect.Begin();
                effect.CurrentTechnique.Passes[0].Begin();
                World.Instance.Track.DrawGeometry();
                effect.CurrentTechnique.Passes[0].End();
                effect.CurrentTechnique.Passes[1].Begin();
                for (int k = 0; k < g.VisibleSegments.Length; k++)
                {
                    for (int j = 0; j < g.VisibleSegments[k].Objects.Count; j++)
                    {
                        projectionParameter.SetValue(g.VisibleSegments[k].Objects[j].WorldMatrix * viewProjection);
                        effect.CommitChanges();
                        g.VisibleSegments[k].Objects[j].RenderingPlugin.DrawGeometry();
                    }
                   
                } 

                effect.CurrentTechnique.Passes[1].End();
                effect.End();
                d.ResolveRenderTarget(0);

                w.Sunlight.ShadowMaps[i] = RenderTargets[qflag][i].GetTexture();

            }
            w.Sunlight.size = getSize(quality);

#if WINDOWS
            //w.Sunlight.ShadowMaps[0].Save("smap" + 0 + ".jpg", ImageFileFormat.Jpg);
            //w.Sunlight.ShadowMaps[1].Save("smap" + 1 + ".jpg", ImageFileFormat.Jpg);
#endif

            d.SetRenderTarget(0, null);
            d.DepthStencilBuffer = backDepthBuffer;
        }
    }
}
