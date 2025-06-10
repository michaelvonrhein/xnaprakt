using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using PraktWS0708.Entities;
using Microsoft.Xna.Framework;

namespace PraktWS0708.Rendering.Effects
{
    public abstract class ManagedEffect
    {
        protected Effect effect;

        public Matrix Orientation = Matrix.Identity;

        //public virtual void GenerateEnvironmentMap() { }

        public ManagedEffect(string shader)
        {
            effect = World.Instance.WorldContent.Load<Effect>(Settings.Configuration.EngineSettings.ShaderDirectory + shader);
        }

        public EffectPassCollection Passes
        {
            get { return effect.CurrentTechnique.Passes; }
        }

        internal void Begin(BaseMeshPart mesh)
        {
            effect.CurrentTechnique = effect.Techniques[(int)RenderManager.Instance.GetEffectQuality(mesh.Entity.RenderingPlugin.DistanceSQ)];
            Update(mesh);
            effect.Begin();
        }

        public void End()
        {
            effect.End();
        }

        public abstract void Update(BaseMeshPart mesh);
    }
}
