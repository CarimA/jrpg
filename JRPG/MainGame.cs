using JRPG.EntityComponent;
using JRPG.ServiceLocator;
using JRPG.ServiceLocator.Services.Data;
using JRPG.ServiceLocator.Services.Graphics;
using JRPG.ServiceLocator.Services.Scripting;
using JRPG.ServiceLocator.Services.Text;
using JRPG.ServiceLocator.Services.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace JRPG
{
    public class MainGame : Game
    {
        public const int TILE_SIZE = 16;

        public GraphicsDeviceManager Graphics;
        public SpriteBatch SpriteBatch { get => spriteBatch; }
        SpriteBatch spriteBatch;
        public Camera Camera { get => camera; }
        Camera camera;
        public MapManager MapManager { get => mapManager; }
        MapManager mapManager;

        EntityManager entityManager;

        public MainGame()
        {
            Graphics = new GraphicsDeviceManager(this);

            Window.AllowUserResizing = true;
            IsMouseVisible = true;

            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            Locator.ProvideGameInstance(this);

            entityManager = new EntityManager(this);
            Locator.ProvideEC(entityManager);

            var player = Locator.Entity.Player(1);
            entityManager.AddEntity(player);

            camera = new Camera(this);
            camera.SetTarget(player);
            mapManager = new MapManager(this, player);

            Locator.ProvideGraphics(new DebugGraphics());
            Locator.ProvideUtility(new DebugUtility());

            Locator.ProvideData(new PrototypeData());
            Locator.ProvideText(new EnglishText());

            // this must be last
            Locator.ProvideScripting(new Scripting());
            Locator.Scripting.Execute(Locator.Data.Scripts["testscript"]);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
        }
        
        protected override void UnloadContent()
        {

        }
        
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            Locator.Graphics.Update(gameTime);

            base.Update(gameTime);
        }
        
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);         

            base.Draw(gameTime);
        }
    }
}
