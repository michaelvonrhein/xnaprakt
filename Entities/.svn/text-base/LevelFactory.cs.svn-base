using System;
using System.Collections.Generic;
using System.Text;
using PraktWS0708.Utils;
using Microsoft.Xna.Framework;
using PraktWS0708.Physics;
using PraktWS0708.Settings;

namespace PraktWS0708.Entities
{
    /// <summary>
    /// this class will handel the loading and reset process
    /// </summary>
    class LevelFactory
    {
        /// <summary>
        /// just an idea:
        /// this enum could be used to optimize the loading process
        /// </summary>
        public enum LoadType
        {
            NEW,
            RESET
        }
        /// <summary>
        /// this funktion should aktually use some kind of level definition file to load a level
        /// but since we dont have any until now, the names get mapped to funktions with hard coded level descriptions
        /// </summary>
        /// <param name="levelName"></param>
        public static void LoadLevel(string levelName,LoadType type)
        {
            if ("default".CompareTo(levelName) == 0)
                LoadDefaultLevel(type);
            else if ("trackOnly".CompareTo(levelName) == 0)
                LoadTrackOnly(type);
            else if ("trackLeanAndMean".CompareTo(levelName) == 0)
                LoadTrackLeanAndMean(type);
            else if ("race".CompareTo(levelName) == 0)
                LoadRace(type);
            else if ("trackeditor".CompareTo(levelName) == 0)
                LoadRace(type);
            else if ("aidemo".CompareTo(levelName) == 0)
                LoadAIDemo(type);
        }

        #region Loaders

        public static void LoadTrackOnly(LoadType type)
        {
            World.Instance.ResetWorld();
            if (type == LoadType.NEW)
                World.Instance.buildWorld(Settings.Configuration.trackName);
            LightmapFactory.buildLightMaps(ref World.Instance.Track, 1024, 32);

            AddPlayersShip(0);

            AddGhostShip(true);

            
            BaseEntity entity = EntityFactory.BuildEntity("schanze");
            BaseEntity entity2 = EntityFactory.BuildEntity("banane1");
            BaseEntity entity3 = EntityFactory.BuildEntity("healthpack1");
            BaseEntity entity4 = EntityFactory.BuildEntity("rocket");
            BaseEntity entity5 = EntityFactory.BuildEntity("turbo1");
            BaseEntity entity6 = EntityFactory.BuildEntity("bomb1");
            BaseEntity entity7 = EntityFactory.BuildParticleEntity(Particles.ParticleSystemType.Fire, 1000, entity6.Position, 0.1f, Vector4.One, entity6.Orientation);
            BaseEntity entity8 = EntityFactory.BuildParticleEntity(Particles.ParticleSystemType.PowerUp, 2000, entity2.Position, 0.1f, Vector4.One, entity2.Orientation);
            BaseEntity entity9 = EntityFactory.BuildParticleEntity(Particles.ParticleSystemType.Billboard, 2, entity6.Position, 0.375f, Vector4.One, entity2.Orientation);

            //World.Instance.AddModel(EntityFactory.BuildEntity("rocket"));
            //World.Instance.AddModel(EntityFactory.BuildEntity("rocket"));
            //World.Instance.AddModel(EntityFactory.BuildEntity("rocket"));
            //World.Instance.AddModel(EntityFactory.BuildEntity("rocket"));

            //ptu = World.Instance.Track.TangentFrames[21];
            //entity.Position = ptu.Position - ptu.Up * ptu.Radius * 0.6f;
            //entity.Orientation = OurMath.MakeCoordSystem(Vector3.Cross(ptu.Tangent, ptu.Up), ptu.Up, ptu.Tangent * -1.0f);
            //entity.Scale = 1f;
            entity.PhysicsPlugin.Reset();
            entity.Update();
            World.Instance.AddModel(entity);
            World.Instance.AddModel(entity2);
            World.Instance.AddModel(entity3);
            World.Instance.AddModel(entity4);
            World.Instance.AddModel(entity5);
            World.Instance.AddModel(entity6);
            //World.Instance.ParticleManager.Add(new Particles.ParticleSystemManagementData(entity7, entity6, -1, Vector3.Zero, Matrix.Identity));
            //World.Instance.ParticleManager.Add(new Particles.ParticleSystemManagementData(entity8, null, -1, Vector3.Zero, Matrix.Identity));
            //World.Instance.ParticleManager.Add(new Particles.ParticleSystemManagementData(entity9, entity6, -1, Vector3.Zero, Matrix.Identity));
        }

        public static void LoadTrackLeanAndMean(LoadType type)
        {
            World.Instance.ResetWorld();
            if (type == LoadType.NEW)
                World.Instance.buildWorld(Settings.Configuration.trackName);
            LightmapFactory.buildLightMaps(ref World.Instance.Track, 1024, 32);

            AddPlayersShip(0);

            AddGhostShip(false);  
        }

        public static void LoadDefaultLevel(LoadType type)
        {
            World.Instance.ResetWorld();

            // build the new track
            if (type == LoadType.NEW)
                World.Instance.buildWorld(Settings.Configuration.trackName);
            LightmapFactory.buildLightMaps(ref World.Instance.Track, 1024, 32);
            AddPlayersShip(0);

            AddEnemyShips(1, 15, 0);

            //AddRigidShip(40);

            AddGhostShip(true);

            //AddDice("black", 20);
            //AddDice("green", 40);
            //AddDice("red", 60);
        }

        public static void LoadSelectShipLevel(LoadType type)
        {
            World.Instance.ResetWorld();

            // build the new track
            if (type == LoadType.NEW)
                World.Instance.buildWorld(Settings.Configuration.trackName);

            AddPlayersShip(0);
        }

        public static void LoadRace(LoadType type)
        {
            World.Instance.ResetWorld();

            // build the new track
            if (type == LoadType.NEW)
                World.Instance.buildWorld(Settings.Configuration.trackName);
            LightmapFactory.buildLightMaps(ref World.Instance.Track, 1024, 32);
            AddPlayersShip(0);

            AddEnemyShips(3, 8, 8);
            //AddEnemyShips(1, 8, 8);
            Settings.TrackDescription.TrackListEntry tmp;
            Settings.Configuration.TrackDescription.TrackForName(Settings.Configuration.trackName, out tmp);

            BaseEntity[] pickup = new BaseEntity[tmp.pickUpPoints.Length];
            for (int i = 0; i < tmp.pickUpPoints.Length; i++)
            {
                switch ((int)tmp.pickUpPoints[i].W)
                {
                    case 1:
                        pickup[i] = EntityFactory.BuildEntity("banane1");
                        break;
                    case 2:
                        pickup[i] = EntityFactory.BuildEntity("bomb1");
                        break;
                    case 3:
                        pickup[i] = EntityFactory.BuildEntity("shield1");
                        break;
                    case 4:
                        pickup[i] = EntityFactory.BuildEntity("turbo1");
                        break;
                    case 5:
                        pickup[i] = EntityFactory.BuildEntity("healthpack1");
                        break;
                    case 6:
                        pickup[i] = EntityFactory.BuildEntity("dicered");
                        break;
                    case 7:
                        pickup[i] = EntityFactory.BuildEntity("rocket2");
                        break;
                    case 8:
                        pickup[i] = EntityFactory.BuildEntity("box1");
                        break;
                   default:
                        break;
                }
                pickup[i].Position = new Vector3(tmp.pickUpPoints[i].X, tmp.pickUpPoints[i].Y, tmp.pickUpPoints[i].Z);
                if((int)tmp.pickUpPoints[i].W == 8)
                {
                    ModelDescription md;
                    if (!Configuration.ModelDescriptions.ModelDescriptionForName("box1", out md))
                        throw new ArgumentException("Entity type not found", "box1");
                    ((RigidBody)pickup[i].PhysicsPlugin).m_BoundingBoxTree = CollisionDetection.generateBoundingBoxTreeFromTriangles(md.PhysicsPluginData.m_oaTriangles,pickup[i].Scale,pickup[i].Orientation,pickup[i].Position);
                }
                /*pickup[i].Position.X = tmp.pickUpPoints[i].X;
                pickup[i].Position.Y = tmp.pickUpPoints[i].Y;
                pickup[i].Position.Z = tmp.pickUpPoints[i].Z;*/
                //set the cur- and des-positions to entity-position
                RigidBody rPickup = (RigidBody)pickup[i].PhysicsPlugin;
                rPickup.Reset(rPickup.m_iCollisionSearch);
                
                World.Instance.AddModel(pickup[i]);
            }

            //AddDice("black", 20);
            //AddDice("green", 40);
            //AddDice("red", 60);
        }

        public static void LoadAIDemo(LoadType type)
        {
            World.Instance.ResetWorld();

            // build the new track
            if (type == LoadType.NEW)
                World.Instance.buildWorld("simpleTrack");
            LightmapFactory.buildLightMaps(ref World.Instance.Track, 1024, 32);
            AddPlayersShip(0);

            AddEnemyHypersonic(8);
            Settings.TrackDescription.TrackListEntry tmp;
            Settings.Configuration.TrackDescription.TrackForName(Settings.Configuration.trackName, out tmp);

            AddDice("black", 40);
            AddDice("green", 80);
            AddDice("red", 120);
        }

        #region Helpers

        private static void AddPlayersShip(int atTangentFrame)
        {
            World.Instance.PlayersShip = EntityFactory.BuildEntity(Settings.Configuration.playerShipName);
            Splines.PositionTangentUpRadius ptu = World.Instance.Track.TangentFrames[atTangentFrame];
            World.Instance.PlayersShip.Position = ptu.Position - ptu.Up * ptu.Radius * 0.6f;
            World.Instance.PlayersShip.Orientation = OurMath.MakeCoordSystem(Vector3.Cross(ptu.Tangent, ptu.Up), ptu.Up, ptu.Tangent * -1.0f);
            World.Instance.PlayersShip.PhysicsPlugin.Reset();
            World.Instance.PlayersShip.Update();
            World.Instance.AddModel(World.Instance.PlayersShip);
        }

        private static Random random = new Random((int)DateTime.Now.Ticks); 

        private static void AddEnemyShips(int numEnemies, int startTangentFrame, int tangentFrameStep)
        {
            World.Instance.EnemyShips = new BaseEntity[numEnemies];
            for (int i = 0; i < numEnemies; i++)
            {
                BaseEntity entity = EntityFactory.BuildEntity("AIShip" + random.Next(1, 10));
                Splines.PositionTangentUpRadius ptu = World.Instance.Track.TangentFrames[startTangentFrame + i * tangentFrameStep];
                entity.Position = ptu.Position -ptu.Up * ptu.Radius * 0.6f;
                entity.Orientation = OurMath.MakeCoordSystem(Vector3.Cross(ptu.Tangent, ptu.Up), ptu.Up, ptu.Tangent * -1.0f);
                entity.PhysicsPlugin.Reset();
                entity.Update();
                World.Instance.AddModel(entity);
                World.Instance.EnemyShips[i] = entity;
            }
        }

        private static void AddEnemyHypersonic(int atTangentFrame)
        {
            World.Instance.EnemyShips = new BaseEntity[1];
            BaseEntity entity = EntityFactory.BuildEntity("AIShip10");
            Splines.PositionTangentUpRadius ptu = World.Instance.Track.TangentFrames[atTangentFrame];
            entity.Position = ptu.Position - ptu.Up * ptu.Radius * 0.6f;
            entity.Orientation = OurMath.MakeCoordSystem(Vector3.Cross(ptu.Tangent, ptu.Up), ptu.Up, ptu.Tangent * -1.0f);
            entity.PhysicsPlugin.Reset();
            entity.Update();
            World.Instance.AddModel(entity);
            World.Instance.EnemyShips[0] = entity;
        }

        private static void AddRigidShip(int atTangentFrame)
        {
            BaseEntity entity = EntityFactory.BuildEntity("RigidShip");
            Splines.PositionTangentUpRadius ptu = World.Instance.Track.TangentFrames[atTangentFrame];
            entity.Position = ptu.Position - ptu.Up * ptu.Radius * 0.6f;
            entity.Orientation = OurMath.MakeCoordSystem(Vector3.Cross(ptu.Tangent, ptu.Up), ptu.Up, ptu.Tangent * -1.0f);
            entity.PhysicsPlugin.Reset();
            entity.Update();
            World.Instance.AddModel(entity);
        }

        private static void AddGhostShip(bool hidden)
        {
            World.Instance.GhostShip = EntityFactory.BuildEntity("ghostShip");
            World.Instance.GhostShip.Position = World.Instance.PlayersShip.Position;
            World.Instance.GhostShip.Orientation = World.Instance.PlayersShip.Orientation;
            World.Instance.GhostShip.PhysicsPlugin.Reset();
            World.Instance.GhostShip.Update();
            World.Instance.GhostShip.RenderingPlugin.Hidden = hidden;
            World.Instance.AddModel(World.Instance.GhostShip);
        }

        private static void AddDice(string color, int atTangentFrame)
        {
            BaseEntity entity = EntityFactory.BuildEntity("dice" + color);
            Splines.PositionTangentUpRadius ptu = World.Instance.Track.TangentFrames[atTangentFrame];
            entity.Position = ptu.Position - ptu.Up * ptu.Radius * 0.6f;
            entity.Orientation = OurMath.MakeCoordSystem(Vector3.Cross(ptu.Tangent, ptu.Up), ptu.Up, ptu.Tangent * -1.0f);
            entity.PhysicsPlugin.Reset(atTangentFrame);
            entity.Update();
            World.Instance.AddModel(entity);
        }

        #endregion

        #endregion
    }
}
