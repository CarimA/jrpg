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
        public bool CanMove { get; set; } = true;
        public Direction MovementDirection { get; set; }

        public float MoveSpeed;

        public Vector2 GetPosition()
        {
            return new Vector2(
                (TileX * MainGame.TILE_SIZE) + (SubTileX * MainGame.TILE_SIZE) + (MainGame.TILE_SIZE / 2),
                (TileY * MainGame.TILE_SIZE) + (SubTileY * MainGame.TILE_SIZE) + (MainGame.TILE_SIZE / 2)
            );
        }

        public MovementFailureReason Move(Direction direction, float moveSpeed = 2.25f)
        {
            if (direction == Direction.None)
            {
                return MovementFailureReason.CurrentlyMoving;
            }
            // move somewhere, return if we can't move yet or if we collide with something
            if (!CanMove)
            {
                return MovementFailureReason.CurrentlyMoving;
            }

            CanMove = false;
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

            return MovementFailureReason.None;
        }
        
        public override void Update(GameTime gameTime)
        {
            if (CanMove)
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
                            CanMove = true;
                        }
                        break;
                    case Direction.Left:
                        SubTileX -= (float)gameTime.ElapsedGameTime.TotalSeconds * MoveSpeed;
                        if (SubTileX < 0)
                        {
                            SubTileX = 0;
                            MovementDirection = Direction.None;
                            CanMove = true;
                        }
                        break;
                    case Direction.Down:
                        SubTileY += (float)gameTime.ElapsedGameTime.TotalSeconds * MoveSpeed;
                        if (SubTileY > 0)
                        {
                            SubTileY = 0;
                            MovementDirection = Direction.None;
                            CanMove = true;
                        }
                        break;
                    case Direction.Up:
                        SubTileY -= (float)gameTime.ElapsedGameTime.TotalSeconds * MoveSpeed;
                        if (SubTileY < 0)
                        {
                            SubTileY = 0;
                            MovementDirection = Direction.None;
                            CanMove = true;
                        }
                        break;
                }
            }
        }
    }
}
