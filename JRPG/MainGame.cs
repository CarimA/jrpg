using JRPG.EntityComponent;
using JRPG.EntityComponent.Components;
using JRPG.Scripting;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.BitmapFonts;
using System.Collections.Generic;

namespace JRPG
{
    public class MainGame : Game
    {
        public const int TILE_SIZE = 16;

        public static BitmapFont Font;

        public GraphicsDeviceManager Graphics;
        public SpriteBatch SpriteBatch { get => spriteBatch; }
        SpriteBatch spriteBatch;
        public Camera Camera { get => camera; }
        Camera camera;
        public MapManager MapManager { get => mapManager; }
        MapManager mapManager;
        public Entity Player { get => player; }
        Entity player;

        EntityManager entityManager;

        public ScriptingManager ScriptingManager { get => scriptingManager; }
        ScriptingManager scriptingManager;

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

            player = new Entity(entityManager, "player");
            player.AddComponents(new List<Component>()
            {
                new InputComponent(),
                new PositionComponent(),
                new TextureComponent("Debug/player"),
                new PlayerComponent()
            });
            entityManager.AddEntity(player);

            camera = new Camera(this);
            camera.SetTarget(player);
            mapManager = new MapManager(this, player);

            scriptingManager = new ScriptingManager(this);

            //Locator.Scripting.Execute(Locator.Data.Scripts["testscript"]);

            base.Initialize();


            //scriptingManager.Execute("var t = wait(5); log(t);");

        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            Font = Content.Load<BitmapFont>("Fonts/pixellari");
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
