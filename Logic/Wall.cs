using System;
using System.Collections.Generic;
using System.Text;
using PraktWS0708.Physics;
using PraktWS0708.Entities;

namespace PraktWS0708.Logic
{
    class Wall : SolidObject
    {
        /// <summary>
        /// A wall has infinty health and allways responds to crashes
        /// </summary>
        /// <param name="oLogicPluginData"></param>
        /// <param name="e"></param>
        public Wall(PraktWS0708.Logic.LogicPlugin.LogicPluginData oLogicPluginData, Entities.BaseEntity e) : base(oLogicPluginData, e) { }

        public override void Interact(BaseEntity entity, float crashSpeed)
        {
            //do nothing
        }

        public override bool isResponding()
        {
            return true;
        }

        public override void addDamageFromExplosion(BaseEntity entity, float damage)
        {
            //DO NOTHING
        }
    }
}
