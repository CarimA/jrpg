using JRPG.Data;
using JRPG.EntityComponent;
using JRPG.EntityComponent.Components;
using JRPG.Scripting;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.BitmapFonts;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;

namespace JRPG
{
    public class MainGame : Game
    {
        public AssetManager Assets;

        public const int GAME_WIDTH = 384;
        public const int GAME_HEIGHT = 216;

        private RenderTarget2D renderTarget1;
        private RenderTarget2D renderTarget2;
        Rectangle dest = new Rectangle();

        public const int TILE_SIZE = 16;

        public GraphicsDeviceManager Graphics;
        public SpriteBatch SpriteBatch { private set; get; }
        public Camera Camera { private set; get; }
        public MapManager MapManager { private set; get; }
        public Entity Player { private set; get; }

        EntityManager entityManager;

        public ScriptingManager ScriptingManager { private set; get; }

        public DataTree EnglishText;

        Effect effect;
        Texture2D palette;
        Texture2D pixel;
        
        public MainGame()
        {
            Graphics = new GraphicsDeviceManager(this);

            Content.RootDirectory = "Content";
            Assets = AssetManager.LoadAll(this, Content);

            EnglishText = DataTree.Load("content/text/english.json");
            Window.Title = EnglishText.Get("Main").Get("GameName").GetString();

            Window.AllowUserResizing = true;
            Window.ClientSizeChanged += (object sender, EventArgs e) =>
            {
                UpdateScaleViewport();
            };

            IsMouseVisible = true;

        }

        public void SaveScreenshot(bool openURL = true)
        {
            using (MemoryStream mem = new MemoryStream())
            {
                // save game screenshot to memory
                renderTarget2.SaveAsPng(mem, GAME_WIDTH, GAME_HEIGHT);

                using (var w = new WebClient())
                {
                    // upload to Imgur
                    // oh yeah, you all can see this too. oh well.
                    string clientID = "36e2260c96b7e67";
                    w.Headers.Add("Authorization", "Client-ID " + clientID);
                    var values = new NameValueCollection
                        {
                            { "image", Convert.ToBase64String(mem.GetBuffer()) }
                        };

                    byte[] response = w.UploadValues("https://api.imgur.com/3/upload.xml", values);

                    // parse through XML and get a link back
                    using (MemoryStream res = new MemoryStream(response))
                    {
                        var xml = XDocument.Load(res);
                        var data = xml.Descendants("data");

                        foreach (var d in data)
                        {
                            var val = d.Element("link").Value;
                            Clipboard.SetText(val);
                            Console.WriteLine($"Screenshot uploaded to {val}");

                            if (openURL)
                            {
                                try
                                {
                                    Process.Start(val);
                                }
                                catch (System.ComponentModel.Win32Exception)
                                {
                                    Process.Start("IExplore.exe", val);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void UpdateScaleViewport()
        {
            float aspectRatio = (float)GAME_WIDTH / (float)GAME_HEIGHT;
            Graphics.PreferredBackBufferWidth = Window.ClientBounds.Width;
            Graphics.PreferredBackBufferHeight = Window.ClientBounds.Height;
            Graphics.ApplyChanges();

            int width = Graphics.PreferredBackBufferWidth;
            int height = (int)(width / aspectRatio + 0.5f);
            if (height > Graphics.PreferredBackBufferHeight)
            {
                height = Graphics.PreferredBackBufferHeight;
                width = (int)(height * aspectRatio + 0.5f);
            }
            
            dest.X = (Graphics.PreferredBackBufferWidth / 2) - (width / 2);
            dest.Y = (Graphics.PreferredBackBufferHeight / 2) - (height / 2);
            dest.Width = width;
            dest.Height = height;
           
            //float vwidth = GraphicsDevice.Viewport.Width / GAME_WIDTH;
            //float vheight = GraphicsDevice.Viewport.Height / GAME_HEIGHT;
            //scale = (float)Math.Floor(Math.Min(vwidth, vheight));
        }

        protected override void Initialize()
        {
            Graphics.PreferredBackBufferWidth = GAME_WIDTH * 3;
            Graphics.PreferredBackBufferHeight = GAME_HEIGHT * 3;
            Graphics.ApplyChanges();
            UpdateScaleViewport();

            entityManager = new EntityManager(this);

            Player = new Entity(entityManager, "player");
            Player.AddComponents(new List<Component>()
            {
                new InputComponent(),
                new PositionComponent(),
                new TextureComponent("Debug/player"),
                new PlayerComponent()
            });
            entityManager.AddEntity(Player);

            Camera = new Camera(this);
            Camera.SetTarget(Player);
            MapManager = new MapManager(this, Player);

            ScriptingManager = new ScriptingManager(this);

            renderTarget1 = new RenderTarget2D(this.GraphicsDevice, GAME_WIDTH, GAME_HEIGHT);
            renderTarget2 = new RenderTarget2D(this.GraphicsDevice, GAME_WIDTH, GAME_HEIGHT);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            SpriteBatch = new SpriteBatch(GraphicsDevice);

            effect = Assets.Get<Effect>(AssetManager.Asset.ShadersPalette); // Content.Load<Effect>("palette");
            palette = Assets.Get<Texture2D>(AssetManager.Asset.InterfacesColourTable); // Content.Load<Texture2D>("interfaces/proj3");

            pixel = new Texture2D(GraphicsDevice, 1, 1);
            pixel.SetData<Color>(new Color[1] { Color.White });
        }
        
        protected override void UnloadContent()
        {

        }



        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        float opac = 0f;
        bool flipflop = false;
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.SetRenderTarget(renderTarget1);

            GraphicsDevice.Clear(Color.CornflowerBlue);
            base.Draw(gameTime);

            /*if (flipflop)
            {
                opac -= ((float)gameTime.ElapsedGameTime.TotalSeconds);
                if (opac <= 0f)
                {
                    flipflop = !flipflop;
                }
            }
            else
            {
                opac += ((float)gameTime.ElapsedGameTime.TotalSeconds);
                if (opac >= 1f)
                {
                    flipflop = !flipflop;
                }
            }*/
           
            SpriteBatch.Begin();
            SpriteBatch.Draw(pixel, new Rectangle(0, 0, GAME_WIDTH, GAME_HEIGHT), Color.DarkSlateGray * opac);
            SpriteBatch.End();


            GraphicsDevice.SetRenderTarget(renderTarget2);

            GraphicsDevice.Clear(Color.Black);

            SpriteBatch.Begin(SpriteSortMode.Immediate);
            effect.CurrentTechnique.Passes[0].Apply();
            effect.Parameters["palette"].SetValue(palette);
            effect.Parameters["tex_width"].SetValue((float)palette.Width);
            effect.Parameters["tex_height"].SetValue((float)palette.Height);

            SpriteBatch.Draw((Texture2D)renderTarget1, Vector2.Zero, Color.White);

            SpriteBatch.End();

            

            GraphicsDevice.SetRenderTarget(null);

            SpriteBatch.Begin(samplerState: SamplerState.PointWrap);

            SpriteBatch.Draw((Texture2D)renderTarget2, dest, Color.White);

            SpriteBatch.End();

        }
    }

    public static class SpriteBatchExtensions
    {
        public static void DrawNineSlice(this SpriteBatch spriteBatch, Texture2D texture, Color color, Rectangle destination, Rectangle? source = null)
        {
            // figure out size of chunks
            int sourceLeft = 0;
            int sourceTop = 0;
            int chunkWidth = 0;
            int chunkHeight = 0;

            if (!source.HasValue)
            {
                chunkWidth = texture.Width / 3;
                chunkHeight = texture.Height / 3;
            }
            else
            {
                sourceTop = source.Value.Top;
                sourceLeft = source.Value.Left;
                chunkWidth = source.Value.Width / 3;
                chunkHeight = source.Value.Height / 3;
            }

            // corners
            spriteBatch.Draw(texture, new Rectangle(destination.Left, destination.Top, chunkWidth, chunkHeight), new Rectangle(sourceLeft, sourceTop, chunkWidth, chunkHeight), color);
            spriteBatch.Draw(texture, new Rectangle(destination.Right - chunkWidth, destination.Top, chunkWidth, chunkHeight), new Rectangle(sourceLeft + (chunkWidth * 2), sourceTop, chunkWidth, chunkHeight), color);
            spriteBatch.Draw(texture, new Rectangle(destination.Left, destination.Bottom - chunkHeight, chunkWidth, chunkHeight), new Rectangle(sourceLeft, sourceTop + (chunkHeight * 2), chunkWidth, chunkHeight), color);
            spriteBatch.Draw(texture, new Rectangle(destination.Right - chunkWidth, destination.Bottom - chunkHeight, chunkWidth, chunkHeight), new Rectangle(sourceLeft + (chunkWidth * 2), sourceTop + (chunkHeight * 2), chunkWidth, chunkHeight), color);

            // edges
            spriteBatch.Draw(texture, new Rectangle(destination.Left + chunkWidth, destination.Top, destination.Width - (chunkWidth * 2), chunkHeight), new Rectangle(sourceLeft + chunkWidth, sourceTop, chunkWidth, chunkHeight), color);
            spriteBatch.Draw(texture, new Rectangle(destination.Left + chunkWidth, destination.Bottom - chunkHeight, destination.Width - (chunkWidth * 2), chunkHeight), new Rectangle(sourceLeft + chunkWidth, sourceTop + (chunkHeight * 2), chunkWidth, chunkHeight), color);

            spriteBatch.Draw(texture, new Rectangle(destination.Left, destination.Top + chunkHeight, chunkWidth, destination.Height - (chunkHeight * 2)), new Rectangle(sourceLeft, sourceTop + chunkHeight, chunkWidth, chunkHeight), color);
            spriteBatch.Draw(texture, new Rectangle(destination.Right - chunkWidth, destination.Top + chunkHeight, chunkWidth, destination.Height - (chunkHeight * 2)), new Rectangle(sourceLeft + (chunkWidth * 2), sourceTop + chunkHeight, chunkWidth, chunkHeight), color);

            // middle
            spriteBatch.Draw(texture, new Rectangle(destination.Left + chunkWidth, destination.Top + chunkHeight, destination.Width - (chunkWidth * 2), destination.Height - (chunkHeight * 2)), new Rectangle(sourceLeft + chunkWidth, sourceTop + chunkHeight, chunkWidth, chunkHeight), color);
        }
    }
}

/* todo tasks
 *      asset manager
 *      sound/music play
 *      state machine
 *          state transition effects
 *          loading support
 *          screens
 *              title screen
 *              main menu
 *                  create/use/delete save
 *                  options
 *              game
 *                  
 *      
 */