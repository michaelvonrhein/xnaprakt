using System;
using System.Collections.Generic;
using System.Text;
using PraktWS0708.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PraktWS0708.Rendering
{
    public class Camera
    {
        public float Fov = MathHelper.PiOver2;
        public float Aspect = 1.0f;
        public float Near = 0.0001f;
        public float Far = 1000.0f;
        public float MaxFar = 1000.0f;

        public BoundingFrustum frustum;

        public Vector3 Up = new Vector3(0.0f, 1.0f, 0.0f);
        public Vector3 Position = new Vector3(0.0f, 0.0f, 0.0f);
        public Vector3 LookAt = new Vector3(0.0f, 0.0f, -1.0f);

        public Matrix ViewProjection;
        public Matrix Projection;
        public Matrix View;

        public Camera()
        {
            Update();
        }

        public void Update()
        {
            Projection = Matrix.CreatePerspectiveFieldOfView(Fov, Aspect, Near, Far);
            View = Matrix.CreateLookAt(Position, LookAt, Up);
            frustum = new BoundingFrustum(View * Projection);
            ViewProjection = View * Projection;
        }

        public bool Visible(BoundingSphere sphere)
        {
            return frustum.Intersects(sphere);
        }
    }
}
