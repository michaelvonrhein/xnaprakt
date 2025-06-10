using System;
using System.Collections.Generic;
using System.Text;
using PraktWS0708.Settings;
using Microsoft.Xna.Framework;
using PraktWS0708.Entities;

namespace PraktWS0708
{
    class OptionsScreen : MenuScreen
    {

        /// <summary>
        /// Constructor populates the menu with empty strings: the real values
        /// are filled in by the Update method to reflect the changing settings.
        /// </summary>
        public OptionsScreen()
        {
            MenuEntries.Add(string.Empty);
            MenuEntries.Add(string.Empty);
            MenuEntries.Add(string.Empty);
            MenuEntries.Add(string.Empty);
            MenuEntries.Add(string.Empty);
            MenuEntries.Add(string.Empty);
            MenuEntries.Add(string.Empty);
            MenuEntries.Add(string.Empty);
            MenuEntries.Add(string.Empty);
            MenuEntries.Add(string.Empty);
            MenuEntries.Add(string.Empty);
            MenuEntries.Add("Back");
        }

        /// <summary>
        /// Updates the options screen, filling in the latest values for the menu text.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            MenuEntries[0] = "Multisampling: " + Configuration.EngineSettings.Multisampling.ToString();
            MenuEntries[1] = "Shadow map resolution: " + Configuration.EngineSettings.ShadowMapResolution.ToString();
            MenuEntries[2] = "Environment map resolution: " + Configuration.EngineSettings.EnvironmentMapResolution.ToString();
            MenuEntries[3] = "Motion blur: " + Configuration.EngineSettings.MotionBlur.ToString();
            MenuEntries[4] = "Bloom: " + Configuration.EngineSettings.Bloom.ToString();
            MenuEntries[5] = "Render Particle Engine: " + Configuration.EngineSettings.RenderParticleEngine.ToString();
            MenuEntries[6] = "Render Shield Effect: " + Configuration.EngineSettings.ShieldDistortion.ToString();
            MenuEntries[7] = "Lens Flare: " + Configuration.EngineSettings.LensFlare.ToString();
            MenuEntries[8] = "Music: " + Configuration.EngineSettings.playMusic.ToString();
            MenuEntries[9] = "Maximum Quality: " + Configuration.EngineSettings.MaxEffectQuality.ToString();
            MenuEntries[10] = "Difficulty: " + Configuration.difficulty.ToString();
        }


        /// <summary>
        /// Responds to user menu selections.
        /// </summary>
        protected override void OnSelectEntry(int entryIndex)
        {
            int res;
            switch (entryIndex)
            {
                case 0:
                    Configuration.EngineSettings.Multisampling = !Configuration.EngineSettings.Multisampling;
                    break;

                case 1:
                    res = Configuration.EngineSettings.ShadowMapResolution;
                    res /= 2;
                    if (res < 64)
                    {
                        res = 2048;
                    }
                    Configuration.EngineSettings.ShadowMapResolution = res;
                    break;

                case 2:
                    res = Configuration.EngineSettings.EnvironmentMapResolution;
                    res /= 2;
                    if (res < 64)
                    {
                        res = 1024;
                    }
                    Configuration.EngineSettings.EnvironmentMapResolution = res;
                    break;

                case 3:
                    Configuration.EngineSettings.MotionBlur = !Configuration.EngineSettings.MotionBlur;
                    break;

                case 4:
                    Configuration.EngineSettings.Bloom = !Configuration.EngineSettings.Bloom;
                    break;

                case 5:
                    Configuration.EngineSettings.RenderParticleEngine = !Configuration.EngineSettings.RenderParticleEngine;
                    break;

                case 6:
                    Configuration.EngineSettings.ShieldDistortion = !Configuration.EngineSettings.ShieldDistortion;
                    break;

                case 7:
                    Configuration.EngineSettings.LensFlare = !Configuration.EngineSettings.LensFlare;
                    break;

                case 8:
                    Configuration.EngineSettings.playMusic = !Configuration.EngineSettings.playMusic;
                    break;

                case 9:
                    if (Configuration.EngineSettings.MaxEffectQuality == PraktWS0708.Rendering.RenderManager.EffectQuality.Low)
                    {
                        Configuration.EngineSettings.MaxEffectQuality = PraktWS0708.Rendering.RenderManager.EffectQuality.Medium;
                    }
                    else if (Configuration.EngineSettings.MaxEffectQuality == PraktWS0708.Rendering.RenderManager.EffectQuality.Medium)
                    {
                        Configuration.EngineSettings.MaxEffectQuality = PraktWS0708.Rendering.RenderManager.EffectQuality.High;
                    }
                    else if (Configuration.EngineSettings.MaxEffectQuality == PraktWS0708.Rendering.RenderManager.EffectQuality.High)
                    {
                        Configuration.EngineSettings.MaxEffectQuality = PraktWS0708.Rendering.RenderManager.EffectQuality.Low;
                    }
                    break;

                case 10:
                    if (Configuration.difficulty == Configuration.Difficulty.Easy)
                        Configuration.difficulty = Configuration.Difficulty.Medium;
                    else if (Configuration.difficulty == Configuration.Difficulty.Medium)
                        Configuration.difficulty = Configuration.Difficulty.Hard;
                    else if (Configuration.difficulty == Configuration.Difficulty.Hard)
                        Configuration.difficulty = Configuration.Difficulty.Easy;
                    
                    break;

                default:
                    // Go back to the main menu.
                    Configuration.RaiseChanged();
                    ExitScreen();
                    break;
            }

            // Update only if level was started
            if (World.Instance.Track != null)
            {
                Rendering.RenderManager.Instance.Initialize();
            }
        }


        /// <summary>
        /// When the user cancels the options screen, go back to the main menu.
        /// </summary>
        protected override void OnCancel()
        {
            Configuration.RaiseChanged();
            ExitScreen();
        }
    }
}
