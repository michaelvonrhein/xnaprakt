#region Using Statements

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using PraktWS0708.Entities;
using PraktWS0708.Utils;
using PraktWS0708.Rendering;
using PraktWS0708.Rendering.Effects;
using PraktWS0708.Settings;

#endregion

namespace PraktWS0708.Particles
{
    public class ParticleSystemPlugin : Rendering.RenderingPlugin
    {
        override public float DistanceSQ
        {
            get { return distancesq; }
            set { distancesq = 0.0f; }
        }
        public ParticleSystemPlugin(BaseEntity entity, RenderingPluginData data, RenderingPluginType type)
            : base(entity, data, type)
        {
            // clear ...
        }

        public override void Draw()
        {
            if (Settings.Configuration.EngineSettings.ParticlesEnabled)
                base.Draw();
        }

        public override void DrawGeometry()
        {
            //clear, weil die Partikel in der MiniMap nicht gezeichnet werden sollen
        }
    }
}
