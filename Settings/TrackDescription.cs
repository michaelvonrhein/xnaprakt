using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.IO;
using System.Xml.Serialization;
using System.Xml;
using PraktWS0708.Entities;
using System.Collections;
using System.Collections.Generic;
using PraktWS0708.Rendering;

namespace PraktWS0708.Settings
{
    public class TrackDescription
    {
        public struct TrackInfo
        {
            public string realName;
            public string editorTime;
            public string difficulty;
            public string dannerComment;
        }

        public struct TrackListEntry
        {
            public string layout;
            public string trackName;
            //public string[] textureMaps;
            //public string[] effectList;
            public PraktWS0708.Utils.Splines.ControlPoint[] controlpoints;
            public Vector4[] pickUpPoints;
            public TrackLight[] lights;
            public TrackInfo trackInfo;

            

            public TrackListEntry(string trackName,  PraktWS0708.Utils.Splines.ControlPoint[] controlpoints, Vector4[] pickUpPoints, string layout, TrackLight[] lights, TrackInfo trackInfo)
            {
                //this.layout = "trackLayout_basic";
                this.layout = layout;
                this.trackName = trackName;
                this.controlpoints = controlpoints;
                //this.textureMaps = new string[1];
                //this.effectList = new string[1];
                this.pickUpPoints = pickUpPoints;
                this.lights = lights;
                this.trackInfo = trackInfo;
            } 
        }




        /*public TrackListEntry[] trackArray = new TrackListEntry[]
			{
			  new TrackListEntry("simpleTrack",
								 "track_diffuse","tracknormal",
								 new Utils.Splines.ControlPoint[]
								 {
										new PraktWS0708.Utils.Splines.ControlPoint(new Vector3(-10, 10, 0),2),
										new PraktWS0708.Utils.Splines.ControlPoint(new Vector3(0, 0, 10),2),
										new PraktWS0708.Utils.Splines.ControlPoint(new Vector3(10, -10, 0),3),
										new PraktWS0708.Utils.Splines.ControlPoint(new Vector3(20, 0, 0),3),
										new PraktWS0708.Utils.Splines.ControlPoint(new Vector3(10, 10, 0),1),
										new PraktWS0708.Utils.Splines.ControlPoint(new Vector3(0, 0, -10),3),
										new PraktWS0708.Utils.Splines.ControlPoint(new Vector3(-10, -10, 0),3),
										new PraktWS0708.Utils.Splines.ControlPoint(new Vector3(-20, 0, 0),2),
										new PraktWS0708.Utils.Splines.ControlPoint(new Vector3(-10, 10, 0),2)
								 }) 
		  };*/

        public List<TrackListEntry> trackArray = new List<TrackListEntry>();

        protected Dictionary<string, TrackListEntry> trackList = new Dictionary<string, TrackListEntry>();

        public bool TrackForName(string name, out TrackListEntry tle)
        {
            return trackList.TryGetValue(name, out tle);
        }


        #region Load/Save code
        /// <summary>
        /// Saves the current settings
        /// </summary>
        /// <param name="filename">The filename to save to</param>
        public void Save(string filename)
        {
            Stream stream = File.Create(filename);

            XmlSerializer serializer = new XmlSerializer(typeof(TrackDescription));
            serializer.Serialize(stream, this);
            stream.Close();
        }

        /// <summary>
        /// Loads settings from a file
        /// </summary>
        /// <param name="filename">The filename to load</param>
        /*public static TrackDescription Load(string filename)
        {
            TrackDescription md;
            Stream stream = File.OpenRead(filename);
            XmlSerializer serializer = new XmlSerializer(typeof(TrackDescription));
            md = (TrackDescription)serializer.Deserialize(stream);
            for (int i = 0; i < md.trackArray.Length; i++)
                md.trackList.Add(md.trackArray[i].trackName, md.trackArray[i]);
            return md;

        }*/

        public void LoadTrackListEntry(string filename)
        {
            TrackListEntry entry;

            Stream stream = File.OpenRead(filename);
            XmlSerializer serializer = new XmlSerializer(typeof(TrackListEntry));

            entry = (TrackListEntry)serializer.Deserialize(stream);

            if(!trackList.ContainsKey(entry.trackName))
            {
                trackArray.Add(entry);
                trackList.Add(entry.trackName, entry);
            }
        }
        #endregion
    }
}
