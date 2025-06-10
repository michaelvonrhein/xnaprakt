using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Content;
using PraktWS0708.Entities;
using PraktWS0708.Utils;

namespace PraktWS0708.Rendering
{
    public sealed class SceneGraph
    {

        public TrackSegment[] Segments;
        public TrackSegment[] VisibleSegments;
        private Track track;
        public int[] splinePart2Segment;
        public int cameraSegment=0;

        /// <summary>
        /// Node list bauen: funktionen
        /// </summary>
        /// <param name="t"></param>
        public SceneGraph(Track track)
        {
            this.track = track;
        }

        /// <summary>
        /// erzeugt einen neuen ScenenGraphen
        /// </summary>
        /// <param name="geometry">die geometrie des tracks. zur erzeugung der boundingSpheres</param>
        public void BuildSceneGraph(List<VertexPositionNormalTextureTangent> geometry, int segments)
        {
            splinePart2Segment = new int[geometry.Count/segments];
            List<TrackSegment> SegmentList = new List<TrackSegment>();
            int verticesPerSegment = segments +1 ; //two for each segment, two to close
            int nextFirstSegment = 0;
            int indexCurrent = 0, indexLast = 0;
            List<Vector3> pointlist = new List<Vector3>();
            BoundingSphere[] bs = new BoundingSphere[2];
            float[] trackPartVolume = new float[2];
            float[] volumeRatio = new float[2];
            //compute initial state
            for (int j = 0; j < verticesPerSegment*2 - 2; j += 3)//last two are connection
                pointlist.Add(geometry[j].Position);
            trackPartVolume[1] = VolumeOfTrackPart(1, 2);
            bs[1] = BoundingSphere.CreateFromPoints(pointlist);
            volumeRatio[1] = trackPartVolume[1] / OurMath.sphereVolume(bs[1]);
            splinePart2Segment[0] = 0;
            for (int i = 2; i < track.TangentFrames.Length - 3; i++)//first and last tangentframes are duplicates,
            {
                splinePart2Segment[i - 1] = SegmentList.Count;
                indexCurrent = i % 2;
                indexLast = (i - 1) % 2;
                for (int j = 0; j < verticesPerSegment * 2 - 2; j += 3)//last two are connection
                    pointlist.Add(geometry[i * verticesPerSegment + j].Position);
                bs[indexCurrent] = BoundingSphere.CreateFromPoints(pointlist);
                trackPartVolume[indexCurrent] = trackPartVolume[indexLast] + VolumeOfTrackPart(i, i + 1);
                volumeRatio[indexCurrent] = trackPartVolume[indexCurrent] / OurMath.sphereVolume(bs[indexCurrent]);
                
                if (volumeRatio[indexCurrent] < volumeRatio[indexLast])
                {
                    //mit neuem trackteil ist das volumen verhältniss schlechter geworden =>letzter zustand war ideal
                    //segment umfasst also die spline parts nextfirstsegment <-> i-1
                    SegmentList.Add(new TrackSegment(bs[indexLast], nextFirstSegment * verticesPerSegment, ((i - 1) - nextFirstSegment) * verticesPerSegment));
                    nextFirstSegment = i - 1;
                    //reset
                    pointlist.Clear();
                    for (int j = 0; j < verticesPerSegment*2 - 2; j += 3)//last two are connection
                    {
                        pointlist.Add(geometry[i * verticesPerSegment + j].Position);
                    }
                    bs[indexCurrent] = BoundingSphere.CreateFromPoints(pointlist);
                    trackPartVolume[indexCurrent] = VolumeOfTrackPart(i, i + 1);
                    volumeRatio[indexCurrent] = trackPartVolume[indexCurrent] / OurMath.sphereVolume(bs[indexCurrent]);
                }
            }
            //the last segment has still to be created!
            SegmentList.Add(new TrackSegment(bs[indexCurrent], nextFirstSegment * verticesPerSegment, ((track.TangentFrames.Length - 3) - nextFirstSegment) * verticesPerSegment));
            //System.Console.WriteLine("BAM! last Segment created\n\t\t vb begin:{0} vertexcount:{1}",
            //                                nextFirstSegment * verticesPerSegment,
            //                                ((TangentFrames.Length - 3) - nextFirstSegment) * verticesPerSegment);
            splinePart2Segment[(track.TangentFrames.Length - 3)] = SegmentList.Count - 1;
            
            Segments = SegmentList.ToArray();
            for (int i = 0; i < Segments.Length; i++)
            {
                Segments[i].BuildDistanceSortedSegments(SegmentList);
            }
        }

        /// <summary>
        /// berechnet das ungefähre volumen des zylinders der von den beiden tangentframes aufgespannt wird.
        /// </summary>
        /// <param name="t1">index des tangentframes des bodens des zylinders</param>
        /// <param name="t2">index des tangentframes der decke des zylinders</param>
        private float VolumeOfTrackPart(int t1, int t2)
        {
            float avgRadius = (track.TangentFrames[t1].Radius + track.TangentFrames[t2].Radius) * 0.5f;
            return avgRadius * avgRadius * OurMath.PI * (track.TangentFrames[t1].Position - track.TangentFrames[t2].Position).Length();
        }

        /// <summary>
        /// bestimmt welche nodes im aktuellen frustum liegen
        /// </summary>
        public void MarkVisibleNodes(Camera c)
        {
            if (World.Instance.PlayersShip == null)
                return;

            List<TrackSegment> VisibleSegmentsList = new List<TrackSegment>();

            // We use the ship's position instead of the camera to ensure it is always
            // drawn last, otherwise it will become partially transparent
            cameraSegment = EstimateSegmentForPosition(World.Instance.PlayersShip.Position);

            foreach (TrackSegment s in Segments[cameraSegment].DistanceSortedSegments)
            {
                if (c.Visible(s.BoundingSphere))
                    VisibleSegmentsList.Add(s);
            }
            //ensure a snug fit
            if (VisibleSegmentsList.Count > 0)
            {
                c.MaxFar = ((float)Math.Sqrt(VisibleSegmentsList[0].DistanceSQ) + VisibleSegmentsList[0].BoundingSphere.Radius) * 0.6f;
            }
            VisibleSegments = VisibleSegmentsList.ToArray();
        }

        public void AddObject(BaseEntity model, int segmentNumber)
        {
            Segments[segmentNumber].Objects.Add(model);
        }
        public void RemoveObject(BaseEntity model, int segmentNumber)
        {
            Segments[segmentNumber].Objects.Remove(model);
        }

        public void AddObject(BaseEntity model)
        {
            model.CurrentSegment=EstimateSegmentForPosition(model.Position);
        }
        public void RemoveObject(BaseEntity model)
        {
            Segments[EstimateSegmentForPosition(model.Position)].Objects.Remove(model);
        }
        public int countRegisteredObjects()
        {
            int c=0;
            foreach (TrackSegment s in Segments)
            {
                c += s.Objects.Count;
            }
            return c;
        }


        /// <summary>
        /// a very quick and very dirty way to estimate in which segment a point lies. usually used to figure out where on the track the camera currently is.
        /// it would be better to keep track of the camera movements or something like that, but this is sufficient for now
        /// </summary>
        /// <param name="pos">the point you are interessted in</param>
        /// <param name="segnr">the result</param>
        public int EstimateSegmentForPosition(Vector3 pos)
        {
            //it may be over complicated, but it looks kinda cool :)
            //int segnr;
            //for (segnr = 0; (segnr < Segments.Length - 1) && (Segments[segnr].boundingSphere.Contains(pos)) != ContainmentType.Contains; segnr++) ;
            //return segnr;
            //all it does is walking through all segments and check if the distance of the given point to the origin is smaller than the segments radius
            int result = 0;
            float minDist = Vector3.DistanceSquared(pos, Segments[result].Position), curDist;
            for (int i = 2; i < Segments.Length; i++)
            {
                curDist = Vector3.DistanceSquared(pos, Segments[i].Position);
                if (curDist < minDist)
                {
                    minDist = curDist;
                    result = i;
                }
            }
            return result;
        }

        public BoundingSphere GetBoundingSphere()
        {
            float distance = 0.0f;
            foreach(TrackSegment s in Segments)
            {
                if(s.Position.Length() + s.BoundingSphere.Radius > distance)
                {
                    distance = s.Position.Length() + s.BoundingSphere.Radius;
                }
            }
            return new BoundingSphere(Vector3.Zero, distance);
        }
    }
}