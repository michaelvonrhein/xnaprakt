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
    [ContentTypeWriter]
    class InputContentWriter : ContentTypeWriter<ContentInput>
    {
        protected override void Write(ContentWriter output, ContentInput value)
        {
            //General
            output.Write(value.GeneralInfo.TypeName);
            output.Write(value.GeneralInfo.ModelName);

            //Physics
            output.Write(value.Physics.Type);
            output.Write(value.Physics.Mass);
            output.Write(value.Physics.MassCenter);
            output.Write(value.Physics.ThrustFactor);
            output.Write(value.Physics.SteeringFactor);
            output.Write(value.Physics.Hover);
            output.Write(value.Physics.Drag);
            output.Write(value.Physics.DragFactor);
            output.Write(value.Physics.Gravity);

            //Rendering
            output.Write(value.Rendering.Type);
            output.WriteRawObject<Dictionary<string, string>>(value.Rendering.Paths);
            output.WriteRawObject<Dictionary<string, Matrix>>(value.Rendering.MatrixParameters);
            output.WriteRawObject<Dictionary<string, Vector4>>(value.Rendering.VectorParameters);
            output.WriteRawObject<Dictionary<string, float>>(value.Rendering.FloatParameters);
            output.WriteRawObject<Dictionary<string, bool>>(value.Rendering.BoolParameters);
            output.WriteRawObject<Dictionary<string, int>>(value.Rendering.IntParameters);

            //Logic
            output.Write(value.Logic.Type);
            output.Write(value.Logic.Health);
            output.Write(value.Logic.Damage);

            output.Write(value.Logic.Position.Length);

            for (int i = 0; i < value.Logic.Position.Length; i++)
            {
                output.Write(value.Logic.Position[i]);
                output.Write(value.Logic.Scale[i]);
                output.Write(value.Logic.Orientation[i]);
            }
            

            //Input
            output.Write(value.Input.Type);
        }

        public override string GetRuntimeType(TargetPlatform targetPlatform)
        {
            //return typeof(ContentInput).AssemblyQualifiedName;
            return "PraktWS0708.ContentPipeline.ContentInput, PraktWS0708, Version=1.0.0.0, Culture=neutral";
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            //return typeof(InputContentReader).AssemblyQualifiedName;
            return "PraktWS0708.ContentPipeline.InputContentReader, PraktWS0708, Version=1.0.0.0, Culture=neutral";
        } 
    }
}
