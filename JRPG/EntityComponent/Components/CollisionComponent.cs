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

            //if (bezel == 0)
            //{
            CollisionBox = new Polygon(new Vector2(0, 0), new List<Vector2>()
                {
                    new Vector2(localBounds.Left, localBounds.Top),
                    new Vector2(localBounds.Right, localBounds.Top),
                    new Vector2(localBounds.Right, localBounds.Bottom),
                    new Vector2(localBounds.Left, localBounds.Bottom)
                });
            /*}
            else
            {
                CollisionBox = new Polygon(new Vector2(0, 0), new List<Vector2>()
                {
                    new Vector2(localBounds.Left + bezel, localBounds.Top),
                    new Vector2(localBounds.Right - bezel, localBounds.Top),
                    new Vector2(localBounds.Right, localBounds.Top + bezel),
                    new Vector2(localBounds.Right, localBounds.Bottom - bezel),
                    new Vector2(localBounds.Right - bezel, localBounds.Bottom),
                    new Vector2(localBounds.Left + bezel, localBounds.Bottom),
                    new Vector2(localBounds.Left, localBounds.Bottom - bezel),
                    new Vector2(localBounds.Left, localBounds.Top + bezel)
                });

            }*/

            //ShapePrimitives.Rectangle(localBounds.X, localBounds.Y, localBounds.Width, localBounds.Height);
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
    }
}
