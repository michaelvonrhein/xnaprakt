using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace PraktWS0708.Entities
{
    class MovementRecorder
    {
        #region Construction

        /// <summary>
        /// Create MovementRecorder instance.
        /// </summary>
        /// <param name="target">Object to be recorded/played back on.</param>
        public MovementRecorder(BaseEntity target)
        {
            this.Target = target;
            this.MinInterval = 100.0;
        }

        /// <summary>
        /// Create MovementRecorder instance.
        /// </summary>
        /// <param name="target">Object to be recorded/played back on.</param>
        /// <param name="minInterval">Minimum interval between two samples, in milliseconds.</param>
        public MovementRecorder(BaseEntity target, double minInterval)
        {
            this.Target = target;
            this.MinInterval = minInterval;
        }

        #endregion

        #region Update

        /// <summary>
        /// Update should be called every frame!
        /// </summary>
        /// <param name="elapsedTime">Current GameTime.</param>
        public void Update(GameTime gameTime)
        {
            if (!this.paused)
            {
                if (this.currentState == State.Recording)
                {
                    this.accumulatedTime += gameTime.ElapsedGameTime.TotalMilliseconds;

                    // take sample?
                    if (this.accumulatedTime >= this.MinInterval)
                    {
                        this.accumulatedTime = 0.0;

                        double timeStamp = gameTime.TotalGameTime.TotalMilliseconds - this.startTime - this.accumulatedPauseTime;
                        this.samples.Add(new Sample(this.Target.Position, this.Target.Orientation, timeStamp));
                    }
                }
                else if (this.currentState == State.Playing)
                {
                    if (this.samples.Count > 1)
                    {
                        double localTime = gameTime.TotalGameTime.TotalMilliseconds - this.startTime - this.accumulatedPauseTime;

                        // find indices into this.samples
                        while (this.lastPlaybackIndex < this.samples.Count - 2 && this.samples[this.lastPlaybackIndex + 1].TimeStamp < localTime)
                            this.lastPlaybackIndex++;

                        // calc weights
                        float weight0 = Math.Max(0.0f,
                                                (float)((this.samples[this.lastPlaybackIndex + 1].TimeStamp - localTime) /
                                                        (this.samples[this.lastPlaybackIndex + 1].TimeStamp - this.samples[this.lastPlaybackIndex].TimeStamp))),
                              weight1 = 1.0f - weight0;

                        // interpolate & apply
                        this.Target.Position = weight0 * this.samples[this.lastPlaybackIndex].Position + weight1 * this.samples[this.lastPlaybackIndex + 1].Position;

                        Matrix newOrientation = Matrix.Lerp(this.samples[this.lastPlaybackIndex].Orientation, this.samples[this.lastPlaybackIndex + 1].Orientation, weight1);
                        // re-orthogonalize
                        newOrientation.Forward = Vector3.Normalize(newOrientation.Forward);
                        newOrientation.Up = Vector3.Normalize(newOrientation.Up);
                        newOrientation.Right = Vector3.Cross(newOrientation.Forward, newOrientation.Up);
                        newOrientation.Up = Vector3.Cross(newOrientation.Right, newOrientation.Forward);

                        this.Target.Orientation = newOrientation;

                        // finished playback?
                        if (this.lastPlaybackIndex + 2 == this.samples.Count)
                            this.currentState = State.Finished;
                    }
                    else if (this.samples.Count == 1)
                    {
                        this.Target.Position = this.samples[0].Position;
                        this.Target.Orientation = this.samples[0].Orientation;

                        this.currentState = State.Finished;
                    }
                    this.Target.Update();
                }
            }
            else
            {
                this.accumulatedPauseTime += gameTime.ElapsedGameTime.TotalMilliseconds;
            }
        }

        #endregion

        #region Controls

        public void StartRecording(GameTime gameTime)
        {
            this.currentState = State.Recording;

            this.startTime = gameTime.TotalGameTime.TotalMilliseconds;
            this.accumulatedPauseTime = 0.0;
            this.accumulatedTime = 0.0;
            this.paused = false;

            // take initial sample
            this.samples.Add(new Sample(this.Target.Position, this.Target.Orientation, 0.0));
        }

        public void StartPlayback(GameTime gameTime)
        {
            this.currentState = State.Playing;

            this.startTime = gameTime.TotalGameTime.TotalMilliseconds;
            this.accumulatedPauseTime = 0.0;
            this.lastPlaybackIndex = 0;
            this.paused = false;

            // apply initial state
            if (this.samples.Count > 0)
            {
                this.Target.Position = this.samples[0].Position;
                this.Target.Orientation = this.samples[0].Orientation;
                this.Target.Update();
            }
        }

        public void Pause()
        {
            this.paused = true;
        }

        public void Resume()
        {
            this.paused = false;
        }

        //TODO Jump

        public void Stop()
        {
            this.currentState = State.Idle;
        }

        /// <summary>
        /// Clear all samples
        /// </summary>
        public void Clear()
        {
            this.samples.Clear();
        }

        #endregion

        #region Public fields/properties

        public enum State
        {
            Idle,
            Recording,
            Playing,
            Finished
        }

        /// <summary>
        /// Object to be recorded.
        /// </summary>
        public BaseEntity Target;

        /// <summary>
        /// Minimum interval between taking samples in recording mode, in milliseconds.
        /// </summary>
        public double MinInterval;

        public State CurrentState
        {
            get { return this.currentState; }
        }

        public int NumSamples
        {
            get { return this.samples.Count; }
        }

        #endregion

        #region Private stuff

        private State currentState = State.Idle;

        private double startTime;
        private double accumulatedTime;
        private double accumulatedPauseTime;
        private bool paused;
        private int lastPlaybackIndex;

        private List<Sample> samples = new List<Sample>();

        private struct Sample
        {
            public Sample(Vector3 position, Matrix orientation, double timeStamp)
            {
                this.Position = position;
                this.Orientation = orientation;
                this.TimeStamp = timeStamp;
            }

            public Vector3 Position;
            public Matrix Orientation;
            public double TimeStamp;
        }

        #endregion
    }
}
