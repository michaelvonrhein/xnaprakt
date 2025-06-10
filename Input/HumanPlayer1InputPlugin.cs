using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace PraktWS0708.Input
{
    public class HumanPlayer1InputPlugin : InputPlugin
    {

        public HumanPlayer1InputPlugin(Entities.BaseEntity e) : base(e) { }

        protected override float AccelerationImpl
        {
            get
            {
#if WINDOWS
                return (Keyboard.GetState().IsKeyDown(Keys.Up)) ? 1.0f : 0.0f + ((Keyboard.GetState().IsKeyDown(Keys.Down)) ? -1.0f : 0.0f);
#elif XBOX
                return GamePad.GetState(PlayerIndex.One).Triggers.Right - GamePad.GetState(PlayerIndex.One).Triggers.Left;
#else
                return 0.0f;
#endif
            }
        }

        protected override float YawImpl
        {
            get
            {
#if WINDOWS
                return (Keyboard.GetState().IsKeyDown(Keys.Right)) ? 1.0f : 0.0f + ((Keyboard.GetState().IsKeyDown(Keys.Left)) ? -1.0f : 0.0f);
#elif XBOX
                return GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.X;
#else
                return 0.0f;
#endif
            }
        }

        protected override bool ShootImpl
        {
            get
            {
#if WINDOWS
                return (Keyboard.GetState().IsKeyDown(Keys.RightControl));
#elif XBOX
                return (GamePad.GetState(PlayerIndex.One).Buttons.LeftShoulder == ButtonState.Pressed);
#else
                return false;
#endif
            }
        }

#if DEBUG
        protected override float PitchImpl
        {
            get
            {
#if WINDOWS
                return (Keyboard.GetState().IsKeyDown(Keys.W)) ? 1.0f : 0.0f + ((Keyboard.GetState().IsKeyDown(Keys.S)) ? -1.0f : 0.0f);
#elif XBOX
                return GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.Y;
#else
                return 0.0f;
#endif
            }
        }
#endif
    }
}
