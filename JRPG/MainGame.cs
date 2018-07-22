using JRPG.EntityComponent;
using JRPG.EntityComponent.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

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

            entityManager = new EntityManager(this);

            Entity e = new Entity(entityManager, "player");
            e.AddComponents(new List<Component>()
            {
                new InputComponent(),
                new PositionComponent(),
                new TextureComponent("Debug/player"),
                new PlayerComponent()
            });
            entityManager.AddEntity(e);

            camera = new Camera(this);
            camera.SetTarget(e);
            mapManager = new MapManager(this, e);
            
            //Locator.Scripting.Execute(Locator.Data.Scripts["testscript"]);

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

            base.Update(gameTime);
        }
        
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);         

            base.Draw(gameTime);
        }
    }
}
