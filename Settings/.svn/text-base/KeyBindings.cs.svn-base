using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using System.IO;
using System.Xml.Serialization;
using System.Xml;
using PraktWS0708.Entities;
using System.Collections;
using System.Collections.Generic;

namespace PraktWS0708.Settings
{
    public class KeyBindings
    {
        
        public struct KeyDefinition
        {
            public string keyName;
            public string actionName;
            public KeyDefinition(string keyName, string actionName)
            {
                this.keyName = keyName;
                this.actionName = actionName;
            }
        }



        public struct LayoutListEntry
        {
            public string layoutname;
            public KeyDefinition[] KeyDefinitions;
            public LayoutListEntry(KeyDefinition[] KeyDefinitions, string layoutname)
            {
                this.KeyDefinitions = KeyDefinitions;
                this.layoutname = layoutname;
            }

        }

        public LayoutListEntry[] LayoutArray = new LayoutListEntry[]
            {
                new LayoutListEntry(new KeyDefinition[]{new KeyDefinition("key","action")},"defaultLayout")
            };

        protected Dictionary<string, LayoutListEntry> LayoutList = new Dictionary<string, LayoutListEntry>();

        public bool getLayout4Name(string name, out KeyDefinition[] kd)
        {
            LayoutListEntry lle;
            kd = null;
            bool ret = LayoutList.TryGetValue(name, out lle);
            if (ret)
                kd = lle.KeyDefinitions;
            return ret;
        }


        #region Load/Save code
        /// <summary>
        /// Saves the current settings
        /// </summary>
        /// <param name="filename">The filename to save to</param>
        public void Save(string filename)
        {
            Stream stream = File.Create(filename);   
            XmlSerializer serializer = new XmlSerializer(typeof(KeyBindings));
            serializer.Serialize(stream, this);
            stream.Close();
        }

        /// <summary>
        /// Loads settings from a file
        /// </summary>
        /// <param name="filename">The filename to load</param>
        /// 
        public static KeyBindings Load(string filename)
        {
            KeyBindings md;

            Stream stream = File.Open(filename, FileMode.Open);         
            XmlSerializer serializer = new XmlSerializer(typeof(KeyBindings));
            md = (KeyBindings)serializer.Deserialize(stream);

            for (int i = 0; i < md.LayoutArray.Length; i++)
                md.LayoutList.Add(md.LayoutArray[i].layoutname, md.LayoutArray[i]);
            return md;

        } 
        #endregion
    }
}
