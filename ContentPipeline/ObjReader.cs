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
    class ObjReader : ContentTypeReader<MeshData[][]>
    {
        protected override MeshData[][] Read(ContentReader input, MeshData[][] existingInstance)
        {
             int length_first = input.ReadInt32();

             MeshData[][] result = new MeshData[length_first][];

            for(int j = 0; j < length_first; j++)
            {
                int length_sec = input.ReadInt32();
                result[j] = new MeshData[length_sec];
            
                for (int i = 0; i < length_sec; i++)
                {
                    result[j][i].position = input.ReadVector3();
                    result[j][i].normal = input.ReadVector3();
                    result[j][i].texture = input.ReadVector2();
                }
            } 

            return result; 
        }
    }
}
