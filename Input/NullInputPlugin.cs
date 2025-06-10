using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace PraktWS0708.Input
{
    public class NullInputPlugin : InputPlugin
    {
        public NullInputPlugin(Entities.BaseEntity e) : base(e) { }
    }
}
