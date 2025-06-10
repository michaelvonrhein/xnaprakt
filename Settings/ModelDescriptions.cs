using System;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Threading;
using System.IO;
using System.Xml.Serialization;
using System.Xml;
using PraktWS0708.Entities;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using PraktWS0708.Physics;
using PraktWS0708.Logic;
using PraktWS0708.Input;
using PraktWS0708.Rendering;
using PraktWS0708.ContentPipeline;
using PraktWS0708.Rendering.Effects;

namespace PraktWS0708.Settings
{
    public struct ModelDescription
    {
        public string typeName;
        public string modelName;
        public RenderingPlugin.RenderingPluginType RenderingPlugin;
        public RenderingPlugin.RenderingPluginData RenderingPluginData;
        public PhysicsPlugin.PhysicsPluginType PhysicsPlugin;
        public PhysicsPlugin.PhysicsPluginData PhysicsPluginData;
        public LogicPlugin.LogicPluginType LogicPlugin;
        public LogicPlugin.LogicPluginData LogicPluginData;
        public InputPlugin.InputPluginType InputPlugin;
        public Vector3 position;
        public float scale;
        public Matrix orientation;
        
        public ModelDescription(string typeName,
                                string modelName,
                                RenderingPlugin.RenderingPluginType renderingPlugin,
                                RenderingPlugin.RenderingPluginData renderingPluginData,
                                PhysicsPlugin.PhysicsPluginType physicsPlugin,
                                PhysicsPlugin.PhysicsPluginData physicsPluginData,
                                LogicPlugin.LogicPluginType logicPlugin,
                                LogicPlugin.LogicPluginData logicPluginData,
                                InputPlugin.InputPluginType inputPlugin)
        {
            this.typeName = typeName;
            this.modelName = modelName;
            this.RenderingPlugin = renderingPlugin;
            this.RenderingPluginData = renderingPluginData;
            this.PhysicsPlugin = physicsPlugin;
            this.PhysicsPluginData = physicsPluginData;
            this.LogicPlugin = logicPlugin;
            this.LogicPluginData = logicPluginData;
            this.InputPlugin = inputPlugin;
            this.position = Vector3.Zero;
            this.scale = 1f;
            this.orientation = Matrix.Identity;
        }
    }

    public class ModelDescriptions 
    {
        public static List<ModelDescription> md = new List<ModelDescription>();
        protected static Dictionary<string, ModelDescription> mddictionary = new Dictionary<string, ModelDescription>();
        private static Queue<String> loadingQueue = new Queue<String>();
        private static ContentManager content;
        private static int threadCount;
        private static List<bool> hang = new List<bool>();

        private Thread[] th;
        private static int affinity;

        public bool isLoaded = false;

        public ModelDescriptions()
        {
            /*if (World.Instance.WorldContent == null)
                World.Instance.WorldContent = new ContentManager();
                ContentInput input = World.Instance.WorldContent.Load<ContentInput>("Content/Models/test"); 
             */
        }
        /*
            new ModelDescription(
                "simpleShip",
                "p1_pencil",
                RenderingPlugin.RenderingPluginType.MainRenderer,
                PhysicsPlugin.PhysicsPluginType.SteeringBody,
                new PhysicsPlugin.PhysicsPluginData(1000f, Vector3.Zero,
                                                    new PhysicsPlugin.PhysicsPluginFlags(true, true, true),
                                                    Vector3.One * 2f, 0.004f, 0.003f, 0.8f),
                LogicPlugin.LogicPluginType.SHIP,
                InputPlugin.InputPluginType.PLAYER1),

            new ModelDescription(
                "RigidShip",
                "p1_saucer",
                RenderingPlugin.RenderingPluginType.MainRenderer,
                //PhysicsPlugin.PhysicsPluginType.RigidBody,
                PhysicsPlugin.PhysicsPluginType.RigidBody,
                new PhysicsPlugin.PhysicsPluginData(1000f, Vector3.Zero,
                                                    new PhysicsPlugin.PhysicsPluginFlags(true, true, true),
                                                    Vector3.One * 2f, 0.004f, 0.003f, 0.8f),
                LogicPlugin.LogicPluginType.SHIP,
                InputPlugin.InputPluginType.NULL),

            new ModelDescription(
                "AIShip",
                "p1_wedge",
                RenderingPlugin.RenderingPluginType.MainRenderer,
                //PhysicsPlugin.PhysicsPluginType.RigidBody,
                PhysicsPlugin.PhysicsPluginType.SteeringBody,
                new PhysicsPlugin.PhysicsPluginData(1000f, Vector3.Zero,
                                                    new PhysicsPlugin.PhysicsPluginFlags(true, true, true),
                                                    Vector3.One * 2f, 0.004f, 0.003f, 0.8f),
                LogicPlugin.LogicPluginType.SHIP,
                InputPlugin.InputPluginType.AI),
            
            new ModelDescription(
                "ghostShip",
                "p1_pencil",
                RenderingPlugin.RenderingPluginType.MainRenderer,
                PhysicsPlugin.PhysicsPluginType.Null,
                new PhysicsPlugin.PhysicsPluginData(1000f, Vector3.Zero,
                                                    new PhysicsPlugin.PhysicsPluginFlags(true, true, true),
                                                    Vector3.One * 3f, 0.004f, 0.003f, 0.001f),
                LogicPlugin.LogicPluginType.NULL, 
                InputPlugin.InputPluginType.NULL),

            new ModelDescription(
                "schanze",
                "schanze",
                RenderingPlugin.RenderingPluginType.MainRenderer,
                PhysicsPlugin.PhysicsPluginType.SolidBody,
                new PhysicsPlugin.PhysicsPluginData(1000f, Vector3.Zero,
                                                    new PhysicsPlugin.PhysicsPluginFlags(true, true, true),
                                                    Vector3.One * 3f, 0.004f, 0.003f, 0.001f),
                LogicPlugin.LogicPluginType.NULL, 
                InputPlugin.InputPluginType.NULL)
        }; */

        

        public bool ModelDescriptionForName(string typeName, out ModelDescription md)
        {
            return mddictionary.TryGetValue(typeName, out md);
        }

        #region Load/Save code
        /// <summary>
        /// Saves the current settings
        /// </summary>
        /// <param name="filename">The filename to save to</param>
        /* public void Save(string filename)
        {
            Stream stream = File.Create(filename);

            XmlSerializer serializer = new XmlSerializer(typeof(ModelDescriptions));
            serializer.Serialize(stream, this);
            stream.Close();
        }

        /// <summary>
        /// Loads settings from a file
        /// </summary>
        /// <param name="filename">The filename to load</param>
        public static ModelDescriptions Load(string filename)
        {
            ModelDescriptions md;
            Stream stream = File.OpenRead(filename);
            XmlSerializer serializer = new XmlSerializer(typeof(ModelDescriptions));
            md = (ModelDescriptions)serializer.Deserialize(stream);
            for (int i = 0; i < md.md.Length; i++)
                md.mddictionary.Add(md.md[i].typeName, md.md[i]);
            return md;
        }  */
        #endregion 

        public void LoadModels(String filename, ContentManager content)
        {
            List<String> models;

            md.Clear();
            mddictionary.Clear();

            Stream stream = File.OpenRead(filename);
            XmlSerializer serializer = new XmlSerializer(typeof(List<String>));

            models = (List<String>)serializer.Deserialize(stream);

            for (int i = 0; i < models.Count; i++)
            {
                LoadModel(models[i], content);
            }

            isLoaded = true;
        }

        private void LoadModel(String filename, ContentManager content)
        {
               ModelDescription newmd = new ModelDescription();
               ContentInput input = content.Load<ContentInput>(filename);

               newmd.typeName = input.GeneralInfo.TypeName;
               newmd.modelName = input.GeneralInfo.ModelName;

               switch (input.Rendering.Type)
               {
                   case "MainRenderer": newmd.RenderingPlugin = RenderingPlugin.RenderingPluginType.MainRenderer; break;
                   default: newmd.RenderingPlugin = RenderingPlugin.RenderingPluginType.MainRenderer; break;
               }

               MeshData[][] modelInput = content.Load<MeshData[][]>(input.Rendering.Paths["mesh"]);
               
               int length = 0;
               for(int i = 0; i < modelInput.Length; i++)
               {
                   length += modelInput[i].Length;
               }

               VertexPositionNormalTexture[] verticesTexture = new VertexPositionNormalTexture[length];
               BaseMeshPart[] baseMeshPart = new BaseMeshPart[modelInput.Length];
               
               int count = 0;
               for (int i = 0; i < modelInput.Length; i++)
               {
                   baseMeshPart[i] = new BaseMeshPart();
                   baseMeshPart[i].StartIndex = count;
                   baseMeshPart[i].PrimitiveCount = modelInput[i].Length / 3;
                   baseMeshPart[i].TextureParameters["baseMap"] = content.Load<Texture2D>(Configuration.EngineSettings.TextureDirectory + input.Rendering.Paths["tex" + (i + 1).ToString()]);

                   switch (input.Rendering.Paths["shader" + (i + 1).ToString()])
                   {    
                       case "Chrome": baseMeshPart[i].Effect = new ChromeEffect(); break;
                       case "Phong": baseMeshPart[i].Effect = new PhongEffect(); break;
                       case "Toon": baseMeshPart[i].Effect = new ToonEffect(); break;
                       case "Particle": baseMeshPart[i].Effect = new ParticleEffect(); break;
                       default: baseMeshPart[i].Effect = new PhongEffect(); break;
                   }
                   
                   if (input.Rendering.Paths.ContainsKey("bump" + (i + 1).ToString()))
                   {
                       baseMeshPart[i].TextureParameters["bumpMap"] = content.Load<Texture2D>(Configuration.EngineSettings.TextureDirectory + input.Rendering.Paths["bump" + (i + 1).ToString()]);
                   }

                   for(int j = 0; j < modelInput[i].Length; j++)
                   {
                       verticesTexture[count].Position = modelInput[i][j].position;
                       verticesTexture[count].Normal = modelInput[i][j].normal;
                       verticesTexture[count].TextureCoordinate = modelInput[i][j].texture;
                       count++;
                   }
               }

               VertexBuffer vb = new VertexBuffer(RenderManager.Instance.GraphicsDevice, VertexPositionNormalTexture.SizeInBytes * verticesTexture.Length, ResourceUsage.None, ResourceManagementMode.Automatic);
               vb.SetData<VertexPositionNormalTexture>(verticesTexture);

               RenderingPlugin.RenderingPluginData rpd = new RenderingPlugin.RenderingPluginData(new VertexDeclaration(RenderManager.Instance.GraphicsDevice, VertexPositionNormalTexture.VertexElements),
                                                                                                 vb,
                                                                                                 baseMeshPart,
                                                                                                 input.Rendering.MatrixParameters["orientation"]);
               
               newmd.RenderingPluginData = rpd;

               newmd.position = convertV4toV3(input.Rendering.VectorParameters["position"]);
               newmd.scale = input.Rendering.FloatParameters["scale"];
               newmd.orientation = input.Rendering.MatrixParameters["orientation"];
                                                                                       

               switch (input.Physics.Type)
               {
                   case "SolidBody": newmd.PhysicsPlugin = PhysicsPlugin.PhysicsPluginType.SolidBody; break;
                   case "RigidBody": newmd.PhysicsPlugin = PhysicsPlugin.PhysicsPluginType.RigidBody; break;
                   case "SteeringBody": newmd.PhysicsPlugin = PhysicsPlugin.PhysicsPluginType.SteeringBody; break;
                   case "PowerUp": newmd.PhysicsPlugin = PhysicsPlugin.PhysicsPluginType.PowerUp; break;
                   default: newmd.PhysicsPlugin = PhysicsPlugin.PhysicsPluginType.Null; break;
               }

               PhysicsPlugin.PhysicsPluginData ppd = new PhysicsPlugin.PhysicsPluginData(input.Physics.Mass,
                                                                                          input.Physics.MassCenter,
                                                                                          new PhysicsPlugin.PhysicsPluginFlags(input.Physics.Hover, input.Physics.Drag, input.Physics.Gravity),
                                                                                          input.Physics.DragFactor,
                                                                                          input.Physics.ThrustFactor,
                                                                                          input.Physics.SteeringFactor,
                                                                                          0.8f,
                                                                                          verticesTexture, input.Rendering.FloatParameters["scale"]);

        

               newmd.PhysicsPluginData = ppd;
               
               // logic plugin
               try
               {
                   newmd.LogicPlugin = (LogicPlugin.LogicPluginType)Enum.Parse(typeof(LogicPlugin.LogicPluginType), input.Logic.Type, false);
               }
               catch (Exception)
               {
                   newmd.LogicPlugin = LogicPlugin.LogicPluginType.NULL;
                   //TODO log or something?
               }

               //LogicPlugin.LogicPluginData lpd = new LogicPlugin.LogicPluginData(10f, 0.001f);
               LogicPlugin.LogicPluginData lpd = new LogicPlugin.LogicPluginData(input.Logic.Health, input.Logic.Damage, input.Logic.Position.Length,
                   input.Logic.Position, input.Logic.Orientation, input.Logic.Scale);

               lpd.name = input.GeneralInfo.ModelName;
               
               newmd.LogicPluginData = lpd;
               
               // input plugin
               try
               {
                   newmd.InputPlugin = (InputPlugin.InputPluginType)Enum.Parse(typeof(InputPlugin.InputPluginType), input.Input.Type, false);
               }
               catch (Exception)
               {
                   newmd.InputPlugin = InputPlugin.InputPluginType.NULL;
                   //TODO log or something?
               }

               md.Add(newmd);
               mddictionary.Add(newmd.typeName, md[md.Count - 1]);
        }
        //Multi-Threading nach Max Power
        public void LoadModelsMultiThread(String filename, ContentManager contentManager, int threadCounter)
        {
            content = contentManager;
            List<String> models;

            threadCount = threadCounter;

            md.Clear();
            mddictionary.Clear();

            Stream stream = File.OpenRead(filename);
            XmlSerializer serializer = new XmlSerializer(typeof(List<String>));

            models = (List<String>)serializer.Deserialize(stream);

            loadingQueue.Clear();

            for (int i = 0; i < models.Count; i++)
            {
                loadingQueue.Enqueue(models[i]);
            }

            th = new Thread[threadCount];

            affinity = 1;

            for (int i = 0; i < threadCount; i++)
            {
                th[i] = new Thread(loadModelMultiThread);
                th[i].Start();
            }
        }
    
        //ThreadStart loadModelMultiThread = delegate
        private void loadModelMultiThread()
        {
/*#if XBOX360
            //nice try
            if(affinity == 2) affinity++;
            if(affinity > 5) affinity = 1;
            Thread.CurrentThread.SetProcessorAffinity(new int[] { affinity });    
            affinity++;    
#endif */

            while (loadingQueue.Count > 0)
            {
                String workData = null;

                lock (loadingQueue)
                {
                    if (loadingQueue.Count > 0) workData = loadingQueue.Dequeue();
                    Console.WriteLine(workData);
                }

                if (workData != null)
                {
                    ModelDescription newmd = new ModelDescription();
                    ContentInput input = content.Load<ContentInput>(workData);
                    MeshData[][] modelInput;

                    newmd.typeName = input.GeneralInfo.TypeName;
                    newmd.modelName = input.GeneralInfo.ModelName;

                    switch (input.Rendering.Type)
                    {
                        case "MainRenderer": newmd.RenderingPlugin = RenderingPlugin.RenderingPluginType.MainRenderer; break;
                        case "ParticleSystem": newmd.RenderingPlugin = RenderingPlugin.RenderingPluginType.ParticleSystem; break;
                        default: newmd.RenderingPlugin = RenderingPlugin.RenderingPluginType.MainRenderer; break;
                    }
                    fail:
                    try
                    {
                        modelInput = content.Load<MeshData[][]>(input.Rendering.Paths["mesh"]);
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("Exception: " + workData);
                        goto fail;      //sorry
                    } 

                    int length = 0;
                    for (int i = 0; i < modelInput.Length; i++)
                    {
                        length += modelInput[i].Length;
                    }

                    VertexPositionNormalTexture[] verticesTexture = new VertexPositionNormalTexture[length];
                    BaseMeshPart[] baseMeshPart = new BaseMeshPart[modelInput.Length];

                    //hack
                    hang.Add(true);
                    lock(hang)
                    {

                        int count = 0;
                        for (int i = 0; i < modelInput.Length; i++)
                        {
                            baseMeshPart[i] = new BaseMeshPart();
                            baseMeshPart[i].StartIndex = count;
                            baseMeshPart[i].PrimitiveCount = modelInput[i].Length / 3;
                            baseMeshPart[i].TextureParameters["baseMap"] = content.Load<Texture2D>(Configuration.EngineSettings.TextureDirectory + input.Rendering.Paths["tex" + (i + 1).ToString()]);


                            switch (input.Rendering.Paths["shader" + (i + 1).ToString()])
                            {
                                case "Chrome": baseMeshPart[i].Effect = new ChromeEffect(); break;
                                case "Phong": baseMeshPart[i].Effect = new PhongEffect(); break;
                                case "Toon": baseMeshPart[i].Effect = new ToonEffect(); break;
                                case "Particle": baseMeshPart[i].Effect = new ParticleEffect(); break;
                                default: baseMeshPart[i].Effect = new PhongEffect(); break;
                            }


                            if (input.Rendering.Paths.ContainsKey("bump" + (i + 1).ToString()))
                            {
                                baseMeshPart[i].TextureParameters["bumpMap"] = content.Load<Texture2D>(Configuration.EngineSettings.TextureDirectory + input.Rendering.Paths["bump" + (i + 1).ToString()]);
                            }

                            for (int j = 0; j < modelInput[i].Length; j++)
                            {
                                verticesTexture[count].Position = modelInput[i][j].position;
                                verticesTexture[count].Normal = modelInput[i][j].normal;
                                verticesTexture[count].TextureCoordinate = modelInput[i][j].texture;
                                count++;
                            }
                        }

                    }

                    VertexBuffer vb = new VertexBuffer(RenderManager.Instance.GraphicsDevice, VertexPositionNormalTexture.SizeInBytes * verticesTexture.Length, ResourceUsage.None, ResourceManagementMode.Automatic);
                    vb.SetData<VertexPositionNormalTexture>(verticesTexture);

                    RenderingPlugin.RenderingPluginData rpd = new RenderingPlugin.RenderingPluginData(new VertexDeclaration(RenderManager.Instance.GraphicsDevice, VertexPositionNormalTexture.VertexElements),
                                                                                                      vb,
                                                                                                      baseMeshPart,
                                                                                                      input.Rendering.MatrixParameters["orientation"]);
                    newmd.RenderingPluginData = rpd;

                    newmd.position = convertV4toV3(input.Rendering.VectorParameters["position"]);
                    newmd.scale = input.Rendering.FloatParameters["scale"];
                    newmd.orientation = input.Rendering.MatrixParameters["orientation"];


                    switch (input.Physics.Type)
                    {
                        case "SolidBody": newmd.PhysicsPlugin = PhysicsPlugin.PhysicsPluginType.SolidBody; break;
                        case "RigidBody": newmd.PhysicsPlugin = PhysicsPlugin.PhysicsPluginType.RigidBody; break;
                        case "SteeringBody": newmd.PhysicsPlugin = PhysicsPlugin.PhysicsPluginType.SteeringBody; break;
                        case "PowerUp": newmd.PhysicsPlugin = PhysicsPlugin.PhysicsPluginType.PowerUp; break;
                        default: newmd.PhysicsPlugin = PhysicsPlugin.PhysicsPluginType.Null; break;
                    }

                    PhysicsPlugin.PhysicsPluginData ppd = new PhysicsPlugin.PhysicsPluginData(input.Physics.Mass,
                                                                                               input.Physics.MassCenter,
                                                                                               new PhysicsPlugin.PhysicsPluginFlags(input.Physics.Hover, input.Physics.Drag, input.Physics.Gravity),
                                                                                               input.Physics.DragFactor,
                                                                                               input.Physics.ThrustFactor,
                                                                                               input.Physics.SteeringFactor,
                                                                                               0.8f,
                                                                                               verticesTexture, input.Rendering.FloatParameters["scale"]);



                    newmd.PhysicsPluginData = ppd;

                    //switch (input.Logic.Type)
                    //{
                    //    case "WALL": newmd.LogicPlugin = LogicPlugin.LogicPluginType.WALL; break;
                    //    case "SHIP": newmd.LogicPlugin = LogicPlugin.LogicPluginType.SHIP; break;
                    //    case "BOMB": newmd.LogicPlugin = LogicPlugin.LogicPluginType.BOMB; break;
                    //    case "BANANE": newmd.LogicPlugin = LogicPlugin.LogicPluginType.BANANE; break;
                    //    case "SHIELD": newmd.LogicPlugin = LogicPlugin.LogicPluginType.SHIELD; break;
                    //    case "ROCKET": newmd.LogicPlugin = LogicPlugin.LogicPluginType.ROCKET; break;
                    //    case "HEALTHPACK": newmd.LogicPlugin = LogicPlugin.LogicPluginType.HEALTHPACK; break;
                    //    case "TURBO": newmd.LogicPlugin = LogicPlugin.LogicPluginType.TURBO; break;
                    //    default: newmd.LogicPlugin = LogicPlugin.LogicPluginType.NULL; break;
                    //}
                    try
                    {
                        newmd.LogicPlugin = (LogicPlugin.LogicPluginType)Enum.Parse(typeof(LogicPlugin.LogicPluginType), input.Logic.Type, false);
                    }
                    catch (Exception)
                    {
                        newmd.LogicPlugin = LogicPlugin.LogicPluginType.NULL;
                        //TODO log or something?
                    }

                    //LogicPlugin.LogicPluginData lpd = new LogicPlugin.LogicPluginData(10f, 0.001f);
                    LogicPlugin.LogicPluginData lpd = new LogicPlugin.LogicPluginData(input.Logic.Health, input.Logic.Damage, input.Logic.Position.Length,
                        input.Logic.Position, input.Logic.Orientation, input.Logic.Scale);

                    newmd.LogicPluginData = lpd;

                    //switch (input.Input.Type)
                    //{
                    //    case "PLAYER1": newmd.InputPlugin = InputPlugin.InputPluginType.PLAYER1; break;
                    //    case "PLAYER2": newmd.InputPlugin = InputPlugin.InputPluginType.PLAYER2; break;
                    //    case "AI": newmd.InputPlugin = InputPlugin.InputPluginType.AI; break;
                    //    default: newmd.InputPlugin = InputPlugin.InputPluginType.NULL; break;
                    //}

                    // way easier than switching:
                    try
                    {
                        newmd.InputPlugin = (InputPlugin.InputPluginType)Enum.Parse(typeof(InputPlugin.InputPluginType), input.Input.Type, false);
                    }
                    catch (Exception)
                    {
                        newmd.InputPlugin = InputPlugin.InputPluginType.NULL;
                        //TODO log or something? 
                    }

                    lock (md)
                    {
                        md.Add(newmd);

                        lock (mddictionary)
                        {
                            mddictionary.Add(newmd.typeName, md[md.Count - 1]);
                        }
                    }
                }
            }
        }

        public void WaitThread()
        {
            for (int i = 0; i < threadCount; i++)
            {
                th[i].Join();
            }
        }

        private static Vector3 convertV4toV3(Vector4 input)
        {
               return (new Vector3(input.X, input.Y, input.Z));
        }
    }
}
