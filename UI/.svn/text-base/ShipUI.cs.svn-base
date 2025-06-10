using System;
using System.Threading;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;

namespace PraktWS0708.ShipUI
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

    class ShipUI
    {
        private int viewport_x;
        private int viewport_y;

        private SpriteBatch spriteBatch;
        private ScreenManager screenManager;
        private SpritePosition[] speedo = new SpritePosition[6];
        private SpritePosition[] stars = new SpritePosition[6];
        private SpritePosition[] crowns = new SpritePosition[6];
        private SpritePosition[] hearts = new SpritePosition[6];
        private SpritePosition[] mass = new SpritePosition[6];

        private SpritePosition infobar;

        private SpriteFont font;
        private SpriteFont trackdescfont;

        private int speedoIndex = 0;
        private int starIndex = 0;
        private int crownIndex = 0;
        private int heartsIndex = 0;
        private int massIndex = 0;

        private bool visible = true;
        public bool trackScreen = false;
        private string description = "";
        private string editorsTime = "";
        private string dannerComment = "";
        private string difficulty = "";
   
        public ShipUI(ContentManager content, ScreenManager screenManager)
        {
            viewport_x = screenManager.GraphicsDevice.Viewport.Width;
            viewport_y = screenManager.GraphicsDevice.Viewport.Height;

            this.screenManager = screenManager;
            spriteBatch = new SpriteBatch(screenManager.GraphicsDevice);

            for (int i = 0; i < 6; i++)
            {
                speedo[i] = new SpritePosition(content.Load<Texture2D>("Content/Textures/UI/schneckerl_" + i.ToString()), (int)(viewport_x * 0.1f), (int)(viewport_y * 0.63936f), (int)(viewport_x * 0.13671875), (int)(viewport_y * 0.03125), Color.White);
                stars[i] = new SpritePosition(content.Load<Texture2D>("Content/Textures/UI/stars_" + i.ToString()), (int)(viewport_x * 0.1f), (int)(viewport_y * 0.88936f), (int)(viewport_x * 0.13671875), (int)(viewport_y * 0.03125), Color.White);
                crowns[i] = new SpritePosition(content.Load<Texture2D>("Content/Textures/UI/crowns_" + i.ToString()), (int)(viewport_x * 0.1f), (int)(viewport_y * 0.68936f), (int)(viewport_x * 0.13671875), (int)(viewport_y * 0.03125), Color.White);
                mass[i] = new SpritePosition(content.Load<Texture2D>("Content/Textures/UI/mass_" + i.ToString()), (int)(viewport_x * 0.1f), (int)(viewport_y * 0.73936f), (int)(viewport_x * 0.13671875), (int)(viewport_y * 0.03125), Color.White);
                hearts[i] = new SpritePosition(content.Load<Texture2D>("Content/Textures/UI/hearts_" + i.ToString()), (int)(viewport_x * 0.1f), (int)(viewport_y * 0.78936f), (int)(viewport_x * 0.13671875), (int)(viewport_y * 0.03125), Color.White);
            }
            
            infobar = new SpritePosition(content.Load<Texture2D>("Content/Textures/UI/UI_Back"), 0, 0, viewport_x, viewport_y, Color.White);

            font = content.Load<SpriteFont>("Content/Fonts/menufont");
            trackdescfont = content.Load<SpriteFont>("Content/Fonts/trackdescfont");

        }

        public void draw(GameTime gameTime)
        {
            
            spriteBatch.Begin();
                spriteBatch.Draw(infobar.sprite, infobar.window, infobar.color);
                if (visible)
                {
                    spriteBatch.DrawString(trackdescfont, "Accel.:", new Vector2((viewport_x * 0.0125f), (viewport_y * 0.645f)), Color.SpringGreen);
                    spriteBatch.DrawString(trackdescfont, "Steering:", new Vector2((viewport_x * 0.0125f), (viewport_y * 0.695f)), Color.SpringGreen);
                    spriteBatch.DrawString(trackdescfont, "Mass:", new Vector2((viewport_x * 0.0125f), (viewport_y * 0.745f)), Color.SpringGreen);
                    spriteBatch.DrawString(trackdescfont, "Health:", new Vector2((viewport_x * 0.0125f), (viewport_y * 0.795f)), Color.SpringGreen);
                    spriteBatch.DrawString(trackdescfont, "Overall:", new Vector2((viewport_x * 0.0125f), (viewport_y * 0.895f)), Color.SpringGreen);
                    spriteBatch.Draw(speedo[speedoIndex].sprite, speedo[speedoIndex].window, speedo[speedoIndex].color);
                    spriteBatch.Draw(crowns[crownIndex].sprite, crowns[crownIndex].window, crowns[crownIndex].color);
                    spriteBatch.Draw(stars[starIndex].sprite, stars[starIndex].window, stars[starIndex].color);
                    spriteBatch.Draw(mass[massIndex].sprite, mass[massIndex].window, mass[massIndex].color);
                    spriteBatch.Draw(hearts[heartsIndex].sprite, hearts[heartsIndex].window, hearts[heartsIndex].color);
                }
                if (!trackScreen)
                {
                    spriteBatch.DrawString(font, description, new Vector2((viewport_x * 0.48f), (viewport_y * 0.025f)), Color.SpringGreen);
                }
                else
                {
                    spriteBatch.DrawString(trackdescfont, description, new Vector2((viewport_x * 0.0125f), (viewport_y * 0.6125f)), Color.SpringGreen);
                    spriteBatch.DrawString(trackdescfont, "Editors Time:\n" + editorsTime 
                        + "\n\nDifficulty:\n" + difficulty
                        + "\n\nC. Danners Comment:\n" + dannerComment, new Vector2((viewport_x * 0.0125f), (viewport_y * 0.6425f)), Color.SpringGreen);
                }
            spriteBatch.End();
        }

        public void setSpeedDisplay(int index)
        {
            speedoIndex = index;
        }

        public void setStarDisplay(int index)
        {
            starIndex = index;
        }
        public void setCrownDisplay(int index)
        {
            crownIndex = index;
        }
        public void setHeartsDisplay(int index)
        {
            heartsIndex = index;
        }
        public void setMassDisplay(int index)
        {
            massIndex = index;
        }
        public void setSymbolVisibility(bool visible)
        {
            this.visible = visible;
        }

        public void setDescriptionText(string text)
        {
            description = text;
        }
        public void setEditorsTime(string text)
        {
            editorsTime = text;
        }
        public void setDannerComment(string text)
        {
            dannerComment = text;
        }
        public void setDifficultyText(string text)
        {
            difficulty = text;
        }
    }
}
