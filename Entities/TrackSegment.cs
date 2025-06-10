using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using PraktWS0708.Rendering;
using PraktWS0708.Utils;

namespace PraktWS0708.Entities
{
    /// <summary>
    /// this construct describes a single segment of the track
    /// it contains information for the scenegraph and the drawing of the track
    /// </summary>
    public sealed class TrackSegment : IComparable
    {
        public BoundingSphere BoundingSphere;
        public bool Visible;
        public float DistanceSQ;
        //a description where in the vertexbuffer this segments vertices are
        public int StartIndex;
        public int VertexCount;
        public TrackSegment[] DistanceSortedSegments;
        public Spotlight[] DistanceSortedSpotlights;

        public Vector3 Position
        {
            set { }
            get { return BoundingSphere.Center; }
        }

        public List<BaseEntity> Objects;

        public TrackSegment(BoundingSphere sphere, int startIndex, int vertexCount)
        {
            this.Objects = new List<BaseEntity>();
            this.StartIndex = startIndex;
            this.VertexCount = vertexCount;
            // Just using the radius is invalid and results in flicker, added safety margin. Ideally this should somehow
            // incorporate the segment width...
            this.BoundingSphere = new BoundingSphere(sphere.Center, sphere.Radius * 2.5f);
            Visible = true;
            DistanceSQ = 0;
        }

        public void BuildDistanceSortedSegments(List<TrackSegment> allSegments)
        {
            for (int i = 0; i < allSegments.Count; i++ )
            {
                allSegments[i].DistanceSQ = Vector3.DistanceSquared(Position, allSegments[i].Position);
            }
            allSegments.Sort();
            DistanceSortedSegments = allSegments.ToArray();
        }

        public void BuildDistanceSortedSpotlights(List<Spotlight> allSpotlights)
        {
            for (int i = 0; i < allSpotlights.Count; i++)
            {
                allSpotlights[i].DistanceSQ = Vector3.DistanceSquared(Position, allSpotlights[i].Camera.Position);
            }
            allSpotlights.Sort();
            DistanceSortedSpotlights = allSpotlights.ToArray();
        }

        public int CompareTo(object obj)
        {
            return (((TrackSegment)obj).DistanceSQ.CompareTo(DistanceSQ));
        }
    }
}
