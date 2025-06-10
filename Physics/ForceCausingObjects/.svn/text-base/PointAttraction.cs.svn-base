#region Using Statements
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
#endregion

namespace PraktWS0708.Physics
{
    class PointAttraction : ForceCausingObject
    {
        #region Fields

        float m_fMaxRange;
        Vector3 m_vPosition;
        float m_fAttractionFactor;

        #endregion

        #region Initialization

        public PointAttraction(float fMaxRange, Vector3 vPosition, float fAttractionFactor)
        {
            m_fMaxRange = fMaxRange;
            m_vPosition = vPosition;
            m_fAttractionFactor = fAttractionFactor;
        }

        #endregion

        #region Force and Torque

        public override ForceTorque CalcForceTorque(RigidBody oRigidBody)
        {
            float fDistance = Vector3.Distance(oRigidBody.CurState.m_vPosition, m_vPosition);
            ForceTorque oForceTorque = new ForceTorque(Vector3.Zero, Vector3.Zero);
            Vector3 vForceDirection = (oRigidBody.CurState.m_vPosition - m_vPosition);
            if (!vForceDirection.Equals(Vector3.Zero))
            {
                vForceDirection.Normalize();
            }
            if (fDistance < m_fMaxRange)
            {
                oForceTorque.Force = -((m_fMaxRange - fDistance) / m_fMaxRange) * m_fAttractionFactor * vForceDirection;
            }
            return oForceTorque;
        }

        public override void CalcForceTorque(RigidBody oRigidBody, ref ForceTorque totalForceTorque)
        {
            float fDistance = Vector3.Distance(oRigidBody.CurState.m_vPosition, m_vPosition);
            //ForceTorque oForceTorque = new ForceTorque(Vector3.Zero, Vector3.Zero);
            Vector3 vForceDirection = (oRigidBody.CurState.m_vPosition - m_vPosition);
            if (!vForceDirection.Equals(Vector3.Zero))
            {
                vForceDirection.Normalize();
            }
            if (fDistance < m_fMaxRange)
            {
                totalForceTorque.Force = -((m_fMaxRange - fDistance) / m_fMaxRange) * m_fAttractionFactor * vForceDirection;
            }
            //return oForceTorque;
        }

        #endregion
    }
}