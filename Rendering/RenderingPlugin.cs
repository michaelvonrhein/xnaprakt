using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using PraktWS0708.Entities;
using PraktWS0708.Utils;
using PraktWS0708.Rendering.Effects;
using PraktWS0708.Settings;

namespace PraktWS0708.Rendering
{
    public class RenderingPlugin : EntityBehaviourPlugin
    {
        public TextureCube EnvironmentMap;
        private Camera[] environmentCamera = new Camera[6];
        private int environmentFace = 0;

        public enum RenderingPluginType
        {
            MainRenderer,
            ParticleSystem,
            ParticleSystemStatic
        }

        public struct RenderingPluginData
        {
            public VertexDeclaration VertexDeclaration;
            public VertexBuffer VertexBuffer;
            public BaseMeshPart[] BaseMeshParts;
            public Matrix Orientation;

            public RenderingPluginData(VertexDeclaration vertexDeclaration, VertexBuffer vertexBuffer, BaseMeshPart[] baseMeshParts, Matrix orientation)
            {
                VertexDeclaration = vertexDeclaration;
                VertexBuffer = vertexBuffer;
                BaseMeshParts = baseMeshParts;
                Orientation = orientation;
            }

            public RenderingPluginData Clone()
            {
                RenderingPluginData d = new RenderingPluginData();
                d.VertexDeclaration = VertexDeclaration;
                d.VertexBuffer = VertexBuffer;
                d.Orientation = Orientation;
                List<BaseMeshPart> parts = new List<BaseMeshPart>();
                foreach (BaseMeshPart p in BaseMeshParts)
                {
                    parts.Add(p.Clone());
                }
                d.BaseMeshParts = parts.ToArray();
                return d;
            }
        }

        public bool Hidden = false;
       
        public BaseEntity Entity;
        public RenderingPluginType Type;
        public RenderingPluginData Data;
        
        protected float distancesq = 0.0f;

        virtual public float DistanceSQ
        {
            get { return distancesq; }
            set { distancesq=value;}
        }

        public RenderingPlugin(BaseEntity entity, RenderingPluginData data, RenderingPluginType type) : base(entity)
        {
            Entity = entity;
            Data = data;
            Type = type;
            foreach (BaseMeshPart b in data.BaseMeshParts)
            {
                b.Entity = entity;
                b.Effect.Orientation = Data.Orientation;
            }
            
            for (int c = 0; c < 6; c++)
            {
                environmentCamera[c] = new Camera();
                environmentCamera[c].Position = Vector3.Zero;
                environmentCamera[c].LookAt = Vector3.Forward;
                environmentCamera[c].Up = Vector3.Up;
                environmentCamera[c].Fov = (float)Math.PI / 2f;
                environmentCamera[c].Aspect = 1f;
                //Entity. World.Instance.Camera.Near;
                environmentCamera[c].Far = World.Instance.Camera.Far;
                environmentCamera[c].Update();
            }

            
            EnvironmentMap = World.Instance.WorldContent.Load<TextureCube>(Settings.Configuration.EngineSettings.TextureDirectory + "BlackCube");
        }

        // Temporary workaround until contentpipeline works
        /*public void LoadModel(Model m)
        {
            this.VertexBuffer = m.Meshes[0].VertexBuffer;
            this.IndexBuffer = m.Meshes[0].IndexBuffer;

            ManagedEffect me = new PhongEffect();
            Texture2D tex = World.Instance.WorldContent.Load<Texture2D>(Configuration.EngineSettings.TextureDirectory + "pencil_p1_diff_v1");

            List<BaseMeshPart> tempParts = new List<BaseMeshPart>();
            foreach (ModelMeshPart p in m.Meshes[0].MeshParts)
            {
                BaseMeshPart b = new BaseMeshPart();
                b.StartIndex = p.StartIndex;
                b.PrimitiveCount = p.PrimitiveCount;
                b.VertexCount = p.NumVertices;
                b.BaseVertex = p.BaseVertex;
                b.Effect = me;
                b.Entity = Entity;
                b.TextureParameters["baseMap"] = tex;
                tempParts.Add(b);

                vertexStride = p.VertexStride;
                vertexDeclaration = p.VertexDeclaration;

                b.Entity.Scale = 0.0005f;
            }
            MeshParts = tempParts.ToArray();
        }*/

        public virtual void Draw()
        {
            if (!Hidden)
            {
                GraphicsDevice g = RenderManager.Instance.GraphicsDevice;
                g.VertexDeclaration = Data.VertexDeclaration;
                g.Vertices[0].SetSource(Data.VertexBuffer, 0, Data.VertexDeclaration.GetVertexStrideSize(0));
                foreach (BaseMeshPart p in Data.BaseMeshParts)
                {
                    p.Draw();
                }
            }
        }

        public virtual void DrawGeometry()
        {
            if (!Hidden)
            {
                GraphicsDevice g = RenderManager.Instance.GraphicsDevice;
                g.VertexDeclaration = Data.VertexDeclaration;
                g.Vertices[0].SetSource(Data.VertexBuffer, 0, Data.VertexDeclaration.GetVertexStrideSize(0));
                foreach (BaseMeshPart p in Data.BaseMeshParts)
                {
                    p.DrawGeometry();
                }
            }
        }

        public void GenerateEnvironmentMap()
        {
            GraphicsDevice d = RenderManager.Instance.GraphicsDevice;
            DepthStencilBuffer backDepthBuffer = d.DepthStencilBuffer;

            Camera worldCamera = World.Instance.Camera;

            environmentCamera[1].LookAt = Entity.Position + Entity.WorldMatrix.Right;
            environmentCamera[0].LookAt = Entity.Position + Entity.WorldMatrix.Left;
            environmentCamera[2].LookAt = Entity.Position + Entity.WorldMatrix.Up;
            environmentCamera[3].LookAt = Entity.Position + Entity.WorldMatrix.Down;
            environmentCamera[4].LookAt = Entity.Position + Entity.WorldMatrix.Backward;
            environmentCamera[5].LookAt = Entity.Position + Entity.WorldMatrix.Forward;

            environmentCamera[0].Up = Entity.WorldMatrix.Up;
            environmentCamera[1].Up = Entity.WorldMatrix.Up;
            environmentCamera[2].Up = Entity.WorldMatrix.Forward;
            environmentCamera[3].Up = Entity.WorldMatrix.Backward;
            environmentCamera[4].Up = Entity.WorldMatrix.Up;
            environmentCamera[5].Up = Entity.WorldMatrix.Up;

            RenderManager.EffectQuality effQual = Configuration.EngineSettings.MaxEffectQuality;
            Configuration.EngineSettings.MaxEffectQuality = RenderManager.EffectQuality.Low;

            for (int c = 0; c < 4; c += 3)
            {
                int face = environmentFace + c;

                environmentCamera[face].Near = ((Physics.RigidBody)Entity.PhysicsPlugin).BoundingSphere.Radius;
                environmentCamera[face].Position = Entity.Position;
                environmentCamera[face].Update();
                

                World w = World.Instance;
                SceneGraph s = w.Track.SceneGraph;

                World.Instance.Camera = environmentCamera[face];

                s.MarkVisibleNodes(World.Instance.Camera);
                RenderManager.Instance.ShadowMapEffect.GenerateShadowMaps(ShadowMapEffect.QUALITY.LOW);
                d.SetRenderTarget(0, ChromeEffect.RenderTarget, (CubeMapFace)face);
                d.DepthStencilBuffer = ChromeEffect.DepthStencilBuffer;
                d.Clear(Color.Black);
                w.gameEnvironment.Draw();

                for (int i = 0; i < s.VisibleSegments.Length; i++)
                {
                    w.Track.Draw();
                    for (int j = 0; j < s.VisibleSegments[i].Objects.Count; j++)
                    {
                        s.VisibleSegments[i].Objects[j].RenderingPlugin.Draw();
                    }
                }
            }

            environmentFace = (environmentFace + 1) % 3;

            d.ResolveRenderTarget(0);
            EnvironmentMap = ChromeEffect.RenderTarget.GetTexture();
#if WINDOWS
            //EnvironmentMap.Save("EnvironmentCube.dds", ImageFileFormat.Dds);
#endif

            d.SetRenderTarget(0, null);
            d.DepthStencilBuffer = backDepthBuffer;

            Configuration.EngineSettings.MaxEffectQuality = effQual;
            World.Instance.Camera = worldCamera;
        }

        internal static RenderingPlugin GetPlugin(RenderingPluginType renderingPluginType, RenderingPluginData data, BaseEntity result)
        {
            switch (renderingPluginType)
            {
                case RenderingPluginType.MainRenderer:
                    return new RenderingPlugin(result, data, renderingPluginType);
                case RenderingPluginType.ParticleSystem:
                    return new Particles.ParticleSystemPlugin(result, data, renderingPluginType);
                case RenderingPluginType.ParticleSystemStatic:
                    return new Particles.ParticleSystemPlugin(result, data, renderingPluginType);
                default:
                    return new RenderingPlugin(result, data, renderingPluginType);
            }
            
        }
    }

    public class BaseMeshPart
    {
        public Dictionary<string, Texture> TextureParameters = new Dictionary<string, Texture>();
        public Dictionary<string, Matrix> MatrixParameters = new Dictionary<string, Matrix>();
        public Dictionary<string, Vector4> VectorParameters = new Dictionary<string, Vector4>();
        public Dictionary<string, float> FloatParameters = new Dictionary<string, float>();
        public Dictionary<string, bool> BoolParameters = new Dictionary<string, bool>();
        public Dictionary<string, int> IntParameters = new Dictionary<string, int>();

        public int StartIndex;
        public int PrimitiveCount; 

        public BaseEntity Entity;
        public ManagedEffect Effect;

        public virtual void Draw()
        {
            GraphicsDevice g = RenderManager.Instance.GraphicsDevice;

            Effect.Begin(this);
            foreach (EffectPass p in Effect.Passes)
            {
                p.Begin();
                g.DrawPrimitives(PrimitiveType.TriangleList, StartIndex, PrimitiveCount);
                p.End();
            }
            Effect.End();
        }

        public virtual void DrawGeometry()
        {
            RenderManager.Instance.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, StartIndex, PrimitiveCount);
        }

        public BaseMeshPart Clone()
        {
            BaseMeshPart clone = new BaseMeshPart();
            clone.TextureParameters = TextureParameters;
            clone.MatrixParameters = MatrixParameters;
            clone.VectorParameters = VectorParameters;
            clone.FloatParameters = FloatParameters;
            clone.BoolParameters = BoolParameters;
            clone.IntParameters = IntParameters;
            clone.StartIndex = StartIndex;
            clone.PrimitiveCount = PrimitiveCount;
            clone.Entity = Entity;
            clone.Effect = Effect;
            return clone;
        }
    }

    public class BaseIndexedMeshPart:BaseMeshPart
    {
        public IndexBuffer indices;

        public int minVertexIndex;
        public int vertexCount;

        public override void Draw()
        {
            GraphicsDevice g = RenderManager.Instance.GraphicsDevice;

            Effect.Begin(this);
            foreach (EffectPass p in Effect.Passes)
            {
                p.Begin();
                g.Indices = indices;
                g.DrawIndexedPrimitives(PrimitiveType.TriangleList, StartIndex, minVertexIndex, vertexCount, 0, PrimitiveCount);
                p.End();
            }
            Effect.End();
        }

        public override void DrawGeometry()
        {
            RenderManager.Instance.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, StartIndex, PrimitiveCount);
        }

        public void buildIndexbuffer(int[] data)
        {
            indices = new IndexBuffer(RenderManager.Instance.GraphicsDevice, data.Length * sizeof(int), ResourceUsage.WriteOnly, IndexElementSize.ThirtyTwoBits);
            indices.SetData(data);
        }
    }
}
