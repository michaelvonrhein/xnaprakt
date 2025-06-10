using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace PraktWS0708.Input
{
    public class HumanPlayer2InputPlugin : InputPlugin
    {

        public HumanPlayer2InputPlugin(Entities.BaseEntity e) : base(e) { }

        protected override float AccelerationImpl
        {
            get
            {
#if WINDOWS
                return (Keyboard.GetState().IsKeyDown(Keys.W)) ? 1.0f : 0.0f - ((Keyboard.GetState().IsKeyDown(Keys.S)) ? -1.0f : 0.0f);
#elif XBOX
                return GamePad.GetState(PlayerIndex.Two).Triggers.Right - GamePad.GetState(PlayerIndex.Two).Triggers.Left;
#else
                return false;
#endif
            }
        }

        protected override float YawImpl
        {
            get
            {
#if WINDOWS
                return (Keyboard.GetState().IsKeyDown(Keys.D)) ? 1.0f : 0.0f - ((Keyboard.GetState().IsKeyDown(Keys.A)) ? -1.0f : 0.0f);
#elif XBOX
                return GamePad.GetState(PlayerIndex.Two).ThumbSticks.Left.X;
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
                return (GamePad.GetState(PlayerIndex.Two).Buttons.LeftShoulder == ButtonState.Pressed);
#else
                return 0.0f;
#endif
            }
        }
    }
}
