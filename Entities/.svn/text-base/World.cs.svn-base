#region Using Statements
using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using PraktWS0708.Utils;
using PraktWS0708.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using PraktWS0708.Rendering;
using PraktWS0708.Physics;
using PraktWS0708.Logic;
#endregion

namespace PraktWS0708.Entities
{
    /// <summary>
    /// this class holds the whole scene.
    /// this includes: the track, the camera and a list of all objects
    /// </summary>

    public enum ViewTypes
    {
        COCKPIT,
        THIRDPERSON
    }


    public class World
    {

        #region Fields

        public static World Instance; //the one and only
        public Logic.PowerUpManager powerUpManager = Logic.PowerUpManager.getInstance();


        public Camera Camera = new Camera();
        public Track Track;
        public PhysicsSystem PhysicsSystem;
        public Particles.ParticleManager ParticleManager;
        public AudioListener AudioListener;
        public BaseEntity PlayersShip;
        public BaseEntity GhostShip;
        public BaseEntity Bomb;
        private GameTime m_GameTime;
        public BaseEntity[] EnemyShips = new BaseEntity[0];
        public Sunlight Sunlight;
        public List<Cue> CueList = new List<Cue>();
        
        //the content manager used to hold the content of the world
        public ContentManager WorldContent;

        //will be replaced someday by a scenegraph 
        public SortedList<int, BaseEntity> Objects = new SortedList<int, BaseEntity>();

        public GameEnvironment gameEnvironment;

        public ViewTypes ViewType = ViewTypes.COCKPIT;

        #endregion

        #region constructor

        public World()
        {
                      
        }

        static World()
        {
            Instance = new World();
            Instance.PhysicsSystem = new PhysicsSystem();
            Instance.ParticleManager = new Particles.ParticleManager();
        }

        #endregion

        #region Methods

        /// <summary>
        /// stores the GameTime instance for the world
        /// </summary>
        public GameTime GameTime
        {
            get
            {
                if (m_GameTime == null) return new GameTime();
                else return m_GameTime;
            }
        }


        /// <summary>
        /// updates physics, particles and audio
        /// </summary>
        /// <param name="oGameTime"></param>
        public void Update(GameTime oGameTime)
        {
            m_GameTime = oGameTime;
            PhysicsSystem.Simulate(oGameTime);
            ParticleManager.Update(oGameTime);
            AudioListener.Position = PlayersShip.Position;  // update AudioListener to PlayerShip - Position
            UpdateCues();   // update active cue list
        }

        /**
         * Update active Cue list - remove inactive ones
         */
        public void UpdateCues()
        {
            for (int iIndex = 0; iIndex < CueList.Count; iIndex++)
            {
                if (CueList[iIndex].IsStopped) CueList.RemoveAt(iIndex);
            }
        }

        /// <summary>
        /// plays a cue at a specific position
        /// </summary>
        /// <param name="sound"></param>
        /// <param name="vPosition"></param>
        public void PlayCue(Sounds sound, Vector3 vPosition)
        {
            Cue cue = Sound.GetCue(sound);
            AudioEmitter oAudioEmitter = new AudioEmitter();
            oAudioEmitter.Position = vPosition;
            cue.Apply3D(AudioListener, oAudioEmitter);
            CueList.Add(cue);
            cue.Play();
        }

        //destroy the world
        public void ResetWorld()
        {
            int c;
            if (World.Instance.Track!=null) c = World.Instance.Track.SceneGraph.countRegisteredObjects();
            //if (WorldContent != null)
            //    WorldContent.Unload();
            
            foreach (KeyValuePair<int, BaseEntity> pair in Objects)
            {
                pair.Value.Destroy();
            }

            PlayersShip = null;
            GhostShip = null;
            Bomb = null;
            EnemyShips = new BaseEntity[0];

            Objects.Clear();
            if (World.Instance.Track != null) c = World.Instance.Track.SceneGraph.countRegisteredObjects();
            PhysicsSystem = new PhysicsSystem();
            AudioListener = new AudioListener();
            ParticleManager.Reset();
            CueList.Clear();

            AI.AISystem.Instance.Reset();
        }

        /// <summary>
        /// we should try to unify this registration process.
        /// whos job is it to register a model anyway? should it be done by the world or the entity factory?
        /// and how should we unregister it? i think the eventhandler approach used by ai is quite a good solution since 
        /// it makes the clean up each subsystems own problem
        /// </summary>
        /// <param name="model"></param>
        public void AddModel(Entities.BaseEntity model)
        {
            // add model to physics engine
            PhysicsSystem.Add(model.ObjectID, model.PhysicsPlugin);
            //add model to object list
            Objects.Add(model.ObjectID, model);
            //add entity to renderering
            RenderManager.Instance.register(model);
        }

        /// <summary>
        /// remove object from the system 
        /// </summary>
        /// <param name="model"></param>
        public void RemoveModel(Entities.BaseEntity model)
        {
            PhysicsSystem.Remove(model.ObjectID);
            ParticleManager.Remove(model.ObjectID);
            Objects.Remove(model.ObjectID);
            model.Destroy();
        }

        public void buildWorld(string trackname)
        {
            Sunlight = new Sunlight();
            TrackFactory.buildTrack(trackname);
            Sunlight.update();
            World.Instance.gameEnvironment = new GameEnvironment();       
        }

        public void buildWorld()
        {
            Sunlight = new Sunlight();
            World.Instance.gameEnvironment = new GameEnvironment();
        }

        #endregion
    }
}
