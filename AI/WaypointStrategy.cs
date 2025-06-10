using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using PraktWS0708.Entities;
using PraktWS0708.Utils;

namespace PraktWS0708.AI
{
    public class WaypointStrategy : SteeringStrategy
    {
        #region Private Fields

        private bool renderWaypoints = false;
        private List<Vector3[]> waypoints = null;
        private Vector3[] dynamicWaypoints = null;

        // must be uneven. routes created will be ideal line and (n - 1)/2 routes on each side of 
        // ideal line, with angular distance of PI/8
        private int numberOfRoutes = 9;

        private int currentRoute;
        private int currentTargetFrame;
        private int lastClosestFrame = -1;
        //private Vector3 currentTarget;
        //private int lastTime;


        // MAGIC
        private int trackLookahead = 8;
        private int findObstacleLookahead = 30;
        private int dynamicWaypointsLookahead = 5;

        #endregion

        #region Public Constructors

        public WaypointStrategy(SteeringAgent agent)
            : base(agent)
        {
        }

        #endregion

        #region Public Methods

        public override void Initialize()
        {
            CalculateWaypoints();

            //lastTime = -1;
        }

        public override void Update(GameTime gameTime)
        {
#if false
            TargetPosition = World.Instance.PlayersShip.Position;
            TargetVelocity = 0.0f;
            TargetDistanceThreshold = 3.0f;

            UpdateSteering();
            return;
#endif

            // FIXME: This should is caused by a missing call to Initialize()
            if (waypoints == null)
                CalculateWaypoints();

            BaseEntity entity = Agent.Entity;
            int tangentFrameCount = waypoints[0].Length;

            int closestTangentFrame = FindClosestTangentFrame(entity.Position, lastClosestFrame);

            //float speedFactor = entity.PhysicsPlugin.Velocity / 0.01f;
            //int lookahead = Math.Max(1, (int)(25 * speedFactor));
            int targetFrame = (closestTangentFrame + trackLookahead) % tangentFrameCount;

            float ownBoundingSphereRadius = ((Physics.RigidBody)entity.PhysicsPlugin).BoundingSphere.Radius;

            float safetyDistance = ownBoundingSphereRadius * 2.0f;

            int newRoute = 0;

            // find obstacles/other ships
            int leastBadRoute = -1, leastBadRouteObstacleFrame = -1;
            float leastBadRouteObstacleDist = 0.0f;
            int firstObstacleFrame = Int32.MaxValue;
            while (newRoute < this.waypoints.Count)
            {
                bool routeGood = true;
                foreach (BaseEntity obstacle in World.Instance.Objects.Values)
                {
                    if (obstacle == Agent.Entity)
                        continue;

                    if (!(obstacle.PhysicsPlugin is Physics.RigidBody))
                        continue;

                    // don't avoid powerups ;)
                    if (obstacle.LogicPlugin is Logic.PowerUp)
                        continue;

                    //// FIXME? ignore the bomb..
                    //if (obstacle.LogicPlugin is Logic.Bomb)
                    //    continue;

                    // FIXME? ignore other ships for now, works better this way
                    //        if we don't want to ignore them, we should treat them differently than obstacles..
                    if (obstacle.LogicPlugin is Logic.Ship)
                        continue;

                    float minDistance = ((Physics.RigidBody)obstacle.PhysicsPlugin).BoundingSphere.Radius + ownBoundingSphereRadius;
                    minDistance += safetyDistance;

                    for (int i = 0; i < findObstacleLookahead; i++)
                    {
                        int tempTargetFrame = (closestTangentFrame + i) % tangentFrameCount;
                        Vector3 tempTarget = waypoints[newRoute][tempTargetFrame];
                        Vector3 away = tempTarget - obstacle.Position;
                        float dist = away.Length();
                        // TODO: measure distance on outside of track instead of euclidean
                        if (dist < minDistance /*&& Vector3.Dot(away, waypoints[0][tempTargetFrame] - entity.Position) > 0*/)
                        {
                            // bad route
                            routeGood = false;
                            // least bad? ;)
                            if (i > leastBadRouteObstacleFrame || (i == leastBadRouteObstacleFrame && dist > leastBadRouteObstacleDist))
                            {
                                leastBadRoute = newRoute;
                                leastBadRouteObstacleFrame = i;
                                leastBadRouteObstacleDist = dist;
                            }
                            if (i < firstObstacleFrame)
                                firstObstacleFrame = i;
                            break;
                        }
                    }
                    if (!routeGood)
                        break;
                }
                if (routeGood)
                    break;

                newRoute++;
            }

            if (newRoute >= this.waypoints.Count)
            {
                // found no route without obstacles

                newRoute = leastBadRoute;

                // TODO should slow down probably?
            }

            if (firstObstacleFrame < trackLookahead)
                targetFrame = (closestTangentFrame + firstObstacleFrame) % tangentFrameCount;


            bool switchedRoute = (currentRoute != newRoute);
            bool switchedTargetFrame = (currentTargetFrame != targetFrame);

            this.currentRoute = newRoute;
            this.currentTargetFrame = targetFrame;

            bool updateDynamicWaypoints = (switchedRoute || switchedTargetFrame || dynamicWaypoints == null);

            if (updateDynamicWaypoints)
            {
                Vector3 targetPos = waypoints[currentRoute][currentTargetFrame];
                Vector3 targetTangent = World.Instance.Track.TangentFrames[currentTargetFrame].Tangent;
                this.dynamicWaypoints = this.CalculateDynamicWaypoints(this.Agent.Entity.Position, this.Agent.Entity.Orientation.Forward, targetPos, targetTangent);
            }

            // find closest dynamic waypoint
            float bestDist = Single.MaxValue;
            int bestIndex = -1;
            for (int i = 0; i < this.dynamicWaypoints.Length; ++i)
            {
                float d = Vector3.Distance(this.dynamicWaypoints[i], entity.Position);
                if (d < bestDist)
                {
                    bestIndex = i;
                    bestDist = d;
                }
            }

            Vector3 target;
            if (bestIndex + dynamicWaypointsLookahead < dynamicWaypoints.Length)
                target = dynamicWaypoints[bestIndex + dynamicWaypointsLookahead];
            else
                target = waypoints[currentRoute][currentTargetFrame];

            if (renderWaypoints)
            {
                // update main waypoints
                Rendering.RenderManager.Instance.HighlightWaypoint = targetFrame;
                if (switchedRoute)
                    Rendering.RenderManager.Instance.SetWaypoints(waypoints[newRoute]);

                // update dynamic waypoints
                if (bestIndex + dynamicWaypointsLookahead < dynamicWaypoints.Length)
                    Rendering.RenderManager.Instance.HighlightWaypoint2 = bestIndex + dynamicWaypointsLookahead;
                else
                    Rendering.RenderManager.Instance.HighlightWaypoint2 = -1;
                if (updateDynamicWaypoints)
                    Rendering.RenderManager.Instance.SetWaypoints2(dynamicWaypoints);
            }

            TargetPosition = target;
            UpdateSteering();

            //System.Console.WriteLine("Velocity: " + Velocity
            //    + "; TargetVelocity: " + TargetVelocity
            //    + "; Yaw: " + Yaw + "; Acc: " + Acceleration);

            lastClosestFrame = closestTangentFrame;
        }

        #endregion

        #region Protected Methods

        protected override void Configure()
        {
            base.Configure();

            switch (Settings.Configuration.difficulty)
            {
                case Settings.Configuration.Difficulty.Easy:
                    this.MaxAcceleration = 0.7f;
                    this.MaxYaw = 0.8f;
                    break;

                case Settings.Configuration.Difficulty.Medium:
                    this.MaxAcceleration = 1.0f;
                    this.MaxYaw = 1.0f;
                    break;

                case Settings.Configuration.Difficulty.Hard:
                    this.MaxAcceleration = 1.15f;
                    this.MaxYaw = 1.35f;
                    break;
            }
        }

        #endregion

        #region Private Methods

        private void CalculateWaypoints()
        {
            Track track = World.Instance.Track;
            BaseEntity entity = Agent.Entity;
            int count = track.TangentFrames.Length;
            this.waypoints = new List<Vector3[]>();
            for (int i = 0; i < numberOfRoutes; i++)
                this.waypoints.Add(new Vector3[count]);
            Vector3[] rightVectors = new Vector3[count];

            // Don't start calculation at first TangetFrame, but at first turn.
            // and the calculation is done backwards (thus firstTurn is actually the last turn of the track).
            // FIXME: i think the tangentFrames actually are backwards, so calculation is done forward right now
            // anyway, it is easier to get a good waypoint chain this way.

            int firstTurn = 0;
            Splines.PositionTangentUpRadius targetFrame = track.TangentFrames[firstTurn];
            Splines.PositionTangentUpRadius turnDetector = track.TangentFrames[(firstTurn + 10) % count];

            Vector3 turnAxis = Vector3.Cross(targetFrame.Tangent, turnDetector.Tangent);
            while (turnAxis.Length() < 0.01f)
            {
                firstTurn++;
                targetFrame = track.TangentFrames[firstTurn];
                turnDetector = track.TangentFrames[(firstTurn + 10) % count];

                turnAxis = Vector3.Cross(targetFrame.Tangent, turnDetector.Tangent);
            }

            // rightVector Calculation first, waypoint are calculated seperately

            rightVectors[firstTurn] = turnAxis;

            for (int i = 1; i < count; i++)
            {
                //as we started at firstTurn (which is i = 0), we need to go to firstTurn - 1, which is 
                //equal to (count - 1 + firstTurn) % count, and count - 1 is exactly the last i of the loop.

                int j = (i + firstTurn) % count;

                targetFrame = track.TangentFrames[j];
                turnDetector = track.TangentFrames[(j + 10) % count];

                turnAxis = Vector3.Cross(targetFrame.Tangent, turnDetector.Tangent);
                if (turnAxis.Length() < 0.01f)
                    rightVectors[j] = rightVectors[(j + 1) % count];
                else
                    rightVectors[j] = turnAxis;
            }

            // here come the waypoints!
            for (int i = 0; i < count; i++)
            {
                Vector3 upVector = Vector3.Cross(rightVectors[i], track.TangentFrames[i].Tangent); //reihenfolge? (müsst passen)
                waypoints[0][i] = track.TangentFrames[i].Position + (-upVector * track.TangentFrames[i].Radius * 0.9f);
                for (int j = 1; j < (numberOfRoutes + 1)/2; j++)
                {
                    waypoints[2 * j - 1][i] = track.TangentFrames[i].Position + (Vector3.Transform(-upVector, Matrix.CreateFromAxisAngle(track.TangentFrames[i].Tangent, j * (float)Math.PI / 8.0f )) * track.TangentFrames[i].Radius * 0.9f);
                    waypoints[2 * j][i] = track.TangentFrames[i].Position + (Vector3.Transform(-upVector, Matrix.CreateFromAxisAngle(track.TangentFrames[i].Tangent, -j * (float)Math.PI / 8.0f)) * track.TangentFrames[i].Radius * 0.9f);
                }
            }
            
            if (!Rendering.RenderManager.Instance.HasWaypoints)
            {
                renderWaypoints = true;
                Rendering.RenderManager.Instance.SetWaypoints(waypoints[0]);
            }
        }

        #endregion
    }
}
/*
 * Backwards Calculation. stored here, in case the tangentFrames are the other way round. see other comments
 * 
            int firstTurn = count - 1;
            Splines.PositionTangentUpRadius targetFrame = track.TangentFrames[firstTurn];
            Splines.PositionTangentUpRadius turnDetector = track.TangentFrames[(firstTurn + 10) % count];

            Vector3 turnAxis = Vector3.Cross(targetFrame.Tangent, turnDetector.Tangent);
            while (turnAxis.Length() < 0.01f)
            {
                firstTurn--;
                targetFrame = track.TangentFrames[firstTurn];
                turnDetector = track.TangentFrames[(firstTurn + 10) % count];

                turnAxis = Vector3.Cross(targetFrame.Tangent, turnDetector.Tangent);
            }

            // upVector Calculation first, waypoint is frame.pos + (- this.upVector * frame.radius)

            upVectors[firstTurn] = Vector3.Cross(turnAxis, targetFrame.Tangent); //FIXME: reihenfolge ???

            for (int i = 1; i < count; i++)
            {
                //as we started at firstTurn (which is i = 0), we need to go to firstTurn - 1, which is 
                // equal to (count - 1 + firstTurn) % count, and count - 1 is the last i of the loop.

                // FIXME: comment above no longer valid. its even more trickier now with backwards calculation.

                int j = (firstTurn - i + count) % count;
*/
