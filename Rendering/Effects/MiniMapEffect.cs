using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using PraktWS0708.Entities;
using PraktWS0708.Input;
using Microsoft.Xna.Framework;
using PraktWS0708.Settings;

namespace PraktWS0708.Rendering
{
    public sealed class MiniMapEffect
    {
        public Texture2D MiniMap;
        public RenderTarget2D RenderTarget;
        public DepthStencilBuffer DepthStencilBuffer;

        public Vector4 TrackColor;
        public Vector4 EntityColor;
        public Vector4 PlayerColor;
        public Vector4 EnemyColor;

        public SpriteBatch SpriteBatch;
        public bool UseStaticCamera = false;
        private Effect effect;
        private EffectParameter worldParameter;
        private EffectParameter worldViewProjectionParameter;
        private EffectParameter colorParameter;

        private EffectParameter minZParameter;
        private EffectParameter maxZParameter;
        private EffectParameter outlineParameter;


        private Camera staticCamera;
        private Camera followingCamera;
        
        
        public MiniMapEffect()
        {
            effect = RenderManager.Instance.PersistentContent.Load<Effect>(Settings.Configuration.EngineSettings.ShaderDirectory + "MiniMap");

            worldParameter = effect.Parameters["world"];
            worldViewProjectionParameter = effect.Parameters["worldViewProjection"];
            
            colorParameter = effect.Parameters["color"];
            minZParameter = effect.Parameters["minZ"];
            maxZParameter = effect.Parameters["maxZ"];
            outlineParameter = effect.Parameters["outline"];
            
            TrackColor = new Vector4(0.3f, 0.3f, 0.8f, 1f);
            EntityColor = new Vector4(0.8f, 0.6f, 0.3f, 1f);
            PlayerColor = new Vector4(0.3f, 0.8f, 0.3f, 1f);
            EnemyColor = new Vector4(0.8f, 0.3f, 0.3f, 1f);
            staticCamera = new Camera();
            staticCamera.Position = new Vector3(0f, 0f, 25f);
            staticCamera.LookAt = Vector3.Zero;
            staticCamera.Update();
            followingCamera = new Camera();

            BoundingBox bb = World.Instance.Track.geometry.aabb;
            minZParameter.SetValue(bb.Min.Z);
            maxZParameter.SetValue(bb.Max.Z);
            outlineParameter.SetValue(.05f);
        }

        public void GenerateMiniMap()
        {
            Camera currentCamera = staticCamera;
            if (!UseStaticCamera)
            {
                BoundingBox bb = World.Instance.Track.geometry.aabb;
                Vector3 CameraPosition = World.Instance.PlayersShip.Position;

                followingCamera.Position = new Vector3(CameraPosition.X, CameraPosition.Y, maxZParameter.GetValueSingle() + 1.0f);
                followingCamera.LookAt = new Vector3(CameraPosition.X, CameraPosition.Y, 0f);
                followingCamera.Up = Vector3.Up; //World.Instance.PlayersShip.Orientation.Forward;
                followingCamera.Update();
                currentCamera = followingCamera;
            }
            
            World w = World.Instance;
            Track t = w.Track;
            SceneGraph g = t.SceneGraph;
            GraphicsDevice d = RenderManager.Instance.GraphicsDevice;

            Matrix Projection = Matrix.CreateOrthographic(30.0f, 22.5f, currentCamera.Near, currentCamera.Far);

            DepthStencilBuffer depthBuffer = d.DepthStencilBuffer;

            d.SetRenderTarget(0, RenderTarget);
            d.DepthStencilBuffer = DepthStencilBuffer;
            
            d.Clear(new Color(new Vector4(1f, 1f, 1f, 0.1f)));
            
            effect.Begin();
            colorParameter.SetValue(TrackColor);

            worldParameter.SetValue(Matrix.Identity);
            worldViewProjectionParameter.SetValue(currentCamera.View * Projection);

            /*effect.CurrentTechnique.Passes["MiniMapTrackOutline"].Begin();
            t.DrawGeometry();
            effect.CurrentTechnique.Passes["MiniMapTrackOutline"].End();*/

            effect.CurrentTechnique.Passes["MiniMapTrack"].Begin();
            t.DrawGeometry();
            effect.CurrentTechnique.Passes["MiniMapTrack"].End();

            foreach (TrackSegment s in g.Segments)
            {
                foreach (BaseEntity e in s.Objects)
                {
                    worldParameter.SetValue(e.RenderingPlugin.Data.Orientation * e.WorldMatrix);
                    worldViewProjectionParameter.SetValue(e.RenderingPlugin.Data.Orientation * e.WorldMatrix * currentCamera.View * Projection);

                    bool zomg = false;

                    if (e == w.PlayersShip)
                    {
                        colorParameter.SetValue(PlayerColor);
                        effect.CurrentTechnique.Passes["MiniMapPlayer"].Begin();
                        zomg = e.RenderingPlugin.Hidden;
                        e.RenderingPlugin.Hidden = false;
                    }
                    else if (e.InputPlugin.Type == InputPlugin.InputPluginType.NULL)
                    {
                        colorParameter.SetValue(EntityColor);
                        effect.CurrentTechnique.Passes["MiniMapEntity"].Begin();
                    }
                    else
                    {
                        colorParameter.SetValue(EnemyColor);
                        effect.CurrentTechnique.Passes["MiniMapEntity"].Begin();
                    }
                    
                    
                    e.RenderingPlugin.DrawGeometry();

                    if (e == w.PlayersShip)
                    {
                        effect.CurrentTechnique.Passes["MiniMapPlayer"].End();
                        e.RenderingPlugin.Hidden = zomg;
                    }
                    else
                    {
                        effect.CurrentTechnique.Passes["MiniMapEntity"].End();
                    }

                }
            }
            effect.End();

            d.ResolveRenderTarget(0);
            MiniMap = RenderTarget.GetTexture();
            d.SetRenderTarget(0, null);
            d.DepthStencilBuffer = depthBuffer;
        }

        public void Draw()
        {
            int width = RenderManager.Instance.GraphicsDevice.PresentationParameters.BackBufferWidth;
            int height = RenderManager.Instance.GraphicsDevice.PresentationParameters.BackBufferHeight;
            int mwidth = MiniMap.Width;
            int mheight = MiniMap.Height;
            SpriteBatch.Begin();
            SpriteBatch.Draw(MiniMap, new Vector2(width - mwidth, height - mheight), new Color(255, 255, 255, 255));
            SpriteBatch.End();
        }
    }
}
