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
    
    public class TrackLayouts
    {
        /// <summary>
        /// ein Track Layout besteht aus einer sammlung von effekten und einem image file, dass benutzt wird um zu bestimmen
        /// welcher effekt auf welchen teil des tracks gelegt werden soll.
        /// dabei wird der blau und grün kanal zur bestimmung des effektes und der rot kanal für attribute benutzt.
        /// die auflösung des bildes bestimmt die geometrie des tracks (anzahl der segmente, faces pro segment)
        /// 
        /// jede effekt beschreibung enthält den effekttyp und einen array, dass die namen der dem effekt zugeordneten texturen enthält.
        /// zur zeit können 3 verschiede effekte auf den track gelegt werden:
        /// BACK:
        /// dieser effekt legt das aussehen der aussenseite des tracks fest. 
        /// der effekt benutzt eine textur.
        /// BASE: 
        /// dieser effekt legt das aussehen der innenseite des tracks fest. es können zwei instanzen auf den track gelegt werden (BASE, BASE2)
        /// der effekt verwendet 9 texturen. die erste textur legt fest, wo welche diffuse textur angewendet wird.
        /// jeder farbkanal ist dabei einer diffusen textur zugeordnet. der farbwert des kanals wird als alphawert zum überblenden der texturen verwendet.
        /// die zweite textur bestimmt auf ähnliche weise wo welche normalmap angewendet wird. (wobei normal maps nicht ohne weiteres überblendet werden können).
        /// die dritte textur legt farbe und glossiness für den spekularen term der lichtberechnung fest. glossiness wird im alpha kanal gespeichert.
        /// die nächsten 3 texturen sind die diffusen texturen, die letzten 3 die normalmaps.
        /// die auflösung der ersten 3 texturen muss der auflösung der layout textur entsprechen.
        /// SIGN: (experimental)
        /// dieser effekt erlaubt es eine textur auf ein mehrere faces grosses gebiet des tracks zu legen.
        /// dazu benötigt der effekt zwei texturen.
        /// die erste textur ist die die auf den track gelegt werden soll. 
        /// die zweite wird benutzt um das gebiet zu bestimmen, auf das die textur gelegt wird. 
        /// dazu werden die textur koordinaten des tracks verschoben und gestaucht, sodass
        /// diesen in dem gewünschten bereich zwischen 0,0 und 1,1 liegen.
        /// der rot und grün kanal enthalten dabei den offset, blau und alpha die grösse des gebietes.
        /// die auflösung dieser textur muss wieder der auflösung der layout textur entsprechen
        /// 
        /// </summary>
        public struct TrackLayoutDescription
        {
            public string name;
            public EffectDescription[] effects;
           
            public TrackLayoutDescription(string name, EffectDescription[] effects)
            {
                this.name = name;
                this.effects = effects;
            }

        }

        public struct EffectDescription
        {
            public TrackFactory.effect_type type;
            public string[] textures;
            
            public EffectDescription(TrackFactory.effect_type type, string[] textures)
            {
                this.type = type;
                this.textures = textures;
            }
        }

        public TrackLayoutDescription[] LayoutArray = new TrackLayoutDescription[10];
        /*{
            new TrackLayoutDescription("trackLayout_basic", 
                            new EffectDescription[]
                            {
                                new EffectDescription(TrackFactory.effect_type.BACK,
                                                      new string[]{"backside"}),
                                new EffectDescription(TrackFactory.effect_type.BASE,
                                                      new string[]{"DescDiffuse","DescNormal","specular",
                                                                   "track_diffuse","checkered","whiteblob",
                                                                   "whiteblob","whiteblob","whiteblob"}),
                                new EffectDescription(TrackFactory.effect_type.SIGN,
                                                      new string[]{"mindbender16square","texcoordOffset"})
                                
                            })
        };*/ 

        protected Dictionary<string, TrackLayoutDescription> LayoutList = new Dictionary<string, TrackLayoutDescription>();

        public bool getLayout4Name(string name, out TrackLayoutDescription kd)
        {  
            bool ret = LayoutList.TryGetValue(name, out kd);
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
            XmlSerializer serializer = new XmlSerializer(typeof(TrackLayouts));
            serializer.Serialize(stream, this);
            stream.Close();
        }

        /// <summary>
        /// Loads settings from a file
        /// </summary>
        /// <param name="filename">The filename to load</param>
        /// 
        public static TrackLayouts Load(string filename)
        {
            TrackLayouts tl;
            Stream stream = File.Open(filename, FileMode.Open);
            XmlSerializer serializer = new XmlSerializer(typeof(TrackLayouts));
            tl = (TrackLayouts)serializer.Deserialize(stream);

            stream.Close();

            for (int i = 0; i < tl.LayoutArray.Length; i++)
               tl.LayoutList.Add(tl.LayoutArray[i].name, tl.LayoutArray[i]);
            return tl;
        }
        #endregion
    }
}
