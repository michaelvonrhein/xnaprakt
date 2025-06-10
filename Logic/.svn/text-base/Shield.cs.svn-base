using System;
using System.Collections.Generic;
using System.Text;
using PraktWS0708.Physics;

namespace PraktWS0708.Logic
{
    /// <summary>
    /// The shild protects the ship from damage the next <SHILDNUMBER> crashes
    /// </summary>
    public class Shield : Logic.Powereffect
    {
        private int shildNumber;

        public Shield(int shildNumber, Physics.RigidBody rigidBody, int durationInMillies)
            : base(rigidBody, durationInMillies)
        {
            type = PowerUpTypes.SHIELD;
            this.shildNumber = shildNumber;
        }

        /// <summary>
        /// Adds the shildcounter. If greater than zero this counter will be reduced on every crash.
        /// It it is greater than zero the ship will take no damage
        /// </summary>
        public override void startEffect()
        {
            ((Ship)rigidBody.entity.LogicPlugin).shieldCounter += shildNumber;
        }

        public override void endEffect()
        {
            //DO nothing as its effect is down on starting
        }
    }
}
