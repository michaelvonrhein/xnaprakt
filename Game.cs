#region File Description
//-----------------------------------------------------------------------------
// Game.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PraktWS0708.Utils;
using PraktWS0708.Rendering;
using System;
using PraktWS0708.Entities;
using Microsoft.Xna.Framework.Content;
using PraktWS0708.Settings;

#endregion

namespace PraktWS0708
{
    public class GameStateManagementGame : Microsoft.Xna.Framework.Game
    {
        #region Fields

        GraphicsDeviceManager graphics;
        ScreenManager screenManager;

        #endregion

        #region Initialization

        /// <summary>
        /// The main game constructor.
        /// </summary>
        public GameStateManagementGame()
        {

            //just contains a couple of tests for new funktions
            Utils.TestEnv.StartTest();

            graphics = new GraphicsDeviceManager(this);

            RenderManager.Instance.PersistentContent = new ContentManager(Services);

            // Create the screen manager component.
            screenManager = new ScreenManager(this);
            
            //Settings.Configuration.screenManger = screenManager;


            Components.Add(screenManager);

            

//#if DEBUG
            PerformanceMeter.Instance = new PerformanceMeter(screenManager, Keys.F10);
            Components.Add(PerformanceMeter.Instance);
//#endif

            // Activate the first screens.
            screenManager.AddScreen(new BackgroundScreen());
            screenManager.AddScreen(new MainMenuScreen());

            

            Settings.Configuration.SaveSettings();
            Settings.Configuration.LoadSettings();

            Sound.Initialize();

            // Unleash frame rates ;-)
            IsFixedTimeStep = true;
            graphics.SynchronizeWithVerticalRetrace = true;
            graphics.IsFullScreen = Settings.Configuration.EngineSettings.Fullscreen;
            graphics.PreferredBackBufferWidth = Settings.Configuration.EngineSettings.ScreenResolution.Width;
            graphics.PreferredBackBufferHeight = Settings.Configuration.EngineSettings.ScreenResolution.Height;
            graphics.PreferMultiSampling = Settings.Configuration.EngineSettings.Multisampling;
            Window.AllowUserResizing = Settings.Configuration.EngineSettings.AllowUserResizing;
            if (Window.AllowUserResizing)
            {
                Window.ClientSizeChanged += new EventHandler(ResizeWindow);
            }

            World.Instance.WorldContent = new ContentManager(screenManager.Game.Services);

        }

        public void ResizeWindow(object sender, EventArgs e)
        {
            RenderManager.Instance.Resize(screenManager.GraphicsDevice.PresentationParameters.BackBufferWidth, screenManager.GraphicsDevice.PresentationParameters.BackBufferHeight);     
        }

        #endregion

        #region Update

        protected override void Update(GameTime gameTime) {
//#if DEBUG
            PerformanceEater last = PerformanceMeter.Instance.PerfomanceEaterChange(PerformanceEater.Framework);
//#endif
            
            base.Update(gameTime);
            
            //updating the sound engine must be called every frame !
            Sound.Update();
            
//#if DEBUG
            PerformanceMeter.Instance.PerfomanceEaterChange(last);
//#endif
        }

        #endregion

        #region Draw


        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        protected override void Draw(GameTime gameTime)
        {
//#if DEBUG
            //PerformanceEater last = PerformanceMeter.Instance.PerfomanceEaterChange(PerformanceEater.Rendering);
//#endif
            
            graphics.GraphicsDevice.Clear(Color.Black);

            // The real drawing happens inside the screen manager component.
            base.Draw(gameTime);
            
//#if DEBUG
            //PerformanceMeter.Instance.PerfomanceEaterChange(last);
//#endif
        }


        #endregion
    }


    #region Entry Point

    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    static class Program
    {
        static void Main()
        {
            using (GameStateManagementGame game = new GameStateManagementGame())
            {
                game.Run();
            }
        }
    }

    #endregion
}
