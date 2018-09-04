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

        public Vector2 Position { get; set; }
        public Vector2 Velocity { get; set; }
        
        public Direction MovementDirection { get; set; }
        public Direction FacingDirection { get; set; }

        public float MoveSpeed;
        

        public void Set(float x, float y)
        {
            Position = new Vector2(x, y);
        }

        public void Move(Vector2 direction, float moveSpeed = 60f)
        {
            Velocity = direction * moveSpeed;

            if (Game.MapManager.Player == GetOwner && Velocity != Vector2.Zero)
            {
                if (Position.Y < 0)
                {
                    Game.MapManager.Move(Direction.Up);
                }
                if (Position.X < 0)
                {
                    Game.MapManager.Move(Direction.Left);
                }
                if (Position.Y >= Game.MapManager.CurrentMap.PixelHeight)
                {
                    Game.MapManager.Move(Direction.Down);
                }
                if (Position.X >= Game.MapManager.CurrentMap.PixelWidth)
                {
                    Game.MapManager.Move(Direction.Right);
                }
                Game.Window.Title = $"{Game.MapManager.CurrentMap.ID} - {{{Position}}}";
            }
        }
        
        public override void Update(GameTime gameTime)
        {

        }

        public override void Receive(MessageType message, Entity entity, Component sender)
        {
            throw new NotImplementedException();
        }
    }
}
