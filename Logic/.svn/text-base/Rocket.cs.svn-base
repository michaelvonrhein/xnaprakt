using System;
using System.Collections.Generic;
using System.Text;
using PraktWS0708.Entities;
using Microsoft.Xna.Framework;
using PraktWS0708.Physics;

namespace PraktWS0708.Logic
{
    /// <summary>
    /// The rocket logic plugin is almost the same as the bomb logic plugin
    /// The regular collision response will be proportional to this factor
    /// </summary>
    public class Rocket : SolidObject, Explosive
    {
        //The strength of the explosion
        private float explosion;
        public enum RocketState { WAITING, HUNTING };

        private RocketState state;

        private bool inactive = false;

        private int timeStamp;

        // Thruster
        private int iNumThruster;
        private Vector3[] vaThrustRelativePositions;
        private Matrix[] maThrustOrientations;
        private Vector3[] vaThrustScales;

        public Rocket(float explosion, LogicPluginData oLogicPluginData, Entities.BaseEntity e)
            : base(oLogicPluginData, e)
        {
            this.explosion = explosion;
            switchState(RocketState.WAITING, null, null);

            iNumThruster = oLogicPluginData.m_iNumThruster;
            vaThrustRelativePositions = oLogicPluginData.m_vaThrustRelativePositions;
            maThrustOrientations = oLogicPluginData.m_maThrustOrientations;
            vaThrustScales = oLogicPluginData.m_vaThrustScales;

            if (this.entity.PhysicsPlugin.Type == PhysicsPlugin.PhysicsPluginType.SteeringBody)
            {
                for (int i = 0; i < iNumThruster; i++)
                {
                    BaseEntity oThrustPS = EntityFactory.BuildParticleEntity(Particles.ParticleSystemType.Thrust, 300, Vector3.Zero, 0.02f, new Vector4(vaThrustScales[i].X, vaThrustScales[i].Y, vaThrustScales[i].Z, 4f), Matrix.Identity);
                    World.Instance.ParticleManager.Add(new Particles.ParticleSystemManagementData(oThrustPS, this.entity, -1, vaThrustRelativePositions[i] * this.entity.Scale, maThrustOrientations[i]));
                }
            }
        }


        /// <summary>
        /// Whether the rocket is active
        /// </summary>
        /// <returns></returns>
        public override bool isResponding()
        {
            return true;
        }

        public void switchState(RocketState state, BaseEntity target, GameTime time)
        {
            switch (state)
            {
                case RocketState.WAITING:
                    entity.PhysicsPlugin.BoundingSphere.m_fRadius = 5f;
                    this.state = RocketState.WAITING;
                    if (inactive) this.entity.InputPlugin.Active = false;
                    //TODO AI
                    timeStamp = 0;
                    break;
                case RocketState.HUNTING:
                    timeStamp += time.ElapsedGameTime.Milliseconds;
                    if (timeStamp < 20) return;
                    
                    entity.PhysicsPlugin.BoundingSphere.m_fRadius = 0.2f;
                    this.state = RocketState.HUNTING;
                    this.entity.InputPlugin.Active = true;
                    //TODO AI
                    break;
            }
        }

        public RocketState getState()
        {
            return state;
        }

        public void explode(BaseEntity oEntity)
        {
            BaseEntity entAni = EntityFactory.BuildParticleEntity(Particles.ParticleSystemType.Billboard, 2, oEntity.Position, 0.375f, Vector4.One, this.entity.Orientation);
            BaseEntity entParticle = EntityFactory.BuildParticleEntity(Particles.ParticleSystemType.Default, 1000, oEntity.Position, 0.1f, Vector4.One, this.entity.Orientation);
            World.Instance.ParticleManager.Add(new Particles.ParticleSystemManagementData(entAni, null, World.Instance.ParticleManager.GetEffectTime(Particles.ParticleSystemType.Explosion), Vector3.Zero, Matrix.Identity));
            World.Instance.ParticleManager.Add(new Particles.ParticleSystemManagementData(entParticle, null, World.Instance.ParticleManager.GetEffectTime(Particles.ParticleSystemType.Explosion), Vector3.Zero, Matrix.Identity));

            //if (oEntity == World.Instance.PlayersShip)
            World.Instance.PlayCue(Sounds.explosion, oEntity.Position);

            //World.Instance.PhysicsSystem.Remove(this.entity.ObjectID);
            //this.entity.RenderingPlugin.Hidden = true;
            //m_bHidden = true; TODO

            inactive = true;
            this.switchState(RocketState.WAITING, null, null);
            try
            {
                /*AerodynamicDrag ad = (AerodynamicDrag)(((SteeringBody)this.entity.PhysicsPlugin).ForceCausingObjects.Find(isAerodynamicDrag));
                ad.Amplify(100, 100);
                AerodynamicDrag ad2 = (AerodynamicDrag)(((SteeringBody)oEntity.PhysicsPlugin).ForceCausingObjects.Find(isAerodynamicDrag));
                ad2.Amplify(100, 100);*/
            }catch (Exception e) {//NO drag, should not happen
            }
            
        }

        private static bool isAerodynamicDrag(ForceCausingObject fo)
        {
            return fo is AerodynamicDrag;
           
        }



        public float getExplosionDamage()
        {
            return explosion;
        }

        public override void Interact(BaseEntity entity, float crashSpeed)
        {
            //DO NOTHING
        }

        public override void addDamageFromExplosion(BaseEntity entity, float damage)
        {
            //DO NOTHING
        }
    }
}
