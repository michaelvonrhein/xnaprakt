#if DEBUG

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PraktWS0708.Physics
{
    public class Debug
    {
        private struct DrawableVector
        {
            public Vector3 m_vPosition;
            public Vector3 m_vVector;

            public DrawableVector(Vector3 vPosition, Vector3 vVector)
            {
                m_vPosition = vPosition;
                m_vVector = vVector;
            }
        }

        private static VertexPositionColor[] m_VertexPositionColor;
        private static VertexBuffer m_VertexBuffer;
        private static VertexDeclaration m_VertexDeclaration;
        private static GraphicsDevice m_GraphicsDevice;
        private static BasicEffect m_Effect;

        private static SortedList<int, DrawableVector> m_vVectors = new SortedList<int,DrawableVector>();

        /// <summary>
        /// Add drawable Vector3 to list and actualize VertexBuffer
        /// </summary>
        /// <param name="iID">Identification of item</param>
        /// <param name="vPosition">Position of vector</param>
        /// <param name="vVector">Vector</param>
        public static void AddLoad(int iID, Vector3 vPosition, Vector3 vVector)
        {
            Add(iID, vPosition, vVector);
            Load();
        }

        /// <summary>
        /// Add drawable Vector3 to list
        /// </summary>
        /// <param name="iID">Identification of item</param>
        /// <param name="vPosition">Position of vector</param>
        /// <param name="vVector">Vector</param>
        public static void Add(int iID, Vector3 vPosition, Vector3 vVector)
        {
            m_vVectors.Add(iID, new DrawableVector(vPosition, vVector));
        }

        /// <summary>
        /// Actualize VertexBuffer
        /// </summary>
        public static void Load()
        {
            if (m_vVectors.Count > 0)
            {
                m_VertexPositionColor = new VertexPositionColor[m_vVectors.Count * 2];
                for (int iIndex = 0; iIndex < m_vVectors.Count; iIndex++)
                {
                    VertexPositionColor oStart = new VertexPositionColor(m_vVectors[iIndex].m_vPosition, Color.Blue);
                    VertexPositionColor oEnd = new VertexPositionColor(m_vVectors[iIndex].m_vPosition + m_vVectors[iIndex].m_vVector, Color.Red);
                    m_VertexPositionColor[iIndex * 2] = oStart;
                    m_VertexPositionColor[iIndex * 2 + 1] = oEnd;
                }

                m_VertexBuffer = new VertexBuffer(m_GraphicsDevice, m_VertexPositionColor.Length * VertexPositionColor.SizeInBytes, ResourceUsage.None, ResourceManagementMode.Automatic);

                m_VertexBuffer.SetData<VertexPositionColor>(m_VertexPositionColor);
            }
        }

        /// <summary>
        /// Initialize the DrawableVectors
        /// </summary>
        /// <param name="oGraphicsDevice">Current graphics device</param>
        public static void Initialize(GraphicsDevice oGraphicsDevice)
        {
            m_GraphicsDevice = oGraphicsDevice;
            m_VertexDeclaration = new VertexDeclaration(m_GraphicsDevice, VertexPositionColor.VertexElements);
            m_Effect = new BasicEffect(m_GraphicsDevice, new EffectPool());
            m_vVectors.Clear();
        }

        /// <summary>
        /// Draw the vectors
        /// </summary>
        public static void Draw()
        {
            if (m_vVectors.Count > 0)
            {
                m_Effect.World = Entities.World.Instance.Track.WorldMatrix;
                m_Effect.View = Entities.World.Instance.Camera.View;
                m_Effect.Projection = Entities.World.Instance.Camera.Projection;
                m_Effect.VertexColorEnabled = true;

                m_Effect.CommitChanges();

                m_Effect.Begin();

                m_Effect.Techniques[0].Passes[0].Begin();

                m_GraphicsDevice.VertexDeclaration = m_VertexDeclaration;
                m_GraphicsDevice.Vertices[0].SetSource(m_VertexBuffer, 0, VertexPositionColor.SizeInBytes);
                m_GraphicsDevice.DrawPrimitives(PrimitiveType.LineList, 0, m_vVectors.Count);

                m_Effect.Techniques[0].Passes[0].End();

                m_Effect.End();
            }
        }

        /// <summary>
        /// get a new ID
        /// </summary>
        /// <returns>unused identifier</returns>
        public static int GetID()
        {
            int iID = m_vVectors.Count;
            while (m_vVectors.ContainsKey(iID))
            {
                iID++;
            }
            return iID;
        }
    }
}

#endif