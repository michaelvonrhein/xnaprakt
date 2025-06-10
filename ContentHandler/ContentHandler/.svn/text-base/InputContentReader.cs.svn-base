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
            content.Rendering.Paths = input.ReadRawObject<Dictionary<string, string>>();
            content.Rendering.MatrixParameters = input.ReadRawObject<Dictionary<string, Matrix>>();
            content.Rendering.VectorParameters = input.ReadRawObject<Dictionary<string, Vector4>>();
            content.Rendering.FloatParameters = input.ReadRawObject<Dictionary<string, float>>();
            content.Rendering.IntParameters = input.ReadRawObject<Dictionary<string, int>>();
            content.Rendering.BoolParameters = input.ReadRawObject<Dictionary<string, bool>>();

            return content;
        }
    }
}
