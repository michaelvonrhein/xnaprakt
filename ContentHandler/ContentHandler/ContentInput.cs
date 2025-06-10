using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using System.IO;

namespace PraktWS0708.ContentHandler
{
    public struct GeneralInfoData
    {
        public String TypeName;
        public String ModelName;
    }

    public struct PhysicsData
    {
        public String Type;
        public float Mass;
        public Vector3 MassCenter;
        public float ThrustFactor;
        public float SteeringFactor;
        public bool Hover;
        public bool Drag;
        public Vector3 DragFactor;
        public bool Gravity;
    }

    public struct LogicData
    {
        public String Type;
        public float Health;
        public float Damage;
        public Vector3[] Position;
        public Vector3[] Scale;
        public Matrix[] Orientation;
    }

    public struct InputData
    {
        public String Type;
    }

    public struct RenderingData
    {
        public String Type;
        public Dictionary<String, String> Paths;
        public Dictionary<string, Matrix> MatrixParameters;
        public Dictionary<string, Vector4> VectorParameters;
        public Dictionary<string, float> FloatParameters;
        public Dictionary<string, bool> BoolParameters;
        public Dictionary<string, int> IntParameters;
    }

    public struct ContentInput
    {
        public GeneralInfoData GeneralInfo;
        public PhysicsData Physics;
        public RenderingData Rendering;
        public LogicData Logic;
        public InputData Input;
    }

    public struct MeshData
    {
        public Vector3 position;
        public Vector3 normal;
        public Vector2 texture;
    }
}