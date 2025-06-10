using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using PraktWS0708.Entities;


namespace PraktWS0708.AI
{
    public class RocketStrategy : KamikazeStrategy
    {
        #region Fields

        private float acquireDistance;

        #endregion

        #region Constructors

        public RocketStrategy(SteeringAgent agent)
            : this(agent, 5.0f, 15.0f)
        {
        }

        public RocketStrategy(SteeringAgent agent, float acquireDist, float maxDist)
            : base(agent, maxDist)
        {
            acquireDistance = acquireDist;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the acquire distance. Any target entity that get closer
        /// than this to us will be hunted.
        /// </summary>
        public float AcquireDistance
        {
            get { return acquireDistance; }
            set { acquireDistance = value; }
        }

        #endregion

        #region Protected Methods

        protected override void Configure()
        {
            base.Configure();

            switch (Settings.Configuration.difficulty)
            {
                case Settings.Configuration.Difficulty.Easy:
                    this.MaxAcceleration = 0.9f;
                    this.MaxYaw = 1.5f;
                    break;

                case Settings.Configuration.Difficulty.Medium:
                    this.MaxAcceleration = 1.1f;
                    this.MaxYaw = 3.0f;
                    break;

                case Settings.Configuration.Difficulty.Hard:
                    this.MaxAcceleration = 1.5f;
                    this.MaxYaw = 5.0f;
                    break;
            }
        }

        protected override BaseEntity SearchTarget()
        {
            BaseEntity target = null;
            float distance = acquireDistance;

            // Search new target
            foreach (BaseEntity entity in World.Instance.Objects.Values)
            {
                if (IsTarget(entity))
                {
                    float d = Vector3.Distance(Agent.Entity.Position, entity.Position);
                    if (d < distance)
                    {
                        target = entity;
                        distance = d;
                    }
                }
            }

            return target;
        }

        protected override bool IsTarget(BaseEntity entity)
        {
            // Override as needed
            return (entity.LogicPlugin is Logic.Ship);
        }

        protected override void UpdateYaw()
        {
            if (TargetDisplacement.IsLeft)
                Yaw = -MaxYaw;
            else
                Yaw = MaxYaw;
        }

        #endregion
    }
}
