using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PraktWS0708.Utils
{

    //this class holds functions to create geometry
    class MeshFactory
    {

        #region tube
      

        //B2A3a,b  B3A1a
        /// <summary>
        /// creates a tube from a list of tangent frames.
        /// </summary>
        /// <param name="tfl">list of tangent frames from which the tube should be created</param>
        /// <param name="radiuslist">contains the radii for each part of the tube</param>
        /// <param name="segments"> the number of segments each part of the tube should have</param>
        /// <param name="geometry">this list will hold the geometry</param>
        /// <param name="close">should the tube be closed or not</param>
        public static void splineExpander(List<Splines.PositionTangentUpRadius> tfl,
                                          int tiles, 
                                          ref List<VertexPositionNormalTextureTangent> geometry)
        {
            //weniger als 3 tiles macht keinen sinn
            if (tiles < 3) tiles = 3;

            //die einzelnen zylinder bauen
            for (int i = 0; i < tfl.Count - 1; i++)
                buildCylinder(tfl[i], tfl[i + 1], tiles, ref geometry);

        }


        //B3A1a
        //B5A2b
        /// <summary>
        /// uses two tangent frames to build a cylinder
        ///
        /// </summary>
        /// <param name="f1"> first tangent frame</param>
        /// <param name="f2">second tangent frame</param>
        /// <param name="rad1">radius the cylinder should have arround the top</param>
        /// <param name="rad2">radius the cylinder should have arround the bottom</param>
        /// <param name="segments">number of segments the cylinder will have</param>
        /// <param name="vertexlist">a list to put the vertices in</param>
        public static void buildCylinder(Splines.PositionTangentUpRadius f1,
                                         Splines.PositionTangentUpRadius f2,
                                         int tiles,
                                         ref List<VertexPositionNormalTextureTangent> vertexlist)
        {
            //how often should the texture be repeated
            
            int startindex = vertexlist.Count;
            float angle = OurMath.RAD360 / (float)tiles;
            float vtexturecoord = 0;
            //matrizen, die die upvektoren der tangent frames entsprechen der anzahl der segmente drehen  
            Matrix rot1 = Matrix.CreateFromAxisAngle(f1.Tangent, angle);
            Matrix rot2 = Matrix.CreateFromAxisAngle(f2.Tangent, angle);
            Vector3 v1 = f1.Up, v2 = f2.Up;
            vertexlist.Add(new VertexPositionNormalTextureTangent(f2.Position + (v2 * f2.Radius), Vector3.Negate(v2), new Vector2(f2.relPos, 0), f2.Tangent));
            vertexlist.Add(new VertexPositionNormalTextureTangent(f1.Position + (v1 * f1.Radius), Vector3.Negate(v1), new Vector2(f1.relPos, 0), f1.Tangent));

            for (int i = 1; i < tiles; i++)
            {
                v1 = Vector3.Transform(v1, rot1); v2 = Vector3.Transform(v2, rot2);
                vtexturecoord = ((float)i / (float)tiles);
                vertexlist.Add(new VertexPositionNormalTextureTangent(f2.Position + (v2 * f2.Radius), Vector3.Negate(v2), new Vector2(f2.relPos, vtexturecoord), f2.Tangent));
                vertexlist.Add(new VertexPositionNormalTextureTangent(f1.Position + (v1 * f1.Radius), Vector3.Negate(v1), new Vector2(f1.relPos, vtexturecoord), f1.Tangent));

            }
            //oberfläche schliesen
            vertexlist.Add(new VertexPositionNormalTextureTangent(vertexlist[startindex].Position,
                                                             vertexlist[startindex].Normal,
                                                             new Vector2(f2.relPos, 1),
                                                             f2.Tangent));
            vertexlist.Add(new VertexPositionNormalTextureTangent(vertexlist[startindex + 1].Position,
                                                             vertexlist[startindex + 1].Normal,
                                                            new Vector2(f1.relPos, 1),
                                                            f1.Tangent));

            //anschluss für neuen strip
            vertexlist.Add(vertexlist[startindex]);
            vertexlist.Add(vertexlist[startindex]);
        }


        //B2A3a,b  B3A1a
        /// <summary>
        /// creates a tube from a list of tangent frames.
        /// </summary>
        /// <param name="tfl">list of tangent frames from which the tube should be created</param>
        /// <param name="radiuslist">contains the radii for each part of the tube</param>
        /// <param name="segments"> the number of segments each part of the tube should have</param>
        /// <param name="geometry">this list will hold the geometry</param>
        /// <param name="close">should the tube be closed or not</param>
        public static void splineExpander2(List<Splines.PositionTangentUpRadius> tfl,
                                          int faces,
                                          ref List<VertexPositionNormalTextureTangent> geometry)
        {
            //weniger als 3 segmente macht keinen sinn
            if (faces < 3) faces = 3;

            //die einzelnen zylinder bauen
            for (int i = 0; i < tfl.Count-1; i++)
                buildCylinder2(tfl[i], faces, ref geometry);
            if (Vector3.Dot(tfl[tfl.Count - 1].Up, tfl[tfl.Count - 2].Up)<0)
                tfl[tfl.Count - 1].Up *= -1.0f;
            buildCylinder2(tfl[tfl.Count - 1], faces, ref geometry);
        }


        //B3A1a
        //B5A2b
        /// <summary>
        /// uses two tangent frames to build a cylinder
        ///
        /// </summary>
        /// <param name="f1"> first tangent frame</param>
        /// <param name="f2">second tangent frame</param>
        /// <param name="rad1">radius the cylinder should have arround the top</param>
        /// <param name="rad2">radius the cylinder should have arround the bottom</param>
        /// <param name="segments">number of segments the cylinder will have</param>
        /// <param name="vertexlist">a list to put the vertices in</param>
        public static void buildCylinder2(Splines.PositionTangentUpRadius tframe,
                                          int faces,
                                          ref List<VertexPositionNormalTextureTangent> vertexlist)
        {
            //how often should the texture be repeated

            int startindex = vertexlist.Count;
            float angle = OurMath.RAD360 / (float)faces;
            float vtexturecoord = 0;
            //matrizen, die die upvektoren der tangent frames entsprechen der anzahl der segmente drehen  
            Matrix rot1 = Matrix.CreateFromAxisAngle(tframe.Tangent, angle);
            Vector3 v1 = tframe.Up;
            vertexlist.Add(new VertexPositionNormalTextureTangent(tframe.Position + (v1 * tframe.Radius), Vector3.Negate(v1), new Vector2(tframe.relPos, 0), tframe.Tangent));

            for (int i = 1; i < faces; i++)
            {
                v1 = Vector3.Transform(v1, rot1); 
                vtexturecoord = ((float)i / (float)faces);
                vertexlist.Add(new VertexPositionNormalTextureTangent(tframe.Position + (v1 * tframe.Radius), Vector3.Negate(v1), new Vector2(tframe.relPos, vtexturecoord), tframe.Tangent));

            }

            vertexlist.Add(new VertexPositionNormalTextureTangent(vertexlist[startindex].Position,
                                                                  vertexlist[startindex].Normal,
                                                                  new Vector2(tframe.relPos, 1),
                                                                  tframe.Tangent));
        }

       

        #endregion

    }
}
