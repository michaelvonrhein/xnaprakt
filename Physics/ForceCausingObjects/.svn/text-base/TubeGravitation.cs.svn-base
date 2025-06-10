#region Using Statements
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using PraktWS0708.Entities;
#endregion

namespace PraktWS0708.Physics
{
    class TubeGravity : ForceCausingObject
    {
        #region Fields

        private TrackBody m_TrackBody;

        #endregion

        #region Initialization

        // fAttractionFactor points in the direction in which a object will be moving
        public TubeGravity(TrackBody oTrackBody)
        {
            m_TrackBody = oTrackBody;
        }

        #endregion

        #region Force and Torque

        public override ForceTorque CalcForceTorque(RigidBody oRigidBody)
        {
            ForceTorque oForceTorque = new ForceTorque(Vector3.Zero, Vector3.Zero);
            int i = oRigidBody.m_iCollisionSearch;
            int sign = 1;
            if (i % 10 >= 5 )
            {
                sign = -1;
            }
            oForceTorque.Force =(m_TrackBody.m_PositionOrientationRadius[i].matOrientation.Forward * sign/800f);
            return oForceTorque;
        }

        public override void CalcForceTorque(RigidBody oRigidBody, ref ForceTorque totalForceTorque)
        {
            
            int i = oRigidBody.m_iCollisionSearch;
            int sign = 1;
            if (i % 10 >= 5)
            {
                sign = -1;
            }
            totalForceTorque.Force += (m_TrackBody.m_PositionOrientationRadius[i].matOrientation.Forward * sign / 800f);
            
        }

        #endregion

    }
}