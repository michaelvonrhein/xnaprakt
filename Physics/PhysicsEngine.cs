#region Using Statements
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using PraktWS0708.Utils;
#endregion

namespace PraktWS0708.Physics
{
    #region ForceTorque

    /// <summary>
    /// ForceTorque consists of a linear force and a torque.
    /// </summary>
    public class ForceTorque
    {
        #region Fields

        public Vector3 m_vForce;
        public Vector3 m_vTorque;

        #endregion

        #region Initialization

        /// <summary>
        /// ForceTorque constructor
        /// </summary>
        /// <param name="vForce">force</param>
        /// <param name="vTorque">torque</param>
        public ForceTorque(Vector3 vForce, Vector3 vTorque)
        {
            m_vForce = vForce;
            m_vTorque = vTorque;
        }

        #endregion

        #region Properties

        /// <summary>
        /// get/set force
        /// </summary>
        public Vector3 Force
        {
            get
            {
                return m_vForce;
            }
            set
            {
                m_vForce = value;
            }
        }

        /// <summary>
        /// get/set torque
        /// </summary>
        public Vector3 Torque
        {
            get
            {
                return m_vTorque;
            }
            set
            {
                m_vTorque = value;
            }
        }

        #endregion
    }

    #endregion

    /// <summary>
    /// The actual PhysicsEngine, which has to compute all sorts of physical computations.
    /// </summary>
    public class PhysicsEngine
    {

        

        #region Methods

        #region Inverted Moment of Inertia

        /// <summary>
        /// Calculates the InvertedMomentOfInertia
        /// </summary>
        /// <param name="oRigidBody"></param>
        /// <returns></returns>
        public static Matrix CalcInvertedMomentOfInertia(RigidBody oRigidBody)
        {
           
            Matrix matMomentOfInertia = Matrix.Identity;

            // calculate inverted moment of inertia for a sphere
            float fMomentOfInertia = 2f * oRigidBody.Mass * (float)Math.Pow(oRigidBody.BoundingSphere.m_fRadius, 2f) / 5f;
            matMomentOfInertia.M11 = fMomentOfInertia;
            matMomentOfInertia.M22 = fMomentOfInertia;
            matMomentOfInertia.M33 = fMomentOfInertia;

            return Matrix.Invert(matMomentOfInertia);   // return inverted moment of inertia
            /*else if (obj.BoundingObject is BoundingCuboid)
            {
                float fMomentOfInertiaX = 1/12f * Mass * (heightY * heightY + depthZ * depthZ;
                float fMomentOfInertiaY = 1/12f * Mass * (widthX * widthX + depthZ * depthZ);
                float fMomentOfInertiaZ = 1/12f * Mass * (widthX * widthX + heightY * heightY);
                matInvertedMomentOfInertia.M11 = fMomentOfInertiaX;
                matInvertedMomentOfInertia.M22 = fMomentOfInertiaY;
                matInvertedMomentOfInertia.M33 = fMomentOfInertiaZ;
                return matInvertedMomentOfInertia;
            }*/
        }

        #endregion

        #region Angular Velocity

        /// <summary>
        /// Calculates the angular velocity
        /// </summary>
        /// <param name="matOrientation">current orientation</param>
        /// <param name="matInvertedMomentOfInertia">inverted moment of inertia</param>
        /// <param name="vAngularMomentum">current angular momentum</param>
        /// <returns></returns>
        public static Vector3 CalcAngularVelocity(Matrix matOrientation, Matrix matInvertedMomentOfInertia, Vector3 vAngularMomentum)
        {
            
            // calculate absolute inverted moment of inertia
            Matrix matAIMOI = matOrientation * matInvertedMomentOfInertia * Matrix.Transpose(matOrientation);
           
            
            // calculate angular velocity
            return Vector3.Transform(vAngularMomentum, matAIMOI);
        }

        #endregion

        #region Torque

        /// <summary>
        /// Calculates the torque caused by a force
        /// </summary>
        /// <param name="vPosition">Position relative to the mass center.</param>
        /// <param name="vForce">The force which causes the torque.</param>
        /// <returns></returns>
        public static Vector3 CalcTorque(Vector3 vPosition, Vector3 vForce)
        {
            return Vector3.Cross(vPosition, vForce);
        }

        #endregion

        #region Derivative Orientation

        /// <summary>
        /// Calculates the derivative orientation matrix
        /// </summary>
        /// <param name="vOmega">angular velocity</param>
        public static Matrix CalcDerivativeOrientationMatrix(Vector3 vOmega)
        {
            Matrix matDerivativeOrientation = new Matrix(0f, vOmega.Z, -vOmega.Y, 0f,
                                                         -vOmega.Z, 0f, vOmega.X, 0f,
                                                         vOmega.Y, -vOmega.X, 0f, 0f,
                                                         0f, 0f, 0f, 1f); // omega tilde
            return matDerivativeOrientation;
        }

        #endregion

        #region Reorthogonalization

        /// <summary>
        /// Reorthogonalize the matrix.
        /// </summary>
        /// <param name="matMatrix">Matrix which should be reorthogonalized.</param>
        /// <returns></returns>
        public static Matrix ReorthogonalizeMatrix(Matrix matMatrix)
        {
            // reorthogonalize orientation matrix
            Vector3 vV1 = new Vector3(matMatrix.M11, matMatrix.M12, matMatrix.M13);
            Vector3 vV2 = new Vector3(matMatrix.M21, matMatrix.M22, matMatrix.M23);
            Vector3 vV3 = new Vector3(matMatrix.M31, matMatrix.M32, matMatrix.M33);

            // reorthogonalized vectors
            Vector3 vU1 = vV1;
            Vector3 vU2 = vV2 - Vector3.Dot(vU1, vV2) / Vector3.Dot(vU1, vU1) * vU1;
            Vector3 vU3 = vV3 - Vector3.Dot(vU1, vV3) / Vector3.Dot(vU1, vU1) * vU1
                              - Vector3.Dot(vU2, vV3) / Vector3.Dot(vU2, vU2) * vU2;

            // normalize vectors
            vU1.Normalize();
            vU2.Normalize();
            vU3.Normalize();

            Matrix matResult = new Matrix(vU1.X, vU1.Y, vU1.Z, 0f,
                                          vU2.X, vU2.Y, vU2.Z, 0f,
                                          vU3.X, vU3.Y, vU3.Z, 0f,
                                          0f, 0f, 0f, 1f);

            return matResult;
        }

        #endregion

        #endregion
    }
}
