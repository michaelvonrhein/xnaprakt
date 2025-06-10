using System;
using System.Collections.Generic;
using System.Text;
using PraktWS0708.Physics;

namespace PraktWS0708.Logic
{

    
    /// <summary>
    /// The logicplugin called healthpack heals your ship
    /// </summary>
        public class HealthPack : Logic.Powereffect
        {
            
            //How much health will be restored
            private float healHp;

            public HealthPack(float healHP, Physics.RigidBody rigidBody, int durationInMillies)
                : base(rigidBody, durationInMillies)
            {
                this.healHp = healHP;
                type = PowerUpTypes.HEALTHPACK;
            }

            /// <summary>
            /// Calling this method, the health of the ship will be
            /// restored. How strong the healing is can be specified by the 
            /// parameter in the constructor
            /// </summary>
            public override void startEffect()
            {
                if (rigidBody.entity.LogicPlugin is Ship)
                {
                    ((Ship)rigidBody.entity.LogicPlugin).addHealth(healHp);
                }
               
            }

            /// <summary>
            /// As restoring the health is a one way effect,
            /// this method will do nothin
            /// </summary>
            public override void endEffect()
            {
                //DO Nothing
            }
        }
        
}
