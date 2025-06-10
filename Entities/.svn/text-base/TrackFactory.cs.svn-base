using System;
using System.Collections.Generic;
using System.Text;
using PraktWS0708.Settings;
using PraktWS0708.Rendering;
using PraktWS0708.Rendering.Effects;
using PraktWS0708.Entities;
using PraktWS0708.Utils;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PraktWS0708.Entities
{
    /// <summary>
    /// this class represents a layout for a track
    /// </summary>
    public class TrackLayout
    {
        public int segments;
        public int tiles;
        protected TrackFactory.layout_type[] layout;

        public float segmentLength;
        public TrackLayout(string name)
        {

            Texture2D tex = World.Instance.WorldContent.Load<Texture2D>(Settings.Configuration.EngineSettings.TextureDirectory + "Track/" + name + "_layout");
            segments = tex.Width;
            tiles = tex.Height;
            Color[] c = new Color[tiles * segments];
            layout = new TrackFactory.layout_type[tiles * segments];
#if XBOX
            tex.GetData<Color>(c);
            for (int i = 0; i < c.Length;i++ )
            {
                layout[i].shader = (ushort)((ushort)(c[i].G) * (ushort)(256));
                layout[i].shader += c[i].B;

                layout[i].attributes = c[i].R;
                layout[i].undef1 = c[i].A;
            }
#else
            tex.GetData<TrackFactory.layout_type>(layout);
#endif

            segmentLength = 1.0f / ((float)segments);
        }


        public bool hasEffect(int tile, int segment, TrackFactory.effect_type type)
        {
            bool ret = (layout[tile * segments + segment].shader & (int)type) == (int)type;
            return ret;
        }






    }

    public class TrackFactory
    {

        public enum effect_type
        {
            BASE = 32768, //G#128
            BACK = 16384, //G#64
            BASE2 = 8192, //G#32
            SIGN = 4096   //G#16
        };


        protected struct trackpart_holder
        {
            public effect_type type;
            public TrackMeshPart tmp;
            public List<int> indices;
            public bool alterTexcoords;
            public trackpart_holder(effect_type type)
            {
                this.type = type;
                this.tmp = new TrackMeshPart();
                this.indices = new List<int>();
                this.alterTexcoords = !(type == effect_type.BACK || type == effect_type.BASE || type == effect_type.BASE2);
            }
        }

        public struct layout_type
        {
            
            public ushort shader;//blue+green
            public byte attributes;//red
            public byte undef1;//alpha
        }

        protected static void buildTrackGeometry(ref TrackGeometry tg, List<trackpart_holder> trackparts)
        {
            tg.TrackMeshParts = new TrackMeshPart[trackparts.Count];
            for (int part = 0; part < trackparts.Count; part++)
            {
                trackparts[part].tmp.buildIndexbuffer(trackparts[part].indices.ToArray());
                tg.TrackMeshParts[part] = trackparts[part].tmp;
            }
        }

        protected static void createTrackparts(ref TrackLayouts.TrackLayoutDescription tld, ref List<trackpart_holder> trackparts,TrackLayout tl)
        {
            trackpart_holder tph;
            foreach (TrackLayouts.EffectDescription ed in tld.effects)
            {
                switch (ed.type)
                {
                    case effect_type.BACK:
                        tph = new trackpart_holder(effect_type.BACK);
                        tph.tmp.Effect = new TrackBackEffect();
                        tph.tmp.AddParameters(ed.textures);
                        trackparts.Add(tph);
                        break;
                    case effect_type.BASE2:
                        tph = new trackpart_holder(effect_type.BASE2);
                        tph.tmp.Effect = new TrackBaseEffect(tl.tiles, tl.segments);
                        tph.tmp.AddParameters(ed.textures);
                        trackparts.Add(tph);
                        break;
                    case effect_type.BASE:
                        tph = new trackpart_holder(effect_type.BASE);
                        tph.tmp.Effect = new TrackBaseEffect(tl.tiles, tl.segments);
                        tph.tmp.AddParameters(ed.textures);
                        trackparts.Add(tph);
                        break;
                    case effect_type.SIGN:
                        tph = new trackpart_holder(effect_type.SIGN);
                        tph.tmp.Effect = new TrackSignEffect();
                        tph.tmp.AddParameters(ed.textures);
                        trackparts.Add(tph);
                        break;
                }


            }
        }

        public static void buildTrack(string name)
        {
            PraktWS0708.Settings.TrackDescription.TrackListEntry tle;
            Settings.Configuration.TrackDescription.TrackForName(name, out tle);
            buildTrack(tle);
        }



        /// <summary>
        /// this is where it all beginns:
        /// this function builds a track based on a track description
        /// </summary>
        /// <param name="tle">the track description</param>
        public static void buildTrack(PraktWS0708.Settings.TrackDescription.TrackListEntry tle)
        {
            List<trackpart_holder> trackparts = new List<trackpart_holder>();
            Track track = new Track();
            TrackGeometry tg = new TrackGeometry();
            //get layoutname from tle
            track.layout = new TrackLayout(tle.layout);
            TrackLayouts.TrackLayoutDescription tld = new TrackLayouts.TrackLayoutDescription();
            Settings.Configuration.TrackLayouts.getLayout4Name(tle.layout, out tld);
            List<VertexPositionNormalTextureTangent> vertices = new List<VertexPositionNormalTextureTangent>();
            List<Splines.ControlPoint> spline = new List<PraktWS0708.Utils.Splines.ControlPoint>();
            List<Splines.PositionTangentUpRadius> TangentFrameList = new List<Splines.PositionTangentUpRadius>();
            createTrackparts(ref tld, ref trackparts, track.layout);
            int vertexOffset;
            //create spline from controllpoints 
            track.length = Splines.BuildTFList(tle.controlpoints, ref TangentFrameList, track.layout.segments);
            //create mesh geometry
            //check if tangentframes are correct there seems to be a gap before the last frame (fehler in CatmullRom_Adaptive)?
            MeshFactory.splineExpander2(TangentFrameList, track.layout.tiles, ref vertices);
            vertexOffset = vertices.Count;
            //vertices.AddRange(vertices);
            //this makes it easier to step through the lists  (i.e getPosition())
            TangentFrameList.Add(new Splines.PositionTangentUpRadius(TangentFrameList[1])); TangentFrameList.Insert(0, new Splines.PositionTangentUpRadius(TangentFrameList[TangentFrameList.Count - 3]));
            TangentFrameList[TangentFrameList.Count - 1].relPos += 1.0f;
            TangentFrameList[0].relPos -= 1.0f;
            track.TangentFrames = TangentFrameList.ToArray();
            //fill structures holding the geometry
            track.SceneGraph.BuildSceneGraph(vertices, track.layout.tiles);
            //build TrackMeshParts

            int startFrame = 0, frameCount = 0, segment, tile;

            for (segment = 0; segment < track.layout.segments; segment++)
            {
                findFrames(track.layout.segmentLength * segment, track.layout.segmentLength, ref track.TangentFrames, ref startFrame, ref frameCount);
                for (tile = 0; tile < track.layout.tiles; tile++)
                {

                    for (int part = 0; part < trackparts.Count; part++)
                    {
                        if (track.layout.hasEffect(tile, segment, trackparts[part].type))
                            addIndices(trackparts[part].indices, startFrame, frameCount, tile, track.layout.tiles);
                    }
                }
            }

            buildTrackGeometry(ref tg, trackparts);
            //create vertexdeclaration
            tg.vertexDeclaration = new VertexDeclaration(RenderManager.Instance.GraphicsDevice, VertexPositionNormalTextureTangent.VertexElements);
            //create Vertex buffer
            tg.vertexBuffer = new VertexBuffer(RenderManager.Instance.GraphicsDevice, VertexPositionNormalTextureTangent.SizeInBytes * vertices.Count, ResourceUsage.WriteOnly, ResourceManagementMode.Automatic);
            tg.vertexBuffer.SetData(vertices.ToArray());
            List<Vector3> points = new List<Vector3>();
            for (int i = 0; i < vertices.Count; i += 4)
                points.Add(vertices[i].Position);
            tg.aabb = BoundingBox.CreateFromPoints(points);
            track.geometry = tg;
            LightmapFactory.defaultLightmaps(out track.lightmap, out track.lightdirmap, new Color(127, 127, 127));
            World.Instance.Track = track;
        }




        /// <summary>
        /// find the frames assoziated with a length of the track
        /// </summary>
        /// <param name="segmentOffset">distance from the start of the track</param>
        /// <param name="segmentLength">length of the part</param>
        /// <param name="tfl">a list of all tangent frames</param>
        /// <param name="startFrame">the first frame</param>
        /// <param name="frameCount">the number of frames</param>
        protected static void findFrames(float segmentOffset, float segmentLength, ref Splines.PositionTangentUpRadius[] tfl, ref int startFrame, ref int frameCount)
        {
            //slow as hell. should make use of last result...
            for (startFrame = 0; tfl[startFrame + 1].relPos < segmentOffset; startFrame++) ;
            for (frameCount = 1; (tfl[startFrame + 1 + frameCount].relPos) < (segmentOffset + segmentLength); frameCount++) ;
        }


        /// <summary>
        /// create the indices that make up one tile on the track
        /// </summary>
        /// <param name="indices">output: a list of the indices</param>
        /// <param name="startFrame">the first of the frames that are part of the tile</param>
        /// <param name="frameCount">the number of frames that make up this tile</param>
        /// <param name="tile">the tile in question</param>
        /// <param name="tiles">the total number of frames per segment</param>
        protected static void addIndices(List<int> indices, int startFrame, int frameCount, int tile, int tiles)
        {
            tiles++;
            for (int i = 0; i < frameCount; i++)
            {
                indices.Add((((startFrame + i) + 0) * tiles + tile) + 0);
                indices.Add((((startFrame + i) + 0) * tiles + tile) + 1);
                indices.Add((((startFrame + i) + 1) * tiles + tile) + 0);
                indices.Add((((startFrame + i) + 0) * tiles + tile) + 1);
                indices.Add((((startFrame + i) + 1) * tiles + tile) + 1);
                indices.Add((((startFrame + i) + 1) * tiles + tile) + 0);
            }
        }



    }
}
