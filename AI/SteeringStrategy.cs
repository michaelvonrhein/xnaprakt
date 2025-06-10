using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using PraktWS0708.Entities;
using PraktWS0708.Utils;
using PraktWS0708.Settings;


namespace PraktWS0708.AI
{
    /// <summary>
    /// A SteeringStrategy determines how a ship is steered by the AI.
    /// </summary>
    public abstract class SteeringStrategy
    {
        #region Documentation

        //
        // Calculation model for linear motion:
        //
        //     v(t)    velocity at frame t
        //     a(t)    acceleration at frame t
        //     A(t)    value of variable 'acceleration' at frame t
        //     dt      time elapsed since last frame
        //     ds      distance travelled since last frame (displacement)
        //
        // Basic linear equations of motion, see also:
        // http://en.wikipedia.org/wiki/Equations_of_motion
        //
        //   [1]   v(t)   = v(t-1)  +  a(t-1) * dt
        //   [2]   ds     = 0.5 * (v(t-1)  +  v(t)) * dt
        //   [3]   ds     = v(t-1) * dt  +  0.5 * a(t) * dt^2
        //   [4]   v(t)^2 = v(t-1)^2  +  2 * a(t) * ds
        //   [5]   ds     = v(t) * dt   -   0.5 * a(t) * dt^2
        //
        // The only parameter we can influence is a(t), and this only indirectly through A(t)
        // because 'acceleration' is an artificial value between -1.0 and 1.0:
        // 
        //   [6]   a(t) = A(t) * X
        //
        // Where X is an unknown function that reflects the physics environment.
        // We try to approximate X by a function F(v), mapping velocity to 'acceleration
        // factors':
        // 
        //   [7]   X = F(v)
        //
        // To learn F(v), we can backpropagate the effect of our last acceleration:
        //        
        //         v(t) = v(t-1) + A(t-1) * F[v(t-1)] * dt
        //   [8]   F[v(t-1)] = (v(t) - v(t-1)) / (A(t-1) * dt)
        //
        // In order to reach a target velocity:
        // 
        //         v(t+1) = v(t) + A(t) * F(v) * dt
        //   [9]   A(t) = (v(t+1) - v(t)) / (F(v) * dt)
        //

        #endregion

        #region Types

        /// <summary>
        /// Event arguments for the Target events.
        /// </summary>
        public class TargetEventArgs : EventArgs
        {
            private SteeringStrategy strategy;

            public SteeringStrategy Strategy
            {
                get { return strategy; }
            }

            public TargetEventArgs(SteeringStrategy strategy)
            {
                this.strategy = strategy;
            }
        }

        /// <summary>
        /// Holds the relative position of a point to another.
        /// </summary>
        public struct Displacement
        {
            public float Distance;
            public float RightDistance;
            public float FrontDistance;
            public float Angle;

            public bool IsLeft
            {
                get { return RightDistance < 0.0f; }
            }

            public bool IsRight
            {
                get { return RightDistance > 0.0f; }
            }

            public bool IsAhead
            {
                get { return FrontDistance > 0.0f; }
            }

            public bool IsBehind
            {
                get { return FrontDistance < 0.0f; }
            }
        };

        #endregion

        #region Events

        public event EventHandler<TargetEventArgs> TargetReached;

        #endregion

        #region Fields

        private SteeringAgent agent;

        private float acceleration;
        private float yaw;

        // Target definition
        private Vector3 targetPosition;
        private float targetVelocity = Single.MaxValue;
        private float targetDistanceThreshold;

        // Constraints
        private float maxVelocity = Single.MaxValue;
        private float maxAcceleration = 1.0f;
        private float maxYaw = 1.0f;

        // Acceleration factors learned during the race
        private InterpolationMap accelerationFactors;

        // Calculated during UpdateState()
        private float velocity;
        private float timeStep;
        private float distanceStep;
        private Vector3 position;
        private Displacement targetDisplacement;

        /// <summary>
        /// "Right plane": Plane that devides the agent's view space
        /// into left and right subspaces.
        /// </summary>
        private Plane rightPlane;

        /// <summary>
        /// "Forward plane": Plane that devides the space into into front
        /// and back of the agent's view.
        /// </summary>
        private Plane forwardPlane;

        // Values from last frame
        private float lastAcceleration;
        private float lastYaw;
        private float lastVelocity;
        private Vector3 lastPosition;
        private Plane lastRightPlane;
        private Plane lastForwardPlane;
        private Displacement lastTargetDisplacement;

        //MAGIC
        private float maxSpeed = 20.0f; // for limiting speed at high yaw angles
        private float speedLimitYawMin = 3.0f;
        private float speedLimitYawMax = 8.0f;
        private float minLimitedSpeed = 8.0f;

        #endregion

        #region Constructors

        protected SteeringStrategy(SteeringAgent a)
        {
            agent = a;
            accelerationFactors = new InterpolationMap(30, 30.0f, 100.0f);

            Configure();
            Configuration.Changed += Configuration_Changed;
        }

        #endregion

        #region Properties

        public SteeringAgent Agent
        {
            get { return agent; }
        }

        public float Acceleration
        {
            get { return acceleration; }
            protected set { yaw = value; }
        }

        public float Yaw
        {
            get { return yaw; }
            protected set { yaw = value; }
        }

        /// <summary>
        /// Gets or sets the target position for steering in the current frame.
        /// </summary>
        public Vector3 TargetPosition
        {
            get { return targetPosition; }
            protected set { targetPosition = value; }
        }

        /// <summary>
        /// Gets or sets the velocity the agent should strive to at the target
        /// point.
        /// </summary>
        public float TargetVelocity
        {
            get { return targetVelocity; }
            protected set { targetVelocity = value; }
        }

        /// <summary>
        /// Gets or sets the threshold value for the distance to the target point.
        /// If the agents gets as close or closer than this threshold to the target
        /// it will assume to have reached target.
        /// </summary>
        public float TargetDistanceThreshold
        {
            get { return targetDistanceThreshold; }
            protected set { targetDistanceThreshold = value; }
        }

        public float MaxVelocity
        {
            get { return maxVelocity; }
            protected set { maxVelocity = value; }
        }

        public float MaxAcceleration
        {
            get { return maxAcceleration; }
            protected set { maxAcceleration = value; }
        }

        public float MaxYaw
        {
            get { return maxYaw; }
            protected set { maxYaw = value; }
        }

        public float Velocity
        {
            get { return velocity; }
        }

        public Displacement TargetDisplacement
        {
            get { return targetDisplacement; }
        }

        #endregion

        #region Public Methods

        public virtual void Initialize()
        {
        }

        public abstract void Update(GameTime gameTime);

        public Displacement GetDisplacement(Vector3 point)
        {
            return CalculateDisplacement(point, position, rightPlane, forwardPlane);
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Sets a new target position and updates yaw and acceleration.
        /// </summary>
        /// <param name="target"></param>
        protected void UpdateSteering(Vector3 target)
        {
            targetPosition = target;
            targetVelocity = Single.MaxValue;
            targetDistanceThreshold = 0.0f;
            UpdateSteering();
        }

        /// <summary>
        /// Updates yaw and acceleration.
        /// </summary>
        protected void UpdateSteering()
        {
            UpdateState();
            UpdateYaw();
            UpdateAcceleration();
        }

        /// <summary>
        /// Updates the current state of the agent
        /// </summary>
        protected void UpdateState()
        {
            lastAcceleration = acceleration;
            lastYaw = yaw;
            lastPosition = position;
            lastVelocity = velocity;
            lastRightPlane = rightPlane;
            lastForwardPlane = forwardPlane;

            position = agent.Entity.Position;
            rightPlane = CreatePlane(agent.Entity.Position,
                agent.Entity.Orientation.Right);
            forwardPlane = CreatePlane(agent.Entity.Position,
                agent.Entity.Orientation.Forward);         
            
            targetDisplacement = CalculateDisplacement(targetPosition, position,
                rightPlane, forwardPlane);

            // This will be true on the first call when there is no last position
            if (lastPosition == Vector3.Zero)
            {
                lastPosition = position;
                lastTargetDisplacement = targetDisplacement;
            }
            else
            {
                lastTargetDisplacement = CalculateDisplacement(targetPosition,
                    lastPosition, lastRightPlane, lastForwardPlane);
            }

            timeStep = 0.001f * agent.ElapsedTime; // In seconds.
            distanceStep = Vector3.Distance(position, lastPosition);
            velocity = distanceStep / timeStep;
        }

        protected virtual void UpdateYaw()
        {
            // Amount of yaw depends on the angle to our target.
            float wantYaw = 18.0f * (targetDisplacement.Angle / MathHelper.Pi); // 1.0 <=> 10°
            yaw = MathHelper.Clamp(
                wantYaw,
                0.0f,
                maxYaw);

            if (wantYaw < speedLimitYawMin)
            {
                this.TargetVelocity = Single.MaxValue;
            }
            else
            {
                if (wantYaw > speedLimitYawMax)
                    wantYaw = speedLimitYawMax;
                this.TargetVelocity = minLimitedSpeed + (maxSpeed - minLimitedSpeed) * (wantYaw - speedLimitYawMin) / (speedLimitYawMax - speedLimitYawMin);
            }

            if (targetDisplacement.IsLeft)
                yaw = -yaw;

            //System.Console.WriteLine("Angle: " + MathHelper.ToDegrees(targetDisplacement.Angle));
        }

        protected virtual void UpdateAcceleration()
        {
            if (velocity == 0.0f && targetDisplacement.Distance > 0.0f)
            {
                // First call, not moved yet, speed up!
                acceleration = 1.0f;
                return;
            }

            // Update acceleration factor. With our physics system, small accelerations
            // have no impact. Therefore we only update if we are above a certain lower
            // limit.
            if (Math.Abs(lastAcceleration) > 0.15f)
            {
                // Corresponds to equation [8] in the documentation section
                float factor = (velocity - lastVelocity) / (lastAcceleration * timeStep);

                // Somethings wrong with our equation; Make sure that the factors
                // stay sane!
                if (factor > 0.0f && factor < 1000.0f)
                    accelerationFactors.Update(lastVelocity, factor);
            }

            // Check whether we are driving backwards
            bool backwards = (lastTargetDisplacement.Distance < targetDisplacement.Distance);

            if (targetDisplacement.Distance < targetDistanceThreshold)
            {
                // At target, try to hold the target velocity constant
                float acc = (targetVelocity - velocity) / (accelerationFactors[velocity] * timeStep);
                if (backwards)
                    acc = -acc;
                acceleration = MathHelper.Clamp(acc, -maxAcceleration, maxAcceleration);
                
                // Report that we reach the target to everyone who wants to know
                OnTargetReached(new TargetEventArgs(this));
            }
            else
            {
                // Max. velocity for this frame
                float tempMaxVelocity = maxVelocity;

                //if (Math.Abs(yaw) > 0.95f)
                //{
                //    // Limit speed on sharp steering
                //    // TODO: Learn this?
                //    tempMaxVelocity = 5.0f;
                //}
                
                // Local maximum acceleration
                float tempMaxAcceleration = MathHelper.Clamp(
                    (tempMaxVelocity - velocity)
                        / (accelerationFactors[velocity] * timeStep),
                    -maxAcceleration,
                    maxAcceleration);

                if (velocity < targetVelocity)
                {
                    // As long as we are slower than our target velocity
                    // we should try to catch up!
                    acceleration = tempMaxAcceleration;
                }
                else
                {
                    float v = velocity;
                    float d = 0.0f;

                    // Calculate braking distance
                    while (d < targetDisplacement.Distance && v > targetVelocity)
                    {
                        float dv = -maxAcceleration * accelerationFactors[v] * timeStep;
                        float dd = (v + 0.5f * dv) * timeStep;
                        if (dd > 0.0f)
                            d += dd;
                        else
                            break;
                        v += dv;
                    }

                    if (d < targetDisplacement.Distance)
                    {
                        acceleration = tempMaxAcceleration;
                    }
                    else
                    {
                        acceleration = -maxAcceleration;
                    }
                }
            }
            //System.Console.WriteLine("velocity=" + velocity + "; yaw=" + yaw + "; acceleration=" + acceleration + "; targetDistance= " + targetDistance +"; d=" + d);
        }

        /// <summary>
        /// Raises the TargetReached event.
        /// </summary>
        /// <param name="e">Event arguments</param>
        protected virtual void OnTargetReached(TargetEventArgs e)
        {
            EventHandler<TargetEventArgs> handler = TargetReached;
            if (handler != null)
                handler(this, e);
        }

        protected static Displacement CalculateDisplacement(Vector3 point, Vector3 position,
            Plane rightPlane, Plane forwardPlane)
        {
            Displacement displacement = new Displacement();
            displacement.Distance = Vector3.Distance(position, point);
            displacement.RightDistance = rightPlane.DotCoordinate(point);
            displacement.FrontDistance = forwardPlane.DotCoordinate(point);

            displacement.Angle = (float)Math.Asin(
                Math.Abs(displacement.RightDistance / displacement.Distance));
            if (displacement.IsBehind)
                displacement.Angle = MathHelper.Pi - displacement.Angle;

            return displacement;
        }

        /// <summary>
        /// Creates a plane that runs through a point with a given normal vector.
        /// </summary>
        /// <param name="point">Source point</param>
        /// <param name="normal">Normal vector</param>
        /// <returns>Resulting Plane</returns>
        protected static Plane CreatePlane(Vector3 point, Vector3 normal)
        {
            normal.Normalize();
            Plane plane = new Plane(normal, 0.0f);
            plane.D = -plane.DotCoordinate(point);
            return plane;
        }


        #region Dynamic waypoints

        // MAGIC for dynamic waypoints
        private float v1Factor = 0.5f, v4Factor = 3.0f;
        private float toleranceFactor = 0.005f;

        protected Vector3[] CalculateDynamicWaypoints(Vector3 position, Vector3 positionTangent, Vector3 target, Vector3 targetTangent)
        {
            float dist = (position - target).Length();

            Vector3 v1 = position - positionTangent * dist * v1Factor;
            Vector3 v4 = target + targetTangent * dist * v4Factor;

            LinkedList<Vector3> dynWPs = CatmullRomAdaptive(v1, position, target, v4, dist * toleranceFactor);
            Vector3[] dynamicWaypoints = new Vector3[dynWPs.Count];
            dynWPs.CopyTo(dynamicWaypoints, 0);

            // move dynamic waypoints to outside of track
            int lastFrame = FindClosestTangentFrame(position);
            Track track = World.Instance.Track;

            for (int i = 0; i < dynamicWaypoints.Length; i++)
            {
                int closestFrame = FindClosestTangentFrame(dynamicWaypoints[i], lastFrame);
                lastFrame = closestFrame;

                Splines.PositionTangentUpRadius curTangentFrame = track.TangentFrames[closestFrame];
                Vector3 projection = FindClosestPointOnLine(curTangentFrame.Position, curTangentFrame.Tangent, dynamicWaypoints[i]);

                Vector3 dir = dynamicWaypoints[i] - projection;
                dir.Normalize();
                dynamicWaypoints[i] = projection + dir * curTangentFrame.Radius * 0.9f;
            }

            return dynamicWaypoints;
        }

        protected static LinkedList<Vector3> CatmullRomAdaptive(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4, float threshold)
        {
            if (threshold <= 0.0f)
                throw new ArgumentException("Threshold must be greater than zero", "threshold");

            LinkedList<Vector3> edge = new LinkedList<Vector3>();
            edge.AddFirst(v2);
            edge.AddLast(v3);

            CatmullRomAdaptiveRecursive(
                ref v1, ref v2, ref v3, ref v4,
                threshold, edge,
                edge.First,
                edge.Last,
                0.0f,
                1.0f);

            return edge;
        }

        private static float CatmullRomAdaptiveRecursive(
            ref Vector3 v1,
            ref Vector3 v2,
            ref Vector3 v3,
            ref Vector3 v4,
            float threshold,
            LinkedList<Vector3> list,
            LinkedListNode<Vector3> leftNode,
            LinkedListNode<Vector3> rightNode,
            float leftAmount,
            float rightAmount)
        {
            float middleAmount = 0.5f * (leftAmount + rightAmount);
            Vector3 middlePoint = 0.5f * (leftNode.Value + rightNode.Value);
            Vector3 precisePoint = Vector3.CatmullRom(v1, v2, v3, v4, middleAmount);
            float distance = Vector3.Distance(middlePoint, precisePoint);

            if (distance > threshold)
            {
                LinkedListNode<Vector3> middleNode = list.AddAfter(leftNode, precisePoint);

                // Descend to the left
                CatmullRomAdaptiveRecursive(
                    ref v1, ref v2, ref v3, ref v4,
                    threshold, list,
                    leftNode,
                    middleNode,
                    leftAmount,
                    middleAmount);

                // Descend to the right
                CatmullRomAdaptiveRecursive(
                    ref v1, ref v2, ref v3, ref v4,
                    threshold, list,
                    middleNode,
                    rightNode,
                    middleAmount,
                    rightAmount);
            }

            return distance;
        }

        protected Vector3 FindClosestPointOnLine(Vector3 lineBase, Vector3 lineDir, Vector3 position)
        {
            float t = Vector3.Dot(lineDir, position - lineBase);
            return lineBase + t * lineDir;
        }

        #endregion

        #region Find tangent frame

        protected int FindClosestTangentFrame(Vector3 position)
        {
            Track track = World.Instance.Track;
            int count = track.TangentFrames.Length;

            float distance = Single.MaxValue;
            int frame = 0;

            for (int i = 0; i < count; ++i)
            {
                float d = Vector3.DistanceSquared(track.TangentFrames[i].Position, position);
                if (d < distance)
                {
                    frame = i;
                    distance = d;
                }
            }

            return frame;
        }

        protected int FindClosestTangentFrame(Vector3 position, int startAtFrame)
        {
            // FIXME: apparently this doesn't work correctly yet, use slower version for now
            return FindClosestTangentFrame(position);


            if (startAtFrame < 0)
                return FindClosestTangentFrame(position);

            Track track = World.Instance.Track;
            int count = track.TangentFrames.Length;

            int frame = startAtFrame;
            float distance = Vector3.DistanceSquared(track.TangentFrames[frame].Position, position);

            // search forwards
            while (Vector3.DistanceSquared(track.TangentFrames[(frame + 1) % count].Position, position) < distance)
                frame = (frame + 1) % count;

            // search backwards
            while (Vector3.DistanceSquared(track.TangentFrames[(frame - 1 + count) % count].Position, position) < distance)
                frame = (frame - 1 + count) % count;

            return frame;
        }

        #endregion

        protected virtual void Configure()
        {
        }

        #endregion

        #region Private Methods

        private void Configuration_Changed(object sender,
            Configuration.ChangedEventArgs e)
        {
            Configure();
        }

        #endregion 
    }
}