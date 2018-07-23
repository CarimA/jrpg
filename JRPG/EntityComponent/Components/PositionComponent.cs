using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JRPG.EntityComponent.Components
{
    public class PositionComponent : Component
    {
        public enum Direction
        {
            None,
            Up,
            Down,
            Left,
            Right
        }

        public enum MovementFailureReason
        {
            None,
            Collision,
            CurrentlyMoving
        }

        public int TileX { get; set; }
        public int TileY { get; set; }
        public float SubTileX { get; set; } // ranges from 0 to 1 or 0 to -1, updates over time
        public float SubTileY { get; set; }
        public Direction MovementDirection { get; set; }
        public Direction FacingDirection { get; set; }

        public float MoveSpeed;

        public Vector2 GetPosition()
        {
            return new Vector2(
                (TileX * MainGame.TILE_SIZE) + (SubTileX * MainGame.TILE_SIZE) + (MainGame.TILE_SIZE / 2),
                (TileY * MainGame.TILE_SIZE) + (SubTileY * MainGame.TILE_SIZE) + (MainGame.TILE_SIZE / 2)
            );
        }

        public void Set(int x, int y)
        {
            TileX = x;
            TileY = y;
            SubTileX = 0;
            SubTileY = 0;
        }

        public MovementFailureReason Move(Direction direction, float moveSpeed = 2.25f)
        {
            // move somewhere, return if we can't move yet or if we collide with something
            if (MovementDirection != Direction.None)
            {
                return MovementFailureReason.CurrentlyMoving;
            }

            FacingDirection = direction;

            // todo: rewrite in a way that's not dumb
            switch (direction)
            {
                case Direction.Right:
                    if (Game.MapManager.CurrentMap.SolidAt(TileX + 1, TileY)) return MovementFailureReason.Collision;
                    break;
                case Direction.Left:
                    if (Game.MapManager.CurrentMap.SolidAt(TileX - 1, TileY)) return MovementFailureReason.Collision;
                    break;
                case Direction.Up:
                    if (Game.MapManager.CurrentMap.SolidAt(TileX, TileY - 1)) return MovementFailureReason.Collision;
                    break;
                case Direction.Down:
                    if (Game.MapManager.CurrentMap.SolidAt(TileX, TileY + 1)) return MovementFailureReason.Collision;
                    break;
            }
            
            MoveSpeed = moveSpeed;
            MovementDirection = direction;

            switch (MovementDirection)
            {
                case Direction.Right:
                    TileX++;
                    SubTileX = -1;
                    break;
                case Direction.Left:
                    TileX--;
                    SubTileX = 1;
                    break;
                case Direction.Down:
                    TileY++;
                    SubTileY = -1;
                    break;
                case Direction.Up:
                    TileY--;
                    SubTileY = 1;
                    break;
            }

            if (Game.MapManager.Player == GetOwner)
            {
                if (TileY < 0)
                {
                    Game.MapManager.Move(Direction.Up);
                }
                if (TileX < 0)
                {
                    Game.MapManager.Move(Direction.Left);
                }
                if (TileY >= Game.MapManager.CurrentMap.MapHeight)
                {
                    Game.MapManager.Move(Direction.Down);
                }
                if (TileX >= Game.MapManager.CurrentMap.MapWidth)
                {
                    Game.MapManager.Move(Direction.Right);
                }
                Game.Window.Title = $"{Game.MapManager.CurrentMap.ID} - {{{TileX}, {TileY}}}";
            }

            return MovementFailureReason.None;
        }
        
        public override void Update(GameTime gameTime)
        {
            if (MovementDirection == Direction.None)
            {
                // just...wait?
            }
            else
            {
                switch (MovementDirection)
                {
                    case Direction.Right:
                        SubTileX += (float)gameTime.ElapsedGameTime.TotalSeconds * MoveSpeed;
                        if (SubTileX > 0)
                        {
                            SubTileX = 0;
                            MovementDirection = Direction.None;
                        }
                        break;
                    case Direction.Left:
                        SubTileX -= (float)gameTime.ElapsedGameTime.TotalSeconds * MoveSpeed;
                        if (SubTileX < 0)
                        {
                            SubTileX = 0;
                            MovementDirection = Direction.None;
                        }
                        break;
                    case Direction.Down:
                        SubTileY += (float)gameTime.ElapsedGameTime.TotalSeconds * MoveSpeed;
                        if (SubTileY > 0)
                        {
                            SubTileY = 0;
                            MovementDirection = Direction.None;
                        }
                        break;
                    case Direction.Up:
                        SubTileY -= (float)gameTime.ElapsedGameTime.TotalSeconds * MoveSpeed;
                        if (SubTileY < 0)
                        {
                            SubTileY = 0;
                            MovementDirection = Direction.None;
                        }
                        break;
                }
            }
        }
    }
}
