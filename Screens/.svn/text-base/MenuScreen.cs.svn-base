

#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PraktWS0708.Utils;
#endregion

namespace PraktWS0708
{
    /// <summary>
    /// Base class for screens that contain a menu of options. The user can
    /// move up and down to select an entry, or cancel to back out of the screen.
    /// </summary>
    abstract class MenuScreen : GameScreen
    {
        #region Fields

        string caption = "";
        List<string> menuEntries = new List<string>();
        int selectedEntry = 0;

        Color selectedItemColor = Color.CornflowerBlue,
              nonSelectedItemColor = Color.Beige,
              captionColor = Color.Red;

        #endregion

        #region Properties

        /// <summary>
        /// Gets/sets the caption string.
        /// </summary>
        protected string Caption
        {
            get { return caption; }
            set { caption = value; }
        }

        /// <summary>
        /// Gets the list of menu entry strings, so derived classes can add
        /// or change the menu contents.
        /// </summary>
        protected IList<string> MenuEntries
        {
            get { return menuEntries; }
        }

        /// <summary>
        /// Color of currently selected menu entry.
        /// </summary>
        public Color SelectedItemColor
        {
            get { return this.selectedItemColor; }
            set { this.selectedItemColor = value; }
        }

        /// <summary>
        /// Color of currently not selected menu entry.
        /// </summary>
        public Color NonSelectedItemColor
        {
            get { return this.nonSelectedItemColor; }
            set { this.nonSelectedItemColor = value; }
        }

        /// <summary>
        /// Color of caption.
        /// </summary>
        public Color CaptionColor
        {
            get { return this.captionColor; }
            set { this.captionColor = value; }
        }

        #endregion

        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public MenuScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(0.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
        }


        #endregion

        #region Handle Input


        /// <summary>
        /// Responds to user input, changing the selected entry and accepting
        /// or cancelling the menu.
        /// </summary>
        public override void HandleInput()
        {
            // Move to the previous menu entry?
            if (InputState.instance.MenuUp)
            {
                selectedEntry--;

                if (selectedEntry < 0)
                    selectedEntry = menuEntries.Count - 1;

                Sound.Play(Sounds.MenuNext);
            }

            // Move to the next menu entry?
            if (InputState.instance.MenuDown)
            {
                selectedEntry++;

                if (selectedEntry >= menuEntries.Count)
                    selectedEntry = 0;

                Sound.Play(Sounds.MenuNext);
            }

            // Accept or cancel the menu?
            if (InputState.instance.MenuSelect)
            {
                Sound.Play(Sounds.MenuConfirm);
                OnSelectEntry(selectedEntry);
            }
            else if (InputState.instance.MenuCancel)
            {
                Sound.Play(Sounds.MenuBack);
                OnCancel();
            }
        }


        /// <summary>
        /// Notifies derived classes that a menu entry has been chosen.
        /// </summary>
        protected abstract void OnSelectEntry(int entryIndex);


        /// <summary>
        /// Notifies derived classes that the menu has been cancelled.
        /// </summary>
        protected abstract void OnCancel();


        #endregion

        #region Draw


        /// <summary>
        /// Draws the menu.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
//#if DEBUG
            //PerformanceEater last = PerformanceMeter.Instance.PerfomanceEaterChange(PerformanceEater.Rendering);
//#endif

            Vector2 position = new Vector2(110, 70);

            // Make the menu slide into place during transitions, using a
            // power curve to make things look more interesting (this makes
            // the movement slow down as it nears the end).
            float transitionOffset = (float)Math.Pow(TransitionPosition, 2);

            if (ScreenState == ScreenState.TransitionOn)
                position.X -= transitionOffset * 256;
            else
                position.X += transitionOffset * 512;

            // Draw each menu entry in turn.
            ScreenManager.SpriteBatch.Begin();

            if (caption.Length > 0)
            {
                // Modify the alpha to fade text out during transitions.
                Color color = new Color(captionColor.R, captionColor.G, captionColor.B, TransitionAlpha);

                // Draw text, centered on the middle of each line.
                Vector2 origin = new Vector2(0, ScreenManager.Font.LineSpacing / 2);

                ScreenManager.SpriteBatch.DrawString(ScreenManager.Font, caption,
                                                     position, color, 0, origin, 1.0f,
                                                     SpriteEffects.None, 0);

                position.Y += ScreenManager.Font.LineSpacing;
            }

            for (int i = 0; i < menuEntries.Count; i++)
            {
                Color color;
                float scale;

                if (IsActive && (i == selectedEntry))
                {
                    // The selected entry has an animating size.
                    double time = gameTime.TotalGameTime.TotalSeconds;

                    float pulsate = (float)Math.Sin(time * 6) + 1;
                    
                    color = this.selectedItemColor;
                    scale = 1 + pulsate * 0.05f;
                }
                else
                {
                    color = this.nonSelectedItemColor;
                    scale = 1;
                }

                // Modify the alpha to fade text out during transitions.
                color = new Color(color.R, color.G, color.B, TransitionAlpha);

                // Draw text, centered on the middle of each line.
                Vector2 origin = new Vector2(0, ScreenManager.Font.LineSpacing / 2);

                ScreenManager.SpriteBatch.DrawString(ScreenManager.Font, menuEntries[i],
                                                     position, color, 0, origin, scale,
                                                     SpriteEffects.None, 0);

                position.Y += ScreenManager.Font.LineSpacing;
            }

            ScreenManager.SpriteBatch.End();

//#if DEBUG
            //PerformanceMeter.Instance.PerfomanceEaterChange(last);
//#endif
        }

        #endregion
    }
}
