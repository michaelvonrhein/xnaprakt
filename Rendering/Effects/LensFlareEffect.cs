using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using PraktWS0708.Settings;
using Microsoft.Xna.Framework;
using PraktWS0708.Entities;

namespace PraktWS0708.Rendering.Effects
{
    public sealed class LensFlareEffect
    {
        public RenderTarget2D OcclusionTarget;
        public RenderTarget2D AlphaTarget;
        public DepthStencilBuffer DepthStencil;
        public Texture2D ResolveTarget;
        public Effect effect;
        public EffectParameter worldViewProjection;
        private EffectParameter occlusionTexture;
        private EffectParameter flareTexture;
        private EffectParameter flareColor;
        private Texture2D glowSprite;

        private Camera SunCamera = new Camera();
        private VertexPositionColor[] tri = new VertexPositionColor[6];
        private Matrix triMat;

        // The lensflare effect is made up from several individual flare graphics,
        // which move across the screen depending on the position of the sun. This
        // helper class keeps track of the position, size, and color for each flare.
        class Flare
        {
            public Flare(float position, float scale, Color color, string textureName)
            {
                Position = position;
                Scale = scale;
                Color = color;
                TextureName = textureName;
            }

            public float Position;
            public float Scale;
            public Color Color;
            public string TextureName;
            public Texture2D Texture;
        }

        // Array describes the position, size, color, and texture for each individual
        // flare graphic. The position value lies on a line between the sun and the
        // center of the screen. Zero places a flare directly over the top of the sun,
        // one is exactly in the middle of the screen, fractional positions lie in
        // between these two points, while negative values or positions greater than
        // one will move the flares outward toward the edge of the screen. Changing
        // the number of flares, or tweaking their positions and colors, can produce
        // a wide range of different lensflare effects without altering any other code.
        Flare[] flares =
        {
            new Flare(-0.5f, 0.7f, new Color( 50,  25,  50), "flare1"),
            new Flare( 0.3f, 0.4f, new Color(100, 255, 200), "flare1"),
            new Flare( 1.2f, 1.0f, new Color(100,  50,  50), "flare1"),
            new Flare( 1.5f, 1.5f, new Color( 50, 100,  50), "flare1"),

            new Flare(-0.3f, 0.7f, new Color(200,  50,  50), "flare2"),
            new Flare( 0.6f, 0.9f, new Color( 50, 100,  50), "flare2"),
            new Flare( 0.7f, 0.4f, new Color( 50, 200, 200), "flare2"),

            new Flare(-0.7f, 0.7f, new Color( 50, 100,  25), "flare3"),
            new Flare( 0.0f, 0.6f, new Color( 25,  25,  25), "flare3"),
            new Flare( 2.0f, 1.4f, new Color( 50,  100, 200), "flare3"),
        };

        public LensFlareEffect()
        {
            effect = RenderManager.Instance.PersistentContent.Load<Effect>(Settings.Configuration.EngineSettings.ShaderDirectory + "LensFlare");
            worldViewProjection = effect.Parameters["worldViewProjection"];
            occlusionTexture = effect.Parameters["occlusionTexture"];
            flareTexture = effect.Parameters["flareTexture"];
            flareColor = effect.Parameters["flareColor"];

            SunCamera.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4 * 0.5f, 1.0f, 0.1f, 1000f);

            // Load the glow and flare textures.
            glowSprite = RenderManager.Instance.PersistentContent.Load<Texture2D>(Settings.Configuration.EngineSettings.TextureDirectory + "Environment/glow");

            foreach (Flare flare in flares)
            {
                flare.Texture = RenderManager.Instance.PersistentContent.Load<Texture2D>(Settings.Configuration.EngineSettings.TextureDirectory + "Environment/" + flare.TextureName);
            }

            tri[0] = new VertexPositionColor(new Vector3(-10f,  10f, -0.5f), Color.White);
            tri[1] = new VertexPositionColor(new Vector3(-10f, -10f, -0.5f), Color.White);
            tri[2] = new VertexPositionColor(new Vector3( 20f,   0f, -0.5f), Color.White);

            triMat = Matrix.CreateOrthographic(1f, 1f, 0f, 1f);
        }

        public void ApplyLensFlare()
        {
            World w = World.Instance;
            GraphicsDevice d = RenderManager.Instance.GraphicsDevice;
            SpriteBatch b = RenderManager.Instance.SpriteBatch;
            DepthStencilBuffer dsb = d.DepthStencilBuffer;
            d.ResolveBackBuffer(ResolveTarget);
            d.SetRenderTarget(0, OcclusionTarget);
            d.DepthStencilBuffer = DepthStencil;
            d.Clear(Color.White);
            // Camera looks exactly at sun area
            SunCamera.View = Matrix.CreateLookAt(w.Camera.Position, w.Camera.Position - w.Sunlight.Direction, w.Camera.Up);
            SunCamera.ViewProjection = SunCamera.View * SunCamera.Projection;
            SunCamera.frustum = new BoundingFrustum(SunCamera.ViewProjection);

            // Render scene geomety as black
            effect.CurrentTechnique = effect.Techniques[0];

            effect.Begin();
            effect.CurrentTechnique.Passes[0].Begin();

            worldViewProjection.SetValue(SunCamera.ViewProjection);
            effect.CommitChanges();
            w.Track.DrawGeometry();

            w.Track.SceneGraph.MarkVisibleNodes(SunCamera);

            foreach (TrackSegment s in w.Track.SceneGraph.VisibleSegments)
            {
                for (int i=0; i< s.Objects.Count; i++)
                {
                    worldViewProjection.SetValue(s.Objects[i].WorldMatrix * SunCamera.ViewProjection);
                    effect.CommitChanges();
                    s.Objects[i].RenderingPlugin.DrawGeometry();
                }
            }
            effect.CurrentTechnique.Passes[0].End();
            effect.End();

            d.ResolveRenderTarget(0);
            d.SetRenderTarget(0, AlphaTarget);
            d.Clear(Color.Black);
            worldViewProjection.SetValue(triMat);
            occlusionTexture.SetValue(OcclusionTarget.GetTexture());
            effect.Begin();
            effect.CurrentTechnique.Passes[1].Begin();
            d.DrawUserPrimitives(PrimitiveType.TriangleList, tri, 0, 1);
            effect.CurrentTechnique.Passes[1].End();
            effect.End();
            d.ResolveRenderTarget(0);
            d.SetRenderTarget(0, null);
            d.DepthStencilBuffer = dsb;
            
            Viewport vp = d.Viewport;
            Vector3 sunPosition = vp.Project(w.Camera.Position + w.Sunlight.Direction, w.Camera.Projection, w.Camera.View, Matrix.Identity);

            b.Begin(SpriteBlendMode.None, SpriteSortMode.Immediate, SaveStateMode.None);
            b.Draw(ResolveTarget, Vector2.Zero, Color.White);
            b.End();

            // Render flares only if camera looks into the direction of the sun
            if (Math.Acos(Vector3.Dot(Vector3.Normalize(w.Camera.Position - w.Camera.LookAt), w.Sunlight.Direction)) > MathHelper.PiOver2)
            {
                DrawGlow(new Vector2(sunPosition.X, sunPosition.Y));
                DrawFlares(new Vector2(sunPosition.X, sunPosition.Y));
            }
        }

        void DrawGlow(Vector2 lightPosition)
        {
            Vector2 origin = new Vector2(glowSprite.Width, glowSprite.Height) / 2;
            float scale = Configuration.EngineSettings.FlareGlowSize * 2 / glowSprite.Width;

            RenderManager.Instance.SpriteBatch.Begin(SpriteBlendMode.Additive, SpriteSortMode.Immediate, SaveStateMode.None);

            flareTexture.SetValue(glowSprite);
            occlusionTexture.SetValue(AlphaTarget.GetTexture());
            flareColor.SetValue(Color.Wheat.ToVector3()); // This time, wheat is actually intentional
            effect.CurrentTechnique = effect.Techniques[1];
            effect.Begin();
            effect.CurrentTechnique.Passes[0].Begin();

            RenderManager.Instance.SpriteBatch.Draw(glowSprite, lightPosition, null, Color.White, 0, origin, scale, SpriteEffects.None, 0);

            RenderManager.Instance.SpriteBatch.End();

            effect.CurrentTechnique.Passes[0].End();
            effect.End();
        }

        void DrawFlares(Vector2 lightPosition)
        {
            Viewport viewport = RenderManager.Instance.GraphicsDevice.Viewport;

            // Lensflare sprites are positioned at intervals along a line that
            // runs from the 2D light position toward the center of the screen.
            Vector2 screenCenter = new Vector2(viewport.Width, viewport.Height) / 2;

            Vector2 flareVector = screenCenter - lightPosition;

            // Draw the flare sprites using additive blending.
            RenderManager.Instance.SpriteBatch.Begin(SpriteBlendMode.Additive, SpriteSortMode.Immediate, SaveStateMode.None);

            effect.CurrentTechnique = effect.Techniques[1];
            flareTexture.SetValue(glowSprite);
            occlusionTexture.SetValue(AlphaTarget.GetTexture());
            effect.Begin();
            effect.CurrentTechnique.Passes[0].Begin();

            foreach (Flare flare in flares)
            {
                // Compute the position of this flare sprite.
                Vector2 flarePosition = lightPosition + flareVector * flare.Position;

                // Center the sprite texture.
                Vector2 flareOrigin = new Vector2(flare.Texture.Width,
                                                  flare.Texture.Height) / 2;

                flareColor.SetValue(flare.Color.ToVector3() * 3f);
                effect.CommitChanges();

                // Draw the flare.
                RenderManager.Instance.SpriteBatch.Draw(flare.Texture, flarePosition, null, Color.White, 1, flareOrigin, flare.Scale, SpriteEffects.None, 0);
            }

            RenderManager.Instance.SpriteBatch.End();

            effect.CurrentTechnique.Passes[0].End();
            effect.End();
        }
    }
}
