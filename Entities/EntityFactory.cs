using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PraktWS0708.Physics;
using PraktWS0708.Logic;
using PraktWS0708.Input;
using PraktWS0708.Settings;
using PraktWS0708.Rendering;

namespace PraktWS0708.Entities
{
    public class EntityFactory
    {
        public static BaseEntity BuildEntity(string entityType)
        {
            ModelDescription md;
            if (!Configuration.ModelDescriptions.ModelDescriptionForName(entityType, out md))
                throw new ArgumentException("Entity type not found", entityType);
            
            BaseEntity result = new BaseEntity();
            result.Position = md.position;
            result.Scale = md.scale;
            result.Orientation = md.orientation;
            
            result.RenderingPlugin = RenderingPlugin.GetPlugin(md.RenderingPlugin, md.RenderingPluginData.Clone(), result);
            result.PhysicsPlugin = PhysicsPlugin.GetPlugin(md.PhysicsPlugin, md.PhysicsPluginData, result);
            result.LogicPlugin = LogicPlugin.GetPlugin(md.LogicPlugin, md.LogicPluginData, result);
            result.InputPlugin = InputPlugin.GetPlugin(md.InputPlugin, result);

            //result.RenderingPlugin.LoadModel(World.Instance.WorldContent.Load<Model>("Content/Models/"+md.modelName));

            return result;
        }

        public static BaseEntity BuildParticleEntity(Particles.ParticleSystemType oParticleSystemType, int primitiveCount, Vector3 vPosition, float fScale, Vector4 vScale, Matrix matOrientation)
        {
            // RenderingPlugin
            VertexDeclaration oVertexDeclaration = Particles.ParticleEngine.Instance.VertexDeclaration;
            VertexBuffer oVertexBuffer = Particles.ParticleEngine.Instance.VertexBuffer;

            BaseMeshPart oBaseMeshPart = new BaseMeshPart();
            oBaseMeshPart.StartIndex = 0;
            if (primitiveCount > Settings.Configuration.EngineSettings.MaxParticleCount)
                 oBaseMeshPart.PrimitiveCount = Settings.Configuration.EngineSettings.MaxParticleCount;
            else oBaseMeshPart.PrimitiveCount = primitiveCount;

            string strBaseMap;

            switch(oParticleSystemType)
            {
                case Particles.ParticleSystemType.Explosion:
                    strBaseMap = "pencil_p1_diff_v1";
                    oBaseMeshPart.Effect = new Rendering.Effects.ExplosionEffect();
                    break;
                case Particles.ParticleSystemType.Fire:
                    strBaseMap = "pencil_p2_diff_v1";
                    oBaseMeshPart.Effect = new Rendering.Effects.FireEffect();
                    break;
                case Particles.ParticleSystemType.Thrust:
                    strBaseMap = "particleFire";
                    oBaseMeshPart.Effect = new Rendering.Effects.ThrustEffect();
                    oBaseMeshPart.VectorParameters.Add("SCALE", vScale);
                    break;
                case Particles.ParticleSystemType.PowerUp:
                    strBaseMap = "particleColor";
                    oBaseMeshPart.Effect = new Rendering.Effects.PowerUpEffect();
                    break;
                case Particles.ParticleSystemType.Billboard:
                    strBaseMap = "particleBillboard";
                    oBaseMeshPart.Effect = new Rendering.Effects.BillboardEffect();
                    break;
                case Particles.ParticleSystemType.Advanced:
                    strBaseMap = "particleColor";
                    oBaseMeshPart.Effect = new Rendering.Effects.ParticleAdvancedEffect(primitiveCount);
                    break;
                default:
                    strBaseMap = "particleColor";
                    oBaseMeshPart.Effect = new Rendering.Effects.ParticleEffect();
                    break;
            }
            oBaseMeshPart.TextureParameters["baseMap"] = World.Instance.WorldContent.Load<Texture2D>(Configuration.EngineSettings.TextureDirectory + strBaseMap);
            
            BaseMeshPart[] oBaseMeshParts = new BaseMeshPart[] {oBaseMeshPart};

            RenderingPlugin.RenderingPluginData oRenderingPluginData =
                new RenderingPlugin.RenderingPluginData(oVertexDeclaration, oVertexBuffer, oBaseMeshParts, Matrix.Identity);

            // PhysicsPlugin
            PhysicsPlugin.PhysicsPluginData oPhysicsPluginData =
                new PhysicsPlugin.PhysicsPluginData(1f, Vector3.Zero, new PhysicsPlugin.PhysicsPluginFlags(), Vector3.Zero, 1f, 1f, 1f, null, 1f);

            // LogicPlugin
            LogicPlugin.LogicPluginData oLogicPluginData =
                new LogicPlugin.LogicPluginData(1f, 1f,0,null,null,null);

            BaseEntity result = new BaseEntity();
            result.Position = vPosition;
            result.Scale = fScale;
            result.Orientation = matOrientation;

            result.RenderingPlugin = RenderingPlugin.GetPlugin(RenderingPlugin.RenderingPluginType.ParticleSystem, oRenderingPluginData, result);
            result.PhysicsPlugin = PhysicsPlugin.GetPlugin(PhysicsPlugin.PhysicsPluginType.Null, oPhysicsPluginData, result);
            result.LogicPlugin = LogicPlugin.GetPlugin(LogicPlugin.LogicPluginType.NULL, oLogicPluginData, result);
            result.InputPlugin = InputPlugin.GetPlugin(InputPlugin.InputPluginType.NULL, result);

            //result.RenderingPlugin.LoadModel(World.Instance.WorldContent.Load<Model>("Content/Models/"+md.modelName));

            return result;
        }
    }
}
