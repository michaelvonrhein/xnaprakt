using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using PraktWS0708.Entities;
using Microsoft.Xna.Framework;
using PraktWS0708.Settings;
using PraktWS0708.Physics;

namespace PraktWS0708.Rendering
{
    public sealed class MirrorEffect
    {
        public RenderTarget2D RenderTarget;
        public DepthStencilBuffer DepthStencilBuffer;



        private Texture2D texture;
        private int width;
        private int height;

        public Texture2D MirrorTexture
        {
            get { return texture; }
        }

        public int Width
        {
            get { return width; }
            set { width = value; }
        }

        public int Height
        {
            get { return height; }
            set { height = value; }
        }

        public MirrorEffect(int w, int h)
        {
            width = w;
            height = h;
        }

        public void GenerateMirrorMap()
        {
            GraphicsDevice d = RenderManager.Instance.GraphicsDevice;

            DepthStencilBuffer depthBuffer = d.DepthStencilBuffer;

            World w = World.Instance;
            Camera c = w.Camera;

            Vector3 tmpPos = c.Position;
            Vector3 tmpLookAt = c.LookAt;
            float tmpAspect = c.Aspect;

            BaseEntity ship = World.Instance.PlayersShip;

            c.Aspect = width / height;
            if (World.Instance.ViewType == ViewTypes.THIRDPERSON)
            {
                c.Position = ship.Position + ship.Orientation.Backward * ((RigidBody)ship.PhysicsPlugin).BoundingSphere.Radius * 2f;
            }
            else
            {
                c.Position = ship.Position + ship.Orientation.Backward;
            }
            c.LookAt = c.Position + ship.Orientation.Backward;
            c.Update();

            SceneGraph s = w.Track.SceneGraph;
 
            s.MarkVisibleNodes(c);
            RenderManager.Instance.ShadowMapEffect.GenerateShadowMaps(ShadowMapEffect.QUALITY.LOW);
            d.SetRenderTarget(0, RenderTarget);
            d.DepthStencilBuffer = DepthStencilBuffer;
            
            d.Clear(Color.Black);
            w.gameEnvironment.Draw();
            w.Track.Draw();
            for (int i = 0; i < s.VisibleSegments.Length; i++)
            {                
                for (int j = 0; j < s.VisibleSegments[i].Objects.Count; j++)
                {
                    RenderingPlugin r = s.VisibleSegments[i].Objects[j].RenderingPlugin;

                    if (r.Type == RenderingPlugin.RenderingPluginType.MainRenderer || Settings.Configuration.EngineSettings.RenderParticleEngine)
                        r.Draw();
                }
               
            }

            d.ResolveRenderTarget(0);

            texture = RenderTarget.GetTexture();

            d.SetRenderTarget(0, null);
            d.DepthStencilBuffer = depthBuffer;

            c.Aspect = tmpAspect;
            c.Position = tmpPos;
            c.LookAt = tmpLookAt;
            c.Update();
        }
    }
}
