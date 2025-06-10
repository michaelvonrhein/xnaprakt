using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using PraktWS0708.Entities;

namespace PraktWS0708.Logic
{
    public interface Explosive
    {
       
       float getExplosionDamage();

        void explode(BaseEntity oEntity);
    }
}
