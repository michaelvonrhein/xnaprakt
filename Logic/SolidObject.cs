using System;
using System.Collections.Generic;
using System.Text;
using PraktWS0708.Physics;
using PraktWS0708.Entities;

namespace PraktWS0708.Logic
{
    public abstract class SolidObject : LogicPlugin
    {
       
        /// <summary>
        /// Super class for all logic plugins realizing some kind of 
        /// solidbody. Subclasses is for example a ship 
        /// </summary>
        /// <param name="oLogicPluginData"></param>
        /// <param name="e"></param>
        public SolidObject(PraktWS0708.Logic.LogicPlugin.LogicPluginData oLogicPluginData, Entities.BaseEntity e)
            : base(oLogicPluginData, e) { }

        public abstract void Interact(BaseEntity entity, float crashSpeed);

        public abstract void addDamageFromExplosion(BaseEntity entity, float damage);
       
    }
}
