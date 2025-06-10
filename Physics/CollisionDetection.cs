#region Using Statements
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PraktWS0708.Rendering;
#endregion

namespace PraktWS0708.Physics
{
    public enum CollisionState { NoCollision, Touching, Interpenetrating, OutOfTrack, Error }

    #region CollisionResult

    public class SphereCollisionResult
    {
        #region Fields

        public CollisionState m_CollisionState;
        public Vector3 m_vPoint;    // position of collision
        public Vector3 m_vNormal;  // normal of surface
        public float m_fDistance; // distance between the objects
        public RigidBody m_oRigidBody;
        public RigidBody m_oRigidBody2;

        #endregion

        #region Initialization

        public SphereCollisionResult()
        {
            m_CollisionState = CollisionState.Error;
            m_vPoint = Vector3.Zero;
            m_vNormal = Vector3.Zero;
            m_fDistance = 0.0f;
        }

        public SphereCollisionResult(CollisionState oCollisionState, Vector3 vNormal, Vector3 vPosition, float fDistance, RigidBody oRigidBody, RigidBody oRigidBody2)
        {
            m_CollisionState = oCollisionState;
            m_vPoint = vPosition;
            m_vNormal = vNormal;
            m_fDistance = fDistance;
            m_oRigidBody = oRigidBody;
            m_oRigidBody2 = oRigidBody2;
        }

        #endregion
    }

    public class TrackCollisionResult
    {
        #region Fields
        public CollisionState m_CollisionState;
        public Vector3 m_vPoint;    // position of collision
        public Vector3 m_vNormal;  // normal of surface
        public Vector3 m_vPerpendicularFoot;    //
        public float m_fT;  // parameter [0..1] - position in tube
        public float m_fDistance; // distance to tube center

        #endregion

        #region Initialization
          public TrackCollisionResult()
        {
            m_CollisionState = CollisionState.Error;
            m_vPoint = Vector3.Zero;
            m_vNormal = Vector3.Zero;
            m_fT = 0.0f;
            m_fDistance = 0.0f;
        }

        public TrackCollisionResult(CollisionState oCollisionState, Vector3 vNormal, Vector3 vPosition, float fT)
        {
            m_CollisionState = oCollisionState;
            m_vPoint = vPosition;
            m_vNormal = vNormal;
            m_fT = fT;
        }
        #endregion
    }

    public class MeshCollisionPoint
    {
        public Vector3[] m_vaTriangle; // vertices of one triangle
        public Vector3 m_vLinkedVector; // the normal including the length: CollisionPoint to Object
        public Vector3 m_CollisionPoint;
        public float m_fDistance;
        public Vector3 m_vNormal;
        #region init
        public MeshCollisionPoint()
        {
            m_vaTriangle = null;
            m_vLinkedVector = Vector3.Zero;
            m_CollisionPoint = Vector3.Zero;
            m_fDistance = 0f;
            
        }
        public MeshCollisionPoint(Vector3[] vaTriangle, Vector3 vLinkedVector, Vector3 vCollisionPoint, float fDistance)
        {
            m_CollisionPoint = vCollisionPoint;
            m_fDistance = fDistance;
            m_vaTriangle = vaTriangle;
            m_vLinkedVector = vLinkedVector;
            
        }
        #endregion

    }

    
    public class MeshCollisionResult
    {
        public CollisionState m_CollisionState;
        public MeshCollisionPoint[] m_CollisionPoints; //all collision-points
        public int m_iCheckedTriangles; // for debug use: number of checked triangles

        #region init
        public MeshCollisionResult()
        {
            m_CollisionState = CollisionState.Error;
            m_CollisionPoints = null;
            m_iCheckedTriangles = 0;
        }
        public MeshCollisionResult(CollisionState oCollisionState, MeshCollisionPoint [] oCollisionPoints)
        {
            m_CollisionState = oCollisionState;
            m_CollisionPoints = oCollisionPoints;
            m_iCheckedTriangles = 0;
        }
        #endregion
    }

    #endregion

    #region BoundingTree

    public class BoundingBoxTree
    {
        public BoundingBoxTree[] m_Children;
        public BoundingBox m_Box;
        //public Vector3 m_vBoxPosition;
        public Vector3 [] m_Triangles;
        public BoundingBoxTree()
        {
            m_Children = null;
            m_Box = new BoundingBox(Vector3.Zero, Vector3.Zero);
        }
        public BoundingBoxTree(BoundingBox box, Vector3 [] triangles)
        {
            this.m_Box = box;
            this.m_Triangles=triangles;
            m_Children = null;
        }
        public void setChildren(BoundingBoxTree[] children)
        {
            this.m_Children = children;
        }
    }

    


    #endregion

    public class CollisionDetection
    {
        #region MeshCollision

        #region Generating the BoundingBoxTree

        public static bool isPointInBoundingBox(BoundingBox oBox, Vector3 vPoint)
        {
            if (vPoint.X < oBox.Min.X || vPoint.Y < oBox.Min.Y || vPoint.Z < oBox.Min.Z)
                return false;
            if (vPoint.X > oBox.Max.X || vPoint.Y > oBox.Max.Y || vPoint.Z > oBox.Max.Z)
                return false;
            return true;
        }

        public static bool isTriangleInBoundingBox(BoundingBox oBox, Vector3 a, Vector3 b, Vector3 c)
        {
            if (isPointInBoundingBox(oBox, a) && isPointInBoundingBox(oBox, b) && isPointInBoundingBox(oBox, c))
                return true;
            return false;
        }

        // generates a bounding-box tree of a mesh (vertexPositionNormalTexture-Array)
        public static BoundingBoxTree generateBoundingBoxTreeFromTriangles(VertexPositionNormalTexture[] oaTriangles, float fScale, Matrix mOrientation, Vector3 vPosition)
        {
            Vector3[] vaTriangles = new Vector3[oaTriangles.Length];
            for (int i = 0; i < oaTriangles.Length; i++)
            {
                vaTriangles[i] = oaTriangles[i].Position*fScale;
            }
            //apply Orientation and Translation to triangles
            for (int i = 0; i < oaTriangles.Length; i++)
            {
                vaTriangles[i] = Vector3.Transform(vaTriangles[i], mOrientation);
                vaTriangles[i] += vPosition;
            }
            //recursive call
            return generateBoundingBoxTreeFromTriangles(vaTriangles);
        }

       private static BoundingBoxTree generateBoundingBoxTreeFromTriangles(Vector3[] vaTriangles)
        {
            //gereate the Main BoundingBox
            int i;
            Vector3 min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            Vector3 max = new Vector3(float.MinValue, float.MinValue, float.MinValue);
            for (i = 0; i < vaTriangles.Length; i++)
            {

                if (vaTriangles[i].X < min.X)
                    min.X = vaTriangles[i].X;
                if (vaTriangles[i].Y < min.Y)
                    min.Y = vaTriangles[i].Y;
                if (vaTriangles[i].Z < min.Z)
                    min.Z = vaTriangles[i].Z;

                if (vaTriangles[i].X > max.X)
                    max.X = vaTriangles[i].X;
                if (vaTriangles[i].Y > max.Y)
                    max.Y = vaTriangles[i].Y;
                if (vaTriangles[i].Z > max.Z)
                    max.Z = vaTriangles[i].Z;

            }
            return generateBoundingBoxSubTree(new BoundingBox(min, max), vaTriangles, Vector3.Zero); //auch hier muss beachtet werden, dass die Position schon vorher gesetzt wurde
        }

        private static BoundingBoxTree generateBoundingBoxSubTree(BoundingBox oBox, Vector3[] vaTriangles, Vector3 vBoxPosition)
        {
            if (vaTriangles == null || oBox==null)
                return null;
            if (vaTriangles.Length == 0)
                return null;
            if (vaTriangles.Length % 3 != 0) // corrupted triangle-list
                return null;
            if (oBox.Min == oBox.Max || vaTriangles.Length<=30)
                //return new BoundingBoxTree(oBox,vBoxPosition,vaTriangles);
                return new BoundingBoxTree(oBox, vaTriangles); 


            BoundingBoxTree oTree = new BoundingBoxTree();
            int i,j;

            //generate sub-boxes
            BoundingBox[] oSubBoxes = new BoundingBox[9];
            Vector3 vTemp;
            Vector3 vAverageBoxLength = new Vector3((oBox.Max.X - oBox.Min.X) / 2f, (oBox.Max.Y - oBox.Min.Y) / 2f, (oBox.Max.Z - oBox.Min.Z) / 2f);
            vTemp=oBox.Min + (vAverageBoxLength * new Vector3(0f, 0f, 0f));
            oSubBoxes[0] = new BoundingBox(vTemp, vTemp+vAverageBoxLength);
            vTemp=oBox.Min + (vAverageBoxLength * new Vector3(0f, 0f, 1f));
            oSubBoxes[1] = new BoundingBox(vTemp, vTemp+vAverageBoxLength);
            vTemp=oBox.Min + (vAverageBoxLength * new Vector3(0f, 1f, 0f));
            oSubBoxes[2] = new BoundingBox(vTemp, vTemp+vAverageBoxLength);
            vTemp=oBox.Min + (vAverageBoxLength * new Vector3(0f, 1f, 1f));
            oSubBoxes[3] = new BoundingBox(vTemp, vTemp+vAverageBoxLength);
            vTemp=oBox.Min + (vAverageBoxLength * new Vector3(1f, 0f, 0f));
            oSubBoxes[4] = new BoundingBox(vTemp, vTemp+vAverageBoxLength);
            vTemp=oBox.Min + (vAverageBoxLength * new Vector3(1f, 0f, 1f));
            oSubBoxes[5] = new BoundingBox(vTemp, vTemp+vAverageBoxLength);
            vTemp=oBox.Min + (vAverageBoxLength * new Vector3(1f, 1f, 0f));
            oSubBoxes[6] = new BoundingBox(vTemp, vTemp+vAverageBoxLength);
            vTemp=oBox.Min + (vAverageBoxLength * new Vector3(1f, 1f, 1f));
            oSubBoxes[7] = new BoundingBox(vTemp, vTemp+vAverageBoxLength);
            
            oSubBoxes[8] = oBox;

            List<Vector3>[] oTriangleLists = new List<Vector3>[9];
            for (i = 0; i < 9; i++)
                oTriangleLists[i] = new List<Vector3>();

            for(i=0;i<vaTriangles.Length;i+=3)
            {
                for(j=0;j<9;j++)
                {
                    if(isTriangleInBoundingBox(oSubBoxes[j],vaTriangles[i],vaTriangles[i+1],vaTriangles[i+2]))
                    {
                        oTriangleLists[j].Add(vaTriangles[i]);
                        oTriangleLists[j].Add(vaTriangles[i+1]);
                        oTriangleLists[j].Add(vaTriangles[i+2]);
                        break;
                    }
                }
            }

            oTree.m_Box=oBox;
            oTree.m_Triangles=oTriangleLists[8].ToArray();
            BoundingBoxTree [] children = new BoundingBoxTree[8];
            for(i=0;i<8;i++)
                children[i]=generateBoundingBoxSubTree(oSubBoxes[i],oTriangleLists[i].ToArray(),vBoxPosition);
            oTree.m_Children=children;

            return oTree;
                


        }

        #endregion



        #endregion

        #region RigidBody <-> RigidBody

        /// <summary>
        /// check collision between two RigidBody's
        /// </summary>
        /// <param name="oRigidBody">first RigidBody object</param>
        /// <param name="oRigidBody2">second RigidBody object</param>
        /// <returns></returns>
        /// 

        public static bool CheckCollisionSphereSphereSimple(RigidBody oRigidBody, RigidBody oRigidBody2)
        {
            float fRadius1 = oRigidBody.BoundingSphere.m_fRadius;
            float fRadius2 = oRigidBody2.BoundingSphere.m_fRadius;
            
            Vector3 vLinkedVector = oRigidBody2.DesState.m_vPosition - oRigidBody.DesState.m_vPosition;
            if (vLinkedVector.Length() <= fRadius1 + fRadius2)
                return true;
            return false;
        }

        public static SphereCollisionResult CheckCollisionSphereSphere(RigidBody oRigidBody, RigidBody oRigidBody2)
        {
            
            
            //if (Math.Abs(oRigidBody.m_iCollisionSearch - oRigidBody2.m_iCollisionSearch) > 5)
            if (Vector3.DistanceSquared(oRigidBody2.DesState.m_vPosition, oRigidBody.DesState.m_vPosition) > 
                Math.Pow(oRigidBody.BoundingSphere.m_fRadius+oRigidBody2.BoundingSphere.m_fRadius,2f))
            { 
               // oCollisionResult.m_CollisionState = CollisionState.NoCollision;
                return null;
            }
            SphereCollisionResult oCollisionResult = new SphereCollisionResult(CollisionState.Error, Vector3.Zero, Vector3.Zero, -1.0f, oRigidBody, oRigidBody2);

            
            float fRadius = oRigidBody.BoundingSphere.m_fRadius;
            float fRadius2 = oRigidBody2.BoundingSphere.m_fRadius;
            

            Vector3 vNormal = oRigidBody.DesState.m_vPosition - oRigidBody2.DesState.m_vPosition;
            float fDistance = vNormal.Length();
            if (vNormal != Vector3.Zero) vNormal.Normalize();

            Vector3 vCollisionPoint = oRigidBody.DesState.m_vPosition + vNormal * fRadius;
            oCollisionResult.m_vNormal = vNormal;
            oCollisionResult.m_vPoint = vCollisionPoint;
            oCollisionResult.m_fDistance = fDistance;
            // check for negative normal velocity
            Matrix matMatrix = Matrix.Identity;
            
            if (vNormal != Vector3.Zero)
            {
                Vector3 vPerpendicularToNormal = new Vector3(vNormal.Y * vNormal.Z, -2 * vNormal.X * vNormal.Z, vNormal.X * vNormal.Y);
                matMatrix = Matrix.CreateLookAt(Vector3.Zero, vNormal, vPerpendicularToNormal);
            }

            
            if (!oRigidBody.m_Flags.m_bMoveable)
            {
                oRigidBody.DesState.m_vLinearVelocity = Vector3.Zero;
            }
            if (!oRigidBody2.m_Flags.m_bMoveable)
            {
                oRigidBody2.DesState.m_vLinearVelocity = Vector3.Zero;
            }

            if ((fDistance > fRadius + fRadius2) ||
                (Vector3.Transform(oRigidBody.DesState.m_vLinearVelocity, matMatrix).Z <
                 Vector3.Transform(oRigidBody2.DesState.m_vLinearVelocity, matMatrix).Z))
            {
                oCollisionResult.m_CollisionState = CollisionState.NoCollision;
            }
            else if (fDistance >= fRadius + fRadius2)
            {
                oCollisionResult.m_CollisionState = CollisionState.Touching;
            }
            else
            {
                oCollisionResult.m_CollisionState = CollisionState.Interpenetrating;
            }
            
            return oCollisionResult;
        }

        public static float calculateBoundingSphereRadius(VertexPositionNormalTexture[] oaTriangles, float fScale)
        {
            float fDistance = 0f;
            for (int i = 0; i < oaTriangles.Length; i++)
            {
                if (oaTriangles[i].Position.Length()*fScale > fDistance)
                    fDistance = oaTriangles[i].Position.Length()*fScale;
            }
            return fDistance;
        }

        public static float calculateBoundingSphereRadius(Vector3[] vaTriangles)
        {
            float fDistance = 0f;
            for (int i = 0; i < vaTriangles.Length; i++)
            {
                if (vaTriangles[i].Length() > fDistance)
                    fDistance = vaTriangles[i].Length();
            }
            return fDistance;
        }

        //Point In A Triangle Algorithms are explained here: http://www.blackpawn.com/texts/pointinpoly/default.html
        
        #region old isPointInTriangle
        /*
        private static bool arePointsOnSameLineSide(Vector3 vP1, Vector3 vP2, Vector3 vA, Vector3 vB)
        {
            Vector3 vCP1 = Vector3.Cross(vB - vA, vP1 - vA);
            Vector3 vCP2 = Vector3.Cross(vB - vA, vP2 - vA);
            if (Vector3.Dot(vCP1, vCP2) >= 0)
                return true;
            return false;
        }

        
        public static bool isPointInTriangleOld(Vector3 vPoint, Vector3 vA, Vector3 vB, Vector3 vC)
        {
            if (arePointsOnSameLineSide(vPoint, vA, vB, vC)
                && arePointsOnSameLineSide(vPoint, vB, vA, vC)
                && arePointsOnSameLineSide(vPoint, vC, vA, vB))
                return true;
            return false;
        }*/
        #endregion

        public static bool isPointInTriangle(Vector3 vPoint, Vector3 vA, Vector3 vB, Vector3 vC)
        {
            Vector3 v0 = vC - vA;
            Vector3 v1 = vB - vA;
            Vector3 v2 = vPoint - vA;
            float dot00 = Vector3.Dot(v0, v0);
            float dot01 = Vector3.Dot(v0, v1);
            float dot02 = Vector3.Dot(v0, v2);
            float dot11 = Vector3.Dot(v1, v1);
            float dot12 = Vector3.Dot(v1, v2);
            float invDenom = 1 / (dot00 * dot11 - dot01 * dot01);
            float u = (dot11 * dot02 - dot01 * dot12) * invDenom;
            float v = (dot00 * dot12 - dot01 * dot02) * invDenom;
            return (u >= 0) && (v >= 0) && (u + v <= 1);
        }

        public static bool CheckCollisionBoundingSphereBoundingBox(BoundingBox oBox, Vector3 vBoxPosition, BoundingSphere oSphere, Vector3 vSpherePosition)
        {
            oBox.Min += vBoxPosition;
            oBox.Max += vBoxPosition;

            int i;

            //is the BoundsSphere-Middle-Point in the BoundingBox?
            if (vSpherePosition.X >= oBox.Min.X
                && vSpherePosition.Y >= oBox.Min.Y
                && vSpherePosition.Z >= oBox.Min.Z
                && vSpherePosition.X <= oBox.Max.X
                && vSpherePosition.Y <= oBox.Max.Y
                && vSpherePosition.Z <= oBox.Max.Z)
                return true;

            //calc min distance from lines

            //calc all corner-points
            Vector3[] vaCornerPoints = new Vector3[8];
            vaCornerPoints[0] = oBox.Min;
            vaCornerPoints[1] = oBox.Min * new Vector3(0f, 0f, 1f) + oBox.Max * new Vector3(1f, 1f, 0f);
            vaCornerPoints[2] = oBox.Min * new Vector3(0f, 1f, 0f) + oBox.Max * new Vector3(1f, 0f, 1f);
            vaCornerPoints[3] = oBox.Min * new Vector3(0f, 1f, 1f) + oBox.Max * new Vector3(1f, 0f, 0f);
            vaCornerPoints[4] = oBox.Min * new Vector3(1f, 0f, 0f) + oBox.Max * new Vector3(0f, 1f, 1f);
            vaCornerPoints[5] = oBox.Min * new Vector3(1f, 0f, 1f) + oBox.Max * new Vector3(0f, 1f, 0f);
            vaCornerPoints[6] = oBox.Min * new Vector3(1f, 1f, 0f) + oBox.Max * new Vector3(0f, 0f, 1f);
            vaCornerPoints[7] = oBox.Min * new Vector3(1f, 1f, 1f) + oBox.Max * new Vector3(0f, 0f, 0f);


            //plane-collision
            PerpendicularDistancePlanePointResult oPerpendicularPlanePointResult;
            Vector3 [,] vaPlanes = new Vector3[6,3];
            vaPlanes[0, 0] = vaCornerPoints[0];
            vaPlanes[0, 1] = vaCornerPoints[4];
            vaPlanes[0, 2] = vaCornerPoints[2];
            vaPlanes[1, 0] = vaCornerPoints[0];
            vaPlanes[1, 1] = vaCornerPoints[4];
            vaPlanes[1, 2] = vaCornerPoints[1];
            vaPlanes[2, 0] = vaCornerPoints[0];
            vaPlanes[2, 1] = vaCornerPoints[2];
            vaPlanes[2, 2] = vaCornerPoints[1];
            vaPlanes[3, 0] = vaCornerPoints[4];
            vaPlanes[3, 1] = vaCornerPoints[6];
            vaPlanes[3, 2] = vaCornerPoints[5];
            vaPlanes[4, 0] = vaCornerPoints[1];
            vaPlanes[4, 1] = vaCornerPoints[5];
            vaPlanes[4, 2] = vaCornerPoints[3];
            vaPlanes[5, 0] = vaCornerPoints[2];
            vaPlanes[5, 1] = vaCornerPoints[6];
            vaPlanes[5, 2] = vaCornerPoints[3];

            for (i = 0; i < 6; i++)
            {
                oPerpendicularPlanePointResult = calcPerpendicularDistancePlanePoint(vSpherePosition, vaPlanes[i, 0], vaPlanes[i, 1], vaPlanes[i, 2]);
                float fS = oPerpendicularPlanePointResult.fS;
                float fT = oPerpendicularPlanePointResult.fT;
                if (fS >= 0 && fS <= 1 && fT >= 0 && fT <= 1 && oPerpendicularPlanePointResult.vLinkedvector.Length() <= oSphere.m_fRadius)
                    return true;
            }

            
            //line-collision
            PerpendicularDistanceLinePointResult oPerpendicularLinePointResult;
            Vector3[,] vaLines = new Vector3[12, 2];
            vaLines[0, 0] = vaCornerPoints[0];
            vaLines[0, 1] = vaCornerPoints[4];
            vaLines[1, 0] = vaCornerPoints[0];
            vaLines[1, 1] = vaCornerPoints[2];
            vaLines[2, 0] = vaCornerPoints[0];
            vaLines[2, 1] = vaCornerPoints[1];
            vaLines[3, 0] = vaCornerPoints[4];
            vaLines[3, 1] = vaCornerPoints[5];
            vaLines[4, 0] = vaCornerPoints[4];
            vaLines[4, 1] = vaCornerPoints[6];
            vaLines[5, 0] = vaCornerPoints[2];
            vaLines[5, 1] = vaCornerPoints[6];
            vaLines[6, 0] = vaCornerPoints[1];
            vaLines[6, 1] = vaCornerPoints[5];
            vaLines[7, 0] = vaCornerPoints[5];
            vaLines[7, 1] = vaCornerPoints[7];
            vaLines[8, 0] = vaCornerPoints[6];
            vaLines[8, 1] = vaCornerPoints[7];
            vaLines[9, 0] = vaCornerPoints[1];
            vaLines[9, 1] = vaCornerPoints[3];
            vaLines[10, 0] = vaCornerPoints[2];
            vaLines[10, 1] = vaCornerPoints[3];
            vaLines[11, 0] = vaCornerPoints[3];
            vaLines[11, 1] = vaCornerPoints[7];

            for (i = 0; i < 12; i++)
            {
                oPerpendicularLinePointResult = calcPerpendicularDistanceLinePoint(vSpherePosition, vaLines[i, 0], vaLines[i, 1]);
                float fT = oPerpendicularLinePointResult.fT;
                if (fT >= 0 && fT <= 1 && oPerpendicularLinePointResult.vLinkedVector.Length() <= oSphere.m_fRadius)
                    return true;
            }


            //calc every distance from every box-corner to the sphere.

            for(i=0;i<8;i++)
            {
                if((vaCornerPoints[i] - vSpherePosition).Length() <= oSphere.m_fRadius)
                    return true;
            }

            //if not returned yet -> false
            return false;
        }


        public static MeshCollisionResult CheckCollisionSphereMesh(BoundingBoxTree oTree, BoundingSphere oSphere, Vector3 vSpherePosition)
        {
            MeshCollisionResult result = new MeshCollisionResult();
            int iCheckedTriangles = 0;

            //valid oTree ??
            if (oTree == null || oSphere == null || vSpherePosition == null)
                //return new MeshCollisionResult(CollisionState.NoCollision, Vector3.Zero, Vector3.Zero, 0f, 0f, 0f);
                return new MeshCollisionResult(CollisionState.NoCollision, null);

            //are the boundobjects colliding each other?
            if (CheckCollisionBoundingSphereBoundingBox(oTree.m_Box, Vector3.Zero, oSphere, vSpherePosition) == false)
                //return new MeshCollisionResult(CollisionState.NoCollision, Vector3.Zero, Vector3.Zero, 0f, 0f, 0f);
                return new MeshCollisionResult(CollisionState.NoCollision, null);

            List<MeshCollisionPoint> lCollisionPoints = new List<MeshCollisionPoint>();

            //check all Triangles in this level
            PerpendicularDistancePlanePointResult oPerpendicularPlanePointResult;
            PerpendicularDistanceLinePointResult oPerpendicularLinePointResult;
            int i, j;
            for (i = 0; i < oTree.m_Triangles.Length; i += 3)
            {
                iCheckedTriangles++;
                Vector3[] triangle = new Vector3[3];
                triangle[0] = oTree.m_Triangles[i];
                triangle[1] = oTree.m_Triangles[i + 1];
                triangle[2] = oTree.m_Triangles[i + 2];

                //checking Plane-Collision
                oPerpendicularPlanePointResult = calcPerpendicularDistancePlanePoint(vSpherePosition, triangle[0], triangle[1], triangle[2]);
                
                if (isPointInTriangle(oPerpendicularPlanePointResult.vPerpendicularFoot, oTree.m_Triangles[i], oTree.m_Triangles[i + 1], oTree.m_Triangles[i + 2]))
                {
                    if (oPerpendicularPlanePointResult.vLinkedvector.Length() <= oSphere.m_fRadius)// * 2f) //die 2 ist dummerweise hardgecodet. das kommt daher, dass die raumschiffe ungefähr eine boundingsphere von 0.5 - 0.6 haben. gespeichert wird allerdings 0.3, weil die kollision zwischen zwei raumschiffen mit der echten boundsphere zu komisch aussähe!
                    {
                        
                        lCollisionPoints.Add(new MeshCollisionPoint(triangle, oPerpendicularPlanePointResult.vLinkedvector, oPerpendicularPlanePointResult.vPerpendicularFoot, oPerpendicularPlanePointResult.vLinkedvector.Length()));
                        result.m_CollisionState = CollisionState.Interpenetrating;
                        continue;
                        
                    }
                }


                //checking the edges
                bool found = false;
                for (j = 0; j < 3; j++)
                {
                    oPerpendicularLinePointResult = calcPerpendicularDistanceLinePoint(vSpherePosition, triangle[j], triangle[(j + 1) % 3]);
                    //if (oPerpendicularLinePointResult.vLinkedVector.Length() <= 0.4)
                    //    Console.WriteLine("disLine: " + oPerpendicularLinePointResult.vLinkedVector.Length());
                    if (oPerpendicularLinePointResult.fT >= 0 && oPerpendicularLinePointResult.fT <= 1)
                    {
                        if (oPerpendicularLinePointResult.vLinkedVector.Length() <= oSphere.m_fRadius)// * 2f) //die 2 ist dummerweise hardgecodet. das kommt daher, dass die raumschiffe ungefähr eine boundingsphere von 0.5 - 0.6 haben. gespeichert wird allerdings 0.3, weil die kollision zwischen zwei raumschiffen mit der echten boundsphere zu komisch aussähe!
                        {
                            lCollisionPoints.Add(new MeshCollisionPoint(triangle, oPerpendicularLinePointResult.vLinkedVector, oPerpendicularLinePointResult.vPerpendicularFoot, oPerpendicularLinePointResult.vLinkedVector.Length()));
                            result.m_CollisionState = CollisionState.Interpenetrating;
                            found = true;
                            break;
                        }
                    }
                }
                if (found)
                    continue; //next triangle


                //checking the vertices
                found = false;
                for(j=0;j<3;j++)
                {
                    Vector3 vLinkedVector = triangle[j]-vSpherePosition;
                    //if(vLinkedVector.Length()<=1)
                    //    Console.WriteLine("disPoint: " + vLinkedVector.Length());
                    if (vLinkedVector.Length() <= oSphere.m_fRadius)// * 2f) //die 2 ist dummerweise hardgecodet. das kommt daher, dass die raumschiffe ungefähr eine boundingsphere von 0.5 - 0.6 haben. gespeichert wird allerdings 0.3, weil die kollision zwischen zwei raumschiffen mit der echten boundsphere zu komisch aussähe!
                    {
                        lCollisionPoints.Add(new MeshCollisionPoint(triangle, vLinkedVector, triangle[j], vLinkedVector.Length()));
                        result.m_CollisionState = CollisionState.Interpenetrating;
                        found = true;
                        break;
                    }
                }
                if (found)
                    continue; //next triangle
                
 

            }

            //check all children
            MeshCollisionResult[] oSubMeshCollisionResult = new MeshCollisionResult[8];
            if (oTree.m_Children == null)
            {
                if (lCollisionPoints.Count == 0)
                    return new MeshCollisionResult(CollisionState.NoCollision, null);
                else
                    return new MeshCollisionResult(CollisionState.Interpenetrating, lCollisionPoints.ToArray());
            }

            for (i = 0; i < 8; i++)
            {
                if (oTree.m_Children[i] == null)
                {
                    oSubMeshCollisionResult[i] = new MeshCollisionResult(CollisionState.NoCollision, null);
                    continue;
                }
                oSubMeshCollisionResult[i] = CheckCollisionSphereMesh(oTree.m_Children[i], oSphere, vSpherePosition);
            }
            //take the worst case
            //order : error < no_coll < interpenetrating
            //result.m_CollisionState = CollisionState.Error;
            List<MeshCollisionResult> oSubResultList= new List<MeshCollisionResult>();
            for(i=0;i<8;i++)
            {
                if(result.m_CollisionState==CollisionState.Error && (oSubMeshCollisionResult[i].m_CollisionState==CollisionState.NoCollision || oSubMeshCollisionResult[i].m_CollisionState==CollisionState.Interpenetrating))
                {
                    result=oSubMeshCollisionResult[i];
                    oSubResultList.Clear();
                    oSubResultList.Add(oSubMeshCollisionResult[i]);
                }
                else if(result.m_CollisionState==CollisionState.NoCollision && oSubMeshCollisionResult[i].m_CollisionState==CollisionState.Interpenetrating)
                {
                    result=oSubMeshCollisionResult[i];
                    oSubResultList.Clear();
                    oSubResultList.Add(oSubMeshCollisionResult[i]);
                }
                else if(result.m_CollisionState==CollisionState.Interpenetrating && oSubMeshCollisionResult[i].m_CollisionState==CollisionState.Interpenetrating)
                {
                    oSubResultList.Add(oSubMeshCollisionResult[i]);
                }
            }
            
            for(i=0;i<oSubResultList.Count;i++)
            {
                iCheckedTriangles += oSubResultList[i].m_iCheckedTriangles;
                if (oSubResultList[i].m_CollisionPoints == null)
                    continue;
                for(j=0;j<oSubResultList[i].m_CollisionPoints.Length;j++)
                {
                    lCollisionPoints.Add(oSubResultList[i].m_CollisionPoints[j]);
                }
            }

            result.m_CollisionPoints=lCollisionPoints.ToArray();
            result.m_iCheckedTriangles = iCheckedTriangles;
            
            return result;
        }

        #endregion

        #region RigidBody <-> Track

        #region CollisionDetection (different radius supported) current version !!

        #region PerpendicularDistancePlanePoint

        private struct PerpendicularDistancePlanePointResult
        {
            public Vector3 vLinkedvector;
            public float fT;
            public float fS;
            public Vector3 vPerpendicularFoot;
        }

        private static PerpendicularDistancePlanePointResult calcPerpendicularDistancePlanePoint(Vector3 vPoint, Vector3 vP1, Vector3 vP2, Vector3 vP3)
        {
            PerpendicularDistancePlanePointResult result = new PerpendicularDistancePlanePointResult();
            Vector3 vA = vP2 - vP1;
            Vector3 vB = vP3 - vP1;
            float fDotAA = Vector3.Dot(vA, vA);
            float fDotBB = Vector3.Dot(vB, vB);
            float fDotAB = Vector3.Dot(vA, vB);
            float fDotP1A = Vector3.Dot(vP1, vA);
            float fDotP1B = Vector3.Dot(vP1, vB);
            float fDotPointA = Vector3.Dot(vPoint, vA);
            float fDotPointB = Vector3.Dot(vPoint, vB);

            float fT, fS;
            Vector3 vLinkedVector;
            Vector3 vPerpendicularFoot;

            fS = ((fDotAB * (-fDotP1A + fDotPointA) / fDotAA - (-fDotP1B + fDotPointB)) / (fDotAB * fDotAB / fDotAA - fDotBB));
            fT = ((-fDotP1A + fDotPointA)/fDotAA) - fS*(fDotAB/fDotAA);

            vPerpendicularFoot = vP1 + fT * vA + fS * vB;
            vLinkedVector = vPerpendicularFoot - vPoint;

            result.fS = fS;
            result.fT = fT;
            result.vPerpendicularFoot = vPerpendicularFoot;
            result.vLinkedvector = vLinkedVector;

            return result;

        }

        #endregion

        #region PerpendicularDistanceLinePoint

        public struct PerpendicularDistanceLinePointResult
        {
            public Vector3 vLinkedVector;
            public float fT;
            public Vector3 vPerpendicularFoot;
        }

        public static PerpendicularDistanceLinePointResult calcPerpendicularDistanceLinePoint(Vector3 vPoint, Vector3 vStart, Vector3 vEnd)
        {
            PerpendicularDistanceLinePointResult result = new PerpendicularDistanceLinePointResult();
            Vector3 vDirection = vEnd - vStart;
            Vector3 vStartPoint = vStart - vPoint;
            Vector3 vPerpendicularFoot;
            Vector3 vNormal;
            float fT = 0f;
            //float fDistance;

            fT = -(vDirection.X * vStartPoint.X
                   + vDirection.Y * vStartPoint.Y
                   + vDirection.Z * vStartPoint.Z)
                 / (vDirection.X * vDirection.X
                   + vDirection.Y * vDirection.Y
                   + vDirection.Z * vDirection.Z);

            if (fT == 0f)
            {
                result.fT = 0f;
                result.vLinkedVector = Vector3.Zero;
                return result;
            }


            vPerpendicularFoot = vStart + fT * vDirection;
            vNormal = vPerpendicularFoot - vPoint;

            result.fT = fT;
            result.vLinkedVector = vNormal;
            result.vPerpendicularFoot = vPerpendicularFoot;

            return result;
        }

        #endregion


        public static TrackCollisionResult CheckCollisionSphereTrack(RigidBody oRigidBody, TrackBody oTrackBody)
        {

            const float cfAccuracy = 0.000f;
            PositionOrientationRadius[] oPositionOrientationradius = oTrackBody.m_PositionOrientationRadius;
            int iCurrentIndex = 0;
            int iNextIndex = 0;
            int iNext2Index = 0;
            int iNext3Index = 0;
            int iPrevIndex = 0;
            int iPrev2Index = 0;
            
            Vector3 vPrevTubeDirection, vCurTubeDirection, vNextTubeDirection;
            float fT, fDistance, fObjRadius;
            int i;
            //int j;

            TrackCollisionResult result = new TrackCollisionResult(CollisionState.NoCollision, Vector3.UnitY, Vector3.Zero, 0f);

            //err checking
            if (float.IsNaN(oRigidBody.DesState.m_vPosition.X) || float.IsNaN(oRigidBody.DesState.m_vPosition.Y) || float.IsNaN(oRigidBody.DesState.m_vPosition.Z))
            {
                result.m_CollisionState = CollisionState.Error;
                return result;
            }

            if (oRigidBody.BoundingSphere is BoundingSphere)
            {
                fObjRadius = ((BoundingSphere)oRigidBody.BoundingSphere).m_fRadius;
            }
            else throw new Exception("BoundingObject not supported");

            #region findObjectInTrack
            //find the RigidBody in the Track and set the iXIndices
            
            //simple and fast find-algo
            const int ciMaxSearch = 10;
            int iSign;
            iCurrentIndex = oRigidBody.m_iCollisionSearch;
            float fMinDistance = float.MaxValue;
            int iMinDistanceIndex = -1;
            int iNumTrackSegements = oTrackBody.m_PositionOrientationRadius.Length;

            // search flipped
            // example: started with index 5: checking order: 5-4-6-3-7-2-8-...
            iSign = -1;
            for (i = 0; i <= ciMaxSearch; )
            {
                int iTempIndex = (iCurrentIndex + i * iSign + iNumTrackSegements) % iNumTrackSegements;
                fDistance = (oTrackBody.m_PositionOrientationRadius[iTempIndex].vPosition - oRigidBody.DesState.m_vPosition).Length();
                if (fDistance < fMinDistance)
                {
                    iMinDistanceIndex = iTempIndex;
                    fMinDistance = fDistance;
                }
                //increment
                if (iSign < 0 || i == 0) //goto next layer if iFlippingOne controled the future and the past
                    i++;
                iSign = -iSign;
            }

            iCurrentIndex = iMinDistanceIndex;
            
            // bis jetzt sind wir maximal einen Tube daneben!
            // dieser fehler wird dann jetzt behoben. es wird in direkter nachbarschaft
            // die parameter der tangenten errechnet und so hoffentlich der fehler korrigiert
            // 
            // till now, there is a max-error of +/- one tube
            // the next code-part will fix this by checking the parmeters of the tangents
            // (should be between 0 and 1). it is possible that the parameters are not in that
            // area because there is a critical area in curves.
            // the algo will take the tube with the min-err in the parmeters

            float fMinT = float.MaxValue;
            int iMinTIndex = 0;

            int iGuessedIndex = iCurrentIndex;

            iSign = -1;
            for (i = 0; i <= 1; )
            {
                iCurrentIndex = (iGuessedIndex + i * iSign + iNumTrackSegements) % iNumTrackSegements;
                iPrevIndex = (iCurrentIndex - 1 + iNumTrackSegements) % iNumTrackSegements;
                iNextIndex = (iCurrentIndex + 1) % iNumTrackSegements;
                PerpendicularDistanceLinePointResult perpendicularResult;
                perpendicularResult = calcPerpendicularDistanceLinePoint(oRigidBody.DesState.m_vPosition, oTrackBody.m_PositionOrientationRadius[iCurrentIndex].vPosition, oTrackBody.m_PositionOrientationRadius[iNextIndex].vPosition);
                //berechne den fehler
                fT = perpendicularResult.fT;
                if (fT < 0f)
                    fT = Math.Abs(fT);
                else if (fT > 1f)
                    fT = fT - 1f;
                else
                    fT = 0f;
                if (fT < fMinT)
                {
                    iMinTIndex = iCurrentIndex;
                    fMinT = fT;
                }
                //increment
                if (iSign < 0 || i == 0)
                    i++;
                iSign = -iSign;
            }

            // set all indices
            iCurrentIndex = iMinTIndex;
            oRigidBody.m_iCollisionSearch = iCurrentIndex;
            iPrev2Index = (iCurrentIndex - 2 + iNumTrackSegements) % iNumTrackSegements;
            iPrevIndex = (iCurrentIndex - 1 + iNumTrackSegements) % iNumTrackSegements;
            iNextIndex = (iCurrentIndex + 1) % iNumTrackSegements;
            iNext2Index = (iCurrentIndex + 2) % iNumTrackSegements;
            iNext3Index = (iCurrentIndex + 3) % iNumTrackSegements;


            #endregion

            vPrevTubeDirection = oTrackBody.m_PositionOrientationRadius[iCurrentIndex].vPosition - oTrackBody.m_PositionOrientationRadius[iPrevIndex].vPosition;
            vCurTubeDirection = oTrackBody.m_PositionOrientationRadius[iNextIndex].vPosition - oTrackBody.m_PositionOrientationRadius[iCurrentIndex].vPosition;
            vNextTubeDirection = oTrackBody.m_PositionOrientationRadius[iNext2Index].vPosition - oTrackBody.m_PositionOrientationRadius[iNextIndex].vPosition;

            PerpendicularDistanceLinePointResult oPerpendicularResult;
            oPerpendicularResult = calcPerpendicularDistanceLinePoint(oRigidBody.DesState.m_vPosition, oTrackBody.m_PositionOrientationRadius[iCurrentIndex].vPosition, oTrackBody.m_PositionOrientationRadius[iNextIndex].vPosition);
            Vector3 vNormal = Vector3.Zero;
            if (oPerpendicularResult.vLinkedVector != Vector3.Zero)
            {
                vNormal = Vector3.Normalize(oPerpendicularResult.vLinkedVector);
            }

            result.m_vNormal = vNormal;
            result.m_fT = oPerpendicularResult.fT;
            fDistance = oPerpendicularResult.vLinkedVector.Length();
            result.m_fDistance = fDistance + fObjRadius;
            float fRadius;
            if (result.m_fT < 0f)
                fRadius = oTrackBody.m_PositionOrientationRadius[iCurrentIndex].fRadius;
            else if (result.m_fT > 1f)
                fRadius = oTrackBody.m_PositionOrientationRadius[iNextIndex].fRadius;
            else
                fRadius = oTrackBody.m_PositionOrientationRadius[iCurrentIndex].fRadius + result.m_fT * (oTrackBody.m_PositionOrientationRadius[iNextIndex].fRadius - oTrackBody.m_PositionOrientationRadius[iCurrentIndex].fRadius);

            result.m_vPerpendicularFoot = oPerpendicularResult.vPerpendicularFoot;

            Matrix matMatrix = Matrix.Identity;
            if (vNormal != Vector3.Zero)
            {
                Vector3 vPerpendicularToNormal = new Vector3(vNormal.Y * vNormal.Z, -2 * vNormal.X * vNormal.Z, vNormal.X * vNormal.Y);
                matMatrix = Matrix.CreateLookAt(Vector3.Zero, vNormal, vPerpendicularToNormal);
            }

            // checking state
            if (vNormal != Vector3.Zero && Vector3.Transform(oRigidBody.DesState.m_vLinearVelocity, matMatrix).Z < 0)
            {
                result.m_CollisionState = CollisionState.NoCollision;
            }
            else if (fDistance + fObjRadius + 0.5f * fObjRadius> fRadius)
            {
                result.m_CollisionState = CollisionState.OutOfTrack;
                result.m_vPoint = oPerpendicularResult.vPerpendicularFoot + fRadius * Vector3.Normalize(oPerpendicularResult.vLinkedVector);
            }
            else if (fDistance + fObjRadius > fRadius)
            {
                result.m_CollisionState = CollisionState.Interpenetrating;
                result.m_vPoint = oPerpendicularResult.vPerpendicularFoot + fRadius * Vector3.Normalize(oPerpendicularResult.vLinkedVector);
            }
           
            else if (fDistance + fObjRadius + cfAccuracy > fRadius)
            {
                result.m_CollisionState = CollisionState.Touching;
                result.m_vPoint = oPerpendicularResult.vPerpendicularFoot + fRadius * Vector3.Normalize(oPerpendicularResult.vLinkedVector);
            }
            if (float.IsNaN(fDistance) || float.IsNaN(fRadius))
            {
                result.m_CollisionState = CollisionState.Error;
            }
            return result;

        }

        #endregion

        
        #region CollisionDtection (new approach ; different radius support) buggy version - not included
        /*
        public static CollisionResult CheckCollision(RigidBody oRigidBody, TrackBody oTrackBody)
        {

            //return CheckCollisionDaniel(oRigidBody, oTrackBody);

            PositionOrientationRadius[] oPositionOrientationradius = oTrackBody.m_PositionOrientationRadius;
            int iCurrentIndex = 0;
            int iNextIndex = 0;
            int iNext2Index = 0;
            int iNext3Index = 0;
            int iPrevIndex = 0;
            int iPrev2Index = 0;
            Vector3 vTubeStart, vTubeEnde, vPoint, vTubeDirection, vPerpendicularFoot;
            Vector3 vNormal, vCollisionPoint, vPrevTubeDirection, vCurTubeDirection, vNextTubeDirection;
            float fT, fDIstance, fObjRadius;
            int i,j;

            CollisionResult result = new CollisionResult(CollisionState.NoCollision, Vector3.UnitY, Vector3.Zero, 0f);

            if (oRigidBody.BoundingSphere is BoundingSphere)
            {
                fObjRadius = ((BoundingSphere)oRigidBody.BoundingSphere).Radius;
            }
            else throw new Exception("BoundingObject not supported");

            #region findObjectInTrack
            //find the RigidBody in the Track and set the iXIndices
            //TODO !!!!!!!!!!!!!!


            //simple and fast find-algo

            const int ciMaxSearch = 10;
            int iFlippingOne;
            iCurrentIndex = oRigidBody.m_iCollisionSearch;
            float fMinDistance=float.MaxValue;
            int iMinDistanceIndex=-1;
            int iNumTrackSegements = oTrackBody.m_PositionOrientationRadius.Length;

            iFlippingOne = -1;
            for(i=0; i<=ciMaxSearch;)
            {
                int iTempIndex = (iCurrentIndex + i*iFlippingOne + iNumTrackSegements) % iNumTrackSegements;
                fDIstance = (oTrackBody.m_PositionOrientationRadius[iTempIndex].vPosition - oRigidBody.DesState.m_vPosition).Length();
                if(fDIstance<fMinDistance)
                {
                    iMinDistanceIndex=iTempIndex;
                    fMinDistance=fDIstance;
                }
                //increment
                if (iFlippingOne < 0 || i==0) //goto next layer if iFlippingOne controled the future and the past
                    i++;
                iFlippingOne = -iFlippingOne;
            }

            //iPrev2Index=(iMinDistanceIndex-2+iNumTrackSegements) % iNumTrackSegements;
            //iPrevIndex=(iMinDistanceIndex-1+iNumTrackSegements) % iNumTrackSegements;
            iCurrentIndex=iMinDistanceIndex;
            //iNextIndex=(iMinDistanceIndex+1) % iNumTrackSegements;
            //iNext2Index=(iMinDistanceIndex+2) % iNumTrackSegements;
            //iNext3Index = (iMinDistanceIndex+3) % iNumTrackSegements;
            

            // bis jetzt sind wir maximal einen Tube daneben!
            // dieser fehler wird dann jetzt behoben. es wird in direkter nachbarschaft
            // die parameter der tangenten errechnet und so hoffentlich der fehler korrigiert

            float fMinT=float.MaxValue;
            int iMinTIndex=0;

            int iGuessedIndex = iCurrentIndex;

            iFlippingOne = -1;
            for(i=0;i<=1;)
            {
                iCurrentIndex = (iGuessedIndex + i * iFlippingOne + iNumTrackSegements) % iNumTrackSegements;
                iPrevIndex = (iCurrentIndex - 1 + iNumTrackSegements) % iNumTrackSegements;
                iNextIndex = (iCurrentIndex + 1) % iNumTrackSegements;
                PerpendicularDistanceLinePointResult perpendicularResult;
                perpendicularResult = calcPerpendicularDistanceLinePoint(oRigidBody.DesState.m_vPosition, oTrackBody.m_PositionOrientationRadius[iCurrentIndex].vPosition, oTrackBody.m_PositionOrientationRadius[iNextIndex].vPosition);
                //berechne den fehler
                fT=perpendicularResult.fT;
                if(fT<0f)
                    fT=Math.Abs(fT);
                else if(fT>1f)
                    fT=fT-1f;
                else
                    fT=0f;
                if(fT<fMinT)
                {
                    iMinTIndex=iCurrentIndex;
                    fMinT=fT;
                }
                //increment
                if (iFlippingOne < 0 || i == 0)
                    i++;
                iFlippingOne = -iFlippingOne;
            }

            iCurrentIndex = iMinTIndex;
            iPrev2Index=(iCurrentIndex-2+iNumTrackSegements) % iNumTrackSegements;
            iPrevIndex=(iCurrentIndex-1+iNumTrackSegements) % iNumTrackSegements;
            iNextIndex=(iCurrentIndex+1) % iNumTrackSegements;
            iNext2Index=(iCurrentIndex+2) % iNumTrackSegements;
            iNext3Index = (iCurrentIndex+3) % iNumTrackSegements;

            oRigidBody.m_iCollisionSearch = iCurrentIndex;

            #endregion

            //Console.WriteLine("curTube: "+iCurrentIndex);

            #region calculate tubeEdges

            const int ciPrevIndex1 = 0;
            const int ciPrevIndex2 = 1;
            const int ciCurIndex1  = 2;
            const int ciCurIndex2  = 3;
            const int ciNextIndex1 = 4;
            const int ciNextIndex2 = 5;
            const int ciNext2Index1 = 6;
            const int ciNext2Index2 = 7;

            vTubeStart=oTrackBody.m_PositionOrientationRadius[iCurrentIndex].vPosition;
            vTubeEnde=oTrackBody.m_PositionOrientationRadius[iNextIndex].vPosition;
            vTubeDirection=vTubeEnde-vTubeStart;

            Vector3[] vaTubeEdges = new Vector3[8];

            //calulating the Normal of TubeTangent <-> RigidBody
            
            PerpendicularDistanceLinePointResult perpendicularMyResult;
            perpendicularMyResult = calcPerpendicularDistanceLinePoint(oRigidBody.DesState.m_vPosition, vTubeStart, vTubeEnde);
            Vector3 vRigidObjToTangent = perpendicularMyResult.vLinkedVector;
            vRigidObjToTangent.Normalize();
            result.m_fT = perpendicularMyResult.fT;
            vPerpendicularFoot = perpendicularMyResult.vPerpendicularFoot;
            //diese berechnung unten ist nicht fähig auf verschiedene radien zu laufen!! //TODO !!
            //result.m_fDistance = perpendicularMyResult.vLinkedVector.Length();
            result.m_fDistance = oTrackBody.m_PositionOrientationRadius[iCurrentIndex].fRadius - perpendicularMyResult.vLinkedVector.Length() + oRigidBody.BoundingSphere.Radius;
            //vNormal = Vector3.Cross(vRigidObjToTangent, vTubeDirection);
            
            //Vector3 vPrevTubeDirection = oTrackBody.m_PositionOrientationRadius[iPrevIndex].matOrientation.Forward;
            //Vector3 vCurTubeDirection = oTrackBody.m_PositionOrientationRadius[iCurrentIndex].matOrientation.Forward;
            //Vector3 vNextTubeDirection = oTrackBody.m_PositionOrientationRadius[iNextIndex].matOrientation.Forward;

            vPrevTubeDirection = oTrackBody.m_PositionOrientationRadius[iCurrentIndex].vPosition - oTrackBody.m_PositionOrientationRadius[iPrevIndex].vPosition;
            vCurTubeDirection = oTrackBody.m_PositionOrientationRadius[iNextIndex].vPosition - oTrackBody.m_PositionOrientationRadius[iCurrentIndex].vPosition;
            vNextTubeDirection = oTrackBody.m_PositionOrientationRadius[iNext2Index].vPosition - oTrackBody.m_PositionOrientationRadius[iNextIndex].vPosition;

            //evtl muss vRigidObjToTangent für jede tube neu berechnet werden
            vNormal = Vector3.Cross(vRigidObjToTangent, vCurTubeDirection);
            vNormal.Normalize();
            Vector3 vOrientation;
            Vector3 vTubeNormal;
            Vector3 vPosition;
            float fRadius;


            vOrientation = oTrackBody.m_PositionOrientationRadius[iCurrentIndex].matOrientation.Forward;
            
            #region calculating the edge-points
            
            //calculating the edge-points Prev
            vPosition = oTrackBody.m_PositionOrientationRadius[iPrevIndex].vPosition;
            //vOrientation = oTrackBody.m_PositionOrientationRadius[iPrevIndex].matOrientation.Forward;
            vTubeNormal = Vector3.Cross(vNormal, vOrientation);
            vTubeNormal.Normalize();
            fRadius = oTrackBody.m_PositionOrientationRadius[iPrevIndex].fRadius;
            vaTubeEdges[ciPrevIndex1] = fRadius * vTubeNormal + vPosition;
            vaTubeEdges[ciPrevIndex2] = -fRadius * vTubeNormal + vPosition;

            //calculating the edge-points Cur
            vPosition = oTrackBody.m_PositionOrientationRadius[iCurrentIndex].vPosition;
            //vOrientation = oTrackBody.m_PositionOrientationRadius[iCurrentIndex].matOrientation.Forward;
            vTubeNormal = Vector3.Cross(vNormal, vOrientation);
            vTubeNormal.Normalize();
            fRadius = oTrackBody.m_PositionOrientationRadius[iCurrentIndex].fRadius;
            vaTubeEdges[ciCurIndex1] = fRadius * vTubeNormal + vPosition;
            vaTubeEdges[ciCurIndex2] = -fRadius * vTubeNormal + vPosition;

            //calculating the edge-points Next
            vPosition = oTrackBody.m_PositionOrientationRadius[iNextIndex].vPosition;
            //vOrientation = oTrackBody.m_PositionOrientationRadius[iNextIndex].matOrientation.Forward;
            vTubeNormal = Vector3.Cross(vNormal, vOrientation);
            vTubeNormal.Normalize();
            fRadius = oTrackBody.m_PositionOrientationRadius[iNextIndex].fRadius;
            vaTubeEdges[ciNextIndex1] = fRadius * vTubeNormal + vPosition;
            vaTubeEdges[ciNextIndex2] = -fRadius * vTubeNormal + vPosition;

            //calculating the edge-points Next2
            vPosition = oTrackBody.m_PositionOrientationRadius[iNext2Index].vPosition;
            //vOrientation = oTrackBody.m_PositionOrientationRadius[iNext2Index].matOrientation.Forward;
            vTubeNormal = Vector3.Cross(vNormal, vOrientation);
            vTubeNormal.Normalize();
            fRadius = oTrackBody.m_PositionOrientationRadius[iNext2Index].fRadius;
            vaTubeEdges[ciNext2Index1] = fRadius * vTubeNormal + vPosition;
            vaTubeEdges[ciNext2Index2] = -fRadius * vTubeNormal + vPosition;



            #endregion


            
            #endregion

            
            #region checkCollision with the help of the edgepoints
            oRigidBody.m_iCollisionSearch = iCurrentIndex;

            Vector3 vDirection, vTemp;

            //return CheckCollisionDaniel(oRigidBody, oTrackBody);
            

            //calculate the 6 lines
            perpendicularMyResult = calcPerpendicularDistanceLinePoint(oRigidBody.DesState.m_vPosition, vaTubeEdges[ciPrevIndex1], vaTubeEdges[ciCurIndex1]);
            vDirection = Vector3.Normalize(vaTubeEdges[ciNextIndex1] - vaTubeEdges[ciCurIndex1]);
            vTemp = Vector3.Normalize(vCurTubeDirection) - vDirection;
            //Console.WriteLine("Dis: " + perpendicularMyResult.vLinkedVector.Length() + "  fT: " + perpendicularMyResult.fT);
            if (perpendicularMyResult.vLinkedVector.Length() < fObjRadius && perpendicularMyResult.fT <= 1f && perpendicularMyResult.fT >= 0f)
            {
                result.m_CollisionState = CollisionState.Interpenetrating;
                result.m_vNormal = -Vector3.Normalize(perpendicularMyResult.vLinkedVector);
                //result.m_vNormal = perpendicularMyResult.vLinkedVector;
                result.m_vPoint = perpendicularMyResult.vPerpendicularFoot;
                return result;
            }

            perpendicularMyResult = calcPerpendicularDistanceLinePoint(oRigidBody.DesState.m_vPosition, vaTubeEdges[ciPrevIndex2], vaTubeEdges[ciCurIndex2]);
            vDirection = Vector3.Normalize(vaTubeEdges[ciNextIndex1] - vaTubeEdges[ciCurIndex1]);
            vTemp = Vector3.Normalize(vCurTubeDirection) - vDirection;
            //Console.WriteLine("Dis: " + perpendicularMyResult.vLinkedVector.Length() + "  fT: " + perpendicularMyResult.fT);
            if (perpendicularMyResult.vLinkedVector.Length() < fObjRadius && perpendicularMyResult.fT <= 1f && perpendicularMyResult.fT >= 0f)
            {
                result.m_CollisionState = CollisionState.Interpenetrating;
                result.m_vNormal = -Vector3.Normalize(perpendicularMyResult.vLinkedVector);
                //result.m_vNormal = perpendicularMyResult.vLinkedVector;
                result.m_vPoint = perpendicularMyResult.vPerpendicularFoot;
                return result;
            }

            perpendicularMyResult = calcPerpendicularDistanceLinePoint(oRigidBody.DesState.m_vPosition, vaTubeEdges[ciCurIndex1], vaTubeEdges[ciNextIndex1]);
            vDirection = Vector3.Normalize(vaTubeEdges[ciNextIndex1] - vaTubeEdges[ciCurIndex1]);
            vTemp = Vector3.Normalize(vCurTubeDirection) - vDirection;
            //Console.WriteLine("Dis: " + perpendicularMyResult.vLinkedVector.Length() + "  fT: "+perpendicularMyResult.fT);
            if (perpendicularMyResult.vLinkedVector.Length() < fObjRadius && perpendicularMyResult.fT <= 1f && perpendicularMyResult.fT >= 0f)
            {
                result.m_CollisionState = CollisionState.Interpenetrating;
                result.m_vNormal = -Vector3.Normalize(perpendicularMyResult.vLinkedVector);
                //result.m_vNormal = perpendicularMyResult.vLinkedVector;
                result.m_vPoint = perpendicularMyResult.vPerpendicularFoot;
                return result;
            }

            perpendicularMyResult = calcPerpendicularDistanceLinePoint(oRigidBody.DesState.m_vPosition, vaTubeEdges[ciCurIndex2], vaTubeEdges[ciNextIndex2]);
            vDirection = Vector3.Normalize(vaTubeEdges[ciNextIndex1] - vaTubeEdges[ciCurIndex1]);
            vTemp = Vector3.Normalize(vCurTubeDirection) - vDirection;
            //Console.WriteLine("Dis: " + perpendicularMyResult.vLinkedVector.Length() + "  fT: " + perpendicularMyResult.fT);
            if (perpendicularMyResult.vLinkedVector.Length() < fObjRadius && perpendicularMyResult.fT <= 1f && perpendicularMyResult.fT >= 0f)
            {
                result.m_CollisionState = CollisionState.Interpenetrating;
                result.m_vNormal = -Vector3.Normalize(perpendicularMyResult.vLinkedVector);
                //result.m_vNormal = perpendicularMyResult.vLinkedVector;
                result.m_vPoint = perpendicularMyResult.vPerpendicularFoot;
                if (Vector3.Dot(vPerpendicularFoot, result.m_vNormal) < 0f)
                {
                    result.m_CollisionState = CollisionState.OutOfTrack;
                    result.m_vNormal = -result.m_vNormal;
                }
                return result;
            }

            perpendicularMyResult = calcPerpendicularDistanceLinePoint(oRigidBody.DesState.m_vPosition, vaTubeEdges[ciNextIndex1], vaTubeEdges[ciNext2Index1]);
            vDirection = Vector3.Normalize(vaTubeEdges[ciNextIndex1] - vaTubeEdges[ciCurIndex1]);
            vTemp = Vector3.Normalize(vCurTubeDirection) - vDirection;
            //Console.WriteLine("Dis: " + perpendicularMyResult.vLinkedVector.Length() + "  fT: " + perpendicularMyResult.fT);
            if (perpendicularMyResult.vLinkedVector.Length() < fObjRadius && perpendicularMyResult.fT <= 1f && perpendicularMyResult.fT >= 0f)
            {
                result.m_CollisionState = CollisionState.Interpenetrating;
                result.m_vNormal = -Vector3.Normalize(perpendicularMyResult.vLinkedVector);
                //result.m_vNormal = perpendicularMyResult.vLinkedVector;
                result.m_vPoint = perpendicularMyResult.vPerpendicularFoot;
                if (Vector3.Dot(vPerpendicularFoot, result.m_vNormal) < 0f)
                {
                    result.m_CollisionState = CollisionState.OutOfTrack;
                    result.m_vNormal = -result.m_vNormal;
                }
                return result;
            }

            perpendicularMyResult = calcPerpendicularDistanceLinePoint(oRigidBody.DesState.m_vPosition, vaTubeEdges[ciNextIndex2], vaTubeEdges[ciNext2Index2]);
            vDirection = Vector3.Normalize(vaTubeEdges[ciNextIndex1] - vaTubeEdges[ciCurIndex1]);
            vTemp = Vector3.Normalize(vCurTubeDirection) - vDirection;
            //Console.WriteLine("Dis: " + perpendicularMyResult.vLinkedVector.Length() + "  fT: " + perpendicularMyResult.fT);
            if (perpendicularMyResult.vLinkedVector.Length() < fObjRadius && perpendicularMyResult.fT <= 1f && perpendicularMyResult.fT >= 0f)
            {
                result.m_CollisionState = CollisionState.Interpenetrating;
                result.m_vNormal = -Vector3.Normalize(perpendicularMyResult.vLinkedVector);
                //result.m_vNormal = perpendicularMyResult.vLinkedVector;
                result.m_vPoint = perpendicularMyResult.vPerpendicularFoot;
                if (Vector3.Dot(vPerpendicularFoot, result.m_vNormal) < 0f)
                {
                    result.m_CollisionState = CollisionState.OutOfTrack;
                    result.m_vNormal = -result.m_vNormal;
                }
                return result;
            }

            return result;
            


            #endregion

       

        }
        */


        #endregion
        
        
        #region CollisionDetection (different radius NOT supported) stable version - not included
        /*
        /// <summary>
        /// Check collision between RigidBody and Track
        /// </summary>
        /// <param name="oRigidBody">RigidBody object</param>
        /// <param name="oTrackBody">TrackBody object</param>
        /// <returns></returns>
        public static CollisionResult CheckCollisionDaniel(RigidBody oRigidBody, TrackBody oTrackBody)
        {
            PositionOrientation[] oPositionOrientation = oTrackBody.m_PositionOrientation;
            float fRadius = oTrackBody.m_fRadius;

            int iCurrentIndex = 0, iNextIndex = 0, iNext2Index = 0, iLastIndex = 0, iCollisionIndex = 0;
            Vector3 vTubeStart, vTubeEnd, vPoint, vTubeDirection, vPointTube, vPerpendicularFoot,
                vNormal, vCollisionPoint, vLastTubeDirection, vNextTubeDirection, vStartRight, vStartUp,
                vEndRight, vEndUp, vStartScale, vEndScale;
            float fT = 0f, fDistance = 0f, fObjRadius = 0f, fMinDistance = float.MaxValue,
                  fStartPhi, fEndPhi, fStartLength, fEndLength;
            bool bFinished = false, bExit = false, bRadiusExceeded1 = false, bRadiusExceeded2 = false;  // finished flag

            if (oRigidBody.BoundingSphere is BoundingSphere)
            {
                fObjRadius = ((BoundingSphere)oRigidBody.BoundingSphere).Radius;
            }
            else throw new Exception("BoundingObject not supported!");

            CollisionResult oCollisionResult = new CollisionResult();
            CollisionResult oBestResult = new CollisionResult();
            oCollisionResult.m_CollisionState = CollisionState.Error;
            oBestResult.m_CollisionState = CollisionState.Error;
            int iCollisionState = 7;    // error

            for (int iIndex = 0; iIndex < oPositionOrientation.Length; iIndex++)
            {
                for (int iNegPos = -1; iNegPos <= 1; iNegPos += 2)  // control zero twice
                {
                    iCurrentIndex = (iNegPos * iIndex + oRigidBody.m_iCollisionSearch + oPositionOrientation.Length * 2) % oPositionOrientation.Length;

                    #region Sphere Collision
                    
                    vNormal = oPositionOrientation[iCurrentIndex].vPosition - oRigidBody.DesState.m_vPosition;
                    fDistance = vNormal.Length() + fObjRadius;
                    oCollisionResult.m_fDistance = fDistance;
                    vNormal.Normalize();
                    vCollisionPoint = oRigidBody.DesState.m_vPosition - vNormal * fObjRadius;

                    if (fDistance < fRadius)
                    {
                        iCollisionState = 0;    // no collision
                        oCollisionResult.m_CollisionState = CollisionState.NoCollision;
                        oCollisionResult.m_vNormal = vNormal;
                        oCollisionResult.m_vPoint = vCollisionPoint;
                        oRigidBody.m_iCollisionSearch = iCurrentIndex;
                        bFinished = true;
                        bExit = true;
                    }
                    else if (fDistance <= fRadius)
                    {
                        if (iCollisionState > 2)
                        {
                            iCollisionState = 2;    // touching (Sphere)
                            oCollisionResult.m_CollisionState = CollisionState.Touching;
                            oCollisionResult.m_vNormal = vNormal;
                            oCollisionResult.m_vPoint = vCollisionPoint;
                            if (bFinished == false)
                            {
                                oRigidBody.m_iCollisionSearch = iCurrentIndex;
                                iCollisionIndex = iCurrentIndex;
                                iIndex = 0;
                                bFinished = true;
                            }
                        }
                    }
                    else if (fDistance <= fRadius + fObjRadius)
                    {
                        if (iCollisionState > 4)
                        {
                            iCollisionState = 4;    // interpenetrating (Sphere)
                            oCollisionResult.m_CollisionState = CollisionState.Interpenetrating;
                            oCollisionResult.m_vNormal = vNormal;
                            oCollisionResult.m_vPoint = vCollisionPoint;
                            if (bFinished == false)
                            {
                                oRigidBody.m_iCollisionSearch = iCurrentIndex;
                                iCollisionIndex = iCurrentIndex;
                                iIndex = 0;
                                bFinished = true;
                            }
                        }
                    }
                    else
                    {
                        if (iCollisionState > 6)
                        {
                            iCollisionState = 6;    // out of track (Sphere)
                            oCollisionResult.m_CollisionState = CollisionState.OutOfTrack;
                            oCollisionResult.m_vNormal = vNormal;
                            oCollisionResult.m_vPoint = vCollisionPoint;
                        }
                    }

                    #endregion

                    if (fDistance <= fMinDistance && // for out of track handling (not tested)
                       (int)oCollisionResult.m_CollisionState <= (int)oBestResult.m_CollisionState)
                    {
                        oBestResult.m_CollisionState = oCollisionResult.m_CollisionState;
                        oBestResult.m_vPoint = oCollisionResult.m_vPoint;
                        oBestResult.m_vNormal = oCollisionResult.m_vNormal;
                        oBestResult.m_fT = oCollisionResult.m_fT;
                        oBestResult.m_fDistance = oCollisionResult.m_fDistance;
                    }

                    #region Cylinder Collision

                    iNextIndex = (iCurrentIndex + 1) % oPositionOrientation.Length;
                    iLastIndex = (iCurrentIndex - 1 + oPositionOrientation.Length) % oPositionOrientation.Length;
                    iNext2Index = (iCurrentIndex + 2) % oPositionOrientation.Length;

                    vTubeStart = oPositionOrientation[iCurrentIndex].vPosition;
                    vTubeEnd = oPositionOrientation[iNextIndex].vPosition;
                    vTubeDirection = vTubeEnd - vTubeStart;
                    vTubeDirection.Normalize();

                    while (vTubeDirection.Length() < 0.001f)  // should not be entered
                    {
                        vTubeEnd = oPositionOrientation[(iNextIndex + 1) % oPositionOrientation.Length].vPosition;
                        vTubeDirection = vTubeEnd - vTubeStart;
                        vTubeDirection.Normalize();
                    }

                    // calculate new length of cylinder depending on orientation
                    vLastTubeDirection = oPositionOrientation[iLastIndex].vPosition - vTubeStart;
                    vLastTubeDirection.Normalize();
                    vNextTubeDirection = oPositionOrientation[iNext2Index].vPosition - vTubeEnd;
                    vNextTubeDirection.Normalize();
                    vStartRight = Vector3.Cross(vLastTubeDirection, vTubeDirection);
                    vEndRight = Vector3.Cross(-vTubeDirection, vNextTubeDirection);
                    vStartUp = Vector3.Cross(vTubeDirection, vStartRight);
                    vEndUp = Vector3.Cross(vEndRight, -vTubeDirection);
                    vStartUp.Normalize();
                    vEndUp.Normalize();
                    vStartScale = (vTubeDirection + vLastTubeDirection);
                    vEndScale = (-vTubeDirection + vNextTubeDirection);
                    vStartScale.Normalize();
                    vEndScale.Normalize();
                    fStartPhi = (float)Math.Acos(Vector3.Dot(vStartUp, vStartScale));
                    fEndPhi = (float)Math.Acos(Vector3.Dot(vEndUp, vEndScale));
                    fStartLength = (float)Math.Abs(Math.Tan(fStartPhi) * fRadius);
                    fEndLength = (float)Math.Abs(Math.Tan(fEndPhi) * fRadius);
                    vTubeStart -= fStartLength * Vector3.Normalize(vTubeDirection);
                    vTubeEnd += fEndLength * Vector3.Normalize(vTubeDirection);

                    vTubeDirection = vTubeEnd - vTubeStart;

                    // calculate perpendicular distance
                    vPoint = oRigidBody.DesState.m_vPosition;

                    vPointTube = vTubeStart - vPoint;

                    fT = -(vTubeDirection.X * vPointTube.X
                           + vTubeDirection.Y * vPointTube.Y
                           + vTubeDirection.Z * vPointTube.Z)
                         / (vTubeDirection.X * vTubeDirection.X
                           + vTubeDirection.Y * vTubeDirection.Y
                           + vTubeDirection.Z * vTubeDirection.Z);

                    if (fT >= 0.0f && fT <= 1.0f)
                    {
                        oCollisionResult.m_fT = fT;
                        vPerpendicularFoot = vTubeStart + fT * vTubeDirection;
                        vNormal = vPerpendicularFoot - oRigidBody.DesState.m_vPosition;
                        fDistance = vNormal.Length() + fObjRadius;
                        oCollisionResult.m_fDistance = fDistance;
                        vNormal.Normalize();
                        vCollisionPoint = oRigidBody.DesState.m_vPosition - vNormal * fObjRadius;

                        if (fDistance < fRadius)
                        {
                            iCollisionState = 0;    // no collision
                            oCollisionResult.m_CollisionState = CollisionState.NoCollision;
                            oCollisionResult.m_vNormal = vNormal;
                            oCollisionResult.m_vPoint = vCollisionPoint;
                            oRigidBody.m_iCollisionSearch = iCurrentIndex;
                            bFinished = true;
                            bExit = true;
                        }
                        else if (fDistance <= fRadius)
                        {
                            if (iCollisionState > 1)
                            {
                                iCollisionState = 1;    // touching (Cylinder)
                                oCollisionResult.m_CollisionState = CollisionState.Touching;
                                oCollisionResult.m_vNormal = vNormal;
                                oCollisionResult.m_vPoint = vCollisionPoint;
                                if (bFinished == false)
                                {
                                    oRigidBody.m_iCollisionSearch = iCurrentIndex;
                                    iCollisionIndex = iCurrentIndex;
                                    iIndex = 0;
                                    bFinished = true;
                                }
                            }
                        }
                        else if (fDistance < fRadius + fObjRadius)
                        {
                            if (iCollisionState > 3)
                            {
                                iCollisionState = 3;    // interpenetrating (Cylinder)
                                oCollisionResult.m_CollisionState = CollisionState.Interpenetrating;
                                oCollisionResult.m_vNormal = vNormal;
                                oCollisionResult.m_vPoint = vCollisionPoint;
                                if (bFinished == false)
                                {
                                    oRigidBody.m_iCollisionSearch = iCurrentIndex;
                                    iCollisionIndex = iCurrentIndex;
                                    iIndex = 0;
                                    bFinished = true;
                                }
                            }
                        }
                        else
                        {
                            if (iCollisionState > 5)
                            {
                                iCollisionState = 5;    // out of track (Cylinder)
                                oCollisionResult.m_CollisionState = CollisionState.OutOfTrack;
                                oCollisionResult.m_vNormal = vNormal;
                                oCollisionResult.m_vPoint = vCollisionPoint;
                            }
                        }
                    }

                    #endregion

                    if (fDistance <= fMinDistance && // for out of track handling (not tested)
                       (int)oCollisionResult.m_CollisionState <= (int)oBestResult.m_CollisionState)
                    {
                        fMinDistance = fDistance;
                        oBestResult.m_CollisionState = oCollisionResult.m_CollisionState;
                        oBestResult.m_vPoint = oCollisionResult.m_vPoint;
                        oBestResult.m_vNormal = oCollisionResult.m_vNormal;
                        oBestResult.m_fT = oCollisionResult.m_fT;
                        oBestResult.m_fDistance = oCollisionResult.m_fDistance;
                    }

                    if (bExit) break;   // collision result found

                    if (bFinished)
                    {
                        // calculate distance
                        if (Vector3.Distance(oPositionOrientation[iCollisionIndex].vPosition,
                                             oPositionOrientation[iCurrentIndex].vPosition)
                            > fRadius * 1.1f)
                        {
                            if (iNegPos < 0) bRadiusExceeded1 = true;
                            else bRadiusExceeded2 = true;
                        }

                        if (bRadiusExceeded1 && bRadiusExceeded2) bExit = true;

                        if (bExit) break;   // collision result found
                    }
                }
                if (bExit) break;
            }

            //if (iCollisionState != 0) Console.WriteLine(iCollisionState);

            return oBestResult;
        }*/
        #endregion
        
        #endregion

        #region initial Search in Track


        public static int findRigidBodyInTrackHardcore(TrackBody oTrackBody, RigidBody oRigidBody)
        {
            Vector3 vLinkedVector;
            int iMinDistanceIndex = 0;
            float fMinDistance;

            //run through the track and search for the min-distance

            vLinkedVector = oTrackBody.m_PositionOrientationRadius[0].vPosition - oRigidBody.CurState.m_vPosition;
            fMinDistance = vLinkedVector.Length();
            for (int i = 1; i < oTrackBody.m_PositionOrientationRadius.Length; i++)
            {
                vLinkedVector = oTrackBody.m_PositionOrientationRadius[i].vPosition - oRigidBody.CurState.m_vPosition;
                if (vLinkedVector.Length() < fMinDistance)
                {
                    fMinDistance = vLinkedVector.Length();
                    iMinDistanceIndex = i;
                }
            }
            return iMinDistanceIndex;
        }


        #endregion
    }
}