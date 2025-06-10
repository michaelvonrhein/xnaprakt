using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using PraktWS0708.Entities;

namespace PraktWS0708.AI
{
    class AggressiveStrategy : WaypointStrategy
    {
        #region Private Fields

        private KamikazeStrategy kamikazeStrategy;
        private bool usingKamikazeStrategy = false;
        private float kamikazeThresholdDistance = 3.0f;

        #endregion

        #region Public Constructors

        public AggressiveStrategy(SteeringAgent agent)
            : base(agent)
        {
            this.kamikazeStrategy = new KamikazeStrategy(agent, kamikazeThresholdDistance);
            this.kamikazeStrategy.TargetLost += OnKamikazeTargetLost;
            this.kamikazeStrategy.MaxAcceleration = MaxAcceleration;
            this.kamikazeStrategy.MaxVelocity = MaxVelocity;
            this.kamikazeStrategy.MaxYaw = MaxYaw;
        }

        #endregion

        #region Public Methods

        public override void Update(GameTime gameTime)
        {
            if (usingKamikazeStrategy)
            {
                // check that target is still in front of us
                if (Vector3.Dot(Agent.Entity.Orientation.Forward, kamikazeStrategy.TargetEntity.Position - Agent.Entity.Position) <= 0.0f)
                    usingKamikazeStrategy = false;
            }

            if (!usingKamikazeStrategy)
            {
                // find closest ship
                int closestShipIndex = -2;
                float closestShipDistance = Single.MaxValue;

                bool orientationOK = (Vector3.Dot(Agent.Entity.Orientation.Forward, World.Instance.PlayersShip.Position - Agent.Entity.Position) > 0.0f);
                if (orientationOK)
                {
                    closestShipIndex = -1;
                    closestShipDistance = Vector3.Distance(World.Instance.PlayersShip.Position, Agent.Entity.Position);
                }

                for (int i = 0; i < World.Instance.EnemyShips.Length; i++)
                {
                    if(World.Instance.EnemyShips[i] == Agent.Entity)
                        continue;

                    float distance = Vector3.Distance(World.Instance.EnemyShips[i].Position, Agent.Entity.Position);
                    orientationOK = (Vector3.Dot(Agent.Entity.Orientation.Forward, World.Instance.PlayersShip.Position - Agent.Entity.Position) > 0.0f);
                    if (distance < closestShipDistance && orientationOK)
                    {
                        closestShipIndex = i;
                        closestShipDistance = distance;
                    }
                }

                // distance < threshold? -> switch to kamikaze mode
                if (closestShipDistance < kamikazeThresholdDistance)
                {
                    usingKamikazeStrategy = true;
                    if (closestShipIndex < 0)
                        kamikazeStrategy.TargetEntity = World.Instance.PlayersShip;
                    else
                        kamikazeStrategy.TargetEntity = World.Instance.EnemyShips[closestShipIndex];
                }
            }

            // update real strategy
            if (!usingKamikazeStrategy)
                base.Update(gameTime);
            else
                kamikazeStrategy.Update(gameTime);
        }

        #endregion

        #region Private Methods

        private void OnKamikazeTargetLost(object sender,
            KamikazeStrategy.KamikazeTargetEventArgs args)
        {
            this.usingKamikazeStrategy = false;
        }

        #endregion
    }
}
