using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PraktWS0708.Entities;
using PraktWS0708.Utils;

namespace PraktWS0708.Rendering
{
    public class Sunlight
    {
        protected Vector3 direction;
        protected Vector3 position;
        public Vector3 AmbientColor = new Vector3(0.5f, 0.5f, 0.5f);
        public float nearPlane;
        public float farPlane;
        public Matrix ViewMatrix;
        public Matrix[] ProjectionMatrices;
        public float[] SplitPlanes;
        public float[] maxDepths;
        public float[] minDepths;
        public Texture2D[] ShadowMaps;
        public int size;
        public Vector3 Position
        {
            get { return position; }
            set { }
        }
        public Vector3 Direction
        {
            get { return direction; }
            set
            {
                direction = Vector3.Normalize(value);
                position = direction * (-10000.0f);

            }
        }

        public void update()
        {
            Vector3 up = new Vector3(0, 1, 0);
            if (Vector3.Dot(direction, up) > 0.8) up = new Vector3(0, 0, 1);
            ViewMatrix=Matrix.CreateLookAt(new Vector3(0,0,0),direction,new Vector3(0,1,0));
            BoundingBox box = OurMath.transformAABB(World.Instance.Track.geometry.aabb, ViewMatrix);
            nearPlane = box.Max.Z;
            farPlane = box.Min.Z;
        }

        public void buildProjectionMatrices(MyFrustum frustum, int splits)
        {
            Vector3[] points;
            BoundingBox box;
            BoundingFrustum f;
            ProjectionMatrices = new Matrix[splits];
            SplitPlanes = new float[splits + 1];
            maxDepths = new float[splits];
            minDepths = new float[splits];
            ShadowMaps = new Texture2D[splits];
            for (int i = 0; i <= splits; i++)
                SplitPlanes[i] = OurMath.linlogSplit(i, splits, frustum.Near, frustum.Far, 0.5f);
            for (int i = 0; i < splits; i++)
            {
                f = frustum.getFrustum(SplitPlanes[i], SplitPlanes[i + 1]);
                points = f.GetCorners();
                for (int j = 0; j < points.Length; j++)
                    points[j] = Vector3.Transform(points[j], ViewMatrix);
                box = BoundingBox.CreateFromPoints(points);
                ProjectionMatrices[i] = ViewMatrix * Matrix.CreateOrthographicOffCenter(box.Min.X, box.Max.X, box.Min.Y, box.Max.Y, nearPlane, farPlane);
                maxDepths[i] = box.Max.Z;
                minDepths[i] = box.Min.Z;
            }
        }


        public Sunlight()
        {
            Direction = new Vector3(0, 0, 1);
        }
    }
}
