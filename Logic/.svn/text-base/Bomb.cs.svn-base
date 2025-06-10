using System;
using System.Collections.Generic;
using System.Text;
using PraktWS0708.Physics;
using PraktWS0708.Entities;
using Microsoft.Xna.Framework;

namespace PraktWS0708.Logic
{
    /// <summary>
    /// The bomb logic plugin only saves a factor. 
    /// The regular collision response will be proportional to this factor
    /// </summary>
    public class Bomb : SolidObject, Explosive
    {
        //The strength of the explosion
        private float explosion;

        public Bomb(float explosion, LogicPluginData oLogicPluginData, Entities.BaseEntity e)
            : base(oLogicPluginData, e) 
        {

            this.explosion = explosion;
        }

        /// <summary>
        /// Get the power of the bomb
        /// </summary>
        /// <returns>the strength</returns>
        public float getExplosionDamage()
        {
            return explosion;
        }

        /// <summary>
        /// Whether the bomb is active
        /// </summary>
        /// <returns></returns>
        public override bool isResponding()
        {
            return true;
        }

        public void explode(BaseEntity oEntity)
        {
            BaseEntity entAni = EntityFactory.BuildParticleEntity(Particles.ParticleSystemType.Billboard, 2, this.entity.Position, 0.375f, Vector4.One, this.entity.Orientation);
            BaseEntity entParticle = EntityFactory.BuildParticleEntity(Particles.ParticleSystemType.Default, 1000, this.entity.Position, 0.1f, Vector4.One, this.entity.Orientation);
            World.Instance.ParticleManager.Add(new Particles.ParticleSystemManagementData(entAni, null, World.Instance.ParticleManager.GetEffectTime(Particles.ParticleSystemType.Explosion), Vector3.Zero, Matrix.Identity));
            World.Instance.ParticleManager.Add(new Particles.ParticleSystemManagementData(entParticle, null, World.Instance.ParticleManager.GetEffectTime(Particles.ParticleSystemType.Explosion), Vector3.Zero, Matrix.Identity));
            
            //if (oEntity == World.Instance.PlayersShip)
            World.Instance.PlayCue(Sounds.explosion, oEntity.Position);    // sound
            //World.Instance.PhysicsSystem.Remove(this.entity.ObjectID);
            //this.entity.RenderingPlugin.Hidden = true;
            //m_bHidden = true; TODO
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
