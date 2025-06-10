using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace PraktWS0708.AI
{
    /// <summary>
    /// Base class for AI agents.
    /// </summary>
    public abstract class Agent
    {
        #region Private Fields

        private int reactionTime = 1;
        private int elapsedTime = 0;

        #endregion

        #region Public Properties

        public int ReactionTime
        {
            get { return reactionTime; }
            set { reactionTime = value; }
        }

        public int ElapsedTime
        {
            get { return elapsedTime; }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Initializes the Agent.
        /// </summary>
        public virtual void Initialize()
        {
        }

        /// <summary>
        /// Updates the Agent according to its ReactionTime.
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update(GameTime gameTime)
        {
            elapsedTime += gameTime.ElapsedRealTime.Milliseconds;
            if (elapsedTime >= reactionTime)
            {
                UpdateImpl(gameTime); 
                elapsedTime = 0;
            }
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Internal Update implementation.
        /// </summary>
        /// <param name="gameTime"></param>
        protected abstract void UpdateImpl(GameTime gameTime);

        #endregion
    }
}
