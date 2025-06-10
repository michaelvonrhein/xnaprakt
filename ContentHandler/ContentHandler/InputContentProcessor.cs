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
    [ContentProcessor]
    class InputContentProcessor : ContentProcessor<ContentInput, ContentInput>
    {
        public override ContentInput Process(ContentInput input, ContentProcessorContext context)
        {
            return input;
        }
    }
}
