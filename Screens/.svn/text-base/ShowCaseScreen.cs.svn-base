#region File Description
//-----------------------------------------------------------------------------
// ShowCaseScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using PraktWS0708.Utils;
using System.Collections.Generic;
//using PraktWS0708.Utils.OurMath;
using PraktWS0708.Entities;
using PraktWS0708.Rendering;
using PraktWS0708.ContentPipeline;
using PraktWS0708.Settings;
using PraktWS0708.ShipUI;
#endregion

namespace PraktWS0708
{
    class ShowCaseScreen : BackgroundScreen
    {
        #region Fields
        ShipUI.ShipUI infobar;
        private int modelCount = 10;
        private int actualModel = 1;
        private float maxhealth;
        private float velocity;
        private float mass;
        private float steerability;
        private float camRadius = 1.25f;
        private double alpha = 0.0f;
        private float camHeight = 0.5f;
        #endregion

        #region Initialization

        /// <summary>
        /// Load graphics content for the game.
        /// </summary>
        public override void LoadGraphicsContent(bool loadAllContent)
        {
            RenderManager.Instance.GraphicsDevice = ScreenManager.GraphicsDevice;
            RenderManager.Instance.SpriteBatch = ScreenManager.SpriteBatch;

            if (loadAllContent)
            {
                if (content == null) content = new ContentManager(ScreenManager.Game.Services);

                backgroundTexture = content.Load<Texture2D>("Content/Textures/black");

                if (!Settings.Configuration.ModelDescriptions.isLoaded)
                {
#if DEBUG
                    DateTime startTime = DateTime.Now;
#endif                    
                    Settings.Configuration.ModelDescriptions.LoadModels("Settings\\Storage\\ModelList.xml", World.Instance.WorldContent);
#if DEBUG
                    DateTime stopTime = DateTime.Now;
                                       
                    TimeSpan duration = stopTime - startTime;
                    Console.WriteLine("Models loaded in:");
                    Console.WriteLine("hours:" + duration.Hours);
                    Console.WriteLine("minutes:" + duration.Minutes);
                    Console.WriteLine("seconds:" + duration.Seconds);
                    Console.WriteLine("milliseconds:" + duration.Milliseconds);
#endif
                }
                Settings.Configuration.TrackDescription.LoadTrackListEntry("Settings\\Storage\\showshiptrack.xml");
                
                infobar = new PraktWS0708.ShipUI.ShipUI(content, ScreenManager);
                infobar.trackScreen = false;

                int viewport_x = ScreenManager.GraphicsDevice.Viewport.Width;
                int viewport_y = ScreenManager.GraphicsDevice.Viewport.Height;

                //make sure the world is empty
                World.Instance.ResetWorld();

                //create a new window to the world
                World.Instance.Camera = new Camera();
                World.Instance.Camera.Aspect = ScreenManager.GraphicsDevice.PresentationParameters.BackBufferWidth / ScreenManager.GraphicsDevice.PresentationParameters.BackBufferHeight;


                //initialize a new content manager for the world
                World.Instance.WorldContent = new ContentManager(ScreenManager.Game.Services);
                RenderManager.Instance.PersistentContent = new ContentManager(ScreenManager.Game.Services);

                World.Instance.ResetWorld();
                
                // build the new track
                loadTrack(PraktWS0708.Entities.LevelFactory.LoadType.NEW);
                loadModelByNumber(1);
                
                RenderManager.Instance.Initialize();

                World.Instance.ViewType = ViewTypes.THIRDPERSON;
                World.Instance.PlayersShip.RenderingPlugin.Hidden = false;

                World.Instance.Camera.Position = new Vector3((float)Math.Cos(alpha) * camRadius
                    + World.Instance.PlayersShip.Position.X,
                    World.Instance.PlayersShip.Position.Y + 0.50f,
                    (float)Math.Sin(alpha) * camRadius + World.Instance.PlayersShip.Position.Z);
                World.Instance.Camera.LookAt = World.Instance.PlayersShip.Position;
                World.Instance.Camera.Up = new Vector3(0.0f, 1.0f, 0.0f);
                World.Instance.Camera.Update();

                

            }
        }

        private void loadTrack(PraktWS0708.Entities.LevelFactory.LoadType type)
        {
            World.Instance.ResetWorld();

            // build the new track
            if (type == PraktWS0708.Entities.LevelFactory.LoadType.NEW)
            {
                World.Instance.buildWorld("showshiptrack");

                //TrackFactory.buildTrack("showshiptrack");
                //World.Instance.gameEnvironment = new GameEnvironment();
            }

        }

        private void loadModelByNumber(int number)
        {
            World.Instance.ResetWorld();

            Splines.PositionTangentUpRadius ptu;

            // build ships
            World.Instance.PlayersShip = EntityFactory.BuildEntity("AIShip" + number);
            ptu = World.Instance.Track.TangentFrames[0];
            World.Instance.PlayersShip.Position = ptu.Position - ptu.Up * ptu.Radius * 0.6f;
            //World.Instance.PlayersShip.Orientation = OurMath.MakeCoordSystem(Vector3.Cross(ptu.Tangent, ptu.Up), ptu.Up, ptu.Tangent * -1.0f);
            World.Instance.PlayersShip.Orientation = Matrix.Identity;
            World.Instance.PlayersShip.PhysicsPlugin.Reset();
            World.Instance.PlayersShip.Update();
            World.Instance.AddModel(World.Instance.PlayersShip);
            ModelDescription md;
            Configuration.ModelDescriptions.ModelDescriptionForName("AIShip" + number, out md);
            maxhealth = md.LogicPluginData.m_fDamageResistence;
            velocity = md.PhysicsPluginData.m_fThrustFactor;
            mass = md.PhysicsPluginData.m_fMass;
            steerability = md.PhysicsPluginData.m_fSteeringFactor;
            calculateShipScore(maxhealth, velocity, mass, steerability);
            
            infobar.setDescriptionText(md.modelName);
        }

        private void calculateShipScore(float health, float velocity, float mass, float steering)
        {
            float UpperHealth = 0.001f;
            float UpperVelocity = 0.01f;
            float UpperMass = 5000f;
            float UpperSteering = 0.01f;

            infobar.setSpeedDisplay(Convert.ToInt32(velocity / UpperVelocity * 5.0f));
            infobar.setCrownDisplay(Convert.ToInt32(steering / UpperSteering * 5.0f));
            infobar.setHeartsDisplay(Convert.ToInt32(health / UpperHealth * 5.0f));
            infobar.setMassDisplay(Convert.ToInt32(mass / UpperMass * 5.0f));

            infobar.setStarDisplay((Convert.ToInt32(steering / UpperSteering * 5.0f) +
                Convert.ToInt32(velocity / UpperVelocity * 5.0f) +
                Convert.ToInt32(health / UpperHealth * 5.0f) +
                Convert.ToInt32(mass / UpperMass * 5.0f)) / 4);
        }


        /// <summary>
        /// Unload graphics content used by the game.
        /// </summary>
        public override void UnloadGraphicsContent(bool unloadAllContent)
        {
            if (unloadAllContent)
            {
                content.Unload();
            }
        }

        #endregion


        /// <summary>
        /// Updates the state of the game. This method checks the GameScreen.IsActive
        /// property, so the game will stop updating when the pause menu is active,
        /// or if you tab away to a different application.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
            World.Instance.Sunlight.Direction = World.Instance.Camera.Position - World.Instance.Camera.LookAt + new Vector3(0f, 1f, 0f);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            RenderManager.Instance.DrawTrackEditor(gameTime);
            infobar.draw(gameTime);
        }


        #region Handle Input
        void ExitMessageBoxAccepted(object sender, EventArgs e)
        {
            Sound.PlayCue(Sounds.MenuBack);
            //Sound.Stop(themeCue);
            ScreenManager.AddScreen(new BackgroundScreen());
            ScreenManager.AddScreen(new MainMenuScreen());
        }
        /// <summary>
        /// Responds to user input, changing the selected ship
        /// or cancelling the menu.
        /// </summary>
        public override void HandleInput()
        {
            if (InputState.instance.PauseGame)
            {
                const string message = "Are you sure you want to exit the program?";

                MessageBoxScreen messageBox = new MessageBoxScreen(message);

                messageBox.Accepted += ExitMessageBoxAccepted;

                ScreenManager.AddScreen(messageBox);
            }else if (InputState.instance.MenuRight)
            {
                actualModel++;
                if (actualModel > modelCount)
                    actualModel = 1;
                loadModelByNumber(actualModel);
            }
            else if (InputState.instance.MenuLeft)
            {
                actualModel--;
                if (actualModel < 1)
                    actualModel = modelCount;
                loadModelByNumber(actualModel);
            }
#if WINDOWS
            else if (InputState.instance.XAxis1 > 0.0f)
#endif
#if XBOX360
            else if (InputState.instance.XAxis2 > 0.0f)
#endif
            {
                alpha -= 0.125d;
                World.Instance.Camera.Position = new Vector3((float)Math.Cos(alpha) * camRadius
                    + World.Instance.PlayersShip.Position.X,
                    World.Instance.PlayersShip.Position.Y + camHeight,
                    (float)Math.Sin(alpha) * camRadius + World.Instance.PlayersShip.Position.Z);
                World.Instance.Camera.LookAt = World.Instance.PlayersShip.Position;
                World.Instance.Camera.Up = new Vector3(0.0f, 1.0f, 0.0f);
                World.Instance.Camera.Update();
            }
#if WINDOWS
            else if (InputState.instance.XAxis1 < 0.0f)
#endif
#if XBOX360
            else if (InputState.instance.XAxis2 < 0.0f)
#endif
            {
                alpha += 0.125d;
                World.Instance.Camera.Position = new Vector3((float)Math.Cos(alpha) * camRadius
                    + World.Instance.PlayersShip.Position.X,
                    World.Instance.PlayersShip.Position.Y + camHeight,
                    (float)Math.Sin(alpha) * camRadius + World.Instance.PlayersShip.Position.Z);
                World.Instance.Camera.LookAt = World.Instance.PlayersShip.Position;
                World.Instance.Camera.Up = new Vector3(0.0f, 1.0f, 0.0f);
                World.Instance.Camera.Update();
                
            }
#if WINDOWS
            else if (InputState.instance.YAxis1 < 0.0f)
#endif
#if XBOX360
            else if (InputState.instance.YAxis2 < 0.0f)
#endif
            {
                camRadius += 0.025f;
                World.Instance.Camera.Position = new Vector3((float)Math.Cos(alpha) * camRadius
                    + World.Instance.PlayersShip.Position.X,
                    World.Instance.PlayersShip.Position.Y + camHeight,
                    (float)Math.Sin(alpha) * camRadius + World.Instance.PlayersShip.Position.Z);
                World.Instance.Camera.LookAt = World.Instance.PlayersShip.Position;
                World.Instance.Camera.Up = new Vector3(0.0f, 1.0f, 0.0f);
                World.Instance.Camera.Update();

            }
#if WINDOWS
            else if (InputState.instance.YAxis1 > 0.0f)
#endif
#if XBOX360
            else if (InputState.instance.YAxis2 > 0.0f)
#endif
            {
                camRadius -= 0.025f;
                World.Instance.Camera.Position = new Vector3((float)Math.Cos(alpha) * camRadius
                    + World.Instance.PlayersShip.Position.X,
                    World.Instance.PlayersShip.Position.Y + camHeight,
                    (float)Math.Sin(alpha) * camRadius + World.Instance.PlayersShip.Position.Z);
                World.Instance.Camera.LookAt = World.Instance.PlayersShip.Position;
                World.Instance.Camera.Up = new Vector3(0.0f, 1.0f, 0.0f);
                World.Instance.Camera.Update();

            }
            else if (InputState.instance.LeftTriggerPressed > 0.0f)
            {
                camHeight -= 0.025f;
                World.Instance.Camera.Position = new Vector3((float)Math.Cos(alpha) * camRadius
                    + World.Instance.PlayersShip.Position.X,
                    World.Instance.PlayersShip.Position.Y + camHeight,
                    (float)Math.Sin(alpha) * camRadius + World.Instance.PlayersShip.Position.Z);
                World.Instance.Camera.LookAt = World.Instance.PlayersShip.Position;
                World.Instance.Camera.Up = new Vector3(0.0f, 1.0f, 0.0f);
                World.Instance.Camera.Update();
            }
            else if (InputState.instance.RightTriggerPressed > 0.0f)
            {
                camHeight += 0.025f;
                World.Instance.Camera.Position = new Vector3((float)Math.Cos(alpha) * camRadius
                     + World.Instance.PlayersShip.Position.X,
                     World.Instance.PlayersShip.Position.Y + camHeight,
                     (float)Math.Sin(alpha) * camRadius + World.Instance.PlayersShip.Position.Z);
                World.Instance.Camera.LookAt = World.Instance.PlayersShip.Position;
                World.Instance.Camera.Up = new Vector3(0.0f, 1.0f, 0.0f);
                World.Instance.Camera.Update();
            }
            
            else if (InputState.instance.MenuCancel)
            {
                Sound.PlayCue(Sounds.MenuBack);
                //Sound.Stop(themeCue);
                ScreenManager.AddScreen(new BackgroundScreen());
                ScreenManager.AddScreen(new MainMenuScreen());
            }
            else if (InputState.instance.MenuSelect)
            {
                Configuration.playerShipName = "AIShip" + actualModel;
                Sound.PlayCue(Sounds.MenuBack);
                //Sound.Stop(themeCue);
                ScreenManager.AddScreen(new BackgroundScreen());
                ScreenManager.AddScreen(new SelectTrackScreen());
                //ScreenManager.RemoveScreen(this.parent);
                this.ExitScreen();
            }
        }
        private Vector3 MatrixVectorMult(Matrix mat, Vector3 vec)
        {
            Vector3 ret = new Vector3();
            ret.X = mat.M11 * vec.X + mat.M12 * vec.Y + mat.M13 * vec.Z;
            ret.Y = mat.M21 * vec.X + mat.M22 * vec.Y + mat.M23 * vec.Z;
            ret.Z = mat.M31 * vec.X + mat.M32 * vec.Y + mat.M33 * vec.Z;

            return ret;
        }

        private Vector3 VectorMatrixMult(Vector3 vec, Matrix mat)
        {
            Vector3 ret = new Vector3();
            ret.X = mat.M11 * vec.X + mat.M21 * vec.Y + mat.M31 * vec.Z;
            ret.Y = mat.M12 * vec.X + mat.M22 * vec.Y + mat.M32 * vec.Z;
            ret.Z = mat.M13 * vec.X + mat.M23 * vec.Y + mat.M33 * vec.Z;

            return ret;
        }

        #endregion
    }
}
