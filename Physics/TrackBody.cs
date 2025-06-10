#region Using Statements
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
#endregion

namespace PraktWS0708.Physics
{
    #region PositionOrientationRadius

    public struct PositionOrientationRadius
    {
        public Vector3 vPosition;
        public Matrix matOrientation;
        public Quaternion qOrientation;
        public float fRadius;
    }

    #endregion

    public class TrackBody
    {
        public PositionOrientationRadius[] m_PositionOrientationRadius;
        
        public TrackBody()
        {
            //laut den grafikern sollte hier nur -2 stehen. is wahrscheinlich bei denen ein bug (Thomas hentschel) 
            int iSegmentCount = Entities.World.Instance.Track.TangentFrames.Length - 3;

            m_PositionOrientationRadius = new PositionOrientationRadius[iSegmentCount];
            for (int iIndex = 0; iIndex < iSegmentCount; iIndex++)
            {
                Vector3 vPosition = Entities.World.Instance.Track.TangentFrames[iIndex+1].Position;
                Vector3 vForward = Entities.World.Instance.Track.TangentFrames[iIndex+1].Tangent;
                Vector3 vUp = Entities.World.Instance.Track.TangentFrames[iIndex+1].Up;

                m_PositionOrientationRadius[iIndex].vPosition = vPosition;
                m_PositionOrientationRadius[iIndex].matOrientation = Matrix.Transpose(Matrix.CreateLookAt(
                    Vector3.Zero, vForward, vUp));
                m_PositionOrientationRadius[iIndex].qOrientation = Quaternion.CreateFromRotationMatrix(
                    m_PositionOrientationRadius[iIndex].matOrientation);
                m_PositionOrientationRadius[iIndex].fRadius = Entities.World.Instance.Track.TangentFrames[iIndex+1].Radius;
            }


        }
    }
}
