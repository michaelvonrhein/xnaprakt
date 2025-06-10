using System;
using System.Collections.Generic;
using System.Text;
using PraktWS0708.Entities;

namespace PraktWS0708.Logic
{
    public class Obstacle : SolidObject
    {
        public Obstacle(PraktWS0708.Logic.LogicPlugin.LogicPluginData oLogicPluginData, Entities.BaseEntity e)
            : base(oLogicPluginData, e)
        {
        }

        public override void Interact(PraktWS0708.Entities.BaseEntity entity, float crashSpeed)
        {
            //Do nothing
        }

        public override bool isResponding()
        {
            return true;
        }

        public override void addDamageFromExplosion(BaseEntity entity, float damage) {
            //DO NOTHING
        }
    }
}
