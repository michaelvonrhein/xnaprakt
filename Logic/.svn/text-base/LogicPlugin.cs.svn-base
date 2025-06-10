using System;
using System.Collections.Generic;
using System.Text;
using PraktWS0708.Entities;
using PraktWS0708.Physics;
using Microsoft.Xna.Framework;

namespace PraktWS0708.Logic
{
    /// <summary>
    /// The LogicPlugin is the super class for all logical behavior classes
    /// </summary>
    public abstract class LogicPlugin : EntityBehaviourPlugin
    {
        #region Enums, Structs, Classes

        #region LogicPluginType

        public enum LogicPluginType
        {
            WALL,
            NULL,
            BOMB,
            SHIP,
            TURBO,
            HEALTHPACK,
            ROCKET,
            BANANE,
            SHIELD,
            ROCKETPICKUP,
            OBSTACLE
        };

        #endregion

        #region LogicPluginData

        public struct LogicPluginData
        {
            #region Fields

            public float m_fMaxHealth;
            public float m_fDamageResistence;
            public int m_iNumThruster;
            public Vector3[] m_vaThrustRelativePositions;
            public Matrix[] m_maThrustOrientations;
            public Vector3[] m_vaThrustScales;

            public string name;

            #endregion

            #region Initialization

            public LogicPluginData(float fMaxHealth, float fDamageResistence, int iNumThruster, Vector3[] vaThrustRelativePositions, Matrix[] maThrustOrientations, Vector3[] vaThrustScales)
            {
                m_fMaxHealth = fMaxHealth;
                m_fDamageResistence = fDamageResistence;
                m_iNumThruster = iNumThruster;
                m_vaThrustRelativePositions = vaThrustRelativePositions;
                m_maThrustOrientations = maThrustOrientations;
                m_vaThrustScales = vaThrustScales;
                name = "";
            }
            

            #endregion
        }

        #endregion

        #endregion

        #region Fields

        private LogicPluginType type;
        public string name;
        
        #endregion

        #region Properties

        public virtual float Health
        {
            get
            {
                return 0;
            }
        }

        public virtual float MaxHealth
        {
            get
            {
                return 0f;
            }
        }

        public virtual bool IsColliding
        {
            get
            {
                return false;
            }
            set
            {
                //Console.WriteLine("pure logic");
            }
        }

        public TimeSpan LastCollisionTime;

        #endregion

        #region Methods

        public abstract bool isResponding();

        

        #endregion

        #region Initialization

        public LogicPlugin(LogicPluginData oLogicPluginData, Entities.BaseEntity e) : base(e) { }

        public static LogicPlugin GetPlugin(LogicPluginType type, LogicPluginData oLogicPluginData, Entities.BaseEntity entity)
        {
            //Factory pattern
            LogicPlugin lm=null;
            switch (type)
            {
                case LogicPluginType.SHIP:
                    lm = new Logic.Ship(oLogicPluginData, entity);
                    break;
        
                case LogicPluginType.WALL:
                    lm = new Logic.Wall(oLogicPluginData, entity);
                    break;
                case LogicPluginType.BOMB:
                    lm = new Logic.Bomb(25f, oLogicPluginData, entity);
                    break;
                case LogicPluginType.ROCKET:
                    lm = new Logic.Rocket(25f, oLogicPluginData, entity);
                    break;
                case LogicPluginType.TURBO:
                    lm = new Logic.PowerUp(PowerUpTypes.TURBO, oLogicPluginData, entity);
                    break;
                case LogicPluginType.BANANE:
                    lm = new Logic.PowerUp(PowerUpTypes.BANANA, oLogicPluginData, entity);
                    break;
                case LogicPluginType.SHIELD:
                    lm = new Logic.PowerUp(PowerUpTypes.SHIELD, oLogicPluginData, entity);
                    break;
                case LogicPluginType.HEALTHPACK:
                    lm = new Logic.PowerUp(PowerUpTypes.HEALTHPACK, oLogicPluginData, entity);
                    break;
                /*case LogicPluginType.ROCKETPICKUP:
                    lm = new Logic.PowerUp(PowerUpTypes.ROCKETPICKUP, oLogicPluginData, entity);
                    break;*/
                case LogicPluginType.OBSTACLE:
                    lm = new Logic.Obstacle(oLogicPluginData, entity);
                    break;
                default:
                    lm = new Logic.NullPlugin(oLogicPluginData, entity);
                    break;
            }
            lm.type = type;

            lm.name = oLogicPluginData.name;

            return lm;
        }

        #endregion
    }
}
