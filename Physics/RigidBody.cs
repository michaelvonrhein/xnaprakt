#region Using Statements
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using PraktWS0708.Physics;
using PraktWS0708.Logic;
using PraktWS0708.Entities;
#endregion

namespace PraktWS0708.Physics
{
    #region BoundingSphere

    public abstract class BoundingObject { }

    public class BoundingSphere : BoundingObject
    {
        #region Fields

        public float m_fRadius;   // radius for bounding sphere (force action, collision detection)

        #endregion

        #region Initialization

        public BoundingSphere(float fRadius)
        {
            this.m_fRadius = fRadius;
        }

        #endregion

        #region Properties

        public float Radius
        {
            get
            {
                return m_fRadius;
            }
        }

        #endregion
    }

    #endregion

    public class RigidBody : PhysicsPlugin
    {
        #region Enums & Structs & Classes

        #region RigidBodyState

        public class RigidBodyState
        {
            #region Fields

            public Vector3 m_vPosition;
            public Matrix m_matOrientation;

            public Vector3 m_vLinearVelocity;   // velocity for translating movement
            public Vector3 m_vAngularVelocity;  // velocity for rotating movement
            public Vector3 m_vAngularMomentum;  // angular momentum for rotating movement

            #endregion

            #region Methods

            public void Reset()
            {
                Reset(Vector3.Zero, Matrix.Identity);
            }

            public void Reset(Vector3 vPosition, Matrix matOrientation)
            {
                m_vPosition = vPosition;
                m_matOrientation = matOrientation;
                m_vLinearVelocity = Vector3.Zero;
                m_vAngularVelocity = Vector3.Zero;
                m_vAngularMomentum = Vector3.Zero;
            }

            #endregion

            #region Initialization

            public RigidBodyState()
            {
                m_vPosition = Vector3.Zero;
                m_matOrientation = Matrix.Identity;
                m_vLinearVelocity = Vector3.Zero;
                m_vAngularMomentum = Vector3.Zero;
            }

            #endregion
        }

        #endregion

        #endregion

        #region Fields


        public SortedList<Logic.PowerUpTypes, List<Logic.Powereffect>> powerUps;

        protected RigidBodyState m_CurState; // current state
        protected RigidBodyState m_DesState; // desired state

        protected float m_fMass; // total mass of moving object
        protected List<ForceCausingObject> m_ForceCausingObjects;
        protected Matrix m_matInvertedMomentOfInertia;

        protected Vector3 m_vMassCenter;    // [UNUSED] - set to (0,0,0)
        protected BoundingSphere m_BoundingSphere;   // bounding object (force action, collision detection)
        
        public int m_iCollisionSearch;  // tube search index for collision
        public TrackCollisionResult m_CollisionResult;
        public float m_fRestitutionCoefficient;

        public PhysicsPluginFlags m_Flags;

        public bool m_bHidden;

        public BoundingBoxTree m_BoundingBoxTree;

        public int hasTurbo = 0;

        #endregion

        #region Initialization

        // Vector3 vPosition, Matrix matOrientation, float fMass, Vector3 vMassCenter, BoundingSphere oBoundingSphere, float fRestitutionCoefficient
        public RigidBody(PhysicsPluginData oPhysicsPluginData, Entities.BaseEntity oEntity, BoundingBoxTree oBoundingBoxTree, PhysicsPluginType type)
            : base(oPhysicsPluginData, oEntity)
        {
            BoundingSphere oBoundingSphere;
            /*if (type == PhysicsPluginType.SolidBody) {
                oBoundingSphere = new BoundingSphere(CollisionDetection.calculateBoundingSphereRadius(oPhysicsPluginData.m_oaTriangles, oPhysicsPluginData.m_fScale));
            } else {
                oBoundingSphere = new BoundingSphere(0.3f);
            }*/
            oBoundingSphere = new BoundingSphere(CollisionDetection.calculateBoundingSphereRadius(oPhysicsPluginData.m_oaTriangles, oPhysicsPluginData.m_fScale));
            if (oEntity.LogicPlugin is Logic.Ship)
                oBoundingSphere.m_fRadius = oBoundingSphere.m_fRadius / 2f;

            m_bHidden = false;

            m_BoundingBoxTree = oBoundingBoxTree;
            powerUps = new SortedList<PowerUpTypes, List<Powereffect>>();
            m_CurState = new RigidBodyState();
            m_CurState.m_vPosition = oEntity.Position;
            m_CurState.m_matOrientation = oEntity.Orientation;
            m_CurState.m_vLinearVelocity = Vector3.Zero;
            m_CurState.m_vAngularMomentum = Vector3.Zero;

            m_DesState = new RigidBodyState();
            m_DesState.m_vPosition = oEntity.Position;
            m_DesState.m_matOrientation = oEntity.Orientation;
            m_DesState.m_vLinearVelocity = Vector3.Zero;
            m_DesState.m_vAngularMomentum = Vector3.Zero;

            m_fMass = oPhysicsPluginData.m_fMass;
            m_vMassCenter = oPhysicsPluginData.m_vMassCenter;
            m_Flags = oPhysicsPluginData.m_Flags;

            m_BoundingSphere = oBoundingSphere;
            m_fRestitutionCoefficient = oPhysicsPluginData.m_fRestitution;

            m_matInvertedMomentOfInertia = PhysicsEngine.CalcInvertedMomentOfInertia(this);
            m_CurState.m_vAngularVelocity = PhysicsEngine.CalcAngularVelocity(m_CurState.m_matOrientation, m_matInvertedMomentOfInertia, m_CurState.m_vAngularMomentum);
            m_DesState.m_vAngularVelocity = PhysicsEngine.CalcAngularVelocity(m_DesState.m_matOrientation, m_matInvertedMomentOfInertia, m_DesState.m_vAngularMomentum);

            m_ForceCausingObjects = new List<ForceCausingObject>();
            m_CollisionResult = new TrackCollisionResult();

            // add forces
            if (oPhysicsPluginData.m_Flags.m_bGravity)
            {
                Add(new TrackGravity(Entities.World.Instance.PhysicsSystem.TrackBody));
                //Add(new TubeGravity(Entities.World.Instance.PhysicsSystem.TrackBody));
            }
            if (oPhysicsPluginData.m_Flags.m_bDrag)
                Add(new AerodynamicDrag(oPhysicsPluginData.m_vDragFactor));
            
        }

        #endregion

        #region Properties

        public RigidBodyState CurState
        {
            get
            {
                return m_CurState;
            }
        }
        public RigidBodyState DesState
        {
            get
            {
                return m_DesState;
            }
        }

        public override Vector3 Position
        {
            get
            {
                return m_CurState.m_vPosition;
            }
        }

        public override Matrix Orientation
        {
            get
            {
                return m_CurState.m_matOrientation;
            }
        }

        public override float Velocity
        {
            get
            {
                return m_CurState.m_vLinearVelocity.Length();
            }
        }

        public override BoundingSphere BoundingSphere
        {
            get
            {
                return m_BoundingSphere;
            }
        }

        public float Mass
        {
            get
            {
                return m_fMass;
            }
        }

        public Matrix InvertedMomentOfInertia
        {
            get
            {
                return m_matInvertedMomentOfInertia;
            }
        }

        public List<ForceCausingObject> ForceCausingObjects
        {
            get
            {
                return m_ForceCausingObjects;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Add a ForceCausingObject to the RigidBody
        /// </summary>
        /// <param name="oForceCausingObject">ForceCausingObject instance</param>
        public void Add(ForceCausingObject oForceCausingObject)
        {
            m_ForceCausingObjects.Add(oForceCausingObject);
        }

        /// <summary>
        /// Switch state from desired to current.
        /// </summary>
        public void SwitchState()
        {
            if (m_Flags.m_bMoveable)
            {
                RigidBodyState oTempState = m_CurState;
                m_CurState = m_DesState;
                m_DesState = oTempState;
                // FIXME: maybe we have to lock these attributes somehow
                entity.Position = m_CurState.m_vPosition;
                entity.Orientation = m_CurState.m_matOrientation;
                // entity.Update(); // das entity-update liegt nun in der simloop
                
            }
        }

        public override void Reset()
        {
            m_CurState.Reset(entity.Position, entity.Orientation);
            m_DesState.Reset(entity.Position, entity.Orientation);
            m_iCollisionSearch = 0;
        }

        public override void Reset(int iCollisionSearch)
        {
            Reset();
            m_iCollisionSearch = iCollisionSearch;
        }

        //adds a powerup to a rigidbody, called from logicplugin.interact()
        public void addPowerUp(Logic.Powereffect powerUp)
        {
            Logic.PowerUpTypes p = powerUp.getType();
            powerUp.startEffect();
            try
            {
                powerUps[p].Add(powerUp);
            }
            catch (KeyNotFoundException)
            {
                //Console.WriteLine("ex1: " + e.ToString());
                powerUps.Add(p, new List<Powereffect>());
                powerUps[p].Add(powerUp);
            }
            if (powerUp is Turbo)
            {
                hasTurbo++;
            }
            
        }

        //updates the powerups and its effects. If the poweruptime is over, the powerup is removed
        public void updatePowerUpEffects(int timeMillies)
        {
           
            IEnumerator<KeyValuePair<Logic.PowerUpTypes, List<Logic.Powereffect>>> it = (IEnumerator<KeyValuePair<PowerUpTypes, List<Logic.Powereffect>>>) powerUps.GetEnumerator();
            while(it.MoveNext())
            {
                List<Powereffect> list = it.Current.Value;
                for (int i = 0; i < list.Count; i++) 
                {
                    if (list[i].getRemainingEffectDuration() <= 0)
                    {
                        if (list[i].getType() == PowerUpTypes.TURBO)
                        {
                            hasTurbo--;
                        }
                        list[i].endEffect();
                        list.RemoveAt(i);
                    }
                    else
                    {
                        list[i].update(timeMillies);
                    }
                }
            }
        }

        public override void destroyElement()
        {
            
        }

        public override Vector3 linearVelocity
        {
            get
            {
                return CurState.m_vLinearVelocity;
            }
        }

        #endregion
    }
}
