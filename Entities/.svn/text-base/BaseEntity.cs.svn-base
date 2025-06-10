using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PraktWS0708.Utils;
using PraktWS0708.Rendering;
using PraktWS0708.Logic;
using PraktWS0708.Physics;
using PraktWS0708.Input;

namespace PraktWS0708.Entities
{
    public class BaseEntity : IComparable
    {
        static private int ObjectCount = 0;

        public int ObjectID;

        public RenderingPlugin RenderingPlugin;
        public LogicPlugin LogicPlugin;
        public InputPlugin InputPlugin;
        public PhysicsPlugin PhysicsPlugin;

        private Vector3 m_vPosition = Vector3.Zero;
        public Matrix Orientation = Matrix.Identity;
        public Matrix LastWorldMatrix = Matrix.Identity;
        public float Scale = 1f;

        private int currentSegment;

        public Vector3 Position
        {
            get
            {
                return m_vPosition;
            }
            set
            {
                m_vPosition = value;
            }
        }

        public BaseEntity()
        {
            this.ObjectID = ObjectCount++;
            CurrentSegment = 0;
        }

        public Matrix WorldMatrix
        {
            get
            {
                return Matrix.CreateScale(Scale) * Orientation * Matrix.CreateTranslation(Position);
            }
        }

        public int CurrentSegment
        {
            get { return currentSegment; }
            set
            {
                //remove object from current segments object list
                World.Instance.Track.SceneGraph.RemoveObject(this, currentSegment);
                //add object to value segments object list
                World.Instance.Track.SceneGraph.AddObject(this, value);
                //set currentSegment to value
                currentSegment = value;
            }
        }

        public void Update()
        {
            int segment = World.Instance.Track.SceneGraph.EstimateSegmentForPosition(Position);
            if (segment != currentSegment)
            {
                CurrentSegment = segment;
            }
        }

        /// <summary>
        /// should not be used directly. 
        /// use World.RemoveModel instead
        /// </summary>
        public virtual void Destroy()
        {
            //World.Instance.RemoveModel(this);
            OnDestroyed(new EntityEventArgs(this));
        }

        public int CompareTo(object obj)
        {
            return (((BaseEntity)obj).RenderingPlugin.DistanceSQ.CompareTo(RenderingPlugin.DistanceSQ));
        }

        #region Public Events

        public event EventHandler<EntityEventArgs> Destroyed;

        protected virtual void OnDestroyed(EntityEventArgs e)
        {
            EventHandler<EntityEventArgs> handler = Destroyed;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        #endregion
    }
}
