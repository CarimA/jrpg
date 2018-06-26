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
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        EntityManager entityManager;

        public MainGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            base.Initialize();

            entityManager = new EntityManager(this);
            Locator.ProvideEC(entityManager);
            entityManager.AddEntity(Locator.Entity.Player(1));


            Locator.ProvideGameInstance(this);

            Locator.ProvideGraphics(new DebugGraphics());
            Locator.ProvideUtility(new DebugUtility());


            Locator.ProvideData(new PrototypeData());
            Locator.ProvideText(new EnglishText());

            // this must be last
            Locator.ProvideScripting(new Scripting());


            Locator.Scripting.Execute(Locator.Data.Scripts["titlescreen.txt"]);
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
            GraphicsDevice.Clear(Color.CornflowerBlue);         

            base.Draw(gameTime);
        }
    }
}
