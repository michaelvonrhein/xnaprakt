using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PraktWS0708.Rendering
{
    public class Spotlight : IComparable
    {
        public Camera Camera = new Camera();
        public Vector3 Specular = Vector3.One;
        public Vector3 Diffuse = Vector3.One;
        public float SpecularPower = 16f;
        public Texture2D ShadowMap;

        public float DistanceSQ = 0.0f;

        public int CompareTo(object obj)
        {
            return DistanceSQ.CompareTo(((Spotlight)obj).DistanceSQ);
        }
    }

    public class TrackLight : IComparable
    {
        public Vector3 Position = Vector3.One;
        public Vector3 Direction = Vector3.One;
        public Vector3 Specular = Vector3.One;
        public Vector3 Diffuse = Vector3.One;
        
        public float DistanceSQ = 0.0f;

        public int CompareTo(object obj)
        {
            return DistanceSQ.CompareTo(((TrackLight)obj).DistanceSQ);
        }
    }
}
