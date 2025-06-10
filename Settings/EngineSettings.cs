using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.IO;
using System.Xml.Serialization;
using System.Xml;
using PraktWS0708.Entities;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using PraktWS0708.Rendering;

namespace PraktWS0708.Settings
{
    public class EngineSettings
    {
#if DEBUG
		public bool Fullscreen = false;
        public bool AllowUserResizing = true;
#else
        public bool Fullscreen = true;
        public bool AllowUserResizing = false;
#endif
        public struct ScreenRes
        {
            public int Width;
            public int Height;
            public ScreenRes(int width, int height)
            {
                this.Height = height;
                this.Width = width;
            }

        }

#if DEBUG
        public ScreenRes ScreenResolution = new ScreenRes(1024, 768);
#else
        public ScreenRes ScreenResolution = new ScreenRes(1024, 768);
        //public ScreenRes ScreenResolution = new ScreenRes(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width, GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height);
#endif

        public int LightMapResolutionX = 1024;
        public int LightMapResolutionY = 32;

        public bool Multisampling = true;

        public int ShadowMapResolution = 1024;

        public float MiniMapSizeRatio = 0.4f;

        public bool MotionBlur = true;

        public bool Bloom = true;

        public int shadowmap_splits=3;

        public float FlareGlowSize = 400f;

        public bool LensFlare = true;

        // Controls how bright a pixel needs to be before it will bloom.
        // Zero makes everything bloom equally, while higher values select
        // only brighter colors. Somewhere between 0.25 and 0.5 is good.
        public float BloomThreshold = 0.4f;

        // Controls how much blurring is applied to the bloom image.
        // The typical range is from 1 up to 10 or so.
        public float BlurAmount = 4f;

        // Controls the amount of the bloom and base images that
        // will be mixed into the final scene.
        public float BloomIntensity = 1.25f;
        public float BaseIntensity = 1f;

        // Independently control the color saturation of the bloom and
        // base images. Zero is totally desaturated, 1.0 leaves saturation
        // unchanged, while higher values increase the saturation level.
        public float BloomSaturation = 2f;
        public float BaseSaturation = 1f;

        public int EnvironmentMapResolution = 64;

        public RenderManager.EffectQuality MaxEffectQuality = RenderManager.EffectQuality.High;
        public float MediumQualityDistanceSQThreshold = 50.0f;
        public float LowQualityDistanceSQThreshold = 200.0f;
        public bool playMusic = false;

        public bool RenderParticleEngine = true;

        public bool ShieldDistortion = true;

        public float ParticleBlurAmount = 6f;

        public string ModelDirectory = "./Content/Models/";
        public string TextureDirectory = "./Content/Textures/";
        public string ShaderDirectory = "./Content/Shader/";
        public string Directory_Fonts = "./Content/Fonts/";
        public string Directory_Audio = "./Content/Audio/";

        //Physics
        public int PhysicsUpdateStepTime = 16; // must be in [1, 16]

        // Physics & Particles
        public bool ParticlesEnabled = true;
        public int MaxParticleCount = 2000;    // maximum primitive count for each particle system
        public int ParticleMapWidth = 64;
        public int ParticleMapHeight = 64;


        #region Load/Save code
        /// <summary>
        /// Saves the current settings
        /// </summary>
        /// <param name="filename">The filename to save to</param>
        public void Save(string filename)
        {
            Stream stream = File.Create(filename);

            XmlSerializer serializer = new XmlSerializer(typeof(EngineSettings));
            serializer.Serialize(stream, this);
            stream.Close();
        }

        /// <summary>
        /// Loads settings from a file
        /// </summary>
        /// <param name="filename">The filename to load</param>
        public static EngineSettings Load(string filename)
        {
            EngineSettings md;
            Stream stream = File.OpenRead(filename);
            XmlSerializer serializer = new XmlSerializer(typeof(EngineSettings));
            md = (EngineSettings)serializer.Deserialize(stream);

            // FIXME: XML loading did not work for me. Set the following #ifdef to
            // true to reduce rendering quality
#if false
            md.MaxEffectQuality = RenderManager.EffectQuality.Low;
            md.Bloom = false;
            md.Multisampling = false;
            md.ShadowMapResolution = 256;
            md.EnvironmentMapResolution = 64;
#endif

            return md;

        }
        #endregion
    }
}
