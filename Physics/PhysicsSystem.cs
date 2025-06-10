#region Using Statements
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using PraktWS0708.Utils;
using PraktWS0708.Logic;
using PraktWS0708.Entities;
using PraktWS0708.AI;
#endregion

namespace PraktWS0708.Physics
{
    /// <summary>
    /// PhysicsSystem
    /// </summary>
    public class PhysicsSystem
    {
        #region Fields, structs

        private SortedList<int, RigidBody> m_RigidBody;    // list of moving objects
        private SortedList<int, RigidBody> m_NotMoveableBody;
        private TrackBody m_TrackBody;  // the track

        private struct Collision
        {
            public SphereCollisionResult sphereCollision;
            public MeshCollisionResult meshCollision;
        }

        #endregion

        #region Initialization

        /// <summary>
        /// PhysicsSystem constructor
        /// </summary>
        public PhysicsSystem()
        {
            m_RigidBody = new SortedList<int, RigidBody>();
            m_NotMoveableBody = new SortedList<int, RigidBody>();
            m_TrackBody = null;

        }

        #endregion

        #region Properties

        public TrackBody TrackBody
        {
            get
            {
                if (m_TrackBody == null) m_TrackBody = new TrackBody();
                return m_TrackBody;
            }
        }

        #endregion

        #region Add/Remove Objects

        /// <summary>
        /// Add PhysicsPlugin to PhysicsSystem
        /// </summary>
        /// <param name="iID">ObjectID of PhysicsPlugin</param>
        /// <param name="oRigidBody">RigidBody object</param>
        public void Add(int iID, PhysicsPlugin oPhysicsPlugin)
        {
            if (oPhysicsPlugin is SteeringBody)
            {
                m_RigidBody.Add(iID, (SteeringBody)oPhysicsPlugin);
            }
            else if (oPhysicsPlugin is RigidBody)
            {
                if (((RigidBody)oPhysicsPlugin).m_Flags.m_bMoveable)
                {
                    m_RigidBody.Add(iID, (RigidBody)oPhysicsPlugin);
                }
                else
                {
                    m_NotMoveableBody.Add(iID, (RigidBody)oPhysicsPlugin);
                }
            }
            if (oPhysicsPlugin is Physics.RigidBody)
            {
                RigidBody r = ((RigidBody)oPhysicsPlugin);
                r.m_iCollisionSearch = CollisionDetection.findRigidBodyInTrackHardcore(World.Instance.PhysicsSystem.TrackBody, r);
            }
        }

        /// <summary>
        /// Sets the current Track
        /// </summary>
        public void SetTrack()
        {
            m_TrackBody = new TrackBody();
        }

        /// <summary>
        /// Remove PhysicsPlugin from PhysicsSystem
        /// </summary>
        /// <param name="iID">ObjectID of PhysicsPlugin</param>
        public bool Remove(int iID)
        {
            bool removed = false;
            if (m_RigidBody.ContainsKey(iID)) removed = m_RigidBody.Remove(iID);
            if (m_NotMoveableBody.ContainsKey(iID)) removed = m_NotMoveableBody.Remove(iID);
            return removed;
        }

        #endregion

        #region Simulate

        /// <summary>
        /// simulates the PhysicsSystem for the given GameTime
        /// </summary>
        /// <param name="oGameTime">GameTime object</param>
        public void Simulate(GameTime oGameTime)
        {
            #region PerformanceEater
            //#if DEBUG
            PerformanceEater last = PerformanceMeter.Instance.PerfomanceEaterChange(PerformanceEater.Physics);
            //#endif
            #endregion

            int timeStep = Settings.Configuration.EngineSettings.PhysicsUpdateStepTime;
            int elapsedTime = oGameTime.ElapsedGameTime.Milliseconds;

            List<Collision> rigidCollisionList = new List<Collision>();
            //respawnPowerUps?
            Entities.World.Instance.powerUpManager.updatePowerUps(elapsedTime);
           
            //float time;
            for (int time = 0; time < elapsedTime; time += timeStep)
            {
                rigidCollisionList.Clear();
               
                //Simulate timeStep
                foreach (KeyValuePair<int, RigidBody> oKeyValuePair in m_RigidBody)
                {
                    Update(oKeyValuePair.Value, timeStep);
                }
                
                //16.5 %
                #region check for collisions
                //Check for collisions
                foreach (KeyValuePair<int, RigidBody> oKeyValuePair in m_RigidBody)
                {
                    if (oKeyValuePair.Value.entity.Equals(World.Instance.PlayersShip)) {
                        int ysdf=0;
                    }
                    if (!oKeyValuePair.Value.m_Flags.m_bMoveable)
                        continue;
                    //Check collision Rigid vs Track
                    oKeyValuePair.Value.m_CollisionResult = CollisionDetection.CheckCollisionSphereTrack(oKeyValuePair.Value, m_TrackBody);
                   
                    

                    if (oKeyValuePair.Value.m_CollisionResult.m_CollisionState == CollisionState.OutOfTrack)
                    {
                        // Peterchen pulls you inside if you are outside ;)
                        //oKeyValuePair.Value.CurState.m_vAngularVelocity += 0.1f * oKeyValuePair.Value.m_CollisionResult.m_vNormal;
                        oKeyValuePair.Value.DesState.m_vPosition += 0.1f * oKeyValuePair.Value.m_CollisionResult.m_vNormal;
                    }


                    #region Check collison moveable vs movable
                    //Check collision Rigid vs Rigid
                    foreach (KeyValuePair<int, RigidBody> oKeyValuePair2 in m_RigidBody)
                    {
                        if (oKeyValuePair.Key <= oKeyValuePair2.Key) break;

                       

                        //As there is a bugs within the collision above, a very simple collision detection does another check
                        if (CollisionDetection.CheckCollisionSphereSphereSimple(oKeyValuePair.Value, oKeyValuePair2.Value))
                        //if (collision.m_CollisionState == CollisionState.Interpenetrating || collision.m_CollisionState == CollisionState.Touching)
                        {
                            //Detect collision based on the bounding spheres
                            SphereCollisionResult collision = CollisionDetection.CheckCollisionSphereSphere(oKeyValuePair.Value, oKeyValuePair2.Value);

                            Collision col = new Collision();
                            col.sphereCollision = collision;
                            rigidCollisionList.Add(col); //we have a crash - save it

                            #region mesh-collision (commented)
                            /* As there are at the moment no flying mesh objects, this is not neccessary
                            //Determine type of collision objects
                            if (oKeyValuePair.Value is RigidBody && oKeyValuePair2.Value is RigidBody)
                            {
                                PhysicsPlugin p = oKeyValuePair.Value;
                                PhysicsPlugin p2 = oKeyValuePair2.Value;

                                //do we have also meshes to run our collision dectection on?
                                if (p.Type == PhysicsPlugin.PhysicsPluginType.SolidBody && (p2.Type == PhysicsPlugin.PhysicsPluginType.RigidBody || p2.Type == PhysicsPlugin.PhysicsPluginType.SteeringBody))
                                {
                                    rigidCollisionList.Remove(col);
                                    MeshCollisionResult collision2 = CollisionDetection.CheckCollisionSphereMesh(((RigidBody)oKeyValuePair.Value).m_BoundingBoxTree, ((RigidBody)oKeyValuePair2.Value).BoundingSphere, ((RigidBody)oKeyValuePair2.Value).DesState.m_vPosition);
                                    if (collision2.m_CollisionState == CollisionState.Interpenetrating || collision2.m_CollisionState == CollisionState.Error)
                                    {
                                        //Save the much more accurate mesh collision too
                                        rigidCollisionList.Remove(col);
                                        col.meshCollision = collision2;
                                        rigidCollisionList.Add(col);
                                    }
                                    continue;
                                }
                                if (p2.Type == PhysicsPlugin.PhysicsPluginType.SolidBody && (p.Type == PhysicsPlugin.PhysicsPluginType.RigidBody || p.Type == PhysicsPlugin.PhysicsPluginType.SteeringBody))
                                {
                                    rigidCollisionList.Remove(col);
                                    MeshCollisionResult collision2 = CollisionDetection.CheckCollisionSphereMesh(((RigidBody)oKeyValuePair2.Value).m_BoundingBoxTree, ((RigidBody)oKeyValuePair.Value).BoundingSphere, ((RigidBody)oKeyValuePair.Value).DesState.m_vPosition);
                                    if (collision2.m_CollisionState == CollisionState.Interpenetrating || collision2.m_CollisionState == CollisionState.Error)
                                    {
                                        //Save the much more accurate mesh collision too
                                        rigidCollisionList.Remove(col);
                                        col.meshCollision = collision2;
                                        rigidCollisionList.Add(col);
                                    }
                                    continue;
                                }
                            }*/
                            #endregion
                        }
                    }
                    #endregion

                    

                    #region Check collison moveable vs notmovable
                    //Check collision Rigid vs Rigid
                    foreach (KeyValuePair<int, RigidBody> oKeyValuePair2 in m_NotMoveableBody)
                    {
                        

                        
                        //As there is a bugs within the collision above, a very simple collision detection does another check
                        if (CollisionDetection.CheckCollisionSphereSphereSimple(oKeyValuePair.Value, oKeyValuePair2.Value))
                        //if (collision.m_CollisionState == CollisionState.Interpenetrating || collision.m_CollisionState == CollisionState.Touching)
                        {
                            
                    
                            //Detect collision based on the bounding spheres
                            SphereCollisionResult collision = CollisionDetection.CheckCollisionSphereSphere(oKeyValuePair.Value, oKeyValuePair2.Value);

                            
                            if (collision == null) continue;

                            Collision col = new Collision();
                            col.sphereCollision = collision;
                            

                            //If we collide with a PowerUp which does not need the collision results,
                            // pick it up and go ahead
                            LogicPlugin l1 = oKeyValuePair.Value.entity.LogicPlugin;
                            LogicPlugin l2 = oKeyValuePair2.Value.entity.LogicPlugin;
                            if (l2 is PowerUp)
                            {
                                        
                                    PowerUp p = (PowerUp)l2;
                                    if (l1 is Ship)
                                    {

                                        p.Interact(oKeyValuePair.Value);
                                        BaseEntity particle = EntityFactory.BuildParticleEntity(Particles.ParticleSystemType.PowerUp, 500, p.entity.Position, 0.2f, Vector4.One, Matrix.Identity);
                                        World.Instance.ParticleManager.Add(new Particles.ParticleSystemManagementData(particle, null, 650f, p.entity.Position, p.entity.Orientation));
                                    }
                                    if (!(l2 is Bomb)) continue;
                            }

                            rigidCollisionList.Add(col); //we have a crash - save it
                            
                            //Determine type of collision objects
                            if (oKeyValuePair.Value is RigidBody && oKeyValuePair2.Value is RigidBody)
                            {
                                PhysicsPlugin p = oKeyValuePair.Value;
                                PhysicsPlugin p2 = oKeyValuePair2.Value;

                                //do we have also meshes to run our collision dectection on?
                                if (p.Type == PhysicsPlugin.PhysicsPluginType.SolidBody && (p2.Type == PhysicsPlugin.PhysicsPluginType.RigidBody || p2.Type == PhysicsPlugin.PhysicsPluginType.SteeringBody))
                                {
                                    rigidCollisionList.Remove(col);
                                    BoundingSphere oHalfSphere = new BoundingSphere(((RigidBody)oKeyValuePair2.Value).BoundingSphere.m_fRadius);
                                    MeshCollisionResult collision2 = CollisionDetection.CheckCollisionSphereMesh(((RigidBody)oKeyValuePair.Value).m_BoundingBoxTree, oHalfSphere, ((RigidBody)oKeyValuePair2.Value).DesState.m_vPosition);
                                    if (collision2.m_CollisionState == CollisionState.Interpenetrating || collision2.m_CollisionState == CollisionState.Error)
                                    {
                                        //Save the much more accurate mesh collision too
                                        rigidCollisionList.Remove(col);
                                        col.meshCollision = collision2;
                                        rigidCollisionList.Add(col);
                                    }
                                    continue;
                                }
                                if (p2.Type == PhysicsPlugin.PhysicsPluginType.SolidBody && (p.Type == PhysicsPlugin.PhysicsPluginType.RigidBody || p.Type == PhysicsPlugin.PhysicsPluginType.SteeringBody))
                                {
                                    rigidCollisionList.Remove(col);
                                    BoundingSphere oHalfSphere = new BoundingSphere(((RigidBody)oKeyValuePair.Value).BoundingSphere.m_fRadius);
                                    MeshCollisionResult collision2 = CollisionDetection.CheckCollisionSphereMesh(((RigidBody)oKeyValuePair2.Value).m_BoundingBoxTree, oHalfSphere, ((RigidBody)oKeyValuePair.Value).DesState.m_vPosition);
                                    if (collision2.m_CollisionState == CollisionState.Interpenetrating || collision2.m_CollisionState == CollisionState.Error)
                                    {
                                        //Save the much more accurate mesh collision too
                                        rigidCollisionList.Remove(col);
                                        col.meshCollision = collision2;
                                        rigidCollisionList.Add(col);
                                        
                                    }
                                    continue;
                                }
                            }

                    #endregion
                        }
                    }
                   
                    //remove Powerups
                    Entities.World.Instance.powerUpManager.doRemovePowerUps();
                    
                }
#endregion

                

                //19.0 %

                #region Work through the collision lists

                foreach (Collision collisionResult in rigidCollisionList)
                {
                    if (collisionResult.sphereCollision == null) { continue; }
                    RigidBody r1 = collisionResult.sphereCollision.m_oRigidBody;
                    RigidBody r2 = collisionResult.sphereCollision.m_oRigidBody2;
                    bool explosion = false;

                    #region rocketCheck
                    
                    //Check whether one of the objects is a waiting rocket
                    if (r1.entity.LogicPlugin is Rocket && r2.entity.LogicPlugin is Ship) 
                    {
                        explosion = true;
                        Rocket ro1 = (Rocket)r1.entity.LogicPlugin;
                        if (ro1.getState() == Rocket.RocketState.WAITING)
                        {
                            ro1.switchState(Rocket.RocketState.HUNTING, r2.entity, oGameTime);
                            continue;
                        }
                       
                    }
                    if (r2.entity.LogicPlugin is Rocket && r1.entity.LogicPlugin is Bomb) continue;
                    if (r1.entity.LogicPlugin is Rocket && r2.entity.LogicPlugin is Bomb) continue;
                    if (r2.entity.LogicPlugin is Rocket && r1.entity.LogicPlugin is Ship) 
                    {
                        explosion = true;
                        Rocket ro2 = (Rocket)r2.entity.LogicPlugin;
                        if (ro2.getState() == Rocket.RocketState.WAITING)
                        {
                            ro2.switchState(Rocket.RocketState.HUNTING, r1.entity, oGameTime);
                            continue;
                        }
                    }
                    #endregion rocketCheck

                    if ((r1.entity.LogicPlugin is Ship && r2.entity.LogicPlugin is Ship) ||
                        (r1.entity.LogicPlugin is Ship && r2.entity.LogicPlugin is Obstacle) ||
                        (r1.entity.LogicPlugin is Obstacle && r2.entity.LogicPlugin is Ship) ||
                        (r1.entity.LogicPlugin is Ship && r2.entity.LogicPlugin is Wall) ||
                        (r1.entity.LogicPlugin is Wall && r2.entity.LogicPlugin is Ship))
                    {
                        World.Instance.PlayCue(Sounds.hit, (r1.Position + r2.Position) * 0.5f); // collision
                    }

                    if (r1.entity.LogicPlugin is Bomb || r2.entity.LogicPlugin is Bomb)
                        explosion = true;
                
                   

                    //Ask the logic whether one of the objects actually will respond on a crash
                    if (r1.entity.LogicPlugin.isResponding() || r2.entity.LogicPlugin.isResponding()) //if (doResponse && doResponse2) TODO
                    {
                          
                        if (collisionResult.meshCollision != null)
                        {
                            #region meshcollision
                            SolidObject s1 = (SolidObject)r1.entity.LogicPlugin;
                            SolidObject s2 = (SolidObject)r2.entity.LogicPlugin;
                            //Mesh collision with one object which is not moveable
                            if (r1.m_Flags.m_bMoveable != true)
                            {
                                bool reallyCrashed = CollisonResponse(r1, r2, collisionResult.meshCollision);

                                if (reallyCrashed && !explosion)
                                {
                                    //Cannot be a powerup
                                    s1.Interact(r2.entity, calcCollisionSpeed(r1, r2, collisionResult.sphereCollision.m_vNormal));
                                    s2.Interact(r1.entity, calcCollisionSpeed(r1, r2, collisionResult.sphereCollision.m_vNormal));
                                    r1.entity.LogicPlugin.IsColliding = true;
                                    r2.entity.LogicPlugin.IsColliding = true;
                                    
                                }
                            }
                            //Mesh collision with one object which is not moveable
                            if (r2.m_Flags.m_bMoveable != true)
                            {
                                bool reallyCrashed = CollisonResponse(r2, r1, collisionResult.meshCollision);
                                if (reallyCrashed && !explosion)
                                {
                                    s1.Interact(r2.entity, calcCollisionSpeed(r1, r2, collisionResult.sphereCollision.m_vNormal));
                                    s2.Interact(r1.entity, calcCollisionSpeed(r1, r2, collisionResult.sphereCollision.m_vNormal));
                                    r1.entity.LogicPlugin.IsColliding = true;
                                    r2.entity.LogicPlugin.IsColliding = true;
                                }
                            }
#endregion
                        }
                        else
                        {
                            #region sphereCollision
                            //SphereSphere collision
                            CollisonResponse(r1, r2, collisionResult.sphereCollision);
                            r1.entity.LogicPlugin.IsColliding = true;
                            r2.entity.LogicPlugin.IsColliding = true;
                            #region collisionlogic
                            //if (!(r1.entity.LogicPlugin is Logic.Obstacle || r2.entity.LogicPlugin is Logic.Obstacle || r1.entity.LogicPlugin is Logic.NullPlugin || r2.entity.LogicPlugin is Logic.NullPlugin))
                            {
                                if (!explosion)
                                {
                                    #region logicCollision
                                    try
                                    {
                                        SolidObject s1 = (SolidObject)r1.entity.LogicPlugin;
                                        s1.Interact(r1.entity, calcCollisionSpeed(r1, r2, collisionResult.sphereCollision.m_vNormal));
                                        
                                    }
                                    catch (Exception e)
                                    {
                                        Console.WriteLine("ex2: " + e.ToString());
                                    } //It was a bomb - no problem

                                    try
                                    {
                                        SolidObject s2 = (SolidObject)r2.entity.LogicPlugin;
                                        s2.Interact(r2.entity, calcCollisionSpeed(r1, r2, collisionResult.sphereCollision.m_vNormal));
                                    }
                                    catch (Exception e)
                                    {
                                        Console.WriteLine("ex3: " + e.ToString());
                                    } //It was a bomb - no problem
                                    #endregion
                                }
                                else
                                {
                                    if (r1.entity.LogicPlugin is Rocket || r1.entity.LogicPlugin is Bomb )
                                    {
                                        SolidObject s = (SolidObject) r2.entity.LogicPlugin;
                                        Explosive exp = (Explosive)(r1.entity.LogicPlugin);
                                        exp.explode(r2.entity);
                                        s.addDamageFromExplosion(r1.entity, exp.getExplosionDamage());
                                    }
                                    if (r2.entity.LogicPlugin is Rocket || r2.entity.LogicPlugin is Bomb)
                                    {
                                        SolidObject s = (SolidObject)r1.entity.LogicPlugin;
                                        Explosive exp = (Explosive)(r2.entity.LogicPlugin);
                                        exp.explode(r1.entity);
                                        s.addDamageFromExplosion(r2.entity, exp.getExplosionDamage());
                                    }
                                    
                               }
                           }
                           #endregion collisionlogic
                           #endregion sphereCollision
                       }
                    }
                }

                #endregion


                //19.2 %

                #region 6% Unimproveable
                // track collision response
                foreach (KeyValuePair<int, RigidBody> oKeyValuePair in m_RigidBody)
                {
                    if (oKeyValuePair.Value.m_Flags.m_bMoveable &&
                        oKeyValuePair.Value.m_CollisionResult.m_CollisionState != CollisionState.NoCollision)
                    {
                       
                        CollisonResponse(oKeyValuePair.Value, oKeyValuePair.Value.m_CollisionResult);
                       
                    }
                }
                #endregion
                //25.2 %
                //switch state
                foreach (KeyValuePair<int, RigidBody> oKeyValuePair in m_RigidBody)
                {
                    if (oKeyValuePair.Value.m_Flags.m_bMoveable)
                    {
                        if (oKeyValuePair.Value.m_Flags.m_bHover)
                        {
                            StickToTube(oKeyValuePair.Value, timeStep);
                        }
                        oKeyValuePair.Value.SwitchState();
                    }
                }
               
            }
            //25.8 %
            // update scenegraph
            for (int i = 0; i < m_RigidBody.Count; i++)
            {
                if(m_RigidBody.Values[i].m_Flags.m_bMoveable)
                    m_RigidBody.Values[i].entity.Update();

                if(m_RigidBody.Values[i].entity.LogicPlugin is Ship)
                    m_RigidBody.Values[i].updatePowerUpEffects(elapsedTime); //updates the PowerUp effects
            }
            //26.2%

            #region PerformanceEater
            //#if DEBUG
            PerformanceMeter.Instance.PerfomanceEaterChange(last);
            //#endif
            #endregion
        }

        #endregion

        #region Update

        /// <summary>
        /// update PhysicsSystem instance
        /// </summary>
        public void Update(RigidBody oRigidBody, float fElapsedGameTime)
        {
            if (oRigidBody.m_Flags.m_bMoveable)
            {
                // stores the total force/torque remaining
                ForceTorque oTotalForceTorque = new ForceTorque(Vector3.Zero, Vector3.Zero);
                //ForceTorque oCurrentForceTorque;    // stores force/torque of current force causing object
                
                #region 7% slightly improved with refs
                foreach (ForceCausingObject oForceCausingObject in oRigidBody.ForceCausingObjects)
                {
                    // calculate force/torque for current force causing object
                    /*oCurrentForceTorque = oForceCausingObject.CalcForceTorque(oRigidBody);

                    // sum up forces/torques
                    oTotalForceTorque.Force += oCurrentForceTorque.Force;
                    oTotalForceTorque.Torque += oCurrentForceTorque.Torque;*/
                    oForceCausingObject.CalcForceTorque(oRigidBody, ref oTotalForceTorque);
                }
                #endregion

                // integrate for the linear velocity
                oRigidBody.DesState.m_vLinearVelocity = oRigidBody.CurState.m_vLinearVelocity
                                                      + fElapsedGameTime * oTotalForceTorque.Force / oRigidBody.Mass;
                
                if (oRigidBody.m_Flags.m_bHover)
                {
                    HoverEffect(ref oRigidBody);
                }
                
                #region 10%
                // integrate for the angular momentum
                oRigidBody.DesState.m_vAngularMomentum = oRigidBody.CurState.m_vAngularMomentum
                                                       + fElapsedGameTime * oTotalForceTorque.Torque;

                // calculate desired position
                oRigidBody.DesState.m_vPosition = oRigidBody.CurState.m_vPosition
                                                + oRigidBody.CurState.m_vLinearVelocity * fElapsedGameTime;

                // calculate derivative orientation
                Matrix matDerivativeOrientation =
                    PhysicsEngine.CalcDerivativeOrientationMatrix(oRigidBody.CurState.m_vAngularVelocity);

                // calculate orientation
                oRigidBody.DesState.m_matOrientation = oRigidBody.CurState.m_matOrientation
                                                     + fElapsedGameTime * matDerivativeOrientation
                                                     * oRigidBody.CurState.m_matOrientation;
                
                // reorthogonalize orientation matrix
                oRigidBody.DesState.m_matOrientation =
                    PhysicsEngine.ReorthogonalizeMatrix(oRigidBody.DesState.m_matOrientation);
               
                // calculate angular velocity
                oRigidBody.DesState.m_vAngularVelocity =
                    PhysicsEngine.CalcAngularVelocity(oRigidBody.DesState.m_matOrientation,
                                                      oRigidBody.InvertedMomentOfInertia,
                                                      oRigidBody.DesState.m_vAngularMomentum);
                #endregion
            }
        }
        
        #endregion
        
        #region HoverEffect (cut velocity)

        /**
         * Hover-Effect is created by cutting the velocity component perpendicular to tube body
         */
        public void HoverEffect(ref RigidBody oRigidBody)
        {
            Vector3 vNormal = oRigidBody.m_CollisionResult.m_vNormal;
            int iStartIndex = oRigidBody.m_iCollisionSearch;
            int iEndIndex = (oRigidBody.m_iCollisionSearch + 1) % m_TrackBody.m_PositionOrientationRadius.Length;
            //Vector3 vTubeStart = m_TrackBody.m_PositionOrientationRadius[iStartIndex].vPosition;
            //Vector3 vTubeEnd = m_TrackBody.m_PositionOrientationRadius[iEndIndex].vPosition;
            Vector3 vTubeStart = m_TrackBody.m_PositionOrientationRadius[iStartIndex].vPosition
                           - oRigidBody.m_CollisionResult.m_vNormal
                           * m_TrackBody.m_PositionOrientationRadius[iStartIndex].fRadius;
            Vector3 vTubeEnd = m_TrackBody.m_PositionOrientationRadius[iEndIndex].vPosition
                             - oRigidBody.m_CollisionResult.m_vNormal
                             * m_TrackBody.m_PositionOrientationRadius[iEndIndex].fRadius;

            Vector3 vTubeDirection = vTubeEnd - vTubeStart;
            float fDistance = oRigidBody.m_CollisionResult.m_fDistance;

            float fCurrentRadius = m_TrackBody.m_PositionOrientationRadius[iStartIndex].fRadius
                + (m_TrackBody.m_PositionOrientationRadius[iEndIndex].fRadius
                - m_TrackBody.m_PositionOrientationRadius[iStartIndex].fRadius)
                * oRigidBody.m_CollisionResult.m_fT;

            //vNormal = CollisionDetection.calcPerpendicularDistanceLinePoint(oRigidBody.DesState.m_vPosition,vTubeStart,vTubeEnd).vLinkedVector;

            if (vNormal != Vector3.Zero
                && fDistance > fCurrentRadius - (oRigidBody.BoundingSphere.m_fRadius * 1.5f))
            //&& Vector3.Dot(vNormal, oRigidBody.DesState.m_vLinearVelocity) < 0)
            {
                vNormal.Normalize();

                // delete velocity component perpendicular to tube body
                Vector3 vUp = Vector3.Cross(Vector3.Cross(vTubeDirection, vNormal), vTubeDirection);
                Matrix matRotation = Matrix.Identity;
                Vector3 vLinearVelocity = Vector3.Zero;
                if (vUp.Length() != 0)
                {
                    vUp.Normalize();
                    matRotation = Matrix.CreateLookAt(Vector3.Zero, Vector3.Normalize(vTubeDirection), vUp);
                    vLinearVelocity = Vector3.Transform(oRigidBody.DesState.m_vLinearVelocity, matRotation);
                }
                float fLinearVelocity = vLinearVelocity.Length();
                if (vLinearVelocity.Y <= 0.0f) vLinearVelocity.Y = 0.00001f;

                // no velocity loss at curves
                if (vLinearVelocity != Vector3.Zero)
                {
                    vLinearVelocity.Normalize();
                    vLinearVelocity *= fLinearVelocity;
                }
                oRigidBody.DesState.m_vLinearVelocity = Vector3.Transform(vLinearVelocity, Matrix.Transpose(matRotation));
            }
        }

        #endregion

        #region StickToTube (set orientation)

        /// <summary>
        /// sets the RigidBody's at the tube if it would leave it
        /// </summary>
        public void StickToTube(RigidBody oRigidBody, float fElapsedGameTime)
        {
            if (!oRigidBody.m_Flags.m_bMoveable)
            {
                return;
            }

            TrackCollisionResult oCollisionResult = oRigidBody.m_CollisionResult;

            // linear interpolation between 2 tube direction vectors
            int iStartIndex = oRigidBody.m_iCollisionSearch;
            int iEndIndex = (oRigidBody.m_iCollisionSearch + 1) % m_TrackBody.m_PositionOrientationRadius.Length;
            int iNextIndex = (oRigidBody.m_iCollisionSearch + 2) % m_TrackBody.m_PositionOrientationRadius.Length;
            Vector3 vTubeStart = m_TrackBody.m_PositionOrientationRadius[iStartIndex].vPosition
                               - oRigidBody.m_CollisionResult.m_vNormal
                               * m_TrackBody.m_PositionOrientationRadius[iStartIndex].fRadius;
            Vector3 vTubeEnd = m_TrackBody.m_PositionOrientationRadius[iEndIndex].vPosition
                             - oRigidBody.m_CollisionResult.m_vNormal
                             * m_TrackBody.m_PositionOrientationRadius[iEndIndex].fRadius;
            /*Vector3 vTubeStart2 = m_TrackBody.m_PositionOrientationRadius[iStartIndex].vPosition
                                - oRigidBody.m_CollisionResult.m_vNormal
                                * m_TrackBody.m_PositionOrientationRadius[iStartIndex].fRadius;
            Vector3 vTubeEnd2 = m_TrackBody.m_PositionOrientationRadius[iEndIndex].vPosition
                              - oRigidBody.m_CollisionResult.m_vNormal
                              * m_TrackBody.m_PositionOrientationRadius[iEndIndex].fRadius;

            Vector3 vTubeDirection = Vector3.Normalize(Vector3.Lerp(
                vTubeEnd - vTubeStart, vTubeStart2 - vTubeEnd2, oCollisionResult.m_fT));*/
            Vector3 vTubeDirection = vTubeEnd - vTubeStart;

            Vector3 vNormal = -CollisionDetection.calcPerpendicularDistanceLinePoint(oRigidBody.DesState.m_vPosition, vTubeStart, vTubeEnd).vLinkedVector;

            float fCurrentRadius = m_TrackBody.m_PositionOrientationRadius[iStartIndex].fRadius
                                 + (m_TrackBody.m_PositionOrientationRadius[iEndIndex].fRadius
                                 - m_TrackBody.m_PositionOrientationRadius[iStartIndex].fRadius)
                                 * oRigidBody.m_CollisionResult.m_fT;

            float fDistance = oCollisionResult.m_fDistance;

            if (vNormal != Vector3.Zero)
            /*if (oCollisionResult.m_vNormal != Vector3.Zero
                && oCollisionResult.m_fDistance > 0.5f * m_TrackBody.m_PositionOrientationRadius[oRigidBody.m_iCollisionSearch].fRadius)// (oCollisionResult.m_CollisionState != CollisionState.NoCollision)*/
            {
                float fMaxAngle = 0.002f * fElapsedGameTime;

                Vector3 vUp = Vector3.Normalize(vNormal);

                if (oRigidBody.CurState.m_matOrientation.Up == vUp)
                {
                    Console.WriteLine("PIPAPO");
                    return;
                }
                
                Vector3 vRotAxis = Vector3.Normalize(Vector3.Cross(oRigidBody.CurState.m_matOrientation.Up, vUp));
                float fCosAngle = Vector3.Dot(oRigidBody.CurState.m_matOrientation.Up, vUp);
                if (fCosAngle > 1f)
                    fCosAngle = 1f;
                if (fCosAngle < -1f)
                    fCosAngle = -1f;

                float fAngle = (float)Math.Acos(fCosAngle);

                if (fAngle > fMaxAngle) fAngle = fMaxAngle;
                else if (fAngle < -fMaxAngle) fAngle = -fMaxAngle;

                

                Quaternion qRotation = Quaternion.CreateFromAxisAngle(vRotAxis, fAngle);
                qRotation.Normalize();

                oRigidBody.DesState.m_matOrientation =
                    Matrix.Transform(oRigidBody.DesState.m_matOrientation, qRotation);
            }
        }

        #endregion

        #region calcCollisionSpeed

        public float calcCollisionSpeed(RigidBody oRigidBody, RigidBody oRigidBody2, Vector3 vNormal)
        {
            if (vNormal == Vector3.Zero) return 0f;
            vNormal.Normalize();
            if (!oRigidBody.m_Flags.m_bMoveable)
            {
                oRigidBody.DesState.m_vLinearVelocity = Vector3.Zero;
            }
            if (!oRigidBody2.m_Flags.m_bMoveable)
            {
                oRigidBody2.DesState.m_vLinearVelocity = Vector3.Zero;
            }
            float speedAtoB = Vector3.Dot(vNormal, oRigidBody.DesState.m_vLinearVelocity);
            float speedBtoA = Vector3.Dot(vNormal, oRigidBody2.DesState.m_vLinearVelocity);
            return Math.Abs(speedAtoB - speedBtoA); //Evtl vorzeichenfehler
        }

        #endregion

        #region Collision Response

        #region RigidBody <-> TrackBody

        /// <summary>
        /// Calculates the collision response
        /// </summary>
        /// <param name="oRigidBody">RigidBody instance</param>
        /// <param name="oCollisionResult">Collision structure</param>
        public void CollisonResponse(RigidBody oRigidBody, TrackCollisionResult oCollisionResult)
        {
            
            if (!oRigidBody.m_Flags.m_bMoveable)
            {
                return;
            }
            Vector3 vNormal = -oCollisionResult.m_vNormal;
            Vector3 vCollisionPoint = oCollisionResult.m_vPoint;

            float fRestitutionCoefficient = oRigidBody.m_fRestitutionCoefficient;

            // linear velocity calculation
            float fCounter = Vector3.Dot(((1f + fRestitutionCoefficient) * oRigidBody.CurState.m_vLinearVelocity), vNormal);
            float fDenominator = Vector3.Dot(vNormal, (vNormal * (1f / oRigidBody.Mass)));
            float fImpuls = fCounter / fDenominator;

            Vector3 vLinearVelocity = oRigidBody.CurState.m_vLinearVelocity - fImpuls * vNormal / oRigidBody.Mass;
            oRigidBody.DesState.m_vLinearVelocity = vLinearVelocity;

            Vector3 vRadius = vCollisionPoint - oRigidBody.CurState.m_vPosition;

            // angular velocity / momentum calculation
            fCounter = Vector3.Dot(-((1f + fRestitutionCoefficient) * oRigidBody.CurState.m_vLinearVelocity), vNormal);
            float fTerm = Vector3.Dot(Vector3.Cross(Vector3.Transform(Vector3.Cross(vRadius, vNormal), oRigidBody.InvertedMomentOfInertia), vRadius), vNormal);
            fDenominator = Vector3.Dot(vNormal, (vNormal * (1f / oRigidBody.Mass))) + fTerm;
            fImpuls = fCounter / fDenominator;

            

            #region 0.8%
            oRigidBody.DesState.m_vAngularMomentum =
                oRigidBody.CurState.m_vAngularMomentum
              - Vector3.Transform(Vector3.Cross(vRadius, fImpuls * vNormal),
                                  Matrix.Transpose(oRigidBody.CurState.m_matOrientation));
            #endregion
            #region 4.2%
            
            oRigidBody.DesState.m_vAngularVelocity =
               // Vector3.Transform(oRigidBody.DesState.m_vAngularMomentum, oRigidBody.Orientation * oRigidBody.InvertedMomentOfInertia * Matrix.Transpose(oRigidBody.Orientation));
                PhysicsEngine.CalcAngularVelocity(oRigidBody.CurState.m_matOrientation,
                oRigidBody.InvertedMomentOfInertia,
                oRigidBody.DesState.m_vAngularMomentum);
           
            #endregion 4.2%
           
        }

        #endregion

        #region RigidBody <-> RigidBody

        /// <summary>
        /// Calculates the collision response
        /// </summary>
        /// <param name="oRigidBody">RigidBody instance</param>
        /// <param name="oRigidBody2">RigidBody2 instance</param>
        /// <param name="oCollisionResult">Collision structure</param>
        public void CollisonResponse(RigidBody oRigidBody, RigidBody oRigidBody2, SphereCollisionResult oCollisionResult)
        {
            if (!oRigidBody.m_Flags.m_bMoveable && !oRigidBody2.m_Flags.m_bMoveable)
            {
                return;
            }

            Vector3 vNormal = oCollisionResult.m_vNormal;
            vNormal = Vector3.Normalize(vNormal);
            Vector3 vCollisionPoint = oCollisionResult.m_vPoint;

            //impulse calculation
            float minRestitution = Math.Min(oRigidBody.m_fRestitutionCoefficient, oRigidBody2.m_fRestitutionCoefficient);
            if (!oRigidBody.m_Flags.m_bMoveable)
            {
                oRigidBody.DesState.m_vLinearVelocity = Vector3.Zero;
            }
            if (!oRigidBody2.m_Flags.m_bMoveable)
            {
                oRigidBody2.DesState.m_vLinearVelocity = Vector3.Zero;
            }
            float speedAtoB = Vector3.Dot(vNormal, oRigidBody.DesState.m_vLinearVelocity);
            float speedBtoA = Vector3.Dot(vNormal, oRigidBody2.DesState.m_vLinearVelocity);
            if (speedAtoB == float.NaN)
            {
                speedAtoB = 0f;
            }
             if (speedBtoA == float.NaN)
            {
                speedBtoA = 0f;
            }
            float fRelativeSpeed = speedAtoB - speedBtoA; //Evtl vorzeichenfehler

            //impulseCounter
            float fCounter = (1f + minRestitution) * fRelativeSpeed;

            //impulseDenominator 
            float fInverseMasses = 1f / oRigidBody.Mass + 1f / oRigidBody2.Mass;

            Vector3 nxPosA = Vector3.Cross(vNormal, vCollisionPoint - oRigidBody.DesState.m_vPosition);
            Vector3 nxPosB = Vector3.Cross(vNormal, vCollisionPoint - oRigidBody2.DesState.m_vPosition);

            Vector3 IAtransNXPOSA = Vector3.Cross(Vector3.Transform(nxPosA, oRigidBody.InvertedMomentOfInertia), vCollisionPoint - oRigidBody.DesState.m_vPosition);
            Vector3 IAtransNXPOSB = Vector3.Cross(Vector3.Transform(nxPosB, oRigidBody.InvertedMomentOfInertia), vCollisionPoint - oRigidBody2.DesState.m_vPosition);

            float fDenominator = fInverseMasses + Vector3.Dot(vNormal, IAtransNXPOSA) + Vector3.Dot(vNormal, IAtransNXPOSB);

            float factor = 1;
            if (oRigidBody.entity.LogicPlugin is Bomb || oRigidBody.entity.LogicPlugin is Rocket)
            {
                factor *= ((Explosive)oRigidBody.entity.LogicPlugin).getExplosionDamage();
                if (oRigidBody2.entity.LogicPlugin is SolidObject)
                {
                    SolidObject obj = (SolidObject)oRigidBody2.entity.LogicPlugin;
                    obj.Interact(oRigidBody.entity, 0f);
                }
            }
            if (oRigidBody2.entity.LogicPlugin is Bomb || oRigidBody2.entity.LogicPlugin is Rocket)
            {
                factor *= ((Explosive)oRigidBody2.entity.LogicPlugin).getExplosionDamage();
                if (oRigidBody.entity.LogicPlugin is SolidObject)
                {
                    SolidObject obj = (SolidObject)oRigidBody.entity.LogicPlugin;
                    obj.Interact(oRigidBody2.entity, 0f);
                }
            }
         

            Vector3 vImpulseA = Vector3.Zero;
            Vector3 vImpulseB = Vector3.Zero;
            float fImpuls = Math.Abs(fCounter / fDenominator);
            if (factor > 1)
            {
                //BOMBE
                vImpulseA = -Vector3.Normalize(vNormal) * Math.Sign(fImpuls) * factor;
                vImpulseB = Vector3.Normalize(vNormal) * Math.Sign(fImpuls) * factor;
            }
            else
            {
                vImpulseA = -Vector3.Normalize(vNormal) * fImpuls;
                vImpulseB = Vector3.Normalize(vNormal) * fImpuls;
            }

            Vector3 vLinearVelocityA = Vector3.Zero;
            Vector3 vLinearVelocityB = Vector3.Zero; 

            if (((Vector3)(oRigidBody.DesState.m_vLinearVelocity - vImpulseA / oRigidBody.Mass)).Length() > 0.1)
            {
                Vector3 temp = (oRigidBody.DesState.m_vLinearVelocity - vImpulseA / oRigidBody.Mass);
                 vLinearVelocityA =  Vector3.Normalize(temp)*0.1f;
            }
            if (((Vector3)(oRigidBody2.DesState.m_vLinearVelocity - vImpulseB / oRigidBody2.Mass)).Length() > 0.1)
            {
                Vector3 temp = (oRigidBody2.DesState.m_vLinearVelocity - vImpulseB / oRigidBody2.Mass);
                vLinearVelocityB = Vector3.Normalize(temp) * 0.1f;
            }
            else
            {
                //set linear velocity
                vLinearVelocityA = oRigidBody.DesState.m_vLinearVelocity - vImpulseA / oRigidBody.Mass;
               
                vLinearVelocityB = oRigidBody2.DesState.m_vLinearVelocity - vImpulseB / oRigidBody2.Mass;
                
                //Console.WriteLine(vImpulseA);
            }
            oRigidBody.DesState.m_vLinearVelocity = vLinearVelocityA;
            oRigidBody2.DesState.m_vLinearVelocity = vLinearVelocityB;

            vImpulseA = -Vector3.Normalize(vNormal) * fImpuls;
            vImpulseB = Vector3.Normalize(vNormal) * fImpuls;

            // angular velocity / momentum calculation
            #region Angular Velocity
            if (oRigidBody.m_Flags.m_bMoveable)
            {
                Vector3 angVeloA = oRigidBody.DesState.m_vAngularVelocity - Vector3.Transform(Vector3.Cross(vImpulseA, vCollisionPoint - oRigidBody.DesState.m_vPosition), oRigidBody.InvertedMomentOfInertia);
                oRigidBody.DesState.m_vAngularMomentum = oRigidBody.DesState.m_vAngularMomentum
                  - Vector3.Cross(vCollisionPoint - oRigidBody.DesState.m_vPosition, vNormal * vImpulseA);
                oRigidBody.DesState.m_vAngularVelocity =
                    PhysicsEngine.CalcAngularVelocity(
                    oRigidBody.DesState.m_matOrientation,
                    oRigidBody.InvertedMomentOfInertia,
                    oRigidBody.DesState.m_vAngularMomentum);
            }
            if (oRigidBody2.m_Flags.m_bMoveable)
            {
                Vector3 angVeloB = oRigidBody2.DesState.m_vAngularVelocity - Vector3.Transform(Vector3.Cross(vImpulseB, vCollisionPoint - oRigidBody2.DesState.m_vPosition), oRigidBody2.InvertedMomentOfInertia);
                oRigidBody2.DesState.m_vAngularMomentum = oRigidBody.DesState.m_vAngularMomentum
                 - Vector3.Cross(vCollisionPoint - oRigidBody2.DesState.m_vPosition, vNormal * vImpulseB);
                oRigidBody2.DesState.m_vAngularVelocity =
                    PhysicsEngine.CalcAngularVelocity(
                    oRigidBody.DesState.m_matOrientation,
                    oRigidBody.InvertedMomentOfInertia,
                    oRigidBody.DesState.m_vAngularMomentum);
            }


            #endregion
        }

        #endregion

        #region RigidBodySphere <-> RigidBodyMesh

        /// <summary>
        /// Calculates the collision response on a crash from a sphere on a unmoveable
        /// object with a mesh
        /// </summary>
        /// <param name="oRigidBody">RigidBody instance, unmoveable, with sphere</param>
        /// <param name="oRigidBody2">RigidBody2 instance, moveable, with boundingsphere</param>
        /// <param name="oCollisionResult">Collision structure</param>
        public bool CollisonResponse(RigidBody oNotMoveableMeshBody, RigidBody oSphereBody, MeshCollisionResult oCollisionResult)
        {
            //The object with the mesh must be not moveable
            if (!oSphereBody.m_Flags.m_bMoveable && oNotMoveableMeshBody.m_Flags.m_bMoveable)
            {
                return false;
            }

            MeshCollisionPoint[] collisionPoints = oCollisionResult.m_CollisionPoints;

            //List to save all the crashes to consider
            List<MeshCollisionPoint> crashList = new List<MeshCollisionPoint>();

            for (int i = 0; i < collisionPoints.Length; i++)
            {
                //calculate the normal out of the triangle
                //the triangles are sorted because they are taken of the mesh
                Vector3[] triangle = collisionPoints[i].m_vaTriangle;
                collisionPoints[i].m_vNormal = Vector3.Cross(triangle[1] - triangle[0], triangle[2] - triangle[1]);

                if (crashList.Count == 0)
                {
                    crashList.Add(collisionPoints[i]);
                }
                else
                {
                    //Test whether the same normal is already in the list
                    for (int j = 0; j < crashList.Count; j++)
                    {
                        MeshCollisionPoint point = crashList[j];

                        //calculate the angle between the two normals 
                        if (0.9 < Math.Abs(Vector3.Dot(collisionPoints[i].m_vNormal, (point.m_vNormal)) / collisionPoints[i].m_vNormal.Length() / point.m_vNormal.Length()))
                        {
                            //almost the same - interpolate
                            crashList[j].m_CollisionPoint = (point.m_CollisionPoint + collisionPoints[i].m_CollisionPoint) / 2;
                            crashList[j].m_fDistance = (point.m_fDistance + collisionPoints[i].m_fDistance) / 2;
                        }
                        else
                        {
                            //new crashpoint
                            crashList.Add(collisionPoints[i]);
                        }
                    }
                }

            }

            //now the crashlist contains only different normals
            //interpolate between all this crashpoints
            Vector3 sumNormal = Vector3.Zero;
            Vector3 sumPoint = Vector3.Zero;
            double sumDistance = 0;
            for (int i = 0; i < crashList.Count; i++)
            {
                sumNormal += crashList[i].m_vNormal;
                sumPoint += crashList[i].m_CollisionPoint;
                sumDistance += sumDistance;
            }
            MeshCollisionPoint crashpoint = crashList[0];
            crashpoint.m_CollisionPoint = sumPoint / crashList.Count;
            crashpoint.m_fDistance = (float)sumDistance / (float)crashList.Count;
            crashpoint.m_vNormal = sumNormal / crashList.Count;
            crashList.Clear();

            //perform the actual crash
            return crashCalculator(crashpoint, oSphereBody, oNotMoveableMeshBody);

        }


        //Performes a crash given two bodys (mesh- and sphereboundingboxes), a normal and a crashpoint 
        private bool crashCalculator(MeshCollisionPoint collisionPoint, RigidBody oSphereBody, RigidBody oNotMoveableMeshBody)
        {

            Vector3 vNormal = Vector3.Normalize(collisionPoint.m_vNormal);
            Vector3 vCollisionPoint = collisionPoint.m_CollisionPoint;
            RigidBody oRigidBody = oSphereBody;
            RigidBody oRigidBody2 = oNotMoveableMeshBody;

            //Console.WriteLine("NORMAL COLLISION");
            float speedAtoB = Vector3.Dot(vNormal, oRigidBody.DesState.m_vLinearVelocity);
            float fRelativeSpeed = speedAtoB; //Evtl vorzeichenfehler

            //impulseCounter
            float fCounter = (1f + oRigidBody.m_fRestitutionCoefficient) * fRelativeSpeed;

            //impulseDenominator 
            float fInverseMasses = 1f / oRigidBody.Mass + 0f;

            Vector3 nxPosA = Vector3.Cross(vNormal, vCollisionPoint - oRigidBody.DesState.m_vPosition);
            //Vector3 nxPosB = Vector3.Cross(vNormal, vCollisionPoint - oRigidBody2.Position);

            Vector3 IAtransNXPOSA = Vector3.Cross(Vector3.Transform(nxPosA, oRigidBody.InvertedMomentOfInertia), vCollisionPoint - oRigidBody.DesState.m_vPosition);
            //Vector3 IAtransNXPOSB = Vector3.Cross(Vector3.Transform(nxPosB, oRigidBody.InvertedMomentOfInertia), vCollisionPoint - oRigidBody2.DesState.m_vPosition);

            float fDenominator = fInverseMasses + Vector3.Dot(vNormal, IAtransNXPOSA); // +Vector3.Dot(vNormal, IAtransNXPOSB);
            float fImpuls = Math.Abs(fCounter / fDenominator);
            Vector3 vImpulseA = -Vector3.Normalize(vNormal) * fImpuls;

            double velocityLength = oRigidBody.DesState.m_vLinearVelocity.Length();
            //set linear velocity

            Vector3 vLinearVelocityA = oRigidBody.DesState.m_vLinearVelocity - vImpulseA / oRigidBody.Mass;
            vLinearVelocityA.Normalize();

            //Problem: The Detection finds a collision but the collision repsonse was already performed
            //Solution: If the impulse speeds the ship up, this is a wrong call - do nothing!
            if ((vLinearVelocityA * (float)velocityLength * 1.2f).LengthSquared() < (oRigidBody.DesState.m_vLinearVelocity - vImpulseA / oRigidBody.Mass).LengthSquared())
            {
                
               //return false;
            }
            
            oRigidBody.DesState.m_vLinearVelocity = vLinearVelocityA * Math.Min(0.1f, (float)velocityLength);


            // angular velocity / momentum calculation

            if (oRigidBody.m_Flags.m_bMoveable)
            {
                Vector3 angVeloA = oRigidBody.DesState.m_vAngularVelocity - Vector3.Transform(Vector3.Cross(vImpulseA, vCollisionPoint - oRigidBody.DesState.m_vPosition), oRigidBody.InvertedMomentOfInertia);
                oRigidBody.DesState.m_vAngularMomentum = oRigidBody.DesState.m_vAngularMomentum
                  - Vector3.Cross(vCollisionPoint - oRigidBody.DesState.m_vPosition, vNormal * vImpulseA);
                oRigidBody.DesState.m_vAngularVelocity =
                    PhysicsEngine.CalcAngularVelocity(
                    oRigidBody.DesState.m_matOrientation,
                    oRigidBody.InvertedMomentOfInertia,
                    oRigidBody.DesState.m_vAngularMomentum);
            }

            return true;
        }
        #endregion

        #endregion
    }
}
