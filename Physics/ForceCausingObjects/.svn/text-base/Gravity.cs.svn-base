#region Using Statements
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
#endregion

namespace PraktWS0708.Physics
{
    class Gravity : ForceCausingObject
    {
        #region Fields

        private Vector3 m_vAttractionFactor;

        #endregion

        #region Initialization

        // fAttractionFactor points in the direction in which a object will be moving
        public Gravity(Vector3 vAttractionFactor)
        {
            m_vAttractionFactor = vAttractionFactor;
        }

        #endregion

        #region Force and Torque

        public override ForceTorque CalcForceTorque(RigidBody oRigidBody)
        {
            ForceTorque oForceTorque = new ForceTorque(Vector3.Zero, Vector3.Zero);
            oForceTorque.Force = m_vAttractionFactor;
            return oForceTorque;
        }

        public override void CalcForceTorque(RigidBody oRigidBody, ref ForceTorque totalForceTorque)
        {

            totalForceTorque.Force += m_vAttractionFactor;
            
        }

        #endregion

    }
}