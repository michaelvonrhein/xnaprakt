#region Dependencies
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework.Audio;
#endregion

namespace PraktWS0708
{
    /// <summary>
    /// An enum for all of the games sounds
    /// </summary>
    public enum Sounds
    {
        MenuNext,
        MenuConfirm,
        MenuBack,
        MenuTheme,
        GameMusic,
        //GameMusic2,
        explosion,
        alien,
        breaks,
        explosion_long,
        fart,
        loser,
        pickup,
        shot,
        hit,
        bump,
        sogeil,
        win,
        thrust,
        movieprojector
    };

    /// <summary>
    /// Abstracts away the sounds for a simple interface using the Sounds enum
    /// </summary>
    public static class Sound
    {
        private static AudioEngine engine;
        private static WaveBank wavebank;
        private static SoundBank soundbank;

       

        private static string[] cueNames = new string[]
        {
            "menu_select",
            "menu_back",
            "menu_scroll",
            "Menu_Loop",
            "game_music",
            //"game_music2",
            "explosion",
            "alien",
            "break",
            "explosion_long",
            "fart",
            "loser",
            "pickup",
            "shot",
            "hit",
            "bump",
            "sogeil",
            "win",
            "thrust",
            "movieprojector"
        };

        /// <summary>
        /// Plays a sound
        /// </summary>
        /// <param name="sound">Which sound to play</param>
        /// <returns>XACT cue to be used if you want to stop this particular looped sound. Can NOT be ignored.  If the cue returned goes out of scope, the sound stops!!</returns>
        public static Cue Play(Sounds sound)
        {
            
            Cue returnValue = soundbank.GetCue(cueNames[(int)sound]);
            returnValue.Play();
            return returnValue;
        }

        /// <summary>
        /// Added to get Cue for applying 3D sound
        /// </summary>
        /// <param name="sound"></param>
        /// <returns></returns>
        public static Cue GetCue(Sounds sound)
        {
            Cue returnValue = soundbank.GetCue(cueNames[(int)sound]);
            return returnValue;
        }

        /// <summary>
        /// Plays a sound
        /// </summary>
        /// <param name="sound">Which sound to play</param>
        /// <returns>Nothing!  This cue will play through to completion and then free itself.</returns>
        public static void PlayCue(Sounds sound)
        {
            soundbank.PlayCue(cueNames[(int)sound]);
        }

        /// <summary>
        /// Pumps the AudioEngine to help it clean itself up
        /// </summary>
        public static void Update()
        {
            engine.Update();
        }

        /// <summary>
        /// Stops a previously playing cue
        /// </summary>
        /// <param name="cue">The cue to stop that you got returned from Play(sound)</param>
        public static void Stop(Cue cue)
        {
            if (cue != null)
                cue.Stop(AudioStopOptions.Immediate);
        }

        /// <summary>
        /// Starts up the sound code
        /// </summary>
        public static void Initialize()
        {
            engine      = new AudioEngine(@"Content/Audio/PraktWS0708.xgs");
            wavebank    = new WaveBank(engine, @"Content/Audio/PraktWS0708.xwb");
            soundbank   = new SoundBank(engine, @"Content/Audio/PraktWS0708.xsb"); 
        }

        /// <summary>
        /// Shuts down the sound code tidily
        /// </summary>
        public static void Shutdown()
        {
            soundbank.Dispose();
            wavebank.Dispose();
            engine.Dispose();
        }
    }
}