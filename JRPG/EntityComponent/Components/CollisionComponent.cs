using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Resolve;

namespace JRPG.EntityComponent.Components
{
    public class CollisionComponent : Component
    {
        public RectangleF LocalBounds;
        public Polygon CollisionBox;

        public CollisionComponent(RectangleF localBounds) //, int bezel = 0)
        {
            LocalBounds = localBounds;
            
            CollisionBox = new Polygon(new Vector2(0, 0), new List<Vector2>()
                {
                    new Vector2(localBounds.Left, localBounds.Top),
                    new Vector2(localBounds.Right, localBounds.Top),
                    new Vector2(localBounds.Right, localBounds.Bottom),
                    new Vector2(localBounds.Left, localBounds.Bottom)
                });
        }

        public override void Update(GameTime gameTime)
        {
            PositionComponent position = GetComponent<PositionComponent>();

            if (Game.MapManager == null || Game.MapManager.CurrentMap == null)
            {
                return;
            }

            List<Polygon> polys = Game.MapManager.CurrentMap.World.Retrieve(new RectangleF(position.Position.X + LocalBounds.X,
                position.Position.Y + LocalBounds.Y,
                LocalBounds.Width,
                LocalBounds.Height));

            CollisionBox.Position = position.Position;
            CollisionBox.Move(polys, position.Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds);
            position.Position = CollisionBox.Position;

        }


        public override void Receive(MessageType message, Entity entity, Component sender)
        {
            throw new NotImplementedException();
        }
    }
}
