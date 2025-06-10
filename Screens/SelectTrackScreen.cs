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
using PraktWS0708.Entities;
using PraktWS0708.Rendering;
using PraktWS0708.ContentPipeline;
using PraktWS0708.Settings;
using PraktWS0708.ShipUI;
using System.IO;
using System.Xml.Serialization;
#endregion

namespace PraktWS0708
{
    class SelectTrackScreen : BackgroundScreen
    {
        #region Fields
        ShipUI.ShipUI infobar;
        private float camRadius = 55.0f;
        private double alpha = 0.0f;
        private float camHeight = 20.0f;
        private int tracknumber = 0;
        private TrackDescription.TrackListEntry trackEntry;
        string path = "Settings/Storage/";
        string tracktype = "Track";
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
                    Settings.Configuration.ModelDescriptions.LoadModels("Settings\\Storage\\ModelList.xml", World.Instance.WorldContent);
                }
                Settings.Configuration.TrackDescription.LoadTrackListEntry("Settings\\Storage\\simpletrack.xml");

                int viewport_x = ScreenManager.GraphicsDevice.Viewport.Width;
                int viewport_y = ScreenManager.GraphicsDevice.Viewport.Height;
                infobar = new PraktWS0708.ShipUI.ShipUI(content, ScreenManager);
                infobar.setSymbolVisibility(false);
                infobar.trackScreen = true;
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
                LevelFactory.LoadSelectShipLevel(LevelFactory.LoadType.NEW);

                RenderManager.Instance.Initialize();


                World.Instance.ViewType = ViewTypes.THIRDPERSON;
                World.Instance.PlayersShip.RenderingPlugin.Hidden = true;

                World.Instance.Camera.Position = new Vector3((float)Math.Cos(alpha) * camRadius, (float)Math.Sin(alpha) * camRadius, camHeight);
                World.Instance.Camera.LookAt = new Vector3(0.0f, 0.0f, 0.0f);
                World.Instance.Camera.Up = new Vector3(0.0f, 0.0f, 1.0f);
                World.Instance.Camera.Update();


                renderTrack();
            }
        }

        private void renderTrack()
        {
            World.Instance.ResetWorld();

            // build the new track
            //buildTrackListEntity();

            if (File.Exists(path + tracktype + tracknumber + ".xml"))
            {
                trackEntry = loadTrack(tracktype + tracknumber + ".xml");
                TrackFactory.buildTrack(trackEntry);
                //World.Instance.gameEnvironment = new GameEnvironment();

                Splines.PositionTangentUpRadius ptu;

                // build ships
                World.Instance.PlayersShip = EntityFactory.BuildEntity("ghostShip");
                ptu = World.Instance.Track.TangentFrames[0];
                World.Instance.PlayersShip.Position = ptu.Position - ptu.Up * ptu.Radius * 0.6f;
                World.Instance.PlayersShip.Orientation = OurMath.MakeCoordSystem(Vector3.Cross(ptu.Tangent, ptu.Up), ptu.Up, ptu.Tangent * -1.0f);
                World.Instance.PlayersShip.PhysicsPlugin.Reset();
                World.Instance.PlayersShip.Update();
                World.Instance.AddModel(World.Instance.PlayersShip);

                //RenderManager.Instance.Initialize();

                infobar.setDescriptionText(trackEntry.trackInfo.realName);
                infobar.setEditorsTime(trackEntry.trackInfo.editorTime);
                infobar.setDannerComment(trackEntry.trackInfo.dannerComment);
                infobar.setDifficultyText(trackEntry.trackInfo.difficulty);
            }          
        }

        public TrackDescription.TrackListEntry loadTrack(string filename)
        {

            TrackDescription.TrackListEntry entry;

            Stream stream = File.OpenRead(path + filename);
            XmlSerializer serializer = new XmlSerializer(typeof(TrackDescription.TrackListEntry));

            entry = (TrackDescription.TrackListEntry)serializer.Deserialize(stream);

            return entry;
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
            }
            else if (InputState.instance.MenuRight)
            {
                tracknumber++;
                if (tracknumber > 10 && tracktype == "customtrack")
                    tracknumber = 10;
                if (File.Exists(path + tracktype + tracknumber + ".xml") && tracktype == "Track")
                {
                    renderTrack();
                }
                else if ((!File.Exists(path + tracktype + tracknumber + ".xml")) && tracktype == "Track")
                {
                    tracktype = "customtrack";
                    tracknumber = 1;
                    renderTrack();
                }
                else if (File.Exists(path + tracktype + tracknumber + ".xml") && tracktype == "customtrack")
                {
                    
                    renderTrack();
                }
            }
            else if (InputState.instance.MenuLeft)
            {
                tracknumber--;
                if (tracknumber < 0 && tracktype == "Track")
                {
                    tracknumber = 0;
                }
                else if (tracknumber <= 0 && tracktype == "customtrack")
                {
                    //MAGIC!!
                    tracknumber = 9;
                    tracktype = "Track";
                }

                if (File.Exists(path + tracktype + tracknumber + ".xml"))
                {
                    renderTrack();
                }
                else
                {
                    tracknumber++;
                    renderTrack();
                }

            }
#if WINDOWS
            else if (InputState.instance.XAxis1 > 0.0f)
#endif
#if XBOX360
            else if (InputState.instance.XAxis2 > 0.0f)
#endif
            {
                alpha -= 0.125d;
                World.Instance.Camera.Position = new Vector3((float)Math.Cos(alpha) * camRadius, (float)Math.Sin(alpha) * camRadius, camHeight);
                World.Instance.Camera.LookAt = new Vector3(0.0f, 0.0f, 0.0f);
                World.Instance.Camera.Up = new Vector3(0.0f, 0.0f, 1.0f);
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
                World.Instance.Camera.Position = new Vector3((float)Math.Cos(alpha) * camRadius, (float)Math.Sin(alpha) * camRadius, camHeight);
                World.Instance.Camera.LookAt = new Vector3(0.0f, 0.0f, 0.0f);
                World.Instance.Camera.Up = new Vector3(0.0f, 0.0f, 1.0f);
                World.Instance.Camera.Update();
            }
#if WINDOWS
            else if (InputState.instance.YAxis1 > 0.0f)
#endif
#if XBOX360
            else if (InputState.instance.YAxis2 > 0.0f)
#endif
            {
                camRadius -= 0.75f;
                if (camRadius <= 0)
                {
                    camRadius = (0.001f);
                }
                World.Instance.Camera.Position = new Vector3((float)Math.Cos(alpha) * camRadius, (float)Math.Sin(alpha) * camRadius, camHeight);
                World.Instance.Camera.LookAt = new Vector3(0.0f, 0.0f, 0.0f);
                World.Instance.Camera.Up = new Vector3(0.0f, 0.0f, 1.0f);
                World.Instance.Camera.Update();
            }
#if WINDOWS
            else if (InputState.instance.YAxis1 < 0.0f)
#endif
#if XBOX360
            else if (InputState.instance.YAxis2 < 0.0f)
#endif
            {
                camRadius += 0.75f;
                World.Instance.Camera.Position = new Vector3((float)Math.Cos(alpha) * camRadius, (float)Math.Sin(alpha) * camRadius, camHeight);
                World.Instance.Camera.LookAt = new Vector3(0.0f, 0.0f, 0.0f);
                World.Instance.Camera.Up = new Vector3(0.0f, 0.0f, 1.0f);
                World.Instance.Camera.Update();
            }
            else if (InputState.instance.LeftTriggerPressed > 0.0f)
            {
                camHeight -= 0.33f;
                World.Instance.Camera.Position = new Vector3((float)Math.Cos(alpha) * camRadius, (float)Math.Sin(alpha) * camRadius, camHeight);
                World.Instance.Camera.LookAt = new Vector3(0.0f, 0.0f, 0.0f);
                World.Instance.Camera.Up = new Vector3(0.0f, 0.0f, 1.0f);
                World.Instance.Camera.Update();
            }
            else if (InputState.instance.RightTriggerPressed > 0.0f)
            {
                camHeight += 0.33f;
                World.Instance.Camera.Position = new Vector3((float)Math.Cos(alpha) * camRadius, (float)Math.Sin(alpha) * camRadius, camHeight);
                World.Instance.Camera.LookAt = new Vector3(0.0f, 0.0f, 0.0f);
                World.Instance.Camera.Up = new Vector3(0.0f, 0.0f, 1.0f);
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
                Configuration.levelName = "race";
                Configuration.trackName= tracktype + tracknumber;
                Sound.PlayCue(Sounds.MenuBack);
                //Sound.Stop(themeCue);
                ScreenManager.AddScreen(new BackgroundScreen());
                ScreenManager.AddScreen(new FlyScreen(FlyScreen.GameStyle.Race));
                //ScreenManager.RemoveScreen(this.parent);
                this.ExitScreen();
            }
        }

        #endregion
    }
}
