

#region Dependencies
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.IO;
using System.Xml.Serialization;
using System.Xml;
using PraktWS0708.Entities;
#endregion

namespace PraktWS0708.Settings
{
    /// <summary>
    /// Diese Klasse enthält instancen aller settings-klassen und methoden zum laden und speichern aller einstellungen
    /// </summary>
	public class Configuration
    {
        #region Changed event

        public class ChangedEventArgs : EventArgs
        {
        }

        public static event EventHandler<ChangedEventArgs> Changed;

        public static void RaiseChanged()
        {
            EventHandler<ChangedEventArgs> handler = Changed;
            if (handler != null)
                handler(null, new ChangedEventArgs());
        }

        #endregion

        /// <summary>
        /// i don't think this really belongs here but right now I just want to get this thing to work
        /// </summary>
        public static string levelName;
        public static string trackName = "simpleTrack";
        public static string playerShipName = "simpleShip";
        public enum Difficulty
        {
            Easy,
            Medium,
            Hard
        }
        public static Difficulty difficulty = Difficulty.Medium;



		public static ModelDescriptions ModelDescriptions = new ModelDescriptions();
		public static TrackDescription TrackDescription = new TrackDescription();
		public static KeyBindings KeyBindings = new KeyBindings();
		public static EngineSettings EngineSettings = new EngineSettings();
        public static TrackLayouts TrackLayouts = new TrackLayouts();
        
		public static void LoadSettings()
		{
            TrackLayouts = TrackLayouts.Load(".\\Settings\\Storage\\TrackLayouts.xml");
            KeyBindings = KeyBindings.Load(".\\Settings\\Storage\\KeyBindings.xml");
            EngineSettings = EngineSettings.Load(".\\Settings\\Storage\\EngineSettings.xml");
		}

		public static void SaveSettings()
		{
            //TrackLayouts.Save(".\\Settings\\Storage\\TrackLayouts.xml");
            KeyBindings.Save(".\\Settings\\Storage\\KeyBindings.xml");
            EngineSettings.Save(".\\Settings\\Storage\\EngineSettings.xml");
		}
	}


}

