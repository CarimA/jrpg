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

            if (input.ButtonPressed("debug"))
            {
                Game.Console.ToggleVisible();
            }

            if (input.ButtonPressed("screenshot"))
            {
                Game.SaveScreenshot();
            }

            if (input.ButtonPressed("up"))
            {
                Game.Console.MoveCursorUp();
            }
            if (input.ButtonPressed("down"))
            {
                Game.Console.MoveCursorDown();
            }

            if (InControl)
            {
                if (input.ButtonPressed("run"))
                {
                    RunToggled = !RunToggled;
                }

                float runSpeed = RunToggled ? 65f : 120f;
                Vector2 walkDir = Vector2.Zero;
                
                if (input.ButtonDown("up"))
                {
                    walkDir.Y--;
                }
                if (input.ButtonDown("down"))
                {
                    walkDir.Y++;
                }
                if (input.ButtonDown("left"))
                {
                    walkDir.X--;
                }
                if (input.ButtonDown("right"))
                {
                    walkDir.X++;
                }

                position.Move(walkDir, runSpeed);

                /*if (walkDir.X < 0)
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
                }*/
                
                if (position.MovementDirection == Direction.None)
                {
                    if (input.ButtonPressed("action"))
                    {
                        throw new NotImplementedException();
                        /*Direction direction = position.FacingDirection;
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
                        }*/
                    }
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
            Game.SpriteBatch.Draw(texture.Texture, position.Position - new Vector2((MainGame.TILE_SIZE / 2) + 8, (MainGame.TILE_SIZE / 2) + 16 + 8 + 6), Color.White);
        }

        public override void DrawUI(GameTime gameTime)
        {

        }
    }
}
