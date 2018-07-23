using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using MonoGame.Extended.BitmapFonts;
using static JRPG.EntityComponent.Components.PositionComponent;

namespace JRPG.EntityComponent.Components
{
    public class PlayerComponent : DrawableComponent
    {
        public float WalkSpeed = 75f;
        public float RunSpeed = 170f;
        public bool RunToggled = false;
        public bool InControl = true;

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

            if (InControl)
            {
                if (input.ButtonPressed("action"))
                {
                    Direction direction = position.FacingDirection;
                    switch (direction)
                    {
                        case Direction.Right:
                            Game.MapManager.CurrentMap.Interact(position.TileX + 1, position.TileY);
                            break;
                        case Direction.Left:
                            Game.MapManager.CurrentMap.Interact(position.TileX - 1, position.TileY);
                            break;
                        case Direction.Up:
                            Game.MapManager.CurrentMap.Interact(position.TileX, position.TileY - 1);
                            break;
                        case Direction.Down:
                            Game.MapManager.CurrentMap.Interact(position.TileX, position.TileY + 1);
                            break;
                    }
                }

                if (input.ButtonPressed("run"))
                {
                    RunToggled = !RunToggled;
                }

                float runSpeed = RunToggled ? 6.5f : 2.25f;
                Vector2 walkDir = Vector2.Zero;

                if (input.ButtonDown("up") || input.ButtonDown("up-alt"))
                {
                    walkDir.Y--;
                }
                if (input.ButtonDown("down") || input.ButtonDown("down-alt"))
                {
                    walkDir.Y++;
                }
                if (input.ButtonDown("left") || input.ButtonDown("left-alt"))
                {
                    walkDir.X--;
                }
                if (input.ButtonDown("right") || input.ButtonDown("right-alt"))
                {
                    walkDir.X++;
                }

                if (walkDir.X < 0)
                {
                    position.Move(PositionComponent.Direction.Left, runSpeed);
                }
                else if (walkDir.X > 0)
                {
                    position.Move(PositionComponent.Direction.Right, runSpeed);
                }
                else if (walkDir.Y < 0)
                {
                    position.Move(PositionComponent.Direction.Up, runSpeed);
                }
                else if (walkDir.Y > 0)
                {
                    position.Move(PositionComponent.Direction.Down, runSpeed);
                }
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
