using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace PraktWS0708.Utils
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>

    public enum PerformanceEater
    {
        Undef, Framework, Physics, Rendering, AI, Logic, Particles
    }

    public class PerformanceMeter : DrawableGameComponent
    {
        public static PerformanceMeter Instance;

        private static TimeSpan OneSecond = TimeSpan.FromSeconds(1.0);

        private int m_DrawCounter;
        private int m_UpdateCounter;

        private float m_UndefCounter;
        private float m_FrameworkCounter;
        private float m_PhysicsCounter;
        private float m_ParticlesCounter;
        private float m_RenderingCounter;
        private float m_AICounter;
        private float m_LogicCounter;

        private PerformanceEater m_CurrentPerfomanceEater;
        private DateTime m_LastCallTime;

        private TimeSpan m_ElapsedTime;
        
        private float m_DrawRate;
        private float m_UpdateRate;
        private float m_UndefRate;
        private float m_FrameworkRate;
        private float m_PhysicsRate;
        private float m_ParticlesRate;
        private float m_RenderingRate;
        private float m_AIRate;
        private float m_LogicRate;

        private bool m_IsVisible = false;

        private Texture2D menuSpriteTex;

        private Keys m_VisibilityKey;
        private bool m_VisibilityKeyLastState;

        public Keys VisibilityKey
        {
            get { return m_VisibilityKey; }
            set { m_VisibilityKey = value; }
        }

        public new bool Visible
        {
            get { return m_IsVisible; }
            set { m_IsVisible = value; }
        }

        public float DrawRate
        {
            get { return m_DrawRate; }
        }       

        public float UpdateRate
        {
            get { return m_UpdateRate; }
        }

        private ScreenManager screenManager;

        public ScreenManager ScreenManager
        {
            get { return screenManager; }
            set { screenManager = value; }
        }


        public PerformanceMeter(ScreenManager manager, Keys key)
            : base(manager.Game)
        {
            screenManager = manager;
            m_VisibilityKey = key;

            // Draw after the ScreenManager
            DrawOrder = manager.DrawOrder + 1;

            m_CurrentPerfomanceEater = PerformanceEater.Undef;
            m_UndefCounter = 0f;
            m_FrameworkCounter = 0f;
            m_PhysicsCounter = 0f;
            m_RenderingCounter = 0f;
            m_AICounter = 0f;
            m_LogicCounter = 0f;
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
        }


        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            PerformanceEater last = this.PerfomanceEaterChange(PerformanceEater.Logic);

            m_ElapsedTime += gameTime.ElapsedGameTime;

            if (m_ElapsedTime > OneSecond)
            {
                float timeSpan = (float)m_ElapsedTime.TotalMilliseconds / 1000f;

                m_DrawRate = (float)(m_DrawCounter / timeSpan);
                m_UpdateRate = (float)(m_UpdateCounter / timeSpan);
                m_DrawCounter = 0;
                m_UpdateCounter = 0;

                m_UndefRate = (float)(m_UndefCounter / timeSpan);
                m_FrameworkRate = (float)(m_FrameworkCounter / timeSpan);
                m_PhysicsRate = (float)(m_PhysicsCounter / timeSpan);
                m_ParticlesRate = (float)(m_ParticlesCounter / timeSpan);
                m_RenderingRate = (float)(m_RenderingCounter / timeSpan);
                m_AIRate = (float)(m_AICounter / timeSpan);
                m_LogicRate = (float)(m_LogicCounter / timeSpan);
                
                m_UndefCounter = 0f;
                m_FrameworkCounter = 0f;
                m_PhysicsCounter = 0f;
                m_ParticlesCounter = 0f;
                m_RenderingCounter = 0f;
                m_AICounter = 0f;
                m_LogicCounter = 0f;

                m_ElapsedTime = TimeSpan.Zero;
            }

            if (!m_VisibilityKeyLastState && (Keyboard.GetState().IsKeyDown(m_VisibilityKey) || GamePad.GetState(PlayerIndex.One).Buttons.LeftShoulder == ButtonState.Pressed))
            {
                m_IsVisible = !m_IsVisible;
                Sound.PlayCue(Sounds.MenuNext);
            }

            m_VisibilityKeyLastState = (Keyboard.GetState().IsKeyDown(m_VisibilityKey) || GamePad.GetState(PlayerIndex.One).Buttons.LeftShoulder == ButtonState.Pressed);

            m_UpdateCounter++;

            base.Update(gameTime);

            this.PerfomanceEaterChange(last);
        }

        public override void Draw(GameTime gameTime)
        {
            PerformanceEater last = this.PerfomanceEaterChange(PerformanceEater.Rendering);

            if (m_IsVisible)
            {
                string message = string.Format("Draws/s: {0:0.0}", m_DrawRate) + "\n" +
                    string.Format("Updates/s: {0:0.0}", m_UpdateRate) + "\n" +
                    string.Format("Undef: {0:0.0}%", m_UndefRate * 100) + "\n" +
                    string.Format("Physics: {0:0.0}%", m_PhysicsRate * 100) + "\n" +
                    string.Format("Particles: {0:0.0}%", m_ParticlesRate * 100) + "\n" +
                    string.Format("Logic: {0:0.0}%", m_LogicRate * 100) + "\n" +
                    string.Format("Rendering: {0:0.0}%", m_RenderingRate * 100) + "\n" +
                    string.Format("AI: {0:0.0}%", m_AIRate * 100); 
                
                Vector2 size = ScreenManager.Font.MeasureString(message);

                if (menuSpriteTex == null)
                {
                    menuSpriteTex = ScreenManager.Content.Load<Texture2D>("Content/Textures/gradient");
                    //menuSpriteTex = new Texture2D(ScreenManager.GraphicsDevice, 32, 32, 0, ResourceUsage.None, SurfaceFormat.Color);
                    //menuSpriteTex.GraphicsDevice.Clear(new Color(128, 128, 128, 255));
                }


                ScreenManager.SpriteBatch.Begin(SpriteBlendMode.AlphaBlend);

                ScreenManager.SpriteBatch.Draw(menuSpriteTex, new Rectangle(0, Settings.Configuration.EngineSettings.ScreenResolution.Height / 11, 136, 170), new Color(128, 128, 255, 192));

                ScreenManager.SpriteBatch.DrawString(
                    ScreenManager.Font,
                    message,
                    new Vector2(Settings.Configuration.EngineSettings.ScreenResolution.Width * 0.01f, Settings.Configuration.EngineSettings.ScreenResolution.Height * 0.1f),
                    Color.WhiteSmoke,
                    0.0f,
                    new Vector2(0, 0),
                    0.5f,
                    SpriteEffects.None,
                    0);

                ScreenManager.SpriteBatch.End();

                base.Draw(gameTime);
            }

            m_DrawCounter++;

            this.PerfomanceEaterChange(last);
        }

        public PerformanceEater PerfomanceEaterChange(PerformanceEater eater)
        {
            PerformanceEater retVal = m_CurrentPerfomanceEater;

            DateTime now = DateTime.Now;
            TimeSpan span = now - m_LastCallTime;

            float spanSeconds = (float)span.TotalMilliseconds / 1000f;
           
            switch (m_CurrentPerfomanceEater)
            {
                case PerformanceEater.Undef:
                    m_UndefCounter += spanSeconds;
                    break;
                case PerformanceEater.AI:
                    m_AICounter += spanSeconds;
                    break;
                case PerformanceEater.Framework:
                    m_FrameworkCounter += spanSeconds;
                    break;
                case PerformanceEater.Physics:
                    m_PhysicsCounter += spanSeconds;
                    break;
                case PerformanceEater.Rendering:
                    m_RenderingCounter += spanSeconds;
                    break;
                case PerformanceEater.Logic:
                    m_LogicCounter += spanSeconds;
                    break;
                case PerformanceEater.Particles:
                    m_ParticlesCounter += spanSeconds;
                    break;
            }

            m_CurrentPerfomanceEater = eater;
            m_LastCallTime = now;

            return retVal;
        }
    }
}


