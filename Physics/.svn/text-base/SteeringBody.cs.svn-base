#region Using Statements
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using PraktWS0708.Entities;
#endregion

namespace PraktWS0708.Physics
{
    class SteeringBody : RigidBody
    {
        #region Fields

        private float m_fThrustFactor;
        private float m_fSteeringFactor;
        
        #endregion

        #region Initialization

        /// <summary>
        /// SteeringBody constructor
        /// </summary>
        /// <param name="oPhysicsPluginData">special physics data has be stored in this object</param>
        /// <param name="oEntity">BaseEntity object</param>
        public SteeringBody(PhysicsPluginData oPhysicsPluginData, Entities.BaseEntity oEntity, BoundingBoxTree oBoundingBoxTree, PhysicsPluginType type)
            : base(oPhysicsPluginData, oEntity, oBoundingBoxTree, type)
        {
            // add forces
            Add(new ThrustSteering());

            m_fThrustFactor = oPhysicsPluginData.m_fThrustFactor;
            m_fSteeringFactor = oPhysicsPluginData.m_fSteeringFactor;
        }

        #endregion

        #region Properties

        public float ThrustFactor
        {
            get
            {
                return m_fThrustFactor;
            }
            set
            {
                m_fThrustFactor = value;
            }
        }

        public float SteeringFactor
        {
            get
            {
                return m_fSteeringFactor;
            }
            set
            {
                m_fSteeringFactor = value;
            }
        }

        #region Positions

        public Vector3 BackPosition
        {
            get
            {
                return new Vector3(m_vMassCenter.X, m_vMassCenter.Y, m_vMassCenter.Z - BoundingSphere.m_fRadius);
            }
        }

        public Vector3 FrontPosition
        {
            get
            {
                return new Vector3(m_vMassCenter.X, m_vMassCenter.Y, m_vMassCenter.Z + BoundingSphere.m_fRadius);
            }
        }

        public Vector3 LeftPosition
        {
            get
            {
                return new Vector3(m_vMassCenter.X - BoundingSphere.m_fRadius, m_vMassCenter.Z, m_vMassCenter.Y);
            }
        }

        public Vector3 RightPosition
        {
            get
            {
                return new Vector3(m_vMassCenter.X + BoundingSphere.m_fRadius, m_vMassCenter.Y, m_vMassCenter.Z);
            }
        }

        #endregion

        public override void destroyElement()
        {
            m_fThrustFactor = 0;
            m_fSteeringFactor = 0;
            
            BaseEntity entAni = EntityFactory.BuildParticleEntity(Particles.ParticleSystemType.Billboard, 2, this.entity.Position, 0.375f, Vector4.One, this.entity.Orientation);
            BaseEntity entParticle = EntityFactory.BuildParticleEntity(Particles.ParticleSystemType.Default, 1000, this.entity.Position, 0.1f, Vector4.One, this.entity.Orientation);
            World.Instance.ParticleManager.Add(new Particles.ParticleSystemManagementData(entAni, null, World.Instance.ParticleManager.GetEffectTime(Particles.ParticleSystemType.Explosion), this.entity.Position, Matrix.Identity));
            World.Instance.ParticleManager.Add(new Particles.ParticleSystemManagementData(entParticle, null, World.Instance.ParticleManager.GetEffectTime(Particles.ParticleSystemType.Explosion), this.entity.Position, Matrix.Identity));
            //if (this.entity == World.Instance.PlayersShip)
            World.Instance.PlayCue(Sounds.explosion_long, this.Position);

            World.Instance.RemoveModel(this.entity);
            //World.Instance.PhysicsSystem.Remove(this.entity.ObjectID);
            //this.entity.RenderingPlugin.Hidden = true;
            //m_bHidden = true;
            if(World.Instance.PlayersShip.Equals(this.entity))
                World.Instance.ViewType= ViewTypes.THIRDPERSON;
        }

        #endregion

       

    }
}

