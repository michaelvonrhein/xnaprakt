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
    class SelectShipScreen : BackgroundScreen
    {
        #region Fields

        Cue                     themeCue;
        private const uint      iNumShips = 6;
        private static uint     iSelectedShip = 0;
        Model                   oShipModel;
        Texture2D               oShipTexture;
        Texture2D               oBlackTexture;
        TextureCube             oReflectionTexture1;
        TextureCube             oReflectionTexture2;
        Vector4                 vEyePos;
        Matrix                  matScale;
        Matrix                  matRotZ;
        Matrix                  matRotY;
        Matrix                  matView;
        Matrix                  matProj;
        private Effect          oEffect;
        private EffectParameter worldParam;
        private EffectParameter worldViewProjectionParam;
        private EffectParameter skinTextureParam;
        private EffectParameter reflectionTextureParam;
        private EffectParameter ambientParam;
        private EffectParameter directionalDirectionParam;
        private EffectParameter directionalColorParam;
        private EffectParameter materialParam;
        private float           fScaleShip = 0.002f;
        ContentManager          oShipContent;
        
        //Material with Color.White has texture so doesn't need material
        private static Color cockpit1   = new Color((byte)(.529f * 255), 255, 255, 0);
        private static Color cockpit2   = new Color(255, 255, (byte)(.373f * 255), 0);
        private static Color engines    = new Color((byte)(.925f * 255), (byte)(.529f * 255), 255, 0);

        private Color[][] shipMaterials = new Color[][]
        {   
                new Color[] {Color.White, engines, cockpit1, cockpit1},
                new Color[] {Color.White, cockpit1, engines},
                new Color[] {Color.White, cockpit1, engines},
                new Color[] {cockpit2, Color.White, Color.White},
                new Color[] {Color.White, Color.White, cockpit2},
                new Color[] {Color.White, cockpit2, Color.White},
        };

        //false = first reflection map, true = second
        private bool[][] shipPartUsesReflection = new bool[][]
        {   
                new bool[] {false, false, false, false},
                new bool[] {false, false, false},
                new bool[] {false, false, false},
                new bool[] {true, true, false},
                new bool[] {true, false, true},
                new bool[] {true, true, false},
        };

        private static string[] shipModels = new string[]
        {
            @"Content/Models/p1_pencil",
            @"Content/Models/p1_saucer",
            @"Content/Models/p1_wedge",
            @"Content/Models/p2_pencil",
            @"Content/Models/p2_saucer",
            @"Content/Models/p2_wedge"
        };

        private static string[] shipTextures = new string[]
        {
            @"Content/Textures/pencil_p1_diff_v1",
            @"Content/Textures/saucer_p1_diff_v1",
            @"Content/Textures/wedge_p1_diff_v1",
            @"Content/Textures/pencil_p2_diff_v1",
            @"Content/Textures/saucer_p2_diff_v1",
            @"Content/Textures/wedge_p2_diff_v1"
        };

        #endregion

        #region Initialization

        /// <summary>
        /// Load graphics content for the game.
        /// </summary>
        public override void LoadGraphicsContent(bool loadAllContent)
        {
            if (loadAllContent)
            {
                if (content == null) content = new ContentManager(ScreenManager.Game.Services);

                backgroundTexture = content.Load<Texture2D>("Content/Textures/ShipSelectBG");
                oBlackTexture     = content.Load<Texture2D>("Content/Textures/Black");

                vEyePos     = new Vector4(0, 7, -10, 0);
                matView     = Matrix.CreateLookAt(new Vector3(vEyePos.X, vEyePos.Y, vEyePos.Z), new Vector3(0, 0, 0), new Vector3(0, 1, 0));
                matProj     = Matrix.CreatePerspectiveFieldOfView(MathHelper.Pi / 4, ScreenManager.GraphicsDevice.PresentationParameters.BackBufferWidth / ScreenManager.GraphicsDevice.PresentationParameters.BackBufferHeight, 1.0f, 1000.0f);
                matRotY     = Matrix.Identity;
                matRotZ     = Matrix.Identity;
                matScale    = Matrix.CreateScale(fScaleShip);

                oEffect                     = content.Load<Effect>("Content/Shader/ship");
                worldParam                  = oEffect.Parameters["world"];
                worldViewProjectionParam    = oEffect.Parameters["worldViewProjection"];
                skinTextureParam            = oEffect.Parameters["SkinTexture"];
                reflectionTextureParam      = oEffect.Parameters["ReflectionTexture"];
                ambientParam                = oEffect.Parameters["Ambient"];
                directionalDirectionParam   = oEffect.Parameters["DirectionalDirection"];
                directionalColorParam       = oEffect.Parameters["DirectionalColor"];
                materialParam               = oEffect.Parameters["Material"];
                
                SetupEffect();
                LoadShip(iSelectedShip);

                themeCue = Sound.Play(Sounds.MenuTheme);
            }
        }

        /// <summary>
        /// Set a new ship and load the data
        /// </summary>
        ///
        private void LoadShip(uint iShipID) 
        {
            if (oShipContent == null)   oShipContent = new ContentManager(ScreenManager.Game.Services);
            else                        oShipContent.Unload();

            oShipModel      = oShipContent.Load<Model>(shipModels[iSelectedShip]);
            oShipTexture    = oShipContent.Load<Texture2D>(shipTextures[iSelectedShip]);

            if (iSelectedShip < 3){
                oReflectionTexture1 = oShipContent.Load<TextureCube>("Content/Textures/p1_reflection_cubemap");
                oReflectionTexture2 = (TextureCube)null;
            }else{
                oReflectionTexture1 = oShipContent.Load<TextureCube>("Content/Textures/p2_reflection_cubemap1");
                oReflectionTexture2 = oShipContent.Load<TextureCube>("Content/Textures/p2_reflection_cubemap2");
            }
        }

        /// <summary>
        /// Set the shader variables
        /// </summary>
        ///
        private void SetupEffect()
        {
            ambientParam.SetValue(new Vector4(1,1,1,1));
            directionalDirectionParam.SetValue(new Vector4(0,10,0,1));
            directionalColorParam.SetValue(new Vector4(.4f, .4f, .8f, 1.0f));
        }


        /// <summary>
        /// Unload graphics content used by the game.
        /// </summary>
        public override void UnloadGraphicsContent(bool unloadAllContent)
        {
            if (unloadAllContent)
            {
                if (oShipContent != null) oShipContent.Unload();
                content.Unload();
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
        }

        public override void Draw(GameTime gameTime)
        {
//#if DEBUG
            PerformanceEater last = PerformanceMeter.Instance.PerfomanceEaterChange(PerformanceEater.Framework);
//#endif

            base.Draw(gameTime);

            Matrix matWorld = matScale * matRotZ * matRotY;
            worldParam.SetValue(matWorld);
            worldViewProjectionParam.SetValue(matWorld * matView * matProj);         

            oEffect.Begin();
                oEffect.Techniques[0].Passes[0].Begin();

                    int id = 0;
                    foreach (ModelMesh modelMesh in oShipModel.Meshes)
                    {
                        foreach (ModelMeshPart meshPart in modelMesh.MeshParts)
                        {
                            if (meshPart.PrimitiveCount > 0)
                            {
                                if ((shipMaterials[iSelectedShip][id]) != Color.White)  skinTextureParam.SetValue(oBlackTexture);
                                else                                                    skinTextureParam.SetValue(oShipTexture);

                                reflectionTextureParam.SetValue(shipPartUsesReflection[iSelectedShip][id] ? oReflectionTexture2 : oReflectionTexture1);
                                materialParam.SetValue((shipMaterials[iSelectedShip][id]).ToVector4());
                                oEffect.CommitChanges();

                                ScreenManager.GraphicsDevice.VertexDeclaration = meshPart.VertexDeclaration;
                                ScreenManager.GraphicsDevice.Vertices[0].SetSource(modelMesh.VertexBuffer, meshPart.StreamOffset, meshPart.VertexStride);
                                ScreenManager.GraphicsDevice.Indices = modelMesh.IndexBuffer;

                                ScreenManager.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, meshPart.BaseVertex, 0, meshPart.NumVertices, meshPart.StartIndex, meshPart.PrimitiveCount);
                            }
                            id++;
                        }
                    }

                oEffect.Techniques[0].Passes[0].End();
            oEffect.End();


            //Draw the Text
            string message = "<- Select Your Ship: " 
                             + shipModels[iSelectedShip].Substring(shipModels[iSelectedShip].IndexOf('_')+1).ToUpper()
                             + " (P"
                             + shipModels[iSelectedShip].Substring(shipModels[iSelectedShip].IndexOf('p') + 1, 1)
                             + ") ->";

            // Center the text in the viewport.
            Vector2 textPosition = new Vector2((ScreenManager.GraphicsDevice.Viewport.Width - ScreenManager.Font.MeasureString(message).X) / 2, 30);
            Color color = new Color(32, 32, 32, TransitionAlpha);

            // Draw the text.
            ScreenManager.SpriteBatch.Begin();

            ScreenManager.SpriteBatch.DrawString(ScreenManager.Font, message,
                                                 textPosition, color);

            ScreenManager.SpriteBatch.End();

//#if DEBUG
            PerformanceMeter.Instance.PerfomanceEaterChange(last);
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
        public override void HandleInput(InputState input)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            if (input.PauseGame)
            {
                const string message = "Are you sure you want to exit the program?";

                MessageBoxScreen messageBox = new MessageBoxScreen(message);

                messageBox.Accepted += ExitMessageBoxAccepted;
                
                ScreenManager.AddScreen(messageBox);
            }


            if (input.MenuRight || input.MenuLeft) 
            {
                if (input.MenuLeft)  iSelectedShip = (iSelectedShip + iNumShips - 1) % iNumShips;
                if (input.MenuRight) iSelectedShip = (iSelectedShip + 1) % iNumShips;
                LoadShip(iSelectedShip); 
            }

            if (input.MenuCancel)
            {
                Sound.PlayCue(Sounds.MenuBack);
                Sound.Stop(themeCue);
                ScreenManager.AddScreen(new BackgroundScreen());
                ScreenManager.AddScreen(new MainMenuScreen());
            }

            if (input.MenuSelect)
            {
                Sound.PlayCue(Sounds.MenuConfirm);
                Sound.Stop(themeCue);
                LoadingScreen.Load(ScreenManager, LoadSelectTrackScreen, true);
                ScreenManager.RemoveScreen(this);
            }
        }

        void LoadSelectTrackScreen(object sender, EventArgs e)
        {
            ScreenManager.AddScreen(new SelectTrackScreen());
        }

        #endregion
    }
}
