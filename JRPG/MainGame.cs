﻿using JRPG.Data;
using JRPG.EntityComponent;
using JRPG.EntityComponent.Components;
using JRPG.GameComponents;
using JRPG.Scripting;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.TextureAtlases;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections;

namespace JRPG
{
    public class MainGame : Game
    {
        // todo: tidy up this ugly god object mess of a class
        public VisualConsole Console;

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
        public TransitionManager Transition { private set; get; }
        public TextInput KeyInput { private set; get; }
        public AudioManager Audio { private set; get; }
        public InputManager Input { private set; get; }

        EntityManager entityManager;

        public ScriptingManager ScriptingManager { private set; get; }

        public DataTree EnglishText;

        Effect effect;
        Texture2D palette;
        
        public MainGame()
        {
            Console = new VisualConsole(this);
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

        public enum ClipboardContents
        {
            URL = 0,
            Image
        }

        public async void SaveScreenshot(ClipboardContents clipboard = ClipboardContents.URL, bool openURL = false)
        {
            int shotNumber = 0;
            Directory.CreateDirectory("Screenshots");
            string path = Path.Combine(Application.StartupPath, "Screenshots", "photovs_screenshot_" + shotNumber.ToString().PadLeft(3, '0') + ".png");
            while (File.Exists(path))
            {
                shotNumber++;
                path = Path.Combine(Application.StartupPath, "Screenshots", "photovs_screenshot_" + shotNumber.ToString().PadLeft(3, '0') + ".png");
            }

            using (FileStream fs = new FileStream(path, FileMode.Create))
            {
                Console.WriteLine("Copied to clipboard and saved screenshot to ", path);
                renderTarget2.SaveAsPng(fs, GAME_WIDTH, GAME_HEIGHT);
            }

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

                            if (clipboard == ClipboardContents.Image)
                            {
                                Clipboard.SetImage(System.Drawing.Image.FromFile(path));
                            }
                            else
                            {
                                Clipboard.SetText(val);
                            }

                            Console.WriteLine($"Screenshot uploaded to {val}");

                            // send to discord
                            using (WebClient client = new WebClient())
                            {
                                HttpResponseMessage resp = await new HttpClient().PostAsync("https://discordapp.com/api/webhooks/482641656361910272/ZKZfPujN8SfUBznuwqLUu_HJ2o-58ws_r5Whd3bcOalT2woGmjMTYAbwK7zuqFXY0rIl",
                                    new StringContent("{\"embeds\":[{\"image\":{\"url\":\"" + val + "\"}}]}", Encoding.UTF8, "application/json"));
                                //Console.WriteLine(resp.ToString());
                                Console.WriteLine("Screenshot posted in PhotoVs Discord");
                            }

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

        public void ToggleFullscreen()
        {
            Graphics.IsFullScreen = !Graphics.IsFullScreen;
            Graphics.PreferredBackBufferHeight = GraphicsDevice.Adapter.CurrentDisplayMode.Height;
            Graphics.PreferredBackBufferWidth = GraphicsDevice.Adapter.CurrentDisplayMode.Width;
            Graphics.ApplyChanges();
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
            Console.Initialise();
            CoroutineManager.Initialise();

            Graphics.PreferredBackBufferWidth = GAME_WIDTH * 3;
            Graphics.PreferredBackBufferHeight = GAME_HEIGHT * 3;
            Graphics.ApplyChanges();
            UpdateScaleViewport();

            entityManager = new EntityManager(this);
            Audio = new AudioManager(this);
            Input = new InputManager(this);

            KeyInput = new TextInput(this);

            Player = new Entity(entityManager, "player");
            Player.AddComponents(new List<Component>()
            {
                new InputTransformComponent(),
                new PositionComponent(),
                new PlayerComponent(),
                new CollisionComponent(new Resolve.RectangleF(-6, -20, 12, 24))
            });

            Camera = new Camera(this);
            Camera.SetTarget(Player);
            MapManager = new MapManager(this, Player);

            ScriptingManager = new ScriptingManager(this);
            Transition = new TransitionManager(this);

            renderTarget1 = new RenderTarget2D(this.GraphicsDevice, GAME_WIDTH, GAME_HEIGHT);
            renderTarget2 = new RenderTarget2D(this.GraphicsDevice, GAME_WIDTH, GAME_HEIGHT);

            //IEnumerator t = Test();
            //CoroutineManager.StartCoroutine(t);

            base.Initialize();
        }

        float time = 1f;
        IEnumerator Test()
        {
            while(true)
            {
                time -= CoroutineManager.DeltaTime;
                if (time <= 0f)
                {
                    time = 1f;
                    Console.WriteLine("tick");
                }
                yield return null;
            }
        }

        protected override void LoadContent()
        {
            SpriteBatch = new SpriteBatch(GraphicsDevice);

            effect = Assets.Get<Effect>(AssetManager.Asset.ShadersPalette); // Content.Load<Effect>("palette");
            palette = Assets.Get<Texture2D>(AssetManager.Asset.InterfacesColourTable); // Content.Load<Texture2D>("interfaces/proj3");
        }
        
        protected override void UnloadContent()
        {

        }



        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            Console.Update();
            CoroutineManager.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
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
            Console.Draw();

            SpriteBatch.End();

        }
    }

    public static class SpriteBatchExtensions
    {
        /*public static Random rand = new Random();
        public static void DrawString(this SpriteBatch spriteBatch, BitmapFont bitmapFont, string text, Vector2 position)
        {
            if (text == null)
                throw new ArgumentNullException(nameof(text));

            if (text.Contains("\n"))
            {
                string[] split = text.Split('\n');
                Vector2 measure = Vector2.Zero;
                foreach (string s in split)
                {
                    spriteBatch.DrawString(bitmapFont, s, position + measure);                    
                    measure.Y += bitmapFont.MeasureString(s).Height;
                }
                return;
            }

            if (!text.Contains("{"))
            {
                spriteBatch.DrawString(bitmapFont, text, position, Color.White);
                return;
            }

            Vector2 offset = Vector2.Zero;

            bool shake = false;

            string[] tokens = text.Split('{');

            foreach (string token in tokens)
            {
                string[] arg = token.Split('}');
                int index = Math.Min(1, arg.Length - 1);

                switch (arg[0])
                {
                    case "shake":
                        shake = true;
                        break;
                    case "/shake":
                        shake = false;
                        break;
                }

                Vector2 shakeAmount = new Vector2(rand.Next(-1, 1), rand.Next(-1, 1));

                spriteBatch.DrawString(bitmapFont, arg[index], position + offset + (shake ? shakeAmount : Vector2.Zero), Color.White);
                offset.X += bitmapFont.MeasureString(arg[index]).Width;
            }
        }*/

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
