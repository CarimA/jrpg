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

        public override void Initialize()
        {
            Subscribe("move", Move);
            Subscribe("set-position", SetPosition);
        }

        public void Move(Entity entity, Component component)
        {
            if (component is PlayerComponent)
            {
                PlayerComponent player = (component as PlayerComponent);
                if (player.InControl)
                {
                    Vector2 direction = player.MovementDirection;
                    float speed = player.CurrentSpeed();

                    Velocity = (direction * speed);
                }
            }
        }

        public void SetPosition(Entity entity, Component component)
        {
            if (component is CollisionComponent)
            {
                CollisionComponent collision = (component as CollisionComponent);

                //
                Position = collision.CollisionBox.Position;
            }
        }

        public void CheckPosition()
        {
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

        public void Set(float x, float y)
        {
            Position = new Vector2(x, y);
        }
        
        public override void Update(GameTime gameTime)
        {
            Velocity *= (float)gameTime.ElapsedGameTime.TotalSeconds;
            CheckPosition();
            Send("move-collision");
        }
    }
}
