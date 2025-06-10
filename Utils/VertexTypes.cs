using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PraktWS0708.Utils
{
    //B3A1a
    /// <summary>
    /// this class represents a vertex with position, normal and color
    /// now obsolete
    /// </summary>
    public struct VertexPositionNormalColor
    {
        public Vector3 pos;
        public Vector3 normal;
        public Color color;

        public static readonly VertexElement[] VertexElements =
            new VertexElement[] { 
                new VertexElement(0,0,VertexElementFormat.Vector3,
                                             VertexElementMethod.Default,VertexElementUsage.Position,0),
                new VertexElement(0,sizeof(float)*3,VertexElementFormat.Vector3,
                                             VertexElementMethod.Default,VertexElementUsage.Normal,0),
                new VertexElement(0,sizeof(float)*6,VertexElementFormat.Color,
                                            VertexElementMethod.Default,VertexElementUsage.Color,0)               
            };


        public VertexPositionNormalColor(Vector3 pos, Vector3 normal, Color color)
        {
            this.pos = new Vector3(pos.X, pos.Y, pos.Z); this.normal = normal; this.color = color;
        }
        public VertexPositionNormalColor(VertexPositionNormalColor v)
        {
            this.pos = v.pos; this.normal = v.normal; this.color = v.color;
        }
        public static int SizeInBytes = sizeof(float) * 10;
    }

    //B5A3
    /// <summary>
    /// this class represents a vertex with position, normal and texture coordinate and tangent.
    /// it is used for normal mapping
    /// </summary>
    public struct VertexPositionNormalTextureTangent
    {
        public Vector3 Position;
        public Vector3 Normal;
        public Vector2 texcoord1;
        public Vector3 tangent;
        public static readonly VertexElement[] VertexElements =
                 new VertexElement[] { 
                new VertexElement(0,0,VertexElementFormat.Vector3,
                                             VertexElementMethod.Default,VertexElementUsage.Position,0),
                new VertexElement(0,sizeof(float)*3,VertexElementFormat.Vector3,
                                             VertexElementMethod.Default,VertexElementUsage.Normal,0),
                new VertexElement(0,sizeof(float)*6,VertexElementFormat.Vector2,
                                            VertexElementMethod.Default,VertexElementUsage.TextureCoordinate,0), 
				new VertexElement(0,sizeof(float)*8,VertexElementFormat.Vector3,
                                            VertexElementMethod.Default,VertexElementUsage.Tangent,0)
            };




        public VertexPositionNormalTextureTangent(Vector3 pos, Vector3 normal, Vector2 texcoord, Vector3 tangent)
        {
            this.Position = new Vector3(pos.X, pos.Y, pos.Z); this.Normal = normal;
            this.texcoord1 = texcoord; this.tangent = tangent;
        }
        public VertexPositionNormalTextureTangent(VertexPositionNormalTextureTangent v)
        {
            this.Position = v.Position; this.Normal = v.Normal; this.texcoord1 = v.texcoord1; this.tangent = v.tangent;
        }
        public VertexPositionNormalTextureTangent(VertexPositionNormalTextureTangent v,Vector2 newTexcoord)
        {
            this.Position = v.Position; this.Normal = v.Normal; this.texcoord1 = newTexcoord; this.tangent = v.tangent;
        }
        public static int SizeInBytes = sizeof(float) * 11;
    }

    public struct VertexPositionNormalTextureTextureTangent
    {
        public Vector3 Position;
        public Vector3 Normal;
        public Vector2 texcoord1;
        public Vector2 texcoord2;
        public Vector3 tangent;
        public static readonly VertexElement[] VertexElements =
                 new VertexElement[] { 
                new VertexElement(0,0,VertexElementFormat.Vector3,
                                             VertexElementMethod.Default,VertexElementUsage.Position,0),
                new VertexElement(0,sizeof(float)*3,VertexElementFormat.Vector3,
                                             VertexElementMethod.Default,VertexElementUsage.Normal,0),
                new VertexElement(0,sizeof(float)*6,VertexElementFormat.Vector2,
                                            VertexElementMethod.Default,VertexElementUsage.TextureCoordinate,0), 
                new VertexElement(0,sizeof(float)*8,VertexElementFormat.Vector2,
                                            VertexElementMethod.Default,VertexElementUsage.TextureCoordinate,1), 
				new VertexElement(0,sizeof(float)*10,VertexElementFormat.Vector3,
                                            VertexElementMethod.Default,VertexElementUsage.Tangent,0)
            };




        public VertexPositionNormalTextureTextureTangent(Vector3 pos, Vector3 normal, Vector2 texcoord1, Vector2 texcoord2, Vector3 tangent)
        {
            this.Position = new Vector3(pos.X, pos.Y, pos.Z); this.Normal = normal;
            this.texcoord1 = texcoord1; this.texcoord2 = texcoord2; this.tangent = tangent;
        }
        public VertexPositionNormalTextureTextureTangent(VertexPositionNormalTextureTextureTangent v)
        {
            this.Position = v.Position; this.Normal = v.Normal; this.texcoord1 = v.texcoord1; this.texcoord2 = v.texcoord2; this.tangent = v.tangent;
        }
        public static int SizeInBytes = sizeof(float) * 13;
    }
}
