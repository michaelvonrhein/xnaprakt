using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using PraktWS0708.Entities;
using PraktWS0708.Input;
using PraktWS0708.Utils;


namespace PraktWS0708.AI
{
    /// <summary>
    /// The AISystem class centralized the control of AI-controlled agents.
    /// </summary>
    public class AISystem
    {
        #region Static members

        private static AISystem instance;

        /// <summary>
        /// Returns the only instance of the AISystem class.
        /// </summary>
        public static AISystem Instance
        {
            get
            {
                if (instance == null)
                    instance = new AISystem();
                return instance;
            }
        }

        #endregion

        #region Private Fields

        private List<Agent> agents;

        #endregion

        #region Properties

        public List<Agent> Agents
        {
            get { return agents; }
        }

        #endregion

        #region Constructors

        private AISystem()
        {
            agents = new List<Agent>();
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Initializes the AI system.
        /// </summary>
        public void Initialize()
        {
            foreach (Agent agent in agents)
            {
                agent.Initialize();
            }
        }

        /// <summary>
        /// Resets the AISystem. Should be called every time when the static
        /// World (eg, the Track) changes.
        /// </summary>
        public void Reset()
        {
            agents.Clear();
        }

        /// <summary>
        /// Updates the AI. Causes all AI-controlled entities to update.
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update(GameTime gameTime)
        {
            #region PerformanceEater
            //#if DEBUG
            PerformanceEater last = PerformanceMeter.Instance.PerfomanceEaterChange(PerformanceEater.AI);
            //#endif
            #endregion

            foreach (Agent agent in agents)
            {
                agent.Update(gameTime);
            }

            #region PerformanceEater
            //#if DEBUG
            PerformanceMeter.Instance.PerfomanceEaterChange(last);
            //#endif
            #endregion
        }

        /// <summary>
        /// Creates an InputPlugin for an entity.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public InputPlugin GetInputPlugin(BaseEntity entity, InputPlugin.InputPluginType type)
        {
            SteeringAgent agent = new SteeringAgent(entity, type);
            agents.Add(agent);
            return agent.InputPlugin;
        }

        #endregion
    }
}