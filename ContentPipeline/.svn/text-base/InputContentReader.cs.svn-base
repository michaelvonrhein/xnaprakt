using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Content;
using System.IO;

namespace PraktWS0708.ContentPipeline
{
    class InputContentReader : ContentTypeReader<ContentInput>
    {
        protected override ContentInput Read(ContentReader input, ContentInput existingInstance)
        {
            ContentInput content = new ContentInput();

            //General
            content.GeneralInfo.TypeName = input.ReadString();
            content.GeneralInfo.ModelName = input.ReadString();

            //Physics
            content.Physics.Type = input.ReadString();
            content.Physics.Mass = input.ReadSingle();
            content.Physics.MassCenter = input.ReadVector3();
            content.Physics.ThrustFactor = input.ReadSingle();
            content.Physics.SteeringFactor = input.ReadSingle();
            content.Physics.Hover = input.ReadBoolean();
            content.Physics.Drag = input.ReadBoolean();
            content.Physics.DragFactor = input.ReadVector3();
            content.Physics.Gravity = input.ReadBoolean();

            //Rendering
            content.Rendering.Type = input.ReadString();
            content.Rendering.Paths = input.ReadRawObject<Dictionary<string, string>>();
            content.Rendering.MatrixParameters = input.ReadRawObject<Dictionary<string, Matrix>>();
            content.Rendering.VectorParameters = input.ReadRawObject<Dictionary<string, Vector4>>();
            content.Rendering.FloatParameters = input.ReadRawObject<Dictionary<string, float>>();
            content.Rendering.BoolParameters = input.ReadRawObject<Dictionary<string, bool>>();
            content.Rendering.IntParameters = input.ReadRawObject<Dictionary<string, int>>();

            //Logic
            content.Logic.Type = input.ReadString();
            content.Logic.Health = input.ReadSingle();
            content.Logic.Damage = input.ReadSingle();

            int length = input.ReadInt32();

            content.Logic.Position = new Vector3[length];
            content.Logic.Scale = new Vector3[length];
            content.Logic.Orientation = new Matrix[length];

            for (int i = 0; i < length; i++)
            {
                content.Logic.Position[i] = input.ReadVector3();
                content.Logic.Scale[i] = input.ReadVector3();
                content.Logic.Orientation[i] = input.ReadMatrix();
            }
            
            //Input
            content.Input.Type = input.ReadString();

            return content;
        }
    }
}
