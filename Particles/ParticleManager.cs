#region Using Statements
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using PraktWS0708.Entities;
using PraktWS0708.Physics;
using PraktWS0708.Utils;
#endregion

namespace PraktWS0708.Particles
{
    #region ParticleSystemManagementData

    public class ParticleSystemManagementData
    {
        #region Fields

        public BaseEntity ParticleSystem;
        public BaseEntity LinkedEntity;
        public float TimeToLive;
        public Vector3 RelativePosition;
        public Matrix RelativeOrientation;
        public float FloatVar;

        public bool bLinked;
        public bool bTimeToLive;
        public bool bThrust;

        #endregion

        #region Initialization

        public ParticleSystemManagementData(BaseEntity oParticleSystem)
            : this(oParticleSystem, null, -1f, Vector3.Zero, Matrix.Identity, -1f) {}

        public ParticleSystemManagementData(BaseEntity oParticleSystem, float fTimeToLive)
            : this(oParticleSystem, null, fTimeToLive, Vector3.Zero, Matrix.Identity, -1f) { }

        public ParticleSystemManagementData(BaseEntity oParticleSystem, BaseEntity oLinkedEntity, float fTimeToLive, Vector3 vRelativePosition, Matrix matRelativeOrientation)
            : this(oParticleSystem, oLinkedEntity, fTimeToLive, vRelativePosition, matRelativeOrientation, -1f) { }

        public ParticleSystemManagementData(BaseEntity oParticleSystem, BaseEntity oLinkedEntity, float fTimeToLive, Vector3 vRelativePosition, Matrix matRelativeOrientation, float fFloatVar)
        {
            ParticleSystem = oParticleSystem;
            LinkedEntity = oLinkedEntity;
            TimeToLive = fTimeToLive;
            RelativePosition = vRelativePosition;
            RelativeOrientation = matRelativeOrientation;
            FloatVar = fFloatVar;

            bLinked = (LinkedEntity != null);
            bTimeToLive = (TimeToLive > 0f);
            bThrust = ((bLinked) && (LinkedEntity.PhysicsPlugin.Type == PhysicsPlugin.PhysicsPluginType.SteeringBody));
        }

        #endregion
    }

    #endregion

    public class ParticleManager
    {
        #region Fields

        // stores ParticleSystemManagementData
        // (BaseEntity, LinkedEntity, TimeToLive, Relative Position, Relative Orientation, additional float var)
        private SortedList<int, ParticleSystemManagementData> m_ParticleSystemManagementData;

        #endregion

        #region Initialization

        /**
         * ParticleManager - constructor
         */
        public ParticleManager()
        {
            m_ParticleSystemManagementData = new SortedList<int, ParticleSystemManagementData>();
        }

        #endregion

        #region Methods

        /**
         * Add ParticleSystem - BaseEntity to ParticleManager
         */
        public void Add(ParticleSystemManagementData oParticleSystemManagementData)
        {
            #region PerformanceEater
            //#if DEBUG
            PerformanceEater last = PerformanceMeter.Instance.PerfomanceEaterChange(PerformanceEater.Particles);
            //#endif
            #endregion

            World.Instance.AddModel(oParticleSystemManagementData.ParticleSystem);

            m_ParticleSystemManagementData.Add(oParticleSystemManagementData.ParticleSystem.ObjectID,
                                               oParticleSystemManagementData);

            // if ThrustEffect - set parameters
            if (oParticleSystemManagementData.bThrust)
            {
                oParticleSystemManagementData.ParticleSystem.RenderingPlugin.Data.BaseMeshParts[0].FloatParameters.Add("ACCELERATION", 0f);
                oParticleSystemManagementData.ParticleSystem.RenderingPlugin.Data.BaseMeshParts[0].BoolParameters.Add("TURBO", false);
            }

            #region PerformanceEater
            //#if DEBUG
            PerformanceMeter.Instance.PerfomanceEaterChange(last);
            //#endif
            #endregion
        }

        public void Remove(int iObjectID)
        {
            for (int iIndex = 0; iIndex < m_ParticleSystemManagementData.Count; iIndex++)
            {
                if (m_ParticleSystemManagementData.Values[iIndex].LinkedEntity != null)
                {
                    if (iObjectID == m_ParticleSystemManagementData.Values[iIndex].LinkedEntity.ObjectID)
                    {
                        m_ParticleSystemManagementData.RemoveAt(iIndex);
                        iIndex--;
                    }
                }
            }
        }

        /**
         * Reset of ParticleManager
         */
        public void Reset()
        {
            m_ParticleSystemManagementData.Clear();
        }

        /**
         * Updates the ParticleSystems (TTL, removes it if 0)
         */
        public void Update(GameTime oGameTime)
        {
            #region PerformanceEater
//#if DEBUG
            PerformanceEater last = PerformanceMeter.Instance.PerfomanceEaterChange(PerformanceEater.Particles);
//#endif
            #endregion

            // update times
            for (int iIndex = 0; iIndex < m_ParticleSystemManagementData.Count; iIndex++)
            {
                int iKey = m_ParticleSystemManagementData.Keys[iIndex];
                ParticleSystemManagementData oValue = m_ParticleSystemManagementData.Values[iIndex];
                float elapsedTimeMillis = oGameTime.ElapsedGameTime.Milliseconds;

                if (oValue.bTimeToLive) oValue.TimeToLive -= elapsedTimeMillis;

                if (oValue.bLinked)
                {
                    oValue.ParticleSystem.RenderingPlugin.Hidden = oValue.LinkedEntity.RenderingPlugin.Hidden;

                    // no ThrustEffect when dead
                    if (oValue.LinkedEntity.RenderingPlugin.Hidden)
                    {
                        oValue.ParticleSystem.RenderingPlugin.Data.BaseMeshParts[0].FloatParameters["ACCELERATION"] = 0f;
                        oValue.ParticleSystem.RenderingPlugin.Data.BaseMeshParts[0].BoolParameters["TURBO"] = false;

                    }
                    else
                    {
                        if (oValue.bThrust)
                        {
                            // update time for acceleration
                            if (oValue.LinkedEntity.InputPlugin.Acceleration > 0f)
                            {
                                oValue.FloatVar += elapsedTimeMillis / 500f * oValue.LinkedEntity.InputPlugin.Acceleration;
                                if (oValue.FloatVar > 1f) oValue.FloatVar = 1f;
                            }
                            else
                            {
                                oValue.FloatVar -= elapsedTimeMillis / 300f;
                                if (oValue.FloatVar < 0f) oValue.FloatVar = 0f;
                            }

                            oValue.ParticleSystem.RenderingPlugin.Data.BaseMeshParts[0].FloatParameters["ACCELERATION"] = oValue.FloatVar;
                        }
                        if (oValue.bThrust)
                        {
                            oValue.ParticleSystem.RenderingPlugin.Data.BaseMeshParts[0].BoolParameters["TURBO"] = (((RigidBody)oValue.LinkedEntity.PhysicsPlugin).hasTurbo > 0);
                        }
                    }
                    oValue.ParticleSystem.Position = oValue.LinkedEntity.Position + Vector3.Transform(oValue.RelativePosition, oValue.LinkedEntity.Orientation);
                    oValue.ParticleSystem.Orientation = oValue.RelativeOrientation * oValue.LinkedEntity.Orientation;

                    // update Rendering
                    oValue.ParticleSystem.Update();
                }

                // if TimeToLive < 0 - remove ParticleSystem
                if (oValue.bTimeToLive)
                {
                    if (oValue.TimeToLive < 0)
                    {
                        // remove from world
                        World.Instance.RemoveModel(oValue.ParticleSystem);
                        // remove from ParticleManager's lists
                        m_ParticleSystemManagementData.Remove(iKey);
                    }
                }
            }

            #region PerformanceEater
//#if DEBUG
            PerformanceMeter.Instance.PerfomanceEaterChange(last);
//#endif
            #endregion
        }

        /**
         * Gets time an effect should last (Explosion ...)
         */
        public float GetEffectTime(ParticleSystemType oParticleSystemType)
        {
            switch (oParticleSystemType)
            {
                case ParticleSystemType.Explosion:
                    return 1000f;
                default:
                    return -1f;
            }
        }

        /**
         * draw all ParticleSystems
         */
        public void Draw()
        {
            for (int iIndex = 0; iIndex < m_ParticleSystemManagementData.Count; iIndex++)
            {
                m_ParticleSystemManagementData.Values[iIndex].ParticleSystem.RenderingPlugin.Draw();
            }
        }

        #endregion
    }
}
