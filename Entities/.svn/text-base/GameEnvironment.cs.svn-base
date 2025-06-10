#region Using Statements
using System;
using System.Collections.Generic;
using System.Text;
using PraktWS0708.Utils;
using PraktWS0708.Rendering;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using PraktWS0708.Rendering.Effects;
using PraktWS0708.Settings;
#endregion

namespace PraktWS0708.Entities
{
    public class SkyBox
    {
        #region Fields

        float m_Size;

        public float Size
        {
            get { return m_Size; }
            set { m_Size = value; createGeometry(); }
        }

        SkyBoxEffect m_Effect;

        private VertexPositionNormalTexture[] vertices;

        #endregion

        #region Initialisation

        public SkyBox(float size)
        {
            m_Size = size;
            createGeometry();
            m_Effect = new SkyBoxEffect();         
        }

        private void createGeometry()
        {
            float halfSize = m_Size / 2.0f;
            Vector3[] edges = new Vector3[8];
            edges[0] = new Vector3(-halfSize, -halfSize, -halfSize);
            edges[1] = new Vector3(halfSize, -halfSize, -halfSize);
            edges[2] = new Vector3(halfSize, halfSize, -halfSize);
            edges[3] = new Vector3(-halfSize, halfSize, -halfSize);
            edges[4] = new Vector3(-halfSize, -halfSize, halfSize);
            edges[5] = new Vector3(halfSize, -halfSize, halfSize);
            edges[6] = new Vector3(halfSize, halfSize, halfSize);
            edges[7] = new Vector3(-halfSize, halfSize, halfSize);

            vertices = new VertexPositionNormalTexture[36];
            
            Vector3 normal = new Vector3(0f, 0f, 1f);
            vertices[0] = new VertexPositionNormalTexture(edges[2], normal, new Vector2(1f, 0f));
            vertices[1] = new VertexPositionNormalTexture(edges[1], normal, new Vector2(1f, 1f));
            vertices[2] = new VertexPositionNormalTexture(edges[0], normal, new Vector2(0f, 1f));
            vertices[3] = new VertexPositionNormalTexture(edges[3], normal, new Vector2(0f, 0f)); 
            vertices[4] = new VertexPositionNormalTexture(edges[2], normal, new Vector2(1f, 0f));
            vertices[5] = new VertexPositionNormalTexture(edges[0], normal, new Vector2(0f, 1f));

            normal = new Vector3(-1f, 0f, 0f);
            vertices[6] = new VertexPositionNormalTexture(edges[5], normal, new Vector2(1f, 0f));
            vertices[7] = new VertexPositionNormalTexture(edges[1], normal, new Vector2(1f, 1f));
            vertices[8] = new VertexPositionNormalTexture(edges[2], normal, new Vector2(0f, 1f));
            vertices[9] = new VertexPositionNormalTexture(edges[6], normal, new Vector2(0f, 0f));
            vertices[10] = new VertexPositionNormalTexture(edges[5], normal, new Vector2(1f, 0f));
            vertices[11] = new VertexPositionNormalTexture(edges[2], normal, new Vector2(0f, 1f));

            normal = new Vector3(0f, -1f, 0f);
            vertices[12] = new VertexPositionNormalTexture(edges[6], normal, new Vector2(1f, 0f));
            vertices[13] = new VertexPositionNormalTexture(edges[2], normal, new Vector2(1f, 1f));
            vertices[14] = new VertexPositionNormalTexture(edges[3], normal, new Vector2(0f, 1f));
            vertices[15] = new VertexPositionNormalTexture(edges[7], normal, new Vector2(0f, 0f));
            vertices[16] = new VertexPositionNormalTexture(edges[6], normal, new Vector2(1f, 0f));
            vertices[17] = new VertexPositionNormalTexture(edges[3], normal, new Vector2(0f, 1f));

            normal = new Vector3(1f, 0f, 0f);
            vertices[18] = new VertexPositionNormalTexture(edges[7], normal, new Vector2(1f, 0f));
            vertices[19] = new VertexPositionNormalTexture(edges[3], normal, new Vector2(1f, 1f));
            vertices[20] = new VertexPositionNormalTexture(edges[0], normal, new Vector2(0f, 1f));
            vertices[21] = new VertexPositionNormalTexture(edges[4], normal, new Vector2(0f, 0f));
            vertices[22] = new VertexPositionNormalTexture(edges[7], normal, new Vector2(1f, 0f));
            vertices[23] = new VertexPositionNormalTexture(edges[0], normal, new Vector2(0f, 1f));

            normal = new Vector3(0f, 1f, 0f);
            vertices[24] = new VertexPositionNormalTexture(edges[4], normal, new Vector2(1f, 0f));
            vertices[25] = new VertexPositionNormalTexture(edges[0], normal, new Vector2(1f, 1f));
            vertices[26] = new VertexPositionNormalTexture(edges[1], normal, new Vector2(0f, 1f));
            vertices[27] = new VertexPositionNormalTexture(edges[6], normal, new Vector2(0f, 0f));
            vertices[28] = new VertexPositionNormalTexture(edges[4], normal, new Vector2(1f, 0f));
            vertices[29] = new VertexPositionNormalTexture(edges[1], normal, new Vector2(0f, 1f));            

            normal = new Vector3(0f, 0f, -1f);
            vertices[30] = new VertexPositionNormalTexture(edges[7], normal, new Vector2(1f, 0f));
            vertices[31] = new VertexPositionNormalTexture(edges[4], normal, new Vector2(1f, 1f));
            vertices[32] = new VertexPositionNormalTexture(edges[5], normal, new Vector2(0f, 1f));
            vertices[33] = new VertexPositionNormalTexture(edges[6], normal, new Vector2(0f, 0f));
            vertices[34] = new VertexPositionNormalTexture(edges[7], normal, new Vector2(1f, 0f));
            vertices[35] = new VertexPositionNormalTexture(edges[5], normal, new Vector2(0f, 1f));
            
        }

        #endregion

        #region Update and Draw

        public void Update(GameTime gameTime)
        {
        }

        public void Draw()
        {
//#if DEBUG
            //PerformanceEater last = PerformanceMeter.Instance.PerfomanceEaterChange(PerformanceEater.Rendering);
//#endif

            GraphicsDevice g = RenderManager.Instance.GraphicsDevice;

            m_Effect.Begin();

            g.VertexDeclaration = new VertexDeclaration(g, VertexPositionNormalTexture.VertexElements);
            g.DrawUserPrimitives(PrimitiveType.TriangleList, vertices, 0, 12);

            m_Effect.End();

//#if DEBUG
            //PerformanceMeter.Instance.PerfomanceEaterChange(last);
//#endif
        }

        #endregion
    }

    public class Planet
    {
        #region Fields

        private PlanetEffect m_Effect;
        private Model m_PlanetModel;

        private Vector3 m_Position;
        private float m_Scale;

        private float m_Rotation = 0f;
        private float m_RotationSpeed = MathHelper.PiOver4;

        private float m_OrbitRotation = 0f;
        private float m_OrbitRadius = 0f;
        private float m_OrbitSpeed = 0f;

        public Vector3 Position
        {
            get { return m_Position; }
            set { m_Position = value; }
        }

        public float Scale
        {
            get { return m_Scale; }
            set { m_Scale = value; }
        }

        public float RotationSpeed
        {
            get { return m_RotationSpeed; }
            set { m_RotationSpeed = value; }
        }

        public float OrbitRadius
        {
            get { return m_OrbitRadius; }
            set { m_OrbitRadius = Math.Abs(value); }
        }

        public float OrbitSpeed
        {
            get { return m_OrbitSpeed; }
            set { m_OrbitSpeed = value; }
        }

        public bool ZTestEnable
        {
            get { return m_Effect.ZTestEnable; }
            set { m_Effect.ZTestEnable = value; }
        }
        
        #endregion

        #region Initialisation

        public Planet(Vector3 position, float scale, string texture, string nightTexture)
        {
            m_Position = position;
            m_Scale = scale;
            m_Effect = new PlanetEffect(World.Instance.Sunlight.Direction, new Vector3 (1f, 1f, 1f), texture, nightTexture);
            m_PlanetModel = World.Instance.WorldContent.Load<Model>(Configuration.EngineSettings.ModelDirectory + "planet");
        }

        public Planet(Vector3 position, float scale, float initialOrbitRotation, string texture, string nightTexture)
        {
            m_Position = position;
            m_Scale = scale;
            m_OrbitRotation = initialOrbitRotation;
            m_Effect = new PlanetEffect(World.Instance.Sunlight.Direction, new Vector3(1f, 1f, 1f), texture, nightTexture);
            m_PlanetModel = World.Instance.WorldContent.Load<Model>(Configuration.EngineSettings.ModelDirectory + "planet");
        }

        #endregion

        #region Update and Draw

        public void Update(GameTime gameTime)
        {
            m_Rotation += (float)gameTime.ElapsedGameTime.TotalSeconds * m_RotationSpeed;
            while (m_Rotation > 2f * (float)Math.PI) m_Rotation -= 2f * (float)Math.PI;
            while (m_Rotation < 0) m_Rotation += 2f * (float)Math.PI;

            m_OrbitRotation += (float)gameTime.ElapsedGameTime.TotalSeconds * m_OrbitSpeed;
            while (m_OrbitRotation > 2f * (float)Math.PI) m_OrbitRotation -= 2f * (float)Math.PI;
            while (m_OrbitRotation < 0) m_OrbitRotation += 2f * (float)Math.PI;

            m_Effect.Worldmatrix = Matrix.CreateScale(m_Scale) * Matrix.CreateRotationY(m_Rotation) * Matrix.CreateTranslation(new Vector3(m_OrbitRadius, 0.0f, 0.0f)) * Matrix.CreateRotationY(m_OrbitRotation) * Matrix.CreateRotationX(MathHelper.PiOver2) * Matrix.CreateTranslation(m_Position);
        }

        public void Draw()
        {
//#if DEBUG
            //PerformanceEater last = PerformanceMeter.Instance.PerfomanceEaterChange(PerformanceEater.Rendering);
//#endif

            GraphicsDevice g = RenderManager.Instance.GraphicsDevice;

            m_Effect.Begin();

            foreach (ModelMesh modelMesh in m_PlanetModel.Meshes)
            {
                foreach (ModelMeshPart meshPart in modelMesh.MeshParts)
                {
                    if (meshPart.PrimitiveCount > 0)
                    {
                        g.VertexDeclaration = meshPart.VertexDeclaration;
                        g.Vertices[0].SetSource(modelMesh.VertexBuffer, meshPart.StreamOffset, meshPart.VertexStride);
                        g.Indices = modelMesh.IndexBuffer;

                        g.DrawIndexedPrimitives(PrimitiveType.TriangleList, meshPart.BaseVertex, 0, meshPart.NumVertices, meshPart.StartIndex, meshPart.PrimitiveCount);
                    }
                }
            }

            m_Effect.End();

//#if DEBUG
            //PerformanceMeter.Instance.PerfomanceEaterChange(last);
//#endif
        }

        #endregion
    }

    public class GameEnvironment
    {

        #region Fields

        SkyBox m_SkyBox;
        Planet m_Earth;
        Planet m_EarthClouds;
        Planet m_Moon;
        Planet m_Mars;

        #endregion

        #region Initialisation
        
        public GameEnvironment()
        {
            m_SkyBox = new SkyBox(1f);

            Vector3 earthPos = new Vector3(10f, 0f, 0f);
            Vector3 marsPos = new Vector3(0f, 0f, 10f);

            m_Earth = new Planet(earthPos, 0.5f, "earth", "earthnight");
            m_Earth.RotationSpeed = (float)Math.PI / 16f;

            m_EarthClouds = new Planet(earthPos, 0.51f, "clouds", "Black");
            m_EarthClouds.RotationSpeed = (float)Math.PI / 24f;
            m_EarthClouds.ZTestEnable = false;

            m_Moon = new Planet(earthPos, 0.1f, "moon", "Black");
            m_Moon.RotationSpeed = 0f;
            m_Moon.OrbitRadius = 6f;
            m_Moon.OrbitSpeed = (float)Math.PI / 32f;

            m_Mars = new Planet(marsPos, 0.4f, MathHelper.PiOver2, "mars", "Black");
            m_Mars.RotationSpeed = -(float)Math.PI / 12f;
            m_Mars.OrbitRadius = 40f;
            m_Mars.OrbitSpeed = (float)Math.PI / 200f;
        }

        #endregion

        #region Update and Draw

        public void Update(GameTime gameTime)
        {
            m_Earth.Update(gameTime);
            m_EarthClouds.Update(gameTime);
            m_Moon.Update(gameTime);
            m_Mars.Update(gameTime);
        }

        public void Draw()
        {
//#if DEBUG
            //PerformanceEater last = PerformanceMeter.Instance.PerfomanceEaterChange(PerformanceEater.Rendering);
//#endif

            GraphicsDevice g = RenderManager.Instance.GraphicsDevice;
            
            m_SkyBox.Draw();
            g.Clear(ClearOptions.DepthBuffer, Color.Black, 1.0f, 0);

            m_Earth.Draw();
            m_EarthClouds.Draw();

            m_Moon.Draw();
            m_Mars.Draw();

            
            g.Clear(ClearOptions.DepthBuffer, Color.Black, 1.0f, 0);

//#if DEBUG
            //PerformanceMeter.Instance.PerfomanceEaterChange(last);
//#endif
        }

        #endregion
    }
}
