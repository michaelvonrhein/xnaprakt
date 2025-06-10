using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PraktWS0708.Rendering;


namespace PraktWS0708.Utils
{
    /// <summary>
    ///  a collection of various mathematical functions and constants
    /// </summary>
    public class OurMath
    {
        #region Constants
        public static readonly float RAD360 = 6.283185f;
        public static readonly float RAD270 = 4.712389f;
        public static readonly float RAD180 = 3.141593f;
        public static readonly float RAD90 = 1.570796f;
        public static readonly float RAD45 = 0.785398f;
        public static readonly float RAD1 = 0.017450f;
        public static readonly float PI = 3.1415926535898f;
        public static readonly float SQRT3 = 1.73205080756887f;

        public static readonly float FEPS = 0.001f;
        public static readonly double DEPS = 0.00000001;
        #endregion

        #region Methods
        public static float SQ(float a) { return a * a; }
        public static bool FLOATZERO(float f) { return Math.Abs(f) < FEPS; }
        public static bool DOUBLEZERO(double f) { return Math.Abs(f) < DEPS; }
        public static bool FLOATEQ(float f1, float f2) { return Math.Abs(f1 - f2) < FEPS; }
        public static bool DOUBLEEQ(double f1, double f2) { return Math.Abs(f1 - f2) < DEPS; }
        public static float RAD2GRAD(float rad) { return rad * 57.29578f; }
        public static float GRAD2RAD(float grad) { return grad * RAD1; }
        public static bool BETWEEN(float x, float min, float max) { return x >= min && x <= max; }
        public static bool NOTBETWEEN(float x, float min, float max) { return x < min && x > max; }
        public static bool BETWEEN(int x, int min, int max) { return x >= min && x <= max; }
        public static float CLAMP(float x, float min, float max) { return (x < min) ? (min) : ((x > max) ? (max) : (x)); }
        public static bool NOTBETWEEN(int x, int min, int max) { return x < min && x > max; }
        public static float MAX3(float a, float b, float c) { return (a > b && a > c) ? a : ((b > c) ? b : c); }
        public static int MAX3(int a, int b, int c) { return (a > b && a > c) ? a : ((b > c) ? b : c); }
        public static float MAX2(float a, float b) { return (a > b) ? a : b; }
        public static float MIN2(float a, float b) { return (a < b) ? a : b; }
        public static int MAX2(int a, int b) { return (a > b) ? a : b; }
        public static int MIN2(int a, int b) { return (a < b) ? a : b; }
        public static float ms2second(float ms) { return ms / 1000.0f; }



        protected static Random rand = new Random();

        /// <summary>
        /// get a random number
        /// </summary>
        public static float getRandom()
        {
            return (float)rand.NextDouble();
        }


        /// <summary>
        /// create a random color
        /// </summary>
        public static Color randomColor()
        {
            return new Color(new Vector3(getRandom(), getRandom(), getRandom()));
        }

        /// <summary>
        /// determinant of a 2x2 matrix
        /// </summary>
        public static float det2x2(float m11, float m12, float m21, float m22)
        {
            return m11 * m22 - m12 * m21;
        }

        /// <summary>
        /// Spheric linear interpolation
        /// </summary>
        /// <param name="v1">first vector</param>
        /// <param name="v2">second vector</param>
        /// <param name="factor">interpolation factor</param>
        /// <param name="res">the resulting vector</param>
        public static void Slerp(Vector3 v1, Vector3 v2, float factor, ref Vector3 res)
        {
            float v1len = v1.Length();
            float v2len = v2.Length();
            v1 /= v1len;
            v2 /= v2len;
            float cosalpha = Vector3.Dot(v1, v2);
            if (cosalpha < 0.0f)
            {
                cosalpha *= -1.0f;
                Vector3.Negate(v1);
            }
            float f1, f2;
            if (FLOATEQ(cosalpha, 1.0f))
            {
                f1 = 1.0f - factor;
                f2 = factor;
            }
            else
            {
                float sinalpha = (float)Math.Sqrt(1.0f - cosalpha * cosalpha);
                float alpha = (float)Math.Atan2(sinalpha, cosalpha);
                f1 = (float)(Math.Sin((1.0f - factor) * alpha) / sinalpha);
                f2 = (float)(Math.Sin(factor * alpha) / sinalpha);
            }
            res = v1 * f1 + v2 * f2;
            res *= MathHelper.Lerp(v1len, v2len, factor);
        }


        /// <summary>
        /// Spheric linear interpolation
        /// </summary>
        /// <param name="v1">first vector</param>
        /// <param name="v2">second vector</param>
        /// <param name="factor">interpolation factor</param>
        /// <param name="res">the resulting vector</param>
        public static Vector3 Slerp(Vector3 v1, Vector3 v2, float factor )
        {
            Vector3 res;
            float v1len = v1.Length();
            float v2len = v2.Length();
            v1 /= v1len;
            v2 /= v2len;
            float cosalpha = Vector3.Dot(v1, v2);
            if (cosalpha < 0.0f)
            {
                cosalpha *= -1.0f;
                Vector3.Negate(v1);
            }
            float f1, f2;
            if (FLOATEQ(cosalpha, 1.0f))
            {
                f1 = 1.0f - factor;
                f2 = factor;
            }
            else
            {
                float sinalpha = (float)Math.Sqrt(1.0f - cosalpha * cosalpha);
                float alpha = (float)Math.Atan2(sinalpha, cosalpha);
                f1 = (float)(Math.Sin((1.0f - factor) * alpha) / sinalpha);
                f2 = (float)(Math.Sin(factor * alpha) / sinalpha);
            }
            res = v1 * f1 + v2 * f2;
            res *= MathHelper.Lerp(v1len, v2len, factor);
            return res;
        }

        /// <summary>
        /// reflect a vector 
        /// </summary>
        /// <param name="v">the vector to be reflected</param>
        /// <param name="normal"> the normal of the plane, the vector gets reflected from </param>
        /// <param name="reflected"> the result</param>
        public static void reflectionVector(Vector3 v, Vector3 normal, ref Vector3 reflected)
        {
            reflected = (2.0f * (Vector3.Dot(normal, v))) * normal - v;
        }

        /// <summary>
        /// project one vector onto another
        /// </summary>
        /// <param name="v">the vector to be projected </param>
        /// <param name="projectOn">the vector be projected on</param>
        /// <param name="result">the result</param>
        public static void projectVectorOntoVector(Vector3 v, Vector3 projectOn, ref Vector3 result)
        {
            result = (projectOn * (Vector3.Dot(v, projectOn)));
        }

        /// <summary>
        /// project one vector onto another
        /// </summary>
        /// <param name="v">the vector to be projected </param>
        /// <param name="projectOn">the vector be projected on</param>
        public static Vector3 projectVectorOntoVector(Vector3 v, Vector3 projectOn)
        {
            return (projectOn * (Vector3.Dot(v, projectOn)));
        }

        /// <summary>
        /// compute the moment of inertia for an arbitrary rotation axis from an inertia tensor
        /// </summary>
        /// <param name="tensor">the inertia tensor</param>
        /// <param name="axis">the rotation axis</param>
        /// <returns>the resulting moment of inertia</returns>
        public static float momentOfInertiaFromTensor(Matrix tensor, Vector3 axis)
        {
            return (tensor.M11 * axis.X * axis.X +
                            tensor.M12 * axis.X * axis.Y +
                            tensor.M13 * axis.X * axis.Z +
                            tensor.M21 * axis.Y * axis.X +
                            tensor.M22 * axis.Y * axis.Y +
                            tensor.M23 * axis.Y * axis.Z +
                            tensor.M31 * axis.Z * axis.X +
                            tensor.M32 * axis.Z * axis.Y +
                            tensor.M33 * axis.Z * axis.Z);

        }

        public static float sphereVolume(BoundingSphere sphere)
        {
            return sphere.Radius * sphere.Radius * sphere.Radius * PI * (4.0f / 3.0f);
        }

        #endregion

 
        public static float SinNorm(float x)
        {
            return (float)((Math.Sin(x) + 1.0f) * 0.5f);
        }

        /// <summary>
        /// build a coordinate system from a set of vectors
        /// </summary>
        /// <param name="x">X-Axis</param>
        /// <param name="y">Y-Axis</param>
        /// <param name="z">Z-Axis</param>
        /// <returns></returns>
        public static Matrix MakeCoordSystem(Vector3 x, Vector3 y, Vector3 z)
        {
            return new Matrix(x.X, x.Y, x.Z, 0,
                              y.X, y.Y, y.Z, 0,
                              z.X, z.Y, z.Z, 0,
                              0, 0, 0, 1);
        }

        /// <summary>
        /// build a transposed coordinate system from a set of vectors
        /// </summary>
        /// <param name="x">X-Axis</param>
        /// <param name="y">Y-Axis</param>
        /// <param name="z">Z-Axis</param>
        /// <returns></returns>
        public static Matrix MakeCoordSystemT(Vector3 x, Vector3 y, Vector3 z)
        {
            return new Matrix(x.X, y.X, z.X, 0,
                                                x.Y, y.Y, z.Y, 0,
                                                x.Z, y.Z, z.Z, 0,
                                                0, 0, 0, 1);
        }


        /// <summary>
        /// create a string representing the matrix
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public static string PrintMatrix(Matrix m)
        {
            return "(" + m.M11 + " | " + m.M12 + " | " + m.M13 + " | " + m.M14 + ")\n" +
                         "(" + m.M21 + " | " + m.M22 + " | " + m.M23 + " | " + m.M24 + ")\n" +
                         "(" + m.M31 + " | " + m.M32 + " | " + m.M33 + " | " + m.M34 + ")\n" +
                         "(" + m.M41 + " | " + m.M42 + " | " + m.M43 + " | " + m.M44 + ")\n";
        }

        /// <summary>
        /// compute an inertia tensor
        /// </summary>
        /// <param name="points">a list of points</param>
        /// <param name="centerOfMass">the objects center of mass</param>
        /// <param name="pointmasses">the mass of each point</param>
        /// <param name="tensor">the resulting tensor</param>
        public static void InertiaTensor(List<Vector3> points, Vector3 centerOfMass, List<float> pointmasses, ref Matrix tensor)
        {
            float distanceSQ;
            tensor = new Matrix(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
            for (int i = 0; i < points.Count; i++)
            {
                points[i] = points[i] - centerOfMass;
                distanceSQ = points[i].LengthSquared();

                tensor.M11 += pointmasses[i] * (distanceSQ - points[i].X * points[i].X);
                tensor.M12 -= pointmasses[i] * points[i].X * points[i].Y;
                tensor.M13 -= pointmasses[i] * points[i].Z * points[i].X;

                tensor.M21 -= pointmasses[i] * points[i].X * points[i].Y;
                tensor.M22 += pointmasses[i] * (distanceSQ - points[i].Y * points[i].Y);
                tensor.M23 -= pointmasses[i] * points[i].Z * points[i].Y;

                tensor.M31 -= pointmasses[i] * points[i].X * points[i].Z;
                tensor.M32 -= pointmasses[i] * points[i].Z * points[i].X;
                tensor.M33 += pointmasses[i] * (distanceSQ - points[i].Z * points[i].Z);
            }

        }

        /// <summary>
        /// compute a tensor under the assumption that each point has the same mass
        /// </summary>
        /// <param name="points">a list of points</param>
        /// <param name="centerOfMass">the objects center of mass</param>
        /// <param name="mass">the objects mass</param>
        /// <param name="tensor">the resulting tensor</param>
        public static void InertiaTensor(List<Vector3> points, Vector3 centerOfMass, float mass, ref Matrix tensor)
        {
            List<float> masslist = new List<float>();
            float massperpoint = mass / points.Count;
            for (int i = 0; i < points.Count; i++)
                masslist.Add(massperpoint);
            InertiaTensor(points, centerOfMass, masslist, ref tensor);
        }


        /// <summary>
        /// compute a tensor under the assumption the objects mass is distributed evenly arround its center of mass
        /// </summary>
        /// <param name="mass"> the objects mass</param>
        /// <param name="tensor">the resulting tensor</param>
        public static void UniformInertiaTensor(float mass, ref Matrix tensor)
        {
            tensor = Matrix.Identity * (2.0f * mass) / 3.0f;
            tensor.M44 = 0.0f;
        }

        public static bool rayPlaneIntersect(Vector3 ray_pos, Vector3 ray_dir, Vector3 plane_normal, Vector3 plane_point, ref Vector3 ip,ref float l)
        {
            float ddotn = Vector3.Dot(ray_dir, plane_normal);
            if (OurMath.FLOATZERO(ddotn)) return false;
            l = -((Vector3.Dot(ray_pos, plane_normal) - Vector3.Dot(plane_normal,plane_point)) / ddotn);
            ip = ray_pos + ray_dir * l;
            return true;
        }

        public static bool rayPlaneIntersect(Vector3 ray_pos, Vector3 ray_dir, Vector3 plane_normal, float plane_d, ref Vector3 ip)
        {
            float ddotn=Vector3.Dot(ray_dir, plane_normal);
            if (OurMath.FLOATZERO(ddotn)) return false;
            float l = -((Vector3.Dot(ray_pos, plane_normal) + plane_d) / ddotn);
            ip = ray_pos + ray_dir * l;
            return true;
        }


        public static bool rayCylinderIntersect(Vector3 ray_pos, Vector3 ray_dir, Vector3 cylinder_base,Vector3 cylinder_axis, float cylinder_radius, ref float l_in, ref float l_out)
        {
            /*
             * ray cylinder intersection based on Graphic Gems IV
             */
            Vector3 raytocylinder= ray_pos - cylinder_base;

            Vector3 n=Vector3.Cross(ray_dir,cylinder_axis);
            Vector3 D,O;
           
            bool ret;
            float t,d,s;
            float n_length = n.Length();
    
            if (OurMath.FLOATZERO(n_length))
            {
                d = Vector3.Dot(raytocylinder, cylinder_axis);
                D = raytocylinder - d * cylinder_axis;
                d = D.Length();
                l_in = float.MinValue;
                l_out = float.MaxValue;
                return d <= cylinder_radius;
            }
            n.Normalize();
            d = (float)(Math.Abs(Vector3.Dot(raytocylinder, n)));
            ret = (d <= cylinder_radius);
            if (ret)
            {

                O = Vector3.Cross(raytocylinder, cylinder_axis);

                t = -Vector3.Dot(O, n) / n_length;
                O = Vector3.Cross(n, cylinder_axis);
                O.Normalize();
                s=(float)(Math.Abs(Math.Sqrt(cylinder_radius*cylinder_radius-d*d)/Vector3.Dot(ray_dir,O)));
                l_in = t - s;
                l_out = t + s;
            }
            return ret;
        }

        public static BoundingBox transformAABB(BoundingBox box,Matrix mat)
        {
            Vector3[] vertices = box.GetCorners();
            for (int i = 0; i < vertices.Length; i++)
                vertices[i] = Vector3.Transform(vertices[i], mat);
            return BoundingBox.CreateFromPoints(vertices);
        }

        public static float linlogSplit(int split, int splitcount, float near, float far,float factor)
        {
            float soc=((float)split/(float)splitcount);
            float slin = near + (far - near) * soc;
            float slog = near * ((float)Math.Pow((far / near), soc));
            return factor * slog + (1.0f - factor) * slin;
        }

    }

    public class MyFrustum
    {
        public float Fov;
        public float Aspect;
        public float Near;
        public float Far;
        public Matrix view;
        public MyFrustum(Camera c)
        {
            this.Fov = c.Fov;
            this.Aspect = c.Aspect;
            this.Near = c.Near;
            this.Far = c.MaxFar;
            this.view = c.View;
        }

        public BoundingFrustum getFrustum(float near, float far)
        {
           return new BoundingFrustum(view*Matrix.CreatePerspectiveFieldOfView(Fov, Aspect, near, far));    
        }

    }

        


       


       
    
}
