using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace PraktWS0708.Utils
{
    /// <summary>
    /// this class holds various functions to compute splines
    /// </summary>
    public class Splines
    {

        protected static float threshold=0.7f; //determines how accurate the spline is computed
        protected static int maxSplineCount = 512;  //
        protected static int minChanges = 8;

        public struct ControlPoint
        {
            public Vector3 position;
            public float radius;
            public ControlPoint(Vector3 pos, float radius)
            {
                this.radius = radius;
                this.position = pos;
            }
        }

        public struct SplineNode
        {
            public Vector3 position;
            public float radius;
            public float relPos;
            public SplineNode(Vector3 pos, float radius, float relPos)
            {
                this.radius = radius;
                this.position = pos;
                this.relPos = relPos;
            }
            public SplineNode(ControlPoint cp)
            {
                this.position = cp.position;
                this.radius = cp.radius;

                this.relPos = 0;
            }
        }
        /// <summary>
        /// this structure represents a tangentframe
        /// </summary>
        public struct PositionTangentUpvector
        {
            public Vector3 Position;
            public Vector3 Up;
            public Vector3 Tangent;
            public float relPos;
            public PositionTangentUpvector(Vector3 pos, Vector3 up, Vector3 tangent, float relPos)
            {
                this.Position = pos;
                this.Up = up;
                this.Tangent = tangent;
                this.relPos = relPos;
            }
            public PositionTangentUpvector(PositionTangentUpvector v)
            {
                this.Position = v.Position;
                this.Up = v.Up;
                this.Tangent = v.Tangent;
                this.relPos = v.relPos;
            }
        };


        /// <summary>
        /// adds a radius to a tangentframe 
        /// </summary>
        public class PositionTangentUpRadius
        {
            public Vector3 Position;
            public Vector3 Up;
            public Vector3 Tangent;
            public float relPos;
            public float Radius;
            public PositionTangentUpRadius(Vector3 pos, Vector3 up, Vector3 tangent, float radius, float relPos)
            {
                this.Position = pos;
                this.Up = up;
                this.Tangent = tangent;
                this.Radius = radius;
                this.relPos = relPos;
            }
            public PositionTangentUpRadius(PositionTangentUpRadius v)
            {
                this.Position = v.Position;
                this.Up = v.Up;
                this.Tangent = v.Tangent;
                this.Radius = v.Radius;
                this.relPos = v.relPos;
            }
            public PositionTangentUpRadius(ControlPoint c)
            {
                this.Position = c.position;
                this.Up = new Vector3();
                this.Tangent = new Vector3();
                this.Radius = c.radius;
                this.relPos = 0;
            }
            public PositionTangentUpRadius(Vector3 pos,float radius)
            {
                this.Position = pos;
                this.Up = new Vector3();
                this.Tangent = new Vector3();
                this.Radius = radius;
                this.relPos = 0;
            }
        };



        public static float BuildTFList(ControlPoint[] cp, ref List<PositionTangentUpRadius> spline,int segments)
        {
            foreach (ControlPoint c in cp)
                spline.Add(new PositionTangentUpRadius(c));
            float length=LengthOfSpline(ref spline);
            CatmullRomFixed(spline, segments);
            ComputeTangentFrames(ref spline);
            for (int i = 1; i < spline.Count - 1; i++)//validate radii
                Splines.validateRadius((spline[i]), (spline[i + 1]));
            CatmullRomAddaptive(ref spline, 1, 2, threshold);
            return length;
        }

        
       
        protected static void CatmullRomAddaptive(ref List<PositionTangentUpRadius> spline,
                                                  int current,int next,double threshold)
        {
            float diff;
            int changes = int.MaxValue;
            //int divisions=0;
            PositionTangentUpRadius v;
            //punkt auf spline berechnen
            while (spline.Count<maxSplineCount&&changes>minChanges)
            {
                changes = 0;
                current = 1;
                while (current < spline.Count - 2)
                {
                    v = new PositionTangentUpRadius(Vector3.CatmullRom(spline[current - 1].Position,
                                                                       spline[current + 0].Position,
                                                                       spline[current + 1].Position,
                                                                       spline[current + 2].Position, 0.5f),
                                                    OurMath.Slerp(spline[current + 0].Up,spline[current + 1].Up,0.5f),
                                                    OurMath.Slerp(spline[current + 0].Tangent, spline[current + 1].Tangent, 0.5f),
                                                    MathHelper.Lerp(spline[current + 0].Radius, spline[current + 1].Radius, 0.5f),
                                                    spline[current].relPos + ((spline[current + 1].relPos - spline[current].relPos) * 0.5f));
                    //abstand von geschätztem zu echtem punkt
                    diff = ((spline[current].Position + (spline[current].Position - spline[current + 1].Position) * 0.5f) - v.Position).Length();
                    //ist der abstand unterhald der grenze, oder die maximale rekursionstiefe(4) erreicht -> abbruch
                    if ((diff > threshold)) { spline.Insert(current + 1, v); current += 2; changes++; }
                    current++;
                }
            }
        }

        

        protected static void CatmullRomFixed(List<PositionTangentUpRadius> spline, int segments)
        {
            spline.Add(new PositionTangentUpRadius(spline[1]));
            spline.Insert(0, new PositionTangentUpRadius(spline[spline.Count - 3]));
            List<PositionTangentUpRadius> cp = new List<PositionTangentUpRadius>();
            cp.AddRange(spline);
            spline.Clear();
            float lengthOfSegment = 1.0f /((float) segments);
            int currentSegment=1;
            float ifactor;
            for (int i = 0; i < segments; i++)
            {
                if (lengthOfSegment * i > cp[currentSegment + 1].relPos) currentSegment++;
                ifactor = ((lengthOfSegment * i) - cp[currentSegment].relPos) / (cp[currentSegment+1].relPos - cp[currentSegment].relPos);
                spline.Add(new PositionTangentUpRadius(Vector3.CatmullRom(cp[currentSegment - 1].Position,
                                                                                          cp[currentSegment + 0].Position,
                                                                                          cp[currentSegment + 1].Position,
                                                                                          cp[currentSegment + 2].Position, ifactor),
                                              new Vector3(), new Vector3(),
                                              MathHelper.Lerp(cp[currentSegment].Radius, cp[currentSegment+1].Radius, ifactor),
                                              lengthOfSegment * i));

            }
            spline.Add(new PositionTangentUpRadius(spline[0]));
            spline[spline.Count - 1].relPos = 1;
        }

        protected static float LengthOfSpline(ref List<PositionTangentUpRadius> spline)
        {
            spline.Add(new PositionTangentUpRadius(spline[1]));
            spline.Insert(0, new PositionTangentUpRadius(spline[spline.Count - 3]));
            float totalLength=0, currentLength=0;

            for (int i = 1; i < (spline.Count - 2); i++)
            {
                spline[i+1].relPos = LengthOfSplinePart(spline[i - 1].Position, 
                                                      spline[i + 0].Position, 
                                                      spline[i + 1].Position, 
                                                      spline[i + 2].Position);
                totalLength += spline[i].relPos;
            }
            totalLength += spline[spline.Count - 2].relPos;
            for (int i = 1; i < (spline.Count - 2); i++)
            {
                currentLength += spline[i].relPos;
                spline[i].relPos = currentLength / totalLength;
            }

            
            spline[1].relPos = 0;
            spline[spline.Count - 2].relPos = 1;
            spline.RemoveAt(0); spline.RemoveAt(spline.Count - 1);
            return totalLength;
        }

        /// <summary>
        /// approximates the length of a given spline 
        /// </summary>
        protected static float LengthOfSplinePart(Vector3 v0, Vector3 v1, Vector3 v2, Vector3 v3)
        {
            int steps = 100, index = 1;
            float length = 0;
            Vector3[] pointsOnSpline = new Vector3[2];
            pointsOnSpline[index] = v1;
            for (int i = 1; i <= steps; i++)
            {
                index = i % 2;
                pointsOnSpline[index] = Vector3.CatmullRom(v0, v1, v2, v3, i / steps);
                length += (pointsOnSpline[1] - pointsOnSpline[0]).Length();
            }
            return length;
        }

       

        /// <summary>
        /// computes tangent frames from a spline
        /// </summary>
        /// <param name="spline">the spline</param>
        /// <param name="target">the resulting list of tangent frames</param>
        /// <param name="close"></param>
        protected static void ComputeTangentFrames(ref List<PositionTangentUpRadius> spline)
        {
            Vector3 yaxis = new Vector3(0, 1, 0);
            Vector3 xaxis = new Vector3(1, 0, 0);
            spline.Add(new PositionTangentUpRadius(spline[1]));
            spline.Insert(0, new PositionTangentUpRadius(spline[spline.Count - 3]));
           
            float dot;
            spline[0].Tangent =spline[1].Position- spline[0].Position;
            dot = Vector3.Dot(xaxis, spline[0].Tangent);
            spline[0].Up = Vector3.Normalize(Vector3.Cross(spline[0].Tangent, Vector3.Cross(yaxis * Math.Sign(dot), spline[0].Tangent)));
            for (int i = 1; i < spline.Count - 1; i++)
            {
                //tangente aus nachbarpunkten
                spline[i].Tangent = Vector3.Normalize(spline[i + 1].Position - spline[i - 1].Position);
                //up vektor
                spline[i].Up = Vector3.Normalize(Vector3.Cross(spline[i].Tangent, Vector3.Cross(spline[i - 1].Up, spline[i-1].Tangent)));
            }


            spline.RemoveAt(0); spline.RemoveAt(spline.Count - 1);
            spline.RemoveAt(spline.Count - 1);
            spline.Add(new PositionTangentUpRadius(spline[0]));
            spline[spline.Count-1].relPos = 1;
            
                
        }


      

        /// <summary>
        /// checks whether or not the given radii are to large for curvature of the given tangent frames to produce a valid cylinder (without overlapping)
        /// if they are, the radii are reduced accordingly 
        /// </summary>
        /// <param name="f1">first tangent frame</param>
        /// <param name="f2">second tangent frame</param>
        protected static void validateRadius(PositionTangentUpRadius f1, PositionTangentUpRadius f2)
        {
            //strecke zwischen den punkten 
            Vector3 a = f2.Position - f1.Position;
            float lengtha = a.Length();
            a = a / lengtha;
            //winkel zwischen a und den senkrechten auf den tangenten (sin(x-90°)=cos(x)) 
            float sinalpha = Vector3.Dot(f1.Tangent, a);
            float sinbeta = Vector3.Dot(f2.Tangent, a);
            //rechter winkel -> keine probleme :)
            if (OurMath.FLOATZERO(sinalpha) || OurMath.FLOATZERO(sinbeta)) return;
            if (OurMath.FLOATEQ(1,sinalpha) || OurMath.FLOATEQ(1,sinbeta)) return;
            //lustige trigonometrie zur berechnung der dreiecksseiten
            float tanalpha = (float)Math.Tan(Math.Asin(sinalpha));
            float tanbeta = (float)Math.Tan(Math.Asin(sinbeta));
            float lengtha2 = lengtha / ((tanbeta / tanalpha) + 1);
            float lengtha1 = lengtha - lengtha2;
            float r1=((lengtha1 * tanalpha) / sinalpha);
            float r2=((lengtha2 * tanbeta) / sinbeta);
            if (r1 < f1.Radius) 
                f1.Radius = r1;
            if (r2 < f2.Radius)
                f2.Radius = r2;
        }
    }
}
