#region File Description
//-----------------------------------------------------------------------------
// GameplayScreen.cs
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
using PraktWS0708.Entities;
using PraktWS0708.Rendering;
using PraktWS0708.ContentPipeline;
using PraktWS0708.Settings;
using PraktWS0708.Physics;
using PraktWS0708.Logic;
#endregion

namespace PraktWS0708
{
    sealed class FlyScreen : BackgroundScreen
    {
        #region Fields

        public enum GameStyle
        {
            Free,
            //TODO TimeAttack,
            Race
        }

        private GameStyle gameStyle = GameStyle.Free;
        private int numLaps = 3;
        private bool raceFinished = false;
        private bool playerDead = false;
        private bool replaying = false;
        
        private bool waitGo = false;
        private bool waitGoPause = false;
        private bool hasGameTime = false;
        private Int32 startGameTime;
        private int soundStep = 1;
        private Int32 delay = 3000;

        // pause stuff
        private bool paused = false;
        private bool inputWasActive = true;

        // replay stuff
        private MovementRecorder[] movementRecorders = new MovementRecorder[0];

        private ChaseCamera ChasingCamera;

        // timing stuff
        private struct LapTimings
        {
            public LapTimings(int curLap)
            {
                this.curLap = curLap;
                this.lapStart = TimeSpan.Zero;
                this.lapTimes = new List<TimeSpan>();
                this.accumulatedPauseTime = TimeSpan.Zero;
            }
            public int curLap;
            public TimeSpan lapStart;
            public List<TimeSpan> lapTimes;
            public TimeSpan accumulatedPauseTime;
        }
        private LapTimings[] lapTimings; // first one is the player's, rest is enemies'
        private int playerPosition;

        private Cue themeCue;
        private Sounds theme = Sounds.GameMusic;

        HUD.HUD cockpit;


        #endregion

        #region Constructors

        public FlyScreen()
        {
            ChasingCamera = new ChaseCamera();
            ChasingCamera.DesiredPositionOffset = new Vector3(0.0f, 0.5f, 1.0f);
        }

        public FlyScreen(GameStyle gameStyle)
        {
            this.gameStyle = gameStyle;

            ChasingCamera = new ChaseCamera();
            ChasingCamera.DesiredPositionOffset = new Vector3(0.0f, 0.5f, 1.0f);
        }

        #endregion

        #region Initialization

        /// <summary>
        /// Load graphics content for the game.
        /// </summary>
        public override void LoadGraphicsContent(bool loadAllContent)
        {
            RenderManager.Instance.GraphicsDevice = ScreenManager.GraphicsDevice;
            RenderManager.Instance.SpriteBatch = ScreenManager.SpriteBatch;
#if DEBUG
            Physics.Debug.Initialize(ScreenManager.GraphicsDevice);
#endif
            if (loadAllContent)
            {
                if (content == null) content = new ContentManager(ScreenManager.Game.Services);

                if (!Settings.Configuration.ModelDescriptions.isLoaded)
                {
                    Settings.Configuration.ModelDescriptions.LoadModels("Settings\\Storage\\ModelList.xml", World.Instance.WorldContent);
                }
                //Settings.Configuration.ModelDescriptions.LoadModels("Settings\\Storage\\ModelList.xml", World.Instance.WorldContent);
                Settings.Configuration.TrackDescription.LoadTrackListEntry("Settings\\Storage\\" + Settings.Configuration.trackName + ".xml");

                backgroundTexture = World.Instance.WorldContent.Load<Texture2D>("Content/Textures/Black");

                //make sure the world is empty
                World.Instance.ResetWorld();
                
                //create a new window to the world
                World.Instance.Camera = new Camera();
                World.Instance.Camera.Aspect = ScreenManager.GraphicsDevice.PresentationParameters.BackBufferWidth / ScreenManager.GraphicsDevice.PresentationParameters.BackBufferHeight;


                //initialize a new content manager for the world
                World.Instance.WorldContent = new ContentManager(ScreenManager.Game.Services);

                cockpit = new HUD.HUD(content, ScreenManager);
                cockpit.SetMaxSpeed(0.0125f);
                cockpit.setLapCountMax(this.numLaps);
                cockpit.LapCountVisible(this.gameStyle != GameStyle.Free);
                cockpit.ResetPositionTable();

               
                //load Level
                LevelFactory.LoadLevel(Configuration.levelName, LevelFactory.LoadType.NEW);

               
                    themeCue = Sound.Play(theme);
                    if (Configuration.EngineSettings.playMusic) Sound.Stop(themeCue);

                Restart();

                

                RenderManager.Instance.Initialize();

                

            }
        }





        /// <summary>
        /// Unload graphics content used by the game.
        /// </summary>
        public override void UnloadGraphicsContent(bool unloadAllContent)
        {
            if (unloadAllContent)
            {
                if (Configuration.EngineSettings.playMusic) Sound.Stop(themeCue);
                content.Unload();
                //clear the world
                World.Instance.ResetWorld();
            }
        }

        #endregion

        #region Update and Draw

        /// <summary>
        /// Updates the state of the game. This method checks the GameScreen.IsActive
        /// property, so the game will stop updating when the pause menu is active,
        /// or if you tab away to a different application.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
//#if DEBUG
            PerformanceEater last = PerformanceMeter.Instance.PerfomanceEaterChange(PerformanceEater.Framework);
//#endif
           
            if(waitGo)
            {
                if (!hasGameTime)
                {
                    startGameTime = Convert.ToInt32(gameTime.TotalGameTime.TotalMilliseconds);
                    hasGameTime = true;
                    cockpit.Go(false);
                    cockpit.Eins(false);
                    cockpit.Zwei(false);
                    cockpit.Drei(false);
                    cockpit.ResetPositionTable();
                }

                Int32 delta = Convert.ToInt32(gameTime.TotalGameTime.TotalMilliseconds) - startGameTime;

                if (delta > 4000 + delay)
                {
                    cockpit.Eins(false);
                    cockpit.Go(true);
                    waitGo = false;
                    if (soundStep == 4)
                    {
                        Sound.PlayCue(Sounds.fart);
                        soundStep = 1;
                    }
                }
                else if (delta > 3000 + delay)
                {
                    cockpit.Zwei(false);
                    cockpit.Eins(true);
                    if (soundStep == 3)
                    {
                        Sound.PlayCue(Sounds.fart);
                        soundStep++;
                    }
                }
                else if (delta > 2000 + delay)
                {
                    cockpit.Drei(false);
                    cockpit.Zwei(true);
                    if (soundStep == 2)
                    {
                        Sound.PlayCue(Sounds.fart);
                        soundStep++;
                    }
                    
                }
                else if (delta > 1000 + delay)
                {
                    cockpit.Drei(true);
                    if (soundStep == 1)
                    {
                        Sound.PlayCue(Sounds.fart);
                        soundStep++;
                    }
                }
            }

            if ((Convert.ToInt32(gameTime.TotalGameTime.TotalMilliseconds) - startGameTime) > 5000 + delay)
            {
                cockpit.Go(false);
            }

            if (!this.paused && !this.raceFinished)
            {
                // init lapStart if necessary
                for (int i = 0; i < this.lapTimings.Length; i++)
                    if (this.lapTimings[i].lapStart == TimeSpan.Zero)
                        this.lapTimings[i].lapStart = gameTime.TotalGameTime;
            }

            if (!this.paused && !this.raceFinished && !waitGo)
            {
                // record segment numbers before physics update
                int[] lastSegment = GetSegmentNumbers();

                // Update the AI.
                if (!this.replaying)
                    AI.AISystem.Instance.Update(gameTime);

                // update all objects in the world
                if (!this.replaying)
                {
                    // update gameTime, PhysicsSystem, ParticleManager
                    World.Instance.Update(gameTime);
                }

                UpdateMovementRecorders(gameTime);

                // record segment numbers after physics update
                int[] curSegment = GetSegmentNumbers();

                // detect if lap was finished
                for (int i = 0; i < this.lapTimings.Length; i++)
                {
                    if (lastSegment[i] > 5 && curSegment[i] == 0)
                    {
                        // lap finished!
                        this.lapTimings[i].curLap++;

                        // finished race? -> update position table
                        if (this.lapTimings[i].curLap > this.numLaps)
                        {
                            if (i == 0)
                                this.cockpit.SetPositionTable(World.Instance.PlayersShip);
                            else
                                this.cockpit.SetPositionTable(World.Instance.EnemyShips[i - 1]);
                        }

                        // if enemy ship finished, disable it's input(ai) plugin
                        if (i > 0 && this.lapTimings[i].curLap > this.numLaps)
                            World.Instance.EnemyShips[i - 1].InputPlugin.Active = false;

                        //if (this.lapTimings[i].curLap > 1 && World.Instance.GhostShip != null)
                        //    World.Instance.GhostShip.RenderingPlugin.Hidden = false;

                        // record time
                        if (this.lapTimings[i].lapTimes.Count < this.lapTimings[i].curLap - 1)
                        {
                            this.lapTimings[i].lapTimes.Add(gameTime.TotalGameTime - this.lapTimings[i].lapStart - this.lapTimings[i].accumulatedPauseTime);
                            this.lapTimings[i].lapStart = gameTime.TotalGameTime;
                            this.lapTimings[i].accumulatedPauseTime = TimeSpan.Zero;
                        }
                    }
                    else if (lastSegment[i] == 0 && curSegment[i] > 5)
                    {
                        // passed finish line in wrong direction!
                        this.lapTimings[i].curLap--;
                    }
                }

                // update cockpit lap counter
                if (this.gameStyle != GameStyle.Free)
                {
                    if (this.lapTimings[0].curLap > this.numLaps)
                        this.raceFinished = true;
                    else
                        this.cockpit.setLapCount(this.lapTimings[0].curLap);
                }

                //Player dead?
                if (World.Instance.PlayersShip.LogicPlugin.Health <= 0.0)
                    this.playerDead = true;
                else
                    this.playerDead = false;

                UpdatePlayerPosition();

                UpdateWrongwayIndicator();

                updatePowerUpDisplay();

                
            }
            else if(!this.raceFinished)
            {
                for (int i = 0; i < this.lapTimings.Length; i++)
                    this.lapTimings[i].accumulatedPauseTime += gameTime.ElapsedGameTime;
                World.Instance.ParticleManager.Update(gameTime);
            }

            // if finished, show menu
            
            if (this.raceFinished && !this.paused)
            {
                this.Pause();
                this.StopMovementRecorders();
                switch (this.gameStyle)
                {
                    case GameStyle.Race:
                        if (this.playerPosition == 1) Sound.PlayCue(Sounds.win);  // winner
                        else if (this.playerPosition == 2) Sound.PlayCue(Sounds.sogeil);
                        else Sound.PlayCue(Sounds.loser);   // loser
                        cockpit.ResetPositionTable();
                        ScreenManager.AddScreen(new PauseMenuScreen(this, false, "You finished " + this.GetPositionString() + "!", this.GetPositionColor()));
                        break;
                    default:
                        Sound.PlayCue(Sounds.fart);
                        ScreenManager.AddScreen(new PauseMenuScreen(this, false));
                        break;
                }
            }

            if (this.playerDead && !this.paused)
            {
                this.Pause();
                this.StopMovementRecorders();
                Sound.PlayCue(Sounds.loser);
                ScreenManager.AddScreen(new PauseMenuScreen(this, false, "You've overestimated your own capabilities! You're dead!", Color.Red));
                this.playerDead = false;
                ((Ship)World.Instance.PlayersShip.LogicPlugin).addHealth(World.Instance.PlayersShip.LogicPlugin.MaxHealth);
            }

            SortedList<int, BaseEntity> blub = World.Instance.Objects;

            // set camera according to current viewmode

            BaseEntity ps = World.Instance.PlayersShip;
            Camera c = World.Instance.Camera;

            if (replaying)
            {
                if (Vector3.Distance(c.Position, ps.Position) > 10f)
                {
                    c.Position = ps.Position + ps.Orientation.Forward * 10f;
                }

                World.Instance.Camera.LookAt = ps.Position;
                World.Instance.Camera.Up = Vector3.Up;
            }
            else
            {

                ChasingCamera.ChasePosition = ps.Position;
                ChasingCamera.ChaseDirection = ps.Orientation.Forward;
                ChasingCamera.Up = ps.Orientation.Up;
                ChasingCamera.Update(gameTime);

                switch (World.Instance.ViewType)
                {
                    case ViewTypes.COCKPIT:
                        World.Instance.Camera.Position = ps.Position - ps.Orientation.Forward * 0.2f;
                        World.Instance.Camera.LookAt = ps.Position + ps.Orientation.Forward;
                        World.Instance.Camera.Up = ps.Orientation.Up;
                        World.Instance.Camera.Update();
                        World.Instance.PlayersShip.RenderingPlugin.Hidden = true;
                        break;
                    case ViewTypes.THIRDPERSON:
                        /*Vector3 pullingPoint = World.Instance.PlayersShip.Position + World.Instance.PlayersShip.Orientation.Up * 0.6f;

                        World.Instance.Camera.Position = pullingPoint + Vector3.Normalize(World.Instance.Camera.Position - pullingPoint) * 1.5f;
                        World.Instance.Camera.LookAt = World.Instance.PlayersShip.Position;
                        World.Instance.Camera.Up = World.Instance.PlayersShip.Orientation.Up;
                        World.Instance.Camera.Update();*/

                        World.Instance.Camera.Position = ChasingCamera.Position;
                        World.Instance.Camera.LookAt = ps.Position;
                        World.Instance.Camera.Up = ps.Orientation.Up;

                        if (((RigidBody)World.Instance.PlayersShip.PhysicsPlugin).m_bHidden == false)
                            World.Instance.PlayersShip.RenderingPlugin.Hidden = false;
                        else
                            World.Instance.PlayersShip.RenderingPlugin.Hidden = true;
                        break;
                }
            }

            if (!Settings.Configuration.EngineSettings.playMusic && themeCue!=null&& themeCue.IsPlaying ) Sound.Stop(themeCue);
            if (Settings.Configuration.EngineSettings.playMusic && !themeCue.IsPlaying) themeCue = Sound.Play(theme);

            //Update Environment (Rotating planets etc.)
            World.Instance.gameEnvironment.Update(gameTime);

            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

//#if DEBUG
            PerformanceMeter.Instance.PerfomanceEaterChange(last);
//#endif
        }

        private void updatePowerUpDisplay()
        {
            if (((RigidBody)World.Instance.PlayersShip.PhysicsPlugin).powerUps.ContainsKey(PowerUpTypes.TURBO))
            {
                if ((((RigidBody)World.Instance.PlayersShip.PhysicsPlugin).powerUps[PowerUpTypes.TURBO]).Count > 0)
                {
                    cockpit.SetPickUp(PraktWS0708.HUD.HUD.PickupType.TURBO, true);
                }
                else
                {
                    cockpit.SetPickUp(PraktWS0708.HUD.HUD.PickupType.TURBO, false);
                }
            }
            if (((RigidBody)World.Instance.PlayersShip.PhysicsPlugin).powerUps.ContainsKey(PowerUpTypes.SHIELD))
            {
                if ((((RigidBody)World.Instance.PlayersShip.PhysicsPlugin).powerUps[PowerUpTypes.SHIELD]).Count > 0)
                {
                    cockpit.SetPickUp(PraktWS0708.HUD.HUD.PickupType.SHIELD, true);
                }
                else
                {
                    cockpit.SetPickUp(PraktWS0708.HUD.HUD.PickupType.SHIELD, false);
                }
            }
            if (((RigidBody)World.Instance.PlayersShip.PhysicsPlugin).powerUps.ContainsKey(PowerUpTypes.HEALTHPACK))
            {
                if ((((RigidBody)World.Instance.PlayersShip.PhysicsPlugin).powerUps[PowerUpTypes.HEALTHPACK]).Count > 0)
                {
                    cockpit.SetPickUp(PraktWS0708.HUD.HUD.PickupType.HEALTHPACK, true);
                }
                else
                {
                    cockpit.SetPickUp(PraktWS0708.HUD.HUD.PickupType.HEALTHPACK, false);
                }
            }
            if (((RigidBody)World.Instance.PlayersShip.PhysicsPlugin).powerUps.ContainsKey(PowerUpTypes.BANANA))
            {
                if ((((RigidBody)World.Instance.PlayersShip.PhysicsPlugin).powerUps[PowerUpTypes.BANANA]).Count > 0)
                {
                    cockpit.SetPickUp(PraktWS0708.HUD.HUD.PickupType.BANANA, true);
                }
                else
                {
                    cockpit.SetPickUp(PraktWS0708.HUD.HUD.PickupType.BANANA, false);
                }
            }/*
            if (((RigidBody)World.Instance.PlayersShip.PhysicsPlugin).powerUps.ContainsKey(PowerUpTypes.ROCKET))
            {
                if ((((RigidBody)World.Instance.PlayersShip.PhysicsPlugin).powerUps[PowerUpTypes.ROCKET]).Count > 0)
                {
                    cockpit.SetPickUp(PraktWS0708.HUD.HUD.PickupType.ROCKET, true);
                }
                else
                {
                    cockpit.SetPickUp(PraktWS0708.HUD.HUD.PickupType.ROCKET, false);
                }
            }*/
        }

        private void UpdateMovementRecorders(GameTime gameTime)
        {
            foreach (MovementRecorder rec in this.movementRecorders)
            {
                if (rec.CurrentState == MovementRecorder.State.Idle)
                {
                    if (this.replaying)
                        rec.StartPlayback(gameTime);
                    else
                        rec.StartRecording(gameTime);
                }
                else
                {
                    rec.Update(gameTime);
                }
            }
        }

        private void StopMovementRecorders()
        {
            foreach (MovementRecorder rec in this.movementRecorders)
                rec.Stop();
        }

        private int[] GetSegmentNumbers()
        {
            int[] segments = new int[this.lapTimings.Length];
            segments[0] = World.Instance.PlayersShip.CurrentSegment;
            for (int i = 1; i < segments.Length; i++)
                segments[i] = World.Instance.EnemyShips[i - 1].CurrentSegment;

            return segments;
        }

        private void UpdatePlayerPosition()
        {
            this.playerPosition = 1;

            for (int i = 1; i < this.lapTimings.Length; i++)
            {
                if (this.lapTimings[i].curLap > this.lapTimings[0].curLap ||
                    (this.lapTimings[i].curLap == this.lapTimings[0].curLap &&
                        World.Instance.EnemyShips[i - 1].CurrentSegment >= World.Instance.PlayersShip.CurrentSegment))
                {
                    this.playerPosition++;
                }
            }

            this.cockpit.setPosition(this.playerPosition);
        }

        private void UpdateWrongwayIndicator()
        {
            Track track = World.Instance.Track;
            BaseEntity entity = World.Instance.PlayersShip;

            int count = track.TangentFrames.Length;
            int frame = 0;
            float distance = Single.MaxValue;

            // Get the closest tangent frame
            for (int i = 0; i < count; ++i)
            {
                float d = Vector3.Distance(track.TangentFrames[i].Position, entity.Position);
                if (d < distance)
                {
                    frame = i;
                    distance = d;
                }
            }

            bool wrongWay = (Vector3.Dot(track.TangentFrames[frame].Tangent, entity.Orientation.Forward) < 0.0f);
            this.cockpit.Wrongway(wrongWay);
        }

        public override void Draw(GameTime gameTime)
        {
//#if DEBUG
            //PerformanceEater last = PerformanceMeter.Instance.PerfomanceEaterChange(PerformanceEater.Rendering);
//#endif

            base.Draw(gameTime);

            // draw all objects of the world

            RenderManager.Instance.DrawMiniMap = RenderManager.Instance.DrawMirror = (!this.paused && !this.replaying);
            RenderManager.Instance.ApplyOldCameraEffect = this.replaying;
            RenderManager.Instance.Draw(gameTime);

            // draw hud
            if (!this.paused && !this.replaying)
            {
                int lapsFinished = this.lapTimings[0].lapTimes.Count;
                if (this.lapTimings[0].lapTimes.Count > 1)
                {
                    cockpit.setLastLapTime(lapsFinished + "  " + MakeTimeString(this.lapTimings[0].lapTimes[lapsFinished - 1]));
                    cockpit.setCurrentLapTime((lapsFinished + 1) + "  " + MakeTimeString(gameTime.TotalGameTime - this.lapTimings[0].lapStart - this.lapTimings[0].accumulatedPauseTime));
                }
                else
                {
                    cockpit.setLastLapTime((lapsFinished + 1) + "  " + MakeTimeString(gameTime.TotalGameTime - this.lapTimings[0].lapStart - this.lapTimings[0].accumulatedPauseTime));
                    cockpit.setCurrentLapTime("");
                }

                cockpit.setDamageBar(100f * Entities.World.Instance.PlayersShip.LogicPlugin.Health / Entities.World.Instance.PlayersShip.LogicPlugin.MaxHealth);
                cockpit.SetCurrentSpeed(World.Instance.PlayersShip.PhysicsPlugin.Velocity);


                cockpit.draw(gameTime);

#if DEBUG
                Physics.Debug.Draw();
#endif
            }

//#if DEBUG
            //PerformanceMeter.Instance.PerfomanceEaterChange(last);
//#endif
        }

        private string GetPositionString()
        {
            if (this.playerPosition == World.Instance.EnemyShips.Length + 1)
                return "Last";

            string result = this.playerPosition.ToString();
            switch (this.playerPosition % 10)
            {
                case 1:
                    result += "st";
                    break;
                case 2:
                    result += "nd";
                    break;
                case 3:
                    result += "rd";
                    break;
                default:
                    result += "th";
                    break;
            }
            return result;
        }

        private Color GetPositionColor()
        {
            if (this.playerPosition == World.Instance.EnemyShips.Length + 1)
                return Color.Red;

            switch (this.playerPosition)
            {
                case 1:
                    return Color.Green;
                case 2:
                    return Color.Yellow;
                case 3:
                    return Color.Orange;
                default:
                    return Color.Gray;
            }
        }

        private string MakeTimeString(TimeSpan time)
        {
            return ((int)time.TotalMinutes).ToString() + ":" + time.Seconds.ToString("00") + "." + time.Milliseconds.ToString("000");
        }

        #endregion

        #region Methods

        public void Restart()
        {
            Rendering.RenderManager.Instance.ClearWaypoints();

            if (Configuration.EngineSettings.playMusic && themeCue.IsPlaying)
            {
                Sound.Stop(themeCue);
                theme = Sounds.GameMusic;
                Sound.Play(theme);
            }

            LevelFactory.LoadLevel(Configuration.levelName, LevelFactory.LoadType.RESET);

            World.Instance.PlayersShip.InputPlugin = new PraktWS0708.Input.HumanPlayer1InputPlugin(World.Instance.PlayersShip);
            World.Instance.PlayersShip.InputPlugin.Type = PraktWS0708.Input.InputPlugin.InputPluginType.PLAYER1;

            AI.AISystem.Instance.Initialize();

            this.cockpit.setDriverCount(1 + World.Instance.EnemyShips.Length);

            // init movement recorders
            this.movementRecorders = new MovementRecorder[World.Instance.Objects.Values.Count];
            for (int i = 0; i < this.movementRecorders.Length; i++)
                this.movementRecorders[i] = new MovementRecorder(World.Instance.Objects.Values[i]);

            this.ResetLapTimings();

            // resume if paused
            this.Resume();

            this.raceFinished = false;
            this.replaying = false;

            hasGameTime = false;

            waitGo = true;

            // enable input
            World.Instance.PlayersShip.InputPlugin.Active = true;

            //ChasingCamera.Position = World.Instance.PlayersShip.Position;
        }

        public void Replay()
        {
            Rendering.RenderManager.Instance.ClearWaypoints();

            if (Configuration.EngineSettings.playMusic && themeCue.IsPlaying)
            {
                Sound.Stop(themeCue);
                theme = Sounds.movieprojector;
                Sound.Play(theme);
            }

            // stop movement recorders
            // replay will be started during next update
            this.StopMovementRecorders();

            this.ResetLapTimings();

            //set playership visible
            World.Instance.PlayersShip.RenderingPlugin.Hidden = false;
            ((SteeringBody)World.Instance.PlayersShip.PhysicsPlugin).m_bHidden = false;

            // resume if paused
            this.Resume();

            this.raceFinished = false;
            this.replaying = true;

            // disable input
            World.Instance.PlayersShip.InputPlugin.Active = false;
        }

        private void ResetLapTimings()
        {
            this.lapTimings = new LapTimings[1 + World.Instance.EnemyShips.Length];
            this.lapTimings[0] = new LapTimings((World.Instance.PlayersShip.CurrentSegment < 20) ? 1 : 0);
            for (int i = 1; i < this.lapTimings.Length; i++)
                this.lapTimings[i] = new LapTimings((World.Instance.EnemyShips[i - 1].CurrentSegment < 20) ? 1 : 0);
        }

        public void Pause()
        {
            this.paused = true;
         //   if(waitGo) waitGoPause = true;
            this.inputWasActive = World.Instance.PlayersShip.InputPlugin.Active;
            World.Instance.PlayersShip.InputPlugin.Active = false;
            foreach (MovementRecorder rec in this.movementRecorders)
                rec.Pause();
        }

        public void Resume()
        {
            this.paused = false;
          /*  if(waitGoPause)
            {
                hasGameTime = false;
                waitGo = true;
                waitGoPause = false;
            } */
            World.Instance.PlayersShip.InputPlugin.Active = this.inputWasActive;
            this.inputWasActive = false;
            foreach (MovementRecorder rec in this.movementRecorders)
                rec.Resume();
        }

        #endregion

        #region Handle Input

        /// <summary>
        /// Responds to user input, changing the selected ship
        /// or cancelling the menu.
        /// </summary>
        public override void HandleInput()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.F11))
            {
                RenderManager.Instance.MakeScreenshot = true;
            }

            if (InputState.instance.PauseGame)
            {
                if (!waitGo)
                {
                    this.Pause();
                    Sound.PlayCue(Sounds.fart);
                    ScreenManager.AddScreen(new PauseMenuScreen(this, true, "Game paused", Color.Gray));
                }
            }
            else if (InputState.instance.MenuSelect)
            {
                if (World.Instance.ViewType == ViewTypes.THIRDPERSON) World.Instance.ViewType = ViewTypes.COCKPIT;
                else World.Instance.ViewType++;
            }
#if DEBUG
            if (InputState.instance.LeftShoulderDown)
            {
                Physics.Debug.Load();
            }
#endif
        }

        #endregion
    }
}
