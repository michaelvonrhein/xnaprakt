using System;
using System.Threading;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using PraktWS0708.Settings;
using PraktWS0708.Entities;

namespace PraktWS0708.HUD
{

    public struct SpritePosition
    {
        public Texture2D sprite;
        public int position_x;
        public int position_y;
        public int size_x;
        public int size_y;
        public Color color;
        public Rectangle window;

        public SpritePosition(Texture2D sprite, int position_x, int position_y, int size_x, int size_y, Color color)
        {
            this.sprite = sprite;
            this.position_x = position_x;
            this.position_y = position_y;
            this.size_x = size_x;
            this.size_y = size_y;
            this.color = color;
            window = new Rectangle(position_x, position_y, size_x, size_y);
        }

        public void setPosX(int pos)
        {
            position_x = pos;
            window = new Rectangle(position_x, position_y, size_x, size_y);
        }

        public void setPosY(int pos)
        {
            position_y = pos;
            window = new Rectangle(position_x, position_y, size_x, size_y);
        }

        public void setColor(Color color)
        {
            this.color = color;
        }
    }

    class HUD
    {
        public enum ShipType
        {
            DIRTYNUTSHELL,
            DELOREAN,
            AIRTRAIN,
            SPACESHUTTLE,
            SCHAKIRA,
            HYPERSONIC,
            HOVERCAR,
            ENTERPRISE,
            TITANIC,
            FUTURAMA
        };

        public enum PickupType
        {
            BANANA,
            HEALTHPACK,
            BOMB,
            SHIELD,
            TURBO,
            ROCKET
        };

        private int viewport_x;
        private int viewport_y;
        
        private SpriteBatch spriteBatch;
        private ScreenManager screenManager;

        private SpritePosition cockpit;
        private SpritePosition miniMap;
        private SpritePosition damageBar;
        private SpritePosition damageText;
        private SpritePosition wrongway;
        private SpritePosition firstPlace;
        private SpritePosition secondPlace;
        private SpritePosition thirdPlace;
        private SpritePosition lastPlace;
        private SpritePosition fastLap;
        private SpritePosition finalLap;
        private SpritePosition go;
        private SpritePosition eins;
        private SpritePosition zwei;
        private SpritePosition drei;
        private SpritePosition gameOver;
        private SpritePosition[] speedo = new SpritePosition[18];
        private SpritePosition[] positionTable;
        private SpritePosition[] ships;
        private SpritePosition[] pickup;
        
        private RenderTarget2D miniMapTarget;
        private RenderTarget2D miniMapTargetOld;

        private SpriteFont font; 
        //private SpriteFont digitalfont;
        //private SpriteFont digitalfontsmall;

        private UInt32 startTime = 0;
        private int checkPointCount;
        private UInt32[] checkPointBest;
        private UInt32[] checkPointCur;

        private int lapCount = 1;
        private int lapCountMax = 0;
        private bool lapCountVisible = true;

        private int position = 1;
        private int driverCount = 1;

        private int speedoIndex = 0;
        private int speedoSpriteCount = 18;
        private float speedMax;
        private float speedCurr = 0;
        private float speedStep;

        private string currentLapTime = "";
        private string lastLapTime = "";

        private int positionTableSpriteCount = 10;
        private bool[] positionTableVisible;
        private int positionCounter = 0;

        private int pickupSpriteCount = 6;
        private bool[] pickupVisible;

        private bool wrongwayVisible = false;
        private bool firstPlaceVisible = false;
        private bool secondPlaceVisible = false;
        private bool thirdPlaceVisible = false;
        private bool lastPlaceVisible = false;
        private bool fastLapVisible = false;
        private bool finalLapVisible = false;
        private bool goVisible = false;
        private bool dreiVisible = false;
        private bool zweiVisible = false;
        private bool einsVisible = false;
        private bool gameOverVisible = false;

        private ContentManager content;

        public int BlinkingSpeed = 500;
   
        public HUD(ContentManager content, ScreenManager screenManager)
        {
            viewport_x = screenManager.GraphicsDevice.Viewport.Width;
            viewport_y = screenManager.GraphicsDevice.Viewport.Height;

            this.screenManager = screenManager;
            this.content = content;
            spriteBatch = new SpriteBatch(screenManager.GraphicsDevice);

            for (int i = 0; i < speedoSpriteCount; i++)
            {
                speedo[i] = new SpritePosition(content.Load<Texture2D>("Content/Textures/HUD/tacho_" + i.ToString()), 10, viewport_y - 120, 270, 120, Color.White);
            }

            pickup = new SpritePosition[pickupSpriteCount];
            pickupVisible = new bool[pickupSpriteCount];

            pickup[0] = new SpritePosition(content.Load<Texture2D>("Content/Textures/HUD/pickup_0"), viewport_x - (int)(viewport_x * 0.1464f), viewport_y - (int)(viewport_y * 0.5859f), (int)(viewport_x * 0.0488f), (int)(viewport_y * 0.0651f), Color.White);
            pickup[1] = new SpritePosition(content.Load<Texture2D>("Content/Textures/HUD/pickup_1"), viewport_x - (int)(viewport_x * 0.1464f), viewport_y - (int)(viewport_y * 0.5f), (int)(viewport_x * 0.0488f), (int)(viewport_y * 0.0651f), Color.White);
            pickup[2] = new SpritePosition(content.Load<Texture2D>("Content/Textures/HUD/pickup_2"), viewport_x - (int)(viewport_x * 0.1464f), viewport_y - (int)(viewport_y * 0.5859f), (int)(viewport_x * 0.0488f), (int)(viewport_y * 0.0651f), Color.White);
            pickup[3] = new SpritePosition(content.Load<Texture2D>("Content/Textures/HUD/pickup_3"), viewport_x - (int)(viewport_x * 0.07f), viewport_y - (int)(viewport_y * 0.5859f), (int)(viewport_x * 0.0488f), (int)(viewport_y * 0.0651f), Color.White);
            pickup[4] = new SpritePosition(content.Load<Texture2D>("Content/Textures/HUD/pickup_4"), viewport_x - (int)(viewport_x * 0.07f), viewport_y - (int)(viewport_y * 0.5f), (int)(viewport_x * 0.0488f), (int)(viewport_y * 0.0651f), Color.White);
            pickup[5] = new SpritePosition(content.Load<Texture2D>("Content/Textures/HUD/pickup_5"), viewport_x - (int)(viewport_x * 0.11f), viewport_y - (int)(viewport_y * 0.54f), (int)(viewport_x * 0.0488f), (int)(viewport_y * 0.0651f), Color.White);

            for (int i = 0; i < pickupSpriteCount; i++)
            {            
                pickupVisible[i] = false;
            }

            cockpit = new SpritePosition(content.Load<Texture2D>("Content/Textures/HUD/cockpit3"), 0, 0, viewport_x, viewport_y, Color.White);
            //fuelBar = new SpritePosition(content.Load<Texture2D>("Content/Textures/fuelbar"), 0, 10, 210, 20, Color.White);
            damageBar = new SpritePosition(content.Load<Texture2D>("Content/Textures/HUD/metal2"), 0, (int)(viewport_y * 0.016667f), (int)(viewport_x * 0.2625f), (int)(viewport_y * 0.0333f), Color.White);
            damageText = new SpritePosition(content.Load<Texture2D>("Content/Textures/HUD/damagetext"), 0, (int)(viewport_y * 0.016667f), (int)(viewport_x * 0.2625f), (int)(viewport_y * 0.0333f), Color.White);

            wrongway = new SpritePosition(content.Load<Texture2D>("Content/Textures/HUD/wrongway"), (int)(viewport_x * 0.25f), (int)(viewport_y * 0.333f), (int)(viewport_x * 0.5f), (int)(viewport_y * 0.333f), new Color(255, 255, 255, 128)/*Color.White*/);
            firstPlace = new SpritePosition(content.Load<Texture2D>("Content/Textures/HUD/1stplace"), (int)(viewport_x * 0.25f), (int)(viewport_y * 0.333f), (int)(viewport_x * 0.5f), (int)(viewport_y * 0.333f), Color.White);
            secondPlace = new SpritePosition(content.Load<Texture2D>("Content/Textures/HUD/2ndplace"), (int)(viewport_x * 0.25f), (int)(viewport_y * 0.333f), (int)(viewport_x * 0.5f), (int)(viewport_y * 0.333f), Color.White);
            thirdPlace = new SpritePosition(content.Load<Texture2D>("Content/Textures/HUD/3rdplace"), (int)(viewport_x * 0.25f), (int)(viewport_y * 0.333f), (int)(viewport_x * 0.5f), (int)(viewport_y * 0.333f), Color.White);
            lastPlace = new SpritePosition(content.Load<Texture2D>("Content/Textures/HUD/lastplace"), (int)(viewport_x * 0.25f), (int)(viewport_y * 0.333f), (int)(viewport_x * 0.5f), (int)(viewport_y * 0.333f), Color.White);
            fastLap = new SpritePosition(content.Load<Texture2D>("Content/Textures/HUD/fastlap"), (int)(viewport_x * 0.25f), (int)(viewport_y * 0.333f), (int)(viewport_x * 0.5f), (int)(viewport_y * 0.333f), Color.White);
            finalLap = new SpritePosition(content.Load<Texture2D>("Content/Textures/HUD/finallap"), (int)(viewport_x * 0.25f), (int)(viewport_y * 0.333f), (int)(viewport_x * 0.5f), (int)(viewport_y * 0.333f), Color.White);
            go = new SpritePosition(content.Load<Texture2D>("Content/Textures/HUD/go"), (int)(viewport_x * 0.416f), (int)(viewport_y * 0.15f), (int)(viewport_x * 0.1669f), (int)(viewport_y * 0.7343f), Color.White);
            eins = new SpritePosition(content.Load<Texture2D>("Content/Textures/HUD/eins"), (int)(viewport_x * 0.416f), (int)(viewport_y * 0.15f), (int)(viewport_x * 0.1669f), (int)(viewport_y * 0.7343f), Color.White);
            zwei = new SpritePosition(content.Load<Texture2D>("Content/Textures/HUD/zwei"), (int)(viewport_x * 0.416f), (int)(viewport_y * 0.15f), (int)(viewport_x * 0.1669f), (int)(viewport_y * 0.7343f), Color.White);
            drei = new SpritePosition(content.Load<Texture2D>("Content/Textures/HUD/drei"), (int)(viewport_x * 0.416f), (int)(viewport_y * 0.15f), (int)(viewport_x * 0.1669f), (int)(viewport_y * 0.7343f), Color.White);
            gameOver = new SpritePosition(content.Load<Texture2D>("Content/Textures/HUD/gameover"), (int)(viewport_x * 0.25f), (int)(viewport_y * 0.333f), (int)(viewport_x * 0.5f), (int)(viewport_y * 0.333f), Color.White);

            miniMap = new SpritePosition(null, viewport_x - 100, viewport_y - 100, 100, 100, Color.White);
            
            miniMapTarget = new RenderTarget2D(screenManager.GraphicsDevice, 256, 256, 0, SurfaceFormat.Color);

            miniMapTarget = new RenderTarget2D(screenManager.GraphicsDevice, 256, 256, 1, SurfaceFormat.Color, screenManager.GraphicsDevice.PresentationParameters.MultiSampleType, screenManager.GraphicsDevice.PresentationParameters.MultiSampleQuality);

            font = content.Load<SpriteFont>("Content/Fonts/menufont");
          //  digitalfont = content.Load<SpriteFont>("Content/Fonts/digitalfont");
          //  digitalfontsmall = content.Load<SpriteFont>("Content/Fonts/digitalfontsmall");

            ships = new SpritePosition[positionTableSpriteCount];
            positionTable = new SpritePosition[positionTableSpriteCount];
            positionTableVisible = new bool[positionTableSpriteCount];

            for (int i = 0; i < positionTableSpriteCount; i++)
            {
                positionTable[i] = new SpritePosition(content.Load<Texture2D>("Content/Textures/HUD/position_" + i.ToString()), 0, (int)(viewport_y * (i + 1) * 0.08333f), (int)(viewport_x * 0.125f), (int)(viewport_y * 0.0833f), Color.White);
                positionTableVisible[i] = false;
            }
        }

        public void draw(GameTime gameTime)
        {
            viewport_x = Settings.Configuration.EngineSettings.ScreenResolution.Width;
            viewport_y = Settings.Configuration.EngineSettings.ScreenResolution.Height;

            spriteBatch.Begin();
                spriteBatch.Draw(cockpit.sprite, cockpit.window, cockpit.color);                            
                //spriteBatch.Draw(fuelBar.sprite, fuelBar.window, fuelBar.color);
                spriteBatch.Draw(damageBar.sprite, damageBar.window, damageBar.color);
                spriteBatch.Draw(damageText.sprite, damageText.window, damageText.color);  

                if (wrongwayVisible && gameTime.TotalGameTime.TotalMilliseconds % BlinkingSpeed >= BlinkingSpeed / 2)
                    spriteBatch.Draw(wrongway.sprite, wrongway.window, wrongway.color);

                if (firstPlaceVisible)
                    spriteBatch.Draw(firstPlace.sprite, firstPlace.window, firstPlace.color);

                if (secondPlaceVisible)
                    spriteBatch.Draw(secondPlace.sprite, secondPlace.window, secondPlace.color);

                if (thirdPlaceVisible)
                    spriteBatch.Draw(thirdPlace.sprite, thirdPlace.window, thirdPlace.color);

                if (lastPlaceVisible)
                    spriteBatch.Draw(lastPlace.sprite, lastPlace.window, lastPlace.color);

                if (fastLapVisible)
                    spriteBatch.Draw(fastLap.sprite, fastLap.window, fastLap.color);

                if (finalLapVisible)
                    spriteBatch.Draw(finalLap.sprite, finalLap.window, finalLap.color);

                if (goVisible)
                    spriteBatch.Draw(go.sprite, go.window, go.color);
                
                if (einsVisible)
                    spriteBatch.Draw(eins.sprite, eins.window, eins.color);

                if (zweiVisible)
                    spriteBatch.Draw(zwei.sprite, zwei.window, zwei.color);

                if (dreiVisible)
                    spriteBatch.Draw(drei.sprite, drei.window, drei.color);

                if (gameOverVisible)
                    spriteBatch.Draw(gameOver.sprite, gameOver.window, gameOver.color);

                //spriteBatch.Draw(miniMap.sprite, miniMap.window, miniMap.color);
                spriteBatch.Draw(speedo[speedoIndex].sprite, speedo[speedoIndex].window, speedo[speedoIndex].color);

                if (lapCountVisible)
                {
                    spriteBatch.DrawString(font, position.ToString() + " / " + driverCount.ToString(), new Vector2((viewport_x / 2) - 24, 5), Color.Black);
                    spriteBatch.DrawString(font, position.ToString() + " / " + driverCount.ToString(), new Vector2((viewport_x / 2) - 25, 3), Color.SpringGreen);

                    spriteBatch.DrawString(font, lapCount.ToString() + " / " + lapCountMax.ToString(), new Vector2(viewport_x - 119, (viewport_y * 0.08315f) - 1), Color.Black);
                    spriteBatch.DrawString(font, lapCount.ToString() + " / " + lapCountMax.ToString(), new Vector2(viewport_x - 120, (viewport_y * 0.08315f)), Color.SpringGreen);
                }
                    
                //long timeElapsed = Convert.ToInt32(gameTime.TotalGameTime.Minutes) * 60000 + Convert.ToInt32(gameTime.TotalGameTime.Seconds) * 1000 + Convert.ToUInt32(gameTime.TotalGameTime.Milliseconds) - startTime;
                //double minutesEl = (timeElapsed / 60000) % 60;
                //Math.Floor(minutesEl);
                //double secondsEl = (timeElapsed / 1000) % 60;
                //Math.Floor(secondsEl);

                spriteBatch.DrawString(font, (lastLapTime), new Vector2((viewport_x * 0.8375f), (viewport_y * 0.216667f)), Color.SpringGreen);
                spriteBatch.DrawString(font, (currentLapTime), new Vector2((viewport_x * 0.8375f), (viewport_y * 0.33333f)), Color.SpringGreen);
                spriteBatch.DrawString(font, (int)(speedCurr * 22500.0f) + " km/h", new Vector2((viewport_x * 0.125f), (viewport_y * 0.91667f)), Color.SpringGreen);

                for (int i = 0; i < pickupSpriteCount; i++)
                {
                    if (pickupVisible[i])
                    {
                        spriteBatch.Draw(pickup[i].sprite, pickup[i].window, pickup[i].color);
                    }
                }

                for (int i = 0; i < positionTableSpriteCount; i++)
                {
                    if (positionTableVisible[i])
                    {
                        spriteBatch.Draw(positionTable[i].sprite, positionTable[i].window, positionTable[i].color);
                        spriteBatch.Draw(ships[i].sprite, ships[i].window, ships[i].color);
                    }
                
                }
#if DEBUG
                Vector2 vPosition = new Vector2(Settings.Configuration.EngineSettings.ScreenResolution.Width * 0.01f, Settings.Configuration.EngineSettings.ScreenResolution.Height * 0.78f);
                string message = string.Format("Position: ({0:0.0}, {1:0.0}, {2:0.0}) ", Entities.World.Instance.PlayersShip.Position.X, Entities.World.Instance.PlayersShip.Position.Y, Entities.World.Instance.PlayersShip.Position.Z);
                spriteBatch.DrawString(font, message, vPosition, Color.WhiteSmoke, 0.0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0);
#endif
            spriteBatch.End();
        }

        public void drawMiniMapBegin()
        {
            miniMapTargetOld = (RenderTarget2D)screenManager.GraphicsDevice.GetRenderTarget(0);
            screenManager.GraphicsDevice.SetRenderTarget(0, miniMapTarget);
        }

        public void drawMiniMapEnd()
        {
            screenManager.GraphicsDevice.ResolveRenderTarget(0);
            screenManager.GraphicsDevice.SetRenderTarget(0, miniMapTargetOld);

            miniMap.sprite = miniMapTarget.GetTexture();
        }

        public void setCurrentLapTime(string time)
        {
            currentLapTime = time;
        }

        public void setLastLapTime(string time)
        {
            lastLapTime = time;
        }

        public void checkPoint(int checkPointId, GameTime gameTime)
        {
            checkPointCur[checkPointId] = startTime - Convert.ToUInt32(gameTime.TotalGameTime.Seconds) * 1000 + Convert.ToUInt32(gameTime.TotalGameTime.Milliseconds);
            float delta = checkPointCur[checkPointId] - checkPointBest[checkPointId];
        }

        public void setCheckPointCount(int count)
        {
            checkPointCount = count;
            checkPointBest = new UInt32[count];
            checkPointCur = new UInt32[count];
        }

        public void setLapCount(int lapCount)
        {
            this.lapCount = lapCount;
        }

        public void setLapCountMax(int max)
        {
            lapCountMax = max;
        }

        public void setDriverCount(int count)
        {
            driverCount = count;
        }

        public void setPosition(int pos)
        {
            position = pos;
        }

        /*public void setFuelBar(float percentage)
        {
            const int delta = 500;

            if(percentage > 100f)
            {
                percentage = 100f;
            }
            if(percentage < 0f)
            {
                percentage = 0f;
            }

            fuelBar.setPosX(0 - (int)(delta * percentage / 100f));
            fuelBar.setColor(new Color((byte)(255 * (1 - percentage / 100f)), (byte)(255 * percentage / 100f), 0));
        }*/

        public void setDamageBar(float percentage)
        {
            int delta = (int)(viewport_x * 0.2625f);

            if (percentage > 100f)
            {
                percentage = 100f;
            }
            if (percentage < 0f)
            {
                percentage = 0f;
            }

            damageBar.setPosX(0 - (int)(delta * percentage / 100f));
            damageBar.setColor(new Color((byte)(255 * (1 - percentage / 100f)), (byte)(255 * percentage / 100f), 0));
        }

        public void SetMaxSpeed(float speed)
        {
            speedMax = speed;
            speedStep = speed / (speedoSpriteCount - 2);
        }

        public void SetCurrentSpeed(float speed)
        {
            if (speed < 0f)
            {
                speedoIndex = 0;
            }
            else if ((speed / speedStep) < (speedoSpriteCount))
            {
                speedoIndex = (int)(speed / speedStep);
            }
            else
            {
                speedoIndex = speedoSpriteCount - 1;
            }
            speedCurr = speed;
        }

        private void SetPosTable(int position, ShipType ship, bool visible)
        {
            if ((position > 0) || (position < positionTableSpriteCount))
            {
                positionTableVisible[position - 1] = visible;
                ships[position - 1] = new SpritePosition(content.Load<Texture2D>("Content/Textures/HUD/ship_" + (Convert.ToInt32(ship)).ToString()), (int)(viewport_x * 0.05f), (int)(viewport_y * position * 0.08333f), (int)(viewport_x * 0.075f), (int)(viewport_y * 0.0833f), Color.White);
            }
        }

        public void SetPositionTable(BaseEntity be)
        {
            
            if (be.LogicPlugin.name == "DirtyNutShell")
            {
                SetPosTable(++positionCounter, ShipType.DIRTYNUTSHELL, true);
            }
            else if (be.LogicPlugin.name == "AirTrain")
            {
                SetPosTable(++positionCounter, ShipType.AIRTRAIN, true);
            }
            else if (be.LogicPlugin.name == "Delorean")
            {
                SetPosTable(++positionCounter, ShipType.DELOREAN, true);
            }
            else if (be.LogicPlugin.name == "Enterprise")
            {
                SetPosTable(++positionCounter, ShipType.ENTERPRISE, true);
            }
            else if (be.LogicPlugin.name == "Futurama")
            {
                SetPosTable(++positionCounter, ShipType.FUTURAMA, true);
            }
            else if (be.LogicPlugin.name == "HoverCar")
            {
                SetPosTable(++positionCounter, ShipType.HOVERCAR, true);
            }
            else if (be.LogicPlugin.name == "Hypersonic")
            {
                SetPosTable(++positionCounter, ShipType.HYPERSONIC, true);
            }
            else if (be.LogicPlugin.name == "Schakira")
            {
                SetPosTable(++positionCounter, ShipType.SCHAKIRA, true);
            }
            else if (be.LogicPlugin.name == "SpaceShuttle")
            {
                SetPosTable(++positionCounter, ShipType.SPACESHUTTLE, true);
            }
            else if (be.LogicPlugin.name == "Titanic")
            {
                SetPosTable(++positionCounter, ShipType.TITANIC, true);
            }
        }

        public void ResetPositionTable()
        {
            for (int i = 0; i < positionTable.Length; i++)
            {                
                positionTableVisible[i] = false;
            }

            positionCounter = 0;
        }





        public void SetPickUp(PickupType type, bool visible)
        {
            pickupVisible[(Convert.ToInt32(type))] = visible;
        }

        public void ClearPositionTable()
        {
            for (int i = 0; i < positionTableSpriteCount; i++)
            {
                positionTableVisible[i] = false;
            }
        }

        public void ClearInfo()
        {
            firstPlaceVisible = false;
            secondPlaceVisible = false;
            thirdPlaceVisible = false;
            lastPlaceVisible = false;
            wrongwayVisible = false;
            fastLapVisible = false;
            finalLapVisible = false;
            goVisible = false;
            gameOverVisible = false;
        }

        public void FirstPlace(bool visible)
        {
            firstPlaceVisible = visible;
        }

        public void SecondPlace(bool visible)
        {
            secondPlaceVisible = visible;
        }

        public void ThirdPlace(bool visible)
        {
            thirdPlaceVisible = visible;
        }

        public void LastPlace(bool visible)
        {
            lastPlaceVisible = visible;
        }

        public void Wrongway(bool visible)
        {
            wrongwayVisible = visible;
        }

        public void FastLap(bool visible)
        {
            fastLapVisible = visible;
        }

        public void FinalLap(bool visible)
        {
            finalLapVisible = visible;
        }

        public void Go(bool visible)
        {
            goVisible = visible;
        }

        public void Eins(bool visible)
        {
            einsVisible = visible;
        }

        public void Zwei(bool visible)
        {
            zweiVisible = visible;
        }

        public void Drei(bool visible)
        {
            dreiVisible = visible;
        }

        public void GameOver(bool visible)
        {
            gameOverVisible = visible;
        }

        public void LapCountVisible(bool visible)
        {
            lapCountVisible = visible;
        }

    }
}
