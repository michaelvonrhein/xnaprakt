using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using PraktWS0708.Entities;


namespace PraktWS0708.AI
{
    /// <summary>
    /// This strategy steers the agent right into target enemy entities.
    /// </summary>
    public class KamikazeStrategy : SteeringStrategy
    {
        #region Types

        public class KamikazeTargetEventArgs : EventArgs
        {
            private KamikazeStrategy strategy;
            private BaseEntity targetEntity;

            public BaseEntity TargetEntity
            {
                get { return targetEntity; }
                set { targetEntity = value; }
            }

            public KamikazeStrategy Strategy
            {
                get { return strategy; }
                set { strategy = value; }
            }

            public KamikazeTargetEventArgs(KamikazeStrategy s, BaseEntity e)
            {
                strategy = s;
                targetEntity = e;
            }
        }

        #endregion

        #region Events

        public event EventHandler<KamikazeTargetEventArgs> TargetAcquired;
        public event EventHandler<KamikazeTargetEventArgs> TargetLost;

        #endregion

        #region Fields

        private BaseEntity targetEntity;
        private float maxDistance;

        #endregion

        #region Constructors

        public KamikazeStrategy(SteeringAgent agent)
            : this(agent, 15.0f)
        {
        }

        public KamikazeStrategy(SteeringAgent agent, float maxDistance)
            : this(agent, maxDistance, null)
        {
        }

        public KamikazeStrategy(SteeringAgent agent, float maxDistance, BaseEntity target)
            : base(agent)
        {
            this.maxDistance = maxDistance;
            this.targetEntity = target;
        }

        #endregion

        #region Properties

        public BaseEntity TargetEntity
        {
            get { return targetEntity; }
            set
            {
                targetEntity = value;
                OnTargetAcquired(new KamikazeTargetEventArgs(this, targetEntity));
            }
        }

        /// <summary>
        /// Gets or sets the maximal distance until the target is lost.
        /// </summary>
        public float MaxDistance
        {
            get { return maxDistance; }
            set { maxDistance = value; }
        }

        #endregion

        #region Public Methods

        public override void Update(GameTime gameTime)
        {
            if (targetEntity != null)
            {
                TargetPosition = targetEntity.Position;
                UpdateState();

                if (TargetDisplacement.Distance <= maxDistance)
                {
                    // Hunt this target!
                    Hunt(true);
                    return;
                }

                // Target lost.
                BaseEntity oldTarget = targetEntity;
                targetEntity = null;
                OnTargetLost(new KamikazeTargetEventArgs(this, oldTarget));
            }

            BaseEntity newTarget = SearchTarget();
            if (newTarget != null)
            {
                // Hunt new target!
                targetEntity = newTarget;
                Hunt(false);
                OnTargetAcquired(new KamikazeTargetEventArgs(this, newTarget));
                return;
            }

            // No target. Idle around.
            Idle();
        }

        #endregion

        #region Protected Methods

        protected virtual void Hunt(bool stateUpdated)
        {
            TargetPosition = targetEntity.Position;
            TargetVelocity = Single.MaxValue;
            TargetDistanceThreshold = 0.0f;
            if (!stateUpdated)
                UpdateState();
            UpdateYaw();
            UpdateAcceleration();
        }

        protected virtual void Idle()
        {
            TargetPosition = Agent.Entity.Position + Agent.Entity.Orientation.Forward;
            TargetVelocity = 0;
            TargetDistanceThreshold = 2.0f;
            UpdateSteering();
        }

        protected virtual BaseEntity SearchTarget()
        {
            // Override as needed
            return null;
        }

        protected virtual bool IsTarget(BaseEntity entity)
        {
            // Override as needed
            return false;
        }

        protected virtual void OnTargetAcquired(KamikazeTargetEventArgs e)
        {
            EventHandler<KamikazeTargetEventArgs> handler = TargetAcquired;
            if (handler != null)
                handler(this, e);
        }

        protected virtual void OnTargetLost(KamikazeTargetEventArgs e)
        {
            EventHandler<KamikazeTargetEventArgs> handler = TargetLost;
            if (handler != null)
                handler(this, e);
        }

        #endregion
    }
}
