#region File Description
//-----------------------------------------------------------------------------
// MainMenuScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using PraktWS0708.Settings;
#endregion

namespace PraktWS0708
{
    /// <summary>
    /// The main menu screen is the first thing displayed when the game starts up.
    /// </summary>
    class MainMenuScreen : MenuScreen
    {
        #region Initialization


        /// <summary>
        /// Constructor fills in the menu contents.
        /// </summary>
        public MainMenuScreen()
        {
#if DEBUG
            MenuEntries.Add("Start Default Level");
            MenuEntries.Add("Start TrackOnly Level");
            MenuEntries.Add("Start Track Lean and Mean");
            MenuEntries.Add("Start Race");
            MenuEntries.Add("Start AI Demo");
            MenuEntries.Add("Start TrackEditor");
            MenuEntries.Add("Start ParticleSystem");
            MenuEntries.Add("Start Select Ship Screen");
            MenuEntries.Add("Start Select Track Screen");
            MenuEntries.Add("Options");
            MenuEntries.Add("Exit Game");
#else
            MenuEntries.Add("Start Game");
            MenuEntries.Add("Start AI Demo");
            MenuEntries.Add("Track Editor");
            MenuEntries.Add("Options");
            MenuEntries.Add("Exit Game");
#endif
        }


        #endregion

        #region Handle Input

        /// <summary>
        /// Responds to user menu selections.
        /// </summary>
        protected override void OnSelectEntry(int entryIndex)
        {
            switch (entryIndex)
            {
#if DEBUG
                case 0:
                    //Start Game
                    Configuration.levelName = "default";
                    Rendering.RenderManager.Instance.RenderWaypoints = false;
                    LoadingScreen.Load(ScreenManager, LoadFlyScreen, true);
                    break;

                case 1:
                    //Start Game
                    Configuration.levelName = "trackOnly";
                    Rendering.RenderManager.Instance.RenderWaypoints = false;
                    LoadingScreen.Load(ScreenManager, LoadFlyScreen, true);
                    break;

                case 2:
                    //Start Game
                    Configuration.levelName = "trackLeanAndMean";
                    Rendering.RenderManager.Instance.RenderWaypoints = false;
                    LoadingScreen.Load(ScreenManager, LoadFlyScreen, true);
                    break;

                case 3:
                    //Start Game
                    Configuration.levelName = "race";
                    Rendering.RenderManager.Instance.RenderWaypoints = false;
                    LoadingScreen.Load(ScreenManager, LoadFlyScreenRace, true);
                    break;

                case 4:
                    // Start AI Demo
                    Configuration.levelName = "aidemo";
                    Rendering.RenderManager.Instance.RenderWaypoints = true;
                    LoadingScreen.Load(ScreenManager, LoadFlyScreenRace, true);
                    break;
                
                case 5:
                    //Start TrackEditor
                    Configuration.levelName = "trackeditor";
                    Rendering.RenderManager.Instance.RenderWaypoints = false;
                    LoadingScreen.Load(ScreenManager, LoadTrackEditor, true);
                    break;

                case 6:
                    //Start ParticleSystem - Test
                    Configuration.levelName = "ParticleSystem";
                    Rendering.RenderManager.Instance.RenderWaypoints = false;
                    LoadingScreen.Load(ScreenManager, LoadParticleSystem, true);
                    break;

                case 7:
                    Configuration.levelName = "trackeditor";
                    Rendering.RenderManager.Instance.RenderWaypoints = false;
                    LoadingScreen.Load(ScreenManager, LoadSelectShipScreen, true);
                    break;

                case 8:
                    Configuration.levelName = "trackeditor";
                    Rendering.RenderManager.Instance.RenderWaypoints = false;
                    LoadingScreen.Load(ScreenManager, LoadSelectTrackScreen, true);
                    break;

                case 9:
                    //ScreenManager.RemoveScreen(this);
                    ScreenManager.AddScreen(new OptionsScreen());
                    break;
 
                case 10:
                    // Exit the game
                    ScreenManager.Game.Exit();
                    break;
#else
                case 0:
                    // Start Game
                    Configuration.levelName = "trackeditor";
                    Rendering.RenderManager.Instance.RenderWaypoints = false;
                    LoadingScreen.Load(ScreenManager, LoadSelectShipScreen, true);
                    break;

                case 1:
                    // Start AI Demo
                    Configuration.levelName = "aidemo";
                    Rendering.RenderManager.Instance.RenderWaypoints = true;
                    LoadingScreen.Load(ScreenManager, LoadFlyScreenRace, true);
                    break;

                case 2:
                    //Start TrackEditor
                    Configuration.levelName = "trackeditor";
                    Rendering.RenderManager.Instance.RenderWaypoints = false;
                    LoadingScreen.Load(ScreenManager, LoadTrackEditor, true);
                    break;

                case 3:
                    // Show Options
                    //ScreenManager.RemoveScreen(this);
                    ScreenManager.AddScreen(new OptionsScreen());
                    break;
 
                case 4:
                    // Exit the game
                    ScreenManager.Game.Exit();
                    break;
#endif
            }
        }

        /// <summary>
        /// When the user cancels the main menu, ask if they want to exit the sample.
        /// </summary>
        protected override void OnCancel()
        {
            const string message = "Are you sure you want to exit the game?";

            MessageBoxScreen messageBox = new MessageBoxScreen(message);

            messageBox.Accepted += ExitMessageBoxAccepted;

            ScreenManager.AddScreen(messageBox);
        }

        /// <summary>
        /// Event handler for when the user selects ok on the "are you sure
        /// you want to exit" message box.
        /// </summary>
        void ExitMessageBoxAccepted(object sender, EventArgs e)
        {
            ScreenManager.Game.Exit();
        }

        /// <summary>
        /// Loading screen callback for activating the ship selection.
        /// </summary>
       

        void LoadFlyScreen(object sender, EventArgs e)
        {
            ScreenManager.AddScreen(new FlyScreen());
        }

        /// <summary>
        /// Callback.
        /// </summary>
        void LoadFlyScreenRace(object sender, EventArgs e)
        {
            ScreenManager.AddScreen(new FlyScreen(FlyScreen.GameStyle.Race));
        }

        void LoadTrackEditor(object sender, EventArgs e)
        {
            ScreenManager.AddScreen(new TrackEditor());
        }

        void LoadSelectShipScreen(object sender, EventArgs e)
        {
            ScreenManager.AddScreen(new ShowCaseScreen());
        }

        void LoadSelectTrackScreen(object sender, EventArgs e)
        {
            ScreenManager.AddScreen(new SelectTrackScreen());
        }

        void LoadParticleSystem(object sender, EventArgs e)
        {
            ScreenManager.AddScreen(new ParticleSystemScreen());
        }

        #endregion
    }
}
