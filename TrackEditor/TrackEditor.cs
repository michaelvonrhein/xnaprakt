#region Using Statements
using System;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Audio;
using PraktWS0708.Utils;
using System.Collections.Generic;
using PraktWS0708.Entities;
using PraktWS0708.Rendering;
using PraktWS0708.ContentPipeline;
using PraktWS0708.Settings;
using PraktWS0708.ShipUI;
using System.IO;
using System.Xml.Serialization;
#endregion

namespace PraktWS0708
{
    class TrackEditor : BackgroundScreen
    {
        private Container containerMainMenu;
        private Button button_tab_cp;
        private Button button_tab_light;
        private Button button_tab_tex;
        private Button button_tab_pickup;
        private Button button_tab_save;
        private Button button_tab_load;

        private Container containerControlPointMenu;
        private Button button_x_up;
        private Button button_x_down;
        private Button button_y_up;
        private Button button_y_down;
        private Button button_z_up;
        private Button button_z_down;
        private Button button_radius_up;
        private Button button_radius_down;

        private Button button_apply;
        private Button button_tableView;

        private Label label_x;
        private Label label_y;
        private Label label_z;
        private Label label_radius;
            
        private Vector3 currentPoint;
        private float currentRadius;

        private Button background_cp;

        private Container containerLightMenu;
        private Button button_light_x_up;
        private Button button_light_x_down;
        private Button button_light_y_up;
        private Button button_light_y_down;
        private Button button_light_z_up;
        private Button button_light_z_down;
        private Button button_lightdir_x_up;
        private Button button_lightdir_x_down;
        private Button button_lightdir_y_up;
        private Button button_lightdir_y_down;
        private Button button_lightdir_z_up;
        private Button button_lightdir_z_down;
        private Button button_light_ambient_r_up;
        private Button button_light_ambient_r_down;
        private Button button_light_ambient_g_up;
        private Button button_light_ambient_g_down;
        private Button button_light_ambient_b_up;
        private Button button_light_ambient_b_down;
        private Button button_light_specular_r_up;
        private Button button_light_specular_r_down;
        private Button button_light_specular_g_up;
        private Button button_light_specular_g_down;
        private Button button_light_specular_b_up;
        private Button button_light_specular_b_down;

        private Button button_light_apply;
        private Button button_light_list;
        private Button button_light_getpos;
        private Button button_light_getdir;

        private Label label_light_x;
        private Label label_light_y;
        private Label label_light_z;
        private Label label_lightdir_x;
        private Label label_lightdir_y;
        private Label label_lightdir_z;
        private Label label_light_ambient_r;
        private Label label_light_ambient_g;
        private Label label_light_ambient_b;
        private Label label_light_specular_r;
        private Label label_light_specular_g;
        private Label label_light_specular_b;

        /*private Vector3 light;
        private Vector3 lightdir;
        private Vector3 lightcolor_ambient;
        private Vector3 lightcolor_specular;*/

        private Button background_light;

        private Container containerPickupPointMenu;
        private Button button_pickup_x_up;
        private Button button_pickup_x_down;
        private Button button_pickup_y_up;
        private Button button_pickup_y_down;
        private Button button_pickup_z_up;
        private Button button_pickup_z_down;
        private Button button_pickup_id_up;
        private Button button_pickup_id_down;

        private Button button_pickup_apply;
        private Button button_pickup_tableView;
        private Button button_pickup_getpos;

        private Label label_pickup_x;
        private Label label_pickup_y;
        private Label label_pickup_z;
        private Label label_pickup_id;

        private Button pic_bomb;
        private Button pic_turbo;
        private Button pic_banana;
        private Button pic_shield;
        private Button pic_health;
        private Button pic_dice;
        private Button pic_wall;
        private Button pic_rocket;

        private Vector3 currentPickupPoint;
        private float currentPickupID;
        private int maxPickUpCount = 8;

        private Button background_pickup;

        private Container containerSaveMenu;
        private Button button_save_right;
        private Button button_save_left;
        private Button button_save_apply;

        private Label label_save_count;
        //private Label label_save_info;

        private int saveSlot = 1;

        private Button background_save;

        private Container containerLoadMenu;
        private Button button_load_right;
        private Button button_load_left;
        private Button button_load_apply;

        private Label label_load_count;
        //private Label label_load_info;

        private int loadSlot = 1;

        private Button background_load;

        private Container containerTextureMenu;
        private Button button_texture_right;
        private Button button_texture_left;
        private Button button_texture_apply;

        private Label label_texture_name;

        private Button background_texture;

        private int currentLayout;
        private TrackLayouts trackLayouts;

        private SpriteFont font;

        private List<Vector4> controlpoints;
        private List<Vector4> pickUpPoints;
        private List<tableview.lightList> lightList;

        private tableview.lightList currentLight;

        string name;

        private PraktWS0708.Settings.TrackDescription.TrackListEntry trackEntry;

        private tableview tableView;
        private tableview pickUpView;
        private tableview lightView;

        private float camRadius;
        private double alpha = 0.0f;
        private float camHeight = 5.0f;

        public override void LoadGraphicsContent(bool loadAllContent)
        {
            RenderManager.Instance.GraphicsDevice = ScreenManager.GraphicsDevice;
            RenderManager.Instance.SpriteBatch = ScreenManager.SpriteBatch;

            controlpoints = new List<Vector4>();
           
            controlpoints.Add(new Vector4(-10.0f, 10.0f, 0.0f, 2.0f));
            controlpoints.Add(new Vector4(10.0f, 10.0f, 0.0f, 2.0f));
            controlpoints.Add(new Vector4(10.0f, -10.0f, 0.0f, 2.0f));
            controlpoints.Add(new Vector4(-10.0f, -10.0f, 0.0f, 2.0f));
            controlpoints.Add(new Vector4(-10.0f, 10.0f, 0.0f, 2.0f));

            pickUpPoints = new List<Vector4>();

            pickUpPoints.Add(new Vector4(-10.0f, 10.0f, 0.0f, 5.0f));

            lightList = new List<tableview.lightList>();
            tableview.lightList tmp = new tableview.lightList();
            tmp.position = new Vector3(10.0f, 10.0f, 0.0f);
            tmp.direction = new Vector3(10.0f, 10.0f, 0.0f);
            tmp.diffuseColor = new Vector3(1.0f, 1.0f, 0.0f);
            tmp.specularColor = new Vector3(1.0f, 1.0f, 0.0f);
            lightList.Add(tmp);
            lightList.Add(tmp);
            lightList.Add(tmp);
            lightList.Add(tmp);
           
#if DEBUG
            Physics.Debug.Initialize(ScreenManager.GraphicsDevice);
#endif
            camRadius = 55.0f;
            if (loadAllContent)
            {
                if (content == null) content = new ContentManager(ScreenManager.Game.Services);
                backgroundTexture = content.Load<Texture2D>("Content/Textures/Black");
                if (!Settings.Configuration.ModelDescriptions.isLoaded)
                {
                    Settings.Configuration.ModelDescriptions.LoadModels("Settings\\Storage\\ModelList.xml", World.Instance.WorldContent);
                }

                trackLayouts = TrackLayouts.Load("Settings\\Storage\\TrackLayouts.xml");

                int viewport_x = ScreenManager.GraphicsDevice.Viewport.Width;
                int viewport_y = ScreenManager.GraphicsDevice.Viewport.Height;
                
                tableView = new tableview(new Vector2(viewport_x * 0.0125f, viewport_y * 0.466f), new Vector2(100.0f, 200.0f), ScreenManager, content, " R: ");
                tableView.setControlPoints(controlpoints);

                pickUpView = new tableview(new Vector2(viewport_x * 0.0125f, viewport_y * 0.466f), new Vector2(100.0f, 200.0f), ScreenManager, content, " ID: ");
                pickUpView.setControlPoints(pickUpPoints);

                lightView = new tableview(new Vector2(viewport_x * 0.425f, viewport_y * 0.1466f), new Vector2(100.0f, 200.0f), ScreenManager, content, " Light: ");
                lightView.LightList = lightList;

                buildTrackListEntity();

                //make sure the world is empty
                World.Instance.ResetWorld();

                //create a new window to the world
                World.Instance.Camera = new Camera();
                World.Instance.Camera.Aspect = ScreenManager.GraphicsDevice.PresentationParameters.BackBufferWidth / ScreenManager.GraphicsDevice.PresentationParameters.BackBufferHeight;


                //initialize a new content manager for the world
                World.Instance.WorldContent = new ContentManager(ScreenManager.Game.Services);
                RenderManager.Instance.PersistentContent = new ContentManager(ScreenManager.Game.Services);

                World.Instance.ResetWorld();

                // build the new track

                //EntityFactory.BuildEditorTrack(trackEntry, "customTrack");
                TrackFactory.buildTrack(trackEntry);
                //World.Instance.gameEnvironment = new GameEnvironment();
                World.Instance.buildWorld();

                Splines.PositionTangentUpRadius ptu;
                
                // build ships
                World.Instance.PlayersShip = EntityFactory.BuildEntity("ghostShip");
                ptu = World.Instance.Track.TangentFrames[0];
                World.Instance.PlayersShip.Position = ptu.Position - ptu.Up * ptu.Radius * 0.6f;
                World.Instance.PlayersShip.Orientation = OurMath.MakeCoordSystem(Vector3.Cross(ptu.Tangent, ptu.Up), ptu.Up, ptu.Tangent * -1.0f);
                World.Instance.PlayersShip.PhysicsPlugin.Reset();
                World.Instance.PlayersShip.Update();
                World.Instance.AddModel(World.Instance.PlayersShip);
                
                RenderManager.Instance.Initialize();

                World.Instance.Camera.Position = new Vector3((float)Math.Cos(alpha) * camRadius, (float)Math.Sin(alpha) * camRadius, 5.0f);
                World.Instance.Camera.LookAt = new Vector3(0.0f, 0.0f, 0.0f);// +(World.Instance.PlayersShip.Orientation.Up * -0.2f);
                World.Instance.Camera.Up = new Vector3(0.0f, 0.0f, 1.0f);
                World.Instance.Camera.Update();

                font = content.Load<SpriteFont>("Content/Fonts/gamefont");

                mainMenuInit();
                controlPointScreenInit();
                lightScreenInit();
                saveScreenInit();
                loadScreenInit();
                pickupPointScreenInit();
                TextureScreenInit();

            }
        }



        /// <summary>
        /// Unload graphics content used by the game.
        /// </summary>
        public override void UnloadGraphicsContent(bool unloadAllContent)
        {
            if (unloadAllContent)
            {
                content.Unload();
            }
        }


        /// <summary>
        /// Updates the state of the game. This method checks the GameScreen.IsActive
        /// property, so the game will stop updating when the pause menu is active,
        /// or if you tab away to a different application.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
            
            //renderTrack();
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            RenderManager.Instance.DrawTrackEditor(gameTime);

            if (containerMainMenu.getCurrentPos() == 0)
            {
                controlPointScreenRender();
                containerMainMenu.unset();
                button_tab_cp.set();
                tableView.Visibility = true;
                pickUpView.Visibility = false;
                lightView.Visibility = false;
            }
            else if (containerMainMenu.getCurrentPos() == 1)
            {
                lightScreenRender();
                containerMainMenu.unset();
                button_tab_light.set();
                tableView.Visibility = false;
                pickUpView.Visibility = false;
                //lightView.Visibility = true;
            }
            else if (containerMainMenu.getCurrentPos() == 2)
            {
                TextureScreenRender();
                containerMainMenu.unset();
                button_tab_tex.set();
                tableView.Visibility = false;
                pickUpView.Visibility = false;
                lightView.Visibility = false;
            }
            else if (containerMainMenu.getCurrentPos() == 3)
            {
                pickupPointScreenRender();
                containerMainMenu.unset();
                button_tab_pickup.set();
                tableView.Visibility = false;
                pickUpView.Visibility = true;
                lightView.Visibility = false;
            }
            else if (containerMainMenu.getCurrentPos() == 4)
            {
                saveScreenRender();
                containerMainMenu.unset();
                button_tab_save.set();
                tableView.Visibility = true;
                pickUpView.Visibility = false;
                lightView.Visibility = false;
            }
            else if (containerMainMenu.getCurrentPos() == 5)
            {
                loadScreenRender();
                containerMainMenu.unset();
                button_tab_load.set();
                tableView.Visibility = false;
                pickUpView.Visibility = false;
                lightView.Visibility = false;
            }

            mainMenuRender();

            if (tableView.Visibility)
                tableView.RenderList();
            if (pickUpView.Visibility)
                pickUpView.RenderList();
            if (lightView.Visibility)
                lightView.renderLightList(); 
        }

        public void Restart()
        {
            LevelFactory.LoadLevel(Configuration.levelName, LevelFactory.LoadType.RESET);          

            // enable input
            World.Instance.PlayersShip.InputPlugin.Active = true;
        }

        public void saveTrack(string filename)
        {
            string path = "Settings/Storage/";

            buildTrackListEntity();
            Stream stream = File.Create(path + filename);

            XmlSerializer serializer = new XmlSerializer(typeof(PraktWS0708.Settings.TrackDescription.TrackListEntry));
            serializer.Serialize(stream, trackEntry);
            stream.Close();
        }

        public TrackDescription.TrackListEntry loadTrack(string filename)
        {
            
            TrackDescription.TrackListEntry entry;

            string path = "Settings\\Storage\\";

            Stream stream = File.OpenRead(path + filename);
            XmlSerializer serializer = new XmlSerializer(typeof(TrackDescription.TrackListEntry));

            entry = (TrackDescription.TrackListEntry)serializer.Deserialize(stream);

           return entry;
        }


        #region Handle Input
        void ExitMessageBoxAccepted(object sender, EventArgs e)
        {
            Sound.PlayCue(Sounds.MenuBack);
            //Sound.Stop(themeCue);
            ScreenManager.AddScreen(new BackgroundScreen());
            ScreenManager.AddScreen(new MainMenuScreen());
        }
        /// <summary>
        /// Responds to user input, changing the selected ship
        /// or cancelling the menu.
        /// </summary>
        public override void HandleInput()
        {
            if (InputState.instance.PauseGame)
            {
                const string message = "Are you sure you want to exit the program?";

                MessageBoxScreen messageBox = new MessageBoxScreen(message);

                messageBox.Accepted += ExitMessageBoxAccepted;

                ScreenManager.AddScreen(messageBox);
            }
            else if (InputState.instance.GamePadY)
            {
                if (tableView.isSelected())
                    tableView.deleteActiveItem();
                if (pickUpView.isSelected())
                    pickUpView.deleteActiveItem();
                if (lightView.isSelected())
                    lightView.deleteActiveLight();
            }
            else if (InputState.instance.GamePadX)
            {
                if (pickUpView.isSelected())
                    pickUpView.insertNewItem();
                if (tableView.isSelected())
                    tableView.insertNewItem();
                if (lightView.isSelected())
                    lightView.insertNewLight();
            }
            else if (InputState.instance.LeftShoulderPressed)
            {
                if (tableView.isSelected())
                    tableView.moveActiveItemDown();
            }
            else if (InputState.instance.RightShoulderPressed)
            {
                if (tableView.isSelected())
                    tableView.moveActtiveItemUp();
            }
            else if (InputState.instance.XAxis1 > 0.0f)
            {
                alpha -= 0.005d;
                World.Instance.Camera.Position = new Vector3((float)Math.Cos(alpha) * camRadius, (float)Math.Sin(alpha) * camRadius, camHeight);
                World.Instance.Camera.LookAt = new Vector3(0.0f, 0.0f, 0.0f);
                World.Instance.Camera.Up = new Vector3(0.0f, 0.0f, 1.0f);
                World.Instance.Camera.Update();
            }
            else if (InputState.instance.XAxis1 < 0.0f)
            {
                alpha += 0.005d;
                World.Instance.Camera.Position = new Vector3((float)Math.Cos(alpha) * camRadius, (float)Math.Sin(alpha) * camRadius, camHeight);
                World.Instance.Camera.LookAt = new Vector3(0.0f, 0.0f, 0.0f);
                World.Instance.Camera.Up = new Vector3(0.0f, 0.0f, 1.0f);
                World.Instance.Camera.Update();
            }
            else if (InputState.instance.YAxis1 > 0.0f)
            {
                camRadius -= 0.125f;
                if (camRadius <= 0)
                {
                    camRadius = (0.001f);
                }
                World.Instance.Camera.Position = new Vector3((float)Math.Cos(alpha) * camRadius, (float)Math.Sin(alpha) * camRadius, camHeight);
                World.Instance.Camera.LookAt = new Vector3(0.0f, 0.0f, 0.0f);
                World.Instance.Camera.Up = new Vector3(0.0f, 0.0f, 1.0f);
                World.Instance.Camera.Update();
            }
            else if (InputState.instance.YAxis1 < 0.0f)
            {
                camRadius += 0.125f;
                World.Instance.Camera.Position = new Vector3((float)Math.Cos(alpha) * camRadius, (float)Math.Sin(alpha) * camRadius, camHeight);
                World.Instance.Camera.LookAt = new Vector3(0.0f, 0.0f, 0.0f);
                World.Instance.Camera.Up = new Vector3(0.0f, 0.0f, 1.0f);
                World.Instance.Camera.Update();
            }
            else if (InputState.instance.LeftTriggerPressed > 0.0f)
            {
                camHeight -= 0.125f;
                World.Instance.Camera.Position = new Vector3((float)Math.Cos(alpha) * camRadius, (float)Math.Sin(alpha) * camRadius, camHeight);
                World.Instance.Camera.LookAt = new Vector3(0.0f, 0.0f, 0.0f);
                World.Instance.Camera.Up = new Vector3(0.0f, 0.0f, 1.0f);
                World.Instance.Camera.Update();
            }
            else if (InputState.instance.RightTriggerPressed > 0.0f)
            {
                camHeight += 0.125f;
                World.Instance.Camera.Position = new Vector3((float)Math.Cos(alpha) * camRadius, (float)Math.Sin(alpha) * camRadius, camHeight);
                World.Instance.Camera.LookAt = new Vector3(0.0f, 0.0f, 0.0f);
                World.Instance.Camera.Up = new Vector3(0.0f, 0.0f, 1.0f);
                World.Instance.Camera.Update();
            }
            else if (InputState.instance.MenuCancel)
            {
                Sound.PlayCue(Sounds.MenuBack);
                //Sound.Stop(themeCue);
                ScreenManager.AddScreen(new BackgroundScreen());
                ScreenManager.AddScreen(new MainMenuScreen());
            }
            else if (InputState.instance.MenuRight)
            {
                containerMainMenu.CurrentPosUp();
                if (containerMainMenu.getCurrentPos() == 1)
                {
                    if (lightView.isSelected())
                    {
                        lightView.Visibility = true;
                    }
                }
            }
            else if (InputState.instance.MenuLeft)
            {
                containerMainMenu.CurrentPosDown();
                if (containerMainMenu.getCurrentPos() == 1)
                {
                    if (lightView.isSelected())
                    {
                        lightView.Visibility = true;
                    }
                }
            }
            else if (InputState.instance.MenuUp)
            {
                if (containerMainMenu.getCurrentPos() == 0)
                {
                    if (tableView.isSelected())
                        tableView.setActiveItem(tableView.getActiveItemNumber() - 1);
                    if(containerControlPointMenu.isSelected())
                        containerControlPointMenu.CurrentPosDown();
                }
                else if (containerMainMenu.getCurrentPos() == 1)
                {
                    if (lightView.isSelected())
                        lightView.setActiveLight(lightView.getActiveItemNumber() - 1);
                    if (containerLightMenu.isSelected())
                        containerLightMenu.CurrentPosDown();
                }
                else if (containerMainMenu.getCurrentPos() == 2)
                {
                    if (containerTextureMenu.isSelected())
                        containerTextureMenu.CurrentPosDown();
                }
                else if (containerMainMenu.getCurrentPos() == 3)
                {
                    if (pickUpView.isSelected())
                        pickUpView.setActiveItem(pickUpView.getActiveItemNumber() - 1);
                    if (containerPickupPointMenu.isSelected())
                        containerPickupPointMenu.CurrentPosDown();
                }
                else if (containerMainMenu.getCurrentPos() == 4)
                {
                    containerSaveMenu.CurrentPosDown();
                }
                else if (containerMainMenu.getCurrentPos() == 5)
                {
                    containerLoadMenu.CurrentPosDown();
                }
            }
            else if (InputState.instance.MenuDown)
            {
                if (containerMainMenu.getCurrentPos() == 0)
                {
                    if (tableView.isSelected())
                        tableView.setActiveItem(tableView.getActiveItemNumber() + 1);
                    if (containerControlPointMenu.isSelected())
                        containerControlPointMenu.CurrentPosUp();
                }
                else if (containerMainMenu.getCurrentPos() == 1)
                {
                    if (lightView.isSelected())
                        lightView.setActiveLight(lightView.getActiveItemNumber() + 1);
                    if (containerLightMenu.isSelected())
                        containerLightMenu.CurrentPosUp();
                }
                else if (containerMainMenu.getCurrentPos() == 2)
                {  
                    if (containerTextureMenu.isSelected())
                        containerTextureMenu.CurrentPosUp();
                }
                else if (containerMainMenu.getCurrentPos() == 3)
                {
                    if (pickUpView.isSelected())
                        pickUpView.setActiveItem(pickUpView.getActiveItemNumber() + 1);
                    if (containerPickupPointMenu.isSelected())
                        containerPickupPointMenu.CurrentPosUp();
                }
                else if (containerMainMenu.getCurrentPos() == 4)
                {
                    containerSaveMenu.CurrentPosUp();
                }
                else if (containerMainMenu.getCurrentPos() == 5)
                {
                    containerLoadMenu.CurrentPosUp();
                }
            }
            else if (InputState.instance.MenuSelect)
            {
                if (containerMainMenu.getCurrentPos() == 0)
                {
                    if (containerControlPointMenu.isSelected())
                    {
                        switch (containerControlPointMenu.getCurrentPos())
                        {
                            case 0: currentPoint.X += 0.1f; label_x.SetText(currentPoint.X.ToString("N1")); break;
                            case 1: currentPoint.X -= 0.1f; label_x.SetText(currentPoint.X.ToString("N1")); break;
                            case 2: currentPoint.Y += 0.1f; label_y.SetText(currentPoint.Y.ToString("N1")); break;
                            case 3: currentPoint.Y -= 0.1f; label_y.SetText(currentPoint.Y.ToString("N1")); break;
                            case 4: currentPoint.Z += 0.1f; label_z.SetText(currentPoint.Z.ToString("N1")); break;
                            case 5: currentPoint.Z -= 0.1f; label_z.SetText(currentPoint.Z.ToString("N1")); break;
                            case 6: currentRadius += 0.1f; label_radius.SetText(currentRadius.ToString("N1")); break;
                            case 7: currentRadius -= 0.1f; label_radius.SetText(currentRadius.ToString("N1")); break;
                            case 8: tableView.updateActiveItem(new Vector4(currentPoint.X, currentPoint.Y, currentPoint.Z, currentRadius));
                                renderTrack(); break;
                            case 9: tableView.setState(true); containerControlPointMenu.setState(false); break;
                            default: break;
                        }
                    }
                    else if (tableView.isSelected())
                    {
                        currentPoint.X = tableView.getActiveItem().X;
                        currentPoint.Y = tableView.getActiveItem().Y;
                        currentPoint.Z = tableView.getActiveItem().Z;
                        currentRadius = tableView.getActiveItem().W;
                        tableView.setState(false);
                        containerControlPointMenu.setState(true);
                        updateText();
                    }
                }
                else if (containerMainMenu.getCurrentPos() == 1)
                {
                    if (containerLightMenu.isSelected())
                    {
                        switch (containerLightMenu.getCurrentPos())
                        {
                            case 0: currentLight.position.X += 0.1f; label_light_x.SetText(currentLight.position.X.ToString("N1")); break;
                            case 1: currentLight.position.X -= 0.1f; label_light_x.SetText(currentLight.position.X.ToString("N1")); break;
                            case 2: currentLight.position.Y += 0.1f; label_light_y.SetText(currentLight.position.Y.ToString("N1")); break;
                            case 3: currentLight.position.Y -= 0.1f; label_light_y.SetText(currentLight.position.Y.ToString("N1")); break;
                            case 4: currentLight.position.Z += 0.1f; label_light_z.SetText(currentLight.position.Z.ToString("N1")); break;
                            case 5: currentLight.position.Z -= 0.1f; label_light_z.SetText(currentLight.position.Z.ToString("N1")); break;
                            case 6: currentLight.direction.X += 0.1f; label_lightdir_x.SetText(currentLight.direction.X.ToString("N1")); break;
                            case 7: currentLight.direction.X -= 0.1f; label_lightdir_x.SetText(currentLight.direction.X.ToString("N1")); break;
                            case 8: currentLight.direction.Y += 0.1f; label_lightdir_y.SetText(currentLight.direction.Y.ToString("N1")); break;
                            case 9: currentLight.direction.Y -= 0.1f; label_lightdir_y.SetText(currentLight.direction.Y.ToString("N1")); break;
                            case 10: currentLight.direction.Z += 0.1f; label_lightdir_z.SetText(currentLight.direction.Z.ToString("N1")); break;
                            case 11: currentLight.direction.Z -= 0.1f; label_lightdir_z.SetText(currentLight.direction.Z.ToString("N1")); break;
                            case 12: currentLight.diffuseColor.X += 0.01f; if (currentLight.diffuseColor.X > 1f) currentLight.diffuseColor.X = 1f; label_light_ambient_r.SetText(currentLight.diffuseColor.X.ToString("N2")); break;
                            case 13: currentLight.diffuseColor.X -= 0.01f; if (currentLight.diffuseColor.X < 0f) currentLight.diffuseColor.X = 0f; label_light_ambient_r.SetText(currentLight.diffuseColor.X.ToString("N2")); break;
                            case 14: currentLight.diffuseColor.Y += 0.01f; if (currentLight.diffuseColor.Y > 1f) currentLight.diffuseColor.Y = 1f; label_light_ambient_g.SetText(currentLight.diffuseColor.Y.ToString("N2")); break;
                            case 15: currentLight.diffuseColor.Y -= 0.01f; if (currentLight.diffuseColor.Y < 0f) currentLight.diffuseColor.Y = 0f; label_light_ambient_g.SetText(currentLight.diffuseColor.Y.ToString("N2")); break;
                            case 16: currentLight.diffuseColor.Z += 0.01f; if (currentLight.diffuseColor.Z > 1f) currentLight.diffuseColor.Z = 1f; label_light_ambient_b.SetText(currentLight.diffuseColor.Z.ToString("N2")); break;
                            case 17: currentLight.diffuseColor.Z -= 0.01f; if (currentLight.diffuseColor.Z < 0f) currentLight.diffuseColor.Z = 0f; label_light_ambient_b.SetText(currentLight.diffuseColor.Z.ToString("N2")); break;
                            case 18: currentLight.specularColor.X += 0.01f; if (currentLight.specularColor.X > 1f) currentLight.specularColor.X = 1f; label_light_specular_r.SetText(currentLight.specularColor.X.ToString("N2")); break;
                            case 19: currentLight.specularColor.X -= 0.01f; if (currentLight.specularColor.X < 0f) currentLight.specularColor.X = 0f; label_light_specular_r.SetText(currentLight.specularColor.X.ToString("N2")); break;
                            case 20: currentLight.specularColor.Y += 0.01f; if (currentLight.specularColor.Y > 1f) currentLight.specularColor.Y = 1f; label_light_specular_g.SetText(currentLight.specularColor.Y.ToString("N2")); break;
                            case 21: currentLight.specularColor.Y -= 0.01f; if (currentLight.specularColor.Y < 0f) currentLight.specularColor.Y = 0f; label_light_specular_g.SetText(currentLight.specularColor.Y.ToString("N2")); break;
                            case 22: currentLight.specularColor.Z += 0.01f; if (currentLight.specularColor.Z > 1f) currentLight.specularColor.Z = 1f; label_light_specular_b.SetText(currentLight.specularColor.Z.ToString("N2")); break;
                            case 23: currentLight.specularColor.Z -= 0.01f; if (currentLight.specularColor.Z < 0f) currentLight.specularColor.Z = 0f; label_light_specular_b.SetText(currentLight.specularColor.Z.ToString("N2")); break;
                            case 24:
                                tableview.lightList tmplist = new tableview.lightList();
                                tmplist.position = currentLight.position;
                                tmplist.direction = currentLight.direction;
                                tmplist.diffuseColor = currentLight.diffuseColor;
                                tmplist.specularColor = currentLight.specularColor;

                                lightView.updateActiveLight(tmplist);
                                renderTrack(); break;
                            case 25: lightView.setState(true); lightView.Visibility = true; containerLightMenu.setState(false); break;
                            case 26: currentLight.position = World.Instance.Camera.Position;
                                label_light_x.SetText(currentLight.position.X.ToString("N1"));
                                label_light_y.SetText(currentLight.position.Y.ToString("N1"));
                                label_light_z.SetText(currentLight.position.Z.ToString("N1"));
                                break;
                            case 27: currentLight.direction = World.Instance.Camera.Position;
                                label_lightdir_x.SetText(currentLight.direction.X.ToString("N1"));
                                label_lightdir_y.SetText(currentLight.direction.Y.ToString("N1"));
                                label_lightdir_z.SetText(currentLight.direction.Z.ToString("N1")); 
                                break;
                            default: break;
                        }
                    }
                    else if (lightView.isSelected())
                    {
                        currentLight.position = lightView.getActiveLight().position;
                        currentLight.direction = lightView.getActiveLight().direction;
                        currentLight.diffuseColor = lightView.getActiveLight().diffuseColor;
                        currentLight.specularColor = lightView.getActiveLight().specularColor;
                        lightView.setState(false);
                        lightView.Visibility = false;
                        containerLightMenu.setState(true);
                        updateText();
                    }
                }
                else if (containerMainMenu.getCurrentPos() == 2)
                {
                    switch(containerTextureMenu.getCurrentPos())
                    {
                        case 0: currentLayout--; if (currentLayout < 0) currentLayout = 0; 
                            label_texture_name.SetText(trackLayouts.LayoutArray[currentLayout].name); break;
                        case 1: currentLayout++; if (currentLayout >= trackLayouts.LayoutArray.Length - 1) currentLayout = trackLayouts.LayoutArray.Length - 2; 
                            label_texture_name.SetText(trackLayouts.LayoutArray[currentLayout].name); break;
                        case 2: renderTrack(); break;
                        default: break;
                    }
                }
                else if (containerMainMenu.getCurrentPos() == 3)
                {
                    if (containerPickupPointMenu.isSelected())
                    {
                        switch (containerPickupPointMenu.getCurrentPos())
                        {
                            case 0: currentPickupPoint.X += 0.1f; label_pickup_x.SetText(currentPickupPoint.X.ToString("N1")); break;
                            case 1: currentPickupPoint.X -= 0.1f; label_pickup_x.SetText(currentPickupPoint.X.ToString("N1")); break;
                            case 2: currentPickupPoint.Y += 0.1f; label_pickup_y.SetText(currentPickupPoint.Y.ToString("N1")); break;
                            case 3: currentPickupPoint.Y -= 0.1f; label_pickup_y.SetText(currentPickupPoint.Y.ToString("N1")); break;
                            case 4: currentPickupPoint.Z += 0.1f; label_pickup_z.SetText(currentPickupPoint.Z.ToString("N1")); break;
                            case 5: currentPickupPoint.Z -= 0.1f; label_pickup_z.SetText(currentPickupPoint.Z.ToString("N1")); break;
                            case 6: currentPickupID++; if (currentPickupID > maxPickUpCount) currentPickupID = maxPickUpCount; label_pickup_id.SetText(currentPickupID.ToString()); break;
                            case 7: currentPickupID--; if (currentPickupID < 1) currentPickupID = 1; label_pickup_id.SetText(currentPickupID.ToString()); break;
                            case 8: pickUpView.updateActiveItem(new Vector4(currentPickupPoint.X, currentPickupPoint.Y, currentPickupPoint.Z, currentPickupID));
                                renderTrack(); break;
                            case 9: pickUpView.setState(true); containerPickupPointMenu.setState(false); break;
                            case 10: currentPickupPoint = World.Instance.Camera.Position;
                                label_pickup_x.SetText(currentPickupPoint.X.ToString("N1"));
                                label_pickup_y.SetText(currentPickupPoint.Y.ToString("N1"));
                                label_pickup_z.SetText(currentPickupPoint.Z.ToString("N1")); break;
                            default: break;
                        }
                    }
                    else if (pickUpView.isSelected())
                    {
                        currentPickupPoint.X = pickUpView.getActiveItem().X;
                        currentPickupPoint.Y = pickUpView.getActiveItem().Y;
                        currentPickupPoint.Z = pickUpView.getActiveItem().Z;
                        currentPickupID = pickUpView.getActiveItem().W;
                        pickUpView.setState(false);
                        containerPickupPointMenu.setState(true);
                        updateText();
                    }
                }
                else if (containerMainMenu.getCurrentPos() == 4)
                {
                    switch (containerSaveMenu.getCurrentPos())
                    {
                        case 0:
                            saveSlot--;
                            if (saveSlot < 1) saveSlot = 1;
                            label_save_count.SetText(saveSlot.ToString()); break;
                        case 1:
                            saveSlot++;
                            if (saveSlot > 10) saveSlot = 10;
                            label_save_count.SetText(saveSlot.ToString()); break;
                        case 2: saveTrack("customtrack" + saveSlot.ToString() + ".xml"); break;
                        default: break;
                    }


                }
                else if (containerMainMenu.getCurrentPos() == 5)
                {
                    switch (containerLoadMenu.getCurrentPos())
                    {
                        case 0:
                            loadSlot--;
                            if (loadSlot < 1) loadSlot = 1;
                            label_load_count.SetText(loadSlot.ToString()); break;
                        case 1:
                            loadSlot++;
                            if (loadSlot > 10) loadSlot = 10;
                            label_load_count.SetText(loadSlot.ToString()); break;
                        case 2:
                            trackEntry = loadTrack("customtrack" + loadSlot.ToString() + ".xml");

                            controlpoints.Clear();
                            pickUpPoints.Clear();
                            lightList.Clear();

                            for (int i = 0; i < trackEntry.controlpoints.Length; i++)
                            {

                                controlpoints.Add(new Vector4(trackEntry.controlpoints[i].position.X,
                                trackEntry.controlpoints[i].position.Y,
                                trackEntry.controlpoints[i].position.Z,
                                trackEntry.controlpoints[i].radius));
                            }

                            for (int i = 0; i < trackEntry.pickUpPoints.Length; i++)
                            {
                                pickUpPoints.Add(new Vector4(trackEntry.pickUpPoints[i].X,
                                trackEntry.pickUpPoints[i].Y,
                                trackEntry.pickUpPoints[i].Z,
                                trackEntry.pickUpPoints[i].W));
                            }

                            for (int i = 0; i < trackEntry.lights.Length; i++)
                            {
                                tableview.lightList tmp = new tableview.lightList();
                                tmp.position = trackEntry.lights[i].Position;
                                tmp.direction = trackEntry.lights[i].Direction;
                                tmp.diffuseColor = trackEntry.lights[i].Diffuse;
                                tmp.specularColor = trackEntry.lights[i].Specular;
                                lightList.Add(tmp);
                            }

                            name = trackEntry.trackName;

                            for (int i = 0; i < trackLayouts.LayoutArray.Length; i++)
                            {
                                if (trackEntry.layout == trackLayouts.LayoutArray[i].name)
                                {
                                    currentLayout = i;
                                    label_texture_name.SetText(trackLayouts.LayoutArray[currentLayout].name);
                                }
                            }
                            //diffuse_texture = trackEntry.diffuseTexture;
                            //normal_map = trackEntry.normalMap;


                            renderTrack(); break;
                        default: break;
                    }


                }
            }
        }

        public void drawTrack()
        {
            buildTrackListEntity();
            TrackFactory.buildTrack(trackEntry.trackName);
            //World.Instance.Track = new Track(trackEntry.controlpoints, 0.01f, 32, true, trackEntry.diffuseTexture, trackEntry.normalMap);
        }

        public void renderTrack()
        {
            World.Instance.ResetWorld();

            // build the new track
            buildTrackListEntity();

            //EntityFactory.BuildEditorTrack(trackEntry, "customTrack");
            TrackFactory.buildTrack(trackEntry);
            //World.Instance.gameEnvironment = new GameEnvironment();

            Splines.PositionTangentUpRadius ptu;

            // build ships
            World.Instance.PlayersShip = EntityFactory.BuildEntity("ghostShip");
            ptu = World.Instance.Track.TangentFrames[0];
            World.Instance.PlayersShip.Position = ptu.Position - ptu.Up * ptu.Radius * 0.6f;
            World.Instance.PlayersShip.Orientation = OurMath.MakeCoordSystem(Vector3.Cross(ptu.Tangent, ptu.Up), ptu.Up, ptu.Tangent * -1.0f);
            World.Instance.PlayersShip.PhysicsPlugin.Reset();
            World.Instance.PlayersShip.Update();
            World.Instance.AddModel(World.Instance.PlayersShip);

            Settings.TrackDescription.TrackListEntry tmp = trackEntry;
            

            BaseEntity[] pickup = new BaseEntity[tmp.pickUpPoints.Length];
            for (int i = 0; i < tmp.pickUpPoints.Length; i++)
            {
                switch ((int)tmp.pickUpPoints[i].W)
                {
                    case 1:
                        pickup[i] = EntityFactory.BuildEntity("banane1");
                        break;
                    case 2:
                        pickup[i] = EntityFactory.BuildEntity("bomb1");
                        break;
                    case 3:
                        pickup[i] = EntityFactory.BuildEntity("shield1");
                        break;
                    case 4:
                        pickup[i] = EntityFactory.BuildEntity("turbo1");
                        break;
                    case 5:
                        pickup[i] = EntityFactory.BuildEntity("healthpack1");
                        break;
                    case 6:
                        pickup[i] = EntityFactory.BuildEntity("dicered");
                        break;
                    case 7:
                        pickup[i] = EntityFactory.BuildEntity("rocket2");
                        break;
                    case 8:
                        pickup[i] = EntityFactory.BuildEntity("box1");
                        break;
                    default:
                        break;
                }
                pickup[i].Position = new Vector3(tmp.pickUpPoints[i].X, tmp.pickUpPoints[i].Y, tmp.pickUpPoints[i].Z);
                /*pickup[i].Position.X = tmp.pickUpPoints[i].X;
                pickup[i].Position.Y = tmp.pickUpPoints[i].Y;
                pickup[i].Position.Z = tmp.pickUpPoints[i].Z;*/

                World.Instance.AddModel(pickup[i]);
            }

            //RenderManager.Instance.Initialize();            
        }

        private void buildTrackListEntity()
        {
            string name = "customtrack" + saveSlot.ToString();
            //string diffuse_texture = "track_diffuse";
            //string normal_map = "tracknormal";

            Utils.Splines.ControlPoint[] points = new Splines.ControlPoint[tableView.Count];
            for (int i = 0; i < tableView.Count; i++)
            {
                points[i].position.X = tableView.getItemByNumber(i).X;
                points[i].position.Y = tableView.getItemByNumber(i).Y;
                points[i].position.Z = tableView.getItemByNumber(i).Z;
                points[i].radius = (int)tableView.getItemByNumber(i).W;
            }

            Vector4[] pickUpPoints = new Vector4[pickUpView.Count];

            for (int i = 0; i < pickUpView.Count; i++)
            {
                pickUpPoints[i].X = pickUpView.getItemByNumber(i).X;
                pickUpPoints[i].Y = pickUpView.getItemByNumber(i).Y;
                pickUpPoints[i].Z = pickUpView.getItemByNumber(i).Z;
                pickUpPoints[i].W = pickUpView.getItemByNumber(i).W;
            }

            //Feature Temporarily unavailable -the rendering
            TrackLight[] spotlights = new TrackLight[lightView.LightCount];
            for (int i = 0; i < lightView.LightCount; i++)
            {
                TrackLight tmp = new TrackLight();
                tmp.Position = lightList[i].position;
                tmp.Direction = lightList[i].direction;
                tmp.Specular = lightList[i].specularColor;
                tmp.Diffuse = lightList[i].diffuseColor;
                spotlights[i] = tmp;
            }

            TrackDescription.TrackInfo ti;
            ti.realName = "Custom Track " + saveSlot.ToString();
            ti.editorTime = "";
            ti.dannerComment = "Das ist eine schoene Strecke.";
            ti.difficulty = "";

            trackEntry = new TrackDescription.TrackListEntry(name, points, pickUpPoints, trackLayouts.LayoutArray[currentLayout].name, spotlights, ti);
        }

        public void mainMenuInit()
        {

            int viewport_x = ScreenManager.GraphicsDevice.Viewport.Width;
            int viewport_y = ScreenManager.GraphicsDevice.Viewport.Height;

            containerMainMenu = new Container();

            Texture2D[] button_tex_cp = new Texture2D[2];
            Texture2D[] button_tex_light = new Texture2D[2];
            Texture2D[] button_tex_tex = new Texture2D[2];
            Texture2D[] button_tex_save = new Texture2D[2];
            Texture2D[] button_tex_load = new Texture2D[2];
            Texture2D[] button_tex_pickup = new Texture2D[2];

            button_tex_cp[0] = content.Load<Texture2D>("Content/Textures/TrackEditor/button_cp");
            button_tex_cp[1] = content.Load<Texture2D>("Content/Textures/TrackEditor/button_cp_active");
            button_tex_light[0] = content.Load<Texture2D>("Content/Textures/TrackEditor/button_lights");
            button_tex_light[1] = content.Load<Texture2D>("Content/Textures/TrackEditor/button_lights_active");
            button_tex_tex[0] = content.Load<Texture2D>("Content/Textures/TrackEditor/button_tex");
            button_tex_tex[1] = content.Load<Texture2D>("Content/Textures/TrackEditor/button_tex_active");
            button_tex_save[0] = content.Load<Texture2D>("Content/Textures/TrackEditor/button_save");
            button_tex_save[1] = content.Load<Texture2D>("Content/Textures/TrackEditor/button_save_active");
            button_tex_load[0] = content.Load<Texture2D>("Content/Textures/TrackEditor/button_load");
            button_tex_load[1] = content.Load<Texture2D>("Content/Textures/TrackEditor/button_load_active");
            button_tex_pickup[0] = content.Load<Texture2D>("Content/Textures/TrackEditor/button_pickup");
            button_tex_pickup[1] = content.Load<Texture2D>("Content/Textures/TrackEditor/button_pickup_active"); 

            button_tab_cp = new Button(new Vector2(viewport_x * 0.0125f, viewport_y * 0.0166f), new Vector2(viewport_x * 0.075f, viewport_y * 0.033f), button_tex_cp, ScreenManager);
            button_tab_light = new Button(new Vector2(viewport_x * 0.133f, viewport_y * 0.0166f), new Vector2(viewport_x * 0.075f, viewport_y * 0.033f), button_tex_light, ScreenManager);
            button_tab_tex = new Button(new Vector2(viewport_x * 0.25f, viewport_y * 0.0166f), new Vector2(viewport_x * 0.075f, viewport_y * 0.033f), button_tex_tex, ScreenManager);
            button_tab_pickup = new Button(new Vector2(viewport_x * 0.36875f, viewport_y * 0.0166f), new Vector2(viewport_x * 0.075f, viewport_y * 0.033f), button_tex_pickup, ScreenManager);
            button_tab_save = new Button(new Vector2(viewport_x * 0.4875f, viewport_y * 0.0166f), new Vector2(viewport_x * 0.075f, viewport_y * 0.033f), button_tex_save, ScreenManager);
            button_tab_load = new Button(new Vector2(viewport_x * 0.6125f, viewport_y * 0.0166f), new Vector2(viewport_x * 0.075f, viewport_y * 0.033f), button_tex_load, ScreenManager);

            containerMainMenu.Add(button_tab_cp);
            containerMainMenu.Add(button_tab_light);
            containerMainMenu.Add(button_tab_tex);
            containerMainMenu.Add(button_tab_pickup);
            containerMainMenu.Add(button_tab_save);
            containerMainMenu.Add(button_tab_load);
        }

        public void mainMenuRender()
        {
            containerMainMenu.Render();
        }

        public void controlPointScreenInit()
        {

            int viewport_x = ScreenManager.GraphicsDevice.Viewport.Width;
            int viewport_y = ScreenManager.GraphicsDevice.Viewport.Height;

            containerControlPointMenu = new Container();

            currentPoint = new Vector3(0f, 0f, 0f);
            currentRadius = 1f;

            Texture2D[] button_tex_down = new Texture2D[2];
            Texture2D[] button_tex_up = new Texture2D[2];
            Texture2D[] button_tex_apply = new Texture2D[2];
            Texture2D[] button_tex_list = new Texture2D[2];
            Texture2D[] background_tex_cp = new Texture2D[2];

            button_tex_up[0] = content.Load<Texture2D>("Content/Textures/TrackEditor/button_up");
            button_tex_up[1] = content.Load<Texture2D>("Content/Textures/TrackEditor/button_up_active");
            button_tex_down[0] = content.Load<Texture2D>("Content/Textures/TrackEditor/button_down");
            button_tex_down[1] = content.Load<Texture2D>("Content/Textures/TrackEditor/button_down_active");

            button_tex_apply[0] = content.Load<Texture2D>("Content/Textures/TrackEditor/button_apply");
            button_tex_apply[1] = content.Load<Texture2D>("Content/Textures/TrackEditor/button_apply_active");
            button_tex_list[0] = content.Load<Texture2D>("Content/Textures/TrackEditor/button_list");
            button_tex_list[1] = content.Load<Texture2D>("Content/Textures/TrackEditor/button_list_active");

            background_tex_cp[0] = content.Load<Texture2D>("Content/Textures/TrackEditor/screen_cp");
            background_tex_cp[1] = content.Load<Texture2D>("Content/Textures/TrackEditor/screen_cp");

            button_x_up = new Button(new Vector2(viewport_x * 0.125f, viewport_y * 0.083f), new Vector2(viewport_x * 0.025f, viewport_y * 0.033f), button_tex_up, ScreenManager);
            button_x_down = new Button(new Vector2(viewport_x * 0.125f, viewport_y * 0.116f), new Vector2(viewport_x * 0.025f, viewport_y * 0.033f), button_tex_down, ScreenManager);
            button_y_up = new Button(new Vector2(viewport_x * 0.125f, viewport_y * 0.233f), new Vector2(viewport_x * 0.025f, viewport_y * 0.033f), button_tex_up, ScreenManager);
            button_y_down = new Button(new Vector2(viewport_x *0.125f, viewport_y * 0.266f), new Vector2(viewport_x * 0.025f, viewport_y * 0.033f), button_tex_down, ScreenManager);
            button_z_up = new Button(new Vector2(viewport_x * 0.125f, viewport_y * 0.383f), new Vector2(viewport_x * 0.025f, viewport_y * 0.033f), button_tex_up, ScreenManager);
            button_z_down = new Button(new Vector2(viewport_x *0.125f, viewport_y * 0.416f), new Vector2(viewport_x * 0.025f, viewport_y * 0.033f), button_tex_down, ScreenManager);
            button_radius_up = new Button(new Vector2(viewport_x * 0.25f, viewport_y * 0.083f), new Vector2(viewport_x * 0.025f, viewport_y * 0.033f), button_tex_up, ScreenManager);
            button_radius_down = new Button(new Vector2(viewport_x * 0.25f, viewport_y * 0.116f), new Vector2(viewport_x * 0.025f, viewport_y * 0.033f), button_tex_down, ScreenManager);

            button_apply = new Button(new Vector2(viewport_x * 0.36875f, viewport_y * 0.95f), new Vector2(viewport_x * 0.075f, viewport_y * 0.033f), button_tex_apply, ScreenManager);
            button_tableView = new Button(new Vector2(viewport_x * 0.4875f, viewport_y * 0.95f), new Vector2(viewport_x * 0.075f, viewport_y * 0.033f), button_tex_list, ScreenManager);

            label_x = new Label(currentPoint.X.ToString("N1"), new Vector2(viewport_x * 0.0166f, viewport_y * 0.083f), font, Color.Red, ScreenManager);
            label_y = new Label(currentPoint.Y.ToString("N1"), new Vector2(viewport_x * 0.0166f, viewport_y * 0.233f), font, Color.Red, ScreenManager);
            label_z = new Label(currentPoint.Z.ToString("N1"), new Vector2(viewport_x * 0.0166f, viewport_y * 0.383f), font, Color.Red, ScreenManager);
            label_radius = new Label(currentRadius.ToString("N1"), new Vector2(viewport_x * 0.166f, viewport_y * 0.083f), font, Color.Red, ScreenManager);

            background_cp = new Button(new Vector2(0, 0), new Vector2(viewport_x, viewport_y), background_tex_cp, ScreenManager);

            containerControlPointMenu.Add(button_x_up);
            containerControlPointMenu.Add(button_x_down);
            containerControlPointMenu.Add(button_y_up);
            containerControlPointMenu.Add(button_y_down);
            containerControlPointMenu.Add(button_z_up);
            containerControlPointMenu.Add(button_z_down);
            containerControlPointMenu.Add(button_radius_up);
            containerControlPointMenu.Add(button_radius_down);
            containerControlPointMenu.Add(button_apply);
            containerControlPointMenu.Add(button_tableView);
        }

        public void updateText()
        {
            label_radius.SetText(currentRadius.ToString("N1"));
            label_x.SetText(currentPoint.X.ToString("N1"));
            label_y.SetText(currentPoint.Y.ToString("N1"));
            label_z.SetText(currentPoint.Z.ToString("N1"));

            label_pickup_id.SetText(currentPickupID.ToString());
            label_pickup_x.SetText(currentPickupPoint.X.ToString("N1"));
            label_pickup_y.SetText(currentPickupPoint.Y.ToString("N1"));
            label_pickup_z.SetText(currentPickupPoint.Z.ToString("N1"));

            label_light_x.SetText(currentLight.position.X.ToString("N1"));
            label_light_y.SetText(currentLight.position.Y.ToString("N1"));
            label_light_z.SetText(currentLight.position.Z.ToString("N1"));

            label_lightdir_x.SetText(currentLight.direction.X.ToString("N1"));
            label_lightdir_y.SetText(currentLight.direction.Y.ToString("N1"));
            label_lightdir_z.SetText(currentLight.direction.Z.ToString("N1"));

            label_light_ambient_r.SetText(currentLight.diffuseColor.X.ToString("N2"));
            label_light_ambient_g.SetText(currentLight.diffuseColor.Y.ToString("N2"));
            label_light_ambient_b.SetText(currentLight.diffuseColor.Z.ToString("N2"));

            label_light_specular_r.SetText(currentLight.specularColor.X.ToString("N2"));
            label_light_specular_g.SetText(currentLight.specularColor.Y.ToString("N2"));
            label_light_specular_b.SetText(currentLight.specularColor.Z.ToString("N2"));

        }

        public void controlPointScreenRender()
        {
            background_cp.Render();

            if (containerControlPointMenu.getCurrentPos() == 0)
            {
                containerControlPointMenu.unset();
                button_x_up.set(); 
            }
            else if (containerControlPointMenu.getCurrentPos() == 1)
            {
                containerControlPointMenu.unset();
                button_x_down.set();
            }
            else if (containerControlPointMenu.getCurrentPos() == 2)
            {
                containerControlPointMenu.unset();
                button_y_up.set();
            }
            else if (containerControlPointMenu.getCurrentPos() == 3)
            {
                containerControlPointMenu.unset();
                button_y_down.set();
            }
            else if (containerControlPointMenu.getCurrentPos() == 4)
            {
                containerControlPointMenu.unset();
                button_z_up.set();
            }
            else if (containerControlPointMenu.getCurrentPos() == 5)
            {
                containerControlPointMenu.unset();
                button_z_down.set();
            }
            else if (containerControlPointMenu.getCurrentPos() == 6)
            {
                containerControlPointMenu.unset();
                button_radius_up.set();
            }
            else if (containerControlPointMenu.getCurrentPos() == 7)
            {
                containerControlPointMenu.unset();
                button_radius_down.set();
            }
            else if (containerControlPointMenu.getCurrentPos() == 8)
            {
                containerControlPointMenu.unset();
                button_apply.set();
            }
            else if (containerControlPointMenu.getCurrentPos() == 9)
            {
                containerControlPointMenu.unset();
                button_tableView.set();
            }

            containerControlPointMenu.Render();
            label_x.Render();
            label_y.Render();
            label_z.Render();
            label_radius.Render();
        }


        public void lightScreenInit()
        {

            int viewport_x = ScreenManager.GraphicsDevice.Viewport.Width;
            int viewport_y = ScreenManager.GraphicsDevice.Viewport.Height;

            containerLightMenu = new Container();

            currentLight = new tableview.lightList();
            currentLight.position = new Vector3(0.0f, 0.0f, 0.0f);
            currentLight.direction = new Vector3(0.0f, 0.0f, 0.0f);
            currentLight.diffuseColor = new Vector3(0f, 0f, 0f);
            currentLight.specularColor = new Vector3(0f, 0f, 0f);

            Texture2D[] button_tex_down = new Texture2D[2];
            Texture2D[] button_tex_up = new Texture2D[2];
            Texture2D[] button_tex_apply = new Texture2D[2];
            Texture2D[] button_tex_list = new Texture2D[2];
            Texture2D[] button_tex_getpos = new Texture2D[2];
            Texture2D[] button_tex_getdir = new Texture2D[2];
            Texture2D[] background_tex_light = new Texture2D[2];

            button_tex_up[0] = content.Load<Texture2D>("Content/Textures/TrackEditor/button_up");
            button_tex_up[1] = content.Load<Texture2D>("Content/Textures/TrackEditor/button_up_active");
            button_tex_down[0] = content.Load<Texture2D>("Content/Textures/TrackEditor/button_down");
            button_tex_down[1] = content.Load<Texture2D>("Content/Textures/TrackEditor/button_down_active");
            button_tex_apply[0] = content.Load<Texture2D>("Content/Textures/TrackEditor/button_apply");
            button_tex_apply[1] = content.Load<Texture2D>("Content/Textures/TrackEditor/button_apply_active");
            button_tex_list[0] = content.Load<Texture2D>("Content/Textures/TrackEditor/button_list");
            button_tex_list[1] = content.Load<Texture2D>("Content/Textures/TrackEditor/button_list_active");
            button_tex_getpos[0] = content.Load<Texture2D>("Content/Textures/TrackEditor/button_getpos");
            button_tex_getpos[1] = content.Load<Texture2D>("Content/Textures/TrackEditor/button_getpos_active");
            button_tex_getdir[0] = content.Load<Texture2D>("Content/Textures/TrackEditor/button_getdir");
            button_tex_getdir[1] = content.Load<Texture2D>("Content/Textures/TrackEditor/button_getdir_active");
            background_tex_light[0] = content.Load<Texture2D>("Content/Textures/TrackEditor/screen_light");
            background_tex_light[1] = content.Load<Texture2D>("Content/Textures/TrackEditor/screen_light");

            button_light_x_up = new Button(new Vector2(viewport_x * 0.125f, viewport_y * 0.083f), new Vector2(viewport_x * 0.025f, viewport_y * 0.033f), button_tex_up, ScreenManager);
            button_light_x_down = new Button(new Vector2(viewport_x * 0.125f, viewport_y * 0.116f), new Vector2(viewport_x * 0.025f, viewport_y * 0.033f), button_tex_down, ScreenManager);
            button_light_y_up = new Button(new Vector2(viewport_x * 0.125f, viewport_y * 0.233f), new Vector2(viewport_x * 0.025f, viewport_y * 0.033f), button_tex_up, ScreenManager);
            button_light_y_down = new Button(new Vector2(viewport_x * 0.125f, viewport_y * 0.266f), new Vector2(viewport_x * 0.025f, viewport_y * 0.033f), button_tex_down, ScreenManager);
            button_light_z_up = new Button(new Vector2(viewport_x * 0.125f, viewport_y * 0.383f), new Vector2(viewport_x * 0.025f, viewport_y * 0.033f), button_tex_up, ScreenManager);
            button_light_z_down = new Button(new Vector2(viewport_x * 0.125f, viewport_y * 0.416f), new Vector2(viewport_x * 0.025f, viewport_y * 0.033f), button_tex_down, ScreenManager);

            button_lightdir_x_up = new Button(new Vector2(viewport_x * 0.125f, viewport_y * 0.533f), new Vector2(viewport_x * 0.025f, viewport_y * 0.033f), button_tex_up, ScreenManager);
            button_lightdir_x_down = new Button(new Vector2(viewport_x * 0.125f, viewport_y * 0.566f), new Vector2(viewport_x * 0.025f, viewport_y * 0.033f), button_tex_down, ScreenManager);
            button_lightdir_y_up = new Button(new Vector2(viewport_x * 0.125f, viewport_y * 0.683f), new Vector2(viewport_x * 0.025f, viewport_y * 0.033f), button_tex_up, ScreenManager);
            button_lightdir_y_down = new Button(new Vector2(viewport_x * 0.125f, viewport_y * 0.716f), new Vector2(viewport_x * 0.025f, viewport_y * 0.033f), button_tex_down, ScreenManager);
            button_lightdir_z_up = new Button(new Vector2(viewport_x * 0.125f, viewport_y * 0.833f), new Vector2(viewport_x * 0.025f, viewport_y * 0.033f), button_tex_up, ScreenManager);
            button_lightdir_z_down = new Button(new Vector2(viewport_x * 0.125f, viewport_y * 0.866f), new Vector2(viewport_x * 0.025f, viewport_y * 0.033f), button_tex_down, ScreenManager);

            button_light_ambient_r_up = new Button(new Vector2(viewport_x * 0.25f, viewport_y * 0.083f), new Vector2(viewport_x * 0.025f, viewport_y * 0.033f), button_tex_up, ScreenManager);
            button_light_ambient_r_down = new Button(new Vector2(viewport_x * 0.25f, viewport_y * 0.116f), new Vector2(viewport_x * 0.025f, viewport_y * 0.033f), button_tex_down, ScreenManager);
            button_light_ambient_g_up = new Button(new Vector2(viewport_x * 0.25f, viewport_y * 0.233f), new Vector2(viewport_x * 0.025f, viewport_y * 0.033f), button_tex_up, ScreenManager);
            button_light_ambient_g_down = new Button(new Vector2(viewport_x * 0.25f, viewport_y * 0.266f), new Vector2(viewport_x * 0.025f, viewport_y * 0.033f), button_tex_down, ScreenManager);
            button_light_ambient_b_up = new Button(new Vector2(viewport_x * 0.25f, viewport_y * 0.383f), new Vector2(viewport_x * 0.025f, viewport_y * 0.033f), button_tex_up, ScreenManager);
            button_light_ambient_b_down = new Button(new Vector2(viewport_x * 0.25f, viewport_y * 0.416f), new Vector2(viewport_x * 0.025f, viewport_y * 0.033f), button_tex_down, ScreenManager);

            button_light_specular_r_up = new Button(new Vector2(viewport_x * 0.25f, viewport_y * 0.533f), new Vector2(viewport_x * 0.025f, viewport_y * 0.033f), button_tex_up, ScreenManager);
            button_light_specular_r_down = new Button(new Vector2(viewport_x * 0.25f, viewport_y * 0.566f), new Vector2(viewport_x * 0.025f, viewport_y * 0.033f), button_tex_down, ScreenManager);
            button_light_specular_g_up = new Button(new Vector2(viewport_x * 0.25f, viewport_y * 0.683f), new Vector2(viewport_x * 0.025f, viewport_y * 0.033f), button_tex_up, ScreenManager);
            button_light_specular_g_down = new Button(new Vector2(viewport_x * 0.25f, viewport_y * 0.716f), new Vector2(viewport_x * 0.025f, viewport_y * 0.033f), button_tex_down, ScreenManager);
            button_light_specular_b_up = new Button(new Vector2(viewport_x * 0.25f, viewport_y * 0.833f), new Vector2(viewport_x * 0.025f, viewport_y * 0.033f), button_tex_up, ScreenManager);
            button_light_specular_b_down = new Button(new Vector2(viewport_x * 0.25f, viewport_y * 0.866f), new Vector2(viewport_x * 0.025f, viewport_y * 0.033f), button_tex_down, ScreenManager);

            button_light_apply = new Button(new Vector2(viewport_x * 0.0125f, viewport_y * 0.95f), new Vector2(viewport_x * 0.075f, viewport_y * 0.033f), button_tex_apply, ScreenManager);
            button_light_list = new Button(new Vector2(viewport_x * 0.133f, viewport_y * 0.95f), new Vector2(viewport_x * 0.075f, viewport_y * 0.033f), button_tex_list, ScreenManager);
            button_light_getpos = new Button(new Vector2(viewport_x * 0.25f, viewport_y * 0.95f), new Vector2(viewport_x * 0.075f, viewport_y * 0.033f), button_tex_getpos, ScreenManager);
            button_light_getdir = new Button(new Vector2(viewport_x * 0.36875f, viewport_y * 0.95f), new Vector2(viewport_x * 0.075f, viewport_y * 0.033f), button_tex_getdir, ScreenManager);

            label_light_x = new Label(currentLight.position.X.ToString("N1"), new Vector2(viewport_x * 0.0166f, viewport_y * 0.083f), font, Color.Red, ScreenManager);
            label_light_y = new Label(currentLight.position.Y.ToString("N1"), new Vector2(viewport_x * 0.0166f, viewport_y * 0.233f), font, Color.Red, ScreenManager);
            label_light_z = new Label(currentLight.position.Z.ToString("N1"), new Vector2(viewport_x * 0.0166f, viewport_y * 0.383f), font, Color.Red, ScreenManager);

            label_lightdir_x = new Label(currentLight.direction.X.ToString("N1"), new Vector2(viewport_x * 0.0166f, viewport_y * 0.533f), font, Color.Red, ScreenManager);
            label_lightdir_y = new Label(currentLight.direction.Y.ToString("N1"), new Vector2(viewport_x * 0.0166f, viewport_y * 0.683f), font, Color.Red, ScreenManager);
            label_lightdir_z = new Label(currentLight.direction.Z.ToString("N1"), new Vector2(viewport_x * 0.0166f, viewport_y * 0.833f), font, Color.Red, ScreenManager);

            label_light_ambient_r = new Label(currentLight.diffuseColor.X.ToString("N2"), new Vector2(viewport_x * 0.166f, viewport_y * 0.083f), font, Color.Red, ScreenManager);
            label_light_ambient_g = new Label(currentLight.diffuseColor.Y.ToString("N2"), new Vector2(viewport_x * 0.166f, viewport_y * 0.233f), font, Color.Red, ScreenManager);
            label_light_ambient_b = new Label(currentLight.diffuseColor.Z.ToString("N2"), new Vector2(viewport_x * 0.166f, viewport_y * 0.383f), font, Color.Red, ScreenManager);

            label_light_specular_r = new Label(currentLight.specularColor.X.ToString("N2"), new Vector2(viewport_x * 0.166f, viewport_y * 0.533f), font, Color.Red, ScreenManager);
            label_light_specular_g = new Label(currentLight.specularColor.Y.ToString("N2"), new Vector2(viewport_x * 0.166f, viewport_y * 0.683f), font, Color.Red, ScreenManager);
            label_light_specular_b = new Label(currentLight.specularColor.Z.ToString("N2"), new Vector2(viewport_x * 0.166f, viewport_y * 0.833f), font, Color.Red, ScreenManager);

            background_light = new Button(new Vector2(0, 0), new Vector2(viewport_x, viewport_y), background_tex_light, ScreenManager);

            containerLightMenu.Add(button_light_x_up);
            containerLightMenu.Add(button_light_x_down);
            containerLightMenu.Add(button_light_y_up);
            containerLightMenu.Add(button_light_y_down);
            containerLightMenu.Add(button_light_z_up);
            containerLightMenu.Add(button_light_z_down);

            containerLightMenu.Add(button_lightdir_x_up);
            containerLightMenu.Add(button_lightdir_x_down);
            containerLightMenu.Add(button_lightdir_y_up);
            containerLightMenu.Add(button_lightdir_y_down);
            containerLightMenu.Add(button_lightdir_z_up);
            containerLightMenu.Add(button_lightdir_z_down);

            containerLightMenu.Add(button_light_ambient_r_up);
            containerLightMenu.Add(button_light_ambient_r_down);
            containerLightMenu.Add(button_light_ambient_g_up);
            containerLightMenu.Add(button_light_ambient_g_down);
            containerLightMenu.Add(button_light_ambient_b_up);
            containerLightMenu.Add(button_light_ambient_b_down);

            containerLightMenu.Add(button_light_specular_r_up);
            containerLightMenu.Add(button_light_specular_r_down);
            containerLightMenu.Add(button_light_specular_g_up);
            containerLightMenu.Add(button_light_specular_g_down);
            containerLightMenu.Add(button_light_specular_b_up);
            containerLightMenu.Add(button_light_specular_b_down);

            containerLightMenu.Add(button_light_apply);
            containerLightMenu.Add(button_light_list);
            containerLightMenu.Add(button_light_getpos);
            containerLightMenu.Add(button_light_getdir);
        }

        public void lightScreenRender()
        {
            background_light.Render();

            if (containerLightMenu.getCurrentPos() == 0)
            {
                containerLightMenu.unset();
                button_light_x_up.set();
            }
            else if (containerLightMenu.getCurrentPos() == 1)
            {
                containerLightMenu.unset();
                button_light_x_down.set();
            }
            else if (containerLightMenu.getCurrentPos() == 2)
            {
                containerLightMenu.unset();
                button_light_y_up.set();
            }
            else if (containerLightMenu.getCurrentPos() == 3)
            {
                containerLightMenu.unset();
                button_light_y_down.set();
            }
            else if (containerLightMenu.getCurrentPos() == 4)
            {
                containerLightMenu.unset();
                button_light_z_up.set();
            }
            else if (containerLightMenu.getCurrentPos() == 5)
            {
                containerLightMenu.unset();
                button_light_z_down.set();
            }
            else if (containerLightMenu.getCurrentPos() == 6)
            {
                containerLightMenu.unset();
                button_lightdir_x_up.set();
            }
            else if (containerLightMenu.getCurrentPos() == 7)
            {
                containerLightMenu.unset();
                button_lightdir_x_down.set();
            }
            else if (containerLightMenu.getCurrentPos() == 8)
            {
                containerLightMenu.unset();
                button_lightdir_y_up.set();
            }
            else if (containerLightMenu.getCurrentPos() == 9)
            {
                containerLightMenu.unset();
                button_lightdir_y_down.set();
            }
            else if (containerLightMenu.getCurrentPos() == 10)
            {
                containerLightMenu.unset();
                button_lightdir_z_up.set();
            }
            else if (containerLightMenu.getCurrentPos() == 11)
            {
                containerLightMenu.unset();
                button_lightdir_z_down.set();
            }
            else if (containerLightMenu.getCurrentPos() == 12)
            {
                containerLightMenu.unset();
                button_light_ambient_r_up.set();
            }
            else if (containerLightMenu.getCurrentPos() == 13)
            {
                containerLightMenu.unset();
                button_light_ambient_r_down.set();
            }
            else if (containerLightMenu.getCurrentPos() == 14)
            {
                containerLightMenu.unset();
                button_light_ambient_g_up.set();
            }
            else if (containerLightMenu.getCurrentPos() == 15)
            {
                containerLightMenu.unset();
                button_light_ambient_g_down.set();
            }
            else if (containerLightMenu.getCurrentPos() == 16)
            {
                containerLightMenu.unset();
                button_light_ambient_b_up.set();
            }
            else if (containerLightMenu.getCurrentPos() == 17)
            {
                containerLightMenu.unset();
                button_light_ambient_b_down.set();
            }
            else if (containerLightMenu.getCurrentPos() == 18)
            {
                containerLightMenu.unset();
                button_light_specular_r_up.set();
            }
            else if (containerLightMenu.getCurrentPos() == 19)
            {
                containerLightMenu.unset();
                button_light_specular_r_down.set();
            }
            else if (containerLightMenu.getCurrentPos() == 20)
            {
                containerLightMenu.unset();
                button_light_specular_g_up.set();
            }
            else if (containerLightMenu.getCurrentPos() == 21)
            {
                containerLightMenu.unset();
                button_light_specular_g_down.set();
            }
            else if (containerLightMenu.getCurrentPos() == 22)
            {
                containerLightMenu.unset();
                button_light_specular_b_up.set();
            }
            else if (containerLightMenu.getCurrentPos() == 23)
            {
                containerLightMenu.unset();
                button_light_specular_b_down.set();
            }
            else if (containerLightMenu.getCurrentPos() == 24)
            {
                containerLightMenu.unset();
                button_light_apply.set();
            }
            else if (containerLightMenu.getCurrentPos() == 25)
            {
                containerLightMenu.unset();
                button_light_list.set();
            }
            else if (containerLightMenu.getCurrentPos() == 26)
            {
                containerLightMenu.unset();
                button_light_getpos.set();
            }
            else if (containerLightMenu.getCurrentPos() == 27)
            {
                containerLightMenu.unset();
                button_light_getdir.set();
            }

            containerLightMenu.Render();
            label_light_x.Render();
            label_light_y.Render();
            label_light_z.Render();
            label_lightdir_x.Render();
            label_lightdir_y.Render();
            label_lightdir_z.Render();
            label_light_ambient_r.Render();
            label_light_ambient_b.Render();
            label_light_ambient_g.Render();
            label_light_specular_r.Render();
            label_light_specular_b.Render();
            label_light_specular_g.Render();
        }

        public void loadScreenInit()
        {

            int viewport_x = ScreenManager.GraphicsDevice.Viewport.Width;
            int viewport_y = ScreenManager.GraphicsDevice.Viewport.Height;

            containerLoadMenu = new Container();

            Texture2D[] button_tex_right = new Texture2D[2];
            Texture2D[] button_tex_left = new Texture2D[2];
            Texture2D[] button_tex_apply = new Texture2D[2];
            Texture2D[] background_tex_load = new Texture2D[2];

            button_tex_right[0] = content.Load<Texture2D>("Content/Textures/TrackEditor/button_right");
            button_tex_right[1] = content.Load<Texture2D>("Content/Textures/TrackEditor/button_right_active");
            button_tex_left[0] = content.Load<Texture2D>("Content/Textures/TrackEditor/button_left");
            button_tex_left[1] = content.Load<Texture2D>("Content/Textures/TrackEditor/button_left_active");
            button_tex_apply[0] = content.Load<Texture2D>("Content/Textures/TrackEditor/button_apply");
            button_tex_apply[1] = content.Load<Texture2D>("Content/Textures/TrackEditor/button_apply_active");
            background_tex_load[0] = content.Load<Texture2D>("Content/Textures/TrackEditor/screen_load");
            background_tex_load[1] = content.Load<Texture2D>("Content/Textures/TrackEditor/screen_load");

            button_load_right = new Button(new Vector2(viewport_x * 0.2f, viewport_y * 0.1f), new Vector2(viewport_x * 0.025f, viewport_y * 0.033f), button_tex_right, ScreenManager);
            button_load_left = new Button(new Vector2(viewport_x * 0.1f, viewport_y * 0.1f), new Vector2(viewport_x * 0.025f, viewport_y * 0.033f), button_tex_left, ScreenManager);
            button_load_apply = new Button(new Vector2(viewport_x * 0.125f, viewport_y * 0.2f), new Vector2(viewport_x * 0.075f, viewport_y * 0.033f), button_tex_apply, ScreenManager);

            label_load_count = new Label(loadSlot.ToString(), new Vector2(viewport_x * 0.14f, viewport_y * 0.09f), font, Color.Red, ScreenManager);
            // label_load_info = new Label(currentPoint.Y.ToString(), new Vector2(viewport_x * 0.0166f, viewport_y * 0.233f), font, Color.Red, ScreenManager);

            background_load = new Button(new Vector2(0, 0), new Vector2(viewport_x, viewport_y), background_tex_load, ScreenManager);
            
            containerLoadMenu.Add(button_load_right);
            containerLoadMenu.Add(button_load_left);
            containerLoadMenu.Add(button_load_apply);
        }

        public void loadScreenRender()
        {
            background_load.Render();
            
            if (containerLoadMenu.getCurrentPos() == 0)
            {
                containerLoadMenu.unset();
                button_load_left.set();
            }
            else if (containerLoadMenu.getCurrentPos() == 1)
            {
                containerLoadMenu.unset();
                button_load_right.set();
            }
            else if (containerLoadMenu.getCurrentPos() == 2)
            {
                containerLoadMenu.unset();
                button_load_apply.set();
            }

            containerLoadMenu.Render();
            label_load_count.Render();
            //label_load_info.Render();

        }

        public void saveScreenInit()
        {

            int viewport_x = ScreenManager.GraphicsDevice.Viewport.Width;
            int viewport_y = ScreenManager.GraphicsDevice.Viewport.Height;

            containerSaveMenu = new Container();

            Texture2D[] button_tex_right = new Texture2D[2];
            Texture2D[] button_tex_left = new Texture2D[2];
            Texture2D[] button_tex_apply = new Texture2D[2];
            Texture2D[] background_tex_save = new Texture2D[2];

            button_tex_right[0] = content.Load<Texture2D>("Content/Textures/TrackEditor/button_right");
            button_tex_right[1] = content.Load<Texture2D>("Content/Textures/TrackEditor/button_right_active");
            button_tex_left[0] = content.Load<Texture2D>("Content/Textures/TrackEditor/button_left");
            button_tex_left[1] = content.Load<Texture2D>("Content/Textures/TrackEditor/button_left_active");
            button_tex_apply[0] = content.Load<Texture2D>("Content/Textures/TrackEditor/button_apply");
            button_tex_apply[1] = content.Load<Texture2D>("Content/Textures/TrackEditor/button_apply_active");
            background_tex_save[0] = content.Load<Texture2D>("Content/Textures/TrackEditor/screen_save");
            background_tex_save[1] = content.Load<Texture2D>("Content/Textures/TrackEditor/screen_save");

            button_save_right = new Button(new Vector2(viewport_x * 0.2f, viewport_y * 0.1f), new Vector2(viewport_x * 0.025f, viewport_y * 0.033f), button_tex_right, ScreenManager);
            button_save_left = new Button(new Vector2(viewport_x * 0.1f, viewport_y * 0.1f), new Vector2(viewport_x * 0.025f, viewport_y * 0.033f), button_tex_left, ScreenManager);
            button_save_apply = new Button(new Vector2(viewport_x * 0.125f, viewport_y * 0.2f), new Vector2(viewport_x * 0.075f, viewport_y * 0.033f), button_tex_apply, ScreenManager);

            label_save_count = new Label(saveSlot.ToString(), new Vector2(viewport_x * 0.14f, viewport_y * 0.09f), font, Color.Red, ScreenManager);
            // label_save_info = new Label(currentPoint.Y.ToString(), new Vector2(viewport_x * 0.0166f, viewport_y * 0.233f), font, Color.Red, ScreenManager);

            background_save = new Button(new Vector2(0, 0), new Vector2(viewport_x, viewport_y), background_tex_save, ScreenManager);

            containerSaveMenu.Add(button_save_right);
            containerSaveMenu.Add(button_save_left);
            containerSaveMenu.Add(button_save_apply);
        }

        public void saveScreenRender()
        {
            background_save.Render();

            if (containerSaveMenu.getCurrentPos() == 0)
            {
                containerSaveMenu.unset();
                button_save_left.set();
            }
            else if (containerSaveMenu.getCurrentPos() == 1)
            {
                containerSaveMenu.unset();
                button_save_right.set();
            }
            else if (containerSaveMenu.getCurrentPos() == 2)
            {
                containerSaveMenu.unset();
                button_save_apply.set();
            }

            containerSaveMenu.Render();
            label_save_count.Render();
            //label_save_info.Render();
        }

        public void pickupPointScreenInit()
        {

            int viewport_x = ScreenManager.GraphicsDevice.Viewport.Width;
            int viewport_y = ScreenManager.GraphicsDevice.Viewport.Height;

            containerPickupPointMenu = new Container();

            currentPickupPoint = new Vector3(0f, 0f, 0f);
            currentPickupID = 1;

            Texture2D[] button_tex_down = new Texture2D[2];
            Texture2D[] button_tex_up = new Texture2D[2];
            Texture2D[] button_tex_apply = new Texture2D[2];
            Texture2D[] button_tex_list = new Texture2D[2];
            Texture2D[] button_tex_getpos = new Texture2D[2];

            Texture2D[] pic_tex_banana = new Texture2D[2];
            Texture2D[] pic_tex_turbo = new Texture2D[2];
            Texture2D[] pic_tex_health = new Texture2D[2];
            Texture2D[] pic_tex_shield = new Texture2D[2];
            Texture2D[] pic_tex_bomb = new Texture2D[2];
            Texture2D[] pic_tex_dice = new Texture2D[2];
            Texture2D[] pic_tex_wall = new Texture2D[2];
            Texture2D[] pic_tex_rocket = new Texture2D[2];
            Texture2D[] background_tex_pickup = new Texture2D[2];

            button_tex_up[0] = content.Load<Texture2D>("Content/Textures/TrackEditor/button_up");
            button_tex_up[1] = content.Load<Texture2D>("Content/Textures/TrackEditor/button_up_active");
            button_tex_down[0] = content.Load<Texture2D>("Content/Textures/TrackEditor/button_down");
            button_tex_down[1] = content.Load<Texture2D>("Content/Textures/TrackEditor/button_down_active");

            button_tex_apply[0] = content.Load<Texture2D>("Content/Textures/TrackEditor/button_apply");
            button_tex_apply[1] = content.Load<Texture2D>("Content/Textures/TrackEditor/button_apply_active");
            button_tex_list[0] = content.Load<Texture2D>("Content/Textures/TrackEditor/button_list");
            button_tex_list[1] = content.Load<Texture2D>("Content/Textures/TrackEditor/button_list_active");
            
            button_tex_getpos[0] = content.Load<Texture2D>("Content/Textures/TrackEditor/button_getpos");
            button_tex_getpos[1] = content.Load<Texture2D>("Content/Textures/TrackEditor/button_getpos_active");

            pic_tex_banana[0] = pic_tex_banana[1] = content.Load<Texture2D>("Content/Textures/TrackEditor/pic_banane");
            pic_tex_bomb[0] = pic_tex_bomb[1] = content.Load<Texture2D>("Content/Textures/TrackEditor/pic_bomb");
            pic_tex_turbo[0] = pic_tex_turbo[1] = content.Load<Texture2D>("Content/Textures/TrackEditor/pic_turbo");
            pic_tex_shield[0] = pic_tex_shield[1] = content.Load<Texture2D>("Content/Textures/TrackEditor/pic_shield");
            pic_tex_health[0] = pic_tex_health[1] = content.Load<Texture2D>("Content/Textures/TrackEditor/pic_healthpack");
            pic_tex_dice[0] = pic_tex_dice[1] = content.Load<Texture2D>("Content/Textures/TrackEditor/pic_dice");
            pic_tex_rocket[0] = pic_tex_rocket[1] = content.Load<Texture2D>("Content/Textures/TrackEditor/pic_rocket");
            pic_tex_wall[0] = pic_tex_wall[1] = content.Load<Texture2D>("Content/Textures/TrackEditor/pic_wall");

            background_tex_pickup[0] = background_tex_pickup[1] = content.Load<Texture2D>("Content/Textures/TrackEditor/screen_pickup");

            button_pickup_x_up = new Button(new Vector2(viewport_x * 0.125f, viewport_y * 0.083f), new Vector2(viewport_x * 0.025f, viewport_y * 0.033f), button_tex_up, ScreenManager);
            button_pickup_x_down = new Button(new Vector2(viewport_x * 0.125f, viewport_y * 0.116f), new Vector2(viewport_x * 0.025f, viewport_y * 0.033f), button_tex_down, ScreenManager);
            button_pickup_y_up = new Button(new Vector2(viewport_x * 0.125f, viewport_y * 0.233f), new Vector2(viewport_x * 0.025f, viewport_y * 0.033f), button_tex_up, ScreenManager);
            button_pickup_y_down = new Button(new Vector2(viewport_x * 0.125f, viewport_y * 0.266f), new Vector2(viewport_x * 0.025f, viewport_y * 0.033f), button_tex_down, ScreenManager);
            button_pickup_z_up = new Button(new Vector2(viewport_x * 0.125f, viewport_y * 0.383f), new Vector2(viewport_x * 0.025f, viewport_y * 0.033f), button_tex_up, ScreenManager);
            button_pickup_z_down = new Button(new Vector2(viewport_x * 0.125f, viewport_y * 0.416f), new Vector2(viewport_x * 0.025f, viewport_y * 0.033f), button_tex_down, ScreenManager);
            button_pickup_id_up = new Button(new Vector2(viewport_x * 0.25f, viewport_y * 0.083f), new Vector2(viewport_x * 0.025f, viewport_y * 0.033f), button_tex_up, ScreenManager);
            button_pickup_id_down = new Button(new Vector2(viewport_x * 0.25f, viewport_y * 0.116f), new Vector2(viewport_x * 0.025f, viewport_y * 0.033f), button_tex_down, ScreenManager);

            button_pickup_apply = new Button(new Vector2(viewport_x * 0.36875f, viewport_y * 0.95f), new Vector2(viewport_x * 0.075f, viewport_y * 0.033f), button_tex_apply, ScreenManager);
            button_pickup_tableView = new Button(new Vector2(viewport_x * 0.4875f, viewport_y * 0.95f), new Vector2(viewport_x * 0.075f, viewport_y * 0.033f), button_tex_list, ScreenManager);
            button_pickup_getpos = new Button(new Vector2(viewport_x * 0.6125f, viewport_y * 0.95f), new Vector2(viewport_x * 0.075f, viewport_y * 0.033f), button_tex_getpos, ScreenManager);

            label_pickup_x = new Label(currentPickupPoint.X.ToString("N1"), new Vector2(viewport_x * 0.0166f, viewport_y * 0.083f), font, Color.Red, ScreenManager);
            label_pickup_y = new Label(currentPickupPoint.Y.ToString("N1"), new Vector2(viewport_x * 0.0166f, viewport_y * 0.233f), font, Color.Red, ScreenManager);
            label_pickup_z = new Label(currentPickupPoint.Z.ToString("N1"), new Vector2(viewport_x * 0.0166f, viewport_y * 0.383f), font, Color.Red, ScreenManager);
            label_pickup_id = new Label(currentPickupID.ToString(), new Vector2(viewport_x * 0.166f, viewport_y * 0.083f), font, Color.Red, ScreenManager);

            pic_banana = new Button(new Vector2(viewport_x * 0.17f, viewport_y * 0.2f), new Vector2(viewport_x * 0.15f, viewport_y * 0.15f), pic_tex_banana, ScreenManager);
            pic_turbo = new Button(new Vector2(viewport_x * 0.17f, viewport_y * 0.2f), new Vector2(viewport_x * 0.15f, viewport_y * 0.15f), pic_tex_turbo, ScreenManager);
            pic_health = new Button(new Vector2(viewport_x * 0.17f, viewport_y * 0.2f), new Vector2(viewport_x * 0.15f, viewport_y * 0.15f), pic_tex_health, ScreenManager);
            pic_bomb = new Button(new Vector2(viewport_x * 0.17f, viewport_y * 0.2f), new Vector2(viewport_x * 0.1f, viewport_y * 0.15f), pic_tex_bomb, ScreenManager);
            pic_shield = new Button(new Vector2(viewport_x * 0.17f, viewport_y * 0.2f), new Vector2(viewport_x * 0.15f, viewport_y * 0.15f), pic_tex_shield, ScreenManager);
            pic_dice = new Button(new Vector2(viewport_x * 0.17f, viewport_y * 0.2f), new Vector2(viewport_x * 0.15f, viewport_y * 0.15f), pic_tex_dice, ScreenManager);
            pic_wall = new Button(new Vector2(viewport_x * 0.17f, viewport_y * 0.2f), new Vector2(viewport_x * 0.15f, viewport_y * 0.15f), pic_tex_wall, ScreenManager);
            pic_rocket = new Button(new Vector2(viewport_x * 0.17f, viewport_y * 0.2f), new Vector2(viewport_x * 0.15f, viewport_y * 0.15f), pic_tex_rocket, ScreenManager);

            background_pickup = new Button(new Vector2(0, 0), new Vector2(viewport_x, viewport_y), background_tex_pickup, ScreenManager); 

            containerPickupPointMenu.Add(button_pickup_x_up);
            containerPickupPointMenu.Add(button_pickup_x_down);
            containerPickupPointMenu.Add(button_pickup_y_up);
            containerPickupPointMenu.Add(button_pickup_y_down);
            containerPickupPointMenu.Add(button_pickup_z_up);
            containerPickupPointMenu.Add(button_pickup_z_down);
            containerPickupPointMenu.Add(button_pickup_id_up);
            containerPickupPointMenu.Add(button_pickup_id_down);
            containerPickupPointMenu.Add(button_pickup_apply);
            containerPickupPointMenu.Add(button_pickup_tableView);
            containerPickupPointMenu.Add(button_pickup_getpos);
        }

        public void pickupPointScreenRender()
        {
            background_pickup.Render();

            if (containerPickupPointMenu.getCurrentPos() == 0)
            {
                containerPickupPointMenu.unset();
                button_pickup_x_up.set();
            }
            else if (containerPickupPointMenu.getCurrentPos() == 1)
            {
                containerPickupPointMenu.unset();
                button_pickup_x_down.set();
            }
            else if (containerPickupPointMenu.getCurrentPos() == 2)
            {
                containerPickupPointMenu.unset();
                button_pickup_y_up.set();
            }
            else if (containerPickupPointMenu.getCurrentPos() == 3)
            {
                containerPickupPointMenu.unset();
                button_pickup_y_down.set();
            }
            else if (containerPickupPointMenu.getCurrentPos() == 4)
            {
                containerPickupPointMenu.unset();
                button_pickup_z_up.set();
            }
            else if (containerPickupPointMenu.getCurrentPos() == 5)
            {
                containerPickupPointMenu.unset();
                button_pickup_z_down.set();
            }
            else if (containerPickupPointMenu.getCurrentPos() == 6)
            {
                containerPickupPointMenu.unset();
                button_pickup_id_up.set();
            }
            else if (containerPickupPointMenu.getCurrentPos() == 7)
            {
                containerPickupPointMenu.unset();
                button_pickup_id_down.set();
            }
            else if (containerPickupPointMenu.getCurrentPos() == 8)
            {
                containerPickupPointMenu.unset();
                button_pickup_apply.set();
            }
            else if (containerPickupPointMenu.getCurrentPos() == 9)
            {
                containerPickupPointMenu.unset();
                button_pickup_tableView.set();
            }
            else if (containerPickupPointMenu.getCurrentPos() == 10)
            {
                containerPickupPointMenu.unset();
                button_pickup_getpos.set();
            }

            containerPickupPointMenu.Render();
            label_pickup_x.Render();
            label_pickup_y.Render();
            label_pickup_z.Render();
            label_pickup_id.Render();

            if (currentPickupID == 1)
            {
                pic_banana.Render();
            }
            else if (currentPickupID == 2)
            {
                pic_bomb.Render();
            }
            else if (currentPickupID == 3)
            {
                pic_shield.Render();
            }
            else if (currentPickupID == 4)
            {
                pic_turbo.Render();
            }
            else if (currentPickupID == 5)
            {
                pic_health.Render();
            }
            else if (currentPickupID == 6)
            {
                pic_dice.Render();
            }
            else if (currentPickupID == 7)
            {
                pic_rocket.Render();
            }
            else if (currentPickupID == 8)
            {
                pic_wall.Render();
            }
        }

        private void TextureScreenInit()
        {
            int viewport_x = ScreenManager.GraphicsDevice.Viewport.Width;
            int viewport_y = ScreenManager.GraphicsDevice.Viewport.Height;

            containerTextureMenu = new Container();

            currentLayout = 0;

            Texture2D[] button_tex_left = new Texture2D[2];
            Texture2D[] button_tex_right = new Texture2D[2];
            Texture2D[] button_tex_apply = new Texture2D[2];
            Texture2D[] background_tex_texture = new Texture2D[2];

            button_tex_left[0] = content.Load<Texture2D>("Content/Textures/TrackEditor/button_left");
            button_tex_left[1] = content.Load<Texture2D>("Content/Textures/TrackEditor/button_left_active");
            button_tex_right[0] = content.Load<Texture2D>("Content/Textures/TrackEditor/button_right");
            button_tex_right[1] = content.Load<Texture2D>("Content/Textures/TrackEditor/button_right_active");
            button_tex_apply[0] = content.Load<Texture2D>("Content/Textures/TrackEditor/button_apply");
            button_tex_apply[1] = content.Load<Texture2D>("Content/Textures/TrackEditor/button_apply_active");

            background_tex_texture[0] = content.Load<Texture2D>("Content/Textures/TrackEditor/screen_textures");
            background_tex_texture[1] = content.Load<Texture2D>("Content/Textures/TrackEditor/screen_textures");

            button_texture_right = new Button(new Vector2(viewport_x * 0.3f, viewport_y * 0.1f), new Vector2(viewport_x * 0.025f, viewport_y * 0.033f), button_tex_right, ScreenManager);
            button_texture_left = new Button(new Vector2(viewport_x * 0.02f, viewport_y * 0.1f), new Vector2(viewport_x * 0.025f, viewport_y * 0.033f), button_tex_left, ScreenManager);
            button_texture_apply = new Button(new Vector2(viewport_x * 0.125f, viewport_y * 0.2f), new Vector2(viewport_x * 0.075f, viewport_y * 0.033f), button_tex_apply, ScreenManager);

            label_texture_name = new Label(trackLayouts.LayoutArray[currentLayout].name, new Vector2(viewport_x * 0.05f, viewport_y * 0.09f), font, Color.Red, ScreenManager);

            background_texture = new Button(new Vector2(0, 0), new Vector2(viewport_x, viewport_y), background_tex_texture, ScreenManager);

            containerTextureMenu.Add(button_texture_left);
            containerTextureMenu.Add(button_texture_right);
            containerTextureMenu.Add(button_texture_apply);


          /*  for(int i = 0; i < trackLayouts.LayoutArray.Length; i++)
            {
                 trackLayouts.LayoutArray */
        }

        private void TextureScreenRender()
        {
            background_texture.Render();

            if (containerTextureMenu.getCurrentPos() == 0)
            {
                containerTextureMenu.unset();
                button_texture_left.set();
            }
            else if (containerTextureMenu.getCurrentPos() == 1)
            {
                containerTextureMenu.unset();
                button_texture_right.set();
            }
            else if (containerTextureMenu.getCurrentPos() == 2)
            {
                containerTextureMenu.unset();
                button_texture_apply.set();
            }

            containerTextureMenu.Render();
            label_texture_name.Render();
            
        }


        #endregion
    }

    class Button
    {
        private SpriteBatch spriteBatch;
        private ScreenManager screenManager;

        public Vector2 position;
        public Vector2 size;
        public bool active = false;
        private Texture2D[] tex;
        private Rectangle window;
       

        public Button(Vector2 position, Vector2 size, Texture2D[] tex, ScreenManager screenManager)
        {
            this.screenManager = screenManager;
            spriteBatch = new SpriteBatch(this.screenManager.GraphicsDevice);
            this.position = position;
            this.size = size;
            this.tex = tex;

            window = new Rectangle((int)position.X, (int)position.Y, (int)size.X, (int)size.Y);
        }

        public void changeState()
        {
            active = !active;
        }

        public void set()
        {
            active = true;
        }

        public void unset()
        {
            active = false;
        }



        public bool IsActive()
        {
            return active;
        }

        public void Render()
        {
            spriteBatch.Begin();
            if (active)
            {
                spriteBatch.Draw(tex[1], window, Color.White);
            }
            else
            {
                spriteBatch.Draw(tex[0], window, Color.White);
            }
            spriteBatch.End();
        }
    }

    class Label
    {
        private SpriteFont font;
        private String text;
        private Vector2 pos;
        private Color color;

        private SpriteBatch spriteBatch;
        private ScreenManager screenManager;

        public Label(String text, Vector2 pos, SpriteFont font, Color color, ScreenManager screenManager)
        {

            this.screenManager = screenManager;
            spriteBatch = new SpriteBatch(this.screenManager.GraphicsDevice);

            this.text = text;
            this.pos = pos;
            this.font = font;
            this.color = color;
        }

        public void SetText(String text)
        {
            this.text = text;
        }

        public void Render()
        {
            spriteBatch.Begin();
                spriteBatch.DrawString(font, text, pos, color);
            spriteBatch.End();
        }
    }

    class Container
    {
        private List<Button> buttonList;
        private int currentPos = 0;
        private bool state = true;

        public Container()
        {
            buttonList = new List<Button>();
        }

        public void Add(Button button)
        {
            buttonList.Add(button);    
        }

        public void unset()
        {
            for (int i = 0; i < buttonList.Count; i++)
            {
                buttonList[i].unset();
            }
        }

        public int getPosCount()
        {
            return buttonList.Count;
        }

        public void CurrentPosUp()
        {
            currentPos++;
            
            if (currentPos >= buttonList.Count)
            {
                currentPos = buttonList.Count - 1;
            }
        }

        public void CurrentPosDown()
        {
            currentPos--;

            if (currentPos < 0)
            {
                currentPos = 0;
            }
        }

        public int getCurrentPos()
        {
            return currentPos;
        }

        public void Render()
        {
            for (int i = 0; i < buttonList.Count; i++)
            {
                buttonList[i].Render();
            }
        }

        public void setState(bool state)
        {
            this.state = state;
        }

        public bool isSelected()
        {
            return state;
        }
    }

    class tableview
    {
        public struct lightList
        {
            public Vector3 position;
            public Vector3 direction;
            public Vector3 diffuseColor;
            public Vector3 specularColor;
        }

        private ScreenManager screenmanager;
        private SpriteBatch sprite;

        private Vector2 position;
        private Vector2 size;
        private Rectangle window;
        private int activeItemNumber = 0;
        private bool state = false;
        private bool visibility = true;

        private SpritePosition tablebackground;
        private SpriteFont font;

        private List<Vector4> controlpoints;
        private List<lightList> lights;

        string typeText;

        public tableview(Vector2 Position, Vector2 Size, ScreenManager screenManager, ContentManager content, string type)
        {
            this.screenmanager = screenManager;
            this.size = Size;
            this.position = Position;
            typeText = type;
            sprite = new SpriteBatch(screenmanager.GraphicsDevice);

            tablebackground = new SpritePosition(content.Load<Texture2D>("Content/Textures/transparent"), (int)position.X, (int)position.Y, (int)size.X, (int)size.Y, Color.White);
            font = content.Load<SpriteFont>("Content/Fonts/tableviewfont");
            window = new Rectangle((int)position.X, (int)position.Y, (int)size.X, (int)size.Y);
        }

        public void setControlPoints(List<Vector4> points)
        {
            controlpoints = points;
        }

        public List<Vector4> getControlPoints()
        {
            return controlpoints;
        }

        public List<lightList> LightList
        {
            get
            {
                return lights;
            }

            set
            {
                lights = value;
            }
        }

        public bool Visibility
        {
            get
            {
                return visibility;
            }

            set
            {
                visibility = value;
            }
        }

        public int LightCount
        {
            get
            {
                return lights.Count;
            }
        }

        public int Count
        {
            get
            {
                return controlpoints.Count;
            }
        }

        public Vector4 getItemByNumber(int i)
        {
            if (i >= 0)
                return controlpoints[i];
            else
                return new Vector4(0.0f, 0.0f, 0.0f, 0.0f);
        }

        public lightList getLightsByNumber(int i)
        {
            if (i >= 0)
                return lights[i];
            else
                return new lightList();
        }

        public void RenderList()
        {
            sprite.Begin();
                sprite.Draw(tablebackground.sprite, tablebackground.window, tablebackground.color);
                Vector2 pos = new Vector2();
                pos = position;
                pos.X += 2;
                //Draw entries of the controlpoint list
                //Todo: limit viewed items to a certain number 
                if (state)
                {
                    for (int i = 0; i < controlpoints.Count; i++)
                    {
                        if (i == activeItemNumber)
                        {
                            sprite.DrawString(font, "X: " + controlpoints[i].X.ToString("N1")
                                + " Y: " + controlpoints[i].Y.ToString("N1")
                                + " Z: " + controlpoints[i].Z.ToString("N1")
                                + typeText + controlpoints[i].W.ToString("N1"), pos, Color.Silver);
                            pos.Y = pos.Y + 20;
                        }
                        else
                        {
                            sprite.DrawString(font, "X: " + controlpoints[i].X.ToString("N1")
                                + " Y: " + controlpoints[i].Y.ToString("N1")
                                + " Z: " + controlpoints[i].Z.ToString("N1")
                                + typeText + controlpoints[i].W.ToString("N1"), pos, Color.Tomato);
                            pos.Y = pos.Y + 20;
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < controlpoints.Count; i++)
                    {
                        if (i == activeItemNumber)
                        {
                            sprite.DrawString(font, "X: " + controlpoints[i].X.ToString("N1")
                                + " Y: " + controlpoints[i].Y.ToString("N1")
                                + " Z: " + controlpoints[i].Z.ToString("N1")
                                + typeText + controlpoints[i].W.ToString("N1"), pos, Color.Plum);
                            pos.Y = pos.Y + 20;
                        }
                        else
                        {
                            sprite.DrawString(font, "X: " + controlpoints[i].X.ToString("N1")
                                + " Y: " + controlpoints[i].Y.ToString("N1")
                                + " Z: " + controlpoints[i].Z.ToString("N1")
                                + typeText + controlpoints[i].W.ToString("N1"), pos, Color.Gray);
                            pos.Y = pos.Y + 20;
                        }
                    }
                }
            sprite.End();
        }

        public void renderLightList()
        {
            sprite.Begin();
            sprite.Draw(tablebackground.sprite, tablebackground.window, tablebackground.color);
            Vector2 pos = new Vector2();
            pos = position;
            pos.X += 2;
            //Draw entries of the controlpoint list
            //Todo: limit viewed items to a certain number 
            if (state)
            {
                for (int i = 0; i < lights.Count; i++)
                {
                    if (i == activeItemNumber)
                    {
                        sprite.DrawString(font, "X: " + lights[i].position.X.ToString("N1")
                            + " Y: " + lights[i].position.Y.ToString("N1")
                            + " Z: " + lights[i].position.Z.ToString("N1"), pos, Color.Silver);
                        pos.Y = pos.Y + 20;
                    }
                    else
                    {
                        sprite.DrawString(font, "X: " + lights[i].position.X.ToString("N1")
                            + " Y: " + lights[i].position.Y.ToString("N1")
                            + " Z: " + lights[i].position.Z.ToString("N1"), pos, Color.Tomato);
                        pos.Y = pos.Y + 20;
                    }
                }
            }
            else
            {
                for (int i = 0; i < lights.Count; i++)
                {
                    if (i == activeItemNumber)
                    {
                        sprite.DrawString(font, "X: " + lights[i].position.X.ToString("N1")
                            + " Y: " + lights[i].position.Y.ToString("N1")
                            + " Z: " + lights[i].position.Z.ToString("N1"), pos, Color.Plum);
                        pos.Y = pos.Y + 20;
                    }
                    else
                    {
                        sprite.DrawString(font, "X: " + lights[i].position.X.ToString("N1")
                            + " Y: " + lights[i].position.Y.ToString("N1")
                            + " Z: " + lights[i].position.Z.ToString("N1"), pos, Color.Tomato);
                        pos.Y = pos.Y + 20;
                    }
                }
            }
            sprite.End();
        }

        public int getActiveItemNumber()
        {
            return activeItemNumber;
        }
        public void setActiveItem(int activeNumber)
        {
            activeItemNumber = activeNumber;
            if (activeItemNumber < 0)
            {
                activeItemNumber = 0;
            }
            else if(activeItemNumber >= controlpoints.Count)
            {
                activeItemNumber = controlpoints.Count - 1;
            }
        }

        public void setActiveLight(int activeNumber)
        {
            activeItemNumber = activeNumber;
            if (activeItemNumber < 0)
            {
                activeItemNumber = 0;
            }
            else if (activeItemNumber >= lights.Count)
            {
                activeItemNumber = lights.Count - 1;
            }
        }

        public Vector4 getActiveItem()
        {
            if (controlpoints.Count > 0)
            {
                return controlpoints[activeItemNumber];
            }
            else
            {
                return new Vector4(0.0f, 0.0f, 0.0f, 5.0f);
            }
        }

        public lightList getActiveLight()
        {
            if (lights.Count > 0)
            {
                return lights[activeItemNumber];
            }
            else
            {
                return new lightList();
            }
        }

        public void deleteActiveItem()
        {
            if (typeText == " R: ")
            {
                if (controlpoints.Count > 4 || activeItemNumber > controlpoints.Count)
                {
                    controlpoints.RemoveAt(activeItemNumber);
                    setActiveItem(activeItemNumber - 1);
                }
            }
            else
            {
                if (controlpoints.Count > 0 || activeItemNumber > controlpoints.Count)
                {
                    controlpoints.RemoveAt(activeItemNumber);
                    setActiveItem(activeItemNumber - 1);
                }
            }
        }

        public void deleteActiveLight()
        {
            if (lights.Count > 4 || activeItemNumber > lights.Count)
            {
                lights.RemoveAt(activeItemNumber);
                setActiveLight(activeItemNumber - 1);
            }
        }
        public void insertNewItem()
        {
            if (controlpoints.Count == 0)
            {
                controlpoints.Add(new Vector4(0.0f, 0.0f, 0.0f, 1.0f));
            } 
            else if (activeItemNumber == controlpoints.Count - 1)
            {
                controlpoints.Insert(activeItemNumber, controlpoints[activeItemNumber]);
            }
            else if (activeItemNumber == 0 && controlpoints.Count > 0)
            {
                controlpoints.Insert(activeItemNumber + 1, controlpoints[activeItemNumber]);
            }
            else if (controlpoints.Count >= 1)
            {
                controlpoints.Insert(activeItemNumber + 1, controlpoints[activeItemNumber]);
            }
           

        }

        public void insertNewLight()
        {
            if (lights.Count == 0)
            {
                lights.Add(new lightList());
            }
            else if (activeItemNumber == lights.Count - 1)
            {
                lights.Insert(activeItemNumber, lights[activeItemNumber]);
            }
            else if (activeItemNumber == 0 && lights.Count > 0)
            {
                lights.Insert(activeItemNumber + 1, lights[activeItemNumber]);
            }
            else if (controlpoints.Count >= 1)
            {
                lights.Insert(activeItemNumber + 1, lights[activeItemNumber]);
            }
        }

        public void moveActtiveItemUp()
        {
            if (activeItemNumber < controlpoints.Count - 2 && activeItemNumber != 0)
            {
                Vector4 tmp = controlpoints[activeItemNumber + 1];
                controlpoints[activeItemNumber + 1] = controlpoints[activeItemNumber];
                controlpoints[activeItemNumber] = tmp;
                setActiveItem(activeItemNumber + 1);
            }
        }

        public void moveActiveItemDown()
        {
            if (activeItemNumber >= 2 && activeItemNumber != controlpoints.Count - 1)
            {
                Vector4 tmp = controlpoints[activeItemNumber - 1];
                controlpoints[activeItemNumber - 1] = controlpoints[activeItemNumber];
                controlpoints[activeItemNumber] = tmp;
                setActiveItem(activeItemNumber - 1);
            }
        }

        public void updateActiveItem(Vector4 item)
        {
            if (item != null && typeText == " R: " && controlpoints.Count > 0)
            {
                if (activeItemNumber == 0)
                {
                    controlpoints[activeItemNumber] = item;
                    controlpoints[controlpoints.Count - 1] = item;
                }
                else if (activeItemNumber == controlpoints.Count - 1)
                {
                    controlpoints[0] = item;
                    controlpoints[activeItemNumber] = item;
                }
                else
                {
                    controlpoints[activeItemNumber] = item;
                }
            }
            else if (item != null && controlpoints.Count > 0)
            {
                controlpoints[activeItemNumber] = item;
            }
        }

        public void updateActiveLight(lightList light)
        {
            if (lights.Count > 0)
            {
                lights[activeItemNumber] = light;
            }
        }

        public bool isSelected()
        {
            return state;
        }

        public void setState(bool state)
        {
            this.state = state;
        }

    }
}