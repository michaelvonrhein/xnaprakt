using System;
using System.Collections.Generic;
using System.Text;

namespace PraktWS0708.Input
{
    public abstract class InputPlugin : Entities.EntityBehaviourPlugin
    {

        public enum InputPluginType
        {
            NULL,
            PLAYER1,
            PLAYER2,
            AI_CONSERVATIVE,
            AI_WAYPOINT,
            AI_KAMIKAZE,
            AI_ROCKET,
            AI_AGGRESSIVE
        };


        protected InputPluginType m_Type;

        public InputPluginType Type
        {
            get
            {
                return m_Type;
            }
            set
            {
                m_Type = value;
            }
        }

        public InputPlugin(Entities.BaseEntity e) : base(e) { }

        public float Acceleration
        {
            get
            {
                if (!this.Active)
                    return 0.0f;
                return this.AccelerationImpl;
            }
        }

        public float Yaw
        {
            get
            {
                if (!this.Active)
                    return 0.0f;
                return this.YawImpl;
            }
        }

        public bool Shoot
        {
            get
            {
                if (!this.Active)
                    return false;
                return this.ShootImpl;
            }
        }

#if DEBUG
        public float Pitch
        {
            get
            {
                if (!this.Active)
                    return 0.0f;
                return this.PitchImpl;
            }
        }
#endif

        protected virtual float AccelerationImpl
        {
            get { return 0.0f; }
        }

        protected virtual float YawImpl
        {
            get { return 0.0f; }
        }

        protected virtual bool ShootImpl
        {
            get { return false; }
        }

#if DEBUG
        protected virtual float PitchImpl
        {
            get { return 0.0f; }
        }
#endif

        public bool Active = true;

        public static InputPlugin GetPlugin(InputPluginType type, Entities.BaseEntity entity)
        {
            InputPlugin im=null;
            switch (type)
            {
                case InputPluginType.NULL:
                    im = new NullInputPlugin(entity);
                    break;
                case InputPluginType.PLAYER1:
                    im = new HumanPlayer1InputPlugin(entity);
                    break;
                case InputPluginType.PLAYER2:
                    im = new HumanPlayer2InputPlugin(entity);
                    break;
                default: //case InputPluginType.AI: 
                    im = AI.AISystem.Instance.GetInputPlugin(entity, type);
                    break;
            }

            im.m_Type = type;

            return im;
        }
    }
}
