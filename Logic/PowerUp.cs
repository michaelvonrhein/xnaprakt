using System;
using System.Collections.Generic;
using System.Text;
using PraktWS0708.Physics;
using Microsoft.Xna.Framework;
using PraktWS0708.Entities;


namespace PraktWS0708.Logic
{
    /// <summary>
    /// The list of all possible powerups
    /// </summary>
    public enum PowerUpTypes {TURBO, BANANA, SHIELD, HEALTHPACK, ROCKETPICKUP }

         
    /// <summary>
    /// Super class of all powerups
    /// </summary>
    public  class PowerUp : LogicPlugin
    {
        /// <summary>
        /// How long it takes until the powerups apear again
        /// </summary>
        public static int RESPAN_TIME_MILLIES = 20000;
        
        public int remainingEffectTime;
        public int timeTillRespawn;

        protected float modifier;
        protected PowerUpTypes type;
        protected int timeRemaining;
        protected GameTime gameTime = new GameTime(new TimeSpan(0), new TimeSpan(0), new TimeSpan(0), new TimeSpan(0));
        protected int respanTime;

        public PowerUp(PowerUpTypes type, LogicPluginData oLogicPluginData, Entities.BaseEntity e)
            : base(oLogicPluginData, e)
        {
            this.type = type;
        }

        public PowerUpTypes getType()
        {
            return type;
        }

        public float getModifier()
        {
            return modifier;
        }

        public override bool isResponding()
        {
            return true;
        }

        /// <summary>
        /// This method is called when a ship "collides" with a powerup
        /// In most cases, this will add the powereffect to the ship
        /// </summary>
        /// <param name="physicsPlugin"></param>
        public void Interact(PhysicsPlugin physicsPlugin)
        {
            if (physicsPlugin is RigidBody)
            {
                Powereffect powerEffect = getEffect(type, (RigidBody)physicsPlugin);
                if (powerEffect == null)
                {
                    return;
                }
                ((RigidBody)physicsPlugin).addPowerUp(powerEffect);
                Entities.World.Instance.powerUpManager.addPowerUpToRemoveList(this);

                //if (physicsPlugin.entity == World.Instance.PlayersShip)
                World.Instance.PlayCue(Sounds.pickup, physicsPlugin.Position);
            }
        }

        private Powereffect getEffect(PowerUpTypes type, RigidBody rigidBody)
        {
            //Factory pattern
            switch (type)
            {
                case PowerUpTypes.BANANA: return new Logic.Banana(rigidBody, 5000);
                case PowerUpTypes.HEALTHPACK: return new HealthPack(10f, rigidBody, 1000);
                case PowerUpTypes.SHIELD: return new Shield(10, rigidBody, 3000);
                case PowerUpTypes.TURBO: return new Turbo(2f, 2.0f, rigidBody, 5000);
                
                default: return null;
            }
        }

    }
}
