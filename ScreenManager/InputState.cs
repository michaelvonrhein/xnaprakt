#region File Description
//-----------------------------------------------------------------------------
// InputState.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using PraktWS0708.Utils;
#endregion

namespace PraktWS0708
{
    /// <summary>
    /// Helper for reading input from keyboard and gamepad. This class tracks both
    /// the current and previous state of both input devices, and implements query
    /// properties for high level input actions such as "move up through the menu"
    /// or "pause the game".
    /// </summary>
    public class InputState
    {
        #region Fields

        public static InputState instance;
        public static Dictionary<string,keyState_dlg> keyStateMappings = new Dictionary<string, InputState.keyState_dlg>();

        protected KeyboardState CurrentKeyboardState;
        protected GamePadState CurrentGamePad1State;
        protected GamePadState CurrentGamePad2State;

        protected KeyboardState LastKeyboardState;
        protected GamePadState LastGamePad1State;
        protected GamePadState LastGamePad2State;

        public delegate float keyState_dlg();
        #endregion

        #region Properties

        static InputState()
        {
            instance = new InputState(); 
            keyStateMappings.Add("GamePad Thumb Up", new keyState_dlg(instance.GP1_Thumb_Up));
            keyStateMappings.Add("GamePad Thumb Down", new keyState_dlg(instance.GP1_Thumb_Down));
            keyStateMappings.Add("Keyboard W", new keyState_dlg(instance.KB_W));
            keyStateMappings.Add("Keyboard A", new keyState_dlg(instance.KB_A));
            keyStateMappings.Add("Keyboard S", new keyState_dlg(instance.KB_S));
            keyStateMappings.Add("Keyboard D", new keyState_dlg(instance.KB_D));
            keyStateMappings.Add("Keyboard Up", new keyState_dlg(instance.KB_Up));
            keyStateMappings.Add("Keyboard Down", new keyState_dlg(instance.KB_Down));
        }


        /// <summary>
        /// Checks for a "menu up" input action (on either keyboard or gamepad).
        /// </summary>
        public bool MenuUp
        {
            get
            {
                return IsNewKeyPress(Keys.Up) ||
                       (CurrentGamePad1State.DPad.Up == ButtonState.Pressed &&
                        LastGamePad1State.DPad.Up == ButtonState.Released) ||
                       (CurrentGamePad1State.ThumbSticks.Left.Y > 0 &&
                        LastGamePad1State.ThumbSticks.Left.Y <= 0);
            }
        }


        /// <summary>
        /// Checks for a "menu down" input action (on either keyboard or gamepad).
        /// </summary>
        public bool MenuDown
        {
            get
            {
                return IsNewKeyPress(Keys.Down) ||
                       (CurrentGamePad1State.DPad.Down == ButtonState.Pressed &&
                        LastGamePad1State.DPad.Down == ButtonState.Released) ||
                       (CurrentGamePad1State.ThumbSticks.Left.Y < 0 &&
                        LastGamePad1State.ThumbSticks.Left.Y >= 0);
            }
        }


        /// <summary>
        /// Checks for a "menu select" input action (on either keyboard or gamepad).
        /// </summary>
        public bool MenuSelect
        {
            get
            {
                return IsNewKeyPress(Keys.Space) ||
                       IsNewKeyPress(Keys.Enter) ||
                       (CurrentGamePad1State.Buttons.A == ButtonState.Pressed &&
                        LastGamePad1State.Buttons.A == ButtonState.Released) ||
                       (CurrentGamePad1State.Buttons.Start == ButtonState.Pressed &&
                        LastGamePad1State.Buttons.Start == ButtonState.Released);
            }
        }


        /// <summary>
        /// Checks for a "menu cancel" input action (on either keyboard or gamepad).
        /// </summary>
        public bool MenuCancel
        {
            get
            {
                return IsNewKeyPress(Keys.Escape) ||
                       (CurrentGamePad1State.Buttons.B == ButtonState.Pressed &&
                        LastGamePad1State.Buttons.B == ButtonState.Released) ||
                       (CurrentGamePad1State.Buttons.Back == ButtonState.Pressed &&
                        LastGamePad1State.Buttons.Back == ButtonState.Released);
            }
        }


        /// <summary>
        /// Checks for a "pause the game" input action (on either keyboard or gamepad).
        /// </summary>
        public bool PauseGame
        {
            get
            {
                return IsNewKeyPress(Keys.Escape) ||
                       (CurrentGamePad1State.Buttons.Back == ButtonState.Pressed &&
                        LastGamePad1State.Buttons.Back == ButtonState.Released) ||
                       (CurrentGamePad1State.Buttons.Start == ButtonState.Pressed &&
                        LastGamePad1State.Buttons.Start == ButtonState.Released);
            }
        }


        /// <summary>
        /// Checks for a "menu right" input action (on either keyboard or gamepad).
        /// </summary>
        public bool MenuLeft
        {
            get
            {
                return IsNewKeyPress(Keys.Left) ||
                       (CurrentGamePad1State.DPad.Left == ButtonState.Pressed &&
                        LastGamePad1State.DPad.Left == ButtonState.Released) ||
                       (CurrentGamePad1State.ThumbSticks.Left.X < 0 &&
                        LastGamePad1State.ThumbSticks.Left.X >= 0);
            }
        }

        /// <summary>
        /// Checks for a "menu right" input action (on either keyboard or gamepad).
        /// </summary>
        public bool MenuRight
        {
            get
            {
                return IsNewKeyPress(Keys.Right) ||
                       (CurrentGamePad1State.DPad.Right == ButtonState.Pressed &&
                        LastGamePad1State.DPad.Right == ButtonState.Released) ||
                       (CurrentGamePad1State.ThumbSticks.Left.X > 0 &&
                        LastGamePad1State.ThumbSticks.Left.X <= 0);
            }
        }

        /// <summary>
        /// Checks for an X-button input action on the Gamepad or keyboard.
        /// </summary>
        public bool GamePadX
        {
            get
            {
                return IsNewKeyPress(Keys.X) ||
                       (CurrentGamePad1State.Buttons.X == ButtonState.Pressed &&
                        LastGamePad1State.Buttons.X == ButtonState.Released);
            }
        }

        /// <summary>
        /// Checks for an Y-Button input action on the Gamepad or keyboard.
        /// </summary>
        public bool GamePadY
        {
            get
            {
                return IsNewKeyPress(Keys.Y) ||
                       (CurrentGamePad1State.Buttons.Y == ButtonState.Pressed &&
                        LastGamePad1State.Buttons.Y == ButtonState.Released);
            }
        }

        public float XAxis1
        {
            get
            {
                float result = CurrentGamePad1State.ThumbSticks.Left.X;
                if (CurrentKeyboardState.IsKeyDown(Keys.A))
                    result -= 1.0f;
                if (CurrentKeyboardState.IsKeyDown(Keys.D))
                    result += 1.0f;
                return result;
            }
        }

        public float YAxis1
        {
            get
            {
                float result = CurrentGamePad1State.ThumbSticks.Left.Y;
                if (CurrentKeyboardState.IsKeyDown(Keys.W))
                    result += 1.0f;
                if (CurrentKeyboardState.IsKeyDown(Keys.S))
                    result -= 1.0f;
                return result;
            }
        }

        public float XAxis2
        {
            get
            {
                float result = CurrentGamePad1State.ThumbSticks.Right.X;
                if (CurrentKeyboardState.IsKeyDown(Keys.Left))
                    result -= 1.0f;
                if (CurrentKeyboardState.IsKeyDown(Keys.Right))
                    result += 1.0f;
                return result;
            }
        }

        public float YAxis2
        {
            get
            {
                float result = CurrentGamePad1State.ThumbSticks.Right.Y;
                if (CurrentKeyboardState.IsKeyDown(Keys.Up))
                    result += 1.0f;
                if (CurrentKeyboardState.IsKeyDown(Keys.Down))
                    result -= 1.0f;
                return result;
            }
        }

        public float Acceleration
        {
            get
            {
                float result = CurrentGamePad1State.Triggers.Right - CurrentGamePad1State.Triggers.Left;
                if (CurrentKeyboardState.IsKeyDown(Keys.Space))
                    result += 1.0f;
                if (CurrentKeyboardState.IsKeyDown(Keys.LeftAlt))
                    result -= 1.0f;
                return result;
            }
        }

        public float RightTriggerPressed
        {
            get
            {
                float result = CurrentGamePad1State.Triggers.Right;
                if(CurrentKeyboardState.IsKeyDown(Keys.E))
                    result = 1.0f;
                return result;
            }
        }
        public float LeftTriggerPressed
        {
            get
            {
                float result = CurrentGamePad1State.Triggers.Left;
                if (CurrentKeyboardState.IsKeyDown(Keys.Q))
                    result = 1.0f;
                return result;
            }
        }

        public bool LeftShoulderDown
        {
            get
            {
                return CurrentGamePad1State.Buttons.LeftShoulder == ButtonState.Pressed ||
                       CurrentKeyboardState.IsKeyDown(Keys.F1);
            }
        }

        public bool LeftShoulderPressed
        {
            get
            {
                return (CurrentGamePad1State.Buttons.LeftShoulder == ButtonState.Pressed &&
                        LastGamePad1State.Buttons.LeftShoulder == ButtonState.Released) ||
                        IsNewKeyPress(Keys.F1);
            }
        }

        public bool RightShoulderDown
        {
            get
            {
                return CurrentGamePad1State.Buttons.RightShoulder == ButtonState.Pressed ||
                       CurrentKeyboardState.IsKeyDown(Keys.F2);
            }
        }

        public bool RightShoulderPressed
        {
            get
            {
                return (CurrentGamePad1State.Buttons.RightShoulder == ButtonState.Pressed &&
                        LastGamePad1State.Buttons.RightShoulder == ButtonState.Released) ||
                        IsNewKeyPress(Keys.F2);
            }
        }

        //the following funktions describe a key
        //whereas a key can be an actual keyboard key, a joystick axis, gamepad button or what ever.

        public float GP1_Thumb_Up()
        {
            return OurMath.MAX2(0,CurrentGamePad1State.ThumbSticks.Left.Y);
        }

        public float GP1_Thumb_Down()
        {
            return OurMath.MIN2(0, CurrentGamePad1State.ThumbSticks.Left.Y);
        }

        public float KB_W()
        {
            if (CurrentKeyboardState.IsKeyDown(Keys.W)) return 1.0f;
            else return 0.0f;
        }

        public float KB_A()
        {
            if (CurrentKeyboardState.IsKeyDown(Keys.A)) return 1.0f;
            else return 0.0f;
        }
        public float KB_S()
        {
            if (CurrentKeyboardState.IsKeyDown(Keys.S)) return 1.0f;
            else return 0.0f;
        }
        public float KB_D()
        {
            if (CurrentKeyboardState.IsKeyDown(Keys.D)) return 1.0f;
            else return 0.0f;
        }
        public float KB_Up()
        {
            
            if (CurrentKeyboardState.IsKeyDown(Keys.Up)) return 1.0f;
            else return 0.0f;
        }
        public float KB_Down()
        {
            if (CurrentKeyboardState.IsKeyDown(Keys.Down)) return 1.0f;
            else return 0.0f;
        }

        #endregion

        #region Methods


        /// <summary>
        /// Reads the latest state of the keyboard and gamepad.
        /// </summary>
        public void Update()
        {
            LastKeyboardState = CurrentKeyboardState;
            LastGamePad1State = CurrentGamePad1State;
            LastGamePad2State=CurrentGamePad2State;
            CurrentKeyboardState = Keyboard.GetState();
            CurrentGamePad1State = GamePad.GetState(PlayerIndex.One);
            CurrentGamePad2State= GamePad.GetState(PlayerIndex.Two);
        }


        /// <summary>
        /// Helper for checking if a key was newly pressed during this update.
        /// </summary>
        protected bool IsNewKeyPress(Keys key)
        {
            return (CurrentKeyboardState.IsKeyDown(key) &&
                    LastKeyboardState.IsKeyUp(key));
        }


        #endregion
    }
}
