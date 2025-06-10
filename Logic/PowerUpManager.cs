using System;
using System.Collections.Generic;
using System.Text;

namespace PraktWS0708.Logic
{
    /// <summary>
    /// Class for controlling the powerups and their reapearing 
    /// </summary>
    public class PowerUpManager
    {
        private List<PowerUp> powerUps;
        private static PowerUpManager powerUpManager = null;
        private List<PowerUp> toRemove;

        private PowerUpManager()
        {
            powerUps = new List<PowerUp>();
            toRemove = new List<PowerUp>();
        }
        /// <summary>
        /// Singleton
        /// </summary>
        /// <returns></returns>
        public static PowerUpManager getInstance()
        {
            if (powerUpManager == null)
            {
                powerUpManager = new PowerUpManager();
            }
            return powerUpManager;
        }

        //Update powerups checks whether a Powerups gets active again
        public void updatePowerUps(int timeMilliesPassed)
        {
            for (int i = 0; i < powerUps.Count; i++ )
            {
                if (powerUps[i].timeTillRespawn <= 0)
                {
                    addPowerUp(powerUps[i]);
                    powerUps.Remove(powerUps[i]);
                    i--;
                }
                else
                {
                    powerUps[i].timeTillRespawn -= timeMilliesPassed;
                }
            
            }

        }
        //As we cannot remove the Powerup at the moment we collide 
        //(cannot remove Elements from a list while using foreach)
        //the PowerUp is moved to a removelist 
        public void addPowerUpToRemoveList(PowerUp powerUp)
        {
            powerUp.timeTillRespawn = PowerUp.RESPAN_TIME_MILLIES;
            toRemove.Add(powerUp);
            
            //TODO evtl auch beim Rendering removen? Oder flag setzen?
        }

        //remove the Powerups marked with addPowerUpToRemoveList
        public void doRemovePowerUps()
        {
            foreach (PowerUp powerUp in toRemove)
            {
                Entities.World.Instance.PhysicsSystem.Remove(powerUp.entity.ObjectID);
                powerUp.entity.RenderingPlugin.Hidden = true;
                powerUps.Add(powerUp);
            }
            toRemove.Clear();
        }

        //Reinserts the powerup to world again
        private void addPowerUp(PowerUp powerUp)
        {
            Entities.World.Instance.PhysicsSystem.Add(powerUp.entity.ObjectID, powerUp.entity.PhysicsPlugin);
            powerUp.entity.RenderingPlugin.Hidden = false;
            //TODO evtl auch beim Rendering removen? Oder flag setzen?
        }

    }
}
