using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace PraktWS0708
{
    class PauseMenuScreen : MenuScreen
    {
        #region Fields

        private FlyScreen parent;
        private bool offerReturnToGame;

        #endregion

        #region Initialization

        /// <summary>
        /// Constructor fills in the menu contents.
        /// </summary>
        public PauseMenuScreen(FlyScreen parent, bool offerReturnToGame)
        {
            this.parent = parent;
            this.offerReturnToGame = offerReturnToGame;

            this.Init();
        }

        /// <summary>
        /// Constructor fills in the menu contents.
        /// </summary>
        public PauseMenuScreen(FlyScreen parent, bool offerReturnToGame, string caption, Color captionColor)
        {
            this.parent = parent;
            this.offerReturnToGame = offerReturnToGame;

            this.Caption = caption;
            this.CaptionColor = captionColor;

            this.Init();
        }

        private void Init()
        {
            if (this.offerReturnToGame)
                this.MenuEntries.Add("Return to Game");
            this.MenuEntries.Add("Restart");
            this.MenuEntries.Add("Watch Replay");
            this.MenuEntries.Add("Options");
            this.MenuEntries.Add("Exit to Main Menu");

            this.SelectedItemColor = Color.CornflowerBlue;
            this.NonSelectedItemColor = Color.White;
        }

        #endregion

        #region Handle Input

        /// <summary>
        /// Responds to user menu selections.
        /// </summary>
        protected override void OnSelectEntry(int entryIndex)
        {
            if (!offerReturnToGame)
                entryIndex++;

            Sound.Play(Sounds.MenuConfirm);

            switch (entryIndex)
            {
                case 0:
                    // return to game
                    this.parent.Resume();
                    this.ExitScreen();

                    break;

                case 1:
                    // restart
                    this.parent.Restart();
                    this.ExitScreen();
                    break;

                case 2:
                    // watch replay
                    this.parent.Replay();
                    this.ExitScreen();
                    break;

                case 3:
                    //ScreenManager.RemoveScreen(this);
                    ScreenManager.AddScreen(new OptionsScreen());
                    break;

                case 4:
                    // go to main menu
                    ScreenManager.AddScreen(new BackgroundScreen());
                    ScreenManager.AddScreen(new MainMenuScreen());
                    ScreenManager.RemoveScreen(this.parent);
                    //TODO: this.parent.UnloadContent etc?
                    this.ExitScreen();
                    break;
            }
        }

        /// <summary>
        /// When the user cancels the main menu, ask if they want to exit the sample.
        /// </summary>
        protected override void OnCancel()
        {
            Sound.Play(Sounds.MenuBack);
            this.parent.Resume();
            this.ExitScreen();
        }

        /// <summary>
        /// Event handler for when the user selects ok on the "are you sure
        /// you want to exit" message box.
        /// </summary>
        void ExitMessageBoxAccepted(object sender, EventArgs e)
        {
            ScreenManager.Game.Exit();
        }

        #endregion
    }
}
