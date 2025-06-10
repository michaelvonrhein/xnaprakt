using System;
using System.Collections.Generic;
using System.Text;
using PraktWS0708.Physics;

namespace PraktWS0708.Logic
{
    /// <summary>
    /// The turbo powerupeffect increases speed and steering
    /// </summary>
    public class Turbo : Logic.Powereffect
    {
        private float factorThrust;
        private float factorSteering;

        public Turbo(float factorSteering, float factorThrust, Physics.RigidBody rigidBody, int durationInMillies)
            : base(rigidBody, durationInMillies)
        {
            type = PowerUpTypes.TURBO;
            this.factorSteering = factorSteering;
            this.factorThrust = factorThrust;
        }

        public override void startEffect()
        {
            if (rigidBody is SteeringBody)
            {
                SteeringBody sb = ((SteeringBody)rigidBody);
                Console.WriteLine(sb.ThrustFactor);
                sb.SteeringFactor *= factorSteering;
                sb.ThrustFactor *= factorThrust;
                Console.WriteLine(sb.ThrustFactor);
            }
        }

        public override void endEffect()
        {
            if (rigidBody is SteeringBody)
            {
                SteeringBody sb = ((SteeringBody)rigidBody);
                sb.SteeringFactor /= factorSteering;
                sb.ThrustFactor /= factorThrust;
            }
        }
    }
}
