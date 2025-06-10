using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using PraktWS0708.Entities;
using Microsoft.Xna.Framework;
using PraktWS0708.Settings;

namespace PraktWS0708.Rendering
{
    public sealed class ShieldDistortionEffect
    {
        public Texture2D ResolveTarget;
        public RenderTarget2D DistortionMap;
        public DepthStencilBuffer DepthStencilBuffer;

        private Effect effect;
        private EffectParameter scene;
        private EffectParameter distortionMapParameter;

        private Effect generationEffect;
        private EffectParameter world;
        private EffectParameter viewProjection;
        private EffectParameter shieldTex;
        private EffectParameter distortionFactor;

        private Model sphere;

        public ShieldDistortionEffect()
        {
            effect = RenderManager.Instance.PersistentContent.Load<Effect>(Settings.Configuration.EngineSettings.ShaderDirectory + "ShieldDistortion");
            effect.CurrentTechnique = effect.Techniques[0];
            scene = effect.Parameters["scene"];
            distortionMapParameter = effect.Parameters["distortionMap"];
            

            generationEffect = RenderManager.Instance.PersistentContent.Load<Effect>(Settings.Configuration.EngineSettings.ShaderDirectory + "ShieldDistortionGen");
            generationEffect.CurrentTechnique = generationEffect.Techniques[0];
            world = generationEffect.Parameters["world"];
            viewProjection = generationEffect.Parameters["viewProjection"];
            distortionFactor = generationEffect.Parameters["distortionFactor"];
            shieldTex = generationEffect.Parameters["shieldTex"];



            shieldTex.SetValue(World.Instance.WorldContent.Load<Texture2D>(Configuration.EngineSettings.TextureDirectory + "shipshield"));

            sphere = World.Instance.WorldContent.Load<Model>(Configuration.EngineSettings.ModelDirectory + "planet");

        }

        public void GenerateDistortionMap(GameTime gameTime)
        {
            World w = World.Instance;
            Track t = w.Track;
            Camera c = w.Camera;
            SceneGraph s = t.SceneGraph;
            GraphicsDevice d = RenderManager.Instance.GraphicsDevice;
            DepthStencilBuffer backDepthBuffer = d.DepthStencilBuffer;

            d.SetRenderTarget(0, DistortionMap);
            d.DepthStencilBuffer = DepthStencilBuffer;

            d.Clear(Color.Gray);

            s.MarkVisibleNodes(c);

            world.SetValue(t.WorldMatrix);
            viewProjection.SetValue(c.ViewProjection);
            generationEffect.Begin();
            generationEffect.CurrentTechnique.Passes["NoDistortionNoCulling"].Begin();

            t.DrawGeometry();

            generationEffect.CurrentTechnique.Passes["NoDistortionNoCulling"].End();

            for (int i = 0; i < s.VisibleSegments.Length; i++)
            {
                for (int j = 0; j < s.VisibleSegments[i].Objects.Count; j++)
                {
                    BaseEntity be = s.VisibleSegments[i].Objects[j];

                    world.SetValue(be.RenderingPlugin.Data.Orientation * be.WorldMatrix);
                    viewProjection.SetValue(c.ViewProjection);
                    generationEffect.CurrentTechnique.Passes["NoDistortionNoCulling"].Begin();
                    be.RenderingPlugin.DrawGeometry();
                    generationEffect.CurrentTechnique.Passes["NoDistortionNoCulling"].End();
                }
            }

            // Rendering Spheres for Shields into Distortion Map

            for (int i = 0; i < s.VisibleSegments.Length; i++)
            {
                for (int j = 0; j < s.VisibleSegments[i].Objects.Count; j++)
                {
                    BaseEntity be = s.VisibleSegments[i].Objects[j];

                    if (be.InputPlugin.Type != Input.InputPlugin.InputPluginType.NULL)
                    {
                        world.SetValue(Matrix.CreateScale(be.PhysicsPlugin.BoundingSphere.Radius / 3f) * Matrix.CreateTranslation(be.Position));
                        if(be.LogicPlugin.IsColliding)
                        {
                            be.LogicPlugin.LastCollisionTime = gameTime.TotalGameTime;
                        }
                        
                        float disFact = 2f - MathHelper.Clamp((float)gameTime.TotalGameTime.TotalSeconds - (float)be.LogicPlugin.LastCollisionTime.TotalSeconds, 0.000001f, 2.0f);

                        disFact /= 2f;

                        distortionFactor.SetValue(disFact * disFact * disFact * disFact);//(disFact == 2f) ? 0f : 0.1f / (disFact * 2f));
                        
                        generationEffect.CurrentTechnique.Passes["Distortion"].Begin();

                        foreach (ModelMesh modelMesh in sphere.Meshes)
                        {
                            foreach (ModelMeshPart meshPart in modelMesh.MeshParts)
                            {
                                if (meshPart.PrimitiveCount > 0)
                                {
                                    d.VertexDeclaration = meshPart.VertexDeclaration;
                                    d.Vertices[0].SetSource(modelMesh.VertexBuffer, meshPart.StreamOffset, meshPart.VertexStride);
                                    d.Indices = modelMesh.IndexBuffer;

                                    d.DrawIndexedPrimitives(PrimitiveType.TriangleList, meshPart.BaseVertex, 0, meshPart.NumVertices, meshPart.StartIndex, meshPart.PrimitiveCount);
                                }
                            }
                        }

                        generationEffect.CurrentTechnique.Passes["Distortion"].End();
                    }
                }
            }


            
            generationEffect.End();
            
            d.ResolveRenderTarget(0);

            d.SetRenderTarget(0, null);
            d.DepthStencilBuffer = backDepthBuffer;

#if WINDOWS
            //DistortionMap.GetTexture().Save("distortion.png", ImageFileFormat.Png);
#endif
            
        }

        public void ApplyShieldDistortion()
        {
            SpriteBatch b = RenderManager.Instance.SpriteBatch;
            GraphicsDevice d = RenderManager.Instance.GraphicsDevice;

            // Save current scene for blur pass
            d.ResolveBackBuffer(ResolveTarget);

            // Just ensure distortion map is valid
            d.Clear (ClearOptions.DepthBuffer | ClearOptions.Target, Color.Black, 0f, 0);

            scene.SetValue(ResolveTarget);
            distortionMapParameter.SetValue(DistortionMap.GetTexture());

            b.Begin(SpriteBlendMode.None, SpriteSortMode.Immediate, SaveStateMode.None);
            effect.Begin();
            effect.CurrentTechnique.Passes[0].Begin();
            b.Draw(ResolveTarget, Vector2.Zero, Color.White);
            b.End();
            effect.CurrentTechnique.Passes[0].End();
            effect.End();
        }
    }
}
