#region File Description
//-----------------------------------------------------------------------------
// SelectShipScreen.cs
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
using System.Collections.Generic;
using PraktWS0708.Utils;
#endregion

namespace PraktWS0708
{
    /// <summary>
    /// This screen currently displays a rotating ship
    /// </summary>
    class ParticleSystemScreen : GameScreen
    {
        #region Fields

        private Cue themeCue;
        InputState oInput;

        // Textures
        //private Texture2D oNoiseTexture;
        private Texture2D oColorTexture;

        // Effects
        private Effect oEffect;

        // EffectParameters
        private EffectParameter worldViewParam;
        private EffectParameter worldViewProjectionParam;
        private EffectParameter colorTextureParam;
        /*private EffectParameter noiseTextureParam;
        private EffectParameter centerParam;
        private EffectParameter amplitudeParam;
        private EffectParameter speedParam;
        private EffectParameter timeParam;*/
        // Start <ParticleMap>
        private EffectParameter positionParam;
        private EffectParameter widthParam;
        private EffectParameter heightParam;
        private EffectParameter timeParam;
        // End <ParticleMap>

        // ContentManager
        ContentManager oContent;

        // VertexBuffer + VertexDeclaration
        VertexBuffer oVertexBuffer;
        VertexDeclaration oVertexDeclaration;
        int iPrimitiveCount = 2048;

        // Additional
        private Vector4 vEyePos;
        private Matrix matScale;
        private Matrix matRotZ;
        private Matrix matRotY;
        private Matrix matView;
        private Matrix matProj;
        private float fScaleShip = 0.4f;

        // param data
        private float fTime = 0f;
        private float fSpeed = 1f;
        private float fAmplitude = 0.5f;
        private Vector4 vCenter = Vector4.Zero;

        #endregion

        #region Initialization

        /// <summary>
        /// Load graphics content for the game.
        /// </summary>
        public override void LoadGraphicsContent(bool loadAllContent)
        {
            if (loadAllContent)
            {
                if (oContent == null) oContent = new ContentManager(ScreenManager.Game.Services);

                vEyePos     = new Vector4(0, 20, -100, 0);
                matView     = Matrix.CreateLookAt(new Vector3(vEyePos.X, vEyePos.Y, vEyePos.Z), Vector3.Zero, Vector3.UnitY);
                matProj     = Matrix.CreatePerspectiveFieldOfView(MathHelper.Pi / 4, ScreenManager.GraphicsDevice.PresentationParameters.BackBufferWidth / ScreenManager.GraphicsDevice.PresentationParameters.BackBufferHeight, 1.0f, 1000.0f);
                matRotY     = Matrix.Identity;
                matRotZ     = Matrix.Identity;
                matScale    = Matrix.CreateScale(fScaleShip);

                oEffect                     = oContent.Load<Effect>(@"Content/Shader/ParticleMap");
                worldViewParam              = oEffect.Parameters["worldView"];
                worldViewProjectionParam    = oEffect.Parameters["worldViewProjection"];
                
                colorTextureParam           = oEffect.Parameters["ColorMap"];
                /*centerParam                 = oEffect.Parameters["center"];
                noiseTextureParam           = oEffect.Parameters["NoiseMap"];
                amplitudeParam              = oEffect.Parameters["amplitude"];
                speedParam                  = oEffect.Parameters["speed"];
                timeParam                   = oEffect.Parameters["time"];

                // load model
                oNoiseTexture = oContent.Load<Texture2D>(@"Content/Textures/particleNoise");*/
                oColorTexture = oContent.Load<Texture2D>(@"Content/Textures/particleColor");

                // Start <ParticleMap>
                positionParam = oEffect.Parameters["PositionMap"];
                widthParam = oEffect.Parameters["width"];
                heightParam = oEffect.Parameters["height"];
                timeParam = oEffect.Parameters["time"];
                // End <ParticleMap>

                // initialize VertexBuffer + VertexDeclaration
                oVertexDeclaration = new VertexDeclaration(ScreenManager.GraphicsDevice, VertexPositionNormalTexture.VertexElements);
                oVertexBuffer = new VertexBuffer(ScreenManager.GraphicsDevice, VertexPositionNormalTexture.SizeInBytes * iPrimitiveCount * 3, ResourceUsage.None);

                VertexPositionNormalTexture[] oVertexPositionNormalTexture = new VertexPositionNormalTexture[iPrimitiveCount * 3];

                Random oRandom = new Random();
                Vector3 vRandom = Vector3.Zero, vRandom2;

                for (int iIndex = 0; iIndex < oVertexPositionNormalTexture.Length; iIndex++)
                {
                    if (iIndex % 3 == 0)
                    {
                        do
                        {
                            vRandom = new Vector3(
                                (float)(oRandom.NextDouble() * 2.0 - 1.0),
                                (float)(oRandom.NextDouble() * 2.0 - 1.0),
                                (float)(oRandom.NextDouble() * 2.0 - 1.0));
                        }
                        while (vRandom.Length() > 0.5f);
                    }

                    do
                    {
                        vRandom2 = new Vector3(
                            (float)(oRandom.NextDouble() * 2.0 - 1.0),
                            (float)(oRandom.NextDouble() * 2.0 - 1.0),
                            (float)(oRandom.NextDouble() * 2.0 - 1.0));
                    }
                    while (vRandom2.Length() > 0.5f);

                    oVertexPositionNormalTexture[iIndex].Position = vRandom2;
                    oVertexPositionNormalTexture[iIndex].Normal = vRandom;
                    oVertexPositionNormalTexture[iIndex].TextureCoordinate = new Vector2(iIndex / 3, iIndex % 3);
                }

                oVertexBuffer.SetData<VertexPositionNormalTexture>(oVertexPositionNormalTexture);

                oInput = new InputState();

                themeCue = Sound.Play(Sounds.MenuTheme);
            }
        }

        /// <summary>
        /// Unload graphics content used by the game.
        /// </summary>
        public override void UnloadGraphicsContent(bool unloadAllContent)
        {
            if (unloadAllContent)
            {
                if (oContent != null) oContent.Unload();
                oEffect.Dispose();
            }
        }   
        
        #endregion

        #region Update and Draw
        
        /// <summary>
        /// Updates the state of the game. This method checks the GameScreen.IsActive
        /// property, so the game will stop updating when the pause menu is active,
        /// or if you tab away to a different application.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
            matRotY *= Matrix.CreateRotationY(MathHelper.ToRadians(Convert.ToSingle(gameTime.ElapsedGameTime.Milliseconds)*0.04f));
            fTime = (float)(Math.Sin(((double)gameTime.TotalRealTime.Milliseconds / 1000.0
                                    + (double)gameTime.TotalRealTime.Seconds) / 40.0));
        }

        public override void Draw(GameTime gameTime)
        {
//#if DEBUG
            //PerformanceEater last = PerformanceMeter.Instance.PerfomanceEaterChange(PerformanceEater.Rendering);
//#endif

            Matrix matWorld = matScale * matRotZ * matRotY;
            worldViewParam.SetValue(matWorld * matView);
            worldViewProjectionParam.SetValue(matWorld * matView * matProj);

            /*noiseTextureParam.SetValue(oNoiseTexture);
            centerParam.SetValue(vCenter);
            amplitudeParam.SetValue(fAmplitude);
            speedParam.SetValue(fSpeed);*/
            colorTextureParam.SetValue(oColorTexture);
            timeParam.SetValue(fTime);
            positionParam.SetValue(oColorTexture);
            widthParam.SetValue(64);
            heightParam.SetValue(64);

            oEffect.GraphicsDevice.RenderState.PointSize = 3.0f;
            oEffect.CurrentTechnique = oEffect.Techniques["ParticleMap"];
            oEffect.CommitChanges();

            oEffect.Begin();
                oEffect.Techniques["ParticleMap"].Passes["Particles"].Begin();

                    for (int iIndex = 0; iIndex < iPrimitiveCount; iIndex++)
                    {
                        ScreenManager.GraphicsDevice.VertexDeclaration = oVertexDeclaration; //new VertexDeclaration(ScreenManager.GraphicsDevice, VertexPositionNormalTexture.VertexElements)
                        ScreenManager.GraphicsDevice.Vertices[0].SetSource(oVertexBuffer, 0, VertexPositionNormalTexture.SizeInBytes);
                        ScreenManager.GraphicsDevice.DrawPrimitives(PrimitiveType.PointList, 0, iPrimitiveCount * 3);
                    }

                oEffect.Techniques["ParticleMap"].Passes["Particles"].End();
            oEffect.End();

            //Draw the Text
            string message = "<< ParticleSystem >>";

            // Center the text in the viewport.
            Vector2 textPosition = new Vector2((ScreenManager.GraphicsDevice.Viewport.Width - ScreenManager.Font.MeasureString(message).X) / 2, 30);
            Color color = new Color(255, 255, 255, TransitionAlpha);

            // Draw the text.
            ScreenManager.SpriteBatch.Begin();
            ScreenManager.SpriteBatch.DrawString(ScreenManager.Font, message, textPosition, color);
            ScreenManager.SpriteBatch.End();

            // Data
            message = "Time: " + fTime + "\nSpeed: " + fSpeed + "\nAmplitude: " + fAmplitude + "\nCenter: " + vCenter;

            // Center the text in the viewport.
            textPosition = new Vector2(20, 80);
            color = new Color(185, 185, 185, TransitionAlpha);

            SpriteFont oSpriteFont = oContent.Load<SpriteFont>("Content/Fonts/tableviewfont");

            // Draw the text.
            ScreenManager.SpriteBatch.Begin();
            ScreenManager.SpriteBatch.DrawString(oSpriteFont, message, textPosition, color);
            ScreenManager.SpriteBatch.End();
//#if DEBUG
            //PerformanceMeter.Instance.PerfomanceEaterChange(last);
//#endif
        }

        #endregion

        #region Handle Input

        /// <summary>
        /// Event handler for when the user selects ok on the "are you sure
        /// you want to exit" message box.
        /// </summary>
        void ExitMessageBoxAccepted(object sender, EventArgs e)
        {
            ScreenManager.Game.Exit();
        }

        /// <summary>
        /// Responds to user input, changing the selected ship
        /// or cancelling the menu.
        /// </summary>
        public override void HandleInput()
        {
            if (InputState.instance.MenuCancel)
            {
                Sound.PlayCue(Sounds.MenuBack);
                Sound.Stop(themeCue);
                ScreenManager.AddScreen(new BackgroundScreen());
                ScreenManager.AddScreen(new MainMenuScreen());
                ExitScreen();
            }

            if (InputState.instance.MenuRight)
            {
                fTime += 0.1f;
            }

            if (InputState.instance.MenuLeft)
            {
                fTime -= 0.1f;
            }

            if (InputState.instance.MenuUp)
            {
                fSpeed += 0.1f;
            }

            if (InputState.instance.MenuDown)
            {
                fSpeed -= 0.1f;
            }
        }

        void LoadSelectTrackScreen(object sender, EventArgs e)
        {
            ScreenManager.AddScreen(new SelectTrackScreen());
        }

        #endregion
    }
}
