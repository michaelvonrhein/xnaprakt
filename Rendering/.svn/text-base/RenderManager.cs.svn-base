using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PraktWS0708.Entities;
using PraktWS0708.Utils;
using PraktWS0708.Settings;
using PraktWS0708.Physics;
using PraktWS0708.Rendering.Effects;
using Microsoft.Xna.Framework.Content;

namespace PraktWS0708.Rendering
{
    public class RenderManager
    {

        #region statische instanz
        public static RenderManager Instance;
        public ContentManager PersistentContent;

        static RenderManager()
        {
            Instance = new RenderManager();
        }
        #endregion

        public enum EffectQuality
        {
            Low,
            Medium,
            High
        }

        public GraphicsDevice GraphicsDevice;
        public SpriteBatch SpriteBatch;
        public ShadowMapEffect ShadowMapEffect;
        protected MiniMapEffect MiniMapEffect;
        protected MotionBlurEffect MotionBlurEffect;
        protected MirrorEffect MirrorEffect;
        protected BloomEffect BloomEffect;
        protected ShieldDistortionEffect ShieldDistortionEffect;
        protected OldCameraEffect OldCameraEffect;
        protected GaussEffect GaussEffect;
        protected LensFlareEffect LensFlareEffect;

        public bool MakeScreenshot = false;
        private Texture2D screenhotTex;

        public bool DrawMirror = true;
        public bool DrawMiniMap = true;
        public bool ApplyOldCameraEffect = false;

        private Texture2D SharedResolveTarget;

        public void Resize(int w, int h)
        {
            World.Instance.Camera.Aspect = (float)w / (float)h;
            World.Instance.Camera.Update();
            UpdateRenderTargets();
            Settings.Configuration.EngineSettings.ScreenResolution.Height = h;
            Settings.Configuration.EngineSettings.ScreenResolution.Width = w;
        }

        public void Initialize()
        {
            ShadowMapEffect = new ShadowMapEffect();
            MiniMapEffect = new MiniMapEffect();
            MiniMapEffect.SpriteBatch = SpriteBatch;
            MotionBlurEffect = new MotionBlurEffect();
            BloomEffect = new BloomEffect();
            ShieldDistortionEffect = new ShieldDistortionEffect();
            OldCameraEffect = new OldCameraEffect();
            GaussEffect = new GaussEffect();
            LensFlareEffect = new LensFlareEffect();

            int w = GraphicsDevice.PresentationParameters.BackBufferWidth;
            int h = GraphicsDevice.PresentationParameters.BackBufferHeight;

            MirrorEffect = new MirrorEffect(w / 3, h / 8);

            UpdateRenderTargets();
        }

        public EffectQuality GetEffectQuality(float distanceSQ)
        {
            if (Configuration.EngineSettings.MaxEffectQuality == EffectQuality.Low || distanceSQ > Configuration.EngineSettings.LowQualityDistanceSQThreshold)
            {
                return EffectQuality.Low;
            }
            else if (Configuration.EngineSettings.MaxEffectQuality == EffectQuality.Medium || distanceSQ > Configuration.EngineSettings.MediumQualityDistanceSQThreshold)
            {
                return EffectQuality.Medium;
            }
            else
            {
                return EffectQuality.High;
            }
        }

        public void Draw(GameTime gametime)
        {
//#if DEBUG
            PerformanceEater last = PerformanceMeter.Instance.PerfomanceEaterChange(PerformanceEater.Rendering);
//#endif
            float relativeSpeed = World.Instance.PlayersShip.PhysicsPlugin.Velocity / 0.0075f; // This is the max speed - better get this from physics...

            // This ensures that speed-based interpolations do not run wild
            relativeSpeed = MathHelper.Clamp(relativeSpeed, 0f, 1f);

            World w = World.Instance;
            Camera c = w.Camera;
            SceneGraph s = w.Track.SceneGraph;

            c.Fov = MathHelper.Lerp(MathHelper.PiOver2, MathHelper.PiOver2 * 1.25f, relativeSpeed);
            c.Update();

            if (Configuration.EngineSettings.ShieldDistortion) ShieldDistortionEffect.GenerateDistortionMap(gametime);

            if (DrawMiniMap) MiniMapEffect.GenerateMiniMap();
            if (DrawMirror) MirrorEffect.GenerateMirrorMap();

            World.Instance.PlayersShip.RenderingPlugin.GenerateEnvironmentMap();
            

            s.MarkVisibleNodes(c);
            ShadowMapEffect.GenerateShadowMaps(ShadowMapEffect.QUALITY.HIGH);
            w.gameEnvironment.Draw();
            w.Track.Draw();
            for (int i = 0; i < s.VisibleSegments.Length; i++)
            {
                for (int j = 0; j < s.VisibleSegments[i].Objects.Count; j++)
                {
                    RenderingPlugin r = s.VisibleSegments[i].Objects[j].RenderingPlugin;

                    if (r.Type == RenderingPlugin.RenderingPluginType.MainRenderer)
                    {
                        r.Draw();
                    }
                }
            }

            if (Settings.Configuration.EngineSettings.RenderParticleEngine)
            {
                GraphicsDevice.ResolveBackBuffer(SharedResolveTarget);
                GraphicsDevice.Clear(Color.Black);

                // Shared for depth reconstruction
                Effect effect = LensFlareEffect.effect;
                EffectParameter worldViewProjection = LensFlareEffect.worldViewProjection;

                effect.CurrentTechnique = effect.Techniques[0];

                effect.Begin();
                effect.CurrentTechnique.Passes[0].Begin();

                worldViewProjection.SetValue(c.ViewProjection);
                effect.CommitChanges();
                w.Track.DrawGeometry();

                w.Track.SceneGraph.MarkVisibleNodes(c);

                foreach (TrackSegment ts in w.Track.SceneGraph.VisibleSegments)
                {
                    for (int i=0; i< ts.Objects.Count; i++)
                    {
                        worldViewProjection.SetValue(ts.Objects[i].WorldMatrix * c.ViewProjection);
                        effect.CommitChanges();
                        ts.Objects[i].RenderingPlugin.DrawGeometry();
                    }
                }
                effect.CurrentTechnique.Passes[0].End();
                effect.End();

                GraphicsDevice.Clear(ClearOptions.Target, Color.Black, 1.0f, 0);

                World.Instance.ParticleManager.Draw();
                GaussEffect.ApplyGauss(Configuration.EngineSettings.ParticleBlurAmount);
                SpriteBatch.Begin(SpriteBlendMode.Additive, SpriteSortMode.Immediate, SaveStateMode.None);
                SpriteBatch.Draw(SharedResolveTarget, Vector2.Zero, Color.White);
                SpriteBatch.End();
            }

            if (Configuration.EngineSettings.ShieldDistortion)
            {
                ShieldDistortionEffect.ApplyShieldDistortion();
            }

            if (Configuration.EngineSettings.MotionBlur)
            {
                MotionBlurEffect.ApplyMotionBlur();
            }

            if (DrawMirror && !MakeScreenshot)
            {
                SpriteBatch.Begin(SpriteBlendMode.None);
                SpriteBatch.Draw(MirrorEffect.MirrorTexture, new Vector2(GraphicsDevice.PresentationParameters.BackBufferWidth / 3.0f, GraphicsDevice.PresentationParameters.BackBufferWidth / 16.0f), null, Color.White, 0f, Vector2.Zero, Vector2.One, SpriteEffects.FlipHorizontally, 0f);
                SpriteBatch.End();
            }

            if (Configuration.EngineSettings.Bloom)
            {
                BloomEffect.ApplyBloom();
            }

            if (Configuration.EngineSettings.LensFlare)
            {
                LensFlareEffect.ApplyLensFlare();
            }

            if (ApplyOldCameraEffect)
            {
                OldCameraEffect.Update(gametime);
                OldCameraEffect.Apply();
            }

            if (RenderWaypoints)
                DrawWaypoints();

#if WINDOWS
            if (MakeScreenshot)
            {
                GraphicsDevice.ResolveBackBuffer(screenhotTex);
                screenhotTex.Save("screenshot.png", ImageFileFormat.Png);
                World.Instance.Sunlight.ShadowMaps[0].Save("smap1.jpg", ImageFileFormat.Jpg);
                World.Instance.Sunlight.ShadowMaps[1].Save("smap2.jpg", ImageFileFormat.Jpg);
                World.Instance.Sunlight.ShadowMaps[2].Save("smap3.jpg", ImageFileFormat.Jpg);
                MakeScreenshot = false;
            }
#endif
            
            if (DrawMiniMap) MiniMapEffect.Draw();

//#if DEBUG
            PerformanceMeter.Instance.PerfomanceEaterChange(last);
//#endif
        }

        public void DrawTrackEditor(GameTime gametime)
        {
//#if DEBUG
            PerformanceEater last = PerformanceMeter.Instance.PerfomanceEaterChange(PerformanceEater.Rendering);
//#endif
            World w = World.Instance;
            Camera c = w.Camera;
            SceneGraph s = w.Track.SceneGraph;

            c.Fov = MathHelper.PiOver2;
            c.Update();

            if (World.Instance.PlayersShip.PhysicsPlugin.Type != PhysicsPlugin.PhysicsPluginType.Null)
                World.Instance.PlayersShip.RenderingPlugin.GenerateEnvironmentMap();

            s.MarkVisibleNodes(c);
            ShadowMapEffect.GenerateShadowMaps(ShadowMapEffect.QUALITY.LOW);
            w.gameEnvironment.Draw();
            w.Track.Draw();
            for (int i = 0; i < s.VisibleSegments.Length; i++)
            {
                for (int j = 0; j < s.VisibleSegments[i].Objects.Count; j++)
                {
                    s.VisibleSegments[i].Objects[j].RenderingPlugin.Draw();
                }
            }

            if (Configuration.EngineSettings.Bloom)
            {
                BloomEffect.ApplyBloom();
            }

#if WINDOWS
            if (MakeScreenshot)
            {
                GraphicsDevice.ResolveBackBuffer(screenhotTex);
                screenhotTex.Save("screenshot.png", ImageFileFormat.Png);
                LensFlareEffect.OcclusionTarget.GetTexture().Save("occlusion.jpg", ImageFileFormat.Jpg);
                MakeScreenshot = false;
            }
#endif

//#if DEBUG
            PerformanceMeter.Instance.PerfomanceEaterChange(last);
//#endif
        }

        //HACK: renderable waypoints

        public bool RenderWaypoints = false;

        public int HighlightWaypoint = -1;
        public int HighlightWaypoint2 = -1;

        private VertexDeclaration waypointDecl;
        private VertexBuffer waypointBuf;
        private VertexBuffer waypointBuf2;
        private BasicEffect waypointEffect;

        public bool HasWaypoints
        {
            get { return waypointDecl != null; }
        }
        public void SetWaypoints(Vector3[] waypoints)
        {
            if (waypointDecl == null)
            {
                waypointDecl = new VertexDeclaration(RenderManager.Instance.GraphicsDevice, new VertexElement[] { new VertexElement(0, 0, VertexElementFormat.Vector3, VertexElementMethod.Default, VertexElementUsage.Position, 0) });
                waypointEffect = new BasicEffect(RenderManager.Instance.GraphicsDevice, new EffectPool());
            }

            waypointBuf = new VertexBuffer(RenderManager.Instance.GraphicsDevice, waypoints.Length * waypointDecl.GetVertexStrideSize(0), ResourceUsage.Points);
            waypointBuf.SetData<Vector3>(waypoints);
        }
        public void SetWaypoints2(Vector3[] waypoints)
        {
            if (waypointDecl == null)
            {
                waypointDecl = new VertexDeclaration(RenderManager.Instance.GraphicsDevice, new VertexElement[] { new VertexElement(0, 0, VertexElementFormat.Vector3, VertexElementMethod.Default, VertexElementUsage.Position, 0) });
                waypointEffect = new BasicEffect(RenderManager.Instance.GraphicsDevice, new EffectPool());
            }

            waypointBuf2 = new VertexBuffer(RenderManager.Instance.GraphicsDevice, waypoints.Length * waypointDecl.GetVertexStrideSize(0), ResourceUsage.Points);
            waypointBuf2.SetData<Vector3>(waypoints);
        }
        public void ClearWaypoints()
        {
            waypointDecl = null;
            waypointEffect = null;
            waypointBuf = null;
            waypointBuf2 = null;
        }
        private void DrawWaypoints()
        {
            if (waypointEffect != null)
            {
                PerformanceEater last = PerformanceMeter.Instance.PerfomanceEaterChange(PerformanceEater.Rendering);
                
                GraphicsDevice g = RenderManager.Instance.GraphicsDevice;
                g.VertexDeclaration = waypointDecl;
                g.Vertices[0].SetSource(waypointBuf, 0, waypointDecl.GetVertexStrideSize(0));
                waypointEffect.World = Entities.World.Instance.Track.WorldMatrix;
                waypointEffect.View = Entities.World.Instance.Camera.View;
                waypointEffect.Projection = Entities.World.Instance.Camera.Projection;
                waypointEffect.LightingEnabled = true;
                waypointEffect.EmissiveColor = Vector3.One;
                waypointEffect.CommitChanges();

                float oldPointSize = g.RenderState.PointSize;
                g.RenderState.PointSize = 5;

                waypointEffect.Begin(SaveStateMode.SaveState);
                waypointEffect.Techniques[0].Passes[0].Begin();

                g.DrawPrimitives(PrimitiveType.PointList, 0, waypointBuf.SizeInBytes / waypointDecl.GetVertexStrideSize(0));

                if (waypointBuf2 != null)
                {
                    g.Vertices[0].SetSource(waypointBuf2, 0, waypointDecl.GetVertexStrideSize(0));
                    waypointEffect.EmissiveColor = Vector3.UnitY;
                    waypointEffect.CommitChanges();
                    g.DrawPrimitives(PrimitiveType.PointList, 0, waypointBuf2.SizeInBytes / waypointDecl.GetVertexStrideSize(0));

                    if (HighlightWaypoint2 >= 0)
                    {
                        waypointEffect.EmissiveColor = Vector3.UnitZ;
                        waypointEffect.CommitChanges();
                        g.DrawPrimitives(PrimitiveType.PointList, HighlightWaypoint2, 1);
                    }
                }

                if (HighlightWaypoint >= 0)
                {
                    g.Vertices[0].SetSource(waypointBuf, 0, waypointDecl.GetVertexStrideSize(0));
                    waypointEffect.EmissiveColor = Vector3.UnitX;
                    waypointEffect.CommitChanges();
                    g.DrawPrimitives(PrimitiveType.PointList, HighlightWaypoint, 1);
                }

                waypointEffect.Techniques[0].Passes[0].End();
                waypointEffect.End();

                g.RenderState.PointSize = oldPointSize;

                PerformanceMeter.Instance.PerfomanceEaterChange(last);
            }
        }

        protected void UpdateRenderTargets()
        {
            int size = Configuration.EngineSettings.ShadowMapResolution;
            int maps = (Configuration.EngineSettings.shadowmap_splits);
            
                ShadowMapEffect.RenderTargets[0][0] = new RenderTarget2D(GraphicsDevice, ShadowMapEffect.getSize(ShadowMapEffect.QUALITY.HIGH), ShadowMapEffect.getSize(ShadowMapEffect.QUALITY.HIGH),1, SurfaceFormat.Single, MultiSampleType.None,0);
                ShadowMapEffect.RenderTargets[1][0] = new RenderTarget2D(GraphicsDevice, ShadowMapEffect.getSize(ShadowMapEffect.QUALITY.LOW), ShadowMapEffect.getSize(ShadowMapEffect.QUALITY.LOW), 1, SurfaceFormat.Single, MultiSampleType.None, 0);
                ShadowMapEffect.DepthStencilBuffers[0] = new DepthStencilBuffer(GraphicsDevice, size, size, DepthFormat.Depth24Stencil8, MultiSampleType.None, 0);
                ShadowMapEffect.RenderTargets[0][1] = new RenderTarget2D(GraphicsDevice, ShadowMapEffect.getSize(ShadowMapEffect.QUALITY.HIGH), ShadowMapEffect.getSize(ShadowMapEffect.QUALITY.HIGH), 1, SurfaceFormat.Single, MultiSampleType.None, 0);
                ShadowMapEffect.RenderTargets[1][1] = new RenderTarget2D(GraphicsDevice, ShadowMapEffect.getSize(ShadowMapEffect.QUALITY.LOW), ShadowMapEffect.getSize(ShadowMapEffect.QUALITY.LOW), 1, SurfaceFormat.Single, MultiSampleType.None, 0);
                ShadowMapEffect.DepthStencilBuffers[1] = new DepthStencilBuffer(GraphicsDevice, size, size, DepthFormat.Depth24Stencil8, MultiSampleType.None, 0);
                ShadowMapEffect.RenderTargets[0][2] = new RenderTarget2D(GraphicsDevice, ShadowMapEffect.getSize(ShadowMapEffect.QUALITY.HIGH), ShadowMapEffect.getSize(ShadowMapEffect.QUALITY.HIGH), 1, SurfaceFormat.Single, MultiSampleType.None, 0);
                ShadowMapEffect.RenderTargets[1][2] = new RenderTarget2D(GraphicsDevice, ShadowMapEffect.getSize(ShadowMapEffect.QUALITY.LOW), ShadowMapEffect.getSize(ShadowMapEffect.QUALITY.LOW), 1, SurfaceFormat.Single, MultiSampleType.None, 0);
                ShadowMapEffect.DepthStencilBuffers[2] = new DepthStencilBuffer(GraphicsDevice, size, size, DepthFormat.Depth24Stencil8, MultiSampleType.None, 0);
            float ratio = Configuration.EngineSettings.MiniMapSizeRatio;
            int w = (int)(ratio * GraphicsDevice.PresentationParameters.BackBufferWidth);
            int h = (int)(ratio * GraphicsDevice.PresentationParameters.BackBufferHeight);
            MiniMapEffect.RenderTarget = new RenderTarget2D(GraphicsDevice, w, h, 1, SurfaceFormat.Color, MultiSampleType.None, 0);
            MiniMapEffect.DepthStencilBuffer = new DepthStencilBuffer(GraphicsDevice, w, h, DepthFormat.Depth24Stencil8, MultiSampleType.None, 0);

            // Look up the resolution and format of our main backbuffer.
            PresentationParameters pp = GraphicsDevice.PresentationParameters;

            int width = GraphicsDevice.PresentationParameters.BackBufferWidth;
            int height = GraphicsDevice.PresentationParameters.BackBufferHeight;

            SurfaceFormat format = GraphicsDevice.PresentationParameters.BackBufferFormat;
            SharedResolveTarget = new Texture2D(GraphicsDevice, width, height, 1, ResourceUsage.ResolveTarget, format, ResourceManagementMode.Manual);
            GaussEffect.ResolveTexture = new Texture2D(GraphicsDevice, width, height, 1, ResourceUsage.ResolveTarget, format, ResourceManagementMode.Manual);

            if (Configuration.EngineSettings.MotionBlur)
            {
                MotionBlurEffect.ResolveTarget = SharedResolveTarget;
            }

            if (Configuration.EngineSettings.Bloom)
            {
                BloomEffect.ResolveTarget = SharedResolveTarget;

                // Create two rendertargets for the bloom processing. These are half the
                // size of the backbuffer, in order to minimize fillrate costs. Reducing
                // the resolution in this way doesn't hurt quality, because we are going
                // to be blurring the bloom images in any case.
                width /= 2;
                height /= 2;

                BloomEffect.RenderTargets[0] = new RenderTarget2D(GraphicsDevice, width, height, 1, format);
                BloomEffect.RenderTargets[1] = new RenderTarget2D(GraphicsDevice, width, height, 1, format);
            }

            int envMapSize = Settings.Configuration.EngineSettings.EnvironmentMapResolution;

            ChromeEffect.RenderTarget = new RenderTargetCube(GraphicsDevice, envMapSize, 0, SurfaceFormat.Color, MultiSampleType.None, 0);
            ChromeEffect.DepthStencilBuffer = new DepthStencilBuffer(GraphicsDevice, envMapSize, envMapSize, DepthFormat.Depth24Stencil8, MultiSampleType.None, 0);

            MirrorEffect.RenderTarget = new RenderTarget2D(GraphicsDevice, MirrorEffect.Width, MirrorEffect.Height, 1, SurfaceFormat.Color, MultiSampleType.None, 0);
            MirrorEffect.DepthStencilBuffer = new DepthStencilBuffer(GraphicsDevice, MirrorEffect.Width, MirrorEffect.Height, DepthFormat.Depth24Stencil8, MultiSampleType.None, 0);

            if (Configuration.EngineSettings.ShieldDistortion)
            {
                width = GraphicsDevice.PresentationParameters.BackBufferWidth;
                height = GraphicsDevice.PresentationParameters.BackBufferHeight;
                ShieldDistortionEffect.ResolveTarget = SharedResolveTarget;
                ShieldDistortionEffect.DepthStencilBuffer = new DepthStencilBuffer(GraphicsDevice, width, height, DepthFormat.Depth24Stencil8, MultiSampleType.None, 0);
                ShieldDistortionEffect.DistortionMap = new RenderTarget2D(GraphicsDevice, width, height, 1, SurfaceFormat.Color, MultiSampleType.None, 0);
            }

            OldCameraEffect.ResolveTarget = SharedResolveTarget;

            screenhotTex = SharedResolveTarget;

            if (Configuration.EngineSettings.RenderParticleEngine)
            {
                width = GraphicsDevice.PresentationParameters.BackBufferWidth;
                height = GraphicsDevice.PresentationParameters.BackBufferHeight;
            }

            if (Configuration.EngineSettings.LensFlare)
            {
                LensFlareEffect.OcclusionTarget = new RenderTarget2D(GraphicsDevice, 16, 16, 1, SurfaceFormat.Color, MultiSampleType.None, 0);
                LensFlareEffect.AlphaTarget = new RenderTarget2D(GraphicsDevice, 1, 1, 1, SurfaceFormat.Color, MultiSampleType.None, 0);
                LensFlareEffect.DepthStencil = new DepthStencilBuffer(GraphicsDevice, 16, 16, DepthFormat.Depth24Stencil8, MultiSampleType.None, 0);
                LensFlareEffect.ResolveTarget = SharedResolveTarget;
            }
        }

        public void register(BaseEntity entity)
        {
            entity.Destroyed += new EventHandler<EntityEventArgs>(removeEntity);
            World.Instance.Track.SceneGraph.AddObject(entity);
        }

        protected void removeEntity(object sender, EntityEventArgs e)
        {
            World.Instance.Track.SceneGraph.RemoveObject(e.Entity, e.Entity.CurrentSegment);
        }
    }
}
