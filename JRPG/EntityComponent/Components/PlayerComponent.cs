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
        public Vector2 MovementDirection;
        public float CurrentSpeed() => RunToggled ? WalkSpeed : RunSpeed;
        public float WalkSpeed = 70f;
        public float RunSpeed = 140f;
        public bool RunToggled = false;
        public bool InControl = false;
        
        public PlayerComponent()
        {
        }

        public override void Initialize()
        {
            ToggleControl();

            Subscribe("fullscreen-pressed", (Entity e, Component c) =>
            {
                Game.ToggleFullscreen();
            });

            Subscribe("debug-pressed", (Entity e, Component c) =>
            {
                Game.Console.ToggleVisible();
            });

            Subscribe("screenshot-pressed", (Entity e, Component c) =>
            {
                Game.SaveScreenshot();
            });

            Subscribe("pgup-pressed", (Entity e, Component c) =>
            {
                Game.Console.MoveCursorUp();
            });

            Subscribe("pgdown-pressed", (Entity e, Component c) =>
            {
                Game.Console.MoveCursorDown();
            });

            Subscribe("input-state", HandleInputState);
            Subscribe("run-pressed", ToggleRun);
        }

        public void ToggleControl() => InControl = !InControl;
        public void ToggleRun(Entity entity, Component component)
        {
            if (InControl)
            { 
                RunToggled = !RunToggled;
            }
        }

        public void HandleInputState(Entity entity, Component component)
        {
            MovementDirection = (component as InputTransformComponent).LeftThumbstick;
            Send("move");
        }


        public override void Update(GameTime gameTime)
        {
            if (InControl)
            {
                //if (position.MovementDirection == Direction.None)
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
    }
}
