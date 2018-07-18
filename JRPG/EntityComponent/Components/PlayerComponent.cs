using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace JRPG.EntityComponent.Components
{
    public class PlayerComponent : DrawableComponent
    {
        public float WalkSpeed = 75f;
        public float RunSpeed = 170f;
        public bool RunToggled = false;

        public override void Update(GameTime gameTime)
        {
            InputComponent input = this.GetComponent<InputComponent>();
            PositionComponent position = this.GetComponent<PositionComponent>();

            // set fullscreen
            if (input.ButtonPressed("fullscreen"))
            {
                Game.Graphics.IsFullScreen = !Game.Graphics.IsFullScreen;
                Game.Graphics.PreferredBackBufferHeight = Game.GraphicsDevice.Adapter.CurrentDisplayMode.Height;
                Game.Graphics.PreferredBackBufferWidth = Game.GraphicsDevice.Adapter.CurrentDisplayMode.Width;
                Game.Graphics.ApplyChanges();
            }

            if (input.ButtonPressed("run"))
            {
                RunToggled = !RunToggled;
            }

            if (input.ButtonDown("up") || input.ButtonDown("up-alt"))
            {
                position.Move(PositionComponent.Direction.Up, RunToggled ? 6.5f : 2.25f);
            }
            if (input.ButtonDown("down") || input.ButtonDown("down-alt"))
            {
                position.Move(PositionComponent.Direction.Down, RunToggled ? 6.5f : 2.25f);
            }
            if (input.ButtonDown("left") || input.ButtonDown("left-alt"))
            {
                position.Move(PositionComponent.Direction.Left, RunToggled ? 6.5f : 2.25f);
            }
            if (input.ButtonDown("right") || input.ButtonDown("right-alt"))
            {
                position.Move(PositionComponent.Direction.Right, RunToggled ? 6.5f : 2.25f);
            }
        }

        public override void DrawFringe(GameTime gameTime)
        {

        }

        public override void DrawMask(GameTime gameTime)
        {
            PositionComponent position = this.GetComponent<PositionComponent>();
            TextureComponent texture = GetComponent<TextureComponent>();
            Game.SpriteBatch.Draw(texture.Texture, position.GetPosition() - new Vector2(MainGame.TILE_SIZE / 2, (MainGame.TILE_SIZE / 2) + 16), Color.White);
        }

        public override void DrawUI(GameTime gameTime)
        {

        }
    }
}
