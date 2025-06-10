#region Using Statements
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
#endregion

namespace PraktWS0708.Physics
{
    class TrackGravity : ForceCausingObject
    {
        #region Fields

        private TrackBody m_TrackBody;

        #endregion

        #region Initialization

        // fAttractionFactor points in the direction in which a object will be moving
        public TrackGravity(TrackBody oTrackBody)
        {
            m_TrackBody = oTrackBody;
        }

        #endregion

        #region Force and Torque (Tube)

        public override ForceTorque CalcForceTorque(RigidBody oRigidBody)
        {
            ForceTorque oForceTorque = new ForceTorque(Vector3.Zero, Vector3.Zero);

            Vector3 vNormal = oRigidBody.m_CollisionResult.m_vNormal;

            if (vNormal != Vector3.Zero) vNormal.Normalize();

            oForceTorque.Force = -vNormal / (2000f);
            
            return oForceTorque;
        }

        public override void CalcForceTorque(RigidBody oRigidBody, ref ForceTorque totalForceTorque)
        {
            //ForceTorque oForceTorque = new ForceTorque(Vector3.Zero, Vector3.Zero);

            Vector3 vNormal = oRigidBody.m_CollisionResult.m_vNormal;

            if (vNormal != Vector3.Zero) vNormal.Normalize();

            totalForceTorque.Force = -vNormal / (2000f);

            //return oForceTorque;
        }

        #endregion

        #region Force and Torque (Plane)

        /*public override ForceTorque CalcForceTorque(RigidBody oRigidBody)
        {
            ForceTorque oForceTorque = new ForceTorque(Vector3.Zero, Vector3.Zero);

            Vector3 vNormal = m_TrackBody.m_PositionOrientation[oRigidBody.m_iCollisionSearch].matOrientation.Up;
            vNormal.Normalize();

            oForceTorque.Force = -vNormal / (10000f);

            return oForceTorque;
        }*/

        #endregion
    }
}