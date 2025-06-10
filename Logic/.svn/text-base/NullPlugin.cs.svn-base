using System;
using System.Collections.Generic;
using System.Text;
using PraktWS0708.Physics;

namespace PraktWS0708.Logic
{
    /// <summary>
    /// If an object does not need a logic plugin (e.g the ghostship) you assign a Nullplugin to it.
    /// A Nullplugin does not respond to collision.
    /// </summary>
    public class NullPlugin:LogicPlugin
    {
        public NullPlugin(LogicPluginData oLogicPluginData, Entities.BaseEntity e) : base(oLogicPluginData, e) { }
        public void Interact(PhysicsPlugin otherObject, float crashSpeed)
        {
            return;   
        }
        public override bool isResponding()
        {
            return false;
        }
    }
}
