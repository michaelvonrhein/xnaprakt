#region Using Statements
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using PraktWS0708.Entities;
using Microsoft.Xna.Framework.Audio;
#endregion

namespace PraktWS0708.Physics
{
    class ThrustSteering : ForceCausingObject
    {
        bool m_bShoot = false;
        bool m_bForward = true;
        bool m_bThrustCue = false;    // for thrust sound

        Cue ThrustCue;

        #region Force and Torque

        public override ForceTorque CalcForceTorque(RigidBody oRigidBody)
        {
            // create ForceTorque object for result force/torque
            ForceTorque oForceTorque = new ForceTorque(Vector3.Zero, Vector3.Zero);

            Input.InputPlugin oInput = oRigidBody.entity.InputPlugin;

            if (oRigidBody is SteeringBody)
            {
                SteeringBody oSteeringBody = (SteeringBody)oRigidBody;

                // ACCELERATION
                if (Math.Abs(oInput.Acceleration) > 0.0001f)
                {
                    Vector3 vAccelForce = Vector3.UnitZ * oSteeringBody.ThrustFactor * oInput.Acceleration;
                    if (oInput.Acceleration < 0f) vAccelForce = 0.0f * vAccelForce;   // backwards half thrust
                    oForceTorque.Force += Vector3.Transform(-vAccelForce, oRigidBody.CurState.m_matOrientation);
                    Sound.PlayCue(Sounds.alien);
                }

                // <YAW>
                if (Math.Abs(oInput.Yaw) > 0.0001f)
                {
                    Vector3 vYawForce = Vector3.UnitZ * oSteeringBody.SteeringFactor * oInput.Yaw;
                    // left force
                    oForceTorque.Torque += PhysicsEngine.CalcTorque(oSteeringBody.LeftPosition, -vYawForce);
                    // right force
                    oForceTorque.Torque += PhysicsEngine.CalcTorque(oSteeringBody.RightPosition, vYawForce);
                }

#if DEBUG
                // <PITCH>
                if (Math.Abs(oInput.Pitch) > 0.0001f)
                {
                    Vector3 vYawForce = Vector3.UnitY * oSteeringBody.SteeringFactor * oInput.Pitch;
                    // left force
                    oForceTorque.Torque += PhysicsEngine.CalcTorque(oSteeringBody.FrontPosition, -vYawForce);
                    // right force
                    oForceTorque.Torque += PhysicsEngine.CalcTorque(oSteeringBody.BackPosition, vYawForce);
                }
#endif
#if DEBUG
                if (oInput.Shoot == true && m_bShoot == false)
                {
                    Sound.PlayCue(Sounds.shot);
                    m_bShoot = true;
                }
                else if (oInput.Shoot == false)
                {
                    m_bShoot = false;
                }
#endif

                //if (oInput.Acceleration > 0.0f)
                //{
                //    Vector3 vForce = Vector3.UnitZ * oSteeringBody.ThrustFactor;
                //    Vector3 vPosition = oSteeringBody.BackPosition;
                //    oForceTorque.Force += Vector3.Transform(-vForce, oRigidBody.CurState.m_matOrientation);
                //}

                //if (oInput.Acceleration < 0.0f)
                //{
                //    Vector3 vForce = Vector3.UnitZ * oSteeringBody.ThrustFactor;
                //    Vector3 vPosition = oSteeringBody.BackPosition;
                //    oForceTorque.Force += Vector3.Transform(vForce, oRigidBody.CurState.m_matOrientation);
                //}

                /*if (oSteeringBody.m_bPitchUp)
                {
                    Vector3 vForce = Vector3.UnitY * oSteeringBody.SteeringFactor;

                    // front force
                    Vector3 vPosition = oSteeringBody.FrontPosition;
                    oForceTorque.Torque += PhysicsEngine.CalcTorque(vPosition, vForce);

                    // back force
                    vPosition = oSteeringBody.BackPosition;
                    oForceTorque.Torque += PhysicsEngine.CalcTorque(vPosition, -vForce);
                }

                if (oSteeringBody.m_bPitchDown)
                {
                    Vector3 vForce = Vector3.UnitY * oSteeringBody.SteeringFactor;

                    // front force
                    Vector3 vPosition = oSteeringBody.FrontPosition;
                    oForceTorque.Torque += PhysicsEngine.CalcTorque(vPosition, -vForce);

                    // back force
                    vPosition = oSteeringBody.BackPosition;
                    oForceTorque.Torque += PhysicsEngine.CalcTorque(vPosition, vForce);
                }*/

                //if (oInput.Yaw < 0.0f)
                //{
                //    Vector3 vForce = Vector3.UnitZ * oSteeringBody.SteeringFactor;

                //    // left force
                //    Vector3 vPosition = oSteeringBody.LeftPosition;
                //    oForceTorque.Torque += PhysicsEngine.CalcTorque(vPosition, vForce);

                //    // right force
                //    vPosition = oSteeringBody.RightPosition;
                //    oForceTorque.Torque += PhysicsEngine.CalcTorque(vPosition, -vForce);
                //}

                //if (oInput.Yaw > 0.0f)
                //{
                //    Vector3 vForce = Vector3.UnitZ * oSteeringBody.SteeringFactor;

                //    // left force
                //    Vector3 vPosition = oSteeringBody.LeftPosition;
                //    oForceTorque.Torque += PhysicsEngine.CalcTorque(vPosition, -vForce);

                //    // right force
                //    vPosition = oSteeringBody.RightPosition;
                //    oForceTorque.Torque += PhysicsEngine.CalcTorque(vPosition, vForce);
                //}

                /*if (oSteeringBody.m_bRollRight)
                {
                    Vector3 vForce = Vector3.UnitY * oSteeringBody.SteeringFactor;

                    // left force
                    Vector3 vPosition = oSteeringBody.LeftPosition;
                    oForceTorque.Torque += PhysicsEngine.CalcTorque(vPosition, -vForce);

                    // right force
                    vPosition = oSteeringBody.RightPosition;
                    oForceTorque.Torque += PhysicsEngine.CalcTorque(vPosition, vForce);
                }

                if (oSteeringBody.m_bRollLeft)
                {
                    Vector3 vForce = Vector3.UnitY * oSteeringBody.SteeringFactor;

                    // left force
                    Vector3 vPosition = oSteeringBody.LeftPosition;
                    oForceTorque.Torque += PhysicsEngine.CalcTorque(vPosition, vForce);

                    // right force
                    vPosition = oSteeringBody.RightPosition;
                    oForceTorque.Torque += PhysicsEngine.CalcTorque(vPosition, -vForce);
                }*/
            }
            return oForceTorque;
        }


        public override void CalcForceTorque(RigidBody oRigidBody, ref ForceTorque totalForceTorque)
        {
            // create ForceTorque object for result force/torque
            //ForceTorque oForceTorque = new ForceTorque(Vector3.Zero, Vector3.Zero);

            Input.InputPlugin oInput = oRigidBody.entity.InputPlugin;

            if (oRigidBody is SteeringBody)
            {
                SteeringBody oSteeringBody = (SteeringBody)oRigidBody;

                // ACCELERATION
                if (Math.Abs(oInput.Acceleration) > 0.0001f)
                {
                    Vector3 vAccelForce = Vector3.UnitZ * oSteeringBody.ThrustFactor * oInput.Acceleration;
                    if (oInput.Acceleration < 0f) vAccelForce = 0.5f * vAccelForce; // less acceleration backwards
                    totalForceTorque.Force += Vector3.Transform(-vAccelForce, oRigidBody.CurState.m_matOrientation);
                    if (World.Instance.PlayersShip == oRigidBody.entity)
                    {
                        // ThrustSound
                        if (!m_bThrustCue && Settings.Configuration.EngineSettings.playMusic)
                        {
                            ThrustCue = Sound.Play(Sounds.thrust);
                            m_bThrustCue = true;
                        }
                        else
                        {
                            if (Settings.Configuration.EngineSettings.playMusic&&ThrustCue.IsStopped)
                                ThrustCue = Sound.Play(Sounds.thrust);
                        }

                        // do only when speed and acceleration direction changed
                        if (Vector3.Transform(oRigidBody.CurState.m_vLinearVelocity, Matrix.Transpose(oRigidBody.Orientation)).Z * oInput.Acceleration > 0.001f
                            && m_bForward != (oInput.Acceleration > 0f))
                        {
                            Sound.PlayCue(Sounds.breaks);
                        }
                        m_bForward = (oInput.Acceleration > 0f);
                    }
                }
                else
                {
                    if (World.Instance.PlayersShip == oRigidBody.entity)
                    {
                        if (ThrustCue != null) Sound.Stop(ThrustCue);
                        m_bThrustCue = false;
                    }
                }

                // <YAW>
                if (Math.Abs(oInput.Yaw) > 0.0001f)
                {
                    Vector3 vYawForce = Vector3.UnitZ * oSteeringBody.SteeringFactor * oInput.Yaw;
                    // left force
                    totalForceTorque.Torque += PhysicsEngine.CalcTorque(oSteeringBody.LeftPosition, -vYawForce);
                    // right force
                    totalForceTorque.Torque += PhysicsEngine.CalcTorque(oSteeringBody.RightPosition, vYawForce);
                }

#if DEBUG
                // <PITCH>
                if (Math.Abs(oInput.Pitch) > 0.0001f)
                {
                    Vector3 vYawForce = Vector3.UnitY * oSteeringBody.SteeringFactor * oInput.Pitch;
                    // left force
                    totalForceTorque.Torque += PhysicsEngine.CalcTorque(oSteeringBody.FrontPosition, -vYawForce);
                    // right force
                    totalForceTorque.Torque += PhysicsEngine.CalcTorque(oSteeringBody.BackPosition, vYawForce);
                }
#endif

#if DEBUG
                if (oInput.Shoot == true && m_bShoot == false)
                {
                    m_bShoot = true;
                    if (World.Instance.PlayersShip == oRigidBody.entity) Sound.PlayCue(Sounds.shot);
                }
                else if (oInput.Shoot == false && m_bShoot == true)
                {
                    m_bShoot = false;
                }
#endif

                //if (oInput.Acceleration > 0.0f)
                //{
                //    Vector3 vForce = Vector3.UnitZ * oSteeringBody.ThrustFactor;
                //    Vector3 vPosition = oSteeringBody.BackPosition;
                //    oForceTorque.Force += Vector3.Transform(-vForce, oRigidBody.CurState.m_matOrientation);
                //}

                //if (oInput.Acceleration < 0.0f)
                //{
                //    Vector3 vForce = Vector3.UnitZ * oSteeringBody.ThrustFactor;
                //    Vector3 vPosition = oSteeringBody.BackPosition;
                //    oForceTorque.Force += Vector3.Transform(vForce, oRigidBody.CurState.m_matOrientation);
                //}

                /*if (oSteeringBody.m_bPitchUp)
                {
                    Vector3 vForce = Vector3.UnitY * oSteeringBody.SteeringFactor;

                    // front force
                    Vector3 vPosition = oSteeringBody.FrontPosition;
                    oForceTorque.Torque += PhysicsEngine.CalcTorque(vPosition, vForce);

                    // back force
                    vPosition = oSteeringBody.BackPosition;
                    oForceTorque.Torque += PhysicsEngine.CalcTorque(vPosition, -vForce);
                }

                if (oSteeringBody.m_bPitchDown)
                {
                    Vector3 vForce = Vector3.UnitY * oSteeringBody.SteeringFactor;

                    // front force
                    Vector3 vPosition = oSteeringBody.FrontPosition;
                    oForceTorque.Torque += PhysicsEngine.CalcTorque(vPosition, -vForce);

                    // back force
                    vPosition = oSteeringBody.BackPosition;
                    oForceTorque.Torque += PhysicsEngine.CalcTorque(vPosition, vForce);
                }*/

                //if (oInput.Yaw < 0.0f)
                //{
                //    Vector3 vForce = Vector3.UnitZ * oSteeringBody.SteeringFactor;

                //    // left force
                //    Vector3 vPosition = oSteeringBody.LeftPosition;
                //    oForceTorque.Torque += PhysicsEngine.CalcTorque(vPosition, vForce);

                //    // right force
                //    vPosition = oSteeringBody.RightPosition;
                //    oForceTorque.Torque += PhysicsEngine.CalcTorque(vPosition, -vForce);
                //}

                //if (oInput.Yaw > 0.0f)
                //{
                //    Vector3 vForce = Vector3.UnitZ * oSteeringBody.SteeringFactor;

                //    // left force
                //    Vector3 vPosition = oSteeringBody.LeftPosition;
                //    oForceTorque.Torque += PhysicsEngine.CalcTorque(vPosition, -vForce);

                //    // right force
                //    vPosition = oSteeringBody.RightPosition;
                //    oForceTorque.Torque += PhysicsEngine.CalcTorque(vPosition, vForce);
                //}

                /*if (oSteeringBody.m_bRollRight)
                {
                    Vector3 vForce = Vector3.UnitY * oSteeringBody.SteeringFactor;

                    // left force
                    Vector3 vPosition = oSteeringBody.LeftPosition;
                    oForceTorque.Torque += PhysicsEngine.CalcTorque(vPosition, -vForce);

                    // right force
                    vPosition = oSteeringBody.RightPosition;
                    oForceTorque.Torque += PhysicsEngine.CalcTorque(vPosition, vForce);
                }

                if (oSteeringBody.m_bRollLeft)
                {
                    Vector3 vForce = Vector3.UnitY * oSteeringBody.SteeringFactor;

                    // left force
                    Vector3 vPosition = oSteeringBody.LeftPosition;
                    oForceTorque.Torque += PhysicsEngine.CalcTorque(vPosition, vForce);

                    // right force
                    vPosition = oSteeringBody.RightPosition;
                    oForceTorque.Torque += PhysicsEngine.CalcTorque(vPosition, -vForce);
                }*/
            }
            
        }


        #endregion
    }
}