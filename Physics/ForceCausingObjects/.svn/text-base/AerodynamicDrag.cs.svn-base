#region Using Statements
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
#endregion

namespace PraktWS0708.Physics
{
    class AerodynamicDrag : ForceCausingObject
    {
        #region Fields

        private Vector3 vDragFactor;

        private static Vector3 vSpinningResistence = new Vector3(1.0f, 2.0f, 0.001f); //TODO
        private int offCounter = 0;
        private int amplifyCounter = 0;
        private float increaseDragBy = 1f;
        private bool off = false;

        #endregion

        #region Initialization

        public AerodynamicDrag(Vector3 vDragFactor)
        {
            this.vDragFactor = vDragFactor;
        }

        #endregion

        #region Force and Torque

        public override ForceTorque CalcForceTorque(RigidBody oRigidBody)
        {
            // create ForceTorque object for result force/torque
            ForceTorque oForceTorque = new ForceTorque(Vector3.Zero, Vector3.Zero);

            

            //Vector3 vSpinningResistence = new Vector3(0.5f, 50f, 0.001f);
            Vector3 vSpinningResistence = new Vector3(1.0f, 2.0f, 0.001f);



                // to local coordinate system
            Vector3 vLinearVelocity = Vector3.Transform(oRigidBody.CurState.m_vLinearVelocity, Matrix.Transpose(oRigidBody.CurState.m_matOrientation));

            // to absolute coordinate system
            oForceTorque.Force -= Vector3.Transform(vSpinningResistence * vDragFactor * vLinearVelocity/2, oRigidBody.CurState.m_matOrientation);

            oForceTorque.Torque -= oRigidBody.CurState.m_vAngularVelocity * vDragFactor / 15f;
            
            if (offCounter <= 0 && !off)
            {
                // to absolute coordinate system
                oForceTorque.Force -= Vector3.Transform(vSpinningResistence * vDragFactor * vLinearVelocity, oRigidBody.CurState.m_matOrientation);

                oForceTorque.Torque -= oRigidBody.CurState.m_vAngularVelocity * vDragFactor;
            }
            else
            {
                if (offCounter > 0)
                {
                    offCounter--;
                }
                
            }
            return oForceTorque;
        }


        public override void CalcForceTorque(RigidBody oRigidBody, ref ForceTorque totalForceTorque)
        {
           

            // to local coordinate system
            Vector3 vLinearVelocity = Vector3.Transform(oRigidBody.CurState.m_vLinearVelocity, Matrix.Transpose(oRigidBody.CurState.m_matOrientation));

            // to absolute coordinate system
            totalForceTorque.Force -= Vector3.Transform(vSpinningResistence * vDragFactor * vLinearVelocity / 2, oRigidBody.CurState.m_matOrientation);

            totalForceTorque.Torque -= oRigidBody.CurState.m_vAngularVelocity * vDragFactor / 15f;

            if (offCounter <= 0 && !off)
            {
                // to absolute coordinate system
                totalForceTorque.Force -= Vector3.Transform(vSpinningResistence * vDragFactor * vLinearVelocity, oRigidBody.CurState.m_matOrientation);

                totalForceTorque.Torque -= oRigidBody.CurState.m_vAngularVelocity * vDragFactor;
            }
            else
            {
                if (offCounter > 0)
                {
                    offCounter--;
                }

            }

            if (amplifyCounter > 0)
            {
                totalForceTorque.Force -= increaseDragBy * Vector3.Transform(vSpinningResistence * vDragFactor * vLinearVelocity, oRigidBody.CurState.m_matOrientation);

                totalForceTorque.Torque -= increaseDragBy * oRigidBody.CurState.m_vAngularVelocity * vDragFactor;
                amplifyCounter--;
            }
            
            
        }

        public void Amplify(float increaseDragBy, int calculationsWithAmplifiedDrag)
        {
            this.increaseDragBy = increaseDragBy;
            this.amplifyCounter = calculationsWithAmplifiedDrag;
        }

        public void setOff(int calculationsWithoutDrag)
        {
            offCounter += calculationsWithoutDrag;
        }

        public void swtichOFF(bool off)
        {
            this.off = off;
        }

        #endregion
    }
}