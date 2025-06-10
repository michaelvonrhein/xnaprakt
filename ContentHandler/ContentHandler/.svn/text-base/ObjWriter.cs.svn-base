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
    class ObjWriter : ContentTypeWriter<MeshData[][]>
    {
        protected override void Write(ContentWriter output, MeshData[][] value)
        {
           output.Write(value.Length);
           
            for(int j = 0; j < value.Length; j++)
            {
               output.Write(value[j].Length);
                
                for (int i = 0; i < value[j].Length; i++)
                { 
                    output.Write(value[j][i].position);
                    output.Write(value[j][i].normal);
                    output.Write(value[j][i].texture);       
                }
            }    
        }

        public override string GetRuntimeType(TargetPlatform targetPlatform)
        {
            //return typeof(MeshData[][]).AssemblyQualifiedName;
            return "PraktWS0708.ContentPipeline.MeshData[][], PraktWS0708, Version=1.0.0.0, Culture=neutral";
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            //return typeof(ObjReader).AssemblyQualifiedName;
            return "PraktWS0708.ContentPipeline.ObjReader, PraktWS0708, Version=1.0.0.0, Culture=neutral";
        }
    }
}
