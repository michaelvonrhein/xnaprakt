using System;
using System.Collections.Generic;
using System.Text;

namespace PraktWS0708.Logic
{
    /// <summary>
    /// Super class for all powerup effects 
    /// </summary>
    public abstract class Powereffect
    {
        protected Physics.RigidBody rigidBody;
        protected int durationInMillies;
        protected PowerUpTypes type;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rigidBody">the rigidbody influenced by the effect</param>
        /// <param name="durationInMillies">the duration of the effect</param>
        public Powereffect(Physics.RigidBody rigidBody, int durationInMillies)
        {
            this.rigidBody = rigidBody;
            this.durationInMillies = durationInMillies;
        }

        public abstract void startEffect();
        public abstract void endEffect();
        
        /// <summary>
        /// Reduces the timecounter of the effect
        /// </summary>
        /// <param name="timeMilliesPassed"></param>
        public void update(int timeMilliesPassed)
        {
            durationInMillies -= timeMilliesPassed;
        }

        /// <summary>
        /// Returns the timecounter of the effect
        /// </summary>
        /// <returns></returns>
        public int getRemainingEffectDuration()
        {
            return durationInMillies;
        }

        /// <summary>
        /// Returns the type of the powereffect
        /// </summary>
        /// <returns></returns>
        public PowerUpTypes getType()
        {
            return type;
        }
    }
}
