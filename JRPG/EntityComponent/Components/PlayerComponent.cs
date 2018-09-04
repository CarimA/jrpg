using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.BitmapFonts;
using static JRPG.EntityComponent.Components.PositionComponent;

namespace JRPG.EntityComponent.Components
{
    public class PlayerComponent : DrawableComponent
    {
        public float WalkSpeed = 125f;
        public float RunSpeed = 270f;
        public bool RunToggled = false;
        public bool InControl = true;

        public override void Update(GameTime gameTime)
        {
            PositionComponent position = this.GetComponent<PositionComponent>();

            // set fullscreen
            if (Game.Input.ButtonPressed("fullscreen"))
            {
                Game.Graphics.IsFullScreen = !Game.Graphics.IsFullScreen;
                Game.Graphics.PreferredBackBufferHeight = Game.GraphicsDevice.Adapter.CurrentDisplayMode.Height;
                Game.Graphics.PreferredBackBufferWidth = Game.GraphicsDevice.Adapter.CurrentDisplayMode.Width;
                Game.Graphics.ApplyChanges();
            }

            if (Game.Input.ButtonPressed("debug"))
            {
                Game.Console.ToggleVisible();
            }

            if (Game.Input.ButtonPressed("screenshot"))
            {
                Game.SaveScreenshot();
            }

            if (Game.Input.ButtonPressed("up"))
            {
                Game.Console.MoveCursorUp();
            }
            if (Game.Input.ButtonPressed("down"))
            {
                Game.Console.MoveCursorDown();
            }

            if (InControl)
            {
                if (Game.Input.ButtonPressed("run"))
                {
                    RunToggled = !RunToggled;
                }

                float runSpeed = RunToggled ? 65f : 120f;
                Vector2 walkDir = Vector2.Zero;
                
                if (Game.Input.ButtonDown("up"))
                {
                    walkDir.Y--;
                }
                if (Game.Input.ButtonDown("down"))
                {
                    walkDir.Y++;
                }
                if (Game.Input.ButtonDown("left"))
                {
                    walkDir.X--;
                }
                if (Game.Input.ButtonDown("right"))
                {
                    walkDir.X++;
                }

                if (walkDir != Vector2.Zero)
                {
                    walkDir.Normalize();
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
                    if (Game.Input.ButtonPressed("action"))
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
            Game.SpriteBatch.Draw(Game.Assets.Get<Texture2D>(AssetManager.Asset.DebugPlayer), position.Position - new Vector2((MainGame.TILE_SIZE / 2) + 8, (MainGame.TILE_SIZE / 2) + 16 + 8 + 6), Color.White);
        }

        public override void DrawUI(GameTime gameTime)
        {

        }


        public override void Receive(MessageType message, Entity entity, Component sender)
        {
            throw new NotImplementedException();
        }
    }
}
