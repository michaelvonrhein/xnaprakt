#region Using Statements
using System;
using System.Collections.Generic;
using System.Text;
#endregion

namespace PraktWS0708.Physics
{
    /// <summary>
    /// An ForceCausingObject is an object which causes a linear force and/or torque.
    /// </summary>
    public abstract class ForceCausingObject
    {
        /// <summary>
        /// Calculates the force/torque for a RigidBody.
        /// </summary>
        /// <param name="obj">RigidBody to calculate the force/torque for.</param>
        /// <returns></returns>
        public abstract ForceTorque CalcForceTorque(RigidBody oRigidBody);
        public abstract void CalcForceTorque(RigidBody oRigidBody, ref ForceTorque oTotalForceTorque);
    }
}