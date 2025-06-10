using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Content;
using PraktWS0708.Entities;
using PraktWS0708.Utils;

namespace PraktWS0708.AI
{
    public class ConservativeStrategy : SteeringStrategy
    {
        #region Public Constructors

        public ConservativeStrategy(SteeringAgent agent)
            : base(agent)
        {
        }

        #endregion

        #region Public Methods

        public override void Update(GameTime gameTime)
        {
            // TODO: Split into path finding and steering to the next waypoint!
            // The current steering is pretty dump but works well for the demo
            // track.

            Track track = World.Instance.Track;
            BaseEntity entity = Agent.Entity;

            int count = track.TangentFrames.Length;
            int frame = 0;
            float distance = -1.0f;

            // Get the closest tangent frame
            for (int i = 0; i < count; ++i)
            {
                float d = Vector3.Distance(track.TangentFrames[i].Position, entity.Position);
                if (distance < 0.0f || d < distance)
                {
                    frame = i;
                    distance = d;
                }
            }

            frame = (frame + 10) % count;
            UpdateSteering(track.TangentFrames[frame].Position);
        }

        #endregion
    }
}