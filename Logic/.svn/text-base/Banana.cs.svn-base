using System;
using System.Collections.Generic;
using System.Text;
using PraktWS0708.Physics;

namespace PraktWS0708.Logic
{
    /// <summary>
    /// Banane is a Powerup which removes the friction of the ship relating
    /// to the steering. The result is that the ship is very hard to control
    /// because the ship stabilizes much slower after a steering action.
    /// </summary>
    public class Banana : Logic.Powereffect
    {


        public Banana(Physics.RigidBody rigidBody, int durationInMillies)
            : base(rigidBody, durationInMillies)
        {
            type = PowerUpTypes.BANANA;
        }

        /// <summary>
        ///When the player picks up a banana, this method is called
        /// </summary>
        public override void startEffect()
        {
            if (rigidBody.entity.PhysicsPlugin is SteeringBody)
            {
                SteeringBody sb = ((SteeringBody)rigidBody.entity.PhysicsPlugin);
                foreach (ForceCausingObject fco in sb.ForceCausingObjects)
                {
                    if (fco is Physics.AerodynamicDrag)
                    {
                        ((Physics.AerodynamicDrag)fco).swtichOFF(true);
                    }
                }
            }
            
        }

        /// <summary>
        /// Every powerup has a time to live, and afterwards the regular settings
        /// will be restored calling this method
        /// </summary>
        public override void endEffect()
        {
            if (rigidBody.entity.PhysicsPlugin is SteeringBody)
            {
                SteeringBody sb = ((SteeringBody)rigidBody.entity.PhysicsPlugin);
                foreach (ForceCausingObject fco in sb.ForceCausingObjects)
                {
                    if (fco is Physics.AerodynamicDrag)
                    {
                        ((Physics.AerodynamicDrag)fco).swtichOFF(false);
                    }
                }
            } 
        }
    }
}
