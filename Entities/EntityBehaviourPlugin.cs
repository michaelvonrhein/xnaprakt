using System;
using System.Collections.Generic;
using System.Text;

namespace PraktWS0708.Entities
{
    public class EntityBehaviourPlugin
    {
        public BaseEntity entity;

        protected EntityBehaviourPlugin(Entities.BaseEntity entity)
        {
            this.entity = entity;
        }
    }
}
