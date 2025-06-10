using System;
using System.Collections.Generic;
using System.Text;
using PraktWS0708.Physics;
using PraktWS0708.Entities;
using Microsoft.Xna.Framework;

namespace PraktWS0708.Logic 
{
    /// <summary>
    /// The logic of a ship. For example a ship crashes and has a certain amount of health.
    /// A ship can explode if the health is lower than 0.
    /// </summary>
    class Ship : SolidObject
    {
        #region Fields

        private float fMaxHealth;
        private float fDamageResistence;
        private float fHealth;
        private bool bIsColliding;
        private double dLastCollision;

        private int iNumThruster;
        private Vector3[] vaThrustRelativePositions;
        private Matrix[] maThrustOrientations;
        private Vector3[] vaThrustScales;

        public int shieldCounter = 0;
        public int rocketCounter = 0;

        #endregion

        #region Initialization


        public Ship(PraktWS0708.Logic.LogicPlugin.LogicPluginData oLogicPluginData, Entities.BaseEntity e)
            : base(oLogicPluginData, e)
        {
            fHealth = fMaxHealth = oLogicPluginData.m_fMaxHealth;
            fDamageResistence = oLogicPluginData.m_fDamageResistence;

            iNumThruster = oLogicPluginData.m_iNumThruster;
            vaThrustRelativePositions = oLogicPluginData.m_vaThrustRelativePositions;
            maThrustOrientations = oLogicPluginData.m_maThrustOrientations;
            vaThrustScales=oLogicPluginData.m_vaThrustScales;

            if (this.entity.PhysicsPlugin.Type == PhysicsPlugin.PhysicsPluginType.SteeringBody)
            {
                for (int i = 0; i < iNumThruster; i++)
                {
                    BaseEntity oThrustPS = EntityFactory.BuildParticleEntity(Particles.ParticleSystemType.Thrust, 300, Vector3.Zero, 0.02f, new Vector4(vaThrustScales[i].X, vaThrustScales[i].Y, vaThrustScales[i].Z, 4f), Matrix.Identity);
                    World.Instance.ParticleManager.Add(new Particles.ParticleSystemManagementData(oThrustPS, this.entity, -1, vaThrustRelativePositions[i] * this.entity.Scale, maThrustOrientations[i]));
                }
            }
            if (entity.PhysicsPlugin is RigidBody)
            {
                RigidBody r = (RigidBody)entity.PhysicsPlugin;
                r.BoundingSphere.m_fRadius = r.BoundingSphere.m_fRadius / 2f;
            }

        }

        #endregion

        #region Properties

        public override float Health
        {
            get
            {
                return fHealth;
            }
        }

        public override float MaxHealth
        {
            get
            {
                return fMaxHealth;
            }
        }

        public override bool IsColliding
        {
            get
            {
                bool result = bIsColliding;
                bIsColliding = false;
                return result;
            }
            set
            {
                //Console.WriteLine("set IsColliding");
                bIsColliding = value;
            }
        }

        #endregion

        public  void Interact(float damage)
        {
           

            fHealth -= damage;
            if (damage <= 0)
                bIsColliding = false;
            else bIsColliding = true;
            if (fHealth <= 0)
            {
                fHealth = 0f;
                ((SteeringBody)this.entity.PhysicsPlugin).destroyElement();
                World.Instance.RemoveModel(this.entity);
            }
        }
        public override void Interact(BaseEntity entity, float crashSpeed)
        {
            float fHealthLoss = (float)Math.Abs(crashSpeed);

            if (entity.LogicPlugin is Bomb || entity.LogicPlugin is Rocket) return;
            //{
            //    fHealthLoss = 0.05f; //- fDamageResistence;
            //    if (fHealthLoss < 0f) fHealthLoss = 0f;
            //}
            //else
            //{
                //crashSpeed = (((RigidBody)entity.PhysicsPlugin).CurState.m_vLinearVelocity - ((RigidBody)this.entity.PhysicsPlugin).CurState.m_vLinearVelocity).Length();




            if (fHealthLoss < 0f) fHealthLoss = 0f;
               /* if (!(entity.PhysicsPlugin is SteeringBody))
                {
                    //Obstacle damage add
                    fHealthLoss *= 100;
                }*/
            //}
                fHealthLoss = Math.Max(0.03f, (float)Math.Abs(crashSpeed));// -fDamageResistence;
                if (fHealthLoss <= 0)
                    bIsColliding = false;
                else bIsColliding = true;

            //dont allow multiple damage
            if (World.Instance.GameTime.TotalGameTime.TotalMilliseconds - dLastCollision <= 100)
                return;
            dLastCollision = World.Instance.GameTime.TotalGameTime.TotalMilliseconds;

            
            //Shield to avoid health loss?
            if (shieldCounter > 0)
            {
                shieldCounter--;
                //Console.WriteLine("SHILD");
                return;
            }

            fHealth -= fHealthLoss;

            
            if (fHealth <= 0)
            {
                fHealth = 0f;
                ((SteeringBody)this.entity.PhysicsPlugin).destroyElement();
                World.Instance.RemoveModel(this.entity);
                
            }
        }

        public override bool isResponding()
        {
            return fHealth > 0;
        }

        public void addHealth(float health)
        {
            fHealth += health;
            if (fHealth > fMaxHealth)
            {
                fHealth = fMaxHealth;
            }
        }

        public override void addDamageFromExplosion(BaseEntity entity, float percentOfHelth)
        {
            this.Interact(percentOfHelth*MaxHealth/100f);
        }
    }
}
