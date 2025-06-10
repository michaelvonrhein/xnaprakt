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
    class LightmapFactory
    {
        public static void buildLightMaps(ref Track track, int width, int height)
        {

            
            LightMap map = new LightMap(width, height, 0.05f, 0.05f, 0.05f, 0.8f);
            Vector3 lightdir;
            //calculate Light Projection:
            Vector3 pos, normal;
            illuminationPoint ip;
            int frame;
            //step through the layout
            for (int x = 0; x < track.layout.segments; x++)
                for (int y = 0; y < track.layout.tiles; y++)
                {

                    //got hole?
                    if (track.layout.hasEffect(y, x, TrackFactory.effect_type.BACK)) continue;
                    //get position of the hole
                    getWC4TD(track.layout, x, y, track.TangentFrames, out frame, out pos, out normal);
                    ip = new illuminationPoint();
                    //get Lightdir and color
                    getLightParams(pos, out lightdir, out ip.color);
                    //if angle lightdir normal >90 no light will shine in the track
                    if (Vector3.Dot(lightdir, normal) <= 0) continue;
                    ip.positionWC = pos;
                    //find out there the light intersects the track
                    trackIntersect(ref ip, lightdir, normal, pos, track.TangentFrames, frame, width, height);

                    map.illumPoint.Add(ip);

                }
            map.generateLightmaps(out track.lightmap, out track.ambiancemap);
            track.lightdirmap = map.buildDirection(track.TangentFrames, track.length, 64);
#if XBOX
            
#else
            track.lightmap.Save("lightmap.bmp", ImageFileFormat.Bmp);
            track.lightdirmap.Save("directionmap.bmp", ImageFileFormat.Bmp);
            track.ambiancemap.Save("ambiancemap.bmp", ImageFileFormat.Bmp);
#endif
        }

        public static void defaultLightmaps(out Texture2D Diffuse, out Texture2D LightDir,Color ambientColor)
        {
            LightDir = World.Instance.WorldContent.Load<Texture2D>(Settings.Configuration.EngineSettings.TextureDirectory + "Track/" + "noNormal");
            Diffuse = new Texture2D(RenderManager.Instance.GraphicsDevice, 4,4, 0, ResourceUsage.AutoGenerateMipMap, SurfaceFormat.Color);
            Color[] lmd = new Color[4*4];
            for (int i = 0; i < lmd.Length; i++)
                lmd[i] = ambientColor;
            Diffuse.SetData(lmd);
        }
                /// <summary>
        /// a illumination point describes an area on the track that is lit by the primary light source
        /// </summary>
        class illuminationPoint : IComparable
        {
            public int positionX; //position with respect to the lightmap
            public int positionY; //position with respect to the lightmap
            public Vector3 positionWC;
            public lightmapdata color;
            public float intensity; //used to weight the impact this secondary light source has in the direction map
            public int frame;

            public int CompareTo(object obj)
            {
                int c = positionX.CompareTo(((illuminationPoint)obj).positionX);
                if (c == 0)
                    return positionY.CompareTo(((illuminationPoint)obj).positionY);
                return c;
            }
        }
        struct lightmapdata
        {
            public float R;
            public float G;
            public float B;

            public lightmapdata(float r, float g, float b)
            {
                this.R = r;
                this.B = b;
                this.G = g;
            }
            public void mul(float val)
            {
                this.R *= val;
                this.G *= val;
                this.B *= val;
            }

            public void div(float val)
            {
                this.R /= val;
                this.G /= val;
                this.B /= val;
            }

            public void set(float r, float g, float b)
            {
                this.R = r;
                this.G = g;
                this.B = b;
            }

            public void set(Vector3 v)
            {
                this.R = v.X;
                this.G = v.Y;
                this.B = v.Z;
            }

            public void add(Vector3 v)
            {
                this.R += v.X;
                this.G += v.Y;
                this.B += v.Z;
            }

            public void add(ref lightmapdata lmd, float factor)
            {
                this.R += lmd.R * factor;
                this.G += lmd.G * factor;
                this.B += lmd.B * factor;
            }
            public void normalize()
            {
                this.R = OurMath.MIN2(R, 1);
                this.G = OurMath.MIN2(G, 1);
                this.B = OurMath.MIN2(B, 1);
            }

        }
        class LMMask
        {
            public float[] mask;
            public int width;
            public int height;


            public LMMask(int width, int height)
            {
                this.width = width;
                this.height = height;
                this.mask = new float[width * height];
            }

            public void setValue(int x, int y, float val)
            {

                mask[y * width + x] = val;
            }

            /// <summary>
            /// apply this mask to a light map
            /// </summary>
            /// <param name="map"></param>
            /// <param name="startX"></param>
            /// <param name="startY"></param>
            /// <param name="color"></param>
            public void applyMask(LightMap map, int startX, int startY, lightmapdata color)
            {
                startX -= width / 2;
                startY -= height / 2;
                for (int y = 0; y < height; y++)
                    for (int x = 0; x < width; x++)
                        map.addColor((startX + x), (startY + y), ref  color, mask[y * width + x]);
            }

            public static LMMask build(int mapHeight, float reflectionIntensity, int ratio)
            {
                LMMask mask = new LMMask(mapHeight * ratio, mapHeight);
                float factorX, factorY, factor;
                float fratio = (float)ratio;
                for (int y = 0; y < mapHeight; y++)
                {
                    factorY = (float)(1.0f - Math.Sin((((float)(y)) / ((float)mapHeight)) * (float)Math.PI));
                    factorY *= factorY;
                    for (int x = 0; x < mapHeight * ratio; x++)
                    {
                        factorX = (float)(Math.Sin((((float)(x)) / ((float)mapHeight * fratio)) * (float)Math.PI));
                        factorX *= factorX;
                        factor = (factorX * factorY * 0.8f + factorX * 0.2f) * reflectionIntensity;
                        mask.setValue(x, y, factor);
                    }
                }
                return mask;
            }
        }


        /// <summary>
        /// this class describes a lightmap.
        /// it contains the ambient map as well as the light direction map
        /// </summary>
        class LightMap
        {
            public lightmapdata[] diffuseMap;
            public lightmapdata[] directionMap;
            public lightmapdata[] ambianceMap;
            public List<illuminationPoint> illumPoint;
            public int width;
            public int height;
            public int height_dir;
            public float maxIntensity;

            public LightMap(int width, int height, float r, float g, float b, float maxIntensity)
            {
                this.maxIntensity = maxIntensity;
                illumPoint = new List<illuminationPoint>();
                this.width = width;
                this.height = height;
                this.height_dir = height;
                diffuseMap = new lightmapdata[width * height];
                directionMap = new lightmapdata[width * height_dir];
                ambianceMap = new lightmapdata[width];
                for (int i = 0; i < diffuseMap.Length; i++)
                    diffuseMap[i].set(r, g, b);
            }

            public void normalize()
            {
                for (int i = 0; i < diffuseMap.Length; i++)
                {
                    diffuseMap[i].R = OurMath.CLAMP(diffuseMap[i].R, 0, maxIntensity);
                    diffuseMap[i].G = OurMath.CLAMP(diffuseMap[i].G, 0, maxIntensity);
                    diffuseMap[i].B = OurMath.CLAMP(diffuseMap[i].B, 0, maxIntensity);
                }
            }


            public void setColor(int x, int y, float r, float g, float b)
            {
                x = (x < 0) ? (width + x) : (x % width);
                y = (y < 0) ? height + y : (y % height);
                diffuseMap[y * width + x].set(r, g, b);
            }
            public void setDirection(int x, int y, Vector3 v)
            {
                x = (x < 0) ? (width + x) : (x % width);
                y = (y < 0) ? height + y : (y % height);
                directionMap[y * width + x].set((1.0f + v.X) * 0.5f, (1.0f + v.Y) * 0.5f, (1.0f + v.Z) * 0.5f);
            }

            public void addColor(int x, int y, ref lightmapdata color, float factor)
            {
                x = (x < 0) ? (width + x) : (x % width);
                y = (y < 0) ? height + y : (y % height);
                diffuseMap[y * width + x].add(ref color, factor);
            }

            public void generateLightmaps(out Texture2D diffuse,out Texture2D ambiance)
            {
                diffuse=buildDiffuse();
                ambiance = buildAmbiance();
            }
            
            

            protected Texture2D buildDiffuse()
            {
                Texture2D tex = new Texture2D(RenderManager.Instance.GraphicsDevice, width, height, 0, ResourceUsage.AutoGenerateMipMap, SurfaceFormat.Color);
                Color[] lmd = new Color[width * height];
                LMMask mask = LMMask.build(height, maxIntensity, 4);

                foreach (illuminationPoint i in illumPoint)
                {
                    mask.applyMask(this, i.positionX, i.positionY, i.color);
                    //setColor(i.positionX, i.positionY, i.color.R, i.color.G, i.color.B);
                }
                normalize();
                for (int i = 0; i < lmd.Length; i++)
                {
                    lmd[i] = new Color((byte)(255 * diffuseMap[i].R), (byte)(255 * diffuseMap[i].G), (byte)(255 * diffuseMap[i].B), 0);
                }
                tex.SetData(lmd);
                return tex;
            }

            protected Texture2D buildAmbiance()
            {
                Texture2D tex = new Texture2D(RenderManager.Instance.GraphicsDevice, width, 1, 0, ResourceUsage.AutoGenerateMipMap, SurfaceFormat.Color);
                Color[] lmd = new Color[width];
                int index;

                for (int x = 0; x < width; x++)
                {
                    for(int y=0;y<height;y++)
                    {
                        index = y * width + x;
                        ambianceMap[x].add(ref diffuseMap[index],1);
                    }
                    ambianceMap[x].div(height);
                }

                for (int i = 0; i < lmd.Length; i++)
                {
                    lmd[i] = new Color((byte)(255 * ambianceMap[i].R), (byte)(255 * ambianceMap[i].G), (byte)(255 * ambianceMap[i].B), 0);
                }
                tex.SetData(lmd);
                return tex;
            }

            protected Vector3[] buildRing()
            {
                Vector3[] ring = new Vector3[height_dir];
                float hOverPi = (float)((Math.PI * 2.0f) / height_dir);
                for (int i = 0; i < height; i++)
                    //ring[i] = new Vector3(0, 0, 1);
                    ring[i] = Vector3.Normalize(new Vector3(0, (float)Math.Sin(i * hOverPi), (float)Math.Cos((i * hOverPi) - Math.PI) + 1.0f));
                ring[0] = new Vector3();
                return ring;
            }

            protected struct secondaryLight
            {
                public int X;
                public int Y;
                public int nextSecondary;
                public int lastSecondary;
                public float intensity;
                public int frame;
                public secondaryLight(illuminationPoint ip)
                {
                    this.X = ip.positionX;
                    this.Y = ip.positionY;
                    this.intensity = ip.intensity;
                    this.nextSecondary = 0;
                    this.lastSecondary = 0;
                    this.frame = ip.frame;
                }
            }


            public Texture2D buildDirection(Splines.PositionTangentUpRadius[] tfl, float lengthOfSpline, int maxdist)
            {
                Texture2D tex = new Texture2D(RenderManager.Instance.GraphicsDevice, width, height_dir, 0, ResourceUsage.None, SurfaceFormat.Color);
                secondaryLight[] sl = new secondaryLight[width + 2];
                Vector3[] ring = buildRing();
                int currentx = -1;
                int xdiff, ydiff, framediff;
                float distance_rel, relpos, factor;
                int currentframe = -1;
                List<int> nextip = new List<int>();
                Color[] lmd = new Color[width * height_dir];
                //sort illumination points according to position on the lightmap
                illumPoint.Sort();
                //unification: if more than one illuminationpoints share the same x coordiante, compute an average
                for (int i = 0; i < illumPoint.Count; i++)
                {
                    if (illumPoint[i].positionX <0|| illumPoint[i].positionX == currentx || illumPoint[i].frame == currentframe) continue;
                    for (int j = (currentx + 1); j < illumPoint[i].positionX; j++)
                    {
                        sl[j + 1] = new secondaryLight();
                    }
                    currentx = illumPoint[i].positionX;
                    currentframe = illumPoint[i].frame;
                    nextip.Add(currentx+1);
                    sl[currentx + 1] = new secondaryLight(illumPoint[i]);
                }
                //makes the traversal easier. we wont have to worry about the first and last element any more
                sl[0] = new secondaryLight(illumPoint[illumPoint.Count-1]);
                sl[0].X =sl[0].X-width;
                sl[width + 1] = new secondaryLight(illumPoint[0]);
                sl[width + 1].X = width + sl[width + 1].X;
                nextip.Insert(0, 0);
                nextip.Add(width + 1);

                for (int i = 0; i < nextip.Count - 1; i++)
                {
                    sl[nextip[i]].lastSecondary = nextip[i];
                    sl[nextip[i]].nextSecondary = nextip[i+1];
                    //is interpolation needed?
                    if (nextip[i + 1] - nextip[i] > 1)
                    {
                        relpos = tfl[sl[nextip[i]].frame].relPos;
                        //the relative distance between the illumpoints
                        distance_rel = tfl[sl[nextip[i + 1]].frame].relPos - relpos;
                        //the differenc between the x, y coordinates of the illumination points
                        xdiff = sl[nextip[i + 1]].X - sl[nextip[i]].X;
                        ydiff = sl[nextip[i + 1]].Y - sl[nextip[i]].Y;
                        framediff = sl[nextip[i + 1]].frame - sl[nextip[i]].frame;
                        //interpolate the missing positions;
                        for (int j = nextip[i] + 1; j < nextip[i + 1]; j++)
                        {
                            factor = ((float)(j - nextip[i])) / ((float)(nextip[i + 1] - nextip[i]));
                            sl[j].frame = (int)(sl[nextip[i]].frame + framediff * factor);
                            sl[j].X = (int)(sl[nextip[i]].X + xdiff * factor);
                            sl[j].Y = (int)(sl[nextip[i]].Y + ydiff * factor);
                            sl[j].lastSecondary = nextip[i];
                            sl[j].nextSecondary = nextip[i + 1];
                        }
                    }
                }
                Vector3 lightDir;
                int dxnext, dxlast,ringlast,ringnext;
                float weightNext, weightLast, lengthOverRadius;
                //walk through ips
                //walk through ring
                //write biased lightdirection into directionmap
                for (int x = 1; x < sl.Length-1; x++)
                {
                    dxnext = sl[sl[x].nextSecondary].X - sl[x].X;
                    dxlast = sl[sl[x].lastSecondary].X - sl[x].X;
                    maxdist = sl[sl[x].nextSecondary].X - sl[sl[x].lastSecondary].X;
                    weightNext = OurMath.MAX2(0,((float)(maxdist-dxnext)) / ((float)maxdist));
                    //weightNext *= weightNext;
                    weightLast = OurMath.MAX2(0,((float)(maxdist+dxlast)) / ((float)maxdist));
                    //weightLast *= weightLast;
                    lengthOverRadius = (0.2f / tfl[sl[x].frame].Radius);
                    for (int y = 0; y < ring.Length; y++)
                    {
                        ringlast=(y + sl[sl[x].lastSecondary].Y)%height_dir;
                        ringnext=(y + sl[sl[x].nextSecondary].Y)%height_dir;
                        lightDir = Vector3.Normalize(new Vector3(dxlast * lengthOverRadius, ring[ringlast].Y, ring[ringlast].Z)) * weightLast;
                        lightDir += Vector3.Normalize(new Vector3(dxnext * lengthOverRadius, ring[ringnext].Y, ring[ringnext].Z)) * weightNext;
                        setDirection(x-1, 31 - y, Vector3.Normalize(lightDir));
                    }
                }

                //write texture
                for (int i = 0; i < lmd.Length; i++)
                    lmd[i] = new Color((byte)(255 * directionMap[i].R), (byte)(255 * directionMap[i].G), (byte)(255 * directionMap[i].B), 0);
                tex.SetData(lmd);
                return tex;
            }
        }









        

        /// <summary>
        /// get the light parameters. hardcoded for now
        /// BUG: for some reason a track is created befor the world is ready
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="lightdir"></param>
        /// <param name="color"></param>
        private static void getLightParams(Vector3 pos, out Vector3 lightdir, out lightmapdata color)
        {
            lightdir = World.Instance.Sunlight.Direction;
            //I've no goddam idea why the f*** we have to invert the lightdirection. but it works and as long as it does i don't give a sh**
            lightdir *= -1.0f;
            color = new lightmapdata(1.0f, 1.0f, 1.0f);
        }




        /// <summary>
        /// detectes where light falling through a hole in the track will hit the track
        /// </summary>
        /// <param name="illumpoint">the result</param>
        /// <param name="lightdir">the direction of the light</param>
        /// <param name="normal">the normal of the hole</param>
        /// <param name="pos">the position of the hole in world coordinates</param>
        /// <param name="tfl">a list of all tangentframes</param>
        /// <param name="frame">the frame that defines the segment in which the hole lies</param>
        /// <param name="width">the width of the lightmap</param>
        /// <param name="height">the height of the light map</param>
        private static void trackIntersect(ref illuminationPoint illumpoint, Vector3 lightdir, Vector3 normal, Vector3 pos, Splines.PositionTangentUpRadius[] tfl, int frame, int width, int height)
        {

            float l_in = 0, l_out = 0;

            float l = 0;
            int direction, f;
            int offset = frame;
            Vector3 ip = new Vector3();

            bool intersect, hit = false;

            direction = Math.Sign(Vector3.Dot(tfl[frame].Tangent, lightdir));

            //find the track segment in which the ray hits the trackwall

            if (direction > 0)
            {
                for (f = frame+1; !hit && (f < tfl.Length); f++)
                {
                    intersect = OurMath.rayPlaneIntersect(pos, lightdir, tfl[f].Tangent, tfl[f].Position, ref ip, ref l);
                    if (!intersect || (((ip - tfl[f].Position).LengthSquared()) > (tfl[f].Radius * tfl[f].Radius)))
                    { hit = true; frame = f; }
                }
                for (f = 0; !hit && (f < frame); f++)
                {
                    intersect = OurMath.rayPlaneIntersect(pos, lightdir, tfl[f].Tangent, tfl[f].Position, ref ip, ref l);
                    if (!intersect || (((ip - tfl[f].Position).LengthSquared()) > (tfl[f].Radius * tfl[f].Radius)))
                    { hit = true; frame = f; }
                }
                frame--;
                //illumpoint.color = new lightmapdata(1, 0, 0);
                
            }
            else if (direction < 0)
            {
                for (f = frame; !hit && (f >= 0); f--)
                {
                    intersect = OurMath.rayPlaneIntersect(pos, lightdir, tfl[f].Tangent, tfl[f].Position, ref ip, ref l);
                    if (!intersect || (((ip - tfl[f].Position).LengthSquared()) > (tfl[f].Radius * tfl[f].Radius)))
                    { hit = true; frame = f; }
                }
                for (f = tfl.Length - 1; !hit && (f > frame); f--)
                {
                    intersect = OurMath.rayPlaneIntersect(pos, lightdir, tfl[f].Tangent, tfl[f].Position, ref ip, ref l);
                    if (!intersect || (((ip - tfl[f].Position).LengthSquared()) > (tfl[f].Radius * tfl[f].Radius)))
                    { hit = true; frame = f; }
                }
               
                //illumpoint.color = new lightmapdata(1, 0, 0);
                
            }


            OurMath.rayCylinderIntersect(pos, lightdir, tfl[frame].Position, tfl[frame].Tangent, tfl[frame].Radius, ref l_in, ref l_out);
            illumpoint.positionWC = pos + (l_out) * lightdir;
            getLM4WC(ref illumpoint, tfl, frame, height, width);
            illumpoint.positionY = 31 - illumpoint.positionY;
            illumpoint.frame = frame;
            shade(ref illumpoint, normal, lightdir);
        }

        private static void shade(ref illuminationPoint ip, Vector3 normal, Vector3 lightdir)
        {
            float val = OurMath.CLAMP(Vector3.Dot(normal, lightdir), 0, 1);
            ip.color.set(ip.color.R * val, ip.color.G * val, ip.color.B * val);
            ip.intensity = val;
        }



        /// <summary>
        /// computes the lightmap position of a point given in world coordinates and a known frame.
        /// </summary>
        /// <param name="ip">the illumiation point that holds the point in world coordiantes. will contain the output</param>
        /// <param name="tfl">list of all tangent frames</param>
        /// <param name="frame">the frame that defines the segment in which the point lies</param>
        /// <param name="height">hight of the lightmap</param>
        /// <param name="width">width of the lightmap</param>
        private static void getLM4WC(ref illuminationPoint ip, Splines.PositionTangentUpRadius[] tfl, int frame, int height, int width)
        {
            int nextframe = (frame + 1) % tfl.Length;
            Vector3 dir = (ip.positionWC - tfl[frame].Position);
            Vector3 dirOnTangent = OurMath.projectVectorOntoVector(dir, tfl[frame].Tangent);
            dir = Vector3.Normalize(dir - dirOnTangent);
            Vector3 bitangent = Vector3.Cross(tfl[frame].Up, tfl[frame].Tangent);
            float dot = OurMath.MIN2(1.0f, Vector3.Dot(dir, tfl[frame].Up));

            float angle = ((float)Math.Acos(dot));
            angle = (Vector3.Dot(dir, bitangent) < 0) ? (OurMath.RAD360 - angle) : (angle);
            float ratio = dirOnTangent.Length() / ((tfl[nextframe].Position - tfl[frame].Position).Length());
            ip.positionY = (int)(angle * (height / OurMath.RAD360));
            ip.positionX = (int)((width - 1) * ((1.0f - ratio) * (tfl[nextframe].relPos - tfl[frame].relPos) + tfl[frame].relPos));

        }

        /// <summary>
        /// computes the center of a tile of the track in world coordinates
        /// </summary>
        /// <param name="tl">the layout of the track</param>
        /// <param name="x">the x coordiante of the tile</param>
        /// <param name="y">the y coordiante of the tile</param>
        /// <param name="TangentFrameList">a list of all tangent frames</param>
        /// <param name="frame">output: the frame that defines the segment holding the tile </param>
        /// <param name="posWC">output: the center of the tile in world coordinates</param>
        /// <param name="normal">output: the normal of the tile</param>
        protected static void getWC4TD(TrackLayout tl, int x, int y, Splines.PositionTangentUpRadius[] TangentFrameList, out int frame, out Vector3 posWC, out Vector3 normal)
        {
            Vector3 pos1, pos2;
            Matrix rot;
            float angle;
            frame = 0;
            int frameCount = 0;
            //find the frame closest to the center of the hole
            findFrames(tl.segmentLength * ((float)x), tl.segmentLength, ref TangentFrameList, ref frame, ref frameCount);
            frame += frameCount / 2;
            //frame++;
            angle = (OurMath.RAD360 / ((float)tl.tiles)) * ((float)(y) + 0.5f);
            rot = Matrix.CreateFromAxisAngle(TangentFrameList[frame].Tangent, angle);
            //use the sucker
            normal = Vector3.Transform(TangentFrameList[frame].Up, rot);
            rot = Matrix.CreateFromAxisAngle(TangentFrameList[frame + 1].Tangent, angle);
            //use our shiny new normal to compute pos
            pos1 = TangentFrameList[frame].Position + normal * TangentFrameList[frame].Radius;
            pos2 = TangentFrameList[frame + 1].Position + Vector3.Transform(TangentFrameList[frame + 1].Up, rot) * TangentFrameList[frame + 1].Radius;
            posWC = pos1 + (pos2 - pos1) * 0.5f;
            normal *= -1.0f;
        }
        protected static void findFrames(float segmentOffset, float segmentLength, ref Splines.PositionTangentUpRadius[] tfl, ref int startFrame, ref int frameCount)
        {
            //slow as hell. should make use of last result...
            for (startFrame = 0; tfl[startFrame + 1].relPos < segmentOffset; startFrame++) ;
            for (frameCount = 1; (tfl[startFrame + 1 + frameCount].relPos) < (segmentOffset + segmentLength); frameCount++) ;
        }

    }
}
