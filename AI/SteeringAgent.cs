using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using PraktWS0708.Entities;
using PraktWS0708.Input;

namespace PraktWS0708.AI
{
    /// <summary>
    /// An agent that can steer a space ship.
    /// </summary>
    public class SteeringAgent : Agent
    {
        #region Private Fields

        private SteeringStrategy strategy;
        private AgentInputPlugin inputPlugin;

        #endregion

        #region Public Properties

        public BaseEntity Entity
        {
            get { return inputPlugin.entity; }
        }

        public InputPlugin InputPlugin
        {
            get { return inputPlugin; }
        }

        public SteeringStrategy Strategy
        {
            get { return strategy; }
            set { strategy = value; }
        }

        #endregion

        #region Public Constructors

        public SteeringAgent(BaseEntity entity, InputPlugin.InputPluginType strategyType)
        {
            switch (strategyType)
            {
                case InputPlugin.InputPluginType.AI_CONSERVATIVE:
                    Init(entity, new ConservativeStrategy(this));
                    break;

                case InputPlugin.InputPluginType.AI_WAYPOINT:
                    Init(entity, new WaypointStrategy(this));
                    break;

                case InputPlugin.InputPluginType.AI_KAMIKAZE:
                    Init(entity, new KamikazeStrategy(this));
                    break;

                case InputPlugin.InputPluginType.AI_ROCKET:
                    Init(entity, new RocketStrategy(this));
                    // Will get activated by the physics...
                    inputPlugin.Active = false;
                    break;

                case InputPlugin.InputPluginType.AI_AGGRESSIVE:
                    Init(entity, new AggressiveStrategy(this));
                    break;

                default:
                    throw new ArgumentException("not an AI type", "strategyType");
            }
        }

        public SteeringAgent(BaseEntity entity, SteeringStrategy strategy)
        {
            Init(entity, strategy);
        }

        #endregion

        #region Protected Methods

        protected override void UpdateImpl(GameTime gameTime)
        {
            if (inputPlugin.Active)
                strategy.Update(gameTime);
        }

        #endregion

        #region Public Methods

        public override void Initialize()
        {
            base.Initialize();
            strategy.Initialize();
        }

        #endregion

        #region Private Methods

        private void Init(BaseEntity entity, SteeringStrategy strategy)
        {
            inputPlugin = new AgentInputPlugin(this, entity);
            entity.Destroyed += new EventHandler<EntityEventArgs>(EntityDestroyed);
            this.strategy = strategy;
        }

        private void EntityDestroyed(object sender, EntityEventArgs e)
        {
            if (e.Entity.InputPlugin == this.inputPlugin)
            {
                AISystem.Instance.Agents.Remove(this);
            }
        }

        #endregion

        #region Private InputPlugin Implementation

        private class AgentInputPlugin : InputPlugin
        {
            private SteeringAgent agent;

            protected override float AccelerationImpl
            {
                get { return agent.Strategy.Acceleration; }
            }

            protected override float YawImpl
            {
                get { return agent.Strategy.Yaw; }
            }

            public AgentInputPlugin(SteeringAgent agent, BaseEntity entity)
                : base(entity)
            {
                this.agent = agent;
            }
        }

        #endregion
    }
}
