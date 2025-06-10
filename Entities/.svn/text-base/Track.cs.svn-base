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
#endregion

namespace PraktWS0708.Entities
{

    public class TrackGeometry
    {
        public VertexPositionNormalTextureTangent[] geometry;
        public VertexDeclaration vertexDeclaration;
        public VertexBuffer vertexBuffer;
        public TrackMeshPart[] TrackMeshParts;
        public BoundingBox aabb;
        
    }



    /// <summary>
    /// the track.
    /// new and improved!
    /// </summary>
    public class Track
    {
        public enum Trackside
        {
            Front,
            Back
        }

        public TrackGeometry geometry;

        public SceneGraph SceneGraph;
        public Matrix WorldMatrix= Matrix.Identity;
        public Utils.Splines.PositionTangentUpRadius[] TangentFrames;
        public float length;
        public Texture2D lightmap;
        public Texture2D lightdirmap;
        public Texture2D ambiancemap;
        public TrackLayout layout;




        public Track()
        {
            SceneGraph = new SceneGraph(this);
        }
     

        public void Update(GameTime gameTime)
        {
        }

        public void Draw()
        {
//#if DEBUG
            //PerformanceEater last = PerformanceMeter.Instance.PerfomanceEaterChange(PerformanceEater.Rendering);
//#endif

           
            

            //draw all segments of the track
            RenderManager.Instance.GraphicsDevice.VertexDeclaration = geometry.vertexDeclaration;
            RenderManager.Instance.GraphicsDevice.Vertices[0].SetSource(geometry.vertexBuffer, 0, geometry.vertexDeclaration.GetVertexStrideSize(0));
            
            for (int i = 0; i < this.geometry.TrackMeshParts.Length; i++)
                geometry.TrackMeshParts[i].Draw();


//#if DEBUG
              //  PerformanceMeter.Instance.PerfomanceEaterChange(last);
//#endif
        }

        public void DrawGeometry()
        {
//#if DEBUG
            //PerformanceEater last = PerformanceMeter.Instance.PerfomanceEaterChange(PerformanceEater.Rendering);
//#endif

            RenderManager.Instance.GraphicsDevice.VertexDeclaration = geometry.vertexDeclaration;
            RenderManager.Instance.GraphicsDevice.Vertices[0].SetSource(geometry.vertexBuffer, 0, geometry.vertexDeclaration.GetVertexStrideSize(0));
            //draw all segments of the track
            geometry.TrackMeshParts[0].DrawGeometry();


//#if DEBUG
            //PerformanceMeter.Instance.PerfomanceEaterChange(last);
//#endif
        }

        //B3A1c
        /// <summary>
        /// get the tangentframe corresponding to the given relative track position
        /// assumes that each segment of the track has the same length.
        /// </summary>
        /// <param name="relPos">the relative position on the track (0-1)</param>
        /// <returns>a tangentframe</returns>
        public Splines.PositionTangentUpRadius Position(float relPos)
        {
            //the relative length of a single segment
            float lengthofsegement = 1.0f / (TangentFrames.Length - 2);
            //which segment are we in?
            int segment = ((int)Math.Floor(relPos / lengthofsegement)) + 1;
            //to avoid out-of-boundary
            if (segment > TangentFrames.Length - 3) segment = TangentFrames.Length - 3;
            //where are we within the segment? 
            float segmentoffset = relPos % lengthofsegement / lengthofsegement;
            Splines.PositionTangentUpRadius ptu = new Splines.PositionTangentUpRadius(TangentFrames[segment]);
            //interpolate position... 
            ptu.Position = Vector3.CatmullRom(TangentFrames[segment - 1].Position,
                                              TangentFrames[segment - 0].Position,
                                              TangentFrames[segment + 1].Position,
                                              TangentFrames[segment + 2].Position, segmentoffset);
            //...and tangent...
            OurMath.Slerp(TangentFrames[segment].Tangent, TangentFrames[segment + 1].Tangent, segmentoffset, ref ptu.Tangent);
            //...and radius
            MathHelper.Lerp(TangentFrames[segment].Radius, TangentFrames[segment + 1].Radius, segmentoffset);
            return ptu;

        }
    }
    /// <summary>
    /// i hate to rewrite the meshpart class, but i need a slightly altered funktionality. 
    /// 
    /// </summary>
    public class TrackMeshPart
    {
        public Texture2D[] Textures;
        

       

        public TrackGeometry geometry;
        public ManagedTrackEffect Effect;
        public IndexBuffer indices;

        protected int minVertexIndex;
        protected int vertexCount;
        protected int indexOffset;
        public int PrimitiveCount;



        public void AddParameters(string[] texnames)
        {
            Textures = new Texture2D[texnames.Length];
            for( int i=0;i<texnames.Length;i++)
                Textures[i]=World.Instance.WorldContent.Load<Texture2D>(Settings.Configuration.EngineSettings.TextureDirectory + "Track/" + texnames[i]);
           
        }

        public void Draw()
        {
           
            Effect.Begin(this);
            foreach (EffectPass p in Effect.Passes)
            {
                p.Begin();
                RenderManager.Instance.GraphicsDevice.Indices = indices;
                RenderManager.Instance.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, indexOffset, minVertexIndex, vertexCount, 0, PrimitiveCount);
                //RenderManager.Instance.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleStrip, 0, PrimitiveCount);
                p.End();
            }
            Effect.End();
        }

        public void DrawGeometry()
        {
            RenderManager.Instance.GraphicsDevice.Indices = indices;
            //RenderManager.Instance.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleStrip, 0, PrimitiveCount);
            RenderManager.Instance.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, indexOffset, minVertexIndex, vertexCount, 0, PrimitiveCount);
        }


        public void buildIndexbuffer(int[] data)
        {
            int maxVertexIndex=0;
            indices = new IndexBuffer(RenderManager.Instance.GraphicsDevice, data.Length * sizeof(int), ResourceUsage.WriteOnly, IndexElementSize.ThirtyTwoBits);
            indices.SetData(data);
            minVertexIndex = int.MaxValue;
            for (int i = 0; i < data.Length; i++)
            {
                minVertexIndex = ((data[i] < minVertexIndex) ? (data[i]) : (minVertexIndex));
                maxVertexIndex = ((data[i] > maxVertexIndex) ? (data[i]) : (maxVertexIndex));
            }
            indexOffset = 0;
            vertexCount = maxVertexIndex - minVertexIndex;
            PrimitiveCount = data.Length / 3;
        }
    }
}
