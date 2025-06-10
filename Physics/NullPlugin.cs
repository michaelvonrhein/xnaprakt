#region Using Statements
using System;
using System.Collections.Generic;
using System.Text;
#endregion

namespace PraktWS0708.Physics
{
    class NullPlugin : PhysicsPlugin
    {
        public NullPlugin(PhysicsPluginData oPhysicsPluginData, Entities.BaseEntity oEntity)
            : base(oPhysicsPluginData, oEntity) { }

        public override void destroyElement()
        {
            //DO NOTHING    
        }
    }
}
