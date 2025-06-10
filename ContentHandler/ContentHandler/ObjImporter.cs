using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using System.IO;

namespace PraktWS0708.ContentHandler
{
    public struct Vector3int
    {
        public int x;
        public int y;
        public int z;
    }

    public struct FaceIndex
    {
        public Vector3int vertex;
        public Vector3int normal;
        public Vector3int texture;
    }

    [ContentImporterAttribute(".obj", DefaultProcessor = "ObjProcessor")]
    public class ObjImporter : ContentImporter<MeshData[][]>
    {
        //private MeshBuilder builder;
        private MeshData[][] meshData;

        private Vector3[] vertices;
        private Vector3[] normals;
        private Vector2[] textures;
        private FaceIndex[][] faces;

        private String value;

        private int vertices_count = 0;
        private int normals_count = 0;
        private int textures_count = 0;
        private int faces_count = 0;
        private int objects_count = 0;

        private List<int> object_count_list;

        //private int chNormals;
        //private int chTextures;

        //private NodeContent rootNode = new NodeContent();

        System.IO.StreamReader reader;
       
        public override MeshData[][] Import(String filename, ContentImporterContext context)
        {  
           //Logger.StartLog("log.txt");

           try
           {
                //builder = MeshBuilder.StartMesh("Mesh");
                //chNormals = builder.CreateVertexChannel<Vector3>(VertexChannelNames.Normal);
                //chTextures = builder.CreateVertexChannel<Vector2>(VertexChannelNames.TextureCoordinate(0));
               
                // 1.Pass: Abzählen der Vertices, Normals, Textures und Faces
                // 2.Pass: Einlesen selbiger
                for(int pass = 0; pass < 2; pass++)
                {
                    reader = new System.IO.StreamReader(filename);

                    if (pass == 0)
                    {
                        object_count_list = new List<int>();
                    }
                    else if (pass == 1)
                    {
                        vertices = new Vector3[vertices_count];
                        normals = new Vector3[normals_count];
                        textures = new Vector2[textures_count];
                        faces = new FaceIndex[objects_count][];
                      
                        object_count_list.Add(faces_count);

                        meshData = new MeshData[object_count_list.Count][];

                        for (int i = 0; i < objects_count; i++)
                        {
                            faces[i] = new FaceIndex[object_count_list[i]];
                            meshData[i] = new MeshData[object_count_list[i] * 3];
                        }

                        vertices_count = 0;
                        normals_count = 0;
                        textures_count = 0;
                        faces_count = 0;
                        objects_count = 0;
                    }

                    while((value = reader.ReadLine()) != null)
                    {
                        char[] seperator_space = {' '};
                        String[] parts = value.Split(seperator_space);

                        System.Globalization.NumberFormatInfo nfi = new System.Globalization.NumberFormatInfo();
                        nfi.NumberDecimalSeparator = ".";
                       
                        if (parts[0] == "#")
                        {
                            continue;
                        }
                        else if (parts[0] == "v")
                        {
                            if (pass == 0)
                            {
                                vertices_count++;
                            }
                            else if (pass == 1)
                            {
                                //parts[1..2] sind Vertices
                                vertices[vertices_count] = new Vector3(float.Parse(parts[1], nfi), float.Parse(parts[2], nfi), float.Parse(parts[3], nfi));
                                vertices_count++;
                            }                            
                        }
                        else if (parts[0] == "vt")
                        {
                            if (pass == 0)
                            {
                                textures_count++;
                            }
                            else if (pass == 1)
                            {
                                //parts[1..2] sind Textures
                                textures[textures_count] = new Vector2(float.Parse(parts[1], nfi), float.Parse(parts[2], nfi));
                                textures_count++;
                            }   
                        }
                        else if (parts[0] == "vn")
                        {
                            if (pass == 0)
                            {
                                normals_count++;
                            }
                            else if (pass == 1)
                            {
                                //parts[1..3] sind Normals
                                normals[normals_count] = new Vector3(float.Parse(parts[1], nfi), float.Parse(parts[2], nfi), float.Parse(parts[3], nfi));
                                normals_count++;
                            }
                        }
                        else if (parts[0] == "g")
                        {
                            //parts[1] ist Modelname
                            if (pass == 0)
                            {
                                if (objects_count > 0)
                                {
                                    object_count_list.Add(faces_count);
                                }
                                objects_count++;
                                faces_count = 0;
                            }
                            else if(pass == 1)
                            {
                                objects_count++;
                                faces_count = 0;
                            }
                        }
                        else if (parts[0] == "f")
                        {
                            if (pass == 0)
                            {
                                faces_count++;
                            }
                            else if (pass == 1)
                            {
                                char[] seperator_slash = {'/'};

                                String[] vert1 = parts[1].Split(seperator_slash);
                                String[] vert2 = parts[2].Split(seperator_slash);
                                String[] vert3 = parts[3].Split(seperator_slash);

                                //vert1[0], vert2[0], vert3[0] sind die Vertices von Face
                                //vert1[1], vert2[1], vert3[1] sind die Textures von Face
                                //vert1[2], vert2[2], vert3[2] sind die Normals von Face
                                faces[objects_count - 1][faces_count].vertex.x = int.Parse(vert1[0]);
                                faces[objects_count - 1][faces_count].vertex.y = int.Parse(vert2[0]);
                                faces[objects_count - 1][faces_count].vertex.z = int.Parse(vert3[0]);

                                faces[objects_count - 1][faces_count].texture.x = int.Parse(vert1[1]);
                                faces[objects_count - 1][faces_count].texture.y = int.Parse(vert2[1]);
                                faces[objects_count - 1][faces_count].texture.z = int.Parse(vert3[1]);

                                faces[objects_count - 1][faces_count].normal.x = int.Parse(vert1[2]);
                                faces[objects_count - 1][faces_count].normal.y = int.Parse(vert2[2]);
                                faces[objects_count - 1][faces_count].normal.z = int.Parse(vert3[2]); 

                                faces_count++; 
                            }
                        }
                        else
                        {
                            //Logger.LogIt("Parsing Error in pass " + pass.ToString() + "!");
                        }

                        
                    }
                    //Logger.LogIt("pass: " + pass.ToString() + " vertices: " + vertices_count.ToString() + " normals: " + normals_count.ToString() + " textures: " + textures_count.ToString() +
                    //    " Objects: " + objects_count.ToString());
                    reader.Close();
                }
                //Logger.LogIt(faces.Length.ToString());

         
             /* for (int i = 0; i < faces_count; i++)
                {
                    builder.CreatePosition(vertices[faces[i].vertex.x - 1]);
                    builder.CreatePosition(vertices[faces[i].vertex.y - 1]);
                    builder.CreatePosition(vertices[faces[i].vertex.z - 1]);
                }
              
                for (int i = 0; i < faces_count; i+=3)
                {
                    builder.SetVertexChannelData(chNormals, normals[faces[i].normal.x - 1]);
                    builder.SetVertexChannelData(chTextures, textures[faces[i].texture.x - 1]);
                    builder.AddTriangleVertex(i);
                    builder.SetVertexChannelData(chNormals, normals[faces[i].normal.y - 1]);
                    builder.SetVertexChannelData(chTextures, textures[faces[i].texture.y - 1]);
                    builder.AddTriangleVertex(i + 1);
                    builder.SetVertexChannelData(chNormals, normals[faces[i].normal.z - 1]);
                    builder.SetVertexChannelData(chTextures, textures[faces[i].texture.z - 1]);
                    builder.AddTriangleVertex(i + 2);
                } */

                for (int j = 0; j < object_count_list.Count; j++)
                {
                    for (int i = 0; i < object_count_list[j] * 3; i += 3)
                    {
                        meshData[j][i].normal = normals[faces[j][i / 3].normal.x - 1];
                        meshData[j][i].texture = textures[faces[j][i / 3].texture.x - 1];
                        meshData[j][i].position = vertices[faces[j][i / 3].vertex.x - 1];
                        meshData[j][i + 1].normal = normals[faces[j][i / 3].normal.y - 1];
                        meshData[j][i + 1].texture = textures[faces[j][i / 3].texture.y - 1];
                        meshData[j][i + 1].position = vertices[faces[j][i / 3].vertex.y - 1];
                        meshData[j][i + 2].normal = normals[faces[j][i / 3].normal.z - 1];
                        meshData[j][i + 2].texture = textures[faces[j][i / 3].texture.z - 1];
                        meshData[j][i + 2].position = vertices[faces[j][i / 3].vertex.z - 1];
                    }
                }
                //Logger.LogIt(meshData.Length.ToString());

                //MeshContent mesh = builder.FinishMesh();
                //rootNode.Children.Add(mesh);
            }
            catch (Exception ex)
            {
                Logger.LogIt("Error: " + ex.Message + ex.StackTrace);
            }
            finally
            {
            //Logger.EndLog();
            }            
            //return rootNode;
            return meshData;
        }
    }
}
