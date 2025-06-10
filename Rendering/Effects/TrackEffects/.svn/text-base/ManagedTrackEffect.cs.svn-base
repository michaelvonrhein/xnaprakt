using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using PraktWS0708.Entities;

namespace PraktWS0708.Rendering.Effects
{
    public abstract class ManagedTrackEffect
    {

        
        protected Effect effect;

        public ManagedTrackEffect(string shader)
        {
            effect = World.Instance.WorldContent.Load<Effect>(Settings.Configuration.EngineSettings.ShaderDirectory + shader);
        }

        public EffectPassCollection Passes
        {
            get { return effect.CurrentTechnique.Passes; }
        }

        internal void Begin(TrackMeshPart mesh)
        {
            effect.CurrentTechnique = effect.Techniques[(int)RenderManager.Instance.GetEffectQuality(0)];
            Update(mesh);
            effect.Begin();
        }

        public void End()
        {
            effect.End();
        }

        public abstract void Update(TrackMeshPart mesh);

      
    }
}
