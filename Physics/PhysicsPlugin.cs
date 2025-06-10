#region Using Statements
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace PraktWS0708.Physics
{
    /// <summary>
    /// PhysicsPlugin is base class for all physics plugins (objects)
    /// is able to construct the plugins with a factory method.
    /// </summary>
    public abstract class PhysicsPlugin : Entities.EntityBehaviourPlugin
    {
        #region Enums & Structs & Classes

        #region PhysicsPluginType

        public enum PhysicsPluginType
        {
            Null,
            RigidBody,
            SolidBody,
            SteeringBody,
            PowerUp
        };

        #endregion

        #region PhysicsPluginFlags

        public struct PhysicsPluginFlags
        {
            #region Fields

            public bool m_bHover;
            public bool m_bDrag;
            public bool m_bGravity;
            public bool m_bMoveable;

            #endregion

            #region Initialization

            public PhysicsPluginFlags(bool bHover, bool bDrag, bool bGravity)
            {
                m_bHover = bHover;
                m_bDrag = bDrag;
                m_bGravity = bGravity;
                m_bMoveable = true;
            }

            #endregion
        }

        #endregion

        #region PhysicsPluginData

        public struct PhysicsPluginData
        {
            #region Fields

            public float m_fMass;
            public Vector3 m_vMassCenter;

            public PhysicsPluginFlags m_Flags;
            public Vector3 m_vDragFactor;

            public float m_fThrustFactor;
            public float m_fSteeringFactor;
            public float m_fRestitution;
            public VertexPositionNormalTexture[] m_oaTriangles;
            public float m_fScale;

            #endregion

            #region Initialization

            public PhysicsPluginData(float fMass, Vector3 vMassCenter, PhysicsPluginFlags oFlags,
                                     Vector3 vDragFactor, float fThrustFactor, float fSteeringFactor, float fRestitution, VertexPositionNormalTexture[] vaTriangles, float fScale)
            {
                m_fMass = fMass;
                m_vMassCenter = vMassCenter;
                m_Flags = oFlags;
                m_vDragFactor = vDragFactor;
                m_fThrustFactor = fThrustFactor;    // intervall [0, 0.01]
                m_fSteeringFactor = fSteeringFactor;    // intervall [0, 0.01]
                m_fRestitution = fRestitution;  // intervall [0, 1]
                m_oaTriangles = vaTriangles;
                m_fScale = fScale;
                
            }

            #endregion
        }

        #endregion

        #endregion

        #region Fields

        protected PhysicsPluginType m_Type;

        #endregion

        #region Properties

        public PhysicsPluginType Type
        {
            get
            {
                return m_Type;
            }
        }

        public virtual Vector3 Position
        {
            get
            {
                return this.entity.Position;
            }
        }

        public virtual Matrix Orientation
        {
            get
            {
                return this.entity.Orientation;
            }
        }

        public virtual float Velocity
        {
            get
            {
                return 0f;
            }
        }

        public virtual Vector3 linearVelocity
        {
            get
            {
                return Vector3.Zero;
            }
        }

        public virtual BoundingSphere BoundingSphere
        {
            get
            {
                return new BoundingSphere(0f);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Resets this object's Position and Orientation to that from the BaseEntity,
        /// and clears any velocities/momenta etc.
        /// </summary>
        public virtual void Reset() { }

        public virtual void Reset(int iCollisionSearch) { }

        #endregion

        #region Initialization

        public PhysicsPlugin(PhysicsPluginData oPhysicsPluginData, Entities.BaseEntity oEntity)
            : base(oEntity) { }

        public static PhysicsPlugin GetPlugin(PhysicsPluginType oPhysicsPluginType, PhysicsPluginData oPhysicsPluginData, Entities.BaseEntity oEntity)
        {
            PhysicsPlugin oPhysicsPlugin = null;
            switch (oPhysicsPluginType)
            {
                case PhysicsPluginType.RigidBody:
                    oPhysicsPluginData.m_Flags.m_bMoveable = true;
                    oPhysicsPlugin = new Physics.RigidBody(oPhysicsPluginData, oEntity, CollisionDetection.generateBoundingBoxTreeFromTriangles(oPhysicsPluginData.m_oaTriangles, oPhysicsPluginData.m_fScale, oEntity.Orientation, oEntity.Position), oPhysicsPluginType);
                    
                    break;
                case PhysicsPluginType.SolidBody:
                    oPhysicsPluginData.m_Flags.m_bMoveable = false;
                    oPhysicsPluginData.m_fMass = 1000000000;
                    //oPhysicsPlugin = new Physics.RigidBody(oPhysicsPluginData, oEntity, CollisionDetection.generateBoundingBoxTreeFromTriangles(oPhysicsPluginData.m_oaTriangles, oPhysicsPluginData.m_fScale, oEntity.Orientation,oEntity.Position), oPhysicsPluginType);
                    oPhysicsPlugin = new Physics.RigidBody(oPhysicsPluginData, oEntity, null, oPhysicsPluginType);
                    break;
                case PhysicsPluginType.SteeringBody:
                    //oPhysicsPlugin = new Physics.SteeringBody(oPhysicsPluginData, oEntity, CollisionDetection.generateBoundingBoxTreeFromTriangles(oPhysicsPluginData.m_oaTriangles, oPhysicsPluginData.m_fScale, oEntity.Orientation, oEntity.Position), oPhysicsPluginType);
                    oPhysicsPlugin = new Physics.SteeringBody(oPhysicsPluginData, oEntity, null, oPhysicsPluginType);
                    break;
                case PhysicsPluginType.PowerUp:
                    oPhysicsPluginData.m_Flags.m_bMoveable = false;
                    oPhysicsPluginData.m_fMass = 1000;
                    //oPhysicsPlugin = new Physics.RigidBody(oPhysicsPluginData, oEntity, CollisionDetection.generateBoundingBoxTreeFromTriangles(oPhysicsPluginData.m_oaTriangles, oPhysicsPluginData.m_fScale, oEntity.Orientation, oEntity.Position), oPhysicsPluginType);
                    oPhysicsPlugin = new Physics.RigidBody(oPhysicsPluginData, oEntity, null, oPhysicsPluginType);
                    break;
                default:
                    oPhysicsPlugin = new Physics.NullPlugin(oPhysicsPluginData, oEntity);
                    break;
            }
            oPhysicsPlugin.m_Type = oPhysicsPluginType;

            return oPhysicsPlugin;
        }

        public abstract void destroyElement();

        #endregion
    }
}
